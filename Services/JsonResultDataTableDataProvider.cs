using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.DisplayManagement;

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
        public abstract LocalizedString Description { get; }


        public async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            var columnsDefinition = GetColumnsDefinition(request.QueryId);
            var columns = columnsDefinition.Columns
                .Select(x => new { Path = x.Name.Replace('_', '.'), x.Name, x.Regex })
                .ToList();
            var order = request.Order.FirstOrDefault() ?? new DataTableOrder
            {
                Column = columnsDefinition.DefaultSortingColumnName,
                Direction = columnsDefinition.DefaultSortingDirection
            };

            var results = await GetResultsAsync(request);
            var json = results[0] is JObject ? results.Cast<JObject>() : results.Select(JObject.FromObject);
            if (!string.IsNullOrEmpty(order.Column))
            {
                var orderColumnName = order.Column.Replace('_', '.');
                JToken Selector(JObject x) => x.SelectToken(orderColumnName)?.ToString();
                json = order.IsAscending ? json.OrderBy(Selector) : json.OrderByDescending(Selector);
            }

            if (request.Start > 0) json = json.Skip(request.Start);
            json = json.Take(request.Length);

            var rows = json.Select((result, index) =>
                new DataTableRow(index, columns
                    .Select(column => new { column.Name, column.Regex, Token = result.SelectToken(column.Path, true) })
                    .ToDictionary(
                        cell => cell.Name,
                        cell => cell.Regex is {} regex
                            ? new JValue(Regex.Replace(cell.Token.ToString(), regex.From, regex.To))
                            : cell.Token)));

            return new DataTableDataResponse
            {
                Data = rows, RecordsFiltered = results.Count, RecordsTotal = results.Count
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
        protected abstract Task<IList<object>> GetResultsAsync(DataTableDataRequest request);

        /// <summary>
        /// When overridden in a derived class it gets the columns definition.
        /// </summary>
        /// <param name="queryId">May be used to dynamically generate the result.</param>
        /// <returns>The default columns definition of this provider.</returns>
        protected abstract DataTableColumnsDefinition GetColumnsDefinition(string queryId);
    }
}
