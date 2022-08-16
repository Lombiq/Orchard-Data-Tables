using Lombiq.DataTables.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Liquid;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Localization;
using OrchardCore.Modules;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.DataTables.Tests.UnitTests.Services;

public class LiquidTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public async Task LiquidEvaluationMatchExpectation(
        string note,
        object[][] dataSet,
        (string Name, string Text, bool Exportable)[] columns,
        string[][] pattern,
        int start,
        int length,
        int orderColumnIndex)
    {
        note.ShouldNotBeEmpty("Please state the purpose of this input set!");

        // Everything in this section of code is required for the Liquid renderer to work. Otherwise it will throw NRE
        // or render empty string results. On the final line shellScope.StartAsyncFlow() initializes the static variable
        // representing the current ShellScope.
        using var shellContext = new ShellContext
        {
            ServiceProvider = new ServiceCollection()
                .AddScoped<IOptions<LiquidViewOptions>>(_ =>
                    new OptionsWrapper<LiquidViewOptions>(new LiquidViewOptions()))
                .AddScoped(_ => new ViewContextAccessor { ViewContext = new ViewContext() })
                .AddScoped(_ => new Mock<IDisplayHelper>().Object)
                .AddScoped(_ => new Mock<IUrlHelperFactory>().Object)
                .AddScoped(_ => new Mock<IShapeFactory>().Object)
                .AddScoped(_ => new Mock<ILayoutAccessor>().Object)
                .AddScoped(_ => new Mock<IViewLocalizer>().Object)
                .AddScoped<ILocalClock>(_ =>
                    new LocalClock(
                        Array.Empty<ITimeZoneSelector>(),
                        new Clock(),
                        new DefaultCalendarManager(Array.Empty<ICalendarSelector>())))
                .BuildServiceProvider(),
        };

        // Disposing this will cause an NRE somehow. This is outside of the scope of the test so we can let it go.
#pragma warning disable CA2000 // Dispose objects before losing scope
        var shellScope = new ShellScope(shellContext);
#pragma warning restore CA2000 // Dispose objects before losing scope

        shellScope.StartAsyncFlow();

        using var memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        var (provider, request) = MockDataProviderHelper.GetProviderAndRequest(
            dataSet,
            columns,
            start,
            length,
            orderColumnIndex,
            memoryCache,
            shellContext.ServiceProvider);
        var rows = (await provider.GetRowsAsync(request)).Data.ToList();

        for (var rowIndex = 0; rowIndex < pattern.Length; rowIndex++)
        {
            var row = rows[rowIndex];
            columns
                .Select(column => row[column.Name.Split(new[] { "||" }, StringSplitOptions.None)[0]])
                .ToArray()
                .ShouldBe(
                    pattern[rowIndex],
                    string.Create(CultureInfo.InvariantCulture, $"Row {rowIndex + 1} didn't match expectation."));
        }
    }

    public static IEnumerable<object[]> Data()
    {
        var dataset = new[]
        {
            new object[] { "now", "Foo Bar Baz" },
            new object[] { "2020-12-31", "The quick brown fox" },
            new object[] { "1970-01-01", "Lorem Ipsum Dolor Sit Amet" },
        };
        var today = DateTime.Today.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

        yield return new object[]
        {
            // By "built-in" we mean only those implemented in Fluid, not the ones added by OrchardCore.
            "Demonstrate some built-in filters.",
            dataset,
            new[]
            {
                ("Num||^.*$||{{ '$0' | date: '%m/%d/%Y' }}", "Dates", true),
                ("Cls||^.*$||{{ '$0' | downcase }}", "Magic Words", true),
            },
            $"{today},foo bar baz;01/01/1970,lorem ipsum dolor sit amet;12/31/2020,the quick brown fox"
                .Split(';')
                .Select(row => row.Split(','))
                .ToArray(),
            0,
            10,
            1,
        };
    }
}
