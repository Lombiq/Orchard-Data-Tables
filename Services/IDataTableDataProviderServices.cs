using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.Liquid;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.DataTables.Services
{
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:Elements should be documented",
        Justification = "There is nothing to add past what's already on the individual services' documentations.")]
    public interface IDataTableDataProviderServices
    {
        IStringLocalizer<JsonResultDataTableDataProvider> StringLocalizer { get; }
        ILiquidTemplateManager LiquidTemplateManager { get; }
        LinkGenerator LinkGenerator { get; }
        IHttpContextAccessor HttpContextAccessor { get; }
        IMemoryCache MemoryCache { get; }
    }
}
