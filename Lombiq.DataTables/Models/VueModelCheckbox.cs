using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

// This class is used in <icbin-datatable>.
public class VueModelCheckbox
{
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "It's necessary to be instance-level for JSON serialization.")]
    [JsonPropertyName("type")]
    public string Type => "checkbox";

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("value")]
    public bool? Value { get; set; }

    [JsonPropertyName("classes")]
    public string Classes { get; set; } = string.Empty;
}
