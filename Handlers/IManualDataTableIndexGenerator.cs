using Lombiq.DataTables.Services;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;

namespace Finitive.CommercialEntities.Handlers
{
    /// <summary>
    /// Service for manually calling index generation for a specified <see cref="ContentItem"/>.
    /// </summary>
    public interface IManualDataTableIndexGenerator
    {
        /// <summary>
        /// Calls index generation the same way as if it was an update event from a content handler. This way you don't
        /// have to prepare for unforeseen consequences from other content handlers if you just want to update the
        /// index.
        /// </summary>
        /// <param name="contentItem"> The item for which the index is generated.</param>
        /// <param name="managedTypeOnly">
        /// If <see langword="true"/>, the <paramref name="contentItem"/> is only handled if its type is in
        /// <see cref="IDataTableIndexGenerator{TIndex}.ManagedContentType"/>, otherwise it gets skipped. Ideal for
        /// running a content item through all supported <see cref="IManualDataTableIndexGenerator"/> implementations.
        /// </param>
        Task GenerateIndicesAsync(ContentItem contentItem, bool managedTypeOnly);
    }
}
