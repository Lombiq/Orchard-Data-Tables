using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnDefinition
    {
        public Dictionary<string, string> AdditionalSettings { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string DataSource { get; set; }

        public string this[string name]
        {
            get { return AdditionalSettings.ContainsKey(name) ? AdditionalSettings[name] : null; }
            set { AdditionalSettings[name] = value; }
        }
    }
}