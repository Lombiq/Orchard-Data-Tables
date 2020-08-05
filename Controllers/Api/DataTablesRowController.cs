using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lombiq.DataTables.Constants;

namespace Lombiq.DataTables.Controllers.Api
{
    public class DataTablesRowController : Controller
    {
        private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviderAccessor;
        private readonly Dictionary<string, IDataTableExportService> _exportServices;
        private readonly IStringLocalizer T;


        public DataTablesRowController(
            IEnumerable<IDataTableDataProvider> dataTableDataProviderAccessor,
            IEnumerable<IDataTableExportService> exportServices,
            IStringLocalizer<DataTablesRowController> stringLocalizer)
        {
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            _exportServices = exportServices.ToDictionary(x => x.Name);
            T = stringLocalizer;
        }


        /// <summary>
        /// Gets the current table view's rows.
        /// </summary>
        /// <param name="request">The request to fulfill.</param>
        /// <param name="order">The <see cref="DataTableDataRequest.Order"/> property.</param>
        /// <returns>The response for this API call.</returns>
        /// <remarks>
        /// ASP.Net Core seems to have trouble with binding array properties in the object so it's necessary to bind
        /// the array separately and manually set the property from the bound parameter.
        /// </remarks>
        public async Task<ActionResult<DataTableDataResponse>> Get(
            DataTableDataRequest request,
            [FromQuery(Name = "order")] ICollection<Dictionary<string, string>> order)
        {
            request.SetOrder(order);
            var dataProvider = _dataTableDataProviderAccessor.GetDataProvider(request.DataProvider);
            if (dataProvider == null)
            {
                var errorText = T["The given data provider name is invalid."].Value;
                return BadRequest(DataTableDataResponse.ErrorResult(errorText));
            }

            if (request.Length == 0)
            {
                return BadRequest(DataTableDataResponse.ErrorResult(T["Length can't be 0."].Value));
            }

            var response = await dataProvider.GetRowsAsync(request);

            // This property identifies the request for the jQuery.DataTables plugin. This needs to be parsed and
            // sent back in order to prevent Cross Site Scripting (XSS) attack.
            // See: https://datatables.net/manual/server-side
            response.Draw = request.Draw;

            return response;
        }


        public async Task<ActionResult<DataTableDataResponse>> Export(
            DataTableDataRequest request,
            [FromQuery(Name = "order")] ICollection<Dictionary<string, string>> order,
            string name,
            bool exportAll = true)
        {
            request.SetOrder(order);
            if (exportAll)
            {
                request.Start = 0;
                request.Length = 999_999; // One for the header, Excel can take a million rows.
            }

            var dataProvider = _dataTableDataProviderAccessor.GetDataProvider(request.DataProvider);
            var stream = await _exportServices[name].ExportAsync(dataProvider, request);

            return new FileStreamResult(stream, _exportServices[name].ContentType)
            {
                FileDownloadName = _exportServices[name].DefaultFileName
            };
        }
    }
}
