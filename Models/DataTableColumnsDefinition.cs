using System;
using Lombiq.DataTables.Constants;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnsDefinition
    {
        public IList<DataTableColumnDefinition> Columns { get; set; } = Array.Empty<DataTableColumnDefinition>();
        public string DefaultSortingColumnName { get; set; } = string.Empty;
        public SortingDirection DefaultSortingDirection { get; set; } = SortingDirection.Ascending;
    }
}
