using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnDefinition
    {
        public Dictionary<string, string> AdditionalSettings { get; set; }
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
            get => AdditionalSettings.ContainsKey(key) ? AdditionalSettings[key] : null;
            set => AdditionalSettings[key] = value;
        }
    }
}
