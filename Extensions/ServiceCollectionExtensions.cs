using Lombiq.DataTables.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a <see cref="IDataTableDataProvider"/> for DataTables.
        /// </summary>
        public static IServiceCollection AddDataTableDataProvider<TDataProvider>(this IServiceCollection services)
            where TDataProvider : class, IDataTableDataProvider =>
            services
                .AddScoped<IDataTableDataProvider, TDataProvider>()
                .AddScoped<TDataProvider>();
    }
}
