using Lombiq.DataTables.Samples.Indexes;
using Lombiq.DataTables.Samples.Migrations;
using Lombiq.DataTables.Samples.Models;
using Lombiq.DataTables.Samples.Navigation;
using Lombiq.DataTables.Samples.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using OrchardCore.Navigation;

namespace Lombiq.DataTables.Samples;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddContentPart<EmployeePart>()
            .WithMigration<EmployeeMigrations>();

        // A JSON-based provider doesn't need anything else.
        services.AddDataTableDataProvider<SampleJsonResultDataTableDataProvider>();

        // As index-based providers rely on their own index, generator, and migration they're registered all at once to
        // ensure their types match.
        services.AddIndexBasedDataTableProvider<
            EmployeeDataTableIndex,
            EmployeeDataTableIndexGenerator,
            EmployeeDataTableMigrations,
            SampleIndexBasedDataTableDataProvider>();

            services.AddScoped<INavigationProvider, DataTablesNavigationProvider>();
    }
}

// END OF TRAINING SECTION: Index-based Data Provider
