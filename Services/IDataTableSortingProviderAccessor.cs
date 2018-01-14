using Orchard;
using System.Linq;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableSortingProviderAccessor : IDependency
    {
        IDataTableSortingProvider GetSortingProvider(string dataSource);
    }
}