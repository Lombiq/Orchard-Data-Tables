using System.Collections.Generic;

namespace Lombiq.DataTables.Models;

public class DataTableColumnDefinition
{
    public IDictionary<string, object> AdditionalSettings { get; } = new Dictionary<string, object>();
    public string Name { get; set; }
    public string Text { get; set; }
    public string DataSource { get; set; }
    public bool Orderable { get; set; } = true;
    public bool Exportable { get; set; } = true;
    public bool Searchable { get; set; } = true;
    public bool IsLiquid { get; set; }
    public (string From, string To)? Regex { get; set; }

    public string this[string key]
    {
        get => AdditionalSettings.ContainsKey(key) ? AdditionalSettings[key]?.ToString() : null;
        set => AdditionalSettings[key] = value;
    }

    public DataTableColumnDefinition() { }

    public DataTableColumnDefinition(string name, string text, bool orderable = true)
    {
        Name = name;
        Text = text;
        Orderable = orderable;
    }
}
