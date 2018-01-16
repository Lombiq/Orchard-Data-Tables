using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    /// <summary>
    /// This Content Part is an extended version of <see cref="Orchard.Taxonomies.Models.TermsPart"/>,
    /// with an additional between a Term and its TitlePartRecord to be able sort content items
    /// based on its Term selected for a specific TaxonomyField.
    /// It will be attached dynamically whenever a TaxonomyField is found on a Content Type.
    /// </summary>
    public class TitleSortableTermsPart : ContentPart<TitleSortableTermsPartRecord>
    {
        public IList<TitleSortableTermContentItem> TermParts
        {
            get { return Record.Terms; }
            set { Record.Terms = value; }
        }
    }
}