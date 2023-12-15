$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-consultas').on('shown.bs.modal', function (e) {
    ajustarScrollBars();
    hideLoading();



    puntosPredefinidos = $.parseJSON(puntosPredefinidos);
    if (puntosPredefinidos != null) {
        AgregarPuntos(puntosPredefinidos);
    }

    parametrosPredefinidos = $.parseJSON(parametrosPredefinidos);
    if (parametrosPredefinidos != null) {
        agregarParametro(parametrosPredefinidos);
    }

    comprobarEstadoLista();


    setTimeout(function () {

        $('.dataTables_scrollBody').niceScroll(getNiceScrollConfig());
        columnsAdjust('Consultas');
    }, 5);

    if (GeoSIT.isOpenLayers()) {
        $('#btnMapaTematico').hide();
    }

});

function init() {

    var container = $('#modal-window-consultas');
    ///////////////////// Scrollbars ////////////////////////
    $("#consultas-datos").niceScroll(getNiceScrollConfig());

    createDataTable("Consultas", 0, {
        "columnDefs": [{
            "targets": [0],
            "visible": false
        }],
    });

    createDataTable("Parametros", 0, {
        "columns": [
            { "data": "Origen" },
            { "data": "Parametro" },
            { "data": "Query" },
        ]
    });

    $("#Consultas tbody").on("click", "tr", function (e) {
        e.preventDefault();
        e.stopPropagation();

        if ($(this).find('td').length > 1) {
            if ($(this).hasClass("selected")) {
                $(this).removeClass("selected");
                $("#btn_EliminarP").addClass('boton-deshabilitado');
            } else {
                $('#Consultas .selected').removeClass('selected');
                $(this).addClass("selected");
                $("#btn_EliminarP").removeClass('boton-deshabilitado');
            }
        }
    });

    $("#Parametros tbody").on("click", "tr", function (e) {
        e.preventDefault();
        e.stopPropagation();

        if ($(this).find('td').length > 1) {
            if ($(this).hasClass("selected")) {
                $(this).removeClass("selected");
                $("#btn_EliminarParam").addClass('boton-deshabilitado');
            } else {
                $('#Parametros .selected').removeClass('selected');
                $(this).addClass("selected");
                $("#btn_EliminarParam").removeClass('boton-deshabilitado');
            }
        }
    });

    $('#btn_AgregarP').click(function () {
        $('#NuevoPunto').modal('show');
    });

    $('#btn_LimpiarP').click(function () {
        confirmModal("Atención", "Desea eliminar todos los puntos de la lista?", function () {
            $('#Consultas').DataTable().rows().remove().draw();
            $("#btn_EliminarP").addClass('boton-deshabilitado');
            comprobarEstadoLista();
        });
    });

    $('#btn_EliminarP').click(function () {
        confirmModal("Atención", "Desea eliminar el punto seleccionado de la lista?", function () {
            var tr = $("#Consultas tr.selected");
            removeRow("Consultas", tr);
            $("#btn_EliminarP").addClass('boton-deshabilitado');
            comprobarEstadoLista();
        });
    });

    $('#btn_Mapa').click(function () {
        showLoading();
        var puntos = [];
        $.each($('#Consultas').DataTable().data(), function (i, row) {
            puntos.push(row[0]);
        });
        $('#contenedor-externo-consultas').load(BASE_URL + "Consultas/Mapa", {puntos: puntos}, function () {
            $('#NuevoPunto').modal('hide');
        });
    });

    $('#btn_Query').click(function () {
        showLoading();
        $('#contenedor-externo-consultas').load(BASE_URL + "Consultas/Query", null, function () {
            $('#NuevoPunto').modal('hide');
        });
    });

    $('#btn_Lista').click(function () {
        showLoading();
        $('#contenedor-externo-consultas').load(BASE_URL + "Consultas/Listas", null, function () {
            $('#NuevoPunto').modal('hide');
        });
    });

    $('#btn_GuardarP').click(function () {
        $('#guardarLista #nombre').val("");
        $('#guardarLista').modal('show');
    });
    $('#btnGuardarListaOk').click(function () {
        var nombre = $('#guardarLista #nombre').val();

        var FeatId = [];
        $.each($('#Consultas').DataTable().data(), function(i,row){
            FeatId.push(row[0]);
        });

        if (nombre.length == 0) {
            mostrarMensaje(false, "Atención", "Debe introducir un nombre para identificar la consulta");
        } else if (FeatId.length == 0) {
            mostrarMensaje(false, "Atención", "La lista se encuentra vacía");
        } else {
            showLoading();

            $.post(BASE_URL + "Consultas/GuardarLista", {
                nombre: nombre,
                FeatId: FeatId
            }, function () {
                $('#guardarLista').modal('hide');
                hideLoading();
                mostrarMensaje(true, "Guardar Consulta de Puntos", "Los datos se guardaron correctamente");
            });
        }
    });



    $('#btn_AgregarParam').click(function () {
        showLoading();
        $('#contenedor-externo-consultas').load(BASE_URL + "Consultas/Parametro", null, function () {
            $('#NuevoPunto').modal('hide');
        });
    });

    $('#btn_LimpiarParam').click(function () {
        confirmModal("Atención", "Desea eliminar todos los parámetros de la lista?", function () {
            $('#Parametros').DataTable().rows().remove().draw();
            $("#btn_EliminarParam").addClass('boton-deshabilitado');
            comprobarEstadoParametros();
        });
    });

    $('#btn_EliminarParam').click(function () {
        confirmModal("Atención", "Desea eliminar el parámetro seleccionado de la lista?", function () {
            var tr = $("#Parametros tr.selected");
            removeRow("Parametros", tr);
            $("#btn_EliminarParam").addClass('boton-deshabilitado');
            comprobarEstadoParametros();
        });
    });

    $('#FechaHasta').change(function () {
        validarFormulario();
    });

    $('#FechaDesde').change(function () {
        validarFormulario();
    });


    $('#btnExcel').click(function () {
        showLoading();
        $.ajax( {
            url: BASE_URL + "Consultas/Excel",
            data: getDatosConsulta(),
            type: "POST",
            dataType: "json",
            success: function (response) {
                hideLoading();
                if (response.result) {
                    if (response.name != null) {
                        window.location = BASE_URL + "Consultas/DownloadExcel?id=" + response.name;
                    }
                } else {
                    mostrarMensaje(false, "Generación de Consultas", response.msg);
                }
            },
            error: function () {
                hideLoading();
                mostrarMensaje(false, "Generación de Consultas", "Se produjo un error en la consulta");
            }
        });
    });

    $('#btnGrafico').click(function () {
        showLoading();
        $('#contenedor-externo-consultas').load(BASE_URL + "Consultas/Grafico", getDatosConsulta(), function (responseText, textStatus, req) {
            if (textStatus == "error") {
                hideLoading();
                alert("Se produjo un error en la consulta");
            }
        });
    });

    $('#btnMapaTematico').click(function () {
        showLoading();
        $('#contenedor-externo-consultas').load(BASE_URL + "Consultas/MapaTematico", getDatosConsulta(), function (responseText, textStatus, req) {
            if (textStatus == "error") {
                hideLoading();
                alert("Se produjo un error en la consulta");
            }
        });
    });

    ajustarmodal();
    $("#modal-window-consultas").modal("show");

}

function getDatosConsulta() {

    var puntos = [];
    $.each($('#Consultas').DataTable().data(), function (i, row) {
        puntos.push(row[0]);
    });

    var filtros = [];
    $.each($('#Parametros').DataTable().data(), function (i, row) {
        filtros.push(row);
    });

    return {
        FeatIds: puntos,
        Filtros: filtros,
        FechaDesde: $('#FechaDesde').val(),
        FechaHasta: $('#FechaHasta').val()
    };

}


function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter;

    $(".consultas-body", "#scroll-content-consultas").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    temp = $(".consultas-body").height();
    $('#consultas-datos').css({ "max-height": temp + 'px' });
    $("#consultas-datos").getNiceScroll().resize();
    $("#consultas-datos").getNiceScroll().show();
}

function habilitarParametros(habilitar) {
    if (habilitar) {
        $('#headingParametros').removeClass('panel-deshabilitado');
        $('#headingParametros > a').removeClass('collapsed');
        $('#collapseParametros').addClass('in');
    } else {
        $('#headingParametros').addClass('panel-deshabilitado');
        $('#headingParametros > a').addClass('collapsed');
        $('#collapseParametros').removeClass('in');
    }
}
function habilitarFechas(habilitar) {
    if (habilitar) {
        $('#headingFechas').removeClass('panel-deshabilitado');
        $('#headingFechas > a').removeClass('collapsed');
        $('#collapseFechas').addClass('in');
    } else {
        $('#headingFechas').addClass('panel-deshabilitado');
        $('#headingFechas > a').addClass('collapsed');
        $('#collapseFechas').removeClass('in');
    }

    validarFormulario();

}

function createDataTable(tableId, scrollY, options) {


    if (scrollY == null)
        scrollY = "250px";

    var config = {
        dom: "rtp",
        //sScrollY: scrollY,
        //bScrollCollapse: true,
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        }

    };

    config = $.extend(config, options);

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

function comprobarEstadoLista() {
    var cant = $('#Consultas').DataTable().data().length;
    if (cant > 0) {
        $("#btn_LimpiarP").removeClass('boton-deshabilitado');
        $("#btn_GuardarP").removeClass('boton-deshabilitado');
        habilitarParametros(true);
        if ($('#Parametros').DataTable().data().length > 0) {
            habilitarFechas(true);
        } else {
            habilitarFechas(false);
        }
    } else {
        $("#btn_LimpiarP").addClass('boton-deshabilitado');
        $("#btn_GuardarP").addClass('boton-deshabilitado');
        habilitarParametros(false);
        habilitarFechas(false);
    }

    ajustarScrollBars();
}

function SetPuntos(puntos) {
    $('#Consultas').DataTable().rows().remove();
    AgregarPuntos(puntos);
}

function AgregarPuntos(puntos) {
    var puntosDefinitivos = [];
    $.each(puntos, function (i, punto) {
        var existe = false;
        $.each($('#Consultas').DataTable().data(), function (i, row) {
            if (row[0] == punto) {
                existe = true;
            }
        });
        if (!existe) {
            puntosDefinitivos.push(punto);
        }
    });
    showLoading();
    if (puntosDefinitivos.length > 0) {
        $.post(BASE_URL + "Puntos/GetJsonByFeatId", { featId: puntosDefinitivos }, function (response) {
            $.each(response, function (i, punto) {
                data = [punto.FeatID, punto.TipoPunto.Nombre, punto.Nombre];
                addRow("Consultas", data);
                hideLoading();
            });
            comprobarEstadoLista();
        });
    } else {
        hideLoading();
    }
    ajustarScrollBars();
}

function mostrarMensaje(result, title, description) {
    var modal = $('#mensajeModal');
    modal.find('#TituloAdvertencia').html(title);
    modal.find('#DescripcionAdvertencia').html(description);

    if (result) {
        modal.find('#MensajeAlerta').removeClass('alert-warning').addClass('alert-success');
    } else {
        modal.find('#MensajeAlerta').removeClass('alert-success').addClass('alert-warning');
    }
    modal.modal('show');
}

function confirmModal(title, description, callback) {
    var modal = $('#confirmModal');
    modal.find('#TituloAdvertencia').html(title);
    modal.find('#DescripcionAdvertencia').html(description);
    modal.find('#btnAdvertenciaOK').unbind('click').click(callback);
    modal.modal('show');
}

function agregarParametro(data) {
    $.each(data, function (i, parametro) {
        addRow("Parametros", parametro);
    });
    ajustarScrollBars();
    comprobarEstadoParametros();
}


function comprobarEstadoParametros() {
    if ($('#Parametros').DataTable().data().length > 0) {
        $('#btn_LimpiarParam').removeClass('boton-deshabilitado');
    } else {
        $('#btn_LimpiarParam').addClass('boton-deshabilitado');
    }
    comprobarEstadoLista();
}

function validarFormulario() {
    if($('#FechaDesde').val() != ""
        && $('#FechaHasta').val() != ""
        && $('#Consultas').DataTable().data().length > 0
        && $('#Parametros').DataTable().data().length > 0) {
        $('#btnExcel').removeClass('boton-deshabilitado');
        $('#btnGrafico').removeClass('boton-deshabilitado');
        $('#btnMapaTematico').removeClass('boton-deshabilitado');
    } else {
        $('#btnExcel').addClass('boton-deshabilitado');
        $('#btnGrafico').addClass('boton-deshabilitado');
        $('#btnMapaTematico').addClass('boton-deshabilitado');
    }
}