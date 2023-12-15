var declaracionesJuradas = {
    currentRowIndex: -1,
    currentNoVigentesRowIndex: -1,
    currentValuacionHistoricaRowIndex: -1,
    tramiteId: 0,
    esTemporal: false,
    tableDDJJ: null,
    tableDDJJNoVigentes: null,
    tableValuacionesHistoricas: null,
    getDDJJCurrentRow: function () {
        return declaracionesJuradas.tableDDJJ.row(declaracionesJuradas.currentRowIndex).data();
    },
    getDDJJNoVigentesCurrentRow: function () {
        return declaracionesJuradas.tableDDJJNoVigentes.row(declaracionesJuradas.currentNoVigentesRowIndex).data();
    },
    getValuacionHistoricaCurrentRow: function () {
        return declaracionesJuradas.tableValuacionesHistoricas.row(declaracionesJuradas.currentValuacionHistoricaRowIndex).data();
    },
    setDDJJRow: function (row) {
        if (declaracionesJuradas.currentRowIndex === declaracionesJuradas.tableDDJJ.row(row).index()) {
            declaracionesJuradas.disableSelect();
        }
        else {
            declaracionesJuradas.enableSelect(row);
        }
    },
    setDDJJNoVigentesRow: function (row) {
        if (declaracionesJuradas.currentNoVigentesRowIndex === declaracionesJuradas.tableDDJJNoVigentes.row(row).index()) {
            declaracionesJuradas.disableNoVigenteSelect();
        }
        else {
            declaracionesJuradas.enableNoVigenteSelect(row);
        }
    },
    setValuacionHistoricaRow: function (row) {
        if (declaracionesJuradas.currentValuacionHistoricaRowIndex === declaracionesJuradas.tableValuacionesHistoricas.row(row).index()) {
            declaracionesJuradas.disableValuacionHistoricaSelect();
        }
        else {
            declaracionesJuradas.enableValuacionHistoricaSelect(row);
        }
    },
    getDatos: function (row) {
        if (declaracionesJuradas.tramiteId === $(row).data('id')) {
            declaracionesJuradas.tramiteId = 0;
            $('#btnEditar, #btnVer, #btnImprimir').addClass('disabled');
        }
        else {
            declaracionesJuradas.tramiteId = $(row).data('id');
            $('#btnEditar, #btnVer, #btnImprimir').removeClass('disabled');
        }
    },
    enableSelect: function (row) {
        var dataRow = declaracionesJuradas.tableDDJJ.row(row);
        declaracionesJuradas.currentRowIndex = dataRow.index();
        $('#btnEditar, #btnVer, #btnImprimir').removeClass('disabled');
        $('#btnBorrar').addClass('disabled');
        if ([1, 2].includes(dataRow.data().IdVersion)) {
            $('#btnBorrar').removeClass('disabled');
        }
    },
    enableNoVigenteSelect: function (row) {
        var dataRow = declaracionesJuradas.tableDDJJNoVigentes.row(row);
        declaracionesJuradas.currentNoVigentesRowIndex = dataRow.index();
        $('#btnVerNoVigente, #btnImprimirNoVigente').removeClass('disabled');
    },
    enableValuacionHistoricaSelect: function (row) {
        const dataRow = declaracionesJuradas.tableValuacionesHistoricas.row(row);
        declaracionesJuradas.currentValuacionHistoricaRowIndex = dataRow.index();
        $('#btnVerValuacionHistorica').removeClass('disabled');
    },
    disableSelect: function () {
        declaracionesJuradas.currentRowIndex = -1;
        $('#btnEditar, #btnVer, #btnImprimir, #btnBorrar').addClass('disabled');
    },
    disableNoVigenteSelect: function () {
        declaracionesJuradas.currentNoVigentesRowIndex = -1;
        $('#btnVerNoVigente, #btnImprimirNoVigente').addClass('disabled');
    },
    disableValuacionHistoricaSelect: function () {
        declaracionesJuradas.currentValuacionHistoricaRowIndex = -1;
        $('#btnVerValuacionHistorica').addClass('disabled');
    },
    ajustarScrollBars: function () {
        $(".ddjj-content").getNiceScroll().resize();
        $(".ddjj-content").getNiceScroll().show();
    },
    ajustarmodal: function () {
        var altura = $(window).height() - 180;
        $(".ddjj-body").css({ "height": altura });
        $(".ddjj-content").css({ "height": altura, "overflow": "hidden" });
        declaracionesJuradas.ajustarScrollBars();
    },
    mostrarMensaje: function (mensaje, titulo, tipo, callback) {
        $('#TituloInfoDeclaracionesJuradas', '#ModalInfoDeclaracionesJuradas').html(titulo);
        $('#DescripcionInfoDeclaracionesJuradas', '#ModalInfoDeclaracionesJuradas').html(mensaje);
        $("[role='alert']", "#ModalInfoDeclaracionesJuradas").removeClass("alert-danger alert-success alert-info alert-warning").addClass(tipo);
        $(".modal-footer", "#ModalInfoDeclaracionesJuradas").hide();

        if (typeof callback === "function") {
            $(".modal-footer", "#ModalInfoDeclaracionesJuradas").show();
            $('#btnInfoDeclaracionesJuradasOK').off('click').on('click', callback);
        }
        else {
            $('#btnInfoDeclaracionesJuradasOK').unbind('click');
        }

        $("#ModalInfoDeclaracionesJuradas").modal('show');
    },
    mostrarMensajeError: function (mensajes, titulo, error, callback) {
        declaracionesJuradas.mostrarMensaje(mensajes, titulo, (error || false ? "alert-danger" : "alert-warning"), callback);
    },
    mostrarMensajeGeneral: function (mensajes, titulo, confirmacion, callback) {
        declaracionesJuradas.mostrarMensaje(mensajes, titulo, (!!confirmacion ? "alert-warning" : "alert-success"), callback);
    },
    removeItemFromArr: function (arr, item) {
        var i = arr.indexOf(item);
        arr.splice(i, 1);
    },
    reloadCroquisClaseParcela: function (idDeclaracionJurada) {
        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/GetClaseParcelaByIdDDJJ",
            data: { idDeclaracionJurada: idDeclaracionJurada },
            type: "GET",
            success: (idClaseParcela) => {
                $.ajax({
                    url: BASE_URL + "DeclaracionesJuradas/GetCroquisClaseParcela",
                    data: { id: idClaseParcela },
                    type: "POST",
                    success: (croqui) => {
                        $("#Croquis_div").removeClass("hidden");
                        $("#clase_div").removeClass("col-xs-12");
                        $("#clase_div").addClass("col-xs-6");
                        document.getElementById("croquisClaseParcela").src = croqui;
                    },
                });
            },
        });
    },
    reloadDeclaracionesJuradas: function () {
        declaracionesJuradas.disableSelect();
        declaracionesJuradas.tableDDJJ.clear().draw();
        declaracionesJuradas.tableDDJJ.ajax.reload();
    },
    reloadDeclaracionesJuradasNoVigentes: function () {
        declaracionesJuradas.disableNoVigenteSelect();
        declaracionesJuradas.tableDDJJNoVigentes.clear().draw();
        declaracionesJuradas.tableDDJJNoVigentes.ajax.reload();
    },
    reloadValuacionesHistoricas: function () {
        declaracionesJuradas.disableValuacionHistoricaSelect();
        declaracionesJuradas.tableValuacionesHistoricas.clear().draw();
        declaracionesJuradas.tableValuacionesHistoricas.ajax.reload();
    },
    getDeclaracionesJuradasHeader: function (idUT) {
        showLoading();
        idUT = idUT || Number($('#IdUnidadTributaria').val());
        $.ajax({
            url: `${BASE_URL}DeclaracionesJuradas/DeclaracionesJuradasHeader?IdUnidadTributaria=${idUT}`,
            dataType: 'json',
            type: 'GET',
            success: function (result) {
                if (result.ValuacionVigente) {
                    var formatter = new Intl.NumberFormat('en-US', {
                        style: 'currency',
                        currency: 'USD',
                    });
                    var date;
                    if (result.VigenciaValorTierra) {
                        date = new Date(parseInt(result.VigenciaValorTierra.substr(6)));
                        $('#tabValuacionVigente #VigenciaValorTierra').val((((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '/' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '/' + date.getFullYear()));
                    }

                    if (result.VigenciaValorMejoras) {
                        date = new Date(parseInt(result.VigenciaValorMejoras.substr(6)));
                        $('#tabValuacionVigente #VigenciaValorMejoras').val((((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '/' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '/' + date.getFullYear()));
                    }

                    if (result.VigenciaValorFiscalTotal) {
                        date = new Date(parseInt(result.VigenciaValorFiscalTotal.substr(6)));
                        $('#tabValuacionVigente #VigenciaValorFiscalTotal').val((((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '/' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '/' + date.getFullYear()));
                    }

                    $('#tabValuacionVigente #ValorTierra').val(formatter.format(result.ValorTierra));
                    $('#tabValuacionVigente #ValorMejoras').val(formatter.format(result.ValorMejoras));
                    $('#tabValuacionVigente #ValorFiscalTotal').val(formatter.format(result.ValorFiscalTotal));
                    $('#tabValuacionVigente #UltimoDecretoAplicado').val(result.UltimoDecretoAplicado);
                    $('#tabValuacionVigente #ListaDecretos').val(result.ListaDecretos);
                } else {
                    $('#tabValuacionVigente #VigenciaValorTierra').val('');
                    $('#tabValuacionVigente #VigenciaValorMejoras').val('');
                    $('#tabValuacionVigente #VigenciaValorFiscalTotal').val('');

                    $('#tabValuacionVigente #ValorTierra').val('');
                    $('#tabValuacionVigente #ValorMejoras').val('');
                    $('#tabValuacionVigente #ValorFiscalTotal').val('');
                    $('#tabValuacionVigente #UltimoDecretoAplicado').val('');
                    $('#tabValuacionVigente #ListaDecretos').val('');
                }

                hideLoading();
            },
            error: function (error) {
                hideLoading();
                declaracionesJuradas.mostrarMensajeError(error.responseText, "Error al obtener los valores de la cabecera", true);
            }
        });
    },
    informarCambio: function () {
        $('#modalDDJJ')
            .off('hide.bs.modal')
            .one('hide.bs.modal', function () {
                setTimeout(function () {
                    window.dispatchEvent(new CustomEvent("cambio-ddjj"));
                }, 100);
            });
    },
    reloadData: function () {
        declaracionesJuradas.getDeclaracionesJuradasHeader();
        declaracionesJuradas.reloadDeclaracionesJuradas();
        declaracionesJuradas.reloadDeclaracionesJuradasNoVigentes();
        declaracionesJuradas.reloadValuacionesHistoricas();
    },
    reloadDataValuacion: function () {
        declaracionesJuradas.getDeclaracionesJuradasHeader();
        declaracionesJuradas.reloadValuacionesHistoricas();
    },
    initGrid: function (grid, options) {
        const debounce = (func) => setTimeout(func, 100);
        const gridOptions = {
            ...{
                destroy: true,
                dom: "<'row'<'col-sm-12'f>><'row remove-margin text-right'<'col-xs-11 remove-padding leyenda'><'col-xs-1 remove-padding switcher'<'row toggle-activos'>>>rt<'row'<'col-sm-12'p>>",
                processing: true,
                ordering: true,
                language: { url: `${BASE_URL}Scripts/dataTables.spanish.txt` },
                searchDelay: 500,
                order: [2, 'desc'],
            }, ...options
        };
        return $(`#${grid}`, ".ddjj-content")
            .DataTable(gridOptions)
            .on('search.dt', debounce.bind(null, declaracionesJuradas.ajustarScrollBars));
    },
    init: function () {
        const idUnidadTributaria = Number($('#IdUnidadTributaria').val());
        const verDDJJ = (currentRow) => {
            showLoading();
            $.ajax({
                url: `${BASE_URL}DeclaracionesJuradas/GetFormulario`,
                data: {
                    IdDeclaracionJurada: currentRow.IdDeclaracionJurada,
                    IdVersion: currentRow.IdVersion,
                    IdUnidadTributaria: idUnidadTributaria,
                    PartidaInmobiliaria: $('#partidaInmobiliaria').text(),
                    ReadOnly: true
                },
                dataType: 'html',
                type: 'POST',
                success: function (data) {
                    if (currentRow.IdVersion === 4) {
                        declaracionesJuradas.reloadCroquisClaseParcela(currentRow.IdDeclaracionJurada);
                    }
                    $("#contenido-formulario").html(data);
                    $('#modalFormulario').modal('show');
                    hideLoading();
                },
                error: function (error) {
                    declaracionesJuradas.mostrarMensajeError(error.responseText, "Recuperar Declaración Jurada", true);
                    hideLoading();
                }
            });
        };
        const imprimirDDJJ = (currentRow) => {
            showLoading();
            $.ajax({
                url: `${BASE_URL}DeclaracionesJuradas/GenerarInformeDDJJ`,
                data: { idDeclaracionJurada: currentRow.IdDeclaracionJurada, tipoDDJJ: currentRow.TipoDDJJ, idTramite: currentRow.IdTramite },
                type: 'POST',
                success: function () {
                    window.open(`${BASE_URL}DeclaracionesJuradas/AbrirInformeDDJJ`);
                },
                error: function () {
                    declaracionesJuradas.mostrarMensajeError("No se puede generar el informe para la declaración jurada.", "Imprimir Declaración Jurada", true);
                },
                complete: hideLoading
            });
        };

        const ddjjVigentesOpts = {
            ajax: `${BASE_URL}DeclaracionesJuradas/GetDeclaracionesJuradas?IdUnidadTributaria=${idUnidadTributaria}`,
            columns: [
                { title: "Tipo", data: "Tipo" },
                { title: "Vigencia Desde", data: "VigenciaDesde", render: { _: "display", sort: "timestamp" } },
                { title: "Versión", data: "Version" },
                { title: "Trámite", data: "Tramite" },
                { title: "Origen", data: "Origen" }
            ],
            createdRow: function (row, data) {
                $('td:last-of-type', row).addClass((data["Origen"] || "npi").toLowerCase().replace("ó", "o"));
                $(row).data('id', data.Tramite);
            },
            initComplete: function (options) {
                $(this).dataTable().api().columns.adjust();
                declaracionesJuradas.ajustarScrollBars();
                $(options.nTBody)
                    .off("click", "tr")
                    .on("click", "tr", function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        $(this).toggleClass('selected')
                            .siblings().removeClass('selected');
                        declaracionesJuradas.setDDJJRow(this);
                    });
            }
        };
        declaracionesJuradas.tableDDJJ = declaracionesJuradas.initGrid("Grilla", ddjjVigentesOpts);

        const ddjjNoVigentesOpts = {
            ajax: `${BASE_URL}DeclaracionesJuradas/GetDeclaracionesJuradasE1E2NoVigentes?IdUnidadTributaria=${idUnidadTributaria}`,
            columns: [
                { title: "Tipo", data: "Tipo" },
                { title: "Vigencia Desde", data: "VigenciaDesde", render: { _: "display", sort: "timestamp" } },
                { title: "Vigencia Hasta", data: "VigenciaHasta", render: { _: "display", sort: "timestamp" } },
                { title: "Versión", data: "Version" },
                { title: "Trámite", data: "Tramite" },
                { title: "Origen", data: "Origen" }
            ],
            createdRow: function (row, data) {
                $('td:last-of-type', row).addClass((data["Origen"] || "npi").toLowerCase().replace("ó", "o"));
                $(row).data('id', data.Tramite);
            },
            initComplete: function (options) {
                $(this).dataTable().api().columns.adjust();
                declaracionesJuradas.ajustarScrollBars();
                $(options.nTBody)
                    .off("click", "tr")
                    .on("click", "tr", function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        $(this).toggleClass('selected')
                            .siblings().removeClass('selected');
                        declaracionesJuradas.setDDJJNoVigentesRow(this);
                    });
            }
        }
        declaracionesJuradas.tableDDJJNoVigentes = declaracionesJuradas.initGrid("GrillaNoVigentes", ddjjNoVigentesOpts);

        const dateTimeRenderer = (data, type) => {
            const value = parseJsonDate(data);
            if (type === "display" || type === "filter") {
                return FormatFechaHora(value, false);
            }
            return value;
        };
        const valuacionesHistoricasOpts = {
            ajax: `${BASE_URL}DeclaracionesJuradas/GetValuacionesHistoricas?idUnidadTributaria=${idUnidadTributaria}`,
            order: [1, 'desc'],
            columns: [
                { title: "Desde", data: "VigenciaDesde", render: dateTimeRenderer },
                { title: "Hasta", data: "VigenciaHasta", render: dateTimeRenderer },
                { title: "Valor Tierra", data: "ValorTierra", render: $.fn.dataTable.render.number(",", ".", 2, '$'), "defaultContent": "-" },
                { title: "Valor Mejoras", data: "ValorMejoras", render: $.fn.dataTable.render.number(",", ".", 2, '$'), "defaultContent": "-" },
                { title: "VFT", data: "VFT", render: $.fn.dataTable.render.number(",", ".", 2, '$'), "defaultContent": "-" },
                { title: "Superficie", data: "Superficie", "defaultContent": "-" },
                { title: "Decreto", data: "Decreto", "defaultContent": "-" },
                { title: "Trámite", data: "Tramite", "defaultContent": "-" }
            ],
            createdRow: function (row, data) {
                $(row).data('id', data.Tramite);
            },
            initComplete: function (options) {
                $(this).dataTable().api().columns.adjust();
                $(options.nTBody)
                    .off("click", "tr")
                    .on("click", "tr", function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        $(this).toggleClass('selected')
                            .siblings().removeClass('selected');
                        declaracionesJuradas.setValuacionHistoricaRow(this);
                    });
            }
        }
        declaracionesJuradas.tableValuacionesHistoricas = declaracionesJuradas.initGrid("GrillaValuacionesHistoricas", valuacionesHistoricasOpts);

        declaracionesJuradas.getDeclaracionesJuradasHeader(idUnidadTributaria);

        $(".ddjj-content").niceScroll(getNiceScrollConfig());
        $(window).resize(declaracionesJuradas.ajustarmodal);

        $("a[data-toggle='tab']", ".ddjj-content").on("shown.bs.tab", function () {
            declaracionesJuradas.ajustarScrollBars();
        });


        $('#btnAgregar').click(function () {
            $("#modalSeleccionFormulario").modal("show");
        });

        $('#btnEditar').click(function () {
            showLoading();
            var currentRow = declaracionesJuradas.getDDJJCurrentRow();
            $.ajax({
                url: BASE_URL + 'DeclaracionesJuradas/GetFormulario',
                data: {
                    IdDeclaracionJurada: currentRow.IdDeclaracionJurada,
                    IdVersion: currentRow.IdVersion,
                    IdUnidadTributaria: idUnidadTributaria,
                    PartidaInmobiliaria: $('#partidaInmobiliaria').text(),
                    esEdicion: true
                },
                dataType: 'html',
                type: 'POST',
                success: function (data) {
                    if (currentRow.IdVersion === 4) {
                        declaracionesJuradas.reloadCroquisClaseParcela(currentRow.IdDeclaracionJurada);
                    }
                    $("#contenido-formulario").html(data);
                    $('#modalFormulario').modal('show');
                },
                error: function () {
                    declaracionesJuradas.mostrarMensajeError("No se ha encontrado el tipo de formulario indicado", "Recuperar Declaración Jurada", true);
                },
                complete: hideLoading
            });
        });

        $("#btnImprimir").click(function () {
            imprimirDDJJ(declaracionesJuradas.getDDJJCurrentRow());
        });

        $("#btnImprimirNoVigente").click(function () {
            imprimirDDJJ(declaracionesJuradas.getDDJJNoVigentesCurrentRow());
        });

        $('#btnVer').click(function () {
            verDDJJ(declaracionesJuradas.getDDJJCurrentRow());
        });

        $('#btnVerNoVigente').click(function () {
            verDDJJ(declaracionesJuradas.getDDJJNoVigentesCurrentRow());
        });

        $('#btnBorrar').click(function () {
            const borrar = () => {
                showLoading();
                const currentRow = declaracionesJuradas.getDDJJCurrentRow();
                $.ajax({
                    type: "POST",
                    async: true,
                    url: `${BASE_URL}DeclaracionesJuradas/BajaDDJJE1E2/${currentRow.IdDeclaracionJurada}`,
                    dataType: 'json',
                    success: (resp) => {
                        $('#Grilla').DataTable().row('.selected').remove().draw(true);
                        declaracionesJuradas.reloadData();
                        if (resp.ok === false) {
                            declaracionesJuradas.mostrarMensajeError("La Mejora se dió de baja correctamente.<br />Para valuar correctamente el inmueble debe COMPLETAR las DDJJ vigentes.", "Baja de Declaración Jurada", false);
                        } else {
                            declaracionesJuradas.mostrarMensajeGeneral("La Mejora se dió de baja correctamente.", "Baja de Declaración Jurada", false);
                        }
                    },
                    error: () => {
                        declaracionesJuradas.mostrarMensajeError("No se pudo dar de baja la declaración jurada", "Baja de Declaración Jurada", true);
                    },
                    complete: hideLoading
                });
            };
            declaracionesJuradas.mostrarMensajeGeneral("Dar de baja una DDJJ de Mejora implica una revaluación completa.<br/><br/>¿Está seguro que desea continuar?", "Baja de Declaración Jurada", true, borrar);
        });

        $('#btnRevaluacion').click(function () {
            showLoading();
            $.ajax({
                url: `${BASE_URL}DeclaracionesJuradas/Revaluacion`,
                dataType: 'html',
                type: 'GET',
                success: function (data) {
                    $("#contenido-formulario").html(data);
                    $('#modalRevaluacion').modal('show');
                },
                error: function (error) {
                    declaracionesJuradas.mostrarMensajeError(error.responseText, "Revaluación", true);
                },
                complete: hideLoading
            });
        });

        $('#btnImprimirHistoricoValuaciones').click(function () {
            showLoading();
            $.ajax({
                url: `${BASE_URL}Valuacion/GenerarHistoricoValuaciones`,
                data: { idUnidadTributaria: idUnidadTributaria },
                type: 'POST',
                success: function () {
                    window.open(`${BASE_URL}Valuacion/AbrirReporte`);
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                },
                complete: hideLoading
            });
        });

        $('#btnVerValuacionHistorica').click(function () {
            showLoading();
            var currentRow = declaracionesJuradas.getValuacionHistoricaCurrentRow();
            $.ajax({
                url: `${BASE_URL}DeclaracionesJuradas/GetValuacion`,
                data: { idValuacion: currentRow.IdValuacion, idUnidadTributaria: idUnidadTributaria, ReadOnly: true },
                dataType: 'html',
                type: 'POST',
                success: function (data) {
                    $("#contenido-formulario").html(data);
                    $('#modalValuacion').modal('show');
                },
                error: function () {
                    declaracionesJuradas.mostrarMensajeError(error.responseText, "Recuperar Valuación", true);
                },
                complete: hideLoading
            });
        });

        $('#modalDDJJ').one('shown.bs.modal', function () {
            declaracionesJuradas.ajustarmodal();
            hideLoading();
        });

        $('#modalDDJJ').one('hidden.bs.modal', function () {
            $('#ddjj-container').empty();
        });

        $("#modalDDJJ").modal("show");

    }
};
//# sourceURL=ddjj.js