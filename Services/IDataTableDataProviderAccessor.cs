namespace Lombiq.DataTables.Services
{
    public interface IDataTableDataProviderAccessor
    {
        IDataTableDataProvider GetDataProvider(string providerName);
    }
}