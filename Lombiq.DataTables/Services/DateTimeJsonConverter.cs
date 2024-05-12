using Lombiq.DataTables.Models;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Services;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);

        if (node?.GetValueKind() == JsonValueKind.String)
        {
            return DateTime.Parse(node.GetValue<string>(), CultureInfo.InvariantCulture);
        }

        if (node.HasMatchingTypeProperty<DateTimeTicks>())
        {
            return node.ToObject<DateTimeTicks>().ToDateTime();
        }

        if (node.HasMatchingTypeProperty<ExportDate>())
        {
            return (DateTime)node.ToObject<ExportDate>();
        }

        throw new InvalidOperationException("Unable to parse JSON!");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        JObject
            .FromObject(new DateTimeTicks(value.Ticks, value.Kind))!
            .WriteTo(writer, options);

    public sealed record DateTimeTicks(long Ticks, DateTimeKind Kind)
    {
        public string Type => nameof(DateTimeTicks);

        public DateTime ToDateTime() => new(Ticks, Kind);
    }
}
