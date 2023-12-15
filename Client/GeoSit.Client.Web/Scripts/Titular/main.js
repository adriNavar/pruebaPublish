$(document).ready(init);
function init() {
    $("#btnGrabar-titular").on('click', function () {
        var form = $("#titular-form");
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();
        if (bootstrapValidator.isValid()) {
            var model = {
                DominioPersonaId: $("#DominioPersonaId", form).val(),
                DominioId: $("#DominioId", form).val(),
                PersonaId: $("#PersonaId", form).val(),
                TipoTitularidadId: $("#TipoTitularidadId :selected", form).val(),
                TipoTitularidad: $("#TipoTitularidadId :selected", form).text(),
                NombreCompleto: $("#NombreCompleto", form).val(),
                TipoNoDocumento: $("#TipoNoDocumento", form).val(),
                PorcientoCopropiedad: $("#PorcientoCopropiedad", form).val(),
                Operacion: $("#Operacion", form).val()
            };

            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/SaveTitular",
                type: 'POST',
                dataType: 'json',
                data: model,
                success: function (titular) {
                    if (titular.error) {
                        errorAlert(titular.mensaje);
                    } else {
                        $(document).trigger({ type: "titularGuardado", titular: titular });
                        $("#modal-window-titular").modal('hide');
                    }
                },
                error: function (err) {
                    errorAlert("Ha ocurrido un error al grabar el titular.");
                }
            });
        }
    });
    $("#persona-search", "#modal-window-titular").on("click", personaSearch);
    errorAlertInit();
    titularFormContent();
    $("#modal-window-titular").one('shown.bs.modal', hideLoading).modal('show');
}

function personaSearch() {
    buscarPersonas()
        .then(function (seleccion) {
            var form = $("#titular-form");
            if (seleccion) {
                $.post(BASE_URL + "MantenimientoParcelario/GetPersonaDatos/" + seleccion, function (persona) {
                    $("#NombreCompleto", form).val(persona.NombreCompleto);
                    $("#PersonaId", form).val(persona.PersonaId);
                    $("#TipoNoDocumento", form).val(persona.TipoDocumentoIdentidad.Descripcion + "/" + (persona.NroDocumento ? persona.NroDocumento : "-"));
                    form.formValidation("revalidateField", "PersonaId");
                    form.formValidation("revalidateField", "NombreCompleto");
                });
            } else {
                mostrarMensaje(false, "Buscar Persona", "No se ha seleccionado ninguna persona.");
                return;
            }
        }).catch(function (err) {
            console.log(err);
        });
}

function buscarPersonas() {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Personas, multiSelect: false, verAgregar: true, titulo: 'Buscar Persona', campos: ['Nombre', 'dni:DNI'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
                $(window).off('agregarObjetoBuscado');
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    resolve(evt.seleccion[2]);
                } else {
                    resolve();
                }
            });
            $(window).off("agregarObjetoBuscado").one("agregarObjetoBuscado", function () {
                showLoading();
                $("#personas-externo-container").load(BASE_URL + "Persona/BuscadorPersona", function () {
                    $(".modal.mainwnd", this).one('hidden.bs.modal', function () {
                        $(window).off('personaAgregada');
                    });
                    $(window).one("personaAgregada", function (evt) {
                        resolve(evt.persona.PersonaId);
                    });
                });
            });
        });
    });
}
function titularFormContent() {
    $("#titular-form").bootstrapValidator({
        framework: "boostrap",
        excluded: [":disabled"],
        fields: {
            PersonaId: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "La persona no existe, verifíquela",
                        value: 0
                    }
                }
            },
            NombreComleto: {
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
                        callback: function (value, validator, $field) {
                            var total = $("#PorcientoCopropiedadTotal").val();
                            if (parseFloat(total) + parseFloat(value) > 100) {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
        }
    });
}

function errorAlertInit() {
    var message = $("#message-error");
    message.find(".close").click(function () {
        $("#message-error").hide();
    });
}

function errorAlert(text) {
    var message = $("#message-error");
    message.find("p").html(text);
    $("#message-error").fadeIn("slow").delay(5000).queue(function () {
        $("#message-error").hide().dequeue();
    });
}

//@ sourceURL=titular.js
