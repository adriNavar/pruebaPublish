$(document).ready(init);
$(window).resize(ajustarmodal);

$('#modal-GeneracionRemito').one('shown.bs.modal', function () {
    setTimeout(ajustarScrollBars, 100);
    hideLoading();
});

function init() {
    $("#generacionRemito-content").niceScroll(getNiceScrollConfig());

    $(".date > input", "#generacionRemito-content").datepicker(getDatePickerConfig());

    $("input.desde", "#generacionRemito-content").datepicker().on("changeDate", function () {
        $("input.hasta", "#generacionRemito-content").datepicker("setStartDate", $(this).datepicker("getDate"));
    });

    $("input.hasta", "#generacionRemito-content").datepicker().on("changeDate", function () {
        $("input.desde", "#generacionRemito-content").datepicker("setEndDate", $(this).datepicker("getDate"));
    });

    createDataTable();

    $("#btnGenerarRemito").prop("disabled", true);

    ajustarmodal();
    $('#modal-GeneracionRemito').modal("show");
}

$("#btnConsultar").click(function () {
    var fechaDesde = $('#txtFechaDesde').val();
    var fechaHasta = $('#txtFechaHasta').val();
    var sectorDestino = $("#cboSectorDestino").val() || '';
    if (!fechaDesde) {
        mostrarMensaje("Consultar", "Debe ingresar la fecha desde", 2);
        return false;
    }
    if (!fechaHasta) {
        mostrarMensaje("Consultar", "Debe ingresar la fecha hasta", 2);
        return false;
    }
    if (!sectorDestino) {
        mostrarMensaje("Consultar", "Debe ingresar el destino", 2);
        return false;
    }
    $("#btnGenerarRemito").prop("disabled", true);
    createDataTable({
        ajax: {
            url: BASE_URL + 'mesaentradas/getmovimientosremito',
            method: 'POST',
            data: { fechaDesde: fechaDesde, fechaHasta: fechaHasta, sectorDestino: sectorDestino }
        }
    });

});
function rowClick(evt) {
    if (!evt) return;
    $(evt.currentTarget).toggleClass('selected');
    var botonHabilitado = $('tr.selected', $(evt.currentTarget).parents("table")).length !== 0;
    $("#btnGenerarRemito").prop("disabled", !botonHabilitado);
}

$("#btnGenerarRemito").click(function () {
    var aMovimientos = $('#Grilla_movimientos').DataTable().rows('.selected').data().map(function (reg) { return reg.IdMovimiento; }).toArray();
    if (aMovimientos.length) {
        var sectorDestino = $('#cboSectorDestino option:selected').val() || '';
        showLoading();
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'mesaentradas/generarremito',
            data: { sectorDestino: sectorDestino, movimientos: aMovimientos },
            success: function () {
                window.open(BASE_URL + 'MesaEntradas/AbrirInformeRemito', "_blank");
                $('#Grilla_movimientos').DataTable().ajax.reload();
            },
            error: function (err) {
                if (err.status === 404) {
                    mostrarMensaje("Generación de Remito", "No se pudo obtener el informe del remito", 3);
                } else {
                    mostrarMensaje("Generación de Remito", "No se pudo guardar el remito", 3);
                }
            },
            complete: hideLoading
        });
    }

});

function mostrarMensaje(title, description, tipo) {
    var modal = $('#mensajeModalGeneracionRemito');
    var alertaBackground = $('div[role="alert"]', modal);
    $('.modal-title', modal).html(title);
    $('.modal-body p', modal).html(description);
    var cls = 'alert-success';
    if (tipo === 2) {
        cls = 'alert-warning';
    } else if (tipo === 3) {
        cls = 'alert-danger';
    }
    alertaBackground.removeClass('alert-warning alert-success alert-danger').addClass(cls);
    modal.modal('show');
}
function ajustarmodal() {
    var altura = $(window).height() - 150; //value corresponding to the modal heading + footer
    $(".generacionRemito-body").css({ "height": altura, "overflow-y": "auto" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $('#generacionRemito-content').css({ "max-height": $('#generacionRemito-content').parent().height() + 'px', "height": "100%" });
    $('#generacionRemito-content').getNiceScroll().resize();
    $('#generacionRemito-content').getNiceScroll().show();
}
function createDataTable(opts) {
    var defaults = {
        language: {
            url: BASE_URL + 'Scripts/dataTables.spanish.txt'
        },
        dom: "t<'row'<'col-sm-6'i><'col-sm-6'p>>",
        orderCellsTop: true,
        destroy: true,
        processing: true,
        paging: true,
        pageLength: 10,
        columns: [
            //{ name: "selector", orderable: false, className: 'select-checkbox' },
            { name: "IdMovimiento", data: "IdMovimiento", visible: false },
            { name: "Fecha", data: "Fecha", type: 'date', width: "95px" },
            { name: "Numero", data: "Numero", width: "110px" },
            { name: "Iniciador", data: "Iniciador" },
            { name: "Estado", data: "Estado", width: "150px" }
        ],
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        initComplete: function () {
            $(this).DataTable().columns.adjust().draw();
        },
        createdRow: function (row) {
            $(row).on('click', rowClick);
        }
    };
    $('#Grilla_movimientos').DataTable(Object.assign(defaults, opts || {}));
}
//# sourceURL=generarRemito.js