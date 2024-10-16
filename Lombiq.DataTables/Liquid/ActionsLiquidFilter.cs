using Fluid;
using Fluid.Values;
using Lombiq.DataTables.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Liquid;

/// <summary>
/// This filter accepts either a string of <see cref="ContentItem.ContentItemId"/>, an <see cref="ActionsDescriptor"/>
/// object or a JToken that deserialized into an <see cref="ActionsDescriptor"/>.
/// </summary>
/// <remarks>
/// <para>Usage with ID:</para>
/// <code>
/// { variableWithContentIdString | actions: returnUrl: 'some/return/url', title: 'Of the Button Shape' }
/// </code>
///
/// <para>Usage with raw JSON:</para>
/// <code>
/// {% capture jsonData %} {"Id": "contentitemid", "MenuItems": [],"WithDefaults": true} {% endcapture %}
/// { jsonData | actions: returnUrl: 'some/return/url', title: 'Of the Button Shape' }
/// </code>
/// </remarks>
public class ActionsLiquidFilter : ILiquidFilter
{
    private readonly IHttpContextAccessor _hca;
    private readonly LinkGenerator _linkGenerator;
    private readonly IShapeFactory _shapeFactory;
    private readonly IDisplayHelper _displayHelper;
    private readonly IStringLocalizer<ActionsLiquidFilter> T;
    private readonly IStringLocalizer<ActionsDescriptor> _actionsDescriptorStringLocalizer;

    public ActionsLiquidFilter(
        IHttpContextAccessor hca,
        LinkGenerator linkGenerator,
        IShapeFactory shapeFactory,
        IDisplayHelper displayHelper,
        IStringLocalizer<ActionsLiquidFilter> stringLocalizer,
        IStringLocalizer<ActionsDescriptor> actionsDescriptorStringLocalizer)
    {
        _hca = hca;
        _linkGenerator = linkGenerator;
        _shapeFactory = shapeFactory;
        _displayHelper = displayHelper;
        T = stringLocalizer;
        _actionsDescriptorStringLocalizer = actionsDescriptorStringLocalizer;
    }

    public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
    {
        // These variables are declared separately because otherwise nameof wouldn't work claiming that the variable
        // doesn't exist in the scope. Once the variable is declared nameof can use it even if it's not defined yet.
        string title, returnUrl;
        title = arguments.HasNamed(nameof(title)) ? arguments[nameof(title)].ToStringValue() : T["Actions"];
        returnUrl = arguments.HasNamed(nameof(returnUrl)) ? arguments[nameof(returnUrl)].ToStringValue() : null;

        return input?.Type switch
        {
            FluidValues.String => FromObjectAsync(new ActionsDescriptor { Id = input!.ToStringValue() }, title, returnUrl),
            FluidValues.Object => input!.ToObjectValue() switch
            {
                ActionsDescriptor model => FromObjectAsync(model, title, returnUrl),
                JsonNode jsonNode => FromObjectAsync(jsonNode.ToObject<ActionsDescriptor>(), title, returnUrl),
                { } unknown => throw GetException(unknown),
                _ => throw new ArgumentNullException(nameof(input)),
            },
            _ => throw GetException(input?.Type),
        };
    }

    private async ValueTask<FluidValue> FromObjectAsync(ActionsDescriptor descriptor, string title, string returnUrl)
    {
        IShape shape = await _shapeFactory.New.Lombiq_Datatables_Actions(
            ButtonTitle: title,
            ExportLinks: descriptor.GetAllMenuItems(_hca.HttpContext, _linkGenerator, _actionsDescriptorStringLocalizer, returnUrl));
        var content = await _displayHelper.ShapeExecuteAsync(shape);

        await using var stringWriter = new StringWriter();
        content.WriteTo(stringWriter, HtmlEncoder.Default);

        return FluidValue.Create(new HtmlString(stringWriter.ToString()), TemplateOptions.Default);
    }

    private static InvalidOperationException GetException(object input) =>
        new($"String {nameof(ContentItem.ContentItemId)}, {nameof(ActionsDescriptor)} or {nameof(JObject)} of " +
            $"{nameof(ActionsDescriptor)} expected. Got '{input}' instead.");
}
