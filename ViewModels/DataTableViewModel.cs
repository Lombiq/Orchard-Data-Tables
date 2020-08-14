using Lombiq.DataTables.Services;
using System.Collections.Generic;

namespace Lombiq.DataTables.ViewModels
{
    public class DataTableViewModel
    {
        public DataTableDefinitionViewModel Definition { get; set; }
        public IDataTableDataProvider Provider { get; set; }

        public IEnumerable<dynamic> BeforeTable { get; set; }
        public IEnumerable<dynamic> AfterTable { get; set; }
    }
}
