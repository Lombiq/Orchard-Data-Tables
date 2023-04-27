using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Indexing;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Search.Services;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Lombiq.DataTables.Services
{
    public class IndexedDataTableSearchService : IIndexedDataTableSearchService
    {
        private readonly IIndexManager _indexManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISearchService _searchService;
        private readonly IProjectionManager _projectionManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IRepository<QueryPartRecord> _queryPartRecordRepository;

        public IndexedDataTableSearchService(
            IIndexManager indexManager,
            ISearchService searchService,
            IProjectionManager projectionManager,
            ITransactionManager transactionManager,
            IWorkContextAccessor workContextAccessor,
            IRepository<QueryPartRecord> queryPartRecordRepository)
        {
            _indexManager = indexManager;
            _searchService = searchService;
            _projectionManager = projectionManager;
            _transactionManager = transactionManager;
            _workContextAccessor = workContextAccessor;
            _queryPartRecordRepository = queryPartRecordRepository;
        }

        public List<int> GetContentItemVersionIds(int queryId)
        {
            return GetContentQueries(queryId).SelectMany(x => x.ListIds()).Distinct().ToList();
        }

        public int GetCount(QueryPartRecord queryPartRecord, IEnumerable<IHqlQuery> contentQueries) =>
            queryPartRecord.FilterGroups.Count > 1 ?
                contentQueries.SelectMany(contentQuery => contentQuery.ListIds()).Distinct().Count() :
                contentQueries.Sum(contentQuery => contentQuery.Count());

        public QueryPartRecord GetQueryPartRecordByQueryId(int queryId) =>
            _queryPartRecordRepository.Get(queryId);

        public IEnumerable<IHqlQuery> GetContentQueries(int queryId)
        {
            var queryPartRecord = GetQueryPartRecordByQueryId(queryId);

            return _projectionManager.GetContentQueries(
                queryPartRecord,
                queryPartRecord.SortCriteria.OrderBy(sc => sc.Position),
                new Dictionary<string, object>());
        }

        public IEnumerable<int> Sajt<TRecord>(int queryId, int skip, int count) where TRecord : ContentPartRecord
        {
            var contentQueries = GetContentQueries(queryId);

            var hqlQuery = contentQueries.First();

            var session = _transactionManager.GetSession();

            const string hql = "select ContentItemVersionRecord.ContentItemRecord.Id " +
                "from Orchard.ContentManagement.Records.ContentItemVersionRecord ContentItemVersionRecord " +
                "where ContentItemVersionRecord.Id in (:ids)";

            var query = session.CreateQuery(hql);
            var queryContentItemVersionIds = contentQueries.SelectMany(x => x.ListIds()).Distinct().ToList();

            query.SetParameterList("ids", queryContentItemVersionIds.Skip(skip).Take(count).ToArray());

            if (skip != 0)
            {
                query.SetFirstResult(skip);
            }
            if (count != 0 && count != int.MaxValue)
            {
                query.SetMaxResults(count);
            }

            var contentPartRecordIds = query.List<int>();

            return hqlQuery.Where(
                alias => alias
                    .ContentPartRecord<TRecord>(),
                filter => filter.In("Id", contentPartRecordIds.ToArray())).ListIds();
        }

        public ISearchBuilder GetSearchBuilder(string indexName) =>
            _indexManager.HasIndexProvider()
                ? _indexManager.GetSearchIndexProvider().CreateSearchBuilder(indexName)
                : new NullSearchBuilder();

        public IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder, NameValueCollection searchCollection)
        {
            return null;
        }

        public IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder, int skip, int count, string remoteTitle, string thoughtLeaderManager)
        {
            if (!string.IsNullOrEmpty(remoteTitle))
            {
                searchBuilder.WithField("EventPart.RemoteTitle", remoteTitle, false).NotAnalyzed().ExactMatch().Mandatory();
            }

            if (!string.IsNullOrEmpty(thoughtLeaderManager))
            {
                searchBuilder.WithField("EventPart.ThoughtLeaderManager", thoughtLeaderManager, false).NotAnalyzed().ExactMatch().Mandatory();
            }

            return GetSearchResults(searchBuilder, skip, count);
        }

        public IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder, int skip, int count) =>
            GetSearchResults(searchBuilder.Slice(skip, count));

        public IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder) =>
            searchBuilder.Search();

        public IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder, IEnumerable<int> ids, int skip, int count)
        {
            foreach (var id in ids)
            {
                searchBuilder.WithField("id", id.ToString());
            }

            searchBuilder = searchBuilder.Slice(skip, count);

            return searchBuilder.Search();
        }
    }
}