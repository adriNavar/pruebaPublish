$(window).resize(ajustarmodal);
$(document).ready(init);
$('#modal-CierreLibroDiario').one('shown.bs.modal', function () {
    hideLoading();
});

function init() {
    $("#lblMsgConfirmacion").hide();
    $(".cierrelibro-content").niceScroll(getNiceScrollConfig());
    var libroDiarioAbierto = $("#hdnLibroDiarioAbierto").val();
    if (libroDiarioAbierto == '1') {
        $('#modal-CierreLibroDiario').modal("show");
    }
    else {
        hideLoading();
        mostrarMensaje("Cierre de Libro Diario", "El Libro Diario ya está cerrado.", 3);
    }
};


$("#btnInformePendientesLibroDiario").click(function () {
    showLoading();
    window.open(BASE_URL + 'MesaEntradas/GetInformePendientesConfirmar/', "_blank");
    $("#btnCerrarLibroDiario").removeClass("boton-deshabilitado");
    $("#lblMsgConfirmacion").show();
    hideLoading();
})

function ajustarmodal() {
    var altura = $(window).height() - 550;
    $(".cierreLibroDiario-body").css({ "height": altura });
    $('.cierrelibro-content').css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $(".cierrelibro-content").getNiceScroll().resize();
    $(".cierrelibro-content").getNiceScroll().show();
}

$("#btnCerrarLibroDiario").click(function () {
    var fechaLibro = $('#txtFechaLibro').val();
    if (!fechaLibro) {
        mostrarMensaje("Cierre de Libro Diario", "Debe ingresar la fecha del libro diario", 2);
        return false;
    }
    showLoading();
    $.ajax({
        type: 'POST',
        url: BASE_URL + 'mesaentradas/cerrarlibrodiario',
        data: { fechaLibro: fechaLibro },
        success: function (data) {
            if (data.success) {
                mostrarMensaje("Cierre de Libro Diario", "El libro diario se cerró correctamente", 2);
                $('#modal-CierreLibroDiario').modal("hide");
            }
            else {
                mostrarMensaje("Cierre de Libro Diario", "No se pudo cerrar el libro diario.", 3);
            }
        },
        error: function (err) {
            mostrarMensaje("Cierre de Libro Diario", "No se pudo cerrar el libro diario", 3);
        },
        complete: hideLoading
    });
});

function mostrarMensaje(title, description, tipo) {
    var modal = $('#mensajeModalCierreLibroDiario');
    var alertaBackground = $('div[role="alert"]', modal);
    $('.modal-title', modal).html(title);
    $('.modal-body p', modal).html(description);
    var cls = 'alert-success';
    if (tipo === 2) {
        cls = 'alert-warning';
    } else if (tipo === 3) {
        cls = 'alert-danger';
    }
    alertaBackground.removeClass('alert-warning alert-success alert-danger').addClass(cls);
    modal.modal('show');
};


//# sourceURL=cierreLibroDiario.js
