$(window).resize(ajustarmodal);
$(document).ready(function init() {
    $('#modal-window-informe-inspecciones').one('shown.bs.modal', function (e) {
        ajustarmodal();
        hideLoading();
    });
    $(".inspectores-content").niceScroll(getNiceScrollConfig());
    $('.soloFecha').datepicker(getDatePickerConfig({ minView: 2, startView: 4 }));
    var defaultOptions = {
        createdRow: function (row) {
            $(row).on('click', toggleRowSelection);
        }
    };
    $("#tablaZona").DataTable(Object.assign(defaultOptions, getDataTableOptions()));
    ajustarmodal();
    $("#modal-window-informe-inspecciones").modal('show');
});
function toggleRowSelection() {
    $(this).toggleClass("selected");
}
function ajustarScrollBars() {
    $(".inspectores-content").getNiceScroll().resize();
    $(".inspectores-content").getNiceScroll().show();
}
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlerta = fn;
    $("#botones-modal-info").find("span:last").hide();
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $("#botones-modal-info").find("span:last").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoInspector").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoInspector").html(titulo);
    $("#DescripcionInfoInspector").html(mensaje);
    $("#ModalInfoInspector").modal('show');
    $("#botones-modal-info").find("span:last").show();
}

//@ sourceURL=informeInspecciones.js