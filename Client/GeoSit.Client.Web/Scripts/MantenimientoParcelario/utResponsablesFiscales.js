var UTResponsablesFiscales = function () {
    var __controller, __parentContainerElem, __moduleContainerElem, __currentUT, tabla;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-ut-responsables-fiscales");
        tabla = $("#tablaResponsablesFiscales", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "bDestroy": true,
            "ajax": BASE_URL + "MantenimientoParcelario/GetUtResponsablesFiscal?idUnidadTributaria=0",
            "columns": [
                { "data": "TipoPersona" },
                { "data": "NombreCompleto" },
                { "data": "DomicilioFisico" }
            ],
            "createdRow": function (row) {
                if (__currentUT && !__currentUT.FechaBaja) {
                    $(row).on("click", rowClicked);
                }
            }
        });
        $("tbody tr", tabla).off("click");
        $("#responsable-delete", __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row(".selected");
            if (selected) {
                var cb = function () {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/DeleteResponsableFiscal",
                        type: "POST",
                        data: selected.data(),
                        success: function () {
                            selected.remove().draw();
                        },
                        error: function () {
                            __controller.mostrarError("Ha ocurrido un error al borrar el responsable fiscal.");
                        }
                    });
                };
                __controller.mostrarConfirmacion("Responsable Fiscal - Eliminar", "¿Desea eliminar a " + selected.data().NombreCompleto + " como responsable fiscal?", cb);
            }
        });
        $("#responsable-edit", __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row(".selected");
            if (selected) {
                loadResponsableFiscal(Object.assign({ Operation: 1 }, selected.data()))
                    .then(function (responsable) {
                        selected.data(responsable).draw();
                    });
            }
        });
        $("#responsable-insert", __moduleContainerElem).oneClick(function () {
            $("tbody tr.selected", tabla).click();
            loadResponsableFiscal()
                .then(function (responsable) {
                    tabla.api().row.add(responsable).draw();
                });
        });
        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        }
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        __parentContainerElem.addEventListener("ut-changed", function (evt) {
            __currentUT = evt.detail;
            $("tbody tr", tabla).removeClass("selected");
            tabla.api().ajax.url(BASE_URL + "MantenimientoParcelario/GetUtResponsablesFiscal?idUnidadTributaria=" + (__currentUT && __currentUT.UnidadTributariaId || 0))
                .load(function () {
                    var buttons = ["#responsable-delete", "#responsable-edit", "#responsable-insert"];
                    if (!__currentUT || __currentUT.FechaBaja) {
                        $(buttons.join(","), __moduleContainerElem).addClass("disabled");
                    } else {
                        $(buttons.splice(0, 2).join(","), __moduleContainerElem).addClass("disabled");
                        $(buttons.join(","), __moduleContainerElem).removeClass("disabled");
                    }
                });
        });
        ["tab-changed", "begin-edition", "end-edition"].forEach(function (evt) {
            __parentContainerElem.addEventListener(evt, function () {
                __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            });
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        $(evt.currentTarget).toggleClass("selected").siblings().removeClass("selected");
        $("#responsable-delete, #responsable-edit", __moduleContainerElem).addClass("disabled");
        if ($(evt.currentTarget).hasClass("selected")) {
            $("#responsable-delete, #responsable-edit", __moduleContainerElem).removeClass("disabled");
        }
    }

    function loadResponsableFiscal(selected) {
        return new Promise(function (resolve) {
            var params = Object.assign({
                UnidadTributariaPersonaId: __controller.getNextId(tabla.api().data(), "UnidadTributariaPersonaId"),
                SavedPersonaId: (selected || { PersonaId: 0 }).PersonaId
            }, selected, { UnidadTributariaId: __currentUT.UnidadTributariaId, Operacion: selected ? 1 : 0 });
            showLoading();
            $.ajax({
                url: BASE_URL + "ResponsableFiscal/FormContent",
                type: 'POST',
                dataType: 'html',
                data: params,
                success: function (html) {
                    $(document).off("responsableFiscalGuardado").one("responsableFiscalGuardado", function (evt) {
                        resolve(evt.responsableFiscal);
                    });
                    __controller.cargarHTML(html);
                },
                error: function (_, textStatus, errorThrown) {
                    __controller.mostrarError(errorThrown);
                }
            });
        });
    }

    return {
        init: init
    };
};
//# sourceURL=utResponsables.js