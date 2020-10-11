using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    /// <summary>
    /// Classes which implement this class only have to provide the provider description, the dataset via
    /// <see cref="GetResultsAsync"/> as <see cref="IList{T}"/> of either <see cref="object"/> or <see cref="JObject"/>
    /// (the former is automatically converted to the latter) and the columns definition via
    /// <see cref="GetColumnsDefinitionInner"/>.
    /// </summary>
    public abstract class JsonResultDataTableDataProvider : IDataTableDataProvider
    {
        private readonly IStringLocalizer T;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly PlainTextEncoder _plainTextEncoder;

        protected readonly LinkGenerator _linkGenerator;
        protected readonly IHttpContextAccessor _hca;

        public abstract LocalizedString Description { get; }
        public abstract IEnumerable<Permission> SupportedPermissions { get; }


        protected JsonResultDataTableDataProvider(
            IDataTableDataProviderServices services)
        {
            T = services.StringLocalizer;
            _liquidTemplateManager = services.LiquidTemplateManager;
            _linkGenerator = services.LinkGenerator;
            _hca = services.HttpContextAccessor;

            _plainTextEncoder = new PlainTextEncoder();
        }

        public async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            var columnsDefinition = GetColumnsDefinitionInner(request.QueryId);
            var columns = columnsDefinition.Columns
                .Select(column =>
                    new ColumnModel
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

            var enumerableResults = await GetResultsAsync(request);
            var results = enumerableResults is IList<object> listResults ? listResults : enumerableResults.ToList();
            if (results.Count == 0) return DataTableDataResponse.Empty();
            var recordsFiltered = results.Count;
            var recordsTotal = results.Count;

            var json = results[0] is JObject ? results.Cast<JObject>() : results.Select(JObject.FromObject);
            if (!string.IsNullOrEmpty(order.Column)) json = OrderByColumn(json, order);

            var rows = json.Select((result, index) =>
                new DataTableRow(index, columns
                    .Select(column => new { column.Name, column.Regex, Token = result.SelectToken(column.Path, false) })
                    .ToDictionary(
                        cell => cell.Name,
                        cell => cell.Regex is { } regex
                            ? new JValue(Regex.Replace(cell.Token?.ToString() ?? string.Empty, regex.From, regex.To))
                            : cell.Token)));

            if (request.Search?.IsRegex == true)
            {
                return DataTableDataResponse.ErrorResult(T["Regex search is not supported at this time."]);
            }

            var searchValue = request.Search?.Value;
            var hasSearch = !string.IsNullOrWhiteSpace(searchValue);
            var columnFilters = request.GetColumnSearches();
            if (hasSearch || columnFilters?.Count > 0)
            {
                (rows, recordsFiltered) = Search(rows, columns, hasSearch, searchValue, columnFilters);
            }

            if (request.Start > 0) rows = rows.Skip(request.Start);
            if (request.Length > 0) rows = rows.Take(request.Length);
            var rowList = rows.ToList();

            var liquidColumns = columns.Where(column => column.IsLiquid).Select(column => column.Name).ToList();
            if (liquidColumns.Count > 0) await RenderLiquidAsync(rowList, liquidColumns);

            return new DataTableDataResponse
            {
                Data = rowList,
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal,
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
                            scope => { });
                    }
                }
            }
        }

        private static (IEnumerable<DataTableRow> Results, int Count) Search(
                IEnumerable<DataTableRow> rows,
                IEnumerable<ColumnModel> columns,
                bool hasSearch,
                string searchValue,
                IReadOnlyCollection<DataTableColumn> columnFilters)
        {
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
                            token?.ToString().Contains(word, StringComparison.InvariantCultureIgnoreCase) == true)));
            }

            var list = filteredRows.ToList();
            return (list, list.Count);
        }

        public Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) =>
            Task.FromResult(GetColumnsDefinitionInner(queryId));

        public Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId) =>
            Task.FromResult(new DataTableChildRowResponse());

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
        protected abstract Task<IEnumerable<object>> GetResultsAsync(DataTableDataRequest request);

        /// <summary>
        /// When overridden in a derived class it gets the columns definition.
        /// </summary>
        /// <param name="queryId">May be used to dynamically generate the result.</param>
        /// <returns>The default columns definition of this provider.</returns>
        protected abstract DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId);


        protected string GetActionsColumn()
        {
            if (_hca?.HttpContext == null)
            {
                // The httpContext would be required to generate the returnUrl parameter.
                return "ContentItemId||^.*$||{{ '$0' | actions }}";
            }

            var returnUrl = _linkGenerator.GetPathByAction(
                _hca?.HttpContext,
                nameof(TableController.Get),
                typeof(TableController).ControllerName(),
                new { providerName = GetType().Name });
            return "ContentItemId||^.*$||{{ '$0' | actions: returnUrl: '" + returnUrl + "' }}";
        }


        private IEnumerable<JObject> OrderByColumn(IEnumerable<JObject> json, DataTableOrder order)
        {
            // Known issue: https://github.com/SonarSource/sonar-dotnet/issues/3126
#pragma warning disable S1854 // Unused assignments should be removed
            var orderColumnName = order.Column.Replace('_', '.');
#pragma warning restore S1854 // Unused assignments should be removed

            JToken Selector(JObject x)
            {
                var jToken = x.SelectToken(orderColumnName);

                if (jToken is JObject jObject && jObject.ContainsKey(nameof(ExportLink.Text)))
                {
                    jToken = jObject[nameof(ExportLink.Text)];
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


        private class ColumnModel
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public (string From, string To)? Regex { get; set; }
            public bool Searchable { get; set; }
            public bool IsLiquid { get; set; }
        }
    }
}
