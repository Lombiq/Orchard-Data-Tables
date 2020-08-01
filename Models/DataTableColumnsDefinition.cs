using Lombiq.DataTables.Constants;
using System.Collections.Generic;
using System.Linq;
using OrchardCore.ContentManagement;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnsDefinition : ContentPart
    {
        public IEnumerable<DataTableColumnDefinition> Columns { get; set; } = Enumerable.Empty<DataTableColumnDefinition>();

        public string DefaultSortingColumnName { get; set; } = "";
        public SortingDirection DefaultSortingDirection { get; set; } = SortingDirection.Ascending;
    }
}
