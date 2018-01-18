using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Services;
using System.Linq;

namespace Lombiq.DataTables.Projections
{
    public class DataTableSortCriterionProvider : ISortCriterionProvider
    {
        private readonly IHttpContextAccessor _hca;
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;
        private readonly IDataTableSortingProviderAccessor _dataTableSortingProviderAccessor;
        public Localizer T { get; set; }


        public DataTableSortCriterionProvider(
            IHttpContextAccessor hca, 
            IDataTableDataProviderAccessor dataTableDataProviderAccessor, 
            IDataTableSortingProviderAccessor dataTableSortingProviderAccessor)
        {
            _hca = hca;
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            _dataTableSortingProviderAccessor = dataTableSortingProviderAccessor;

            T = NullLocalizer.Instance;
        }


        public void Describe(DescribeSortCriterionContext describe)
        {
            describe
                .For("Data Table", T("Data Table"), T("Data table sort criteria"))
                .Element(
                    "Data Table Column",
                    T("Data Table Column"),
                    T("Orders Data Table columns by the parameters given in the query string of the request."),
                    context =>
                    {
                        var request = _hca.Current().Request;

                        var dataProvider = _dataTableDataProviderAccessor.GetDataProvider(request.QueryString["dataProvider"]);

                        if (dataProvider == null) return;

                        var columnIndexValue = request.QueryString["order[0][column]"] ?? "";
                        var columnName = request.QueryString[$"columns[{columnIndexValue}][name]"];
                        var direction = request.QueryString["order[0][dir]"];

                        if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(direction)) return;

                        var columnDefinition = dataProvider.GetColumns().FirstOrDefault(column => column.Name == columnName);

                        if (columnDefinition == null) return;

                        var sortingProvider = _dataTableSortingProviderAccessor.GetSortingProvider(columnDefinition.DataSource);

                        if (sortingProvider == null) return;

                        sortingProvider.Sort(new DataTableSortingContext
                        {
                            ColumnDefinition = columnDefinition,
                            Direction = direction == "asc" ? SortingDirection.Asc : SortingDirection.Desc,
                            Query = context.Query
                        });
                    },
                    context => T("Columns ordered by the Data Table sorting parameters in the query string.")
                );
        }
    }
}