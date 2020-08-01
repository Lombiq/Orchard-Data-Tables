using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableDataProvider
    {
        string Name => GetType().Name;
        LocalizedString Description { get; }

        Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request);
        Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId);
        Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId);
    }
}
