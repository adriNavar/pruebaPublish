$(document).ready(init);
$('#modal-window-informe-actas-vencidas').one('shown.bs.modal', hideLoading);

function init() {
    $('.soloFecha').datepicker(getDatePickerConfig({ minView: 2, startView: 4 }));

    $('#modal-window-informe-actas-vencidas').modal('show');


    $("#aceptar-informe-actas-vencidas").click(function () {
        if (!$('#FechaPlazo').val()) {
            alerta('Advertencia', 'Debe ingresar una Fecha Válida.', 2);
        } else {
            var formData = {
                "fecha": $("#FechaPlazo").val()
            };
            showLoading();
            $.ajax({
                url: BASE_URL + 'Actas/GenerarInformeActasVencidas',//aca llamar al getActasVencidas...
                type: 'POST',
                data: formData,
                success: function (data) {
                    if (data.success) {
                        window.open(BASE_URL + "Actas/GetFileInformeActasVencida?file=" + data.file);
                    } else {
                        hideLoading();
                        alerta('Advertencia', data.message, 2);
                    }
                },
                error: function (_, __, errorThrown) {
                    hideLoading();
                    alerta('Error', errorThrown, 3);
                }
            }).always(function () {
                hideLoading();
            });
        }
    });

    $("#cancelar-informe-actas-vencidas").click(function () {
        $('#modal-window-informe-actas-vencidas').modal('toggle');
    });


}

function alerta(titulo, mensaje, tipo) {
    var cls = "";
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }

    $("#MensajeInfoActa").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoActa").html(titulo);
    $("#DescripcionInfoActa").html(mensaje);
    $("#ModalInfoActa").modal('show');
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        paddingBottom = 70,
        headerFooter = 220,
        altura = viewportHeight - headerFooter;
    altura = altura - (altura > headerFooter + paddingBottom ? paddingBottom : 0); //value corresponding to the modal heading + footer
    $(".modal-body", "#modal-window-informe-actas-vencidas").css({ "height": altura, "overflow": "hidden" });

}

