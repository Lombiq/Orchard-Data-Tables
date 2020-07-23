using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lombiq.DataTables.Controllers.Api
{
    public class DataTablesChildRowController : Controller
    {
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;

        public IStringLocalizer T { get; }


        public DataTablesChildRowController(
            IDataTableDataProviderAccessor dataTableDataProviderAccessor,
            IStringLocalizer<DataTablesChildRowController> stringLocalizer)
        {
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            T = stringLocalizer;
        }


        public ActionResult<DataTableChildRowResponse> Get(int contentItemId, string dataProvider)
        {
            var provider = _dataTableDataProviderAccessor.GetDataProvider(dataProvider);
            if (provider == null)
            {
                var errorText = T["The given data provider name is invalid."].Value;
                return BadRequest(DataTableChildRowResponse.ErrorResult(errorText));
            }

            var response = provider.GetChildRow(contentItemId);

            return response;
        }
    }
}