using System;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using static Lombiq.DataTables.Constants.DataTableColumnDefinitionSettingsKeys;

namespace Lombiq.DataTables.Services
{
    public class TaxonomyTermDataTableSortingProvider : IDataTableSortingProvider
    {
        public bool CanSort(string dataSource) => dataSource == DataTableDataSources.TaxonomyTerm;

        public void Sort(DataTableSortingContext context)
        {
            if (!CanSort(context.ColumnDefinition.DataSource)) return;

            var fieldName = context.ColumnDefinition[TaxonomyTermSorting.FieldName];

            if (string.IsNullOrEmpty(fieldName)) return;
            throw new NotImplementedException();
            /*

            var aliasName = "SortableTerms";

            context
                .Query
                .Where(
                    alias => alias
                        .ContentPartRecord<TitleSortableTermsPartRecord>()
                        .Property(nameof(TitleSortableTermsPartRecord.Terms), aliasName),
                    predicate => predicate
                        .And(
                            left => left.Eq(nameof(TitleSortableTermContentItem.Field), fieldName),
                            right => right.Eq(nameof(TitleSortableTermContentItem.IsFirst), true)))
                .OrderBy(
                    alias => alias.Named(aliasName),
                    order =>
                    {
                        if (context.Direction == SortingDirection.Ascending) order.Asc(nameof(TitleSortableTermContentItem.Title));
                        else order.Desc(nameof(TitleSortableTermContentItem.Title));
                    }); */
        }
    }
}