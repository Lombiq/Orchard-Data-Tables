using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

public class DataTableColumn
{
    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("searchable")]
    public bool Searchable { get; set; }

    [JsonPropertyName("orderable")]
    public bool Orderable { get; set; }

    [JsonPropertyName("search")]
    public DataTableSearchParameters Search { get; set; }
}
