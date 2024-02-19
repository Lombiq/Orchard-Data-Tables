using Atata;
using Lombiq.DataTables.Samples.Services;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    private static readonly object[] _oldest =
    [
        "Ashton Cox",
        "Junior Technical Author",
        "San Francisco",
        "66",
        new DateTime(2009, 1, 12, 12, 0, 0, DateTimeKind.Utc),
        "$86,000",
    ];

    private static readonly object[] _alphabeticallyFirst =
    [
        "Airlee Saturn",
        "Accountant",
        "Tokyo",
        "33",
        new DateTime(2008, 11, 28, 12, 0, 0, DateTimeKind.Utc),
        "$162,700",
    ];

    private static readonly string[] ExpectedSampleMainMenuElements =
    [
        "/Lombiq.DataTables.Samples/Sample/DataTableTagHelper",
        "/Lombiq.DataTables.Samples/Sample/ProviderWithShape",
        "/Admin/DataTable/SampleJsonResultDataTableDataProvider?paging=true&viewAction=false",
        "/Admin/DataTable/SampleIndexBasedDataTableDataProvider?paging=true&viewAction=false",
    ];

    /// <param name="isNugetTest">Set this parameter if you are
    /// using Lombiq.DataTables.Tests.UI as a NuGet package.</param>
    public static async Task TestDataTableRecipeDataAsync(this UITestContext context, bool isNugetTest = false)
    {
        await context.SignInDirectlyAsync();
        await context.ExecuteDataTablesSampleRecipeDirectlyAsync();

        if (!isNugetTest)
        {
            await context.GoToHomePageAsync();
            context.TestDataTableSampleMainMenu();
        }

        await context.TestDataTableTagHelperAsync();
        await context.TestDataTableProviderWithShapeAsync();
        await context.TestDataTableIndexBasedProviderAsync();
    }

    public static async Task TestDataTableTagHelperAsync(this UITestContext context)
    {
        await context.GoToDataTableTagHelperAsync();
        context.VerifyDataTablePager(pageCount: 6);
        VerifyText(context, _oldest);
        await context.ClickReliablyOnAsync(By.CssSelector("th[data-name='Name']"));
        VerifyText(context, _alphabeticallyFirst);
    }

    public static async Task TestDataTableProviderWithShapeAsync(this UITestContext context)
    {
        await context.GoToDataTableProviderWithShapeAsync();
        await context.TestDataTableProviderAsync();
    }

    public static async Task TestDataTableIndexBasedProviderAsync(this UITestContext context)
    {
        await context.GoToAdminDataTableAsync<SampleIndexBasedDataTableDataProvider>();
        await context.TestDataTableProviderAsync();
    }

    public static async Task TestDataTableProviderAsync(this UITestContext context)
    {
        context.VerifyDataTablePager(pageCount: 6);
        VerifyText(context, AdjustForProvider(_alphabeticallyFirst));

        var ageColumnHeader = By.CssSelector("th[data-name='Age']");
        await context.ClickAndWaitForTableChangeAsync(ageColumnHeader);
        await context.ClickAndWaitForTableChangeAsync(ageColumnHeader);

        VerifyText(context, AdjustForProvider(_oldest));
    }

    public static void TestDataTableSampleMainMenu(this UITestContext context)
    {
        var byTopMenu = By.XPath("//li[contains(@class, 'dropdown') and contains(., 'Data Tables')]");
        var bySubMenu = byTopMenu.Then(By.CssSelector("a.menuWidget__dropdownItem[href]").OfAnyVisibility()).OfAnyVisibility();

        context.Exists(byTopMenu);
        context
            .GetAll(bySubMenu)
            .Select(element => new Uri(element.GetAttribute("href")).PathAndQuery)
            .ToArray()
            .ShouldBe(ExpectedSampleMainMenuElements);
    }

    private static void VerifyText(UITestContext context, IEnumerable<object> texts) =>
        context.VerifyElementTexts(
            By.CssSelector(".dataTable tbody > tr:first-child td"),
            texts.Select(item => item is DateTime date
                ? date.ToString(
                    "d",
                    new CultureInfo(context.Configuration.BrowserConfiguration.AcceptLanguage.Name, useUserOverride: false))
                : item));

    private static IEnumerable<object> AdjustForProvider(object[] source) =>
        source[..^1]
            .Concat(new object[]
            {
                ((string)source[^1]).Replace(",", string.Empty),
                null,
            });
}
