var UTValuacion = function (modules) {
    var __moduleContainerElem = document.querySelector(".panel-ut-valuacion"), __controller, __currentUT;
    function init(container, controller) {
        __controller = controller;
        var parentContainerElem = document.querySelector(container);

        modules.forEach(function (module) {
            var ctrl = Object.assign({}, __controller, { isUTValida: existeUT, obtenerUT: obtenerUT });
            module.init(__moduleContainerElem, ctrl, parentContainerElem);
        });

        parentContainerElem.addEventListener("ut-changed", function (evt) {
            __currentUT = evt.detail;
            loadValuacion();
        });
        parentContainerElem.addEventListener("reload-valuacion", loadValuacion);
        parentContainerElem.addEventListener("reload-ddjj", loadValuacion);
    }
    function loadValuacion() {
        $("#ut-valor-tierra", __moduleContainerElem).html("");
        $("#ut-valor-mejoras", __moduleContainerElem).html("");
        $("#ut-valor-total", __moduleContainerElem).html("");
        $("#ut-vigencia", __moduleContainerElem).html("");
        __moduleContainerElem.dispatchEvent(new CustomEvent("ut-changed", { detail: null }));
        if (existeUT()) {
            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/GetValuacionUnidadTributaria",
                data: { id: __currentUT.UnidadTributariaId },
                type: "GET",
                dataType: "json",
                success: function (valuacion) {
                    $("#ut-valor-tierra", __moduleContainerElem).html(__controller.formatoValorMoneda(valuacion.ValorTierra));
                    $("#ut-valor-mejoras", __moduleContainerElem).html(__controller.formatoValorMoneda(valuacion.ValorMejoras));
                    $("#ut-valor-total", __moduleContainerElem).html(__controller.formatoValorMoneda(valuacion.ValorTotal));
                    $("#ut-vigencia", __moduleContainerElem).html(FormatFechaHora(parseJsonDate(valuacion.VigenciaDesde), false));
                    __moduleContainerElem.dispatchEvent(new CustomEvent("ut-changed", { detail: __currentUT }));
                    if (__currentUT.TipoUnidadTributariaID == 2) {
                        $("#btnImprimirCertificadoValuatorio").addClass("disabled");
                    } else {
                        $("#btnImprimirCertificadoValuatorio").removeClass("disabled");
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });
        }
    }

    function existeUT() {
        return __currentUT && __currentUT.UnidadTributariaId > 0;
    }
    function obtenerUT() {
        return __currentUT;
    }

    return {
        init: init
    };
};