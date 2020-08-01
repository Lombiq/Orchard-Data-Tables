using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableDataProvider
    {
        string Name { get; }

        LocalizedString Description { get; }

        Task<DataTableRow> GetRowAsync(ContentItem contentItem);

        Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request);

        Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync();

        Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId);
    }
}
