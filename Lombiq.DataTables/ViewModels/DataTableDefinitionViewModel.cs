using System.Text.Json.Nodes;

namespace Lombiq.DataTables.ViewModels;

public class DataTableDefinitionViewModel : DataTableDataViewModel
{
    public int? PageSize { get; set; }
    public int? Skip { get; set; }
    public string QueryId { get; set; }
    public bool ProgressiveLoadingEnabled { get; set; }
    public string QueryStringParametersLocalStorageKey { get; set; }
    public string DataProvider { get; set; }
    public string DataTableCssClasses { get; set; }
    public JsonObject AdditionalDatatableOptions { get; }

    public DataTableDefinitionViewModel(JsonObject additionalDatatableOptions = null)
    {
        if (additionalDatatableOptions != null) AdditionalDatatableOptions = additionalDatatableOptions;
    }
}
