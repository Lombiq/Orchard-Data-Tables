using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class DataTableDataResponse
    {
        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonProperty(PropertyName = "recordsTotalIds")]
        public IEnumerable<int> RecordsTotalIds { get; set; }

        [JsonProperty(PropertyName = "recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonProperty(PropertyName = "data")]
        public IEnumerable<DataTableRow> Data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }


        public static DataTableDataResponse ErrorResult(string errorText) =>
            new DataTableDataResponse { Error = errorText };
    }
}