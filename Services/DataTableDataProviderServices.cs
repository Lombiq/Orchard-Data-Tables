using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.Liquid;

namespace Lombiq.DataTables.Services
{
    public class DataTableDataProviderServices : IDataTableDataProviderServices
    {
        public IHttpContextAccessor HttpContextAccessor { get; }
        public LinkGenerator LinkGenerator { get; }
        public ILiquidTemplateManager LiquidTemplateManager { get; }
        public IMemoryCache MemoryCache { get; }
        public IStringLocalizer<JsonResultDataTableDataProvider> StringLocalizer { get; }

        public DataTableDataProviderServices(
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator,
            ILiquidTemplateManager liquidTemplateManager,
            IMemoryCache memoryCache,
            IStringLocalizer<JsonResultDataTableDataProvider> stringLocalizer)
        {
            HttpContextAccessor = httpContextAccessor;
            LinkGenerator = linkGenerator;
            LiquidTemplateManager = liquidTemplateManager;
            MemoryCache = memoryCache;
            StringLocalizer = stringLocalizer;
        }
    }
}
