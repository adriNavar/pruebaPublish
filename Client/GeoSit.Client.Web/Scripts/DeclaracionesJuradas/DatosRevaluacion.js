var datosRevaluacion = {
    ajustarmodal: function () {
        var altura = $(window).height() - 210;
        $(".revaluacion-body").css({ "max-height": altura });
        $(".revaluacion-content").css({ "max-height": altura, "overflow": "hidden" });
        datosRevaluacion.ajustarScrollBars();
    },
    ajustarScrollBars: function () {
        $('#revaluacion-content').getNiceScroll().resize();
        $('#revaluacion-content').getNiceScroll().show();
    },
    validate: function () {
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();
        return bootstrapValidator.isValid();
    },
    init: function () {
        const form = $("#formDatosValuacion", "#modalValuacion");
        $(window).resize(datosRevaluacion.ajustarmodal);
        $("#revaluacion-content").niceScroll(getNiceScrollConfig());
        $('#btnValuar').on('click', function () {
            declaracionesJuradas.mostrarMensajeGeneral("¿Desea generar una revaluación del inmueble?", "Confirmación", true, function () {
                showLoading();
                $('#btnValuar').addClass('boton-deshabilitado');
                const idUnidadTributaria = $('#IdUnidadTributaria').val();
                $.ajax({
                    url: `${BASE_URL}DeclaracionesJuradas/Revaluar`,
                    data: { idUnidadTributaria: idUnidadTributaria },
                    dataType: 'json',
                    type: 'POST',
                    success: function (data) {
                        if (data.success) {
                            $('#modalRevaluacion').modal('hide');
                            declaracionesJuradas.reloadDataValuacion(idUnidadTributaria);
                            declaracionesJuradas.mostrarMensajeGeneral("La revaluación del inmueble se ha generado correctamente.", "Revaluación");
                            window.dispatchEvent(new CustomEvent("nueva-valuacion"));
                        }
                        else {
                            declaracionesJuradas.mostrarMensajeError("Se ha producido un error en la revaluación. Por favor, verifique los datos del inmueble.", "Revaluación", true);
                            $('#btnValuar').removeClass('boton-deshabilitado');
                        }

                    },
                    error: function (err) {
                        declaracionesJuradas.mostrarMensajeError(err.responseText, "Revaluación", true);
                        $('#btnValuar').removeClass('boton-deshabilitado');
                    },
                    complete: hideLoading
                });

            });
        });
    }
};