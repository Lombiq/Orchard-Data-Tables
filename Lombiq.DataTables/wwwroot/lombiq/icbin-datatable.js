"use strict";

function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); if (enumerableOnly) { symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; }); } keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i] != null ? arguments[i] : {}; if (i % 2) { ownKeys(Object(source), true).forEach(function (key) { _defineProperty(target, key, source[key]); }); } else if (Object.getOwnPropertyDescriptors) { Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)); } else { ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _typeof(obj) { "@babel/helpers - typeof"; if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

function _slicedToArray(arr, i) { return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _unsupportedIterableToArray(arr, i) || _nonIterableRest(); }

function _nonIterableRest() { throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _iterableToArrayLimit(arr, i) { var _i = arr == null ? null : typeof Symbol !== "undefined" && arr[Symbol.iterator] || arr["@@iterator"]; if (_i == null) return; var _arr = []; var _n = true; var _d = false; var _s, _e; try { for (_i = _i.call(arr); !(_n = (_s = _i.next()).done); _n = true) { _arr.push(_s.value); if (i && _arr.length === i) break; } } catch (err) { _d = true; _e = err; } finally { try { if (!_n && _i["return"] != null) _i["return"](); } finally { if (_d) throw _e; } } return _arr; }

function _arrayWithHoles(arr) { if (Array.isArray(arr)) return arr; }

function _toConsumableArray(arr) { return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread(); }

function _nonIterableSpread() { throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }

function _iterableToArray(iter) { if (typeof Symbol !== "undefined" && iter[Symbol.iterator] != null || iter["@@iterator"] != null) return Array.from(iter); }

function _arrayWithoutHoles(arr) { if (Array.isArray(arr)) return _arrayLikeToArray(arr); }

function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }

/* globals Vue */
window.icbinDataTable = {}; // events emitted:
// - special(cell): If the cell has a property called "special" (value is not null or
//   undefined) this event is called inside the sortedData. This gives an opportunity for the
//   client code to update the cell data (e.g. by setting the "component" and "hiddenInput"
//   properties) with domain-specific behavior without having to edit this component.
// - update(data): Sends the new desired value of the "data" property to the parent. Alternatively
//   v-model can also be used.
// - column(columns): Sends an updated columns array to the parent so it can replace the columns parameter with it.
// - component(rowIndex, columnName, userData): Passed on from the child component. If it's a column header component
//   then columnName is -1.
//
// events received:
// - delete(promptText): cell components may emit this event to signal a request to delete the row
//   from the table's data. Optionally a non-empty String may be passed as event argument. If that
//   happens, a prompt will be displayed with the given text to confirm with the user that they
//   really want to remove the row.
// - update(data): Same as above. The component may have a "data" property for this purpose.
// - component(userData): A child component may raise this to be bubbled up to the parent component.

window.icbinDataTable.table = {
  name: 'icbin-datatable',
  model: {
    prop: 'data',
    event: 'update'
  },
  props: {
    data: {
      // [
      //   {
      //     $rowIndex: Number,
      //     columnName: {
      //       // These can come from the server:
      //       text: String,
      //       html: String?,
      //       badge: String?
      //       sort: Any?,
      //       href: String?,
      //       multipleLinks: { link: String, text: String }? or [ { link: String, text: String } ]?,
      //       special: Any?,
      //       hiddenInput: { name: String, value: String }? or [ { name: String, value: String } ]?
      //       // These can be set in JS code (e.g. with the "special" event):
      //       component: { name: String?, value: Object }?
      //       rowClasses: String?
      //     }
      //   }
      // ]
      // note: The name and value in the hiddenInput properties may contain the {{ index }} expression which is
      //       substituted with a zero-based index when generating the hiddenInputs computed property.
      type: Array,
      required: true
    },
    columns: {
      type: Array,
      required: true
    },
    text: {
      // Expected properties: lengthPicker, displayCount, previous, next, all.
      type: Object,
      required: true
    },
    defaultSort: {
      // { name: "columnName", ascending: true }
      "default": null
    },
    defaultLength: {
      type: Number,
      "default": 10
    },
    lengths: {
      type: Array,
      "default": function _default() {
        return [10, 25, 50, 100];
      }
    },
    paging: {
      "default": true
    },
    filter: {
      "default": function _default() {
        return function (collection) {
          return collection;
        };
      }
    }
  },
  data: function data() {
    return {
      pageIndex: 0,
      length: 10,
      sort: {
        name: null,
        ascending: true
      }
    };
  },
  computed: {
    total: function total(self) {
      return self.data.length;
    },
    lengthPickerBefore: function lengthPickerBefore(self) {
      return self.text.lengthPicker.split('{{ count }}')[0];
    },
    lengthPickerAfter: function lengthPickerAfter(self) {
      var parts = self.text.lengthPicker.split('{{ count }}');
      return parts.length > 1 ? parts[1] : '';
    },
    displayCountText: function displayCountText(self) {
      var itemIndex = self.pageIndex * self.length;
      var mathMin = Math.min(itemIndex + self.length, self.total);
      return self.text.displayCount.replace(/{{\s*from\s*}}/, itemIndex + 1).replace(/{{\s*to\s*}}/, mathMin === -1 ? self.total : mathMin).replace(/{{\s*total\s*}}/, self.total);
    },
    pagination: function pagination(self) {
      if (self.total < 1) return [0];
      var pageCount = self.length > 0 ? Math.ceil(self.total / self.length) : 1;

      var range = _toConsumableArray(Array(pageCount).keys());

      if (self.pageIndex > 3) {
        range = [0, '...'].concat(range.slice(self.pageIndex - 1));
      }

      if (pageCount - self.pageIndex > 3) {
        range = range.slice(0, 5).concat(['...', pageCount - 1]);
      }

      return range;
    },
    sortedData: function sortedData(self) {
      var lower = self.sort.ascending ? -1 : 1;
      var sorted = self.filter(self.data.concat()) // The concat ensures the sort can't alter the original.
      .sort(function (row1, row2) {
        var _row1$self$sort$name$, _row1$self$sort$name, _row1$self$sort$name2, _row2$self$sort$name$, _row2$self$sort$name, _row2$self$sort$name2;

        var sortable1 = (_row1$self$sort$name$ = (_row1$self$sort$name = row1[self.sort.name]) === null || _row1$self$sort$name === void 0 ? void 0 : _row1$self$sort$name.sort) !== null && _row1$self$sort$name$ !== void 0 ? _row1$self$sort$name$ : (_row1$self$sort$name2 = row1[self.sort.name]) === null || _row1$self$sort$name2 === void 0 ? void 0 : _row1$self$sort$name2.text.toLowerCase();
        var sortable2 = (_row2$self$sort$name$ = (_row2$self$sort$name = row2[self.sort.name]) === null || _row2$self$sort$name === void 0 ? void 0 : _row2$self$sort$name.sort) !== null && _row2$self$sort$name$ !== void 0 ? _row2$self$sort$name$ : (_row2$self$sort$name2 = row2[self.sort.name]) === null || _row2$self$sort$name2 === void 0 ? void 0 : _row2$self$sort$name2.text.toLowerCase();
        if (sortable1 < sortable2) return lower;
        if (sortable1 > sortable2) return -lower;
        return 0;
      });
      var page = sorted;

      if (self.paging && self.length > 0) {
        var startIndex = self.pageIndex * self.length;
        page = sorted.slice(startIndex, startIndex + self.length);
      }

      return page.map(function (row) {
        return Object.fromEntries(Object.entries(row).map(function (cellPair) {
          var _cellPair = _slicedToArray(cellPair, 2),
              name = _cellPair[0],
              cell = _cellPair[1];

          if (cell.special !== null && cell.special !== undefined) {
            // This lets the client code alter the cell.
            self.$emit('special', cell);
          }

          return [name, cell];
        }));
      });
    },
    hiddenInputs: function hiddenInputs(self) {
      var inputs = [];
      self.data.forEach(function (row) {
        Object.values(row).filter(function (cell) {
          return _typeof(cell) === 'object' && 'hiddenInput' in cell;
        }).forEach(function (cell) {
          return Array.isArray(cell.hiddenInput) ? inputs.push.apply(inputs, _toConsumableArray(cell.hiddenInput)) : inputs.push(cell.hiddenInput);
        });
      }); // Calculate index

      var regex = /{{\s*index\s*}}/;

      for (var index = 0; index < inputs.length; index++) {
        var input = inputs[index];
        inputs[index] = {
          name: input.name.replace(regex, index),
          value: typeof input.value === 'string' ? input.value.replace(regex, index) : input.value
        };
      }

      return inputs;
    }
  },
  methods: {
    changePage: function changePage(page) {
      if (page >= 0 && page < this.total) this.pageIndex = page;
    },
    deleteRow: function deleteRow(rowIndex, promptText) {
      if (!window.confirm(promptText)) return;
      this.updateData(this.data.filter(function (row) {
        return row.$rowIndex !== rowIndex;
      }));
    },
    updateData: function updateData(newData) {
      this.$emit('update', newData);
    },
    updateSort: function updateSort(column) {
      if (!column.orderable) return;
      var sort = this.sort; // It only goes to descending on the second click of the same column header.

      var toAscending = !(sort.name === column.name && sort.ascending);
      sort.name = column.name;
      sort.ascending = toAscending;
    },
    updateColumn: function updateColumn(columnIndex, column) {
      var newColumns = this.columns.concat();
      newColumns.splice(columnIndex, 1, column);
      this.$emit('column', newColumns);
    },
    rowClasses: function rowClasses(row) {
      var classes = [];
      Object.values(row).forEach(function (cell) {
        if (typeof cell.rowClasses === 'string') {
          classes.push(cell.rowClasses);
        }
      });
      return classes.join(' ');
    }
  },
  created: function created() {
    var self = this;
    var changed = false;

    function updateData(rowIndex, columnName, newCell) {
      var newRow = _objectSpread({}, self.data[rowIndex]);

      newRow[columnName] = _objectSpread({}, newCell);
      Vue.set(self.data, rowIndex, newRow); // Regenerate this row for reactivity.

      changed = true;
    }

    function cloneCell(cell) {
      var newCell = _objectSpread({}, cell);

      delete newCell.special;
      return newCell;
    }

    self.data.forEach(function (row, rowIndex) {
      Object.keys(row).filter(function (key) {
        return key[0] !== '$';
      }).forEach(function (columnName) {
        var _cell$special;

        var cell = row[columnName];

        switch (cell === null || cell === void 0 ? void 0 : (_cell$special = cell.special) === null || _cell$special === void 0 ? void 0 : _cell$special.type) {
          case 'checkbox':
            {
              var special = cell.special;
              var newCell = cloneCell(cell);
              newCell.hiddenInput = {
                name: special.name,
                value: JSON.stringify(!!special.value)
              };
              newCell.component = {
                name: 'icbin-datatable-checkbox',
                value: {
                  label: special.label,
                  checked: !!special.value,
                  disabled: special.value === null,
                  classes: special.classes
                }
              };
              newCell.sort = special.value;
              updateData(rowIndex, columnName, newCell);
              break;
            }

          default:
            break;
        }
      });
    });
    if (changed) this.updateData(this.data);
  },
  mounted: function mounted() {
    var self = this;

    if (self.defaultSort) {
      self.sort.name = self.defaultSort.name;
      self.sort.ascending = self.defaultSort.ascending;
    } else {
      self.sort.name = self.columns[0].name;
    }

    if (self.defaultLength) self.length = self.defaultLength;
  },
  template: "\n    <div class=\"icbinDatatable\">\n        <div class=\"icbinDatatable__lengthPicker\" v-if=\"paging\">\n            {{ lengthPickerBefore }}\n            <select v-model=\"length\">\n                <option v-for=\"lengthOption in lengths\" :value=\"lengthOption\">\n                    {{ lengthOption > 0 ? lengthOption : text.all }}\n                </option>\n            </select>\n            {{ lengthPickerAfter }}\n        </div>\n        <div>\n            <div class=\"icbinDatatable__aboveHeader\"><slot></slot></div>\n            <table class=\"icbinDatatable__table dataTable row-border stripe table data-table no-footer\" role=\"grid\">\n                <thead class=\"dataTable__header\">\n                <tr class=\"dataTable__headerRow\" role=\"row\">\n                    <th v-for=\"(column, columnIndex) in columns\"\n                        class=\"dataTable__headerCell dataTable__cell sorting\"\n                        scope=\"col\"\n                        data-class-name=\"dataTable__cell\"\n                        :class=\"sort.name === column.name ? (sort.ascending ? 'sorting_asc' : 'sorting_desc') : ''\"\n                        :key=\"'icbinDatatable__column_' + columnIndex\"\n                        :data-orderable=\"(!!column.orderable).toString()\"\n                        :data-name=\"column.name\"\n                        :data-data=\"column.text\"\n                        @click=\"column.orderable && updateSort(column)\">\n                        <component v-if=\"column.component\"\n                                   :is=\"column.component.name\"\n                                   :data=\"data\"\n                                   :column=\"column\"\n                                   v-bind=\"column.component.value\"\n                                   @update=\"updateColumn(columnIndex, $event)\"\n                                   @component=\"$emit('component', -1, column, $event)\"  />\n                        <div class=\"dataTables_sizing\">\n                            {{ column.text }}\n                        </div>\n                    </th>\n                </tr>\n                </thead>\n                <tbody class=\"dataTable__body\">\n                <tr v-for=\"(row, rowIndex) in sortedData\"\n                    role=\"row\"\n                    class=\"dataTable__row\"\n                    :key=\"'icbinDatatable__row_' + rowIndex\"\n                    :class=\"(rowIndex % 2 ? 'even ' : 'odd ') + rowClasses(row)\">\n                    <td v-for=\"(column, columnIndex) in columns\"\n                        :key=\"'icbinDatatable__cell_' + rowIndex + 'x' + columnIndex\"\n                        class=\"dataTable__cell\"\n                        :class=\"{ sorting_1: sort.name === column.name }\">\n                        <template v-for=\"cell in [column.name in row ? row[column.name] : { text : '' }]\">\n                            <component v-if=\"cell.component\"\n                                       :is=\"cell.component.name\"\n                                       :data=\"data\"\n                                       :row-index=\"row.$rowIndex\"\n                                       :column-name=\"column.name\"\n                                       v-bind=\"cell.component.value\"\n                                       @delete=\"deleteRow(row.$rowIndex, $event)\"\n                                       @update=\"updateData($event)\"\n                                       @component=\"$emit('component', rowIndex, column.name, $event)\" />\n                            <a v-else-if=\"cell.href\"\n                               :href=\"cell.href\"\n                               :class=\"cell.badge ? ('badge badge-' + cell.badge) : ''\">\n                                {{ cell.text }}\n                            </a>\n                            <div v-else-if=\"cell.multipleLinks\">\n                                <a v-for=\"(link, index) in cell.multipleLinks\" :href=\"link.url\">\n                                    {{ link.text }}\n                                    <span v-if=\"cell.multipleLinks.length > 1 && cell.multipleLinks.length != index + 1\">, </span>\n                                </a>\n                            </div>\n                            <div v-else-if=\"cell.html\" v-html=\"cell.html\"></div>\n                            <span v-else :class=\"cell.badge ? ('badge badge-' + cell.badge) : ''\">{{ cell.text }}</span>\n                        </template>\n                    </td>\n                </tr>\n                </tbody>\n            </table>\n            <div class=\"icbinDatatable__footer\" v-if=\"paging\">\n                <div class=\"icbinDatatable__displayCount\">\n                    {{ displayCountText }}\n                </div>\n                <div class=\"icbinDatatable__pager\">\n                    <div class=\"dataTables_paginate paging_simple_numbers\">\n                        <ul class=\"pagination\">\n                            <li class=\"paginate_button page-item previous\" :class=\"{ disabled: pageIndex < 1 }\">\n                                <a class=\"page-link\" @click=\"changePage(pageIndex - 1)\">\n                                    {{ text.previous }}\n                                </a>\n                            </li>\n                            <li v-for=\"page in pagination\"\n                                class=\"paginate_button page-item\"\n                                :class=\"{ active: page === pageIndex }\">\n                                <a v-if=\"page !== '...'\" class=\"page-link\" @click=\"changePage(page)\">{{ page + 1 }}</a>\n                                <span v-else class=\"page-link\">...</span>\n                            </li>\n                            <li class=\"paginate_button page-item next\"\n                                :class=\"{ disabled: ((pageIndex + 1) * length) >= total }\">\n                                <a class=\"page-link\" @click=\"changePage(pageIndex + 1)\">\n                                    {{ text.next }}\n                                </a>\n                            </li>\n                        </ul>\n                    </div>\n                </div>\n            </div>\n        </div>\n        <div hidden>\n            <input v-for=\"hiddenInput in hiddenInputs\"\n                   :key=\"hiddenInput.name\"\n                   :name=\"hiddenInput.name\"\n                   :value=\"hiddenInput.value\"\n                   type=\"hidden\">\n        </div>\n    </div>"
};
window.icbinDataTable.remove = {
  name: 'icbin-datatable-remove',
  props: {
    text: {
      type: Object,
      required: true
    },
    disabled: {
      type: Boolean,
      "default": false
    }
  },
  template: "\n    <a href=\"javascript:void(0)\"\n       :class=\"{ 'icbinDatatableRemove': true, disabled: disabled }\"\n       @click=\"!disabled && $emit('delete', text.prompt)\">\n        <i class=\"fas fa-trash-alt\"></i>\n        {{ text.remove }}\n    </a>"
};
window.icbinDataTable.checkbox = {
  name: 'icbin-datatable-checkbox',
  props: {
    data: {
      type: Array,
      required: true
    },
    rowIndex: {
      type: Number,
      required: true
    },
    columnName: {
      type: String,
      required: true
    },
    label: {
      "default": ''
    },
    checked: {
      "default": undefined
    },
    disabled: {
      type: Boolean,
      "default": false
    },
    classes: {
      "default": ''
    }
  },
  methods: {
    update: function update(checked) {
      var _this = this;

      var cell = this.data.filter(function (row) {
        return row.$rowIndex === _this.rowIndex;
      })[0][this.columnName];
      cell.component.value.checked = checked;
      cell.hiddenInput.value = JSON.stringify(!!checked);
      cell.sort = checked;
      this.$emit('update', this.data);
      this.$emit('component', 'checked');
    }
  },
  mounted: function mounted() {
    this.$emit('component', 'checked');
  },
  template: "\n    <label class=\"icbinDatatableCheckbox__container\">\n        <input\n            :checked=\"checked\"\n            :disabled=\"disabled\"\n            :class=\"classes\"\n            class=\"icbinDatatableCheckbox\"\n            type=\"checkbox\"\n            @change=\"update($event.target.checked)\">\n        <span v-if=\"label\">{{ label }}</span>\n    </label>"
};