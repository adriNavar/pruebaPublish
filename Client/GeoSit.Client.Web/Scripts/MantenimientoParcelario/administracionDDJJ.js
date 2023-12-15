var AdministracionDDJJ = function () {
    function init(moduleContainerElem, controller) {
        $('#btnDDJJ', moduleContainerElem).oneClick(function () {
            if (controller.isUTValida()) {
                showLoading();
                $.ajax({
                    url: BASE_URL + "DeclaracionesJuradas/DeclaracionesJuradas",
                    method: "GET",
                    data: { idUnidadTributaria: controller.obtenerUT().UnidadTributariaId },
                    dataType: "html",
                    success: function (html) {
                        controller.cargarHTML(html);
                    },
                    error: function () {
                        controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al cargar el formulario de Declaraciones Juradas.");
                    }
                });
            }
        });
        moduleContainerElem.addEventListener("ut-changed", function (evt) {
            if (controller.isUTValida()) {
                $("#btnDDJJ", moduleContainerElem).removeClass("disabled");
            } else {
                $("#btnDDJJ", moduleContainerElem).addClass("disabled");
            }
        });
    }

    return {
        init: init
    };
};