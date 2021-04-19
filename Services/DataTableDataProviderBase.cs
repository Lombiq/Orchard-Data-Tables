using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Core.Utilities;
using System;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public abstract class DataTableDataProviderBase : IDataTableDataProvider
    {
        protected readonly IHttpContextAccessor _hca;
        protected readonly LinkGenerator _linkGenerator;

        public abstract LocalizedString Description { get; }

        protected DataTableDataProviderBase(IDataTableDataProviderServices services)
        {
            _linkGenerator = services?.LinkGenerator;
            _hca = services?.HttpContextAccessor;
        }

        public abstract Task<DataTableDataResponse> GetRowsAsync(DataTableDataRequest request);

        public virtual Task<DataTableColumnsDefinition> GetColumnsDefinitionAsync(string queryId) =>
            Task.FromResult(GetColumnsDefinitionInner(queryId));

        public virtual Task<DataTableChildRowResponse> GetChildRowAsync(int contentItemId) => throw new NotSupportedException();

        /// <summary>
        /// When overridden in a derived class it gets the columns definition.
        /// </summary>
        /// <param name="queryId">May be used to dynamically generate the result.</param>
        /// <returns>The default columns definition of this provider.</returns>
        protected virtual DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) =>
            throw new InvalidOperationException(
                $"You must override {nameof(GetColumnsDefinitionAsync)} or {nameof(GetColumnsDefinitionInner)}.");

        protected string GetActionsColumn(string columnName = nameof(ContentItem.ContentItemId), bool fromJson = false)
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
    }
}
