/*var Rubro1IncEyFFormUValidator = function (containers) {
    const form = $("#Rubro1IncEyF", containers.section);
    return {
        validate: function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();
            return bootstrapValidator.isValid();

        },
        init: function () {
            $('#AguaCorriente', containers.section).change(function () {
                $('#DDJJU_AguaCorriente', containers.section).val(this.checked ? 1 : 0);
            });
            $('#Cloaca', containers.section).change(function () {
                $('#DDJJU_Cloaca', containers.section).val(this.checked ? 1 : 0);
            });
            $('#DDJJU_SuperficiePlano', containers.section).change(function () {
                $("#Rubro1IncEyF", containers.section).data("bootstrapValidator")
                    .updateStatus("DDJJU.SuperficieTitulo", "NOT_VALIDATED", null)
                    .validateField("DDJJU.SuperficieTitulo");
            });
            $('#DDJJU_SuperficieTitulo', containers.section).change(function () {
                $("#Rubro1IncEyF", containers.section).data("bootstrapValidator")
                    .updateStatus("DDJJU.SuperficiePlano", "NOT_VALIDATED", null)
                    .validateField("DDJJU.SuperficiePlano");
            });

            form.bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {
                    'DDJJU.SuperficiePlano': {
                        validators: {
                            numeric: {
                                message: 'El campo debe ser un número'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            },
                            callback: {
                                message: 'Si no indica una Superficie de Título debe especificar una Superficie de Plano y si indica una Superficie de Plano, debe completarla con un valor mayor a cero',
                                callback: function (value) {
                                    return !isNaN(value) && Number(value) > 0 || !value && Number(!!$('#DDJJU_SuperficieTitulo', containers.section).val()) > 0;
                                }
                            }
                        }
                    },
                    'DDJJU.SuperficieTitulo': {
                        validators: {
                            numeric: {
                                message: 'El campo debe ser un número'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            },
                            callback: {
                                message: 'Si indica una Superficie de Título, debe completarla con un valor mayor a cero',
                                callback: function (value) {
                                    return !isNaN(value) && Number(value) > 0 || !value && Number(!!$('#DDJJU_SuperficiePlano', containers.section).val()) > 0;
                                }
                            }
                        }
                    },
                    'DDJJU.NumeroHabitantes': {
                        validators: {
                            integer: {
                                message: 'El campo debe ser un número entero'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            },
                            callback: {
                                message: 'Número de habitantes es obligatorio',
                                callback: function (value) {
                                    var caracteristica = _.find($.parseJSON($('#uOtrasCarJSON', containers.section).val()), function (e) { return e.OtrasCarRequerida.toUpperCase() === 'NUMERO DE HABITANTES'; });
                                    if (caracteristica) {
                                        return caracteristica.Requerido === 1 && value !== '' || !caracteristica.Requerido;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            });
        }
    };
};*/
//# sourceURL=R1EyF.js