$(document).ready(init);
$(window).resize(ajustarmodalListas);
$('#modal-window-consultas-listas').on('shown.bs.modal', function (e) {
    ajustarScrollBarsListas();

    $('#modal-window-consultas-listas .dataTables_scrollBody').niceScroll(getNiceScrollConfig());
    columnsAdjust("modal-window-consultas-listas .table");
    hideLoading();
});

function init() {

    var container = $('#modal-window-consultas-listas');

    var config = {
        sScrollY: "40vh",
        bScrollCollapse: true,
        dom: "rtp",
        "columnDefs": [{
            "targets": 0,
            "visible": false
        }],
        "order": [[1, "asc"]],
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        }
    };

    $('#SelectListas').change(function () {
        var i = $(this).val();
        $('.lista').hide();
        $('[data-lista="' + i + '"]').show();
    });


    $('#btnAgregarLista').click(function () {
        var data = $('[data-lista="' + $('#SelectListas').val() + '"] table').DataTable().data();
        var featId = [];
        $.each(data, function (i,row) {
            featId.push(row[0]);
        });

        $("#modal-window-consultas-listas").modal("hide");
        if (typeof AgregarPuntos == "function") {
            showLoading();
            AgregarPuntos(featId);
        }

    });

    $('#modal-window-consultas-listas .table').DataTable(config);
    ajustarmodalListas();
    $("#modal-window-consultas-listas").modal("show");

}


function ajustarmodalListas() {
    //var viewportHeight = $(window).height(),
    //    headerFooter = 190,
    //    altura = viewportHeight - headerFooter;

    //$(".consultas-listas-body", "#scroll-content-consultas-listas").css({ "height": altura, "overflow": "hidden" });

    ajustarScrollBars();
}

function ajustarScrollBarsListas() {
    temp = $(".consultas-listas-body").height();
    $('#consultas-listas-datos').css({ "max-height": temp + 'px' });
    $("#consultas-listas-datos").getNiceScroll().resize();
    $("#consultas-listas-datos").getNiceScroll().show();
}
