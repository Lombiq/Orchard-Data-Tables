using Lombiq.DataTables.Layouts;
using Lombiq.DataTables.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.DataTables.Forms
{
    public class DataTableLayoutForm : Component, IFormProvider
    {
        private readonly dynamic _shapeFactory;
        private readonly IEnumerable<IDataTableDataProvider> _dataTableDataProviders;


        public DataTableLayoutForm(IShapeFactory shapeFactory, IEnumerable<IDataTableDataProvider> dataTableDataProviders)
        {
            _shapeFactory = shapeFactory;
            _dataTableDataProviders = dataTableDataProviders;
        }


        public void Describe(DescribeContext context)
        {
            context.Form(nameof(DataTableLayout), shape =>
            {
                var form = _shapeFactory.Form(
                    Id: nameof(DataTableLayout),
                    _Options: _shapeFactory.Fieldset(
                        Title: T("Data table options"),
                        _DataProvider: _shapeFactory.SelectList(
                            Id: "DataProvider",
                            Name: "DataProvider",
                            Title: T("Data provider name"),
                            Description: T("Data provider that provides rows to the DataTable after paging or filtering."),
                            Multiple: false,
                            Size: _dataTableDataProviders.Count()),
                        _ProgressiveLoadingEnabled: _shapeFactory.Checkbox(
                            Id: "ProgressiveLoadingEnabled",
                            Name: "ProgressiveLoadingEnabled",
                            Title: T("Progressive loading is enabled"),
                            Value: "true",
                            Description: T("If this is enabled the items are loaded progressively immediately after opening the page instead of using server-side paging.")),
                        _ChildRowsEnabled: _shapeFactory.Checkbox(
                            Id: "ChildRowsEnabled",
                            Name: "ChildRowsEnabled",
                            Title: T("Child rows are enabled"),
                            Value: "true",
                            Description: T("If this is enabled child rows are supported in the data tables displaying content items using a specific display type."))
                        )
                    );

                foreach (var dataProvider in _dataTableDataProviders)
                {
                    form._Options._DataProvider.Add(
                        new SelectListItem
                        {
                            Value = dataProvider.Name,
                            Text = dataProvider.Description.Text
                        });
                }

                return form;
            });

        }
    }
}