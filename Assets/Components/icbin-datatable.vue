<template>
    <div class="icbin-datatable">
        <div class="icbin-datatable-length-picker">
            {{ lengthPickerBefore }}
            <select v-model="showEntries">
                <option v-for="length in lengths" :value="length">{{ length }}</option>
            </select>
            {{ lengthPickerAfter }}
        </div>
        <div>
            <slot name="aboveHeader"></slot>
            <table class="icbin-datatable-table">
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
    </div>
</template>

<script>

export default {
    name: 'icbin-datatable',
    props: {
        text: {
            // Expected properties: lengthPicker, displayCount, previous, next.
            type: Object,
            required: true
        },
        length: {
            type: Number,
            default: 10,
        },
        lengths: {
            type: Array,
            default: () => [ 10, 25, 50, 100 ],
        },
        columns: {
            type: Array,
            required: true,
        },
        data: {
            type: Array,
            required: true,
        },
        total: {
            type: Number,
            required: true,
        },
    },
    data: function() {
        return {
            pageIndex: 0,

        };
    },
    computed: {
        total(self) { return self.data.length; },
        lengthPickerBefore(self) {
            return self.text.lengthPicker.split('{{ count }}')[0];
        },
        lengthPickerAfter(self) {
            const parts = self.text.lengthPicker.split('{{ count }}');
            return parts.length > 1 ? parts.length[1] : '';
        },
        displayCountText(self) {
            const itemIndex = self.pageIndex * self.length;
            return self.text.displayCount
                .replace(/{{\s*from\s*}}/, itemIndex + 1)
                .replace(/{{\s*to\s*}}/, Math.min(itemIndex + self.length, self.total))
                .replace(/{{\s*total\s*}}/, self.total);
        },
        pagination(self) {
            let range = [...Array(self.total).keys()];
            if (self.pageIndex > 3) {
                range = [0, '...'].concat(range.slice(self.pageIndex - 1))
            }
            if (self.total - self.pageIndex > 3)
            {
                range = range.slice(0, 5).concat(['...', self.total - 1]);
            }

            return range;
        },
    },
    methods: {
        changePage(page) {

        },
    },
}
</script>
