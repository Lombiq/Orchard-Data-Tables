using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnDefinition
    {
        public Dictionary<string, object> AdditionalSettings { get; } = new();
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

        /// <summary>
        /// Adds a "special" entry to the <see cref="AdditionalSettings"/> for use in &lt;icbin-datatable&gt; which is
        /// handled by the same event that handles <see cref="VueModel.Special"/>.
        /// </summary>
        public DataTableColumnDefinition WithSpecial(object special)
        {
            AdditionalSettings[nameof(special)] = special;
            return this;
        }
    }
}
