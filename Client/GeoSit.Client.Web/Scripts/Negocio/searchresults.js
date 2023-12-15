var detalleObjetoInicializado = false;
$(function () {
    $(window).resize(function () {
        if (!$('#search-results').is(":visible")) return;
        validateSearchResultPanelSize();
        ajustarScrollBarDetalle();
    });
    $("#accordion-detalle-objeto-main").niceScroll(getNiceScrollConfig());
    $('#accordion-detalle-objeto-main .panel-heading').click(function () {
        setTimeout(function () {
            ajustarScrollBarDetalle();
        }, 10);
    });
    $(".detalle-objeto-header .minimizar").click(function (evt) {
        evt.stopPropagation();
        evt.preventDefault();
        if ($(this).hasClass('fa-chevron-up')) {
            minimizarDetalle();
        } else {
            maximizarDetalle();
        }
        setTimeout(function () {
            ajustarScrollBarDetalle();
        }, 250);
    });
    $(".detalle-objeto-header .cerrar").click(function (evt) {
        cerrarDetalleObjeto();
        evt.stopPropagation();
    });

    $('#detalle-objeto-main').draggable({ handle: ".detalle-objeto-header" });
});

function validateSearchResultPanelSize() {
    $('#search-results').collapse('show');

    var temp = $(window).height() - $('.header').height() - 50,
        bottomGap = 40, outerHeight = 0;

    $('#accordion  .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion  .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });

    temp = Math.min(outerHeight + bottomGap, temp);

    $('#search-results').css({ "max-height": temp + 'px' });
    $('#accordion').css({ "max-height": temp - bottomGap + 'px' });
    $("#search-results .results").getNiceScroll().resize();
    $("#search-results .results").getNiceScroll().show();
}

function updateResultsArea() {
    validateSearchResultPanelSize();
    $("#search-results .results").niceScroll(getNiceScrollConfig());
    $('#search-results .panel-heading, .reset-search-btn').click(function () {
        setTimeout(function () {
            $("#search-results .results").getNiceScroll().resize();
        }, 10);
    });

    $("#search-results .srow").each(function (_, element) {
        $(element).hover(
            function () {
                $(this).find('.controls').show('fast');
            },
            function () {
                $(this).find('.controls').hide();
            }
        );
        $(this).find('.controls').hide();
    });
    $('.results .tooltips').tooltip({ container: 'body' });
    $(".reset-search-btn").click(function () {
        toggleSearch(true);
    });
}
function inicializarGrillasDetalleResultado() {
    var opts = {
        paging: false,
        destroy: true,
        searching: false,
        bInfo: false,
        order: [],
        aaSorting: [[0, 'asc']],
        language: { url: BASE_URL + "Scripts/dataTables.spanish.txt" }
    };
    $('.grilla-atributos', '#detalle-objeto-main').DataTable(opts);

    $('.grilla-relaciones', '#detalle-objeto-main').DataTable(Object.assign({}, opts, {
        columnDefs: [
            {
                targets: 'calleCodigo',
                orderable: false,
                visible: false,
                searchable: false
            }],
        aoColumnDefs: [{
            bSortable: false,
            aTargets: [2]
        }]
    }));

    $("[data-tipo-grafico].table", '#detalle-objeto-main').DataTable(opts);
}
function pasarGrillaResultados(idO, idC) {
    $.ajax({
        url: BASE_URL + 'Search/ObtenerLayer/' + idC,
        type: 'GET',
        success: function (result) {
            var newFeature = { "id": idO, "doctype": result.Data };
            var grupos = {};
            $(".results div.roundChkBox:not(.grpChkBox) > input").each(function () {
                (grupos[$(this).attr("data-componente")] = grupos[$(this).attr("data-componente")] || []).push($(this).attr("data-name"));
            });
            var busqueda = Object.keys(grupos).reduce(function (acc, key) {
                acc.capas.push(key);
                acc.seleccion.push(grupos[key]);
                return acc;
            }, { capas: [], seleccion: [] });
            angular.element(document.getElementById('search-bar')).scope().searchByFeaturesDocType(busqueda, newFeature);
        },
        error: function (error) {
            alert(error.statusText);
        }
    });
}
function mostrarDetalleObjetoSearch(detalle) {
    if (!detalleObjetoInicializado) {
        detalleObjetoInicializado = true;
        var elemento = $("#accordion");
        $('#detalle-objeto-main').css({
            top: 42,
            left: elemento.width() + 1 + 'px',
            width: elemento.width() * 0.5
        });
    }
    var top = 42,
        bottom = 50,
        maxHeight = $(window).height() - top - bottom;

    var temp = $(window).height() - $('.header').height() - 50,
        bottomGap = 40;

    $('#detalle-objeto-main').css({ "max-height": maxHeight + 'px' });
    $('#accordion-detalle-objeto-main').css({ 'max-height': temp - bottomGap + 1 + 'px' });

    inicializarGrillasDetalleResultado();
    loadDetalleObjeto('#detalle-objeto-main', detalle, extraFieldsRelaciones);
    ajustarScrollBarDetalle();
    hideLoading();
}
function minimizarDetalle() {
    $('.detalle-objeto-header .minimizar').removeClass('fa-chevron-up').addClass('fa-chevron-down');
    $('.datos-detalle').slideUp();
}

function maximizarDetalle() {
    $('.detalle-objeto-header .minimizar').removeClass('fa-chevron-down').addClass('fa-chevron-up');
    $('.datos-detalle').slideDown();
}

function ajustarScrollBarDetalle() {
    $("#detalle-objeto-main .datos-detalle").getNiceScroll().resize();
    $("#detalle-objeto-main .datos-detalle").getNiceScroll().show();
}

function extraFieldsRelaciones(campos, data) {
    var html = "";
    if (data.Grafico && data.Grafico !== 5) {
        var capas = data.Capa.split(',');
        var ids = capas.map(function () { return [[data.FeatId]]; });
        html = "<span class='fa fa-map-marker cursor-pointer' title='Ver' name='ver-objeto-en-mapa' onclick='GeoSIT.MapaController.seleccionarObjetos(" + JSON.stringify(ids) + "," + JSON.stringify(capas) + ")'" +
            "data-id-objeto='" + data.ObjetoId + "' data-valor='" + data.Valor + "' data-id-componente='" + data.ComponenteId + "'" +
            "data-descripcion-componente='" + data.Descripcion + "'></span> ";
    }

    var htmlSpan = '<span id="' + data.ObjetoId + '" class="fa  fa-list-alt cursor-pointer" title="Pasar a grilla de Resultados" name="pasar-a-grilla-de-resultados" ' +
        'data-id-objeto="' + data.ObjetoId + '" data-id-componente="' + data.ComponenteId + '" data-descripcion-componente="' + data.Descripcion +
        '"onclick="pasarGrillaResultados(' + data.ObjetoId + ',' + data.ComponenteId + ')"></span>';
    campos.push(html + htmlSpan);
}

function cerrarDetalleObjeto() {
    $("#accordion-detalle-objeto-main").getNiceScroll().hide();
    $(".alturas-check-content .scroll").getNiceScroll().hide();
    $('.datos-adicionales').hide();
}
function loadDetalleObjeto(parentElement, detalle, extraFieldsRelacionesCB) {
    $(parentElement).show();

    $('.editarAtributos', parentElement).removeClass("fa-pencil");

    $(".tabsGraficos .nav-tabs", parentElement).addClass("hidden");
    $(".tabsGraficos li[data-tipo-grafico]", parentElement).addClass("hidden");
    $(".tabsGraficos li[data-tipo-grafico]", parentElement).removeClass("tiene-objeto");

    $("img[data-tipo-grafico]", parentElement).removeAttr('src');
    $("img[data-tipo-grafico]", parentElement).parent().hide();

    var grillaAtributos = $('.grilla-atributos', parentElement).DataTable(),
        grillaRelaciones = $('.grilla-relaciones', parentElement).DataTable(),
        grillasGraficos = $('[data-tipo-grafico].table', parentElement).DataTable();

    grillaAtributos.clear();
    grillaRelaciones.clear();
    grillasGraficos.clear();

    $.each(detalle.data.Atributos, function (_, value) {
        grillaAtributos.row.add([value.Nombre, value.Valor]);
    });
    $.each(detalle.data.Relaciones, function (_, value) {
        var campos = [value.Nombre, value.Valor];
        if (extraFieldsRelacionesCB) {
            extraFieldsRelacionesCB(campos, value);
        }
        grillaRelaciones.row.add(campos);
    });
    $.each(detalle.data.Graficos, function (_, value) {
        $(".tabsGraficos li[data-tipo-grafico=" + value.TipoGrafico + "]", parentElement).removeClass("hidden").addClass("tiene-objeto");
        //no uso la variable porque necesito agregar a cada grilla dependiendo del tipo
        $("[data-tipo-grafico=" + value.TipoGrafico + "].table", parentElement).DataTable().row.add([value.Nombre, value.Valor]);
    });

    if (detalle.imgData) {
        $.each(detalle.imgData, function (_, value) {
            $(".tabsGraficos img[data-tipo-grafico=" + value.TipoGrafico + "]", parentElement).parent().show();
            $(".tabsGraficos img[data-tipo-grafico=" + value.TipoGrafico + "]", parentElement).attr('src', "data:image/png;base64," + value.Imagen);
        });
    }

    grillaAtributos.draw();
    grillaRelaciones.draw();
    grillasGraficos.draw();

    $(".tabsGraficos li[data-tipo-grafico]", parentElement).removeClass("active");
    $(".tabsGraficos .tab-content .tab-pane", parentElement).removeClass("active");
    var li = $(".tabsGraficos li[data-tipo-grafico].tiene-objeto:first", parentElement).addClass("active");
    $($("a", li).data("target")).addClass("active");
    if ($(".tabsGraficos li[data-tipo-grafico].tiene-objeto", parentElement).length > 1) {
        $(".tabsGraficos .nav-tabs", parentElement).removeClass("hidden");
    }
}