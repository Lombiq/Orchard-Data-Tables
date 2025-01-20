using Atata;
using ClosedXML.Excel;
using Lombiq.DataTables.Samples.Services;
using Lombiq.Tests.UI.Constants;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

    /// <summary>
    /// Signs in, executes the test-specific recipe, then performs the provided test <paramref name="sections"/>.
    /// </summary>
    /// <param name="sections">
    /// Flags to indicate which parts of the overall test should be executed. Defaults to every section.
    /// </param>
    /// <remarks><para>
    /// We suggest testing different sections in individual tests, or in one <c>[Theory]</c> that sets <paramref
    /// name="sections"/> via parameter, so it's more clear at a glance which section fails.
    /// </para></remarks>
    public static async Task TestDataTableRecipeDataAsync(
        this UITestContext context,
        TestDataTableRecipeDataSections sections = TestDataTableRecipeDataSections.All)
    {
        await context.SignInDirectlyAsync();
        await context.ExecuteDataTablesSampleRecipeDirectlyAsync();

        if (sections.HasFlag(TestDataTableRecipeDataSections.MainMenu))
        {
            await context.GoToHomePageAsync();
            context.TestDataTableSampleMainMenu();
        }

        if (sections.HasFlag(TestDataTableRecipeDataSections.TagHelper))
        {
            await context.TestDataTableTagHelperAsync();
        }

        if (sections.HasFlag(TestDataTableRecipeDataSections.ProviderWithShape))
        {
            await context.TestDataTableProviderWithShapeAsync();
        }

        if (sections.HasFlag(TestDataTableRecipeDataSections.JsonBasedProvider))
        {
            await context.GoToAdminDataTableAsync<SampleJsonResultDataTableDataProvider>();
            await context.TestDataTableProviderAsync();
        }

        if (sections.HasFlag(TestDataTableRecipeDataSections.IndexBasedProvider))
        {
            await context.TestDataTableIndexBasedProviderAsync();
        }
    }

    /// <param name="checkMainMenu">
    /// Set to <see langword="false"/> if you don't want to check that the sample's main menu item is properly displayed
    /// (needs Lombiq Base Theme for Orchard Core as the site theme).
    /// </param>
    public static Task TestDataTableRecipeDataAsync(this UITestContext context, bool checkMainMenu) =>
        context.TestDataTableRecipeDataAsync(checkMainMenu
            ? TestDataTableRecipeDataSections.All
            : TestDataTableRecipeDataSections.All & TestDataTableRecipeDataSections.MainMenu);

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

    public static Task TestDataTableProviderAsync(this UITestContext context) =>
        context.TestDataTableProviderAsync(testExport: true);

    public static async Task TestDataTableProviderAsync(this UITestContext context, bool testExport)
    {
        context.VerifyDataTablePager(pageCount: 6);
        VerifyText(context, AdjustForProvider(_alphabeticallyFirst));

        var ageColumnHeader = By.CssSelector("th[data-name='Age']");
        await context.ClickAndWaitForTableChangeAsync(ageColumnHeader);
        await context.ClickAndWaitForTableChangeAsync(ageColumnHeader);

        VerifyText(context, AdjustForProvider(_oldest));

        if (testExport)
        {
            await DownloadSpreadsheetAsync(context, By.ClassName("dataTables_button-exportAll"), expectedLength: 58);
            await DownloadSpreadsheetAsync(context, By.ClassName("dataTables_button-exportVisible"), expectedLength: 11);
        }
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
            .Concat(
            [
                ((string)source[^1]).Replace(",", string.Empty),
                null,
            ]);

    private static async Task DownloadSpreadsheetAsync(
        this UITestContext context,
        By downloadButtonBy,
        int expectedLength)
    {
        var path = context.GetTempSubDirectoryPath(DirectoryPaths.Downloads, "export.xlsx");
        if (File.Exists(path)) File.Delete(path);

        await context.ClickReliablyOnAsync(downloadButtonBy);
        context.DoWithRetriesOrFail(() => File.Exists(path), TimeSpan.FromMinutes(2));

        using (var workbook = new XLWorkbook(path))
        {
            var sheet = workbook.Worksheet(1);
            sheet.Rows().Count().ShouldBe(expectedLength);
        }

        File.Delete(path);
    }
}
