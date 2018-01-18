using Orchard;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableSortingProviderAccessor : IDependency
    {
        IDataTableSortingProvider GetSortingProvider(string dataSource);
    }
}