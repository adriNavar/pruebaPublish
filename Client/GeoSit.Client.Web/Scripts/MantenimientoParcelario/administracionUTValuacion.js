var AdministracionUTValuacion = function () {
    function init(moduleContainerElem, controller, mainContainerElem) {
        var readonly = true;
        $('#btnValuaciones', moduleContainerElem).oneClick(function () {
            if (controller.isUTValida()) {
                showLoading();
                $.ajax({
                    url: BASE_URL + "DeclaracionesJuradas/Valuaciones",
                    method: "GET",
                    data: { idUnidadTributaria: controller.obtenerUT().UnidadTributariaId, editable: !readonly },
                    dataType: "html",
                    success: function (html) {
                        controller.cargarHTML(html);
                    },
                    error: function () {
                        controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al cargar el formulario de Valuaciones.");
                    }
                });
            }
        });

        mainContainerElem.addEventListener("begin-edition", function () { readonly = false; });
        mainContainerElem.addEventListener("end-edition", function () { readonly = true; });

        moduleContainerElem.addEventListener("ut-changed", function () {
            if (controller.isUTValida()) {
                $("#btnValuaciones", moduleContainerElem).removeClass("disabled");
            } else {
                $("#btnValuaciones", moduleContainerElem).addClass("disabled");
            }
        });
    }

    return {
        init: init
    };
};