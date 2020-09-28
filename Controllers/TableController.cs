using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Controllers
{
    [Admin]
    public class TableController : Controller
    {
        private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviders;
        private readonly IContentManager _contentManager;

        public TableController(
            IEnumerable<IDataTableDataProvider> dataTableDataProviders,
            IContentManager contentManager)
        {
            _dataTableDataProviders = dataTableDataProviders;
            _contentManager = contentManager;
        }

        public async Task<IActionResult> Query(string queryName, string contentId)
        {
            var provider = _dataTableDataProviders
                .Single(provider => provider.Name == nameof(QueryDataTableDataProvider));
            var definition = new DataTableDefinitionViewModel
            {
                DataProvider = provider.Name,
                QueryId = queryName,
                ColumnsDefinition = (await _contentManager.GetAsync(contentId))
                    .As<DataTableColumnsDefinitionPart>()
                    .Definition,
            };

            return View(nameof(Get), new DataTableViewModel
            {
                Definition = definition,
                Provider = provider,
                BeforeTable = await provider.GetShapesBeforeTableAsync(),
                AfterTable = await provider.GetShapesAfterTableAsync(),
            });
        }

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
}
