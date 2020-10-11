using Lombiq.DataTables.Liquid;
using Lombiq.DataTables.Migrations;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.DataTables
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddDataTableDataProvider<QueryDataTableDataProvider>();
            services.AddDataTableExportService<ExcelDataTableExportService>();

            services.AddScoped<IShapeTableProvider, ContentPickerDataTableShapeTableProvider>();
            services.AddScoped<IShapeTableProvider, ContentPickerLayoutShapeTableProvider>();

            services.AddTagHelpers<DataTableTagHelper>();

            services.AddScoped<IResourceManifestProvider, ResourceManifest>();
            services.AddScoped<IDataMigration, ColumnsDefinitionMigrations>();

            services.AddLiquidFilter<ActionsLiquidFilter>("actions");

            services.AddScoped<IDataTableDataProviderServices, DataTableDataProviderServices>();
        }
    }
}
