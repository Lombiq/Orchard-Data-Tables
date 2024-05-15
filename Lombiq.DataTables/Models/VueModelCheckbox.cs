using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

// This class is used in <icbin-datatable>.
public class VueModelCheckbox
{
    // While the warning doesn't show up in VS it does with dotnet build, but inconsistently.
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "It's necessary to be instance-level for JSON serialization.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
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
