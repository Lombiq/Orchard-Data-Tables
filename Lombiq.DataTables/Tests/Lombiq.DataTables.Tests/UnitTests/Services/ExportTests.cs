using ClosedXML.Excel;
using ClosedXML.Graphics;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.Tests.Helpers;
using Lombiq.HelpfulLibraries.Common.Utilities;
using Lombiq.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq.AutoMock;
using Shouldly;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.DataTables.Tests.UnitTests.Services;

public class ExportTests
{
    // ClosedXML looks at the CurrentCulture to initialize the workbook's culture.
    private static readonly CultureInfo _worksheetCulture = new("en-US", useUserOverride: false);

    [Theory]
    [MemberData(nameof(Data))]
    public async Task ExportTableShouldMatchExpectation(
        string note,
        object[][] dataSet,
        (string Name, string Text, bool Exportable)[] columns,
        string[][] pattern,
        int start,
        int length,
        int orderColumnIndex)
    {
        // ClosedXML looks at the CurrentCulture to initialize the workbook's culture. They also to set it like this in
        // their own unit tests. See:
        // https://github.com/ClosedXML/ClosedXML/blob/c2d408b/ClosedXML_Tests/Excel/CalcEngine/LookupTests.cs#L16-L17
        // Since this is an async method, it runs in its own thread; so this has no effect on other tests.
        Thread.CurrentThread.CurrentCulture = _worksheetCulture;

        note.ShouldNotBeEmpty("Please state the purpose of this input set!");

        using var memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        var (provider, request) = MockDataProviderHelper.GetProviderAndRequest(
            dataSet,
            columns,
            start,
            length,
            orderColumnIndex,
            memoryCache);

        var service = MockHelper.CreateAutoMockerInstance<ExcelDataTableExportService>(
            mocker => mocker.MockStringLocalizer<ExcelDataTableExportService>());

        Dictionary<int, string> customNumberFormat = null;
        int columnIndex = 0;
        do
        {
            if (columns[columnIndex].Name == "Time")
            {
                customNumberFormat = new Dictionary<int, string> { [columnIndex + 1] = "h:mm:ss AM/PM" };
            }

            columnIndex++;
        }
        while (columnIndex < columns.Length && columns[columnIndex - 1].Name != "Time");

        Stream stream = null;

        stream = OperatingSystem.IsOSPlatform(nameof(OSPlatform.Windows))
            // On non-Windows platforms, we need to specify a fallback font manually for ClosedXML to work.
            ? await service.ExportAsync(provider, request, customNumberFormat: customNumberFormat)
            : await TryExportWithFallbackFontsAsync(service, provider, request, customNumberFormat);

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.Worksheet(1);

        Enumerable.Range(1, pattern[0].Length)
            .Select(index => worksheet.Cell(1, index).GetString())
            .ToArray()
            .ShouldBe(columns.Where(column => column.Exportable).Select(column => column.Text).ToArray());

        if (columns[columnIndex - 1].Name == "Time")
        {
            for (var rowIndex = 0; rowIndex < pattern.Length; rowIndex++)
            {
                worksheet.Cell(2 + rowIndex, columnIndex).Style.NumberFormat.Format.ShouldBe("h:mm:ss AM/PM");
            }
        }

        for (var rowIndex = 0; rowIndex < pattern.Length; rowIndex++)
        {
            Enumerable.Range(1, pattern[0].Length)
                .Select(index => worksheet.Cell(2 + rowIndex, index).GetFormattedString())
                .ToArray()
                .ShouldBe(
                    pattern[rowIndex],
                    StringHelper.CreateInvariant($"Row {rowIndex + 1} didn't match expectation."));
        }
    }

    public static IEnumerable<object[]> Data()
    {
        var dataset = new[]
        {
            [
                1,
                "z",
                "foo",
            ],
            [
                new ExportLink("http://example.com/", 2),
                "y",
                "bar",
            ],
            new object[]
            {
                10,
                "x",
                "baz",
            },
        };

        var columns = new[]
        {
            ("Num", "Numbers", true),
            ("Letters", "Letters", true),
            ("MagicWords", "Magic Words", true),
        };

        yield return new object[]
        {
            "Show full data set.",
            dataset,
            columns,
            "1,z,foo;2,y,bar;10,x,baz".Split(';').Select(row => row.Split(',')).ToArray(),
            0,
            10,
            0,
        };

        yield return new object[]
        {
            "Make last column not exportable.",
            dataset,
            new[]
            {
                ("Num", "Numbers", true),
                ("Letters", "Letters", true),
                ("MagicWords", "Magic Words", false),
            },
            "1,z;2,y;10,x".Split(';').Select(row => row.Split(',')).ToArray(),
            0,
            10,
            0,
        };

        yield return new object[]
        {
            "Test pagination.",
            dataset,
            columns,
            new[] { "10,x,baz".Split(',') },
            2,
            10,
            0,
        };

        yield return new object[]
        {
            "Test sorting on 2nd column.",
            dataset,
            columns,
            "10,x,baz;2,y,bar;1,z,foo".Split(';').Select(row => row.Split(',')).ToArray(),
            0,
            10,
            1,
        };

        yield return new object[]
        {
            "Test sorting on 3nd column.",
            dataset,
            columns,
            "2,y,bar;10,x,baz;1,z,foo".Split(';').Select(row => row.Split(',')).ToArray(),
            0,
            10,
            2,
        };

        yield return new object[]
        {
            "Verify boolean formatting.",
            new[]
            {
                [1, true],
                [2, true],
                new object[] { 3, false },
            },
            new[] { ("Num", "Numbers", true), ("Bool", "Booleans", true) },
            "1,Yes;2,Yes;3,No".Split(';').Select(row => row.Split(',')).ToArray(),
            0,
            10,
            0,
        };

        var date1 = new DateTime(2020, 11, 26, 23, 42, 01, DateTimeKind.Utc);
        var date2 = new DateTime(2020, 11, 26, 13, 42, 01, DateTimeKind.Utc);
        var date3 = new DateTime(2020, 11, 26, 1, 42, 01, DateTimeKind.Utc);

        // The date value should be the same, only the formatting changes.
#pragma warning disable IDE0300 // Simplify collection initialization
        // This use case would prevent the 3rd object to be converted into a simplified collection.
        yield return new object[]
        {
            "Verify custom number formatting.",
            new[]
            {
                new object[] { 1, date1 },
                new object[] { 2, date2 },
                new object[] { 3, date3 },
            },
            new[] { ("Num", "Numbers", true), ("Time", "Time", true) },
            string.Format(
                    _worksheetCulture,
                    "1,{0:h:mm:ss tt};2,{1:h:mm:ss tt};3,{2:h:mm:ss tt}",
                    date1,
                    date2,
                    date3)
                .Split(';')
                .Select(row => row.Split(','))
                .ToArray(),
            0,
            10,
            0,
        };
#pragma warning restore IDE0300 // Simplify collection initialization
    }

    // Sometimes a font is available, however, it's corrupted or missing a table (for example, this can happen on
    // GitHub-hosted runners). We can't check directly if a font is missing a table or corrupted, but we can try
    // other fonts if this happens.
    private static async Task<Stream> TryExportWithFallbackFontsAsync(
        ExcelDataTableExportService service,
        IDataTableDataProvider provider,
        DataTableDataRequest request,
        IDictionary<int, string> customNumberFormat)
    {
        var fontFamilies = SystemFonts.Collection.Families.ToArray();

        var maxAttempts = Math.Min(3, fontFamilies.Length);

        for (int i = 0; i < maxAttempts; i++)
        {
            var fallbackFont = fontFamilies[i].Name;

            LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine(fallbackFont);

            try
            {
                return await service.ExportAsync(provider, request, customNumberFormat: customNumberFormat);
            }
            catch (MissingFontTableException missingFontTableException)
            {
                DebugHelper.WriteLineTimestamped(
                    $"Attempt {(i + 1).ToTechnicalString()} of exporting the data table with the font " +
                    $"{fallbackFont} failed with the MissingFontTableException: {missingFontTableException.Message}.");

                if (i + 1 < maxAttempts)
                {
                    DebugHelper.WriteLineTimestamped("Retrying with a different font...");
                }
                else
                {
                    throw;
                }
            }
        }

        return null;
    }
}
