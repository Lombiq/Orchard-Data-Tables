using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Models
{
    public class VueModel
    {
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("html", NullValueHandling = NullValueHandling.Ignore)]
        public string Html { get; set; }

        [JsonProperty("sort", NullValueHandling = NullValueHandling.Ignore)]
        public object Sort { get; set; }

        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }

        [JsonProperty("special", NullValueHandling = NullValueHandling.Ignore)]
        public object Special { get; set; }

        [JsonProperty("hiddenInput", NullValueHandling = NullValueHandling.Ignore)]
        public HiddenInputValue HiddenInput { get; set; }

        public VueModel() { }

        public VueModel(string text, string href = null)
        {
            Text = text;
            Href = href;
        }

        public VueModel SetHiddenInput(string name, string value)
        {
            HiddenInput = new HiddenInputValue { Name = name, Value = value };
            return this;
        }

        public static async Task<JArray> TableToJsonAsync<T>(
            IEnumerable<T> collection,
            Func<T, Task<Dictionary<string, VueModel>>> select)
        {
            var rows = (await collection.AwaitEachAsync(select))
                .Select((row, rowIndex) =>
                {
                    var castRow = row.ToDictionary(pair => pair.Key, pair => JToken.FromObject(pair.Value));
                    castRow["$rowIndex"] = rowIndex;
                    return castRow;
                });

            return JArray.FromObject(rows);
        }

        public class HiddenInputValue
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
