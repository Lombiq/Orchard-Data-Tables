using Newtonsoft.Json;
using System;
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

    [JsonConverter(typeof(FormattedDataTableRowValueConverter))]
    public class FormattedDataTableRowValue
    {
        public object Value { get; set; }
        public Func<object, string> Formatter { get; set; }

        public FormattedDataTableRowValue(object value, Func<object, string> formatter)
        {
            Value = value;
            Formatter = formatter;
        }
    }

    public class FormattedDataTableRowValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FormattedDataTableRowValue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var cell = value as FormattedDataTableRowValue;

            if (cell != null && cell.Formatter != null)
            {
                string formattedValue = cell.Formatter(cell.Value);
                writer.WriteValue(formattedValue);
            }
            else
            {
                writer.WriteValue(cell?.Value?.ToString() ?? string.Empty);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}