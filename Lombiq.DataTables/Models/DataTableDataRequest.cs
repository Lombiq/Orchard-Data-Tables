using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

public class DataTableDataRequest
{
    public string QueryId { get; set; }

    public string DataProvider { get; set; }

    [JsonRequired]
    public int Draw { get; set; }

    [JsonRequired]
    public int Start { get; set; }

    [JsonRequired]
    public int Length { get; set; }

    public IEnumerable<DataTableColumn> ColumnFilters { get; set; }

    public DataTableSearchParameters Search { get; set; }

    public IEnumerable<DataTableOrder> Order { get; set; }

    public bool HasSearch => !string.IsNullOrWhiteSpace(Search?.Value);

    public IReadOnlyCollection<DataTableColumn> GetColumnSearches() =>
        ColumnFilters?
            .Where(filter => !string.IsNullOrWhiteSpace(filter.Search?.Value))
            .ToList() ?? [];
}
