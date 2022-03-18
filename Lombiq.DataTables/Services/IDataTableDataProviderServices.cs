using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using System.Diagnostics.CodeAnalysis;
using ISession = YesSql.ISession;

namespace Lombiq.DataTables.Services;

/// <summary>
/// Bundle of services that are always needed in <see cref="IDataTableDataProvider"/> implementations.
/// </summary>
[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "There is nothing to add past what's already on the individual services' documentations.")]
public interface IDataTableDataProviderServices
{
    IHttpContextAccessor HttpContextAccessor { get; }
    LinkGenerator LinkGenerator { get; }
    ILiquidTemplateManager LiquidTemplateManager { get; }
    IMemoryCache MemoryCache { get; }
    IShapeFactory ShapeFactory { get; }
    ISession Session { get; }
    IAuthorizationService AuthorizationService { get; }
    IContentManager ContentManager { get; }
}
