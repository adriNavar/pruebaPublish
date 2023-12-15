var Nomenclaturas = function () {
    var __controller, __parentContainerElem, __moduleContainerElem;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-nomenclaturas");

        var tabla = $("#nomenclaturas", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "bDestroy": true,
            "ajax": BASE_URL + "MantenimientoParcelario/GetNomenclaturas",
            "columns": [
                { "data": "Tipo.Descripcion" },
                { "data": "Nombre" },
                {
                    "data": "FechaModificacion", "render": function (data) {
                        return FormatFechaHora(data, false);
                    }
                }
            ],
            "createdRow": function (row) {
                $(row).on("click", rowClicked);
            }
        });

        $("tbody tr", tabla).off("click");
        $("#nomenclatura-insert", __moduleContainerElem).oneClick(function () {
            $("tbody tr.selected", tabla).click();
            loadNomenclaturas()
                .then(function (nomenclatura) {
                    nomenclatura.NomenclaturaID = __controller.getNextId(tabla.api().data(), "NomenclaturaID");
                    $.ajax({
                        url: BASE_URL + 'MantenimientoParcelario/AddNomenclatura',
                        data: { nomenclatura: nomenclatura },
                        type: 'POST',
                        dataType: 'json',
                        success: function (data) {
                            tabla.api().row.add(data).draw();
                        },
                        error: function (_, textStatus, errorThrown) {
                            console.log(textStatus, errorThrown);
                        }
                    });
                });
        });
        $("#nomenclatura-edit", __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                loadNomenclaturas(selected.data())
                    .then(function (nomenclatura) {
                        $.ajax({
                            url: BASE_URL + 'MantenimientoParcelario/EditNomenclatura',
                            data: { nomenclatura: nomenclatura },
                            type: 'POST',
                            dataType: 'json',
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
        $("#nomenclatura-delete", __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                var nomenclatura = selected.data();
                const noEsPrescripcion = $("#cboClase", __parentContainerElem).val() != '2';
                if (tabla.api().rows().data().length === 1 && noEsPrescripcion) {
                    __controller.mostrarError("Nomenclaturas - Eliminar", "No se puede eliminar la nomenclatura " + nomenclatura.Nombre + ".<br />La parcela no puede quedar sin nomenclaturas.");
                } else {
                    var cb = function () {
                        nomenclatura.Tipo = "";
                        nomenclatura.Parcela = null;
                        $.ajax({
                            url: BASE_URL + 'MantenimientoParcelario/DeleteNomenclatura',
                            type: 'POST',
                            data: { nomenclatura: nomenclatura },
                            success: function () {
                                selected.remove().draw();
                            },
                            error: function (_, textStatus, errorThrown) {
                                console.log(textStatus, errorThrown);
                            }
                        });
                    };
                    __controller.mostrarConfirmacion("Nomenclaturas - Eliminar", "¿Desea eliminar la nomenclatura " + nomenclatura.Nombre + "?", cb);
                }
            }
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
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        $(evt.currentTarget).toggleClass("selected").siblings().removeClass("selected");
        $("#nomenclatura-delete, #nomenclatura-edit", __moduleContainerElem).addClass("disabled");
        if ($(evt.currentTarget).hasClass("selected")) {
            $("#nomenclatura-delete, #nomenclatura-edit", __moduleContainerElem).removeClass("disabled");
        }
    }
    function loadNomenclaturas(selected) {
        return new Promise(function (resolve) {
            var parametros = {};
            if (selected) {
                selected.Tipo.ExpresionRegular = "";
                selected.Parcela = null;
                parametros = { nomenclatura: selected };
            }
            $.ajax({
                url: BASE_URL + "Nomenclatura/GetNomenclatura",
                type: 'POST',
                data: parametros,
                dataType: 'html',
                success: function (html) {
                    $(document).off("nomenclaturaGuardada").one("nomenclaturaGuardada", function (evt) {
                        resolve(evt.nomenclatura);
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