var timeout;
var gridPendIni = 0;
var gridProcIni = 0;
var gridReasgIni = 0;
var aTramite = [];
var comandoSelec = '';
var comandoSelecTexto = '';

$(window).resize(ajustarmodal);
$(document).ready(init);

$('#modal-mesaentradas').one('shown.bs.modal', function () {
    ajustarmodal();
    hideLoading();
});

function init() {
    $("#divTabContent").niceScroll(getNiceScrollConfig());
    inicializarGrilla('Grilla_mesaEntradas', 'tramites', [{ pos: 6, col: { name: "SectorOrigen", data: "SectorOrigen" } }], [9, 'desc']);

    var esProfesional = parseInt($("#hdnEsProfesional").val());
    if (esProfesional === 0) {
        $('#tabsTramites > li:nth-child(2) > a').one("click", refreshGridPendientes);
        $('#tabsTramites > li:nth-child(3) > a').one("click", refreshGridProcesados);
        $('#tabsTramites > li:nth-child(4) > a').one("click", refreshGridReasignados);
        $('#tabsTramites > li').on('click', function () {
            rowClick();
            $('.mesaEntradas-body .tab-content tbody tr').removeClass('selected');
            setTimeout(ajustarmodal, 100);
        });
    }

    $("#modal-mesaentradas .date .form-control").datepicker(getDatePickerConfig({ clearBtn: true }));

    $('#modal-mesaentradas').modal("show");
}

$("#refreshButton").click(function () {
    var esProfesional = parseInt($("#hdnEsProfesional").val());
    if (esProfesional === 1) {
        refreshGridEnProceso();
    }
    else {
        switch ($("li.active", "#tabsTramites").data('tab')) {
            case 'enProceso':
                refreshGridEnProceso();
                break;
            case 'pendientes':
                refreshGridPendientes(true);
                break;
            case 'procesados':
                refreshGridProcesados(true);
                break;
            case 'reasignados':
                refreshGridReasignados(true);
                break;
            default:
        }
    }
});
function refreshGridEnProceso() {
    $('#Grilla_mesaEntradas').DataTable().ajax.reload();
}
function refreshGridPendientes(forceRefresh) {
    if (gridPendIni === 0) {
        inicializarGrilla('Grilla_mesaEntradasPendientes', 'tramitespendientes', [{ pos: 6, col: { name: "SectorOrigen", data: "SectorOrigen" } }], [9, 'desc']);
        gridPendIni = 1;
    } else if (forceRefresh) {
        $('#Grilla_mesaEntradasPendientes').DataTable().ajax.reload();
    }
}
function refreshGridProcesados(forceRefresh) {
    if (gridProcIni === 0) {
        inicializarGrilla('Grilla_mesaEntradasProcesados', 'tramitesprocesados', [{ pos: 6, col: { name: "SectorDestino", data: "SectorDestino" } }]);
        gridProcIni = 1;
    } else if (forceRefresh) {
        $('#Grilla_mesaEntradasProcesados').DataTable().ajax.reload();
    }
}
function refreshGridReasignados(forceRefresh) {
    if (gridReasgIni === 0) {
        inicializarGrilla('Grilla_mesaEntradasReasignados', 'tramitesreasignados', [{ pos: 6, col: { name: "SectorOrigen", data: "SectorOrigen" } }]);
        gridReasgIni = 1;
    } else if (forceRefresh) {
        $('#Grilla_mesaEntradasReasignados').DataTable().ajax.reload();
    }
}
function refreshGrids() {
    refreshGridEnProceso();
    refreshGridPendientes(true);
    refreshGridProcesados(true);
    refreshGridReasignados(true);
}

function cargarAcciones(tabla) {
    aTramite = [];
    var onFinally = () => { };
    if (tabla) {
        aTramite = Array.apply(null, tabla.dataTable().api().rows('.selected').data()).reduce((acc, elem) => [...acc, elem.IdTramite], []);
        $(tabla).css("cursor", "wait");
        $("tbody tr", tabla).css("pointer-events", "none");
        onFinally = () => {
            $(tabla).css("cursor", "");
            $("tbody tr", tabla).css("pointer-events", "auto");
        };
    }

    var cargas = [];
    cargas.push(new Promise(function (resolve, reject) {
        $.post(BASE_URL + 'mesaentradas/cargaraccionesGenerales', { cantTramites: aTramite.length, grillaReasignable: !!(tabla && tabla.prop("id") === "Grilla_mesaEntradasReasignados") })
            .done(resolve)
            .fail(reject);
    }));
    cargas.push(new Promise(function (resolve, reject) {
        $.post(BASE_URL + 'mesaentradas/cargaracciones', { tramites: aTramite, grillaReasignable: !!(tabla && tabla.prop("id") === "Grilla_mesaEntradasReasignados") })
            .done(resolve)
            .fail(reject);
    }));
    Promise.all(cargas)
        .then(([generales, puntuales]) => {
            var acciones = [];
            var agregarAccion = accion => {
                acciones.push('<li><a href="javascript:void(0)" data-tipo-movimiento="' + accion.IdTipoMovimiento + '" onclick="javascript:ejecutar(this);">' + accion.Descripcion + '</a></li>');
            };
            var agregarSeparador = () => {
                acciones.push('<li><div class="dropdown-divider"></div></li>');
            };
            $(".btn-acciones > ul").empty();
            if (generales.length || puntuales.length) {
                $(".btn-acciones").show();
            }
            var primeras = [], ultimas = [];
            var puntualesPrimeras = [], puntualesUltimas = [];
            if (generales.length) {
                primeras = generales.filter(accion => accion.IdTipoMovimiento < 100 && (accion.IdTipoMovimiento !== EnumAcciones.Editar || $("li.active", "#tabsTramites").data('tab') === "enProceso"));
                ultimas = generales.filter(accion => accion.IdTipoMovimiento >= 100);
                primeras.forEach(agregarAccion);
            }
            if (puntuales.length) {
                puntualesPrimeras = puntuales.filter(accion => accion.IdTipoMovimiento < 100);
                puntualesUltimas = puntuales.filter(accion => accion.IdTipoMovimiento >= 100);
                //puntuales.forEach(agregarAccion);
                if (puntualesPrimeras.length) {
                    agregarSeparador();
                }
                puntualesPrimeras.forEach(agregarAccion);
            }
            [...ultimas.filter(a => a.IdTipoMovimiento === 102), ...puntualesUltimas.filter(a => a.IdTipoMovimiento === 104)].forEach(accion => {
                agregarSeparador();
                agregarAccion(accion);
            })
            ultimas.forEach(accion => {
                if (accion.IdTipoMovimiento != 102) {
                    agregarAccion(accion);
                }
            });
            puntualesUltimas.forEach(accion => {
                if (accion.IdTipoMovimiento != 104) {
                    agregarAccion(accion);
                }
            });
            $(".btn-acciones > ul").html(acciones.join(""));
        })
        .catch(err => console.log(err))
        .finally(onFinally);
}
function rowClick(evt) {
    clearTimeout(timeout);
    var tabla;
    if (evt) {
        $(evt.currentTarget).toggleClass('selected');
        tabla = $(evt.currentTarget).parents("table");
    }
    timeout = setTimeout(cargarAcciones, 500, tabla);
}
var EnumAcciones = {
    None: 0,
    Crear: 1,
    Editar: 2,
    Anular: 3,
    Derivar: 4,
    Recibir: 5,
    Anular_derivacion: 6,
    Finalizar: 7,
    Despachar: 8,
    Rechazar: 9,
    Reingresar: 10,
    Presentar: 11,
    Confirmar: 12,
    Anular_Carga_Libro: 13,
    Archivar: 14,
    Desarchivar: 15,
    Recibir_Presentado: 16,
    Entregar: 17,
    Procesar: 18,
    Reasignar: 19,
    Imprimir_Caratula: 100,
    Imprimir_Informe_Detallado: 101,
    Consultar: 102,
    Imprimir_Informe_Adjudicacion: 103,
    Notificar: 104

};
function ejecutar(target) {
    var idTramite, operacion;
    comandoSelec = $(target).data('tipoMovimiento');
    comandoSelecTexto = $(target).html();
    switch (comandoSelec) {
        case EnumAcciones.Crear:
            idTramite = 0;
            operacion = 'Alta';
            callEjecutarEditar(idTramite, operacion);
            break;
        case EnumAcciones.Editar:
            idTramite = 0;
            operacion = 'Alta';
            if (aTramite.length > 0) {
                idTramite = aTramite[0];
                operacion = 'Modificacion';
            }
            callEjecutarEditar(idTramite, operacion);
            break;
        case EnumAcciones.Consultar:
            idTramite = 0;
            operacion = 'Consulta';
            if (aTramite.length > 0) {
                idTramite = aTramite[0];
                operacion = 'Consulta';
            }
            callEjecutarEditar(idTramite, operacion);
            break;
        case EnumAcciones.Reingresar:
        case EnumAcciones.Archivar:
        case EnumAcciones.Desarchivar:
        case EnumAcciones.Finalizar:
        case EnumAcciones.Entregar:
            if (comandoSelecTexto === "FINALIZAR") {
                modalConfirmAcciones("¿Desea finalizar el trámite con los datos generados?", toCamelCase(comandoSelecTexto))
                $("#btnAceptarAdvertenciaAcciones").off("click").one("click", function () {
                    showModalObservacion(comandoSelecTexto, 'Observación:', callEjecutarAccion);
                });
            } else {
                showModalObservacion(comandoSelecTexto, 'Observación:', callEjecutarAccion);
            }
            break;
        case EnumAcciones.Imprimir_Caratula:
            imprimirCaratula();
            break;
        case EnumAcciones.Imprimir_Informe_Detallado:
            imprimirInformeDetallado();
            break;
        case EnumAcciones.Reasignar:
            showModalObservacion(comandoSelecTexto, 'Observación:', callEjecutarAccion);
            break;
        case EnumAcciones.Derivar:
        case EnumAcciones.Despachar:
            showModalObservacion(comandoSelecTexto, 'Observación:', true, callEjecutarAccion);
            break;
        case EnumAcciones.Rechazar:
            showModalObservacion(comandoSelecTexto, 'Motivo del Rechazo:', callEjecutarAccion);
            break;
        case EnumAcciones.Recibir_Presentado:
        case EnumAcciones.Confirmar:
        case EnumAcciones.Anular_derivacion:
        case EnumAcciones.Recibir:
        case EnumAcciones.Procesar:
        case EnumAcciones.Presentar:
            if (comandoSelecTexto === "PROCESAR") {
                modalConfirmAcciones("¿Desea procesar el trámite con los datos generados?", toCamelCase(comandoSelecTexto))
                $("#btnAceptarAdvertenciaAcciones").off("click").one("click", function () {
                    callEjecutarAccion();
                });
            } else {
                callEjecutarAccion();
            }
            break;
        case EnumAcciones.Anular_Carga_Libro:
        case EnumAcciones.Anular:
            if (comandoSelecTexto === "ANULAR") {
                modalConfirmAcciones("¿Desea anular el trámite? Esto borrará sus datos asociados.", toCamelCase(comandoSelecTexto))
                $("#btnAceptarAdvertenciaAcciones").off("click").one("click", function () {
                    showModalObservacion(comandoSelecTexto, 'Motivo de Anulación:', callEjecutarAccion);
                });
            } else {
                showModalObservacion(comandoSelecTexto, 'Motivo de Anulación:', callEjecutarAccion);
            }
            break;
        case EnumAcciones.Imprimir_Informe_Adjudicacion:
            imprimirInformeAdjudicacion();
            break;
        case EnumAcciones.Notificar:
            notificarTramite();
            break;
        default:
            break;
    }
    return false;
}
function callEjecutarAccion() {
    var sector = '';
    var usuario = '';
    var observ = '';
    var comando = comandoSelec;
    var comandoTexto = comandoSelecTexto;
    usuario = $('#cboUsuariosInfoObservaciones').val();
    sector = $('#cboSectorInfoObservaciones').val();
    observ = $('#txtObservacionInfoObservaciones').val();
    $("#ModalObservaciones").modal('hide');
    if (!sector && !$('#cboSectorInfoObservaciones').is(':hidden')) {
        mostrarMensajeMain(2, toCamelCase(comandoTexto), "Debe ingresar el sector");
        return false;
    }
    if (!usuario && !$('#cboUsuariosInfoObservaciones').is(':hidden')) {
        mostrarMensajeMain(2, toCamelCase(comandoTexto), "Debe ingresar el Usuario");
        return false;
    }
    showLoading();
    $.post(BASE_URL + 'mesaentradas/ejecutaraccion', {
        accion: comando,
        tramites: aTramite,
        sector: sector,
        usuario: usuario,
        observ: observ
    }).done(function (data) {
        if (!!data && data.error) {
            var mensajes = ["No se pudieron guardar los cambios. Verifique lo siguiente:", ""];
            mostrarMensajeMain(3, toCamelCase(comandoTexto), [...mensajes, ...data.mensajes].join("<br />"));
            return;
        }
        let func;
        if (comandoTexto === "FINALIZAR" || comandoTexto === "PROCESAR") {
            const tramites = [...aTramite];
            func = () => notificarTramite(tramites);
        }
        refreshGrids();
        if (comandoTexto === "ANULAR") {
            mostrarMensajeMain(1, toCamelCase(comandoTexto), "El trámite se ha anulado correctamente.", func);
        } else {
            mostrarMensajeMain(1, toCamelCase(comandoTexto), "Los cambios se guardaron correctamente.", func);
        }
    }).fail(function () {
        mostrarMensajeMain(3, toCamelCase(comandoTexto), "Ha ocurrido un error al ejecutar la acción.");
    }).always(hideLoading);
}
function callEjecutarEditar(idTramite, operacion) {
    showLoading();
    $(window)
        .off("RefrescarGrilla")
        .on("RefrescarGrilla", function () {
            refreshGridEnProceso();
        });
    $("#divEdicionTramiteContainer").load(`${BASE_URL}MesaEntradas/EdicionTramite?idTramite=${idTramite}&operacion=${operacion}`);
}
function imprimirCaratula() {
    if (aTramite.length) {
        idTramite = aTramite[0];
        showLoading();
        window.open(`${BASE_URL}MesaEntradas/GetInformeCaratulaExpediente/${idTramite}`, "_blank");
        hideLoading();
    }
}
function imprimirInformeDetallado() {
    if (aTramite.length) {
        idTramite = aTramite[0];
        showLoading();
        $.get(`${BASE_URL}MesaEntradas/GetInformeDetallado/${idTramite}`, () => {
            window.open(`${BASE_URL}MesaEntradas/AbrirInformeDetallado/`, "_blank");
            hideLoading();
        });
    }
}
function imprimirInformeAdjudicacion() {
    if (aTramite.length) {
        idTramite = aTramite[0];
        showLoading();
        window.open(`${BASE_URL}MesaEntradas/GetInformeAdjudicacion/${idTramite}`, "_blank");
        hideLoading();
    }
}

function modalConfirmEmail(message) {
    $("#DescripcionAdvertenciaEmail").text(message);
    $("#mensajeModalEnviarMail").modal('show');
}

function modalConfirmAcciones(message, title) {
    $("#DescripcionAdvertenciaAcciones").html(message);
    $("#TituloAdvertenciaAcciones").text(title);
    $("#mensajeModalAcciones").modal('show');
}

function getMensajeNoEmail(idTramite) {
    if (idTramite) {
        $.ajax({
            type: "POST",
            url: `${BASE_URL}MesaEntradas/GetTramiteByIdEmail?idTramite=${idTramite}`,
            success: function (tramite) {
                mostrarMensajeMain(2, "Enviar Email", `El iniciador del trámite ${tramite.Numero} no posee registrada una dirección de email. No es posible enviar la notificación.`);
            },
            error: function (_, __, errorThrown) {
                console.log(errorThrown);
            }
        });
    }
}

function notificarTramite(tramites) {
    tramites = tramites || aTramite;
    if (tramites.length) {
        idTramite = tramites[0];
        $.ajax({
            type: "POST",
            url: BASE_URL + "MesaEntradas/GetPersonaByIdTramite?idTramite=" + idTramite,
            success: function (persona) {
                if (persona.Email) {
                    modalConfirmEmail("¿Desea notificar del estado del trámite al iniciador " + persona.NombreCompleto + "?");
                    $("#btnAceptarAdvertenciaEnviarMail").off("click").one("click", function () {
                        $.ajax({
                            url: BASE_URL + "MesaEntradas/GetEmailTramite?idTramite=" + idTramite.toString(),
                            type: "GET",
                            success: function () {
                                mostrarMensajeMain(1, "Enviar Email", "Notificación enviada.");
                            },
                            error: function () {
                                mostrarMensajeMain(3, "Error", "No se pudo enviar el email al iniciador. Se requiere el envio manual. El email del iniciador es: " + persona.Email);
                            }
                        });
                    });
                } else {
                    getMensajeNoEmail(idTramite);
                }
            },
            error: function () {
                mostrarMensajeMain(3, "Error", "No se pudo recuperar los datos del iniciador.");
            },
            complete: hideLoading
        });
    }
}

function mostrarMensajeMain(result, title, description, func) {
    var modal = $("#mensajeModal", "#contenido").off("hidden.bs.modal");
    modal.find('#TituloAdvertencia').html(title);
    modal.find('#DescripcionAdvertencia').html(description);
    var alertaBackground = $('div[role="alert"]', modal);
    alertaBackground.removeClass('alert-warning alert-danger alert-success');
    switch (result) {
        case 1:
            alertaBackground.addClass('alert-success');
            break;
        case 2:
            alertaBackground.addClass('alert-warning');
            break;
        case 3:
            alertaBackground.addClass('alert-danger');
            break;
    }
    if (typeof func === "function") {
        $("#mensajeModal", "#contenido").one("hidden.bs.modal", func);
    }
    modal.modal('show');
}

function ajustarmodal() {
    var altura = $(window).height() - 150; //value corresponding to the modal heading + footer
    $(".mesaEntradas-body").css({ "height": altura, "overflow-y": "auto" });
    $('#divTabContent').css({ "max-height": altura - 141 + 'px' });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $('#divTabContent').getNiceScroll().resize();
    $('#divTabContent').getNiceScroll().show();
}
function inicializarGrilla(grilla, url, customCols, ordercols) {
    ordercols = ordercols || [0, 'asc'];
    var cols = [
        { name: "IdTramite", data: "IdTramite", width: "75px" },
        { name: "Numero", data: "Numero", defaultContent: "", width: "95px" },
        { name: "Iniciador", data: "Iniciador", defaultContent: "", width: "95px" },
        { name: "Tipo", data: "Tipo" },
        { name: "Objeto", data: "Objeto" },
        { name: "Estado", data: "Estado" },
        { name: "Prioridad", data: "Prioridad" },
        { name: "FechaUltAct", data: "FechaUltAct" },
        { name: "FechaVenc", data: "FechaVenc" }
    ];
    cols = customCols
        .sort(function (a, b) { return (a.pos - b.pos) * -1; })
        .reduce(function (accum, elem) {
            return accum.slice(0, elem.pos).concat(elem.col).concat(accum.slice(elem.pos));
        }, cols);
    $('#' + grilla).dataTable({
        language: {
            url: BASE_URL + 'Scripts/dataTables.spanish.txt'
        },
        dom: "tr<'row'<'col-sm-6'i><'col-sm-6'p>>",
        order: ordercols,
        orderCellsTop: true,
        serverSide: true,
        processing: true,
        paging: true,
        pageLength: 10,
        ajax: {
            url: BASE_URL + 'mesaentradas/' + url,
            method: 'POST',
            // Ticket S009 - No mostrar los anulados al menos que se seleccione del filtro
            // Se genera función dataFilter para filtrar la data y mostrar los anulados 
            // solo en caso de haberse seleccionado del input select de estados
            dataFilter: function (data) {
                var response = JSON.parse(data);
                const estadosSelect = document.getElementById('estados');
                response.data = response.data.filter(function (item) {
                    if (estadosSelect.value === '6') {
                        return item.Estado;
                    } else {
                        return (item.Estado !== 'ANULADO' && item.Estado);
                    }
                    
                });
                return JSON.stringify(response);
            }
            //-----------------------------------------------------------------------------
        },
        columns: cols,
        createdRow: function (row) {
            $(row).on('click', rowClick);
        },
        drawCallback: ajustarScrollBars,
        initComplete: function () {
            var tablaDOM = this,
                tabla = $(tablaDOM).DataTable();

            // Ticket S009 - No mostrar los anulados al menos que se seleccione del filtro
            // Se obtiene el input select de Estados 
            const estadosSelect = document.getElementById('estados');
            //---------------------------------------------------------------------------

            tabla.columns.adjust().draw();

            var filterChanged = function (evt) {
                var col = tabla.column($(evt.target).parents("th").index());
                if (col.search() !== evt.target.value) {
                    col.search(evt.target.value).draw();
                }
            };

            $('thead > tr:nth-child(2) input', tablaDOM).on('input change', function (evt) {
                clearTimeout(timeout);
                timeout = setTimeout(filterChanged, 500, evt);
            });
            $('thead > tr:nth-child(2) select:not(select[name="IdTipoTramite"])', tablaDOM).on('change', filterChanged);
            $("select[name='IdTipoTramite']", tablaDOM).on('change', function (evt) {
                var filtroObjetoTramite = $("select[name='IdObjetoTramite']", tablaDOM).empty();
                tabla.column($(filtroObjetoTramite).parents("th").index()).search("");
                var tt = Number($(this).val());
                [{ IdObjetoTramite: '', Descripcion: '- Todos -' }]
                    .concat(JSON.parse($("#hdnObjetosTramite").val())
                        .filter(function (obt) { return obt.IdTipoTramite === tt }))
                    .forEach(function (obt) {
                        $("select[name='IdObjetoTramite']", tablaDOM).append("<option value='" + obt.IdObjetoTramite + "'>" + obt.Descripcion + "</option>");
                    });

                filterChanged(evt);
            });

            // Ticket S009 - No mostrar los anulados al menos que se seleccione del filtro
            // Listener de tipo "change" para el input select de estados
            estadosSelect.addEventListener("change", (event) => {
                clearTimeout(timeout);
                timeout = setTimeout(filterChanged, 500, event);
            });
            //----------------------------------------------------------

            cargarAcciones();
            ajustarmodal();
            $("select[name='IdTipoTramite']", tablaDOM).change();
        }
    });
}

$("#ModalObservaciones").on('hidden.bs.modal', function () {
    $('#cboSectorInfoObservaciones').val("");
    $('#txtObservacionInfoObservaciones').val("");
    $('#btnAceptarInfoObservaciones').off("click"); //esto es por si se cancela el modal
});

function showModalObservacion(titulo, labelObserv, showSector, fn) {
    if (typeof showSector === 'function') {
        fn = showSector;
        showSector = null;
    }
    $('#btnAceptarInfoObservaciones').one('click', fn);
    $("#botones-modal-info-mt-Resu").find("span:last").hide();
    $("#TituloInfoObservaciones").html(titulo);

    if (labelObserv) {
        $("#lblObservacionInfoObservaciones").text(labelObserv);
    }
    $('#cboSectorInfoObservaciones').val('');
    $('#cboUsuariosInfoObservaciones').val('');
    $('#txtObservacionInfoObservaciones').val('');
    $('#lblUsuarioInfoObservaciones').hide();
    $('#cboUsuariosInfoObservaciones').hide();
    $('#lblDestinoInfoObservaciones').hide();
    $('#cboSectorInfoObservaciones').hide();

    if (showSector) {
        $('#lblDestinoInfoObservaciones').show();
        $('#cboSectorInfoObservaciones').show();
    } else if (titulo === "REASIGNAR") {
        $('#lblUsuarioInfoObservaciones').show();
        $('#cboUsuariosInfoObservaciones').show();
    }

    $("#ModalObservaciones").modal('show');
}
function toCamelCase(sentenceCase) {
    return sentenceCase
        .toString()
        .split(" ")
        .reduce(function (out, word) {
            return `${out} ${word[0].toUpperCase()}${word.slice(1).toLowerCase()}`;
        }, '')
        .trim();
}

//# sourceURL=me.js