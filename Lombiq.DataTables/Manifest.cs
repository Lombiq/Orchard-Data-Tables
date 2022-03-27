using OrchardCore.Modules.Manifest;
using static Lombiq.DataTables.Constants.FeatureIds;

[assembly: Module(
    Name = "Lombiq Data Tables",
    Author = "Lombiq Technologies",
    Version = "3.0.2",
    Description = "Module for displaying data using jQuery Data Tables.",
    Website = "https://github.com/Lombiq/Orchard-Data-Tables"
)]

[assembly: Feature(
    Id = DataTables,
    Name = "Lombiq Data Tables",
    Category = "Content",
    Description = "Displays data using jQuery Data Tables.",
    Dependencies = new[]
    {
        "OrchardCore.Contents",
        "OrchardCore.ResourceManagement",
        "OrchardCore.Queries",
    }
)]
