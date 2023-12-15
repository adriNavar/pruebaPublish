$(document).ready(init);

var callbackGenerarMapaTematico;

function init() {
    ajustarmodalConsultasMT();
    
    mapasTematicosData = $.parseJSON(mapasTematicosData);
    $('#MapasTematicos').on('click','i',function (e) {
        e.stopPropagation();
        e.preventDefault();

        if ($(this).hasClass('cursor-pointer')) {

            var tr = $(this).parents('tr');

            var index = $('#MapasTematicos').DataTable().row(tr).index();

            var data = mapasTematicosData[index];
            var operacion = tr.find('select').val();

            $('#container-configuracion-mapa').load(BASE_URL + "MapasTematicos/ConfigurarMapa", { Medicion: data, Operacion: operacion });

            callbackGenerarMapaTematico = function () {
                tr.find('i').removeClass('cursor-pointer').removeClass('black').removeClass('fa-arrow-right').addClass('green').addClass('fa-check');
                tr.find('select').remove();
            }
        }

    });

    createDataTable('MapasTematicos', null, {
        dom: "rt",
        "columnDefs": [ {
            "targets": [0,1,2],
            "orderable": false
        } ]
    });

    $("#modal-window-consultas-mt").modal("show");

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

function marcarMapaComoGenerado(id) {
    $('[data-mt="'+id+'"]').addClass('disabled').addClass('btn-success');
}

$('#modal-window-consultas-mt').on('shown.bs.modal', function (e) {
    hideLoading();
});

function ajustarmodalConsultasMT() {
    //var viewportHeight = $(window).height(),
    //    headerFooter = 200,
    //    altura = viewportHeight - headerFooter;
    //$(".consultas-mt-body", "#scroll-content-consultas-mt").css({ "height": altura, "overflow": "hidden" });
}