using Lombiq.DataTables.Forms;
using Lombiq.DataTables.Services;
using Lombiq.DataTables.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Layout;
using Orchard.Projections.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace Lombiq.DataTables.Layouts
{
    public class DataTableLayout : ILayoutProvider
    {
        private readonly dynamic _shapeFactory;
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;

        public IStringLocalizer T { get; }


        public DataTableLayout(
            IShapeFactory shapeFactory,
            IDataTableDataProviderAccessor dataTableDataProviderAccessor,
            IStringLocalizer<DataTableLayout> stringLocalizer)
        {
            _shapeFactory = shapeFactory;
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            T = stringLocalizer;
        }


        public void Describe(DescribeLayoutContext describe) =>
            describe
                .For("Html", T["Html"], T["Html Layouts"])
                .Element("DataTable", T["Data Table"], T["Contents are displayed in a jQuery DataTable component."],
                    DisplayLayout, RenderLayout, nameof(DataTableLayout));

        public LocalizedString DisplayLayout(LayoutContext context) =>
            T["Renders contents in a jQuery DataTable using {0}.", context.State.DataProvider];

        public dynamic RenderLayout(LayoutContext context, IEnumerable<LayoutComponentResult> layoutComponentResults)
        {
            var values = new DataTableLayoutFormElements(context.State);

            var dataProviderName = values.DataProvider;

            return _shapeFactory.Lombiq_DataTable(ViewModel: new DataTableDefinitionViewModel
            {
                // The QueryId property in the context.State can be invalid since it is exported instead of the Query's identity.
                QueryId = context.LayoutRecord.QueryPartRecord.Id,
                ColumnsDefinition = _dataTableDataProviderAccessor.GetDataProvider(dataProviderName)?.GetColumnsDefinition(),
                ChildRowsEnabled = values.ChildRowsEnabled,
                ProgressiveLoadingEnabled = values.ProgressiveLoadingEnabled,
                DataProvider = dataProviderName,
                DataTableId = values.DataTableId,
                DataTableCssClasses = values.DataTableCssClasses,
                QueryStringParametersLocalStorageKey = values.QueryStringParametersLocalStorageKey
            });
        }
    }
}