using Dapper;
using Lombiq.DataTables.Services;
using Lombiq.HelpfulLibraries.OrchardCore.Data;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql.Indexes;
using ISession = YesSql.ISession;

namespace Lombiq.DataTables.Handlers;

public class DataTableIndexGeneratorContentHandler<TIndexGenerator, TIndex>(
    Lazy<IHttpContextAccessor> hcaLazy,
    Lazy<IManualConnectingIndexService<TIndex>> indexServiceLazy,
    Lazy<ISession> sessionLazy,
    Lazy<TIndexGenerator> indexGeneratorLazy)
    : ContentHandlerBase, IManualDataTableIndexGenerator
    where TIndexGenerator : IDataTableIndexGenerator
    where TIndex : MapIndex
{
    public bool IsInMiddlewarePipeline { get; set; }

    public override Task CreatedAsync(CreateContentContext context) => ReserveIndexGenerationAsync(context);

    public override Task UpdatedAsync(UpdateContentContext context) => ReserveIndexGenerationAsync(context);

    public override Task ImportedAsync(ImportContentContext context) => ReserveIndexGenerationAsync(context);

    public override Task PublishedAsync(PublishContentContext context) => ReserveIndexGenerationAsync(context);

    public override Task UnpublishedAsync(PublishContentContext context) => ReserveIndexGenerationAsync(context);

    public override Task RemovedAsync(RemoveContentContext context) => ReserveIndexGenerationAsync(context);

    public async Task GenerateOrderedIndicesAsync()
    {
        var indexGenerator = indexGeneratorLazy.Value;

        if (!indexGenerator.IndexGenerationIsRemovalByType.Any()) return;
        await indexGenerator.GenerateIndexAsync();

        // Clear out any deleted items. We use raw queries for quick communication between SQL and ASP.NET servers.
        await RemoveInvalidAsync();
    }

    public Task ScheduleDeferredIndexGenerationAsync(ContentItem contentItem, bool managedTypeOnly) =>
        indexGeneratorLazy.Value.ManagedContentTypes.Contains(contentItem.ContentType)
            ? ReserveIndexGenerationAsync(new UpdateContentContext(contentItem))
            : Task.CompletedTask;

    private async Task ReserveIndexGenerationAsync(ContentContextBase context)
    {
        var generator = indexGeneratorLazy.Value;
        var contentItem = context.ContentItem;
        var isRemove = generator.ManagedContentTypes.Contains(contentItem.ContentType) && context.IsRemove();

        if (!await generator.NeedsUpdatingAsync(context)) return;
        if (!IsInMiddlewarePipeline) await sessionLazy.Value.FlushAsync();
        await generator.ScheduleDeferredIndexGenerationAsync(contentItem, isRemove);

        // The middlewares don't execute during setup so we have to update here.
        if (!IsInMiddlewarePipeline && generator.IndexGenerationIsRemovalByType.Any())
        {
            await GenerateOrderedIndicesAsync();
        }
    }

    private async Task RemoveInvalidAsync()
    {
        // Using very raw query because it's too complex for the parser.
        var session = sessionLazy.Value;
        var transaction = await session.BeginTransactionAsync();
        var dialect = session.Store.Configuration.SqlDialect;
        var prefix = session.Store.Configuration.TablePrefix;
        var schema = session.Store.Configuration.Schema;

        var contentItemIndex = dialect.QuoteForTableName(prefix + nameof(ContentItemIndex), schema);
        var dataTableIndex = dialect.QuoteForTableName(prefix + typeof(TIndex).Name, schema);

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

        // OrchardCore.AuditTrail compatibility check. As it just uses the regular Orchard Core facilities there is no
        // need to make it a dependency.
        var restored = hcaLazy.Value.HttpContext?.Items.GetMaybe("OrchardCore.AuditTrail.Restored");
        if (restored is ContentItem { ContentItemId: { } } restoredContentItem)
        {
            invalidIds = invalidIds.Where(id => id != restoredContentItem.Id);
        }

        foreach (var invalidId in invalidIds) await indexServiceLazy.Value.RemoveByIndexAsync(invalidId, session);
    }
}
