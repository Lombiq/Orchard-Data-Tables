using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Shouldly;
using System.Linq;

namespace Lombiq.DataTables.Tests.UnitTests.Services
{
    public class MockDataProviderTestsBase
    {
        protected (IDataTableDataProvider, DataTableDataRequest) GetProviderAndRequest(
            string note,
            object[][] dataSet,
            (string Name, string Text, bool Exportable)[] columns,
            int start,
            int length,
            int orderColumnIndex)
        {
            note.ShouldNotBeEmpty("Please state the purpose of this input set!");

            var provider = (IDataTableDataProvider)new MockDataProvider(dataSet,
                new DataTableColumnsDefinition()
                {
                    Columns = columns
                        .Select(column => new DataTableColumnDefinition
                        {
                            Name = column.Name, Text = column.Text, Exportable = column.Exportable
                        })
                        .ToList()
                });
            var request = new DataTableDataRequest
            {
                DataProvider = provider.Name,
                Length = length,
                Start = start,
                Order = new[] { new DataTableOrder { Column = columns[orderColumnIndex].Name } }
            };

            return (provider, request);
        }
    }
}
