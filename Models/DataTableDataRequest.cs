using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class DataTableDataRequest
    {
        [JsonProperty(PropertyName = "queryId")]
        public int QueryId { get; set; }

        [JsonProperty(PropertyName = "dataProvider")]
        public string DataProvider { get; set; }

        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }

        [JsonProperty(PropertyName = "columns")]
        public List<DataTableColumn> Columns { get; set; }

        [JsonProperty(PropertyName = "search")]
        public DataTableSearchParameters Search { get; set; }

        [JsonProperty(PropertyName = "order")]
        public DataTableOrder[] Order { get; set; }
    }
}