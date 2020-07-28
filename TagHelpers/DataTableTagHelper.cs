using System.Threading.Tasks;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.DisplayManagement.TagHelpers;

namespace Lombiq.DataTables.TagHelpers
{
    [HtmlTargetElement("datatable")]
    public class DataTableTagHelper : DataTableDefinitionViewModel, ITagHelper
    {
        private readonly ShapeTagHelper _shapeTagHelper;
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;

        public int Order { get; } = 0;

        public DataTableDefinitionViewModel ViewModel { get; set; }


        public DataTableTagHelper(ShapeTagHelper shapeTagHelper,
            IDataTableDataProviderAccessor dataTableDataProviderAccessor)
        {
            _shapeTagHelper = shapeTagHelper;
            _shapeTagHelper.Type = "Lombiq_DataTable";
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
        }


        public void Init(TagHelperContext context)
        {
        }

        public void Process(TagHelperContext context, TagHelperOutput output)
        {
        }

        public Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ViewModel ??= this;
            ViewModel.ColumnsDefinition ??= _dataTableDataProviderAccessor.GetDataProvider(ViewModel.DataProvider)
                ?.GetColumnsDefinition();
            _shapeTagHelper.Properties[nameof(ViewModel)] = ViewModel;

            return _shapeTagHelper.ProcessAsync(context, output);
        }
    }
}