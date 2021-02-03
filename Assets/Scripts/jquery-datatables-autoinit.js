(function ($) {
    $(() => {
        $('table.data-table').each(function () {
            var options = this.getAttribute('data-options');
            $(this).dataTable(options ? JSON.parse(options) : undefined);
        });
    });
}(jQuery));
