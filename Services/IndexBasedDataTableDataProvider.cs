using Dapper;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;
using YesSql.Sql;

namespace Lombiq.DataTables.Services
{
    public abstract class IndexBasedDataTableDataProvider<TIndex> : DataTableDataProviderBase
        where TIndex : MapIndex
    {
        protected readonly ISession _session;
        protected readonly IDictionary<string, string> _columnMapping = new Dictionary<string, string>();

        protected IndexBasedDataTableDataProvider(IDataTableDataProviderServices services, ISession session)
            : base(services) => _session = session;

        public override async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            var query = new SqlBuilder(_session.Store.Configuration.TablePrefix, _session.Store.Dialect);
            query.Select();
            query.Table(typeof(TIndex).Name);
            query.Selector("*");

            if (request.HasSearch)
            {
                await GlobalSearchAsync(query, request.Search);
            }

            var columnsDefinition = await GetColumnsDefinitionAsync(request.QueryId);
            if (request.Order?.Any() != true)
            {
                var defaultOrderableColumnName = columnsDefinition
                    .Columns
                    .First(column => column.Orderable)
                    .Name;
                request.Order = new[] { new DataTableOrder { Column = defaultOrderableColumnName } };
            }

            var sqlOrder = request
                .Order
                .Select(order => new DataTableOrder
                {
                    Column = _columnMapping.GetMaybe(order.Column) ?? order.Column,
                    Direction = order.Direction,
                });
            await SortAsync(query, sqlOrder, columnsDefinition);

            query.Skip(request.Start.ToTechnicalString());
            query.Take(request.Length.ToTechnicalString());
            var sql = query.ToSqlString();

            var transaction = await _session.DemandAsync();
            var results = await transaction.Connection.QueryAsync<TIndex>(sql, query.Parameters, transaction);
            var rows = (await TransformAsync(results))
                .Select((item, index) => new DataTableRow(index, JObject.FromObject(item)))
                .ToList();

            return DataTableDataResponse.FromRows(
                rows,
                request.HasSearch ? await _session.QueryIndex<TIndex>().CountAsync() : rows.Count);
        }

        protected virtual ValueTask<IEnumerable<object>> TransformAsync(IEnumerable<TIndex> rows) => new(rows);

        protected virtual async Task GlobalSearchAsync(ISqlBuilder sqlBuilder, DataTableSearchParameters parameters)
        {
            var columnsDefinition = await GetColumnsDefinitionAsync(null);
            foreach (var columnDefinition in columnsDefinition.Columns.Where(definition => definition.Searchable))
            {
                sqlBuilder.WhereAlso($"{columnDefinition.Name} like '%{parameters.Value}%'");
            }
        }

        protected virtual Task SortAsync(
            ISqlBuilder sqlBuilder,
            IEnumerable<DataTableOrder> orders,
            DataTableColumnsDefinition columnsDefinition)
        {
            var wasOrderedOnce = false;
            var columns = typeof(TIndex).GetProperties().Select(property => property.Name).ToHashSet();

            foreach (var dataTableOrder in orders)
            {
                if (!columns.Contains(dataTableOrder.Column)) continue;

                OrderByColumn(sqlBuilder, dataTableOrder, wasOrderedOnce);
                wasOrderedOnce = true;
            }

            return Task.CompletedTask;
        }

        private static void OrderByColumn(ISqlBuilder sqlBuilder, DataTableOrder order, bool wasOrderedOnce)
        {
            if (!wasOrderedOnce)
            {
                if (order.Direction == SortingDirection.Ascending) sqlBuilder.OrderBy(order.Column);
                else sqlBuilder.OrderByDescending(order.Column);
            }
            else
            {
                if (order.Direction == SortingDirection.Ascending) sqlBuilder.ThenOrderBy(order.Column);
                else sqlBuilder.ThenOrderByDescending(order.Column);
            }
        }
    }
}
