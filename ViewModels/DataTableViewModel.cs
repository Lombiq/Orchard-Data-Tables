using System.Collections.Generic;
using Lombiq.DataTables.Services;
using OrchardCore.DisplayManagement;

namespace Lombiq.DataTables.ViewModels
{
    public class DataTableViewModel
    {
        public DataTableDefinitionViewModel Definition { get; set; }
        public IDataTableDataProvider Provider { get; set; }

        public IEnumerable<IShape> BeforeTable { get; set; }
        public IEnumerable<IShape> AfterTable { get; set; }
    }
}
