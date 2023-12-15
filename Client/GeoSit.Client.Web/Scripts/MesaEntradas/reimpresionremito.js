$(document).ready(init);

$('#modal-ReimpresionRemito').one('shown.bs.modal', function () {
    hideLoading();
});

function init() {
    $('#modal-ReimpresionRemito').modal("show");
}

$("#btnReimprimirRemito").click(function () {
    var numero = $('#txtNumero').val();
    if (!numero) {
        mostrarMensaje("Reimprimir Remito", "Debe ingresar el número de remito", 2);
        return false;
    }
    showLoading();
    $.ajax({
        type: 'POST',
        url: BASE_URL + 'mesaentradas/reimprimirremito',
        data: { numero: numero },
        success: function () {
            window.open(BASE_URL + 'MesaEntradas/AbrirInformeRemito', "_blank");
            //$('#modal-ReimpresionRemito').modal("hide");
        },
        error: function (err) {
            if (err.status === 404) {
                mostrarMensaje("Reimprimir Remito", "No se pudo obtener el informe del remito", 3);
            } else {
                mostrarMensaje("Reimprimir Remito", "No se pudo obtener el remito", 3);
            }
        },
        complete: hideLoading
    });
});

function mostrarMensaje(title, description, tipo) {
    var modal = $('#mensajeModalReimpresionRemito');
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

//# sourceURL=reimprimirRemito.js
