var GeoMapaConsultas = {
    MapVersion: 0,
    Basico: 0,
    Mapa: null,
    Config: null,
    isLoaded: function () {
        return this.Mapa != null && this.Mapa.isLoaded();
    },
    getCurrentSelection: function () {
        return this.Mapa.getCurrentSelection();
    },
    showLayer: function (layerName, visible) {
        this.Mapa.showLayer(layerName, visible);
    },
    //seleccionarElementos: function (featIds) {
    //    if (featIds != null && featIds.length > 0) {
    //        this.Mapa.verEnMapa(featIds, ["PUNTOS_VISTA"], false);
    //    }
    //}
};

$(document).ready(init);
$('#modal-window-consultas-mapa').on('hidden.bs.modal', function () {
    GeoMapaConsultas.Mapa = null;

});
var listaSeleccionados = [];
function init() {
    
    $('#btnAgregarMapa').click(function() {
        var data = GeoMapaConsultas.getCurrentSelection();
        var featId = [];
        $.each(data, function (i,row) {
            featId.push(row.id);
        });
        $("#modal-window-consultas-listas").modal("hide");
        if (typeof AgregarPuntos == "function") {
            $("#modal-window-consultas-mapa").modal("hide");
            showLoading();
            AgregarPuntos(featId);
        }

    });


    ajustarmodalMapa();
    showMapa();

    $("#modal-window-consultas-mapa").modal("show");

}

function ajustarmodalMapa() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter;
    $(".consultas-mapa-body", "#scroll-content-consultas-mapa").css({ "height": altura, "overflow": "hidden" });
}

function showMapa() {

    GeoMapaConsultas.Mapa = new MapaMG("Mapa", "frameGeoMapaConsultas", GeoMapaConsultas);
    GeoMapaConsultas.Mapa.load();
    interval = setInterval(function () {
        if (GeoMapaConsultas.isLoaded()) {
            clearInterval(interval);
            hideLoading();
            //var listaSeleccionados = $.parseJSON(puntosSeleccionados);
            //GeoMapaConsultas.seleccionarElementos(listaSeleccionados);
        }
    }.bind(this), 500);
}
