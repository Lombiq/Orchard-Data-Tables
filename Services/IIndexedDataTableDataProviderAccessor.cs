using Orchard;

namespace Lombiq.DataTables.Services
{
    public interface IIndexedDataTableDataProviderAccessor : IDependency
    {
        IIndexedDataTableDataProvider GetIndexedDataProvider(string providerName);
    }
}