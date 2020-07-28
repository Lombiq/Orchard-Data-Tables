using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.DisplayManagement.TagHelpers;

namespace Lombiq.DataTables.TagHelpers
{
    [HtmlTargetElement("datatable")]
    public class DataTableTagHelper : TagHelper
    {
        private readonly ShapeTagHelper _shapeTagHelper;
        
        public DataTableTagHelper(ShapeTagHelper shapeTagHelper)
        {
            _shapeTagHelper = shapeTagHelper;
        }


        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _shapeTagHelper.Type = "Lombiq_DataTable";
            return _shapeTagHelper.ProcessAsync(context, output);
        }
    }
}