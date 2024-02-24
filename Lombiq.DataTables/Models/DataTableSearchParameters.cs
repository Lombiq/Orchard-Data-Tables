using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class DataTableSearchParameters
{
    public string Value { get; set; }

    [JsonProperty(PropertyName = "regex")]
    public bool IsRegex { get; set; }
}
