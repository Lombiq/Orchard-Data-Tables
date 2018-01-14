using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
    }
}