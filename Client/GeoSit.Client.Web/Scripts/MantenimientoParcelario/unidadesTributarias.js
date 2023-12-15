var UnidadesTributarias = function () {
    var __controller, __parentContainerElem, __moduleContainerElem, tabla;
    function init(container, controller) {
        var readonly = true;
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-unidades-tributarias");

        tabla = $("#unidadesTributarias", __moduleContainerElem).dataTable({
            "scrollX": true,
            "scrollY": "150px",
            "scrollCollapse": true,
            "paging": false,
            "searching": true,
            "processing": true,
            "dom": "frt",
            "order": [[0, "asc"]],
            "language": { "url": `${BASE_URL}Scripts/dataTables.spanish.txt` },
            "ajax": `${BASE_URL}MantenimientoParcelario/GetUnidadesTributarias`,
            "columns": [
                { "data": "CodigoProvincial", "className": "uppercase" },
                { "data": "TipoUnidadTributaria.Descripcion" },
                { "data": "Piso" },
                { "data": "Unidad" },
                { "data": "PorcentajeCopropiedad" },
                {
                    "data": "FechaAlta", "render": function (data) {
                        return FormatFechaHora(data, false);
                    }
                },
                {
                    "data": "FechaBaja", "render": function (data) {
                        return FormatFechaHora(data, false);
                    }
                }
            ],
            "rowCallback": function (row, data) {
                $(row).off("click").on("click", rowClicked.bind(data));
            },
            "initComplete": function () {
                $(tabla.api().row().node()).click();
                if (tabla.fnGetData().length > 1) {
                    $(".mantenedor-body .tabla-con-botones dl").css("margin-top", "35px");
                }
                else {
                    $("#unidadesTributarias_filter").hide();
                }
                disableReportUtButton();
            }
        });
        $("tbody tr", tabla).off("click");
        $("#ut-insert").oneClick(function () {
            $('tbody tr', tabla).removeClass("selected");
            publishUTChanged(null);
            loadUT({ UnidadTributariaId: __controller.getNextId(tabla.api().data(), "UnidadTributariaId") })
                .then(function (ut) {
                    showLoading();
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/AddUnidadTributaria",
                        data: { unidadTributaria: ut },
                        type: "POST",
                        success: function (ut) {
                            var row = tabla.api().row.add(ut);
                            row.draw();
                            $(row.node()).click();
                        },
                        error: function (err) {
                            __controller.mostrarError("Unidades Tributarias - Agregar", err.statusText);
                        },
                        complete: hideLoading
                    });
                });
        });
        $("#ut-edit").oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected) {
                loadUT(selected.data())
                    .then(function (ut) {
                        showLoading();
                        $.ajax({
                            url: BASE_URL + "MantenimientoParcelario/EditUnidadTributaria",
                            data: { unidadTributaria: ut },
                            type: "POST",
                            success: function () {
                                selected.data(ut).draw();
                                $(selected.node()).click();
                            },
                            error: function (err) {
                                __controller.mostrarError("Unidades Tributarias - Modificar", err.statusText);
                            },
                            complete: hideLoading
                        });
                    });
            }
        });
        $("#ut-delete").oneClick(function () {
            if (tabla.api().rows().data().length === 1) {
                __controller.mostrarConfirmacion("Unidades Tributarias - Eliminar", "No puede eliminar la única unidad tributaria de la parcela");
                return;
            }
            var selected = tabla.api().row('.selected');
            if (selected) {
                __controller.mostrarConfirmacion("Unidades Tributarias - Eliminar", `¿Desea eliminar la unidad tributaria ${selected.data().CodigoProvincial.toUpperCase()}?`, deleteUT.bind(null, selected));
            }
        });
        $('#btnDDJJ').oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected) {
                showLoading();
                $.ajax({
                    url: BASE_URL + "DeclaracionesJuradas/DeclaracionesJuradas",
                    method: "GET",
                    data: { idUnidadTributaria: selected.data().UnidadTributariaId, editable: readonly },
                    dataType: "html",
                    success: function (html) {
                        __controller.cargarHTML(html);
                    },
                    error: function () {
                        __controller.mostrarError("Mantenedor Parcelario - Error", "Se ha producido un error al cargar el formulario de Declaraciones Juradas.");
                    }
                });
            }
        });
        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        };
        __parentContainerElem.addEventListener("begin-edition", function () { readonly = false; });
        __parentContainerElem.addEventListener("end-edition", function () { readonly = true; });
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        __parentContainerElem.addEventListener("tab-changed", function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
        });
        __parentContainerElem.addEventListener("reset-tables", function () {
            tabla.api().ajax.url(BASE_URL + "MantenimientoParcelario/GetUnidadesTributarias")
                .load(function () {
                    $(tabla.api().row().node()).click();
                });
        });
        __parentContainerElem.addEventListener("clase-parcela-changed", function (evt) {
            console.log(__parentContainerElem);
            console.log(evt.detail);
            __parentContainerElem.dispatchEvent(new CustomEvent(evt.type, { detail: ut }));
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        if (!$(evt.currentTarget).hasClass("selected")) {
            $(evt.currentTarget).addClass("selected").siblings().removeClass("selected");
            publishUTChanged(this);
        }
        disableReportUtButton();
        $("#observacionesUT").val(this.Observaciones);
    }
    function enableActions(ut) {
        var buttons = ["#btnDDJJ", "#exportarHistoricoCambiosUT", "#ut-delete", "#ut-edit", "#ut-insert"];
        if (!ut || ut.FechaBaja) {
            var disabled = buttons.splice(0, 4);
            $(disabled.join(","), __moduleContainerElem).addClass("disabled");
        }
        $(buttons.join(","), __moduleContainerElem).removeClass("disabled");
    }
    function deleteUT(selected) {
        debugger
        $.ajax({
            url: BASE_URL + 'MantenimientoParcelario/DeleteUnidadTributaria',
            data: { unidadTributariaId: selected.data().UnidadTributariaId },
            type: 'POST',
            success: function () {
                __parentContainerElem.dispatchEvent(new CustomEvent("ut-deleted"));
                selected.remove().draw();
            },
            error: function (_, textStatus, errorThrown) {
                console.log(textStatus, errorThrown);
            }
        });
    }
    function disableReportUtButton() {
        if (tabla.api().row('.selected').data().TipoUnidadTributariaID == 2) {
            $("#ut-reporte").addClass("disabled");
        } else {
            $("#ut-reporte").removeClass("disabled");
        }
    }
    function loadUT(selected) {
        return new Promise(function (resolve) {
            var total = tabla.api().rows().data().toArray().reduce(function (accum, row) {
                return accum + (!row.FechaBaja && selected !== row) ? row.PorcentajeCopropiedad : 0;
            }, 0);
            selected = Object.assign(selected, { Parcela: null, PorcientoCopropiedadTotal: total });
            $.ajax({
                url: BASE_URL + "UnidadTributaria/GetUnidadTributaria",
                type: 'POST',
                data: { ut: selected },
                dataType: 'html',
                success: function (html) {
                    $(document).off("unidadTributariaGuardada").one("unidadTributariaGuardada", function (evt) {
                        resolve(evt.unidadTributaria);
                    });
                    __controller.cargarHTML(html);
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                    __controller.mostrarError("Unidades Tributarias - Error", "Ha ocurrido un error al querer abrir el formulario de carga de datos de unidades tributarias");
                }
            });
        });
    }
    function publishUTChanged(ut) {
        __parentContainerElem.dispatchEvent(new CustomEvent("ut-changed", { detail: ut }));
        enableActions(ut);
    }
    function validate() {
        let error;
        const uts = tabla.api().rows().data().reduce((accum, ut) => { accum[`t${ut.TipoUnidadTributariaID}`] = Number(accum[`t${ut.TipoUnidadTributariaID}`] || 0) + 1; return accum; }, {});
        if (!uts["t1"] && !uts["t2"] || uts["t1"] && uts["t2"]) {
            error = "La parcela debe tener una unidad tributaria de tipo <strong>COMUN</strong> o <strong>PH</strong> o <strong>CI</strong>. Estas son EXCLUYENTES entre si.";
        } else if (uts["t1"] > 1) {
            error = "Una parcela no puede tener más de una unidad tributaria de tipo <strong>COMUN</strong>.";
        } else if (uts["t2"] > 1) {
            error = "Una parcela no puede tener más de una unidad tributaria de tipo <strong>PH</strong> o <strong>CI</strong>.";
        } else if (uts["t1"] && uts["t3"]) {
            error = "Una parcela con una unidad tributaria <strong>COMUN</strong> no puede tener otras unidades tributarias de tipo <strong>UF de PH</strong> o <strong>UP de CI</strong>.";
        }
        return { valid: !error, error: error };
    }

    function addModule(module) {
        var ctrl = Object.assign({}, controller);
        module.init(__parentContainerElem, ctrl);
    }
    return {
        init: init,
        addModule: addModule,
        validate: validate
    };
};