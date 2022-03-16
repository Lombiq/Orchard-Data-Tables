using Dapper;
using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Indexes;
using YesSql.Sql;
using static OrchardCore.Contents.CommonPermissions;

namespace Lombiq.DataTables.Services;

public abstract class IndexBasedDataTableDataProvider<TIndex> : DataTableDataProviderBase
    where TIndex : MapIndex
{
    private readonly IContentManager _contentManager;
    protected readonly IAuthorizationService _authorizationService;
    protected readonly ISession _session;
    private readonly IDictionary<string, string> _columnMapping = new Dictionary<string, string>();

    protected IndexBasedDataTableDataProvider(IDataTableDataProviderServices services)
        : base(services)
    {
        _contentManager = services.ContentManager;
        _session = services.Session;
        _authorizationService = services.AuthorizationService;
    }

    public override async Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request)
    {
        var query = new SqlBuilder(_session.Store.Configuration.TablePrefix, _session.Store.Configuration.SqlDialect);
        query.Select();
        query.Table(typeof(TIndex).Name);

        var columnsDefinition = await GetColumnsDefinitionAsync(request.QueryId);

        if (request.HasSearch) await GlobalSearchAsync(query, request.Search, columnsDefinition);
        foreach (var filter in request.ColumnFilters) await ColumnSearchAsync(query, filter, columnsDefinition);

        query.Selector("COUNT(*) as Count");
        var countSql = query.ToSqlString();
        query.Selector("*");

        Sort(query, request.Order, columnsDefinition);

        if (request.Length is > 0 and not int.MinValue)
        {
            query.Skip(request.Start.ToTechnicalString());
            query.Take(request.Length.ToTechnicalString());
        }

        var sql = query.ToSqlString();

        var transaction = await _session.BeginTransactionAsync();
        var queryResults = await transaction.Connection.QueryAsync<TIndex>(sql, query.Parameters, transaction);

        var rowList = SubstituteByColumn(
                (await TransformAsync(queryResults)).Select(JObject.FromObject),
                columnsDefinition.Columns.ToList())
            .Select((item, index) => new DataTableRow(index, JObject.FromObject(item)))
            .ToList();

        var liquidColumns = columnsDefinition
            .Columns
            .Where(column => column.IsLiquid)
            .Select(column => column.Name)
            .ToList();
        if (liquidColumns.Count > 0) await RenderLiquidAsync(rowList, liquidColumns);

        var total = await _session.QueryIndex<TIndex>().CountAsync();
        return new DataTableDataResponse
        {
            Data = rowList,
            RecordsFiltered = request.HasSearch || request.ColumnFilters.Any()
                ? await transaction.Connection.QueryFirstAsync<int>(countSql, query.Parameters, transaction)
                : total,
            RecordsTotal = total,
        };
    }

    protected virtual ValueTask<IEnumerable<object>> TransformAsync(IEnumerable<TIndex> rows) => new(rows);

    protected virtual Task GlobalSearchAsync(
        ISqlBuilder sqlBuilder,
        DataTableSearchParameters parameters,
        DataTableColumnsDefinition columnsDefinition)
    {
        var conditions = columnsDefinition
            .Columns
            .Where(definition => definition.Searchable)
            .Select(definition => $"{GetIndexColumnName(definition)} LIKE @{nameof(GlobalSearchAsync)}");

        sqlBuilder.WhereAnd($"({string.Join(" OR ", conditions)})");
        sqlBuilder.Parameters[nameof(GlobalSearchAsync)] = $"%{parameters.Value}%";

        return Task.CompletedTask;
    }

    protected virtual Task ColumnSearchAsync(
        ISqlBuilder sqlBuilder,
        DataTableColumn columnFilter,
        DataTableColumnsDefinition columnsDefinition)
    {
        var definition = columnsDefinition.Columns.SingleOrDefault(item => item.Name == columnFilter.Name);
        if (definition?.Searchable != true) return Task.CompletedTask;

        var parameterName = $"{nameof(ColumnSearchAsync)}_{definition.Name}";

        sqlBuilder.WhereAnd($"{GetIndexColumnName(definition)} LIKE @{parameterName}");
        sqlBuilder.Parameters[parameterName] = $"%{columnFilter.Search.Value}%";

        return Task.CompletedTask;
    }

    protected void Sort(
        ISqlBuilder sqlBuilder,
        IEnumerable<DataTableOrder> orders,
        DataTableColumnsDefinition columnsDefinition)
    {
        var wasOrderedOnce = false;
        var columns = typeof(TIndex).GetProperties().Select(property => property.Name).ToHashSet();

        orders = orders?
            .SelectWhere(
                order => new DataTableOrder
                {
                    Column = GetIndexColumnName(order.Column),
                    Direction = order.Direction,
                },
                order => columns.Contains(order.Column)
            );
        var ordersList = orders?.ToList() ?? new List<DataTableOrder>();

        if (!ordersList.Any())
        {
            var defaultOrderableColumnName = columnsDefinition
                .Columns
                .First(column => column.Orderable)
                .Name;
            ordersList.Add(new DataTableOrder
            {
                Column = defaultOrderableColumnName,
                Direction = SortingDirection.Ascending,
            });
        }

        foreach (var dataTableOrder in ordersList)
        {
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

    protected async Task<bool> CanDeleteAsync(string contentType) =>
        _hca?.HttpContext.User is { } user &&
        await _authorizationService.AuthorizeAsync(user, DeleteContent, await _contentManager.NewAsync(contentType));

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
