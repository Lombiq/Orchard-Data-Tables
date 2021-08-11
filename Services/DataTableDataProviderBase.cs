using Fluid;
using Fluid.Values;
using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public abstract class DataTableDataProviderBase : IDataTableDataProvider
    {
        protected readonly IHttpContextAccessor _hca;
        protected readonly LinkGenerator _linkGenerator;
        protected readonly ILiquidTemplateManager _liquidTemplateManager;
        protected readonly IShapeFactory _shapeFactory;

        public abstract LocalizedString Description { get; }
        public virtual IEnumerable<Permission> AllowedPermissions => Enumerable.Empty<Permission>();

        protected DataTableDataProviderBase(IDataTableDataProviderServices services)
        {
            _linkGenerator = services.LinkGenerator;
            _hca = services.HttpContextAccessor;
            _liquidTemplateManager = services.LiquidTemplateManager;
            _shapeFactory = services.ShapeFactory;
        }

        public virtual Task<IEnumerable<dynamic>> GetShapesBeforeTableAsync() =>
            Task.FromResult<IEnumerable<dynamic>>(Array.Empty<IShape>());

        public virtual Task<IEnumerable<dynamic>> GetShapesAfterTableAsync() =>
            Task.FromResult<IEnumerable<dynamic>>(Array.Empty<IShape>());

        public abstract Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request);

        public virtual Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId) => throw new NotSupportedException();

        public virtual Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) =>
            Task.FromResult(GetColumnsDefinitionInner(queryId));

        public string GetActionsColumn(string columnName = nameof(ContentItem.ContentItemId), bool fromJson = false)
        {
            var beforePipe = string.Empty;
            var source = "'$0'";
            var call = "actions";

            if (_hca?.HttpContext != null)
            {
                var returnUrl = _linkGenerator.GetPathByAction(
                    _hca?.HttpContext,
                    nameof(TableController.Get),
                    typeof(TableController).ControllerName(),
                    new { providerName = GetType().Name });
                call = "actions: returnUrl: '" + returnUrl + "'";
            }

            if (fromJson)
            {
                beforePipe = "{% capture jsonData %} $0 {% endcapture %} ";
                source = "jsonData | jsonparse";
            }

            return columnName + "||^.*$||" + beforePipe + "{{ " + source + " | " + call + " }}";
        }

        /// <summary>
        /// When overridden in a derived class it gets the columns definition.
        /// </summary>
        /// <param name="queryId">May be used to dynamically generate the result.</param>
        /// <returns>The default columns definition of this provider.</returns>
        protected virtual DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) =>
            throw new NotSupportedException(
                $"You must override {nameof(GetColumnsDefinitionAsync)} or {nameof(GetColumnsDefinitionInner)}.");

        protected static IEnumerable<DataTableRow> SubstituteByColumn(
            IEnumerable<JObject> json,
            IList<DataTableColumnDefinition> columns) =>
            json.Select((result, index) =>
                new DataTableRow(index, columns
                    .Select(column => (column.Name, column.Regex, Token: result.SelectToken(column.Name, false)))
                    .ToDictionary(
                        cell => cell.Name,
                        cell => cell.Regex is { } regex
                            ? new JValue(Regex.Replace(cell.Token?.ToString() ?? string.Empty, regex.From, regex.To))
                            : cell.Token)));

        protected async Task RenderLiquidAsync(IEnumerable<DataTableRow> rowList, IList<string> liquidColumns)
        {
            foreach (var row in rowList)
            {
                foreach (var liquidColumn in liquidColumns)
                {
                    if (row.ValuesDictionary.TryGetValue(liquidColumn, out var token) &&
                        token?.ToString() is { } template)
                    {
                        row[liquidColumn] = await _liquidTemplateManager.RenderStringAsync(
                            template,
                            NullEncoder.Default,
                            row,
                            Array.Empty<KeyValuePair<string, FluidValue>>());
                    }
                }
            }
        }
    }
}
