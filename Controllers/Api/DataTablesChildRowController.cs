using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Controllers.Api
{
    public class DataTablesChildRowController : Controller
    {
        private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviderAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IStringLocalizer T;


        public DataTablesChildRowController(
            IEnumerable<IDataTableDataProvider> dataTableDataProviderAccessor,
            IAuthorizationService authorizationService,
            IStringLocalizer<DataTablesChildRowController> stringLocalizer)
        {
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            _authorizationService = authorizationService;
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

            if (!(await provider.Authorize(_authorizationService, User)))
            {
                return DataTableChildRowResponse.ErrorResult(T["Unauthorized!"]);
            }

            return await provider.GetChildRowAsync(contentItemId);
        }
    }
}
