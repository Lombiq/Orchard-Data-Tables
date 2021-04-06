using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;
using YesSql.Sql;

namespace Lombiq.DataTables.Services
{
    public abstract class IndexBasedDataTableDataProvider<TIndex> : IDataTableDataProvider
        where TIndex : MapIndex
    {
        protected readonly ISession _session;

        public IndexBasedDataTableDataProvider(ISession session)
        {
            _session = session;
        }

        public abstract LocalizedString Description { get; }
        public abstract IEnumerable<Permission> SupportedPermissions { get; }
        public async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            var query = new SqlBuilder(_session.Store.Configuration.TablePrefix, _session.Store.Dialect);
            query.Select();
            query.Table(typeof(TIndex).Name)

            if (request.HasSearch)
            {
                await GlobalSearchAsync(query, request.Search);
            }

            if (request.Order.Any())
            {
                await SortAsync(query, request.Order);
            }

            query.Skip(request.Start.ToTechnicalString());
            query.Take(request.Length.ToTechnicalString());
            var sql = query.ToSqlString();

            _session.E
        }

        protected abstract Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync();

        public Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) =>
            GetColumnsDefinitionAsync();

        public Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId) => throw new System.NotImplementedException();

        protected virtual async Task GlobalSearchAsync(ISqlBuilder sqlBuilder, DataTableSearchParameters parameters)
        {
            var columnsDefinition = await GetColumnsDefinitionAsync(null);
            foreach (var columnDefinition in columnsDefinition.Columns.Where(definition => definition.Searchable))
            {
                sqlBuilder.WhereAlso($"{columnDefinition.Name} like '%{parameters.Value}%'");
            }
        }

        protected virtual async Task SortAsync(ISqlBuilder sqlBuilder, IEnumerable<DataTableOrder> orders)
        {
            var columnsDefinition = await GetColumnsDefinitionAsync(null);
            var wasOrderedOnce = false;
            foreach (var dataTableOrder in orders)
            {
                var columnDefinition = columnsDefinition.Columns.FirstOrDefault(definition => definition.Name == dataTableOrder.Column);
                if (columnDefinition != null)
                {
                    OrderByColumn(sqlBuilder, dataTableOrder, columnDefinition, wasOrderedOnce);
                    wasOrderedOnce = true;
                }
            }
        }

        protected virtual void OrderByColumn(
            ISqlBuilder sqlBuilder,
            DataTableOrder order,
            DataTableColumnDefinition columnDefinition,
            bool wasOrderedOnce)
        {
            if (!wasOrderedOnce)
            {
                if (order.Direction == SortingDirection.Ascending) sqlBuilder.OrderBy(columnDefinition.Name);
                else sqlBuilder.OrderByDescending(columnDefinition.Name);
            }
            else
            {
                if (order.Direction == SortingDirection.Ascending) sqlBuilder.ThenOrderBy(columnDefinition.Name);
                else sqlBuilder.ThenOrderByDescending(columnDefinition.Name);
            }
        }
    }
}
