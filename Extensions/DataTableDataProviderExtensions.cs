using System.Linq;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using static Lombiq.DataTables.Constants.SortingDirection;

namespace Lombiq.DataTables.Services
{
    public static class DataTableDataProviderExtensions
    {
        public static DataTableColumnsDefinition DefineColumns(this IDataTableDataProvider self,
            string sortingColumn,
            SortingDirection sortingDirection,
            params (string Name, string Text)[] columns) =>
            new DataTableColumnsDefinition
            {
                DefaultSortingColumnName = sortingColumn,
                DefaultSortingDirection = sortingDirection,
                Columns = columns.Select(x =>
                    {
                        var nameParts = x.Name.Contains('|') ? x.Name.Split('|') : new[] { x.Name };
                        return new DataTableColumnDefinition
                        {
                            DataSource = self.Name,
                            Name = nameParts[0],
                            Text = x.Text,
                            Regex = nameParts.Length == 3 ? (nameParts[1], nameParts[2]) as (string, string)? : null,
                        };
                    })
                    .ToArray()
            };

        public static DataTableColumnsDefinition DefineColumns(this IDataTableDataProvider self,
            params (string Name, string Text)[] columns) =>
            DefineColumns(self, columns[0].Name, Ascending, columns);
    }
}
