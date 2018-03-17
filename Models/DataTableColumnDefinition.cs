using Orchard.ContentManagement.Records;
using System.Collections.Generic;
using static Lombiq.DataTables.Constants.DataTableColumnDefinitionSettingsKeys;

namespace Lombiq.DataTables.Models
{
    public class DataTableColumnDefinition
    {
        public Dictionary<string, string> AdditionalSettings { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string DataSource { get; set; }

        public string this[string key]
        {
            get { return AdditionalSettings.ContainsKey(key) ? AdditionalSettings[key] : null; }
            set { AdditionalSettings[key] = value; }
        }


        public static Dictionary<string, string> AdditionalSettingsForContentPartRecordPropertySorting<TRecord>(
            string propertyName) where TRecord : ContentPartRecord =>
            new Dictionary<string, string>()
            {
                {
                    ContentPartRecordPropertySorting.RecordTypeAssemblyQualifiedName,
                    typeof(TRecord).AssemblyQualifiedName
                },
                {
                    ContentPartRecordPropertySorting.PropertyName,
                    propertyName
                }
            };
    }
}