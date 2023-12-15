$(document).ready(init);
$(window).resize(ajustarmodal);

function init() {
    $(".indicaciones-content").niceScroll(getNiceScrollConfig());

    $('.indicaciones-content .panel-heading').click(function () {
        setTimeout(function () {
            ajustarScrollBars();
        }, 10);
    });

    $('#modal-window-indicaciones').modal('show');

    $('.modal-footer span[title="Exportar"]').click(function (event) {
        IS_DOWNLOAD = true;
        window.location = BASE_URL + 'Ruteo/ExportarRuteo';
    });

    $('.modal-footer span[title="Atras"]').click(function (event) {
        $('#modal-window-indicaciones').modal('hide');
        $('#modal-window-ruteo').modal('show');
    });

    ajustarmodal();
    dibujarRuta();
    hideLoading();
    $('#modal-window-indicaciones').draggable();

    setTimeout(function () {
        ajustarScrollBars();
    }, 10);
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

    $("#MensajeInfoRuteo").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoRuteo").html(titulo);
    $("#DescripcionInfoRuteo").html(mensaje);
    $("#ModalInfoRuteo").modal('show');
}

function getMaxHeight() {
    return $(window).height() - 230;
}

function ajustarmodal() {
    var altura = getMaxHeight();
    $('.indicaciones-body').css({ "max-height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $('.indicaciones-content').css({ "max-height": getMaxHeight() + 'px' })
    $(".indicaciones-content").getNiceScroll().resize();
    $(".indicaciones-content").getNiceScroll().show();
}

function checkObjetosTitle() {
    setTimeout(function () {
        $('.titulo-objeto').each(function (i, e) {
            var span = $(e);
            console.log(span.width());
            if (span.width() > 500) {
                var trimText = '';
                while (span.width() > 480) {
                    trimText = span.text().substring(0, span.text().length - 1);
                    span.text(trimText)
                }
                span.text(trimText + ' (...)');
            }
        });
    }, 500);
}

function dibujarRuta() {
    var path = google.decodePolyline($('#hdfPolyline').val());
    var points = new Array();
    $('div[name*="marker-object-"]').each(function (i, e) {
        points.push({
            name: $('input[name="name"]', this).val(),
            description: $('input[name="address"]', this).val(),
            lat: parseFloat($('input[name="lat"]', this).val().replace(",", ".")),
            lon: parseFloat($('input[name="lon"]', this).val().replace(",", "."))
        });
    });
    GeoSIT.MapaController.dibujarRuta(points, path);
}

//# sourceURL=indicacionesruteo.js