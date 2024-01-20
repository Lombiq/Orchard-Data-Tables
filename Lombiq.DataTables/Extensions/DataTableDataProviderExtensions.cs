using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Lombiq.DataTables.Constants.SortingDirection;

namespace Lombiq.DataTables.Services;

public static class DataTableDataProviderExtensions
{
    private const string Separator = "||";

    public static DataTableColumnsDefinition DefineColumns(
        this IDataTableDataProvider dataProvider,
        string sortingColumn,
        SortingDirection sortingDirection,
        params (string Name, string Text)[] columns) =>
        new()
        {
            DefaultSortingColumnName = sortingColumn,
            DefaultSortingDirection = sortingDirection,
            Columns = columns.Select(column =>
                {
                    var (name, text) = column;
                    var nameParts = name.Contains(Separator, StringComparison.Ordinal)
                        ? name.Split(Separator)
                        : new[] { name };
                    var key = nameParts[nameParts.Length == 3 ? 2 : 0];

                    var searchable = true;
                    var exportable = true;
                    var isLiquid = key.StartsWith("{{", StringComparison.Ordinal) ||
                                   key.StartsWith("{%", StringComparison.Ordinal);

                    if (isLiquid)
                    {
                        // Don't search if it is a liquid expression, also don't export if it's "actions".
                        searchable = false;
                        exportable = !key.Contains("actions:", StringComparison.InvariantCultureIgnoreCase);
                    }
                    else if (nameParts[0].EndsWith("DateUtc", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchable = false;
                    }

                    return new DataTableColumnDefinition
                    {
                        DataSource = dataProvider.Name,
                        Name = nameParts[0],
                        Text = text,
                        Regex = nameParts.Length == 3 ? (nameParts[1], nameParts[2]) : null,
                        Searchable = searchable,
                        Exportable = exportable,
                        IsLiquid = isLiquid,
                    };
                })
                .ToArray(),
        };

    /// <summary>
    /// Creates columns definition using tuple parameters.
    /// </summary>
    /// <param name="dataProvider">The data provider to create for.</param>
    /// <param name="columns">
    /// Tuples each describing a column. They must be (string Name, string Text, bool Searchable, bool Exportable) with
    /// the last 2 being optional.
    /// </param>
    /// <returns>The generated columns definition.</returns>
    public static DataTableColumnsDefinition DefineColumns(
        this IDataTableDataProvider dataProvider,
        params (string Name, string Text)[] columns) =>
        DefineColumns(
            dataProvider,
            columns[0].Name.Split(Separator)[0],
            Ascending,
            columns);

    /// <summary>
    /// Checks if the user can be authorized against the dataProvider.
    /// </summary>
    /// <param name="dataProvider">Supplies the acceptable <see cref="Permission"/>s.</param>
    /// <param name="authorizationService">Authorizes the user.</param>
    /// <param name="user">The user to check.</param>
    /// <returns>
    /// <see langword="true"/> if the user has at least one of the <see cref="Permission"/> s given by the <paramref
    /// name="dataProvider"/> or if its <see cref="IDataTableDataProvider.AllowedPermissions"/> is null or empty.
    /// </returns>
    public static async Task<bool> AuthorizeAsync(
        this IDataTableDataProvider dataProvider,
        IAuthorizationService authorizationService,
        ClaimsPrincipal user)
    {
        if (dataProvider.AllowedPermissions == null)
        {
            throw new InvalidOperationException(
                $"Please provide a collection of {nameof(Permission)}s in the {dataProvider.GetType().Name}." +
                $"{nameof(dataProvider.AllowedPermissions)} property. It can be empty too, but not null.");
        }

        var hasFailedPermissions = false;
        foreach (var permission in dataProvider.AllowedPermissions)
        {
            if (await authorizationService.AuthorizeAsync(user, permission)) return true;
            hasFailedPermissions = true;
        }

        return !hasFailedPermissions;
    }

    public static ActionsDescriptor GetCustomActions(
        this IDataTableDataProvider dataProvider,
        string contentItemId,
        bool canDelete,
        IHttpContextAccessor hca,
        LinkGenerator linkGenerator,
        IStringLocalizer<ActionsDescriptor> actionsStringLocalizer)
    {
        var returnUrl = linkGenerator.GetPathByAction(
            hca.HttpContext,
            nameof(TableController.Get),
            typeof(TableController).ControllerName(),
            new { providerName = dataProvider.GetType().Name });

        var menuItems = new List<ExportLink>
        {
            ActionsDescriptor.GetEditLink(
                contentItemId,
                hca.HttpContext,
                linkGenerator,
                actionsStringLocalizer,
                returnUrl),
        };

        if (canDelete)
        {
            menuItems.Add(ActionsDescriptor.GetRemoveLink(
                contentItemId,
                hca.HttpContext,
                linkGenerator,
                actionsStringLocalizer,
                returnUrl));
        }

        return new ActionsDescriptor
        {
            Id = contentItemId,
            MenuItems = menuItems,
            WithDefaults = false,
        };
    }

    public static string GetCustomActionsJson(
        this IDataTableDataProvider dataProvider,
        string contentItemId,
        bool canDelete,
        IHttpContextAccessor hca,
        LinkGenerator linkGenerator,
        IStringLocalizer<ActionsDescriptor> actionsStringLocalizer) =>
        JsonConvert.SerializeObject(GetCustomActions(
            dataProvider,
            contentItemId,
            canDelete,
            hca,
            linkGenerator,
            actionsStringLocalizer));
}
