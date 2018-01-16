using Orchard.Core.Title.Models;
using Orchard.Data.Conventions;
using Orchard.Taxonomies.Models;

namespace Lombiq.DataTables.Models
{
    /// <summary>
    /// Just like <see cref="TermContentItem"/>, this class represents a relationship
    /// between a Content Item and a Term, but also includes the TitlePartRecord for the Term,
    /// so we are able to sort content items based on their selected Taxonomy Terms.
    /// If there are multiple Terms selected, the first one (based on <see cref="TermPart"/>'s own sorting)
    /// will be flagged to be used for the comparison.
    /// </summary>
    public class TitleSortableTermContentItem
    {
        public virtual int Id { get; set; }
        public virtual string Field { get; set; }
        public virtual bool IsFirstTerm { get; set; }
        public virtual TermPartRecord TermRecord { get; set; }
        public virtual TitlePartRecord TitlePartRecord { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual TitleSortableTermsPartRecord TitleSortableTermsPartRecord { get; set; }
    }
}