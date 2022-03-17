using ClosedXML.Excel;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.Tests.Helpers;
using Lombiq.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq.AutoMock;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.DataTables.Tests.UnitTests.Services;

public class ExportTests
{
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
                customNumberFormat = new Dictionary<int, string>
                {
                    [columnIndex + 1] = "h:mm:ss AM/PM",
                };
            }

            columnIndex++;
        }
        while (columnIndex < columns.Length && columns[columnIndex - 1].Name != "Time");

        var stream = await service.ExportAsync(provider, request, customNumberFormat: customNumberFormat);

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
                .Select(index => worksheet.Cell(2 + rowIndex, index).Value switch
                {
                    XLHyperlink hyperlink => hyperlink.Tooltip,
                    { } value => value.ToString(),
                    null => "NULL",
                })
                .ToArray()
                .ShouldBe(
                    pattern[rowIndex],
                    FormattableString.Invariant($"Row {rowIndex + 1} didn't match expectation."));
        }
    }

    public static IEnumerable<object[]> Data()
    {
        var dataset = new[]
        {
            new object[] { 1, "z", "foo" },
            new object[] { new ExportLink("http://example.com/", 2), "y", "bar" },
            new object[] { 10, "x", "baz" },
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
            new[] { ("Num", "Numbers", true), ("Letters", "Letters", true), ("MagicWords", "Magic Words", false) },
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
            new[] { new object[] { 1, true }, new object[] { 2, true }, new object[] { 3, false } },
            new[] { ("Num", "Numbers", true), ("Bool", "Booleans", true) },
            "1,Yes;2,Yes;3,No".Split(';').Select(row => row.Split(',')).ToArray(),
            0,
            10,
            0,
        };

        var date1 = new DateTime(2020, 11, 26, 23, 42, 01);
        var date2 = new DateTime(2020, 11, 26, 13, 42, 01);
        var date3 = new DateTime(2020, 11, 26, 1, 42, 01);

        // The date value should be the same, only the formatting changes.
        yield return new object[]
        {
            "Verify custom number formatting.",
            new[]
            {
                new object[] { 1, date1.ToString(CultureInfo.InvariantCulture) },
                new object[] { 2, date2.ToString(CultureInfo.InvariantCulture) },
                new object[] { 3, date3.ToString(CultureInfo.InvariantCulture) },
            },
            new[] { ("Num", "Numbers", true), ("Time", "Time", true) },
            FormattableString.CurrentCulture($"1,{date1};2,{date2};3,{date3}")
                .Split(';')
                .Select(row => row.Split(','))
                .ToArray(),
            0,
            10,
            0,
        };
    }
}
