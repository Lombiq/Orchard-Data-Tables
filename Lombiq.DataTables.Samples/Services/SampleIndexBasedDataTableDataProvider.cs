using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Samples.Indexes;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Localization;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.DataTables.Samples.Constants.ContentTypes;

namespace Lombiq.DataTables.Samples.Services;

public class SampleIndexBasedDataTableDataProvider(
    IStringLocalizer<ActionsDescriptor> actionsStringLocalizer,
    IDataTableDataProviderServices services,
    IStringLocalizer<SampleIndexBasedDataTableDataProvider> stringLocalizer) : IndexBasedDataTableDataProvider<EmployeeDataTableIndex>(services)
{
    public override LocalizedString Description => stringLocalizer["Index-based Sample Data Provider"];

    // You can provide required permissions, the viewer will need at least one to display results on the page. If it's
    // empty then no permission check is required.
    public override IEnumerable<Permission> AllowedPermissions => Enumerable.Empty<Permission>();

    // This is the method where you map your index row into a display type. Actually this result becomes JSON too, so
    // you don't have to worry about type safety as long as the names match.
    protected override async ValueTask<IEnumerable<object>> TransformAsync(IEnumerable<EmployeeDataTableIndex> rows)
    {
        // This protected method looks at the current user to see if they have delete permission for that content type.
        // It is used in the Actions dropdown menu.
        var canDelete = await CanDeleteAsync(Employee);

        var results = new List<object>();
        foreach (var row in rows)
        {
            var actions = this.GetCustomActions(
                row.ContentItemId,
                canDelete,
                _hca,
                _linkGenerator,
                actionsStringLocalizer);

            // Here we demonstrate export links. This is the way to return a cell with link text that still correctly
            // sorts and works in the Excel export too.
            var name = new ExportLink(
                url: "https://lombiq.com",
                text: row.Name,
                attributes: new Dictionary<string, string>
                {
                    ["class"] = "btn btn-primary",
                    ["role"] = "button",
                });

            // Here we demonstrate export dates. These are safe from client side time zone troubles if you don't need a
            // precise time, just a calendar day.
            var startDate = row.StartDate is { } date
                ? new ExportDate { Year = date.Year, Month = date.Month, Day = date.Day }
                : null;

            // We will return an anonymous type here which is similar to EmployeeJsonResult.
            var result = new
            {
                row.ContentItemId,
                Name = name,
                row.Position,
                row.Office,
                row.Age,
                StartDate = startDate,
                row.Salary,
                Actions = actions,
            };

            results.Add(result);
        }

        return results;
    }

    // This is the same as in SampleJsonResultDataTableDataProvider. See more comments there Normally you'd never do two
    // different types of providers for the exact same table so we don't care about DRY here.
    protected override DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) =>
        this.DefineColumns(
            nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Name),
            SortingDirection.Ascending,
            (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Name), stringLocalizer["Name"]),
            (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Position), stringLocalizer["Position"]),
            (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Office), stringLocalizer["Office"]),
            (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Age), stringLocalizer["Age"]),
            (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.StartDate), stringLocalizer["Start Date"]),
            (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Salary) + "||^||$", stringLocalizer["Salary"]),
            (GetActionsColumn(nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Actions), fromJson: true), stringLocalizer["Actions"]));

    // Once you log in as admin you can access it on the /Admin/DataTable/SampleJsonResultDataTableDataProvider or the
    // /Lombiq.DataTables.Samples/Sample/ProviderWithShape relative URLs.
}

// NEXT STATION: Startup.cs
