var UTDominios = function () {
    var __controller, __parentContainerElem, __moduleContainerElem, __currentUT, tabla, idClaseParcela;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-ut-dominios");
        var tieneTitulares = false;
        tabla = $("#tablaDominios", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "bDestroy": true,
            "ajax": BASE_URL + "MantenimientoParcelario/GetUtDominios?idUnidadTributaria=0",
            "columns": [
                { "data": "TipoInscripcionDescripcion" },
                { "data": "Inscripcion" },
                {
                    "data": "Fecha", "render": function (data) {
                        return FormatFechaHora(data, false);
                    }
                }
            ],
            "createdRow": function (row, data) {
                $(row).off("click").on("click", rowClicked.bind(data));
            }
        });
        $("tbody tr", tabla).off("click");
        $('#dominio-delete', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                var cb = function () {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/DeleteDominio",
                        type: 'POST',
                        data: selected.data(),
                        success: function () {
                            selected.remove().draw();
                            if (tabla.api().data().length) {
                                $(tabla.api().row().node()).click();
                            } else {
                                publishDominioChanged();
                            }
                        }
                    });
                };
                var msgs = ["¿Desea eliminar el dominio con tipo de inscripción " + selected.data().TipoInscripcionDescripcion +
                    ", inscripción " + selected.data().Inscripcion + " y fecha " + FormatFechaHora(selected.data().Fecha, false) + "?"];
                if (tieneTitulares) {
                    msgs = [...msgs, "Tenga en cuenta que los titulares asociados también serán dados de baja."];
                }
                __controller.mostrarConfirmacion("Dominios - Eliminar", msgs, cb);
            }
        });
        $('#dominio-edit', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                loadDominio(Object.assign(selected.data(), { Operacion: 1 }))
                    .then(function (dominio) {
                        selected.data(dominio).draw();
                    });
            }
        });
        $('#dominio-insert', __moduleContainerElem).oneClick(function () {
            loadDominio({ DominioId: __controller.getNextId(tabla.api().data(), "DominioID") })
                .then(function (dominio) {
                    tabla.api().row.add(dominio).draw();
                });
        });
        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        };
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        __parentContainerElem.addEventListener("titulares-changed", function (evt) {
            tieneTitulares = evt.detail.cantidad > 0;
        });
        ["tab-changed", "begin-edition", "end-edition"].forEach(function (evt) {
            __parentContainerElem.addEventListener(evt, function () {
                __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            });
        });
        __parentContainerElem.addEventListener("ut-changed", function (evt) {
            __currentUT = evt.detail;
            $("tbody tr", tabla).removeClass("selected");
            tabla.api().ajax.url(BASE_URL + "MantenimientoParcelario/GetUtDominios?idUnidadTributaria=" + (__currentUT && __currentUT.UnidadTributariaId || 0))
                .load(function (resp) {
                    var buttons = ["#dominio-delete", "#dominio-edit", "#dominio-insert"];
                    if (!__currentUT || __currentUT.FechaBaja) {
                        $(buttons.join(","), __moduleContainerElem).addClass("disabled");
                    } else {
                        $(buttons.splice(0, 2).join(","), __moduleContainerElem).addClass("disabled");
                        $(buttons.join(","), __moduleContainerElem).removeClass("disabled");
                    }
                    if (!resp.data.length) {
                        publishDominioChanged(null);
                    } else {
                        $(tabla.api().row().node()).click();
                    }
                });
        });
        __parentContainerElem.addEventListener("clase-parcela-changed", function (evt) {
            idClaseParcela = evt.detail;
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        if (!$(evt.currentTarget).hasClass("selected")) {
            $(evt.currentTarget).addClass("selected").siblings().removeClass("selected");
            publishDominioChanged(this);
        }
        $("#dominio-delete, #dominio-edit", __moduleContainerElem).addClass("disabled");
        if ($(evt.currentTarget).hasClass("selected")) {
            $("#dominio-delete, #dominio-edit", __moduleContainerElem).removeClass("disabled");
        }
    }
    function publishDominioChanged(dominio) {
        __parentContainerElem.dispatchEvent(new CustomEvent("dominio-changed", { detail: dominio }));
    }
    function loadDominio(selected) {
        return new Promise(function (resolve) {
            var dominio = Object.assign(selected, { UnidadTributariaID: __currentUT.UnidadTributariaId });
            showLoading();
            $.ajax({
                url: `${BASE_URL}Dominio/FormContent`,
                type: 'POST',
                dataType: 'html',
                data: { dominio, idClaseParcela },
                success: function (html) {
                    $(document).off("dominioGuardado").one("dominioGuardado", function (evt) {
                        resolve(evt.dominio);
                    });
                    __controller.cargarHTML(html);
                },
                error: function (err) {
                    __controller.mostrarError(err.statusText);
                }
            });
        });
    }

    return {
        init: init
    };
};
//# sourceURL=dominios.js