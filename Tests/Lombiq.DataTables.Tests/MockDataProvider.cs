using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OrchardCore.Liquid.Services;
using OrchardCore.Localization;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Tests
{
    public class MockDataProvider : JsonResultDataTableDataProvider
    {
        public DataTableColumnsDefinition Definition { get; set; }
        private readonly object[][] _dataSet;

        public override LocalizedString Description { get; } = new LocalizedString("Test", "Test");
        public override IEnumerable<Permission> SupportedPermissions { get; } = null;


        public MockDataProvider(object[][] dataSet, DataTableColumnsDefinition definition = null)
            : base(
                new StringLocalizer<MockDataProvider>(new NullStringLocalizerFactory()),
                new LiquidTemplateManager(
                    new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()))),
                linkGenerator:null,
                hca: null)
        {
            Definition = definition;
            _dataSet = dataSet;
        }


        protected override async Task<IEnumerable<object>> GetResultsAsync(DataTableDataRequest request)
        {
            var columns = (await GetColumnsDefinitionAsync(null))
                .Columns
                .Select((item, index) => new { item.Name, Index = index });

            return _dataSet.Select(row =>
                JObject.FromObject(columns.ToDictionary(column => column.Name, column => row[column.Index])));
        }

        protected override DataTableColumnsDefinition GetColumnsDefinition(string queryId) => Definition;
    }
}
