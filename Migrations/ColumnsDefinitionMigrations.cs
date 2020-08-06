using Lombiq.DataTables.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace Lombiq.DataTables.Migrations
{
    public class ColumnsDefinitionMigrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;


        public ColumnsDefinitionMigrations(IContentDefinitionManager contentDefinitionManager) =>
            _contentDefinitionManager = contentDefinitionManager;


        public int Create()
        {
            _contentDefinitionManager.AlterTypeDefinition("ColumnsDefinition", type => type
                .Creatable()
                .Securable()
                .WithPart(nameof(DataTableColumnsDefinitionPart))
            );

            return 1;
        }
    }
}
