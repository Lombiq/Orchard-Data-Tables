using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Orchard.Localization;
using System.Net;
using System.Web.Http;

namespace Lombiq.DataTables.Controllers.Api
{
    public class DataTablesChildRowController : ApiController
    {
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;

        public Localizer T { get; set; }


        public DataTablesChildRowController(IDataTableDataProviderAccessor dataTableDataProviderAccessor)
        {
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;

            T = NullLocalizer.Instance;
        }


        public IHttpActionResult Get(int contentItemId, string dataProvider)
        {
            var provider = _dataTableDataProviderAccessor.GetDataProvider(dataProvider);
            if (provider == null)
            {
                return Content(HttpStatusCode.BadRequest, DataTableChildRowResponse.ErrorResult(T("The given data provider name is invalid.").Text));
            }

            var response = provider.GetChildRow(contentItemId);

            return Content(HttpStatusCode.OK, response);
        }
    }
}