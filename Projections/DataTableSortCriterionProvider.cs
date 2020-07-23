using System.Linq;
using System.Web;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Lombiq.DataTables.Projections
{
    public class DataTableSortCriterionProvider : ISortCriterionProvider
    {
        private readonly IHttpContextAccessor _hca;
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;
        private readonly IDataTableSortingProviderAccessor _dataTableSortingProviderAccessor;
        public IStringLocalizer T { get; }


        public DataTableSortCriterionProvider(
            IHttpContextAccessor hca, 
            IDataTableDataProviderAccessor dataTableDataProviderAccessor, 
            IDataTableSortingProviderAccessor dataTableSortingProviderAccessor,
            IStringLocalizer<DataTableSortCriterionProvider> stringLocalizer)
        {
            _hca = hca;
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            _dataTableSortingProviderAccessor = dataTableSortingProviderAccessor;
            T = stringLocalizer;
        }


        public void Describe(DescribeSortCriterionContext describe)
        {
            describe
                .For("Data Table", T["Data Table"], T["Data table sort criteria"])
                .Element(
                    "Data Table Column",
                    T["Data Table Column"],
                    T["Orders Data Table columns by the parameters given in the query string of the request."],
                    context =>
                    {
                        var query = HttpUtility.ParseQueryString(_hca.HttpContext?.Request?.QueryString.Value ?? "?");

                        var dataProvider = _dataTableDataProviderAccessor.GetDataProvider(query["dataProvider"]);

                        if (dataProvider == null) return;
                        
                        var columnName = query["order[0][column]"];
                        var direction = query["order[0][direction]"];

                        if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(direction)) return;

                        var columnDefinition = dataProvider.GetColumnsDefinition().Columns.FirstOrDefault(column => column.Name == columnName);

                        if (columnDefinition == null) return;

                        var sortingProvider = _dataTableSortingProviderAccessor.GetSortingProvider(columnDefinition.DataSource);

                        if (sortingProvider == null) return;

                        sortingProvider.Sort(new DataTableSortingContext
                        {
                            ColumnDefinition = columnDefinition,
                            Direction = direction == "ascending" ? SortingDirection.Ascending : SortingDirection.Descending,
                            Query = context.Query
                        });
                    },
                    context => T["Columns ordered by the Data Table sorting parameters in the query string."]
                );
        }
    }
}