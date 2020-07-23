namespace Lombiq.DataTables.Services
{
    public interface IDataTableSortingProviderAccessor
    {
        IDataTableSortingProvider GetSortingProvider(string dataSource);
    }
}