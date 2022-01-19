using Lombiq.DataTables.Samples.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System;
using static Lombiq.DataTables.Samples.Constants.ContentTypes;

namespace Lombiq.DataTables.Samples.Migrations
{
    // Just the bare minimum to set up the content type for storing the sample data.
    public class EmployeeMigrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public EmployeeMigrations(IContentDefinitionManager contentDefinitionManager) => _contentDefinitionManager = contentDefinitionManager;

        public int Create()
        {
            _contentDefinitionManager.AlterPartDefinition(nameof(EmployeePart), part => part
                .WithField(nameof(EmployeePart.Name), ConfigureField<TextField>(name: "Name"))
                .WithField(nameof(EmployeePart.Position), ConfigureField<TextField>(name: "Position"))
                .WithField(nameof(EmployeePart.Office), ConfigureField<TextField>(name: "Office Location"))
                .WithField(nameof(EmployeePart.Age), ConfigureField<NumericField>(name: "Age"))
                .WithField(nameof(EmployeePart.StartDate), ConfigureField<DateField>(name: "Start Date"))
                .WithField(nameof(EmployeePart.Salary), ConfigureField<NumericField>(name: "Salary")));

            _contentDefinitionManager.AlterTypeDefinition(Employee, type => type
                .Listable()
                .WithPart(nameof(EmployeePart)));

            return 1;
        }

        private static Action<ContentPartFieldDefinitionBuilder> ConfigureField<T>(string name) =>
            field => field.OfType(typeof(T).Name).WithDisplayName(name);
    }
}
