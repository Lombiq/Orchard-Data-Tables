using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Controllers;

public class TableController : Controller
{
    private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviders;

    public TableController(IEnumerable<IDataTableDataProvider> dataTableDataProviders) =>
        _dataTableDataProviders = dataTableDataProviders;

    [Admin("DataTable/{providerName}/{queryId?}")]
    public async Task<IActionResult> Get(string providerName, string queryId = null, bool paging = true, bool viewAction = false)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var provider = _dataTableDataProviders.Single(provider => provider.Name == providerName);
        if (string.IsNullOrEmpty(queryId)) queryId = providerName;

        var additionalDatatableOptions = JObject.FromObject(new { paging, viewAction })!.AsObject();
        var definition = new DataTableDefinitionViewModel(additionalDatatableOptions)
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
