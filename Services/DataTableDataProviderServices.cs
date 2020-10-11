using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.Liquid;

namespace Lombiq.DataTables.Services
{
    public class DataTableDataProviderServices : IDataTableDataProviderServices
    {
        public IStringLocalizer<JsonResultDataTableDataProvider> StringLocalizer { get; }
        public ILiquidTemplateManager LiquidTemplateManager { get; }
        public LinkGenerator LinkGenerator { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public IMemoryCache MemoryCache { get; }

        public DataTableDataProviderServices(
            IStringLocalizer<JsonResultDataTableDataProvider> stringLocalizer,
            ILiquidTemplateManager liquidTemplateManager,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache)
        {
            StringLocalizer = stringLocalizer;
            LiquidTemplateManager = liquidTemplateManager;
            LinkGenerator = linkGenerator;
            HttpContextAccessor = httpContextAccessor;
            MemoryCache = memoryCache;
        }
    }
}
