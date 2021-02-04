(function ($) {
    $(() => {
        $('table.data-table').each(function () {
            const options = this.getAttribute('data-options');
            $(this).dataTable(options ? JSON.parse(options) : undefined);
        });
    });
}(jQuery));
