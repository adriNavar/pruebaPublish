$(document).ready(init);
$('#modal-window-informe-tramites').on('shown.bs.modal', function (e) {
    //ajustarScrollBars();
    hideLoading();
});

function init() {
    //ajustarmodal();
    $('#soloFecha').datepicker(getDatePickerConfig({ minView: 2, startView: 4 }));


    //$('#modal-window-informe-tramites').modal('show');

    $('#numero').keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    function exportarExcel(idMunicipio) {
        $.ajax({
            url: "/Reportes/ExportarAExcel",
            type: "GET",
            data: "idMunicipio=" + JSON.stringify(idMunicipio),
            success: function () {
                alert("Ha sido ejecutada la acción.");
            },
            error: function (xhr) {
                console.log(xhr);
            }
        });
    }
    function ajustarmodal() {
        var viewportHeight = $(window).height(),
            paddingBottom = 70,
            headerFooter = 220,
            altura = viewportHeight - headerFooter,
            altura = altura - (altura > headerFooter + paddingBottom ? paddingBottom : 0); //value corresponding to the modal heading + footer
        $(".modal-body", "#modal-window-informe-tramites").css({ "height": altura, "overflow": "hidden" });

    }

