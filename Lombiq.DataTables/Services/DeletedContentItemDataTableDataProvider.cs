using LinqToDB;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Models;
using Lombiq.HelpfulLibraries.Libraries.DependencyInjection;
using Lombiq.HelpfulLibraries.LinqToDb;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Contents;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.DataTables.Services
{
    /// <summary>
    /// This is a data provider which returns the list of deleted items with some <see cref="ContentItem"/> information.
    /// </summary>
    public class DeletedContentItemDataTableDataProvider : JsonResultDataTableDataProvider
    {
        private readonly ISession _session;
        private readonly IStringLocalizer T;

        public DeletedContentItemDataTableDataProvider(
            IOrchardServices<DeletedContentItemDataTableDataProvider> orchardServices,
            IDataTableDataProviderServices services)
            : base(services, orchardServices.StringLocalizer.Value)
        {
            _session = orchardServices.Session.Value;
            T = orchardServices.StringLocalizer.Value;
        }

        public override LocalizedString Description => T["Deleted Content Items"];

        public override IEnumerable<Permission> AllowedPermissions =>
            new[] { Permissions.DeleteContent };

        protected override async Task<JsonResultDataTableDataProviderResult> GetResultsAsync(DataTableDataRequest request) =>
            new(await GetDeletedContentItemIndicesAsync(_session, request.QueryId));

        protected override DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) =>
            this.DefineColumns(
                nameof(ContentItemIndex.Id),
                SortingDirection.Descending,
                (nameof(ContentItemIndex.Id), T["ID"]),
                (nameof(ContentItemIndex.ContentItemId), T["Content Item ID"]),
                (nameof(ContentItemIndex.ContentItemVersionId), T["Content Item Version ID"]),
                (nameof(ContentItemIndex.DisplayText), T["Title"]),
                (nameof(ContentItemIndex.Author), T["Author"]),
                (nameof(ContentItemIndex.Owner), T["Owner"]),
                (nameof(ContentItemIndex.CreatedUtc), T["Created Date"]),
                (nameof(ContentItemIndex.ModifiedUtc), T["Modified Date"]),
                (nameof(ContentItemIndex.PublishedUtc), T["Deleted Date"]));

        public static NavigationItemBuilder AddMenuItem(
            NavigationItemBuilder builder,
            string queryId) =>
            builder
                .AddClass("deletedContent" + queryId)
                .Action(
                    nameof(TableController.Get),
                    typeof(TableController).ControllerName(),
                    new
                    {
                        area = FeatureIds.DataTables,
                        providerName = nameof(DeletedContentItemDataTableDataProvider),
                        queryId,
                    })
                .Permission(Permissions.DeleteContent)
                .LocalNav();

        /// <summary>
        /// Returns the list of each deleted <see cref="ContentItem"/>'s index for the specified content type.
        /// </summary>
        public static async Task<IEnumerable<ContentItemIndex>> GetDeletedContentItemIndicesAsync(
            ISession session,
            string contentType)
        {
            // This returns the indices which don't have a published or draft content item version. Still needs to be
            // grouped offline to only select the latest one.
            Task<ContentItemIndex[]> SqlQuery(ITableAccessor accessor) =>
                (from deleted in accessor.GetTable<ContentItemIndex>()
                 from notDeleted in
                     (from contentItemIndex in accessor.GetTable<ContentItemIndex>()
                      where contentItemIndex.ContentType == contentType
                         && (contentItemIndex.Latest || contentItemIndex.Published)
                      group contentItemIndex by contentItemIndex.ContentItemId into contentItemIdGroup
                      select contentItemIdGroup.Key)
                      .LeftJoin(notDeleted => notDeleted == deleted.ContentItemId)
                 where deleted.ContentType == contentType && notDeleted == null
                 select deleted)
                .ToArrayAsync();

            var result = await session.LinqQueryAsync(SqlQuery);

            return result
                .GroupBy(index => index.ContentItemId)
                .Select(group => group
                    .OrderBy(index => string.IsNullOrWhiteSpace(index.DisplayText) ? 1 : 0)
                    .ThenByDescending(index => index.PublishedUtc)
                    .First());
        }
    }
}
