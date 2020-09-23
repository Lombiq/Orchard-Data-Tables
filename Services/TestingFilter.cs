using Lombiq.DataTables.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchardCore;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public class TestingFilter : IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly IShapeFactory _shapeFactory;
        private readonly IShellFeaturesManager _orchardHelper;

        public TestingFilter(ILayoutAccessor layoutAccessor, IShapeFactory shapeFactory, IShellFeaturesManager orchardHelper)
        {
            _layoutAccessor = layoutAccessor;
            _shapeFactory = shapeFactory;
            _orchardHelper = orchardHelper;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context?.HttpContext == null ||
                !(context.Controller is TableController))
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
