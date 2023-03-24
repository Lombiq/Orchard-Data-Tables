using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Indexing;
using Orchard.Projections.Models;
using System.Collections.Generic;

namespace Lombiq.DataTables.Services
{
    public interface IIndexedDataTableDataProvider : IDependency
    {
        string IndexName { get; }

        List<int> GetContentItemVersionIds(int queryId);

        int GetCount(QueryPartRecord queryPartRecord, IEnumerable<IHqlQuery> contentQueries);

        QueryPartRecord GetQueryPartRecordByQueryId(int queryId);

        IEnumerable<IHqlQuery> GetContentQueries(int queryId);

        IEnumerable<int> Sajt<TRecord>(int queryId, int skip, int count) where TRecord : ContentPartRecord;

        ISearchBuilder GetSearchBuilder(string indexName);

        IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder);
        IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder, int skip, int count, string remoteTitle, string thoughtLeaderManager);

        IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder, int skip, int count);

        IEnumerable<ISearchHit> GetSearchResults(ISearchBuilder searchBuilder, IEnumerable<int> ids, int skip, int count);
    }
}
