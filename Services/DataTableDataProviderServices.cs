using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.ContentManagement;
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
        public IContentManager ContentManager { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "These are the common DataTable services.")]
        public DataTableDataProviderServices(
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator,
            ILiquidTemplateManager liquidTemplateManager,
            IMemoryCache memoryCache,
            IShapeFactory shapeFactory,
            ISession session,
            IAuthorizationService authorizationService,
            IContentManager contentManager)
        {
            HttpContextAccessor = httpContextAccessor;
            LinkGenerator = linkGenerator;
            LiquidTemplateManager = liquidTemplateManager;
            MemoryCache = memoryCache;
            ShapeFactory = shapeFactory;
            Session = session;
            AuthorizationService = authorizationService;
            ContentManager = contentManager;
        }
    }
}
