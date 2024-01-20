using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using ISession = YesSql.ISession;

namespace Lombiq.DataTables.Services;

[method: System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "These are the common DataTable services.")]
public class DataTableDataProviderServices(
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator,
    ILiquidTemplateManager liquidTemplateManager,
    IMemoryCache memoryCache,
    IShapeFactory shapeFactory,
    ISession session,
    IAuthorizationService authorizationService,
    IContentManager contentManager) : IDataTableDataProviderServices
{
    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;
    public LinkGenerator LinkGenerator { get; } = linkGenerator;
    public ILiquidTemplateManager LiquidTemplateManager { get; } = liquidTemplateManager;
    public IMemoryCache MemoryCache { get; } = memoryCache;
    public IShapeFactory ShapeFactory { get; } = shapeFactory;
    public ISession Session { get; } = session;
    public IAuthorizationService AuthorizationService { get; } = authorizationService;
    public IContentManager ContentManager { get; } = contentManager;
}
