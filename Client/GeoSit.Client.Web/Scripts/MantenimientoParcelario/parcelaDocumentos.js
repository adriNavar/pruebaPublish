var ParcelaDocumentos = function () {
    var __controller, __parentContainerElem, __moduleContainerElem, tabla;
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __moduleContainerElem = __parentContainerElem.querySelector(".panel-parcela-documentos");

        tabla = $("#tablaParcelaDocumentos", __moduleContainerElem).dataTable({
            "scrollY": "100px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "order": [[0, "asc"]],
            "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            "bDestroy": true,
            "ajax": BASE_URL + "MantenimientoParcelario/GetParcelaDocumentos",
            "columns": [
                { "data": "Tipo.Descripcion" },
                { "data": "nombre_archivo" },
                {
                    "data": "fecha_alta_1", "render": function (data) {
                        return FormatFechaHora(data, false);
                    }
                },
                { "data": "descripcion" }
            ],
            "createdRow": function (row) {
                $(row).on("click", rowClicked);
            }
        });
        $("tbody tr", tabla).off("click");
        $('#documento-delete', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected) {
                var cb = function () {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/DeleteParcelaDocumento",
                        type: 'POST',
                        data: {
                            idDocumento: selected.data().id_documento,
                        },
                        success: function () {
                            selected.remove().draw();
                        },
                        error: function (_, textStatus, errorThrown) {
                            console.log(textStatus, errorThrown);
                        }
                    });
                };
                __controller.mostrarConfirmacion("Documentos - Eliminar", "¿Desea eliminar el documento " + selected.data().nombre_archivo + "?", cb);
            }
        });
        $('#documento-edit', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected');
            if (selected) {
                showLoading();
                $.post(BASE_URL + "Documento/Editable", function () {
                    loadDocumentos(selected.data())
                        .then(function (documento) {
                            $.ajax({
                                url: BASE_URL + "MantenimientoParcelario/EditParcelaDocumento",
                                type: 'POST',
                                data: { idDocumento: documento.id_documento },
                                success: function () {
                                    tabla.api().row(selected).data(documento).draw();
                                },
                                error: function (_, textStatus, errorThrown) {
                                    console.log(textStatus, errorThrown);
                                }
                            });
                        });
                });
            }
        });
        $('#documento-view', __moduleContainerElem).oneClick(function () {
            var selected = tabla.api().row('.selected').data();
            if (selected && ["pdf", "jpg", "jpeg", "png", "tif", "tiff", "gif", "bmp"].indexOf(selected.extension_archivo.toLowerCase()) !== -1) {
                showLoading();
                $.ajax({
                    url: BASE_URL + "PdfInternalViewer/View/",
                    method: "GET",
                    data: { id: selected.id_documento },
                    dataType: "html",
                    success: function (html) {
                        __controller.cargarHTML(html);
                    },
                    error: function (_, textStatus, errorThrown) {
                        console.log(textStatus, errorThrown);
                    }
                });
            } else if (selected) {
                var a = document.createElement("a");
                a.style = "display: none";
                a.href = BASE_URL + "MantenimientoParcelario/DownloadDocumento/" + selected.id_documento;
                a.click();
                a.remove();
            }
        });
        $('#documento-insert', __moduleContainerElem).oneClick(function () {
            $("tbody tr.selected", tabla).click();
            showLoading();
            loadDocumentos()
                .then(function (documento) {
                    $.ajax({
                        url: BASE_URL + "MantenimientoParcelario/AddParcelaDocumento",
                        type: 'POST',
                        data: { idDocumento: documento.id_documento },
                        success: function () {
                            tabla.api().row.add(documento).draw();
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
            tabla.api().ajax.url(BASE_URL + "MantenimientoParcelario/GetParcelaDocumentos").load();
        });
        __controller.registrarAjustarColumnas("resizeTableColumns", tabla);
    }
    function rowClicked(evt) {
        $(evt.currentTarget).toggleClass("selected").siblings().removeClass("selected");
        $("#documento-delete, #documento-edit, #documento-view", __moduleContainerElem).addClass("disabled");
        if ($(evt.currentTarget).hasClass("selected")) {
            $("#documento-view", __moduleContainerElem).removeClass("disabled");
            var selected = tabla.api().row(evt.currentTarget).data().Tipo.TipoDocumentoId;
            //if (selected != 7 && selected != 15) {
                $("#documento-edit, #documento-delete", __moduleContainerElem).removeClass("disabled");
            //}
        }
    }
    function loadDocumentos(selected) {
        return new Promise(function (resolve) {
            showLoading();
            var id = (selected || { id_documento: 0 }).id_documento;
            $.ajax({
                url: BASE_URL + "Documento/DatosDocumento",
                method: "GET",
                data: { id: id, esMantenedorParcelario: true },
                dataType: "html",
                success: function (html) {
                    $(document).off("documentoGuardado").one("documentoGuardado", function (evt) {
                        resolve(evt.documento);
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