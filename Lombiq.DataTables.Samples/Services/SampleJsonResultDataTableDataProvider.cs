using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Samples.Models;
using Lombiq.DataTables.Services;
using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using static Lombiq.DataTables.Samples.Constants.ContentTypes;

namespace Lombiq.DataTables.Samples.Services;

/// <summary>
/// This is a data provider that can return a page of data from any available source. Also see <see
/// cref="DeletedContentItemDataTableDataProvider"/> for a practical example that uses a LinqToDB query.
/// </summary>
public class SampleJsonResultDataTableDataProvider(
    ISession session,
    IDataTableDataProviderServices services,
    IStringLocalizer<ActionsDescriptor> actionsStringLocalizer,
    IStringLocalizer<SampleJsonResultDataTableDataProvider> implementationStringLocalizer) : JsonResultDataTableDataProvider(services, implementationStringLocalizer)
{
    private readonly IStringLocalizer T = implementationStringLocalizer;

    // This value is displayed in the Excel export and the /Admin/DataTable/{providerName} page.
    public override LocalizedString Description => T["JSON-based Sample Data Provider"];

    // You can provide required permissions, the viewer will need at least one to display results on the page. If it's
    // empty then no permission check is required.
    public override IEnumerable<Permission> AllowedPermissions => Enumerable.Empty<Permission>();

    // You can inject shapes before or after the table using this and GetShapesAfterTableAsync(). It's used by the
    // /Admin/DataTable/{providerName} page.
    public override async Task<IEnumerable<IShape>> GetShapesBeforeTableAsync() => new[]
    {
        await _shapeFactory.CreateAsync("ShapeBeforeExample"),
    };

    // You must override this method and return a page of data based on the request.
    protected override async Task<JsonResultDataTableDataProviderResult> GetResultsAsync(DataTableDataRequest request)
    {
        var query = session.QueryContentItem(PublicationStatus.Published)
            .Where(index => index.ContentType == Employee);
        var totalCount = await query.CountAsync();

        // For the sake of simplicity, we only make this one column searchable. With a content part index you can search
        // all the relevant fields on the database side. Otherwise you'd need to do the filtering offline after
        // downloading all possible rows from the database server without pagination. That can be very slow on larger
        // tables so avoid it if possible. If you need to filter by some computed data, consider using an
        // IndexBasedDataTableDataProvider instead so you can prepare the calculations in a separate index when the
        // backing data changes.
        var isFiltered = request.HasSearch;
        if (isFiltered) query = query.Where(index => index.DisplayText.Contains(request.Search.Value));
        var filteredCount = isFiltered ? await query.CountAsync() : -1;

        // For the same reason we can only paginate on the SQL side if it's sorted by name. This is why, unless you have
        // use for content part indexes anyway it's best to use IndexBasedDataTableDataProvider for large data. The
        // query needs to be ordered before it can be reliably paginated; you may need to branch this for all orderable
        // columns.
        var isPaginated = request.Order.FirstOrDefault()?.Column == nameof(EmployeeJsonResult.Name);
        if (isPaginated)
        {
            query = request.Order.First().IsAscending
                ? query.OrderBy(index => index.DisplayText)
                : query.OrderByDescending(index => index.DisplayText);
        }

        // We have this helper method to avoid confusion because DataTables and YesSql describes slices differently.
        var results = (isPaginated ? await PaginateAsync(query, request) : await query.ListAsync())
            // The result will be converted into JSON so it's a good practice to strip anything unneeded to save
            // bandwidth. Also you may have cyclic references in your results which this eliminates.
            .Select(contentItem => contentItem.As<EmployeePart>())
            .Select(part => new EmployeeJsonResult
            {
                ContentItemId = part.ContentItem.ContentItemId,
                Name = part.Name.Text,
                Position = part.Position.Text,
                Office = part.Office.Text,
                Age = part.Age.Value,
                StartDate = part.StartDate.Value,
                Salary = part.Salary.Value,
                Actions = this.GetCustomActions(
                    part.ContentItem.ContentItemId,
                    canDelete: true,
                    _hca,
                    _linkGenerator,
                    actionsStringLocalizer),
            });

        // If you didn't filter or paginate (sufficient for known very small datasets) you can just pass the
        // constructor. Otherwise, make sure to fill all properties too.
        return new JsonResultDataTableDataProviderResult(results)
        {
            IsFiltered = isFiltered,
            IsPaginated = isPaginated,

            // To get an accurate total even when filtering, we need the counts of the query before and after filtering,
            // always before pagination. To save time, you can omit the filtered query when there was no search and
            // return -1 instead. The presence of a positive number is what tells DataTables on the client-side that
            // it's a filtered result set.
            Count = totalCount,
            CountFiltered = filteredCount,
        };
    }

    // You also need to override GetColumnsDefinitionAsync or GetColumnsDefinitionInner to provide the column headers.
    // The latter is ideal in most cases, but the async version is available in case you need to fetch a site setting or
    // database value before providing your columns.
    // The this.DefineColumns extension lets you simply provide a default sorting column, sorting direction and an array
    // of property name-display text pairs. However you are free to make your DataTableColumnsDefinition from scratch if
    // you need it.
    // You can also amend a regex search-replace pattern which will be applied to every cell in that column. This is
    // also a good way to inject Liquid expressions into the value, which are evaluated on the server side before the
    // page is returned. A typical use case for that is the content item actions dropdown, we have a prebuilt method for
    // that.
    protected override DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) =>
        this.DefineColumns(
            nameof(EmployeeJsonResult.Name),
            SortingDirection.Ascending,
            (nameof(EmployeeJsonResult.Name), T["Name"]),
            (nameof(EmployeeJsonResult.Position), T["Position"]),
            (nameof(EmployeeJsonResult.Office), T["Office"]),
            (nameof(EmployeeJsonResult.Age), T["Age"]),
            (nameof(EmployeeJsonResult.StartDate), T["Start Date"]),
            (nameof(EmployeeJsonResult.Salary) + "||^||$", T["Salary"]),
            (GetActionsColumn(nameof(EmployeeJsonResult.Actions), fromJson: true), T["Actions"]));

    public class EmployeeJsonResult
    {
        public string ContentItemId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Office { get; set; }
        public decimal? Age { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? Salary { get; set; }
        public ActionsDescriptor Actions { get; set; }
    }

    // Once you log in as admin you can access it on the /Admin/DataTable/SampleJsonResultDataTableDataProvider or the
    // /Lombiq.DataTables.Samples/Sample/ProviderWithShape relative URLs. We'll check out the latter next.
}

// Before you go, check it out on /Admin/DataTable/SampleJsonResultDataTableDataProvider too!

// NEXT STATION: Views/Sample/ProviderWithShape.cshtml
