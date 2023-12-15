$(document).ready(init);
function init() {
    var form = "#nomenclatura-form",
        dato_nomenclatura = $("#Nombre", form).val(),
        tipo_nomenclatura = $("#TipoNomenclaturaID", form).val();

    var habilitarGrabarNomenclatura = function () {
        var nomenclaturaIgual = dato_nomenclatura && dato_nomenclatura === $("#Nombre").val(),
            tipoIgual = tipo_nomenclatura && tipo_nomenclatura === $("#TipoNomenclaturaID").val();

        $("#btnGrabarNomenclatura").addClass("boton-deshabilitado");
        if (!nomenclaturaIgual || !tipoIgual) {
            $("#btnGrabarNomenclatura").removeClass("boton-deshabilitado");
            return true;
        }
        return false;
    };
    var GuardarNomenclatura = function () {
        if (habilitarGrabarNomenclatura()) {
            var bv = $(form).data("bootstrapValidator");
            bv.validate();

            if (!bv.isValid()) return;
        }

        var tabla = $("#nomenclaturas").dataTable().api(),
            nomenclaturas = tabla.rows().data().toArray(),
            nomenclaturaId = parseInt($("#NomenclaturaID", form).val()),
            tipoNomenclaturaId = parseInt($("#TipoNomenclaturaID", form).val());

        if (nomenclaturas.some(function (nom) { return nom.TipoNomenclaturaID === tipoNomenclaturaId && nom.NomenclaturaID !== nomenclaturaId })) {
            errorAlert("El tipo de nomenclatura para la parcela ya existe");
            return;
        }

        var nomenclatura = {
            NomenclaturaID: nomenclaturaId,
            TipoNomenclaturaID: tipoNomenclaturaId,
            Nombre: $("#Nombre", form).val(),
            FechaModificacion: $("#FechaModificacion", form).val(),
            UsuarioAltaID: $("#UsuarioAltaID", form).val()
        };
        $.ajax({
            type: 'POST',
            data: { nomenclatura: nomenclatura },
            url: BASE_URL + 'Nomenclatura/ValidarNomenclatura',
            dataType: 'json',
            success: function (data) {
                if (data.OK) {
                    var fecha = new Date(parseInt(data.nomenclatura.FechaModificacion.substr(6)));
                    var anio = fecha.getFullYear();
                    var mes = fecha.getMonth() + 1;
                    var dia = fecha.getDate();
                    data.nomenclatura.FechaModificacion = anio + "-" + mes + "-" + dia;
                    data.nomenclatura.Tipo.ExpresionRegular = "";
                    $(document).trigger({ type: 'nomenclaturaGuardada', nomenclatura: data.nomenclatura });
                    $("#modal-window-nomenclatura").modal('hide');
                }
                else {
                    errorAlert(data.msg);
                }
            },
            error: function (_, textStatus, errorThrown) {
                console.log(textStatus, errorThrown);
            }
        });
    };
    var errorAlertInit = function () {
        var message = $("#message-error", "#modal-window-nomenclatura");
        $(".close", message).click(function () {
            message.hide();
        });
    };
    var errorAlert = function (text) {
        var message = $("#message-error");
        message.find("p").html(text);
        $("#message-error").fadeIn("slow").delay(5000).queue(function () {
            $("#message-error").hide().dequeue();
        });
    };
    $("#FechaModificacion", form).datepicker(getDatePickerConfig({ enableOnReadonly: false }));
    $('#modal-window-nomenclatura').one('shown.bs.modal', hideLoading);
    $('#modal-window-nomenclatura').one('hidden.bs.modal', function () {
        $(document).off("nomenclaturaGuardada");
    });
    $("#btnGrabarNomenclatura").on('click', GuardarNomenclatura);

    errorAlertInit();

    $(form).bootstrapValidator({
        framework: "bootstrap",
        fields: {
            Nombre: {
                validators: {
                    callback: {
                        message: "El campo nomenclatura no cumple con el formato",
                        callback: function (value) {
                            var response = $.ajax({
                                url: BASE_URL + "Nomenclatura/ValidarTipoNomenclatura",
                                type: "POST",
                                data: {
                                    idTipoNomenclatura: $("#TipoNomenclaturaID").val(),
                                    value: value
                                },
                                async: false
                            }).responseText;
                            return response === "true";
                        }
                    }
                }
            }
        }
    });
    $("#TipoNomenclaturaID", form).change(function () {
        if ($("#Nombre", form).val()) {
            $(form).data('formValidation').revalidateField("Nombre");
        }
        habilitarGrabarNomenclatura();
    });

    $("#Nombre", form).on("change input", habilitarGrabarNomenclatura);

    $("#btnGenerar", form).click(function () {
        $.ajax({
            type: 'GET',
            url: BASE_URL + 'Nomenclatura/Generar',
            data: { tipo: $("#TipoNomenclaturaID", form).val() },
            success: function (nomenclatura) {
                $("#Nombre", form).val(nomenclatura);
            },
            error: function (_, textStatus, errorThrown) {
                console.log(textStatus, errorThrown);
            },
            complete: hideLoading
        });
    });

    $("#modal-window-nomenclatura").modal('show');
}