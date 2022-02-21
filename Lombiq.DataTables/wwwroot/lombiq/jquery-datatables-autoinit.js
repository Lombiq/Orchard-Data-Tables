"use strict";

jQuery(function ($) {
  $('table.data-table').each(function dataTableEach() {
    var options = this.getAttribute('data-options');
    $(this).dataTable(options ? JSON.parse(options) : undefined);
  });
});