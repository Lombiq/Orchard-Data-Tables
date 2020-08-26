using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Lombiq.Projections.Helpers;
using System;
using System.Linq;
using static Lombiq.DataTables.Constants.DataTableColumnDefinitionSettingsKeys.ContentPartRecordPropertySorting;

namespace Lombiq.DataTables.Services
{
    public class ContentPartRecordPropertyDataTableSortingProvider : IDataTableSortingProvider
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

            var recordListReferencePropertyNames = ChainableMemberBindingHelper
                .GetRecordListReferencePropertyNames(recordType, propertyName);

            if (recordListReferencePropertyNames == null) return;

            var filterPropertyName = string.Join(".", propertyName
                .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(recordListReferencePropertyNames.Count()));

            context.SortCriterionContext.Query
                .OrderBy(
                    alias => ChainableMemberBindingHelper.GetChainableMemberBindingAlias(
                        alias, recordType, recordListReferencePropertyNames, filterPropertyName),
                    order =>
                    {
                        if (context.Direction == SortingDirection.Ascending) order.Asc(filterPropertyName);
                        else order.Desc(filterPropertyName);
                    });
        }
    }
}