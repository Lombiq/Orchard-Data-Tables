/**
 * @summary     Lombiq - Data Tables
 * @description Abstraction over the jQuery.DataTables plugin to display Query results in a data table.
 * @version     1.0
 * @file        lombiq-datatables.js
 * @author      Lombiq Technologies Ltd.
 */

/* global URI */

(function ($, window, document) {
    'use strict';

    var pluginName = 'lombiq_DataTables';
    var useDefaultButtons = 'useDefaultButtons';

    var defaults = {
        dataTablesOptions: {
            searching: true,
            paging: true,
            processing: true,
            info: true,
            lengthChange: true,
            scrollX: true,
            dom: "<'row dataTables_buttons'<'col-md-12'B>>" +
                "<'row dataTables_controls'<'col-md-6 dataTables_length'l><'col-md-6 dataTables_search'f>>" +
                "<'row dataTables_content'<'col-md-12't>>" +
                "<'row dataTables_footer'<'col-md-12'ip>>",
            buttons: useDefaultButtons
        },
        rowClassName: '',
        queryId: '',
        dataProvider: '',
        rowsApiUrl: '',
        serverSidePagingEnabled: false,
        queryStringParametersLocalStorageKey: '',
        templates: {},
        errorsSelector: null,
        childRowOptions: {
            childRowsEnabled: false,
            asyncLoading: false,
            apiUrl: '',
            childRowDisplayType: '',
            additionalDataTablesOptions: {
                columnDefs: [{ orderable: false, targets: 0 }],
                order: [[1, 'asc']]
            },
            childRowClassName: '',
            toggleChildRowButtonClassName: '',
            childRowVisibleClassName: ''
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
            ajaxDataLoadedCallback: function (response) { }
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
        originalQueryStringParameters: '',

        /**
         * Initializes the Lombiq DataTable plugin where the jQuery DataTables plugin will be also initialized.
         */
        init: function () {
            var plugin = this;
            var stateJson = '{}';

            plugin.customizeAjaxParameters = function (parameters) { return parameters; };
            plugin.originalQueryStringParameters = new URI().search(true);

            var dataTablesOptions = $.extend({}, plugin.settings.dataTablesOptions);

            dataTablesOptions.rowCallback = function (row, data, index) {
                if (data.id) {
                    $(row)
                        .addClass(plugin.settings.rowClassName)
                        .attr('data-contentitemid', data.id);
                }
            };

            function convertDate(date) {
                var locale = 'en-US';
                if (plugin.settings.culture) locale = plugin.settings.culture;
                return date.toLocaleDateString(locale);
            }

            // Conditional renderer.
            dataTablesOptions.columnDefs = [{
                targets: '_all',
                render: function (data) {
                    if (data == null) return '';

                    // If data is Boolean.
                    if (data === !!data) return data ? plugin.settings.texts.yes : plugin.settings.texts.no;

                    if ($.isArray(data)) return data.join(', ');

                    var isString = typeof data === 'string';

                    // If data is ISO date.
                    if (isString && data.match(/\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.?\d*([+-][0-2]\d:[0-5]\d|Z)/)) {
                        return convertDate(new Date(data));
                    }

                    // If data is a template.
                    var template = isString ?
                        data.match(/^\s*{{\s*([^:]+)\s*:\s*([^}]*[^ \t}])\s*}}\s*$/) : null;
                    if (template && template[1] && template[2]) {
                        var templateName = template[1];
                        var templateData = template[2];
                        return dataTablesOptions.templates[templateName].replace(/{{\s*data\s*}}/g, templateData);
                    }

                    switch (data.Type) { // eslint-disable-line default-case
                        case 'ExportLink': return '<a href="' + data.Url + '">' + data.Text + '</a>';
                        case 'ExportDate': return convertDate(new Date(data.Year, data.Month - 1, data.Day));
                    }

                    return data;
                }
            }];

            // This is a workaround to properly adjust column widths.
            var originalInitCompleteHandler = dataTablesOptions.initComplete
                ? dataTablesOptions.initComplete
                : function () { };
            dataTablesOptions.initComplete = function () {
                plugin.adjustColumns();
                originalInitCompleteHandler.apply(this);
            };

            if (plugin.settings.childRowOptions.childRowsEnabled) {
                dataTablesOptions.order = [[1, 'asc']];
                dataTablesOptions.columnDefs.push({
                    orderable: false,
                    defaultContent: '<div class="btn button ' + plugin.settings.childRowOptions.toggleChildRowButtonClassName + '"></div>',
                    targets: 0
                });
            }

            // Initialize server-side paging unless progressive loading is enabled.
            if (plugin.settings.serverSidePagingEnabled &&
                !plugin.settings.progressiveLoadingOptions.progressiveLoadingEnabled) {
                dataTablesOptions.serverSide = true;
                dataTablesOptions.ajax = {
                    method: 'GET',
                    url: plugin.settings.rowsApiUrl,
                    data: function (params) {
                        var internalParameters = plugin.cleanUpDataTablesAjaxParameters(params);

                        var extendedParameters = plugin.customizeAjaxParameters($.extend({}, internalParameters, {
                            queryId: plugin.settings.queryId,
                            dataProvider: plugin.settings.dataProvider,
                            originalUrl: window.location.href
                        }));
                        var jsonParameters = JSON.stringify(extendedParameters);
                        stateJson = jsonParameters;

                        if (plugin.settings.queryStringParametersLocalStorageKey) {
                            localStorage.setItem(
                                plugin.settings.queryStringParametersLocalStorageKey,
                                jsonParameters
                            );
                        }

                        if (plugin.settings.errorsSelector) $(plugin.settings.errorsSelector).hide();

                        if (!jsonParameters || !jsonParameters.match || jsonParameters.match(/^\s*$/)) {
                            alert('jsonParameters is null or empty!\n' +
                                'params:\n' + JSON.stringify(params) + '\n' +
                                'internalParameters:\n' + JSON.stringify(internalParameters) + '\n' +
                                'extendedParameters:\n' + JSON.stringify(extendedParameters) + '\n' +
                                'jsonParameters:\n' + JSON.stringify(jsonParameters) + '\n');
                        }
                        return plugin.buildQueryStringParameters({ requestJson: jsonParameters });
                    },
                    dataSrc: function (response) {
                        plugin.settings.callbacks.ajaxDataLoadedCallback(response);

                        return response.data;
                    }
                };
            }

            function exportAction(exportAll) {
                return function () {
                    location.href = URI(plugin.settings.export.api)
                        .search({ requestJson: stateJson, exportAll: exportAll });
                };
            }
            function getExportButtons() {
                return [
                    {
                        text: plugin.settings.export.textAll,
                        action: exportAction(true)
                    },
                    {
                        text: plugin.settings.export.textVisible,
                        action: exportAction(false)
                    }
                ];
            }
            if (dataTablesOptions.buttons === useDefaultButtons) {
                dataTablesOptions.buttons = getExportButtons();
            }
            else if (dataTablesOptions.buttons && dataTablesOptions.buttons.forEach) {
                dataTablesOptions.buttons.forEach((button) => {
                    if (button.buttons === useDefaultButtons) button.buttons = getExportButtons();
                });
            }

            if (plugin.settings.errorsSelector) {
                $.fn.dataTable.ext.errMode = 'none';
                $(plugin.element).on('error.dt', (e, settings, techNote, message) => {
                    $(plugin.settings.errorsSelector).text(message).show();
                });
            }

            plugin.dataTableElement = $(plugin.element).dataTable(dataTablesOptions);
            plugin.dataTableApi = plugin.dataTableElement.api();

            // Register toggle button click listeners if child rows are enabled.
            if (plugin.settings.childRowOptions.childRowsEnabled) {
                plugin.dataTableElement.on('click', '.' + plugin.settings.childRowOptions.toggleChildRowButtonClassName, function () {
                    var parentRowElement = $(this).closest('tr');

                    if (plugin.settings.childRowOptions.asyncLoading) {
                        var contentItemId = parentRowElement.attr('data-contentitemid');

                        $.ajax({
                            type: 'GET',
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
                        var childRowContent = $('[data-parent="' + parentRowElement.attr('id') + '"]').html();

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

        /**
         * Removes unnecessary DataTables ajax parameters and updates property names and values to match server data
         * model.
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
                orderData.direction = orderData.dir === 'asc' ? 'ascending' : 'descending';
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
         * Builds query string parameters that includes the given parameters and the current URL's query string
         * parameters.
         * The original query string parameters are traditionally encoded to preserve their query string keys,
         * while the ones used by DataTables aren't.
         * @param {object} data Data that needs to be merged with the current URL query string parameters.
         * @returns {object} Merged query string parameters.
         */
        buildQueryStringParameters: function (data) {
            var finalQueryString = '';

            // This is necessary to preserve the original structure of the initial query string:
            // Traditional encoding ensures that if a key has multiple values (e.g. "?name=value1&name=value2"),
            // then the key won't be changed to "name[]".
            var originalQueryStringEncoded = $.param(this.originalQueryStringParameters, true);

            if (originalQueryStringEncoded) {
                finalQueryString += originalQueryStringEncoded + '&';
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
                type: 'GET',
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
            setTimeout(() => {
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
                        $.each(response.data, (index, value) => {
                            options.itemCallback(index, value, response);
                        });
                    }

                    if (count > 0 && count >= options.batchSize) {
                        skip += count;

                        plugin.loadRows(skip, options, callback);
                    }
                    else if (options.finishedCallback) {
                        options.finishedCallback(true, total);
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
            if (options && !$.data(this, 'plugin_' + pluginName)) {
                // ... then create a plugin instance ...
                $.data(this, 'plugin_' + pluginName, new Plugin($(this), options));
            }

            // ... and then return the plugin instance, which might be null
            // if the plugin is not instantiated on this element and 'options' is undefined.
            return $.data(this, 'plugin_' + pluginName);
        });
    };
})(jQuery, window, document);
