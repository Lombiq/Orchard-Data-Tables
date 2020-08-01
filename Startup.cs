using Lombiq.DataTables.Services;
using Lombiq.DataTables.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.DataTables
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataTableDataProvider, QueryDataTableDataProvider>();
            services.AddScoped<QueryDataTableDataProvider>();

            services.AddScoped<IDataTableSortingProvider, ContentFieldDataTableSortingProvider>();
            services.AddScoped<IDataTableSortingProvider, ContentPartRecordPropertyDataTableSortingProvider>();
            services.AddScoped<IDataTableSortingProvider, TaxonomyTermDataTableSortingProvider>();

            services.AddScoped<IShapeTableProvider, ContentPickerDataTableShapeTableProvider>();
            services.AddScoped<IShapeTableProvider, ContentPickerLayoutShapeTableProvider>();

            services.AddTagHelpers<DataTableTagHelper>();

            services.AddScoped<IResourceManifestProvider, ResourceManifest>();
        }
    }
}