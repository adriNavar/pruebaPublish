$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-unidad-tributaria').one('shown.bs.modal', hideLoading);

function init() {
    let modoAutomatico = true;

    const activarModoPartida = () => {
        if (modoAutomatico) {
            $("i:first-of-type", "#modal-window-unidad-tributaria #btnToggleModoGeneracionPartida").show();
            $("i:last-of-type", "#modal-window-unidad-tributaria #btnToggleModoGeneracionPartida").hide();
            $("#btnGenerarPartida", "#modal-window-unidad-tributaria").prop("disabled", false).show();
            $("#CodigoProvincial", "#modal-window-unidad-tributaria").prop("readonly", true).val("").trigger("input");
            $("#btnToggleModoGeneracionPartida", "#modal-window-unidad-tributaria").prop("title", "Activar Partida Manual");
        } else {
            $("i:first-of-type", "#modal-window-unidad-tributaria #btnToggleModoGeneracionPartida").hide();
            $("i:last-of-type", "#modal-window-unidad-tributaria #btnToggleModoGeneracionPartida").show();
            $("#btnGenerarPartida", "#modal-window-unidad-tributaria").prop("disabled", true).hide();
            $("#CodigoProvincial", "#modal-window-unidad-tributaria").prop("readonly", false);
            $("#btnToggleModoGeneracionPartida", "#modal-window-unidad-tributaria").prop("title", "Activar Partida Automática");
        }
    };

    $(".ut-fecha", ".unidad-tributaria-body")
        .datepicker(getDatePickerConfig());

    $(".ut-fecha-vigencia", ".unidad-tributaria-body")
        .datepicker(getDatePickerConfig({ enableOnReadonly: false }))
        .change(function () {
            $("#datos-form", "#modal-window-unidad-tributaria").data("bootstrapValidator")
                .updateStatus("Vigencia", "NOT_VALIDATED", null)
                .validateField("Vigencia");
        });

    $("#btnGrabarUT").oneClick(function () {
        var form = $("#datos-form", "#modal-window-unidad-tributaria"),
            validator = form.data("bootstrapValidator");
        validator.resetForm();
        validator.validate();
        var prom = new Promise(function (ok, err) {
            var interval = setInterval(() => {
                if (validator.isValid() != null) {
                    clearInterval(interval);
                    if (validator.isValid()) {
                        ok();
                    } else {
                        err();
                    }

                    ok(validator.isValid());
                }
            }, 50);
        });
        prom.then(function () {
            var ut = {
                UnidadTributariaId: $("#UnidadTributariaId", form).val(),
                TipoUnidadTributariaID: $("#cboTipoUT", form).val(),
                Vigencia: $("#Vigencia", form).val(),
                CodigoProvincial: $("#CodigoProvincial", form).val(),
                UnidadFuncional: $("#UnidadFuncional", form).val(),
                Superficie: $("#Superficie", form).val(),
                PlanoId: $("#PlanoId", form).val(),
                JurisdiccionID: $("#JurisdiccionID", form).val(),
                PorcentajeCopropiedad: $("#PorcentajeCopropiedad", form).val(),
                CodigoMunicipal: $("#CodigoMunicipal", form).val(),
                Observaciones: $("#Observaciones", form).val(),
                Piso: $("#Piso", form).val(),
                Unidad: $("#Unidad", form).val(),
                FechaVigenciaDesde: $("#FechaVigenciaDesde", form).val(),
                FechaVigenciaHasta: $("#FechaVigenciaHasta", form).val(),
                FechaAlta: $("#FechaAlta", form).val(),
                FechaBaja: $("#FechaBaja", form).val(),
                TipoUnidadTributaria: {
                    TipoUnidadTributariaID: $("#cboTipoUT", form).val(),
                    Descripcion: $("#cboTipoUT option:selected", form).text()
                }
            };
            $(document).trigger({ type: 'unidadTributariaGuardada', unidadTributaria: ut });
            $("#modal-window-unidad-tributaria").modal('hide');
        });
    });
    $("#btnGenerarPartida", "#modal-window-unidad-tributaria").on("click", () => {
        showLoading();
        const form = $("#datos-form", "#modal-window-unidad-tributaria"),
            /*la zonificación es siempre 1 para las UF o UP*/
            tipoZonificacion = parseInt($("#cboTipoUT", form).val()) === 3 ? 1 : parseInt($("#cboTipo :selected", "form#parcela-form").val());
        $.post(`${BASE_URL}UnidadTributaria/GenerarPartida`, { jurisdiccion: parseInt($("#JurisdiccionID :selected", form).val()), tipo: tipoZonificacion })
            .done(partida => {
                $("#CodigoProvincial", form).val(partida).trigger("input");
                $("#btnGenerarPartida", form).prop("disabled", true);
            })
            .fail(() => errorModal("Parcelas Destino - Error", "No se pudo obtener la partida."))
            .always(hideLoading);
    });

    $("#btnToggleModoGeneracionPartida").on("click", () => {
        modoAutomatico = !modoAutomatico;
        activarModoPartida();
    });

    $("#datos-form", "#modal-window-unidad-tributaria").bootstrapValidator({
        framework: "boostrap",
        submitButtons: "#btnGrabarUT",
        fields: {
            CodigoProvincial: {
                message: 'El Código Provincial es inválido.',
                validators: {
                    notEmpty: {
                        message: 'El Código Provincial es obligatorio.'
                    },
                    stringLength: {
                        min: 9,
                        max: 9,
                        message: 'El Código Provincial es inválido.'
                    },
                    remote: {
                        url: BASE_URL + 'UnidadTributaria/ValidarPartida',
                        data: function () {
                            return {
                                IdUnidadTributaria: parseInt($("#UnidadTributariaId", "#modal-window-unidad-tributaria").val())
                            };
                        },
                    },
                }
            },
            Superficie: {
                message: "La superficie es obligatoria para este tipo de unidad tributaria.",
                validators: {
                    callback: {
                        callback: function (val) {
                            if (Number($("#cboTipoUT").val()) === 3 && (isNaN(val) || Number(val) <= 0)) {
                                return { valid: false };
                            }
                            return true;
                        }
                    }
                }
            },
            Vigencia: {
                validators: {
                    date: {
                        format: "DD/MM/YYYY",
                        message: "El campo Vigencia debe ser una fecha válida"
                    },
                    notEmpty: {
                        message: 'La vigencia es obligatoria.'
                    },
                }
            },
            PorcentajeCopropiedad: {
                message: "El Porcentaje de Copropiedad es inválido.",
                validators: {
                    callback: {
                        callback: function (val) {
                            var valid = true, errorMsg = [], tipoUT = Number($("#cboTipoUT").val());
                            if (tipoUT !== 3 /*&& tipoUT !== 4*/ && !isNaN(val) && Number(val) !== 0) {
                                valid = false;
                                errorMsg = ["El Porcentaje de Copropiedad debe ser 0 para este tipo de unidad tributaria."];
                            } else if (tipoUT === 3 /*|| tipoUT === 4*/) {
                                if (!val) {
                                    valid = false;
                                    errorMsg = ["El Porcentaje de Copropiedad es obligatorio."];
                                }
                                if (isNaN(val)) {
                                    valid = false;
                                    errorMsg.push("El Porcentaje de Copropiedad debe ser numérico.");
                                } else if (val > 100) {
                                    valid = false;
                                    errorMsg.push("El Porcentaje de Copropiedad debe ser menor o igual a 100.");
                                }
                                else if (val <= 0) {
                                    valid = false;
                                    errorMsg.push("El Porcentaje de Copropiedad debe ser mayor a 0.");
                                }
                                if (!isNaN(val) && parseFloat($("#PorcientoCopropiedadTotal").val()) + Number(val) > 100) {
                                    valid = false;
                                    errorMsg.push("El Porcentaje de Copropiedad Total supera al 100%.");
                                }
                            }
                            if (!valid) {
                                return { valid: valid, message: errorMsg.join("<br />") };
                            }
                            return true;
                        }
                    }
                }
            }
        }
    });

    ajustarmodal();
    $("#modal-window-unidad-tributaria").modal('show');
}
function ajustarmodal() {
    var altura = Math.max($(window).height() - 170, 150);
    $(".unidad-tributaria-body", "#scroll-content-unidad-tributaria").css({ "max-height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $('#divTabContent').getNiceScroll().resize();
    $('#divTabContent').getNiceScroll().show();
}

//# sourceURL=ut.js