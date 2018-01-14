using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Orchard.Localization;
using System.Net;
using System.Web.Http;

namespace Lombiq.DataTables.Controllers.Api
{
    public class DataTablesRowController : ApiController
    {
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;
        
        public Localizer T { get; set; }


        public DataTablesRowController(IDataTableDataProviderAccessor dataTableDataProviderAccessor)
        {
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;

            T = NullLocalizer.Instance;
        }


        public IHttpActionResult Post([FromBody]DataTableDataRequest request)
        {
            var dataProvider = _dataTableDataProviderAccessor.GetDataProvider(request.DataProvider);
            if (dataProvider == null)
            {
                return Content(HttpStatusCode.BadRequest, DataTableDataResponse.ErrorResult(T("The given data provider name is invalid.").Text));
            }

            if (request.Length == 0)
            {
                return Content(HttpStatusCode.BadRequest, DataTableDataResponse.ErrorResult(T("Length can't be 0.").Text));
            }

            var response = dataProvider.GetRows(request);

            // This property identifies the request for the jQuery.DataTables plugin. This needs to be parsed and
            // sent back in order to prevent Cross Site Scripting (XSS) attack.
            // See: https://datatables.net/manual/server-side
            response.Draw = request.Draw;

            return Content(HttpStatusCode.OK, response);
        }
    }
}