﻿/**
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
        displayColorboxBorder: true,
        finishCallback: function (contentPicker) { },
        cancelCallback: function (contentPicker) { },
        contentPicker: {
            selectedContentItemIds: [],
            finish: false
        },
        colorboxSettings: {
            iframe: true,
            width: "80%",
            height: "90%",
            maxWidth: "80%",
            maxHeight: "90%",
            left: "10%",
            top: "5%",
            fastIframe: false,
            closeButton: false,
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
            },
            onOpen: function () {
                if (!plugin.settings.displayColorboxBorder) {
                    $("#cboxTopLeft").remove();
                    $("#cboxTopRight").remove();
                    $("#cboxBottomLeft").remove();
                    $("#cboxBottomRight").remove();
                    $("#cboxMiddleLeft").remove();
                    $("#cboxMiddleRight").remove();
                    $("#cboxTopCenter").remove();
                    $("#cboxBottomCenter").remove();
                }
            },
            onLoad: function () {
                if (!plugin.settings.displayColorboxBorder) {
                    $("#cboxLoadedContent").css("margin-bottom", "0");
                }
            }
        });

        $.colorbox(colorboxSettings);

        var resizeTimer;
        function resizeColorBox() {
            if (resizeTimer) clearTimeout(resizeTimer);
            resizeTimer = setTimeout(function () {
                if (jQuery("#cboxOverlay").is(":visible")) {
                    jQuery.colorbox.resize({
                        width: window.innerWidth > parseInt(plugin.settings.colorboxSettings.width)
                            ? plugin.settings.colorboxSettings.width
                            : plugin.settings.colorboxSettings.maxWidth,
                        height: window.innerHeight > parseInt(plugin.settings.colorboxSettings.maxHeight)
                            ? plugin.settings.colorboxSettings.maxHeight
                            : plugin.settings.colorboxSettings.height
                    });
                }
            }, 300);
        }

        // Resize Colorbox when resizing window or changing mobile device orientation.
        window.addEventListener("orientationchange", resizeColorBox, false);
    };
})(jQuery, window, document);