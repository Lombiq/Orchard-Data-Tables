using Fluid.Values;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json.Linq;
using OrchardCore.DisplayManagement.Liquid;
using OrchardCore.Liquid;
using OrchardCore.Liquid.Services;
using OrchardCore.Localization;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Tests
{
    public class MockDataProvider : JsonResultDataTableDataProvider
    {
        public DataTableColumnsDefinition Definition { get; set; }
        private readonly object[][] _dataSet;

        public override LocalizedString Description { get; } = new LocalizedString("Test", "Test");
        public override IEnumerable<Permission> AllowedPermissions { get; }

        public MockDataProvider(object[][] dataSet, IMemoryCache memoryCache, DataTableColumnsDefinition definition = null)
            : base(
                new DataTableDataProviderServices(
                    httpContextAccessor: null,
                    linkGenerator: null,
                    MockLiquidTemplateManager(),
                    memoryCache,
                    shapeFactory: null,
                    session: null,
                    authorizationService: null,
                    contentManager: null),
                new StringLocalizer<MockDataProvider>(new NullStringLocalizerFactory()))
        {
            _dataSet = dataSet;
            Definition = definition;
            AllowedPermissions = null;
        }

        protected override async Task<JsonResultDataTableDataProviderResult> GetResultsAsync(DataTableDataRequest request)
        {
            var columns = (await GetColumnsDefinitionAsync(null))
                .Columns
                .Select((item, index) => new { item.Name, Index = index });

            return new(_dataSet.Select(row =>
                JObject.FromObject(columns.ToDictionary(column => column.Name, column => row[column.Index]))));
        }

        protected override DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) => Definition;

        private static ILiquidTemplateManager MockLiquidTemplateManager()
        {
            var mock = new Mock<ILiquidTemplateManager>();
            mock
                .Setup(x => x.RenderAsync(
                    It.IsAny<string>(),
                    It.IsAny<TextWriter>(),
                    It.IsAny<TextEncoder>(),
                    It.IsAny<object>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, FluidValue>>>()))
                .Returns(Task.CompletedTask);
            return mock.Object;
        }
    }
}
