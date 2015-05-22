(function ($) {
    var methods = {
        init: function (options) {
            return build($(this));
        },
        close: function (index) {
            $(("." + buildClass(index))).remove();$(this).css("max-height","none");
        }
    };
    function build(container) {
        container.html('');
        var count = $(".load_Wrap").length, newclass = buildClass(count+1), x = container.offset().left, y = container.offset().top;
        $("body").append('<div class="load_Wrap ' + newclass + '"><div class="load_Main"></div></div>');
        var obj = $(("." + newclass));
        obj.css({ top: y, left: x, width: container.width(), height: container.height() });
        obj.find(".load_Main").css("margin-top", (container.height() / 2 - 12));
        container.css({ "max-height": container.height(), "overflow": "hidden" });
        return count + 1;
    }
    function buildClass(index) { return 'lw_index_' + index; }

    $.fn.sunload = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            throw new Error("调用方法不存在");
        };
    };
})(jQuery);