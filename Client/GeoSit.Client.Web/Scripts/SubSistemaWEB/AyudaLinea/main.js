$(document).ready(initAyuda);
$(window).resize(ajustarmodal);
$('#modal-window-ayuda').on('shown.bs.modal', function (e) {
    //ajustarScrollBars();
    ajustarmodal();
    hideLoading();
});
function initAyuda() {
    ///////////////////// Scrollbars ////////////////////////
    $(".ayuda-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-ayuda .panel-body').resize(ajustarScrollBars);
    $('.ayuda-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".ayuda-content").getNiceScroll().resize();
        }, 10);
    });
    /////////////////////////////////////////////////////////

    $('.collapse').collapse('hide');

    ////////////////////////////////////////////////////////
    ajustarmodal();
    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-ayuda .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-ayuda").modal('show');

};

function ajustarmodal() {
    var altura = $(window).height() - 190;
    $(".ayuda-body").css({ "height": altura});
    $(".ayuda-content").css({ "max-height": altura, "overflow": "hidden" });

    ajustarScrollBars();
}
function ajustarScrollBars() {
    $(".ayuda-content").getNiceScroll().resize();
    $(".ayuda-content").getNiceScroll().show();

}

//////////PDF//////////

/*function verDocumento(id) {
    showLoading();
    $('#visorPdf').load(BASE_URL + "AyudaLinea/View/" + id);
}*/

//////////END PDF//////////

//@ sourceURL=WebInstructivo.js