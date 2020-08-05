using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Nito.AsyncEx;
using static Lombiq.DataTables.Constants.SortingDirection;

namespace Lombiq.DataTables.Services
{
    public static class DataTableDataProviderExtensions
    {
        public static DataTableColumnsDefinition DefineColumns(this IDataTableDataProvider dataProvider,
            string sortingColumn,
            SortingDirection sortingDirection,
            params (string Name, string Text)[] columns) =>
            new DataTableColumnsDefinition
            {
                DefaultSortingColumnName = sortingColumn,
                DefaultSortingDirection = sortingDirection,
                Columns = columns.Select(column =>
                    {
                        var nameParts = column.Name.Contains('|') ? column.Name.Split('|') : new[] { column.Name };
                        return new DataTableColumnDefinition
                        {
                            DataSource = dataProvider.Name,
                            Name = nameParts[0],
                            Text = column.Text,
                            Regex = nameParts.Length == 3 ? (nameParts[1], nameParts[2]) as (string, string)? : null,
                        };
                    })
                    .ToArray()
            };

        public static DataTableColumnsDefinition DefineColumns(this IDataTableDataProvider dataProvider,
            params (string Name, string Text)[] columns) =>
            DefineColumns(dataProvider, columns[0].Name, Ascending, columns);

        public static async Task<bool> Authorize(
            this IDataTableDataProvider dataProvider,
            IAuthorizationService authorizationService,
            ClaimsPrincipal user)
        {
            if (dataProvider.SupportedPermissions == null) return true;

            var authorizations = await dataProvider.SupportedPermissions
                .Select(permission => authorizationService.AuthorizeAsync(user, permission))
                .WhenAll();
            return authorizations.Any(success => success);

        }
    }
}
