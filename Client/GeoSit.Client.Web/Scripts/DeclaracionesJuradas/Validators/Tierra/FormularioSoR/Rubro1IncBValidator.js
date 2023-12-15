var Rubro1IncBFormSoRValidator = function (containers) {
    const form = $("#Rubro1IncB", containers.section);
    function buscar(tipo, titulo, idPadre) {
        return new Promise(function (resolve) {
            var data = { tipos: tipo, multiSelect: false, verAgregar: false, titulo: titulo, campos: ['Nombre'], filters: ["idpadre=" + idPadre], includeSearch: true };
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
            $("#btnBuscarParajes", containers.section).click(function () {
                buscar('parajes', 'Buscar Parajes/Islas', $("#DDJJDesignacion_IdDepartamento", containers.formularioDDJJ).val())
                    .then(function (seleccion) {
                        if (seleccion.length) {
                            $("#DDJJDesignacion_Paraje", containers.section).val(seleccion[0]);
                            $("#DDJJDesignacion_IdParaje", containers.section).val(seleccion[1]);
                            form.data("bootstrapValidator")
                                .updateStatus("DDJJDesignacion.Paraje", "NOT_VALIDATED", null)
                                .validateField("DDJJDesignacion.Paraje");
                        } else {
                            controller.mostrarAdvertencia('Buscar Parajes/Islas', 'No se ha seleccionado ningún paraje.');
                            return;
                        }
                    })
                    .catch(function (err) {
                        console.log(err);
                    });
            });


            $("#btnBuscarSecciones", containers.section).click(function () {
                buscar('secciones', 'Buscar Secciones', $("#DDJJDesignacion_IdDepartamento", containers.formularioDDJJ).val())
                    .then(function (seleccion) {
                        if (seleccion.length) {
                            $("#DDJJDesignacion_Seccion", containers.section).val(seleccion[0]);
                            $("#DDJJDesignacion_IdSeccion", containers.section).val(seleccion[1]);
                            form.data("bootstrapValidator")
                                .updateStatus("DDJJDesignacion.Seccion", "NOT_VALIDATED", null)
                                .validateField("DDJJDesignacion.Seccion");
                        } else {
                            controller.mostrarAdvertencia('Buscar Secciones', 'No se ha seleccionado ninguna sección.');
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
                    'DDJJDesignacion.Paraje': {
                        validators: {
                            notEmpty: {
                                message: "El campo es obligatorio"
                            }
                        }
                    },
                    'DDJJDesignacion.Seccion': {
                        validators: {
                            notEmpty: {
                                message: "El campo es obligatorio"
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
                    'DDJJDesignacion.Quinta': {
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