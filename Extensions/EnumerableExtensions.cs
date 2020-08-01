using Lombiq.DataTables.Services;
using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static IDataTableDataProvider GetDataProvider(this IEnumerable<IDataTableDataProvider> providers, string name) =>
            providers.FirstOrDefault(x => x.Name == name);

        public static IDataTableSortingProvider GetSortingProvider(this IEnumerable<IDataTableSortingProvider> providers, string dataSource) =>
            providers.FirstOrDefault(provider => provider.CanSort(dataSource));
    }
}
