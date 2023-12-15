$(function () {
    var InterfaseRentas = {
        REPROCESAR_TODO: "T",
        REPROCESAR_SELECCION: "S",
        MESSAGE_TYPE_INFO: 0,
        MESSAGE_TYPE_SUCCESS: 1,
        MESSAGE_TYPE_WARNING: 2,
        MESSAGE_TYPE_ERROR: 3,
        pendientes: 0,
        procesados: 0,
        logs: []
    },
    winLogInterfaseRentas = $("#rnt_winLogInterfaseRentas"),
    tblLogInterfaseRentas = $("#rnt_tblLogInterfaseRentas");

    init();

    function init() {
        tblLogInterfaseRentas.dataTable({
            scrollX: false,
            scrollY: "400px",
            scrollCollapse: false,
            info: false,
            paging: false,
            searching: false,
            processing: false,
            rowId: "LogID",
            order: [[1, "desc"]],
            language: { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            columns: [
                { name: "Selected", orderable: false, className: "dt-body-center" },
                { name: "LogID", data: "LogID" },
                { name: "TransactionID", data: "TransactionID" },
                { name: "Fecha", data: "Fecha" },
                { name: "ParcelaID", data: "ParcelaID" },
                { name: "Partida", data: "Partida" },
                { name: "WebService", data: "WebService" },
                { name: "Operacion", data: "Operacion" },
                { name: "Resultado", data: "Resultado" },
                {
                    name: "Estado", data: "Estado", render: function (data) {
                        if (data == 1) {
                            return "OK";
                        } else if (data == -1) {
                            return "Error";
                        }
                        return "";
                    }
                }
            ],
            select: {
                style: "multi"
            },
            initComplete: function () {
                tblLogInterfaseRentas.find("input.checkbox").on({
                    click: function () {
                        rowCheckboxClicked($(this));
                    }
                });
            }
        });

        $("#rnt_chkSeleccionarTodo").on({
            click: function () {
                tblLogInterfaseRentas.find("input.checkbox").prop("checked", $(this).prop("checked"));
                actualizarUI();
            }
        });

        $("#rnt_btnReprocesarTodo").on({
            click: function () {
                reprocesar(InterfaseRentas.REPROCESAR_TODO);
            }
        });
        $("#rnt_btnReprocesarSeleccion").on({
            click: function () {
                reprocesar(InterfaseRentas.REPROCESAR_SELECCION);
            }
        });

        winLogInterfaseRentas.on("shown.bs.modal", function () {
            tblLogInterfaseRentas.dataTable().api().columns.adjust();
        });
        winLogInterfaseRentas.modal("show");
        window.setTimeout(ajustarModal, 100);
        $(window).resize(ajustarModal);
        actualizarUI();

        if ($.isFunction(hideLoading)) {
            hideLoading();
        }
    }

    function reprocesar(modo) {
        var selectedLogs, message;
        if (modo == InterfaseRentas.REPROCESAR_SELECCION) {
            selectedLogs = tblLogInterfaseRentas.dataTable().api().rows(".selected").data();
            message = "No ha seleccionado ningún registro.";
        } else if (modo == InterfaseRentas.REPROCESAR_TODO) {
            selectedLogs = $.grep(tblLogInterfaseRentas.dataTable().api().rows().data(), function (item) {
                return parseInt(item.Estado) === -1;
            });
            message = "No hay registros para procesar.";
        }
        if (selectedLogs.length == 0) {
            mostrarMensaje("Reprocesar", message, InterfaseRentas.MESSAGE_TYPE_WARNING);
            return;
        }
        iniciarReproceso(selectedLogs);
    }

    function iniciarReproceso(logs) {
        $("#rnt_pnlReprocessProgress").find(".progress-bar").css("width", "0").text("0%");
        $("#rnt_btnReprocesarTodo, #btnReprocesarSeleccion").prop("disabled", true);
        $("#rnt_pnlReprocessProgress").show();

        InterfaseRentas.totales = logs.length;
        InterfaseRentas.procesados = 0;
        InterfaseRentas.logs = logs;
        reprocesarLogs();
    }

    function finalizarReproceso() {
        $("#rnt_btnReprocesarTodo, #rnt_btnReprocesarSeleccion").prop("disabled", false);
        window.setTimeout(function () {
            $("#rnt_pnlReprocessProgress").hide();
        }, 750);
    }

    function reprocesarLogs() {
        var log = InterfaseRentas.logs.shift();
        if (log != null) {
            $.ajax({
                type: "POST",
                dataType: "json",
                url: BASE_URL + "InterfaseRentas/Reprocesar?logId=" + log.LogID,
                success: function (result) {
                    if (result) {
                        var rowSelector = "#rnt_log_" + result.LogID;
                            row = tblLogInterfaseRentas.dataTable().api().row(rowSelector),
                            newData = $.extend(row.data(), result);                        

                        row.data(newData).draw();
                        InterfaseRentas.procesados++;

                        var jqRow = $(rowSelector),
                            checkbox = jqRow.find("input.checkbox");

                        checkbox.prop("checked", jqRow.hasClass("selected"));
                        checkbox.on({ click: function () { rowCheckboxClicked(checkbox) } });

                        var progress = Math.round(InterfaseRentas.procesados / InterfaseRentas.totales * 100) + "%";
                        $("#rnt_pnlReprocessProgress").find(".progress-bar").css("width", progress).text(progress);
                    }
                    reprocesarLogs();
                },
                error: function (xhr) {
                    var mensaje = "Ha ocurrido un error durante el reprocesamiento.";
                    mostrarMensaje("Reprocesar", mensaje, InterfaseRentas.MESSAGE_TYPE_ERROR, finalizarReproceso);
                }
            });
        } else {
            window.setTimeout(function () {
                mostrarMensaje("Reprocesar", "Reprocesamiento finalizado.", InterfaseRentas.MESSAGE_TYPE_INFO);
            }, 750);
            finalizarReproceso();
        }
    }

    function mostrarMensaje(titulo, mensaje, type, callback) {
        var modalInfo = $("#rnt_ModalInfo").off("hidden.bs.modal"),
            modalHeader = modalInfo.find(".modal-header"),
            mensajeInfo = $("#rnt_MensajeInfo"),
            tituloInfo = $('#rnt_TituloInfo');

        type = type || 0;
        var clases = ["alert-info", "alert-success", "alert-warning", "alert-danger"];
        for (var i = 0; i < clases.length; i++) {
            modalHeader.removeClass(clases[i]);
            mensajeInfo.removeClass(clases[i]);
        }
        tituloInfo.html(titulo);
        $('#rnt_DescripcionInfo').html(mensaje);

        modalHeader.addClass(clases[type]);
        mensajeInfo.addClass(clases[type]);

        if ($.isFunction(callback)) {
            modalInfo.on("hidden.bs.modal", callback);
        }
        modalInfo.modal('show');
    }

    function rowCheckboxClicked(checkbox ) {
        var tableRow = checkbox.closest("tr"),
            className = "selected";

        if (checkbox.prop("checked")) {
            tableRow.addClass(className);
        } else {
            tableRow.removeClass(className);
        }
        actualizarUI();
    }

    function ajustarModal() {
        var modalHeight = $(window).height() - 220;
        if (modalHeight >= 200) {
            var tableHeight = modalHeight - 110;
            $("#rnt_winLogInterfaseRentas").find(".modal-body").height(modalHeight);
            $("#rnt_tblLogInterfaseRentas").closest(".dataTables_scrollBody").height(tableHeight);
        }
    }

    function actualizarUI() {
        var seleccionados = 0;
        tblLogInterfaseRentas.find("input.checkbox").each(function() {
            var elem = $(this);
            if (elem.is(":checked")) {
                elem.closest("tr").addClass("selected");
                seleccionados++;
            } else {
                elem.closest("tr").removeClass("selected");
            }
        });        

        $("#rnt_btnReprocesarTodo").prop("disabled", false);
        $("#rnt_btnReprocesarSeleccion").prop("disabled", seleccionados == 0);
    }
});