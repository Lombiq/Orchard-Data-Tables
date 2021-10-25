window.icbinDataTable = {};

// events emitted:
// - special(cell): If the cell has a property called "special" (value is not null or
//   undefined) this event is called inside the sortedData. This gives an opportunity for the
//   client code to update the cell data (e.g. by setting the "component" and "hiddenInput"
//   properties) with domain-specific behavior without having to edit this component.
// - update(data): Sends the new desired value of the "data" property to the parent. Alternatively
//   v-model can also be used.
//
// events received:
// - delete(promptText): cell components may emit this event to signal a request to delete the row
//   from the table's data. Optionally a non-empty String may be passed as event argument. If that
//   happens, a prompt will be displayed with the given text to confirm with the user that they
//   really want to remove the row.
// - update(data): Same as above. The component may have a "data" property for this purpose.

window.icbinDataTable.table = {
    name: 'icbin-datatable',
    model: {
        prop: 'data',
        event: 'update',
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
            //       sort: Any?,
            //       href: String?,
            //       special: Any?,
            //       // These can be set in JS code (e.g. with the "special" event):
            //       component: { name: String?, props: Object }?
            //       hiddenInput: { name: String, value: String }?
            //     }
            //   }
            // ]
            type: Array,
            required: true,
        },
        columns: {
            type: Array,
            required: true,
        },
        text: {
            // Expected properties: lengthPicker, displayCount, previous, next.
            type: Object,
            required: true,
        },
        defaultSort: {
            // { name: "columnName", ascending: true }
            default: null,
        },
        defaultLength: {
            type: Number,
            default: 10,
        },
        lengths: {
            type: Array,
            default: () => [10, 25, 50, 100],
        },
    },
    data: function () {
        return {
            pageIndex: 0,
            length: 10,
            sort: {
                name: null,
                ascending: true,
            },
        };
    },
    computed: {
        total(self) {
            return self.data.length;
        },
        lengthPickerBefore(self) {
            return self.text.lengthPicker.split('{{ count }}')[0];
        },
        lengthPickerAfter(self) {
            const parts = self.text.lengthPicker.split('{{ count }}');
            return parts.length > 1 ? parts[1] : '';
        },
        displayCountText(self) {
            const itemIndex = self.pageIndex * self.length;
            return self.text.displayCount
                .replace(/{{\s*from\s*}}/, itemIndex + 1)
                .replace(/{{\s*to\s*}}/, Math.min(itemIndex + self.length, self.total))
                .replace(/{{\s*total\s*}}/, self.total);
        },
        pagination(self) {
            const pageCount = Math.ceil(self.total / self.length);
            let range = [...Array(pageCount).keys()];
            if (self.pageIndex > 3) {
                range = [0, '...'].concat(range.slice(self.pageIndex - 1));
            }
            if (pageCount - self.pageIndex > 3) {
                range = range.slice(0, 5).concat(['...', pageCount - 1]);
            }

            return range;
        },
        sortedData(self) {
            const lower = self.sort.ascending ? -1 : 1;
            const sorted = self.data
                .concat() // Prevents the sort altering the original.
                .sort((row1, row2) => {
                    const sortable1 = row1[self.sort.name]?.sort ?? row1[self.sort.name]?.text;
                    const sortable2 = row2[self.sort.name]?.sort ?? row2[self.sort.name]?.text;

                    if (sortable1 < sortable2) return lower;
                    if (sortable1 > sortable2) return -lower;

                    return 0;
                });

            const page = sorted.slice(self.pageIndex * self.length, self.length);

            return page.map((row) => Object.fromEntries(
                Object
                    .entries(row)
                    .map((cellPair) => {
                        const [name, cell] = cellPair;

                        if (cell.special !== null && cell.special !== undefined) {
                            // This lets the client code alter the cell.
                            self.$emit('special', cell);
                        }

                        return [name, cell];
                    })));
        },
        hiddenInputs(self) {
            const inputs = [];

            self.data.forEach((row) => {
                Object.values(row)
                    .filter((cell) => typeof cell === 'object' && 'hiddenInput' in cell)
                    .forEach((cell) => inputs.push(cell.hiddenInput));
            });

            return inputs;
        },
    },
    methods: {
        changePage(page) {
            if (page >= 0 && page < this.total) this.pageIndex = page;
        },
        deleteRow(rowIndex, promptText) {
            if (!confirm(promptText)) return;
            this.updateData(this.data.filter((row) => row.$rowIndex !== rowIndex));
        },
        updateData(newData) {
            this.$emit('update', newData);
        },
        updateSort(column) {
            if (!column.orderable) return;
            const sort = this.sort;

            // It only goes to descending on the second click of the same column header.
            const toAscending = !(sort.name === column.name && sort.ascending);

            sort.name = column.name;
            sort.ascending = toAscending;
        },
    },
    mounted: function () {
        const self = this;

        if (self.defaultSort) {
            self.sort.name = self.defaultSort.name;
            self.sort.ascending = self.defaultSort.ascending;
        }
        else {
            self.sort.name = self.columns[0].name;
        }

        if (self.defaultLength) self.length = self.defaultLength;
    },
    template: `
    <div class="icbin-datatable">
        <div class="icbin-datatable-length-picker">
            {{ lengthPickerBefore }}
            <select v-model="length">
                <option v-for="lengthOption in lengths" :value="lengthOption">
                    {{ lengthOption }}
                </option>
            </select>
            {{ lengthPickerAfter }}
        </div>
        <div>
            <slot></slot>
            <table class="icbin-datatable-table dataTable row-border stripe table data-table no-footer" role="grid">
                <thead class="dataTable__header">
                <tr class="dataTable__headerRow" role="row">
                    <th v-for="(column, columnIndex) in columns"
                        class="dataTable__headerCell dataTable__cell sorting"
                        scope="col"
                        data-class-name="dataTable__cell"
                        :class="sort.name === column.name ? (sort.ascending ? 'sorting_asc' : 'sorting_desc') : ''"
                        :key="'icbin-datatable-column-' + columnIndex"
                        :data-orderable="(!!column.orderable).toString()"
                        :data-name="column.name"
                        :data-data="column.name"
                        @click="updateSort(column)">
                        <div class="dataTables_sizing">
                            {{ column.text }}
                        </div>
                    </th>
                </tr>
                </thead>
                <tbody class="dataTable__body">
                <tr v-for="(row, rowIndex) in sortedData"
                    role="row"
                    :key="'icbin-datatable-row-' + rowIndex"
                    :class="'dataTable__row ' + (rowIndex % 2 ? 'even' : 'odd')">
                    <td v-for="(column, columnIndex) in columns"
                        :key="'icbin-datatable-cell-' + rowIndex + '-' + columnIndex"
                        :class="{ dataTable__cell: true, sorting_1: sort.name === column.name }">
                        <template v-for="cell in [column.name in row ? row[column.name] : { text : '' }]">
                            <component v-if="cell.component"
                                       :is="cell.component.name"
                                       :data="data"
                                       v-bind="cell.component.value"
                                       @delete="deleteRow(row.$rowIndex, $event)"
                                       @update="updateData($event)" />
                            <a v-else-if="cell.href" :href="cell.href">{{ cell.text }}</a>
                            <template v-else-if="cell.html" v-html="cell.html"></template>
                            <span v-else>{{ cell.text }}</span>
                        </template>
                    </td>
                </tr>
                </tbody>
            </table>
            <div class="icbin-datatable-footer">
                <div class="icbin-datatable-display-count">
                    {{ displayCountText }}
                </div>
                <div class="icbin-datatable-display-pager">
                    <div class="dataTables_paginate paging_simple_numbers">
                        <ul class="pagination">
                            <li class="paginate_button page-item previous" :class="{ disabled: pageIndex < 1 }">
                                <a class="page-link" @click="changePage(pageIndex - 1)">
                                    {{ text.previous }}
                                </a>
                            </li>
                            <li v-for="page in pagination" class="paginate_button page-item" :class="{ active: page === pageIndex }">
                                <a v-if="page !== '...'" class="page-link" @click="changePage(page)">{{ page + 1 }}</a>
                                <span v-else class="page-link">...</span>
                            </li>
                            <li class="paginate_button page-item next" :class="{ disabled: (pageIndex * length) >= total }">
                                <a class="page-link" @click="changePage(pageIndex + 1)">
                                    {{ text.next }}
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <div hidden>
            <input v-for="hiddenInput in hiddenInputs"
                   :key="hiddenInput.name"
                   :name="hiddenInput.name"
                   :value="hiddenInput.value"
                   type="hidden">
        </div>
    </div>`,
};

window.icbinDataTable.remove = {
    name: 'icbin-datatable-remove',
    props: {
        text: { type: Object, required: true },
    },
    template: `<a href="javascript:void(0)" @click="$emit('delete', text.prompt)">
        <i class="fas fa-trash-alt"></i>
        {{ text.remove }}
    </a>`,
};
