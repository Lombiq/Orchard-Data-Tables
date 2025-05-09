jQuery(($) => {
    $('table.data-table').each(function dataTableEach() {
        const options = this.getAttribute('data-options');
        $(this).dataTable(options ? JSON.parse(options) : undefined);
    });
});
