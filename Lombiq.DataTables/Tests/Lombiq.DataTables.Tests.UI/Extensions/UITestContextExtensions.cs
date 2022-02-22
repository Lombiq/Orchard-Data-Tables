using Lombiq.DataTables.Samples.Controllers;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.Tests.UI.Models;
using Lombiq.HelpfulLibraries.Libraries.Mvc;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Tests.UI.Extensions
{
    public static class UITestContextExtensions
    {
        public static Task ExecuteDataTablesSampleRecipeDirectlyAsync(this UITestContext context) =>
            context.ExecuteRecipeDirectlyAsync("Lombiq.DataTables.Samples.Content");

        public static Task GoToDataTableTagHelperAsync(this UITestContext context) =>
            context.GoToAsync<SampleController>(controller => controller.DataTableTagHelper());

        public static Task GoToDataTableProviderWithShapeAsync(this UITestContext context) =>
            context.GoToAsync<SampleController>(controller => controller.ProviderWithShape());

        public static Task GoToAdminDataTableAsync<T>(this UITestContext context)
            where T : IDataTableDataProvider =>
            context.GoToAdminDataTableAsync(typeof(T).Name);

        public static Task GoToAdminDataTableAsync(this UITestContext context, string providerName) =>
            context.GoToRelativeUrlAsync("/Admin/DataTable/" + providerName);

        public static void VerifyDataTablePager(this UITestContext context, int pageCount, int currentPage = 1)
        {
            const string pagerItemXPath = "//li[contains(@class, 'paginate_button') and not(contains(@class, 'page-item next'))]";

            context.Exists(By.XPath(FormattableString.Invariant($"({pagerItemXPath})[last()]/a[@data-dt-idx='{pageCount}']")));

            static void VerifyNavigation(UITestContext context, string className, bool exists)
            {
                var classes = context.Get(By.CssSelector($".page-item.{className}")).GetAttribute("class");

                if (exists)
                {
                    classes.ShouldNotContain("disabled");
                }
                else
                {
                    classes.ShouldContain("disabled");
                }
            }

            VerifyNavigation(context, "previous", currentPage > 1);
            VerifyNavigation(context, "next", currentPage < pageCount);
        }

        public static async Task ClickAndWaitForTableChangeAsync(this UITestContext context, By selector)
        {
            var state = new TableDrawState(context);
            await context.ClickReliablyOnAsync(selector);
            state.Wait();
        }
    }
}
