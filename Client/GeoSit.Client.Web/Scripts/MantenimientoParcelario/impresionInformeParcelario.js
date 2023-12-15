var InformeParcelario = function () {
    function init(container, controller) {
        var __parentContainerElem = document.querySelector(container);
        $("#print-all", __parentContainerElem).oneClick(function () {
            showLoading();
            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/GetInformeParcelario",
                type: "POST",
                success: function () {
                    setTimeout(function () {
                        window.open(BASE_URL + "MantenimientoParcelario/Abrir", "_blank");
                    },10);
                },
                error: function () {
                    controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al generar el Informe Parcelario.");
                },
                complete: hideLoading
            });
        });

        $("#exportarHistoricoCambiosParcela").click(function () {
            showLoading();
            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/GetInformeHistoricoCambiosParcela",
                type: 'POST',
                success: function () {
                    window.open(BASE_URL + "MantenimientoParcelario/Abrir", "_blank");
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                },
                complete: hideLoading
            });
        });
    }

    return {
        init: init
    }; 
};