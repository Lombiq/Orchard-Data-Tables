using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models
{
    public class DataTableRow
    {
        [JsonExtensionData]
        internal Dictionary<string, object> ValuesDictionary { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        public object this[string name]
        {
            get { return ValuesDictionary.ContainsKey(name) ? ValuesDictionary[name] : null; }
            set { ValuesDictionary[name] = value; }
        }

        public DataTableRow() => ValuesDictionary = new Dictionary<string, object>();

        public IEnumerable<object> GetValues() =>
            ValuesDictionary.Values.Select(value => value);

        public IEnumerable<object> GetValuesOrderedByColumns(IEnumerable<DataTableColumnDefinition> columnDefinitions) =>
            columnDefinitions.Where(column => column.DisplayCondition()).Select(columnDefinition => this[columnDefinition.Name]);

        /// <summary>
        /// Can be useful if certain columns only needed in the export table.
        /// </summary>
        public IEnumerable<object> GetValuesOrderedByColumnsWithNotDisplayedColumns(IEnumerable<DataTableColumnDefinition> columnDefinitions) =>
            columnDefinitions.Select(columnDefinition => this[columnDefinition.Name] ?? string.Empty);
    }
}