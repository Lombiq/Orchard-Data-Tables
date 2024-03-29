using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services;

/// <summary>
/// A service for generating DataTables indexes for a specific table.
/// </summary>
public interface IDataTableIndexGenerator
{
    /// <summary>
    /// Gets a dictionary where the key is the <see cref="ContentItem.ContentItemId"/> where index generation is ordered
    /// and the value is <see langword="true"/> if the index should be removed, <see langword="false"/> if it should be
    /// updated.
    /// </summary>
    IDictionary<string, bool> IndexGenerationIsRemovalByType { get; }

    /// <summary>
    /// Gets the content type names that the index is made for. Other content types may only trigger index updates by
    /// being in some relationship with these types.
    /// </summary>
    IEnumerable<string> ManagedContentTypes { get; }

    /// <summary>
    /// Checks if the index generator is applicable and if it needs to do anything.
    /// </summary>
    ValueTask<bool> NeedsUpdatingAsync(ContentContextBase context);

    /// <summary>
    /// Places an index generation order for <paramref name="contentItem"/> or any related content items that fit the
    /// operation of this index generator.
    /// </summary>
    Task ScheduleDeferredIndexGenerationAsync(ContentItem contentItem, bool remove);

    /// <summary>
    /// Generates new indexes for the content items stored in <see cref="IndexGenerationIsRemovalByType"/>.
    /// </summary>
    Task GenerateIndexAsync();
}

public static class DataTableIndexGeneratorExtensions
{
    /// <summary>
    /// Adds a content item to the index generation orders. Update (<see langword="false"/>) is preferred over remove
    /// (<see langword="true"/>).
    /// </summary>
    public static void AddIndexGenerationOrder(this IDataTableIndexGenerator generator, string id, bool remove)
    {
        if (string.IsNullOrEmpty(id)) return;

        generator.IndexGenerationIsRemovalByType[id] =
            remove && generator.IndexGenerationIsRemovalByType.GetMaybe(id);
    }

    /// <summary>
    /// Returns the <see cref="ContentItem.ContentItemId"/> s from the <see
    /// cref="IDataTableIndexGenerator.IndexGenerationIsRemovalByType"/> for updates only.
    /// </summary>
    public static IList<string> GetUpdateIds(this IDataTableIndexGenerator generator) =>
        generator.IndexGenerationIsRemovalByType
            .Where(pair => !pair.Value)
            .Select(pair => pair.Key)
            .ToList();
}
