using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Queries;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public class QueryDataTableDataProvider : IDataTableDataProvider
    {
        private readonly IQueryManager _queryManager;

        public string Name => nameof(QueryDataTableDataProvider);
        public LocalizedString Description => T["Query"];

        public IStringLocalizer T { get; set; }


        public QueryDataTableDataProvider(
            IQueryManager queryManager,
            IStringLocalizer<QueryDataTableDataProvider> stringLocalizer)
        {
            _queryManager = queryManager;
            T = stringLocalizer;
        }


        public Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId) => throw new NotImplementedException();

        public async Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync()
        {
            //var queryResult = await _queryManager.ExecuteQueryAsync(_query, _queryParameters);
            return new DataTableColumnsDefinition
            {
                //Columns = queryResult.Items.Select(x => new DataTableColumnDefinition()),
                //DefaultSortingColumnName = _defaultSortingColumnName,
                DefaultSortingDirection = Constants.SortingDirection.Ascending,
            };
        }

        public Task<DataTableRow> GetRowAsync(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            try
            {
                var query = await _queryManager.GetQueryAsync(request.QueryId);
                var isContentItem = query is OrchardCore.Lucene.LuceneQuery { ReturnContentItems: true };

                var parameters = new Dictionary<string, object>();
                var queryResult = await _queryManager.ExecuteQueryAsync(query, parameters);

                var columnDefinitions = new DataTableColumnDefinition[]
                    {
                        new DataTableColumnDefinition { Name = "Content.ContentItem.DisplayText", Orderable = true, Text = "Display Text" },
                        new DataTableColumnDefinition { Name = "Content.ContentItem.PublishedUtc", Orderable = true, Text = "Published UTC" },
                    };

                return new DataTableDataResponse
                {
                    Data = queryResult.Items
                    .Select((x, i) => new DataTableRow
                    {
                        Id = i,
                        ValuesDictionary = JObject.FromObject(x)
                    }),
                };
            }
            catch(Exception ex)
            {
                return new DataTableDataResponse
                {
                    Error = ex.Message,
                };
            }
        }
    }
}
