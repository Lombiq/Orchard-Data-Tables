using Fluid;
using Lombiq.DataTables.Models;
using Lombiq.DataTables.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Liquid;
using OrchardCore.Liquid.Services;
using OrchardCore.Localization;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Tests;

public class MockDataProvider : JsonResultDataTableDataProvider
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters =
        {
            new DateTimeJsonConverter(),
        },
    };

    public DataTableColumnsDefinition Definition { get; set; }
    private readonly object[][] _dataSet;

    public override LocalizedString Description { get; } = new("Test", "Test");
    public override IEnumerable<Permission> AllowedPermissions { get; }

    public MockDataProvider(
        object[][] dataSet,
        IMemoryCache memoryCache,
        IServiceProvider serviceProvider,
        DataTableColumnsDefinition definition = null)
        : base(
            new DataTableDataProviderServices(
                httpContextAccessor: null,
                linkGenerator: null,
                CreateLiquidTemplateManager(memoryCache, serviceProvider),
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
        var columns = (await GetColumnsDefinitionAsync(queryId: null))
            .Columns
            .Select((item, index) => new { item.Name, Index = index });

        return new(_dataSet.Select(row => JsonSerializer.SerializeToNode(
            columns.ToDictionary(column => column.Name, column => row[column.Index]),
            _options)));
    }

    protected override DataTableColumnsDefinition GetColumnsDefinitionInner(string queryId) => Definition;

    private static LiquidTemplateManager CreateLiquidTemplateManager(IMemoryCache memoryCache, IServiceProvider serviceProvider)
    {
        var optionsFactory = new OptionsFactory<TemplateOptions>(
                [],
                []);

        return new LiquidTemplateManager(
            memoryCache,
            new LiquidViewParser(Options.Create(new LiquidViewOptions())),
            new OptionsManager<TemplateOptions>(optionsFactory),
            serviceProvider);
    }
}
