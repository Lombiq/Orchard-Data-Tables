using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Lombiq.DataTables.Tests.Helpers
{
    public static class MockDataProviderHelper
    {
        public static (IDataTableDataProvider Provider, DataTableDataRequest Request) GetProviderAndRequest(
            object[][] dataSet,
            (string Name, string Text, bool Exportable)[] columns,
            int start,
            int length,
            int orderColumnIndex,
            IMemoryCache memoryCache,
            IServiceProvider shellContextServiceProvider = null)
        {
            // For when Liquid is not needed.
            shellContextServiceProvider ??= new ServiceCollection().BuildServiceProvider();

            var provider = new MockDataProvider(dataSet, memoryCache, shellContextServiceProvider);
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
