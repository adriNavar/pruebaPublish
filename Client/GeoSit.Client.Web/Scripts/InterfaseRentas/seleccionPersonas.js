$(function () {
    var currentIndex = -1,
        mainDialog = $("#modal-window-seleccion-personas");

    if (SeleccionPersonas.model != null && SeleccionPersonas.targetCount > 0) {
        $("#tblSeleccionPersonas_Grid").dataTable({
            scrollX: false,
            scrollY: "300px",
            scrollCollapse: false,
            info: false,
            paging: false,
            searching: false,
            processing: false,
            rowId: "Codigo",
            order: [[1, "asc"]],
            language: { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
            columns: [
                { name: "Codigo", data: "Codigo", className: "hide" },
                { name: "Nombre", data: "Nombre" },
                { name: "TipoDocumento", data: "TipoDocumento" },
                { name: "NroDocumento", data: "NroDocumento" }
            ]
        });
        $("#tblSeleccionPersonas_Grid tbody").on("click", "tr", function () {
            $("#tblSeleccionPersonas_Grid tbody tr.selected").removeClass("selected");
            $("#pnlSeleccionPersonas_Status").css("opacity", "0").addClass("hidden");
            $(this).toggleClass("selected");            
        });
    }

    $("#btnSeleccionPersonas_Siguiente").on("click", function () {
        if ($(this).hasClass("btn-deshabilitado")) return;
        processCurrent(processNext);
    });

    $("#btnSeleccionPersonas_Aceptar").on("click", function () {
        if ($(this).hasClass("btn-deshabilitado")) return;
        processCurrent(accept);
    });

    $("#btnSeleccionPersonas_Cancelar").on("click", function () {
        SeleccionPersonas = null;
        mainDialog.modal("hide");
    });

    mainDialog.modal("show");    
    hideLoading();
    processNext();

    function processCurrent(callback) {
        var handled = false;
        if (currentIndex < SeleccionPersonas.model.length && currentIndex > -1) {
            var persona = $("#tblSeleccionPersonas_Grid").dataTable().api().row(".selected").data();
            if (persona != null) {
                SeleccionPersonas.model[currentIndex].ResponsableFiscal.CodSistemaTributario = persona.Codigo;
                SeleccionPersonas.model[currentIndex].ResponsableFiscal.TipoPersonaList = [];

                if ($.isFunction(GeoSIT.SeleccionPersonas.updateCallback)) {
                    GeoSIT.SeleccionPersonas.updateCallback(SeleccionPersonas.model[currentIndex].ResponsableFiscal, true).done(callback);
                }
            } else {
                $("#pnlSeleccionPersonas_Status").removeClass("hidden").animate({ "opacity": "1" });
            }
            handled = true;
        }
        if (!handled) callback();
    }

    function processNext() {
        while (currentIndex < SeleccionPersonas.model.length) {
            currentIndex += 1;
            if (currentIndex < SeleccionPersonas.model.length &&
                SeleccionPersonas.model[currentIndex].Personas != null) {
                break;
            }
        }
        if (currentIndex < SeleccionPersonas.model.length && currentIndex > -1) {
            var table = $("#tblSeleccionPersonas_Grid").dataTable().api(),
                tbody = $(table.table().body()).unhighlight();

            $("#stSelecctionPersonas_Nombre").text(SeleccionPersonas.model[currentIndex].ResponsableFiscal.NombreCompleto);

            table.clear();
            table.rows.add(SeleccionPersonas.model[currentIndex].Personas);
            table.draw();

            var nameParts = SeleccionPersonas.model[currentIndex].ResponsableFiscal.NombreCompleto.split(/[\s,;]+/);            
            for (var i = 0; i < nameParts.length; i++) {
                tbody.highlight(nameParts[i]);
            }
        }
        if (currentIndex >= SeleccionPersonas.model.length) {

            $("#stSelecctionPersonas_Prompt").text("").hide();
            $("#stSelecctionPersonas_Nombre").text("").hide();

            $("#pnlSeleccionPersonas_Info").removeClass("hidden");
            $("#pnlSeleccionPersonas_Grid").hide();

            $("#btnSeleccionPersonas_Siguiente").addClass("btn-deshabilitado").hide();
            $("#btnSeleccionPersonas_Aceptar").removeClass("btn-deshabilitado").addClass("cursor-pointer");
        }
    }

    function accept() {
        GeoSIT.SeleccionPersonas.accepted = true;
        if ($.isFunction(GeoSIT.SeleccionPersonas.acceptCallback)) {
            GeoSIT.SeleccionPersonas.acceptCallback();
        }
        mainDialog.modal("hide");
    }
});