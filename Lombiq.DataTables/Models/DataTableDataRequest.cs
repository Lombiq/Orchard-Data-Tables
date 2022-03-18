using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class DataTableDataRequest
{
    public string QueryId { get; set; }
    public string DataProvider { get; set; }
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public IEnumerable<DataTableColumn> ColumnFilters { get; set; }
    public DataTableSearchParameters Search { get; set; }
    public IEnumerable<DataTableOrder> Order { get; set; }

    public bool HasSearch => !string.IsNullOrWhiteSpace(Search?.Value);

    public IReadOnlyCollection<DataTableColumn> GetColumnSearches() =>
        ColumnFilters?
            .Where(filter => !string.IsNullOrWhiteSpace(filter.Search?.Value))
            .ToList() ?? new List<DataTableColumn>();
}
