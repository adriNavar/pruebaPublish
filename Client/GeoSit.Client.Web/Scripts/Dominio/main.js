$(document).ready(init);
$("#modal-window-dominio").one('shown.bs.modal', hideLoading);
function init() {
    $("#FechaHora").datepicker(getDatePickerConfig());
    document.getElementById("labelNumero").style.display = 'none';

    $("#TipoInscripcionID").change(function () {
        $.get(BASE_URL + "Dominio/GetInscripcionRegexExample?regex=" + btoa($(":selected", this).attr("regexp").replaceAll("\"", "")), function (regex) {
            debugger;
            $("input[name='Inscripcion']").attr("placeholder", "Formato: " + (regex.ejemplo || "Libre"));
            document.getElementById("labelNumero").style.display = 'block';
            if (regex.ejemplo != null) {
                document.getElementById('labelNumero').innerHTML = regex.message;
            } else {
                document.getElementById('labelNumero').innerHTML = "El formato del campo Inscripción debe ser: Libre";
            }
        });

        $("#Inscripcion").val("");
        $("#dominio-form").data("formValidation").resetField("Inscripcion");
    });

    $("#btnGrabar-dominio").on('click', function () {
        var form = $("#dominio-form");
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {

            var model = {
                DominioID: $("#DominioID").val(),
                UnidadTributariaID: $("#UnidadTributariaID").val(),
                TipoInscripcionID: $("#TipoInscripcionID :selected").val(),
                TipoInscripcionDescripcion: $("#TipoInscripcionID :selected").text(),
                Inscripcion: $("#Inscripcion").val(),
                FechaHora: $("#FechaHora").val(),
                Operacion: $("#Operacion").val()
            };

            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/SaveDominio",
                type: 'POST',
                dataType: 'JSON',
                data: model,
                success: function (dominio) {
                    if (dominio.error) {
                        errorAlert(dominio.mensaje);
                    } else {
                        $(document).trigger({ type: "dominioGuardado", dominio: dominio });
                        $("#modal-window-dominio").modal('hide');
                    }
                },
                error: function (err) {
                    errorAlert("Ha ocurrido un error al grabar el responsable.");
                }
            });
        }
    });
    errorAlertInit();
    dominioFormContent();
    $("#modal-window-dominio").modal('show');
}

function dominioFormContent() {
    $("#dominio-form").bootstrapValidator({
        framework: "boostrap",
        excluded: [":disabled"],
        fields: {
            Inscripcion: {
                validators: {
                    callback: {
                        message: "El campo Inscripción no cumple con el formato requerido",
                        callback: function (value) {
                            var regexp = new RegExp($("#TipoInscripcionID :selected").attr("regexp"));
                            if (regexp.test(value)) return true;
                            return false;
                        }
                    },
                    notEmpty: {
                        message: "El campo Inscripción es obligatorio"
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
//@ sourceURL=dominio.js
