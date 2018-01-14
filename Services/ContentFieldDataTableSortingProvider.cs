using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lombiq.DataTables.Constants.DataTableColumnDefinitionSettingsKeys;

namespace Lombiq.DataTables.Services
{
    public class ContentFieldDataTableSortingProvider : IDataTableSortingProvider
    {
        private const string AcceptedDataSource = DataTableDataSources.ContentField;


        private readonly Lazy<IEnumerable<IFieldTypeEditor>> _lazyFieldTypeEditors;


        public ContentFieldDataTableSortingProvider(Lazy<IEnumerable<IFieldTypeEditor>> lazyFieldTypeEditors)
        {
            _lazyFieldTypeEditors = lazyFieldTypeEditors;
        }
        

        public bool CanSort(string dataSource) => dataSource == AcceptedDataSource;

        public void Sort(DataTableSortingContext context)
        {
            if (!CanSort(context.ColumnDefinition.DataSource)) return;

            var valueType = Type.GetType(context.ColumnDefinition[ContentFieldSorting.ValueType]);
            var fieldTypeEditor = _lazyFieldTypeEditors.Value.FirstOrDefault(editor => editor.CanHandle(valueType));

            if (fieldTypeEditor == null) return;

            var propertyName = context.ColumnDefinition[ContentFieldSorting.PropertyName];
            var aliasName = propertyName.ToSafeName();
            var relationship = fieldTypeEditor.GetFilterRelationship(aliasName);

            context
                .Query
                .Where(
                    fieldTypeEditor.GetFilterRelationship(aliasName),
                    predicate => predicate.Eq("PropertyName", propertyName))
                .OrderBy(
                    alias => alias.Named(aliasName),
                    order =>
                    {
                        if (context.Direction == SortingDirection.Ascending) order.Asc("Value");
                        else order.Desc("Value");
                    });
        }
    }
}