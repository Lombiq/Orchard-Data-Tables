using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Controllers
{
    public class QueryController : Controller
    {
        private readonly IDataTableDataProvider _dataTableDataProvider;

        public QueryController(IEnumerable<IDataTableDataProvider> dataTableDataProviders)
        {
            _dataTableDataProvider = dataTableDataProviders.Single(x => x.Name == nameof(QueryDataTableDataProvider));
        }

        [Admin]
        public Task<IActionResult> AdminGet() => Get();

        public async Task<IActionResult> Get()
        {
            var model = new DataTableDefinitionViewModel
            {
                DataProvider = _dataTableDataProvider.Name,
                QueryId = "UserProfilesQuery",
                ColumnsDefinition = new DataTableColumnsDefinition
                {
                    Columns = new DataTableColumnDefinition[]
                    {
                        new DataTableColumnDefinition { Name = "Content.ContentItem.DisplayText", Orderable = true, Text = "Display Text" },
                        new DataTableColumnDefinition { Name = "Content.ContentItem.PublishedUtc", Orderable = true, Text = "Published UTC" },
                    },
                    DefaultSortingColumnName = "",
                    DefaultSortingDirection = Constants.SortingDirection.Ascending,
                },

            };

            return View(nameof(Get), model);
        }
    }
}
