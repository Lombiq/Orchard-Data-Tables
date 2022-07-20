# Lombiq Data Tables for Orchard Core

[![Lombiq.DataTables NuGet](https://img.shields.io/nuget/v/Lombiq.DataTables?label=Lombiq.DataTables)](https://www.nuget.org/packages/Lombiq.DataTables/) [![Lombiq.DataTables.Samples NuGet](https://img.shields.io/nuget/v/Lombiq.DataTables?label=Lombiq.DataTables.Samples)](https://www.nuget.org/packages/Lombiq.DataTables.Samples/) [![Lombiq.DataTables.Tests.UI NuGet](https://img.shields.io/nuget/v/Lombiq.DataTables?label=Lombiq.DataTables.Tests.UI)](https://www.nuget.org/packages/Lombiq.DataTables.Tests.UI/)

## About

An Orchard Core wrapper around the [DataTables](https://datatables.net/) library for displaying tabular data from custom data sources.

Note that this module has an Orchard 1 version in the [dev-orchard-1 branch](https://github.com/Lombiq/Orchard-Data-Tables/tree/dev-orchard-1).

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## How to use

You can find a sample module with a commented walkthrough in this repository. Check it out [here](Lombiq.DataTables.Samples/Readme.md)!

### Static Content With Tag Helper

In the most basic form you can use [DOM sourced data](https://datatables.net/examples/data_sources/dom.html) with the `<datatable>` tag helper.

### Asynchronous Content With a Custom Provider

The `Lombiq_DataTable` shape can display sortable, searchable, paginated data, but you must make your own data provider by extending `DataTableDataProviderBase` or either of the abstract base classes provided by us (`JsonResultDataTableDataProvider`, `IndexBasedDataTableDataProvider`). The latter is for complex database queries that would take a long time to calculate on demand, are tied to one or more content items and can be indexed ahead of time.

Once you have your data provider, it must be registered using `services.AddDataTableDataProvider<TProvider>()`, or if it's index-based then using `services.AddIndexBasedDataTableProvider<TIndex, TGenerator, TMigration, TProvider>()`.

If you need an admin page with just one data table you don't need to define a view, just link to `/Admin/DataTable/{providerName}/{queryId?}`.

You can find an example using [LinqToDb](https://github.com/Lombiq/Helpful-Libraries/blob/dev/Lombiq.HelpfulLibraries.LinqToDb/Readme.md) queries do display a table of deleted content items at [DeletedContentItemDataTableDataProvider.cs](Lombiq.DataTables/Services/DeletedContentItemDataTableDataProvider.cs).

If you want a display a table of deleted content items, we already happen to have a provider for that called [`DeletedContentItemDataTableDataProvider`](Lombiq.DataTables/Services/DeletedContentItemDataTableDataProvider.cs).

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

## Troubleshooting

### Visual Studio 2022

If you get the IntelliSense-only error _TS6053 File "(...)/types/types.d.ts" not found. The file is in the program because: The root file specified for compilation__ you need to disable automatic type acquisition. This problem isn't present in VS2019 or third party IDEs, and it's cause by the Javascript Language Service. Solution:

1. From the menu select Tools → Options.
2. From the sidebar select Text Editor → JavaScript/Typescript → Project → General.
3. Scroll down and untick _Enable automatic type acquisition (TS 4.1 or later)_.

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
