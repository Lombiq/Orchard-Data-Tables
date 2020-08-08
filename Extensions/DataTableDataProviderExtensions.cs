using System;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Nito.AsyncEx;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Lombiq.DataTables.Constants.SortingDirection;

namespace Lombiq.DataTables.Services
{
    public static class DataTableDataProviderExtensions
    {
        public static DataTableColumnsDefinition DefineColumns(this IDataTableDataProvider dataProvider,
            string sortingColumn,
            SortingDirection sortingDirection,
            params object[] columns) =>
            new DataTableColumnsDefinition
            {
                DefaultSortingColumnName = sortingColumn,
                DefaultSortingDirection = sortingDirection,
                Columns = columns.Select(column =>
                    {
                        var (name, text, searchable, exportable) = ToColumnTuple(column);
                        var nameParts = name.Contains('|') ? name.Split('|') : new[] { name };

                        return new DataTableColumnDefinition
                        {
                            DataSource = dataProvider.Name,
                            Name = nameParts[0],
                            Text = text,
                            Regex = nameParts.Length == 3 ? (nameParts[1], nameParts[2]) as (string, string)? : null,
                            Searchable = searchable,
                            Exportable = exportable
                        };
                    })
                    .ToArray()
            };

        public static DataTableColumnsDefinition DefineColumns(this IDataTableDataProvider dataProvider,
            params object[] columns) =>
            DefineColumns(dataProvider, ToColumnTuple(columns[0]).Item1, Ascending, columns);

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


        private static (string, string, bool, bool) ToColumnTuple(object column)
        {
            string name, text;
            bool searchable = true, exportable = true;
            if (column is ValueTuple<string, string, bool, bool> tuple4)
            {
                (name, text, searchable, exportable) = tuple4;
            }
            else if (column is ValueTuple<string, string, bool> tuple3)
            {
                (name, text, searchable) = tuple3;
            }
            else if (column is ValueTuple<string, string> tuple2)
            {
                (name, text) = tuple2;
            }
            else throw new ArgumentException("The argument 'columns' must be (string, string, bool, bool)");

            return (name, text, searchable, exportable);
        }
    }
}
