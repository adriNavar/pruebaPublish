var ParcelaValuacion = function () {
    var __parentContainerElem, __moduleContainerElem, __controller;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = document.querySelector(".panel-parcela-valuacion");

        var postLoadedTasks = function () {
            loadValuacion();
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        };
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        __parentContainerElem.addEventListener("reload-valuacion", loadValuacion);
        __parentContainerElem.addEventListener("reload-ddjj", loadValuacion);

        $("#btnImprimirInformeValuatiorio", __moduleContainerElem).on("click", imprimirInformeValuacion);
    }
    function loadValuacion() {
        $.ajax({
            url: BASE_URL + "MantenimientoParcelario/GetValuacionParcela",
            type: "GET",
            dataType: "json",
            success: function (valuacion) {
                $("#valor-tierra", __moduleContainerElem).html(__controller.formatoValorMoneda(valuacion.ValorTierra));
                $("#vigencia", __moduleContainerElem).html(FormatFechaHora(parseJsonDate(valuacion.VigenciaDesde), false));
                $("#decretos", __moduleContainerElem).html(valuacion.Decretos);
            },
            error: function (_, textStatus, errorThrown) {
                console.log(textStatus, errorThrown);
            }
        });
    }

    function imprimirInformeValuacion() {
        showLoading();
        $.ajax({
            url: BASE_URL + "Valuacion/GenerarInformeValuacionParcelario",
            type: 'POST',
            success: function () {
                window.open(BASE_URL + "Valuacion/AbrirReporte");
            },
            error: function (err, textStatus, errorThrown) {
                if (err.status === 501) {
                    __controller.mostrarConfirmacion("Mantenedor Parcelario - Informe de Valuación", "El Informe de valuación para este tipo de parcelas aún no ha sido desarrollado.");
                    return;
                } else if (err.status === 409) {
                    __controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al generar el Informe de Valuación. La partida no posee DDJJ, carguela.");
                    return;
                }
                console.log(err);
                console.log(textStatus, errorThrown);
            },
            complete: hideLoading
        });
    }
    return {
        init: init
    };
};