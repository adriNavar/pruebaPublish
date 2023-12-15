var municipio;
var numeroParcelaLen;
var format;

let modoAutomatico = true;
let jurisdiccionesParcela;
$(document).ready(init);
$(window).resize(function () {
    ajustarmodal();
    ajustColumnsSize();
});

$('#alfanumericoModal').one('shown.bs.modal', function () {
    ajustarmodal();
    hideLoading();
});


function setBootstrapValidator() {
    $.get(BASE_URL + "AlfanumericoParcela/GetExpedienteRegularExpression", function ({ regex, ejemplo }) {
        $("#datos-form").bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            fields: {
                NumeroExpediente: {
                    validators: {
                        regexp: {
                            message: "El campo debe tener los siguientes formatos: 999-9999-999999/99 o 999-9999-99999/99",
                            regexp: regex
                        }
                    }
                },

                FechaExpediente: {
                    validators: {
                        date: {
                            format: "DD/MM/YYYY",
                            message: "El campo Fecha de Creación debe ser una fecha válida"
                        },
                        notEmpty: {
                            message: "El campo Fecha es obligatorio"
                        }
                    }
                },
            }
        });

        $("#destino-form").bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled", ":hidden"],
            fields: {
                Vigencia: {
                    validators: {
                        date: {
                            format: "DD/MM/YYYY",
                            message: "El campo Vigencia debe ser una fecha válida"
                        },
                        notEmpty: {
                            message: "El campo Vigencia es obligatorio"
                        }
                    }
                },

                Superficie: {
                    validators: {
                        callback: {
                            message: 'Debe tener cargado un valor numérico positivo',
                            callback: function (value) {
                                if ($("#Superficie").val() && (isNaN(value) || Number(value) <= 0)) {
                                    return {
                                        valid: false,
                                    }
                                }
                                return true;
                            }
                        },
                        notEmpty: {
                            message: "El campo Superficie es obligatorio"
                        }
                    }
                },
            }
        });
    });
}

function init() {
    document.getElementById("JurisdiccionId").selectedIndex = 0;
    document.getElementById("TipoOperacionId").selectedIndex = 0;
    ///////////////////// Scrollbars ////////////////////////
    $(".alfanumerico-content").niceScroll(getNiceScrollConfig());
    ////////////////////////////////////////////////////////
    let tspo = 500;
    $('#accordion-parcelas-origen > a').click(function () {
        columnsAdjust('parcelas-origen', tspo);
        tspo = null;
    });
    $('#accordion-datos-destino > a').click(function () {
        setTimeout(ajustarScrollBars, 10);
    });
    let tspd = 500;
    $('#accordion-parcelas-destino > a').click(function () {
        columnsAdjust('parcelas-destino', tspd);
        tspd = null;
    });

    setBootstrapValidator();
    parcelasOrigenInit();
    parcelasDestinoInit();

    $("#Superficie").val("");

    $("#TipoParcelaId").change(function (evt) {
        $.get(`${BASE_URL}AlfanumericoParcela/SetTipoParcelas/${evt.target.value}`, function () {
            $("#igaParcelaTipo").text(evt.target.value);
        });
    });

    $("#TipoOperacionId").change(function (evt) {
        debugger;
        habilitaParcelasOrigen();
        habilitaParcelasDestino();
        $("#parcelas-origen").dataTable().api().clear().draw();
        $("#parcelas-destino").dataTable().api().clear().draw();
        if ($("#parcelas-origen").dataTable().api().data().length == 0) {
            $.get(`${BASE_URL}AlfanumericoParcela/GetJurisdicciones`, function (data) {
                const cbo = $("#JurisdiccionId");
                cbo.empty();
                cbo.append("<option value selected>- Seleccione -</option>");
                data.values.forEach(v => cbo.append(`<option value="${v.FeatId}">${v.Codigo} - ${v.Nombre}</option>`));
                cbo.trigger("change");

            }).always(hideLoading);     
        }
        $("#parcelas-origen-delete").addClass("boton-deshabilitado");
        $("#NumeroExpediente").val("");
        $("#FechaExpediente").val(FormatFechaHora(new Date()));
        if ($("#TipoOperacionId").val() == 4) {
            $("#Superficie").removeClass("hidden");
            $("#labelSuperficie").removeClass("hidden");
        } else {
            $("#Superficie").addClass("hidden");
            $("#labelSuperficie").addClass("hidden");
        }
        if (esPrescripcion()) {//prescripcion adquisitiva
            $("#ClaseParcelaId").val(2);
        } else if (Number(evt.currentTarget.value) === 10) {//derecho real de superficie
            $("#ClaseParcelaId").val(8);
        }

        habilitarGrabar();
    });

    $("#JurisdiccionId").change(function () {
        $.get(`${BASE_URL}AlfanumericoParcela/SetJurisdiccion/${$(this).val()}`, function (codigo) {
            $("#igaJurisdiccion").text(codigo);
        });
    });
    $("#ClaseParcelaId").addClass("boton-deshabilitado");

    $("#FechaExpediente").datepicker(getDatePickerConfig({ endDate: new Date() }))
        .datepicker('update', FormatFechaHora(new Date()));
    $("#FechaExpediente").on("change changeDate", function () {
        setTimeout(function () {
            $("#datos-form").data("bootstrapValidator")
                .updateStatus("FechaExpediente", "NOT_VALIDATED", null)
                .validateField("FechaExpediente");
            habilitarGrabar();
        }, 200);
    });

    $("#Vigencia").datepicker(getDatePickerConfig())
        .on("change changeDate", function () {
            setTimeout(function () {
                $("#destino-form").data("bootstrapValidator")
                    .updateStatus("Vigencia", "NOT_VALIDATED", null)
                    .validateField("Vigencia");
                habilitarGrabar();
            }, 200);
        }).val(FormatFechaHora(new Date()));

    initEnter("#NumeroParcela", "#parcelas-destino-insert");

    ajustarmodal();
    $("#alfanumericoModal").modal("show");
}

function habilitarGrabar() {
    parcelasOrigenValidator(1)
        .then(parcelasDestinoValidator)
        .then(datosDestinoValidator)
        .then(vigenciaValidator)
        .then(function () {
            $("#save-all").removeClass("boton-deshabilitado");
            $("#save-all").off("click").on("click", saveAllClick);
        })
        .catch(function () {
            $("#save-all").addClass("boton-deshabilitado");
            $("#save-all").off("click");
        });
}

function habilitaParcelasOrigen() {
    if (!esCreacion()) {
        $("#parcelas-origen-insert").removeClass("boton-deshabilitado");
        $("#ClaseParcelaId").addClass("boton-deshabilitado");
    } else {
        $("#parcelas-origen-insert").addClass("boton-deshabilitado");
        $("#ClaseParcelaId").removeClass("boton-deshabilitado");
    }
}

function habilitaParcelasDestino() {
    $("#parcelas-destino-insert").addClass("boton-deshabilitado");
    if (!permiteSoloUnaParcelaDestino() || !$("#parcelas-destino").dataTable().api().data().toArray().length) {
        $("#parcelas-destino-insert").removeClass("disabled");
        $("#btnToggleModoGeneracionPartida").removeClass("disabled");
        modoAutomatico = true;
        activarModoPartida();
    } else {
        $("#parcelas-destino-insert").addClass("disabled");
        $("#NumeroParcela").attr("readonly", "readonly");
        $("#btnGenerarPartida").addClass("disabled");
        $("#btnToggleModoGeneracionPartida").addClass("disabled");
    }
}

function habilitarDatosComunes() {
    $("#TipoParcelaId,#EstadoParcelaId,#JurisdiccionId,#ClaseParcelaId").addClass("boton-deshabilitado");
    if (!$("#parcelas-destino").dataTable().api().data().length) {
        $("#TipoParcelaId,#EstadoParcelaId,#JurisdiccionId").removeClass("boton-deshabilitado");
        if (esCreacion()) {
            $("#ClaseParcelaId").removeClass("boton-deshabilitado");
        }
    }
}

function saveAllClick() {
    parcelasOrigenValidator(3)
        .then(parcelasDestinoValidator)
        .then(datosDestinoValidator)
        .then(function () {
            showLoading();
            return new Promise(function (ok, error) {
                $.ajax({
                    type: "POST",
                    url: BASE_URL + "AlfanumericoParcela/Save",
                    data: { operacion: $("#TipoOperacionId :selected").val(), expediente: $("#NumeroExpediente").val(), fecha: $("#FechaExpediente").val(), vigencia: $("#Vigencia").val() },
                    dataType: "json",
                    success: function (data) {
                        if (data === "IDX_SOLR") {
                            warningModal("Buscador - Advertencia", ["Los datos han sido guardados correctamente.", "Es posible que no se encuentren estos cambios temporalmente dado que el buscador está actualizándose."].join("<br />"));
                        } else if (data === "MVW_PARCELA") {
                            warningModal("Capa Parcela - Advertencia", ["Los datos han sido guardados correctamente pero no se pudieron actualizar las parcelas del mapa.", "Por favor, avise al administrador."].join("<br />"));
                        }
                        ok();
                    },
                    error: function (err) {
                        error(err.status === 409 ? "MultiplesGraficos" : "ErrorDatabase");
                    },
                    complete: hideLoading
                });
            });
        })
        .then(function () {
            $("#parcelas-origen").dataTable().api().clear().draw();
            $("#parcelas-destino").dataTable().api().clear().draw();
            $("#datos-form").formValidation("resetForm", true);
            $("#FechaExpediente").val(FormatFechaHora(new Date()));
            setTimeout(function () { habilitarInsertOrigen(); habilitarDatosComunes(); }, 100);
        })
        .catch(function (err) {
            if (err === "ErrorDatabase") {
                errorModal("Guardar - Error", "Hubo un Error en la base de datos");
            } else if (err === "MultiplesGraficos") {
                errorModal("Guardar - Error", "La parcela origen tiene más de un gráfico asociado");
            } else if (err.title) {
                errorModal(err.title, err.msg);
            } else if (err) {
                errorModal("Guardar - Error", "Ha ocurrido un error al guardar los datos");
            }
        })
        .finally(habilitarGrabar);
}

function clearParcelasDestino() {
    $("#NumeroParcela").val("");

    $.get(BASE_URL + "AlfanumericoParcela/OperacionesParcelasDestinosClear", function () {
        $("#parcelas-destino").dataTable().api().clear().draw();
    });
}

function habilitarInsertOrigen() {
    $("#parcelas-origen-insert").removeClass("boton-deshabilitado");
    if (esSubdivision() && $("#parcelas-origen").dataTable().api().data().toArray().length === 1) {
        $("#parcelas-origen-insert").addClass("boton-deshabilitado");
    }
}

function parcelasOrigenInit() {
    $("#parcelas-origen").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": false,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { data: "Partida", title: "Partida" }
        ],
        "initComplete": function () {
            console.log("init");
            const tabla = $(this);
            habilitarInsertOrigen();
            $("tbody", tabla).on("click", "tr", function () {
                if (tabla.DataTable().row(this).data()) {
                    parcelasOrigenEnableControls(!$(this).hasClass("selected"));
                    $(this).siblings().removeClass("selected");
                    $(this).toggleClass("selected");
                }
            });
        }
    });

    $("#parcelas-origen-insert").click(function () {
        if (!esCreacion()) {
            buscarParcelas()
                .then(function (seleccion) {
                    if (seleccion) {
                        var saved = [];
                        showLoading();
                        seleccion.forEach(function (data) {
                            if (!parcelasOrigenDuplicates(Number(data[1]))) {
                                saved.push((function (obj) {
                                    return new Promise(function (resolve, reject) {
                                        $.ajax({
                                            type: "POST",
                                            url: `${BASE_URL}AlfanumericoParcela/SaveParcelaOrigen?operacion=${$("#TipoOperacionId :selected").val()}&idparcela=${obj[1]}`,
                                            success: function (parcelaOrigen) {
                                                parcelaOrigen.partida = obj[0];
                                                resolve(parcelaOrigen);
                                            },
                                            error: reject
                                        });
                                    });
                                })(data));
                            }
                        });
                        Promise.all(saved)
                            .then(function (data) {
                                if (data.length) {
                                    var table;
                                    if (esUnificacion) {
                                        table = $("#parcelas-origen").one("draw.dt", getJurisdiccionesByParcelaOrigenUnificacion).dataTable().api();
                                    } else {
                                        table = $("#parcelas-origen").one("draw.dt", getJurisdiccionesByParcelaOrigen).dataTable().api();
                                    }
                                    data.forEach(function (parcela) {
                                        table.row.add({
                                            "ParcelaId": parcela.id,
                                            "TipoId": parcela.idTipo,
                                            "ClaseId": parcela.idClase,
                                            "Partida": parcela.partida
                                        });
                                    });
                                    table.draw();
                                    setTimeout(function () {
                                        parcelasOrigenValidator(2)
                                            .then(function () {
                                                if (!esDerechoReal() && !esPrescripcion()) {
                                                    $("#ClaseParcelaId").val(table.data().toArray()[0].ClaseId);
                                                }
                                                ajustarScrollBars();
                                                habilitarGrabar();
                                            })
                                            .catch(function (err) {
                                                errorModal(err.title, err.msg);
                                            })
                                            .then(habilitarInsertOrigen);
                                    }, 500);
                                }
                            })
                            .catch(function (err) {
                                console.log(err);
                            })
                            .finally(hideLoading);
                    } else {
                        return;
                    }
                })
                .catch(function (err) {
                    console.log(err);
                });
        }
    });

    $("#parcelas-origen-delete").click(function () {
        var selectedRow = $("#parcelas-origen").dataTable().api().row(".selected");
        if (selectedRow.data()) {
            modalConfirm("Eliminar - Parcela Origen", "¿Desea eliminar la parcela origen con partida " + selectedRow.data().Partida + "?", function () {
                showLoading();
                $.ajax({
                    url: BASE_URL + "AlfanumericoParcela/DeleteParcelaOrigen?idParcela=" + selectedRow.data().ParcelaId,
                    type: "POST",
                    success: function () {
                        if (esUnificacion) {
                            $("#parcelas-origen").one("draw.dt", getJurisdiccionesByParcelaOrigenUnificacion);
                        } else {
                            $("#parcelas-origen").one("draw.dt", getJurisdiccionesByParcelaOrigen);
                        }
                        selectedRow.remove().draw();
                        parcelasOrigenEnableControls(false);
                        parcelasOrigenValidator(1)
                            .then(function () {
                                if ($("#parcelas-origen").dataTable().api().data().length) {
                                    $("#ClaseParcelaId").val($("#parcelas-origen").dataTable().api().data().toArray()[0].ClaseId);
                                } else {
                                    $.get(`${BASE_URL}AlfanumericoParcela/GetJurisdicciones`)
                                        .done(loadComboJurisdicciones)
                                        .fail(() => errorModal("Parcelas Origen - Cargar Jurisdicciones", "Ha ocurrido un error al cargar las jurisdicciones."))
                                        .always(hideLoading);
                                }
                                ajustarScrollBars();
                                habilitarGrabar();
                            })
                            .catch(function (err) {
                                console.log(err);
                            })
                            .finally(habilitarInsertOrigen);
                    },
                    complete: hideLoading
                });
            });
        }
    });

    function loadComboJurisdicciones(data) {
        const cbo = $("#JurisdiccionId");
        cbo.empty();
        cbo.append("<option value selected>- Seleccione -</option>");
        data.values.forEach(v => cbo.append(`<option value="${v.FeatId}">${v.Codigo} - ${v.Nombre}</option>`));
        cbo.trigger("change");
    }

    function getJurisdiccionesByParcelaOrigen() {
        const parcelasOrigen = $("#parcelas-origen").dataTable().api().data().toArray();
        if (parcelasOrigen.length && !parcelasOrigen.some(po => po.ParcelaId === jurisdiccionesParcela)) {
            showLoading();
            jurisdiccionesParcela = parcelasOrigen[0].ParcelaId;
            parcelasOrigen.forEach(element => console.log(element));
            $.get(`${BASE_URL}AlfanumericoParcela/GetJurisdiccionesByDepartamentoParcela/${parcelasOrigen[0].ParcelaId}`)
                .done(loadComboJurisdicciones)
                .fail(() => errorModal("Parcelas Origen - Seleccionar Jurisdicción", `Ha ocurrido un error al seleccionar la jurisdicción de la parcela <strong>${parcelasOrigen[0].Partida}</strong>.`))
                .always(hideLoading);
        }
    }

    function getJurisdiccionesByParcelaOrigenUnificacion() {
        const parcelasOrigenId = $("#parcelas-origen").dataTable().api().data().toArray().map(function (a) { return a.ParcelaId; });
        showLoading();
        $.ajax({
            url: BASE_URL + "AlfanumericoParcela/GetJurisdiccionesByDepartamentoParcelaUnificacion",
            data: { ids: parcelasOrigenId },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                console.log(data);
                const cbo = $("#JurisdiccionId");
                cbo.empty();
                cbo.append("<option value selected>- Seleccione -</option>");
                var jurisdicciones = data.reduce((accum, jur) => [...accum, ...jur], []).sort((a, b) => a.Nombre > b.Nombre ? 1 : -1);
                jurisdicciones.forEach(v => cbo.append(`<option value="${v.FeatId}">${v.Codigo} - ${v.Nombre}</option>`));
                cbo.trigger("change");
            },
            error: function () {
            }
        });
    }
}

function buscarParcelas() {
    return new Promise(function (resolve) {
        var data = {
            tipos: BuscadorTipos.Parcelas,
            multiSelect: !esSubdivision(),
            verAgregar: false,
            titulo: 'Buscar Parcelas',
            campos: ["Partida", "nomenclatura:Nomenclatura"],
            readonlyText: false
        };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
            });
            $(window).one("seleccionAceptada", function (evt) {
                var seleccion = evt.seleccion;
                if (!data.multiSelect) {
                    seleccion = [evt.seleccion];
                }

                if (seleccion) {
                    resolve(seleccion.map(p => p.slice(1)));
                } else {
                    resolve([]);
                }
            });
        });
    });
}

function parcelasOrigenEnableControls(enable) {
    if (enable) {
        $("#parcelas-origen-delete").removeClass("boton-deshabilitado");
    } else {
        $("#parcelas-origen-delete").addClass("boton-deshabilitado");
    }
}

function parcelasOrigenDuplicates(idParcela) {
    return $("#parcelas-origen")
        .dataTable()
        .api().data()
        .toArray()
        .some(function (par) { return par.ParcelaId === idParcela; });
}

function parcelasOrigenValidator(type) {
    return new Promise(function (ok, error) {
        var evaluate = type === 1, save = type === 3;
        var data = $("#parcelas-origen").dataTable().api().data().toArray();

        if (save && (esSubdivision() || esDerechoReal() || esPrescripcion()) && data.length === 0) {
            error({ title: "Parcelas Origen - Cantidad Insuficiente", msg: "Debe ingresar al menos una parcela" });
            return;
        } else if (save && esSubdivision() && data.length > 1) {
            error({ title: "Parcelas Origen - Cantidad Excesiva", msg: "Debe ingresar sólo una parcela" });
            return;
        } else if (save && (esUnificacion() || esRedistribucion()) && data.length === 1) {
            error({ title: "Parcelas Origen - Cantidad Insuficiente", msg: "Debe ingresar más de una parcela" });
            return;
        } else if (!esCreacion()) {
            var validaciones = data.reduce(function (accum, par) {
                accum.tipos["" + par.TipoId] = 1;
                accum.clases["" + par.ClaseId] = 1;
                return accum;
            }, { tipos: {}, clases: {} });
            if (!evaluate && Object.keys(validaciones.clases).length > 1) {
                error({ title: "Parcelas Origen - Conflictos", msg: "Las parcelas seleccionadas deben ser todas de la misma clase" });
                return;
            } else if (Object.keys(validaciones.tipos).length > 1) {
                if (!save) {
                    warningModal("Parcelas Origen - Conflictos", "Las parcelas seleccionadas no son todas del mismo tipo");
                    $("#WarningModal").one("hidden.bs.modal", ok);
                    return;
                }
            }
        }
        ok();
    });
}

function esCreacion() {
    return Number($("#TipoOperacionId :selected").val()) === 4;
}

function esUnificacion() {
    return Number($("#TipoOperacionId :selected").val()) === 2;
}

function esRedistribucion() {
    return Number($("#TipoOperacionId :selected").val()) === 3;
}

function esPrescripcion() {
    return Number($("#TipoOperacionId :selected").val()) === 6;
}

function esDerechoReal() {
    return Number($("#TipoOperacionId :selected").val()) === 10;
}

function esSubdivision() {
    return Number($("#TipoOperacionId :selected").val()) === 1;
}

function permiteSoloUnaParcelaDestino() {
    return esUnificacion() || esPrescripcion() || esDerechoReal();
}

function datosDestinoInit() {
    $("#NumeroExpediente").blur(function () {
        var numeroExpediente = $(this);
        $.ajax({
            type: "POST",
            url: BASE_URL + "AlfanumericoParcela/VerifyNumeroExpediente?numeroExpediente=" + numeroExpediente.val(),
            success: function (data) {
                if (data !== "") {
                    $("#FechaExpediente").val(data);
                } else {
                    numeroExpediente.val("");
                    $("#FechaExpediente").val("");
                }
            }
        });
    });
}

function datosDestinoValidator() {
    return new Promise(function (ok, error) {
        try {
            var bootstrapValidator = $("#datos-form").data("bootstrapValidator");
            bootstrapValidator.validate();
            if (bootstrapValidator.isValid()) {
                ok();
            } else {
                error();
            }
        } catch (ex) {
            error(ex);
        }
    });
}

function vigenciaValidator() {
    return new Promise(function (ok, error) {
        try {
            var bootstrapValidator = $("#destino-form").data("bootstrapValidator");
            bootstrapValidator.validate();
            if (bootstrapValidator.isValid()) {
                ok();
            } else {
                error();
            }
        } catch (ex) {
            error(ex);
        }
    });

}

function activarModoPartida() {
    if (modoAutomatico) {
        $("i:first-of-type", "#btnToggleModoGeneracionPartida").show();
        $("i:last-of-type", "#btnToggleModoGeneracionPartida").hide();
        $("#btnGenerarPartida").removeClass("disabled").show();
        $("#NumeroParcela").attr("readonly", "readonly").val("").trigger("input");
        $("#btnToggleModoGeneracionPartida").prop("title", "Activar Partida Manual");
    } else {
        $("i:first-of-type", "#btnToggleModoGeneracionPartida").hide();
        $("i:last-of-type", "#btnToggleModoGeneracionPartida").show();
        $("#btnGenerarPartida").addClass("disabled").hide();
        $("#NumeroParcela").removeAttr("readonly");
        $("#btnToggleModoGeneracionPartida").prop("title", "Activar Partida Automática");
    }
}
function parcelasDestinoInit() {
    $("#btnGenerarPartida").on("click", evt => {
        if (evt.currentTarget.classList.contains("disabled")) {
            return;
        }
        showLoading();
        $.post(`${BASE_URL}UnidadTributaria/GenerarPartida`, { jurisdiccion: parseInt($("#JurisdiccionId :selected", "#accordion-alfanumerico").val()), tipo: parseInt($("#TipoParcelaId :selected", "#accordion-alfanumerico").val()), stripped: true })
            .done(partida => {
                $("#NumeroParcela").val(partida).trigger("input");
                $("#btnGenerarPartida").addClass("disabled");
            })
            .fail(() => errorModal("Parcelas Destino - Error", "No se pudo obtener la partida."))
            .always(hideLoading());
    });
    $("#btnToggleModoGeneracionPartida").on("click", () => {
        modoAutomatico = !modoAutomatico;
        activarModoPartida();
    });

    $("#parcelas-destino").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { data: "Partida", title: "Partida" }
        ],
        "initComplete": function () {
            const tabla = $(this);
            $("tbody", tabla).on("click", "tr", function () {
                if (tabla.dataTable().api().row(this).data()) {
                    parcelasDestinoEnableControls(!$(this).hasClass("selected"));
                    $(this).siblings().removeClass("selected");
                    $(this).toggleClass("selected");
                }
            });
        }
    });

    $("#NumeroParcela")
        .on("keypress", function (evt) {
            var isDigit = /\d/, regex = /^\d{1,6}$/;
            if (!isDigit.test(evt.key) || !regex.test(evt.target.value + evt.key)) {
                evt.preventDefault();
            }
        })
        .on("input", function (evt) {
            var regex = /^[0-9]{1,6}$/;
            if (!regex.test(evt.target.value)) {
                evt.target.value = "";
            }
            $("#parcelas-destino-insert").addClass("boton-deshabilitado");
            if (evt.target.value.length === 6) {
                $("#parcelas-destino-insert").removeClass("boton-deshabilitado");
            }
        });

    $("#parcelas-destino-insert").click(function () {
        debugger;
        parcelasDestinoDuplicada()
            .then(function (partida) {
                var table = $("#parcelas-destino").dataTable().api();

                var idTipoOperacion = $("#TipoOperacionId :selected").val();
                var idParcela = getNextId(table.rows().data(), "ParcelaId");
                var idTipoParcela = $("#TipoParcelaId :selected").val();
                var idClaseParcela = $("#ClaseParcelaId :selected").val();
                var idEstadoParcela = $("#EstadoParcelaId :selected").val();
                var superficie = $("#Superficie").val();

                $.ajax({
                    type: "POST",
                    url: BASE_URL + "AlfanumericoParcela/SaveParcelaDestino",
                    data: {
                        partida: partida,
                        idTipoOperacion: idTipoOperacion,
                        idparcela: idParcela,
                        idtipoparcela: idTipoParcela,
                        idclaseparcela: idClaseParcela,
                        idestadoparcela: idEstadoParcela,
                        superficie: superficie
                    },
                    success: function () {
                        table.row.add({
                            "ParcelaId": idParcela,
                            "Partida": partida
                        }).draw();
                        $("#NumeroParcela").val("");
                        habilitaParcelasDestino();
                        habilitarGrabar();
                        habilitarDatosComunes();
                        ajustarScrollBars();
                    },
                    error: function (err) {
                        errorModal("Parcelas Destino - Error", err.status === 409 ? "La partida pertenece a una unidad tributaria existente." : err.statusText);
                    }
                });
            })
            .catch(function (error) {
                if (error) {
                    errorModal("Parcelas Destino - Error", error);
                }
            });
    });

    $("#parcelas-destino-delete").click(function () {
        var selectedRow = $("#parcelas-destino").dataTable().api().row(".selected");
        if (selectedRow.data()) {
            modalConfirm("Eliminar - Parcela Destino", "¿Desea eliminar la parcela destino con partida " + selectedRow.data().Partida + "?", function () {
                showLoading();
                $.ajax({
                    url: BASE_URL + "AlfanumericoParcela/DeleteParcelaDestino?&idParcela=" + selectedRow.data().ParcelaId,
                    type: "POST",
                    success: function () {
                        selectedRow.remove().draw();
                        parcelasDestinoEnableControls(false);
                        habilitaParcelasDestino();
                        habilitarDatosComunes();
                        habilitarGrabar();
                    },
                    complete: hideLoading
                });
            });
        }
    });
}

function parcelasDestinoEnableControls(enable) {
    if (enable) {
        $("#parcelas-destino-delete").removeClass("boton-deshabilitado");
    } else {
        $("#parcelas-destino-delete").addClass("boton-deshabilitado");
    }
}

function parcelasDestinoDuplicada() {
    return new Promise(function (ok, error) {
        $.ajax({
            url: BASE_URL + "AlfanumericoParcela/ValidarPartida?numero=" + $("#NumeroParcela").val(),
            success: ok,
            error: function (err) {
                if (err.status === 409) {
                    error("La partida ya existe en la operación");
                } else {
                    error(err.statusText);
                }
            }
        });
    });
}

function parcelasDestinoValidator() {
    return new Promise(function (ok, error) {
        var data = $("#parcelas-destino").dataTable().api().data().toArray();

        if (esCreacion() && data.length === 0) {
            error({ title: "Parcelas Destino - Cantidad Insuficiente", msg: "La operación debe tener al menos una parcela destino" });
            return;
        } if (permiteSoloUnaParcelaDestino() && data.length !== 1) {
            error({ title: "Parcelas Destino - Cantidad Incorrecta", msg: "La operación debe tener sólo una parcela destino" });
            return;
        } else if ((esSubdivision() || esRedistribucion()) && data.length <= 1) {
            error({ title: "Parcelas Destino - Cantidad Insuficiente", msg: "La operación debe tener más de una parcela destino" });
            return;
        }
        ok();
    });
}

function accordionTabHide(accordionTab) {
    if (accordionTab.hasClass("in")) {
        accordionTab.collapse("hide");
        //arrow
        accordionTab.addClass("collapsed");
        accordionTab.attr("aria-expanded", "false");
    }
}

function accordionTabShow(accordionTab) {
    accordionTab.collapse("show");
    //arrow
    accordionTab.removeClass("collapsed");
    accordionTab.attr("aria-expanded", "true");
}

function columnsAdjust(tableId, ts) {
    setTimeout(() => {
        $(`#${tableId}`)
            .on("draw.dt", () => setTimeout(ajustarScrollBars, ts || 100))
            .dataTable().api().columns.adjust().draw();
    }, 30);
}

function ajustarmodal() {
    const altura = $(window).height() - 180;
    $(".alfanumerico-body").css({ "height": altura });
    $(".alfanumerico-content").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $('.alfanumerico-content').getNiceScroll().resize();
    $('.alfanumerico-content').getNiceScroll().show();
}

function getNextId(arr, key) {
    var elem;
    var max = 0;
    var id;
    for (elem in arr) {
        if (arr.hasOwnProperty(elem)) {
            id = Math.abs(arr[elem][key]);
            if (id > max)
                max = id;
        }
    }
    return (max + 1) * -1;
}

function modalConfirm(title, message, cb) {
    $("#TituloConfirmacion", "#confirmModal").text(title);
    $("#DescripcionConfirmacion", "#confirmModal").html(message);
    $("#btnOkConfirm", "#confirmModal").off("click").one("click", function () {
        $("#confirmModal").modal("hide");
        cb();
    });
    $("#confirmModal").modal("show");
}

function warningModal(title, message, type) {
    type = type || "warning";
    $("#TituloAdvertencia", "#WarningModal").text(title);
    $("#DescripcionAdvertencia", "#WarningModal").html(message);
    $("[role='alert']", "#WarningModal").removeClass("alert-danger alert-warning").addClass("alert-" + type);
    $("#WarningModal").modal("show");
}

function errorModal(title, message) {
    warningModal(title, message, "danger");
}

function initEnter(input, button) {
    $(input).keypress(function (e) {
        if (e.which === 13) {
            $(button).click();
        }
    });
}

//@ sourceURL=alfanumerico.js
