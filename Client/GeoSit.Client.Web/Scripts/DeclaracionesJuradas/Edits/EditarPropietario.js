var EditarPropietario = function (containers) {
    const modalContainer = "#modalEditarPropietario";
    function ajustarmodal() {
        var altura = $(window).height() - 210;
        $(".titular-body", modalContainer).css({ "max-height": altura });
        $(".titular-content", modalContainer).css({ "max-height": altura, "overflow": "hidden" });
        ajustarScrollBars();
    }
    function ajustarScrollBars() {
        $('#titular-content', modalContainer).getNiceScroll().resize();
        $('#titular-content', modalContainer).getNiceScroll().show();
    }
    function buscarPersonas() {
        return new Promise(function (resolve) {
            var data = { tipos: 'personas', multiSelect: false, verAgregar: true, titulo: 'Buscar Titular', campos: ['Nombre', 'dni:DNI'], includeSearch: false };
            $(containers.buscador).load(BASE_URL + "BuscadorGenerico", data, function () {
                $(".modal", this).one('hidden.bs.modal', function () {
                    $(window).off('seleccionAceptada');
                    $(window).off('agregarObjetoBuscado');
                    $(containers.buscador).empty();
                });
                $(window).one("seleccionAceptada", function (evt) {
                    if (evt.seleccion) {
                        resolve(evt.seleccion[2]);
                    } else {
                        resolve();
                    }
                });
                $(window).one("agregarObjetoBuscado", function () {
                    showLoading();
                    $(containers.buscador).load(BASE_URL + "Persona/BuscadorPersona", function () {
                        $(".modal.mainwnd", this).one('hidden.bs.modal', function () {
                            $(window).off('personaAgregada');
                            $(containers.buscador).empty();
                        });
                        $(window).one("personaAgregada", function (evt) {
                            resolve(evt.persona.PersonaId);
                        });
                    });
                });
            });
        });
    }
    function personaSearch() {
        buscarPersonas()
            .then(function (seleccion) {
                if (seleccion) {
                    $.post(BASE_URL + "Persona/GetDatosPersonaJson/" + seleccion, (persona) => {
                        const form = $("#titular-form", modalContainer);
                        $("#NombreCompleto", form).val(persona.NombreCompleto);
                        $("#IdPersona", form).val(persona.PersonaId);
                        $("#TipoNoDocumento", form).val(persona.NroDocumento || "-");

                        form.formValidation("revalidateField", "IdPersona");
                        form.formValidation("revalidateField", "NombreCompleto");
                    });
                } else {
                    mostrarMensaje('Buscar Titular', 'No se ha seleccionado ningún titular.');
                }
            })
            .catch(function (err) {
                console.log(err);
            });
    }

    return {
        init: function () {
            $(window).resize(ajustarmodal);
            $("#titular-content", modalContainer).niceScroll(getNiceScrollConfig());
            $("#titular-form", modalContainer).bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {
                    NombreCompleto: {
                        validators: {
                            notEmpty: {
                                message: "El campo Persona es obligatorio"
                            }
                        }
                    },
                    PorcientoCopropiedad: {
                        validators: {
                            notEmpty: {
                                message: "El campo % de Copropiedad es obligatorio"
                            },
                            numeric: {
                                message: "El campo % de Copropiedad debe ser numérico"
                            },
                            greaterThan: {
                                inclusive: false,
                                message: "El campo % de Copropiedad debe ser mayor a cero",
                                value: 0
                            },
                            callback: {
                                message: "El campo % de Copropiedad supera al 100%",
                                callback: function (value) {
                                    if (parseFloat(value) > 100) {
                                        return false;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            });
            $("#btn-persona-search", modalContainer).off("click").on("click", personaSearch);
            $(modalContainer).one("shown.bs.modal", ajustarmodal);

            $('#btnGuardarPropietario', modalContainer).on('click', function () {

                var form = $("#titular-form", modalContainer);
                var bootstrapValidator = form.data("bootstrapValidator");
                bootstrapValidator.validate();
                if (bootstrapValidator.isValid()) {
                    $(document).trigger({
                        type: 'propietarioGuardado',
                        propietario: {
                            IdDominioTitular: Number($('#IdDominioTitular', form).val()),
                            IdDominio: Number($('#IdDominio', form).val()),
                            IdTipoTitularidad: Number($('#IdTipoTitularidad', form).val()),
                            TipoTitularidad: $('#IdTipoTitularidad option:selected', form).text(),
                            IdPersona: Number($('#IdPersona', form).val()),
                            TipoNoDocumento: $('#TipoNoDocumento', form).val(),
                            NombreCompleto: $('#NombreCompleto', form).val(),
                            PorcientoCopropiedad: Number($('#PorcientoCopropiedad', form).val())
                        }
                    });

                    $(modalContainer).modal("hide");
                }
            });
            $(modalContainer).modal("show");
        }
    };
};