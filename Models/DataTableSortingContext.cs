using Lombiq.DataTables.Constants;
using Orchard.Projections.Descriptors.SortCriterion;

namespace Lombiq.DataTables.Models
{
    public class DataTableSortingContext
    {
        public SortCriterionContext SortCriterionContext { get; set; }
        public SortingDirection Direction { get; set; }
        public DataTableColumnDefinition ColumnDefinition { get; set; }
    }
}