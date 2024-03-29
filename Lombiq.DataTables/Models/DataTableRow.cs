using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class DataTableRow
{
    [JsonExtensionData]
    internal IDictionary<string, JToken> ValuesDictionary { get; set; }

    public int Id { get; set; }

    public string this[string name]
    {
        get => ValuesDictionary.TryGetValue(name, out var value) ? value.Value<string>() : null;
        set => ValuesDictionary[name] = value;
    }

    public DataTableRow() => ValuesDictionary = new Dictionary<string, JToken>();

    public DataTableRow(int id, IDictionary<string, JToken> valuesDictionary)
    {
        Id = id;
        ValuesDictionary = valuesDictionary;
    }

    public IEnumerable<string> GetValues() =>
        ValuesDictionary.Values.Select(value => value.Value<string>());

    public IEnumerable<string> GetValuesOrderedByColumns(IEnumerable<DataTableColumnDefinition> columnDefinitions) =>
        columnDefinitions.Select(columnDefinition => this[columnDefinition.Name] ?? string.Empty);
}
