$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-AperturaLibroDiario').one('shown.bs.modal', function () {
    hideLoading();
});

function init() {
    var libroDiarioAbierto = $("#hdnLibroDiarioAbierto").val();
    if (libroDiarioAbierto == '0') {
        $('#modal-AperturaLibroDiario').modal("show");
    }
    else {
        hideLoading();
        mostrarMensaje("Apertura de Libro Diario", "El Libro Diario ya está abierto.", 3);
    } 
}

$("#btnAbrirLibroDiario").click(function () {
    var fechaLibro = $('#txtFechaLibro').val();
    if (!fechaLibro) {
        mostrarMensaje("Apertura de Libro Diario", "Debe ingresar la fecha del libro diario", 2);
        return false;
    }
    showLoading();
    $.ajax({
        type: 'POST',
        url: BASE_URL + 'mesaentradas/abrirlibrodiario',
        data: { fechaLibro: fechaLibro },
        success: function (data) {
            if (data.success) {
                mostrarMensaje("Apertura de Libro Diario", "El libro diario se abrió correctamente", 2);
                $('#modal-AperturaLibroDiario').modal("hide");
            }
            else {
                mostrarMensaje("Apertura de Libro Diario", "No se pudo abrir el libro diario.", 3);
            }
        },
        error: function (err) {
            mostrarMensaje("Apertura de Libro Diario", "No se pudo abrir el libro diario", 3);
        },
        complete: hideLoading
    });
});

function ajustarmodal() {
    var altura = $(window).height() - 550;
    $(".aperturaLibroDiario-body").css({ "height": altura });
    $('.aperturalibro-content').css({ "height": altura, "overflow": "hidden" });
}

function mostrarMensaje(title, description, tipo) {
    var modal = $('#mensajeModalAperturaLibroDiario');
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
}

//# sourceURL=aperturaLibroDiario.js
