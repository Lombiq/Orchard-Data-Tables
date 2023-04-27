using Lombiq.DataTables.Models;
using Orchard;
using Orchard.Indexing;

namespace Lombiq.DataTables.Services
{
    public interface IIndexedDataTableDataProvider : IDependency
    {
        string Name { get; }
        
        ISearchBuilder GetSearchBuilder();
        DataTableRow GetRow(ISearchHit result);

        DataTableDataResponse GetRows(DataTableDataRequest request);

        DataTableRow GetRows(string indexName);

        DataTableColumnsDefinition GetColumnsDefinition();
    }
}