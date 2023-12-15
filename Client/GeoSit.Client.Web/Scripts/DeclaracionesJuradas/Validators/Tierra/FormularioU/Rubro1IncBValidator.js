var Rubro1IncBFormUValidator = function (containers) {
    const form = $("#Rubro1IncB", containers.section);
    function buscar(tipo, titulo, idPadre) {
        return new Promise(function (resolve) {
            var data = { tipos: tipo, multiSelect: false, verAgregar: false, titulo: titulo, campos: ['Nombre'], filters: ["idpadre=" + idPadre], includeSearch: true};
            $(containers.buscador).load(BASE_URL + "BuscadorGenerico", data, function () {
                $(".modal", this).one('hidden.bs.modal', function () {
                    $(window).off('seleccionAceptada');
                    $(containers.buscador).empty();
                });

                $(window).one("seleccionAceptada", function (evt) {
                    if (evt.seleccion) {
                        resolve(evt.seleccion.slice(1));
                    } else {
                        resolve();
                    }
                });

            });

        });

    }
    return {
        validate: function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();
            return bootstrapValidator.isValid();
        },
        init: function (controller) {
            $("#btnBuscarCalles", containers.section).click(function () {
                buscar('calles', 'Buscar Calles', $("#DDJJDesignacion_IdLocalidad", containers.formularioDDJJ).val())
                    .then(function (seleccion) {
                        if (seleccion.length) {
                            $("#DDJJDesignacion_Calle", containers.section).val(seleccion[0]);
                            $("#DDJJDesignacion_IdCalle", containers.section).val(seleccion[1]);
                            form.data("bootstrapValidator")
                                .updateStatus("DDJJDesignacion.Calle", "NOT_VALIDATED", null)
                                .validateField("DDJJDesignacion.Calle");
                        } else {
                            controller.mostrarAdvertencia('Buscar Calles', 'No se ha seleccionado ninguna calle.');
                            return;
                        }
                    })
                    .catch(function (err) {
                        console.log(err);
                    });
            });


            $("#btnBuscarManzanas", containers.section).click(function () {
                buscar('manzanas', 'Buscar Manzanas', $("#DDJJDesignacion_IdLocalidad", containers.formularioDDJJ).val())
                    .then(function (seleccion) {
                        if (seleccion.length) {
                            $("#DDJJDesignacion_Manzana", containers.section).val(seleccion[0]);
                            $("#DDJJDesignacion_IdManzana", containers.section).val(seleccion[1]);
                            form.data("bootstrapValidator")
                                .updateStatus("DDJJDesignacion.Manzana", "NOT_VALIDATED", null)
                                .validateField("DDJJDesignacion.Manzana");
                        } else {
                            controller.mostrarAdvertencia('Buscar Manzanas', 'No se ha seleccionado ninguna manzana.');
                            return;
                        }
                    })
                    .catch(function (err) {
                        console.log(err);
                    });
            });

            form.bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {
                    'DDJJDesignacion.Manzana': {
                        validators: {
                            notEmpty: {
                                message: "El campo es obligatorio"
                            }
                        }
                    },
                    'DDJJDesignacion.Calle': {
                        validators: {
                            notEmpty: {
                                message: "El campo es obligatorio"
                            }
                        }
                    },
                    'DDJJDesignacion.Numero': {
                        validators: {
                            stringLength: {
                                max: 22,
                                message: 'El campo no debe superar los 22 caracteres'
                            }
                        }
                    },
                    'DDJJDesignacion.Chacra': {
                        validators: {
                            stringLength: {
                                max: 22,
                                message: 'El campo no debe superar los 22 caracteres'
                            }
                        }
                    },
                    'DDJJDesignacion.Fraccion': {
                        validators: {
                            stringLength: {
                                max: 22,
                                message: 'El campo no debe superar los 22 caracteres'
                            }
                        }
                    }
                }
            });
        }
    };
};