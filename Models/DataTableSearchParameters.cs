using Newtonsoft.Json;

namespace Lombiq.DataTables.Models
{
    public class DataTableSearchParameters
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "regex")]
        public bool Regex { get; set; }
    }
}
