using Lombiq.DataTables.Constants;
using Orchard.ContentManagement;

namespace Lombiq.DataTables.Models
{
    public class DataTableSortingContext
    {
        public IHqlQuery Query { get; set; }
        public SortingDirection Direction { get; set; }
        public DataTableColumnDefinition ColumnDefinition { get; set; }
    }
}