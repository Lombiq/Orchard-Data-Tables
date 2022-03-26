using Lombiq.DataTables.Samples.Indexes;
using Lombiq.HelpfulLibraries.OrchardCore.Data;
using System;
using YesSql.Sql.Schema;

namespace Lombiq.DataTables.Samples.Migrations;

// The migration for the data table index must inherit from IndexDataMigration<TIndex> so they can all be registered
// together in Startup in a tightly coupled fashion that reduces the chance of mistakes.
public class EmployeeDataTableMigrations : IndexDataMigration<EmployeeDataTableIndex>
{
    protected override void CreateIndex(ICreateTableCommand table) =>
        table
            .Column<string>(nameof(EmployeeDataTableIndex.ContentItemId))
            .Column<string>(nameof(EmployeeDataTableIndex.Name))
            .Column<string>(nameof(EmployeeDataTableIndex.Position))
            .Column<string>(nameof(EmployeeDataTableIndex.Office))
            .Column<short?>(nameof(EmployeeDataTableIndex.Age))
            .Column<DateTime?>(nameof(EmployeeDataTableIndex.StartDate))
            .Column<int?>(nameof(EmployeeDataTableIndex.Salary));
}

// NEXT STATION: Services/SampleIndexBasedDataTableDataProvider.cs
