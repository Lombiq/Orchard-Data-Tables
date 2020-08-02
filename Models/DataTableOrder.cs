using Lombiq.DataTables.Constants;
using Newtonsoft.Json;

namespace Lombiq.DataTables.Models
{
    public class DataTableOrder
    {
        [JsonProperty(PropertyName = "column")]
        public string Column { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public SortingDirection Direction { get; set; }

        [JsonIgnore] public bool IsAscending => Direction == SortingDirection.Ascending;
    }
}
