using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Lombiq.DataTables.Models
{
    public class DataTableDataResponse
    {
        [JsonProperty(PropertyName = "draw")]
        [Obsolete("For internal use only. It's overwritten during normal use.")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }

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
