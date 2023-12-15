var VIR = function () {
    var __parentContainerElem, __moduleContainerElem;

    function init(container, controller) {
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-VIR");

        const idInmueble = $('#IdInmueble', __moduleContainerElem).val();

        $("#tablaVIR", __moduleContainerElem).dataTable({
            "scrollX": true,
            "scrollY": "150px",
            "scrollCollapse": true,
            "paging": false,
            "searching": true,
            "processing": true,
            "dom": "rt",
            "order": [[0, "desc"]],
            "language": { "url": `${BASE_URL}Scripts/dataTables.spanish.txt` },
            "bDestroy": true,
            "ajax": `${BASE_URL}VIR/GetVIR?idInmueble=${idInmueble}`,
            "columnDefs": [{ className: "text-right", "targets": "_all" }],
            "columns": [
                {
                    "data": "VigenciaDesde", "type": "date", className: "text-left", "render": function (data, type) {
                        if (type === "display") {
                            return FormatFechaHora(data, false);
                        }
                        return data;
                    }
                },
                {
                    "data": "ValorTotal", "render": function (data) {
                        return controller.formatoValorMoneda(data)
                    }
                },
                {
                    "data": "ValorTierra", "render": function (data) {
                        return controller.formatoValorMoneda(data)
                    }
                },
                {
                    "data": "ValorMejoras", "render": function (data) {
                        return controller.formatoValorMoneda(data)
                    }
                }
            ],
            "createdRow": function (row, data) {
                if (!!data.Vigente) {
                    $(row).addClass('valuacion-vir-vigente');
                }
            }
        });
        $("#btnInformeParcelarioVIR", __moduleContainerElem).oneClick(function () {
            showLoading();
            $.ajax({
                url: `${BASE_URL}MantenimientoParcelario/GetInformeParcelarioVIR`,
                type: "POST",
                success: function () {
                    setTimeout(function () {
                        window.open(BASE_URL + "MantenimientoParcelario/Abrir", "_blank");
                    }, 10);
                },
                error: function () {
                    controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al generar el Reporte Parcelario VIR.");
                },
                complete: hideLoading
            });
        });
        $('#btnVIR-edit', __moduleContainerElem).oneClick(function () {
            showLoading();
            $.ajax({
                url: `${BASE_URL}VIR/VIREdit`,
                method: "GET",
                data: { idInmueble: idInmueble },
                dataType: "html",
                success: function (html) {
                    $(document)
                        .off("virError")
                        .on("virError", (evt) => controller.mostrarError(evt.message));

                    debugger
                    controller.cargarHTML(html);
                },
                error: function () {
                    controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al cargar el formulario de VIR.");
                },
                complete: hideLoading
            });
        });
    }

    return {
        init: init
    };

};