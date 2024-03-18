using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class DataTableColumn
{
    public string Data { get; set; }
    public string Name { get; set; }
    public bool Searchable { get; set; }
    public bool Orderable { get; set; }
    public DataTableSearchParameters Search { get; set; }
}
