var IdOrigen = null,
        query = "";

$(document).ready(init);
$(window).resize(ajustarmodalQuery);
$('#modal-window-query').on('shown.bs.modal', function (e) {
    ajustarScrollBarsQuery();

    setTimeout(function () {
        $('.dataTables_scrollBody').niceScroll(getNiceScrollConfig());
        columnsAdjust('Grilla_Consulta_Parametros');
        updateEditor();
        ajustarScrollBarsQuery();
    }, 5);
});

function init() {

    var container = $('#modal-window-query');

    ///////////////////// Scrollbars ////////////////////////
    $("#query-datos").niceScroll(getNiceScrollConfig());
    $('#scroll-content-query .panel-body').resize(ajustarScrollBarsQuery);
    $('#query-datos .panel-heading').click(function () {
        setTimeout(function () {
            $("#query-datos").getNiceScroll().resize();
        }, 10);
    });

    IdOrigen = container.find('#Origen').val();

    container.find('#Origen').on('change', function () {
        IdOrigen = $(this).val();
        reloadParametros();
        if (IdOrigen == 1) { //LIMS
            container.find('#accordionBusqueda').show();
        } else {
            container.find('#accordionBusqueda').hide();
            container.find('#accordionBusqueda select').val("");
            container.find('#accordionBusqueda .CircleCollapse').removeClass("in");
        }
    });


    var timeOutSearch = null;
    container.find('#Filtro').on('keyup', function () {
        if (timeOutSearch != null) {
            clearTimeout(timeOutSearch);
        }
        query = $(this).val();
        timeOutSearch = setTimeout(function () {
            $('#listaParametros .listado').jstree(true).search(query);
        }, 500);
    });

    createDataTable("Grilla_Consulta_Parametros", null, {
        "bSort": false,
        "columnDefs": [{
            "targets": 0,
            "render": function (data, type, full, meta) {
                if (data != null && data != "") {
                    return $("#CmbComparador option[value='" + data + "']").html();
                } else {
                    return "";
                }
            }
        }, {
            "targets": 2,
            "render": function (data, type, full, meta) {
                if (data != null && data != "") {
                    return $("#CmbConector option[value='" + data + "']").html();
                } else {
                    return "";
                }
            }
        }]
    });

    $("#Grilla_Consulta_Parametros tbody").on("click", "tr", function () {
        if ($(this).find('td').length > 1) {
            if ($(this).hasClass("selected")) {
                $(this).removeClass("selected");
                selectedLayerId = 0;
                $("#btn_EliminarQ").addClass('boton-deshabilitado');
                $("#btn_ModificarQ").addClass('boton-deshabilitado');
            } else {
                $("tr.selected", "#Grilla_Consulta_Parametros tbody").removeClass("selected");
                $(this).addClass("selected");

                ConsultaId = Number($(this).find("td:First").html());

                ConsultaComp = $(this).find("td:eq(2)").text().trim();
                ConsultaAtributo = $(this).find("td:eq(1)").text();
                ConsultaValor = $(this).find("td:eq(3)").text();
                ConsultaConec = $(this).find("td:eq(4)").text().trim();

                $("#CmbAtributo").val(ConsultaAtributo).trigger('change');
                $("#CmbComparador option").filter(function () { return (ConsultaComp == $(this).html()); }).prop("selected", true);
                $("#CmbConector option").filter(function () { return (ConsultaConec == $(this).html()); }).prop("selected", true);
                $("#txtValor").val(ConsultaValor);
                $("#txtValorSelect").val(ConsultaValor);

                $("#btn_EliminarQ").removeClass('boton-deshabilitado');
                $("#btn_ModificarQ").removeClass('boton-deshabilitado');

            }
        }
    });

    $("#btn_AgregarQ").click(function () {
        var comparador = $("#CmbComparador").val();
        var valor = $("#txtValor").val();
        var conector = $('#CmbConector').val();

        if (conector == 3 || conector == 4) {
            comparador = null;
            valor = null;
        }

        addRow("Grilla_Consulta_Parametros", [comparador, valor, conector]);
        comprobarBotonesGrillaConsulta();
    });

    $("#btn_ModificarQ").click(function () {
        var comparador = $("#CmbComparador").val();
        var valor = $("#txtValor").val();
        var conector = $('#CmbConector').val();

        updateRow("Grilla_Consulta_Parametros", $('#Grilla_Consulta_Parametros tr.selected'), [comparador, valor, conector]);
    });

    $("#btn_EliminarQ").click(function () {
        removeRow("Grilla_Consulta_Parametros", $('#Grilla_Consulta_Parametros tr.selected'));
        $("#btn_ModificarQ").addClass('boton-deshabilitado');
        $("#btn_EliminarQ").addClass('boton-deshabilitado');
        comprobarBotonesGrillaConsulta();
    });

    $("#btn_LimpiarQ").click(function () {
        $('#Grilla_Consulta_Parametros').DataTable().rows().remove().draw();
        $("#btn_ModificarQ").addClass('boton-deshabilitado');
        $("#btn_EliminarQ").addClass('boton-deshabilitado');
        comprobarBotonesGrillaConsulta();
    });

    ajustarmodalQuery();
    $("#modal-window-query").modal("show");

    $('.CircleCollapse a').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(this).parents('.CircleCollapse').toggleClass('in');
        ajustarScrollBarsQuery();
    });


    container.find('#btnAgregarQuery').click(function () {
        if (validarQuery()) {
            var data = {
                IdTipoPunto: $('#CmbTipoPunto').val(),
                Parametro: {
                    IdParametro: parametroData.Id_Parametro,
                    IdOrigen: parametroData.Id_Origen,
                    Filtros: getFiltros(),
                },
                FechaDesde: $('#modal-window-query #FechaDesde').val(),
                FechaHasta: $('#modal-window-query #FechaHasta').val()
            };

            showLoading();
            $.post(BASE_URL + "Consultas/QueryPost", data, function (response) {
                hideLoading();
                if (response.result && response.puntos != null && response.puntos.length > 0) {
                    if (typeof AgregarPuntos == "function") {
                        AgregarPuntos(response.puntos);
                    }
                    container.modal('hide');
                } else {
                    mostrarMensaje(false, "Generador de Query Punto", "No se encontraron puntos para la busqueda realizada");
                }
            });


        }
    });

    cargarParametros();
}

function getFiltros() {
    var parametros = [];
    $.each($('#Grilla_Consulta_Parametros').DataTable().data(), function (i, row) {

        var conector = "";
        switch (parseInt(row[2])) {
            case 1:
                conector = "AND";
                break;
            case 2:
                conector = "OR";
                break;
            case 3:
                conector = "(";
                break;
            case 4:
                conector = ")";
                break;
            case 5:
                conector = "XOR";
                break;
        }
        var signo = "";
        switch (parseInt(row[0])) {
            case 1:
                signo = "=";
                break;
            case 2:
                signo = "!=";
                break;
            case 3:
                signo = "<";
                break;
            case 4:
                signo = "<=";
                break;
            case 5:
                signo = ">";
                break;
            case 6:
                signo = ">=";
                break;
        }
        parametros.push({
            conector: conector,
            signo: signo,
            valor: row[1]
        });

    });

    if ($('#Entidad').val() != "") {
        parametros.push({
            parametro: "ENTIDAD",
            valor: $('#Entidad option:selected').html()
        });
    }

    if ($('#CausaExtraccion').val() != "") {
        parametros.push({
            parametro: "EXTRACCION",
            valor: $('#CausaExtraccion option:selected').html()
        });
    }

    if ($('#EstudioEspecial').val() != "") {
        parametros.push({
            parametro: "ESTUDIO",
            valor: $('#EstudioEspecial option:selected').html()
        });
    }
    return parametros;
}

function getQueryParametro() {
    var query = "";
    $.each($('#Grilla_Consulta_Parametros').DataTable().data(), function (i, row) {

        var conector = $("#CmbConector option[value='" + row[2] + "']").html();


        if (row[0] != null) {
            if (query.length > 0 && query.substr(-1, 1) != "(") {
                query += " " + conector + " ";
            }

            var signo = "";
            switch (parseInt(row[0])) {
                case 1:
                    signo = "=";
                    break;
                case 2:
                    signo = "&ne;";
                    break;
                case 3:
                    signo = "<";
                    break;
                case 4:
                    signo = "&le;";
                    break;
                case 5:
                    signo = ">";
                    break;
                case 6:
                    signo = "&ge;";
                    break;
            }
            query += "valor " + signo + " " + row[1];
        } else {
            if (query.length > 0 && conector != ")") {
                query += " ";
            }
            query += conector;
        }

    });

    if ($('#Entidad').val() != "") {
        if (query.length > 0) {
            query += ". ";
        }
        query += "Entidad: " + $('#Entidad option:selected').html();
    }

    if ($('#CausaExtraccion').val() != "") {
        if (query.length > 0) {
            query += ". ";
        }
        query += "Causa Extracción: " + $('#CausaExtraccion option:selected').html();
    }

    if ($('#EstudioEspecial').val() != "") {
        if (query.length > 0) {
            query += ". ";
        }
        query += "Estudio Especial: " + $('#EstudioEspecial option:selected').html();
    }
    return query;
}

var parametroData;

function validarQuery() {
    var valid = true;

    var selected = false;
    parametroData = null;
    $.each($('#listaParametros .listado').jstree("get_selected"), function (i, id) {
        var data = $('#listaParametros .listado').jstree(true).get_node(id).data;
        if (typeof data.parametro != "undefined") {
            parametroData = data.parametro;
            selected = true;
        }
    });

    if (!selected) {
        mostrarMensaje(false, "Error", "Debe seleccionar un parámetro");
        valid = false;
    }

    return valid;
}

function comprobarBotonesGrillaConsulta() {
    if ($('#Grilla_Consulta_Parametros').DataTable().data().length > 0) {
        $("#btn_LimpiarQ").removeClass('boton-deshabilitado');
    } else {
        $("#btn_LimpiarQ").addClass('boton-deshabilitado');
    }
}

function ajustarmodalQuery() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter;

    $(".query-body", "#scroll-content-query").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBarsQuery();
}

function ajustarScrollBarsQuery() {
    temp = $(".query-body").height();
    $('#query-datos').css({ "max-height": temp + 'px' });
    $("#query-datos").getNiceScroll().resize();
    $("#query-datos").getNiceScroll().show();
}

function createDataTable(tableId, scrollY, options) {

    var config = {
        dom: "rt",
        scrollY: scrollY,
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        },

    };

    config = $.extend(config, options);

    if (scrollY == null)
        scrollY = "250px";

    $("#" + tableId).DataTable(config);
}

function addRow(tableId, data) {
    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        var table = $(id).DataTable();
        table.row.add(data).draw();
    }
}
function updateRow(tableId, row, data) {
    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        var table = $(id).DataTable();
        table.row(row).data(data).draw();
    }
}

function removeRow(tableId, row) {
    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        var table = $(id).DataTable();
        table.row(row).remove().draw();
    }
}

function destroyDataTable(tableId) {
    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        var table = $(id).dataTable();
        table.api().clear().draw();
        table.api().destroy();

    }
}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

function reloadParametros() {
    cargarParametros();
    $('#Filtro').val('');
}

function cargarParametros() {
    var container = $('#modal-window-query');

    container.find('#listaParametros').load(BASE_URL + "Parametros/GetParametros", { IdOrigen: IdOrigen }, function () {

        container.find('#listaParametros .listado')
        .on('loaded.jstree', function () {
            setTimeout(function () {
                updateEditor();
            }, 50);
            if ($('#listaParametros .listado').jstree("get_selected").length == 1) {
                habilitarFechasQuery(true);
            } else {
                habilitarFechasQuery(false);
            }
        })
        .on('select_node.jstree', function (e, data) {
            setTimeout(function () {
                updateEditor();
            }, 50);
            habilitarFechasQuery(true);
        })
        .on('deselect_node.jstree', function (e, data) {
            setTimeout(function () {
                updateEditor();
            }, 50);
            habilitarFechasQuery(false);
        })
        .jstree({
            'conditionalselect': function (node) {
                return (typeof node.data.parametro != "undefined")
            },
            "core": {
                "multiple": false,
                "animation": 0,
                "themes": {
                    "icons": false
                },
            },
            "checkbox": {
                "three_state": false,
                "whole_node": false
            },
            "plugins": ["checkbox", "types", "conditionalselect", "search"]
        });

        hideLoading();

    });


}

(function ($, undefined) {
    "use strict";
    $.jstree.defaults.conditionalselect = function () { return true; };

    $.jstree.plugins.conditionalselect = function (options, parent) {
        // own function
        this.select_node = function (obj, supress_event, prevent_open) {
            if (this.settings.conditionalselect.call(this, this.get_node(obj))) {
                parent.select_node.call(this, obj, supress_event, prevent_open);
            }
        };
    };
})(jQuery);

function updateEditor() {
    var selected = 0;
    $.each($('#listaParametros .listado').jstree("get_selected"), function (i, id) {
        var data = $('#listaParametros .listado').jstree(true).get_node(id).data;
        if (typeof data.parametro != "undefined") {
            selected++;
        }
    });
    if (selected == 1) {
        $('#editor').removeClass('fullDisabled');
        $('#accordionBusqueda').removeClass('fullDisabled');
    } else {
        $('#editor').addClass('fullDisabled');
        $('#accordionBusqueda').addClass('fullDisabled');
    }
}

function habilitarFechasQuery(habilitar) {
    if (habilitar) {
        $('#headingQueryFechas').removeClass('panel-deshabilitado');
        $('#headingQueryFechas > a').removeClass('collapsed');
        $('#collapseQueryFechas').addClass('in');
    } else {
        $('#headingQueryFechas').addClass('panel-deshabilitado');
        $('#headingQueryFechas > a').addClass('collapsed');
        $('#collapseQueryFechas').removeClass('in');
    }

    validarFormularioQuery();

}

function validarFormularioQuery() {
    if ($('#FechaDesde').val() != ""
        && $('#FechaHasta').val() != ""
        && $('#listaParametros .listado').jstree("get_selected").length == 1) {
        $('#btnAgregarQuery').removeClass('boton-deshabilitado');
    } else {
        $('#btnAgregarQuery').addClass('boton-deshabilitado');
    }
}