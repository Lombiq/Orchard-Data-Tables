using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DataTableDataRequest
    {
        public string QueryId { get; set; }
        public string DataProvider { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<DataTableColumn> ColumnFilters { get; set; }
        public DataTableSearchParameters Search { get; set; }
        public DataTableOrder[] Order { get; set; }


    }
}
