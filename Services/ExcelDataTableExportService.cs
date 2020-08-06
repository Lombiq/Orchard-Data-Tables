using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Lombiq.DataTables.Models;
using Newtonsoft.Json.Linq;

namespace Lombiq.DataTables.Services
{
    public class ExcelDataTableExportService : IDataTableExportService
    {
        public string Name => nameof(ExcelDataTableExportService);
        public string DefaultFileName => "export.xlsx";


        public async Task<Stream> ExportAsync(
            IDataTableDataProvider dataProvider,
            DataTableDataRequest request,
            DataTableColumnsDefinition columnsDefinition = null)
        {
            columnsDefinition ??= await dataProvider.GetColumnsDefinitionAsync(request.QueryId);
            var columns = columnsDefinition.Columns.Where(column => column.Exportable).ToList();

            var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(dataProvider.Description);

            var response = await dataProvider.GetRowsAsync(request);
            if (!string.IsNullOrWhiteSpace(response.Error))
            {
                worksheet.Cell(2, 1).Value = response.Error;
                return Save(workbook, stream);
            }

            // Create table header.
            for (var c = 0; c < columns.Count; c++) worksheet.Cell(1, c + 1).Value = columns[c].Text;
            worksheet.Range(1, 1, 1, columns.Count).Style.Font.Bold = true;
            worksheet.SheetView.Freeze(2, 1);

            // Create table body.
            var results = response.Data.ToList();
            for (int i = 0; i < results.Count; i++)
            {
                var item = results[i];
                var row = 2 + i;
                for (var c = 0; c < columns.Count; c++)
                {
                    var cell = worksheet.Cell(row, c + 1);
                    var value = item.ValuesDictionary[columns[c].Name];

                    if (value.Type == JTokenType.Date) cell.Style.NumberFormat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";

                    cell.Value = value.Type switch
                    {
                        JTokenType.Boolean => value.ToObject<bool>(),
                        JTokenType.Date => value.ToObject<DateTime>(),
                        JTokenType.Float => value.ToObject<double>(),
                        JTokenType.Integer => value.ToObject<int>(),
                        JTokenType.Null => null,
                        JTokenType.TimeSpan => value.ToObject<TimeSpan>(),
                        JTokenType.Array => string.Join(", ", ((JArray)value).Select(item => item.ToString())),
                        _ => value.ToString()
                    };
                }
            }

            // Make the content auto-fit the columns.
            worksheet.RecalculateAllFormulas();
            worksheet.Columns().AdjustToContents();

            return Save(workbook, stream);
        }


        private static Stream Save(XLWorkbook workbook, Stream stream)
        {
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
