using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lombiq.DataTables.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataTableChildRowResponse
    {
        public string Error { get; set; }
        public string Content { get; set; }

        public static DataTableChildRowResponse ErrorResult(string errorText) => new() { Error = errorText };
    }
}
