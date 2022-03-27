using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Lombiq Data Tables - Samples",
    Author = "Lombiq Technologies",
    Website = "https://github.com/Lombiq/Orchard-Data-Tables",
    Version = "3.0.2",
    Description = "Samples for Lombiq Data Tables.",
    Category = "Development",
    Dependencies = new[] { Lombiq.DataTables.Constants.FeatureIds.DataTables }
)]
