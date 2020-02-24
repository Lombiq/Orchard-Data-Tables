using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnsDefinition
    {
        public IEnumerable<DataTableColumnDefinition> Columns { get; set; } = Enumerable.Empty<DataTableColumnDefinition>();

        public string DefaultSortingColumnName { get; set; } = "";
    }
}