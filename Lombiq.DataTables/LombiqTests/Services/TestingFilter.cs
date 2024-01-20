using Lombiq.DataTables.Controllers;
using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using System.Threading.Tasks;

namespace Lombiq.DataTables.LombiqTests.Services;

public class TestingFilter(ILayoutAccessor layoutAccessor, IShapeFactory shapeFactory) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context?.HttpContext == null || context.Controller is not TableController || context.IsNotFullViewRendering())
        {
            await next();
            return;
        }

        IShape testingHeader = await shapeFactory.New.Lombiq_Datatable_Testing_Header();
        await layoutAccessor.AddShapeToZoneAsync("Header", testingHeader);

        await next();
    }
}
