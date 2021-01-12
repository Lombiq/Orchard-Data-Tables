#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.DataTables.Models
{
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
        public JToken Text { get; set; }

        [JsonProperty]
        public IDictionary<string, string> Attributes { get; internal set; } = new Dictionary<string, string>();

        public ExportLink(string url, JToken text, IDictionary<string, string>? attributes = null)
        {
            Url = url;
            Text = text;
            if (attributes != null) Attributes = attributes;
        }

        public override string ToString() => Text.ToString();

        public static bool IsInstance(JObject jObject) =>
            jObject[nameof(Type)]?.ToString() == nameof(ExportLink);

        public static string? GetText(JObject jObject) => jObject[nameof(Text)]?.ToString();
    }
}
