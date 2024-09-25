using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

public class DataTableSearchParameters
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("regex")]
    public bool? IsRegex { get; set; }
}
