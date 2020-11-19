using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OrchardCore.ContentManagement
{
    public static class DataTableContentItemExtensions
    {
        public static string CreateEditLink(
            this ContentItem contentItem,
            LinkGenerator linkGenerator,
            HttpContext httpContext,
            string bootstrapTabType = null) =>
            CreateEditLink(contentItem.ContentItemId, linkGenerator, httpContext, bootstrapTabType);

        public static string CreateEditLink(
            string contentItemId,
            LinkGenerator linkGenerator,
            HttpContext httpContext,
            string bootstrapTabType = null) =>
            linkGenerator.GetUriByAction(
                httpContext,
                "Edit",
                "Admin",
                new
                {
                    area = "OrchardCore.Contents",
                    contentItemId = contentItemId,
                    bootstraptab = bootstrapTabType == null
                        ? null
                        : $"tab-{bootstrapTabType}-{contentItemId}",
                });
    }
}
