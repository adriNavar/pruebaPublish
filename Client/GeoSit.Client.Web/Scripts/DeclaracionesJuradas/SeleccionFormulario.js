var seleccionFormulario = {
    versiones: [],
    container: $("#modalSeleccionFormulario"),
    init: function () {
        seleccionFormulario.versiones = $.parseJSON($('#Versiones', seleccionFormulario.container).val());

        $("#FormularioSeleccionado", seleccionFormulario.container).change(function () {
            $('#btnCheck', seleccionFormulario.container).addClass('boton-deshabilitado');

            if ($("#FormularioSeleccionado", seleccionFormulario.container).val()) {
                var versiones = _.filter(seleccionFormulario.versiones, function (e) {
                    return e.TipoDeclaracionJurada === $("#FormularioSeleccionado option:selected").text();
                });

                if (versiones.length > 1) {
                    $('#btnCheck', seleccionFormulario.container).addClass('boton-deshabilitado');

                    $("#VersionSeleccionada", seleccionFormulario.container).empty();
                    $("#VersionSeleccionada", seleccionFormulario.container).append("<option value>Seleccionar Versión..</option>");

                    for (let v in versiones) {
                        if (versiones[v].FechaBaja === null || versiones[v].UsuarioBaja === null) {
                            $("#VersionSeleccionada", seleccionFormulario.container).append(`<option value='${versiones[v].IdVersion}'>${versiones[v].VersionDeclaracionJurada}</option>`);
                        } else {
                            declaracionesJuradas.removeItemFromArr(versiones, versiones[v]);
                        }
                    }
                    if (versiones.length > 1) {
                        $('#rowVersion').show();
                    } else if (versiones.length === 1) {
                        $('#IdVersion', seleccionFormulario.container).val($("#FormularioSeleccionado").val());
                        $('#rowVersion', seleccionFormulario.container).hide();
                        $('#btnCheck', seleccionFormulario.container).removeClass('boton-deshabilitado');
                    }

                }
                else {
                    $('#IdVersion', seleccionFormulario.container).val($("#FormularioSeleccionado").val());
                    $('#rowVersion', seleccionFormulario.container).hide();
                    $('#btnCheck', seleccionFormulario.container).removeClass('boton-deshabilitado');
                }
            }
        });

        $("#VersionSeleccionada", seleccionFormulario.container).change(function () {
            if ($("#VersionSeleccionada", seleccionFormulario.container).val()) {
                $('#btnCheck', seleccionFormulario.container).removeClass('boton-deshabilitado');
                $('#IdVersion', seleccionFormulario.container).val($("#VersionSeleccionada", seleccionFormulario.container).val());
            }
            else {
                $('#btnCheck', seleccionFormulario.container).addClass('boton-deshabilitado');
            }
        });

        $('#btnCheck', seleccionFormulario.container).off('click').on('click', function () {
            showLoading();
            const id = $('#IdUnidadTributaria', seleccionFormulario.container).val(),
                version = $('#IdVersion', seleccionFormulario.container).val();
            $.post(`${BASE_URL}DeclaracionesJuradas/ValidarConsistencia`, { id, version })
                .done(data => {
                    if (data.error) {
                        declaracionesJuradas.mostrarMensajeError(data.message, "DDJJ - Datos Incompletos", true);
                        return;
                    }

                    let doSubmit = Promise.resolve();
                    if (data.message) {
                        doSubmit = new Promise(ok => {
                            declaracionesJuradas.mostrarMensajeError([data.message, ...["<br/>", "Puede continuar pero corrija esta situación."]], "DDJJ - Datos Incompletos", false, ok);
                        });
                    }
                    doSubmit
                        .then(() => {
                            showLoading();
                            $('#formSeleccionFormulario', seleccionFormulario.container).submit();
                        });

                }).always(hideLoading);

        });
    },
    onSuccess: function (data) {
        $('#modalSeleccionFormulario').modal('hide');
        $('#btnCheck', seleccionFormulario.container).addClass('boton-deshabilitado');
        $("#VersionSeleccionada", seleccionFormulario.container).empty();
        $('#rowVersion', seleccionFormulario.container).hide();
        $("#FormularioSeleccionado", seleccionFormulario.container).prop('selectedIndex', 0);
        $("#contenido-formulario").html(data);
        hideLoading();
    },
    onFailure: function (xhr) {
        var response = xhr.responseJSON;
        window.location.href = response.RedirectUrl;
    }
};