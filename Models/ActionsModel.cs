using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Models
{
    public class ActionsModel
    {
        private const string OrchardCoreContents = "OrchardCore.Contents";

        public string Id { get; set; }
        public IEnumerable<ExportLink> MenuItems { get; set; }
        public bool WithDefaults { get; set; } = true;

        public IEnumerable<ExportLink> GetAllMenuItems(
            HttpContext context,
            LinkGenerator linkGenerator,
            IStringLocalizer<ActionsModel> stringLocalizer,
            string returnUrl)
        {
            if (string.IsNullOrEmpty(Id)) WithDefaults = false;
            if (!WithDefaults) return MenuItems ?? Enumerable.Empty<ExportLink>();

            var links = new List<ExportLink>
            {
                GetEditLink(Id, context, linkGenerator, stringLocalizer, returnUrl),
                GetRemoveLink(Id, context, linkGenerator, stringLocalizer, returnUrl),
            };

            if (MenuItems != null) links.AddRange(MenuItems);
            return links;
        }

        public static ExportLink GetEditLink(
            string contentItemId,
            HttpContext context,
            LinkGenerator linkGenerator,
            IStringLocalizer<ActionsModel> stringLocalizer,
            string returnUrl,
            LocalizedString text = null)
        {
            var values = new { area = OrchardCoreContents, contentItemId, returnUrl };

            return new ExportLink(
                linkGenerator.GetUriByAction(context, "Edit", "Admin", values),
                (text ?? stringLocalizer["Edit"]).Value);
        }

        public static ExportLink GetRemoveLink(
            string contentItemId,
            HttpContext context,
            LinkGenerator linkGenerator,
            IStringLocalizer<ActionsModel> stringLocalizer,
            string returnUrl,
            LocalizedString text = null)
        {
            var values = new { area = OrchardCoreContents, contentItemId, returnUrl };

            return new ExportLink(
                linkGenerator.GetUriByAction(context, "Remove", "Admin", values),
                (text ?? stringLocalizer["Delete"]).Value,
                new Dictionary<string, string>
                {
                    ["itemprop"] = "RemoveUrl UnsafeUrl",
                    ["data-title"] = text ?? stringLocalizer["Delete"],
                    ["data-message"] = stringLocalizer["Are you sure you want to delete this content item?"],
                });
        }
    }
}
