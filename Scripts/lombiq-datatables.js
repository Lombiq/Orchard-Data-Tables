/**
 * @summary     Lombiq - Data Table
 * @description Abstraction over the jQuery.DataTables plugin to display Query results in a data table..
 * @version     1.0
 * @file        lombiq-datatable.js
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
            bLengthChange: false
        },
        rowClassName: "",
        queryId: "",
        dataProvider: "",
        rowsApiUrl: "",
        serverSidePagingEnabled: false,
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
            finishedCallback: null,
            batchCallback: null,
            itemCallback: null,
            loadingIndicatorClassName: ""
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

        /**
		 * Initializes the Consultant Link DataTable plugin where the jQuery DataTables plugin will be also initialized.
		 */
        init: function () {
            var plugin = this;

            var dataTablesOptions = $.extend({}, plugin.settings.dataTablesOptions);

            dataTablesOptions.rowCallback = function (row, data, index) {
                if (data.id) {
                    $(row).attr("data-contentitemid", data.id);
                }
            }

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
                    method: "POST",
                    url: plugin.settings.rowsApiUrl,
                    data: {
                        queryId: plugin.settings.queryId,
                        dataProvider: plugin.settings.dataProvider
                    }
                }
            }

            plugin.dataTableElement = $(plugin.element).dataTable(dataTablesOptions);
            plugin.dataTableApi = plugin.dataTableElement.api();

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
                                dataProvider: plugin.settings.dataProvider
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
                plugin.fetchRows();
            }
        },

        /**
		 * Shows or hides child row filled with the given content.
		 *  @param {JQuery} parentRowElement Parent row element where the child row will be displayed.
		 *  @param {object} childRowContent Content of the child row. A <tr> wrapper will be added automatically.
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
        fetchRows: function () {
            var plugin = this;

            plugin.dataTableElement.fadeTo(100, .5);

            var options = {
                queryId: plugin.settings.queryId,
                dataProvider: plugin.settings.dataProvider,
                apiUrl: plugin.settings.rowsApiUrl,
                itemCallback: function (id, data, response) {
                    if (plugin.settings.progressiveLoadingOptions.itemCallback) {
                        plugin.settings.progressiveLoadingOptions.itemCallback(id, data, response);
                    }
                    
                    var node = plugin.dataTableApi.row.add(data).draw();
                },
                finishedCallback: function (success, total) {
                    if (plugin.settings.progressiveLoadingOptions.finishedCallback) {
                        plugin.settings.progressiveLoadingOptions.finishedCallback(success, total);
                    }
                    
                    plugin.dataTableElement.fadeTo(100, 1);
                }
            };

            plugin.progressiveLoad($.extend({}, plugin.settings.progressiveLoadingOptions, options))
        },

        /**
         * Low-level functionality of progressive loading. It will fetch content shapes from the given API.
         * The shapes will be processed using callbacks.
         * @param {Object} options Progressive loading options including API URL and callbacks.
         */
        progressiveLoad: function (options) {
            var total = 0;
            var skip = options.skip;

            if (options.loadingIndicatorClassName) {
                $("." + options.loadingIndicatorClassName).show();
            }

            var showIndicator = function (show) {
                if (options.loadingIndicatorClassName) {
                    if (show) {
                        $("." + options.loadingIndicatorClassName).show();
                    }
                    else {
                        $("." + options.loadingIndicatorClassName).hide();
                    }
                }
            }

            showIndicator(true);

            var fetch = function () {
                $.ajax({
                    type: "GET",
                    url: options.apiUrl,
                    data: {
                        queryId: options.queryId,
                        start: skip,
                        length: options.batchSize,
                        dataProvider: options.dataProvider
                    },
                    success: function (response) {
                        if (!response.error) {
                            console.log(response);

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

                                fetch();
                            }
                            else {
                                showIndicator(false);

                                if (options.finishedCallback) {
                                    options.finishedCallback(true, total)
                                }
                            }
                        }
                        else {
                            alert(response.error);
                        }
                    },
                    fail: function () {
                        showIndicator(false);

                        if (options.finishedCallback) {
                            options.finishedCallback(false, total)
                        }
                    }
                });
            }

            fetch();
        }
    });

    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, "plugin_" + pluginName)) {
                $.data(this, "plugin_" + pluginName, new Plugin(this, options));
            }
        });
    };
})(jQuery, window, document);