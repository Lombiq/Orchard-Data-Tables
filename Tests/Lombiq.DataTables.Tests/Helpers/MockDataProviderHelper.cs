using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using System;
using System.Linq;

namespace Lombiq.DataTables.Tests.Helpers
{
    public static class MockDataProviderHelper
    {
        public static (IDataTableDataProvider Provider, DataTableDataRequest Request) GetProviderAndRequest(
            string note,
            object[][] dataSet,
            (string Name, string Text, bool Exportable)[] columns,
            int start,
            int length,
            int orderColumnIndex,
            IMemoryCache memoryCache)
        {
            note.ShouldNotBeEmpty("Please state the purpose of this input set!");

            var provider = new MockDataProvider(dataSet, memoryCache);
            provider.Definition = provider.DefineColumns(
                columns.Select(column => (column.Name, column.Text)).ToArray());

            provider.Definition.Columns = provider.Definition.Columns
                .Select((columnDefinition, index) =>
                {
                    if (!columns[index].Exportable) columnDefinition.Exportable = false;
                    return columnDefinition;
                });

            var order = columns[orderColumnIndex].Name.Split(new[] { "||" }, StringSplitOptions.None)[0];
            var request = new DataTableDataRequest
            {
                DataProvider = nameof(MockDataProvider),
                Length = length,
                Start = start,
                Order = new[] { new DataTableOrder { Column = order } },
            };

            return (provider, request);
        }
    }
}
