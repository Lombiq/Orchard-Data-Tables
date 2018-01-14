using Lombiq.DataTables.Constants;
using Newtonsoft.Json;

namespace Lombiq.DataTables.Models
{
    public class DataTableOrder
    {
        [JsonProperty(PropertyName = "column")]
        public int Column { get; set; }

        [JsonProperty(PropertyName = "dir")]
        public SortingDirection Dir { get; set; }
    }
}