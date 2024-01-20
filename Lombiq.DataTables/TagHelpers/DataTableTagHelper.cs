using Lombiq.DataTables.Constants;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.ResourceManagement;
using System.Text.Encodings.Web;

namespace Lombiq.DataTables.TagHelpers;

[HtmlTargetElement("datatable")]
public class DataTableTagHelper(IResourceManager resourceManager) : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        resourceManager.RegisterResource("script", ResourceNames.DataTables.AutoInit).AtFoot();
        resourceManager.RegisterResource("stylesheet", ResourceNames.DataTables.Bootstrap4).AtHead();

        output.TagName = "table";
        output.AddClass("data-table", HtmlEncoder.Default);
    }
}
