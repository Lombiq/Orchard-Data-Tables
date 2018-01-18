using Lombiq.DataTables.Forms;
using Lombiq.DataTables.Services;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Layout;
using Orchard.Projections.Services;
using Orchard.Settings;
using System;
using System.Collections.Generic;

namespace Lombiq.DataTables.Layouts
{
    public class DataTableLayout : ILayoutProvider
    {
        private readonly dynamic _shapeFactory;
        private readonly IDataTableDataProviderAccessor _dataTableDataProviderAccessor;
        private readonly Lazy<ISiteService> _lazySiteService;

        public Localizer T { get; set; }


        public DataTableLayout(
            IShapeFactory shapeFactory,
            IDataTableDataProviderAccessor dataTableDataProviderAccessor,
            Lazy<ISiteService> lazySiteService)
        {
            _shapeFactory = shapeFactory;
            _dataTableDataProviderAccessor = dataTableDataProviderAccessor;
            _lazySiteService = lazySiteService;

            T = NullLocalizer.Instance;
        }


        public void Describe(DescribeLayoutContext describe) =>
            describe
                .For("Html", T("Html"), T("Html Layouts"))
                .Element(
                    "DataTable",
                    T("Data Table"),
                    T("Contents are displayed in a jQuery DataTable component."),
                    DisplayLayout,
                    RenderLayout,
                    nameof(DataTableLayout)
                );

        public LocalizedString DisplayLayout(LayoutContext context) =>
            T("Renders contents in a jQuery DataTable.");

        public dynamic RenderLayout(LayoutContext context, IEnumerable<LayoutComponentResult> layoutComponentResults)
        {
            var values = new DataTableLayoutFormElements(context.State);

            var dataProviderName = values.DataProvider;

            return _shapeFactory.Lombiq_DataTable(
                // The QueryId property in the context.State can be invalid since it is exported instead of the Query's identity.
                QueryId: context.LayoutRecord.QueryPartRecord.Id,
                Columns: _dataTableDataProviderAccessor.GetDataProvider(dataProviderName)?.GetColumns(),
                ChildRowsEnabled: values.ChildRowsEnabled,
                ProgressiveLoadingEnabled: values.ProgressiveLoadingEnabled,
                DataProvider: dataProviderName);
        }
    }
}