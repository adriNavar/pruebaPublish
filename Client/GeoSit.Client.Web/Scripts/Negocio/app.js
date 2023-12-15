var appJS = angular.module('app', ['ui.bootstrap']);
var timeoutResultado;

var BuscadorTipos = {
    Actas: 'actas',
    Chracras: 'chacras',
    Fracciones: 'fracciones',
    Quintas: 'quintas',
    Parajes: 'parajes',
    Parcelas: 'parcelas',
    Personas: 'personas',
    Secciones: 'secciones',
    UnidadesTributarias: 'unidadestributarias',
    Mensuras: 'mensuras',
    ParcelaSanear: 'parcelassaneables',
    Prescripciones: 'prescripciones',
    Historicas: 'unidadestributariasbaja',
   
};
$.fn.visibleHeight = function () {
    var elBottom, elTop, scrollBot, scrollTop, visibleBottom, visibleTop;
    scrollTop = $(window).scrollTop();
    scrollBot = scrollTop + $(window).height();
    elTop = this.offset().top;
    elBottom = elTop + this.outerHeight();
    visibleTop = elTop < scrollTop ? scrollTop : elTop;
    visibleBottom = elBottom > scrollBot ? scrollBot : elBottom;
    return visibleBottom - visibleTop;
};

var GeoSIT = {
    MapaController: null,
    SolrController: null
};
function MapaController(idMapa, div, zoom, rotacion, selector, streetView, seleccion, toolbarExterno, impresion, medicion,
    dibujoPunto, dibujoLinea, dibujoPoligono) {
    var mapa = new Mapa(div, new OpcionesMapa(zoom, rotacion, selector, streetView, seleccion, toolbarExterno, impresion, medicion, dibujoPunto, dibujoLinea, dibujoPoligono));
    $.ajax({
        type: "GET",
        dataType: "json",
        url: `${BASE_URL}Mapa/GetConfiguracionByIdMapa/${idMapa}`,
        success: function (config) {
            mapa.iniciar(config.capas, config.centro, config.zoom);
        },
        error: function () {
            alert("error");
        }
    });
    return {
        centrar: mapa.centrar,
        obtenerDibujos: mapa.obtenerDibujos,
        obtenerSeleccion: mapa.obtenerSeleccion,
        editarObjeto: mapa.editarObjeto,
        seleccionarObjetos: mapa.seleccionarObjetos,
        dibujarRuta: mapa.dibujarRuta,
        dibujarSeleccion: mapa.dibujarSeleccion,
        activarDibujoLinea: mapa.activarDibujoLinea,
        activarDibujoPoligono: mapa.activarDibujoPoligono,
        activarDibujoPunto: mapa.activarDibujoPunto,
        activarSeleccion: mapa.activarSeleccion,
        limpiar: mapa.limpiar,
        refrescar: mapa.refrescar,
        filtrarCapa: mapa.filtrarCapa,
        agregarCapaTemporal: mapa.agregarCapaTemporal,
        agregarItemMenu: mapa.agregarItemMenu,
        zoomExtent: mapa.zoomExtent,
        obtenerExtent: mapa.obtenerExtent,
        obtenerCapasVisibles: mapa.obtenerCapasVisibles,
        on: mapa.on
    };
}
function SolrController() {
    var scope = angular.element(document.getElementById('search-bar')).scope();
    scope.$on('limpiarGrilla', function () { GeoSIT.MapaController.seleccionarObjetos(); });
    return {
        searchByFeatures: function (features, filterType) {
            if (features.capas.length) {
                scope.searchByFeatures(features, filterType);
            } else {
                toggleSearch(true);
            }
        }
    };
}

function closeLeftSideMenu() {
    $('.popover-markup>.trigger').popover('hide').removeClass('active');
}

function closeRightSideMenu() {
    $("#right-side-controls").animate({ "right": "10" }, 250);
    $("#layers-panel").animate({ "right": "-250" }, 250, function () {
        $(this).hide();
    });
    rightSideMenuOpened = false;
    $('.btnLayers').removeClass('active');
}

function openRightSideMenu() {
    $("#right-side-controls").animate({ "right": "260" }, 250);
    $("#layers-panel").show().animate({ "right": "-0" }, 250, function () { });
    $('.btnLayers').addClass('active');
    closeLeftSideMenu();
    rightSideMenuOpened = true;
}

function loadView(url, data) {
    closeLeftSideMenu();
    showLoading();

    if (url.indexOf("?") === -1) {
        url = url + "?" + new Date().getTime();
    }
    else {
        url = url + "&unique=" + new Date().getTime();
    }
    if (data) {
        $("#contenido").load(url, data);
    } else {
        $("#contenido").load(url);
    }
}

function hideSearchBlur() {
    toggleSearch(true);
}

function toggleSearch(close, open) {
    clearTimeout(timeoutResultado);
    timeoutResultado = setTimeout(function (close, open) {
        cerrarDetalleObjeto();
        var iElem = $('#collapse-search-btn > span');
        var searchResults = $("#search-results");
        if ((close || !open) && iElem.hasClass('open')) {
            searchResults.collapse('hide');
            iElem.removeClass('open');
            $("#search-results .results").getNiceScroll().hide();
        } else if ((open || !close) && !iElem.hasClass('open')) {
            searchResults.collapse('show');
            iElem.addClass('open');
            $("#search-results .results").getNiceScroll().show();
        }
    }, 150, close, open);
}

function showLoading() {
    if ($(".loading-status-ui-backdrop").css('display') !== "block") {
        $(".loading-status-ui-backdrop").css("display", "block");
    }

    if ($(".loading-status-ui").css('display') !== "block") {
        $(".loading-status-ui").css("display", "block");
    }
}

function hideLoading() {
    if ($(".loading-status-ui-backdrop").css('display') !== "none") {
        $(".loading-status-ui-backdrop").css("display", "none");
    }

    if ($(".loading-status-ui").css('display') !== "none") {
        $(".loading-status-ui").css("display", "none");
    }
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function descargarGeometria() {
    const capas = ["vw_mejoras", "PARCELAS_HUERFANAS", "VW_PARCELAS_GRAF_ALFA", "VW_PARCELAS_GRAF_ALFA_RURALES", "VW_MANZANAS"];
    let idxCapa = 0;
    let encontro = false;
    do {
        const idx = GeoSIT.MapaController.obtenerSeleccion().capas.indexOf(capas[idxCapa]);
        if (idx > -1) {
            encontro = true;
            const id = GeoSIT.MapaController.obtenerSeleccion().seleccion[idx][0],
                capa = capas[idxCapa];
            showLoading();
            window.open(`${BASE_URL}Mapa/DescargarGeometria?idObjeto=${parseInt(id)}&capa=${capa}`, "_blank");
            hideLoading();
        }
        idxCapa++;
    } while (!encontro && idxCapa < capas.length);
}

function generarReporteParcelario() {
    showLoading();
    var loc = BASE_URL + "Reportes/DownloadFile";
    $.ajax({
        url: BASE_URL + "Reportes/ReporteParcelario",
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        success: function () {
            window.location = loc;
        },
        error: function (error) {
            mostrarMensaje("Generar Reporte Parcelario", error.statusText, 3);
        },
        complete: hideLoading
    });
    return false;
}

function descargarGeometrias(selection) {
    if (selection.capas.indexOf('vw_mejoras') > -1 || selection.capas.indexOf('PARCELAS_HUERFANAS') > -1 || selection.capas.indexOf('VW_PARCELAS_GRAF_ALFA') > -1 || selection.capas.indexOf('VW_PARCELAS_GRAF_ALFA_RURALES') > -1 || selection.capas.indexOf('VW_MANZANAS') > -1) {
        showLoading();

        for (var iCapa = 0; iCapa < selection.capas.length; iCapa++) {
            var capa = selection.capas[iCapa];
            if (capa == 'vw_mejoras' || capa == 'PARCELAS_HUERFANAS' || capa == 'VW_PARCELAS_GRAF_ALFA' || capa == 'VW_PARCELAS_GRAF_ALFA_RURALES' || capa == 'VW_MANZANAS') {
                var featuresCapa = selection.seleccion[iCapa];
                //var features = JSON.stringify(featuresCapa);
                var features = featuresCapa.join(",");
                window.open(BASE_URL + 'Mapa/DescargarGeometrias?capa=' + capa + '&features=' + features, "_blank");
            }
        }

        hideLoading();
    }
}

$(function () {
    GeoSIT.SolrController = new SolrController();
    GeoSIT.MapaController = new MapaController(1, "map", true, true, true, true, true, false, true, true, true, true, true);
    GeoSIT.MapaController.on('iniciarstreetview', function (data) { $(data.contenedor).draggable({ containment: "#map", handle: data.target }); });
    GeoSIT.MapaController.on('buscarseleccion', GeoSIT.SolrController.searchByFeatures);
    GeoSIT.MapaController.on('descargarGeometrias', descargarGeometrias);

    /////////////////////////////////////////////////////////////
    //////////////////  Tool tips ///////////////////////////////
    /////////////////////////////////////////////////////////////

    $('.tooltips').tooltip({ trigger: 'hover' });

    //////////////////////////////////////////////////////////////
    ////////////////////   Side menus ////////////////////////////
    //////////////////////////////////////////////////////////////

    tmp = "";
    rightSideMenuOpened = false;


    $('#map-container').click(function () {
        closeLeftSideMenu();
        closeRightSideMenu();
    });

    $('.sidebar-toggle-box').click(function () {
        $(".main-menu-container").toggle();
    });

});
