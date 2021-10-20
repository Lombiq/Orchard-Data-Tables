﻿/**
 * @summary     Lombiq - Data Tables
 * @description Abstraction over the jQuery.DataTables plugin to display Query results in a data table.
 * @version     1.0
 * @file        lombiq-datatables.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_DataTables";

    var defaults = {
        dataTablesOptions: {
            searching: false,
            paging: true,
            processing: true,
            info: false,
            lengthChange: false,
            scrollX: true,
            colReorder: true,
            stateSave: true,
            stateSaveParams: function (settings, data) {
                // The paging will be reset back to the first page instead of saving the current page.
                data.start = 0;
            }
        },
        rowClassName: "",
        queryId: "",
        dataProvider: "",
        rowsApiUrl: "",
        serverSidePagingEnabled: false,
        queryStringParametersLocalStorageKey: "",
        childRowOptions: {
            childRowsEnabled: false,
            asyncLoading: false,
            apiUrl: "",
            childRowDisplayType: "",
            additionalDataTablesOptions: {
                "columnDefs": [{ "orderable": false, targets: 0 }],
                "order": [[1, 'asc']]
            },
            childRowClassName: "",
            toggleChildRowButtonClassName: "",
            childRowVisibleClassName: ""
        },
        progressiveLoadingOptions: {
            progressiveLoadingEnabled: false,
            skip: 0,
            batchSize: 0,
            finishedCallback: function () { },
            batchCallback: function () { },
            itemCallback: function () { }
        },
        callbacks: {
            ajaxDataLoadedCallback: function (response) { },
            onInitComplete: function () { }
        }
    };

    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        dataTableElement: null,
        dataTableApi: null,
        originalQueryStringParameters: "",

        /**
         * Initializes the Lombiq DataTable plugin where the jQuery DataTables plugin will be also initialized.
         */
        init: function () {
            var plugin = this;

            plugin.originalQueryStringParameters = new URI().search(true);

            if (!plugin.settings.queryStringParametersLocalStorageKey) {
                plugin.settings.queryStringParametersLocalStorageKey = "DataTables_" + window.location.pathname;
            }

            var dataTablesOptions = $.extend({}, plugin.settings.dataTablesOptions);

            dataTablesOptions.rowCallback = function (row, data, index) {
                if (data.id) {
                    $(row)
                        .addClass(plugin.settings.rowClassName)
                        .attr("data-contentitemid", data.id);
                }
            };

            // This is a workaround to properly adjust column widths.
            dataTablesOptions.initComplete = function () {
                plugin.adjustColumns();
                plugin.settings.callbacks.onInitComplete();
            };

            if (plugin.settings.childRowOptions.childRowsEnabled) {
                dataTablesOptions.order = [[1, "asc"]];
                dataTablesOptions.columnDefs = [{
                    orderable: false,
                    defaultContent: "<div class=\"btn button " + plugin.settings.childRowOptions.toggleChildRowButtonClassName + "\"></div>",
                    targets: 0
                }];
            }

            // Initialize server-side paging unless progressive loading is enabled.
            if (plugin.settings.serverSidePagingEnabled &&
                !plugin.settings.progressiveLoadingOptions.progressiveLoadingEnabled) {
                dataTablesOptions.serverSide = true;
                dataTablesOptions.ajax = {
                    method: "GET",
                    url: plugin.settings.rowsApiUrl,
                    data: function (params) {
                        var internalParameters = plugin.cleanUpDataTablesAjaxParameters(params);

                        var extendedParameters = $.extend({}, internalParameters, {
                            queryId: plugin.settings.queryId,
                            dataProvider: plugin.settings.dataProvider,
                            originalUrl: window.location.href
                        });

                        if (plugin.settings.queryStringParametersLocalStorageKey) {
                            var additionalQueryStringParameters = localStorage.getItem(plugin.settings.queryStringParametersLocalStorageKey);
                            additionalQueryStringParameters = !additionalQueryStringParameters ? {} : JSON.parse(additionalQueryStringParameters);
                            var colReorderArray = {};
                            colReorderArray["ColReorder"] = additionalQueryStringParameters.ColReorder;

                            $.extend(true, extendedParameters, colReorderArray);

                            plugin.setQueryStringParameters(extendedParameters);
                        }

                        return plugin.buildQueryStringParameters(extendedParameters);
                    },
                    dataSrc: function (response) {
                        plugin.settings.callbacks.ajaxDataLoadedCallback(response);

                        return response.data;
                    }
                };
            }

            // Send the datatable state to /dev/null instead of the default local storage. 
            // We can't use this callback for ColReorder because it's called not just on save but on draw too.
            dataTablesOptions.stateSaveCallback = function (settings, data) { }

            // Load the datatable state from the custom local storage.
            dataTablesOptions.stateLoadCallback = function (settings, callback) {
                var additionalQueryStringParameters = plugin.getQueryStringParameters();
                settings.oLoadedState = additionalQueryStringParameters;

                return additionalQueryStringParameters;
            }

            plugin.dataTableElement = $(plugin.element).dataTable(dataTablesOptions);
            plugin.dataTableApi = plugin.dataTableElement.api();

            plugin.dataTableElement.on("column-reorder.dt order.dt", function (e, settings, details) {
                var additionalQueryStringParameters = localStorage.getItem(plugin.settings.queryStringParametersLocalStorageKey);
                additionalQueryStringParameters = !additionalQueryStringParameters ? {} : JSON.parse(additionalQueryStringParameters);
                var colReorderArray = {};
                var rowSortArray = {};
                colReorderArray["ColReorder"] = plugin.dataTableApi.colReorder.order();
                rowSortArray["SortDirection"] = plugin.dataTableApi.order()[0][1];
               
                if (plugin.dataTableApi.column(0).header().innerHTML.trim() == "Actions") {
                    rowSortArray["SortColumnIndex"] = plugin.dataTableApi.order()[0][0] - 1;

                    for (var i = 1; i < colReorderArray["ColReorder"].length; i++) {
                        colReorderArray["ColReorder"][i] -= 1;
                    }
                    colReorderArray["ColReorder"].shift();
                } else {
                    rowSortArray["SortColumnIndex"] = plugin.dataTableApi.order()[0][0];
                }

                $.extend(true, additionalQueryStringParameters, colReorderArray);
                $.extend(true, additionalQueryStringParameters, rowSortArray);

                plugin.setQueryStringParameters(additionalQueryStringParameters);
            });

            // Register toggle button click listeners if child rows are enabled.
            if (plugin.settings.childRowOptions.childRowsEnabled) {
                plugin.dataTableElement.on("click", "." + plugin.settings.childRowOptions.toggleChildRowButtonClassName, function () {
                    var parentRowElement = $(this).closest("tr");

                    if (plugin.settings.childRowOptions.asyncLoading) {
                        var contentItemId = parentRowElement.attr("data-contentitemid");

                        $.ajax({
                            type: "GET",
                            url: plugin.settings.childRowOptions.apiUrl,
                            data: {
                                contentItemId: contentItemId,
                                dataProvider: plugin.settings.dataProvider,
                                originalUrl: window.location.href
                            },
                            success: function (data) {
                                if (!data.error) {
                                    plugin.toggleChildRow(parentRowElement, data.content);
                                }
                                else {
                                    alert(data.error);
                                }
                            }
                        });
                    }
                    else {
                        var childRowContent = $("[data-parent='" + parentRowElement.attr("id") + "']").html();

                        plugin.toggleChildRow(parentRowElement, childRowContent);
                    }
                });
            }

            // Fetch items if progressive loading is enabled.
            if (!plugin.settings.serverSidePagingEnabled &&
                plugin.settings.progressiveLoadingOptions.progressiveLoadingEnabled) {
                plugin.fetchRowsProgressively();
            }
        },

        /* Retrieves the additional query string parameters from local storage and deserializes if they are valid. */
        getQueryStringParameters: function () {
            var additionalQueryStringParameters = localStorage.getItem(this.settings.queryStringParametersLocalStorageKey);

            additionalQueryStringParameters = !additionalQueryStringParameters ? {} : JSON.parse(additionalQueryStringParameters);

            return additionalQueryStringParameters;
        },

        /* Stores the additional query string parameters in local storage. */
        setQueryStringParameters: function (queryStringParameters) {
            localStorage.setItem(this.settings.queryStringParametersLocalStorageKey, JSON.stringify(queryStringParameters));
        },

        /**
         * Removes unnecessary DataTables ajax parameters and updates property names and values to match server data model.
         * @param {object} parameters Parameters generated by the DataTables plugin to be sent to the server.
         * @returns {object} Cleaned-up query string parameters.
         */
        cleanUpDataTablesAjaxParameters: function (parameters) {
            // Replacing column index to column name. 
            // Also rename properties and values to match back-end data model.
            for (var i = 0; i < parameters.order.length; i++) {
                var orderData = parameters.order[i];
                var columnIndex = orderData.column;
                orderData.column = parameters.columns[columnIndex].name;
                orderData.direction = orderData.dir === "asc" ? "ascending" : "descending";
                delete orderData.dir;
            }

            // Send only filtered column data.
            var columnFilters = [];
            for (var j = 0; j < parameters.columns.length; j++) {
                var column = parameters.columns[j];
                if (column.search.value) columnFilters.push(column);
            }

            parameters.columnFilters = columnFilters;
            delete parameters.columns;

            // Remove global search parameters if there is no search value given.
            if (!parameters.search.value) delete parameters.search;
            return parameters;
        },

        /**
         * Shows or hides child row filled with the given content.
         * @param {JQuery} parentRowElement Parent row element where the child row will be displayed.
         * @param {object} childRowContent Content of the child row. A <tr> wrapper will be added automatically.
         */
        toggleChildRow: function (parentRowElement, childRowContent) {
            var plugin = this;

            var dataTableRow = plugin.dataTableApi.row(parentRowElement);

            if (dataTableRow.child.isShown()) {
                dataTableRow.child.hide();

                parentRowElement.removeClass(plugin.settings.childRowOptions.childRowVisibleClassName);
            }
            else {
                dataTableRow.child(childRowContent, plugin.settings.childRowOptions.childRowClassName).show();

                parentRowElement.addClass(plugin.settings.childRowOptions.childRowVisibleClassName);
            }
        },

        /**
         * Fetches the rows from the API using progressive loading.
         */
        fetchRowsProgressively: function () {
            var plugin = this;

            if (!plugin.settings.progressiveLoadingOptions.progressiveLoadingEnabled) return;

            plugin.dataTableApi.processing(true);

            var options = {
                queryId: plugin.settings.queryId,
                dataProvider: plugin.settings.dataProvider,
                apiUrl: plugin.settings.rowsApiUrl,
                itemCallback: function (id, data, response) {
                    if (plugin.settings.progressiveLoadingOptions.itemCallback) {
                        plugin.settings.progressiveLoadingOptions.itemCallback(id, data, response);
                    }

                    plugin.dataTableApi.row.add(data).draw();
                },
                finishedCallback: function (success, total) {
                    if (plugin.settings.progressiveLoadingOptions.finishedCallback) {
                        plugin.settings.progressiveLoadingOptions.finishedCallback(success, total);
                    }

                    plugin.dataTableApi.processing(false);
                }
            };

            plugin.progressiveLoad($.extend({}, plugin.settings.progressiveLoadingOptions, options));
        },

        /**
         * Builds query string parameters that includes the given parameters and the current URL's query string parameters.
         * The original query string parameters are traditionally encoded to preserve their query string keys,
         * while the ones used by DataTables aren't.
         * @param {object} data Data that needs to be merged with the current URL query string parameters.
         * @returns {object} Merged query string parameters.
         */
        buildQueryStringParameters: function (data) {
            var finalQueryString = "";

            // This is necessary to preserve the original structure of the initial query string:
            // Traditional encoding ensures that if a key has multiple values (e.g. "?name=value1&name=value2"),
            // then the key won't be changed to "name[]".
            var originalQueryStringEncoded = $.param(this.originalQueryStringParameters, true);

            if (originalQueryStringEncoded) {
                finalQueryString += originalQueryStringEncoded + "&";
            }

            finalQueryString += $.param(data);

            return finalQueryString;
        },

        /**
         * Low-level functionality for loading rows from the API. The result is accessible using the callback.
         * @param {number} skip Number of items to be skipped by the API.
         * @param {Object} options Options required for the API call (e.g. API URL, data provider).
         * @param {callback} callback Callback for returning rows.
         */
        loadRows: function (skip, options, callback) {
            var plugin = this;

            $.ajax({
                type: "GET",
                url: options.apiUrl,
                data: plugin.buildQueryStringParameters({
                    queryId: options.queryId,
                    start: skip,
                    length: options.batchSize,
                    dataProvider: options.dataProvider,
                    originalUrl: window.location.href
                }),
                success: function (response) {
                    if (callback) {
                        callback(!response.error, response);
                    }
                },
                fail: function () {
                    if (callback) {
                        callback(false);
                    }
                }
            });
        },

        /**
         * Adjusts datatable columns.
         */
        adjustColumns: function () {
            var plugin = this;

            // This is a workaround to properly adjust column widths.
            setTimeout(function () {
                plugin.dataTableApi.columns.adjust();
            }, 10);
        },

        /**
         * Low-level functionality of progressive loading. It will fetch content shapes from the given API.
         * The shapes will be processed using callbacks.
         * @param {Object} options Progressive loading options including API URL and callbacks.
         */
        progressiveLoad: function (options) {
            var plugin = this;
            var total = 0;
            var skip = options.skip;

            var callback = function (success, response) {
                if (success && response) {
                    var count = response.data.length;
                    total += count;

                    if (options.batchCallback) {
                        options.batchCallback(response, total);
                    }

                    if (count > 0 && options.itemCallback) {
                        $.each(response.data, function (index, value) {
                            options.itemCallback(index, value, response);
                        });
                    }

                    if (count > 0 && count >= options.batchSize) {
                        skip += count;

                        plugin.loadRows(skip, options, callback);
                    }
                    else {
                        if (options.finishedCallback) {
                            options.finishedCallback(true, total);
                        }
                    }
                }
                else {
                    if (response) {
                        alert(response.error);
                    }

                    if (options.finishedCallback) {
                        options.finishedCallback(false, total);
                    }
                }
            };

            plugin.loadRows(skip, options, callback);
        }
    });

    $.fn[pluginName] = function (options) {
        // Return null if the element query is invalid.
        if (!this || this.length === 0) return null;

        // "map" makes it possible to return the already existing or currently initialized plugin instances.
        return this.map(function () {
            // If "options" is defined, but the plugin is not instantiated on this element ...
            if (options && !$.data(this, "plugin_" + pluginName)) {
                // ... then create a plugin instance ...
                $.data(this, "plugin_" + pluginName, new Plugin($(this), options));
            }

            // ... and then return the plugin instance, which might be null
            // if the plugin is not instantiated on this element and "options" is undefined.
            return $.data(this, "plugin_" + pluginName);
        });
    };
})(jQuery, window, document);