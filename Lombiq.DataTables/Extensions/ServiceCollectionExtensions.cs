using Lombiq.DataTables.Handlers;
using Lombiq.DataTables.Services;
using Lombiq.HelpfulLibraries.OrchardCore.Data;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data.Migration;
using YesSql.Indexes;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an implementation of <see cref="IDataTableDataProvider"/> for DataTables.
    /// </summary>
    public static IServiceCollection AddDataTableDataProvider<TDataProvider>(this IServiceCollection services)
        where TDataProvider : class, IDataTableDataProvider =>
        services.AddScoped<IDataTableDataProvider, TDataProvider>();

    /// <summary>
    /// Registers an implementation of <see cref="IDataTableExportService"/> for DataTables.
    /// </summary>
    public static IServiceCollection AddDataTableExportService<TService>(this IServiceCollection services)
        where TService : class, IDataTableExportService =>
        services.AddScoped<IDataTableExportService, TService>();

    public static IServiceCollection AddIndexBasedDataTableProvider<TIndex, TGenerator, TMigration, TProvider>(
        this IServiceCollection services)
        where TIndex : MapIndex
        where TGenerator : class, IDataTableIndexGenerator
        where TMigration : IndexDataMigration<TIndex>
        where TProvider : IndexBasedDataTableDataProvider<TIndex>
    {
        services.AddSingleton<IManualConnectingIndexService<TIndex>, ManualConnectingIndexService<TIndex>>();
        services.AddScoped<TGenerator, TGenerator>();
        services.AddScoped<IContentHandler, DataTableIndexGeneratorContentHandler<TGenerator, TIndex>>();
        services.AddScoped<IManualDataTableIndexGenerator, DataTableIndexGeneratorContentHandler<TGenerator, TIndex>>();
        services.AddScoped<IDataMigration, TMigration>();
        services.AddDataTableDataProvider<TProvider>();

        return services;
    }
}
