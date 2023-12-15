var UTTitulares = function () {
    var __controller, __parentContainerElem, __moduleContainerElem, __currentDominio, __cargando, idClaseParcela;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-ut-titulares");
        var tabla = $("#tablaTitulares", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "bDestroy": true,
            "columns": [
                { "data": "NombreCompleto" },
                { "data": "TipoNoDocumento" },
                { "data": "PorcientoCopropiedad" }
            ],
            "createdRow": function (row) {
                if (__currentDominio && !__currentDominio.FechaBaja) {
                    $(row).on("click", rowClicked);
                }
            }
        });
        $("tbody tr", tabla).off("click");
        $('#titular-delete', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                var cb = function () {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/DeleteTitular",
                        type: 'POST',
                        data: selected.data(),
                        success: function () {
                            selected.remove().draw();
                            var titulares = tabla.api().rows().data();
                            enableInsert(titulares);
                            publishTitularesChanged(titulares);
                        }
                    });
                };
                __controller.mostrarConfirmacion("Titulares - Eliminar", "¿Desea eliminar a " + selected.data().NombreCompleto + " como titular?", cb);
            }
        });
        $('#titular-edit', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                loadTitular(Object.assign(selected.data(), { Operacion: 1, PorcientoCopropiedadTotal: getPorcentajeCopropiedadTotal(tabla.api().rows().data()) - selected.data().PorcientoCopropiedad }))
                    .then(function (titular) {
                        selected.data(titular).draw();
                        enableInsert(tabla.api().rows().data());
                    });
            }
        });
        $('#titular-insert', __moduleContainerElem).oneClick(function () {
            $("tbody tr.selected", tabla).click();
            var titulares = tabla.api().rows().data();
            loadTitular({ DominioPersonaId: __controller.getNextId(titulares, "DominioPersonaId"), PorcientoCopropiedadTotal: getPorcentajeCopropiedadTotal(titulares) })
                .then(function (titular) {
                    tabla.api().row.add(titular).draw();
                    var titulares = tabla.api().rows().data();
                    enableInsert(titulares);
                    publishTitularesChanged(titulares);
                });
        });
        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        };
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        ["tab-changed", "begin-edition", "end-edition"].forEach(function (evt) {
            __parentContainerElem.addEventListener(evt, function () {
                __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            });
        });
        __parentContainerElem.addEventListener("dominio-changed", function (evt) {
            __currentDominio = evt.detail;
            $("tbody tr", tabla).removeClass("selected");
            __cargando = true;
            tabla.api().ajax.url(BASE_URL + "MantenimientoParcelario/GetUtTitulares?idDominio=" + (__currentDominio && __currentDominio.DominioID || -1))
                .load(function (resp) {
                    publishTitularesChanged(resp.data);
                    var buttons = ["#titular-delete", "#titular-edit", "#titular-insert"];
                    if (!__currentDominio) {
                        $(buttons.join(","), __moduleContainerElem).addClass("disabled");
                    } else {
                        $(buttons.splice(0, 2).join(","), __moduleContainerElem).addClass("disabled");
                        $(buttons.join(","), __moduleContainerElem).removeClass("disabled");
                        enableInsert(resp.data);
                    }
                });
        });
        __parentContainerElem.addEventListener("clase-parcela-changed", function (evt) {
            idClaseParcela = evt.detail;
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function publishTitularesChanged(titulares) {
        __parentContainerElem.dispatchEvent(new CustomEvent("titulares-changed", { detail: { cantidad: titulares.length } }));
    }
    function getPorcentajeCopropiedadTotal(titulares) {
        return titulares.reduce(function (accum, titular) { return accum + parseFloat(titular.PorcientoCopropiedad); }, 0);
    }
    function enableInsert(titulares) {
        $('#titular-insert', __moduleContainerElem).hide();
        $('#titular-insert', __moduleContainerElem).removeAttr("data-editable");
        if (getPorcentajeCopropiedadTotal(titulares) < 100) {
            $('#titular-insert', __moduleContainerElem).show();
            $('#titular-insert', __moduleContainerElem).attr("data-editable", "");
        }
    }
    function rowClicked(evt) {
        $(evt.currentTarget).toggleClass("selected").siblings().removeClass("selected");
        $("#titular-delete, #titular-edit", __moduleContainerElem).addClass("disabled");
        if ($(evt.currentTarget).hasClass("selected")) {
            $("#titular-delete, #titular-edit", __moduleContainerElem).removeClass("disabled");
        }
    }
    function loadTitular(selected) {
        return new Promise(function (resolve) {
            var dominioTitular = Object.assign(selected, { DominioId: __currentDominio.DominioID });
            showLoading();
            $.ajax({
                url: `${BASE_URL}DominioTitular/FormContent`,
                type: 'POST',
                dataType: 'html',
                data: { dominioTitular, idClaseParcela },
                success: function (html) {
                    $(document).off("titularGuardado").one("titularGuardado", function (evt) {
                        resolve(evt.titular);
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
//# sourceURL=titulares.js