using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.DisplayManagement.TagHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.DataTables.TagHelpers
{
    [HtmlTargetElement("datatable")]
    public class DataTableTagHelper : DataTableDefinitionViewModel, ITagHelper
    {
        private readonly ShapeTagHelper _shapeTagHelper;
        private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviderAccessor;

        public int Order { get; } = 0;


        public DataTableTagHelper(ShapeTagHelper shapeTagHelper,
            IEnumerable<IDataTableDataProvider> dataTableDataProviderAccessor)
        {
            _shapeTagHelper = shapeTagHelper;
            _shapeTagHelper.Type = "Lombiq_DataTable";
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
        }


        public void Init(TagHelperContext context) { }
        public void Process(TagHelperContext context, TagHelperOutput output) { }

        public async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (ColumnsDefinition == null &&
                _dataTableDataProviderAccessor.GetDataProvider(DataProvider) is { } dataProvider)
                ColumnsDefinition = await dataProvider.GetColumnsDefinitionAsync();
            _shapeTagHelper.Properties["ViewModel"] = this;

            await _shapeTagHelper.ProcessAsync(context, output);
        }
    }
}
