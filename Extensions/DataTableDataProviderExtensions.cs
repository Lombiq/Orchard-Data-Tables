using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Nito.AsyncEx;
using OrchardCore.Security.Permissions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Lombiq.DataTables.Constants.SortingDirection;

namespace Lombiq.DataTables.Services
{
    public static class DataTableDataProviderExtensions
    {
        public static DataTableColumnsDefinition DefineColumns(
            this IDataTableDataProvider dataProvider,
            string sortingColumn,
            SortingDirection sortingDirection,
            params (string Name, string Text)[] columns) =>
            new DataTableColumnsDefinition
            {
                DefaultSortingColumnName = sortingColumn,
                DefaultSortingDirection = sortingDirection,
                Columns = columns.Select(column =>
                    {
                        var (name, text) = column;
                        var nameParts = name.Contains('|') ? name.Split('|') : new[] { name };
                        var key = nameParts[nameParts.Length == 3 ? 2 : 0];

                        var searchable = true;
                        var exportable = true;
                        var isLiquid = key.StartsWith("{{") || key.StartsWith("{%");

                        if (isLiquid)
                        {
                            // Don't search if it is a liquid expression, also don't export if it's "actions".
                            searchable = false;
                            exportable = !key.Contains("actions:");
                        }
                        else if (nameParts[0].EndsWith("DateUtc"))
                        {
                            searchable = false;
                        }

                        return new DataTableColumnDefinition
                        {
                            DataSource = dataProvider.Name,
                            Name = nameParts[0],
                            Text = text,
                            Regex = nameParts.Length == 3 ? (nameParts[1], nameParts[2]) as (string, string)? : null,
                            Searchable = searchable,
                            Exportable = exportable,
                            IsLiquid = isLiquid
                        };
                    })
                    .ToArray()
            };

        /// <summary>
        /// Creates columns definition using tuple parameters.
        /// </summary>
        /// <param name="dataProvider">The data provider to create for.</param>
        /// <param name="columns">
        /// Tuples each describing a column. They must be (string Name, string Text, bool Searchable, bool Exportable)
        /// with the last 2 being optional.
        /// </param>
        /// <returns>The generated columns definition</returns>
        public static DataTableColumnsDefinition DefineColumns(
            this IDataTableDataProvider dataProvider,
            params (string Name, string Text)[] columns) =>
            DefineColumns(dataProvider, columns[0].Name, Ascending, columns);

        /// <summary>
        /// Checks if the <see cref="user"/> can be authorized against the <see cref="dataProvider"/>.
        /// </summary>
        /// <param name="dataProvider">Supplies the acceptable <see cref="Permission"/>s.</param>
        /// <param name="authorizationService">Authorizes the user.</param>
        /// <param name="user">The user to check.</param>
        /// <returns>
        /// True if the user has at least one of the <see cref="Permission"/>s given by the <see cref="dataProvider"/>.
        /// </returns>
        public static async Task<bool> Authorize(
            this IDataTableDataProvider dataProvider,
            IAuthorizationService authorizationService,
            ClaimsPrincipal user)
        {
            if (dataProvider.SupportedPermissions == null) return false;

            var authorizations = await dataProvider.SupportedPermissions
                .Select(permission => authorizationService.AuthorizeAsync(user, permission))
                .WhenAll();

            return authorizations.Any(success => success);
        }
    }
}
