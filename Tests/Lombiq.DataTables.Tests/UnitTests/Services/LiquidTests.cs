using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Liquid;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.DataTables.Tests.UnitTests.Services
{
    public class LiquidTests : MockDataProviderTestsBase
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
            // Everything in this section of code is required for the Liquid renderer to work. Otherwise it will throw
            // NRE or render empty string results. On the final line shellScope.StartAsyncFlow() initializes the static
            // variable representing the current ShellScope.
            var shellScope = new ShellScope(new ShellContext
            {
                ServiceProvider = new ServiceCollection()
                    .AddScoped<IOptions<LiquidOptions>>(provider => new OptionsWrapper<LiquidOptions>(new LiquidOptions()))
                    .AddScoped(provider => new ViewContextAccessor { ViewContext = new ViewContext() })
                    .AddScoped(provider => new Mock<IDisplayHelper>().Object)
                    .AddScoped(provider => new Mock<IUrlHelperFactory>().Object)
                    .AddScoped(provider => new Mock<IShapeFactory>().Object)
                    .AddScoped(provider => new Mock<ILayoutAccessor>().Object)
                    .AddScoped(provider => new Mock<IViewLocalizer>().Object)
                    .BuildServiceProvider()
            });
            shellScope.StartAsyncFlow();

            using var memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
            var (provider, request) = GetProviderAndRequest(note, dataSet, columns, start, length, orderColumnIndex, memoryCache);
            var rows = (await provider.GetRowsAsync(request)).Data.ToList();

            for (var rowIndex = 0; rowIndex < pattern.Length; rowIndex++)
            {
                var row = rows[rowIndex];
                columns
                    .Select(column => row[column.Name.Split(new[] { "||" }, StringSplitOptions.None)[0]])
                    .ToArray()
                    .ShouldBe(pattern[rowIndex], $"Row {rowIndex + 1} didn't match expectation.");
            }
        }

        public static IEnumerable<object[]> Data()
        {
            var dataset = new[]
            {
                new object[] { "now", "Foo Bar Baz" },
                new object[] { "2020-12-31", "The quick brown fox" },
                new object[] { "1970-01-01", "Lorem Ipsum Dolor Sit Amet" }
            };
            var today = DateTime.Today.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            yield return new object[]
            {
                "Demonstrate some built-in filters.",
                dataset,
                new[]
                {
                    ("Num||^.*$||{{ '$0' | date: '%m/%d/%Y' }}", "Dates", true),
                    ("Cls||^.*$||{{ '$0' | html_class  }}", "Magic Words", true)
                },
                $"{today},foo-bar-baz;01/01/1970,lorem-ipsum-dolor-sit-amet;12/31/2020,the-quick-brown-fox"
                    .Split(';')
                    .Select(row => row.Split(','))
                    .ToArray(),
                0,
                10,
                1
            };
        }
    }
}
