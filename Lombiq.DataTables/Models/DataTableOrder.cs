using Lombiq.DataTables.Constants;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class DataTableOrder
{
    public string Column { get; set; }
    public SortingDirection Direction { get; set; }

    [JsonIgnore]
    public bool IsAscending => Direction == SortingDirection.Ascending;
}
