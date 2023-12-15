var Rubro2FormE1E2Validator = function (containers) {
    const form = $("#Rubro2", containers.section);
    return {
        validate: function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();
            return $("#Mejora_IdDestinoMejora", containers.section).val() === "99" || bootstrapValidator.isValid();
        },
        init: function () {
            form.bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {
                    requiredInteger: {
                        selector: '.requiredInteger',
                        validators: {
                            notEmpty: {
                                message: 'El campo es obligatorio'
                            },
                            integer: {
                                message: 'El campo debe ser un número entero'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            }
                        }
                    },
                    integer: {
                        selector: '.integer',
                        validators: {
                            integer: {
                                message: 'El campo debe ser un número entero'
                            },
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            }
                        }
                    },
                    numeric: {
                        selector: '.numeric',
                        validators: {
                            regexp: {
                                message: 'El campo debe ser un número real',
                                regexp: /^[0-9]+(?:\.[0-9]+)?$/
                            }
                        }
                    },
                    requiredNumeric: {
                        selector: '.requiredNumeric',
                        validators: {
                            regexp: {
                                message: 'El campo es debe ser un número real',
                                regexp: /^[0-9]+(?:\.[0-9]+)?$/
                            },
                            notEmpty: {
                                message: 'El campo es obligatorio'
                            }
                        }
                    },
                    requiredIntegerYear: {
                        selector: '.requiredIntegerYear',
                        validators: {
                            notEmpty: {
                                message: 'El campo es obligatorio'
                            },
                            integer: {
                                message: 'El campo debe ser un número entero'
                            },
                            stringLength: {
                                min: 4,
                                max: 4,
                                message: 'El campo debe tener 4 caracteres'
                            }
                        }
                    },
                    'Mejora.IdEstadoConservacion': {
                        validators: {
                            notEmpty: {
                                message: 'El campo es obligatorio'
                            }
                        }
                    }
                }
            });
        }
    };
};