using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class ExportLink
    {
        [JsonProperty]
        public string Type => nameof(ExportLink);

        public string Url { get; set; }
        public string Text { get; set; }

        [JsonProperty]
        public IDictionary<string, string> Attributes { get; internal set; } = new Dictionary<string, string>();

        public ExportLink() { }

        public ExportLink(string url, string text, IDictionary<string, string> attributes = null)
        {
            Url = url;
            Text = text;
            if (attributes != null) Attributes = attributes;
        }
    }
}
