using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableDataProvider
    {
        string Name { get; }

        LocalizedString Description { get; }

        DataTableRow GetRow(ContentItem contentItem);

        DataTableDataResponse GetRows(DataTableDataRequest request);

        DataTableColumnsDefinition GetColumnsDefinition();

        DataTableChildRowResponse GetChildRow(int contentItemId);
    }
}