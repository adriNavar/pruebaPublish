var UTDomicilios = function () {
    var __controller, __parentContainerElem, __moduleContainerElem, __currentUT;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-ut-domicilios");
        var tabla = $("#tablaDomicilios", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "bDestroy": true,
            "ajax": BASE_URL + "MantenimientoParcelario/GetUTDomicilios?idUT=0",
            "columns": [
                { "data": "TipoDomicilio.Descripcion" },
                {
                    "data": "ViaNombre", "render": function (data, _, full) {
                        return data + " " + (full.numero_puerta || "");
                    }
                },
                { "data": "codigo_postal" },
                { "data": "localidad" }
            ],
            "createdRow": function (row) {
                if (__currentUT && !__currentUT.FechaBaja) {
                    $(row).on("click", rowClicked);
                }
            }
        });

        $("tbody tr", tabla).off("click");
        $("#domicilio-delete", __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row(".selected");
            if (selected) {
                var cb = function () {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/DeleteUnidadTributariaDomicilio",
                        type: 'POST',
                        data: { idUT: __currentUT.UnidadTributariaId, idDomicilio: selected.DomicilioId },
                        dataType: 'json',
                        success: function () {
                            selected.remove().draw();
                        },
                        error: function (err) {
                            __controller.mostrarError("Domicilios - Agregar", err.statusText);
                        }
                    });
                };
                __controller.mostrarConfirmacion("Domicilios - Eliminar", "¿Desea eliminar el domicilio " + (selected.data().ViaNombre + " " + selected.data().numero_puerta).trim() + "?", cb);
            }
        });
        $("#domicilio-edit", __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row(".selected");
            if (selected) {
                loadDomicilios(selected.data())
                    .then(function (domicilio) {
                        $.ajax({
                            url: BASE_URL + "MantenimientoParcelario/EditUnidadTributariaDomicilio",
                            type: 'POST',
                            dataType: 'json',
                            data: { domicilio: domicilio, idUT: __currentUT.UnidadTributariaId },
                            success: function (data) {
                                selected.data(data).draw();
                            },
                            error: function (err) {
                                let msgs = ["No se puede editar el domicilio.",""];
                                switch (err.status) {
                                    case 400:
                                        msgs = "La unidad tributaria no existe.";
                                        break;
                                    case 412:
                                        msgs = "El domicilio no está asociado a la unidad tributaria.";
                                        break;
                                    case 409:
                                        msgs = "El domicilio no existe.";
                                        break;
                                    default:
                                        msgs = [...msgs, err.statusText];
                                        break;
                                }
                                __controller.mostrarError("Domicilios - Modificar", msg.join("<br />"));
                            }
                        });
                    });
            }
        });
        $("#domicilio-insert", __moduleContainerElem).oneClick(function () {
            $("tbody tr.selected", tabla).click();
            loadDomicilios()
                .then(function (domicilio) {
                    domicilio.DomicilioId = __controller.getNextId(tabla.api().data(), "DomicilioId");
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/AddUnidadTributariaDomicilio",
                        type: 'POST',
                        dataType: 'json',
                        data: { domicilio: domicilio, idUT: __currentUT.UnidadTributariaId },
                        success: function (data) {
                            tabla.api().row.add(data).draw();
                        },
                        error: function (err) {
                            if (err.status === 400) {
                                __controller.mostrarError("Domicilios - Agregar", "No se puede agregar el domicilio.<br />La unidad tributaria no existe");
                            } else {
                                __controller.mostrarError("Domicilios - Agregar", err.statusText);
                            }
                        }
                    });
                });
        });

        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        }
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        ["tab-changed", "begin-edition", "end-edition"].forEach(function (evt) {
            __parentContainerElem.addEventListener(evt, function () {
                __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            });
        });
        __parentContainerElem.addEventListener("ut-changed", function (evt) {
            __currentUT = evt.detail;
            $("tbody tr", tabla).removeClass("selected");
            tabla.api().ajax.url(BASE_URL + "MantenimientoParcelario/GetUTDomicilios?idUT=" + (__currentUT && __currentUT.UnidadTributariaId || 0))
                .load(function () {
                    var buttons = ["#domicilio-delete", "#domicilio-edit", "#domicilio-insert"];
                    if (!__currentUT || __currentUT.FechaBaja) {
                        $(buttons.join(","), __moduleContainerElem).addClass("disabled");
                    } else {
                        $(buttons.splice(0, 2).join(","), __moduleContainerElem).addClass("disabled");
                        $(buttons.join(","), __moduleContainerElem).removeClass("disabled");
                    }
                });
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        $(evt.currentTarget).toggleClass("selected").siblings().removeClass("selected");
        $("#domicilio-delete, #domicilio-edit", __moduleContainerElem).addClass("disabled");
        if ($(evt.currentTarget).hasClass("selected")) {
            $("#domicilio-delete, #domicilio-edit", __moduleContainerElem).removeClass("disabled");
        }
    }
    function loadDomicilios(selected) {
        return new Promise(function (resolve) {
            var id = (selected || { DomicilioId: 0 }).DomicilioId;
            showLoading();
            $.ajax({
                url: BASE_URL + "Domicilio/DatosDomicilio",
                method: "POST",
                data: { id: id, domicilio: selected },
                dataType: "html",
                success: function (html) {
                    $(document).off("domicilioGuardado").one("domicilioGuardado", function (evt) {
                        resolve(evt.domicilio);
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
};
//# sourceURL=utDomicilios.js