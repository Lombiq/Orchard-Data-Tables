using Orchard.Core.Title.Models;
using Orchard.Data.Conventions;
using Orchard.Taxonomies.Models;

namespace Lombiq.DataTables.Models
{
    /// <summary>
    /// Just like <see cref="TermContentItem"/>, this class represent a relationship
    /// between a Content Item and a Term, but also includes the TitlePartRecord for the Term
    /// to be able to sort content items based on its selected Taxonomy Terms.
    /// </summary>
    public class TitleSortableTermContentItem
    {
        public virtual int Id { get; set; }
        public virtual string Field { get; set; }
        public virtual TermPartRecord TermRecord { get; set; }
        public virtual TitlePartRecord TitlePartRecord { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual TitleSortableTermsPartRecord TitleSortableTermsPartRecord { get; set; }
    }
}