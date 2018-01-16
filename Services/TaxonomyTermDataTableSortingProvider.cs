using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
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

            var aliasName = "SortableTerms";

            context
                .Query
                .Where(
                    alias => alias
                        .ContentPartRecord<TitleSortableTermsPartRecord>()
                        .Property(nameof(TitleSortableTermsPartRecord.Terms), aliasName),
                    predicate => predicate
                        .Eq(nameof(TitleSortableTermContentItem.Field), fieldName))
                .OrderBy(
                    alias => alias.Named(aliasName),
                    order =>
                    {
                        var property = $"{nameof(TitlePartRecord)}.{nameof(TitlePart.Title)}";

                        if (context.Direction == SortingDirection.Asc) order.Asc(property);
                        else order.Desc(property);
                    });
        }
    }
}