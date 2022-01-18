using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.HelpfulLibraries.Libraries.Contents;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using YesSql;
using static Lombiq.DataTables.Samples.Constants.ContentTypes;

namespace Lombiq.DataTables.Samples.Services
{
    /// <summary>
    /// This is a data provider that can return a page of data from any available source. Also see <see
    /// cref="DeletedContentItemDataTableDataProvider"/> for a practical example that uses a LinqToDB query.
    /// </summary>
    public class SampleJsonResultDataTableDataProvider : JsonResultDataTableDataProvider
    {
        private readonly ISession _session;
        private readonly IStringLocalizer T;

        public override LocalizedString Description => T["Sample Data Provider"];

        public SampleJsonResultDataTableDataProvider(
            ISession session,
            IDataTableDataProviderServices services,
            IStringLocalizer implementationStringLocalizer)
            : base(services, implementationStringLocalizer)
        {
            _session = session;
            T = implementationStringLocalizer;
        }

        // You must override this method and return a page of data based on the request.
        protected override async Task<JsonResultDataTableDataProviderResult> GetResultsAsync(DataTableDataRequest request)
        {
            var query = _session.QueryContentItem(PublicationStatus.Published)
                .Where(index => index.ContentType == Employee);
            var allContentsQuery = query; // Without filtering or pagination.

            // For the sake of simplicity, we only make this one column searchable. With a content part index you can
            // search all the relevant fields on the database side. Otherwise you'd need to do the filtering offline
            // after downloading all possible rows from the database server without pagination. That can be very slow on
            // larger tables so avoid it if possible. If you need to filter by some computed data, consider using an
            // IndexBasedDataTableDataProvider instead so you can prepare the calculations in a separate index when the
            // backing data changes.
            var isFiltered = request.HasSearch;
            if (isFiltered) query = query.Where(index => index.DisplayText.Contains(request.Search.Value));
            var filteredQuery = query;

            // We have this helper method to avoid confusion because DataTables and YesSql describes slices differently.
            var results = await PaginateAsync(query, request);

            // To get an accurate total even when filtering, we need the counts of the query before and after filtering,
            // always before pagination. To save time, you can omit the filtered query when there was no search and
            // return -1 instead. The presence of a positive number is what tells DataTables on the client side that
            // it's a filtered result set.
            var totalCount = await allContentsQuery.CountAsync();
            var filteredCount = isFiltered ? await filteredQuery.CountAsync() : -1;

            // If you didn't filter or paginate (sufficient for known very small datasets) you can just pass the
            // constructor. Otherwise, make sure to fill all properties too.
            return new JsonResultDataTableDataProviderResult(results)
            {
                IsFiltered = isFiltered,
                IsPaginated = true,
                Count = totalCount,
                CountFiltered = filteredCount,
            };
        }
    }
}
