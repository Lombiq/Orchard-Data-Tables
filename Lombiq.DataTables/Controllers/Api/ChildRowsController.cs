using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Controllers.Api;

public class ChildRowsController(
    IEnumerable<IDataTableDataProvider> dataTableDataProviderAccessor,
    IAuthorizationService authorizationService,
    IStringLocalizer<ChildRowsController> stringLocalizer) : Controller
{
    private readonly IStringLocalizer T = stringLocalizer;

    public async Task<ActionResult<DataTableChildRowResponse>> Get(int contentItemId, string dataProvider)
    {
        var provider = dataTableDataProviderAccessor.GetDataProvider(dataProvider);
        if (provider == null)
        {
            var errorText = T["The given data provider name is invalid."].Value;
            return BadRequest(DataTableChildRowResponse.ErrorResult(errorText));
        }

        return !await provider.AuthorizeAsync(authorizationService, User)
            ? DataTableChildRowResponse.ErrorResult(T["Unauthorized!"])
            : (ActionResult<DataTableChildRowResponse>)await provider.GetChildRowAsync(contentItemId);
    }
}
