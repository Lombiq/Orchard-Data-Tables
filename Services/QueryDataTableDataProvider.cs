using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lombiq.DataTables.Constants;

namespace Lombiq.DataTables.Services
{
    public class QueryDataTableDataProvider : IDataTableDataProvider
    {
        private readonly IQueryManager _queryManager;
        private readonly IContentManager _contentManager;
        private readonly IStringLocalizer T;

        public string Name => nameof(QueryDataTableDataProvider);
        public LocalizedString Description => T["Query"];


        public QueryDataTableDataProvider(
            IQueryManager queryManager,
            IContentManager contentManager,
            IStringLocalizer<QueryDataTableDataProvider> stringLocalizer)
        {
            _queryManager = queryManager;
            _contentManager = contentManager;
            T = stringLocalizer;
        }


        public Task<DataTableChildRowResponse> GetChildRowAsync(int id) => throw new NotImplementedException();
        public Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) => throw new NotImplementedException();


        public async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            try
            {
                var query = await _queryManager.GetQueryAsync(request.QueryId);
                var isContentItem = query is OrchardCore.Lucene.LuceneQuery { ReturnContentItems: true };

                var order = request.Order.FirstOrDefault();
                var parameters = new Dictionary<string, object>
                {
                    ["from"] = request.Start,
                    ["size"] = request.Length,
                    ["sort"] = order?.Column?.Replace('_', '.') ?? "Content.ContentItem.PublishedUtc",
                    ["order"] = order?.Direction == SortingDirection.Descending ? "desc" : "asc",
                };
                var queryResult = await _queryManager.ExecuteQueryAsync(query, parameters);

                var items = isContentItem
                    ? await Task.WhenAll(queryResult.Items.Cast<ContentItem>().Select(_contentManager.LoadAsync))
                    : queryResult.Items.ToArray();
                var count = queryResult is LuceneQueryResults lucene ? lucene.Count : items.Length;

                var result = new DataTableDataResponse
                {
                    Data = items.Select((x, i) => new DataTableRow
                    {
                        Id = i,
                        ValuesDictionary = (JObject.FromObject(x) as IDictionary<string, JToken>)
                            .ToDictionary(x => x.Key.Replace(".", "_"), x => x.Value)
                    }),
                    RecordsTotal = count,
                    RecordsFiltered = count,
                };
                return result;
            }
            catch (Exception ex)
            {
                return new DataTableDataResponse { Error = ex.Message, };
            }
        }
    }
}
