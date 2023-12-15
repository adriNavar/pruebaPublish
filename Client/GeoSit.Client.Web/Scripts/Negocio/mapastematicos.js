

function opacityScroll() {
    $('ul li').each(function (i) {
        var oTop = $(this).offset().top;
        var oHeight = $(this).outerHeight();

        var wTop = $(window).scrollTop();
        var wHeight = $(window).height();

        if (oTop < wTop + wHeight) {
            var diff = ((wTop + wHeight - oTop) / oHeight);

            if (diff > 1) diff = 1;
            else if (diff < 0) diff = 0;

            $(this).css('opacity', diff);
        }
    });
}
