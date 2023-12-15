$(document).ready(init);
$('#modal-window-informe-tramites').on('shown.bs.modal', function (e) {
    //ajustarScrollBars();
    hideLoading();
});

function init() {
    //ajustarmodal();
    $('#soloFecha').datepicker(getDatePickerConfig({ minView: 2, startView: 4 }));


    $('#modal-window-informe-tramites').modal('show');

    $('#numero').keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $("#aceptar-informe-tramites").click(function () {
        if ($('#numero').val() == '') {
            alerta('Advertencia', 'Debe ingresar un número valido.', 3);

        } else {
            var formData = {
                "Identificador": $('#TipoDeTramite').val(),
                "Numero": $('#numero').val(),
                "Operacion": $('input[name=RadioId]:checked').val()
            };
            showLoading();
            $.ajax({
                url: BASE_URL + 'TramitesCertificados/GenerarInformeEstadoTramite',//aca llamar al getActasVencidas...
                type: 'POST',
                data: formData,
                success: function (data) {
                    if (data.success) {
                        window.open(BASE_URL + 'TramitesCertificados/GetFileInformeEstadoTramite?file=' + data.file);
                    } else {
                        hideLoading();
                        alerta('Advertencia', data.message, 3);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    hideLoading();
                    alerta('Error', errorThrown, 3);
                }
            }).always(function () {
                hideLoading();
            });
        }
    });
}

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlerta = fn;
    $("#botones-modal-info").find("span:last").hide();
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $("#botones-modal-info").find("span:last").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }


    $("#MensajeInfoEstado").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoEstado").html(titulo);
    $("#DescripcionInfoEstado").html(mensaje);
    $("#ModalInfoEstado").modal('show');
    $("#botones-modal-info").find("span:last").show();
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        paddingBottom = 70,
        headerFooter = 220,
        altura = viewportHeight - headerFooter,
        altura = altura - (altura > headerFooter + paddingBottom ? paddingBottom : 0); //value corresponding to the modal heading + footer
    $(".modal-body", "#modal-window-informe-tramites").css({ "height": altura, "overflow": "hidden" });

}

