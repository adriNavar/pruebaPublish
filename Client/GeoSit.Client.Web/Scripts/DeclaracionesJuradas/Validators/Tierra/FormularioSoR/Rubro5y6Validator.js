var Rubro5y6FormSoRValidator = function (cantidadAptitudes, containers) {
    const form = $("#Rubro5y6", containers.section);
    let _controller;

    function validateAptitudes() {
        var errorMessageSuperficie = false;
        var errorMessageCaracteristicas = false;
        var existeCaracteriticas = false;

        for (var i = 0; i < cantidadAptitudes; i++) {
            const inputs = ["Relieves", "Espesores", "Colores", "Aguas", "Capacidades", "Estados"].map(elem => $(`#${elem}_${i}`, form)).filter(input => $(input).is(":visible"));
            const superficie = $(`#Superficie_${i}`, form);
            if (inputs.some(input => input.val()) && Number(superficie.val()) === 0) {
                errorMessageSuperficie = true;
                superficie.parent().removeClass('has-success').addClass('has-error');
            }
            //input.val() == false devuelve true cuando => !input.val() && input.val() != null
            if (Number(superficie.val()) > 0 && inputs.some(input => !input.val())) {
                errorMessageCaracteristicas = true;
            }

            existeCaracteriticas |= (Number(superficie.val()) > 0 && !inputs.some(input => !input.val()));

        }
        let mensajes = [];
        if (errorMessageCaracteristicas) {
            mensajes = [...mensajes, "Se deben elegir todas las características de las aptitudes con superficie cargada."];
        }
        if (errorMessageSuperficie) {
            mensajes = [...mensajes, "La superficie no puede ser 0 si hay características cargadas para una aptitud."];
        }
        if (!existeCaracteriticas && !errorMessageSuperficie && !errorMessageCaracteristicas) {
            mensajes = ["Debe haber al menos una aptitud cargada."];
        }
        if (mensajes.length) {
            _controller.mostrarAdvertencia("Validación Rubro 5 y 6", `Verifique lo siguiente:<br><br>${mensajes.join('<br>')}`);
        }

        return !errorMessageSuperficie && !errorMessageCaracteristicas && existeCaracteriticas;
    }

    function validateCamino() {
        const distanciaCamino = parseFloat($("#DDJJSor_DistanciaCamino", form).val()),
            idCamino = $("#DDJJSor_IdCamino", form).val();
        let errorMessageCamino = false, msgError;

        if (!isNaN(distanciaCamino)) {
            if (!idCamino) {
                msgError = "Si indica distancia al camino más próximo, debe especificar el camino más próximo.";
                errorMessageCamino = true;
                $("#DDJJSor_IdCamino", form).parent().removeClass('has-success').addClass('has-error');
                $("#DDJJSor_DistanciaCamino", form).parent().removeClass('has-error').addClass('has-success');
            } else {
                $("#DDJJSor_IdCamino", form).parent().removeClass('has-error').addClass('has-success');
            }
        } else if (idCamino) {
            if (isNaN(distanciaCamino)) {
                msgError = "Si indica el camino más próximo, debe especificar la distancia a ese camino.";
                errorMessageCamino = true;
                $("#DDJJSor_DistanciaCamino", form).parent().removeClass('has-success').addClass('has-error');
                $("#DDJJSor_IdCamino", form).parent().removeClass('has-error').addClass('has-success');
            } else {
                $("#DDJJSor_DistanciaCamino", form).parent().removeClass('has-error').addClass('has-success');
            }
        }

        if (errorMessageCamino) {
            _controller.mostrarAdvertencia("Validación Rubro 5 y 6", msgError);
        }
        return !errorMessageCamino;
    }

    function validateLocalidad() {
        const distanciaLocalidad = parseFloat($("#DDJJSor_DistanciaLocalidad", form).val()),
            idLocalidad = $("#DDJJSor_IdLocalidad", form).val();
        let errorMessageLocalidad = false, msgError = "";

        if (!isNaN(distanciaLocalidad)) {
            if (!idLocalidad) {
                msgError = "Si indica distancia a la localidad, debe especificar la localidad.";
                errorMessageLocalidad = true;
                $("#DDJJSor_IdLocalidad", form).parent().removeClass('has-success').addClass('has-error');
                $("#DDJJSor_DistanciaLocalidad", form).parent().removeClass('has-error').addClass('has-success');
            } else {
                $("#DDJJSor_IdLocalidad", form).parent().removeClass('has-error').addClass('has-success');
            }
        } else if (idLocalidad) {
            if (isNaN(distanciaLocalidad)) {
                msgError = "Si indica la localidad, debe especificar la distancia a esa localidad.";
                errorMessageLocalidad = true;
                $("#DDJJSor_DistanciaLocalidad", form).parent().removeClass('has-success').addClass('has-error');
                $("#DDJJSor_IdLocalidad", form).parent().removeClass('has-error').addClass('has-success');
            } else {
                $("#DDJJSor_DistanciaLocalidad", form).parent().removeClass('has-error').addClass('has-success');
            }
        }

        if (errorMessageLocalidad) {
            _controller.mostrarAdvertencia("Validación Rubro 5 y 6", msgError);
        }

        return !errorMessageLocalidad;
    }

    return {
        validate: function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();
            return bootstrapValidator.isValid() && validateAptitudes() && validateCamino() && validateLocalidad();

        },
        init: function init(controller) {
            _controller = controller;
            $("#Rubro5y6").bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {
                    superficie: {
                        selector: '.superficie',
                        container: '.superficieError',
                        validators: {
                            numeric: {
                                message: 'La superficie ser un número'
                            }
                        }
                    },
                    'DDJJSor.DistanciaEmbarque': {
                        validators: {
                            integer: {
                                message: 'El campo debe ser un número entero'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            },
                            callback: {
                                message: 'El campo es requerido',
                                callback: function (value) {
                                    var caracteristica = _.find($.parseJSON($('#sorOtrasCarJSON').val()), function (e) { return e.OtrasCarRequerida.toLowerCase() === "a lugar de embarque"; });
                                    return !caracteristica || !caracteristica.Requerido || caracteristica.Requerido === 1 && !isNaN(parseInt(value));
                                }
                            }
                        }
                    },
                    'DDJJSor.DistanciaCamino': {
                        validators: {
                            integer: {
                                message: 'El campo debe ser un número entero'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            },
                            callback: {
                                message: 'El campo es requerido',
                                callback: function (value) {
                                    var caracteristica = _.find($.parseJSON($('#sorOtrasCarJSON').val()), function (e) { return e.OtrasCarRequerida.toLowerCase() === "a camino mas proximo"; });
                                    return !caracteristica || !caracteristica.Requerido || caracteristica.Requerido === 1 && !isNaN(parseInt(value));
                                }
                            }
                        }
                    },
                    'DDJJSor.IdCamino': {
                        validators: {
                            callback: {
                                message: 'El campo es requerido',
                                callback: function (value) {
                                    var caracteristica = _.find($.parseJSON($('#sorOtrasCarJSON').val()), function (e) { return e.OtrasCarRequerida.toLowerCase() === "a camino mas proximo"; });
                                    return !caracteristica || !caracteristica.Requerido || caracteristica.Requerido === 1 && !isNaN(parseInt(value));
                                }
                            }
                        }
                    },
                    'DDJJSor.DistanciaLocalidad': {
                        validators: {
                            integer: {
                                message: 'El campo debe ser un número entero'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            },
                            callback: {
                                message: 'El campo es requerido',
                                callback: function (value) {
                                    var caracteristica = _.find($.parseJSON($('#sorOtrasCarJSON').val()), function (e) { return e.OtrasCarRequerida.toLowerCase() === "a poblacion mas proxima"; });
                                    return !caracteristica || !caracteristica.Requerido || caracteristica.Requerido === 1 && !isNaN(parseInt(value));
                                }
                            }
                        }
                    },
                    'DDJJSor.IdLocalidad': {
                        validators: {
                            callback: {
                                message: 'El campo es requerido',
                                callback: function (value) {
                                    var caracteristica = _.find($.parseJSON($('#sorOtrasCarJSON').val()), function (e) { return e.OtrasCarRequerida.toLowerCase() === "a poblacion mas proxima"; });
                                    return !caracteristica || !caracteristica.Requerido || caracteristica.Requerido === 1 && !isNaN(parseInt(value));
                                }
                            }
                        }
                    }
                }
            });

            let timeoutLocalidades;
            $("#DDJJSor_DistanciaLocalidad", form).on('input', function (evt) {
                clearTimeout(timeoutLocalidades);
                timeoutLocalidades = setTimeout(filtrarLocalidades, 300, evt);
            });

            function filtrarLocalidades(evt) {
                $("#DDJJSor_IdLocalidad", form).empty();
                if (!evt) {
                    return Promise.resolve();
                }
                return new Promise((ok) => {
                    $.post(BASE_URL + 'DeclaracionesJuradas/GetLocalidadesByDistancia', { DistanciaLocalidad: evt.target.value }, function (localidades) {
                        console.log(localidades);
                        const items = localidades.reduce((accum, loc) => accum + `<option value='${loc.Value}'>${loc.Text}</option>`, "");
                        $("#DDJJSor_IdLocalidad", form).append("<option value>- Seleccione -</option>");
                        $("#DDJJSor_IdLocalidad", form).append(items);

                    }).done(ok);
                });
            }

            $('.superficie', form).keypress(function (e) {
                if (e.key === ',') {
                    e.preventDefault();
                    $(this).val($(this).val() + '.');

                    form.data("bootstrapValidator")
                        .updateStatus($(this), "NOT_VALIDATED", null)
                        .validateField($(this));
                }
            });
            $('.superficie', form).on('change', function () {
                $('#SuperficieTotal').val(Number(_.sumBy($('.superficie', form), function (o) { return parseFloat(o.value.replace(',', '.')); })).toFixed(4));
            });
            $('.customDropdown', form).on('change', function () {
                if ($(this).val()) {
                    $(this).addClass('select-with-value');
                }
                else {
                    $(this).removeClass('select-with-value');
                }
            });
            $('.customDropdown option:selected[value!=""]', form).parent().addClass('select-with-value');

            setTimeout(_controller.ajustarScrollBars, 100);
        }
    };
};
//# sourceURL=R5y6.js