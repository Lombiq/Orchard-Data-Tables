using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumn
    {
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "searchable")]
        public bool Searchable { get; set; }

        [JsonProperty(PropertyName = "orderable")]
        public bool Orderable { get; set; }

        [JsonProperty(PropertyName = "search")]
        public DataTableSearchParameters Search { get; set; }
    }
}