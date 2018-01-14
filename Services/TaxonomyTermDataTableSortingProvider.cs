using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Orchard.Core.Title.Models;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Taxonomies.Models;
using Orchard.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lombiq.DataTables.Constants.DataTableColumnDefinitionSettingsKeys;

namespace Lombiq.DataTables.Services
{
    public class TaxonomyTermDataTableSortingProvider : IDataTableSortingProvider
    {
        private const string AcceptedDataSource = DataTableDataSources.TaxonomyTerm;
        

        public bool CanSort(string dataSource) => dataSource == AcceptedDataSource;

        public void Sort(DataTableSortingContext context)
        {
            if (!CanSort(context.ColumnDefinition.DataSource)) return;

            var fieldName = context.ColumnDefinition[TaxonomyTermSorting.FieldName];
            var aliasName = fieldName.ToSafeName();

            context
                .Query
                .Where(
                    alias => alias
                        .ContentPartRecord<TermsPartRecord>()
                        .Property("Terms", aliasName),
                    predicate => predicate
                        .Eq("Field", fieldName))
                .OrderBy(
                    alias => alias.Named(aliasName),
                    order =>
                    {
                        if (context.Direction == SortingDirection.Ascending) order.Asc("TermRecord.Path");
                        else order.Desc("TermRecord.Path");
                    });
        }
    }
}