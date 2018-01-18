using Lombiq.DataTables.Layouts;
using Lombiq.DataTables.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.DataTables.Forms
{
    internal class DataTableLayoutFormElements
    {
        public string DataProvider { get; set; }
        public bool ProgressiveLoadingEnabled { get; set; }
        public bool ChildRowsEnabled { get; set; }


        public DataTableLayoutFormElements(dynamic formState)
        {
            DataProvider = (string)formState?.DataProvider ?? "";
            ChildRowsEnabled = (bool?)formState?.ChildRowsEnabled ?? false;
            ProgressiveLoadingEnabled = (bool?)formState?.ProgressiveLoadingEnabled ?? false;
        }
    }

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
                        Title: T("Data Table options"),
                        _DataProvider: _shapeFactory.SelectList(
                            Id: nameof(DataTableLayoutFormElements.DataProvider),
                            Name: nameof(DataTableLayoutFormElements.DataProvider),
                            Title: T("Data provider name"),
                            Description: T("Data provider that provides rows to the DataTable after paging or filtering."),
                            Multiple: false),
                        _ProgressiveLoadingEnabled: _shapeFactory.Checkbox(
                            Id: nameof(DataTableLayoutFormElements.ProgressiveLoadingEnabled),
                            Name: nameof(DataTableLayoutFormElements.ProgressiveLoadingEnabled),
                            Title: T("Progressive loading"),
                            Value: "true",
                            Description: T("When enabled, the items are loaded progressively immediately after opening the page instead of using server-side paging.")),
                        _ChildRowsEnabled: _shapeFactory.Checkbox(
                            Id: nameof(DataTableLayoutFormElements.ChildRowsEnabled),
                            Name: nameof(DataTableLayoutFormElements.ChildRowsEnabled),
                            Title: T("Child rows"),
                            Value: "true",
                            Description: T("When enabled, child rows can be displayed in the data table using a specific display type."))
                        )
                    );

                foreach (var dataProvider in _dataTableDataProviders)
                    form._Options._DataProvider.Add(
                        new SelectListItem
                        {
                            Value = dataProvider.Name,
                            Text = dataProvider.Description.Text
                        });

                return form;
            });

        }
    }
}