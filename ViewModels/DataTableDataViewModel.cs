using Lombiq.DataTables.Models;
using System.Collections.Generic;

namespace Lombiq.DataTables.ViewModels
{
    public class DataTableDataViewModel
    {
        public string DataTableId { get; set; }
        public DataTableColumnsDefinition ColumnsDefinition { get; set; }
        public IEnumerable<IEnumerable<string>> Rows { get; set; }
        public bool ChildRowsEnabled { get; set; }
    }
}