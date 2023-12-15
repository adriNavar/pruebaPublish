(function ($) {
    $.fn.oneClick = function (handler) {
        if ($.isFunction(handler)) {
            this.click(function() {
                var me = $(this);
                if (!me.data("isClicked")) {
                    handler.call(this);
                    me.data("isClicked", true);
                    setTimeout(function () {
                        me.removeData("isClicked");
                    }, 3000);
                }
            });
        }
        return this;
    };
}(jQuery));

