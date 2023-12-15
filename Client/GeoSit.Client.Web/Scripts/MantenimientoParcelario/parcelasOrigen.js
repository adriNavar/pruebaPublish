var ParcelasOrigen = function () {
    var __controller, __parentContainerElem, __moduleContainerElem;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = document.querySelector(".panel-parcelas-origen");

        var tabla = $("#parcelas-origen", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": `${BASE_URL}Scripts/dataTables.spanish.txt` },
            "bDestroy": true,
            "ajax": `${BASE_URL}MantenimientoParcelario/GetParcelasOrigen`,
            "columns": [
                { "data": "TipoParcela" },
                { "data": "CodigoProvincial" },
                { "data": "TipoOperacion" },
                {
                    "data": "FechaAlta", "render": function (data) {
                        return FormatFechaHora(data, false);
                    }
                }
            ],
            "createdRow": function (row) {
                $(row).on("click", rowClicked);
            },
            "initComplete": function () { rowClicked(); }
        });
        $('#parcela-origen-delete', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected) {
                var cb = function () {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/DeleteParcelaOrigen",
                        type: 'POST',
                        data: { id: selected.data().IdOperacion },
                        success: function () {
                            selected.remove().draw();
                            rowClicked();
                        },
                        error: function (_, textStatus, errorThrown) {
                            console.log(textStatus, errorThrown);
                        }
                    });
                };
                __controller.mostrarConfirmacion("Parcelas Origen - Eliminar", `¿Desea eliminar la relación con la parcela ${selected.data().CodigoProvincial}?`, cb);
            }
        });
        $('#parcela-origen-edit', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected) {
                showLoading();
                loadParcelaOrigen(selected)
                    .then(function (parcelaOrigen) {
                        $.ajax({
                            url: BASE_URL + "MantenimientoParcelario/SaveParcelaOrigen",
                            type: 'POST',
                            data: parcelaOrigen,
                            success: function (data) {
                                selected.data(data).draw();
                            },
                            error: function (_, textStatus, errorThrown) {
                                console.log(textStatus, errorThrown);
                            }
                        });
                    });
            }
        });
        $('#parcela-origen-insert', __moduleContainerElem).oneClick(function () {
            $("tbody tr.selected", tabla).click();
            showLoading();
            loadParcelaOrigen()
                .then(function (parcelaOrigen) {
                    parcelaOrigen.IdOperacion = __controller.getNextId(tabla.api().data(), "IdOperacion");
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/SaveParcelaOrigen",
                        type: 'POST',
                        data: parcelaOrigen,
                        success: function (data) {
                            tabla.api().row.add(data).draw();
                        },
                        error: function (_, textStatus, errorThrown) {
                            console.log(textStatus, errorThrown);
                        }
                    });
                });
        });
        $("tbody tr", tabla).off("click");
        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        };
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        __parentContainerElem.addEventListener("tab-changed", function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
        });
        __parentContainerElem.addEventListener("reset-tables", function () {
            tabla.api().ajax.url(`${BASE_URL}MantenimientoParcelario/GetParcelasOrigen`).load();
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        $("#parcela-origen-delete, #parcela-origen-edit", __moduleContainerElem).addClass("disabled");
        if (evt && evt.currentTarget) {
            $(evt.currentTarget).toggleClass("selected").siblings().removeClass("selected");
            if ($(evt.currentTarget).hasClass("selected")) {
                $("#parcela-origen-delete, #parcela-origen-edit", __moduleContainerElem).removeClass("disabled");
            }
        }
    }
    function loadParcelaOrigen(selected) {
        return new Promise(function (resolve) {
            const parametros = selected && { id: selected.data().IdOperacion } || {};
            $.ajax({
                url: BASE_URL + "MantenimientoParcelario/GetParcelaOrigen",
                type: 'GET',
                data: parametros,
                dataType: 'html',
                success: function (html) {
                    $(document)
                        .off("parcelaOrigenGuardada")
                        .one("parcelaOrigenGuardada", function (evt) {
                            resolve(evt.parcelaOrigen);
                        });
                    __controller.cargarHTML(html);
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                }
            });
        });
    }
    return {
        init: init
    };
}