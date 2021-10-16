using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models
{
    public class VueModel
    {
        [JsonProperty("text", NullValueHandling=NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("html", NullValueHandling=NullValueHandling.Ignore)]
        public string Html { get; set; }

        [JsonProperty("sort", NullValueHandling=NullValueHandling.Ignore)]
        public object Sort { get; set; }

        [JsonProperty("href", NullValueHandling=NullValueHandling.Ignore)]
        public string Href { get; set; }

        [JsonProperty("special", NullValueHandling=NullValueHandling.Ignore)]
        public object Special { get; set; }

        public VueModel() { }

        public VueModel(string text, string href = null)
        {
            Text = text;
            Href = href;
        }

        public VueModel(IContent content)
        {
            Text = content.ContentItem.DisplayText;
            Href = $"/Admin/Contents/ContentItems/{content.ContentItem.ContentItemId}/Edit";
        }

        public static JArray TableToJson<T>(IEnumerable<T> collection, Func<T, Dictionary<string, VueModel>> select)
        {
            var rows = collection
                .Select(select)
                .Select((row, rowIndex) =>
                {
                    var castRow = row.ToDictionary(pair => pair.Key, pair => JToken.FromObject(pair.Value));
                    castRow["$rowIndex"] = rowIndex;
                    return castRow;
                });

            return JArray.FromObject(rows);
        }
    }
}
