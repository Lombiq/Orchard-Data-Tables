using Lombiq.DataTables.Liquid;
using Lombiq.DataTables.Migrations;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.TagHelpers;
using Lombiq.HelpfulLibraries.AspNetCore.Middlewares;
using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.Data.Migration;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;
using System;

namespace Lombiq.DataTables;

public class Startup : StartupBase
{
    private readonly AdminOptions _adminOptions;

    public Startup(IOptions<AdminOptions> adminOptions) => _adminOptions = adminOptions.Value;

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataTableExportService<ExcelDataTableExportService>();

        services.AddTagHelpers<DataTableTagHelper>();

        services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();
        services.AddDataMigration<ColumnsDefinitionMigrations>();

        services.AddLiquidFilter<ActionsLiquidFilter>("actions");

        services.AddScoped<IDataTableDataProviderServices, DataTableDataProviderServices>();
        services.AddOrchardServices();

        services.AddScoped<IDeferredTask, IndexGeneratorDeferredTask>();

        services.AddDataTableDataProvider<DeletedContentItemDataTableDataProvider>();

        AdminRouteAttributeRouteMapper.AddToServices(services);
    }

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) =>
        app.UseDeferredTasks();
}
