using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class ExportLink
    {
        public string Type { get => nameof(ExportLink); set {} }
        public string Url { get; set; }


        // Make sure this appears first after serialization for sorting purposes. This is faster and easier than trying
        // to type-check the resulting JObjects after the fact.
        [JsonProperty(Order = -1)]
        public string Text { get; set; }

        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public ExportLink() { }

        public ExportLink(string url, string text, IDictionary<string, string> attributes = null)
        {
            Url = url;
            Text = text;
            if (attributes != null) Attributes = attributes;
        }

        public override string ToString() => Text;
    }
}
