using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataTableDataResponse
    {
        /// <summary>
        /// This property identifies the request for the jQuery.DataTables plugin. This needs to be parsed and
        /// sent back in order to prevent Cross Site Scripting (XSS) attack.
        /// See: https://datatables.net/manual/server-side
        /// </summary>
        /// <remarks>For internal use only. It's overwritten during normal use.</remarks>
        [JsonProperty]
        internal int Draw { get; set; }

        /// <summary>
        /// Extra informational field that shows the actual total if filtering (such as keyword search) is used. When
        /// not filtering it must be the same as <see cref="RecordsFiltered"/>.
        /// </summary>
        public int RecordsTotal { get; set; }

        /// <summary>
        /// The total number of results; used for paging.
        /// </summary>
        public int RecordsFiltered { get; set; }

        /// <summary>
        /// The table contents of the current page.
        /// </summary>
        public IEnumerable<DataTableRow> Data { get; set; }

        /// <summary>
        /// User-facing error message in case something went wrong.
        /// </summary>
        public string Error { get; set; }


        /// <summary>
        /// Creates a new response with only the <see cref="Error"/> filed set.
        /// </summary>
        /// <param name="errorText">The text to display.</param>
        public static DataTableDataResponse ErrorResult(string errorText) =>
            new DataTableDataResponse
            {
                Error = errorText,
                Data = Array.Empty<DataTableRow>(),
                RecordsFiltered = 0,
                RecordsTotal = 0
            };

        /// <summary>
        /// Creates a new response with empty <see cref="Data"/>.
        /// </summary>
        public static DataTableDataResponse Empty() => ErrorResult(null);
    }
}
