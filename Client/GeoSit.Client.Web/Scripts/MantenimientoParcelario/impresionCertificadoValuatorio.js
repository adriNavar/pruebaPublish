var CertificadoValuatorio = function () {
    function init(moduleContainerElem, controller) {
        $("#btnImprimirCertificadoValuatorio", moduleContainerElem).oneClick(function () {
            if (controller.isUTValida()) {
                showLoading();
                $.ajax({
                    url: BASE_URL + "Valuacion/GenerarReporteValuatorio",
                    data: { idUnidadTributaria: controller.obtenerUT().UnidadTributariaId },
                    type: 'POST',
                    success: function () {
                        window.open(BASE_URL + "Valuacion/AbrirReporte");
                    },
                    error: function (err, textStatus, errorThrown) {
                        if (err.status === 409) {
                            controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al generar el Certificado Valuatorio. La partida no posee dominio o su superficie es 0.");
                            return;
                        } else if (err.status === 400) {
                            controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al generar el Certificado Valuatorio. La partida no posee DDJJ, carguela.");
                        }else {
                            controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al generar el Certificado Valuatorio.");
                        }
                    },
                    complete: hideLoading
                });
            }
        });
        moduleContainerElem.addEventListener("ut-changed", function () {
            if (controller.isUTValida()) {
                $("#btnImprimirCertificadoValuatorio", moduleContainerElem).removeClass("disabled");
            } else {
                $("#btnImprimirCertificadoValuatorio", moduleContainerElem).addClass("disabled");
            }
        });
    }

    return {
        init: init
    };
};