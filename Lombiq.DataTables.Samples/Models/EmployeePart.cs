using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace Lombiq.DataTables.Samples.Models;

public class EmployeePart : ContentPart
{
    public TextField Name { get; set; } = new();
    public TextField Position { get; set; } = new();
    public TextField Office { get; set; } = new();
    public NumericField Age { get; set; } = new();
    public DateField StartDate { get; set; } = new();
    public NumericField Salary { get; set; } = new();
}
