using Lombiq.DataTables.LombiqTests.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace Lombiq.DataTables.LombiqTests;

[RequireFeatures("Lombiq.Tests.UI.Shortcuts")]
public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services) =>
        services.Configure<MvcOptions>(options => options.Filters.Add(typeof(TestingFilter)));
}
