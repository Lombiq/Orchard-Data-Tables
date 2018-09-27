using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models
{
    public class DataTableRow
    {
        [JsonExtensionData]
        internal Dictionary<string, JToken> ValuesDictionary { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        public string this[string name]
        {
            get { return ValuesDictionary.ContainsKey(name) ? ValuesDictionary[name].Value<string>() : null; }
            set { ValuesDictionary[name] = value; }
        }


        public DataTableRow()
        {
            ValuesDictionary = new Dictionary<string, JToken>();
        }


        public IEnumerable<string> GetValues() =>
            ValuesDictionary.Values.Select(value => value.Value<string>());

        public IEnumerable<string> GetValuesOrderedByColumns(IEnumerable<DataTableColumnDefinition> columnDefinitions) =>
            columnDefinitions.Select(columnDefinition => this[columnDefinition.Name] ?? "");
    }
}