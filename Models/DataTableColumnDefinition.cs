using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4004:Collection properties should be readonly", Justification = "JSON")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "JSON")]
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
