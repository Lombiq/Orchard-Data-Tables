using Lombiq.DataTables.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lombiq.DataTables.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class DataTableOrder
{
    public string Column { get; set; }
    public SortingDirection Direction { get; set; }

    [JsonIgnore]
    public bool IsAscending => Direction == SortingDirection.Ascending;
}
