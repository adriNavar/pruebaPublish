$(document).ready(init);
$(window).resize(ajustarmodalEditarTablasAuxiliares);

var idComponente, idElemento;

$('#modal-window-editar-tabla-auxiliar').on('shown.bs.modal', function (e) {
    ajustarScrollBarsEditarTablasAuxiliares();
    hideLoading();
});

function init() {

    var container = $('#modal-window-editar-tabla-auxiliar');
    ///////////////////// Scrollbars ////////////////////////
    $("#editar-tabla-auxiliar-datos").niceScroll(getNiceScrollConfig());

    ajustarmodalEditarTablasAuxiliares();

    container.find('input[required],select[required],textarea[required]').on('keyup', function () {
        $(this).removeClass('alert-danger');
    });

    container.find('#btnGuardar').click(function () {
        container.find('form').submit();
    });

    container.find('#btnCerrar').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        confirmModal("Tablas Auxiliares", "¿Seguro que desea cancelar? Se perderán los datos no guardados.", function () {
            $('#modal-window-editar-tabla-auxiliar').modal('hide');
        });
    });

    container.find('.fecha').datetimepicker({
        useCurrent: false,
        format: 'DD/MM/YYYY HH:mm:ss'
    });

    $.each(container.find('[data-numeric]'), function () {
        var el = $(this);
        var decimal = el.data('numeric-decimal-places');
        var decimalPlaces = el.data('numeric-decimal-places');
        var data = {};
        if (decimal) {
            data = { decimalPlaces: decimalPlaces };
        } else {
            data = { decimal: false };
        }
        el.numeric(data);
    });

    container.find('form').on('submit', function (e) {
        e.stopPropagation();
        e.preventDefault();
        var form = $(this);
        $.each($(this).find('[data-checkbox-target]'), function () {
            var checkbox = $(this);
            var target = checkbox.data('checkbox-target');
            form.find('[name="' + target + '"]').val(checkbox.prop('checked') ? 1 : 0);
        });

        var data = $(this).serialize();

        if ($(this).isValid()) {

            confirmModal("Tablas Auxiliares", "Confirma que desea guardar los datos introducidos?", function () {
                showLoading();
                $.ajax({
                    type: "POST",
                    url: BASE_URL + "Mantenimiento/GuardarDatos",
                    data: data,
                    success: function (response) {
                        if (response.success) {
                            $('#modal-window-editar-tabla-auxiliar').modal('hide');
                            mostrarMensaje(true, "Tablas Auxiliares", "Los cambios se guardaron correctamente");
                            if (typeof registroActualizado === "function") {
                                registroActualizado();
                            }
                        } else {
                            mostrarMensaje(false, "Tablas Auxiliares", "Se produjo un error al guardar el registro");
                        }
                    },
                    error: function () {
                        mostrarMensaje(false, "Tablas Auxiliares", "Se produjo un error al guardar el registro");
                    },
                    complete: hideLoading
                });
            });
        }

    });

    $("#modal-window-editar-tabla-auxiliar").modal("show");

}


function ajustarmodalEditarTablasAuxiliares() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter;

    $(".editar-tabla-auxiliar-body", "#scroll-content-editar-tabla-auxiliar").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBarsEditarTablasAuxiliares();
}
function ajustarScrollBarsEditarTablasAuxiliares() {
    temp = $(".editar-tabla-auxiliar-body").height();
    $('#editar-tabla-auxiliar-datos').css({ "max-height": temp + 'px' });
    $("#editar-tabla-auxiliar-datos").getNiceScroll().resize();
    $("#editar-tabla-auxiliar-datos").getNiceScroll().show();
}

$.fn.isValid = function () {
    var valid = true;
    var inputs = $(this).find('input[required],select[required],textarea[required]');
    inputs.removeClass('alert-danger');
    $.each(inputs, function () {
        if (!$(this).val()) {
            valid = false;
            $(this).addClass('alert-danger');
        }
    });
    if (!valid) {
        mostrarMensaje(false, "Tablas Auxiliares", "Debe rellenar todos los campos obligatorios");
    }
    return valid;
}