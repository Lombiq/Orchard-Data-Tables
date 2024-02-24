using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class DataTableRow
{
    [JsonExtensionData]
    internal IDictionary<string, JsonNode> ValuesDictionary { get; set; }

    public int Id { get; set; }

    public string this[string name]
    {
        get => ValuesDictionary.TryGetValue(name, out var value) ? value.Value<string>() : null;
        set => ValuesDictionary[name] = value;
    }

    public DataTableRow() => ValuesDictionary = new Dictionary<string, JsonNode>();

    public DataTableRow(int id, IDictionary<string, JsonNode> valuesDictionary)
    {
        Id = id;
        ValuesDictionary = valuesDictionary;
    }

    public IEnumerable<string> GetValues() =>
        ValuesDictionary.Values.Select(value => value.Value<string>());

    public IEnumerable<string> GetValuesOrderedByColumns(IEnumerable<DataTableColumnDefinition> columnDefinitions) =>
        columnDefinitions.Select(columnDefinition => this[columnDefinition.Name] ?? string.Empty);
}
