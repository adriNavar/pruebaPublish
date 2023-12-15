var Zonificacion = function () {
    var __controller, __parentContainerElem, __moduleContainerElem;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = document.querySelector(".panel-atributos-zonificacion");

        var tabla = $("#atributosZonificacion", __moduleContainerElem).dataTable({
            "scrollY": "148px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "aaSorting": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "ajax": {
                "url": BASE_URL + "MantenimientoParcelario/GetAtributosZonificacion",
                "type": "POST"
            },
            "columns": [
                { "data": "Descripcion" },
                { "data": "Valor" },
                { "data": "UnidadMedida" }
            ]
        });
        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        }
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        __parentContainerElem.addEventListener("tab-changed", function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    return {
        init: init
    };
};