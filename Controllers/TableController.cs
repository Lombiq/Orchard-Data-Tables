using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.ContentManagement;

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
                .Single(x => x.Name == nameof(QueryDataTableDataProvider));
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

        public async Task<IActionResult> Get(string providerName, string id = null)
        {
            var provider = _dataTableDataProviders.Single(x => x.Name == providerName);
            if (string.IsNullOrEmpty(id)) id = providerName;
            var definition = new DataTableDefinitionViewModel
            {
                DataProvider = providerName,
                QueryId = id,
                ColumnsDefinition = await provider.GetColumnsDefinitionAsync(id),
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
