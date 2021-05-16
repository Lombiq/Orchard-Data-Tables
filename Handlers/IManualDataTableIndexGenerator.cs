using Lombiq.DataTables.Services;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Handlers
{
    /// <summary>
    /// Service for manually calling index generation for a specified <see cref="ContentItem"/>.
    /// </summary>
    public interface IManualDataTableIndexGenerator
    {
        /// <summary>
        /// Calls to place an order for index generation the same way as if it was an update event from a content
        /// handler. This way you don't have to prepare for unforeseen consequences from other content handlers if you
        /// just want to update the index.
        /// </summary>
        /// <param name="contentItem"> The item for which the index is generated.</param>
        /// <param name="managedTypeOnly">
        /// If <see langword="true"/>, the <paramref name="contentItem"/> is only handled if its type is in
        /// <see cref="IDataTableIndexGenerator{TIndex}.ManagedContentType"/>, otherwise it gets skipped. Ideal for
        /// running a content item through all supported <see cref="IManualDataTableIndexGenerator"/> implementations.
        /// </param>
        Task OrderIndexGenerationAsync(ContentItem contentItem, bool managedTypeOnly);

        /// <summary>
        /// Asks every <see cref="IDataTableIndexGenerator{TIndex}"/> to start generating indexes for the content items
        /// that received an order for it.
        /// </summary>
        Task GenerateReservedIndicesAsync();
    }

    public static class ManualDataTableIndexGeneratorExtensions
    {
        public static async Task OrderIndexGenerationAsync(
            this IEnumerable<IManualDataTableIndexGenerator> indexGenerators,
            ICollection<ContentItem> contentItems,
            bool managedTypeOnly)
        {
            foreach (var indexGenerator in indexGenerators)
            {
                foreach (var contentItem in contentItems)
                {
                    if (contentItem != null)
                    {
                        await indexGenerator.OrderIndexGenerationAsync(contentItem, managedTypeOnly);
                    }
                }
            }
        }

        public static Task GenerateReservedIndicesAsync(this IEnumerable<IManualDataTableIndexGenerator> indexGenerators) =>
            indexGenerators.FirstOrDefault()?.GenerateReservedIndicesAsync() ?? Task.CompletedTask;
    }
}
