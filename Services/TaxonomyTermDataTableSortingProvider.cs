using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Lombiq.Projections.Models;
using Orchard.Core.Title.Models;
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

            var aliasName = "SortableTerm";

            context
                .Query
                .Where(
                    alias => alias
                        .ContentPartRecord<TitleSortableTermsPartRecord>()
                        .Property(nameof(TitleSortableTermsPartRecord.Terms), aliasName),
                    predicate => predicate
                        .And(
                            left => left.Eq(nameof(TitleSortableTermContentItem.Field), fieldName),
                            right => right.Eq(nameof(TitleSortableTermContentItem.IsFirstTerm), true)))
                .OrderBy(
                    alias => alias.Named(aliasName),
                    order =>
                    {
                        var byTitle = $"{nameof(TitleSortableTermContentItem.TitlePartRecord)}.{nameof(TitlePart.Title)}";

                        if (context.Direction == SortingDirection.Asc) order.Asc(byTitle);
                        else order.Desc(byTitle);
                    });
        }
    }
}