using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace Lombiq.DataTables.Services
{
    public class ExcelDataTableExportService : IDataTableExportService
    {
        private readonly IStringLocalizer<ExcelDataTableExportService> T;

        public ExcelDataTableExportService(IStringLocalizer<ExcelDataTableExportService> stringLocalizer) =>
            T = stringLocalizer;


        public async Task<Stream> ExportAsync(
            IDataTableDataProvider dataProvider,
            DataTableDataRequest request,
            DataTableColumnsDefinition columnsDefinition = null)
        {
            columnsDefinition ??= await dataProvider.GetColumnsDefinitionAsync(request.QueryId);
            var columns = columnsDefinition.Columns.ToList();

            var stream = new MemoryStream();
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.Add(dataProvider.Description);

            var response = await dataProvider.GetRowsAsync(request);
            if (!string.IsNullOrWhiteSpace(response.Error))
            {
                worksheet.Cells[2, 1].Value = response.Error;
                return Save(package, stream);
            }

            // Create table header.
            for (var c = 0; c < columns.Count; c++) worksheet.Cells[1, c + 1].Value = columns[c].Text;
            worksheet.Cells[1, 1, 1, columns.Count].Style.Font.Bold = true;
            worksheet.View.FreezePanes(2, 1);

            // Create table body.
            var results = response.Data.ToList();
            for (int i = 0; i < results.Count; i++)
            {
                var item = results[i];
                var row = 2 + i;
                for (var c = 0; c < columns.Count; c++)
                {
                    var cell = worksheet.Cells[row, c + 1];
                    var value = item.ValuesDictionary[columns[c].Name];

                    cell.Value = value;
                    if (value.Type == JTokenType.Date) cell.Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";
                }
            }

            // Make the content auto-fit the columns.
            worksheet.Calculate();
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return Save(package, stream);
        }


        private static Stream Save(ExcelPackage package, Stream stream)
        {
            package.Save();
            stream.Position = 0;
            return stream;
        }
    }
}
