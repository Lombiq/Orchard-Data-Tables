using Lombiq.DataTables.Samples.Indexes;
using Lombiq.DataTables.Samples.Models;
using Lombiq.DataTables.Services;
using Lombiq.HelpfulLibraries.Libraries.Database;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql;
using static Lombiq.DataTables.Samples.Constants.ContentTypes;

namespace Lombiq.DataTables.Samples.Services
{
    // This is the service that crunches all the data for the index. It is automatically used in a content handler, but
    // you may call ScheduleDeferredIndexGenerationAsync manually from any controller, filter, etc that's on the forward
    // side of the middleware pipeline.
    public class EmployeeDataTableIndexGenerator : DataTableIndexGeneratorBase<EmployeeDataTableIndex>
    {
        // This column is used automatically to delete old rows.
        protected override string IdColumnName => nameof(EmployeeDataTableIndex.ContentItemId);

        // If your table represents a content type provide it here. This means the "main" content type, if you need to
        // join other content types to it for related information (e.g. with ContentPickerField), those don't go here.
        public override IEnumerable<string> ManagedContentType { get; } = new[] { Employee };

        public EmployeeDataTableIndexGenerator(
            IContentManager contentManager,
            IManualConnectingIndexService<EmployeeDataTableIndex> indexService,
            ISession session)
            : base(contentManager, indexService, session)
        {
        }

        // This is called by the DataTableIndexGeneratorContentHandler<TIndexGenerator, TIndex> to check if this
        // generator wants to update anything on any content item event (updated, published, deleted, etc). Unlike the
        // ManagedContentType property, here you should actually consider all related content types yours may depend on.
        // There also may be more complex use-cases where you want to do further checks on the passed content type
        // before making a decision, for example you may want to check if relevant changes were actually made before
        // committing to an update that will cascade down and change a lot of rows in the index table.
        //
        // In this sample there aren't any, but to raise an example: if we wanted to have an Office content type and use
        // instead of the Employee's Office column, then this method would return true if the content in the context is
        // either Employee or Office.
        public override ValueTask<bool> NeedsUpdatingAsync(ContentContextBase context) =>
            // Actually if all you want to check is the ManagedContentType then you don't need to override this method.
            new(context.ContentItem.ContentType == Employee);

        // Once we have decided that a content item causes an update, we have to include it (or the related content
        // items that are ManagedContentType) and note if we need to delete or update. Since we don't work with related
        // content types here, it's pretty simple.
        public override Task ScheduleDeferredIndexGenerationAsync(ContentItem contentItem, bool remove)
        {
            this.AddIndexGenerationOrder(contentItem.ContentItemId, remove);
            return Task.CompletedTask;
        }

        // The deletion of old index rows is already handled, but you need to add the new ones. If you need to return
        // multiple, then override GenerateIndexAsync() instead.
        protected override Task<EmployeeDataTableIndex> GenerateIndexAsync(ContentItem contentItem)
        {
            var part = contentItem.As<EmployeePart>();
            if (part == null) return Task.FromResult<EmployeeDataTableIndex>(null);

            var index = new EmployeeDataTableIndex
            {
                ContentItemId = contentItem.ContentItemId,
                Name = part.Name.Text,
                Position = part.Position.Text,
                Office = part.Office.Text,
                Age = part.Age.Value is { } age ? (short?)age : null,
                StartDate = part.StartDate.Value,
                Salary = part.Salary.Value is { } salary ? (int?)salary : null,
            };

            return Task.FromResult(index);
        }

        // Note that if you add or change the table after there is already content, you need to create a maintenance
        // action to regenerate the indexes. This should fetch the content items that need to be regenerated and call
        // IEnumerable<IManualDataTableIndexGenerator>.ScheduleDeferredIndexGenerationAsync(contentItems).
    }
}

// NEXT STATION: Migrations/EmployeeDataTableMigrations.cs
