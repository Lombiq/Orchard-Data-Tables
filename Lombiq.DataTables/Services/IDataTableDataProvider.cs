using Lombiq.DataTables.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services;

/// <summary>
/// The data source for a DataTable.
/// </summary>
/// <remarks>
/// <para>
/// Unlike in O1, implementing services have to be registered manually using the extension method called <see
/// cref="ServiceCollectionExtensions.AddDataTableDataProvider{TDataProvider}"/>.
/// </para>
/// </remarks>
public interface IDataTableDataProvider
{
    /// <summary>
    /// Gets the technical name used to identify the provider.
    /// </summary>
    string Name => GetType().Name;

    /// <summary>
    /// Gets short human-readable name of the provider.
    /// </summary>
    LocalizedString Description { get; }

    /// <summary>
    /// Gets an optional collection of permissions. If not null, the user must authorize against at least one.
    /// </summary>
    IEnumerable<Permission> AllowedPermissions { get; }

    /// <summary>
    /// Returns the table body created based on the provided request.
    /// </summary>
    /// <param name="request">Describes the desired state of the table.</param>
    /// <returns>The resulting table content and some metadata.</returns>
    Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request);

    /// <summary>
    /// Gets the default column set supplied by the provider.
    /// </summary>
    /// <param name="queryId">May be used to hint additional conditions during dynamic column generation.</param>
    /// <returns>The description of the columns.</returns>
    Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId);

    /// <summary>
    /// If the table supports child rows, provides them similarly to <see cref="GetRowsAsync"/>.
    /// </summary>
    Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId);

    /// <summary>
    /// Optional shapes to be displayed before the table.
    /// </summary>
    Task<IEnumerable<IShape>> GetShapesBeforeTableAsync();

    /// <summary>
    /// Optional shapes to be displayed after the table.
    /// </summary>
    Task<IEnumerable<IShape>> GetShapesAfterTableAsync();
}
