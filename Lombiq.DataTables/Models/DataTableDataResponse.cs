using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lombiq.DataTables.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class DataTableDataResponse
{
    /// <summary>
    /// Gets or sets the request identifier for the jQuery.DataTables plugin. This needs to be parsed and sent back in
    /// order to prevent Cross Site Scripting (XSS) attack. See https://datatables.net/manual/server-side for more info.
    /// </summary>
    /// <remarks>
    /// <para>For internal use only. It's overwritten during normal use.</para>
    /// </remarks>
    [JsonInclude]
    internal int Draw { get; set; }

    /// <summary>
    /// Gets or sets the extra informational field that shows the actual total if filtering (such as keyword
    /// search) is used. When not filtering it must be the same as <see cref="RecordsFiltered"/>.
    /// </summary>
    public int RecordsTotal { get; set; }

    /// <summary>
    /// Gets or sets the total number of results; used for paging.
    /// </summary>
    public int RecordsFiltered { get; set; }

    /// <summary>
    /// Gets or sets the table contents of the current page.
    /// </summary>
    public IEnumerable<DataTableRow> Data { get; set; }

    /// <summary>
    /// Gets or sets the user-facing error message in case something went wrong.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Creates a new response with only the <see cref="Error"/> filed set.
    /// </summary>
    /// <param name="errorText">The text to display.</param>
    public static DataTableDataResponse ErrorResult(string errorText) =>
        new()
        {
            Error = errorText,
            Data = Array.Empty<DataTableRow>(),
            RecordsFiltered = 0,
            RecordsTotal = 0,
        };

    /// <summary>
    /// Creates a new response with empty <see cref="Data"/>.
    /// </summary>
    public static DataTableDataResponse Empty() => ErrorResult(errorText: null);
}
