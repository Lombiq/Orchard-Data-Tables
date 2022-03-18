using Lombiq.DataTables.Constants;
using OrchardCore.Queries;

namespace Lombiq.DataTables.Models;

public class DataTableSortingContext
{
    public Query Query { get; set; }
    public SortingDirection Direction { get; set; }
    public DataTableColumnDefinition ColumnDefinition { get; set; }
}
