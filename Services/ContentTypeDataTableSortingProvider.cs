using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Orchard.ContentManagement.Records;

namespace Lombiq.DataTables.Services
{
    public class ContentTypeDataTableSortingProvider : IDataTableSortingProvider
    {
        public bool CanSort(string dataSource) => dataSource == DataTableDataSources.ContentType;

        public void Sort(DataTableSortingContext context)
        {
            if (!CanSort(context.ColumnDefinition.DataSource)) return;

            context.Query.OrderBy(
                alias => alias.ContentType(),
                order =>
                {
                    if (context.Direction == SortingDirection.Ascending) order.Asc(nameof(ContentTypeRecord.Name));
                    else order.Desc(nameof(ContentTypeRecord.Name));
                });
        }
    }
}