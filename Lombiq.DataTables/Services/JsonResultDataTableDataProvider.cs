using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services;

/// <summary>
/// Classes which implement this class only have to provide the provider description, the dataset via <see
/// cref="GetResultsAsync"/> as <see cref="IList{T}"/> of either <see cref="object"/> or <see cref="JObject"/> (the
/// former is automatically converted to the latter).
/// </summary>
public abstract class JsonResultDataTableDataProvider : DataTableDataProviderBase
{
    private readonly IStringLocalizer T;

    protected JsonResultDataTableDataProvider(
        IDataTableDataProviderServices services,
        IStringLocalizer implementationStringLocalizer)
        : base(services) =>
        T = implementationStringLocalizer;

    public override async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
    {
        var columnsDefinition = GetColumnsDefinitionInner(request.QueryId);
        var columns = columnsDefinition.Columns.ToList();
        var order = request.Order.FirstOrDefault() ?? new DataTableOrder
        {
            Column = columnsDefinition.DefaultSortingColumnName,
            Direction = columnsDefinition.DefaultSortingDirection,
        };

        var metaData = await GetResultsAsync(request);
        var results = metaData.Results.AsList();
        if (results.Count == 0) return DataTableDataResponse.Empty();
        var recordsFiltered = metaData.Count >= 0 && !metaData.IsFiltered ? metaData.Count : results.Count;
        var recordsTotal = results.Count;

        if (metaData.CountFiltered >= 0) recordsFiltered = metaData.CountFiltered;

        var json = results[0] is JsonObject
            ? results.Cast<JsonObject>()
            : results.Select(result => JsonSerializer.SerializeToNode(result)?.AsObject());
        if (!string.IsNullOrEmpty(order.Column)) json = OrderByColumn(json, order);

        if (request.Search?.IsRegex == true)
        {
            return DataTableDataResponse.ErrorResult(T["Regex search is not supported at this time."]);
        }

        var rows = SubstituteByColumn(json, columns);

        if (!metaData.IsFiltered || !metaData.IsPaginated)
        {
            (rows, recordsFiltered) = FilterAndPaginate(request, metaData, rows, columns, recordsFiltered);
        }

        var rowList = rows.ToList();

        var liquidColumns = columns.Where(column => column.IsLiquid).Select(column => column.Name).ToList();
        if (liquidColumns.Count > 0) await RenderLiquidAsync(rowList, liquidColumns);

        return new DataTableDataResponse
        {
            Data = rowList,
            RecordsFiltered = recordsFiltered,
            RecordsTotal = (metaData.IsPaginated || metaData.IsFiltered) && metaData.Count >= 0 ? metaData.Count : recordsTotal,
        };
    }

    private static (IEnumerable<DataTableRow> Rows, int RecordsFiltered) FilterAndPaginate(
        DataTableDataRequest request,
        JsonResultDataTableDataProviderResult meta,
        IEnumerable<DataTableRow> rows,
        List<DataTableColumnDefinition> columns,
        int recordsFiltered)
    {
        var searchValue = request.Search?.Value;
        var columnFilters = request.GetColumnSearches();
        int recordsFilteredResponse = recordsFiltered;

        if (!meta.IsFiltered && (request.HasSearch || columnFilters?.Count > 0))
        {
            (rows, recordsFilteredResponse) = Search(rows, columns, request.HasSearch, searchValue, columnFilters);
        }

        if (!meta.IsPaginated)
        {
            if (request.Start > 0) rows = rows.Skip(request.Start);
            if (request.Length > 0) rows = rows.Take(request.Length);
        }

        return (rows, recordsFilteredResponse);
    }

    private static (IEnumerable<DataTableRow> Results, int Count) Search(
            IEnumerable<DataTableRow> rows,
            IEnumerable<DataTableColumnDefinition> columns,
            bool hasSearch,
            string searchValue,
            IReadOnlyCollection<DataTableColumn> columnFilters)
    {
        static string PrepareToken(JsonNode node) =>
            node switch
            {
                JsonObject link when ExportLink.IsInstance(link) => ExportLink.GetText(link),
                JsonObject date when ExportDate.IsInstance(date) => ExportDate.GetText(date),
                { } => node.ToString(),
                null => null,
            };

        var filteredRows = rows;

        if (columnFilters?.Count > 0)
        {
            filteredRows = filteredRows.Where(row =>
                columnFilters.All(filter =>
                    row.ValuesDictionary.TryGetValue(filter.Name, out var token) &&
                    token?.ToString().Contains(filter.Search.Value, StringComparison.InvariantCulture) == true));
        }

        if (hasSearch)
        {
            var words = searchValue
                .Split()
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .ToList();
            filteredRows = filteredRows.Where(row =>
                words.TrueForAll(word =>
                    columns.Any(filter =>
                        filter.Searchable &&
                        row.ValuesDictionary.TryGetValue(filter.Name, out var token) &&
                        PrepareToken(token)?.Contains(word, StringComparison.InvariantCultureIgnoreCase) == true)));
        }

        var list = filteredRows.ToList();
        return (list, list.Count);
    }

    /// <summary>
    /// When overridden in a derived class it gets the content which is then turned into <see cref="JsonNode"/> if
    /// necessary and then queried down using the column names into a dictionary.
    /// </summary>
    /// <param name="request">The input of <see cref="GetRowsAsync"/>.</param>
    /// <returns>A list of results or <see cref="JObject"/>s.</returns>
    protected abstract Task<JsonResultDataTableDataProviderResult> GetResultsAsync(DataTableDataRequest request);

    private IEnumerable<JsonObject> OrderByColumn(IEnumerable<JsonObject> json, DataTableOrder order)
    {
        var orderColumnName = order.Column.Replace('_', '.');

        JsonNode Selector(JsonObject item)
        {
            var node = item.SelectNode(orderColumnName);

            if (node is JsonObject jObject)
            {
                if (jObject.TryGetPropertyValue(nameof(ExportLink.Text), out var textValue))
                {
                    node = textValue;
                }
                else if (ExportDate.IsInstance(jObject))
                {
                    node = (DateTime)node.ToObject<ExportDate>();
                }
            }

            return node switch
            {
                null => null,
                JsonValue value when value.GetValueKind() == JsonValueKind.String => value.Value<string>(),
                _ => node.ToString().ToUpperInvariant(),
            };
        }

        return order.IsAscending ? json.OrderBy(Selector) : json.OrderByDescending(Selector);
    }
}
