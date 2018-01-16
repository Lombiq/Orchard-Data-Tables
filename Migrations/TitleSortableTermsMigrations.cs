using Lombiq.DataTables.Models;
using Orchard.Data.Migration;

namespace Lombiq.DataTables.Migrations
{
    public class TitleSortableTermsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder
                .CreateTable(nameof(TitleSortableTermContentItem), table => table
                    .Column<int>(nameof(TitleSortableTermContentItem.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(TitleSortableTermContentItem.Field), column => column.WithLength(50))
                    .Column<bool>(nameof(TitleSortableTermContentItem.IsFirstTerm))
                    .Column<int>("TermRecord_id")
                    .Column<int>("TitlePartRecord_id")
                    .Column<int>("TitleSortableTermsPartRecord_id"))
                .AlterTable(nameof(TitleSortableTermContentItem), table =>
                    {
                        table.CreateIndex("IDX_TitleSortableTermsPartRecord_id", "TitleSortableTermsPartRecord_id");
                        table.CreateIndex("IDX_TitleSortableTermsPartRecord_id_Field_IsFirstTerm",
                            "TitleSortableTermsPartRecord_id",
                            nameof(TitleSortableTermContentItem.Field),
                            nameof(TitleSortableTermContentItem.IsFirstTerm));
                    });

            SchemaBuilder.CreateTable(nameof(TitleSortableTermsPartRecord), table => table.ContentPartRecord());

            return 1;
        }
    }
}