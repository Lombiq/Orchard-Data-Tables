using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using System;
using System.Linq;
using static Lombiq.DataTables.Constants.DataTableColumnDefinitionSettingsKeys.ContentPartRecordPropertySorting;

namespace Lombiq.DataTables.Services
{
    public class TitlePartTitleDataTableSortingProvider : IDataTableSortingProvider
    {
        public bool CanSort(string dataSource) => dataSource == DataTableDataSources.ContentPartRecordProperty;

        public void Sort(DataTableSortingContext context)
        {
            if (!CanSort(context.ColumnDefinition.DataSource)) return;

            var settings = context.ColumnDefinition.AdditionalSettings;

            if (!settings.Any()) return;

            var propertyName = settings[PropertyName];

            if (string.IsNullOrEmpty(propertyName)) return;

            var recordTypeAssemblyQualifiedName = settings[RecordTypeAssemblyQualifiedName];

            if (string.IsNullOrEmpty(recordTypeAssemblyQualifiedName)) return;

            var recordType = Type.GetType(recordTypeAssemblyQualifiedName);

            if (recordType == null) return;

            context.Query.OrderBy(
                alias => alias.ContentPartRecord(recordType),
                order =>
                {
                    if (context.Direction == SortingDirection.Ascending) order.Asc(propertyName);
                    else order.Desc(propertyName);
                });
        }
    }
}