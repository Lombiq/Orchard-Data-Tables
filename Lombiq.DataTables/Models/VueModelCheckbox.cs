using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

// This class is used in <icbin-datatable>.
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class VueModelCheckbox
{
    // While the warning doesn't show up in VS it does with dotnet build, but inconsistently.
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "It's necessary to be instance-level for JSON serialization.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    public string Type => "checkbox";

    public string Name { get; set; }

    public string Text { get; set; }

    public bool? Value { get; set; }

    public string Classes { get; set; } = string.Empty;
}
