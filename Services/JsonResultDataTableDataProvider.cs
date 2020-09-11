using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
    /// <see cref="GetColumnsDefinition"/>.
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
            IStringLocalizer stringLocalizer,
            ILiquidTemplateManager liquidTemplateManager,
            LinkGenerator linkGenerator,
            IHttpContextAccessor hca)
        {
            T = stringLocalizer;
            _liquidTemplateManager = liquidTemplateManager;
            _linkGenerator = linkGenerator;
            _hca = hca;

            _plainTextEncoder = new PlainTextEncoder();
        }


        public async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            var columnsDefinition = GetColumnsDefinition(request.QueryId);
            var columns = columnsDefinition.Columns
                .Select(column =>
                    new
                    {
                        Path = column.Name.Replace('_', '.'),
                        column.Name,
                        column.Regex,
                        column.Searchable,
                        column.IsLiquid,
                    })
                .ToList();
            var order = request.Order.FirstOrDefault() ?? new DataTableOrder
            {
                Column = columnsDefinition.DefaultSortingColumnName,
                Direction = columnsDefinition.DefaultSortingDirection
            };

            var enumerableResults = await GetResultsAsync(request);
            var results = enumerableResults is IList<object> listResults ? listResults : enumerableResults.ToList();
            if (results.Count == 0) return DataTableDataResponse.Empty();
            var recordsFiltered = results.Count;
            var recordsTotal = results.Count;

            var json = results[0] is JObject ? results.Cast<JObject>() : results.Select(JObject.FromObject);
            if (!string.IsNullOrEmpty(order.Column))
            {
                var orderColumnName = order.Column.Replace('_', '.');
                JToken Selector(JObject x) => x.SelectToken(orderColumnName)?.ToString();
                json = order.IsAscending ? json.OrderBy(Selector) : json.OrderByDescending(Selector);
            }

            var rows = json.Select((result, index) =>
                new DataTableRow(index, columns
                    .Select(column => new { column.Name, column.Regex, Token = result.SelectToken(column.Path, false) })
                    .ToDictionary(
                        cell => cell.Name,
                        cell => cell.Regex is {} regex
                            ? new JValue(Regex.Replace(cell.Token?.ToString() ?? string.Empty, regex.From, regex.To))
                            : cell.Token)));

            var searchValue = request.Search?.Value;
            var hasSearch = !string.IsNullOrWhiteSpace(searchValue);
            var columnFilters = request.GetColumnSearches();
            if (hasSearch || columnFilters?.Count > 0)
            {
                if (request.Search?.IsRegex == true)
                {
                    return DataTableDataResponse.ErrorResult(T["Regex search is not supported at this time."]);
                }

                var filteredRows = rows;

                if (columnFilters?.Count > 0)
                {
                    filteredRows = filteredRows.Where(row =>
                        columnFilters.All(filter =>
                            row.ValuesDictionary.TryGetValue(filter.Name, out var token) &&
                            token?.ToString().Contains(filter.Search.Value) == true));
                }

                if (hasSearch)
                {
                    var words = searchValue
                        .Split()
                        .Where(word => !string.IsNullOrWhiteSpace(word))
                        .Select(word => word.ToLower())
                        .ToList();
                    filteredRows = filteredRows.Where(row =>
                            words.All(word =>
                                columns.Any(filter =>
                                    filter.Searchable &&
                                    row.ValuesDictionary.TryGetValue(filter.Name, out var token) &&
                                    token?.ToString().ToLower().Contains(word) == true)));
                }

                var list = filteredRows.ToList();
                rows = list;
                recordsFiltered = list.Count;
            }

            if (request.Start > 0) rows = rows.Skip(request.Start);
            if (request.Length > 0) rows = rows.Take(request.Length);
            var rowList = rows.ToList();

            var liquidColumns = columns.Where(column => column.IsLiquid).Select(column => column.Name).ToList();
            if (liquidColumns.Count > 0)
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

            return new DataTableDataResponse
            {
                Data = rowList, RecordsFiltered = recordsFiltered, RecordsTotal = recordsTotal
            };
        }

        public Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) =>
            Task.FromResult(GetColumnsDefinition(queryId));

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
        protected abstract DataTableColumnsDefinition GetColumnsDefinition(string queryId);


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
    }
}
