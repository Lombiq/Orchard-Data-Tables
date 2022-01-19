using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Samples.Indexes;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.DataTables.Samples.Constants.ContentTypes;

namespace Lombiq.DataTables.Samples.Services
{
    public class SampleIndexBasedDataTableDataProvider : IndexBasedDataTableDataProvider<EmployeeDataTableIndex>
    {
        private readonly IStringLocalizer<ActionsDescriptor> _actionsStringLocalizer;
        private readonly IStringLocalizer<SampleIndexBasedDataTableDataProvider> T;

        public override LocalizedString Description => T["Index based Sample Data Provider"];

        public SampleIndexBasedDataTableDataProvider(
            IStringLocalizer<ActionsDescriptor> actionsStringLocalizer,
            IDataTableDataProviderServices services,
            IStringLocalizer<SampleIndexBasedDataTableDataProvider> stringLocalizer)
            : base(services)
        {
            _actionsStringLocalizer = actionsStringLocalizer;
            T = stringLocalizer;
        }

        // This is the method where you map your index row into a display type. Actually this result becomes JSON too,
        // so you don't have to worry about type safety as long as the names match.
        protected override async ValueTask<IEnumerable<object>> TransformAsync(IEnumerable<EmployeeDataTableIndex> rows)
        {
            // This protected method looks at the current user to see if they have delete permission for that content
            // type. It is used in the Actions dropdown menu.
            var canDelete = await CanDeleteAsync(Employee);

            var results = new List<object>();
            foreach (var row in rows)
            {
                var actions = this.GetCustomActions(
                    row.ContentItemId,
                    canDelete,
                    _hca,
                    _linkGenerator,
                    _actionsStringLocalizer);

                // Here we demonstrate export links. This is the way to return a cell with link text that still
                // correctly sorts and works in the Excel export too.
                var name = new ExportLink(
                    url: "https://www.lombiq.com",
                    text: row.Name,
                    attributes: new Dictionary<string, string>
                    {
                        ["class"] = "btn btn-primary",
                        ["role"] = "button",
                    });

                // Here we demonstrate export dates. These are safe from client side time zone troubles if you don't
                // need a precise time, just a calendar day.
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

        // This is the same as in SampleJsonResultDataTableDataProvider. See more comments there Normally you'd never do
        // two different types of providers for the exact same table so we don't care about DRY here.
        protected override DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) =>
            this.DefineColumns(
                nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Name),
                SortingDirection.Ascending,
                (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Name), T["Name"]),
                (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Position), T["Position"]),
                (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Office), T["Office"]),
                (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Age), T["Age"]),
                (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.StartDate), T["Start Date"]),
                (nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Salary) + "||^||$", T["Salary"]),
                (GetActionsColumn(nameof(SampleJsonResultDataTableDataProvider.EmployeeJsonResult.Actions), fromJson: true), T["Actions"]));
    }
}

// NEXT STATION: Startup.cs
