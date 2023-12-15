$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-zonas-atributos').one('shown.bs.modal', hideLoading);

function init() {
    const alerta = (titulo, mensaje, tipo) => {
        $("#MensajeInfoActa").removeClass("alert-info alert-warning alert-danger alert-success").addClass(tipo === 2 && "alert-warning" || "alert-danger");
        $("#TituloInfoActa").html(titulo);
        $("#DescripcionInfoActa").html(mensaje);
        $("#ModalInfoActa").modal('show');
    }

    $('#tablaZona').DataTable({
        "dom": "t",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[1, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "orderable": true
        }],
        "initComplete": () => {
            $("tr", this).on("click", (evt) => {
                $(evt.currentTarget).toggleClass("selected");
            });
        }
    });

    $(".atributozona-content").niceScroll(getNiceScrollConfig());

    $('#modal-window-zonas-atributos').modal('show');

    $("#aceptar-informe-periodo").on("click", function () {
        showLoading();
        const formData = {
            "Observaciones": $("#observaciones").prop('checked') && "1" || "0",
            "Zonas": $("tr.selected", "#tablaZona").map((_, r) => ({ Id: $("td:first", r).text(), Nombre: $("td:last", r).text() })).toArray()
        };

        $.ajax({
            url: BASE_URL + 'ZonaAtributo/GenerateInformeZona',
            type: 'POST',
            data: formData,
            success: function (data) {
                if (data.success) {
                    window.open(`${BASE_URL}ZonaAtributo/GetFileInformeZonas?file=${data.file}`);
                } else {
                    alerta('Advertencia', data.message, 3);
                }
            },
            error: function (_, __, errorThrown) {
                alerta('Error', errorThrown, 3);
            }
        }).always(hideLoading);
    });
}

function ajustarmodal() {
    var altura = $(window).height() - 360;
    $(".atributozona-body").css({ "height": altura });
    $('.atributozona-content').css({ "max-height": altura, "overflow": "hidden" });
    ajustarScrollBars();

}

function ajustarScrollBars() {
    $(".inspectores-content").getNiceScroll().resize();
    $(".inspectores-content").getNiceScroll().show();
}