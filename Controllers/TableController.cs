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
    public class QueryController : Controller
    {
        private readonly IDataTableDataProvider _dataTableDataProvider;
        private readonly IContentManager _contentManager;

        public QueryController(
            IEnumerable<IDataTableDataProvider> dataTableDataProviders,
            IContentManager contentManager)
        {
            _dataTableDataProvider = dataTableDataProviders.Single(x => x.Name == nameof(QueryDataTableDataProvider));
            _contentManager = contentManager;
        }

        [Admin]
        public async Task<IActionResult> Query(string queryName, string contentId)
        {
            var model = new DataTableDefinitionViewModel
            {
                DataProvider = _dataTableDataProvider.Name,
                QueryId = queryName,
                ColumnsDefinition = (await _contentManager.GetAsync(contentId)).As<DataTableColumnsDefinition>(),
            };

            return View(model);
        }
    }
}
