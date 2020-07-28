using System;
using System.Collections.Generic;
using System.Linq;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using OrchardCore.ContentManagement.Utilities;
using static Lombiq.DataTables.Constants.DataTableColumnDefinitionSettingsKeys;

namespace Lombiq.DataTables.Services
{
    public class ContentFieldDataTableSortingProvider : IDataTableSortingProvider
    {
        // private readonly Lazy<IEnumerable<IFieldTypeEditor>> _lazyFieldTypeEditors;


        // public ContentFieldDataTableSortingProvider(Lazy<IEnumerable<IFieldTypeEditor>> lazyFieldTypeEditors)
        // {
        //     _lazyFieldTypeEditors = lazyFieldTypeEditors;
        // }
        

        public bool CanSort(string dataSource) => dataSource == DataTableDataSources.ContentField;

        public void Sort(DataTableSortingContext context)
        {
            throw new NotImplementedException();
            /*
            if (!CanSort(context.ColumnDefinition.DataSource)) return;

            var valueType = Type.GetType(context.ColumnDefinition[ContentFieldSorting.ValueType]);
            // var fieldTypeEditor = _lazyFieldTypeEditors.Value.FirstOrDefault(editor => editor.CanHandle(valueType));
            //if (fieldTypeEditor == null) return;

            var propertyName = context.ColumnDefinition[ContentFieldSorting.PropertyName];
            var aliasName = propertyName.ToSafeName();

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
                    }); // */
        }
    }
}