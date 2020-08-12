using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OrchardCore.ContentManagement
{
    public static class DataTableContentItemExtensions
    {
        public static string CreateEditLink(this ContentItem contentItem, LinkGenerator linkGenerator, HttpContext httpContext) =>
            linkGenerator.GetUriByAction(httpContext,
                "Edit",
                "Admin",
                new
                {
                    area = "OrchardCore.Contents",
                    contentItemId = contentItem.ContentItemId,
                    returnUrl = httpContext.Request.PathBase +
                                httpContext.Request.Path +
                                httpContext.Request.QueryString,
                });
    }
}
