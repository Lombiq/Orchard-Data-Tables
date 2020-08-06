# Lombiq Data Tables for Orchard Core



## About

An Orchard Core wrapper around the [DataTables](https://datatables.net/) library for displaying tabular data from [Queries](https://docs.orchardcore.net/en/dev/docs/reference/modules/Queries/) and custom data sources. 


## How to use

In the most basic form you can use [DOM sourced data](https://datatables.net/examples/data_sources/dom.html) with the `<datatable>` tag helper.

If your data source is a Query, you can use the built-in `QueryDataTableDataProvider`.
Otherwise implement your own provider - to make it simpler use the `JsonResultDataTableDataProvider` as base class. Register the provider using the `services.AddDataTableDataProvider<TProvider>()` extension method and use the `Lombiq_DataTable` shape to display it. 