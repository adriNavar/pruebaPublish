var UTInforme = function () {
    var __parentContainerElem, __currentUT;
    function init(container, controller) {
        __parentContainerElem = document.querySelector(container);
        $("#ut-reporte", __parentContainerElem).oneClick(function () {
            if (__currentUT) {
                showLoading();
                $.ajax({
                    url: BASE_URL + "MantenimientoParcelario/GetInformeUT",
                    data: { idUnidadTributaria: __currentUT.UnidadTributariaId },
                    type: 'POST',
                    success: function () {
                        window.open(BASE_URL + "MantenimientoParcelario/Abrir");
                    },
                    error: function () {
                        controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al generar el Informe Tributario.");
                    },
                    complete: hideLoading
                });
            }
        });

        $("#exportarHistoricoCambiosUT").click(function () {
            showLoading();
            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/GetInformeHistoricoCambiosUnidadTributaria",
                data: { idUnidadTributaria: __currentUT.UnidadTributariaId },
                type: 'POST',
                success: function () {
                    window.open(BASE_URL + "MantenimientoParcelario/Abrir");
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                },
                complete: hideLoading
            });
        });
        __parentContainerElem.addEventListener("ut-changed", function (evt) {
            __currentUT = evt.detail;
            if (__currentUT) {
                $("#ut-reporte", __parentContainerElem).removeClass("disabled");
            } else {
                $("#ut-reporte", __parentContainerElem).addClass("disabled");
            }
        });
    }

    return {
        init: init
    };
};