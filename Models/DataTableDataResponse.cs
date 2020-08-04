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
        /// <summary>
        /// This property identifies the request for the jQuery.DataTables plugin. This needs to be parsed and
        /// sent back in order to prevent Cross Site Scripting (XSS) attack.
        /// See: https://datatables.net/manual/server-side
        /// </summary>
        [JsonProperty(PropertyName = "draw")]
        [Obsolete("For internal use only. It's overwritten during normal use.")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "itemsTotal")]
        public int ItemsTotal { get; set; }

        [JsonProperty(PropertyName = "itemsFiltered")]
        public int ItemsFiltered { get; set; }

        [JsonProperty(PropertyName = "data")]
        public IEnumerable<DataTableRow> Data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }


        public static DataTableDataResponse ErrorResult(string errorText) =>
            new DataTableDataResponse { Error = errorText };
    }
}
