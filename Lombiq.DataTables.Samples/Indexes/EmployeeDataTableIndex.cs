using System;
using YesSql.Indexes;

namespace Lombiq.DataTables.Samples.Indexes;

// An index-based provider needs its own dedicated index which represents the final computed or joined data to be
// displayed in the table. During display you only need to query this index alone to get all table data. However, to
// make it happen you must update the index with an index generator.
public class EmployeeDataTableIndex : MapIndex
{
    // If you are coming from the SampleJsonResultDataTableDataProvider, you can see this is very similar to the
    // EmployeeJsonResult there, because it serves the same purpose. This one doesn't have the Actions property which
    // only contains additional data related to the user visiting the page. Also the numeric types are smaller to
    // conserve space in the database. This can be very important on large sets.
    public string ContentItemId { get; set; }

    public string Name { get; set; }
    public string Position { get; set; }
    public string Office { get; set; }
    public short? Age { get; set; }
    public DateTime? StartDate { get; set; }
    public int? Salary { get; set; }
}

// NEXT STATION: Services/EmployeeDataTableIndexGenerator.cs
