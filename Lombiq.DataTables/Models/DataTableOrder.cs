using Lombiq.DataTables.Constants;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

public class DataTableOrder
{
    public string Column { get; set; }

    [JsonIgnore]
    public SortingDirection Direction { get; set; }

    [JsonPropertyName("direction")]
    [JsonInclude]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "It's used for JSON conversion.")]
    private string DirectionString
    {
        get => IsAscending ? "ascending" : "descending";
        set => Direction = value == "descending" ? SortingDirection.Descending : SortingDirection.Ascending;
    }

    [JsonIgnore]
    public bool IsAscending => Direction == SortingDirection.Ascending;
}
