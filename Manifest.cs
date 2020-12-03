using OrchardCore.Modules.Manifest;
using static Lombiq.DataTables.Constants.FeatureIds;

[assembly: Module(
    Name = "Lombiq Data Tables",
    Author = "Lombiq Technologies",
    Version = "2.0",
    Description = "Module for displaying data using jQuery Data Tables.",
    Website = "https://lombiq.com"
)]

[assembly: Feature(
    Id = DataTables,
    Name = "Data Tables",
    Category = "Content",
    Description = "Displays data using jQuery Data Tables.",
    Dependencies = new[]
    {
        "OrchardCore.Contents",
        "OrchardCore.ResourceManagement",
        "OrchardCore.Queries",
    }
)]
