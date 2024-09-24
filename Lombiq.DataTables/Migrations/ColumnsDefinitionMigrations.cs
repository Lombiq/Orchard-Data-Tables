using Lombiq.DataTables.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Migrations;

public sealed class ColumnsDefinitionMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public ColumnsDefinitionMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public async Task<int> CreateAsync()
    {
        await _contentDefinitionManager.AlterTypeDefinitionAsync("ColumnsDefinition", type => type
            .Creatable()
            .Securable()
            .WithPart(nameof(DataTableColumnsDefinitionPart))
        );

        return 1;
    }
}
