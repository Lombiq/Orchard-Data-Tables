using System.Collections.Generic;

namespace Lombiq.DataTables.Models;

public class JsonResultDataTableDataProviderResult(IEnumerable<object> results = null)
{
    /// <summary>
    /// Gets or sets the results given by the provider.
    /// </summary>
    public IEnumerable<object> Results { get; set; } = results;

    public bool IsPaginated { get; set; }
    public bool IsFiltered { get; set; }
    public int Count { get; set; } = -1;
    public int CountFiltered { get; set; } = -1;
}
