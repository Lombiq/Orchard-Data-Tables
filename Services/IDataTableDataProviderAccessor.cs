using Orchard;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableDataProviderAccessor : IDependency
    {
        IDataTableDataProvider GetDataProvider(string providerName);
    }
}