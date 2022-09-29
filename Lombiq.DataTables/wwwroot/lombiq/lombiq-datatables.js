"use strict";

function _typeof(obj) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (obj) { return typeof obj; } : function (obj) { return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }, _typeof(obj); }

/**
 * @summary     Lombiq - Data Tables
 * @description Abstraction over the jQuery.DataTables plugin to display Query results in a data table.
 * @version     1.0
 * @file        lombiq-datatables.js
 * @author      Lombiq Technologies Ltd.
 */

/* global URI */
(function lombiqDatatables($, window, document, history) {
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
      dom: "<'row dataTables_buttons'<'col-md-12'B>>" + "<'row dataTables_controls'<'col-md-6 dataTables_length'l><'col-md-6 dataTables_search'f>>" + "<'row dataTables_content'<'col-md-12't>>" + "<'row dataTables_footer'<'col-md-12'ip>>",
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
        columnDefs: [{
          orderable: false,
          targets: 0
        }],
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
      finishedCallback: function finishedCallback() {},
      batchCallback: function batchCallback() {},
      itemCallback: function itemCallback() {}
    },
    callbacks: {
      ajaxDataLoadedCallback: function ajaxDataLoadedCallback() {}
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
    init: function init() {
      var plugin = this;
      var stateJson = '{}';

      plugin.customizeAjaxParameters = function customizeParameters(parameters) {
        return parameters;
      };

      plugin.originalQueryStringParameters = new URI().search(true);
      var dataTablesOptions = $.extend({}, plugin.settings.dataTablesOptions);

      dataTablesOptions.rowCallback = function dataTablesRowCallback(row, data) {
        if (data.id) {
          $(row).addClass(plugin.settings.rowClassName).attr('data-contentitemid', data.id);
        }
      };

      function convertDate(date) {
        var locale = 'en-US';
        if (plugin.settings.culture) locale = plugin.settings.culture;
        return date.toLocaleDateString(locale);
      } // Conditional renderer.


      dataTablesOptions.columnDefs = [{
        targets: '_all',
        render: function render(data) {
          if (data == null) return ''; // If data is Boolean.

          if (data === !!data) return data ? plugin.settings.texts.yes : plugin.settings.texts.no;
          if ($.isArray(data)) return data.join(', ');
          var isString = typeof data === 'string'; // If data is ISO date.

          if (isString && data.match(/\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.?\d*([+-][0-2]\d:[0-5]\d|Z)/)) {
            return convertDate(new Date(data));
          } // If data is a template.


          var template = isString ? data.match(/^\s*{{\s*([^:]+)\s*:\s*([^}]*[^ \t}])\s*}}\s*$/) : null;

          if (template && template[1] && template[2]) {
            var templateName = template[1];
            var templateData = template[2];
            return dataTablesOptions.templates[templateName].replace(/{{\s*data\s*}}/g, templateData);
          }

          switch (data.Type) {
            case 'ExportLink':
              return '<a href="' + data.Url + '">' + data.Text + '</a>';

            case 'ExportDate':
              return convertDate(new Date(data.Year, data.Month - 1, data.Day));

            default:
              return data;
          }
        }
      }]; // This is a workaround to properly adjust column widths.

      var originalInitCompleteHandler = dataTablesOptions.initComplete ? dataTablesOptions.initComplete : function emptyFunction() {};

      dataTablesOptions.initComplete = function dataTablesInitComplete() {
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

      var providerName = window.location.href.includes('/Admin/DataTable/') ? window.location.href.replace(/.*\/Admin\/DataTable\/([^/?]+)[/?].*/, '$1') : URI(window.location.href).search(true).providerName;
      plugin.providerName = providerName; // Initialize server-side paging unless progressive loading is enabled.

      if (plugin.settings.serverSidePagingEnabled && !plugin.settings.progressiveLoadingOptions.progressiveLoadingEnabled) {
        var _window$performance$n;

        var $element = $(plugin.element);
        var latestDraw = 0;
        dataTablesOptions.serverSide = true;
        plugin.history = {
          isHistory: false,
          isRedraw: false,
          isFirst: true
        };

        var getJsonParameters = function getJsonParameters(params) {
          var internalParameters = plugin.cleanUpDataTablesAjaxParameters(params);
          var extendedParameters = plugin.customizeAjaxParameters($.extend({}, internalParameters, {
            queryId: plugin.settings.queryId,
            dataProvider: plugin.settings.dataProvider,
            originalUrl: window.location.href
          }));
          var jsonParameters = JSON.stringify(extendedParameters);
          stateJson = jsonParameters;

          if (plugin.settings.queryStringParametersLocalStorageKey && 'localStorage' in window) {
            var key = plugin.settings.queryStringParametersLocalStorageKey;

            try {
              localStorage.setItem(key, jsonParameters);
            } catch (exception) {
              try {
                localStorage[key] = jsonParameters;
              } catch (innerException) {// If localStorage won't work there is nothing to do.
              }
            }
          }

          if (plugin.settings.errorsSelector) $(plugin.settings.errorsSelector).hide();

          if (!jsonParameters || !jsonParameters.match || jsonParameters.match(/^\s*$/)) {
            alert('jsonParameters is null or empty!\n' + 'params:\n' + JSON.stringify(params) + '\n' + 'internalParameters:\n' + JSON.stringify(internalParameters) + '\n' + 'extendedParameters:\n' + JSON.stringify(extendedParameters) + '\n' + 'jsonParameters:\n' + JSON.stringify(jsonParameters) + '\n');
          }

          return jsonParameters;
        };

        var createHistoryState = function createHistoryState(data) {
          var state = {
            data: data,
            providerName: providerName,
            order: $element.DataTable().order()
          };
          var userEvent = {
            plugin: plugin,
            state: state
          };
          $element.trigger('createstate.lombiqdt', userEvent);
          return userEvent.state;
        };

        $element.on('preXhr.dt', function () {
          if (plugin.history.isFirst || plugin.history.isHistory || plugin.history.isRedraw || window.history.state === null) {
            plugin.history.isFirst = false;
            return;
          }

          history.pushState(createHistoryState(), document.title);
        });
        $(window).on('popstate', function (event) {
          var state = event.originalEvent.state;
          if (!state || !state.providerName || state.providerName !== providerName) return;
          plugin.history.isHistory = true;
          var userEvent = {
            plugin: plugin,
            state: state,
            cancel: false
          };
          $element.trigger('popstate.lombiqdt', userEvent);
          if (!userEvent.cancel) $element.DataTable().ajax.reload();
          plugin.history.isHistory = false;
        }); // See: https://stackoverflow.com/questions/5004978/check-if-page-gets-reloaded-or-refreshed-in-javascript/53307588#53307588

        var pageAccessedByReload = ((_window$performance$n = window.performance.navigation) === null || _window$performance$n === void 0 ? void 0 : _window$performance$n.type) === 1 || window.performance.getEntriesByType('navigation').map(function (nav) {
          return nav.type;
        }).includes('reload');

        dataTablesOptions.ajax = function dataTablesOptionsAjax(params, callback) {
          var _history$state, _latestDraw, _requestData$search$v, _requestData$search, _history$state$data$s, _history$state2, _history$state2$data, _history$state2$data$;

          var isNewRequest = pageAccessedByReload || _typeof(history.state) !== 'object' || !((_history$state = history.state) !== null && _history$state !== void 0 && _history$state.data);

          if (isNewRequest) {
            var data = JSON.parse(getJsonParameters(params));
            history.replaceState(createHistoryState(data), document.title);
          }

          var requestData = $.extend({}, history.state.data);
          if (!isNewRequest) requestData.draw = ((_latestDraw = latestDraw) !== null && _latestDraw !== void 0 ? _latestDraw : 0) + 3;
          var $wrapper = $element.closest('.dataTables_wrapper');
          var instance = $element.DataTable();
          $wrapper.find('.dataTables_filter input[type="search"][aria-controls="dataTable"]').val((_requestData$search$v = (_requestData$search = requestData.search) === null || _requestData$search === void 0 ? void 0 : _requestData$search.value) !== null && _requestData$search$v !== void 0 ? _requestData$search$v : '');
          $wrapper.find('.dataTables_length select[aria-controls="dataTable"]').val(requestData.length);
          instance.order(history.state.order);
          instance.search((_history$state$data$s = (_history$state2 = history.state) === null || _history$state2 === void 0 ? void 0 : (_history$state2$data = _history$state2.data) === null || _history$state2$data === void 0 ? void 0 : (_history$state2$data$ = _history$state2$data.search) === null || _history$state2$data$ === void 0 ? void 0 : _history$state2$data$.value) !== null && _history$state$data$s !== void 0 ? _history$state$data$s : '');
          var userEvent = {
            plugin: plugin,
            requestData: requestData,
            isHistory: plugin.history.isHistory
          };
          $element.trigger('preXhr.lombiqdt', userEvent);
          $.ajax({
            method: 'GET',
            url: plugin.settings.rowsApiUrl,
            data: plugin.buildQueryStringParameters({
              requestJson: JSON.stringify(userEvent.requestData)
            }),
            success: function success(response) {
              plugin.settings.callbacks.ajaxDataLoadedCallback(response);
              latestDraw = response.draw;
              $wrapper.attr('data-draw', latestDraw);
              callback(response);
              var page = history.state.data.start / history.state.data.length;
              plugin.history.isRedraw = true;
              if (instance.page() !== page) instance.page(page).draw('page');

              if (instance.page.len() !== history.state.data.length) {
                instance.page.len(history.state.data.length).draw('page');
              }

              plugin.history.isRedraw = false;
            }
          });
        };
      }

      function exportAction(exportAll) {
        return function getExports() {
          window.location.href = URI(plugin.settings.export.api).search({
            requestJson: stateJson,
            exportAll: exportAll
          });
        };
      }

      function getExportButtons() {
        return [{
          text: plugin.settings.export.textAll,
          action: exportAction(true)
        }, {
          text: plugin.settings.export.textVisible,
          action: exportAction(false)
        }];
      }

      if (dataTablesOptions.buttons === useDefaultButtons) {
        dataTablesOptions.buttons = getExportButtons();
      } else if (dataTablesOptions.buttons && dataTablesOptions.buttons.forEach) {
        dataTablesOptions.buttons.forEach(function (button) {
          if (button.buttons === useDefaultButtons) button.buttons = getExportButtons();
        });
      }

      if (plugin.settings.errorsSelector) {
        $.fn.dataTable.ext.errMode = 'none';
        $(plugin.element).on('error.dt', function (e, settings, techNote, message) {
          $(plugin.settings.errorsSelector).text(message).show();
        });
      }

      plugin.dataTableElement = $(plugin.element).dataTable(dataTablesOptions);
      plugin.dataTableApi = plugin.dataTableElement.api(); // Register toggle button click listeners if child rows are enabled.

      if (plugin.settings.childRowOptions.childRowsEnabled) {
        plugin.dataTableElement.on('click', '.' + plugin.settings.childRowOptions.toggleChildRowButtonClassName, function dataTableElementOnClick() {
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
              success: function success(data) {
                if (!data.error) {
                  plugin.toggleChildRow(parentRowElement, data.content);
                } else {
                  alert(data.error);
                }
              }
            });
          } else {
            var childRowContent = $('[data-parent="' + parentRowElement.attr('id') + '"]').html();
            plugin.toggleChildRow(parentRowElement, childRowContent);
          }
        });
      } // Fetch items if progressive loading is enabled.


      if (!plugin.settings.serverSidePagingEnabled && plugin.settings.progressiveLoadingOptions.progressiveLoadingEnabled) {
        plugin.fetchRowsProgressively();
      }
    },

    /**
    * Removes unnecessary DataTables ajax parameters and updates property names and values to match server data
    * model.
    * @param {object} parameters Parameters generated by the DataTables plugin to be sent to the server.
    * @returns {object} Cleaned-up query string parameters.
    */
    cleanUpDataTablesAjaxParameters: function cleanUpDataTablesAjaxParameters(parameters) {
      // Replacing column index to column name.
      // Also rename properties and values to match back-end data model.
      for (var i = 0; i < parameters.order.length; i++) {
        var orderData = parameters.order[i];
        var columnIndex = orderData.column;
        orderData.column = parameters.columns[columnIndex].name;
        orderData.direction = orderData.dir === 'asc' ? 'ascending' : 'descending';
        delete orderData.dir;
      } // Send only filtered column data.


      var columnFilters = [];

      for (var j = 0; j < parameters.columns.length; j++) {
        var column = parameters.columns[j];
        if (column.search.value) columnFilters.push(column);
      }

      parameters.columnFilters = columnFilters;
      delete parameters.columns; // Remove global search parameters if there is no search value given.

      if (!parameters.search.value) delete parameters.search;
      return parameters;
    },

    /**
    * Shows or hides child row filled with the given content.
    * @param {jQuery} parentRowElement Parent row element where the child row will be displayed.
    * @param {object} childRowContent Content of the child row. A <tr> wrapper will be added automatically.
    */
    toggleChildRow: function toggleChildRow(parentRowElement, childRowContent) {
      var plugin = this;
      var dataTableRow = plugin.dataTableApi.row(parentRowElement);

      if (dataTableRow.child.isShown()) {
        dataTableRow.child.hide();
        parentRowElement.removeClass(plugin.settings.childRowOptions.childRowVisibleClassName);
      } else {
        dataTableRow.child(childRowContent, plugin.settings.childRowOptions.childRowClassName).show();
        parentRowElement.addClass(plugin.settings.childRowOptions.childRowVisibleClassName);
      }
    },

    /**
    * Fetches the rows from the API using progressive loading.
    */
    fetchRowsProgressively: function fetchRowsProgressively() {
      var plugin = this;
      if (!plugin.settings.progressiveLoadingOptions.progressiveLoadingEnabled) return;
      plugin.dataTableApi.processing(true);
      var options = {
        queryId: plugin.settings.queryId,
        dataProvider: plugin.settings.dataProvider,
        apiUrl: plugin.settings.rowsApiUrl,
        itemCallback: function itemCallback(id, data, response) {
          if (plugin.settings.progressiveLoadingOptions.itemCallback) {
            plugin.settings.progressiveLoadingOptions.itemCallback(id, data, response);
          }

          plugin.dataTableApi.row.add(data).draw();
        },
        finishedCallback: function finishedCallback(success, total) {
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
    buildQueryStringParameters: function buildQueryStringParameters(data) {
      // This is necessary to preserve the original structure of the initial query string:
      // Traditional encoding ensures that if a key has multiple values (e.g. "?name=value1&name=value2"),
      // then the key won't be changed to "name[]".
      var originalQueryStringEncoded = $.param(this.originalQueryStringParameters, true);
      return (originalQueryStringEncoded ? originalQueryStringEncoded + '&' : '') + $.param(data);
    },

    /**
    * Low-level functionality for loading rows from the API. The result is accessible using the callback.
    * @param {number} skip Number of items to be skipped by the API.
    * @param {Object} options Options required for the API call (e.g. API URL, data provider).
    * @param {callback} callback Callback for returning rows.
    */
    loadRows: function loadRows(skip, options, callback) {
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
        success: function success(response) {
          if (callback) {
            callback(!response.error, response);
          }
        },
        fail: function fail() {
          if (callback) {
            callback(false);
          }
        }
      });
    },

    /**
    * Adjusts datatable columns.
    */
    adjustColumns: function adjustColumns() {
      var plugin = this; // This is a workaround to properly adjust column widths.

      setTimeout(function () {
        plugin.dataTableApi.columns.adjust();
      }, 10);
    },

    /**
    * Low-level functionality of progressive loading. It will fetch content shapes from the given API.
    * The shapes will be processed using callbacks.
    * @param {Object} options Progressive loading options including API URL and callbacks.
    */
    progressiveLoad: function progressiveLoad(options) {
      var plugin = this;
      var total = 0;
      var skip = options.skip;

      var callback = function callback(success, response) {
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
          } else if (options.finishedCallback) {
            options.finishedCallback(true, total);
          }
        } else {
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

  $.fn[pluginName] = function pluginNameFunction(options) {
    // Return null if the element query is invalid.
    if (!this || this.length === 0) return null; // "map" makes it possible to return the already existing or currently initialized plugin instances.

    return this.map(function pluginMapFunction() {
      // If "options" is defined, but the plugin is not instantiated on this element ...
      if (options && !$.data(this, 'plugin_' + pluginName)) {
        // ... then create a plugin instance ...
        $.data(this, 'plugin_' + pluginName, new Plugin($(this), options));
      } // ... and then return the plugin instance, which might be null
      // if the plugin is not instantiated on this element and 'options' is undefined.


      return $.data(this, 'plugin_' + pluginName);
    });
  };
})(jQuery, window, document, window.history);
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJuYW1lcyI6WyJsb21iaXFEYXRhdGFibGVzIiwiJCIsIndpbmRvdyIsImRvY3VtZW50IiwiaGlzdG9yeSIsInBsdWdpbk5hbWUiLCJ1c2VEZWZhdWx0QnV0dG9ucyIsImRlZmF1bHRzIiwiZGF0YVRhYmxlc09wdGlvbnMiLCJzZWFyY2hpbmciLCJwYWdpbmciLCJwcm9jZXNzaW5nIiwiaW5mbyIsImxlbmd0aENoYW5nZSIsInNjcm9sbFgiLCJkb20iLCJidXR0b25zIiwicm93Q2xhc3NOYW1lIiwicXVlcnlJZCIsImRhdGFQcm92aWRlciIsInJvd3NBcGlVcmwiLCJzZXJ2ZXJTaWRlUGFnaW5nRW5hYmxlZCIsInF1ZXJ5U3RyaW5nUGFyYW1ldGVyc0xvY2FsU3RvcmFnZUtleSIsInRlbXBsYXRlcyIsImVycm9yc1NlbGVjdG9yIiwiY2hpbGRSb3dPcHRpb25zIiwiY2hpbGRSb3dzRW5hYmxlZCIsImFzeW5jTG9hZGluZyIsImFwaVVybCIsImNoaWxkUm93RGlzcGxheVR5cGUiLCJhZGRpdGlvbmFsRGF0YVRhYmxlc09wdGlvbnMiLCJjb2x1bW5EZWZzIiwib3JkZXJhYmxlIiwidGFyZ2V0cyIsIm9yZGVyIiwiY2hpbGRSb3dDbGFzc05hbWUiLCJ0b2dnbGVDaGlsZFJvd0J1dHRvbkNsYXNzTmFtZSIsImNoaWxkUm93VmlzaWJsZUNsYXNzTmFtZSIsInByb2dyZXNzaXZlTG9hZGluZ09wdGlvbnMiLCJwcm9ncmVzc2l2ZUxvYWRpbmdFbmFibGVkIiwic2tpcCIsImJhdGNoU2l6ZSIsImZpbmlzaGVkQ2FsbGJhY2siLCJiYXRjaENhbGxiYWNrIiwiaXRlbUNhbGxiYWNrIiwiY2FsbGJhY2tzIiwiYWpheERhdGFMb2FkZWRDYWxsYmFjayIsIlBsdWdpbiIsImVsZW1lbnQiLCJvcHRpb25zIiwic2V0dGluZ3MiLCJleHRlbmQiLCJfZGVmYXVsdHMiLCJfbmFtZSIsImluaXQiLCJwcm90b3R5cGUiLCJkYXRhVGFibGVFbGVtZW50IiwiZGF0YVRhYmxlQXBpIiwib3JpZ2luYWxRdWVyeVN0cmluZ1BhcmFtZXRlcnMiLCJwbHVnaW4iLCJzdGF0ZUpzb24iLCJjdXN0b21pemVBamF4UGFyYW1ldGVycyIsImN1c3RvbWl6ZVBhcmFtZXRlcnMiLCJwYXJhbWV0ZXJzIiwiVVJJIiwic2VhcmNoIiwicm93Q2FsbGJhY2siLCJkYXRhVGFibGVzUm93Q2FsbGJhY2siLCJyb3ciLCJkYXRhIiwiaWQiLCJhZGRDbGFzcyIsImF0dHIiLCJjb252ZXJ0RGF0ZSIsImRhdGUiLCJsb2NhbGUiLCJjdWx0dXJlIiwidG9Mb2NhbGVEYXRlU3RyaW5nIiwicmVuZGVyIiwidGV4dHMiLCJ5ZXMiLCJubyIsImlzQXJyYXkiLCJqb2luIiwiaXNTdHJpbmciLCJtYXRjaCIsIkRhdGUiLCJ0ZW1wbGF0ZSIsInRlbXBsYXRlTmFtZSIsInRlbXBsYXRlRGF0YSIsInJlcGxhY2UiLCJUeXBlIiwiVXJsIiwiVGV4dCIsIlllYXIiLCJNb250aCIsIkRheSIsIm9yaWdpbmFsSW5pdENvbXBsZXRlSGFuZGxlciIsImluaXRDb21wbGV0ZSIsImVtcHR5RnVuY3Rpb24iLCJkYXRhVGFibGVzSW5pdENvbXBsZXRlIiwiYWRqdXN0Q29sdW1ucyIsImFwcGx5IiwicHVzaCIsImRlZmF1bHRDb250ZW50IiwicHJvdmlkZXJOYW1lIiwibG9jYXRpb24iLCJocmVmIiwiaW5jbHVkZXMiLCIkZWxlbWVudCIsImxhdGVzdERyYXciLCJzZXJ2ZXJTaWRlIiwiaXNIaXN0b3J5IiwiaXNSZWRyYXciLCJpc0ZpcnN0IiwiZ2V0SnNvblBhcmFtZXRlcnMiLCJwYXJhbXMiLCJpbnRlcm5hbFBhcmFtZXRlcnMiLCJjbGVhblVwRGF0YVRhYmxlc0FqYXhQYXJhbWV0ZXJzIiwiZXh0ZW5kZWRQYXJhbWV0ZXJzIiwib3JpZ2luYWxVcmwiLCJqc29uUGFyYW1ldGVycyIsIkpTT04iLCJzdHJpbmdpZnkiLCJrZXkiLCJsb2NhbFN0b3JhZ2UiLCJzZXRJdGVtIiwiZXhjZXB0aW9uIiwiaW5uZXJFeGNlcHRpb24iLCJoaWRlIiwiYWxlcnQiLCJjcmVhdGVIaXN0b3J5U3RhdGUiLCJzdGF0ZSIsIkRhdGFUYWJsZSIsInVzZXJFdmVudCIsInRyaWdnZXIiLCJvbiIsInB1c2hTdGF0ZSIsInRpdGxlIiwiZXZlbnQiLCJvcmlnaW5hbEV2ZW50IiwiY2FuY2VsIiwiYWpheCIsInJlbG9hZCIsInBhZ2VBY2Nlc3NlZEJ5UmVsb2FkIiwicGVyZm9ybWFuY2UiLCJuYXZpZ2F0aW9uIiwidHlwZSIsImdldEVudHJpZXNCeVR5cGUiLCJtYXAiLCJuYXYiLCJkYXRhVGFibGVzT3B0aW9uc0FqYXgiLCJjYWxsYmFjayIsImlzTmV3UmVxdWVzdCIsInBhcnNlIiwicmVwbGFjZVN0YXRlIiwicmVxdWVzdERhdGEiLCJkcmF3IiwiJHdyYXBwZXIiLCJjbG9zZXN0IiwiaW5zdGFuY2UiLCJmaW5kIiwidmFsIiwidmFsdWUiLCJsZW5ndGgiLCJtZXRob2QiLCJ1cmwiLCJidWlsZFF1ZXJ5U3RyaW5nUGFyYW1ldGVycyIsInJlcXVlc3RKc29uIiwic3VjY2VzcyIsInJlc3BvbnNlIiwicGFnZSIsInN0YXJ0IiwibGVuIiwiZXhwb3J0QWN0aW9uIiwiZXhwb3J0QWxsIiwiZ2V0RXhwb3J0cyIsImV4cG9ydCIsImFwaSIsImdldEV4cG9ydEJ1dHRvbnMiLCJ0ZXh0IiwidGV4dEFsbCIsImFjdGlvbiIsInRleHRWaXNpYmxlIiwiZm9yRWFjaCIsImJ1dHRvbiIsImZuIiwiZGF0YVRhYmxlIiwiZXh0IiwiZXJyTW9kZSIsImUiLCJ0ZWNoTm90ZSIsIm1lc3NhZ2UiLCJzaG93IiwiZGF0YVRhYmxlRWxlbWVudE9uQ2xpY2siLCJwYXJlbnRSb3dFbGVtZW50IiwiY29udGVudEl0ZW1JZCIsImVycm9yIiwidG9nZ2xlQ2hpbGRSb3ciLCJjb250ZW50IiwiY2hpbGRSb3dDb250ZW50IiwiaHRtbCIsImZldGNoUm93c1Byb2dyZXNzaXZlbHkiLCJpIiwib3JkZXJEYXRhIiwiY29sdW1uSW5kZXgiLCJjb2x1bW4iLCJjb2x1bW5zIiwibmFtZSIsImRpcmVjdGlvbiIsImRpciIsImNvbHVtbkZpbHRlcnMiLCJqIiwiZGF0YVRhYmxlUm93IiwiY2hpbGQiLCJpc1Nob3duIiwicmVtb3ZlQ2xhc3MiLCJhZGQiLCJ0b3RhbCIsInByb2dyZXNzaXZlTG9hZCIsIm9yaWdpbmFsUXVlcnlTdHJpbmdFbmNvZGVkIiwicGFyYW0iLCJsb2FkUm93cyIsImZhaWwiLCJzZXRUaW1lb3V0IiwiYWRqdXN0IiwiY291bnQiLCJlYWNoIiwiaW5kZXgiLCJwbHVnaW5OYW1lRnVuY3Rpb24iLCJwbHVnaW5NYXBGdW5jdGlvbiIsImpRdWVyeSJdLCJzb3VyY2VzIjpbIi4uLy4uL0Fzc2V0cy9TY3JpcHRzL2xvbWJpcS1kYXRhdGFibGVzLmpzIl0sInNvdXJjZXNDb250ZW50IjpbIi8qKlxyXG4gKiBAc3VtbWFyeSAgICAgTG9tYmlxIC0gRGF0YSBUYWJsZXNcclxuICogQGRlc2NyaXB0aW9uIEFic3RyYWN0aW9uIG92ZXIgdGhlIGpRdWVyeS5EYXRhVGFibGVzIHBsdWdpbiB0byBkaXNwbGF5IFF1ZXJ5IHJlc3VsdHMgaW4gYSBkYXRhIHRhYmxlLlxyXG4gKiBAdmVyc2lvbiAgICAgMS4wXHJcbiAqIEBmaWxlICAgICAgICBsb21iaXEtZGF0YXRhYmxlcy5qc1xyXG4gKiBAYXV0aG9yICAgICAgTG9tYmlxIFRlY2hub2xvZ2llcyBMdGQuXHJcbiAqL1xyXG5cclxuLyogZ2xvYmFsIFVSSSAqL1xyXG5cclxuKGZ1bmN0aW9uIGxvbWJpcURhdGF0YWJsZXMoJCwgd2luZG93LCBkb2N1bWVudCwgaGlzdG9yeSkge1xyXG4gICAgY29uc3QgcGx1Z2luTmFtZSA9ICdsb21iaXFfRGF0YVRhYmxlcyc7XHJcbiAgICBjb25zdCB1c2VEZWZhdWx0QnV0dG9ucyA9ICd1c2VEZWZhdWx0QnV0dG9ucyc7XHJcblxyXG4gICAgY29uc3QgZGVmYXVsdHMgPSB7XHJcbiAgICAgICAgZGF0YVRhYmxlc09wdGlvbnM6IHtcclxuICAgICAgICAgICAgc2VhcmNoaW5nOiB0cnVlLFxyXG4gICAgICAgICAgICBwYWdpbmc6IHRydWUsXHJcbiAgICAgICAgICAgIHByb2Nlc3Npbmc6IHRydWUsXHJcbiAgICAgICAgICAgIGluZm86IHRydWUsXHJcbiAgICAgICAgICAgIGxlbmd0aENoYW5nZTogdHJ1ZSxcclxuICAgICAgICAgICAgc2Nyb2xsWDogdHJ1ZSxcclxuICAgICAgICAgICAgZG9tOiBcIjwncm93IGRhdGFUYWJsZXNfYnV0dG9ucyc8J2NvbC1tZC0xMidCPj5cIiArXHJcbiAgICAgICAgICAgICAgICBcIjwncm93IGRhdGFUYWJsZXNfY29udHJvbHMnPCdjb2wtbWQtNiBkYXRhVGFibGVzX2xlbmd0aCdsPjwnY29sLW1kLTYgZGF0YVRhYmxlc19zZWFyY2gnZj4+XCIgK1xyXG4gICAgICAgICAgICAgICAgXCI8J3JvdyBkYXRhVGFibGVzX2NvbnRlbnQnPCdjb2wtbWQtMTIndD4+XCIgK1xyXG4gICAgICAgICAgICAgICAgXCI8J3JvdyBkYXRhVGFibGVzX2Zvb3Rlcic8J2NvbC1tZC0xMidpcD4+XCIsXHJcbiAgICAgICAgICAgIGJ1dHRvbnM6IHVzZURlZmF1bHRCdXR0b25zLFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgcm93Q2xhc3NOYW1lOiAnJyxcclxuICAgICAgICBxdWVyeUlkOiAnJyxcclxuICAgICAgICBkYXRhUHJvdmlkZXI6ICcnLFxyXG4gICAgICAgIHJvd3NBcGlVcmw6ICcnLFxyXG4gICAgICAgIHNlcnZlclNpZGVQYWdpbmdFbmFibGVkOiBmYWxzZSxcclxuICAgICAgICBxdWVyeVN0cmluZ1BhcmFtZXRlcnNMb2NhbFN0b3JhZ2VLZXk6ICcnLFxyXG4gICAgICAgIHRlbXBsYXRlczoge30sXHJcbiAgICAgICAgZXJyb3JzU2VsZWN0b3I6IG51bGwsXHJcbiAgICAgICAgY2hpbGRSb3dPcHRpb25zOiB7XHJcbiAgICAgICAgICAgIGNoaWxkUm93c0VuYWJsZWQ6IGZhbHNlLFxyXG4gICAgICAgICAgICBhc3luY0xvYWRpbmc6IGZhbHNlLFxyXG4gICAgICAgICAgICBhcGlVcmw6ICcnLFxyXG4gICAgICAgICAgICBjaGlsZFJvd0Rpc3BsYXlUeXBlOiAnJyxcclxuICAgICAgICAgICAgYWRkaXRpb25hbERhdGFUYWJsZXNPcHRpb25zOiB7XHJcbiAgICAgICAgICAgICAgICBjb2x1bW5EZWZzOiBbeyBvcmRlcmFibGU6IGZhbHNlLCB0YXJnZXRzOiAwIH1dLFxyXG4gICAgICAgICAgICAgICAgb3JkZXI6IFtbMSwgJ2FzYyddXSxcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgY2hpbGRSb3dDbGFzc05hbWU6ICcnLFxyXG4gICAgICAgICAgICB0b2dnbGVDaGlsZFJvd0J1dHRvbkNsYXNzTmFtZTogJycsXHJcbiAgICAgICAgICAgIGNoaWxkUm93VmlzaWJsZUNsYXNzTmFtZTogJycsXHJcbiAgICAgICAgfSxcclxuICAgICAgICBwcm9ncmVzc2l2ZUxvYWRpbmdPcHRpb25zOiB7XHJcbiAgICAgICAgICAgIHByb2dyZXNzaXZlTG9hZGluZ0VuYWJsZWQ6IGZhbHNlLFxyXG4gICAgICAgICAgICBza2lwOiAwLFxyXG4gICAgICAgICAgICBiYXRjaFNpemU6IDAsXHJcbiAgICAgICAgICAgIGZpbmlzaGVkQ2FsbGJhY2s6IGZ1bmN0aW9uICgpIHsgfSxcclxuICAgICAgICAgICAgYmF0Y2hDYWxsYmFjazogZnVuY3Rpb24gKCkgeyB9LFxyXG4gICAgICAgICAgICBpdGVtQ2FsbGJhY2s6IGZ1bmN0aW9uICgpIHsgfSxcclxuICAgICAgICB9LFxyXG4gICAgICAgIGNhbGxiYWNrczoge1xyXG4gICAgICAgICAgICBhamF4RGF0YUxvYWRlZENhbGxiYWNrOiAoKSA9PiB7IH0sXHJcbiAgICAgICAgfSxcclxuICAgIH07XHJcblxyXG4gICAgZnVuY3Rpb24gUGx1Z2luKGVsZW1lbnQsIG9wdGlvbnMpIHtcclxuICAgICAgICB0aGlzLmVsZW1lbnQgPSBlbGVtZW50O1xyXG4gICAgICAgIHRoaXMuc2V0dGluZ3MgPSAkLmV4dGVuZCh0cnVlLCB7fSwgZGVmYXVsdHMsIG9wdGlvbnMpO1xyXG4gICAgICAgIHRoaXMuX2RlZmF1bHRzID0gZGVmYXVsdHM7XHJcbiAgICAgICAgdGhpcy5fbmFtZSA9IHBsdWdpbk5hbWU7XHJcblxyXG4gICAgICAgIHRoaXMuaW5pdCgpO1xyXG4gICAgfVxyXG5cclxuICAgICQuZXh0ZW5kKFBsdWdpbi5wcm90b3R5cGUsIHtcclxuICAgICAgICBkYXRhVGFibGVFbGVtZW50OiBudWxsLFxyXG4gICAgICAgIGRhdGFUYWJsZUFwaTogbnVsbCxcclxuICAgICAgICBvcmlnaW5hbFF1ZXJ5U3RyaW5nUGFyYW1ldGVyczogJycsXHJcblxyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICogSW5pdGlhbGl6ZXMgdGhlIExvbWJpcSBEYXRhVGFibGUgcGx1Z2luIHdoZXJlIHRoZSBqUXVlcnkgRGF0YVRhYmxlcyBwbHVnaW4gd2lsbCBiZSBhbHNvIGluaXRpYWxpemVkLlxyXG4gICAgICAgICovXHJcbiAgICAgICAgaW5pdDogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICBjb25zdCBwbHVnaW4gPSB0aGlzO1xyXG4gICAgICAgICAgICBsZXQgc3RhdGVKc29uID0gJ3t9JztcclxuXHJcbiAgICAgICAgICAgIHBsdWdpbi5jdXN0b21pemVBamF4UGFyYW1ldGVycyA9IGZ1bmN0aW9uIGN1c3RvbWl6ZVBhcmFtZXRlcnMocGFyYW1ldGVycykgeyByZXR1cm4gcGFyYW1ldGVyczsgfTtcclxuICAgICAgICAgICAgcGx1Z2luLm9yaWdpbmFsUXVlcnlTdHJpbmdQYXJhbWV0ZXJzID0gbmV3IFVSSSgpLnNlYXJjaCh0cnVlKTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGRhdGFUYWJsZXNPcHRpb25zID0gJC5leHRlbmQoe30sIHBsdWdpbi5zZXR0aW5ncy5kYXRhVGFibGVzT3B0aW9ucyk7XHJcblxyXG4gICAgICAgICAgICBkYXRhVGFibGVzT3B0aW9ucy5yb3dDYWxsYmFjayA9IGZ1bmN0aW9uIGRhdGFUYWJsZXNSb3dDYWxsYmFjayhyb3csIGRhdGEpIHtcclxuICAgICAgICAgICAgICAgIGlmIChkYXRhLmlkKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJChyb3cpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5hZGRDbGFzcyhwbHVnaW4uc2V0dGluZ3Mucm93Q2xhc3NOYW1lKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAuYXR0cignZGF0YS1jb250ZW50aXRlbWlkJywgZGF0YS5pZCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH07XHJcblxyXG4gICAgICAgICAgICBmdW5jdGlvbiBjb252ZXJ0RGF0ZShkYXRlKSB7XHJcbiAgICAgICAgICAgICAgICBsZXQgbG9jYWxlID0gJ2VuLVVTJztcclxuICAgICAgICAgICAgICAgIGlmIChwbHVnaW4uc2V0dGluZ3MuY3VsdHVyZSkgbG9jYWxlID0gcGx1Z2luLnNldHRpbmdzLmN1bHR1cmU7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gZGF0ZS50b0xvY2FsZURhdGVTdHJpbmcobG9jYWxlKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgLy8gQ29uZGl0aW9uYWwgcmVuZGVyZXIuXHJcbiAgICAgICAgICAgIGRhdGFUYWJsZXNPcHRpb25zLmNvbHVtbkRlZnMgPSBbe1xyXG4gICAgICAgICAgICAgICAgdGFyZ2V0czogJ19hbGwnLFxyXG4gICAgICAgICAgICAgICAgcmVuZGVyOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChkYXRhID09IG51bGwpIHJldHVybiAnJztcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgLy8gSWYgZGF0YSBpcyBCb29sZWFuLlxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChkYXRhID09PSAhIWRhdGEpIHJldHVybiBkYXRhID8gcGx1Z2luLnNldHRpbmdzLnRleHRzLnllcyA6IHBsdWdpbi5zZXR0aW5ncy50ZXh0cy5ubztcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCQuaXNBcnJheShkYXRhKSkgcmV0dXJuIGRhdGEuam9pbignLCAnKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgaXNTdHJpbmcgPSB0eXBlb2YgZGF0YSA9PT0gJ3N0cmluZyc7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vIElmIGRhdGEgaXMgSVNPIGRhdGUuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGlzU3RyaW5nICYmXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRhdGEubWF0Y2goL1xcZHs0fS1bMDFdXFxkLVswLTNdXFxkVFswLTJdXFxkOlswLTVdXFxkOlswLTVdXFxkXFwuP1xcZCooWystXVswLTJdXFxkOlswLTVdXFxkfFopLykpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGNvbnZlcnREYXRlKG5ldyBEYXRlKGRhdGEpKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vIElmIGRhdGEgaXMgYSB0ZW1wbGF0ZS5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCB0ZW1wbGF0ZSA9IGlzU3RyaW5nID9cclxuICAgICAgICAgICAgICAgICAgICAgICAgZGF0YS5tYXRjaCgvXlxccyp7e1xccyooW146XSspXFxzKjpcXHMqKFtefV0qW14gXFx0fV0pXFxzKn19XFxzKiQvKSA6IG51bGw7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRlbXBsYXRlICYmIHRlbXBsYXRlWzFdICYmIHRlbXBsYXRlWzJdKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IHRlbXBsYXRlTmFtZSA9IHRlbXBsYXRlWzFdO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCB0ZW1wbGF0ZURhdGEgPSB0ZW1wbGF0ZVsyXTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGRhdGFUYWJsZXNPcHRpb25zLnRlbXBsYXRlc1t0ZW1wbGF0ZU5hbWVdLnJlcGxhY2UoL3t7XFxzKmRhdGFcXHMqfX0vZywgdGVtcGxhdGVEYXRhKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHN3aXRjaCAoZGF0YS5UeXBlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNhc2UgJ0V4cG9ydExpbmsnOiByZXR1cm4gJzxhIGhyZWY9XCInICsgZGF0YS5VcmwgKyAnXCI+JyArIGRhdGEuVGV4dCArICc8L2E+JztcclxuICAgICAgICAgICAgICAgICAgICAgICAgY2FzZSAnRXhwb3J0RGF0ZSc6IHJldHVybiBjb252ZXJ0RGF0ZShuZXcgRGF0ZShkYXRhLlllYXIsIGRhdGEuTW9udGggLSAxLCBkYXRhLkRheSkpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBkZWZhdWx0OiByZXR1cm4gZGF0YTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB9XTtcclxuXHJcbiAgICAgICAgICAgIC8vIFRoaXMgaXMgYSB3b3JrYXJvdW5kIHRvIHByb3Blcmx5IGFkanVzdCBjb2x1bW4gd2lkdGhzLlxyXG4gICAgICAgICAgICBjb25zdCBvcmlnaW5hbEluaXRDb21wbGV0ZUhhbmRsZXIgPSBkYXRhVGFibGVzT3B0aW9ucy5pbml0Q29tcGxldGVcclxuICAgICAgICAgICAgICAgID8gZGF0YVRhYmxlc09wdGlvbnMuaW5pdENvbXBsZXRlXHJcbiAgICAgICAgICAgICAgICA6IGZ1bmN0aW9uIGVtcHR5RnVuY3Rpb24oKSB7IH07XHJcbiAgICAgICAgICAgIGRhdGFUYWJsZXNPcHRpb25zLmluaXRDb21wbGV0ZSA9IGZ1bmN0aW9uIGRhdGFUYWJsZXNJbml0Q29tcGxldGUoKSB7XHJcbiAgICAgICAgICAgICAgICBwbHVnaW4uYWRqdXN0Q29sdW1ucygpO1xyXG4gICAgICAgICAgICAgICAgb3JpZ2luYWxJbml0Q29tcGxldGVIYW5kbGVyLmFwcGx5KHRoaXMpO1xyXG4gICAgICAgICAgICB9O1xyXG5cclxuICAgICAgICAgICAgaWYgKHBsdWdpbi5zZXR0aW5ncy5jaGlsZFJvd09wdGlvbnMuY2hpbGRSb3dzRW5hYmxlZCkge1xyXG4gICAgICAgICAgICAgICAgZGF0YVRhYmxlc09wdGlvbnMub3JkZXIgPSBbWzEsICdhc2MnXV07XHJcbiAgICAgICAgICAgICAgICBkYXRhVGFibGVzT3B0aW9ucy5jb2x1bW5EZWZzLnB1c2goe1xyXG4gICAgICAgICAgICAgICAgICAgIG9yZGVyYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICAgICAgZGVmYXVsdENvbnRlbnQ6ICc8ZGl2IGNsYXNzPVwiYnRuIGJ1dHRvbiAnICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGx1Z2luLnNldHRpbmdzLmNoaWxkUm93T3B0aW9ucy50b2dnbGVDaGlsZFJvd0J1dHRvbkNsYXNzTmFtZSArICdcIj48L2Rpdj4nLFxyXG4gICAgICAgICAgICAgICAgICAgIHRhcmdldHM6IDAsXHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgY29uc3QgcHJvdmlkZXJOYW1lID0gd2luZG93LmxvY2F0aW9uLmhyZWYuaW5jbHVkZXMoJy9BZG1pbi9EYXRhVGFibGUvJylcclxuICAgICAgICAgICAgICAgID8gd2luZG93LmxvY2F0aW9uLmhyZWYucmVwbGFjZSgvLipcXC9BZG1pblxcL0RhdGFUYWJsZVxcLyhbXi8/XSspWy8/XS4qLywgJyQxJylcclxuICAgICAgICAgICAgICAgIDogVVJJKHdpbmRvdy5sb2NhdGlvbi5ocmVmKS5zZWFyY2godHJ1ZSkucHJvdmlkZXJOYW1lO1xyXG4gICAgICAgICAgICBwbHVnaW4ucHJvdmlkZXJOYW1lID0gcHJvdmlkZXJOYW1lO1xyXG5cclxuICAgICAgICAgICAgLy8gSW5pdGlhbGl6ZSBzZXJ2ZXItc2lkZSBwYWdpbmcgdW5sZXNzIHByb2dyZXNzaXZlIGxvYWRpbmcgaXMgZW5hYmxlZC5cclxuICAgICAgICAgICAgaWYgKHBsdWdpbi5zZXR0aW5ncy5zZXJ2ZXJTaWRlUGFnaW5nRW5hYmxlZCAmJlxyXG4gICAgICAgICAgICAgICAgIXBsdWdpbi5zZXR0aW5ncy5wcm9ncmVzc2l2ZUxvYWRpbmdPcHRpb25zLnByb2dyZXNzaXZlTG9hZGluZ0VuYWJsZWQpIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0ICRlbGVtZW50ID0gJChwbHVnaW4uZWxlbWVudCk7XHJcblxyXG4gICAgICAgICAgICAgICAgbGV0IGxhdGVzdERyYXcgPSAwO1xyXG5cclxuICAgICAgICAgICAgICAgIGRhdGFUYWJsZXNPcHRpb25zLnNlcnZlclNpZGUgPSB0cnVlO1xyXG4gICAgICAgICAgICAgICAgcGx1Z2luLmhpc3RvcnkgPSB7XHJcbiAgICAgICAgICAgICAgICAgICAgaXNIaXN0b3J5OiBmYWxzZSxcclxuICAgICAgICAgICAgICAgICAgICBpc1JlZHJhdzogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICAgICAgaXNGaXJzdDogdHJ1ZSxcclxuICAgICAgICAgICAgICAgIH07XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc3QgZ2V0SnNvblBhcmFtZXRlcnMgPSBmdW5jdGlvbiAocGFyYW1zKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgaW50ZXJuYWxQYXJhbWV0ZXJzID0gcGx1Z2luLmNsZWFuVXBEYXRhVGFibGVzQWpheFBhcmFtZXRlcnMocGFyYW1zKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgZXh0ZW5kZWRQYXJhbWV0ZXJzID0gcGx1Z2luLmN1c3RvbWl6ZUFqYXhQYXJhbWV0ZXJzKCQuZXh0ZW5kKHt9LCBpbnRlcm5hbFBhcmFtZXRlcnMsIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcXVlcnlJZDogcGx1Z2luLnNldHRpbmdzLnF1ZXJ5SWQsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFQcm92aWRlcjogcGx1Z2luLnNldHRpbmdzLmRhdGFQcm92aWRlcixcclxuICAgICAgICAgICAgICAgICAgICAgICAgb3JpZ2luYWxVcmw6IHdpbmRvdy5sb2NhdGlvbi5ocmVmLFxyXG4gICAgICAgICAgICAgICAgICAgIH0pKTtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBqc29uUGFyYW1ldGVycyA9IEpTT04uc3RyaW5naWZ5KGV4dGVuZGVkUGFyYW1ldGVycyk7XHJcbiAgICAgICAgICAgICAgICAgICAgc3RhdGVKc29uID0ganNvblBhcmFtZXRlcnM7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChwbHVnaW4uc2V0dGluZ3MucXVlcnlTdHJpbmdQYXJhbWV0ZXJzTG9jYWxTdG9yYWdlS2V5ICYmICdsb2NhbFN0b3JhZ2UnIGluIHdpbmRvdykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBrZXkgPSBwbHVnaW4uc2V0dGluZ3MucXVlcnlTdHJpbmdQYXJhbWV0ZXJzTG9jYWxTdG9yYWdlS2V5O1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGxvY2FsU3RvcmFnZS5zZXRJdGVtKGtleSwganNvblBhcmFtZXRlcnMpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNhdGNoIChleGNlcHRpb24pIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgbG9jYWxTdG9yYWdlW2tleV0gPSBqc29uUGFyYW1ldGVycztcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhdGNoIChpbm5lckV4Y2VwdGlvbikge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIElmIGxvY2FsU3RvcmFnZSB3b24ndCB3b3JrIHRoZXJlIGlzIG5vdGhpbmcgdG8gZG8uXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChwbHVnaW4uc2V0dGluZ3MuZXJyb3JzU2VsZWN0b3IpICQocGx1Z2luLnNldHRpbmdzLmVycm9yc1NlbGVjdG9yKS5oaWRlKCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghanNvblBhcmFtZXRlcnMgfHwgIWpzb25QYXJhbWV0ZXJzLm1hdGNoIHx8IGpzb25QYXJhbWV0ZXJzLm1hdGNoKC9eXFxzKiQvKSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhbGVydCgnanNvblBhcmFtZXRlcnMgaXMgbnVsbCBvciBlbXB0eSFcXG4nICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICdwYXJhbXM6XFxuJyArIEpTT04uc3RyaW5naWZ5KHBhcmFtcykgKyAnXFxuJyArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAnaW50ZXJuYWxQYXJhbWV0ZXJzOlxcbicgKyBKU09OLnN0cmluZ2lmeShpbnRlcm5hbFBhcmFtZXRlcnMpICsgJ1xcbicgK1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJ2V4dGVuZGVkUGFyYW1ldGVyczpcXG4nICsgSlNPTi5zdHJpbmdpZnkoZXh0ZW5kZWRQYXJhbWV0ZXJzKSArICdcXG4nICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICdqc29uUGFyYW1ldGVyczpcXG4nICsgSlNPTi5zdHJpbmdpZnkoanNvblBhcmFtZXRlcnMpICsgJ1xcbicpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4ganNvblBhcmFtZXRlcnM7XHJcbiAgICAgICAgICAgICAgICB9O1xyXG5cclxuICAgICAgICAgICAgICAgIGNvbnN0IGNyZWF0ZUhpc3RvcnlTdGF0ZSA9IChkYXRhKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgc3RhdGUgPSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHByb3ZpZGVyTmFtZTogcHJvdmlkZXJOYW1lLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcmRlcjogJGVsZW1lbnQuRGF0YVRhYmxlKCkub3JkZXIoKSxcclxuICAgICAgICAgICAgICAgICAgICB9O1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCB1c2VyRXZlbnQgPSB7IHBsdWdpbiwgc3RhdGUgfTtcclxuICAgICAgICAgICAgICAgICAgICAkZWxlbWVudC50cmlnZ2VyKCdjcmVhdGVzdGF0ZS5sb21iaXFkdCcsIHVzZXJFdmVudCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiB1c2VyRXZlbnQuc3RhdGU7XHJcbiAgICAgICAgICAgICAgICB9O1xyXG5cclxuICAgICAgICAgICAgICAgICRlbGVtZW50Lm9uKCdwcmVYaHIuZHQnLCAoKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHBsdWdpbi5oaXN0b3J5LmlzRmlyc3QgfHxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGx1Z2luLmhpc3RvcnkuaXNIaXN0b3J5IHx8XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBsdWdpbi5oaXN0b3J5LmlzUmVkcmF3IHx8XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHdpbmRvdy5oaXN0b3J5LnN0YXRlID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBsdWdpbi5oaXN0b3J5LmlzRmlyc3QgPSBmYWxzZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaGlzdG9yeS5wdXNoU3RhdGUoY3JlYXRlSGlzdG9yeVN0YXRlKCksIGRvY3VtZW50LnRpdGxlKTtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgICQod2luZG93KS5vbigncG9wc3RhdGUnLCAoZXZlbnQpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBzdGF0ZSA9IGV2ZW50Lm9yaWdpbmFsRXZlbnQuc3RhdGU7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFzdGF0ZSB8fCAhc3RhdGUucHJvdmlkZXJOYW1lIHx8IHN0YXRlLnByb3ZpZGVyTmFtZSAhPT0gcHJvdmlkZXJOYW1lKSByZXR1cm47XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHBsdWdpbi5oaXN0b3J5LmlzSGlzdG9yeSA9IHRydWU7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgdXNlckV2ZW50ID0geyBwbHVnaW46IHBsdWdpbiwgc3RhdGU6IHN0YXRlLCBjYW5jZWw6IGZhbHNlIH07XHJcbiAgICAgICAgICAgICAgICAgICAgJGVsZW1lbnQudHJpZ2dlcigncG9wc3RhdGUubG9tYmlxZHQnLCB1c2VyRXZlbnQpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghdXNlckV2ZW50LmNhbmNlbCkgJGVsZW1lbnQuRGF0YVRhYmxlKCkuYWpheC5yZWxvYWQoKTtcclxuICAgICAgICAgICAgICAgICAgICBwbHVnaW4uaGlzdG9yeS5pc0hpc3RvcnkgPSBmYWxzZTtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgIC8vIFNlZTogaHR0cHM6Ly9zdGFja292ZXJmbG93LmNvbS9xdWVzdGlvbnMvNTAwNDk3OC9jaGVjay1pZi1wYWdlLWdldHMtcmVsb2FkZWQtb3ItcmVmcmVzaGVkLWluLWphdmFzY3JpcHQvNTMzMDc1ODgjNTMzMDc1ODhcclxuICAgICAgICAgICAgICAgIGNvbnN0IHBhZ2VBY2Nlc3NlZEJ5UmVsb2FkID0gd2luZG93LnBlcmZvcm1hbmNlLm5hdmlnYXRpb24/LnR5cGUgPT09IDEgfHxcclxuICAgICAgICAgICAgICAgICAgICB3aW5kb3dcclxuICAgICAgICAgICAgICAgICAgICAgICAgLnBlcmZvcm1hbmNlXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5nZXRFbnRyaWVzQnlUeXBlKCduYXZpZ2F0aW9uJylcclxuICAgICAgICAgICAgICAgICAgICAgICAgLm1hcCgobmF2KSA9PiBuYXYudHlwZSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgLmluY2x1ZGVzKCdyZWxvYWQnKTtcclxuXHJcbiAgICAgICAgICAgICAgICBkYXRhVGFibGVzT3B0aW9ucy5hamF4ID0gZnVuY3Rpb24gZGF0YVRhYmxlc09wdGlvbnNBamF4KHBhcmFtcywgY2FsbGJhY2spIHtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBpc05ld1JlcXVlc3QgPSBwYWdlQWNjZXNzZWRCeVJlbG9hZCB8fFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlb2YgaGlzdG9yeS5zdGF0ZSAhPT0gJ29iamVjdCcgfHxcclxuICAgICAgICAgICAgICAgICAgICAgICAgIWhpc3Rvcnkuc3RhdGU/LmRhdGE7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGlzTmV3UmVxdWVzdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBkYXRhID0gSlNPTi5wYXJzZShnZXRKc29uUGFyYW1ldGVycyhwYXJhbXMpKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaGlzdG9yeS5yZXBsYWNlU3RhdGUoY3JlYXRlSGlzdG9yeVN0YXRlKGRhdGEpLCBkb2N1bWVudC50aXRsZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByZXF1ZXN0RGF0YSA9ICQuZXh0ZW5kKHt9LCBoaXN0b3J5LnN0YXRlLmRhdGEpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghaXNOZXdSZXF1ZXN0KSByZXF1ZXN0RGF0YS5kcmF3ID0gKGxhdGVzdERyYXcgPz8gMCkgKyAzO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCAkd3JhcHBlciA9ICRlbGVtZW50LmNsb3Nlc3QoJy5kYXRhVGFibGVzX3dyYXBwZXInKTtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBpbnN0YW5jZSA9ICRlbGVtZW50LkRhdGFUYWJsZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICR3cmFwcGVyXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5maW5kKCcuZGF0YVRhYmxlc19maWx0ZXIgaW5wdXRbdHlwZT1cInNlYXJjaFwiXVthcmlhLWNvbnRyb2xzPVwiZGF0YVRhYmxlXCJdJylcclxuICAgICAgICAgICAgICAgICAgICAgICAgLnZhbChyZXF1ZXN0RGF0YS5zZWFyY2g/LnZhbHVlID8/ICcnKTtcclxuICAgICAgICAgICAgICAgICAgICAkd3JhcHBlclxyXG4gICAgICAgICAgICAgICAgICAgICAgICAuZmluZCgnLmRhdGFUYWJsZXNfbGVuZ3RoIHNlbGVjdFthcmlhLWNvbnRyb2xzPVwiZGF0YVRhYmxlXCJdJylcclxuICAgICAgICAgICAgICAgICAgICAgICAgLnZhbChyZXF1ZXN0RGF0YS5sZW5ndGgpO1xyXG4gICAgICAgICAgICAgICAgICAgIGluc3RhbmNlLm9yZGVyKGhpc3Rvcnkuc3RhdGUub3JkZXIpO1xyXG4gICAgICAgICAgICAgICAgICAgIGluc3RhbmNlLnNlYXJjaChoaXN0b3J5LnN0YXRlPy5kYXRhPy5zZWFyY2g/LnZhbHVlID8/ICcnKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgdXNlckV2ZW50ID0geyBwbHVnaW46IHBsdWdpbiwgcmVxdWVzdERhdGE6IHJlcXVlc3REYXRhLCBpc0hpc3Rvcnk6IHBsdWdpbi5oaXN0b3J5LmlzSGlzdG9yeSB9O1xyXG4gICAgICAgICAgICAgICAgICAgICRlbGVtZW50LnRyaWdnZXIoJ3ByZVhoci5sb21iaXFkdCcsIHVzZXJFdmVudCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1ldGhvZDogJ0dFVCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHVybDogcGx1Z2luLnNldHRpbmdzLnJvd3NBcGlVcmwsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IHBsdWdpbi5idWlsZFF1ZXJ5U3RyaW5nUGFyYW1ldGVycyh7IHJlcXVlc3RKc29uOiBKU09OLnN0cmluZ2lmeSh1c2VyRXZlbnQucmVxdWVzdERhdGEpIH0pLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBsdWdpbi5zZXR0aW5ncy5jYWxsYmFja3MuYWpheERhdGFMb2FkZWRDYWxsYmFjayhyZXNwb25zZSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbGF0ZXN0RHJhdyA9IHJlc3BvbnNlLmRyYXc7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkd3JhcHBlci5hdHRyKCdkYXRhLWRyYXcnLCBsYXRlc3REcmF3KTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjYWxsYmFjayhyZXNwb25zZSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgcGFnZSA9IGhpc3Rvcnkuc3RhdGUuZGF0YS5zdGFydCAvIGhpc3Rvcnkuc3RhdGUuZGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBwbHVnaW4uaGlzdG9yeS5pc1JlZHJhdyA9IHRydWU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoaW5zdGFuY2UucGFnZSgpICE9PSBwYWdlKSBpbnN0YW5jZS5wYWdlKHBhZ2UpLmRyYXcoJ3BhZ2UnKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChpbnN0YW5jZS5wYWdlLmxlbigpICE9PSBoaXN0b3J5LnN0YXRlLmRhdGEubGVuZ3RoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaW5zdGFuY2UucGFnZS5sZW4oaGlzdG9yeS5zdGF0ZS5kYXRhLmxlbmd0aCkuZHJhdygncGFnZScpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcGx1Z2luLmhpc3RvcnkuaXNSZWRyYXcgPSBmYWxzZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH07XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGZ1bmN0aW9uIGV4cG9ydEFjdGlvbihleHBvcnRBbGwpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybiBmdW5jdGlvbiBnZXRFeHBvcnRzKCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5ocmVmID0gVVJJKHBsdWdpbi5zZXR0aW5ncy5leHBvcnQuYXBpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAuc2VhcmNoKHsgcmVxdWVzdEpzb246IHN0YXRlSnNvbiwgZXhwb3J0QWxsOiBleHBvcnRBbGwgfSk7XHJcbiAgICAgICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGZ1bmN0aW9uIGdldEV4cG9ydEJ1dHRvbnMoKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gW1xyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGV4dDogcGx1Z2luLnNldHRpbmdzLmV4cG9ydC50ZXh0QWxsLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhY3Rpb246IGV4cG9ydEFjdGlvbih0cnVlKSxcclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGV4dDogcGx1Z2luLnNldHRpbmdzLmV4cG9ydC50ZXh0VmlzaWJsZSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgYWN0aW9uOiBleHBvcnRBY3Rpb24oZmFsc2UpLFxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBdO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGlmIChkYXRhVGFibGVzT3B0aW9ucy5idXR0b25zID09PSB1c2VEZWZhdWx0QnV0dG9ucykge1xyXG4gICAgICAgICAgICAgICAgZGF0YVRhYmxlc09wdGlvbnMuYnV0dG9ucyA9IGdldEV4cG9ydEJ1dHRvbnMoKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBlbHNlIGlmIChkYXRhVGFibGVzT3B0aW9ucy5idXR0b25zICYmIGRhdGFUYWJsZXNPcHRpb25zLmJ1dHRvbnMuZm9yRWFjaCkge1xyXG4gICAgICAgICAgICAgICAgZGF0YVRhYmxlc09wdGlvbnMuYnV0dG9ucy5mb3JFYWNoKChidXR0b24pID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoYnV0dG9uLmJ1dHRvbnMgPT09IHVzZURlZmF1bHRCdXR0b25zKSBidXR0b24uYnV0dG9ucyA9IGdldEV4cG9ydEJ1dHRvbnMoKTtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBpZiAocGx1Z2luLnNldHRpbmdzLmVycm9yc1NlbGVjdG9yKSB7XHJcbiAgICAgICAgICAgICAgICAkLmZuLmRhdGFUYWJsZS5leHQuZXJyTW9kZSA9ICdub25lJztcclxuICAgICAgICAgICAgICAgICQocGx1Z2luLmVsZW1lbnQpLm9uKCdlcnJvci5kdCcsIChlLCBzZXR0aW5ncywgdGVjaE5vdGUsIG1lc3NhZ2UpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAkKHBsdWdpbi5zZXR0aW5ncy5lcnJvcnNTZWxlY3RvcikudGV4dChtZXNzYWdlKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcGx1Z2luLmRhdGFUYWJsZUVsZW1lbnQgPSAkKHBsdWdpbi5lbGVtZW50KS5kYXRhVGFibGUoZGF0YVRhYmxlc09wdGlvbnMpO1xyXG4gICAgICAgICAgICBwbHVnaW4uZGF0YVRhYmxlQXBpID0gcGx1Z2luLmRhdGFUYWJsZUVsZW1lbnQuYXBpKCk7XHJcblxyXG4gICAgICAgICAgICAvLyBSZWdpc3RlciB0b2dnbGUgYnV0dG9uIGNsaWNrIGxpc3RlbmVycyBpZiBjaGlsZCByb3dzIGFyZSBlbmFibGVkLlxyXG4gICAgICAgICAgICBpZiAocGx1Z2luLnNldHRpbmdzLmNoaWxkUm93T3B0aW9ucy5jaGlsZFJvd3NFbmFibGVkKSB7XHJcbiAgICAgICAgICAgICAgICBwbHVnaW4uZGF0YVRhYmxlRWxlbWVudC5vbihcclxuICAgICAgICAgICAgICAgICAgICAnY2xpY2snLFxyXG4gICAgICAgICAgICAgICAgICAgICcuJyArIHBsdWdpbi5zZXR0aW5ncy5jaGlsZFJvd09wdGlvbnMudG9nZ2xlQ2hpbGRSb3dCdXR0b25DbGFzc05hbWUsXHJcbiAgICAgICAgICAgICAgICAgICAgZnVuY3Rpb24gZGF0YVRhYmxlRWxlbWVudE9uQ2xpY2soKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IHBhcmVudFJvd0VsZW1lbnQgPSAkKHRoaXMpLmNsb3Nlc3QoJ3RyJyk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocGx1Z2luLnNldHRpbmdzLmNoaWxkUm93T3B0aW9ucy5hc3luY0xvYWRpbmcpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IGNvbnRlbnRJdGVtSWQgPSBwYXJlbnRSb3dFbGVtZW50LmF0dHIoJ2RhdGEtY29udGVudGl0ZW1pZCcpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ0dFVCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBwbHVnaW4uc2V0dGluZ3MuY2hpbGRSb3dPcHRpb25zLmFwaVVybCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnRlbnRJdGVtSWQ6IGNvbnRlbnRJdGVtSWQsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFQcm92aWRlcjogcGx1Z2luLnNldHRpbmdzLmRhdGFQcm92aWRlcixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgb3JpZ2luYWxVcmw6IHdpbmRvdy5sb2NhdGlvbi5ocmVmLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKCFkYXRhLmVycm9yKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwbHVnaW4udG9nZ2xlQ2hpbGRSb3cocGFyZW50Um93RWxlbWVudCwgZGF0YS5jb250ZW50KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGFsZXJ0KGRhdGEuZXJyb3IpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgY2hpbGRSb3dDb250ZW50ID0gJCgnW2RhdGEtcGFyZW50PVwiJyArIHBhcmVudFJvd0VsZW1lbnQuYXR0cignaWQnKSArICdcIl0nKS5odG1sKCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcGx1Z2luLnRvZ2dsZUNoaWxkUm93KHBhcmVudFJvd0VsZW1lbnQsIGNoaWxkUm93Q29udGVudCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgLy8gRmV0Y2ggaXRlbXMgaWYgcHJvZ3Jlc3NpdmUgbG9hZGluZyBpcyBlbmFibGVkLlxyXG4gICAgICAgICAgICBpZiAoIXBsdWdpbi5zZXR0aW5ncy5zZXJ2ZXJTaWRlUGFnaW5nRW5hYmxlZCAmJlxyXG4gICAgICAgICAgICAgICAgcGx1Z2luLnNldHRpbmdzLnByb2dyZXNzaXZlTG9hZGluZ09wdGlvbnMucHJvZ3Jlc3NpdmVMb2FkaW5nRW5hYmxlZCkge1xyXG4gICAgICAgICAgICAgICAgcGx1Z2luLmZldGNoUm93c1Byb2dyZXNzaXZlbHkoKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcblxyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICogUmVtb3ZlcyB1bm5lY2Vzc2FyeSBEYXRhVGFibGVzIGFqYXggcGFyYW1ldGVycyBhbmQgdXBkYXRlcyBwcm9wZXJ0eSBuYW1lcyBhbmQgdmFsdWVzIHRvIG1hdGNoIHNlcnZlciBkYXRhXHJcbiAgICAgICAgKiBtb2RlbC5cclxuICAgICAgICAqIEBwYXJhbSB7b2JqZWN0fSBwYXJhbWV0ZXJzIFBhcmFtZXRlcnMgZ2VuZXJhdGVkIGJ5IHRoZSBEYXRhVGFibGVzIHBsdWdpbiB0byBiZSBzZW50IHRvIHRoZSBzZXJ2ZXIuXHJcbiAgICAgICAgKiBAcmV0dXJucyB7b2JqZWN0fSBDbGVhbmVkLXVwIHF1ZXJ5IHN0cmluZyBwYXJhbWV0ZXJzLlxyXG4gICAgICAgICovXHJcbiAgICAgICAgY2xlYW5VcERhdGFUYWJsZXNBamF4UGFyYW1ldGVyczogZnVuY3Rpb24gKHBhcmFtZXRlcnMpIHtcclxuICAgICAgICAgICAgLy8gUmVwbGFjaW5nIGNvbHVtbiBpbmRleCB0byBjb2x1bW4gbmFtZS5cclxuICAgICAgICAgICAgLy8gQWxzbyByZW5hbWUgcHJvcGVydGllcyBhbmQgdmFsdWVzIHRvIG1hdGNoIGJhY2stZW5kIGRhdGEgbW9kZWwuXHJcbiAgICAgICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgcGFyYW1ldGVycy5vcmRlci5sZW5ndGg7IGkrKykge1xyXG4gICAgICAgICAgICAgICAgY29uc3Qgb3JkZXJEYXRhID0gcGFyYW1ldGVycy5vcmRlcltpXTtcclxuICAgICAgICAgICAgICAgIGNvbnN0IGNvbHVtbkluZGV4ID0gb3JkZXJEYXRhLmNvbHVtbjtcclxuICAgICAgICAgICAgICAgIG9yZGVyRGF0YS5jb2x1bW4gPSBwYXJhbWV0ZXJzLmNvbHVtbnNbY29sdW1uSW5kZXhdLm5hbWU7XHJcbiAgICAgICAgICAgICAgICBvcmRlckRhdGEuZGlyZWN0aW9uID0gb3JkZXJEYXRhLmRpciA9PT0gJ2FzYycgPyAnYXNjZW5kaW5nJyA6ICdkZXNjZW5kaW5nJztcclxuICAgICAgICAgICAgICAgIGRlbGV0ZSBvcmRlckRhdGEuZGlyO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAvLyBTZW5kIG9ubHkgZmlsdGVyZWQgY29sdW1uIGRhdGEuXHJcbiAgICAgICAgICAgIGNvbnN0IGNvbHVtbkZpbHRlcnMgPSBbXTtcclxuICAgICAgICAgICAgZm9yIChsZXQgaiA9IDA7IGogPCBwYXJhbWV0ZXJzLmNvbHVtbnMubGVuZ3RoOyBqKyspIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IGNvbHVtbiA9IHBhcmFtZXRlcnMuY29sdW1uc1tqXTtcclxuICAgICAgICAgICAgICAgIGlmIChjb2x1bW4uc2VhcmNoLnZhbHVlKSBjb2x1bW5GaWx0ZXJzLnB1c2goY29sdW1uKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcGFyYW1ldGVycy5jb2x1bW5GaWx0ZXJzID0gY29sdW1uRmlsdGVycztcclxuICAgICAgICAgICAgZGVsZXRlIHBhcmFtZXRlcnMuY29sdW1ucztcclxuXHJcbiAgICAgICAgICAgIC8vIFJlbW92ZSBnbG9iYWwgc2VhcmNoIHBhcmFtZXRlcnMgaWYgdGhlcmUgaXMgbm8gc2VhcmNoIHZhbHVlIGdpdmVuLlxyXG4gICAgICAgICAgICBpZiAoIXBhcmFtZXRlcnMuc2VhcmNoLnZhbHVlKSBkZWxldGUgcGFyYW1ldGVycy5zZWFyY2g7XHJcbiAgICAgICAgICAgIHJldHVybiBwYXJhbWV0ZXJzO1xyXG4gICAgICAgIH0sXHJcblxyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICogU2hvd3Mgb3IgaGlkZXMgY2hpbGQgcm93IGZpbGxlZCB3aXRoIHRoZSBnaXZlbiBjb250ZW50LlxyXG4gICAgICAgICogQHBhcmFtIHtqUXVlcnl9IHBhcmVudFJvd0VsZW1lbnQgUGFyZW50IHJvdyBlbGVtZW50IHdoZXJlIHRoZSBjaGlsZCByb3cgd2lsbCBiZSBkaXNwbGF5ZWQuXHJcbiAgICAgICAgKiBAcGFyYW0ge29iamVjdH0gY2hpbGRSb3dDb250ZW50IENvbnRlbnQgb2YgdGhlIGNoaWxkIHJvdy4gQSA8dHI+IHdyYXBwZXIgd2lsbCBiZSBhZGRlZCBhdXRvbWF0aWNhbGx5LlxyXG4gICAgICAgICovXHJcbiAgICAgICAgdG9nZ2xlQ2hpbGRSb3c6IGZ1bmN0aW9uIChwYXJlbnRSb3dFbGVtZW50LCBjaGlsZFJvd0NvbnRlbnQpIHtcclxuICAgICAgICAgICAgY29uc3QgcGx1Z2luID0gdGhpcztcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGRhdGFUYWJsZVJvdyA9IHBsdWdpbi5kYXRhVGFibGVBcGkucm93KHBhcmVudFJvd0VsZW1lbnQpO1xyXG5cclxuICAgICAgICAgICAgaWYgKGRhdGFUYWJsZVJvdy5jaGlsZC5pc1Nob3duKCkpIHtcclxuICAgICAgICAgICAgICAgIGRhdGFUYWJsZVJvdy5jaGlsZC5oaWRlKCk7XHJcblxyXG4gICAgICAgICAgICAgICAgcGFyZW50Um93RWxlbWVudC5yZW1vdmVDbGFzcyhwbHVnaW4uc2V0dGluZ3MuY2hpbGRSb3dPcHRpb25zLmNoaWxkUm93VmlzaWJsZUNsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgZWxzZSB7XHJcbiAgICAgICAgICAgICAgICBkYXRhVGFibGVSb3cuY2hpbGQoY2hpbGRSb3dDb250ZW50LCBwbHVnaW4uc2V0dGluZ3MuY2hpbGRSb3dPcHRpb25zLmNoaWxkUm93Q2xhc3NOYW1lKS5zaG93KCk7XHJcblxyXG4gICAgICAgICAgICAgICAgcGFyZW50Um93RWxlbWVudC5hZGRDbGFzcyhwbHVnaW4uc2V0dGluZ3MuY2hpbGRSb3dPcHRpb25zLmNoaWxkUm93VmlzaWJsZUNsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG5cclxuICAgICAgICAvKipcclxuICAgICAgICAqIEZldGNoZXMgdGhlIHJvd3MgZnJvbSB0aGUgQVBJIHVzaW5nIHByb2dyZXNzaXZlIGxvYWRpbmcuXHJcbiAgICAgICAgKi9cclxuICAgICAgICBmZXRjaFJvd3NQcm9ncmVzc2l2ZWx5OiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHBsdWdpbiA9IHRoaXM7XHJcblxyXG4gICAgICAgICAgICBpZiAoIXBsdWdpbi5zZXR0aW5ncy5wcm9ncmVzc2l2ZUxvYWRpbmdPcHRpb25zLnByb2dyZXNzaXZlTG9hZGluZ0VuYWJsZWQpIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIHBsdWdpbi5kYXRhVGFibGVBcGkucHJvY2Vzc2luZyh0cnVlKTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IG9wdGlvbnMgPSB7XHJcbiAgICAgICAgICAgICAgICBxdWVyeUlkOiBwbHVnaW4uc2V0dGluZ3MucXVlcnlJZCxcclxuICAgICAgICAgICAgICAgIGRhdGFQcm92aWRlcjogcGx1Z2luLnNldHRpbmdzLmRhdGFQcm92aWRlcixcclxuICAgICAgICAgICAgICAgIGFwaVVybDogcGx1Z2luLnNldHRpbmdzLnJvd3NBcGlVcmwsXHJcbiAgICAgICAgICAgICAgICBpdGVtQ2FsbGJhY2s6IGZ1bmN0aW9uIChpZCwgZGF0YSwgcmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocGx1Z2luLnNldHRpbmdzLnByb2dyZXNzaXZlTG9hZGluZ09wdGlvbnMuaXRlbUNhbGxiYWNrKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBsdWdpbi5zZXR0aW5ncy5wcm9ncmVzc2l2ZUxvYWRpbmdPcHRpb25zLml0ZW1DYWxsYmFjayhpZCwgZGF0YSwgcmVzcG9uc2UpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgcGx1Z2luLmRhdGFUYWJsZUFwaS5yb3cuYWRkKGRhdGEpLmRyYXcoKTtcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBmaW5pc2hlZENhbGxiYWNrOiBmdW5jdGlvbiAoc3VjY2VzcywgdG90YWwpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocGx1Z2luLnNldHRpbmdzLnByb2dyZXNzaXZlTG9hZGluZ09wdGlvbnMuZmluaXNoZWRDYWxsYmFjaykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBwbHVnaW4uc2V0dGluZ3MucHJvZ3Jlc3NpdmVMb2FkaW5nT3B0aW9ucy5maW5pc2hlZENhbGxiYWNrKHN1Y2Nlc3MsIHRvdGFsKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHBsdWdpbi5kYXRhVGFibGVBcGkucHJvY2Vzc2luZyhmYWxzZSk7XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB9O1xyXG5cclxuICAgICAgICAgICAgcGx1Z2luLnByb2dyZXNzaXZlTG9hZCgkLmV4dGVuZCh7fSwgcGx1Z2luLnNldHRpbmdzLnByb2dyZXNzaXZlTG9hZGluZ09wdGlvbnMsIG9wdGlvbnMpKTtcclxuICAgICAgICB9LFxyXG5cclxuICAgICAgICAvKipcclxuICAgICAgICAqIEJ1aWxkcyBxdWVyeSBzdHJpbmcgcGFyYW1ldGVycyB0aGF0IGluY2x1ZGVzIHRoZSBnaXZlbiBwYXJhbWV0ZXJzIGFuZCB0aGUgY3VycmVudCBVUkwncyBxdWVyeSBzdHJpbmdcclxuICAgICAgICAqIHBhcmFtZXRlcnMuXHJcbiAgICAgICAgKiBUaGUgb3JpZ2luYWwgcXVlcnkgc3RyaW5nIHBhcmFtZXRlcnMgYXJlIHRyYWRpdGlvbmFsbHkgZW5jb2RlZCB0byBwcmVzZXJ2ZSB0aGVpciBxdWVyeSBzdHJpbmcga2V5cyxcclxuICAgICAgICAqIHdoaWxlIHRoZSBvbmVzIHVzZWQgYnkgRGF0YVRhYmxlcyBhcmVuJ3QuXHJcbiAgICAgICAgKiBAcGFyYW0ge29iamVjdH0gZGF0YSBEYXRhIHRoYXQgbmVlZHMgdG8gYmUgbWVyZ2VkIHdpdGggdGhlIGN1cnJlbnQgVVJMIHF1ZXJ5IHN0cmluZyBwYXJhbWV0ZXJzLlxyXG4gICAgICAgICogQHJldHVybnMge29iamVjdH0gTWVyZ2VkIHF1ZXJ5IHN0cmluZyBwYXJhbWV0ZXJzLlxyXG4gICAgICAgICovXHJcbiAgICAgICAgYnVpbGRRdWVyeVN0cmluZ1BhcmFtZXRlcnM6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIC8vIFRoaXMgaXMgbmVjZXNzYXJ5IHRvIHByZXNlcnZlIHRoZSBvcmlnaW5hbCBzdHJ1Y3R1cmUgb2YgdGhlIGluaXRpYWwgcXVlcnkgc3RyaW5nOlxyXG4gICAgICAgICAgICAvLyBUcmFkaXRpb25hbCBlbmNvZGluZyBlbnN1cmVzIHRoYXQgaWYgYSBrZXkgaGFzIG11bHRpcGxlIHZhbHVlcyAoZS5nLiBcIj9uYW1lPXZhbHVlMSZuYW1lPXZhbHVlMlwiKSxcclxuICAgICAgICAgICAgLy8gdGhlbiB0aGUga2V5IHdvbid0IGJlIGNoYW5nZWQgdG8gXCJuYW1lW11cIi5cclxuICAgICAgICAgICAgY29uc3Qgb3JpZ2luYWxRdWVyeVN0cmluZ0VuY29kZWQgPSAkLnBhcmFtKHRoaXMub3JpZ2luYWxRdWVyeVN0cmluZ1BhcmFtZXRlcnMsIHRydWUpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIChvcmlnaW5hbFF1ZXJ5U3RyaW5nRW5jb2RlZCA/IChvcmlnaW5hbFF1ZXJ5U3RyaW5nRW5jb2RlZCArICcmJykgOiAnJykgKyAkLnBhcmFtKGRhdGEpO1xyXG4gICAgICAgIH0sXHJcblxyXG4gICAgICAgIC8qKlxyXG4gICAgICAgICogTG93LWxldmVsIGZ1bmN0aW9uYWxpdHkgZm9yIGxvYWRpbmcgcm93cyBmcm9tIHRoZSBBUEkuIFRoZSByZXN1bHQgaXMgYWNjZXNzaWJsZSB1c2luZyB0aGUgY2FsbGJhY2suXHJcbiAgICAgICAgKiBAcGFyYW0ge251bWJlcn0gc2tpcCBOdW1iZXIgb2YgaXRlbXMgdG8gYmUgc2tpcHBlZCBieSB0aGUgQVBJLlxyXG4gICAgICAgICogQHBhcmFtIHtPYmplY3R9IG9wdGlvbnMgT3B0aW9ucyByZXF1aXJlZCBmb3IgdGhlIEFQSSBjYWxsIChlLmcuIEFQSSBVUkwsIGRhdGEgcHJvdmlkZXIpLlxyXG4gICAgICAgICogQHBhcmFtIHtjYWxsYmFja30gY2FsbGJhY2sgQ2FsbGJhY2sgZm9yIHJldHVybmluZyByb3dzLlxyXG4gICAgICAgICovXHJcbiAgICAgICAgbG9hZFJvd3M6IGZ1bmN0aW9uIChza2lwLCBvcHRpb25zLCBjYWxsYmFjaykge1xyXG4gICAgICAgICAgICBjb25zdCBwbHVnaW4gPSB0aGlzO1xyXG5cclxuICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgIHR5cGU6ICdHRVQnLFxyXG4gICAgICAgICAgICAgICAgdXJsOiBvcHRpb25zLmFwaVVybCxcclxuICAgICAgICAgICAgICAgIGRhdGE6IHBsdWdpbi5idWlsZFF1ZXJ5U3RyaW5nUGFyYW1ldGVycyh7XHJcbiAgICAgICAgICAgICAgICAgICAgcXVlcnlJZDogb3B0aW9ucy5xdWVyeUlkLFxyXG4gICAgICAgICAgICAgICAgICAgIHN0YXJ0OiBza2lwLFxyXG4gICAgICAgICAgICAgICAgICAgIGxlbmd0aDogb3B0aW9ucy5iYXRjaFNpemUsXHJcbiAgICAgICAgICAgICAgICAgICAgZGF0YVByb3ZpZGVyOiBvcHRpb25zLmRhdGFQcm92aWRlcixcclxuICAgICAgICAgICAgICAgICAgICBvcmlnaW5hbFVybDogd2luZG93LmxvY2F0aW9uLmhyZWYsXHJcbiAgICAgICAgICAgICAgICB9KSxcclxuICAgICAgICAgICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjYWxsYmFjaykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjYWxsYmFjayghcmVzcG9uc2UuZXJyb3IsIHJlc3BvbnNlKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgZmFpbDogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjYWxsYmFjaykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjYWxsYmFjayhmYWxzZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfSxcclxuXHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgKiBBZGp1c3RzIGRhdGF0YWJsZSBjb2x1bW5zLlxyXG4gICAgICAgICovXHJcbiAgICAgICAgYWRqdXN0Q29sdW1uczogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICBjb25zdCBwbHVnaW4gPSB0aGlzO1xyXG5cclxuICAgICAgICAgICAgLy8gVGhpcyBpcyBhIHdvcmthcm91bmQgdG8gcHJvcGVybHkgYWRqdXN0IGNvbHVtbiB3aWR0aHMuXHJcbiAgICAgICAgICAgIHNldFRpbWVvdXQoKCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgcGx1Z2luLmRhdGFUYWJsZUFwaS5jb2x1bW5zLmFkanVzdCgpO1xyXG4gICAgICAgICAgICB9LCAxMCk7XHJcbiAgICAgICAgfSxcclxuXHJcbiAgICAgICAgLyoqXHJcbiAgICAgICAgKiBMb3ctbGV2ZWwgZnVuY3Rpb25hbGl0eSBvZiBwcm9ncmVzc2l2ZSBsb2FkaW5nLiBJdCB3aWxsIGZldGNoIGNvbnRlbnQgc2hhcGVzIGZyb20gdGhlIGdpdmVuIEFQSS5cclxuICAgICAgICAqIFRoZSBzaGFwZXMgd2lsbCBiZSBwcm9jZXNzZWQgdXNpbmcgY2FsbGJhY2tzLlxyXG4gICAgICAgICogQHBhcmFtIHtPYmplY3R9IG9wdGlvbnMgUHJvZ3Jlc3NpdmUgbG9hZGluZyBvcHRpb25zIGluY2x1ZGluZyBBUEkgVVJMIGFuZCBjYWxsYmFja3MuXHJcbiAgICAgICAgKi9cclxuICAgICAgICBwcm9ncmVzc2l2ZUxvYWQ6IGZ1bmN0aW9uIChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHBsdWdpbiA9IHRoaXM7XHJcbiAgICAgICAgICAgIGxldCB0b3RhbCA9IDA7XHJcbiAgICAgICAgICAgIGxldCBza2lwID0gb3B0aW9ucy5za2lwO1xyXG5cclxuICAgICAgICAgICAgY29uc3QgY2FsbGJhY2sgPSBmdW5jdGlvbiAoc3VjY2VzcywgcmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgIGlmIChzdWNjZXNzICYmIHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgY291bnQgPSByZXNwb25zZS5kYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgICAgICAgICB0b3RhbCArPSBjb3VudDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKG9wdGlvbnMuYmF0Y2hDYWxsYmFjaykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcHRpb25zLmJhdGNoQ2FsbGJhY2socmVzcG9uc2UsIHRvdGFsKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjb3VudCA+IDAgJiYgb3B0aW9ucy5pdGVtQ2FsbGJhY2spIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgJC5lYWNoKHJlc3BvbnNlLmRhdGEsIChpbmRleCwgdmFsdWUpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuaXRlbUNhbGxiYWNrKGluZGV4LCB2YWx1ZSwgcmVzcG9uc2UpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjb3VudCA+IDAgJiYgY291bnQgPj0gb3B0aW9ucy5iYXRjaFNpemUpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgc2tpcCArPSBjb3VudDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBsdWdpbi5sb2FkUm93cyhza2lwLCBvcHRpb25zLCBjYWxsYmFjayk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIGVsc2UgaWYgKG9wdGlvbnMuZmluaXNoZWRDYWxsYmFjaykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcHRpb25zLmZpbmlzaGVkQ2FsbGJhY2sodHJ1ZSwgdG90YWwpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhbGVydChyZXNwb25zZS5lcnJvcik7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAob3B0aW9ucy5maW5pc2hlZENhbGxiYWNrKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuZmluaXNoZWRDYWxsYmFjayhmYWxzZSwgdG90YWwpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfTtcclxuXHJcbiAgICAgICAgICAgIHBsdWdpbi5sb2FkUm93cyhza2lwLCBvcHRpb25zLCBjYWxsYmFjayk7XHJcbiAgICAgICAgfSxcclxuICAgIH0pO1xyXG5cclxuICAgICQuZm5bcGx1Z2luTmFtZV0gPSBmdW5jdGlvbiBwbHVnaW5OYW1lRnVuY3Rpb24ob3B0aW9ucykge1xyXG4gICAgICAgIC8vIFJldHVybiBudWxsIGlmIHRoZSBlbGVtZW50IHF1ZXJ5IGlzIGludmFsaWQuXHJcbiAgICAgICAgaWYgKCF0aGlzIHx8IHRoaXMubGVuZ3RoID09PSAwKSByZXR1cm4gbnVsbDtcclxuXHJcbiAgICAgICAgLy8gXCJtYXBcIiBtYWtlcyBpdCBwb3NzaWJsZSB0byByZXR1cm4gdGhlIGFscmVhZHkgZXhpc3Rpbmcgb3IgY3VycmVudGx5IGluaXRpYWxpemVkIHBsdWdpbiBpbnN0YW5jZXMuXHJcbiAgICAgICAgcmV0dXJuIHRoaXMubWFwKGZ1bmN0aW9uIHBsdWdpbk1hcEZ1bmN0aW9uKCkge1xyXG4gICAgICAgICAgICAvLyBJZiBcIm9wdGlvbnNcIiBpcyBkZWZpbmVkLCBidXQgdGhlIHBsdWdpbiBpcyBub3QgaW5zdGFudGlhdGVkIG9uIHRoaXMgZWxlbWVudCAuLi5cclxuICAgICAgICAgICAgaWYgKG9wdGlvbnMgJiYgISQuZGF0YSh0aGlzLCAncGx1Z2luXycgKyBwbHVnaW5OYW1lKSkge1xyXG4gICAgICAgICAgICAgICAgLy8gLi4uIHRoZW4gY3JlYXRlIGEgcGx1Z2luIGluc3RhbmNlIC4uLlxyXG4gICAgICAgICAgICAgICAgJC5kYXRhKHRoaXMsICdwbHVnaW5fJyArIHBsdWdpbk5hbWUsIG5ldyBQbHVnaW4oJCh0aGlzKSwgb3B0aW9ucykpO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAvLyAuLi4gYW5kIHRoZW4gcmV0dXJuIHRoZSBwbHVnaW4gaW5zdGFuY2UsIHdoaWNoIG1pZ2h0IGJlIG51bGxcclxuICAgICAgICAgICAgLy8gaWYgdGhlIHBsdWdpbiBpcyBub3QgaW5zdGFudGlhdGVkIG9uIHRoaXMgZWxlbWVudCBhbmQgJ29wdGlvbnMnIGlzIHVuZGVmaW5lZC5cclxuICAgICAgICAgICAgcmV0dXJuICQuZGF0YSh0aGlzLCAncGx1Z2luXycgKyBwbHVnaW5OYW1lKTtcclxuICAgICAgICB9KTtcclxuICAgIH07XHJcbn0pKGpRdWVyeSwgd2luZG93LCBkb2N1bWVudCwgd2luZG93Lmhpc3RvcnkpO1xyXG4iXSwibWFwcGluZ3MiOiI7Ozs7QUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUVBLENBQUMsU0FBU0EsZ0JBQVQsQ0FBMEJDLENBQTFCLEVBQTZCQyxNQUE3QixFQUFxQ0MsUUFBckMsRUFBK0NDLE9BQS9DLEVBQXdEO0VBQ3JELElBQU1DLFVBQVUsR0FBRyxtQkFBbkI7RUFDQSxJQUFNQyxpQkFBaUIsR0FBRyxtQkFBMUI7RUFFQSxJQUFNQyxRQUFRLEdBQUc7SUFDYkMsaUJBQWlCLEVBQUU7TUFDZkMsU0FBUyxFQUFFLElBREk7TUFFZkMsTUFBTSxFQUFFLElBRk87TUFHZkMsVUFBVSxFQUFFLElBSEc7TUFJZkMsSUFBSSxFQUFFLElBSlM7TUFLZkMsWUFBWSxFQUFFLElBTEM7TUFNZkMsT0FBTyxFQUFFLElBTk07TUFPZkMsR0FBRyxFQUFFLDZDQUNELDJGQURDLEdBRUQsMENBRkMsR0FHRCwwQ0FWVztNQVdmQyxPQUFP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