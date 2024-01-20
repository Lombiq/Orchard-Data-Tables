using Lombiq.DataTables.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Migrations;

public class ColumnsDefinitionMigrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
{
    public async Task<int> CreateAsync()
    {
        await contentDefinitionManager.AlterTypeDefinitionAsync("ColumnsDefinition", type => type
            .Creatable()
            .Securable()
            .WithPart(nameof(DataTableColumnsDefinitionPart))
        );

        return 1;
    }
}
