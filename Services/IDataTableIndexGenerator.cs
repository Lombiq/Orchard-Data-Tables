using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql.Indexes;

namespace Lombiq.DataTables.Services
{
    /// <summary>
    /// A service for generating DataTables indexes for a specific table.
    /// </summary>
    /// <typeparam name="TIndex">The type of <see cref="MapIndex"/> this service maintains.</typeparam>
    public interface IDataTableIndexGenerator<TIndex>
        where TIndex : MapIndex
    {
        /// <summary>
        /// Gets the content type names that the index is made for. Other content types may only trigger index updates
        /// by being in some relationship with these types.
        /// </summary>
        IEnumerable<string> ManagedContentType { get; }

        /// <summary>
        /// Checks if the index generator is applicable and if it needs to do anything.
        /// </summary>
        ValueTask<bool> NeedsUpdatingAsync(ContentContextBase context);

        /// <summary>
        /// Generates new indexes for the provided <paramref name="contentItem"/> change. If <paramref name="remove" />
        /// is <see langword="true"/> then the indexes are only removed.
        /// </summary>
        Task GenerateIndexAsync(ContentItem contentItem, bool remove);
    }
}
