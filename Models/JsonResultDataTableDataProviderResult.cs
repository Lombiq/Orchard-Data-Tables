﻿using System.Collections.Generic;

namespace Lombiq.DataTables.Models
{
    public class JsonResultDataTableDataProviderResult
    {
        /// <summary>
        /// Gets or sets the results given by the provider.
        /// </summary>
        public IEnumerable<object> Results { get; set; }

        public bool IsPaginated { get; set; }

        public JsonResultDataTableDataProviderResult(IEnumerable<object> results = null) => Results = results;
    }
}
