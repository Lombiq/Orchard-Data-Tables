using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using ISession = YesSql.ISession;

namespace Lombiq.DataTables.Services
{
    public class DataTableDataProviderServices : IDataTableDataProviderServices
    {
        public IHttpContextAccessor HttpContextAccessor { get; }
        public LinkGenerator LinkGenerator { get; }
        public ILiquidTemplateManager LiquidTemplateManager { get; }
        public IMemoryCache MemoryCache { get; }
        public IShapeFactory ShapeFactory { get; }
        public ISession Session { get; }
        public IAuthorizationService AuthorizationService { get; }

        public DataTableDataProviderServices(
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator,
            ILiquidTemplateManager liquidTemplateManager,
            IMemoryCache memoryCache,
            IShapeFactory shapeFactory,
            ISession session,
            IAuthorizationService authorizationService)
        {
            HttpContextAccessor = httpContextAccessor;
            LinkGenerator = linkGenerator;
            LiquidTemplateManager = liquidTemplateManager;
            MemoryCache = memoryCache;
            ShapeFactory = shapeFactory;
            Session = session;
            AuthorizationService = authorizationService;
        }
    }
}
