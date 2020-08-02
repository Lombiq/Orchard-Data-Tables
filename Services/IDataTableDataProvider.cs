using System;
using System.Collections.Generic;
using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;
using OrchardCore.DisplayManagement;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableDataProvider
    {
        string Name => GetType().Name;
        LocalizedString Description { get; }

        Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request);
        Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId);
        Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId);

        Task<IEnumerable<IShape>> GetShapesBeforeTableAsync() => Task.FromResult<IEnumerable<IShape>>(Array.Empty<IShape>());
        Task<IEnumerable<IShape>> GetShapesAfterTableAsync() => Task.FromResult<IEnumerable<IShape>>(Array.Empty<IShape>());
    }
}
