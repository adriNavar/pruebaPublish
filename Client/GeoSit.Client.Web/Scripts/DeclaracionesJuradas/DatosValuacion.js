var datosValuacion = {
    validate: function () {

        var form = $("#formDatosValuacion");
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();
        return bootstrapValidator.isValid();

    },
    onSuccess: function (data) {
        hideLoading();
        if (data.success) {
            valuaciones.mostrarMensajeGeneral('La valuación se ha guardado con éxito', 'Guardar Valuación', false, null);
            $('#modalValuacion').modal('hide');
            valuaciones.getValuaciones();
            valuaciones.getValuacionesHeader();
            window.dispatchEvent(new CustomEvent("nueva-valuacion"));
            return true;
        }
        else {
            valuaciones.mostrarMensajeError('Ha ocurrido un error al guardar la valuación: ' + (data.message === undefined ? 'Error Interno' : data.message), 'Guardar Formulario', false, null);
            return false;
        }
    },
    onFailure: function (xhr) {
        hideLoading();
        response = xhr.responseJSON;
        window.location.href = response.RedirectUrl;
    },
    init: function () {

        $('.closeValuacion').on('click', function () {
            if ($('#valuacion-content #ReadOnly').val() === "true") {
                $('#modalValuacion').modal("hide");
            }
            else {
                valuaciones.mostrarMensajeGeneral("Si sale de la valuación perderá los datos no guardados, ¿desea continuar?", "Valuación", true, function () {
                    $('#modalValuacion').modal("hide");
                });
            }
        });

        $('#btnGuardarValuacion').on('click', function () {
            if (datosValuacion.validate()) {
                $('#formDatosValuacion').submit();
            }
        });

        $('#modalValuacion #ValorTierra').on('change', function () {
            var valorTierra = parseFloat($('#modalValuacion #ValorTierra').val()) || 0;
            var valorMejoras = parseFloat($('#modalValuacion #ValorMejoras').val()) || 0;

            var formatter = new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD',
            });

            $('#modalValuacion #ValorFiscalTotal').val(formatter.format(valorTierra + valorMejoras));
        });

        $("#exportarDDJJU").click(function () {
            $.ajax({
                url: BASE_URL + 'Valuacion/GenerarDDJJU?idDeclaracionJurada=' + $("#IdDeclaracionJurada").val() + '&idTramite=' + idtramite,
                type: 'POST',
                success: function () {
                    window.open(BASE_URL + "Valuacion/AbrirReporte6");
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                },
                complete: hideLoading
            });
        });


        $("#exportarDDJJSoR").click(function () {
            $.ajax({
                url: BASE_URL + 'Valuacion/GenerarDDJJSoR?idDeclaracionJurada=' + $("#IdDeclaracionJurada").val() + '&idTramite=' + idtramite,
                type: 'POST',
                success: function () {
                    window.open(BASE_URL + "Valuacion/AbrirReporte7");
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                },
                complete: hideLoading
            });
        });

        $("#exportarE1E2").click(function () {
            $.ajax({
                url: BASE_URL + 'Valuacion/GenerarE1E2?idDeclaracionJurada=' + $("#IdDeclaracionJurada").val() + '&idTramite=' + idtramite,
                type: 'POST',
                success: function () {
                    window.open(BASE_URL + "Valuacion/AbrirReporte8");
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                },
                complete: hideLoading
            });
        });

        $('#modalValuacion #ValorMejoras').on('change', function () {
            var valorTierra = parseFloat($('#modalValuacion #ValorTierra').val()) ?? 0;
            var valorMejoras = parseFloat($('#modalValuacion #ValorMejoras').val()) ?? 0;

            if (isNaN(valorTierra))
                valorTierra = 0;

            if (isNaN(valorMejoras))
                valorMejoras = 0;

            var formatter = new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD',
            });

            $('#modalValuacion #ValorFiscalTotal').val(formatter.format(valorTierra + valorMejoras));
        });

        if ($('#valuacion-content #ReadOnly').val() === "true") {
            $(function () {
                $('#valuacion-content').find('input, select, textarea').attr('disabled', 'disabled');
                $('#valuacion-content').find('span').addClass('disabled');
            });
        }

        $("#formDatosValuacion").bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            fields: {
                'Fecha': {
                    validators: {
                        notEmpty: {
                            message: "El campo es obligatorio"
                        },
                    }
                },
                'ValorTierra': {
                    validators: {
                        numeric: {
                            message: "El campo debe ser un número"
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                    }
                },
                'ValorMejoras': {
                    validators: {
                        numeric: {
                            message: "El campo debe ser un número"
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                    }
                },
                'DecretoAplicado': {
                    validators: {
                        integer: {
                            message: "El campo debe ser un número"
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                    }
                },
                'Tramite': {
                    validators: {
                        integer: {
                            message: "El campo debe ser un número"
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                    }
                },
                'Superficie': {
                    validators: {
                        notEmpty: {
                            message: "El campo es obligatorio"
                        },
                        numeric: {
                            message: "El campo debe ser un número"
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                    }
                },
            }
        });

        $("#Fecha").datepicker(getDatePickerConfig({ defaultDate: new Date() }))
            .change(function () {
                const validador = $('#formDatosValuacion').data("bootstrapValidator");
                var datos = $("#Fecha").val();
                if (datos) {
                    validador.updateStatus("Fecha", "VALID");
                }
            });
    }
};