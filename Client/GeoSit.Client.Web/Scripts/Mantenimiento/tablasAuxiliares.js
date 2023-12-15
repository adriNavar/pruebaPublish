$(document).ready(init);
$(window).resize(ajustarmodalTablasAuxiliares);
$('#modal-window-tablas-auxiliares').on('shown.bs.modal', function (e) {
    ajustarScrollBarsTablasAuxiliares();
    hideLoading();
    columnsAdjust('Tablas');
});

function init() {
    var container = $('#modal-window-tablas-auxiliares');
    ///////////////////// Scrollbars ////////////////////////
    $("#tablas-auxiliares-datos").niceScroll(getNiceScrollConfig(true));

    createDataTable("Tablas", 0, {
        dom: "frtip",
        pageLength: 5,
        scrollCollapse: true,
        columnDefs: [{
            "targets": [0],
            "visible": false
        }, {
            "targets": [1],
            "width": "50%"
        }]
    });
    $("#Tablas_wrapper").css('width', '100%;');
    $('#Tablas').on('draw.dt', function () {
        habilitarResultado();
    });

    $("#Tablas tbody").on("click", "tr", function (e) {
        e.preventDefault();
        e.stopPropagation();

        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        } else {
            $('#Tablas .selected').removeClass('selected');
            $(this).addClass("selected");
        }
        habilitarResultado();
    });

    ajustarmodalTablasAuxiliares();
    $("#modal-window-tablas-auxiliares").modal("show");

}


function ajustarmodalTablasAuxiliares() {
    var viewportHeight = $(window).height(),
        headerFooter = 130,
        altura = viewportHeight - headerFooter;

    $(".tablas-auxiliares-body", "#scroll-content-tablas-auxiliares").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBarsTablasAuxiliares();
}
function ajustarScrollBarsTablasAuxiliares() {
    temp = $(".tablas-auxiliares-body").height();
    $('#tablas-auxiliares-datos').css({ "max-height": temp + 'px' });
    $("#tablas-auxiliares-datos").getNiceScroll().resize();
    $("#tablas-auxiliares-datos").getNiceScroll().show();
}

function habilitarResultado() {
    var habilitar = $("#Tablas tbody tr.selected").length > 0;
    if (habilitar) {
        var data = $('#Tablas').DataTable().row($("#Tablas tbody tr.selected")).data();
        $('#headingResultado').removeClass('panel-deshabilitado');
        $('#headingResultado > a').removeClass('collapsed');
        $('#collapseResultado').addClass('in');
        showLoading();
        $('#collapseResultado > div').load(BASE_URL + "Mantenimiento/TablasAuxiliaresData?id=" + data[0], function () {
            hideLoading();
            funcionesResultado();
        });
    } else {
        $('#headingResultado').addClass('panel-deshabilitado');
        $('#headingResultado > a').addClass('collapsed');
        $('#collapseResultado').removeClass('in');
    }
    setTimeout(refreshScrollBarsTablasAuxiliares(), 100);
}

function refreshScrollBarsTablasAuxiliares() {
    $("#tablas-auxiliares-datos").getNiceScroll().resize();
}

function createDataTable(tableId, scrollY, options) {
    var config = {
        dom: "rtp",
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

function funcionesResultado() {
    createDataTable('Parametros', 0, {
        dom: 'frtip',
        pageLength: 7,
        scrollX: '100%',
        scrollCollapse: true,
        columnDefs: [{
            targets: "hidden",
            visible: false
        }],
        createdRow: function (row) {
            $(row).on("click", (evt) => {
                evt.preventDefault();
                evt.stopImmediatePropagation();
                $('#Parametros tr.selected').not(row).removeClass('selected');
                $(row).toggleClass("selected");
                habilitarAcciones();
            });
        },
        initComplete: function () {
            $(this).dataTable().api().columns.adjust();
        }
    });

    $('#btn_AgregarRow').click(function () {
        var data = $('#Tablas').DataTable().row($("#Tablas tbody tr.selected")).data();
        $('#contenedor-externo-tablas-auxiliares').load(BASE_URL + "Mantenimiento/EditarRow?idComponente=" + data[0]);
    });

    $('#btn_EditarRow').click(function () {
        var data = $('#Tablas').DataTable().row($("#Tablas tbody tr.selected")).data();
        var dataRow = $('#Parametros').DataTable().row($("#Parametros tbody tr.selected")).data();

        $('#contenedor-externo-tablas-auxiliares').load(BASE_URL + "Mantenimiento/EditarRow?idComponente=" + data[0] + "&id=" + dataRow[0]);
    });

    $('#btn_EliminarRow').click(function () {
        var data = $('#Tablas').DataTable().row($("#Tablas tbody tr.selected")).data();

        var dataRow = $('#Parametros').DataTable().row($("#Parametros tbody tr.selected")).data();

        confirmModal("Tablas Paramétricas", "¿Confirma que desea eliminar el registro?", function () {
            $.ajax({
                type: "POST",
                url: BASE_URL + "Mantenimiento/GetEliminaRegistroAsignacion",
                data: {
                    IdComponente: data[0],
                    IdTabla: dataRow[0]
                },
                success: function (response) {
                    if (response.success) {
                        mostrarMensaje(true, "Tablas Paramétricas", "Los cambios se guardaron correctamente");
                        if (typeof registroActualizado == "function") {
                            registroActualizado();
                        }
                    } else {
                        mostrarMensaje(false, "Tablas Paramétricas", "Se produjo un error al guardar el registro");
                    }
                },
                error: function () {
                    mostrarMensaje(false, "Tablas Paramétricas", "Se produjo un error al guardar el registro");
                }
            });
        });
    });

}

function habilitarAcciones() {
    var habilitar = $("#Parametros tbody tr.selected").length > 0;
    if (habilitar) {
        $('#btn_EliminarRow').removeClass('boton-deshabilitado');
        $('#btn_EditarRow').removeClass('boton-deshabilitado');
    } else {
        $('#btn_EliminarRow').addClass('boton-deshabilitado');
        $('#btn_EditarRow').addClass('boton-deshabilitado');
    }
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

function registroActualizado() {
    habilitarResultado();
}