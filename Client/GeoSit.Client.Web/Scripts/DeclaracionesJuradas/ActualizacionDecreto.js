var actualizacionDecreto = {
    interval: null,
    ajustarmodal: function () {
        var altura = $(window).height() - 210;
        $(".actualizacionDecreto-body").css({ "max-height": altura });
        $(".actualizacionDecreto-content").css({ "max-height": altura, "overflow": "hidden" });
        actualizacionDecreto.ajustarScrollBars();
    },
    ajustarScrollBars: function () {
        $('#actualizacionDecreto-content').getNiceScroll().resize();
        $('#actualizacionDecreto-content').getNiceScroll().show();
    },
    mostrarMensaje: function (mensaje, titulo, tipo, callback) {
        $('#TituloInfoActualizacionDecreto', '#ModalInfoActualizacionDecreto').html(titulo);
        $('#DescripcionInfoActualizacionDecreto', '#ModalInfoActualizacionDecreto').html(mensaje);
        $("[role='alert']", "#ModalInfoActualizacionDecreto").removeClass("alert-danger alert-success alert-info alert-warning").addClass(tipo);
        $(".modal-footer", "#ModalInfoActualizacionDecreto").hide();
        if (tipo === "alert-info") {
            $(".modal-footer", "#ModalInfoActualizacionDecreto").show();
        }

        if ($.isFunction(callback)) {
            $('#btnInfoActualizacionDecretoOK').off('click').on('click', callback);
        }
        else {
            $('#btnInfoActualizacionDecretoOK').unbind('click');
        }

        $("#ModalInfoActualizacionDecreto").modal('show');
    },
    mostrarMensajeError: function (mensajes, titulo, error, callback) {
        actualizacionDecreto.mostrarMensaje(mensajes, titulo, (error || false ? "alert-danger" : "alert-warning"), callback);
    },
    mostrarMensajeGeneral: function (mensajes, titulo, confirmacion, callback) {
        actualizacionDecreto.mostrarMensaje(mensajes, titulo, (!!confirmacion ? "alert-info" : "alert-success"), callback);
    },
    getStatus: function () {
        $.ajax({
            url: BASE_URL + 'DeclaracionesJuradas/GetAplicarDecretoStatus',
            dataType: 'json',
            type: 'GET',
            success: function (result) {
                if (result.success) {
                    $('#progresoAplicacion').text(result.message);
                    var progress = result.message.split(' / ');
                    var valeur = parseInt(parseInt(progress[0]) * 100 / parseInt(progress[1]));
                    $('.progress-bar').css('width', valeur + '%').attr('aria-valuenow', valeur);
                    $('.progress-bar').text(valeur + '%');
                    if (progress[0] == progress[1]) {
                        clearInterval(actualizacionDecreto.interval);
                    }
                }
                else
                    actualizacionDecreto.mostrarMensajeError("Ha ocurrido un error interno al obtener el estado del proceso. Por favor contacte al Administrador", "Actualización de decreto", true);
            },
            error: function (error) {
                actualizacionDecreto.mostrarMensajeError(error.responseText, "Actualización de decreto", true);
            }
        });
    },
    init: function () {
        $(window).resize(actualizacionDecreto.ajustarmodal);
        $("#actualizacionDecreto-content").niceScroll(getNiceScrollConfig());

        $('#modalActualizacionDecreto').one('shown.bs.modal', hideLoading);

        $('#modalActualizacionDecreto').on('hidden.bs.modal', function (e) {
            clearInterval(actualizacionDecreto.interval);
        });

        $("#modalActualizacionDecreto").modal("show");  

        if ($('#IsRunning').val() == 'True') {
            actualizacionDecreto.getStatus();
            actualizacionDecreto.interval = setInterval(actualizacionDecreto.getStatus, 30000);
        }

        $('#btnAplicarDecreto').on('click', function () {
            if (!$('#idDecreto').val()) {
                actualizacionDecreto.mostrarMensajeError('Por favor, seleccione un decreto', "Actualización de decreto", true);
                return;
            }

            actualizacionDecreto.mostrarMensajeGeneral("¿Desea aplicar el decreto seleccionado?", "Confirmación", true, function () {
                $('#btnAplicarDecreto').addClass('boton-deshabilitado');
                showLoading();
                $('#progressBar').show();
                actualizacionDecreto.interval = setInterval(actualizacionDecreto.getStatus, 30000);

                $.ajax({
                    url: BASE_URL + 'DeclaracionesJuradas/AplicarDecreto',
                    data: { idDecreto: $('#idDecreto').val() },
                    dataType: 'json',
                    type: 'POST',
                    timeout: 0,
                    success: function (result) {
                        if (result.success) {
                            $('#urbanas').val(result.data.parcelasUrbanasCount);
                            $('#suburbanas').val(result.data.parcelasSuburbanasCount);
                            $('#rurales').val(result.data.parcelasRuralesCount);
                        } else {
                            actualizacionDecreto.mostrarMensajeError("Ha ocurrido un error interno al aplicar el decreto. Por favor contacte al Administrador", "Actualización de decreto", true);
                        }
                    },
                    error: function (error) {
                        actualizacionDecreto.mostrarMensajeError(error.responseText, "Actualización de decreto", true);
                    },
                    complete: hideLoading
                });
            });
        });
    },
}