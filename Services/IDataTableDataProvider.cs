using Lombiq.DataTables.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using System.Collections.Generic;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableDataProvider : IDependency
    {
        string Name { get; }

        LocalizedString Description { get; }

        DataTableRow GetRow(ContentItem contentItem);

        DataTableDataResponse GetRows(DataTableDataRequest request);

        IEnumerable<DataTableColumnDefinition> GetColumns();

        DataTableChildRowResponse GetChildRow(int contentItemId);
    }
}