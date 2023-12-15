var Grilla_InspectoresBusqueda;
var Grilla_Actas;
var Grilla_UnidadesTributarias;
var domicilios;
var Grilla_Personas;
var Grilla_Documentos;
var Grilla_ActaOrigen;
var Grilla_OtrosObjetos;
var Grilla_ActasResultadoBusqueda;

var selectedUT;
var selectedPer;
var selectedActasOrigen;
var selectedActaResultado;
var selectedOtrosObjetos;
var selectedDoc;
var selectedEstado;

var inspectores = null;
var actasEstado = null;
var actasEncontradas = null;
var _idPanel = 1;
var _prevPanel = 1;

var rolesPersonas = null;

var tiposDocumento = null;
$(document).ready(init);
$(window).resize(ajustarmodal);

var _enabled = true;
var altaUT = false;

var buscaFecha = 0;
var buscaNumero = 0;
var buscaInspectores = 0;
var buscaUnidad = 0;
var buscaEstado = 0;

function ajustarmodal() {
    var altura = $(window).height() - 220; //value corresponding to the modal heading + footer
    $(".actas-body", "#actasModal").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $('.actas-content').collapse('show');
    temp = $(".actas-body").height();
    var outerHeight = 0;
    $('#accordion-actas .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-actas .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.actas-content').css({ "max-height": temp + 'px' });
    $('#accordion-actas').css({ "max-height": temp + 1 + 'px' });
    $(".actas-content").getNiceScroll().resize(); //REMOVER
    $(".actas-content").getNiceScroll().show(); //REMOVER
}
function convertDate(fecha) {
    return moment(fecha).format("DD/MM/YYYY");
}
function convertDateComplete(fecha) {
    return moment(fecha).format("DD/MM/YYYY HH:mm");
}
function formatDate(data) {
    if (!data) return null;
    var parts = data.split("-");
    if (!parts[2]) return data;
    var dd = parts[2].split("T");
    return convertDate(new Date(parts[0], parts[1] - 1, dd[0]));
}

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlerta = fn;
    $("#ModalInfoActas .modal-footer").hide();
    switch (tipo) {
        case 1:
            $("#ModalInfoActas .modal-footer").show();
            cls = "alert-success";
            break;
        case 2:
            $("#ModalInfoActas .modal-footer").show();
            cls = "alert-warning";
            break;
        case 3:
            $("#ModalInfoActas .modal-footer").show();
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#alert_message_btnSi_result").val(0);
    $("#alert_message_btnNo_result").val(0);
    $("#MensajeInfoActas").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoActas").html(titulo);
    $("#DescripcionInfoActas").html(mensaje);
    $("#ModalInfoActas").modal('show');
}
function EnablePanel(idPanel) {
    _prevPanel = _idPanel;
    _idPanel = idPanel;
    switch (idPanel) {
        case 1:
            $("#Panel_Botones span[aria-controls='button']").removeClass("black");
            $("#Panel_Botones span[aria-controls='button']").addClass("boton-deshabilitado");

            $("#headingPlanificacion").removeClass("panel-deshabilitado");
            $("#collapsePlanificacion").removeClass("in").addClass("in").css('height', '');

            $("#headingResultado").addClass("panel-deshabilitado");
            $("#collapseResultados").removeClass("in").css('heigth', '0px');

            $("#headingDatos").addClass("panel-deshabilitado");
            $("#collapseDatos").removeClass("in").css('heigth', '0px');

            $("#headingUnidadesTributarias").addClass("panel-deshabilitado");
            $("#collapseUnidadesTributarias").removeClass("in").css('heigth', '0px');

            $("#headingPersonas").addClass("panel-deshabilitado");
            $("#collapsePersonas").removeClass("in").css('heigth', '0px');

            $("#headingDocumentos").addClass("panel-deshabilitado");
            $("#collapseDocumentos").removeClass("in").css('heigth', '0px');
            try {
                Grilla_ActasResultadoBusqueda.clear().draw();
                Grilla_ActasResultadoBusqueda.destroy();
            }
            catch { console.log('--parche--'); }
            break;
        case 2:
            $("#Panel_Botones span[aria-controls='button']").removeClass("black");
            $("#Panel_Botones span[aria-controls='button']").addClass("boton-deshabilitado");
            $("#btnCerrar").removeClass("boton-deshabilitado");

            $("#headingPlanificacion").addClass("panel-deshabilitado");
            $("#collapsePlanificacion").removeClass("in").css('heigth', '0px');

            $("#headingResultado").removeClass("panel-deshabilitado");
            $("#collapseResultados").removeClass("in").css('heigth', '0px');

            $("#headingDatos").addClass("panel-deshabilitado");
            $("#collapseDatos").removeClass("in").addClass("in").css('height', '');

            $("#headingUnidadesTributarias").addClass("panel-deshabilitado");
            $("#collapseUnidadesTributarias").removeClass("in").css('heigth', '0px');

            $("#headingPersonas").addClass("panel-deshabilitado");
            $("#collapsePersonas").removeClass("in").css('heigth', '0px');

            $("#headingDocumentos").addClass("panel-deshabilitado");
            $("#collapseDocumentos").removeClass("in").css('heigth', '0px');
            break;
        case 3:
            $("#Panel_Botones span[aria-controls='button']").addClass("black");
            $("#Panel_Botones span[aria-controls='button']").removeClass("boton-deshabilitado");

            $("#headingPlanificacion").addClass("panel-deshabilitado");
            $("#collapsePlanificacion").removeClass("in").css('heigth', '0px');
            $("#collapseResultados").removeClass("in").addClass("in").css('height', '');
            $("#headingDatos").removeClass("panel-deshabilitado");

            $("#headingUnidadesTributarias").removeClass("panel-deshabilitado");
            $("#collapseUnidadesTributarias").removeClass("in").addClass("in").css('height', '');

            $("#headingPersonas").removeClass("panel-deshabilitado");
            $("#collapsePersonas").removeClass("in").addClass("in").css('height', '');

            $("#headingDocumentos").removeClass("panel-deshabilitado");
            $("#collapseDocumentos").removeClass("in").addClass("in").css('height', '');
            break;
    }
    setTimeout(function () {
        if (Grilla_InspectoresBusqueda)
            Grilla_InspectoresBusqueda.draw();
        if (Grilla_Actas)
            Grilla_Actas.draw();
        if (Grilla_UnidadesTributarias)
            Grilla_UnidadesTributarias.draw();
        if (domicilios)
            domicilios.draw();
        if (Grilla_Personas)
            Grilla_Personas.draw();
        if (Grilla_Documentos)
            Grilla_Documentos.draw();
        if (Grilla_ActaOrigen)
            Grilla_ActaOrigen.draw();
        if (Grilla_OtrosObjetos)
            Grilla_OtrosObjetos.draw();
    }, 100);
}
function setEventGrilla_InspectoresBusqueda() {
    Grilla_InspectoresBusqueda.draw();
    $('#Grilla_InspectoresBusqueda tbody').off('click', 'tr');
    $('#Grilla_InspectoresBusqueda tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
        $("#selectedInspectoresBusqueda").val(Grilla_InspectoresBusqueda.rows('.selected').data().toArray().map(function (r) { return r[0]; }).join(','));
    });
}
function setEventGrilla_ActasOrigen() {
    $('#Grilla_ActaOrigen tbody').off('click', 'tr');
    $('#Grilla_ActaOrigen tbody').on('click', 'tr', function () {
        if (_enabled) {
            var d = Grilla_ActaOrigen.row(this).data();
            if ($(this).hasClass('selected')) {
                var ssplit = $("#selectedActasOrigen").val().split(',');
                var founded = false;
                removeResult = "";

                ssplit.forEach(function (entry) {
                    if (entry == d[1]) {
                        founded = true;
                    } else {
                        removeResult += entry + ',';
                    }
                });
                if (removeResult.length > 1) {
                    $("#selectedActasOrigen").val(removeResult.substring(0, removeResult.length - 1));
                } else {
                    $("#selectedActasOrigen").val("");
                }

                $(this).removeClass('selected').removeClass('selected');
                $("#btnRemActas").addClass('boton-deshabilitado');
            }
            else {
                $(this).removeClass('selected').addClass('selected');
                selectedActasOrigen = Buscador_Actas + '-' + d[0];
                $("#btnRemActas").removeClass('boton-deshabilitado');
            }
        }


    });
}
function setEventGrilla_ActasResultadoBusqueda() {
    $("#btnBorrar").removeClass("boton-deshabilitado").addClass('boton-deshabilitado');
    $("#btnEditar").removeClass("boton-deshabilitado").addClass('boton-deshabilitado');
    $('#Grilla_ActasResultadoBusqueda tbody').off('click', 'tr');
    $('#Grilla_ActasResultadoBusqueda tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected').removeClass('selected');
            $("#btnBorrar").addClass("boton-deshabilitado");
            $("#btnEditar").addClass("boton-deshabilitado");
            VaciarDatos();
            EnablePanel(_prevPanel);
            $(".actas-content").getNiceScroll().resize(); //REMOVER
        } else {
            VaciarDatos();
            var d = Grilla_ActasResultadoBusqueda.row(this).data();
            Grilla_ActasResultadoBusqueda.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
            $("#actaId").val(d[0]);
            showActa(d[0]);
            if ($("#actaId").val() !== "0") {
                $("#btnBorrar").removeClass("boton-deshabilitado");
                $("#btnEditar").removeClass("boton-deshabilitado");
            } else {
                $("#btnBorrar").addClass("boton-deshabilitado");
                $("#btnEditar").addClass("boton-deshabilitado");
            }
        }
    });

}
function setEventGrilla_UnidadesTributarias() {
    $('#Grilla_UnidadesTributarias tbody').off('click', 'tr');
    $('#Grilla_UnidadesTributarias tbody').on('click', 'tr', function () {
        if (_enabled) {
            $(this).toggleClass('selected').siblings().removeClass('selected');
            $("#btnRemUT").addClass('boton-deshabilitado');
            if ($(this).hasClass('selected') && Grilla_UnidadesTributarias.data().any()) {
                $("#btnRemUT").removeClass('boton-deshabilitado');
            }
        }
    });
}
$('#domicilios tbody').on('click', 'tr', function () {
    if (_enabled) {
        $(this).toggleClass('selected').siblings().removeClass('selected');
        $("#btnRemDom").addClass('boton-deshabilitado');
        if ($(this).hasClass('selected') && domicilios.data().any()) {
            $("#btnRemDom").removeClass('boton-deshabilitado');
        }
    }
});
function setEventGrilla_Personas() {
    //$('#Grilla_Personas tbody').off('click', 'tr');
    $('#Grilla_Personas tbody').off('click', 'tr');
    $('#Grilla_Personas tbody').on('click', 'tr', function () {
        if (_enabled) {
            $(this).toggleClass('selected').siblings().removeClass('selected');
            $("#btnRemPer").addClass('boton-deshabilitado');
            if ($(this).hasClass('selected') && Grilla_Personas.data().any()) {
                $("#btnRemPer").removeClass('boton-deshabilitado');
            }
        }
    });

}
function setEventGrillaDocumentos() {
    $('#documentos tbody').off('click', 'tr');
    $('#documentos tbody').on('click', 'tr', function () {
        if (_enabled) {
            $(this).toggleClass('selected').siblings().removeClass('selected');
            $("#btnRemDoc,#btnEditDoc,#btnViewDoc").addClass('boton-deshabilitado');
            if ($(this).hasClass('selected') && Grilla_Documentos.data().any()) {
                $("#btnRemDoc,#btnEditDoc,#btnViewDoc").removeClass('boton-deshabilitado');
            }
        }
    });
}
function setEventGrilla_OtrosObjetos() {
    $('#Grilla_OtrosObjetos tbody').off('click', 'tr');
    $('#Grilla_OtrosObjetos tbody').on('click', 'tr', function () {
        if (_enabled) {
            $(this).toggleClass('selected').siblings().removeClass('selected');
            $("#btnRemObj").addClass('boton-deshabilitado');
            if ($(this).hasClass('selected') && Grilla_OtrosObjetos.data().any()) {
                $("#btnRemObj").removeClass('boton-deshabilitado');
            }
        }
    });
}
function noExiste(id) {
    return !$('#domicilios').dataTable().api().rows(function (_, data) { return data[2] === id; }).data().toArray().length;
}
function LoadUnidadesTributarias(uts) {
    $("#btnRemUT").addClass('boton-deshabilitado');
    Grilla_UnidadesTributarias.clear().draw();
    var asociarUTDs = [];
    if (uts) {
        uts.forEach(function (ut) {
            Grilla_UnidadesTributarias.row.add({
                "0": ut[1],
                "1": ut[0]
            });
            if (altaUT && noExiste(ut[1])) {
                asociarUTDs.push(new Promise(function (resolve) {
                    $.get(BASE_URL + 'Actas/AddUnidadTributariaDomicilios', "id=" + ut[1], function (data) { resolve({ domicilios: data, idUT: ut[1] }); });
                }));
            }
        });
        Grilla_UnidadesTributarias.draw();
        $("#selectedUT").val(Grilla_UnidadesTributarias.data().toArray().map(function (data) { return data[0]; }).join(','));
        Promise.all(asociarUTDs)
            .then(function (resultados) {
                var agregarDomicilios = [];
                resultados.forEach(function (data) {
                    agregarDomicilios.push(...JSON.parse(data.domicilios).map(function (obj) {
                        return new Promise(function (resolve) {
                            $.ajax({
                                url: BASE_URL + "Actas/PostGuardarDomicilio",
                                type: 'POST',
                                dataType: 'json',
                                data: { domicilio: obj },
                                success: function (resultado) {
                                    resolve({ domicilio: JSON.parse(resultado), idUT: data.idUT });
                                }
                            });
                        });
                    }));
                });
                Promise.all(agregarDomicilios)
                    .then(function (resultados) {
                        resultados.forEach(function (resultado) {
                            var dom = {
                                "0": resultado.domicilio.DomicilioId,
                                "1": resultado.domicilio.ViaNombre + " " + (resultado.domicilio.numero_puerta || ""),
                                "2": resultado.idUT
                            };
                            domicilios.row.add(dom);
                        });
                        domicilios.draw();
                        $("#selectedDom").val(domicilios.data().toArray().map(function (d) { return d[0]; }).join(','));
                    });
                setEventGrilla_UnidadesTributarias();
            });
    }
}
function SetEventosBuscadorGenerico() {
    // Unidades Tributarias
    $("#btnAddUT").on("click", function () {
        buscarUnidadesTributarias(true).then(function (seleccion) {
            if (seleccion.length) {
                altaUT = true;
                $("#selectedUT").val(seleccion.map(function (data) { return data[1]; }).join(','));
                LoadUnidadesTributarias(seleccion);
            }
        }).catch(function (err) { console.log(err); });
    });
    $("#btnRemUT").click(function () {
        var deleted = Grilla_UnidadesTributarias.rows(".selected");
        if (deleted.length) {
            alerta('Advertencia', 'Se quitará la relación con las Unidades Tributarias seleccionadas. ¿Desea continuar?', 2, function () {
                var domicilios = $('#domicilios').dataTable().api();
                deleted.data().toArray().forEach(function (r) {
                    domicilios.rows(function (_, data) { return data[2] === r[0]; }).remove();
                });
                deleted.remove().draw();
                domicilios.draw();
                $("#selectedUT").val(Grilla_UnidadesTributarias.data().toArray().map(function (r) { return r[0]; }).join(","));
                $("#btnRemUT").addClass('boton-deshabilitado');
            });
        }
    });

    $("#btnRemDom").click(function () { //Evento Borrar Domicilio
        var objeto = $('#domicilios').dataTable().api().row('.selected').data();

        if (objeto[2]) {
            var objeto2 = Grilla_UnidadesTributarias.data().toArray();

            for (var index in objeto2) {
                var aux = objeto2[index];
                if (objeto[2] == aux[0]) {
                    alerta('Advertencia', 'El domicilio que quiere dar de baja esta asociado a la Unidad Tributaria ' + aux[1], 4);
                    return;
                }
            }
        }
        alerta('Advertencia', 'Se quitará la relación con los Domicilios Seleccionados. ¿Desea Continuar?', 2, function () {
            remDomicilios($('#domicilios').dataTable().api().row('.selected').data()[0]);
        });
    });

    //Personas
    $("#btnAddPer").click(function () {
        $("#ModalRolPersona").modal('show');
    });
    $("#btnBuscarPersona").click(function () {
        new Promise(function (resolve) {
            var data = {
                tipos: BuscadorTipos.Personas,
                multiSelect: false,
                verAgregar: false,
                titulo: 'Buscar Persona',
                campos: ["Nombre", "dni:DNI"]
            };
            $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
                $(".modal", this).one('hidden.bs.modal', function () {
                    $(window).off('seleccionAceptada');
                });
                $(window).one("seleccionAceptada", function (evt) {
                    if (evt.seleccion) {
                        resolve(evt.seleccion.slice(1));
                    } else {
                        resolve([]);
                    }
                });
            });
        }).then(function (seleccion) {
            if (seleccion.length) {
                $("#idPersonaSelected").val(seleccion[1]);
                $("#personaSelected").val(seleccion[0]);
            }
        }).catch(function (err) { console.log(err); });
    });
    $("#btnAceptarRolPersona").click(function (e) {
        var value = [
            $("#idPersonaSelected").val(),
            $("#rolesPersona").val(),
            $('#rolesPersona option[value="' + $("#rolesPersona").val() + '"]').text().trim(),
            $("#personaSelected").val().trim()
        ].join("-");
        var currPersonas = [];
        if ($("#selectedPer").val()) {
            currPersonas = $("#selectedPer").val().split("@");
        }
        if (currPersonas.indexOf(value) === -1) {
            currPersonas.push(value);
        }
        LoadPersonas(currPersonas);
    });
    $("#btnRemPer").click(function () {
        var deleted = Grilla_Personas.rows(".selected");
        if (deleted.length) {
            alerta('Advertencia', 'Se quitará la relación con la persona. ¿Desea continuar?', 2, function () {
                if ($("#alert_message_btnSi_result").val() === "1") {
                    deleted.remove().draw();
                    $("#selectedPer").val(Grilla_Personas.data().toArray().map(function (r) { return r.join("-"); }).join("@"));
                    $("#btnRemPer").addClass('boton-deshabilitado');
                }
            });
        }
    });

    ////Actas
    //$("#btnAddActas").click(function () {
    //    //BuscadorGenerico(
    //    //    [Buscador_Actas], //tipos
    //    //    "buscadorGenerico", //ubicacion
    //    //    "selectedActasOrigen", //devolucion
    //    //    "Actas",
    //    //    "Descripcion",
    //    //    function () { //OK
    //    //        LoadActasOrigen();
    //    //    },
    //    //    function () { //CANCEL 
    //    //    }
    //    //);
    //});

    //$("#btnRemActas").click(function () {
    //    var deleted = Grilla_ActaOrigen.rows(".selected");
    //    if (deleted.length) {
    //        alerta('Advertencia', 'Se quitará la relación con el acta. ¿Desea continuar?', 2, function () {
    //            if ($("#alert_message_btnSi_result").val() === "1") {
    //                deleted.remove().draw();
    //                $("#selectedActasOrigen").val(Grilla_ActaOrigen.data().toArray().map(function (r) { return r.join("-"); }).join("@"));
    //                $("#btnRemActas").addClass('boton-deshabilitado');
    //            }
    //        });
    //    }
    //    //var ssplit = $("#selectedActasOrigen").val().split(',');
    //    //var founded = false;
    //    //removeResult = "";

    //    //ssplit.forEach(function (entry) {
    //    //    if (entry == selectedActasOrigen) {
    //    //        founded = true;
    //    //    } else {
    //    //        removeResult += entry + ',';
    //    //    }
    //    //});
    //    //if (founded) {
    //    //    alerta('Advertencia', 'Se quitará la relación con el acta. ¿Continuar?', 2, function () {
    //    //        if (removeResult.length > 1) {
    //    //            $("#selectedActasOrigen").val(removeResult.substring(0, removeResult.length - 1));
    //    //        } else {
    //    //            $("#selectedActasOrigen").val("");
    //    //        }
    //    //        LoadActasOrigen();
    //    //    });
    //    //}

    //});
}
function SetEnabledActas(enabled) {
    _enabled = enabled;
    $("#SelectedTipoActa").attr("disabled", !enabled);
    $("#nroActa").attr("disabled", !enabled);
    $("#plazo").attr("disabled", !enabled);
    $("#SelectedInspector").attr("disabled", !enabled);
    $("#SelectedEstadoActa").attr("disabled", !enabled);
    $("#selectedUT").attr("disabled", !enabled);
    $("#selectedPer").attr("disabled", !enabled);
    $("#selectedActasOrigen").attr("disabled", !enabled);
    $("#selectedDocs").attr("disabled", !enabled);
    $("#selectedDom").attr("disabled", !enabled);
    $("#SelectedTipoActa").attr("disabled", !enabled);
    $("#selectedOtrosObjetos").attr("disabled", !enabled);
    $('#observaciones').attr("disabled", !enabled);
    $("#rolesPersona").attr("disabled", !enabled);
    $('#idPersonaSelected').attr("disabled", !enabled);
    $('#personaSelected').attr("disabled", !enabled);
    $('#otrosObjetosTipo').attr("disabled", !enabled);
    $('#otrosObjetosNumero').attr("disabled", !enabled);
    $("#Grilla_ActasResultadoBusqueda").attr("disabled", enabled);

    $("#fecha").attr("disabled", !enabled);
    $("#hora").attr("disabled", !enabled);

    $("#btnRemUT").addClass('boton-deshabilitado');
    $("#btnRemPer").addClass('boton-deshabilitado');
    $("#btnRemActas").addClass('boton-deshabilitado');
    $("#btnRemDoc").addClass('boton-deshabilitado');
    $("#btnEditDoc").addClass('boton-deshabilitado');
    $("#btnViewDoc").addClass('boton-deshabilitado');
    $("#btnRemObj").addClass('boton-deshabilitado');
    $("#btnAdd2").removeClass('boton-deshabilitado');
    $("#btnBorrar").removeClass('boton-deshabilitado');
    $("#btnEditar").removeClass('boton-deshabilitado');

    if (!_enabled) {
        $("#btnAddUT").addClass('boton-deshabilitado');
        $("#btnAddPer").addClass('boton-deshabilitado');
        $("#btnAddActas").addClass('boton-deshabilitado');
        $("#btnAddDom").addClass('boton-deshabilitado');
        $("#btnAddDoc").addClass('boton-deshabilitado');
        $("#btnAddObj").addClass('boton-deshabilitado');
        $("#btnGrabar").addClass('boton-deshabilitado');
        $("#btnRemDom").addClass('boton-deshabilitado');
    } else {
        $("#btnAdd2").addClass('boton-deshabilitado');
        $("#btnBorrar").addClass('boton-deshabilitado');
        $("#btnEditar").addClass('boton-deshabilitado');

        $("#btnAddUT").removeClass('boton-deshabilitado');
        $("#btnAddPer").removeClass('boton-deshabilitado');
        $("#btnAddActas").removeClass('boton-deshabilitado');
        $("#btnAddDom").removeClass('boton-deshabilitado');
        $("#btnAddDoc").removeClass('boton-deshabilitado');
        $("#btnAddObj").removeClass('boton-deshabilitado');
        $("#btnGrabar").removeClass("boton-deshabilitado");
    }
}
function accordionSearchHandler(tabname) {
    $("#collapse" + tabname).on("shown.bs.collapse", function () {
        if ($(".panel-collapse", "#accordionBusqueda").hasClass("in")) {
            //$("#btnBuscar").removeClass("boton-deshabilitado");
            $("#clear-all").removeClass("boton-deshabilitado");
        }
    });

    $("#collapse" + tabname).on("hidden.bs.collapse", function () {
        if (!$(".panel-collapse", "#accordionBusqueda").hasClass("in")) {
            //$("#btnBuscar").addClass("boton-deshabilitado");
            $("#clear-all").addClass("boton-deshabilitado");
        }

        switch (tabname) {
            case "FC":
                buscaFecha = 0;
                break;
            case "UT":
                buscaUnidad = 0;
                break;
            case "NA":
                buscaNumero = 0;
                break;
            case "Inspectores":
                buscaInspectores = 0;
                break;
            case "Estado":
                buscaEstado = 0;
                break;
        }
    });

    $("#accordion" + tabname).click(function () {
        var accordionTab = $("#collapse" + tabname);
        if (accordionTab.hasClass("in")) {
            accordionSearchTabHide("#collapse" + tabname, "#heading" + tabname);
        } else {
            accordionSearchTabShow("#collapse" + tabname, "#heading" + tabname);
        }
        setTimeout(function () {
            $(".actas-content").getNiceScroll().resize();
        }, 10);
    });
}
function accordionSearchTabShow(accordionTab, heading) {
    switch (accordionTab) {
        case "#collapseFC":
            buscaFecha = 1;
            break;
        case "#collapseUT":
            buscaUnidad = 1;
            break;
        case "#collapseNA":
            buscaNumero = 1;
            break;
        case "#collapseInspectores":
            buscaInspectores = 1;
            setTimeout(function () { Grilla_InspectoresBusqueda.draw(); }, 100);
            break;
        case "#collapseEstado":
            buscaEstado = 1;
            break;
    }
    var tab = $(accordionTab);
    tab.collapse("show");
    $(heading + " .circle").addClass("selected");
}
function accordionSearchTabHide(accordionTab, heading) {
    var tab = $(accordionTab);
    if (tab.hasClass("in")) {
        tab.collapse("hide");
        $(heading + " .circle").removeClass("selected");
    }
}
function VaciarDatos() {
    $("#actaId").val("0");
    $("#SelectedTipoActa").val("0");
    $("#nroActa").val("");
    $("#plazo").val("");
    var hoy = new Date();
    {
        var dHoy = hoy.getDate();
        if (dHoy < 10)
            dHoy = "0" + dHoy;
        var mHoy = hoy.getMonth() + 1;
        if (mHoy < 10)
            mHoy = "0" + mHoy;
        var hHoy = hoy.getHours();
        if (hHoy < 10)
            hHoy = "0" + hHoy;
        var iHoy = hoy.getMinutes();
        if (iHoy < 10)
            iHoy = "0" + iHoy;

        $("#fecha").val(dHoy + '/' + mHoy + '/' + hoy.getFullYear());
        $("#hora").val(hHoy + ':' + iHoy);
    }
    $("#SelectedInspector").val("0");
    $("#SelectedEstadoActa").val("0");
    $("#selectedUT").val("");
    $("#selectedPer").val("");
    $("#selectedActasOrigen").val("");
    $("#selectedDom").val("");
    $("#selectedDocs").val("");
    $("#SelectedTipoActa").val("");
    $("#selectedOtrosObjetos").val("");
    $('#observaciones').val("");
    $("#rolesPersona").val("0");
    $('#idPersonaSelected').val("");
    $('#personaSelected').val("");
    $('#otrosObjetosTipo').val("1");
    $('#otrosObjetosNumero').val("");

    LoadUnidadesTributarias();
    LoadPersonas([]);
    LoadDocumentos([]);
    //LoadActasOrigen();
    //LoadOtrosObjetos();
    LoadDomicilio([]);

    $.ajax({
        url: BASE_URL + 'Actas/InicializarDomicilios',
        dataType: 'json',
        type: 'GET',
        success: function (data) {
            //inspectores = data;
        }, error: function (ex) {

        }
    });

}
function inicializarGrillas() {
    Grilla_InspectoresBusqueda = $('#Grilla_InspectoresBusqueda').DataTable({
        //"scrollX": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "scrollY": "400px",
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "visible": false,
        }],
        "initComplete": function () {
            Grilla_InspectoresBusqueda.draw();
            setEventGrilla_InspectoresBusqueda();
        }
    });
    Grilla_UnidadesTributarias = $('#Grilla_UnidadesTributarias').DataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "visible": false,
        }, {
            "targets": 'single-column',
            "sWidth": "100%"
        }
        ],
        "initComplete": function () {
            Grilla_UnidadesTributarias.draw();
            setEventGrilla_UnidadesTributarias();
        }
    });
    domicilios = $('#domicilios').DataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "visible": false
        },
        {
            'bSortable': false,
            'aTargets': [1]
        },
        {
            "targets": [2],
            "visible": false
        },
        ],
        "initComplete": function () {
            domicilios.draw();
            //setEventdomicilios();
        }
    });
    Grilla_Personas = $('#Grilla_Personas').DataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "visible": false
        }],
        "initComplete": function () {
            Grilla_Personas.draw();
            setEventGrilla_Personas();
        }
    });
    Grilla_Documentos = $('#documentos').DataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "visible": false
        }, {
            "targets": [3],
            "iDataSort": "5"
        }],
        "initComplete": function () {
            Grilla_Documentos.draw();
        }
    });
    //Grilla_ActaOrigen = $('#Grilla_ActaOrigen').DataTable({
    //    "scrollY": "100px",
    //    "scrollCollapse": true,
    //    "paging": false,
    //    "searching": false,
    //    "bInfo": false,
    //    "aaSorting": [[0, 'asc']],
    //    "language": {
    //        "url": BASE_URL + "Scripts/dataTables.spanish.txt"
    //    },
    //    "columnDefs": [{
    //        "targets": 'no-sort',
    //        "orderable": false,
    //        "visible": false,
    //    }],
    //    "initComplete": function () {
    //        Grilla_ActaOrigen.draw();
    //        setEventGrilla_ActasOrigen();
    //    }
    //});
    //Grilla_OtrosObjetos = $('#Grilla_OtrosObjetos').DataTable({
    //    "scrollY": "160px",
    //    "scrollCollapse": true,
    //    "paging": false,
    //    "searching": false,
    //    "bInfo": false,
    //    "aaSorting": [[0, 'asc']],
    //    "language": {
    //        "url": BASE_URL + "Scripts/dataTables.spanish.txt"
    //    },
    //    "columnDefs": [{
    //        "targets": 'no-sort',
    //        "orderable": false,
    //        "visible": false,
    //    }],
    //    "initComplete": function () {
    //        Grilla_OtrosObjetos.draw();
    //        setEventGrilla_OtrosObjetos();
    //    }
    //});
}
function limpiarErrores() {
    $("#SelectedTipoActa_required").addClass('oculto');
    $("#nroActa_required").addClass('oculto');
    $("#plazo_required").addClass('oculto');
    $("#SelectedInspector_required").addClass('oculto');
    $("#SelectedEstadoActa_required").addClass('oculto');
    $("#selectedPer_required").addClass('oculto');
}
$('#modal-window-actas').on('shown.bs.modal', function (e) {
    ajustarScrollBars();
    hideLoading();
});
function init() {
    ///////////////////// Scrollbars //////////////////////// REMOVER
    $(".actas-content").niceScroll(getNiceScrollConfig());
    $('#actasModal .panel-body').resize(ajustarScrollBars);
    $('.actas-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".actas-content").getNiceScroll().resize(); //REMOVER
        }, 10);
    });
    ////////////////////////////////////////////////////////
    ajustarmodal();
    ///////////////////// Tooltips /////////////////////////
    $('#actasModal .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-actas").modal('show');

    ////////////////////////////////////////////////////////
    $.ajax({
        url: BASE_URL + 'Documento/GetTiposDocumentosJson',
        type: 'GET',
        success: function (data) {
            tiposDocumento = data;
        }, error: function (ex) {
            alert(ex);
        }
    });

    $.ajax({
        url: BASE_URL + 'ObrasParticulares/GetAllInspectores',
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            inspectores = data;
            //getActasBuscar();
        }, error: function (ex) {

        }
    });

    $.ajax({
        url: BASE_URL + 'Actas/GetActaRolPersona',
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            rolesPersonas = data;
            data.forEach(function (entry) {
                $('#rolesPersona').append($('<option>', {
                    value: entry.ActaRolId,
                    text: entry.Descripcion
                }));
            });

        }, error: function (ex) {

        }
    });

    inicializarGrillas();

    var hoyString = moment(new Date()).format('DD/MM/YYYY');
    $("#fechaDesde").val(hoyString);
    $("#fechaHasta").val(hoyString);

    $('.fechaPicker').datepicker(getDatePickerConfig({ minViewMode: 'days', maxViewMode: 'years' }));
    $("#numeroDesde").numeric();
    $("#numeroHasta").numeric();
    $("#nroActa").numeric();
    $("#otrosObjetosNumero").numeric();

    $("#btnAdd,#btnAdd2").click(function (e) {
        VaciarDatos();
        SetEnabledActas(true);
        EnablePanel(3);
        e.preventDefault;
    });

    //Objetos

    $("#btnAddObj").click(function (e) {
        e.preventDefault();
        setTimeout(function () {
            $("#ModalOtrosObjeto").modal('show');
        }, 10);
        $('#otrosObjetosNumero').val('');
        $('#otrosObjetosTipo').val('1');
    });

    $("#btnAceptarOtrosObjetos").click(function () {
        $.ajax({
            url: BASE_URL + 'ObrasParticulares/GetExisteObjeto/?objeto=' + $('#otrosObjetosTipo').val() + '&tipo=0&identificador=' + $("#otrosObjetosNumero").val(),
            dataType: 'json',
            type: 'GET',
            success: function (data) {
                if (data) {
                    if (data < 0 && ($("#otrosObjetosTipo").val() !== "0" || $("#otrosObjetosNumero").val() !== "0" || !$("#otrosObjetosNumero").val())) {
                        alerta('Error', 'La relación seleccionada no existe', 4);
                    } else {
                        var ssplit = $("#selectedOtrosObjetos").val().split(',');
                        var founded = false;

                        ssplit.forEach(function (entry) {
                            if (entry == $("#otrosObjetosTipo").val() + '-' + data) {
                                founded = true;
                            }
                        });
                        if (!founded) {
                            Grilla_OtrosObjetos.row.add({
                                "0": $("#otrosObjetosTipo").val(),
                                "1": data,
                                "2": $("#otrosObjetosTipo option:selected").text(),
                                "3": $("#otrosObjetosNumero").val()
                            }).draw();
                            if ($("#selectedOtrosObjetos").val() == "")
                                $("#selectedOtrosObjetos").val($("#otrosObjetosTipo").val() + '-' + data);
                            else
                                $("#selectedOtrosObjetos").val($("#selectedOtrosObjetos").val() + ',' + $("#otrosObjetosTipo").val() + '-' + data);
                        } else {
                            alerta('Error', 'La relación ya ha sido agregada', 4);
                        }

                    }

                }
            }
        });

    });
    $("#btnRemObj").click(function () {
        alerta('Advertencia', 'Se quitará la relación. ¿Continuar?', 2, function () {
            $("#btnRemObj").addClass("boton-deshabilitado");
            var obj = $('#Grilla_OtrosObjetos').dataTable().api().row('.selected').data();
            $('#Grilla_OtrosObjetos').dataTable().api().row('.selected').remove().draw();

            var ssplit = $("#selectedOtrosObjetos").val().split(',');
            var founded = false;
            removeResult = "";

            ssplit.forEach(function (entry) {
                if (entry == selectedOtrosObjetos) {
                    founded = true;
                } else {
                    removeResult += entry + ',';
                }
            });
            if (removeResult.length > 1) {
                $("#selectedOtrosObjetos").val(removeResult.substring(0, removeResult.length - 1));
            } else {
                $("#selectedOtrosObjetos").val("");
            }

        });



    });

    SetEventosBuscadorGenerico();

    $("#btnGrabar").click(function () {
        limpiarErrores();
        var valido = true;
        var campos = [];
        if (!$("#SelectedTipoActa").val() || $("#SelectedTipoActa").val() === "0") {
            valido = false;
            $("#SelectedTipoActa_required").removeClass('oculto');
            campos.push("Tipo de Acta");
        } else {
            $("#SelectedTipoActa_required").addClass('oculto');
        }
        if (!$("#nroActa").val()) {
            valido = false;
            $("#nroActa_required").removeClass('oculto');
            campos.push("Numero de Acta");
        } else {
            $("#nroActa_required").addClass('oculto');
        }
        if (!$("#SelectedInspector").val() || $("#SelectedInspector").val() === "0") {
            valido = false;
            $("#SelectedInspector_required").removeClass('oculto');
            campos.push("Inspector Asignado");
        } else {
            $("#SelectedInspector_required").addClass('oculto');
        }
        if (!$("#SelectedEstadoActa").val() || $("#SelectedEstadoActa").val() === "0") {
            valido = false;
            $("#SelectedEstadoActa_required").removeClass('oculto');
            campos.push("Estado de Acta");
        } else {
            $("#SelectedEstadoActa_required").addClass('oculto');
        }
        if (!$("#selectedPer").val() || $("#selectedPer").val() === "0") {
            valido = false;
            $("#selectedPer_required").removeClass('oculto');
            campos.push("Debe seleccionar al menos una persona asociada");
        } else {
            $("#selectedPer_required").addClass('oculto');
        }

        if ($("#actaId").val() === "0" && $("#SelectedEstadoActa").val() === "4") {
            valido = false;
            $("#selectedPer_required").removeClass('oculto');
            campos.push("No se puede crear un acta con estado vencido");
        }

        if (!valido) {
            alerta('Error', 'Por favor verifique los datos:<br/>' + campos.join(", "), 3);
        } else {
            var formData = {
                "ActaId": $("#actaId").val(),
                "ActaTipoId": $("#SelectedTipoActa").val(),
                "NroActa": $("#nroActa").val(),
                "Plazo": $("#plazo").val(),
                "Fecha": $("#fecha").val() + ' ' + $("#hora").val(),
                "InspectorId": $("#SelectedInspector").val(),
                "SelectedEstadoActa": $("#SelectedEstadoActa").val(),
                "selectedUT": $("#selectedUT").val(),
                "SelectedDomicilio": $("#selectedDom").val(),
                "selectedPer": $("#selectedPer").val(),
                "selectedDocs": $("#selectedDocs").val(),
                "observaciones": $("#observaciones").val()
            };
            $.ajax({
                url: BASE_URL + 'Actas/PostActaGuardar',
                type: "POST",
                data: formData,
                success: function (data) {
                    if (data === "OK")
                        alerta('Actas', 'Se ha guardado correctamente el acta', 1, function () {
                            EnablePanel(1);
                            //ActualizarGridObjetos();
                            SetEnabledActas(false);
                        });
                    else
                        alerta('Error', 'Ha ocurrido un error al guardar el acta<br>' + data.Message, 3);

                },
                error: function (_, __, errorThrown) {
                    alerta('Error', errorThrown, 3);
                }
            });
        }

    });
    $("#btnCerrar").click(function () {
        if (_idPanel === 2) {
            EnablePanel(1);
        } else {
            EnablePanel(_prevPanel);
            SetEnabledActas(false);
        }
    });

    $("#btnBuscar").click(function () {
        ActualizarGridObjetos();
    });

    $('#btnEditar').click(function () {
        EditarActa();
    });

    $('#btnRemDoc').on('click', function () {
        var deleted = Grilla_Documentos.rows(".selected");
        if (deleted.length) {
            alerta('Advertencia', 'Se quitará la relación con los Documentos seleccionados. ¿Desea continuar?', 2, function () {
                deleted.data().toArray().forEach(function (r) {
                    Grilla_Documentos.rows(function (_, data) { return data[2] === r[0]; }).remove();
                });
                deleted.remove().draw();
                Grilla_Documentos.draw();
                $("#selectedDocs").val(Grilla_Documentos.data().toArray().map(function (r) { return r[0]; }).join(","));
                $("#btnRemDoc").addClass('boton-deshabilitado');
            });
        }
    });
    $('#btnEditDoc').on('click', function () {
        var obj = Grilla_Documentos.row('.selected').data();
        loadDocumentos(obj[0]);
    });
    $('#btnViewDoc').on('click', function () {
        var obj = Grilla_Documentos.row('.selected').data();
        //$.ajax({
        //    url: BASE_URL + "Documento/Visualizar",
        //    type: "POST",
        //    async: false,
        //    success: function (data) {
        //    }
        //});
        loadDocumentos(obj[0]);
    });
    $('#btnAddDoc').on('click', function () {
        $('#documentos tbody tr').removeClass('selected');
        loadDocumentos();
    });
    $('#btnAddDom').on('click', function () {
        showLoading();
        loadDomicilios();
    });

    $('#btnAceptarAlert').on('click', function () {
        $("#alert_message_btnSi_result").val("1");
        if (fnResultAlerta)
            fnResultAlerta();
    });

    $("#btnBorrar").on('click', function () {
        alerta('Advertencia', 'Se dara de baja el Acta ¿Continuar?', 2, function () {
            if ($("#alert_message_btnSi_result").val() === "1") {
                $.get(BASE_URL + 'Actas/GetActaBaja', "actaID=" + $("#actaId").val(),
                    function (data) {
                        if (data.Response === "OK") {
                            EnablePanel(1);
                            ActualizarGridObjetos();
                        }
                    });
            }
        });
    });

    accordionSearchHandler("UT");
    accordionSearchHandler("NA");
    accordionSearchHandler("FC");
    accordionSearchHandler("Estado");
    accordionSearchHandler("Inspectores");

    buscaId = parseInt($("#NonBuscaId").attr("value"));
    if (buscaId === 1) {
        ActualizarGridObjetos();
        $("#NonBuscaId").attr("value", "0");
        buscaId = 0;
        $("#NonActaId").attr("value", buscaId);
    }

    $("#ut-search").click(function () {
        buscarUnidadesTributarias()
            .then(function (seleccion) {
                if (seleccion.length) {
                    $("#UnidadTributariaId").val(seleccion[1]);
                    $("#UnidadTributaria").val(seleccion[0]);
                }
            }).catch(function (err) { console.log(err); });
    });
}
function EditarActa() {
    SetEnabledActas($("#actaId").val() && $("#actaId").val() !== "0");
}
function loadDocumentos(id) {
    $("#documentos-externo").load(BASE_URL + "Documento/DatosDocumento?id=" + (id || 0), function () {
        $(document).one("documentoGuardado", function (evt) {
            documentoGuardado(evt.documento);
        });
    });
}
function documentoGuardado(data) {
    var selected = Grilla_Documentos.row('.selected').data(),
        tipo = tiposDocumento.filter(function (tipo) { return parseInt(tipo.TipoDocumentoId) === parseInt(data.id_tipo_documento); })[0] || { Descripcion: "UNDEFINED" };
    if (!selected) {
        var doc = {
            "0": data.id_documento,
            "1": tipo.Descripcion,
            "2": data.descripcion,
            "3": formatDate(data.fecha),
            "4": data.nombre_archivo
        };
        Grilla_Documentos.row.add(doc).draw();
        $("#selectedDocs").val(Grilla_Documentos.data().toArray().map(function (r) { return r[0]; }).join(","));
    } else {
        selected["1"] = tipo.Descripcion;
        selected["2"] = data.descripcion;
        selected["3"] = data.fecha_alta_1;
        selected["4"] = data.observaciones;
        $('#documentos').dataTable().api().row('.selected').data(selected).draw();
    }
    setEventGrillaDocumentos();
}
function LoadDomicilio(domicilios) {
    $("#btnAddDom").addClass('boton-deshabilitado');
    $("#btnRemDom").addClass('boton-deshabilitado');
    $('#domicilios').dataTable().api().clear().draw();
    domicilios.forEach(function (domicilio) {
        if (domicilio) {
            var partes = domicilio.split("-");
            var dom = {
                "0": partes[0],
                "1": partes[1],
                "2": partes[2]
            };
            $('#domicilios').dataTable().api().row.add(dom).draw();
        }
    });
    $("#selectedDom").val($('#domicilios').dataTable().api().data().toArray().map(function (d) { return d[0]; }).join(','));
    setEventGrillaDocumentos();
}
function LoadPersonas(personas) {
    $("#btnRemPer").addClass('boton-deshabilitado');
    Grilla_Personas.clear().draw();
    if (personas.length) {
        personas.forEach(function (entry) {
            var partes = entry.split("-");
            Grilla_Personas.row.add({
                "0": partes[0],
                "1": partes[1],
                "2": partes[2],
                "3": partes[3]
            });
        });
        Grilla_Personas.draw();
        setEventGrilla_Personas();
    }
    $("#selectedPer").val(personas.join("@"));
}
function LoadDocumentos(documentos) {
    $("#btnRemDoc").addClass('boton-deshabilitado');
    $("#btnEditDoc").addClass('boton-deshabilitado');
    $("#btnViewDoc").addClass('boton-deshabilitado');
    Grilla_Documentos.clear().draw();
    if ($("#selectedDocs").val()) {
        documentos.forEach(function (entry) {
            var partes = entry.split("-");
            var doc = {
                "0": partes[0],
                "1": partes[1],
                "2": partes[2],
                "3": partes[3],
                "4": partes[4]
            };
            Grilla_Documentos.row.add(doc);
        });
        Grilla_Documentos.draw();
        $("#selectedDocs").val(Grilla_Documentos.data().toArray().map(function (r) { return r[0]; }).join(","));
        setEventGrillaDocumentos();
    }
}
function LoadActasOrigen() {
    $("#btnRemActas").addClass('boton-deshabilitado');
    Grilla_ActaOrigen.clear().draw();
    if ($("#selectedActasOrigen").val()) {
        $.get(BASE_URL + 'BuscadorGenerico/GetElements', "elements=" + $("#selectedActasOrigen").val() + "&tipos=" + Buscador_Actas,
            function (data) {
                var jsonResult = JSON.parse(data.Result);
                jsonResult.response.docs.forEach(function (item) {
                    var iFecha = item.descripcion.indexOf('Fecha');
                    var iCodigo = item.descripcion.indexOf('Padron');
                    var iActa = item.descripcion.indexOf('Acta');
                    var iClase = item.descripcion.indexOf('Clase');
                    var iDomicilio = item.descripcion.indexOf('Domicilio');
                    Grilla_ActaOrigen.row.add({
                        "0": item.id,
                        "1": item.descripcion.substring(iActa + 7, iClase - 2),
                        "2": item.descripcion.substring(iFecha + 8, iFecha + 27),
                        "3": item.descripcion.substring(iCodigo + 9, iDomicilio - 2)
                    }).draw();
                });
            });
        setEventGrilla_ActasOrigen();
    }
}
function LoadOtrosObjetos() {
    $("#btnRemObj").addClass('boton-deshabilitado');
    Grilla_OtrosObjetos.clear().draw();
    if ($("#selectedOtrosObjetos").val()) {
        var objetos = $("#selectedOtrosObjetos").val().split(',');
        objetos.forEach(function (entry) {
            var currObj = entry.split('-');
            $.get(BASE_URL + 'ObrasParticulares/GetOtrosObjetos', "tipo=" + currObj[0] + "&id=" + currObj[1],
                function (data) {
                    var objetoVal = data.split(',');

                    Grilla_OtrosObjetos.row.add({
                        "0": currObj[0],
                        "1": currObj[1],
                        "2": $('#otrosObjetosTipo option[value="' + currObj[0] + '"]').text(),
                        "3": objetoVal[1]
                    }).draw();
                });
        });
        setEventGrilla_OtrosObjetos();
    }
}
function loadDomicilios() {
    var domicilio = $('#domicilios').dataTable().api().row('.selected').data();
    var parametros = {};
    if (domicilio) {
        parametros = { id: domicilio.DomicilioId, domicilio: domicilio };
    }
    $.ajax({
        url: BASE_URL + "Domicilio/DatosDomicilio",
        type: 'POST',
        data: parametros,
        dataType: 'html',
        success: function (result) {
            $(document).one("domicilioGuardado", function (evt) {
                domicilioGuardado(evt.domicilio);
            });
            $("#mantenedor-externo-container").html(result);
        },
        error: function (_, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
        }
    });
}

function remDomicilios(id) {
    $.ajax({
        url: BASE_URL + "Actas/RemDomicilio/?id=" + id,
        type: 'POST',
        success: function () {
            domicilios.row('.selected').remove().draw();
            $("#selectedDom").val(domicilios.data().toArray().map(function (d) { return d[0]; }).join(','));
        },
        error: function (_, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
        }
    });
}
function domicilioGuardado(data) {
    var domicilio = $("#domicilios").dataTable().api().row('.selected').data();
    if (!domicilio) {
        $.ajax({
            url: BASE_URL + "Actas/PostGuardarDomicilio",
            type: 'POST',
            dataType: 'json',
            data: { domicilio: data },
            success: function () {
                var doc = {
                    "0": data.DomicilioId,//LALALA
                    "1": data.ViaNombre + " " + data.numero_puerta,
                    "2": ""
                };
                $("#domicilios").dataTable().api().row.add(doc).draw();
                $("#selectedDom").val(domicilios.data().toArray().map(function (d) { return d[0]; }).join(','));
            },
            error: function (_, textStatus, errorThrown) {
                console.log(textStatus, errorThrown);
            }
        });
    }
}


function buscarUnidadesTributarias(multiselect) {
    return new Promise(function (resolve) {
        var data = {
            tipos: BuscadorTipos.UnidadesTributarias,
            multiSelect: multiselect || false,
            verAgregar: false,
            titulo: 'Buscar Unidades Tributarias',
            campos: ["Partida"]
        };
        if (data.multiSelect && $("#selectedUT").val()) {
            data.seleccionActual = Grilla_UnidadesTributarias.data().toArray().map(function (elem) { return [BuscadorTipos.UnidadesTributarias, elem[1], elem[0]]; });

        }

        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    if (multiselect) {
                        resolve(evt.seleccion.map(function (ut) { return ut.slice(1); }));
                    } else {
                        resolve(evt.seleccion.slice(1));
                    }
                } else {
                    resolve([]);
                }
            });
        });
    });
}


function destroyDataTable(tableId) {
    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        var table = $(id).dataTable();
        table.api().clear().draw();
        table.api().destroy();

    }
}

function ActualizarGridObjetos() {
    var nroDesde = 0;
    var nroHasta = 2147483647; //max int

    if (!isNaN(parseInt($("#numeroDesde").val(), 10)))
        nroDesde = $("#numeroDesde").val();
    if (!isNaN(parseInt($("#numeroHasta").val(), 10)))
        nroHasta = $("#numeroHasta").val();

    var formData = {
        "buscaFecha": buscaFecha,
        "buscaNumero": buscaNumero,
        "buscaInspectores": buscaInspectores,
        "buscaEstado": buscaEstado,
        "buscaId": buscaId,
        "buscaUnidad": buscaUnidad,
        "fechaDesde": $("#fechaDesde").val(),
        "fechaHasta": $("#fechaHasta").val() + " 23:59:59",
        "numeroDesde": nroDesde,
        "numeroHasta": nroHasta,
        "idActa": $("#NonActaId").attr("value"),
        "idUnidad": $("#UnidadTributariaId").attr("value"),
        "idEstado": $("#idEstadoBusqueda").val(),
        "selectedInspectoresBusqueda": $("#selectedInspectoresBusqueda").val()
    };
    actasEncontradas = null;
    showLoading();
    destroyDataTable("Grilla_ActasResultadoBusqueda");
    $.ajax({
        url: BASE_URL + 'Actas/PostActaBuscar',
        type: "POST",
        data: formData,
        success: function (data) {
            if (!data.length) {
                alerta('No se encontraron datos', 'No se encontraron datos con los parámetros ingresados', 4);
            } else {
                EnablePanel(2);
                Grilla_ActasResultadoBusqueda = $('#Grilla_ActasResultadoBusqueda').DataTable({
                    scrollY: "200px",
                    scrollCollapse: true,
                    paging: true,
                    searching: false,
                    processing: true,
                    dom: 'rtp',
                    language: {
                        url: BASE_URL + "Scripts/dataTables.spanish.txt"
                    },
                    columns: [
                        { name: "0", visible: false },
                        { name: "1" },
                        { name: "2" },
                        { name: "3", render: function (fecha) { return moment(parseJsonDate(fecha)).format("DD/MM/YYYY"); } },
                        { name: "4" },
                        { name: "5" },
                        { name: "6" }
                    ],
                    data: data,
                    initComplete: function () {
                        setEventGrilla_ActasResultadoBusqueda();
                        this.dataTable().api().columns.adjust();
                    }
                });
            }
        },
        error: function (_, __, errorThrown) {
            alerta('Error', errorThrown, 3);
        },
        complete: hideLoading
    });
}

function showActa(id) {
    $.ajax({
        url: BASE_URL + 'Actas/GetActabyId?idActa=' + id,
        type: "GET",
        success: function (acta) {
            $("#selectedUT").val(acta.selectedUT);
            $("#selectedPer").val(acta.selectedPer);
            $("#selectedDocs").val(acta.selectedDocs);
            $("#selectedDom").val(acta.SelectedDomicilio);
            $("#SelectedTipoActa").val(acta.ActaTipoId);

            $('#nroActa').val(acta.NroActa);
            $('#plazo').val(acta.Plazo);
            var fecha = moment(parseJsonDate(acta.Fecha));
            $("#fecha").val(fecha.format("DD/MM/YYYY"));
            $("#hora").val(fecha.format("HH:mm"));

            $("#SelectedInspector").val(acta.InspectorId);
            $("#SelectedEstadoActa").val(acta.SelectedEstadoActa);


            $('#observaciones').val(acta.observaciones);
            LoadUnidadesTributarias($("#selectedUT").val().split(",").reduce(function (accum, ut) {
                if (ut) {
                    accum = [...accum, ut.split("-")];
                }
                return accum;
            }, []));
            LoadPersonas($("#selectedPer").val().split("@"));
            LoadDocumentos($("#selectedDocs").val().split(","));
            LoadDomicilio($("#selectedDom").val().split(","));
            EnablePanel(3);
            _prevPanel = 2;
            SetEnabledActas(false);
            ajustarScrollBars();
        }
    });
}

//@ sourceURL=actas.js