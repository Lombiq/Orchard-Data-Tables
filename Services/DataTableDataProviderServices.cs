using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.Liquid;

namespace Lombiq.DataTables.Services
{
    // Providers inheriting from JsonResultDataTableDataProvider should not be affected by S107, but they somehow are:
    // https://github.com/SonarSource/sonar-dotnet/issues/1015
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
