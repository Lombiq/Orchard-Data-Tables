using Lombiq.HelpfulLibraries.OrchardCore.Data;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;

namespace Lombiq.DataTables.Services;

/// <summary>
/// A base class for implementing <see cref="IDataTableIndexGenerator{TIndex}"/>, a service for generating DataTables
/// indexes for a specific table.
/// </summary>
/// <typeparam name="TIndex">The type of <see cref="MapIndex"/> this service maintains.</typeparam>
public abstract class DataTableIndexGeneratorBase<TIndex>
    : IDataTableIndexGenerator<TIndex>
    where TIndex : MapIndex
{
    protected readonly IContentManager _contentManager;
    protected readonly IManualConnectingIndexService<TIndex> _indexService;
    protected readonly ISession _session;

    /// <summary>
    /// Gets the name of the property which represents the <see cref="ContentItem.ContentItemId"/> column in
    /// <typeparamref name="TIndex"/>.
    /// </summary>
    protected abstract string IdColumnName { get; }

    public IDictionary<string, bool> IndexGenerationIsRemovalByType { get; } = new Dictionary<string, bool>();
    public abstract IEnumerable<string> ManagedContentTypes { get; }

    protected DataTableIndexGeneratorBase(
        IContentManager contentManager,
        IManualConnectingIndexService<TIndex> indexService,
        ISession session)
    {
        _contentManager = contentManager;
        _indexService = indexService;
        _session = session;
    }

    public virtual ValueTask<bool> NeedsUpdatingAsync(ContentContextBase context) =>
        new(ManagedContentTypes.Contains(context.ContentItem.ContentType));

    public abstract Task ScheduleDeferredIndexGenerationAsync(ContentItem contentItem, bool remove);

    public virtual async Task GenerateIndexAsync()
    {
        foreach (var contentItemId in IndexGenerationIsRemovalByType.Keys)
        {
            await _indexService.RemoveAsync(IdColumnName, contentItemId, _session);
        }

        foreach (var contentItem in await _contentManager.GetAsync(this.GetUpdateIds()))
        {
            var index = await GenerateIndexAsync(contentItem);
            if (index != null) await _indexService.AddAsync(index, _session, contentItem.Id);
        }
    }

    protected abstract Task<TIndex> GenerateIndexAsync(ContentItem contentItem);
}
