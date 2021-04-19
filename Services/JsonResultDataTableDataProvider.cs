using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    /// <summary>
    /// Classes which implement this class only have to provide the provider description, the dataset via <see
    /// cref="GetResultsAsync"/> as <see cref="IList{T}"/> of either <see cref="object"/> or <see cref="JObject"/> (the
    /// former is automatically converted to the latter).
    /// </summary>
    public abstract class JsonResultDataTableDataProvider : DataTableDataProviderBase
    {
        private readonly IStringLocalizer T;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly PlainTextEncoder _plainTextEncoder;

        public abstract IEnumerable<Permission> SupportedPermissions { get; }

        protected JsonResultDataTableDataProvider(
            IDataTableDataProviderServices services,
            IStringLocalizer implementationStringLocalizer)
            : base(services)
        {
            T = implementationStringLocalizer;
            _liquidTemplateManager = services?.LiquidTemplateManager;

            _plainTextEncoder = new PlainTextEncoder();
        }

        public override async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            var columnsDefinition = GetColumnsDefinitionInner(request.QueryId);
            var columns = columnsDefinition.Columns
                .Select(column =>
                    new JsonResultColumn
                    {
                        Path = column.Name.Replace('_', '.'),
                        Name = column.Name,
                        Regex = column.Regex,
                        Searchable = column.Searchable,
                        IsLiquid = column.IsLiquid,
                    })
                .ToList();
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

            var json = results[0] is JObject ? results.Cast<JObject>() : results.Select(JObject.FromObject);
            if (!string.IsNullOrEmpty(order.Column)) json = OrderByColumn(json, order);

            var rows = json.Select((result, index) =>
                new DataTableRow(index, columns
                    .Select(column => (column.Name, column.Regex, Token: result.SelectToken(column.Path, false)))
                    .ToDictionary(
                        cell => cell.Name,
                        cell => cell.Regex is { } regex
                            ? new JValue(Regex.Replace(cell.Token?.ToString() ?? string.Empty, regex.From, regex.To))
                            : cell.Token)));

            if (request.Search?.IsRegex == true)
            {
                return DataTableDataResponse.ErrorResult(T["Regex search is not supported at this time."]);
            }

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

        private async Task RenderLiquidAsync(IEnumerable<DataTableRow> rowList, IList<string> liquidColumns)
        {
            foreach (var row in rowList)
            {
                foreach (var liquidColumn in liquidColumns)
                {
                    if (row.ValuesDictionary.TryGetValue(liquidColumn, out var token) &&
                        token?.ToString() is { } template)
                    {
                        row[liquidColumn] = await _liquidTemplateManager.RenderAsync(
                            template,
                            _plainTextEncoder,
                            row,
                            _ => { });
                    }
                }
            }
        }

        private static (IEnumerable<DataTableRow> Rows, int RecordsFiltered) FilterAndPaginate(
            DataTableDataRequest request,
            JsonResultDataTableDataProviderResult meta,
            IEnumerable<DataTableRow> rows,
            List<JsonResultColumn> columns,
            int recordsFiltered)
        {
            var searchValue = request.Search?.Value;
            var columnFilters = request.GetColumnSearches();

            if (!meta.IsFiltered && (request.HasSearch || columnFilters?.Count > 0))
            {
                (rows, recordsFiltered) = Search(rows, columns, request.HasSearch, searchValue, columnFilters);
            }

            if (!meta.IsPaginated)
            {
                if (request.Start > 0) rows = rows.Skip(request.Start);
                if (request.Length > 0) rows = rows.Take(request.Length);
            }

            return (rows, recordsFiltered);
        }

        private static (IEnumerable<DataTableRow> Results, int Count) Search(
                IEnumerable<DataTableRow> rows,
                IEnumerable<JsonResultColumn> columns,
                bool hasSearch,
                string searchValue,
                IReadOnlyCollection<DataTableColumn> columnFilters)
        {
            static string PrepareToken(JToken token) =>
                token switch
                {
                    JObject link when ExportLink.IsInstance(link) => ExportLink.GetText(link),
                    JObject date when ExportDate.IsInstance(date) => ExportDate.GetText(date),
                    { } => token.ToString(),
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
                    words.All(word =>
                        columns.Any(filter =>
                            filter.Searchable &&
                            row.ValuesDictionary.TryGetValue(filter.Name, out var token) &&
                            PrepareToken(token)?.Contains(word, StringComparison.InvariantCultureIgnoreCase) == true)));
            }

            var list = filteredRows.ToList();
            return (list, list.Count);
        }

        public override Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) =>
            Task.FromResult(GetColumnsDefinitionInner(queryId));

        public virtual Task<IEnumerable<dynamic>> GetShapesBeforeTableAsync() =>
            Task.FromResult<IEnumerable<dynamic>>(Array.Empty<IShape>());

        public virtual Task<IEnumerable<dynamic>> GetShapesAfterTableAsync() =>
            Task.FromResult<IEnumerable<dynamic>>(Array.Empty<IShape>());

        /// <summary>
        /// When overridden in a derived class it gets the content which is then turned into <see cref="JToken"/> if
        /// necessary and then queried down using the column names into a dictionary.
        /// </summary>
        /// <param name="request">The input of <see cref="GetRowsAsync"/>.</param>
        /// <returns>A list of results or <see cref="JObject"/>s.</returns>
        protected abstract Task<JsonResultDataTableDataProviderResult> GetResultsAsync(DataTableDataRequest request);

        private IEnumerable<JObject> OrderByColumn(IEnumerable<JObject> json, DataTableOrder order)
        {
            // Known issue: https://github.com/SonarSource/sonar-dotnet/issues/3126
#pragma warning disable S1854 // Unused assignments should be removed
            var orderColumnName = order.Column.Replace('_', '.');
#pragma warning restore S1854 // Unused assignments should be removed

            JToken Selector(JObject x)
            {
                var jToken = x.SelectToken(orderColumnName);

                if (jToken is JObject jObject)
                {
                    if (jObject.ContainsKey(nameof(ExportLink.Text)))
                    {
                        jToken = jObject[nameof(ExportLink.Text)];
                    }
                    else if (ExportDate.IsInstance(jObject))
                    {
                        jToken = (DateTime)jToken.ToObject<ExportDate>();
                    }
                }

                return jToken switch
                {
                    null => null,
                    JValue jValue when jValue.Type != JTokenType.String => jValue,
                    _ => jToken.ToString().ToUpperInvariant(),
                };
            }

            return order.IsAscending ? json.OrderBy(Selector) : json.OrderByDescending(Selector);
        }

        [DebuggerDisplay("{ToString()}")]
        private class JsonResultColumn
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public (string From, string To)? Regex { get; set; }
            public bool Searchable { get; set; }
            public bool IsLiquid { get; set; }

            public override string ToString() => Name;
        }
    }
}
