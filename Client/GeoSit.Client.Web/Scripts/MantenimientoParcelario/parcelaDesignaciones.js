var ParcelaDesignaciones = function () {
    var __controller, __parentContainerElem, __moduleContainerElem, tabla;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-parcela-designaciones");

        tabla = $("#designaciones", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "bDestroy": true,
            "ajax": BASE_URL + "MantenimientoParcelario/GetParcelaDesignaciones",
            "columns": [
                { "data": "Designacion.TipoDesignador.Nombre", "width": "100px" },
                { "data": "Descripcion" }
            ],
            "createdRow": function (row) {
                $(row).on("click", rowClicked);
            },
            "initComplete": function () {
                enableInsert(tabla);
            }
        });
        $("tbody tr", tabla).off("click");
        $('#designacion-delete', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                var cb = function () {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/DeleteParcelaDesignacion",
                        type: 'POST',
                        data: { idDesignacion: selected.data().Designacion.IdDesignacion },
                        success: function () {
                            tabla.one("draw.dt", () => enableInsert(tabla));
                            $(selected.node()).click();
                            selected.remove().draw();
                        },
                        error: function (err) {
                            if (err.status === 409) {
                                __controller.mostrarError("Designaciones - Eliminar", "No existe la designación que está eliminando.");
                            }
                        }
                    });
                };
                __controller.mostrarConfirmacion("Designaciones - Eliminar", "¿Desea eliminar la designación de " + selected.data().Designacion.TipoDesignador.Nombre + "?", cb);
            }
        });
        $('#designacion-edit', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected.length) {
                loadDesignacion(selected.data())
                    .then(function (designacion) {
                        $.ajax({
                            url: BASE_URL + "MantenimientoParcelario/EditParcelaDesignacion",
                            type: 'POST',
                            data: { designacion: designacion },
                            success: function (formatted) {
                                selected.data(formatted).draw();
                            },
                            error: function (_, textStatus, errorThrown) {
                                console.log(textStatus, errorThrown);
                            }
                        });
                    });
            }
        });
        $('#designacion-insert', __moduleContainerElem).oneClick(function () {
            $("tbody tr.selected", tabla).click();
            loadDesignacion()
                .then(function (designacion) {
                    designacion.IdDesignacion = __controller.getNextId(tabla.api().data().map(d => d.Designacion), "IdDesignacion");
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/AddParcelaDesignacion",
                        type: 'POST',
                        data: { designacion: designacion },
                        success: function (formatted) {
                            tabla.one("draw.dt", () => enableInsert(tabla));
                            tabla.api().row.add(formatted).draw();
                        },
                        error: function (_, textStatus, errorThrown) {
                            console.log(textStatus, errorThrown);
                        }
                    });
                });
        });
        var postLoadedTasks = function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
            __parentContainerElem.removeEventListener("form-loaded", postLoadedTasks);
        }
        __parentContainerElem.addEventListener("form-loaded", postLoadedTasks);
        __parentContainerElem.addEventListener("tab-changed", function () {
            __parentContainerElem.dispatchEvent(new CustomEvent("resizeTableColumns"));
        });
        __parentContainerElem.addEventListener("reset-tables", function () {
            tabla.api().ajax.url(BASE_URL + "MantenimientoParcelario/GetParcelaDesignaciones").load();
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        $(evt.currentTarget).toggleClass("selected").siblings().removeClass("selected");
        $("#designacion-delete, #designacion-edit", __moduleContainerElem).addClass("disabled");
        if ($(evt.currentTarget).hasClass("selected")) {
            $("#designacion-delete, #designacion-edit", __moduleContainerElem).removeClass("disabled");
        }
    }
    function loadDesignacion(selected) {
        return new Promise(function (resolve) {
            showLoading();
            var designacion = (selected || { Designacion: { IdTipoDesignador: $("#cboClase", __parentContainerElem).val()} }).Designacion;
            $.ajax({
                url: BASE_URL + "Designaciones",
                method: "POST",
                data: designacion,
                dataType: "html",
                success: function (html) {
                    $(document).off("designacionGuardada").one("designacionGuardada", function (evt) {
                        resolve(evt.designacion);
                    });
                    __controller.cargarHTML(html);
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                }
            });
        });
    }
    function enableInsert(tabla) {
        const esPrescripcion = $("#cboClase", __parentContainerElem).val() === '2';
        $('#designacion-insert', __moduleContainerElem).hide();
        $('#designacion-insert', __moduleContainerElem).removeAttr("data-editable");
        /*if (esPrescripcion && tabla.api().rows().data().length < 1 || !esPrescripcion && tabla.api().rows().data().length < 2) {
            $('#designacion-insert', __moduleContainerElem).show();
            $('#designacion-insert', __moduleContainerElem).attr("data-editable", "");
        }*/
        $.get(`${BASE_URL}MantenimientoParcelario/PuedeAgregarDesignacion`, { idClaseParcela: $("#cboClase", __parentContainerElem).val()}, (result) => {
            if (result) {
                $('#designacion-insert', __moduleContainerElem).show();
                $('#designacion-insert', __moduleContainerElem).attr("data-editable", "");
            }
        });
    }

    function validate() {
        const esPrescripcion = $("#cboClase", __parentContainerElem).val() === '2';
        const error = esPrescripcion && tabla.api().data().toArray().some(r => r.Designacion.IdTipoDesignador === 1);

        return { valid: !error, error: error && "Una parcela de clase PRESCRIPCION no puede tener una designación de tipo TITULO." };
    }

    return {
        init: init,
        validate: validate
    };
};
//# sourceURL=parcelaDesignaciones.js