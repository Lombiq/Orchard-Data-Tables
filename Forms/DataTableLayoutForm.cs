using Lombiq.DataTables.Constants;
using Lombiq.DataTables.Layouts;
using Lombiq.DataTables.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Lombiq.DataTables.Forms
{
    internal class DataTableLayoutFormElements
    {
        public string DataProvider { get; set; }
        public bool ProgressiveLoadingEnabled { get; set; }
        public int DefaultSortingColumnIndex { get; set; }
        public SortingDirection DefaultSortingDirection { get; set; }
        public bool ChildRowsEnabled { get; set; }
        public string DataTableId { get; set; }
        public string DataTableCssClasses { get; set; }
        public string QueryStringParametersLocalStorageKey { get; set; }


        public DataTableLayoutFormElements(dynamic formState)
        {
            DataProvider = (string)formState?.DataProvider ?? "";
            DefaultSortingColumnIndex = int.TryParse((string)formState?.DefaultSortingColumnIndex, out int defaultSortingColumnIndex) ?
                defaultSortingColumnIndex : 0;
            DefaultSortingDirection = Enum.TryParse((string)formState?.DefaultSortingDirection, out SortingDirection defaultSortingDirection) ?
                defaultSortingDirection : SortingDirection.Ascending;
            ChildRowsEnabled = (bool?)formState?.ChildRowsEnabled ?? false;
            ProgressiveLoadingEnabled = (bool?)formState?.ProgressiveLoadingEnabled ?? false;
            DataTableId = (string)formState?.DataTableId ?? "";
            DataTableCssClasses = (string)formState?.DataTableCssClasses ?? "";
            QueryStringParametersLocalStorageKey = (string)formState?.QueryStringParametersLocalStorageKey ?? "";
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
                        // "min" attribute is not supported here, but at least we can use type="number".
                        _DefaultSortingColumnIndex: _shapeFactory.Input(
                            Id: "sortingColumnIndex", Name: nameof(DataTableLayoutFormElements.DefaultSortingColumnIndex), Type: "number",
                            Title: T("Default sorting column index"), Value: 0,
                            Description: T("The zero-based index of the column whose values will define the order of the rows when the table renders.")),
                        _DefaultSortingDirection: _shapeFactory.FieldSet(
                            _Label: _shapeFactory.InputLabel(
                                Title: T("Default sorting direction")),
                            _AnyTerm: _shapeFactory.Radio(
                                Id: "sortingDirectionAscending", Name: nameof(DataTableLayoutFormElements.DefaultSortingDirection),
                                Title: T("Ascending"), Value: SortingDirection.Ascending, Checked: true),
                            _AllTerms: _shapeFactory.Radio(
                                Id: "sortingDirectionDescending", Name: nameof(DataTableLayoutFormElements.DefaultSortingDirection),
                                Title: T("Descending"), Value: SortingDirection.Descending)),
                        _ChildRowsEnabled: _shapeFactory.Checkbox(
                            Id: nameof(DataTableLayoutFormElements.ChildRowsEnabled),
                            Name: nameof(DataTableLayoutFormElements.ChildRowsEnabled),
                            Title: T("Child rows"),
                            Value: "true",
                            Description: T("When enabled, child rows can be displayed in the data table using a specific display type.")),
                        _DataTableId: _shapeFactory.TextBox(
                            Id: nameof(DataTableLayoutFormElements.DataTableId),
                            Name: nameof(DataTableLayoutFormElements.DataTableId),
                            Title: T("Data table ID"),
                            Description: T("The ID of the data table element. Default value is \"{0}\", however, a unique ID is recommended if multiple tables are displayed.", ElementNames.DataTableElementName)),
                        _DataTableCssClasses: _shapeFactory.TextBox(
                            Id: nameof(DataTableLayoutFormElements.DataTableCssClasses),
                            Name: nameof(DataTableLayoutFormElements.DataTableCssClasses),
                            Title: T("Data table CSS classes"),
                            Description: T("CSS classes to add to the data table wrapper element. Note, that it's not the table element itself or the wrapper generated by the plugin. " +
                            "This value shoudn't be \"dataTable\" since it is reserved for the table element by the plugin as technical class name.")),
                        _QueryStringParametersLocalStorageKey: _shapeFactory.TextBox(
                            Id: nameof(DataTableLayoutFormElements.QueryStringParametersLocalStorageKey),
                            Name: nameof(DataTableLayoutFormElements.QueryStringParametersLocalStorageKey),
                            Title: T("Query string parameters local storage key"),
                            Description: T("Query string parameters generated by the DataTables plugin during the server-side loading are stored in the local storage for further usage. " +
                            "If this is empty then the parameters won't be saved in the local storage."))
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