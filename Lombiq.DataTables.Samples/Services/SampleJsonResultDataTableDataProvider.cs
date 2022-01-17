using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.HelpfulLibraries.Libraries.Contents;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using YesSql;

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

        protected override Task<JsonResultDataTableDataProviderResult> GetResultsAsync(DataTableDataRequest request)
        {
            _session.QueryContentItem(PublicationStatus.Published)
                .Where(index => index.ContentType == request.QueryId)
        }
    }
}
