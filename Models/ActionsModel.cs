﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models
{
    public class ActionsModel
    {
        public string Id { get; set; }
        public IEnumerable<ExportLink> MenuItems { get; set; }
        public bool WithDefaults { get; set; } = true;

        public IEnumerable<ExportLink> GetAllMenuItems(
            HttpContext context,
            LinkGenerator linkGenerator,
            IStringLocalizer stringLocalizer)
        {
            if (string.IsNullOrEmpty(Id)) WithDefaults = false;
            if (!WithDefaults) return MenuItems ?? Enumerable.Empty<ExportLink>();

            var values = new
            {
                area = "OrchardCore.Contents",
                contentItemId = Id,
                // See RazorPage.FullRequestPath.
                returnUrl = context.Request.PathBase + context.Request.Path + context.Request.QueryString,
            };
            var links = new List<ExportLink>
            {
                new ExportLink(linkGenerator.GetUriByAction(context, "Edit", "Admin", values), stringLocalizer["Edit"]),
                new ExportLink(
                    linkGenerator.GetUriByAction(context, "Remove", "Admin", values),
                    stringLocalizer["Delete"],
                    new Dictionary<string, string>
                    {
                        ["itemprop"] = "RemoveUrl UnsafeUrl",
                        ["data-title"] = stringLocalizer["Delete"],
                        ["data-message"] = stringLocalizer["Are you sure you want to delete this content item?"]
                    }),
            };

            if (MenuItems != null) links.AddRange(MenuItems);
            return links;
        }

    }
}
