using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Services
{
    public class DataTableDataProviderAccessor : IDataTableDataProviderAccessor
    {
        private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviders;


        public DataTableDataProviderAccessor(IEnumerable<IDataTableDataProvider> dataTableDataProviders)
        {
            _dataTableDataProviders = dataTableDataProviders;
        }


        public IDataTableDataProvider GetDataProvider(string providerName) =>
            _dataTableDataProviders.FirstOrDefault(provider => provider.Name == providerName);
    }
}