using Newtonsoft.Json.Linq;
using NodaTime;
using System;

namespace Lombiq.DataTables.Models
{
    public class ExportDate
    {
        public string Type => nameof(ExportDate);

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string ExcelFormat { get; set; }

        public static bool IsInstance(JObject jObject) =>
            jObject[nameof(Type)]?.ToString() == nameof(ExportDate);

        public static string GetText(JObject jObject) => ((LocalDate)jObject.ToObject<ExportDate>()).ToShortDateString();

        public static implicit operator ExportDate(LocalDate localDate) =>
            new ExportDate
            {
                Year = localDate.Year,
                Month = localDate.Month,
                Day = localDate.Day,
            };

        public static explicit operator LocalDate(ExportDate exportDate) =>
            new LocalDate(exportDate.Year, exportDate.Month, exportDate.Day);

        public static explicit operator DateTime(ExportDate exportDate) =>
            new DateTime(exportDate.Year, exportDate.Month, exportDate.Day);
    }
}
