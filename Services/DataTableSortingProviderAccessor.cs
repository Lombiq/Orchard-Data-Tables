using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Services
{
    public class DataTableSortingProviderAccessor : IDataTableSortingProviderAccessor
    {
        private readonly IEnumerable<IDataTableSortingProvider> _providers;


        public DataTableSortingProviderAccessor(IEnumerable<IDataTableSortingProvider> providers)
        {
            _providers = providers;
        }


        public IDataTableSortingProvider GetSortingProvider(string dataSource) =>
            _providers.FirstOrDefault(provider => provider.CanSort(dataSource));
    }
}