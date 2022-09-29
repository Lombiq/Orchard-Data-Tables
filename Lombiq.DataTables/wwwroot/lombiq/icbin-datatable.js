"use strict";

function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); enumerableOnly && (symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; })), keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = null != arguments[i] ? arguments[i] : {}; i % 2 ? ownKeys(Object(source), !0).forEach(function (key) { _defineProperty(target, key, source[key]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)) : ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _typeof(obj) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (obj) { return typeof obj; } : function (obj) { return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }, _typeof(obj); }

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
      default: null
    },
    defaultLength: {
      type: Number,
      default: 10
    },
    lengths: {
      type: Array,
      default: function _default() {
        return [10, 25, 50, 100];
      }
    },
    paging: {
      default: true
    },
    filter: {
      default: function _default() {
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
      default: false
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
      default: ''
    },
    checked: {
      default: undefined
    },
    disabled: {
      type: Boolean,
      default: false
    },
    classes: {
      default: ''
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJuYW1lcyI6WyJ3aW5kb3ciLCJpY2JpbkRhdGFUYWJsZSIsInRhYmxlIiwibmFtZSIsIm1vZGVsIiwicHJvcCIsImV2ZW50IiwicHJvcHMiLCJkYXRhIiwidHlwZSIsIkFycmF5IiwicmVxdWlyZWQiLCJjb2x1bW5zIiwidGV4dCIsIk9iamVjdCIsImRlZmF1bHRTb3J0IiwiZGVmYXVsdCIsImRlZmF1bHRMZW5ndGgiLCJOdW1iZXIiLCJsZW5ndGhzIiwicGFnaW5nIiwiZmlsdGVyIiwiY29sbGVjdGlvbiIsInBhZ2VJbmRleCIsImxlbmd0aCIsInNvcnQiLCJhc2NlbmRpbmciLCJjb21wdXRlZCIsInRvdGFsIiwic2VsZiIsImxlbmd0aFBpY2tlckJlZm9yZSIsImxlbmd0aFBpY2tlciIsInNwbGl0IiwibGVuZ3RoUGlja2VyQWZ0ZXIiLCJwYXJ0cyIsImRpc3BsYXlDb3VudFRleHQiLCJpdGVtSW5kZXgiLCJtYXRoTWluIiwiTWF0aCIsIm1pbiIsImRpc3BsYXlDb3VudCIsInJlcGxhY2UiLCJwYWdpbmF0aW9uIiwicGFnZUNvdW50IiwiY2VpbCIsInJhbmdlIiwia2V5cyIsImNvbmNhdCIsInNsaWNlIiwic29ydGVkRGF0YSIsImxvd2VyIiwic29ydGVkIiwicm93MSIsInJvdzIiLCJzb3J0YWJsZTEiLCJ0b0xvd2VyQ2FzZSIsInNvcnRhYmxlMiIsInBhZ2UiLCJzdGFydEluZGV4IiwibWFwIiwicm93IiwiZnJvbUVudHJpZXMiLCJlbnRyaWVzIiwiY2VsbFBhaXIiLCJjZWxsIiwic3BlY2lhbCIsInVuZGVmaW5lZCIsIiRlbWl0IiwiaGlkZGVuSW5wdXRzIiwiaW5wdXRzIiwiZm9yRWFjaCIsInZhbHVlcyIsImlzQXJyYXkiLCJoaWRkZW5JbnB1dCIsInB1c2giLCJyZWdleCIsImluZGV4IiwiaW5wdXQiLCJ2YWx1ZSIsIm1ldGhvZHMiLCJjaGFuZ2VQYWdlIiwiZGVsZXRlUm93Iiwicm93SW5kZXgiLCJwcm9tcHRUZXh0IiwiY29uZmlybSIsInVwZGF0ZURhdGEiLCIkcm93SW5kZXgiLCJuZXdEYXRhIiwidXBkYXRlU29ydCIsImNvbHVtbiIsIm9yZGVyYWJsZSIsInRvQXNjZW5kaW5nIiwidXBkYXRlQ29sdW1uIiwiY29sdW1uSW5kZXgiLCJuZXdDb2x1bW5zIiwic3BsaWNlIiwicm93Q2xhc3NlcyIsImNsYXNzZXMiLCJqb2luIiwiY3JlYXRlZCIsImNoYW5nZWQiLCJjb2x1bW5OYW1lIiwibmV3Q2VsbCIsIm5ld1JvdyIsIlZ1ZSIsInNldCIsImNsb25lQ2VsbCIsImtleSIsIkpTT04iLCJzdHJpbmdpZnkiLCJjb21wb25lbnQiLCJsYWJlbCIsImNoZWNrZWQiLCJkaXNhYmxlZCIsIm1vdW50ZWQiLCJ0ZW1wbGF0ZSIsInJlbW92ZSIsIkJvb2xlYW4iLCJjaGVja2JveCIsIlN0cmluZyIsInVwZGF0ZSJdLCJzb3VyY2VzIjpbIi4uLy4uL0Fzc2V0cy9TY3JpcHRzL2ljYmluLWRhdGF0YWJsZS5qcyJdLCJzb3VyY2VzQ29udGVudCI6WyIvKiBnbG9iYWxzIFZ1ZSAqL1xyXG5cclxud2luZG93LmljYmluRGF0YVRhYmxlID0ge307XHJcblxyXG4vLyBldmVudHMgZW1pdHRlZDpcclxuLy8gLSBzcGVjaWFsKGNlbGwpOiBJZiB0aGUgY2VsbCBoYXMgYSBwcm9wZXJ0eSBjYWxsZWQgXCJzcGVjaWFsXCIgKHZhbHVlIGlzIG5vdCBudWxsIG9yXHJcbi8vICAgdW5kZWZpbmVkKSB0aGlzIGV2ZW50IGlzIGNhbGxlZCBpbnNpZGUgdGhlIHNvcnRlZERhdGEuIFRoaXMgZ2l2ZXMgYW4gb3Bwb3J0dW5pdHkgZm9yIHRoZVxyXG4vLyAgIGNsaWVudCBjb2RlIHRvIHVwZGF0ZSB0aGUgY2VsbCBkYXRhIChlLmcuIGJ5IHNldHRpbmcgdGhlIFwiY29tcG9uZW50XCIgYW5kIFwiaGlkZGVuSW5wdXRcIlxyXG4vLyAgIHByb3BlcnRpZXMpIHdpdGggZG9tYWluLXNwZWNpZmljIGJlaGF2aW9yIHdpdGhvdXQgaGF2aW5nIHRvIGVkaXQgdGhpcyBjb21wb25lbnQuXHJcbi8vIC0gdXBkYXRlKGRhdGEpOiBTZW5kcyB0aGUgbmV3IGRlc2lyZWQgdmFsdWUgb2YgdGhlIFwiZGF0YVwiIHByb3BlcnR5IHRvIHRoZSBwYXJlbnQuIEFsdGVybmF0aXZlbHlcclxuLy8gICB2LW1vZGVsIGNhbiBhbHNvIGJlIHVzZWQuXHJcbi8vIC0gY29sdW1uKGNvbHVtbnMpOiBTZW5kcyBhbiB1cGRhdGVkIGNvbHVtbnMgYXJyYXkgdG8gdGhlIHBhcmVudCBzbyBpdCBjYW4gcmVwbGFjZSB0aGUgY29sdW1ucyBwYXJhbWV0ZXIgd2l0aCBpdC5cclxuLy8gLSBjb21wb25lbnQocm93SW5kZXgsIGNvbHVtbk5hbWUsIHVzZXJEYXRhKTogUGFzc2VkIG9uIGZyb20gdGhlIGNoaWxkIGNvbXBvbmVudC4gSWYgaXQncyBhIGNvbHVtbiBoZWFkZXIgY29tcG9uZW50XHJcbi8vICAgdGhlbiBjb2x1bW5OYW1lIGlzIC0xLlxyXG4vL1xyXG4vLyBldmVudHMgcmVjZWl2ZWQ6XHJcbi8vIC0gZGVsZXRlKHByb21wdFRleHQpOiBjZWxsIGNvbXBvbmVudHMgbWF5IGVtaXQgdGhpcyBldmVudCB0byBzaWduYWwgYSByZXF1ZXN0IHRvIGRlbGV0ZSB0aGUgcm93XHJcbi8vICAgZnJvbSB0aGUgdGFibGUncyBkYXRhLiBPcHRpb25hbGx5IGEgbm9uLWVtcHR5IFN0cmluZyBtYXkgYmUgcGFzc2VkIGFzIGV2ZW50IGFyZ3VtZW50LiBJZiB0aGF0XHJcbi8vICAgaGFwcGVucywgYSBwcm9tcHQgd2lsbCBiZSBkaXNwbGF5ZWQgd2l0aCB0aGUgZ2l2ZW4gdGV4dCB0byBjb25maXJtIHdpdGggdGhlIHVzZXIgdGhhdCB0aGV5XHJcbi8vICAgcmVhbGx5IHdhbnQgdG8gcmVtb3ZlIHRoZSByb3cuXHJcbi8vIC0gdXBkYXRlKGRhdGEpOiBTYW1lIGFzIGFib3ZlLiBUaGUgY29tcG9uZW50IG1heSBoYXZlIGEgXCJkYXRhXCIgcHJvcGVydHkgZm9yIHRoaXMgcHVycG9zZS5cclxuLy8gLSBjb21wb25lbnQodXNlckRhdGEpOiBBIGNoaWxkIGNvbXBvbmVudCBtYXkgcmFpc2UgdGhpcyB0byBiZSBidWJibGVkIHVwIHRvIHRoZSBwYXJlbnQgY29tcG9uZW50LlxyXG5cclxud2luZG93LmljYmluRGF0YVRhYmxlLnRhYmxlID0ge1xyXG4gICAgbmFtZTogJ2ljYmluLWRhdGF0YWJsZScsXHJcbiAgICBtb2RlbDoge1xyXG4gICAgICAgIHByb3A6ICdkYXRhJyxcclxuICAgICAgICBldmVudDogJ3VwZGF0ZScsXHJcbiAgICB9LFxyXG4gICAgcHJvcHM6IHtcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIC8vIFtcclxuICAgICAgICAgICAgLy8gICB7XHJcbiAgICAgICAgICAgIC8vICAgICAkcm93SW5kZXg6IE51bWJlcixcclxuICAgICAgICAgICAgLy8gICAgIGNvbHVtbk5hbWU6IHtcclxuICAgICAgICAgICAgLy8gICAgICAgLy8gVGhlc2UgY2FuIGNvbWUgZnJvbSB0aGUgc2VydmVyOlxyXG4gICAgICAgICAgICAvLyAgICAgICB0ZXh0OiBTdHJpbmcsXHJcbiAgICAgICAgICAgIC8vICAgICAgIGh0bWw6IFN0cmluZz8sXHJcbiAgICAgICAgICAgIC8vICAgICAgIGJhZGdlOiBTdHJpbmc/XHJcbiAgICAgICAgICAgIC8vICAgICAgIHNvcnQ6IEFueT8sXHJcbiAgICAgICAgICAgIC8vICAgICAgIGhyZWY6IFN0cmluZz8sXHJcbiAgICAgICAgICAgIC8vICAgICAgIG11bHRpcGxlTGlua3M6IHsgbGluazogU3RyaW5nLCB0ZXh0OiBTdHJpbmcgfT8gb3IgWyB7IGxpbms6IFN0cmluZywgdGV4dDogU3RyaW5nIH0gXT8sXHJcbiAgICAgICAgICAgIC8vICAgICAgIHNwZWNpYWw6IEFueT8sXHJcbiAgICAgICAgICAgIC8vICAgICAgIGhpZGRlbklucHV0OiB7IG5hbWU6IFN0cmluZywgdmFsdWU6IFN0cmluZyB9PyBvciBbIHsgbmFtZTogU3RyaW5nLCB2YWx1ZTogU3RyaW5nIH0gXT9cclxuICAgICAgICAgICAgLy8gICAgICAgLy8gVGhlc2UgY2FuIGJlIHNldCBpbiBKUyBjb2RlIChlLmcuIHdpdGggdGhlIFwic3BlY2lhbFwiIGV2ZW50KTpcclxuICAgICAgICAgICAgLy8gICAgICAgY29tcG9uZW50OiB7IG5hbWU6IFN0cmluZz8sIHZhbHVlOiBPYmplY3QgfT9cclxuICAgICAgICAgICAgLy8gICAgICAgcm93Q2xhc3NlczogU3RyaW5nP1xyXG4gICAgICAgICAgICAvLyAgICAgfVxyXG4gICAgICAgICAgICAvLyAgIH1cclxuICAgICAgICAgICAgLy8gXVxyXG4gICAgICAgICAgICAvLyBub3RlOiBUaGUgbmFtZSBhbmQgdmFsdWUgaW4gdGhlIGhpZGRlbklucHV0IHByb3BlcnRpZXMgbWF5IGNvbnRhaW4gdGhlIHt7IGluZGV4IH19IGV4cHJlc3Npb24gd2hpY2ggaXNcclxuICAgICAgICAgICAgLy8gICAgICAgc3Vic3RpdHV0ZWQgd2l0aCBhIHplcm8tYmFzZWQgaW5kZXggd2hlbiBnZW5lcmF0aW5nIHRoZSBoaWRkZW5JbnB1dHMgY29tcHV0ZWQgcHJvcGVydHkuXHJcbiAgICAgICAgICAgIHR5cGU6IEFycmF5LFxyXG4gICAgICAgICAgICByZXF1aXJlZDogdHJ1ZSxcclxuICAgICAgICB9LFxyXG4gICAgICAgIGNvbHVtbnM6IHtcclxuICAgICAgICAgICAgdHlwZTogQXJyYXksXHJcbiAgICAgICAgICAgIHJlcXVpcmVkOiB0cnVlLFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgdGV4dDoge1xyXG4gICAgICAgICAgICAvLyBFeHBlY3RlZCBwcm9wZXJ0aWVzOiBsZW5ndGhQaWNrZXIsIGRpc3BsYXlDb3VudCwgcHJldmlvdXMsIG5leHQsIGFsbC5cclxuICAgICAgICAgICAgdHlwZTogT2JqZWN0LFxyXG4gICAgICAgICAgICByZXF1aXJlZDogdHJ1ZSxcclxuICAgICAgICB9LFxyXG4gICAgICAgIGRlZmF1bHRTb3J0OiB7XHJcbiAgICAgICAgICAgIC8vIHsgbmFtZTogXCJjb2x1bW5OYW1lXCIsIGFzY2VuZGluZzogdHJ1ZSB9XHJcbiAgICAgICAgICAgIGRlZmF1bHQ6IG51bGwsXHJcbiAgICAgICAgfSxcclxuICAgICAgICBkZWZhdWx0TGVuZ3RoOiB7XHJcbiAgICAgICAgICAgIHR5cGU6IE51bWJlcixcclxuICAgICAgICAgICAgZGVmYXVsdDogMTAsXHJcbiAgICAgICAgfSxcclxuICAgICAgICBsZW5ndGhzOiB7XHJcbiAgICAgICAgICAgIHR5cGU6IEFycmF5LFxyXG4gICAgICAgICAgICBkZWZhdWx0OiAoKSA9PiBbMTAsIDI1LCA1MCwgMTAwXSxcclxuICAgICAgICB9LFxyXG4gICAgICAgIHBhZ2luZzoge1xyXG4gICAgICAgICAgICBkZWZhdWx0OiB0cnVlLFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZmlsdGVyOiB7XHJcbiAgICAgICAgICAgIGRlZmF1bHQ6ICgpID0+IChjb2xsZWN0aW9uKSA9PiBjb2xsZWN0aW9uLFxyXG4gICAgICAgIH0sXHJcbiAgICB9LFxyXG4gICAgZGF0YTogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgIHBhZ2VJbmRleDogMCxcclxuICAgICAgICAgICAgbGVuZ3RoOiAxMCxcclxuICAgICAgICAgICAgc29ydDoge1xyXG4gICAgICAgICAgICAgICAgbmFtZTogbnVsbCxcclxuICAgICAgICAgICAgICAgIGFzY2VuZGluZzogdHJ1ZSxcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICB9O1xyXG4gICAgfSxcclxuICAgIGNvbXB1dGVkOiB7XHJcbiAgICAgICAgdG90YWwoc2VsZikge1xyXG4gICAgICAgICAgICByZXR1cm4gc2VsZi5kYXRhLmxlbmd0aDtcclxuICAgICAgICB9LFxyXG4gICAgICAgIGxlbmd0aFBpY2tlckJlZm9yZShzZWxmKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBzZWxmLnRleHQubGVuZ3RoUGlja2VyLnNwbGl0KCd7eyBjb3VudCB9fScpWzBdO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgbGVuZ3RoUGlja2VyQWZ0ZXIoc2VsZikge1xyXG4gICAgICAgICAgICBjb25zdCBwYXJ0cyA9IHNlbGYudGV4dC5sZW5ndGhQaWNrZXIuc3BsaXQoJ3t7IGNvdW50IH19Jyk7XHJcbiAgICAgICAgICAgIHJldHVybiBwYXJ0cy5sZW5ndGggPiAxID8gcGFydHNbMV0gOiAnJztcclxuICAgICAgICB9LFxyXG4gICAgICAgIGRpc3BsYXlDb3VudFRleHQoc2VsZikge1xyXG4gICAgICAgICAgICBjb25zdCBpdGVtSW5kZXggPSBzZWxmLnBhZ2VJbmRleCAqIHNlbGYubGVuZ3RoO1xyXG4gICAgICAgICAgICBjb25zdCBtYXRoTWluID0gTWF0aC5taW4oaXRlbUluZGV4ICsgc2VsZi5sZW5ndGgsIHNlbGYudG90YWwpO1xyXG4gICAgICAgICAgICByZXR1cm4gc2VsZi50ZXh0LmRpc3BsYXlDb3VudFxyXG4gICAgICAgICAgICAgICAgLnJlcGxhY2UoL3t7XFxzKmZyb21cXHMqfX0vLCBpdGVtSW5kZXggKyAxKVxyXG4gICAgICAgICAgICAgICAgLnJlcGxhY2UoL3t7XFxzKnRvXFxzKn19LywgbWF0aE1pbiA9PT0gLTEgPyBzZWxmLnRvdGFsIDogbWF0aE1pbilcclxuICAgICAgICAgICAgICAgIC5yZXBsYWNlKC97e1xccyp0b3RhbFxccyp9fS8sIHNlbGYudG90YWwpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgcGFnaW5hdGlvbihzZWxmKSB7XHJcbiAgICAgICAgICAgIGlmIChzZWxmLnRvdGFsIDwgMSkgcmV0dXJuIFswXTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IHBhZ2VDb3VudCA9IHNlbGYubGVuZ3RoID4gMCA/IE1hdGguY2VpbChzZWxmLnRvdGFsIC8gc2VsZi5sZW5ndGgpIDogMTtcclxuICAgICAgICAgICAgbGV0IHJhbmdlID0gWy4uLkFycmF5KHBhZ2VDb3VudCkua2V5cygpXTtcclxuICAgICAgICAgICAgaWYgKHNlbGYucGFnZUluZGV4ID4gMykge1xyXG4gICAgICAgICAgICAgICAgcmFuZ2UgPSBbMCwgJy4uLiddLmNvbmNhdChyYW5nZS5zbGljZShzZWxmLnBhZ2VJbmRleCAtIDEpKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBpZiAocGFnZUNvdW50IC0gc2VsZi5wYWdlSW5kZXggPiAzKSB7XHJcbiAgICAgICAgICAgICAgICByYW5nZSA9IHJhbmdlLnNsaWNlKDAsIDUpLmNvbmNhdChbJy4uLicsIHBhZ2VDb3VudCAtIDFdKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHJhbmdlO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc29ydGVkRGF0YShzZWxmKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IGxvd2VyID0gc2VsZi5zb3J0LmFzY2VuZGluZyA/IC0xIDogMTtcclxuICAgICAgICAgICAgY29uc3Qgc29ydGVkID0gc2VsZi5maWx0ZXIoc2VsZi5kYXRhLmNvbmNhdCgpKSAvLyBUaGUgY29uY2F0IGVuc3VyZXMgdGhlIHNvcnQgY2FuJ3QgYWx0ZXIgdGhlIG9yaWdpbmFsLlxyXG4gICAgICAgICAgICAgICAgLnNvcnQoKHJvdzEsIHJvdzIpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBzb3J0YWJsZTEgPSByb3cxW3NlbGYuc29ydC5uYW1lXT8uc29ydCA/PyByb3cxW3NlbGYuc29ydC5uYW1lXT8udGV4dC50b0xvd2VyQ2FzZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHNvcnRhYmxlMiA9IHJvdzJbc2VsZi5zb3J0Lm5hbWVdPy5zb3J0ID8/IHJvdzJbc2VsZi5zb3J0Lm5hbWVdPy50ZXh0LnRvTG93ZXJDYXNlKCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChzb3J0YWJsZTEgPCBzb3J0YWJsZTIpIHJldHVybiBsb3dlcjtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoc29ydGFibGUxID4gc29ydGFibGUyKSByZXR1cm4gLWxvd2VyO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gMDtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgbGV0IHBhZ2UgPSBzb3J0ZWQ7XHJcbiAgICAgICAgICAgIGlmIChzZWxmLnBhZ2luZyAmJiBzZWxmLmxlbmd0aCA+IDApIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IHN0YXJ0SW5kZXggPSBzZWxmLnBhZ2VJbmRleCAqIHNlbGYubGVuZ3RoO1xyXG4gICAgICAgICAgICAgICAgcGFnZSA9IHNvcnRlZC5zbGljZShzdGFydEluZGV4LCBzdGFydEluZGV4ICsgc2VsZi5sZW5ndGgpO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcGFnZS5tYXAoKHJvdykgPT4gT2JqZWN0LmZyb21FbnRyaWVzKFxyXG4gICAgICAgICAgICAgICAgT2JqZWN0XHJcbiAgICAgICAgICAgICAgICAgICAgLmVudHJpZXMocm93KVxyXG4gICAgICAgICAgICAgICAgICAgIC5tYXAoKGNlbGxQYWlyKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IFtuYW1lLCBjZWxsXSA9IGNlbGxQYWlyO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGNlbGwuc3BlY2lhbCAhPT0gbnVsbCAmJiBjZWxsLnNwZWNpYWwgIT09IHVuZGVmaW5lZCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gVGhpcyBsZXRzIHRoZSBjbGllbnQgY29kZSBhbHRlciB0aGUgY2VsbC5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHNlbGYuJGVtaXQoJ3NwZWNpYWwnLCBjZWxsKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFtuYW1lLCBjZWxsXTtcclxuICAgICAgICAgICAgICAgICAgICB9KSkpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgaGlkZGVuSW5wdXRzKHNlbGYpIHtcclxuICAgICAgICAgICAgY29uc3QgaW5wdXRzID0gW107XHJcblxyXG4gICAgICAgICAgICBzZWxmLmRhdGEuZm9yRWFjaCgocm93KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBPYmplY3QudmFsdWVzKHJvdylcclxuICAgICAgICAgICAgICAgICAgICAuZmlsdGVyKChjZWxsKSA9PiB0eXBlb2YgY2VsbCA9PT0gJ29iamVjdCcgJiYgJ2hpZGRlbklucHV0JyBpbiBjZWxsKVxyXG4gICAgICAgICAgICAgICAgICAgIC5mb3JFYWNoKChjZWxsKSA9PiAoQXJyYXkuaXNBcnJheShjZWxsLmhpZGRlbklucHV0KVxyXG4gICAgICAgICAgICAgICAgICAgICAgICA/IGlucHV0cy5wdXNoKC4uLmNlbGwuaGlkZGVuSW5wdXQpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIDogaW5wdXRzLnB1c2goY2VsbC5oaWRkZW5JbnB1dCkpKTtcclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAvLyBDYWxjdWxhdGUgaW5kZXhcclxuICAgICAgICAgICAgY29uc3QgcmVnZXggPSAve3tcXHMqaW5kZXhcXHMqfX0vO1xyXG4gICAgICAgICAgICBmb3IgKGxldCBpbmRleCA9IDA7IGluZGV4IDwgaW5wdXRzLmxlbmd0aDsgaW5kZXgrKykge1xyXG4gICAgICAgICAgICAgICAgY29uc3QgaW5wdXQgPSBpbnB1dHNbaW5kZXhdO1xyXG4gICAgICAgICAgICAgICAgaW5wdXRzW2luZGV4XSA9IHtcclxuICAgICAgICAgICAgICAgICAgICBuYW1lOiBpbnB1dC5uYW1lLnJlcGxhY2UocmVnZXgsIGluZGV4KSxcclxuICAgICAgICAgICAgICAgICAgICB2YWx1ZTogKHR5cGVvZiBpbnB1dC52YWx1ZSA9PT0gJ3N0cmluZycpID8gaW5wdXQudmFsdWUucmVwbGFjZShyZWdleCwgaW5kZXgpIDogaW5wdXQudmFsdWUsXHJcbiAgICAgICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gaW5wdXRzO1xyXG4gICAgICAgIH0sXHJcbiAgICB9LFxyXG4gICAgbWV0aG9kczoge1xyXG4gICAgICAgIGNoYW5nZVBhZ2UocGFnZSkge1xyXG4gICAgICAgICAgICBpZiAocGFnZSA+PSAwICYmIHBhZ2UgPCB0aGlzLnRvdGFsKSB0aGlzLnBhZ2VJbmRleCA9IHBhZ2U7XHJcbiAgICAgICAgfSxcclxuICAgICAgICBkZWxldGVSb3cocm93SW5kZXgsIHByb21wdFRleHQpIHtcclxuICAgICAgICAgICAgaWYgKCF3aW5kb3cuY29uZmlybShwcm9tcHRUZXh0KSkgcmV0dXJuO1xyXG4gICAgICAgICAgICB0aGlzLnVwZGF0ZURhdGEodGhpcy5kYXRhLmZpbHRlcigocm93KSA9PiByb3cuJHJvd0luZGV4ICE9PSByb3dJbmRleCkpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgdXBkYXRlRGF0YShuZXdEYXRhKSB7XHJcbiAgICAgICAgICAgIHRoaXMuJGVtaXQoJ3VwZGF0ZScsIG5ld0RhdGEpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgdXBkYXRlU29ydChjb2x1bW4pIHtcclxuICAgICAgICAgICAgaWYgKCFjb2x1bW4ub3JkZXJhYmxlKSByZXR1cm47XHJcbiAgICAgICAgICAgIGNvbnN0IHNvcnQgPSB0aGlzLnNvcnQ7XHJcblxyXG4gICAgICAgICAgICAvLyBJdCBvbmx5IGdvZXMgdG8gZGVzY2VuZGluZyBvbiB0aGUgc2Vjb25kIGNsaWNrIG9mIHRoZSBzYW1lIGNvbHVtbiBoZWFkZXIuXHJcbiAgICAgICAgICAgIGNvbnN0IHRvQXNjZW5kaW5nID0gIShzb3J0Lm5hbWUgPT09IGNvbHVtbi5uYW1lICYmIHNvcnQuYXNjZW5kaW5nKTtcclxuXHJcbiAgICAgICAgICAgIHNvcnQubmFtZSA9IGNvbHVtbi5uYW1lO1xyXG4gICAgICAgICAgICBzb3J0LmFzY2VuZGluZyA9IHRvQXNjZW5kaW5nO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgdXBkYXRlQ29sdW1uKGNvbHVtbkluZGV4LCBjb2x1bW4pIHtcclxuICAgICAgICAgICAgY29uc3QgbmV3Q29sdW1ucyA9IHRoaXMuY29sdW1ucy5jb25jYXQoKTtcclxuICAgICAgICAgICAgbmV3Q29sdW1ucy5zcGxpY2UoY29sdW1uSW5kZXgsIDEsIGNvbHVtbik7XHJcbiAgICAgICAgICAgIHRoaXMuJGVtaXQoJ2NvbHVtbicsIG5ld0NvbHVtbnMpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgcm93Q2xhc3Nlcyhyb3cpIHtcclxuICAgICAgICAgICAgY29uc3QgY2xhc3NlcyA9IFtdO1xyXG5cclxuICAgICAgICAgICAgT2JqZWN0LnZhbHVlcyhyb3cpLmZvckVhY2goKGNlbGwpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmICh0eXBlb2YgY2VsbC5yb3dDbGFzc2VzID09PSAnc3RyaW5nJykge1xyXG4gICAgICAgICAgICAgICAgICAgIGNsYXNzZXMucHVzaChjZWxsLnJvd0NsYXNzZXMpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBjbGFzc2VzLmpvaW4oJyAnKTtcclxuICAgICAgICB9LFxyXG4gICAgfSxcclxuICAgIGNyZWF0ZWQ6IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICBjb25zdCBzZWxmID0gdGhpcztcclxuICAgICAgICBsZXQgY2hhbmdlZCA9IGZhbHNlO1xyXG5cclxuICAgICAgICBmdW5jdGlvbiB1cGRhdGVEYXRhKHJvd0luZGV4LCBjb2x1bW5OYW1lLCBuZXdDZWxsKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IG5ld1JvdyA9IHsgLi4uc2VsZi5kYXRhW3Jvd0luZGV4XSB9O1xyXG4gICAgICAgICAgICBuZXdSb3dbY29sdW1uTmFtZV0gPSB7IC4uLm5ld0NlbGwgfTtcclxuICAgICAgICAgICAgVnVlLnNldChzZWxmLmRhdGEsIHJvd0luZGV4LCBuZXdSb3cpOyAvLyBSZWdlbmVyYXRlIHRoaXMgcm93IGZvciByZWFjdGl2aXR5LlxyXG4gICAgICAgICAgICBjaGFuZ2VkID0gdHJ1ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGZ1bmN0aW9uIGNsb25lQ2VsbChjZWxsKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IG5ld0NlbGwgPSB7IC4uLmNlbGwgfTtcclxuICAgICAgICAgICAgZGVsZXRlIG5ld0NlbGwuc3BlY2lhbDtcclxuICAgICAgICAgICAgcmV0dXJuIG5ld0NlbGw7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBzZWxmLmRhdGEuZm9yRWFjaCgocm93LCByb3dJbmRleCkgPT4ge1xyXG4gICAgICAgICAgICBPYmplY3Qua2V5cyhyb3cpXHJcbiAgICAgICAgICAgICAgICAuZmlsdGVyKChrZXkpID0+IGtleVswXSAhPT0gJyQnKVxyXG4gICAgICAgICAgICAgICAgLmZvckVhY2goKGNvbHVtbk5hbWUpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBjZWxsID0gcm93W2NvbHVtbk5hbWVdO1xyXG4gICAgICAgICAgICAgICAgICAgIHN3aXRjaCAoY2VsbD8uc3BlY2lhbD8udHlwZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjYXNlICdjaGVja2JveCc6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IHNwZWNpYWwgPSBjZWxsLnNwZWNpYWw7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBuZXdDZWxsID0gY2xvbmVDZWxsKGNlbGwpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG5ld0NlbGwuaGlkZGVuSW5wdXQgPSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgbmFtZTogc3BlY2lhbC5uYW1lLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhbHVlOiBKU09OLnN0cmluZ2lmeSghIXNwZWNpYWwudmFsdWUpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBuZXdDZWxsLmNvbXBvbmVudCA9IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBuYW1lOiAnaWNiaW4tZGF0YXRhYmxlLWNoZWNrYm94JyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YWx1ZToge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBsYWJlbDogc3BlY2lhbC5sYWJlbCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgY2hlY2tlZDogISFzcGVjaWFsLnZhbHVlLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkaXNhYmxlZDogc3BlY2lhbC52YWx1ZSA9PT0gbnVsbCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3Nlczogc3BlY2lhbC5jbGFzc2VzLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbmV3Q2VsbC5zb3J0ID0gc3BlY2lhbC52YWx1ZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHVwZGF0ZURhdGEocm93SW5kZXgsIGNvbHVtbk5hbWUsIG5ld0NlbGwpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgZGVmYXVsdDpcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBpZiAoY2hhbmdlZCkgdGhpcy51cGRhdGVEYXRhKHRoaXMuZGF0YSk7XHJcbiAgICB9LFxyXG4gICAgbW91bnRlZDogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgIGNvbnN0IHNlbGYgPSB0aGlzO1xyXG5cclxuICAgICAgICBpZiAoc2VsZi5kZWZhdWx0U29ydCkge1xyXG4gICAgICAgICAgICBzZWxmLnNvcnQubmFtZSA9IHNlbGYuZGVmYXVsdFNvcnQubmFtZTtcclxuICAgICAgICAgICAgc2VsZi5zb3J0LmFzY2VuZGluZyA9IHNlbGYuZGVmYXVsdFNvcnQuYXNjZW5kaW5nO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlIHtcclxuICAgICAgICAgICAgc2VsZi5zb3J0Lm5hbWUgPSBzZWxmLmNvbHVtbnNbMF0ubmFtZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChzZWxmLmRlZmF1bHRMZW5ndGgpIHNlbGYubGVuZ3RoID0gc2VsZi5kZWZhdWx0TGVuZ3RoO1xyXG4gICAgfSxcclxuICAgIHRlbXBsYXRlOiBgXHJcbiAgICA8ZGl2IGNsYXNzPVwiaWNiaW5EYXRhdGFibGVcIj5cclxuICAgICAgICA8ZGl2IGNsYXNzPVwiaWNiaW5EYXRhdGFibGVfX2xlbmd0aFBpY2tlclwiIHYtaWY9XCJwYWdpbmdcIj5cclxuICAgICAgICAgICAge3sgbGVuZ3RoUGlja2VyQmVmb3JlIH19XHJcbiAgICAgICAgICAgIDxzZWxlY3Qgdi1tb2RlbD1cImxlbmd0aFwiPlxyXG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2LWZvcj1cImxlbmd0aE9wdGlvbiBpbiBsZW5ndGhzXCIgOnZhbHVlPVwibGVuZ3RoT3B0aW9uXCI+XHJcbiAgICAgICAgICAgICAgICAgICAge3sgbGVuZ3RoT3B0aW9uID4gMCA/IGxlbmd0aE9wdGlvbiA6IHRleHQuYWxsIH19XHJcbiAgICAgICAgICAgICAgICA8L29wdGlvbj5cclxuICAgICAgICAgICAgPC9zZWxlY3Q+XHJcbiAgICAgICAgICAgIHt7IGxlbmd0aFBpY2tlckFmdGVyIH19XHJcbiAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgPGRpdj5cclxuICAgICAgICAgICAgPGRpdiBjbGFzcz1cImljYmluRGF0YXRhYmxlX19hYm92ZUhlYWRlclwiPjxzbG90Pjwvc2xvdD48L2Rpdj5cclxuICAgICAgICAgICAgPHRhYmxlIGNsYXNzPVwiaWNiaW5EYXRhdGFibGVfX3RhYmxlIGRhdGFUYWJsZSByb3ctYm9yZGVyIHN0cmlwZSB0YWJsZSBkYXRhLXRhYmxlIG5vLWZvb3RlclwiIHJvbGU9XCJncmlkXCI+XHJcbiAgICAgICAgICAgICAgICA8dGhlYWQgY2xhc3M9XCJkYXRhVGFibGVfX2hlYWRlclwiPlxyXG4gICAgICAgICAgICAgICAgPHRyIGNsYXNzPVwiZGF0YVRhYmxlX19oZWFkZXJSb3dcIiByb2xlPVwicm93XCI+XHJcbiAgICAgICAgICAgICAgICAgICAgPHRoIHYtZm9yPVwiKGNvbHVtbiwgY29sdW1uSW5kZXgpIGluIGNvbHVtbnNcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICBjbGFzcz1cImRhdGFUYWJsZV9faGVhZGVyQ2VsbCBkYXRhVGFibGVfX2NlbGwgc29ydGluZ1wiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHNjb3BlPVwiY29sXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGF0YS1jbGFzcy1uYW1lPVwiZGF0YVRhYmxlX19jZWxsXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgOmNsYXNzPVwic29ydC5uYW1lID09PSBjb2x1bW4ubmFtZSA/IChzb3J0LmFzY2VuZGluZyA/ICdzb3J0aW5nX2FzYycgOiAnc29ydGluZ19kZXNjJykgOiAnJ1wiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIDprZXk9XCInaWNiaW5EYXRhdGFibGVfX2NvbHVtbl8nICsgY29sdW1uSW5kZXhcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICA6ZGF0YS1vcmRlcmFibGU9XCIoISFjb2x1bW4ub3JkZXJhYmxlKS50b1N0cmluZygpXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgOmRhdGEtbmFtZT1cImNvbHVtbi5uYW1lXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgOmRhdGEtZGF0YT1cImNvbHVtbi50ZXh0XCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgQGNsaWNrPVwiY29sdW1uLm9yZGVyYWJsZSAmJiB1cGRhdGVTb3J0KGNvbHVtbilcIj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgPGNvbXBvbmVudCB2LWlmPVwiY29sdW1uLmNvbXBvbmVudFwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgOmlzPVwiY29sdW1uLmNvbXBvbmVudC5uYW1lXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA6ZGF0YT1cImRhdGFcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDpjb2x1bW49XCJjb2x1bW5cIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHYtYmluZD1cImNvbHVtbi5jb21wb25lbnQudmFsdWVcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEB1cGRhdGU9XCJ1cGRhdGVDb2x1bW4oY29sdW1uSW5kZXgsICRldmVudClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBjb21wb25lbnQ9XCIkZW1pdCgnY29tcG9uZW50JywgLTEsIGNvbHVtbiwgJGV2ZW50KVwiICAvPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiZGF0YVRhYmxlc19zaXppbmdcIj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHt7IGNvbHVtbi50ZXh0IH19XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxyXG4gICAgICAgICAgICAgICAgICAgIDwvdGg+XHJcbiAgICAgICAgICAgICAgICA8L3RyPlxyXG4gICAgICAgICAgICAgICAgPC90aGVhZD5cclxuICAgICAgICAgICAgICAgIDx0Ym9keSBjbGFzcz1cImRhdGFUYWJsZV9fYm9keVwiPlxyXG4gICAgICAgICAgICAgICAgPHRyIHYtZm9yPVwiKHJvdywgcm93SW5kZXgpIGluIHNvcnRlZERhdGFcIlxyXG4gICAgICAgICAgICAgICAgICAgIHJvbGU9XCJyb3dcIlxyXG4gICAgICAgICAgICAgICAgICAgIGNsYXNzPVwiZGF0YVRhYmxlX19yb3dcIlxyXG4gICAgICAgICAgICAgICAgICAgIDprZXk9XCInaWNiaW5EYXRhdGFibGVfX3Jvd18nICsgcm93SW5kZXhcIlxyXG4gICAgICAgICAgICAgICAgICAgIDpjbGFzcz1cIihyb3dJbmRleCAlIDIgPyAnZXZlbiAnIDogJ29kZCAnKSArIHJvd0NsYXNzZXMocm93KVwiPlxyXG4gICAgICAgICAgICAgICAgICAgIDx0ZCB2LWZvcj1cIihjb2x1bW4sIGNvbHVtbkluZGV4KSBpbiBjb2x1bW5zXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgOmtleT1cIidpY2JpbkRhdGF0YWJsZV9fY2VsbF8nICsgcm93SW5kZXggKyAneCcgKyBjb2x1bW5JbmRleFwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzPVwiZGF0YVRhYmxlX19jZWxsXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgOmNsYXNzPVwieyBzb3J0aW5nXzE6IHNvcnQubmFtZSA9PT0gY29sdW1uLm5hbWUgfVwiPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICA8dGVtcGxhdGUgdi1mb3I9XCJjZWxsIGluIFtjb2x1bW4ubmFtZSBpbiByb3cgPyByb3dbY29sdW1uLm5hbWVdIDogeyB0ZXh0IDogJycgfV1cIj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxjb21wb25lbnQgdi1pZj1cImNlbGwuY29tcG9uZW50XCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgOmlzPVwiY2VsbC5jb21wb25lbnQubmFtZVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDpkYXRhPVwiZGF0YVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDpyb3ctaW5kZXg9XCJyb3cuJHJvd0luZGV4XCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgOmNvbHVtbi1uYW1lPVwiY29sdW1uLm5hbWVcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB2LWJpbmQ9XCJjZWxsLmNvbXBvbmVudC52YWx1ZVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEBkZWxldGU9XCJkZWxldGVSb3cocm93LiRyb3dJbmRleCwgJGV2ZW50KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEB1cGRhdGU9XCJ1cGRhdGVEYXRhKCRldmVudClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBAY29tcG9uZW50PVwiJGVtaXQoJ2NvbXBvbmVudCcsIHJvd0luZGV4LCBjb2x1bW4ubmFtZSwgJGV2ZW50KVwiIC8+XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YSB2LWVsc2UtaWY9XCJjZWxsLmhyZWZcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgOmhyZWY9XCJjZWxsLmhyZWZcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgOmNsYXNzPVwiY2VsbC5iYWRnZSA/ICgnYmFkZ2UgYmFkZ2UtJyArIGNlbGwuYmFkZ2UpIDogJydcIj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB7eyBjZWxsLnRleHQgfX1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgdi1lbHNlLWlmPVwiY2VsbC5tdWx0aXBsZUxpbmtzXCI+XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgdi1mb3I9XCIobGluaywgaW5kZXgpIGluIGNlbGwubXVsdGlwbGVMaW5rc1wiIDpocmVmPVwibGluay51cmxcIj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAge3sgbGluay50ZXh0IH19XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIHYtaWY9XCJjZWxsLm11bHRpcGxlTGlua3MubGVuZ3RoID4gMSAmJiBjZWxsLm11bHRpcGxlTGlua3MubGVuZ3RoICE9IGluZGV4ICsgMVwiPiwgPC9zcGFuPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiB2LWVsc2UtaWY9XCJjZWxsLmh0bWxcIiB2LWh0bWw9XCJjZWxsLmh0bWxcIj48L2Rpdj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIHYtZWxzZSA6Y2xhc3M9XCJjZWxsLmJhZGdlID8gKCdiYWRnZSBiYWRnZS0nICsgY2VsbC5iYWRnZSkgOiAnJ1wiPnt7IGNlbGwudGV4dCB9fTwvc3Bhbj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgPC90ZW1wbGF0ZT5cclxuICAgICAgICAgICAgICAgICAgICA8L3RkPlxyXG4gICAgICAgICAgICAgICAgPC90cj5cclxuICAgICAgICAgICAgICAgIDwvdGJvZHk+XHJcbiAgICAgICAgICAgIDwvdGFibGU+XHJcbiAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJpY2JpbkRhdGF0YWJsZV9fZm9vdGVyXCIgdi1pZj1cInBhZ2luZ1wiPlxyXG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzcz1cImljYmluRGF0YXRhYmxlX19kaXNwbGF5Q291bnRcIj5cclxuICAgICAgICAgICAgICAgICAgICB7eyBkaXNwbGF5Q291bnRUZXh0IH19XHJcbiAgICAgICAgICAgICAgICA8L2Rpdj5cclxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJpY2JpbkRhdGF0YWJsZV9fcGFnZXJcIj5cclxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwiZGF0YVRhYmxlc19wYWdpbmF0ZSBwYWdpbmdfc2ltcGxlX251bWJlcnNcIj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgPHVsIGNsYXNzPVwicGFnaW5hdGlvblwiPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnaW5hdGVfYnV0dG9uIHBhZ2UtaXRlbSBwcmV2aW91c1wiIDpjbGFzcz1cInsgZGlzYWJsZWQ6IHBhZ2VJbmRleCA8IDEgfVwiPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxhIGNsYXNzPVwicGFnZS1saW5rXCIgQGNsaWNrPVwiY2hhbmdlUGFnZShwYWdlSW5kZXggLSAxKVwiPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB7eyB0ZXh0LnByZXZpb3VzIH19XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9hPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9saT5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxsaSB2LWZvcj1cInBhZ2UgaW4gcGFnaW5hdGlvblwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3M9XCJwYWdpbmF0ZV9idXR0b24gcGFnZS1pdGVtXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA6Y2xhc3M9XCJ7IGFjdGl2ZTogcGFnZSA9PT0gcGFnZUluZGV4IH1cIj5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8YSB2LWlmPVwicGFnZSAhPT0gJy4uLidcIiBjbGFzcz1cInBhZ2UtbGlua1wiIEBjbGljaz1cImNoYW5nZVBhZ2UocGFnZSlcIj57eyBwYWdlICsgMSB9fTwvYT5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiB2LWVsc2UgY2xhc3M9XCJwYWdlLWxpbmtcIj4uLi48L3NwYW4+XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2xpPlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPGxpIGNsYXNzPVwicGFnaW5hdGVfYnV0dG9uIHBhZ2UtaXRlbSBuZXh0XCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA6Y2xhc3M9XCJ7IGRpc2FibGVkOiAoKHBhZ2VJbmRleCArIDEpICogbGVuZ3RoKSA+PSB0b3RhbCB9XCI+XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9XCJwYWdlLWxpbmtcIiBAY2xpY2s9XCJjaGFuZ2VQYWdlKHBhZ2VJbmRleCArIDEpXCI+XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHt7IHRleHQubmV4dCB9fVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvYT5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvbGk+XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIDwvdWw+XHJcbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgICAgICAgICA8L2Rpdj5cclxuICAgICAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgPGRpdiBoaWRkZW4+XHJcbiAgICAgICAgICAgIDxpbnB1dCB2LWZvcj1cImhpZGRlbklucHV0IGluIGhpZGRlbklucHV0c1wiXHJcbiAgICAgICAgICAgICAgICAgICA6a2V5PVwiaGlkZGVuSW5wdXQubmFtZVwiXHJcbiAgICAgICAgICAgICAgICAgICA6bmFtZT1cImhpZGRlbklucHV0Lm5hbWVcIlxyXG4gICAgICAgICAgICAgICAgICAgOnZhbHVlPVwiaGlkZGVuSW5wdXQudmFsdWVcIlxyXG4gICAgICAgICAgICAgICAgICAgdHlwZT1cImhpZGRlblwiPlxyXG4gICAgICAgIDwvZGl2PlxyXG4gICAgPC9kaXY+YCxcclxufTtcclxuXHJcbndpbmRvdy5pY2JpbkRhdGFUYWJsZS5yZW1vdmUgPSB7XHJcbiAgICBuYW1lOiAnaWNiaW4tZGF0YXRhYmxlLXJlbW92ZScsXHJcbiAgICBwcm9wczoge1xyXG4gICAgICAgIHRleHQ6IHsgdHlwZTogT2JqZWN0LCByZXF1aXJlZDogdHJ1ZSB9LFxyXG4gICAgICAgIGRpc2FibGVkOiB7IHR5cGU6IEJvb2xlYW4sIGRlZmF1bHQ6IGZhbHNlIH0sXHJcbiAgICB9LFxyXG4gICAgdGVtcGxhdGU6IGBcclxuICAgIDxhIGhyZWY9XCJqYXZhc2NyaXB0OnZvaWQoMClcIlxyXG4gICAgICAgOmNsYXNzPVwieyAnaWNiaW5EYXRhdGFibGVSZW1vdmUnOiB0cnVlLCBkaXNhYmxlZDogZGlzYWJsZWQgfVwiXHJcbiAgICAgICBAY2xpY2s9XCIhZGlzYWJsZWQgJiYgJGVtaXQoJ2RlbGV0ZScsIHRleHQucHJvbXB0KVwiPlxyXG4gICAgICAgIDxpIGNsYXNzPVwiZmFzIGZhLXRyYXNoLWFsdFwiPjwvaT5cclxuICAgICAgICB7eyB0ZXh0LnJlbW92ZSB9fVxyXG4gICAgPC9hPmAsXHJcbn07XHJcblxyXG53aW5kb3cuaWNiaW5EYXRhVGFibGUuY2hlY2tib3ggPSB7XHJcbiAgICBuYW1lOiAnaWNiaW4tZGF0YXRhYmxlLWNoZWNrYm94JyxcclxuICAgIHByb3BzOiB7XHJcbiAgICAgICAgZGF0YTogeyB0eXBlOiBBcnJheSwgcmVxdWlyZWQ6IHRydWUgfSxcclxuICAgICAgICByb3dJbmRleDogeyB0eXBlOiBOdW1iZXIsIHJlcXVpcmVkOiB0cnVlIH0sXHJcbiAgICAgICAgY29sdW1uTmFtZTogeyB0eXBlOiBTdHJpbmcsIHJlcXVpcmVkOiB0cnVlIH0sXHJcbiAgICAgICAgbGFiZWw6IHsgZGVmYXVsdDogJycgfSxcclxuICAgICAgICBjaGVja2VkOiB7IGRlZmF1bHQ6IHVuZGVmaW5lZCB9LFxyXG4gICAgICAgIGRpc2FibGVkOiB7IHR5cGU6IEJvb2xlYW4sIGRlZmF1bHQ6IGZhbHNlIH0sXHJcbiAgICAgICAgY2xhc3NlczogeyBkZWZhdWx0OiAnJyB9LFxyXG4gICAgfSxcclxuICAgIG1ldGhvZHM6IHtcclxuICAgICAgICB1cGRhdGUoY2hlY2tlZCkge1xyXG4gICAgICAgICAgICBjb25zdCBjZWxsID0gdGhpcy5kYXRhLmZpbHRlcigocm93KSA9PiByb3cuJHJvd0luZGV4ID09PSB0aGlzLnJvd0luZGV4KVswXVt0aGlzLmNvbHVtbk5hbWVdO1xyXG4gICAgICAgICAgICBjZWxsLmNvbXBvbmVudC52YWx1ZS5jaGVja2VkID0gY2hlY2tlZDtcclxuICAgICAgICAgICAgY2VsbC5oaWRkZW5JbnB1dC52YWx1ZSA9IEpTT04uc3RyaW5naWZ5KCEhY2hlY2tlZCk7XHJcbiAgICAgICAgICAgIGNlbGwuc29ydCA9IGNoZWNrZWQ7XHJcbiAgICAgICAgICAgIHRoaXMuJGVtaXQoJ3VwZGF0ZScsIHRoaXMuZGF0YSk7XHJcbiAgICAgICAgICAgIHRoaXMuJGVtaXQoJ2NvbXBvbmVudCcsICdjaGVja2VkJyk7XHJcbiAgICAgICAgfSxcclxuICAgIH0sXHJcbiAgICBtb3VudGVkOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgdGhpcy4kZW1pdCgnY29tcG9uZW50JywgJ2NoZWNrZWQnKTtcclxuICAgIH0sXHJcbiAgICB0ZW1wbGF0ZTogYFxyXG4gICAgPGxhYmVsIGNsYXNzPVwiaWNiaW5EYXRhdGFibGVDaGVja2JveF9fY29udGFpbmVyXCI+XHJcbiAgICAgICAgPGlucHV0XHJcbiAgICAgICAgICAgIDpjaGVja2VkPVwiY2hlY2tlZFwiXHJcbiAgICAgICAgICAgIDpkaXNhYmxlZD1cImRpc2FibGVkXCJcclxuICAgICAgICAgICAgOmNsYXNzPVwiY2xhc3Nlc1wiXHJcbiAgICAgICAgICAgIGNsYXNzPVwiaWNiaW5EYXRhdGFibGVDaGVja2JveFwiXHJcbiAgICAgICAgICAgIHR5cGU9XCJjaGVja2JveFwiXHJcbiAgICAgICAgICAgIEBjaGFuZ2U9XCJ1cGRhdGUoJGV2ZW50LnRhcmdldC5jaGVja2VkKVwiPlxyXG4gICAgICAgIDxzcGFuIHYtaWY9XCJsYWJlbFwiPnt7IGxhYmVsIH19PC9zcGFuPlxyXG4gICAgPC9sYWJlbD5gLFxyXG59O1xyXG4iXSwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUFBO0FBRUFBLE1BQU0sQ0FBQ0MsY0FBUCxHQUF3QixFQUF4QixDLENBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBRCxNQUFNLENBQUNDLGNBQVAsQ0FBc0JDLEtBQXRCLEdBQThCO0VBQzFCQyxJQUFJLEVBQUUsaUJBRG9CO0VBRTFCQyxLQUFLLEVBQUU7SUFDSEMsSUFBSSxFQUFFLE1BREg7SUFFSEMsS0FBSyxFQUFFO0VBRkosQ0FGbUI7RUFNMUJDLEtBQUssRUFBRTtJQUNIQyxJQUFJLEVBQUU7TUFDRjtNQUNBO01BQ0E7TUFDQTtNQUNBO01BQ0E7TUFDQTtNQUNBO01BQ0E7TUFDQTtNQUNBO01BQ0E7TUFDQTtNQUNBO01BQ0E7TUFDQTtNQUNBO01BQ0E7TUFDQTtNQUNBO01BQ0E7TUFDQUMsSUFBSSxFQUFFQyxLQXRCSjtNQXVCRkMsUUFBUSxFQUFFO0lBdkJSLENBREg7SUEwQkhDLE9BQU8sRUFBRTtNQUNMSCxJQUFJLEVBQUVDLEtBREQ7TUFFTEMsUUFBUSxFQUFFO0lBRkwsQ0ExQk47SUE4QkhFLElBQUksRUFBRTtNQUNGO01BQ0FKLElBQUksRUFBRUssTUFGSjtNQUdGSCxRQUFRLEVBQUU7SUFIUixDQTlCSDtJQW1DSEksV0FBVyxFQUFFO01BQ1Q7TUFDQUMsT0FBTyxFQUFFO0lBRkEsQ0FuQ1Y7SUF1Q0hDLGFBQWEsRUFBRTtNQUNYUixJQUFJLEVBQUVTLE1BREs7TUFF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