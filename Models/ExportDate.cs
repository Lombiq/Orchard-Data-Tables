using Newtonsoft.Json.Linq;
using NodaTime;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.DataTables.Models
{
    public class ExportDate
    {
        // While the warning doesn't show up in VS it does with dotnet build.
#pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage(
            "Performance",
            "CA1822:Mark members as static",
            Justification = "It's necessary to be instance-level for JSON serialization.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        public string Type => nameof(ExportDate);

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string ExcelFormat { get; set; }

        public static bool IsInstance(JObject jObject) =>
            jObject[nameof(Type)]?.ToString() == nameof(ExportDate);

        public static string GetText(JObject jObject) => ((LocalDate)jObject.ToObject<ExportDate>()).ToShortDateString();

        public static implicit operator ExportDate(LocalDate localDate) =>
            new()
            {
                Year = localDate.Year,
                Month = localDate.Month,
                Day = localDate.Day,
            };

        public static implicit operator ExportDate(DateTime dateTime) =>
            new()
            {
                Year = dateTime.Year,
                Month = dateTime.Month,
                Day = dateTime.Day,
            };

        public static explicit operator LocalDate(ExportDate exportDate) =>
            new(exportDate.Year, exportDate.Month, exportDate.Day);

        public static explicit operator DateTime(ExportDate exportDate) =>
            new LocalDate(exportDate.Year, exportDate.Month, exportDate.Day).ToDateTimeUnspecified();

        public static implicit operator ExportDate(LocalDate? localDateNullable) =>
            localDateNullable is { } localDate ? localDate : (ExportDate)null;

        public static implicit operator ExportDate(DateTime? dateTimeNullable) =>
            dateTimeNullable is { } dateTime ? dateTime : (ExportDate)null;
    }
}
