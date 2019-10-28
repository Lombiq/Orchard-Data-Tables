/**
 * @summary     Lombiq - Content Picker
 * @description Initializes a content picker shape and uses the parent window as the container of the content picker settings.
 * @version     1.0
 * @file        lombiq-contentpicker.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_ContentPicker";

    var defaults = {
        url: "",
        finishButtonContent: "",
        cancelButtonContent: "",
        finishCallback: function (contentPicker) { },
        cancelCallback: function (contentPicker) { },
        contentPicker: {
            selectedContentItemIds: [],
            finish: false
        },
        colorboxSettings: {
            iframe: true,
            width: "1000px",
            height: "80%",
            fixed: true,
            fastIframe: false,
            closeButton: false
        }
    };

    $[pluginName] = function (options) {
        var plugin = this;

        plugin.settings = $.extend(true, {}, defaults, options);

        window.contentPicker = plugin.settings.contentPicker;
        window.contentPicker.finishButtonContent = plugin.settings.finishButtonContent;
        window.contentPicker.cancelButtonContent = plugin.settings.cancelButtonContent;

        var colorboxSettings = $.extend(true, {}, plugin.settings.colorboxSettings, {
            href: plugin.settings.url,
            onCleanup: function () {
                if (window.contentPicker.finish) {
                    plugin.settings.finishCallback(window.contentPicker);
                }
                else {
                    plugin.settings.cancelCallback();
                }
            }
        });

        $.colorbox(colorboxSettings);
    };
})(jQuery, window, document);