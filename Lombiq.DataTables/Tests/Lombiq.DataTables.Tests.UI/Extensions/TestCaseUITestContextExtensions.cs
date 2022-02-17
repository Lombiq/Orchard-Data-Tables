using Lombiq.DataTables.Samples.Services;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Tests.UI.Extensions
{
    public static class TestCaseUITestContextExtensions
    {
        private static readonly object[] _oldest =
        {
            "Ashton Cox",
            "Junior Technical Author",
            "San Francisco",
            "66",
            "1/12/2009",
            "$86,000",
        };

        private static readonly object[] _alphabeticallyFirst =
        {
            "Airi Satou",
            "Accountant",
            "Tokyo",
            "33",
            "11/28/2008",
            "$162,700",
        };

        public static async Task TestDataTableRecipeData(this UITestContext context)
        {
            await context.SignInDirectlyAsync();
            await context.ExecuteDataTablesSampleRecipeDirectlyAsync();

            await context.TestDataTableTagHelperAsync();
            await context.TestDataTableProviderWithShapeAsync();
            await context.TestDataTableIndexBasedProviderAsync();
        }

        public static async Task TestDataTableTagHelperAsync(this UITestContext context)
        {
            await context.GoToDataTableTagHelperAsync();
            context.VerifyDataTablePager(pageCount: 6);
            VerifyText(context, _oldest);
            context.ClickReliablyOn(By.CssSelector("th[data-name='Name']"));
            VerifyText(context, _alphabeticallyFirst);
        }

        public static async Task TestDataTableProviderWithShapeAsync(this UITestContext context)
        {
            await context.GoToDataTableProviderWithShapeAsync();
            context.TestDataTableProvider();
        }

        public static async Task TestDataTableIndexBasedProviderAsync(this UITestContext context)
        {
            await context.GoToAdminDataTableAsync<SampleIndexBasedDataTableDataProvider>();
            context.TestDataTableProvider();
        }

        public static void TestDataTableProvider(this UITestContext context)
        {
            context.VerifyDataTablePager(pageCount: 6);
            VerifyText(context, AdjustForProvider(_alphabeticallyFirst));

            var ageColumnHeader = By.CssSelector("th[data-name='Age']");
            context.ClickAndWaitForTableChange(ageColumnHeader);
            context.ClickAndWaitForTableChange(ageColumnHeader);

            VerifyText(context, AdjustForProvider(_oldest));
        }

        private static void VerifyText(UITestContext context, IEnumerable<object> texts) =>
            context.VerifyElementTexts(By.CssSelector(".dataTable tbody > tr:first-child td"), texts);

        private static IEnumerable<object> AdjustForProvider(object[] source) =>
            source[..^1]
                .Concat(new object[]
                {
                    ((string)source[^1]).Replace(",", string.Empty),
                    null,
                });
    }
}
