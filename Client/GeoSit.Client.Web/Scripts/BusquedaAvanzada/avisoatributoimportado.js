$('#myModalBA').on('shown.bs.modal', function (e) {
    hideLoading();
});
$(document).ready(function () {
    $("#formVolver").ajaxForm({
        success: function (data) {
            $('#myModalBA').modal('hide');
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
            $('#myModalBA').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $("#contenido").html(data);
            hideLoading();
        },
        error: function () {
            alert("error en siguiente");
        }
    });

    $('#myModalBA').on('shown.bs.modal', function (e) {
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
    $(".modal-content").css({ "height": altura, "overflow": "hidden" });
    $(".modal-content").css({ "width": ancho });
    $(".modal-dialog").css({ "height": altura });
    $(".modal-dialog").css({ "width": ancho });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $("#myModalBA .modal-content").getNiceScroll().resize();
    $("#myModalBA .modal-content").getNiceScroll().show();
}