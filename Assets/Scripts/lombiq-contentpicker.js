/**
 * @summary     Lombiq - Content Picker
 * @description Initializes a content picker shape and uses the parent window as the container of the content picker
 * settings.
 * @version     1.0
 * @file        lombiq-contentpicker.js
 * @author      Lombiq Technologies Ltd.
 */

(function contentPicker($, window) {
    const pluginName = 'lombiq_ContentPicker';

    const defaults = {
        url: '',
        finishButtonContent: '',
        cancelButtonContent: '',
        displayColorboxBorder: true,
        finishCallback: function () { },
        cancelCallback: function () { },
        contentPicker: {
            selectedContentItemIds: [],
            finish: false,
        },
        colorboxSettings: {
            iframe: true,
            width: '1000px',
            height: '80%',
            maxWidth: '95%',
            maxHeight: '80%',
            fixed: true,
            fastIframe: false,
            closeButton: false,
        },
    };

    $[pluginName] = function contentPickerByPlugin(options) {
        const plugin = this;

        plugin.settings = $.extend(true, {}, defaults, options);

        window.contentPicker = plugin.settings.contentPicker;
        window.contentPicker.finishButtonContent = plugin.settings.finishButtonContent;
        window.contentPicker.cancelButtonContent = plugin.settings.cancelButtonContent;

        const colorboxSettings = $.extend(true, {}, plugin.settings.colorboxSettings, {
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
                    $('#cboxTopLeft').hide();
                    $('#cboxTopRight').hide();
                    $('#cboxBottomLeft').hide();
                    $('#cboxBottomRight').hide();
                    $('#cboxMiddleLeft').hide();
                    $('#cboxMiddleRight').hide();
                    $('#cboxTopCenter').hide();
                    $('#cboxBottomCenter').hide();
                }
            },
            onLoad: function () {
                if (!plugin.settings.displayColorboxBorder) {
                    $('#cboxLoadedContent').css('margin-bottom', '0');
                }
            },
        });

        $.colorbox(colorboxSettings);

        let resizeTimer;
        function resizeColorBox() {
            if (resizeTimer) clearTimeout(resizeTimer);
            resizeTimer = setTimeout(() => {
                if ($('#cboxOverlay').is(':visible')) {
                    $.colorbox.resize({
                        width: window.innerWidth > parseInt(plugin.settings.colorboxSettings.width, 10)
                            ? plugin.settings.colorboxSettings.width
                            : plugin.settings.colorboxSettings.maxWidth,
                        height: window.innerHeight > parseInt(plugin.settings.colorboxSettings.maxHeight, 10)
                            ? plugin.settings.colorboxSettings.maxHeight
                            : plugin.settings.colorboxSettings.height,
                    });
                }
            }, 300);
        }

        // Resize Colorbox when resizing window or changing mobile device orientation.
        $(window).resize(resizeColorBox);
        window.addEventListener('orientationchange', resizeColorBox, false);
    };
})(jQuery, window, document);
