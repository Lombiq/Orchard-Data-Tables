using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;

namespace Lombiq.DataTables.Tests
{
    public class MockDataProvider : IDataTableDataProvider
    {
        private readonly object[][] _dataSet;
        private readonly DataTableColumnsDefinition _definition;
        public LocalizedString Description { get; } = new LocalizedString("Test", "Test");


        public MockDataProvider(object[][] dataSet, DataTableColumnsDefinition definition)
        {
            _dataSet = dataSet;
            _definition = definition;
        }


        public async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            // We aren't trying to test any fancy sorting, just point at which column to sort by. (asc)
            var orderByColumn = request.Order.FirstOrDefault()?.Column is {} column ? int.Parse(column) : 0;

            var columns = (await GetColumnsDefinitionAsync(null))
                .Columns
                .Select((item, index) => new { item.Name, Index = index });

            var data = _dataSet
                .OrderBy(row => row[orderByColumn])
                .Skip(request.Start)
                .Take(request.Length)
                .Select((row, index) => new DataTableRow(index, columns
                    .ToDictionary(column => column.Name, column => JToken.FromObject(row[column.Index]))));

            return new DataTableDataResponse
            {
                Data = data,
                RecordsFiltered = _dataSet.Length,
                RecordsTotal = _dataSet.Length,
            };
        }

        public Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) =>
            Task.FromResult(_definition);

        public Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId) =>
            Task.FromCanceled<DataTableChildRowResponse>(CancellationToken.None);
    }
}
