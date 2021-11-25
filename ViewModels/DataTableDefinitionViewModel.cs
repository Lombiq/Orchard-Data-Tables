namespace Lombiq.DataTables.ViewModels
{
    public class DataTableDefinitionViewModel : DataTableDataViewModel
    {
        public int? PageSize { get; set; }
        public int? Skip { get; set; }
        public int QueryId { get; set; }
        public bool ProgressiveLoadingEnabled { get; set; }
        public string QueryStringParametersLocalStorageKey { get; set; }
        public string DataProvider { get; set; }
        public string DataTableCssClasses { get; set; }
        public string HideCheckboxIfColumnIsEmpty { get; set; }
    }
}