using System;
using System.Collections.Generic;
using System.Linq;
using Lombiq.DataTables.Constants;
using Newtonsoft.Json;

namespace Lombiq.DataTables.Models
{
    public class DataTableDataRequest
    {
        [JsonProperty(PropertyName = "queryId")]
        public string QueryId { get; set; }

        [JsonProperty(PropertyName = "dataProvider")]
        public string DataProvider { get; set; }

        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }

        [JsonProperty(PropertyName = "columnFilters")]
        public List<DataTableColumn> ColumnFilters { get; set; }

        [JsonProperty(PropertyName = "search")]
        public DataTableSearchParameters Search { get; set; }

        [JsonProperty(PropertyName = "order")]
        public DataTableOrder[] Order { get; set; }

        public void SetOrder(IEnumerable<Dictionary<string, string>> orders) =>
            Order = orders?
                .Where(order => order.ContainsKey("column") && order.ContainsKey("direction"))
                .Select(order => new DataTableOrder
                {
                    Column = order["column"],
                    Direction = order["direction"] == "ascending"
                        ? SortingDirection.Ascending
                        : SortingDirection.Descending
                })
                .ToArray() ?? Array.Empty<DataTableOrder>();
    }
}
