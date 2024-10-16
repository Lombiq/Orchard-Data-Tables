using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

public class DataTableRow
{
    [JsonExtensionData]
    [JsonInclude]
    [JsonPropertyName("valuesDictionary")]
    internal IDictionary<string, object> ValuesDictionary { get; set; } = new Dictionary<string, object>();

    [JsonPropertyName("id")]
    public int Id { get; set; }

    public string this[string name]
    {
        get => ValuesDictionary.GetMaybe(name)?.ToString();
        set => ValuesDictionary[name] = value;
    }

    public DataTableRow() { }

    public DataTableRow(int id, IDictionary<string, JsonNode> valuesDictionary)
    {
        Id = id;

        if (valuesDictionary != null)
        {
            foreach (var (key, value) in valuesDictionary)
            {
                ValuesDictionary[key] = value;
            }
        }
    }

    public IEnumerable<string> GetValues() =>
        ValuesDictionary.Values.Select(value => value.ToString());

    public IEnumerable<string> GetValuesOrderedByColumns(IEnumerable<DataTableColumnDefinition> columnDefinitions) =>
        columnDefinitions.Select(columnDefinition => this[columnDefinition.Name] ?? string.Empty);

    internal JsonNode GetValueAsJsonNode(string name) =>
        ValuesDictionary.GetMaybe(name) switch
        {
            JsonNode node => node,
            { } otherValue => JObject.FromObject(otherValue),
            null => null,
        };
}
