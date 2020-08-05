using System.Collections.Generic;
using System.Threading.Tasks;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lombiq.DataTables.Controllers.Api
{
    public class DataTablesChildRowController : Controller
    {
        private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviderAccessor;
        private readonly IStringLocalizer T;


        public DataTablesChildRowController(
            IEnumerable<IDataTableDataProvider> dataTableDataProviderAccessor,
            IStringLocalizer<DataTablesChildRowController> stringLocalizer)
        {
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            T = stringLocalizer;
        }


        public async Task<ActionResult<DataTableChildRowResponse>> Get(int contentItemId, string dataProvider)
        {
            var provider = _dataTableDataProviderAccessor.GetDataProvider(dataProvider);
            if (provider == null)
            {
                var errorText = T["The given data provider name is invalid."].Value;
                return BadRequest(DataTableChildRowResponse.ErrorResult(errorText));
            }

            return await provider.GetChildRowAsync(contentItemId);
        }
    }
}
