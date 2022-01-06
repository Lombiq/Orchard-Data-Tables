/* globals Vue */

window.icbinDataTable = {};

// events emitted:
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
            required: true,
        },
        columns: {
            type: Array,
            required: true,
        },
        text: {
            // Expected properties: lengthPicker, displayCount, previous, next, all.
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
        paging: {
            default: true,
        },
        filter: {
            default: () => (collection) => collection,
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
            if (self.total < 1) return [0];

            const pageCount = self.length > 0 ? Math.ceil(self.total / self.length) : 1;
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
            const sorted = self.filter(self.data.concat()) // The concat ensures the sort can't alter the original.
                .sort((row1, row2) => {
                    const sortable1 = row1[self.sort.name]?.sort ?? row1[self.sort.name]?.text;
                    const sortable2 = row2[self.sort.name]?.sort ?? row2[self.sort.name]?.text;

                    if (sortable1 < sortable2) return lower;
                    if (sortable1 > sortable2) return -lower;

                    return 0;
                });

            let page = sorted;
            if (self.paging && self.length > 0) {
                const startIndex = self.pageIndex * self.length;
                page = sorted.slice(startIndex, startIndex + self.length);
            }

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
                    .forEach((cell) => (Array.isArray(cell.hiddenInput)
                        ? inputs.push(...cell.hiddenInput)
                        : inputs.push(cell.hiddenInput)));
            });

            // Calculate index
            const regex = /{{\s*index\s*}}/;
            for (let index = 0; index < inputs.length; index++) {
                const input = inputs[index];
                inputs[index] = {
                    name: input.name.replace(regex, index),
                    value: (typeof input.value === 'string') ? input.value.replace(regex, index) : input.value,
                };
            }

            return inputs;
        },
    },
    methods: {
        changePage(page) {
            if (page >= 0 && page < this.total) this.pageIndex = page;
        },
        deleteRow(rowIndex, promptText) {
            if (!window.confirm(promptText)) return;
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
        updateColumn(columnIndex, column) {
            const newColumns = this.columns.concat();
            newColumns.splice(columnIndex, 1, column);
            this.$emit('column', newColumns);
        },
        rowClasses(row) {
            const classes = [];

            Object.values(row).forEach((cell) => {
                if (typeof cell.rowClasses === 'string') {
                    classes.push(cell.rowClasses);
                }
            });

            return classes.join(' ');
        },
    },
    created: function () {
        const self = this;
        let changed = false;

        function updateData(rowIndex, columnName, newCell) {
            const newRow = { ...self.data[rowIndex] };
            newRow[columnName] = { ...newCell };
            Vue.set(self.data, rowIndex, newRow); // Regenerate this row for reactivity.
            changed = true;
        }

        function cloneCell(cell) {
            const newCell = { ...cell };
            delete newCell.special;
            return newCell;
        }

        self.data.forEach((row, rowIndex) => {
            Object.keys(row)
                .filter((key) => key[0] !== '$')
                .forEach((columnName) => {
                    const cell = row[columnName];
                    switch (cell?.special?.type) {
                        case 'checkbox': {
                            const special = cell.special;
                            const newCell = cloneCell(cell);

                            newCell.hiddenInput = {
                                name: special.name,
                                value: JSON.stringify(!!special.value),
                            };

                            newCell.component = {
                                name: 'icbin-datatable-checkbox',
                                value: {
                                    label: special.label,
                                    checked: !!special.value,
                                    disabled: special.value === null,
                                    classes: special.classes,
                                },
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
    <div class="icbinDatatable">
        <div class="icbinDatatable__lengthPicker" v-if="paging">
            {{ lengthPickerBefore }}
            <select v-model="length">
                <option v-for="lengthOption in lengths" :value="lengthOption">
                    {{ lengthOption > 0 ? lengthOption : text.all }}
                </option>
            </select>
            {{ lengthPickerAfter }}
        </div>
        <div>
            <div class="icbinDatatable__aboveHeader"><slot></slot></div>
            <table class="icbinDatatable__table dataTable row-border stripe table data-table no-footer" role="grid">
                <thead class="dataTable__header">
                <tr class="dataTable__headerRow" role="row">
                    <th v-for="(column, columnIndex) in columns"
                        class="dataTable__headerCell dataTable__cell sorting"
                        scope="col"
                        data-class-name="dataTable__cell"
                        :class="sort.name === column.name ? (sort.ascending ? 'sorting_asc' : 'sorting_desc') : ''"
                        :key="'icbinDatatable__column_' + columnIndex"
                        :data-orderable="(!!column.orderable).toString()"
                        :data-name="column.name"
                        :data-data="column.text"
                        @click="column.orderable && updateSort(column)">
                        <component v-if="column.component"
                                   :is="column.component.name"
                                   :data="data"
                                   :column="column"
                                   v-bind="column.component.value"
                                   @update="updateColumn(columnIndex, $event)"
                                   @component="$emit('component', -1, column, $event)"  />
                        <div class="dataTables_sizing">
                            {{ column.text }}
                        </div>
                    </th>
                </tr>
                </thead>
                <tbody class="dataTable__body">
                <tr v-for="(row, rowIndex) in sortedData"
                    role="row"
                    class="dataTable__row"
                    :key="'icbinDatatable__row_' + rowIndex"
                    :class="(rowIndex % 2 ? 'even ' : 'odd ') + rowClasses(row)">
                    <td v-for="(column, columnIndex) in columns"
                        :key="'icbinDatatable__cell_' + rowIndex + 'x' + columnIndex"
                        class="dataTable__cell"
                        :class="{ sorting_1: sort.name === column.name }">
                        <template v-for="cell in [column.name in row ? row[column.name] : { text : '' }]">
                            <component v-if="cell.component"
                                       :is="cell.component.name"
                                       :data="data"
                                       :row-index="row.$rowIndex"
                                       :column-name="column.name"
                                       v-bind="cell.component.value"
                                       @delete="deleteRow(row.$rowIndex, $event)"
                                       @update="updateData($event)"
                                       @component="$emit('component', rowIndex, column.name, $event)" />
                            <a v-else-if="cell.href"
                               :href="cell.href"
                               :class="cell.badge ? ('badge badge-' + cell.badge) : ''">
                                {{ cell.text }}
                            </a>
                            <div v-else-if="cell.multipleLinks">
                                <a v-for="(link, index) in cell.multipleLinks" :href="link.url">
                                    {{ link.text }}
                                    <span v-if="cell.multipleLinks.length > 1 && cell.multipleLinks.length != index + 1">, </span>
                                </a>
                            </div>
                            <div v-else-if="cell.html" v-html="cell.html"></div>
                            <span v-else :class="cell.badge ? ('badge badge-' + cell.badge) : ''">{{ cell.text }}</span>
                        </template>
                    </td>
                </tr>
                </tbody>
            </table>
            <div class="icbinDatatable__footer" v-if="paging">
                <div class="icbinDatatable__displayCount">
                    {{ displayCountText }}
                </div>
                <div class="icbinDatatable__pager">
                    <div class="dataTables_paginate paging_simple_numbers">
                        <ul class="pagination">
                            <li class="paginate_button page-item previous" :class="{ disabled: pageIndex < 1 }">
                                <a class="page-link" @click="changePage(pageIndex - 1)">
                                    {{ text.previous }}
                                </a>
                            </li>
                            <li v-for="page in pagination"
                                class="paginate_button page-item"
                                :class="{ active: page === pageIndex }">
                                <a v-if="page !== '...'" class="page-link" @click="changePage(page)">{{ page + 1 }}</a>
                                <span v-else class="page-link">...</span>
                            </li>
                            <li class="paginate_button page-item next"
                                :class="{ disabled: ((pageIndex + 1) * length) >= total }">
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
        disabled: { type: Boolean, default: false },
    },
    template: `
    <a href="javascript:void(0)"
       :class="{ 'icbinDatatableRemove': true, disabled: disabled }"
       @click="!disabled && $emit('delete', text.prompt)">
        <i class="fas fa-trash-alt"></i>
        {{ text.remove }}
    </a>`,
};

window.icbinDataTable.checkbox = {
    name: 'icbin-datatable-checkbox',
    props: {
        data: { type: Array, required: true },
        rowIndex: { type: Number, required: true },
        columnName: { type: String, required: true },
        label: { default: '' },
        checked: { default: undefined },
        disabled: { type: Boolean, default: false },
        classes: { default: '' },
    },
    methods: {
        update(checked) {
            const cell = this.data.filter((row) => row.$rowIndex === this.rowIndex)[0][this.columnName];
            cell.component.value.checked = checked;
            cell.hiddenInput.value = JSON.stringify(!!checked);
            cell.sort = checked;
            this.$emit('update', this.data);
            this.$emit('component', 'checked');
        },
    },
    mounted: function () {
        this.$emit('component', 'checked');
    },
    template: `
    <label class="icbinDatatableCheckbox__container">
        <input
            :checked="checked"
            :disabled="disabled"
            :class="classes"
            class="icbinDatatableCheckbox"
            type="checkbox"
            @change="update($event.target.checked)">
        <span v-if="label">{{ label }}</span>
    </label>`,
};
