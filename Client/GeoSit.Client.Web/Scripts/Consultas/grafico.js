$(document).ready(init);
function init() {
    ajustarmodalGrafico();
    $("#modal-window-consultas-grafico").modal("show");

}
$('#modal-window-consultas-grafico').on('shown.bs.modal', function (e) {
    hideLoading();
    drawChart();
});

function getDate(jsonDate) {
    var re = /-?\d+/; 
    var m = re.exec(jsonDate); 
    var d = new Date(parseInt(m[0]));
    return d;
}

function drawChart() {

    consultaData = $.parseJSON(consultaData);
    $.each(consultaData, function (i, parametro) {
        // Create the data table.
        var data = new google.visualization.DataTable();
        data.addColumn('datetime', 'Fecha');
        var dataByDate = [];
        var container = $('<div>').attr('data-container-graph', i + 1).css('height', '100%');
        $('.consultas-grafico-body').append(container);
        $.each(parametro.Puntos, function (ipunto, row) {
            data.addColumn('number', row.Punto.Nombre);
            $.each(row.Mediciones, function (i, rowMedicion) {
                var date = getDate(rowMedicion.Fecha);
                var rowData = null
                $.each(dataByDate, function (i, idata) {
                    if (idata["fecha"] == date.getTime()) {
                        rowData = idata;
                    }
                });
                if (rowData == null) {
                    rowData = {
                        "fecha": date.getTime()
                    };
                    dataByDate.push(rowData)
                }
                rowData[ipunto + 1] = rowMedicion.Valor;
            });
        });

        $.each(dataByDate, function (i, row) {
            data.addRow();
            data.setCell(i, 0, new Date(row.fecha));
            var cont = 0;
            $.each(row, function (col, val) {
                if (col != "fecha") {
                    data.setCell(i, parseInt(col), val);
                }
                cont++;
            });
        });
        // Set chart options
        var options = {
            title: parametro.Parametro.Nombre,
            hAxis: {
                gridlines: {
                    count: -1
                }
            },
        };
        

        var chart = new google.visualization.LineChart(container[0]);
        chart.draw(data, options);

        $('[data-container-graph]').hide();
        showGraph(1);
    });

    $('[data-pagination]').click(function (e) {
        var val = $(this).data('pagination');
        e.preventDefault();
        e.stopPropagation();
        if (val == "prev") {
            var current = $('.paginationGrafico .active a').data('pagination');
            if (current > 1) {
                $('.paginationGrafico .active').removeClass('active');
                $('.paginationGrafico [data-pagination="' + (current - 1) + '"]').parent().addClass('active');
                showGraph(current - 1);
            }
        } else if (val == "next") {
            var current = $('.paginationGrafico .active a').data('pagination');
            if (current < $('.paginationGrafico li').length - 2) {
                $('.paginationGrafico .active').removeClass('active');
                $('.paginationGrafico [data-pagination="' + (current + 1) + '"]').parent().addClass('active');
                showGraph(current + 1);
            }
        } else {
            $('.paginationGrafico .active').removeClass('active');
            $(this).parent().addClass('active');
            showGraph(val);
        }
    });

}
function showGraph(val) {
    $('[data-container-graph]').hide();
    $('[data-container-graph="' + val + '"]').show();
}

function ajustarmodalGrafico() {
    var viewportHeight = $(window).height(),
        headerFooter = 200,
        altura = viewportHeight - headerFooter;
    $(".consultas-grafico-body", "#scroll-content-consultas-grafico").css({ "height": altura, "overflow": "hidden" });
}