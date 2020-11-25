#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class ExportLink
    {
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
