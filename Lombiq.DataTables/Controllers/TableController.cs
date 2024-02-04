using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Controllers;

[Admin]
public class TableController : Controller
{
    private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviders;

    public TableController(IEnumerable<IDataTableDataProvider> dataTableDataProviders) =>
        _dataTableDataProviders = dataTableDataProviders;

    [AdminRoute("DataTable/{providerName}/{queryId?}")]
    public async Task<IActionResult> Get(string providerName, string queryId = null, bool paging = true, bool viewAction = false)
    {
        var provider = _dataTableDataProviders.Single(provider => provider.Name == providerName);
        if (string.IsNullOrEmpty(queryId)) queryId = providerName;
        var definition = new DataTableDefinitionViewModel(JObject.FromObject(new { paging, viewAction }))
        {
            DataProvider = providerName,
            QueryId = queryId,
            ColumnsDefinition = await provider.GetColumnsDefinitionAsync(queryId),
        };

        return View(nameof(Get), new DataTableViewModel
        {
            Definition = definition,
            Provider = provider,
            BeforeTable = await provider.GetShapesBeforeTableAsync(),
            AfterTable = await provider.GetShapesAfterTableAsync(),
        });
    }
}
