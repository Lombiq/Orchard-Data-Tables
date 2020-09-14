using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
        public class ExportLink
        {
            public string Type => nameof(ExportLink);
            public string Url { get; set; }
            public string Text { get; set; }

            public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

            public ExportLink(string url, string text, IDictionary<string, string> attributes = null)
            {
                Url = url;
                Text = text;
                if (attributes != null) Attributes = attributes;
            }

            public override string ToString() => Text;
        }
}
