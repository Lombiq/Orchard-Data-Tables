using Lombiq.DataTables.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using System.Threading.Tasks;

namespace Lombiq.DataTables.LombiqTests.Services
{
    public class TestingFilter : IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly IShapeFactory _shapeFactory;

        public TestingFilter(ILayoutAccessor layoutAccessor, IShapeFactory shapeFactory)
        {
            _layoutAccessor = layoutAccessor;
            _shapeFactory = shapeFactory;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context?.HttpContext == null || !(context.Controller is TableController))
            {
                await next();
                return;
            }

            dynamic layout = await _layoutAccessor.GetLayoutAsync();
            var zone = layout.Zones["Header"];
            zone.Add(await _shapeFactory.New.Lombiq_Datatable_Testing_Header());

            await next();
        }
    }
}
