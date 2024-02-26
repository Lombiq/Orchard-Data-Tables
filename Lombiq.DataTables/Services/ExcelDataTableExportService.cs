using ClosedXML.Excel;
using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services;

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
        DataTableColumnsDefinition columnsDefinition = null,
        IDictionary<int, string> customNumberFormat = null)
    {
        columnsDefinition ??= await dataProvider.GetColumnsDefinitionAsync(request.QueryId);
        var columns = columnsDefinition.Columns.Where(column => column.Exportable).ToList();
        var response = await dataProvider.GetRowsAsync(request);
        var results = response.Data
            .Select(item => columns.Select(column => item.GetValueAsJsonNode(column.Name)).ToArray())
            .ToArray();

        return CollectionToStream(
            dataProvider.Description,
            columns.Select(column => column.Text).ToArray(),
            results,
            T,
            response.Error,
            customNumberFormat);
    }

    /// <summary>
    /// Returns downloadable XML workbook.
    /// </summary>
    /// <param name="worksheetName">The desired name of the worksheet.</param>
    /// <param name="columns">The name of the columns.</param>
    /// <param name="results">The data provided for the table.</param>
    /// <param name="localizer">IStringLocalizer instance.</param>
    /// <param name="error">User-facing error message in case something went wrong.</param>
    /// <param name="customNumberFormat">
    /// Custom formatting of columns. The key should be the number of the column from left to right. The value should be
    /// the format. For example: Key: 2 Value: "h:mm:ss AM/PM", meaning second column ("B" column) and format like
    /// 4:42:15 PM.
    /// </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Blocker Code Smell",
        "S2368:Public methods should not have multidimensional array parameters",
        Justification = "We are rendering a table.")]
    public static Stream CollectionToStream(
        string worksheetName,
        string[] columns,
        JsonNode[][] results,
        IStringLocalizer<ExcelDataTableExportService> localizer,
        string error = null,
        IDictionary<int, string> customNumberFormat = null)
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

        // Permit it to be null if we don't plan to use it anyway.
        var dateFormat = localizer == null ? string.Empty : localizer["mm\"/\"dd\"/\"yyyy"].Value;

        // Create table body.
        for (int i = 0; i < results.Length; i++)
        {
            var row = 2 + i;
            for (var c = 0; c < columns.Length; c++)
            {
                var cell = worksheet.Cell(row, c + 1);
                var value = results[i][c];
                CreateTableCell(cell, value, dateFormat, localizer);
            }
        }

        if (customNumberFormat != null)
        {
            foreach ((int columnNumber, string numberFormat) in customNumberFormat)
            {
                worksheet.Column(columnNumber).Style.NumberFormat.Format = numberFormat;
            }
        }

        // Make the content auto-fit the columns.
        worksheet.RecalculateAllFormulas();
        worksheet.Columns().AdjustToContents();

        return Save(workbook);
    }

    private static void CreateTableCell(IXLCell cell, JsonNode node, string dateFormat, IStringLocalizer localizer)
    {
        if (node.HasTypeProperty<ExportLink>())
        {
            var link = node.ToObject<ExportLink>();
            if (link != null) cell.FormulaA1 = $"HYPERLINK(\"{link.Url}\",\"{link.Text}\")";
        }
        else if (node.HasTypeProperty<ExportDate>())
        {
            var date = node.ToObject<ExportDate>();
            cell.Value = (DateTime)date!;
            cell.Style.DateFormat.Format = date?.ExcelFormat ?? dateFormat;
        }
        else if (node.HasTypeProperty<DateTimeJsonConverter.DateTimeTicks>())
        {
            cell.Value = node.ToObject<DateTimeJsonConverter.DateTimeTicks>().ToDateTime();
            cell.Style.DateFormat.Format = dateFormat;
        }
        else if (node is JsonArray array)
        {
            cell.Value = string.Join(", ", array.Select(item => item.ToString()));
        }
        else if (node is JsonValue value)
        {
            cell.Value = value.GetValueKind() switch
            {
                JsonValueKind.True => localizer["Yes"].Value,
                JsonValueKind.False => localizer["No"].Value,
                JsonValueKind.Undefined => string.Empty,
                JsonValueKind.Null => string.Empty,
                JsonValueKind.Number => value.ToString().Contains('.') ? value.Value<decimal>() : value.Value<int>(),
                _ => value.ToString(),
            };
        }
    }

    private static MemoryStream Save(XLWorkbook workbook)
    {
        var stream = new MemoryStream();

        workbook.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }
}
