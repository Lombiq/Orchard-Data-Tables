#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

public class ExportLink
{
    // While the warning doesn't show up in VS it does with dotnet build.
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "It's necessary to be instance-level for JSON serialization.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    public string Type => nameof(ExportLink);
    public string Url { get; set; }
    public JsonNode Text { get; set; }

    [JsonInclude]
    public IDictionary<string, string> Attributes { get; internal set; } = new Dictionary<string, string>();

    public ExportLink(string url, JsonNode text, IDictionary<string, string>? attributes = null)
    {
        Url = url;
        Text = text;
        if (attributes != null) Attributes = attributes;
    }

    public override string ToString() => Text.ToString();

    public static bool IsInstance(JsonObject jsonObject) =>
        jsonObject.HasTypeProperty<ExportLink>();

    public static string? GetText(JsonObject jObject) => jObject[nameof(Text)]?.ToString();
}
