$('#myModalMT').on('shown.bs.modal', function (e) {
    hideLoading();
});
$(document).ready(function () {
    $("#formVolver").ajaxForm({
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $("#contenido").html(data);
            hideLoading();
        },
        error: function () {
            alert("error al volver");
        }
    });

    $("#formAtributos").ajaxForm({
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $("#contenido").html(data);
            hideLoading();
        },
        error: function () {
            alert("error en siguiente");
        }
    });

    $('#myModalMT').on('shown.bs.modal', function (e) {
        ajustarScrollBars();
    });
    ///////////////////// Scrollbars ////////////////////////
    $(".modal-body").niceScroll(getNiceScrollConfig());
    $('.modal-body').resize(ajustarScrollBars);
    ////////////////////////////////////////////////////////
    ajustarmodal();

    //$('#ModalAdvertencia').modal('show');

    $("#btnGrabarAdvertencia").click(function () {
        $("#formAtributos").submit();
    });

    $("#btnCancelarAdvertencia").click(function () {
        $("#volverbtn").click();
    });
    $("#ModalAdvertencia").modal("show");
});

function ajustarmodal() {
    var altura = $(window).height() - 60; //value corresponding to the modal heading + footer
    var ancho = $(window).width() - 150; //value corresponding to the modal heading + footer
    $("#myModalMT .modal-content").css({ "height": altura, "overflow": "hidden" });
    $("#myModalMT .modal-content").css({ "width": ancho });
    $("#myModalMT .modal-dialog").css({ "height": altura });
    $("#myModalMT .modal-dialog").css({ "width": ancho });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $("#myModal .modal-content").getNiceScroll().resize();
    $("#myModal .modal-content").getNiceScroll().show();
}
//@ sourceURL=avisoatributoimportadoMT.js