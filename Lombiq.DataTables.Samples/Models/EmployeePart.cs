using OrchardCore.ContentManagement;
using System;

namespace Lombiq.DataTables.Samples.Models
{
    public class EmployeePart : ContentPart
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Office { get; set; }
        public int Age { get; set; }
        public DateTime StartDateUtc { get; set; }
        public int Salary { get; set; }
    }
}
