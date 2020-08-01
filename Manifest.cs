using OrchardCore.Modules.Manifest;
using static Lombiq.DataTables.Constants.ResourceNames;

[assembly: Module(
    Name = "Data Tables",
    Author = "The Orchard Team",
    Version = "2.0",
    Description = "Module for displaying data from Projections using jQuery Data Tables."
)]


[assembly: Feature(
    Id = Lombiq_DataTables,
    Name = "Data Tables",
    Category = "Content",
    Description = "Module for displaying data from Queries using jQuery Data Tables.",
    Dependencies = new[]
    {
        "OrchardCore.Contents",
        "OrchardCore.ResourceManagement",
        "OrchardCore.Queries"
    }
)]
