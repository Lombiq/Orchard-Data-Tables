using ClosedXML.Excel;
using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public class ExcelDataTableExportService : IDataTableExportService
    {
        public IStringLocalizer<ExcelDataTableExportService> T { get; }
        public string Name => nameof(ExcelDataTableExportService);
        public string DefaultFileName => "export.xlsx";


        public ExcelDataTableExportService(IStringLocalizer<ExcelDataTableExportService> stringLocalizer) =>
            T = stringLocalizer;


        public async Task<Stream> ExportAsync(
            IDataTableDataProvider dataProvider,
            DataTableDataRequest request,
            DataTableColumnsDefinition columnsDefinition = null)
        {
            columnsDefinition ??= await dataProvider.GetColumnsDefinitionAsync(request.QueryId);
            var columns = columnsDefinition.Columns.Where(column => column.Exportable).ToList();
            var response = await dataProvider.GetRowsAsync(request);
            var results = response.Data
                .Select(item => columns.Select(column => item.ValuesDictionary[column.Name]).ToArray())
                .ToArray();

            return CollectionToStream(
                dataProvider.Description,
                columns.Select(column => column.Text).ToArray(),
                results,
                T,
                response.Error);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Blocker Code Smell",
            "S2368:Public methods should not have multidimensional array parameters",
            Justification = "We are rendering a table.")]
        public static Stream CollectionToStream(
            string worksheetName,
            string[] columns,
            JToken[][] results,
            IStringLocalizer<ExcelDataTableExportService> localizer,
            string error = null)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(worksheetName);

            if (!string.IsNullOrWhiteSpace(error))
            {
                worksheet.Cell(2, 1).Value = error;
                return Save(workbook);
            }

            // Create table header.
            for (var c = 0; c < columns.Length; c++) worksheet.Cell(1, c + 1).Value = columns[c];
            worksheet.Range(1, 1, 1, columns.Length).Style.Font.Bold = true;
            worksheet.SheetView.Freeze(1, 0);

            var dateFormat = localizer["mm/dd/yyyy"].Value;

            // Create table body.
            for (int i = 0; i < results.Length; i++)
            {
                var row = 2 + i;
                for (var c = 0; c < columns.Length; c++)
                {
                    var cell = worksheet.Cell(row, c + 1);
                    var value = results[i][c];

                    if (value.Type == JTokenType.Date) cell.Style.DateFormat.Format = dateFormat;

                    if (value is JObject jObject && jObject["Type"]?.ToString() == nameof(ExportLink))
                    {
                        var link = jObject.ToObject<ExportLink>();
                        if (link != null) cell.FormulaA1 = $"HYPERLINK(\"{link.Url}\",\"{link.Text}\")";
                    }
                    else
                    {
                        cell.Value = value.Type switch
                        {
                            JTokenType.Boolean => value.ToObject<bool>()
                                ? localizer["Yes"].Value
                                : localizer["No"].Value,
                            JTokenType.Date => value.ToObject<DateTime>(),
                            JTokenType.Float => value.ToObject<double>(),
                            JTokenType.Integer => value.ToObject<int>(),
                            JTokenType.Null => null,
                            JTokenType.TimeSpan => value.ToObject<TimeSpan>(),
                            JTokenType.Array => string.Join(", ", ((JArray)value).Select(item => item.ToString())),
                            _ => value.ToString(),
                        };
                    }
                }
            }

            // Make the content auto-fit the columns.
            worksheet.RecalculateAllFormulas();
            worksheet.Columns().AdjustToContents();

            return Save(workbook);
        }


        private static Stream Save(XLWorkbook workbook)
        {
            var stream = new MemoryStream();

            workbook.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }
    }
}
