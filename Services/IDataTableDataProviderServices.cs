using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
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
        IHttpContextAccessor HttpContextAccessor { get; }
        LinkGenerator LinkGenerator { get; }
        ILiquidTemplateManager LiquidTemplateManager { get; }
        IMemoryCache MemoryCache { get; }
    }
}
