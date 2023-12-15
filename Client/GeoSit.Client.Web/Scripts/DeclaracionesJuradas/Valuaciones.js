var valuaciones = {
    currentRowIndex: -1,
    table: null,
    getCurrentRow: function () {
        return valuaciones.table.row(valuaciones.currentRowIndex).data();
    },
    setRow: function (row) {
        if (valuaciones.currentRowIndex === valuaciones.table.row(row).index()) {
            valuaciones.disableSelect();
        }
        else {
            valuaciones.enableSelect(row);
        }
    },
    getDatos: function (row) {
        if (valuaciones.tramiteId === $(row).data('id')) {
            valuaciones.tramiteId = 0;
            $('#btnEditar, #btnVer').addClass('disabled');
        }
        else {
            valuaciones.tramiteId = $(row).data('id');
            $('#btnEditar, #btnVer').removeClass('disabled');
        }
    },
    enableSelect: function (row) {
        valuaciones.currentRowIndex = valuaciones.table.row(row).index();
        $('#btnVer').removeClass('disabled');

        if (valuaciones.getCurrentRow().Vigente) {
            $('#btnEditar, #btnEliminar').removeClass('disabled');
        }
        else {
            $('#btnEditar, #btnEliminar').addClass('disabled');
        }
    },
    disableSelect: function () {
        valuaciones.currentRowIndex = -1;
        $('#btnEditar, #btnVer, #btnEliminar').addClass('disabled');
    },
    ajustarScrollBars: function () {
        $(".valuaciones-content").getNiceScroll().resize();
        $(".valuaciones-content").getNiceScroll().show();
    },
    ajustarmodal: function () {
        var altura = $(window).height() - 180;
        $(".valuaciones-body").css({ "height": altura });
        $(".valuaciones-content").css({ "height": altura, "overflow": "hidden" });
        valuaciones.ajustarScrollBars();
    },
    mostrarMensaje: function (mensaje, titulo, tipo, callback) {
        $('#TituloInfoValuaciones', '#ModalInfoValuaciones').html(titulo);
        $('#DescripcionInfoValuaciones', '#ModalInfoValuaciones').html(mensaje);
        $("[role='alert']", "#ModalInfoValuaciones").removeClass("alert-danger alert-success alert-info alert-warning").addClass(tipo);
        $(".modal-footer", "#ModalInfoValuaciones").hide();
        if (tipo === "alert-info") {
            $(".modal-footer", "#ModalInfoValuaciones").show();
        }

        if ($.isFunction(callback)) {
            $('#btnInfoValuacionesOK').off('click').on('click', callback);
        }
        else {
            $('#btnInfoValuacionesOK').unbind('click');
        }

        $("#ModalInfoValuaciones").modal('show');
    },
    mostrarMensajeError: function (mensajes, titulo, error, callback) {
        valuaciones.mostrarMensaje(mensajes, titulo, (error || false ? "alert-danger" : "alert-warning"), callback);
    },
    mostrarMensajeGeneral: function (mensajes, titulo, confirmacion, callback) {
        valuaciones.mostrarMensaje(mensajes, titulo, (!!confirmacion ? "alert-info" : "alert-success"), callback);
    },
    mostrarMensajeAprobacion: function (mensajes, titulo, aprobacion, callback) {
        valuaciones.mostrarMensaje(mensajes, titulo, (!!aprobacion ? "alert-success" : "alert-danger"), callback);
    },
    getValuaciones: function () {
        valuaciones.disableSelect();
        showLoading();
        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/GetValuaciones",
            data: { idUnidadTributaria: + $('#IdUnidadTributaria').val() },
            dataType: 'json',
            type: 'GET',
            success: function (valuacion) {
                valuaciones.table.clear().draw();
                valuaciones.table.rows.add(valuacion).draw();
            },
            error: function (error) {
                valuaciones.mostrarMensajeError(error.responseText, "Recuperar Valuaciones", true);
            },
            complete: hideLoading
        });
    },
    getValuacionesHeader: function () {
        showLoading();
        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/GetValuacionesHeader",
            data: { idUnidadTributaria: + $('#IdUnidadTributaria').val() },
            dataType: 'json',
            type: 'GET',
            success: function (result) {

                if (result.ValuacionVigente) {

                    var formatter = new Intl.NumberFormat('en-US', {
                        style: 'currency',
                        currency: 'USD',
                    });

                    if (result.VigenciaValorTierra) {
                        $('#valuacionPanel #VigenciaValorTierra').val(FormatFechaHora(parseJsonDate(result.VigenciaValorTierra), false));
                    }

                    if (result.VigenciaValorMejoras) {
                        $('#valuacionPanel #VigenciaValorMejoras').val(FormatFechaHora(parseJsonDate(result.VigenciaValorMejoras), false));
                    }

                    if (result.VigenciaValorFiscalTotal) {
                        $('#valuacionPanel #VigenciaValorFiscalTotal').val(FormatFechaHora(parseJsonDate(result.VigenciaValorFiscalTotal), false));
                    }

                    $('#valuacionPanel #ValorTierra').val(formatter.format(result.ValorTierra));
                    $('#valuacionPanel #ValorMejoras').val(formatter.format(result.ValorMejoras));
                    $('#valuacionPanel #ValorFiscalTotal').val(formatter.format(result.ValorFiscalTotal));
                    $('#valuacionPanel #UltimoDecretoAplicado').val(result.UltimoDecretoAplicado);
                    $('#valuacionPanel #ListaDecretos').val(result.ListaDecretos);
                    $('#valuacionPanel #Tramite').val(result.Tramite);
                }
                else {
                    $('#valuacionPanel #VigenciaValorTierra').val('');
                    $('#valuacionPanel #VigenciaValorMejoras').val('');
                    $('#valuacionPanel #VigenciaValorFiscalTotal').val('');

                    $('#valuacionPanel #ValorTierra').val('');
                    $('#valuacionPanel #ValorMejoras').val('');
                    $('#valuacionPanel #ValorFiscalTotal').val('');
                    $('#valuacionPanel #UltimoDecretoAplicado').val('');
                    $('#valuacionPanel #ListaDecretos').val('');
                    $('#valuacionPanel #Tramite').val('');
                }
            },
            error: function (error) {
                valuaciones.mostrarMensajeError(error.responseText, "Error al obtener los valores de la cabecera", true);
            },
            complete: hideLoading
        });
    },
    deleteValuacion: function (idValuacion) {
        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/DeleteValuacion",
            data: { idValuacion: idValuacion },
            dataType: 'json',
            type: 'POST',
            success: function (result) {
                if (result.success) {
                    valuaciones.getValuaciones();
                    valuaciones.getValuacionesHeader();
                    window.dispatchEvent(new CustomEvent("nueva-valuacion"));
                }
                else {
                    valuaciones.mostrarMensajeError(result.message, "Eliminar Valuación", true);
                }
            },
            error: function (error) {
                valuaciones.mostrarMensajeError([error.responseText], "Recuperar Valuaciones", true);
            },
            complete: hideLoading
        });
    },
    init: function () {
        valuaciones.table = $("#Grilla").DataTable({
            dom: "<'row'<'col-sm-12'f>><'row remove-margin text-right'<'col-xs-11 remove-padding leyenda'><'col-xs-1 remove-padding switcher'<'row toggle-activos'>>>tr<'row'<'col-sm-12'p>>",
            destroy: true,
            language: {
                url: BASE_URL + "Scripts/dataTables.spanish.txt",
                "decimal": ".",
                "thousands": ","
            },
            order: [3, 'desc'],
            data: [],
            columns: [
                { data: "IdValuacion", visible: false },
                { title: "FechaDesde", data: "FechaDesde", visible: false },
                { title: "Fecha", data: "Fecha", orderable: true },
                { title: 'Vigente', data: "Vigente", visible: false },
                { title: "Valor Tierra", data: "ValorTierra", orderable: true, render: $.fn.dataTable.render.number(",", ".", 2, '$'), "defaultContent": "-" },
                { title: "Valor Mejoras", data: "ValorMejoras", orderable: true, render: $.fn.dataTable.render.number(",", ".", 2, '$'), "defaultContent": "-" },
                { title: "VFT", data: "VFT", orderable: true, render: $.fn.dataTable.render.number(",", ".", 2, '$'), "defaultContent": "-" },
                { title: "Superficie", data: "Superficie", orderable: true, "defaultContent": "-" },
                { title: "Decreto", data: "Decreto", orderable: true, "defaultContent": "-" },
                { title: "Trámite", data: "Tramite", orderable: true, "defaultContent": "-" }
            ],
            createdRow: function (row, data) {
                $(row).data('id', data.Tramite);
                $(row).addClass('cursor-pointer');
            },
            initComplete: function (options) {
                $(this).dataTable().api().columns.adjust();
                $(options.nTBody).off("click", "tr");
                $(options.nTBody).on("click", "tr", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    $(this).siblings().removeClass('selected');
                    $(this).toggleClass('selected');
                    valuaciones.setRow(this);
                });
            }
        });

        valuaciones.getValuaciones();
        valuaciones.getValuacionesHeader();
        $(".valuaciones-content").niceScroll(getNiceScrollConfig());
        $(window).resize(valuaciones.ajustarmodal);


        $('#btnAgregar').click(function () {
            $.ajax({
                url: BASE_URL + 'DeclaracionesJuradas/GetValuacion',
                data: { idValuacion: 0, idUnidadTributaria: $('#IdUnidadTributaria').val() },
                dataType: 'html',
                type: 'POST',
                success: function (data) {
                    $("#contenido-formulario").html(data);
                    $('#modalValuacion').modal('show');
                },
                error: function (error) {
                    valuaciones.mostrarMensajeError(error.responseText, "Recuperar Valuación", true);
                },
                complete: hideLoading
            });
        });

        $('#btnEditar').click(function () {
            showLoading();
            var currentRow = valuaciones.getCurrentRow();
            $.ajax({
                url: BASE_URL + 'DeclaracionesJuradas/GetValuacion',
                data: { idValuacion: currentRow.IdValuacion, idUnidadTributaria: $('#IdUnidadTributaria').val() },
                dataType: 'html',
                type: 'POST',
                success: function (data) {
                    $("#contenido-formulario").html(data);
                    $('#modalValuacion').modal('show');
                },
                error: function (error) {
                    valuaciones.mostrarMensajeError(error.responseText, "Recuperar Valuación", true);
                },
                complete: hideLoading
            });
        });

        $('#btnVer').click(function () {
            showLoading();
            var currentRow = valuaciones.getCurrentRow();
            $.ajax({
                url: BASE_URL + 'DeclaracionesJuradas/GetValuacion',
                data: { idValuacion: currentRow.IdValuacion, idUnidadTributaria: $('#IdUnidadTributaria').val(), ReadOnly: true },
                dataType: 'html',
                type: 'POST',
                success: function (data) {
                    $("#contenido-formulario").html(data);
                    $('#modalValuacion').modal('show');
                },
                error: function (jqXHR, exception) {
                    valuaciones.mostrarMensajeError(error.responseText, "Recuperar Valuación", true);
                },
                complete: hideLoading
            });
        });

        $('#btnEliminar').on('click', function () {
            valuaciones.mostrarMensajeGeneral("¿Desea eliminar la valuación seleccionada?", "Confirmación", true, function () {
                valuaciones.deleteValuacion(valuaciones.getCurrentRow().IdValuacion);
            });
        });

        $('#btnRevaluacion').click(function () {
            showLoading();
            $.ajax({
                url: BASE_URL + 'DeclaracionesJuradas/Revaluacion',
                dataType: 'html',
                type: 'GET',
                success: function (data) {
                    $("#contenido-formulario").html(data);
                    $('#modalRevaluacion').modal('show');
                },
                error: function (error) {
                    valuaciones.mostrarMensajeError(error.responseText, "Revaluación", true);
                },
                complete: hideLoading
            });
        });

        $('#btnImprimirHistoricoValuaciones').click(function () {
            showLoading();
            $.ajax({
                url: BASE_URL + "Valuacion/GenerarHistoricoValuaciones",
                data: { idUnidadTributaria: $('#IdUnidadTributaria').val() },
                type: 'POST',
                success: function () {
                    window.open(BASE_URL + "Valuacion/AbrirReporte");
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                },
                complete: hideLoading
            });
        });

        $('#modalDDJJValuaciones').one('shown.bs.modal', function () {
            valuaciones.ajustarmodal();
            hideLoading();
        });

        $("#modalDDJJValuaciones").modal("show");
    }
};
