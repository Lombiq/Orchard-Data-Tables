using ClosedXML.Excel;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Lombiq.Tests.Helpers;
using Moq.AutoMock;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.DataTables.Tests.UnitTests.Services
{
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

            var service = MockHelper.CreateAutoMockerInstance<ExcelDataTableExportService>(
                mocker => mocker.MockStringLocalizer<ExcelDataTableExportService>());
            var provider = (IDataTableDataProvider)new MockDataProvider(dataSet,
                new DataTableColumnsDefinition()
                {
                    Columns = columns
                        .Select(column => new DataTableColumnDefinition
                        {
                            Name = column.Name, Text = column.Text, Exportable = column.Exportable,
                        })
                        .ToList(),
                });
            var request = new DataTableDataRequest
            {
                DataProvider = provider.Name,
                Length = length,
                Start = start,
                Order = new[] { new DataTableOrder { Column = orderColumnIndex.ToString() } },
            };

            var stream = await service.ExportAsync(provider, request);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.Worksheet(1);

            Enumerable.Range(1, pattern[0].Length)
                .Select(index => worksheet.Cell(1, index).GetString())
                .ToArray()
                .ShouldBe(columns.Where(column => column.Exportable).Select(column => column.Text).ToArray());

            for (var rowIndex = 0; rowIndex < pattern.Length; rowIndex++)
            {
                Enumerable.Range(1, pattern[0].Length)
                    .Select(index => worksheet.Cell(2 + rowIndex, index).GetString())
                    .ToArray()
                    .ShouldBe(pattern[rowIndex], $"Row {rowIndex + 1} didn't match expectation.");
            }
        }

        public static IEnumerable<object[]> Data()
        {
            var dataset = new[]
            {
                new object[] { 1, "z", "foo" }, new object[] { 2, "y", "bar" }, new object[] { 3, "x", "baz" },
            };
            var columns = new[]
            {
                ("Num", "Numbers", true), ("Letters", "Letters", true), ("MagicWords", "Magic Words", true),
            };

            yield return new object[]
            {
                "Show full data set.",
                dataset,
                columns,
                "1,z,foo;2,y,bar;3,x,baz".Split(';').Select(row => row.Split(',')).ToArray(),
                0,
                10,
                0,
            };


            yield return new object[]
            {
                "Make last column not exportable.",
                dataset,
                new[] { ("Num", "Numbers", true), ("Letters", "Letters", true), ("MagicWords", "Magic Words", false) },
                "1,z;2,y;3,x".Split(';').Select(row => row.Split(',')).ToArray(),
                0,
                10,
                0,
            };

            yield return new object[]
            {
                "Test pagination.",
                dataset,
                columns,
                new[] { "3,x,baz".Split(',') },
                2,
                10,
                0,
            };

            yield return new object[]
            {
                "Test sorting on 2nd column.",
                dataset,
                columns,
                "3,x,baz;2,y,bar;1,z,foo".Split(';').Select(row => row.Split(',')).ToArray(),
                0,
                10,
                1,
            };

            yield return new object[]
            {
                "Test sorting on 3nd column.",
                dataset,
                columns,
                "2,y,bar;3,x,baz;1,z,foo".Split(';').Select(row => row.Split(',')).ToArray(),
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
        }
    }
}
