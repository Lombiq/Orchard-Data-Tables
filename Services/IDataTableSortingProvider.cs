using Lombiq.DataTables.Models;
using Orchard;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableSortingProvider
    {
        bool CanSort(string dataSource);

        void Sort(DataTableSortingContext context);
    }
}