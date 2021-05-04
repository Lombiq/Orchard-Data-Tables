using Dapper;
using Finitive.CommercialEntities.Handlers;
using Lombiq.DataTables.Services;
using Lombiq.HelpfulLibraries.Libraries.Database;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Records;
using System;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;
using ISession = YesSql.ISession;

namespace Lombiq.DataTables.Handlers
{
    public class DataTableIndexGeneratorContentHandler<TIndexGenerator, TIndex>
        : ContentHandlerBase, IManualDataTableIndexGenerator
        where TIndexGenerator : IDataTableIndexGenerator<TIndex>
        where TIndex : MapIndex
    {
        private readonly Lazy<IHttpContextAccessor> _hcaLazy;
        private readonly Lazy<IManualConnectingIndexService<TIndex>> _indexServiceLazy;
        private readonly Lazy<ISession> _sessionLazy;
        private readonly Lazy<TIndexGenerator> _indexGeneratorLazy;

        public DataTableIndexGeneratorContentHandler(
            Lazy<IHttpContextAccessor> hcaLazy,
            Lazy<IManualConnectingIndexService<TIndex>> indexServiceLazy,
            Lazy<ISession> sessionLazy,
            Lazy<TIndexGenerator> indexGeneratorLazy)
        {
            _hcaLazy = hcaLazy;
            _indexServiceLazy = indexServiceLazy;
            _sessionLazy = sessionLazy;
            _indexGeneratorLazy = indexGeneratorLazy;
        }

        public override Task CreatedAsync(CreateContentContext context) => GenerateIndicesAsync(context);
        public override Task UpdatedAsync(UpdateContentContext context) => GenerateIndicesAsync(context);
        public override Task ImportedAsync(ImportContentContext context) => GenerateIndicesAsync(context);
        public override Task PublishedAsync(PublishContentContext context) => GenerateIndicesAsync(context);
        public override Task UnpublishedAsync(PublishContentContext context) => GenerateIndicesAsync(context);
        public override Task RemovedAsync(RemoveContentContext context) => GenerateIndicesAsync(context);

        public Task GenerateIndicesAsync(ContentItem contentItem, bool managedTypeOnly) =>
            _indexGeneratorLazy.Value.ManagedContentType.Contains(contentItem.ContentType)
                ? GenerateIndicesAsync(new UpdateContentContext(contentItem))
                : Task.CompletedTask;

        private async Task GenerateIndicesAsync(ContentContextBase context)
        {
            var generator = _indexGeneratorLazy.Value;
            var contentItem = context.ContentItem;
            var isRemove = generator.ManagedContentType.Contains(contentItem.ContentType) && context.IsRemove();

            if (!await generator.NeedsUpdatingAsync(context)) return;
            await _sessionLazy.Value.FlushAsync(); // This was only necessary during setup.
            await generator.GenerateIndexAsync(contentItem, isRemove);

            // OrchardCore.AuditTrail compatibility check. As it just uses the regular Orchard Core facilities there is
            // no need to make it a dependency.
            var httpContext = _hcaLazy.Value.HttpContext;
            var isRestored =
                httpContext != null &&
                httpContext.Items.TryGetValue("OrchardCore.AuditTrail.Restored", out var contentItemObject) &&
                (contentItemObject as ContentItem)?.ContentItemId == context.ContentItem.ContentItemId;
            if (isRestored) return;

            // Clear out any deleted items. We use raw queries for quick communication between SQL and ASP.Net servers.
            await RemoveInvalidAsync();
        }

        private async Task RemoveInvalidAsync()
        {
            // Using very raw query because it's too complex for the parser.
            var session = _sessionLazy.Value;
            var transaction = await session.DemandAsync();
            var dialect = TransactionSqlDialectFactory.For(transaction);
            var prefix = session.Store.Configuration.TablePrefix;

            var contentItemIndex = dialect.QuoteForTableName(prefix + nameof(ContentItemIndex));
            var dataTableIndex = dialect.QuoteForTableName(prefix + typeof(TIndex).Name);

            const string documentId = "DocumentId";
            const string contentItemId = nameof(ContentItemIndex.ContentItemId);
            const string latest = nameof(ContentItemIndex.Latest);
            const string published = nameof(ContentItemIndex.Published);

            var deletedSql = @$"
            SELECT old.{documentId}
                FROM (
                    SELECT {documentId}, {contentItemId}
                        FROM {contentItemIndex}
                        WHERE {latest} = 0 AND {published} = 0) old
                LEFT JOIN (
                    SELECT {contentItemId}
                        FROM {contentItemIndex}
                        WHERE {latest} = 1
                        GROUP BY {contentItemId}) new
                    ON old.{contentItemId} = new.{contentItemId}
                INNER JOIN {dataTableIndex} dataTable
                    ON old.{documentId} = dataTable.{documentId}
                WHERE new.{contentItemId} IS NULL";
            var invalidIds = await transaction.Connection.QueryAsync<int>(deletedSql, transaction: transaction);

            foreach (var invalidId in invalidIds) await _indexServiceLazy.Value.RemoveByIndexAsync(invalidId, session);
        }
    }
}
