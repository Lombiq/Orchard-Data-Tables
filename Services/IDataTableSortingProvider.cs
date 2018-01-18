using Lombiq.DataTables.Models;
using Orchard;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableSortingProvider : IDependency
    {
        bool CanSort(string dataSource);

        void Sort(DataTableSortingContext context);
    }
}