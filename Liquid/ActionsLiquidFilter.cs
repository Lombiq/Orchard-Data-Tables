using Fluid;
using Fluid.Values;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.Liquid;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Liquid
{
    public class ActionsLiquidFilter : ILiquidFilter
    {
        private readonly IHttpContextAccessor _hca;
        private readonly LinkGenerator _linkGenerator;
        private readonly IStringLocalizer<ActionsLiquidFilter> T;

        public ActionsLiquidFilter(
            IHttpContextAccessor hca,
            LinkGenerator linkGenerator,
            IStringLocalizer<ActionsLiquidFilter> stringLocalizer)
        {
            _hca = hca;
            _linkGenerator = linkGenerator;
            T = stringLocalizer;
        }

        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            string title, returnUrl;
            title = arguments.HasNamed(nameof(title)) ? arguments[nameof(title)].ToStringValue() :
                arguments.Count > 0 ? arguments.At(0).ToStringValue() :
                T["Actions"];
            returnUrl = arguments.HasNamed(nameof(returnUrl)) ? arguments[nameof(returnUrl)].ToStringValue() :
                arguments.Count > 1 ? arguments.At(1).ToStringValue() :
                null;

            return input?.Type switch
            {
                FluidValues.String => FromObject(new ActionsModel { Id = input.ToStringValue() }, title, returnUrl),
                FluidValues.Object => input.ToObjectValue() switch
                {
                    ActionsModel model => FromObject(model, title, returnUrl),
                    JToken jToken => FromObject(jToken.ToObject<ActionsModel>(), title, returnUrl),
                    {} unknown => throw GetException(unknown),
                    _ => throw new ArgumentNullException(nameof(input))
                },
                _ => throw GetException(input?.Type)
            };
        }

        private ValueTask<FluidValue> FromObject(ActionsModel model, string title, string returnUrl)
        {
            var html = new StringBuilder();
            html.AppendLine("<div class=\"btn-group\">")
                .Append("<button type=\"button\" class=\"btn btn-secondary btn-sm dropdown-toggle\" ")
                .AppendLine("data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">")
                .AppendLine(title)
                .AppendLine("</button>")
                .AppendLine("<div class=\"dropdown-menu dropdown-menu-right\">");

            foreach (var menuItem in model.GetAllMenuItems(_hca.HttpContext, _linkGenerator, T, returnUrl))
            {
                html.Append($"<a class=\"dropdown-item btn-sm\" href=\"{menuItem.Url}\"");
                foreach (var (name, value) in menuItem.Attributes) html.Append($" {name}=\"{value}\"");
                html.AppendLine($">{menuItem.Text}</a>");
            }

            html.AppendLine("</div></div>");

            return new ValueTask<FluidValue>(FluidValue.Create(new HtmlString(html.ToString())));
        }

        private static InvalidOperationException GetException(object input) =>
            new InvalidOperationException(
                $"String ContentItemId, ActionsModel or JObject of ActionsModel expected. Got '{input}' instead.");
    }
}
