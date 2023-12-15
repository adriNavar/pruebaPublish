$(document).ready(init);
$(window).resize(ajustarmodal);

$('#generacionCertificadosModal').on('shown.bs.modal', function (e) {
    columnsAdjust("resultado-busqueda");
    ajustarScrollBars();
    preloadTramite();
    hideLoading();
});

var selectedRowTramites = null, selectedRowTramitePersona = null, selectedRowTramiteUTS = null, selectedRowTramiteDoc = null;
var documentoModificar;
var aux;//Fue muy de negro usar esto, pero era necesario. Perdon \^_^/
var funcionFinalizado;
var tramiteExpresion;
function init() {
    $(".certificados-content").niceScroll(getNiceScrollConfig());

    $('.certificados-content .panel-heading').click(function () {
        refreshScrollBar();
    });

    $('.certificados-body span').bind('click', function () {
        refreshScrollBar();
    });

    ////////////////////////////////////////////////////////

    $('#heading-busqueda-certificados').click(function () {
        setTimeout(function () {
            columnsAdjust('resultado-busqueda');
        }, 10);
    });

    $('#heading-certificados-uts').click(function () {
        setTimeout(function () { columnsAdjust('unidades-tributarias-certificados'); }, 30);
    });

    $('#heading-personas').click(function () {
        setTimeout(function () { columnsAdjust('personas-certificados'); }, 30);
    });

    $('#heading-documentos').click(function () {
        setTimeout(function () { columnsAdjust('documentos-certificados'); }, 30);
    });

    accordionSearchHandler("UT");
    accordionSearchHandler("Tipo");
    accordionSearchHandler("NumeroTramite");
    accordionSearchHandler("Fecha");
    accordionSearchHandler("Estado");

    $("#generacionCertificadosModal .modal-body").scroll(function () {
        hideDatepicker("#dtFechaDesde");
        hideDatepicker("#dtFechaHasta");
    });

    $("#dtFechaDesde").datepicker(getDatePickerConfig())
        .on("changeDate", function () {
            $("#dtFechaHasta").datepicker("setStartDate", $(this).datepicker("getDate"));
        });

    $("#dtFechaHasta").datepicker(getDatePickerConfig())
        .on("changeDate", function () {
            $("#dtFechaDesde").datepicker("setEndDate", $(this).datepicker("getDate"));
        });

    busquedaInit();

    datosGeneralesFormContent();

    utsInit();
    utsImpresionFormContent();

    personasInit();
    personasImpresionFormContent();
    personasSaveFormContent();

    documentosInit();
    documentosImpresionFormContent();

    informeFinalFormContent();

    $("#save-all,#cancel-all,#print-all").addClass("boton-deshabilitado");
    $("#btnAdvertenciaCierreOK").click(function () {
        //Configura el estado de cierre del trámite.
        //aux = false;
        $("#save-all,#cancel-all,#print-all").addClass("boton-deshabilitado");
        saveAllFunction(true, 4);
    });
    $("#btnCancelarAdvertenciaCierre").click(function () {
        busquedaEnableControls(true);
        columnsAdjust("resultado-busqueda");
    });

    $("#save-all").click(function () {
        var msj;
        if ($("#tramite-numero-verify").not(".boton-deshabilitado").size() === 1) {
            if ($("#tramite-numero-verify").attr("style") != null) {
                msj = "Está a punto de grabar el certificado. ¿Desea Continuar?";
            } else {
                msj = "No ha verificado el Número de Trámite con datos externos ¿Está seguro que desea continuar?";
            }
        } else {
            msj = "Está a punto de grabar el certificado. ¿Desea Continuar?";
        }


        modalConfirm("Grabar - Certificado", msj);
        $("#btnAdvertenciaOK").unbind("click").click(function () {
            saveAllFunction(true, null);
        });
    });

    $("#cancel-all").click(function () {
        modalConfirm("Cancelar - Trámite", "Está a punto de cancelar los cambios. ¿Desea Continuar?");
        $("#btnAdvertenciaOK").unbind("click").click(function () {
            $.post(BASE_URL + "TramitesCertificados/CancelAll", function () {
                initTramite();
            });
        });
    });

    ajustarmodal();
    $("#generacionCertificadosModal").modal("show");

    $("#ut-search").click(function () {
        buscarUnidadesTributarias(false)
            .then(function (data) {
                if (data) {
                    $("#UnidadTributariaId").val(data[1]);
                    $("#UnidadTributaria").val(data[0]);
                }
            })
            .catch(function (err) { console.log(err); });
    });

    //$("#btnCancelarAdvertenciaGuardado").click(function () {
    //    initTramite();
    //})
    //$("#btnAdvertenciaOKGuardado").click(function () {
    //    var ident = $("#txtIdentificador").val();
    //    initTramite();
    //    var parameters = '?pTipoId=' + '' +
    //     '&pNumDesde=' + '' +
    //     '&pNumHasta=' + '' +
    //     '&pFechaDesde=' + '' +
    //     '&pFechaHasta=' + '' +
    //     '&pEstadoId=' + '' +
    //     '&pUnidadT=' + '' +
    //     '&pIdTramite=' + '' +
    //     '&pIdentificador=' + ident;

    //    $("#resultado-busqueda").dataTable().api().ajax.url(BASE_URL + "TramitesCertificados/_ObjetosTramitesCertificados" + parameters).load(function () {

    //        //insertUpdateCertificados();
    //        ////Carga combo de estados y configura el estado actual.
    //        //var estado_tramite = "";
    //        //estado_tramite = $("#txtEstadoTramite").val();

    //        //enableEstados([2, 3]);
    //        //$("#txtEstadoTramite").val(estado_tramite);

    //        //$("#CmbTipoTramiteEditar").enable(false);
    //        //$("#txtIdentificador").prop("readOnly", true);
    //    });

    //    $("#ModalAdvertenciaGuardado").modal("hide");

    //    //$("#accordion-certificados").removeClass("panel-deshabilitado");
    //    //$("#collapse-busqueda-certificados").removeClass("in");
    //});
    //getValidateUT();
}

function preloadTramite() {
    buscaId = parseInt($("#buscaId").attr("value"));
    if (buscaId === 1) {
        var parameters = '?pTipoId=' + '' +
            '&pNumDesde=' + '' +
            '&pNumHasta=' + '' +
            '&pFechaDesde=' + '' +
            '&pFechaHasta=' + '' +
            '&pEstadoId=' + '' +
            '&pUnidadT=' + '' +
            '&pIdTramite=' + $("#tramiteId").val();

        $("#tramiteId").val("");
        $("#resultado-busqueda").dataTable().api().ajax.url(BASE_URL + "TramitesCertificados/_ObjetosTramitesCertificados" + parameters).load();

        $("#tramiteId").remove();
        $("#buscaId").remove();
    }
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 180,
        altura = viewportHeight - headerFooter;
    $(".certificados-body").css({ "height": altura });
    $(".certificados-content").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    temp = $(".certificados-body").height();
    var outerHeight = 20;
    $('#accordion-certificados .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-certificados .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.certificados-content').css({ "max-height": temp + 'px' })
    $('#accordion-certificados').css({ "max-height": temp + 1 + 'px' })
    $(".certificados-content").getNiceScroll().resize();
    $(".certificados-content").getNiceScroll().show();
}

function refreshScrollBar() {
    setTimeout(function () {
        $(".certificados-content").getNiceScroll().resize();
    }, 30);
}

// COMUN
function loadData(data) {
    $.each(['datos-certificados-form', 'uts-impresion-form', 'personas-impresion-form', 'documentos-impresion-form', 'informe-final-form'], function () {
        loadFormData(data, this);
    });
    $("#hdfDefault_Final").val(data.TipoTramite.Plantilla_Final);
    $("#Tramite_Estado").val(data.Estado);
}

function loadFormData(data, form) {
    $.each(data, function (key, value) {
        var control = $("[name='" + key + "']", "#" + form);
        if (control.length > 0) {
            if (control.hasClass("date") && value) {
                var parts = value.split("-");
                var dd = parts[2].split("T");
                control.datepicker("update",
                    $.datepicker.formatDate("dd/mm/yy", new Date(parts[0], parts[1] - 1, dd[0])));
            } else {
                if (control.hasClass('cmn-toggle'))
                    control.prop("checked", value);
                control.val(value);
            }
        }
    });
}

function unselectRow(row) {
    if (row) {
        row.removeClass("selected");
        return null;
    }
}

function viewGridControls(gridId, view) {
    $.each($("#" + gridId + " span"), function (index, element) {
        if (view)
            $(element).show();
        else
            $(element).hide();
    });
}

function enableFormContent(formId, enable) {
    var formElements = $("#" + formId + " :input");
    $.each(formElements, function (index, element) {
        $(element).enable(enable);
    });
}

function insertUpdateCertificados() {
    $("#save-all,#cancel-all").removeClass("boton-deshabilitado");
    $("#print-all").addClass("boton-deshabilitado");
    //$("#save-all,#cancel-all,#print-all").removeClass("boton-deshabilitado");
    datosGeneralesTab(true);
    unidadesTributariasTab(true);
    personasTab(true);
    documentosTab(true);
    seccionesTab(true);
    //informeFinalTab(true);
    seccionTab(true);
    $("#heading-datos-generales").find("a:first[aria-expanded=false]").click();
    $("div[role='region']").not(".deshabilitado").removeClass("panel-deshabilitado");

    $("#heading-busqueda-certificados").find("a:first[aria-expanded='true']").click();
    $("#heading-busqueda-certificados").addClass("panel-deshabilitado");
}

function fixSwitches(container) {
    var cmtoggle = $("#" + container + " .cmn-toggle");
    $.each(cmtoggle, function (index, item) {
        $("#" + item.id).after("<label for=\"" + item.id + "\"></label>");
    });
}

function errorAlert(text) {
    var message = $("#message-error");
    message.find("p").html(text);
    $("#message-error").fadeIn("slow").delay(5000).queue(function () {
        $("#message-error").hide().dequeue();
    });
}

function enableEstados(estados) {
    var enableAll = estados.length == 0;
    $.each($("#Estado option"), function () {
        $(this).remove();
    });
    if (enableAll) {
        $.each($("#CmbEstadoTramite option"), function () {
            var option = $(this);
            $("#Estado").append($("<option></option>")
                .attr("value", option.val())
                .text(option.text()));
        });
    } else {
        $.each(estados, function () {
            var option = $("#CmbEstadoTramite option[value='" + this + "']");
            $("#Estado").append($("<option></option>")
                .attr("value", option.val())
                .text(option.text()));
        });
    }
}

function modalConfirm(title, message) {
    $("#TituloAdvertencia").text(title);
    $("#DescripcionAdvertencia").text(message);
    $("#confirmModal").modal("show");
}

function tieneFiltro(tabId) {
    return $('#collapse' + tabId).hasClass('in');
}

function getTramiteId() {
    var id = 0;
    if (selectedRowTramites)
        id = selectedRowTramites.find("td").eq(0).html();
    return id;
}

function initTramite() {
    selectedRowTramites = unselectRow(selectedRowTramites);
    $("#secciones-certificados").empty();
    $("#resultado-busqueda").dataTable().api().clear().draw();
    busquedaEnableControls(false);
    $("#save-all,#cancel-all,#print-all").addClass("boton-deshabilitado");
    $("#tramite-numero-verify").css("color", "");
    $("#tramite-numero-verify").addClass("boton-deshabilitado");
    columnsAdjust("resultado-busqueda");
}

function accordionSearchHandler(tabname) {
    $("#collapse" + tabname).on("shown.bs.collapse", function () {
        if ($(".panel-collapse", "#accordionBusquedaCertificados").hasClass("in")) {
            $("#clear-all").removeClass("boton-deshabilitado");
        }
    });

    $("#collapse" + tabname).on("hidden.bs.collapse", function () {
        if (!$(".panel-collapse", "#accordionBusquedaCertificados").hasClass("in")) {
            $("#clear-all").addClass("boton-deshabilitado");
        }

        switch (tabname) {
            case "Tipo":
                $("#CmbTipoTramite").val("0");
                break;
            case "NumeroTramite":
                $("input[name=TipoPorNumero]").filter("[value=0]").prop("checked", true);
                $("#txtNumeroDesde").val("");
                $("#txtNumeroHasta").val("");
                break;
            case "Fecha":
                $("input[name=TipoPorFecha]").filter("[value=0]").prop("checked", true);
                $("#dtFechaDesde").datepicker("clearDates");
                $("#dtFechaHasta").datepicker("clearDates");
                break;
            case "Estado":
                $("#CmbEstadoTramite").val("0");
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
            $(".certificados-content").getNiceScroll().resize();
        }, 10);
    });

    refreshScrollBar();
}

function accordionSearchTabHide(accordionTab, heading) {
    var tab = $(accordionTab);
    if (tab.hasClass("in")) {
        tab.collapse("hide");
        $(heading + " .circle").css("background-color", "rgb(255, 255, 255)");
    }
}

function accordionSearchTabShow(accordionTab, heading) {
    var tab = $(accordionTab);
    tab.collapse("show");
    $(heading + " .circle").css("background-color", "rgb(53, 149, 180)");
}

function busquedaEnableControls(enable) {
    if (enable) {
        $("#certificado-edit").click(function () {
            if ($("#txtEstadoTramite").val() === "4") {
                modalConfirm("Cancelar - Tramite", "Está a punto de modificar un Tramite ya finalizado ¿Desea Continuar?");
                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    edicion();
                });
            } else {
                edicion();
            }
        }).removeClass("boton-deshabilitado");
        $("#certificado-delete").click(function () {
            if (selectedRowTramites) {
                var id = selectedRowTramites.find("td").eq(0).html();
                var certificado = selectedRowTramites.find("td").eq(1).html();
                modalConfirm("Eliminar - Certificado", "¿Desea eliminar el certificado  " + certificado + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "TramitesCertificados/DeleteTramiteCertificado?Id_Tramite=" + id,
                        type: "POST",
                        success: function (data) {
                            if (data.success == true) {
                                //initTramite();
                                //selectedRowTramites.remove();
                                $('#resultado-busqueda').dataTable().api().row('.selected').remove().draw();
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");
        $("div[role='region']").removeClass("panel-deshabilitado");
        $("#heading-datos-generales").find("a:first[aria-expanded=false]").click();
    } else {
        $("#heading-busqueda-certificados").removeClass("panel-deshabilitado");
        $("#heading-busqueda-certificados").find("a:first[aria-expanded=false]").click();
        $("div[role='region']").find("a:first[aria-expanded=true]").click();
        $("div[role='region']").addClass("panel-deshabilitado");
        $("#certificado-edit").unbind("click").addClass("boton-deshabilitado");
        $("#certificado-delete").unbind("click").addClass("boton-deshabilitado");
        $("#heading-datos-generales").find("a:first[aria-expanded=false]").click();//PARCHE FIX SOLAPA DATOS-GENERALES BUG
    }
}

// BUSQUEDA
function busquedaInit() {
    createDataTableBusqueda();

    $("#certificado-insert").click(function () {
        $.each(['datos-certificados-form', 'uts-impresion-form', 'personas-impresion-form', 'documentos-impresion-form', 'informe-final-form'], function () {
            $("#" + this)[0].reset();
        });

        $("#txtFechaInicio").val(null);
        selectedRowTramites = unselectRow(selectedRowTramites);
        insertUpdateCertificados();
        enableEstados([1]);
        updateTipoTramite();

        var cargas = [unidadesTributariasList(true), personasList(true), documentosList(true)];
        Promise.all(cargas)
            .finally(function () {
                datosGeneralesTab(true);
                unidadesTributariasTab(true);
                personasTab(true);
                documentosTab(true);

                informeFinalPass();

                informeFinalTab(true);

                seccionTab(true);

                if (!$("#print-all").hasClass("boton-deshabilitado"))
                    $("#print-all").addClass("boton-deshabilitado");

                finalizarOption(false);
            });
    });

    $("#print-all").click(function () {
        aux = false;
        var id = $("#IdTRamite").val();
        if (saveAllFunction(false, null)) {
            openCertificado(id);
            /*var message = "Está a punto de dar finalizado el Trámite, continua?";
            $("#TituloAdvertenciaCierre").text("Cambio de Estado Trámite");
            $("#DescripcionAdvertenciaCierre").text(message);
            $("#confirmCierre").modal("show");*/
        }
    });




    $("#search-all").click(function () {
        selectedRowTramites = null;
        busquedaEnableControls(false);
        $.each(['datos-certificados-form', 'uts-impresion-form', 'personas-impresion-form', 'documentos-impresion-form', 'informe-final-form'], function () {
            $("#" + this)[0].reset();
        });

        var pTipoId, pNumDesde, pNumHasta, pFechaDesde, pFechaHasta, pEstadoId, pUnidadT;

        if (tieneFiltro('UT')) {
            pUnidadT = $("#UnidadTributariaId").attr("value");
        } else {
            pUnidadT = '';
        }

        if (tieneFiltro('Tipo')) {
            pTipoId = $("#CmbTipoTramite").val();
        } else {
            pTipoId = '';
        }

        if (tieneFiltro('NumeroTramite')) {
            pNumDesde = $("#txtNumeroDesde").val();
            pNumHasta = $("#txtNumeroHasta").val();
        } else {
            pNumDesde = '';
            pNumHasta = '';
        }

        if (tieneFiltro('Fecha')) {
            pFechaDesde = $("#dtFechaDesde").datepicker().val();
            pFechaHasta = $("#dtFechaHasta").datepicker().val();
        } else {
            pFechaDesde = '';
            pFechaHasta = '';
        }

        if (tieneFiltro('Estado')) {
            pEstadoId = $("#CmbEstadoTramite").val();
        } else {
            pEstadoId = '';
        }
        var parameters = '?pTipoId=' + pTipoId +
            '&pNumDesde=' + pNumDesde +
            '&pNumHasta=' + pNumHasta +
            '&pFechaDesde=' + pFechaDesde +
            '&pFechaHasta=' + pFechaHasta +
            '&pEstadoId=' + pEstadoId +
            '&pUnidadT=' + pUnidadT;

        $("#resultado-busqueda").dataTable().api().ajax.url(BASE_URL + "TramitesCertificados/_ObjetosTramitesCertificados" + parameters).load(function () {
            columnsAdjust("resultado-busqueda");
        });
    });

    $("#clear-all").click(function () {
        if ($(".panel-collapse", "#accordionBusquedaCertificados").hasClass("in")) {
            $("#CmbTipoTramite").val("0");

            $("#txtNumeroDesde").val("");
            $("#txtNumeroHasta").val("");

            $("#dtFechaDesde").datepicker("clearDates");
            $("#dtFechaHasta").datepicker("clearDates");

            $("#CmbEstadoTramite").val("0");
        }
    });

    $("#resultado-busqueda tbody").on("click", "tr", function () {

        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
            selectedRowTramites = null;
            busquedaEnableControls(false);
            informeFinalPass();
        } else {
            $("tr.selected", "#resultado-busqueda tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowTramites = $(this);
            showLoading();
            new Promise(function (resolve) {

                if (selectedRowTramites.children().hasClass("dataTables_empty")) {
                    selectedRowTramites = null;
                    hideLoading();
                    $("tr.selected", "#resultado-busqueda tbody").removeClass("selected");
                } else {
                    var cargas = [unidadesTributariasList(false), personasList(false), documentosList(false), seccionesList(false)];
                    Promise.all(cargas)
                        .finally(function () {
                            datosGeneralesTab(false);
                            unidadesTributariasTab(false);
                            personasTab(false);
                            documentosTab(false);
                            informeFinalTab(false);
                            busquedaEnableControls(true);

                            //carga de datos
                            var data = $("#resultado-busqueda").dataTable().api().row(selectedRowTramites).data();
                            //Determina la identificación del trámite y construye el reporte.
                            $("#IdTRamite").val(data.Id_Tramite);
                            enableEstados([]);
                            loadData(data);

                            //Agregar permisos Certificados Finalizados
                            if (funcionTramiteFinalizado()) {
                                if (parseInt(data.Estado) === 4) {
                                    $("#certificado-delete").unbind("click").addClass("boton-deshabilitado");
                                }
                                if (parseInt(data.Estado) === 3 || parseInt(data.Estado) === 4) {
                                    if ($("#print-all").hasClass("boton-deshabilitado"))
                                        $("#print-all").removeClass("boton-deshabilitado");
                                } else if (!$("#print-all").hasClass("boton-deshabilitado")) {
                                    $("#print-all").addClass("boton-deshabilitado");
                                }
                            } else {
                                //Si esta Finalizado no permite editar ni borrar el tramite.
                                if (parseInt(data.Estado) === 4) {
                                    $("#certificado-edit").unbind("click").addClass("boton-deshabilitado");
                                    $("#certificado-delete").unbind("click").addClass("boton-deshabilitado");
                                }

                                // Solo permite la impresión cuando esta pendiente de cierre.
                                if (parseInt(data.Estado) === 3) {
                                    if ($("#print-all").hasClass("boton-deshabilitado"))
                                        $("#print-all").removeClass("boton-deshabilitado");
                                } else {
                                    if (!$("#print-all").hasClass("boton-deshabilitado"))
                                        $("#print-all").addClass("boton-deshabilitado");
                                }
                            }
                            resolve();
                        });
                }
            }).finally(function () {
                informeFinalPass();
                finalizarOption(true);
                seccionTab(false);
                setFormatNroTramite();
                hideLoading();
            });
        }
    });
}

function openCertificado(id) {
    window.open(BASE_URL + 'TramitesCertificados/GetInformeTramite/' + id, "_blank");
}

function createDataTableBusqueda() {
    selectedRowTramites = null;
    $("#resultado-busqueda").dataTable({
        scrollY: "150px",
        scrollX: true,
        scrollCollapse: true,
        paging: true,
        searching: false,
        processing: true,
        displayLength: 10,
        dom: 'rtip',
        aaSorting: [[0, "asc"]],
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        data: [],
        columns: [
            { data: "Id_Tramite", "className": "hide" },
            { width: "30%", data: "TipoTramite.Nombre" },
            { width: "15%", data: "Nro_Tramite" },
            { width: "15%", data: "Cod_Tramite" },
            { width: "10%", data: "Fecha", "render": function (data) { return formatDate(data); } },
            { width: "14%", data: "Estado", "className": "hide", "render": function (data) { return NombreEstado(data); } }
        ],
        initComplete: function () {
            columnsAdjust("resultado-busqueda");
        }
    });
}

function cargarDefault(itemId) {
    var defaultText = $("#hdfDefault_" + itemId).val();
    $("#txtSeccion_" + itemId).val(defaultText);
}

function hideDatepicker(inputId) {
    var input = $(inputId);
    input.datepicker("hide");
    input.blur();
}

function formatDate(data) {
    if (!data) return null;
    var parts = data.split("-");
    if (!parts[2]) return data;
    var dd = parts[2].split("T");
    return $.datepicker.formatDate("dd/mm/yy", new Date(parts[0], parts[1] - 1, dd[0]));
}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

function NombreEstado(data) {
    switch (parseInt(data)) {
        case 1:
            return "Iniciado";
        case 2:
            return "En Proceso";
        case 3:
            return "Pendiente de Cierre";
        case 4:
            return "Finalizado";
        case 5:
            return "Anulado";
        default:
            return null;
    }
}

// GENERAL
function datosGeneralesFormContent() {
    var form = $("#datos-certificados-form");
    form.load(BASE_URL + "TramitesCertificados/GeneralFormContent", function () {

        enableFormContent("datos-certificados-form", false);
        //Fix for switches
        fixSwitches('datos-certificados-form');
        $("#CmbTipoTramiteEditar").change(function () {
            $(".panel-heading").not("#heading-datos-generales, #collapse-busqueda-certificados").find("a:first[aria-expanded=true]").click();
            updateTipoTramite();
            seccionTab(true);
            informeFinalPass();
            informeFinalTab(true);
        });

        $("#txtFechaInicio").datepicker(getDatePickerConfig({ enableOnReadonly: false }))
            .on("changeDate", function () {
                form.data("bootstrapValidator")
                    .updateStatus("Fecha", "NOT_VALIDATED", null)
                    .validateField("Fecha");
            });


        $.ajax({
            type: "POST",
            url: BASE_URL + "TramitesCertificados/ExpresionRegularTramite",
            success: function (data) {
                if (data.expresion != "0") {
                    tramiteExpresion = data.patron;
                    form.bootstrapValidator({
                        framework: "bootstrap",
                        excluded: [":disabled"],
                        fields: {
                            Cod_Tramite: {
                                validators: {
                                    callback: {
                                        message: "El campo Identificador es obligatorio",
                                        callback: function () {
                                            return $("#txtIdentificador").val().trim().length > 0;
                                        }
                                    }
                                }
                            },
                            Fecha: {
                                validators: {
                                    callback: {
                                        message: "El campo Fecha de Inicio es obligatorio",
                                        callback: function () {
                                            return $("#txtFechaInicio").val().trim().length > 0;
                                        }
                                    }
                                }
                            },
                            Nro_Tramite: {
                                validators: {
                                    callback: {
                                        message: "El campo debe tener el siguiente formato: " + data.patron,
                                        callback: function (value) {
                                            var numeroTramite = $("#txtNroTramite").val(),
                                                expRegTramite = data.expresion;

                                            if (value.length === 0 && numeroTramite.length === 0)
                                                return false;
                                            if (value.length > 0)
                                                return value.match(expRegTramite) != null;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    });
                } else {
                    tramiteExpresion = null;
                    form.bootstrapValidator({
                        framework: "bootstrap",
                        excluded: [":disabled"],
                        fields: {
                            Cod_Tramite: {
                                validators: {
                                    callback: {
                                        message: "El campo Identificador es obligatorio",
                                        callback: function () {
                                            return $("#txtIdentificador").val().trim().length > 0;
                                        }
                                    }
                                }
                            },
                            Fecha: {
                                validators: {
                                    callback: {
                                        message: "El campo Fecha de Inicio es obligatorio",
                                        callback: function () {
                                            return $("#txtFechaInicio").val().trim().length > 0;
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
            }
        });

        $("#tramite-numero-verify").click(function () {
            $.ajax({
                type: "POST",
                url: BASE_URL + "TramitesCertificados/GetDatosTramiteKontaktar?numero=" + $("#txtNroTramite").val(),
                success: function (data) {
                    if (!data.length) {
                        $("#btnAdvertenciaOK").unbind("click").click(function () {
                            $("#confirmModal").modal("hide");
                        });
                        modalConfirm("Referencia Kontaktar", "El número ingresado no coincide con los datos externos");

                        $("#tramite-numero-verify").css("color", "#ff1a1a");
                    }
                    else {
                        //var list = JSON.parse(data).response.docs;
                        if (data.length == 1) {
                            var obj = data[0];
                            var text = "Se ha validado el tramite con los siguientes datos externos </br>" +
                                "</br>" +
                                "Numero Tramite: " + obj.NroTramite + "</br>" +
                                "Fecha: " + obj.Fecha + "</br>" +
                                "Contribuyente: " + obj.Contribuyente + "</br>" +
                                "Documento: " + obj.DocContribuyente + "</br>" +
                                "Imponible: " + obj.Imponible + "</br>" +
                                "Observaciones: " + obj.Observaciones + "</br>";
                            //TERMINAR

                            $("#btnAdvertenciaOK").unbind("click").click(function () {
                                $("#confirmModal").modal("hide");
                            });

                            modalConfirm("Referencia Kontaktar", "");
                            $("#DescripcionAdvertencia").html(text);
                            $("#tramite-numero-verify").css("color", "#80ffaa"); //Green

                        } else {
                            //DataTable Results
                            var table = $("#resultado-vista").dataTable().api();
                            table.clear().draw();
                            for (var i in data) {
                                var node = table.row.add({
                                    NroTramite: data[i].NroTramite,
                                    Fecha: data[i].Fecha,
                                    Contribuyente: data[i].Contribuyente,
                                    DocContribuyente: data[i].DocContribuyente,
                                    Imponible: data[i].Imponible,
                                    Observaciones: data[i].Observaciones
                                    //TERMINAR
                                }).draw().node();
                            }

                            $("#btnVistaResultsOK").click(function () {
                                $("#ModalVistaResults").modal("hide");
                            })

                            $("#DescripcionVistaResults").text("Se han encontrado " + data.length + " datos con el mismo Número de Trámite");
                            $("#ModalVistaResults").modal("show");
                            $("#tramite-numero-verify").css("color", "#ffad33");
                        }
                    }
                }
            });
        });


        $("#resultado-vista").dataTable({
            "scrollY": "148px",
            "scrollX": true,
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "fixedHeader": true,
            "language": {
                "url": BASE_URL + "Scripts/dataTables.spanish.txt"
            },
            "columns": [
                { "data": "NroTramite" },
                { "data": "Fecha" },
                { "data": "Contribuyente" },
                { "data": "DocContribuyente" },
                { "data": "Imponible" },
                { "data": "Observaciones" }
            ]
        });

        $("#txtNroTramite").change(function () {
            btnVerficarenabled();
        });

        $("#txtNroTramite").keyup(function () {
            btnVerficarenabled();
        });

    });
}

function btnVerficarenabled() {
    $("#tramite-numero-verify").css("color", "");
    if ($("small[data-bv-for='Nro_Tramite'][style='display: block;']").size() > 0 || $("#txtNroTramite").val() == "")
        $("#tramite-numero-verify").addClass("boton-deshabilitado");
    else
        $("#tramite-numero-verify").removeClass("boton-deshabilitado");
}

function datosGeneralesTab(enable) {
    enableFormContent("datos-certificados-form", enable);
}


function updateTipoTramite() {
    var tipoTramite = $("#CmbTipoTramiteEditar").val();
    $.get(BASE_URL + "TramitesCertificados/UpdateTipoTramite/" + tipoTramite, function (data) {
        var numerador = '';
        if (data.Autonumerico) {
            numerador = data.Numerador + 1;
        }
        data.InformeFinal = data.Plantilla_Final;
        $("#txtIdentificador").val(numerador);
        $("#txtIdentificador").prop("readOnly", data.Autonumerico);
        $.each(['uts-impresion-form', 'personas-impresion-form', 'documentos-impresion-form', 'informe-final-form'], function () {
            loadFormData(data, this);
        });
        $("#hdfDefault_Final").val(data.Plantilla_Final);
    });
    seccionesList(true);
}

// UNIDADES TRIBUTARIAS
function utsInit() {
    $("#unidades-tributarias-certificados tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
            selectedRowTramiteUTS = null;
            utsEnableControls(false);
        } else {
            $("tr.selected", "#unidades-tributarias-certificados tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowTramiteUTS = $(this);

            if (!selectedRowTramiteUTS.children().hasClass("dataTables_empty"))
                utsEnableControls(true);
        }
    });

    $("#unidades-tributarias-certificados").dataTable({
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
            { "data": "Id_Unidad_Tributaria", "className": "hide" },
            { "data": "UnidadTributaria.CodigoProvincial" }
        ]
    });

    $("#unidades-insert").click(function () {
        buscarUnidadesTributarias(true)
            .then(function (seleccion) {
                showLoading();
                if (seleccion) {
                    var promises = seleccion.map(function (ut) {
                        return new Promise(function (resolve) {
                            $.ajax({
                                type: "POST",
                                url: BASE_URL + "TramitesCertificados/SaveUts",
                                data: { id: ut[1], id_tramite: getTramiteId() },
                                success: function (resp) {
                                    resolve({ resp: resp, id: parseInt(ut[1]), codigo: ut[0] });
                                }
                            });
                        });
                    });
                    Promise.all(promises)
                        .then(function (data) {
                            var table = $("#unidades-tributarias-certificados").dataTable().api();
                            for (var idx in data) {
                                var elem = data[idx];
                                if (elem.resp === "Ok") {
                                    var node = table.row.add({
                                        'Id_Unidad_Tributaria': elem.id,
                                        'UnidadTributaria': { 'CodigoProvincial': elem.codigo }
                                    }).node();
                                    $(node).find("td:first").addClass("hide");

                                    $("#unidad-tributaria-error").addClass("hide");
                                    $("#save-all").removeClass("boton-deshabilitado");
                                } else {
                                    errorAlert(elem.resp);
                                }
                            }
                            table.rows().draw();
                        })
                        .catch(function (err) { console.log(err); })
                        .finally(hideLoading);
                }
            })
            .catch(function (err) { console.log(err); });
    });
}

function utsImpresionFormContent() {
    var form = $("#uts-impresion-form");
    form.load(BASE_URL + "TramitesCertificados/ImpresionUnidadesTributarias", function () {
        enableFormContent("uts-impresion-form", false);
        fixSwitches('uts-impresion-form');
    });
}

function utsEnableControls(enable) {
    if (enable) {
        $("#unidades-delete").click(function () {
            if (selectedRowTramiteUTS) {
                var id = selectedRowTramiteUTS.find("td").eq(0).html();
                var unidad = selectedRowTramiteUTS.find("td").eq(1).html();
                modalConfirm("Eliminar - Unidad Tributaria", "¿Desea eliminar la unidad tributaria " + unidad + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "TramitesCertificados/DeleteUts/" + id,
                        type: "POST",
                        success: function (data) {
                            var table = $("#unidades-tributarias-certificados").dataTable().api();
                            table.row(".selected").remove().draw(false);
                            selectedRowTramiteUTS = null;
                            $("#unidades-delete").unbind("click").addClass("boton-deshabilitado");
                            validateUnidadesTributarias(table);
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#unidades-delete").unbind("click").addClass("boton-deshabilitado");
    }
}

function validateUnidadesTributarias(table) {
    var isValid;
    $.ajax({
        async: false,
        url: BASE_URL + "TramitesCertificados/getValidateUT",
        type: "POST",
        success: function (data) {
            if (data != "0") {
                table = table || $("#unidades-tributarias-certificados").dataTable().api();
                isValid = table.rows().data().length;
            } else {
                isValid = true;
            }
            if (!isValid) {
                $("#unidad-tributaria-error").removeClass("hide");
                if (!$("#collapse-certificados-uts").hasClass("in"))
                    $('#heading-certificados-uts a').click();

            }
        }
    });

    return isValid;
}

function unidadesTributariasList(insert) {
    return new Promise(function (resolve) {
        var table = $("#unidades-tributarias-certificados").dataTable().api();
        if (insert) {
            table.clear().draw();
            resolve();
        } else {
            var idTramite = getTramiteId();
            table.ajax.url(BASE_URL + "TramitesCertificados/_ObjetosTramiteUTS/" + idTramite).load(function () {
                $("#unidades-delete").addClass("boton-deshabilitado");
                columnsAdjust('unidades-tributarias-certificados');
                resolve();
            });
        }
    });
}

function unidadesTributariasTab(enable) {
    enableFormContent("uts-impresion-form", enable);
    viewGridControls('uts-controls', enable);
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

// PERSONAS
function personasInit() {
    $("#personas-certificados tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowTramitePersona = null;
            personasEnableControls(false);
        } else {
            $("tr.selected", "#personas-certificados tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowTramitePersona = $(this);

            if (selectedRowTramitePersona.children().hasClass("dataTables_empty"))
                selectedRowTramitePersona = null;
            else personasEnableControls(true);
        }
    });

    $("#personas-insert").click(function () {
        $("#personas-panel-body").hide();
        $("#persona-panel-body").show();
    });

    $("#personas-certificado-form").ajaxForm({
        success: function (data) {
            if (data === "Ok") {
                var table = $("#personas-certificados").dataTable().api();
                var node = table.row.add({
                    Id_Persona: $("#Id_Persona").val(),
                    Id_Rol: $("#Id_Rol :selected").val(),
                    Rol: { Descripcion: $("#Id_Rol :selected").text() },
                    Persona: { NombreCompleto: $("#personas-certificado-form #Persona_NombreCompleto").val() }
                }).draw().node();
                $(node).find("td:first").addClass("hide");
                $(node).find("td:nth-child(2)").addClass("hide");

                selectedRowTramitePersona = unselectRow(selectedRowTramitePersona);
                $("#personas-certificado-form").formValidation("resetForm", true);

                $("#personas-panel-body").show();
                $("#persona-panel-body").hide();

                columnsAdjust("personas-certificados");
            } else {
                errorAlert(data);
            }
        }
    });

    $("#personas-certificados").dataTable({
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
            { "data": "Id_Persona", "className": "hide" },
            { "data": "Id_Rol", "className": "hide" },
            { "data": "Rol.Descripcion" },
            { "data": "Persona.NombreCompleto" }
        ]
    });
}

function personasImpresionFormContent() {
    var form = $("#personas-impresion-form");
    form.load(BASE_URL + "TramitesCertificados/ImpresionPersonas", function () {
        enableFormContent("personas-impresion-form", false);
        fixSwitches('personas-impresion-form');
    });
}

function personasList(insert) {
    return new Promise(function (resolve) {
        var table = $("#personas-certificados").dataTable().api();
        $("#personas-panel-body").show();
        $("#persona-panel-body").hide();
        if (insert) {
            table.clear().draw();
            resolve();
        } else {
            var idTramite = getTramiteId();
            table.ajax.url(BASE_URL + "TramitesCertificados/_ObjetosTramitePersonas/" + idTramite).load(function () {
                $("#personas-delete").addClass("boton-deshabilitado");
                columnsAdjust('personas-certificados');
                resolve();
            });
        }
    });
}

function personasTab(enable) {
    enableFormContent("personas-impresion-form", enable);
    enableFormContent("persona-certificado-form", enable);
    viewGridControls('personas-controls', enable);
}

function personasSaveFormContent() {
    var form = $("#personas-certificado-form");
    form.load(BASE_URL + "TramitesCertificados/_LoadPersonaForm", function () {
        $("#persona-submit").click(function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();

            if (bootstrapValidator.isValid())
                $("#personas-certificado-form").submit();
        });

        $("#persona-cancel").click(function () {
            selectedRowTramitePersona = unselectRow(selectedRowTramitePersona);

            form.formValidation("resetForm", true);

            personasEnableControls(false);

            $("#personas-panel-body").show();
            $("#persona-panel-body").hide();
        });

        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            fields: {
                Id_Persona: {
                    validators: {
                        greaterThan: {
                            inclusive: false,
                            message: "La persona no existe, verifíquela",
                            value: 0
                        },
                        notEmpty: {
                            message: "El campo Persona es obligatorio"
                        }
                    }
                },
                Persona_NombreCompleto: {
                    validators: {
                        notEmpty: {
                            message: "El campo Persona es obligatorio"
                        }
                    }
                }
            }
        });
    });
}

function personasEnableControls(enable) {
    if (enable) {
        $("#personas-delete").click(function () {
            if (selectedRowTramitePersona) {
                var id = selectedRowTramitePersona.find("td").eq(0).html();
                var rol = selectedRowTramitePersona.find("td").eq(1).html();
                var razonSocial = selectedRowTramitePersona.find("td").eq(3).html();
                modalConfirm("Eliminar - Persona", "¿Desea eliminar la persona " + razonSocial + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "TramitesCertificados/DeletePersona?idPersona=" + id + "&idRol=" + rol,
                        type: "POST",
                        success: function (data) {
                            if (data == "Ok") {
                                var table = $("#personas-certificados").dataTable().api();
                                table.row(".selected").remove().draw(false);
                                selectedRowTramitePersona = null;
                                $("#personas-delete").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

    } else {
        $("#personas-delete").unbind("click").addClass("boton-deshabilitado");
    }
}

function personaSearch() {
    buscarPersonas()
        .then(function (seleccion) {
            $("#personas-certificado-form #Persona_NombreCompleto").val(seleccion[0]);
            $("#personas-certificado-form #Id_Persona").val(seleccion[1]);
        })
        .catch(function (err) { console.log(err); });
}

function buscarPersonas() {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Personas, multiSelect: false, verAgregar: false, titulo: 'Buscar Persona', campos: ['Nombre', 'dni:DNI'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
                //if (agregarPersona) {
                //    $(window).off('agregarObjetoBuscado');
                //}
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    resolve(evt.seleccion.slice(1));
                } else {
                    resolve();
                }
            });
            //if (agregarPersona) {
            //    $(window).one("agregarObjetoBuscado", function () {
            //        showLoading();
            //        $("#personas-externo-container").load(BASE_URL + "Persona/BuscadorPersona", function () {
            //            $(".modal.mainwnd", this).one('hidden.bs.modal', function () {
            //                $(window).off('personaAgregada');
            //            });
            //            $(window).one("personaAgregada", function (evt) {
            //                resolve(evt.persona.PersonaId);
            //            });
            //        });
            //    });
            //}
        });
    });
}

//function buscarPersonas(multiselect, funcOk, funcCancel) {
//    var form = $("#personas-certificado-form");
//    form.formValidation("resetForm", true);
//    $("#hfresultadobusqueda").val('');
//    BuscadorGenerico(
//        [Buscador_Per], //tipos
//        "certificados-buscador-generico", //ubicacion
//        "hfresultadobusqueda", //devolucion
//        "Personas",
//        "Descripcion",
//        function () { //OK
//            $.get(BASE_URL + 'BuscadorGenerico/GetElements', "elements=" + $("#hfresultadobusqueda").val() + "&tipos=" + Buscador_Per, function (data) { funcOk(data) });
//        },
//        function () { funcCancel(); },
//        multiselect);
//}

// DOCUMENTOS
function documentosInit() {
    $("#documentos-certificados tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowTramiteDoc = null;
            documentosEnableControls(false);
        } else {
            $("tr.selected", "#documentos-certificados tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowTramiteDoc = $(this);

            if (selectedRowTramiteDoc.children().hasClass("dataTables_empty"))
                selectedRowTramiteDoc = null;
            else documentosEnableControls(true);
        }
    });
    $("#documentos-certificados").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt",
        },
        "columns": [
            { "data": "Id_Documento", "className": "hide" },
            { "data": "Documento.Tipo.Descripcion" },
            { "data": "Documento.descripcion" },
            { "data": "Documento.fecha", "render": function (data) { return formatDate(data); } },
            { "data": "Documento.nombre_archivo" }
        ]
    });
    $("#documentos-insert").click(function () {
        showLoading();
        documentoModificar = false;
        $("#abm-documentos-externo").load(BASE_URL + 'Documento/DatosDocumento', function () {
            $(document).one("documentoGuardado", function (evt) {
                documentoGuardado(evt.documento);
            });
        });
    });

    $("#documentos-display, #documentos-view").click(function () {
        showLoading();
        documentoModificar = true;
        var modo = this.id === "documentos-view" ? "SoloLectura" : "Editable";
        $.post(BASE_URL + "Documento/" + modo, function () {
            $("#abm-documentos-externo").load(BASE_URL + 'Documento/DatosDocumento?id=' + $("#documentos-certificados tr.selected td.hide").html(), function () {
                $(document).one("documentoGuardado", function (evt) {
                    documentoGuardado(evt.documento);
                });
            });
        });
    });
}

function documentosTab(enable) {
    enableFormContent("documentos-impresion-form", enable);
    viewGridControls('documentos-controls', enable);
    $("#documentos-view").removeAttr("style", "none");//Agregado para el boton siempre sea visualizado
}

function documentosList(insert) {
    return new Promise(function (resolve) {
        var table = $("#documentos-certificados").dataTable().api();
        if (insert) {
            table.clear().draw();
            resolve();
        } else {
            var idTramite = getTramiteId();
            table.ajax.url(BASE_URL + "TramitesCertificados/_ObjetosTramiteDocumentos/" + idTramite).load(function () {
                $("#documentos-delete").addClass("boton-deshabilitado");
                columnsAdjust('documentos-certificados');
                resolve();
            });
        }
    });
}

function documentosEnableControls(enable) {
    if (enable) {
        $("#documentos-delete").click(function () {
            if (selectedRowTramiteDoc) {
                var id = selectedRowTramiteDoc.find("td").eq(0).html();
                var tipo = selectedRowTramiteDoc.find("td").eq(1).html();
                var descripcion = selectedRowTramiteDoc.find("td").eq(2).html();
                var fecha = selectedRowTramiteDoc.find("td").eq(3).html();
                var archivo = selectedRowTramiteDoc.find("td").eq(4).html();
                modalConfirm("Eliminar - Documento", "¿Desea eliminar el documento tipo " + tipo +
                    ", descripción " + descripcion + ", fecha " + fecha + ", archivo " + archivo + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "TramitesCertificados/DeleteDocumento/" + id,
                        type: "POST",
                        success: function (data) {
                            if (data === "Ok") {
                                var table = $("#documentos-certificados").dataTable().api();
                                table.row(".selected").remove().draw(false);
                                selectedRowTramiteDoc = null;
                                $("#documentos-delete").unbind("click").addClass("boton-deshabilitado");
                                //$("#documentos-modify").unbind("click").addClass("boton-deshabilitado");
                                $("#documentos-display").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#documentos-display , #documentos-view").removeClass("boton-deshabilitado");

        //$("#documentos-view").click(function () {
        //    //if (selectedRowTramiteDoc) {
        //    //    var id = selectedRowTramiteDoc.find("td").eq(0).html();
        //    //    showLoading();
        //    //    $("#contenedor-forms-externos").load(BASE_URL + 'Documento/DatosDocumento?id=' + id);
        //    //}
        //    //if (selectedRowTramiteDoc) {
        //    //    var data = $("#imagenVisualizar").attr("src"),
        //    //   filename = $('#DatosDocumento_nombre_archivo').val(),
        //    //   id = selectedRowTramiteDoc.find("td").eq(0).html();
        //    //    $.ajax({
        //    //        async: false,
        //    //        type: 'GET',
        //    //        url: BASE_URL + 'Documento/GetDataBaseState',
        //    //        dataType: 'json',
        //    //        success: function (data) {
        //    //            if (data == "1")
        //    //                window.location = BASE_URL + 'Documento/Download?id=' + id;
        //    //            else
        //    //                window.location = BASE_URL + 'Documento/DownloadServerFile?id=' + id;
        //    //        }

        //    //    })
        //    //}
        //}).removeClass("boton-deshabilitado");
    } else {
        $("#documentos-delete").unbind("click").addClass("boton-deshabilitado");
        //$("#documentos-modify").unbind("click").addClass("boton-deshabilitado");
        $("#documentos-display").unbind("click").addClass("boton-deshabilitado");
        $("#documentos-view").unbind("click").addClass("boton-deshabilitado");
    }
}

function documentosImpresionFormContent() {
    var form = $("#documentos-impresion-form");
    form.load(BASE_URL + "TramitesCertificados/ImpresionDocumentos", function () {
        enableFormContent("documentos-impresion-form", false);
        fixSwitches('documentos-impresion-form');
    });
}

function documentoGuardado(documento) {
    $.ajax({
        type: "POST",
        url: BASE_URL + "TramitesCertificados/SaveDocumento?id=" + documento.id_documento + "&modificar=" + documentoModificar,
        success: function (responseText) {
            if (responseText === "Ok") {
                var table = $("#documentos-certificados").dataTable().api();
                var node = table.row.add({
                    Id_Documento: documento.id_documento,
                    Documento: documento,
                }).draw().node();
                $(node).find("td:first").addClass("hide");

                $("#save-all").removeClass("boton-deshabilitado");
            } else {
                errorAlert(responseText);
            }
        }
    });
}

// SECCIONES
function seccionesList(insert) {
    return new Promise(function (resolve) {
        var metodo, idTramite = getTramiteId();
        if (insert) {
            var idTipoTramite = $("#CmbTipoTramiteEditar").val();
            metodo = "TramitesCertificados/_ObjetosTramiteSeccionesByTipoTramite/" + idTipoTramite;
        } else {
            metodo = "TramitesCertificados/_ObjetosTramiteSecciones/" + idTramite;
        }
        $("#secciones-certificados").load(BASE_URL + metodo, function () {
            fixSwitches('secciones-certificados');
            $('#secciones-certificados .panel-heading').click(function () {
                refreshScrollBar();
            });
            $('#secciones-certificados span').bind('click', function () {
                refreshScrollBar();
            });
            resolve();
        });
    });
}

function seccionesTab(enable) {
    $("#secciones-certificados .collapse.permisohabilitado").toArray().forEach(function (element) {
        $("#" + element.id + " :input").toArray().forEach(function (input) { $(input).enable(enable); });
        $("#" + element.id + " span").toArray().forEach(function (span) {
            if (view) {
                $(span).removeClass("boton-deshabilitado");
            } else {
                $(span).addClass("boton-deshabilitado");
            }
        });
    });
}

// INFORME FINAL
function informeFinalFormContent() {
    var form = $("#informe-final-form");
    form.load(BASE_URL + "TramitesCertificados/InformeFinalFormContent", function () {
        enableFormContent("informe-final-form", false);
        //Fix for switches
        fixSwitches('informe-final-form');
    });
}

function informeFinalTab(enable) {
    if ($("#informe-final-form.noEditable").size() === 0) {
        enableFormContent("informe-final-form", enable);
        viewGridControls('informe-final-controls', enable);
    } else {
        enableFormContent("informe-final-form", false);
        viewGridControls('informe-final-controls', false);
    }
}

function seccionTab(enable) {
    $(".seccion").not(".noEditable").map(function () {
        var id = $(this).attr("id");
        enableFormContent(id, enable);

        $.each($("#" + id + " #seccion-controls span"), function (_, element) {
            if (enable) {
                $(element).removeClass("boton-deshabilitado");
            } else {
                $(element).addClass("boton-deshabilitado");
            }
        });
    });
}

function saveAllFunction(reinicie, cambioestado) {
    var form = $("#datos-certificados-form");
    form.formValidation("revalidateField", "Cod_Tramite")
        .formValidation("revalidateField", "Fecha");
    var bootstrapValidator = form.data("bootstrapValidator");
    if (!bootstrapValidator) {
        form.bootstrapValidator({
            framework: "bootstrap",
            excluded: [":disabled"],
            fields: {
                Cod_Tramite: {
                    validators: {
                        callback: {
                            message: "El campo Identificador es obligatorio",
                            callback: function () {
                                return $("#txtIdentificador").val().trim().length > 0;
                            }
                        }
                    }
                },
                Fecha: {
                    validators: {
                        callback: {
                            message: "El campo Fecha de Inicio es obligatorio",
                            callback: function () {
                                return $("#txtFechaInicio").val().trim().length > 0;
                            }
                        }
                    }
                }
            }
        });
        bootstrapValidator = form.data("bootstrapValidator");
    }
    bootstrapValidator.validate();

    if (bootstrapValidator.isValid() && getValidateUT()) {
        var ret = true;
        showLoading();

        var modelSecciones = eval($("#secciones-json").val());

        $.each(modelSecciones, function (idx, obj) {
            var nombreControl = "txtSeccion_" + obj.Id_Tipo_Seccion;
            var nombreCheck = "chkSeccion_" + obj.Id_Tipo_Seccion;

            modelSecciones[idx].Detalle = $("#" + nombreControl).val();
            modelSecciones[idx].Imprime = $("#" + nombreCheck).is(":checked");
        });

        $.ajax({
            async: false,
            url: BASE_URL + "TramitesCertificados/PostSecciones",
            type: "POST",
            dataType: "json",
            data: { secciones: modelSecciones },
            success: function (data) {
                if (data["success"]) {
                    modelSecciones = [];
                    var var_estado = "";
                    if (cambioestado) {
                        //var_estado = cambioestado;
                        var_estado = $("#txtEstadoTramite option[value=3]").val();
                    } else {
                        //var_estado = $("#txtEstadoTramite").val();
                        var_estado = $("#txtEstadoTramite option[value=3]").val();
                    }

                    var tipoTramiteId = $('#CmbTipoTramiteEditar option:selected').map(function () { return this.value; }).get();

                    var cert = {
                        Tipo: tipoTramiteId,
                        Identificador: $("#txtIdentificador").val(),
                        FechaInicio: $("#txtFechaInicio").val(),
                        NumeroTramite: $("#txtNroTramite").val(),
                        InformeFinal: $("#txtSeccion_Final").val(),
                        Estado: var_estado,
                        ImprimeUTS: $("#Imprime_UTS").is(":checked"),
                        ImprimePersonas: $("#Imprime_Per").is(":checked"),
                        ImprimeDocumentos: $("#Imprime_Doc").is(":checked"),
                        ImprimeInformeFinal: $("#Imprime_Final").is(":checked")
                    };
                    $.ajax({
                        async: false,
                        type: "POST",
                        url: BASE_URL + "TramitesCertificados/SaveAll",
                        data: cert,
                        success: function (data) {
                            if (isNaN(data)) {
                                ret = false;
                                errorAlert(data);
                            } else if (reinicie) {
                                initTramite();
                                $("#print-save").off("click").one("click", function () {
                                    openCertificado(data);
                                });
                                $("#confirmPrint").modal("show");
                            }
                        }
                    });
                }
            }
        });
        hideLoading();
        return ret;
    } else {
        errorAlert("Los datos ingresados no son válidos, por favor revíselos e intente nuevamente.");
        return false;
    }
}
//consulta si es necesario validar las unidades tributarias a la hora de grabar.
function getValidateUT() {
    var result = null;
    $.ajax({
        async: false,
        url: BASE_URL + "TramitesCertificados/getValidateUT",
        type: "POST",
        //dataType: "json",
        //data: { secciones: modelSecciones },
        success: function (data) {
            result = data;
        }
    });

    if (result != "0") {
        return validateUnidadesTributarias();
    } else {
        //$("#unidad-tributaria-error").addClass("hide");
        $("#unidad-tributaria-error").remove();
        return true;
    }
}
function informeFinalPass() {
    $.ajax({
        type: "POST",
        url: BASE_URL + "TramitesCertificados/VerificarFuncionesJson?idSeccion=0&idTipoTramite=" + $("#CmbTipoTramiteEditar").val() + "&text=Visualizar",
        async: false,
        success: function (data) {
            if (data == true) {
                $("#heading-informe-final").removeClass("panel-deshabilitado");
                $("#heading-informe-final").removeClass("deshabilitado");
                $.ajax({
                    type: "POST",
                    async: false,
                    url: BASE_URL + "TramitesCertificados/VerificarFuncionesJson?idSeccion=0&idTipoTramite=" + $("#CmbTipoTramiteEditar").val() + "&text=Editar",
                    success: function (dato) {
                        if (dato == false) {
                            $("#informe-final-form").addClass("noEditable");
                        }
                        else {
                            $("#informe-final-form").removeClass("noEditable");
                        }
                    }
                });
            } else {
                $("#heading-informe-final").addClass("panel-deshabilitado");
                $("#heading-informe-final").addClass("deshabilitado");
            }
        }
    });
}

function finalizarOption(parameter) {
    if (parameter == true) {
        $("#txtEstadoTramite option[value=4]").show();
    } else {
        $("#txtEstadoTramite option[value=4]").hide();
    }
}
//@ sourceURL=tramites.js

function edicion() {
    insertUpdateCertificados();
    //Carga combo de estados y configura el estado actual.
    var estado_tramite = "";
    //estado_tramite = $("#txtEstadoTramite").val();
    estado_tramite = $("#txtEstadoTramite option[value=3]").val();

    enableEstados([2, 3]);
    $("#txtEstadoTramite").val(estado_tramite);

    $("#CmbTipoTramiteEditar").enable(false);
    $("#txtIdentificador").prop("readOnly", true);
    //VerficarPermiso
    if (funcionTramiteFinalizado() == true) {
        //EDITAR PERMISO FINALIZADO
        if ($("#txtEstadoTramite option").val() == 4) {
            finalizarOption(true);
        } else {
            finalizarOption(false);
        }
        $("#txtEstadoTramite").attr("disabled", true);
    } else {
        //EDITAR SIN PERMISO FINALIZADO
        finalizarOption(false);
    }
    informeFinalTab(true);
    $("#tramite-numero-verify").removeClass("boton-deshabilitado");
}

function funcionTramiteFinalizado() {
    if (funcionFinalizado == null) {
        showLoading();
        $.ajax({
            url: BASE_URL + "TramitesCertificados/PermisosTramiteFinalizado",
            type: "POST",
            success: function (result) {
                funcionFinalizado = result;
                hideLoading();
            }
        })
    }

    return funcionFinalizado;
}

function setFormatNroTramite() {
    if (tramiteExpresion != undefined || tramiteExpresion != null) {
        var space = "";
        var txtNroTramite = $("#txtNroTramite").val();
        for (i = 0; i < tramiteExpresion.length - txtNroTramite.length; i++) {
            space += "0";
        }
        $("#txtNroTramite").val(space + txtNroTramite);
    }
}

//# sourceURL=pp.js