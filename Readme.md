# Lombiq Data Tables for Orchard Core


## About

An Orchard Core wrapper around the [DataTables](https://datatables.net/) library for displaying tabular data from [Queries](https://docs.orchardcore.net/en/latest/docs/reference/modules/Queries/) and custom data sources. 

Note that this module has an Orchard 1 version in the [dev-orchard-1 branch](https://github.com/Lombiq/Orchard-Data-Tables/tree/dev-orchard-1).


## How to use


If you'd like to learn by example, check out our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!
- [The <datatable> Tag Helper](Lombiq.DataTables.Samples/Views/Sample/DataTableTagHelper.cshtml)
- [JSON Data Provider](Lombiq.DataTables.Samples/Services/SampleJsonResultDataTableDataProvider.cs)
- [Index-based Data Provider](Lombiq.DataTables.Samples/Indexes/EmployeeDataTableIndex.cs)

### Static Content With Tag Helper

In the most basic form you can use [DOM sourced data](https://datatables.net/examples/data_sources/dom.html) with the `<datatable>` tag helper.


### Asynchronous Content With a Custom Provider

The `Lombiq_DataTable` shape can display sortable, searchable, paginated data, but you must make your own data provider by extending `DataTableDataProviderBase` or either of the abstract base classes provided by us (`JsonResultDataTableDataProvider`, `IndexBasedDataTableDataProvider`). The latter is for complex database queries that would take a long time to calculate on demand, are tied to one or more content items and can be indexed ahead of time.

Once you have your data provider, it must be registered using `services.AddDataTableDataProvider<TProvider>()`, or if it's index-based then using `services.AddIndexBasedDataTableProvider<TIndex, TGenerator, TMigration, TProvider>()`.

If you need an admin page with just one data table you don't need to define a view, just link to `/Admin/DataTable/{providerName}/{queryId?}`.

You can find an example using [LinqToDb](https://github.com/Lombiq/Helpful-Libraries/blob/dev/Lombiq.HelpfulLibraries.LinqToDb/Readme.md) queries do display a table of deleted content items at [DeletedContentItemDataTableDataProvider.cs](Lombiq.DataTables/Services/DeletedContentItemDataTableDataProvider.cs).


## Client-side Extensibility


### With Shape

For configuring the setup logic or global customization you can override the `Lombiq.DataTable.Resources` shape, for example in your theme. This is displayed before the `Lombiq.DataTable` shape, but after the basic configurations have been initialized (note: not applicable to the `<datatable>` tag helper).

Here you can edit the `window.dataTableResources` object. It contains the following properties:
- `options`: a regular DataTables [options](https://datatables.net/manual/options) object. For example use `window.dataTableResources.options.dom` to set the table layout for your theme.
- `created`: a callback (`function (wrapperElementJQuery, plugin)`) that's executed after the plugin has been initialized. This is the ideal place for adding custom controls.


### With Events

You can use all the regular DataTables [events](https://datatables.net/manual/events), but we've also implemented some custom ones. While DataTables events use the `.dt` namespace, these use the `.lombiqdt` namespace to avoid future conflict. They target the same DataTable element.

- `popstate.lombiqdt`: fired after a history back/forward between different states of the table. The event is `{ plugin: jQueryPlugin, state: { providerName, data, order }, cancel: false }`. Change `cancel` to `true` if you don't want to load this history.
- `createstate.lombiqdt`: fired before a new DataTable history state is placed. The event is `{ plugin, state }` and you can alter the `state` object from the event handler.
- `preXhr.lombiqdt`: fired inside the `options.ajax` function right before the web request is sent out. The event is `{ plugin, requestData, isHistory }`. You can change `requestData` here.


## Vue.js Alternative

You may say, _I Can't Believe It's Not DataTable!_ but it really is not. Use the `<icbin-datatable>` component for scenarios where you need something that matches the look and feel of DataTables and is a reusable Vue component with MVVM style logic.

```vue
<div id="my-app-id" class="my-app">
    <icbin-datatable v-model="data"
                     :columns="columns"
                     :text="text"
                     @@special="onSpecial">
        <div>Content here goes between the page length picker and the table.</div>
    </icbin-datatable>
</div>
```


- `data`: a serialized array of [`VueModel`](Lombiq.DataTables/Models/VueModel.cs) (`v-model` here refers to the `data` property and the `update` event).
- `columns`: a serialized array of [`DataTableColumnDefinition`](Lombiq.DataTables/Models/DataTableColumnDefinition.cs).
- `text`: an object of keys and display texts (i.e. string-string dictionary). Its expected properties are: `lengthPicker`, `displayCount`, `previous`, `next`.
- `onspecial`: a function that receives a cell that has the `VueModel.Special` property and can edit it.

For additional properties and notes on the events take a look at the comments [in the component](Lombiq.DataTables/Assets/Scripts/icbin-datatable.js).

_Note: use `@Json.Serialize()` to automatically camelCase the data for JS._

