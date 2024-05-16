using System.Diagnostics.CodeAnalysis;

namespace Lombiq.DataTables.Tests.UnitTests.Services;

[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is just an input class for a unit test.")]
public class DataTableShouldMatchExpectationInput(
    string note,
    object[][] dataSet,
    (string Name, string Text, bool Exportable)[] columns,
    string[][] pattern,
    int start,
    int length,
    int orderColumnIndex)
{
    public string Note { get; set; } = note;
    public object[][] DataSet { get; set; } = dataSet;
    public (string Name, string Text, bool Exportable)[] Columns { get; set; } = columns;
    public string[][] Pattern { get; set; } = pattern;
    public int Start { get; set; } = start;
    public int Length { get; set; } = length;
    public int OrderColumnIndex { get; set; } = orderColumnIndex;
}
