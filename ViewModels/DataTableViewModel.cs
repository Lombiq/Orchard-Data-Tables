using Lombiq.DataTables.Services;

namespace Lombiq.DataTables.ViewModels
{
    public class DataTableViewModel
    {
        public DataTableDefinitionViewModel Definition { get; set; }
        public IDataTableDataProvider Provider { get; set; }
    }
}
