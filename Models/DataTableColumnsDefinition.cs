using Lombiq.DataTables.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnsDefinition
    {
        public IEnumerable<DataTableColumnDefinition> Columns { get; set; } = Enumerable.Empty<DataTableColumnDefinition>();

        public string DefaultSortingColumnName { get; set; } = "";
        public SortingDirection DefaultSortingDirection { get; set; } = SortingDirection.Ascending;
    }

    public static class DataTableColumnsDefinitionExtensions
    {
        public static IEnumerable<DataTableColumnDefinition> GetVisibleColumns(this DataTableColumnsDefinition definition) =>
            definition.Columns.Where(column => column.DisplayCondition());

        public static int GetDefaultSortingColumnIndex(this DataTableColumnsDefinition definition) =>
            Math.Max(
                0,
                Math.Max(
                    definition.GetVisibleColumns().ToList().FindIndex(column => column.Orderable),
                    definition.GetVisibleColumns().ToList().FindIndex(column => column.Name == definition.DefaultSortingColumnName)));

        public static string GetDefaultSortingDirectionTechnicalValue(this DataTableColumnsDefinition definition) =>
            definition.DefaultSortingDirection == SortingDirection.Ascending ? "asc" : "desc";
    }
}