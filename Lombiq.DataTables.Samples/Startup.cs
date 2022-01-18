using Lombiq.DataTables.Samples.Migrations;
using Lombiq.DataTables.Samples.Models;
using Lombiq.DataTables.Samples.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;

namespace Lombiq.DataTables.Samples
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddContentPart<EmployeePart>()
                .WithMigration<EmployeeMigrations>();

            services.AddDataTableDataProvider<SampleJsonResultDataTableDataProvider>();
        }
    }
}
