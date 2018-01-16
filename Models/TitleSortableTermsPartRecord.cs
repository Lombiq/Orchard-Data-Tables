using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class TitleSortableTermsPartRecord : ContentPartRecord
    {
        public TitleSortableTermsPartRecord()
        {
            Terms = new List<TitleSortableTermContentItem>();
        }

        [CascadeAllDeleteOrphan]
        public virtual IList<TitleSortableTermContentItem> Terms { get; set; }
    }
}