using Lombiq.DataTables.Models;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableSortingProvider
    {
        bool CanSort(string dataSource);

        void Sort(DataTableSortingContext context);
    }
}