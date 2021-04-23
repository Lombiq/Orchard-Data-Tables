﻿using Dapper;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
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
        private readonly IDictionary<string, string> _columnMapping = new Dictionary<string, string>();

        protected IndexBasedDataTableDataProvider(IDataTableDataProviderServices services, ISession session)
            : base(services) => _session = session;

        public override async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
        {
            var query = new SqlBuilder(_session.Store.Configuration.TablePrefix, _session.Store.Dialect);
            query.Select();
            query.Table(typeof(TIndex).Name);
            query.Selector("*");

            var columnsDefinition = await GetColumnsDefinitionAsync(request.QueryId);
            var liquidColumns = columnsDefinition
                .Columns
                .Where(column => column.IsLiquid)
                .Select(column => column.Name).ToList();

            if (request.HasSearch)
            {
                await GlobalSearchAsync(query, request.Search, columnsDefinition);
            }

            if (request.Order?.Any() != true)
            {
                var defaultOrderableColumnName = columnsDefinition
                    .Columns
                    .First(column => column.Orderable)
                    .Name;
                request.Order = new[] { new DataTableOrder { Column = defaultOrderableColumnName } };
            }

            Sort(query, request.Order);

            query.Skip(request.Start.ToTechnicalString());
            query.Take(request.Length.ToTechnicalString());
            var sql = query.ToSqlString();

            var transaction = await _session.DemandAsync();
            var queryResults = await transaction.Connection.QueryAsync<TIndex>(sql, query.Parameters, transaction);

            var rowList = SubstituteByColumn(
                    (await TransformAsync(queryResults)).Select(JObject.FromObject),
                    columnsDefinition.Columns.ToList())
                .Select((item, index) => new DataTableRow(index, JObject.FromObject(item)))
                .ToList();
            if (liquidColumns.Count > 0) await RenderLiquidAsync(rowList, liquidColumns);

            return DataTableDataResponse.FromRows(
                rowList,
                request.HasSearch ? await _session.QueryIndex<TIndex>().CountAsync() : rowList.Count);
        }

        protected virtual ValueTask<IEnumerable<object>> TransformAsync(IEnumerable<TIndex> rows) => new(rows);

        protected virtual Task GlobalSearchAsync(
            ISqlBuilder sqlBuilder,
            DataTableSearchParameters parameters,
            DataTableColumnsDefinition columnsDefinition)
        {
            var search = parameters.Value.Replace("'", "''", StringComparison.Ordinal);
            var conditions = columnsDefinition
                .Columns
                .Where(definition => definition.Searchable)
                .Select(definition => $"{GetIndexColumnName(definition)} like '%{search}%'");

            sqlBuilder.WhereAlso($"({string.Join(" OR ", conditions)})");
            return Task.CompletedTask;
        }

        protected void Sort(ISqlBuilder sqlBuilder, IEnumerable<DataTableOrder> orders)
        {
            var wasOrderedOnce = false;
            var columns = typeof(TIndex).GetProperties().Select(property => property.Name).ToHashSet();

            orders = orders.Select(order => new DataTableOrder
            {
                Column = GetIndexColumnName(order.Column),
                Direction = order.Direction,
            });

            foreach (var dataTableOrder in orders)
            {
                if (!columns.Contains(dataTableOrder.Column)) continue;

                OrderByColumn(sqlBuilder, dataTableOrder, wasOrderedOnce);
                wasOrderedOnce = true;
            }
        }

        protected ExportLink CreateLink(string id, string text, string tab = null) =>
            id == null
                ? null
                : new ExportLink(
                    DataTableContentItemExtensions.CreateEditLink(id, _linkGenerator, _hca.HttpContext, tab),
                    text);

        protected void AddColumnMapping(string tableResultColumnName, string indexColumnName) =>
            _columnMapping[tableResultColumnName] = indexColumnName;

        protected string GetIndexColumnName(string tableResultColumnName) =>
            _columnMapping.GetMaybe(tableResultColumnName) ?? tableResultColumnName;
        protected string GetIndexColumnName(DataTableColumnDefinition definition) =>
            _columnMapping.GetMaybe(definition.Name) ?? definition.Name;

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
