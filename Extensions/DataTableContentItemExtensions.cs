using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace OrchardCore.ContentManagement
{
    public static class DataTableContentItemExtensions
    {
        public static string CreateEditLink(
            this ContentItem contentItem,
            LinkGenerator linkGenerator,
            HttpContext httpContext,
            Dictionary<string, string> attributes = null,
            string bootstrapTabType = null) =>
                linkGenerator.GetUriByAction(httpContext,
                    "Edit",
                    "Admin",
                    new
                    {
                        area = "OrchardCore.Contents",
                        contentItemId = contentItem.ContentItemId,
                        bootstraptab = attributes?["bootstraptab"] == null
                        ? null
                        : $"tab-{attributes?["bootstraptab"]}-{contentItem.ContentItemId}"
                    });
    }
}
