$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-mensura').one('shown.bs.modal', function () {
    ajustarScrollBars();
    $('#Grilla_mensuras').dataTable().api().columns.adjust();
    hideLoading();
});
var selectedRowUnidades = null;
var selectedRowDocumentos = null;
var selectedRowMensRel = null;

function init() {
    let modoAutomatico = true;
    const activarModoMensura = (noLimpiar) => {
        if (modoAutomatico) {
            $("i:first-of-type", "#btnToggleModoGeneracionMensura").show();
            $("i:last-of-type", "#btnToggleModoGeneracionMensura").hide();
            $("#btnGenerarMensura", ".btn-generacion-mensura").prop("disabled", false).show();
            if (!noLimpiar) {
                $("#DatosMensura_Numero,#DatosMensura_Letra,#DatosMensura_Descripcion").val("").trigger("input");
            }
            $("#DatosMensura_Numero,#DatosMensura_Letra,#DatosMensura_Descripcion").prop("readonly", true);
            $("#btnToggleModoGeneracionMensura").prop("title", "Activar Generación Manual");
        } else {
            $("i:first-of-type", "#btnToggleModoGeneracionMensura").hide();
            $("i:last-of-type", "#btnToggleModoGeneracionMensura").show();
            $("#btnGenerarMensura", ".btn-generacion-mensura").prop("disabled", true).hide();
            $("#DatosMensura_Numero,#DatosMensura_Letra,#DatosMensura_Descripcion").prop("readonly", false);
            $("#btnToggleModoGeneracionMensura").prop("title", "Activar Generación Automática");
        }
    };
    ///////////////////// DataTable /////////////////////////
    $('#Grilla_mensuras').dataTable({
        "scrollY": "132px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[1, 'asc']],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
        columnDefs: [
            { "targets": 1, "width": "30%" },
            { "targets": 2, "width": "40%" }
        ]
    });
    $("#Grilla_mensuras tbody").on("click", "tr", function () {
        $(this).toggleClass("selected");
        $("tr.selected", "#Grilla_mensuras tbody").not(this).removeClass("selected");
        mensuraEnableControls($(this).hasClass("selected"));
        if ($(this).hasClass("selected")) {
            CargarDatosDeLaMensura();
            documentosEnableControls(false);
        }
    });

    unidadesInit();
    documentosInit();
    mensurasRelacInit();

    ////////////////////////////////////////////////////////
    $("#formMensura").ajaxForm({
        beforeSubmit: showLoading,
        beforeSerialize: (form) => {
            $("input[name=ParcelaId]", form).remove();
            const parIds = $("#unidades").dataTable().api().data().toArray().map(elem => elem[4]);
            for (let idx in parIds) {
                $(form).append(`<input type="hidden" name="ParcelaId" value="${parIds[idx]}">`);
            }
            $("input[name=MensuraRelacOrigenId]", form).remove();
            $("input[name=MensuraRelacDestinoId]", form).remove();
            const mensurasId = $("#mensurasRelacionadas").dataTable().api().data().toArray().map(elem => ({ mensuraOrigenId: elem[4], mensuraDestinoId: elem[5] }));
            for (let idx in mensurasId) {
                $(form).append(`<input type="hidden" name="MensuraRelacOrigenId" value="${mensurasId[idx].mensuraOrigenId}">`);

                $(form).append(`<input type="hidden" name="MensuraRelacDestinoId" value="${mensurasId[idx].mensuraDestinoId}">`);
            }
            $("input[name=DocumentoId]", form).remove();
            const docIds = $("#documentos").dataTable().api().data().toArray().map(elem => elem[5]);
            for (let idx in docIds) {
                $(form).append(`<input type="hidden" name="DocumentoId" value="${docIds[idx]}">`);
            }
        },
        resetForm: true,
        success: function (data) {
            if (typeof mensuraGuardada === "function") {
                mensuraGuardada(data);
            } else {
                let descripcion = "Mensura";
                if ($("#EstadoOperacion").val() === "Alta") {
                    descripcion = "Nueva Mensura";
                }
                $('#TituloInfoMensura').html(`Información - ${descripcion}`);
                $('#DescripcionInfoMensura').html(`Los datos de la ${descripcion.toLowerCase()} han sido guardados satisfactoriamente.<br/>Es posible que no se encuentren estos cambios temporalmente dado que el buscador está actualizándose.`);
                $("#ModalInfoMensura").modal('show');
                modoAutomatico = true;
                activarModoMensura();

                resetMensuraControls();
                $("#btnSearch").click();
            }
        },
        error: function () {
            $('#TituloInfoMensura').html("Información - Error");
            $('#DescripcionInfoMensura').html("Se ha producido un error al grabar los datos.");
            $("#ModalInfoMensura").modal('show');
        },
        complete: hideLoading
    });
    ///////////////////// Scrollbars ////////////////////////
    $(".mensura-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-mensura .panel-body').resize(ajustarScrollBars);
    $('.mensura-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".mensura-content").getNiceScroll().resize();
        }, 10);
    });

    $("#formMensura").submit(function (evt) {
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $("#DatosMensura_IdTipoMensura").select2().val();

    $("#DatosMensura_Numero", "#formMensura").on("input", descripcionChange);
    $("#DatosMensura_Letra", "#formMensura").on("input", descripcionChange);

    $(".date .fecPresentacion").datepicker(getDatePickerConfig({
        enableOnReadonly: false,
        showButtonPanel: true,
        beforeShow: function (input) {
            dpClearButton(input);
        },
        onChangeMonthYear: function (yy, mm, inst) {
            dpClearButton(inst.input);
        }
    }));
    $(".date .fecAprobacion").datepicker(getDatePickerConfig({
        enableOnReadonly: false,
        showButtonPanel: true,
        beforeShow: function (input) {
            dpClearButton(input);
        },
        onChangeMonthYear: function (yy, mm, inst) {
            dpClearButton(inst.input);
        }
    }));

    $("#btnGrabar").click(function () {
        let confirmaGrabacion = Promise.resolve();
        if ("Edicion" === $("#EstadoOperacion").val()) {
            confirmaGrabacion = new Promise((ok) => {
                $('#TituloAdvertenciaMensura').html("Advertencia - Modificación de Datos");
                $('#DescripcionAdvertenciaMensura').html(`Está a punto de modificar los datos de la mensura ${$('#DatosMensura_Numero').val()}-${$('#DatosMensura_Letra').val().toUpperCase()}<br>¿Desea Continuar?`);
                $("#btnGrabarAdvertenciaMensura").off("click").one("click", ok);
                $('#ModalAdvertenciaMensura').modal('show');
            });
        }
        confirmaGrabacion
            .then(ValidarDatosMensura)
            .then((data) => {
                let ignorarConflicto = Promise.resolve();
                /*if (!$("#unidades").DataTable().column(0).data().length) {
                    modalConfirm("", "El plano no posee partidas vinculadas.Desea continuar ?");
                }*/
                if (data && data.error) {
                    ignorarConflicto = new Promise((ok) => {
                        $('#TituloInfoMensura').html("Advertencia - Mensura Existente");
                        $('#DescripcionInfoMensura').html(`${data.mensaje}`);
                        setTimeout(() => $("#ModalInfoMensura").modal('show'), 500);
                    });
                }
                ignorarConflicto.then(() => {
                    if (!$("#unidades").DataTable().column(0).data().length) {
                        modalConfirm("", "El plano no posee partidas vinculadas.Desea continuar ?");
                        $("#btnAdvertenciaOK").off("click").one("click", function () {
                            $("#formMensura").submit();
                        });
                    } else {
                        $("#formMensura").submit();
                    }
                })
            })
            .catch(err => {
                $('#TituloInfoMensura').html("Información - Grabar Mensura");
                $('#DescripcionInfoMensura').html(`Los datos de la mensura están incompletos: ${err}`);
                $("#ModalInfoMensura").modal('show');
            });
        //confirmaGrabacion
        //    .then(() => {
        //        const mensaje = ValidarDatosMensura();
        //        if (mensaje) {
        //            throw mensaje;
        //        }
        //        $("#formMensura").submit();
        //    }).catch(err => {
        //        $('#TituloInfoMensura').html("Información - Grabar Mensura");
        //        $('#DescripcionInfoMensura').html(`Los datos de la mensura están incompletos: ${err}`);
        //        $("#ModalInfoMensura").modal('show');
        //    });
    });

    $("#btn_Modificar").click(function () {
        if (!$(this).hasClass("boton-deshabilitado")) {
            $("#EstadoOperacion").val("Edicion");
            //Modo edición se oculta grilla y se visulizan botones para grabar.
            grillaBusquedaEnableControls(false);
            modoEdicionEnableControls(true);
            $("select", "#collapseMensuraInfo > .panel-body").prop('disabled', false);
            $("input:not(.static),textarea", "#collapseMensuraInfo > .panel-body").prop('readonly', false);
            modoAutomatico = true;
            activarModoMensura(true);
        }
    });

    $("#btn_Agregar").click(function () {
        $("#EstadoOperacion").val("Alta");
        //LIMPIA CONTROLES ?
        InicializaCamposMensura();

        grillaBusquedaEnableControls(false);
        modoEdicionEnableControls(true);
        modoAutomatico = true;
        activarModoMensura();

        $('#unidades').dataTable().api().clear().draw();
        $('#documentos').dataTable().api().clear().draw();
        $('#mensurasRelacionadas').dataTable().api().clear().draw();

        $("select", "#collapseMensuraInfo > .panel-body").prop('disabled', false);
        $("input:not(.static),textarea", "#collapseMensuraInfo > .panel-body").prop('readonly', false);

        // Configura lista de tipo de mensuras
        $("#DatosMensura_TipoDocId").val("1");
        $("#DatosMensura_TipoIdMensura").find("option[value='1']").attr("selected", true);

        // Configurar en modo edición
        $('#ModoEdicion').val("true");
    });

    $("#btn_Eliminar").click(function () {
        if (!$(this).hasClass("boton-deshabilitado")) {
            new Promise((ok) => {
                $('#TituloAdvertenciaMensura').html("Advertencia - Dar de baja");
                $('#DescripcionAdvertenciaMensura').html(`Está a punto de DAR DE BAJA la mensura ${$('#DatosMensura_Descripcion').val()}.<br>¿Desea Continuar?`);
                $("#btnGrabarAdvertenciaMensura").off("click").one("click", ok);
                $('#ModalAdvertenciaMensura').modal('show');
            }).then(() => {
                $.ajax({
                    type: "POST",
                    async: false,
                    url: BASE_URL + 'Mensura/DeleteMensuraJson',
                    dataType: 'json',
                    data: { id: $("#DatosMensura_IdMensura").val() },
                    success: () => {
                        $('#DescripcionInfoMensura').html(`La mensura ha sido eliminada satisfactoriamente.<br/>Es posible que temporalmente aún pueda encontrarse la mensura dado que el buscador está actualizándose.`);
                        $('#TituloInfoMensura').html("Información - Eliminar Mensura");
                        $("#ModalInfoMensura").modal('show');

                        $('#Grilla_mensuras').DataTable().row('.selected').remove().draw(false);

                        resetMensuraControls();
                    },
                    error: (err) => { console.log(err); }
                });
            });
        }
    });

    $("#btnCancelar").click(function () {
        $('#TituloAdvertenciaMensura').html("Advertencia - Cancelar Operación");
        $('#DescripcionAdvertenciaMensura').html("Está a punto de cancelar la operación y se perderán los cambios.<br>¿Desea Continuar?");
        $("#btnGrabarAdvertenciaMensura").off("click").one("click", () => {
            modoAutomatico = true;
            activarModoMensura();
            resetMensuraControls();
        });
        $('#ModalAdvertenciaMensura').modal('show');
    });

    $("#headingMensuraInfo").on("click", () => {
        $("#DatosMensura_IdTipoMensura").select2().val();
    });
    $('#panel_titulo_parcela').click(function () {
        setTimeout(function () {
            $("#unidades").dataTable().api().columns.adjust();
        }, 10);
    });

    $('#panel_titulo_documento').click(function () {
        setTimeout(function () {
            $("#documentos").dataTable().api().columns.adjust();
        }, 10);
    });

    $('#panel_titulo_mensuraRelac').click(function () {
        setTimeout(function () {
            $("#mensurasRelacionadas").dataTable().api().columns.adjust();
        }, 10);
    });

    $("#btnGenerarMensura", ".btn-generacion-mensura").on("click", () => {
        new Promise((ok, err) => {
            $("#btnSeleccionOk", "#mdlSeleccionDepartamento").one("click", () => {
                showLoading();
                $.post(`${BASE_URL}Mensura/GenerarNumero`, { departamento: parseInt($("#cboDepartamentos :selected", "#mdlSeleccionDepartamento").val()) })
                    .done(ok)
                    .fail(err);
            });
            $("#mdlSeleccionDepartamento").one("hidden.bs.modal", () => $("#btnSeleccionOk", "#mdlSeleccionDepartamento").off("click")).modal("show");
        })
            .then((datos) => {
                $("#DatosMensura_Numero").val(datos[0]);
                $("#DatosMensura_Letra").val(datos[1]).trigger("input");
                $("#DatosMensura_Descripcion").val(datos.join("-"));
                $("#btnGenerarMensura", ".btn-generacion-mensura").prop("disabled", true);
            })
            .catch(() => errorAlert("No se pudo generar un número de mensura"))
            .finally(hideLoading);
    });
    $("#btnToggleModoGeneracionMensura").on("click", () => {
        modoAutomatico = !modoAutomatico;
        activarModoMensura();
    });
    setTimeout(function () {
        var mensuraId = $("#IdMensura").val();
        if (mensuraId !== "0") {
            var tabla = $("#Grilla_mensuras").dataTable().api();
            var node = tabla.row.add([mensuraId,
                $("#Descripcion").val(),
                $("#Tipo").val(),
                $("#Estado").val()]).node();
            $(node).find("td:first").addClass("hide");
            tabla.draw();
            $(node).addClass("selected");
            CargarDatosDeLaMensura();
            mensuraEnableControls(true);
        }
    }, 300);

    ////////////////////////////////////////////////////////
    ajustarmodal();
    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-mensura .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-mensura").modal('show');

    modoEdicionEnableControls(false);

}
function resetMensuraControls() {
    $("#EstadoOperacion").val("Consulta");
    grillaBusquedaEnableControls(true);
    modoEdicionEnableControls(false);
    mensuraEnableControls(false);
    $("select", "#collapseMensuraInfo > .panel-body").prop('disabled', true);
    $("input,textarea", "#collapseMensuraInfo > .panel-body").prop('readonly', true);
    $("#EstadoOperacion").val("Consulta");
    $(".panel-collapse").prop('disabled', true);
    $("#btnCerrar").css("display", "");
    InicializaCamposMensura();
    $('#documentos').dataTable().api().clear().draw();
    $("#unidades").dataTable().api().clear().draw();
    $("#mensurasRelacionadas").dataTable().api().clear().draw();
    $('#ModoEdicion').val("false");
}
function dpClearButton(input) {
    setTimeout(function () {
        var buttonPane = $(input).datepicker("widget").find(".ui-datepicker-buttonpane");
        $("<button>", {
            text: "Limpiar",
            click: function () { jQuery.datepicker._clearDate(input); }
        }).appendTo(buttonPane).addClass("ui-datepicker-clear ui-state-default ui-priority-primary ui-corner-all btn btn-secondary");
    }, 1);
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 220,
        altura = viewportHeight - headerFooter;
    $(".mensura-body", "#scroll-content-mensura").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    temp = $(".mensura-body").height();
    var outerHeight = 20;
    $('#accordion-mensura .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-mensura .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.mensura-content').css({ "max-height": temp + 'px' });
    $('#accordion-mensura').css({ "max-height": temp + 1 + 'px' });
    $(".mensura-content").getNiceScroll().resize();
    $(".mensura-content").getNiceScroll().show();
}

function mensuraEnableControls(enable) {
    if (enable) {
        $("#btn_Modificar").removeClass("boton-deshabilitado");
        $("#btn_Eliminar").removeClass("boton-deshabilitado");
        $("#body-content-mensura div[role='region']").removeClass("panel-deshabilitado");
    }
    else {
        $("#btn_Modificar").addClass("boton-deshabilitado");
        $("#btn_Eliminar").addClass("boton-deshabilitado");
        $("#body-content-mensura div[role='region']").addClass("panel-deshabilitado");
        $("#body-content-mensura div[role='region']").find("a:first[aria-expanded=true]").click();
    }
}

function unidadesEnableControls(enable) {
    if (enable) {
        $("#unidades-delete").click(function () {
            if (selectedRowUnidades) {
                var unidad = selectedRowUnidades.find("td").eq(1).html();

                modalConfirm("Eliminar - Unidad Tributaria", "¿Desea eliminar la unidad tributaria " + unidad + "?");

                $("#btnAdvertenciaOK").off("click").one('click', function () {
                    var table = $("#unidades").dataTable().api();
                    table.row(".selected").remove().draw(true);
                    selectedRowUnidades = null;
                    $("#unidades-delete").off("click").addClass("boton-deshabilitado");

                    if (!table.rows().data().length) {
                        $("#btnGrabar").removeClass("boton-deshabilitado");
                    }
                });
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#unidades-delete").off("click").addClass("boton-deshabilitado");
    }
}

function mensurasRelacEnableControls(enable) {
    if (enable) {
        $("#mensurasRelacionadas-delete").click(function () {
            if (selectedRowMensRel) {
                var mensRel = selectedRowMensRel.find("td").eq(1).html();

                modalConfirm("Eliminar - Mensura Relacionada", "¿Desea eliminar la mensura relacionada " + mensRel + "?");

                $("#btnAdvertenciaOK").off("click").one('click', function () {
                    var table = $("#mensurasRelacionadas").dataTable().api();
                    table.row(".selected").remove().draw(false);
                    selectedRowMensRel = null;
                    $("#mensurasRelacionadas-delete").off("click").addClass("boton-deshabilitado");

                    if (!table.rows().data().length) {
                        $("#btnGrabar").removeClass("boton-deshabilitado");
                    }
                });
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#mensurasRelacionadas-delete").off("click").addClass("boton-deshabilitado");
    }
}

function modalConfirm(title, message) {
    $("#TituloAdvertencia").text(title);
    $("#DescripcionAdvertencia").text(message);
    $("#confirmModal").modal("show");
}

function ValidarDatosMensura() {
    return new Promise((ok, error) => {
        let mensajes = [];

        /*if (!$("#unidades").DataTable().column(0).data().length) {
            mensajes.push("Debe cargar al menos un parcela para la mensura.");
        }*/
        if (!$("#DatosMensura_IdTipoMensura").val()) {
            mensajes.push("Debe Ingresar el tipo de mensura");
        }
        /*if ($("#DatosMensura_IdEstadoMensura").val() == 1 && !$("#DatosMensura_FechaAprobacion").val()) {
            mensajes.push("Debe Ingresar la fecha de aprobación");
        }*/
        if (!$("#DatosMensura_Numero").val()) {
            mensajes.push("Debe Ingresar el número de mensura");
        }
        else if ($("#DatosMensura_Numero").val().length > 6) {
            mensajes.push("El número de mensura debe tener como máximo 6 dígitos");
        }
        if (!$("#DatosMensura_Letra").val()) {
            mensajes.push("Debe Ingresar la letra de mensura");
        }
        else if ($("#DatosMensura_Letra").val().length > 2) {
            mensajes.push("La letra de mensura debe tener como máximo 2 dígitos");
        }
        if (!$("#DatosMensura_Descripcion").val()) {
            mensajes.push("Debe Ingresar la descripción");
        }
        if (moment($("#DatosMensura_FechaPresentacion").val(), 'DD-MM-YYYY') > moment($("#DatosMensura_FechaAprobacion").val(), 'DD-MM-YYYY') && $("#DatosMensura_FechaAprobacion").val()) {
            mensajes.push("La fecha de aprobación no puede ser menor a la fecha de presentación");
        }
        if (mensajes.length) {
            error(mensajes.join("<br />"));
        } else {
            $.get(`${BASE_URL}Mensura/ValidarDisponible`, { numero: $("#DatosMensura_Numero").val(), letra: $("#DatosMensura_Letra").val(), id: Number($("#DatosMensura_IdMensura").val()) })
                .done(ok)
                .fail(() => {
                    error("Ha ocurrido un error al validar la disponibilidad del número de mensura.");
                });
        }
    });
}

function modoEdicionEnableControls(enable) {
    if (enable) {
        $("#documentos-insert").removeClass("boton-deshabilitado");
        $("#unidades-insert").removeClass("boton-deshabilitado");
        $("#mensurasRelacionadas-insert").removeClass("boton-deshabilitado");
        $("button", ".btn-generacion-mensura").prop("disabled", false);
        //Habilita los bloques de mensura, parcela y documento.
        $("#body-content-mensura div[role='region']").removeClass("panel-deshabilitado");
    } else {
        $("#documentos-insert").addClass("boton-deshabilitado");
        $("#documentos-delete").addClass("boton-deshabilitado");
        $("#documentos-edit").addClass("boton-deshabilitado");
        $("#unidades-insert").addClass("boton-deshabilitado");
        $("#unidades-delete").addClass("boton-deshabilitado");
        $("#mensurasRelacionadas-insert").addClass("boton-deshabilitado");
        $("#mensurasRelacionadas-delete").addClass("boton-deshabilitado");
        $("button", ".btn-generacion-mensura").prop("disabled", true);
        //Deshabilita los bloques de mensura, parcela y documento.
        $("#body-content-mensura div[role='region']").addClass("panel-deshabilitado");
        $("#body-content-mensura div[role='region']").find("a:first[aria-expanded=true]").click();
    }

    //EnableCamposMensura(enable);
}

function grillaBusquedaEnableControls(enable) {
    if (enable) {
        $("#panel_listado_mensuras").css("display", "");
        $("#Panel_Botones_Mensuras").css("display", "none");
        $("#btnGrabar").css("display", "none");
        $("#btnCancelar").css("display", "none");
        $("#btnCerrar").css("display", "");
    }
    else {
        $("#panel_listado_mensuras").css("display", "none");
        $("#Panel_Botones_Mensuras").css("display", "");
        $("#btnGrabar").css("display", "");
        $("#btnCancelar").css("display", "");
        $("#btnCerrar").css("display", "none");
    }
}

/*REVISAR E INCLUIR LO QUE SEA NECESARIO DENTRO DE LA FUNCION "init"*/
$(document).ready(function () {
    $("#btnSearch").click(function () {
        if ($("#txtFiltroMensura").val()) {
            showLoading();
            $.post(BASE_URL + "Mensura/GetMensurasJson", { descripcion: $("#txtFiltroMensura").val() })
                .done(function (data) {
                    const tabla = $('#Grilla_mensuras').dataTable().api().clear();
                    data.forEach(function (p) {
                        const node = tabla.row.add([p.IdMensura, p.Descripcion, p.TipoMensura.Descripcion, p.EstadoMensura.Descripcion]).node();
                        $(node).find("td:first").addClass("hide");
                    });
                    tabla.draw();
                })
                .fail(function (data) {
                    console.log(data);
                }).always(hideLoading);
        }
    });

    $("#borrarBusqueda").click(function () {
        $("#txtFiltroMensura").val("");

        $("#unidades").dataTable().api().clear().draw();
        $("#Grilla_mensuras").dataTable().api().clear().draw();

        InicializaCamposMensura();
    });
});


$("#btn_Agregar_parcela").click(function () {
    showLoading();
    $('#mensura-externo').load(BASE_URL + 'Parcela/DatosParcela', function () {
        $(document).one("actualizaDataParcela", ActualizaDataParcela);
    });
    parcelaEnableControls(false);
});

$("#btn_Eliminar_parcela").click(function () {
    const partida = $("#unidades").dataTable().api().row(".selected").data()[1];
    $('#TituloAdvertenciaMensura').html("Advertencia - Eliminar parcela");
    $('#DescripcionAdvertenciaMensura').html(`Está a punto de Eliminar la unidad tributaria: ${partida}<br>¿Desea Continuar?`);
    $('#ModalAdvertenciaMensura').modal('show');
});

function Solo_Numerico(variable) {
    if ($("#DatosMensura_TipoDocId").val() === "3" && variable.substr(variable.length - 1, 1) === "-" && (variable.length === 3 || variable.length === 12))
        return variable;
    else {
        return !variable || isNaN(variable) ? "" : parseInt(variable);
    }
}

function ValNumero(Control) {
    Control.value = Solo_Numerico(Control.value);
}

function InicializaCamposMensura() {
    $("#DatosMensura_IdMensura").val("");
    $("#DatosMensura_IdTipoMensura").val("");
    $("#DatosMensura_Numero").val("");
    $("#DatosMensura_Letra").val("");
    $("#DatosMensura_Descripcion").val("");
    $("#DatosMensura_IdEstadoMensura").val("");
    $("#DatosMensura_Observaciones").val("");
    $("#DatosMensura_FechaPresentacion").val("");
    $("#DatosMensura_FechaAprobacion").val("");
}

function EnableCamposMensura(enabled) {
    if (enabled) {
        $("#DatosMensura_IdMensura").removeClass("disabled");
        $("#DatosMensura_IdTipoMensura").removeClass("disabled");
        $("#DatosMensura_Numero").removeClass("disabled");
        $("#DatosMensura_Letra").removeClass("disabled");
        $("#DatosMensura_Descripcion").removeClass("disabled");
        $("#DatosMensura_IdEstadoMensura").removeClass("disabled");
        $("#DatosMensura_Observaciones").removeClass("disabled");
        $("#DatosMensura_FechaPresentacion").removeClass("disabled");
        $("#DatosMensura_FechaAprobacion").removeClass("disabled");
    }
    else {
        $("#DatosMensura_IdMensura").addClass("disabled");
        $("#DatosMensura_IdTipoMensura").addClass("disabled");
        $("#DatosMensura_Numero").addClass("disabled");
        $("#DatosMensura_Letra").addClass("disabled");
        $("#DatosMensura_Descripcion").addClass("disabled");
        $("#DatosMensura_IdEstadoMensura").addClass("disabled");
        $("#DatosMensura_Observaciones").addClass("disabled");
        $("#DatosMensura_FechaPresentacion").addClass("disabled");
        $("#DatosMensura_FechaAprobacion").addClass("disabled");
    }
}

function validarSiNumero(numero) {
    return /^([0-9])*$/.test(numero) ? 1 : 0;
}

function CargarDatosDeLaMensura() {
    var mensuraId = $("#Grilla_mensuras").dataTable().api().row(".selected").data()[0];

    $.post(BASE_URL + "Mensura/GetDatosMensuraJson", { id: mensuraId })
        .done(function (data) {
            $("#DatosMensura_IdMensura").val(data.IdMensura);
            $("#DatosMensura_IdTipoMensura").select2().val(data.IdTipoMensura);
            $("#DatosMensura_Numero").val(data.Numero);
            $("#DatosMensura_Letra").val(data.Letra);
            $("#DatosMensura_Descripcion").val(data.Descripcion);
            $("#DatosMensura_IdEstadoMensura").val(data.IdEstadoMensura);
            $("#DatosMensura_Observaciones").val(data.Observaciones);
            $("#DatosMensura_FechaPresentacion").val(FormatFechaHora(data.FechaPresentacion));
            $("#DatosMensura_FechaAprobacion").val(FormatFechaHora(data.FechaAprobacion));

            $('#unidades').dataTable().api().clear();
            if (data.ParcelasMensuras && data.ParcelasMensuras.length > 0) {
                var idsParcelas = data.ParcelasMensuras.map(obj => obj.IdParcela).join(",");
                $.post(BASE_URL + "Mensura/GetDatosUnidadesByParcelasJson", { idsParcelas: idsParcelas })
                    .done(function (data) {
                        var tabla = $('#unidades').dataTable().api().clear();
                        if (data && data.length > 0) {
                            data.forEach(function (p) {
                                var unidadTributariaId = p.UnidadTributariaId;
                                var idParcela = p.ParcelaID;
                                var codigoProvincial = p.CodigoProvincial;
                                if (idParcela) {
                                    if (p.FechaBaja == null) {
                                        aux = '<span onclick="fnClickMasInfo(' + idParcela + ')" class="fa fa-th black cursor-pointer" aria-hidden="true"></span>'; 
                                    } else {
                                        aux = '<span class="badge bg-danger" style="background-color: red; color: white">DADA DE BAJA</span>';
                                    }
                                }
                                else {
                                    aux = '<span class="fa fa-th black cursor-pointer boton-deshabilitado" aria-hidden="true"></span>';
                                }
                                var node = tabla.row.add([unidadTributariaId, codigoProvincial, aux
                                    , '<input type="hidden" id="UnidadTributaria_' + unidadTributariaId + '" name="UnidadTributariaId" value="' + unidadTributariaId + '" >'
                                    , idParcela]).node();
                                $(node).find("td:first").addClass("hide");
                                $(node).find("td:eq(3)").addClass("hide");
                                $(node).find("td:eq(4)").addClass("hide");
                            });
                            tabla.order([1, 'asc']).draw().columns.adjust();
                        }

                    })
                    .fail(function (data) {
                        console.log(data);
                    });
            }

            var tablaDoc = $('#documentos').dataTable().api().clear();
            if (data.MensurasDocumentos && data.MensurasDocumentos.length) {
                data.MensurasDocumentos.forEach(function (p) {
                    var node = tablaDoc.row.add([p.Documento.id_documento, p.Documento.Tipo.Descripcion, p.Documento.descripcion, p.Documento.fecha, p.Documento.nombre_archivo, p.Documento.id_documento]).node();
                    $(node).find("td:first").addClass("hide");
                    $(node).find("td:eq(5)").addClass("hide");
                });
            }
            tablaDoc.order([2, 'asc']).draw().columns.adjust();


            var tablaMenRel = $('#mensurasRelacionadas').dataTable().api().clear();
            if (data.MensurasRelacionadasOrigen && data.MensurasRelacionadasOrigen.length > 0) {
                data.MensurasRelacionadasOrigen.forEach(function (p) {
                    var idMensura = p.IdMensuraOrigen;
                    var node = tablaMenRel.row.add([idMensura, p.MensuraOrigenDescripcion, p.MensuraOrigenTipo, p.MensuraOrigenEstado, p.IdMensuraOrigen, p.IdMensuraDestino]).node();
                    $(node).find("td:first").addClass("hide");
                    $(node).find("td:eq(4)").addClass("hide");
                    $(node).find("td:eq(5)").addClass("hide");
                });
            }
            if (data.MensurasRelacionadasDestino && data.MensurasRelacionadasDestino.length > 0) {
                data.MensurasRelacionadasDestino.forEach(function (p) {
                    var idMensura = p.IdMensuraDestino;
                    var node = tablaMenRel.row.add([idMensura, p.MensuraDestinoDescripcion, p.MensuraDestinoTipo, p.MensuraDestinoEstado, p.IdMensuraOrigen, p.IdMensuraDestino]).node();
                    $(node).find("td:first").addClass("hide");
                    $(node).find("td:eq(4)").addClass("hide");
                    $(node).find("td:eq(5)").addClass("hide");
                });
                tablaMenRel.order([1, 'asc']);
            }

            tablaMenRel.order([1, 'asc']).draw().columns.adjust();
        })
        .fail(function (data) {
            console.log(data);
        });
}

function runSearch(e) {
    if (e.keyCode === 13) {
        $("#btnSearch").click();
        return false;
    }
}

function fnClickMasInfo(valor) {
    showLoading();
    setTimeout(function () {
        $("#parcela-grafica-externo-container").load(BASE_URL + "MantenimientoParcelario/Get/" + valor);
    }, 10);
}

function ActualizaDataParcela(evt) {
    var tabla = $("#unidades").dataTable().api();
    if (evt.parcela.id_parcela && parseInt(evt.parcela.id_parcela) !== 0)
        tabla.row('.selected').remove().draw(false);
    var node = tabla.row.add([evt.parcela.id_parcela, '<input type="hidden" id="Parcela_' + evt.parcela.id_registro + '" name="ParcelaMensura" value="' + evt.parcela.XMLParcela + '" >',
    evt.parcela.id_registro, evt.parcela.desc, evt.parcela.desc_parcela, evt.parcela.desc_cp, evt.parcela.desc_localidad, evt.parcela.clave]).node();
    $(node).find("td:first").addClass("hide");
    $(node).find("td:eq(1)").addClass("hide");
    $(node).find("td:eq(2)").addClass("hide");
    $(node).find("td:eq(7)").addClass("hide");
    tabla.draw();
}

function unidadesInit() {
    $("#unidades tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowUnidades = null;
            unidadesEnableControls(false);
        } else {
            $("tr.selected", "#unidades tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowUnidades = $(this);

            unidadesEnableControls(!selectedRowUnidades.children().hasClass("dataTables_empty") && $("#EstadoOperacion").val() !== "Consulta");
        }
    });

    createDataTable('unidades', {
        columnDefs: [
            { "targets": 1, "width": "auto" },
            { "targets": 2, "width": "20px" }
        ],
        dom: "trp",
        pageLength: 10,
        scrollX: false,
        scrollCollapse: true
    }, 150);

    $("#unidades-insert").click(function () {
        buscarUnidadesTributarias(true)
            .then(function (seleccion) {
                if (seleccion) {
                    $.post(`${BASE_URL}Mensura/GetUnidadesTributarias`, { "ids[]": seleccion }, (uts) => {
                        const rows = uts.map(ut => {
                            let iconoMantenedor = '<span class="fa fa-th black cursor-pointer boton-deshabilitado" aria-hidden="true"></span>';
                            if (ut.ParcelaID) {
                                if (ut.FechaBaja == null) {
                                    iconoMantenedor = `<span onclick="fnClickMasInfo(${ut.ParcelaID})" class="fa fa-th black cursor-pointer" aria-hidden="true"></span>`;
                                } else {
                                    iconoMantenedor = '<span class="badge bg-danger" style="background-color: red; color: white">DADA DE BAJA</span>';
                                }
                            }
                            return [
                                ut.UnidadTributariaId,
                                ut.CodigoProvincial,
                                iconoMantenedor,
                                `<input type="hidden" id="UnidadTributaria_${ut.UnidadTributariaId}" name="UnidadTributariaId" value="${ut.UnidadTributariaId}">`,
                                ut.ParcelaID
                            ];
                        });
                        const tabla = $("#unidades").dataTable().api();
                        tabla.rows.add(rows).nodes().to$().each((_, row) => {
                            $("td:first", row).addClass("hide");
                            $("td:eq(3)", row).addClass("hide");
                            $("td:eq(4)", row).addClass("hide");
                        });
                        tabla.order([1, "asc"]).draw();
                        setTimeout(() => tabla.columns.adjust(), 100);
                        $("#unidad-tributaria-error").addClass("hide");
                        $("#save-all").removeClass("boton-deshabilitado");
                    });
                }
            })
            .catch(function (err) { console.log(err); });
    });
}

function buscarUnidadesTributarias(multiselect) {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.UnidadesTributarias + ", " + BuscadorTipos.Historicas, multiSelect: multiselect, verAgregar: false, titulo: 'Buscar Unidad Tributaria', campos: ['Nombre'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    if (multiselect) {
                        resolve(evt.seleccion.map(o => o[2]));
                    } else {
                        resolve(evt.seleccion.slice(1));
                    }
                } else {
                    resolve();
                }
            });
        });
    });
}

function buscarMensuras(multiselect) {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Mensuras, multiSelect: multiselect, verAgregar: false, titulo: 'Buscar Mensura', campos: ['Descripcion'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    if (multiselect) {
                        resolve(evt.seleccion.map(o => o[2]));
                    } else {
                        resolve(evt.seleccion.slice(1));
                    }
                } else {
                    resolve();
                }
            });
        });
    });
}

function mensurasRelacInit() {
    $("#mensurasRelacionadas tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowMensRel = null;
            mensurasRelacEnableControls(false);
        } else {
            $("tr.selected", "#mensurasRelacionadas tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowMensRel = $(this);

            mensurasRelacEnableControls(!selectedRowMensRel.children().hasClass("dataTables_empty") && $("#EstadoOperacion").val() !== "Consulta");
        }
    });

    createDataTable('mensurasRelacionadas', {
        columnDefs: [
            { "targets": 1, "width": "40%" },
            { "targets": 2, "width": "35%" }
        ],
        dom: "trp",
        pageLength: 10,
        scrollX: false,
        scrollCollapse: true
    }, 150);

    $("#mensurasRelacionadas-insert").click(function () {
        buscarMensuras(true)
            .then(function (seleccion) {
                if (seleccion) {
                    $.post(`${BASE_URL}Mensura/GetMensurasDetalleByIds`, { "ids[]": seleccion }, (mensuras) => {
                        let mensuraId = Math.floor(Math.random() * 10) - 9;
                        const rows = mensuras.map(mensura => {
                            --mensuraId;
                            return [
                                mensura.IdMensura,
                                mensura.Descripcion,
                                mensura.Tipo,
                                mensura.Estado,
                                mensura.IdMensura/*`<input type="hidden" id="MensuraRelacOrigenId_${mensuraId}" name="MensuraRelacOrigenId" value="${mensura.IdMensura}">`*/,
                                mensuraId/*`<input type="hidden" id="MensuraRelacDestinoId_${mensuraId}" name="MensuraRelacDestinoId" value="${mensuraId}">`*/
                            ];
                        });
                        const tabla = $("#mensurasRelacionadas").dataTable().api();
                        tabla.rows.add(rows).nodes().to$().each((_, row) => {
                            $("td:first", row).addClass("hide");
                            $("td:eq(4)", row).addClass("hide");
                            $("td:eq(5)", row).addClass("hide");
                        });
                        //tabla.order([1, "asc"]).draw().columns.adjust();
                        tabla.order([1, "asc"]).draw();
                        setTimeout(() => tabla.columns.adjust(), 100);
                        $("#btnGrabar").removeClass("boton-deshabilitado");
                    });
                }
            })
            .catch(function (err) { console.log(err); });
    });
}

function destroyDataTable(tableId) {
    $("#" + tableId).dataTable().api().destroy();
}

function createDataTable(id, options) {
    var defaults = {
        language: {
            url: BASE_URL + 'Scripts/dataTables.spanish.txt'
        },
        dom: "tr",
        scrollY: "150px",
        scrollX: true,
        orderCellsTop: true,
        destroy: true,
        bAutoWidth: false
    };
    $('#' + id).DataTable(Object.assign(defaults, options || {}));
}

function documentosInit() {
    $("#documentos tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowDocumentos = null;
            documentosEnableControls(false);
        } else {
            $("tr.selected", "#documentos tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowDocumentos = $(this);

            documentosEnableControls(!selectedRowDocumentos.children().hasClass("dataTables_empty") && $("#EstadoOperacion").val() !== "Consulta");
        }
    });

    $("#documentos-insert").click(function () {
        selectedRowDocumentos = unselectRow(selectedRowDocumentos);
        $.ajax({
            url: `${BASE_URL}Documento/Editable`,
            type: "POST",
            success: () => {
                showLoading();
                $("#contenedor-forms-externos").load(BASE_URL + 'Documento/DatosDocumento', function () {
                    $(document).one("documentoGuardado", function (evt) {
                        documentoGuardado(evt.documento);
                    });
                });
            },
            error: hideLoading
        });
    });

    createDataTable('documentos', {
        columnDefs: [
            { "targets": 1, "width": "20%" },
            { "targets": 2, "width": "20%" },
            {
                "targets": 3, "width": "10%", "render": function (data) {
                    return FormatFechaHora(data);
                }
            },
        ],
        dom: "trp",
        pageLength: 10,
        scrollX: false,
        scrollCollapse: true
    }, 150);
}

function documentosEnableControls(enable) {
    if (enable) {
        $("#documentos-delete").click(function () {
            if (selectedRowDocumentos) {
                const tipo = selectedRowDocumentos.find("td").eq(1).html();
                const descripcion = selectedRowDocumentos.find("td").eq(2).html();
                const fecha = selectedRowDocumentos.find("td").eq(3).html();
                modalConfirm("Eliminar - Documento", `¿Desea eliminar el documento tipo ${tipo}, descripción ${descripcion} fecha ${fecha}?`);

                $("#btnAdvertenciaOK").off("click").one("click", function () {
                    var table = $("#documentos").dataTable().api();
                    table.row(".selected").remove().draw(false);
                    selectedRowDocumentos = null;
                    $("#documentos-delete").off("click").addClass("boton-deshabilitado");
                    $("#documentos-edit").off("click").addClass("boton-deshabilitado");
                    $("#documentos-view").off("click").addClass("boton-deshabilitado");
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#documentos-edit").off("click").click(function () {
            if (selectedRowDocumentos) {
                $.ajax({
                    url: `${BASE_URL}Documento/Editable`,
                    type: "POST",
                    success: () => {
                        const id = selectedRowDocumentos.find("td").eq(0).html();
                        showLoading();
                        $("#contenedor-forms-externos").load(`${BASE_URL}Documento/DatosDocumento/${id}`, function () {
                            $(document).one("documentoGuardado", function (evt) {
                                documentoGuardado(evt.documento);
                            });
                        });
                    },
                    error: hideLoading
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#documentos-view").click(function () {
            if (selectedRowDocumentos) {
                $.ajax({
                    url: `${BASE_URL}Documento/SoloLectura`,
                    type: "POST",
                    success: () => {
                        const id = selectedRowDocumentos.find("td").eq(0).html();
                        showLoading();
                        $("#contenedor-forms-externos").load(`${BASE_URL}Documento/DatosDocumento/${id}`);
                    }
                });
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#documentos-delete").off("click").addClass("boton-deshabilitado");
        $("#documentos-edit").off("click").addClass("boton-deshabilitado");
        $("#documentos-view").off("click").addClass("boton-deshabilitado");
    }
}

function documentoGuardado(data) {
    debugger;
    var table = $("#documentos").dataTable().api();
    if (selectedRowDocumentos) {
        var selectedRow = table.row(".selected");
        var d = selectedRow.data();
        d[1] = data.Tipo.Descripcion;
        d[2] = data.descripcion;
        d[3] = FormatFechaHora(data.fecha);
        d[4] = data.nombre_archivo;
        selectedRow.data(d).draw();
    } else {
        $.ajax({
            type: "POST",
            url: BASE_URL + "MensuraDocumento/Save?idDocumento=" + data.id_documento,
            success: function (responseText) {
                if (responseText === "Ok") {
                    var node = table.row.add([
                        data.id_documento,
                        data.Tipo.Descripcion,
                        data.descripcion,
                        FormatFechaHora(data.fecha),
                        data.nombre_archivo,
                        data.id_documento /*'<input type="hidden" id="Documento_' + data.id_documento + '" name="DocumentoId" value="' + data.id_documento + '" >'*/
                    ]).draw().node();
                    $(node).find("td:first").addClass("hide");
                    $(node).find("td:eq(5)").addClass("hide");

                    setTimeout(() => $("#documentos").dataTable().api().columns.adjust(), 100);
                    $("#save-all").removeClass("boton-deshabilitado");
                } else {
                    errorAlert(responseText);
                }
            }
        });
    }
}

function unselectRow(selectedRow) {
    if (selectedRow) {
        selectedRow.removeClass("selected");
        return null;
    }
}

function descripcionChange() {
    let descripcion = `${$("#DatosMensura_Numero").val()}-${$("#DatosMensura_Letra").val()}`.trim();
    if (descripcion === "-") {
        descripcion = "";
    }
    $("#DatosMensura_Descripcion").val(descripcion.toUpperCase());
}

//# sourceURL=mensuras.js