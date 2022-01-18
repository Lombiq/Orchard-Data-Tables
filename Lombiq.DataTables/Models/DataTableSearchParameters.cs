using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lombiq.DataTables.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataTableSearchParameters
    {
        public string Value { get; set; }

        [JsonProperty(PropertyName = "regex")]
        public bool IsRegex { get; set; }
    }
}
