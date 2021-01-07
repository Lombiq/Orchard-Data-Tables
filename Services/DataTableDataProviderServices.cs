using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Liquid;

namespace Lombiq.DataTables.Services
{
    public class DataTableDataProviderServices : IDataTableDataProviderServices
    {
        public IHttpContextAccessor HttpContextAccessor { get; }
        public LinkGenerator LinkGenerator { get; }
        public ILiquidTemplateManager LiquidTemplateManager { get; }
        public IMemoryCache MemoryCache { get; }

        public DataTableDataProviderServices(
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator,
            ILiquidTemplateManager liquidTemplateManager,
            IMemoryCache memoryCache)
        {
            HttpContextAccessor = httpContextAccessor;
            LinkGenerator = linkGenerator;
            LiquidTemplateManager = liquidTemplateManager;
            MemoryCache = memoryCache;
        }
    }
}
