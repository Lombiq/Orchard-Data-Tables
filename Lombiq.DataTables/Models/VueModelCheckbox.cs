using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

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
    [JsonProperty("type")]
    public string Type => "checkbox";

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("value")]
    public bool? Value { get; set; }

    [JsonProperty("classes")]
    public string Classes { get; set; } = string.Empty;
}
