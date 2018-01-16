using Lombiq.DataTables.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.Taxonomies.Fields;
using System.Linq;

namespace Lombiq.DataTables.Handlers
{
    public class TitleSortableTitleSortableTermsPartHandler : ContentHandler
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;


        public TitleSortableTitleSortableTermsPartHandler(
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager,
            IRepository<TitleSortableTermsPartRecord> repository)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;

            Filters.Add(StorageFilter.For(repository));
        }


        protected override void Activating(ActivatingContentContext context)
        {
            base.Activating(context);

            // weld the TitleSortableTermsPart dynamically, if a field has been assigned to one of its parts
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(context.ContentType);
            if (contentTypeDefinition == null) return;

            if (contentTypeDefinition.Parts.Any(
                part => part.PartDefinition.Fields.Any(
                    field => field.FieldDefinition.Name == typeof(TaxonomyField).Name)))
            {
                context.Builder.Weld<TitleSortableTermsPart>();
            }
        }
    }
}
