$(document).ready(init);
function init() {
    $("#btnGrabar-responsable-fiscal").on('click', function () {
        var form = $("#responsable-fiscal-form");
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {

            var model = {
                UnidadTributariaPersonaId: $("#UnidadTributariaPersonaId").val(),
                UnidadTributariaId: $("#UnidadTributariaId").val(),
                TipoPersonaId: $("#TipoPersonaId :selected").val(),
                PersonaId: $("#PersonaId").val(),
                NombreCompleto: $("#NombreCompleto").val(),
                DomicilioFisico: $("#DomicilioFisico").val(),
                TipoPersona: $("#TipoPersonaId :selected").text(),
                SavedPersonaId: $("#SavedPersonaId").val(),
                Operacion: $("#Operacion").val()
            };

            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/SaveResponsableFiscal",
                type: 'POST',
                dataType: 'json',
                data: model,
                success: function (responsable) {
                    if (responsable.error) {
                        errorAlert(responsable.mensaje);
                    } else {
                        $(document).trigger({ type: "responsableFiscalGuardado", responsableFiscal: responsable });
                        $("#modal-window-responsable-fiscal").modal('hide');
                    }
                },
                error: function () {
                    errorAlert("Ha ocurrido un error al grabar el responsable.");
                }
            });
        }
    });
    $("#persona-search", "#modal-window-responsable-fiscal").on("click", personaSearch);
    errorAlertInit();
    responsableFiscalFormContent();
    $("#modal-window-responsable-fiscal").one('shown.bs.modal', hideLoading).modal('show');
}

function personaSearch() {
    buscarPersonas()
        .then(function (seleccion) {
            var form = $("#responsable-fiscal-form");
            if (seleccion) {
                $.post(BASE_URL + "PersonaExpedienteObra/GetPersona/" + seleccion, function (persona) {
                    $("#NombreCompleto", form).val(persona.NombreCompleto);
                    $("#PersonaId", form).val(persona.PersonaInmuebleId);
                    $("#DomicilioFisico", form).val(persona.DomicilioFisico);
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
            $(window).one("agregarObjetoBuscado", function () {
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

function responsableFiscalFormContent() {
    $("#responsable-fiscal-form").bootstrapValidator({
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

//@ sourceURL=responsableFiscal.js