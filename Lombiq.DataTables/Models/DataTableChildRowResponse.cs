using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class DataTableChildRowResponse
{
    public string Error { get; set; }
    public string Content { get; set; }

    public static DataTableChildRowResponse ErrorResult(string errorText) => new() { Error = errorText };
}
