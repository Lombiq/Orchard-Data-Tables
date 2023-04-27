using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Services
{
    public class IndexedDataTableDataProviderAccessor : IIndexedDataTableDataProviderAccessor
    {
        private readonly IEnumerable<IIndexedDataTableDataProvider> _indexedDataTableDataProviders;

        public IndexedDataTableDataProviderAccessor(IEnumerable<IIndexedDataTableDataProvider> indexedDataTableDataProviders)
        {
            _indexedDataTableDataProviders = indexedDataTableDataProviders;
        }

        public IIndexedDataTableDataProvider GetIndexedDataProvider(string providerName) =>
            _indexedDataTableDataProviders.FirstOrDefault(provider => provider.Name == providerName);
    }
}