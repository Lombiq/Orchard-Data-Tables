# Lombiq Data Tables for Orchard Core



## About

An Orchard Core wrapper around the [DataTables](https://datatables.net/) library for displaying tabular data from [Queries](https://docs.orchardcore.net/en/dev/docs/reference/modules/Queries/) and custom data sources. 


## How to use

In the most basic form you can use [DOM sourced data](https://datatables.net/examples/data_sources/dom.html) with the `<datatable>` tag helper.

If your data source is a Query, you can use the built-in `QueryDataTableDataProvider`.
Otherwise implement your own provider - to make it simpler use the `JsonResultDataTableDataProvider` as the base class. Register the provider using the `services.AddDataTableDataProvider<TProvider>()` extension method and use the `Lombiq_DataTable` shape to display it. 


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
