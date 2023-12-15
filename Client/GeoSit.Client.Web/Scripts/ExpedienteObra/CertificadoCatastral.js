$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-informe-tramites').on('shown.bs.modal', function (e) {
    ajustarmodal();
    //ajustarScrollBars();
    hideLoading();
});

function init() {
    $('#soloFecha').datepicker(getDatePickerConfig({ minView: 2, startView: 4 }));

    $(".certificadocatastral-content").niceScroll(getNiceScrollConfig());

    $("#aceptar-informe-tramites").click(function () {

        /*if (!$('#numero').val()) {
            alerta('Error', 'Debe ingresar un número de certificado.', 3);
        }
        else*/ if (!$('#fechaemision').val()) {
            alerta('Error', 'Debe ingresar la fecha de emisión.', 3);
        }
        else if (!$('#motivo').val()) {
            alerta('Error', 'Debe ingresar un motivo.', 3);
        }
        else if (!$('#vigencia').val()) {
            alerta('Error', 'Debe ingresar una vigencia.', 3);
        }
        else if (!$('#descripcion').val()) {
            alerta('Error', 'Debe ingresar la descripción.', 3);
        }
        else if (!$('#observaciones').val()) {
            alerta('Error', 'Debe ingresar las observaciones.', 3);
        }
        else if (!$('#MensuraId').val() || Number($('#MensuraId').val()) === 0) {
            alerta('Error', 'Debe ingresar una mensura.', 3);
        }
        else if (!$('#UnidadTributariaId').val() || Number($('#UnidadTributariaId').val()) === 0) {
            alerta('Error', 'Debe ingresar una unidad tributaria.', 3);
        }
        else if (!$('#SolicitanteId').val() || Number($('#SolicitanteId').val()) === 0) {
            alerta('Error', 'Debe ingresar una persona.', 3);
        }
        else {
            generarReporte();
        }
    });

    $("#ut-search").click(function () {
        buscarUnidadesTributarias(false)
            .then(function (data) {
                if (data) {
                    $("#UnidadTributariaId").val(data[1]);
                    $("#UnidadTributaria").val(data[0]);
                }
            })
            .catch(function (err) { console.log(err); });
    });
    $("#persona-search").click(function () {
        buscarPersonas(false)
            .then(function (seleccion) {
                $("#Solicitante").val(seleccion[0]);
                $("#SolicitanteId").val(seleccion[1]);
            })
            .catch(function (err) { console.log(err); });
    });
    $("#mensura-search").click(function () {
        buscarMensuras(false)
            .then(function (seleccion) {
                $("#txtMensura").val(seleccion[0]);
                $("#MensuraId").val(seleccion[1]);
            })
            .catch(function (err) { console.log(err); });
    });
    /*$("#mensuraVep-search").click(function () {
        buscarMensurasVep(false)
            .then(function (seleccion) {
                $("#txtMensuraVep").val(seleccion[0]);
                $("#MensuraVepId").val(seleccion[1]);
            })
            .catch(function (err) { console.log(err); });
    });*/

    createDataTableBusqueda();

    ajustarmodal();

    $('#modal-window-informe-tramites').modal('show');
}

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlerta = fn;
    $("#botones-modal-info").find("span:last").hide();
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $("#botones-modal-info").find("span:last").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }

    $("#MensajeInfoInspector").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoInspector").html(titulo);
    $("#DescripcionInfoInspector").html(mensaje);
    $("#ModalInfoInspector").modal('show');
    $("#botones-modal-info").find("span:last").show();
}

function GetInformacionExpedienteObra(numero) {
    var formData = {
        "numero": numero,
        "tipoBusqueda": $('input[name=tipoBusqueda]:checked').val()
    };
    showLoading();
    $.ajax({
        url: 'ExpedienteObra/GenerateInformeExpedienteObraDetallado',
        type: 'POST',
        data: formData,
        success: function (data) {
            if (data.success) {
                window.open(BASE_URL + "ExpedienteObra/GetFileInformeExpedienteObra?file=" + data.file);
            } else {
                hideLoading();
                alerta('Advertencia', data.message, 3);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoading();
            alerta('Error', errorThrown, 3);
        }
    }).always(function () {
        hideLoading();
    });

}

function GetCertificadoCatastral() {
    var formData = {
        //"numero": $("#numero").val(),
        "fechaemision": $("#fechaemision").val(),
        "motivo": $("#motivo").val(),
        "vigencia": $("#vigencia").val(),
        "descripcion": $("#descripcion").val(),
        "observaciones": $("#observaciones").val(),
        "planoMensura": $("#MensuraId").val(),
        //"planoVep": $("#MensuraVepId").val(),
        "solicitante": $("#SolicitanteId").val(),
        "partida": $("#UnidadTributariaId").val()
    };
    showLoading();
    $.ajax({
        url: BASE_URL + 'ExpedienteObra/GenerateInformeCertificadoCatastral2',
        type: 'POST',
        data: formData,
        success: function (data) {
            if (data.error) {
                alerta('Advertencia', data.message, 3);
            } else {
                //limpiar campos
                $("#fechaemision,#motivo,#vigencia,#descripcion,#observaciones,#txtMensura,#Solicitante,#UnidadTributaria").val("");
                window.open(`${BASE_URL}ExpedienteObra/GetFileInformeExpedienteObra?file=${data.file}`);
            }
        },
        error: function (_, __, errorThrown) {
            alerta('Error', errorThrown, 3);
        }
    }).always(hideLoading);
}

function generarReporte() {
    GetCertificadoCatastral();
}


function ajustarmodal() {
    var altura = $(window).height() - 190;
    $(".certificadocatastral-body").css({ "height": altura });
    $('.certificadocatastral-content').css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $(".certificadocatastral-content").getNiceScroll().resize();
    $(".certificadocatastral-content").getNiceScroll().show();
}

/*function ajustarScrollBars() {
    var temp = $(".expedienteobra-body").height();
    var outerHeight = 20;
    $('#accordion-expediente-obra .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-expediente-obra .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.expedienteobra-content').css({ "max-height": temp + 'px' });
    $('#accordion-expediente-obra').css({ "max-height": temp + 1 + 'px' });
    $(".expedienteobra-content").getNiceScroll().resize();
    $(".expedienteobra-content").getNiceScroll().show();
}*/

function refreshScrollBar() {
    setTimeout(function () {
        $(".expedienteobra-content").getNiceScroll().resize();
    }, 30);
}

function modalConfirm(title, message) {
    $("#TituloAdvertencia").text(title);
    $("#DescripcionAdvertencia").text(message);
    //$("#confirmModal").modal("show");
}

function createDataTableBusqueda() {
    $("#busqueda").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "searching": false,
        "processing": true,
        "paging": true,
        "displayLength": 10,
        dom: 'C<"clear">lBfrtip',
        "aaSorting": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { "data": "ExpedienteObraId", "className": "hide" },
            { "data": "NumeroExpediente" },
            {
                "data": "FechaExpediente", "render": function (data, type, full, meta) {
                    return formatDate(data);
                }, "iDataSort": 5
            },
            { "data": "NumeroLegajo" },
            {
                "data": "FechaLegajo", "render": function (data, type, full, meta) {
                    return formatDate(data);
                }, "iDataSort": 6
            },
            {
                "data": "FechaExpediente", "render": function (data, type, full, meta) {
                    return formatDate2(data);
                }, "className": "hide"
            },
            {
                "data": "FechaLegajo", "render": function (data, type, full, meta) {
                    return formatDate2(data);
                }, "className": "hide"
            }
        ]
    });
}

function createDataTable(tableId) {
    $("#" + tableId).DataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "dom": "rt",
        "aaSorting": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [
            {
                "targets": "no-sort",
                "orderable": false
            }
        ]//,
        //"initComplete": function (settings, json) {
        //    $("#" + tableId).dataTable().fnAdjustColumnSizing();
        //}
    });
}

function destroyDataTable(tableId) {
    $("#" + tableId).dataTable().api().destroy();
}

function accordionSearchHandler(tabname) {
    $("#collapse" + tabname).on("shown.bs.collapse", function () {
        if ($(".panel-collapse", "#accordionBusqueda").hasClass("in")) {
            $("#clear-all").removeClass("boton-deshabilitado");
        }
    });

    $("#collapse" + tabname).on("hidden.bs.collapse", function () {
        if (!$(".panel-collapse", "#accordionBusqueda").hasClass("in")) {
            $("#clear-all").addClass("boton-deshabilitado");
        }

        switch (tabname) {
            case "UT":
                $("#UnidadTributaria").val("");
                $("#UnidadTributariaId").val("0");
                break;
            case "TNE":
                $("input[name=TipoPorNumero]").filter("[value=0]").prop("checked", true);
                $("#NumeroDesde").val("");
                $("#NumeroHasta").val("");
                break;
            case "FC":
                $("input[name=TipoPorFecha]").filter("[value=0]").prop("checked", true);
                $("#FechaDesde").datepicker("clearDates");
                $("#FechaHasta").datepicker("clearDates");
                break;
            case "Persona":
                $("#Persona").val("");
                $("#PersonaId").val("0");
                break;
            case "Estado":
                $("#Estado").val("0");
                break;
        }
    });

    $("#accordion" + tabname).click(function () {
        var accordionTab = $("#collapse" + tabname);
        if (accordionTab.hasClass("in")) {
            accordionSearchTabHide("#collapse" + tabname, "#heading" + tabname);
        } else {
            accordionSearchTabShow("#collapse" + tabname, "#heading" + tabname);
        }
        setTimeout(function () {
            $(".expedienteobra-content").getNiceScroll().resize();
        }, 10);
    });
}

function unidadesInit() {
    $("#unidades tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowUnidades = null;
            unidadesEnableControls(false);
        } else {
            $("tr.selected", "#unidades tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowUnidades = $(this);

            if (!selectedRowUnidades.children().hasClass("dataTables_empty"))
                unidadesEnableControls(true);
        }
    });

    createDataTable("unidades");

    $("#unidades-insert").click(function () {
        buscarUnidadesTributarias(true)
            .then(function (seleccion) {
                if (seleccion) {
                    seleccion.forEach(function (item) {
                        $.ajax({
                            type: "POST",
                            url: BASE_URL + "UnidadTributariaExpedienteObra/GetUnidadTributaria/" + item,
                            success: function (unidad) {
                                $.ajax({
                                    type: "POST",
                                    url: BASE_URL + "UnidadTributariaExpedienteObra/Save?idUnidadTributaria=" + item,
                                    success: function (data) {
                                        if (unidad.ParcelaID)
                                            aux = '<span onclick="fnClickMasInfo(' + unidad.ParcelaID + ')" class="fa fa-th black cursor-pointer" aria-hidden="true"></span>';
                                        else
                                            aux = '<span class="fa fa-th black cursor-pointer boton-deshabilitado" aria-hidden="true"></span>';

                                        if (data.DomicilioId) {
                                            var table = $("#unidades").dataTable().api();
                                            var node = table.row.add([
                                                unidad.UnidadTributariaId,
                                                unidad.CodigoProvincial,
                                                aux
                                            ]).draw().node();
                                            $(node).find("td:first").addClass("hide");

                                            table = $("#ubicaciones").dataTable().api();
                                            node = table.row.add([
                                                data.DomicilioId,
                                                data.ViaNombre,
                                                data.numero_puerta,
                                                data.barrio,
                                                data.codigo_postal,
                                                "<span></span>",
                                                "No"
                                            ]).draw().node();
                                            $(node).find("td:first").addClass("hide");
                                            $(node).find("td:last").addClass("hide");
                                            $("#unidad-tributaria-error").addClass("hide");
                                            $("#save-all").removeClass("boton-deshabilitado");
                                        } else {
                                            errorAlert(data);
                                        }
                                    }
                                });
                            }
                        });
                    });
                }
            })
            .catch(function (err) { console.log(err); });
    });
}

function identificacionInit() {
    $.post(BASE_URL + "ExpedienteObra/ShowActas", function (result) {
        if (result !== "1") {
            $("#actas-panel-body").hide();
        }
    });

    $("#expediente-insert").click(function () {
        $("#identificacion-form")[0].reset();
        $("#datos-form")[0].reset();

        $("div[role='region']").removeClass("panel-deshabilitado");
        $("#heading-identificacion").find("a:first[aria-expanded=false]").click();

        if (selectedRowExpedientes) {
            selectedRowExpedientes.removeClass("selected");
            selectedRowExpedientes = null;
        }
        insertUpdateExpediente();

        unidadesList(true);
        tipoTramiteList(true);
        ubicacionesList(true);
        actasList(true);
        estadosList(true);

        superficiesList(true);
        serviciosList(true);
        documentosList(true);

        personasList(true);

        liquidacionesList(true);
        controlesList(true);
        observacionesList(true);

        if ($("#ActivarAutonumeracionLegajo").val() === "1") {
            $.post(BASE_URL + "ExpedienteObra/GetNumeroLegajoSiguiente", function (data) {
                $("#NumeroLegajo").val(data);
            });
        }
        $("#NumeroExpediente").inputmask('Regex', { regex: $("#ExpresionRegularExpediente").val() });
        $("#NumeroLegajo").inputmask('Regex', { regex: $("#ExpresionRegularLegajo").val() });
        $("#Chapa").inputmask('Regex', { regex: $("#ExpresionRegularChapa").val() });

    });
}

function identificacionFormContent() {
    var form = $("#identificacion-form");
    form.load(BASE_URL + "Identificacion/FormContent", function () {
        enableFormContent("identificacion-form", false);

        if ($("#ActivarAutonumeracionLegajo").val() === "1") {
            $("#NumeroLegajo").prop("readonly", true);
        }


        //Datepicker
        $("#FechaExpediente").datepicker(getDatePickerConfig({ endDate: new Date(), enableOnReadonly: false }))
            .on("changeDate", function () {
                form.data("bootstrapValidator")
                    .updateStatus("FechaExpediente", "NOT_VALIDATED", null)
                    .validateField("FechaExpediente")
                    .updateStatus("FechaLegajo", "NOT_VALIDATED", null)
                    .validateField("FechaLegajo");
            });

        $("#FechaLegajo").datepicker(getDatePickerConfig({ endDate: new Date(), enableOnReadonly: false }))
            .on("changeDate", function () {
                form.data("bootstrapValidator")
                    .updateStatus("FechaExpediente", "NOT_VALIDATED", null)
                    .validateField("FechaExpediente")
                    .updateStatus("FechaLegajo", "NOT_VALIDATED", null)
                    .validateField("FechaLegajo");
            });


        //Fix for switches
        var cmtoggle = $(".cmn-toggle", "#identificacion-form");
        $.each(cmtoggle, function (index, item) {
            $("#" + item.id).after("<label for=\"" + item.id + "\"></label>");
        });

        form.bootstrapValidator({
            framework: "bootstrap",
            excluded: [":disabled"],
            fields: {
                NumeroExpediente: {
                    validators: {
                        callback: {
                            message: "El campo Número debe tener el siguiente formato: " + $("#ExpresionRegularExpedienteVisible").val(),
                            callback: function (value, validator, $field) {
                                var numeroLegajo = $("#NumeroLegajo").val(),
                                    expRegExpediente = $("#ExpresionRegularExpediente").val();

                                if (value.length === 0 && numeroLegajo.length === 0)
                                    return false;
                                if (value.length > 0)
                                    return value.match(expRegExpediente) != null;
                                return true;
                            }
                        }
                    }
                },
                FechaExpediente: {
                    validators: {
                        callback: {
                            message: "El campo Fecha es obligatorio",
                            callback: function (value, validator, $field) {
                                if ($("#NumeroExpediente").val().length > 0 &&
                                    $("#FechaExpediente").val().length === 0)
                                    return false;
                                return true;
                            }
                        }
                    }
                },
                NumeroLegajo: {
                    validators: {
                        callback: {
                            message: "El campo Número debe tener el siguiente formato: " + $("#ExpresionRegularLegajoVisible").val(),
                            callback: function (value, validator, $field) {
                                var numeroExpediente = $("#NumeroExpediente").val(),
                                    expRegLegajo = $("#ExpresionRegularLegajo").val();

                                if (value.length === 0 && numeroExpediente.length === 0)
                                    return false;
                                if (value.length > 0)
                                    return value.match(expRegLegajo) != null;
                                return true;
                            }
                        }
                    }
                },
                FechaLegajo: {
                    validators: {
                        callback: {
                            message: "El campo Fecha es obligatorio",
                            callback: function (value, validator, $field) {
                                if ($("#NumeroLegajo").val().length > 0 &&
                                    $("#FechaLegajo").val().length === 0)
                                    return false;
                                return true;
                            }
                        }
                    }
                },
                Chapa: {
                    validators: {
                        callback: {
                            message: "El campo Número de Chapa debe ser de la forma " + $("#ExpresionRegularChapaVisible").val(),
                            callback: function (value, validator, $field) {
                                var expRegChapa = $("#ExpresionRegularChapa").val();
                                if (value.length > 0)
                                    return value.match(expRegChapa) != null;
                                return true;
                            }
                        }
                    }
                }
            }
        }).on("error.field.bv", function (e, data) {
            $("#save-all").addClass("boton-deshabilitado");
        }).on("status.field.bv", function (e, data) {
            $("#save-all").removeClass("boton-deshabilitado");
        });
    });

}

function tipoTramiteInit() {
    $("#tipo-tramite-form").ajaxForm();
}

function tipoTramiteFormContent() {
    var form = $("#tipo-tramite-form");
    form.load(BASE_URL + "TipoTramite/FormContent", function () {
        //Autocomplete select
        var select = $(".select2", "#tipo-tramite-form");
        select.select2();

        select.on("change", function (e) {
            if (e.added) {
                $.post(BASE_URL + "TipoTramite/Save/" + e.added.id);
            } else if (e.removed) {
                $.post(BASE_URL + "TipoTramite/Delete/" + e.removed.id);
            }
        });

        $("#tipo-tramite-form").bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            //icon: {
            //    valid: "glyphicon glyphicon-ok",
            //    invalid: "glyphicon glyphicon-remove",
            //    validating: "glyphicon glyphicon-refresh"
            //},
            fields: {
                IdTipo: {
                    validators: {
                        callback: {
                            message: "El campo Tipo de Trámite es obligatorio",
                            callback: function (value, validator) {
                                // Get the selected options
                                var options = validator.getFieldElements("IdTipo").val();
                                return (options != null && options.length > 0);
                            }
                        }
                    }
                }
            }
        }).on("error.field.bv", function (e, data) {
            $("#save-all").addClass("boton-deshabilitado");
        }).on("status.field.bv", function (e, data) {
            $("#save-all").removeClass("boton-deshabilitado");
        });
    });
}

function actasInit() {
    $("#actas tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        } else {
            $("tr.selected", "#actas tbody").removeClass("selected");
            $(this).addClass("selected");
        }
    });
    createDataTable("actas");
}

function actasList(insert) {
    if (insert) {
        $.get(BASE_URL + "Acta/ClearId");
        $("#actas").dataTable().api().clear().draw();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        destroyDataTable("actas");

        $("#actas tbody").load(BASE_URL + "Acta/List/" + idExpedienteObra, function () {
            createDataTable("actas");
        });
    }
}

function ubicacionesInit() {
    $("#ubicaciones tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowUbicaciones = null;
            ubicacionesEnableControls(false);
            enablePrimary(false);
        } else {
            $("tr.selected", "#ubicaciones tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowUbicaciones = $(this);

            if (!selectedRowUbicaciones.children().hasClass("dataTables_empty")) {
                ubicacionesEnableControls(true);
                enablePrimary(selectedRowUbicaciones.find("td:last").html() === "No");
            }
        }
    });

    $("#ubicaciones-insert").click(function () {
        $("#ubicaciones-panel-body").hide();
        $("#ubicacion-panel-body").show();
        $("#domicilios").dataTable().api().clear().draw();
        $("#selected_error").addClass("hide");
        $("#IdDomicilio").val("");
        $("#search").val("");
    });

    createDataTable("ubicaciones");
}

function ubicacionFormContent() {
    var form = $("#ubicacion-form");
    form.load(BASE_URL + "Ubicacion/FormContent", function () {
        $("#domicilios").dataTable({
            "scrollY": "148px",
            "scrollCollapse": true,
            "paging": false,
            "searching": false,
            "processing": true,
            "dom": "rt",
            "aaSorting": [[1, "asc"]],
            "language": {
                "url": BASE_URL + "Scripts/dataTables.spanish.txt"
            },
            "columns": [
                { "data": "DomicilioId", "className": "hide" },
                { "data": "localidad" },
                { "data": "ViaNombre" },
                { "data": "numero_puerta" },
                { "data": "piso" },
                { "data": "codigo_postal", "className": "hide" }
            ]
        });

        $("#domicilios tbody").on("click", "tr", function () {
            if ($(this).hasClass("selected")) {
                $(this).removeClass("selected");

                selectedRowDomicilios = null;
                $("#IdDomicilio").val("");
            } else {
                $("tr.selected", "#domicilios tbody").removeClass("selected");
                $(this).addClass("selected");

                selectedRowDomicilios = $(this);

                if (selectedRowDomicilios.children().hasClass("dataTables_empty"))
                    selectedRowDomicilios = null;
                else {
                    var id = Number(selectedRowDomicilios.find("td:first").html());
                    $("#IdDomicilio").val(id);
                }
            }
        });

        $("#domicilios-search").click(function () {
            $("#selected_error").addClass("hide");
            var searchUrl = BASE_URL + "DomicilioExpedienteObra/Search?nombreVia=" + $("#search").val();
            $("#search", "#ubicacion-form").enable(false);
            $("#domicilios-search").enable(false);
            $("#domicilios").dataTable().api().ajax.url(searchUrl).load(function () {
                $("#search", "#ubicacion-form").enable(true);
                $("#domicilios-search").enable(true);
            });
        });

        $("#domicilio-insert").click(function () {
            var idDomicilio = prompt("id domicilio inmueble");
            if (idDomicilio) {
                $.ajax({
                    type: "POST",
                    url: BASE_URL + "DomicilioExpedienteObra/GetDomicilio/" + idDomicilio,
                    success: function (domicilio) {
                        var table = $("#domicilios").dataTable().api();
                        table.clear().draw();

                        $("#search").val("");

                        table.row.add({
                            "DomicilioId": domicilio.DomicilioId,
                            "localidad": domicilio.localidad,
                            "ViaNombre": domicilio.ViaNombre,
                            "numero_puerta": domicilio.numero_puerta,
                            "piso": domicilio.piso,
                            "codigo_postal": domicilio.codigo_postal
                        }).draw();
                    }
                });
            }
        });

        //Fix for switches
        var cmtoggle = $(".cmn-toggle", "#ubicacion-form");
        $.each(cmtoggle, function (index, item) {
            $("#" + item.id).after("<label for=\"" + item.id + "\"></label>");
        });

        $("#ubicaciones-submit").click(function () {
            form.formValidation("revalidateField", "IdDomicilio");

            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();

            if (bootstrapValidator.isValid()) {

                var idDomicilio = $("#IdDomicilio").val();

                var trs = $("#ubicaciones tbody tr");
                var found = false;
                $.each(trs, function (index, tr) {
                    if ($(tr).find("td:first").html() == idDomicilio) {
                        found = true;
                    }
                });

                if (found) {
                    $("#selected_error").removeClass("hide");
                } else {
                    $("#selected_error").addClass("hide");
                    $.post(BASE_URL + "Ubicacion/Save?idDomicilioInmueble=" + idDomicilio, function () {
                        var ubicacion = selectedRowDomicilios.find("td").eq(2).html();
                        var numeroPuerta = selectedRowDomicilios.find("td").eq(3).html();
                        var barrio = selectedRowDomicilios.find("td").eq(1).html();
                        var codigoPostal = selectedRowDomicilios.find("td").eq(5).html();

                        var table = $("#ubicaciones").dataTable().api();
                        var node = table.row.add([
                            idDomicilio,
                            ubicacion,
                            numeroPuerta,
                            barrio,
                            codigoPostal,
                            "<span></span>",
                            "No"
                        ]).draw().node();
                        $(node).find("td:first").addClass("hide");
                        $(node).find("td:last").addClass("hide");

                        $("#ubicaciones-panel-body").show();
                        $("#ubicacion-panel-body").hide();
                        columnsAdjust("ubicaciones");
                    });
                }
            }
        });

        $("#ubicaciones-cancel").click(function () {
            selectedRowUbicaciones = unselectRow(selectedRowUbicaciones);

            ubicacionesEnableControls(false);

            $("#ubicaciones-panel-body").show();
            $("#ubicacion-panel-body").hide();

            columnsAdjust("ubicaciones");
        });

        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            fields: {
                IdDomicilio: {
                    validators: {
                        notEmpty: {
                            message: "Debe seleccionar una Ubicación de la Obra"
                        }
                    }
                }
            }
        });
    });
}

function ubicacionesList(insert) {
    if (insert) {
        $.get(BASE_URL + "Ubicacion/ClearId");
        $("#ubicaciones").dataTable().api().clear().draw();

        $("#ubicaciones-panel-body").show();
        $("#ubicacion-panel-body").hide();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        destroyDataTable("ubicaciones");

        $("#ubicaciones tbody").load(BASE_URL + "Ubicacion/List/" + idExpedienteObra, function () {
            createDataTable("ubicaciones");

            $("#ubicaciones-delete").addClass("boton-deshabilitado");

            $("#ubicaciones-panel-body").show();
            $("#ubicacion-panel-body").hide();
            $('[data-toggle="tooltip"]').tooltip({ container: 'body' });
            boolUbicacion = true;
            checkAllTables();
        });
    }
}

function estadosInit() {
    $("#estados tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowEstados = null;
            estadosEnableControls(false);
        } else {
            $("tr.selected", "#estados tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowEstados = $(this);

            if (selectedRowEstados.children().hasClass("dataTables_empty"))
                selectedRowEstados = null;
            else {
                //carga de datos
                var data = $("#estados").dataTable().api().row(this).data();

                $.each(data, function (key, value) {
                    var control = $("[name='" + key + "']", "#estado-form");
                    if (control.length > 0) {
                        control.val(value);
                    }
                });

                estadosEnableControls(true);
            }
        }
    });

    $("#estados-insert").click(function () {
        selectedRowEstados = unselectRow(selectedRowEstados);

        $("#estado-form")[0].reset();
        //$("#estado-form").formValidation("resetForm", true);

        eliminarUltimoEstado();

        $("#estados-panel-body").hide();
        $("#estado-panel-body").show();

        $("#EstadoExpedienteId").unbind("mousedown");
        $("#Insert").val("true");
    });

    $("#estados-edit").click(function () {
        if (selectedRowEstados) {
            $("#estados-panel-body").hide();
            $("#estado-panel-body").show();

            //prevent edition
            $("#EstadoExpedienteId").mousedown(function (e) {
                e.preventDefault();
            });

            $("#Insert").val("false");
        }
    });

    $("#ubicacion-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function (formData, jqForm, options) {
            var idDomicilio = $("#IdDomicilio").val();

            var trs = $("#ubicaciones tbody tr");
            var found = false;
            $.each(trs, function (index, tr) {
                if ($(tr).find("td:first").html() == idDomicilio) {
                    found = true;
                }
            });
            return found;
        }
    });

    $("#estado-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function (formData, jqForm, options) {
            var table = $("#estados").dataTable().api();

            if (selectedRowEstados == null) {
                table.row.add({
                    "ExpedienteObraId": 0,
                    "EstadoExpedienteId": $("#EstadoExpedienteId :selected").val(),
                    "EstadoExpediente": { "Descripcion": $("#EstadoExpedienteId :selected").text() },
                    "Fecha": formatFechaHora(new Date()),
                    "Observaciones": $("#Observaciones").val()
                }).draw();
            } else {
                var selectedRow = table.row(".selected");
                var d = selectedRow.data();

                d.EstadoExpedienteId = $("#EstadoExpedienteId :selected").val();
                d.EstadoDescripcion = $("#EstadoExpedienteId :selected").text();
                d.Observaciones = $("#Observaciones").val();

                selectedRow.data(d).draw();
            }
        },
        success: function (data) {
            selectedRowEstados = unselectRow(selectedRowEstados);

            estadosEnableControls(false);

            $("#estados-panel-body").show();
            $("#estado-panel-body").hide();

            columnsAdjust("estados");
        }
    });

    $("#estados").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "autoWidth": false,
        "dom": "rt",
        "order": [[4, "desc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [
            { "orderable": false, "targets": 2 },
            { "orderable": false, "targets": 3 }
        ],
        "columns": [
            { "data": "ExpedienteObraId", "className": "hide" },
            { "data": "EstadoExpedienteId", "className": "hide" },
            { "data": "EstadoExpediente.Descripcion" },
            {
                "data": "Fecha", "render": function (data, type, full, meta) {
                    return formatDate(data);
                }
            },
            { "data": "Fecha", "className": "hide" },
            { "data": "Observaciones", "className": "hide" }
        ]
    });
}

function estadosList(insert) {
    var table = $("#estados").dataTable().api();

    if (insert) {
        $.get(BASE_URL + "Estado/ClearId");

        table.clear().draw();

        table.row.add({
            "ExpedienteObraId": 0,
            "EstadoExpedienteId": $("#EstadoExpedienteId option:first").val(),
            "EstadoExpediente": { "Descripcion": $("#EstadoExpedienteId option:first").text() },
            "Fecha": formatFechaHora(new Date()),
            "Observaciones": ""
        }).draw();

        $("#estados-insert").hide();
        estadosEnableControls(false);

        $("#estados-panel-body").show();
        $("#estado-panel-body").hide();

        $.post(BASE_URL + "Estado/Save?Insert=true&" + "EstadoExpedienteId=" +
            $("#EstadoExpedienteId").val() + "&Observaciones=" + $("#Observaciones").val());
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        table.ajax.url(BASE_URL + "Estado/List/" + idExpedienteObra).load(function () {
            $("#estados-insert").show();
            estadosEnableControls(false);

            $("#estados-panel-body").show();
            $("#estado-panel-body").hide();
            boolEstados = true;
            checkAllTables();
        });
    }
}

function estadoFormContent() {
    var form = $("#estado-form");
    form.load(BASE_URL + "Estado/FormContent", function () {
        $("#estado-submit").click(function () {
            form.submit();
        });

        $("#estado-cancel").click(function () {
            selectedRowEstados = unselectRow(selectedRowEstados);

            form.formValidation("resetForm", true);

            estadosEnableControls(false);

            $("#estados-panel-body").show();
            $("#estado-panel-body").hide();
        });
    });
}

function datosGeneralesInit() {
    //$("#datos-form").ajaxForm({        
    //    success: function (data) {

    //    }
    //});
}

function datosGeneralesFormContent() {
    $("#datos-form").load(BASE_URL + "DatosGenerales/FormContent", function () {
        //Fix for switches
        var cmtoggle = $(".cmn-toggle", "#datos-form");
        $.each(cmtoggle, function (index, item) {
            $("#" + item.id).after("<label for=\"" + item.id + "\"></label>");
        });
    });
}

function enableFormContent(formId, enable) {
    var formElements = $("#" + formId + " :input");
    if (enable) {
        $.each(formElements, function (index, element) {
            $(element).enable(true);
            //if (element.type === "file")
            //$(element).fileinput("refresh");
        });
    } else {
        $.each(formElements, function (index, element) {
            $(element).enable(false);
            //if (element.type === "file")
            //$(element).fileinput("refresh");
        });
    }
}

function unselectRow(selectedRow) {
    if (selectedRow) {
        selectedRow.removeClass("selected");
        return null;
    }
}

function initExpedientesObras() {
    $("#busqueda").dataTable().api().clear().draw();
    busquedaEnableControls(false);
    idExpedienteObra = null;
    $("#identificacion-form").formValidation("resetForm", false);

    identificacionTab(false);
    unidadesList(false);
    ubicacionesList(false);
    estadosList(false);

    $("#save-all").hide();
    $("#cancel-all").hide();
}

//IDENTIFICACION

function identificacionTab(enable) {
    enableFormContent("identificacion-form", enable);
    enableFormContent("tipo-tramite-form", enable);

    if (enable) {
        $("#unidades-controls").show();
        $("#ubicaciones-controls").show();
        $("#estados-controls").show();
        $("#unidades tbody td span").each(function (index, element) {
            if ($(element).attr("onclick")) {
                $(element).removeClass("boton-deshabilitado");
            }
        })
    } else {
        $("#unidades-controls").hide();
        $("#ubicaciones-controls").hide();
        $("#estados-controls").hide();
    }
}

function unidadesList(insert) {
    if (insert) {
        $.get(BASE_URL + "UnidadTributariaExpedienteObra/ClearId");
        $("#unidades").dataTable().api().clear().draw();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        destroyDataTable("unidades");

        $("#unidades tbody").load(BASE_URL + "UnidadTributariaExpedienteObra/List/" + idExpedienteObra, function () {
            createDataTable("unidades");
            $("#unidades-delete").addClass("boton-deshabilitado");
            boolUT = true;
            checkAllTables();
            //columnsAdjust('unidades');
        });
    }
}

function tipoTramiteList(insert) {
    if (insert) {
        $("#IdTipo").select2("val", "0");
        $.get(BASE_URL + "TipoTramite/ClearId");
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();
        $.ajax({
            type: "POST",
            url: BASE_URL + "TipoTramite/List/" + idExpedienteObra,
            dataType: "json",
            success: function (data) {
                var selectData = [];
                $.each(data, function (key, object) {
                    selectData.push({ id: object.TipoExpedienteId, text: object.TipoExpediente.Descripcion });
                });
                $("#IdTipo").select2("data", selectData);
                boolTipoTramite = true;
                checkAllTables();
            }
        });
    }
}

//DATOS GENERALES

function datosGeneralesTab(enable) {
    enableFormContent("datos-form", enable);
    enableFormContent("superficie-form", enable);
    enableFormContent("servicio-form", enable);
    enableFormContent("documento-form", enable);

    if (enable) {
        $("#superficies-controls").show();
        //$("#documentos-controls").show();
        $("#documentos-insert").show();
        $("#documentos-delete").show();
        $("#documentos-edit").show();
        $("#servicio-submit").show();
        $("#servicio-cancel").show();
    } else {
        $("#superficies-controls").hide();
        $("#documentos-insert").hide();
        $("#documentos-delete").hide();
        $("#documentos-edit").hide();
        //$("#documentos-controls").hide();
        $("#servicio-submit").hide();
        $("#servicio-cancel").hide();
    }

    $("#documentos-view").removeAttr("style", "none");
}

function superficiesInit() {
    $("#superficies tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowSuperficies = null;
            superficiesEnableControls(false);
        } else {
            $("tr.selected", "#superficies tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowSuperficies = $(this);

            if (selectedRowSuperficies.children().hasClass("dataTables_empty"))
                selectedRowSuperficies = null;
            else {
                var data = $("#superficies").dataTable().api().row(this).data();

                $.each(data, function (key, value) {
                    var control = $("[name='" + key + "']", "#superficie-form");
                    if (control.length > 0) {
                        if (control.hasClass("date")) {
                            control.datepicker("update", formatDate(value));
                        } else control.val(value);
                    }
                });

                superficiesEnableControls(true);
            }
        }
    });

    $("#superficies-insert").click(function () {
        selectedRowSuperficies = unselectRow(selectedRowSuperficies);

        $("#superficie-form")[0].reset();
        $("#superficie-form").formValidation("resetForm", true);

        var table = $("#superficies").dataTable().api();
        $("#ExpedienteObraSuperficieId").val(getNextId(table.rows().data(), "ExpedienteObraSuperficieId"));

        var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
            fechaLegajoParts = $("#FechaLegajo").val().split("/"),
            fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
            fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
            minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
        $("#Fecha", "#superficie-form").datepicker("setStartDate", new Date(minDate));

        $("#superficies-panel-body").hide();
        $("#superficie-panel-body").show();
    });

    $("#superficie-form").ajaxForm({
        success: function (responseText, statusText, xhr, $form) {
            if (responseText == "Ok") {
                var table = $("#superficies").dataTable().api();

                if (selectedRowSuperficies == null) {
                    table.row.add({
                        "ExpedienteObraSuperficieId": $("#ExpedienteObraSuperficieId").val(),
                        "ExpedienteObraId": "",
                        "TipoSuperficieId": $("#TipoSuperficieId :selected").val(),
                        "DestinoId": $("#DestinoId :selected").val(),
                        "Fecha": $("#Fecha", "#superficie-form").val(),
                        "TipoSuperficie": { "Descripcion": $("#TipoSuperficieId :selected").text() },
                        "Superficie": Number($("#Superficie").val()),
                        "Destino": { "Descripcion": $("#DestinoId :selected").text() },
                        "CantidadPlantas": Number($("#CantidadPlantas").val())
                    }).order([0, "asc"]).draw();
                } else {
                    var selectedRow = table.row(".selected");
                    var d = selectedRow.data();

                    d.TipoSuperficieId = $("#TipoSuperficieId :selected").val();
                    d.DestinoId = $("#DestinoId :selected").val();
                    d.Fecha = $("#Fecha").val();
                    d.TipoSuperficie.Descripcion = $("#TipoSuperficieId :selected").text();
                    d.Superficie = Number($("#Superficie").val());
                    d.Destino.Descripcion = $("#DestinoId :selected").text();
                    d.CantidadPlantas = Number($("#CantidadPlantas").val());

                    selectedRow.data(d).draw();
                }


                selectedRowSuperficies = unselectRow(selectedRowSuperficies);
                $("#superficie-form").formValidation("resetForm", true);

                $("#superficies-panel-body").show();
                $("#superficie-panel-body").hide();

                columnsAdjust("superficies");
            } else {
                errorAlert(responseText);
            }
        }
    });

    $("#superficies").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[3, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { "data": "ExpedienteObraSuperficieId", "className": "hide" },
            { "data": "ExpedienteObraId", "className": "hide" },
            { "data": "TipoSuperficieId", "className": "hide" },
            { "data": "DestinoId", "className": "hide" },
            {
                "data": "Fecha", "render": function (data, type, full, meta) {
                    return formatDate(data);
                }
            },
            { "data": "TipoSuperficie.Descripcion" },
            { "data": "Superficie" },
            { "data": "Destino.Descripcion" },
            { "data": "CantidadPlantas", "className": "hide" }
        ]
    });
}

function superficieFormContent() {
    var form = $("#superficie-form");
    form.load(BASE_URL + "Superficie/FormContent", function () {
        //Datepicker
        $("#Fecha", "#superficie-form").datepicker(getDatePickerConfig({ endDate: new Date(), todayHighlight: true }))
            .on("changeDate", function () {
                form.data("bootstrapValidator")
                    .updateStatus("Fecha", "NOT_VALIDATED", null)
                    .validateField("Fecha");
            });

        $("#superficie-submit").click(function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();

            if (bootstrapValidator.isValid()) {
                form.submit();
            }
        });

        $("#superficie-cancel").click(function () {
            selectedRowSuperficies = unselectRow(selectedRowSuperficies);

            form.formValidation("resetForm", true);

            superficiesEnableControls(false);

            $("#superficies-panel-body").show();
            $("#superficie-panel-body").hide();

            columnsAdjust("superficies");
        });

        form.bootstrapValidator({
            framework: "bootstrap",
            excluded: [":disabled"],
            //icon: {
            //    valid: "glyphicon glyphicon-ok",
            //    invalid: "glyphicon glyphicon-remove",
            //    validating: "glyphicon glyphicon-refresh"
            //},
            fields: {
                Fecha: {
                    validators: {
                        date: {
                            format: "DD/MM/YYYY",
                            message: "El campo Fecha debe ser una fecha válida"
                        },
                        notEmpty: {
                            message: "El campo Fecha es obligatorio"
                        }
                    }
                },
                Superficie: {
                    validators: {
                        greaterThan: {
                            inclusive: false,
                            message: "El valor de Superficie no debe ser cero",
                            value: 0
                        },
                        numeric: {
                            message: "El campo Superficie debe ser numérico"
                        },
                        notEmpty: {
                            message: "El campo Superficie es obligatorio"
                        }
                    }
                },
                Plantas: {
                    validators: {
                        numeric: {
                            message: "El campo Plantas debe ser numérico"
                        }
                    }
                }
            }
        });
    });
}

function superficiesList(insert) {
    var table = $("#superficies").dataTable().api();
    if (insert) {
        $.get(BASE_URL + "Superficie/ClearId");

        table.clear().draw();

        $("#superficies-panel-body").show();
        $("#superficie-panel-body").hide();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        table.ajax.url(BASE_URL + "Superficie/List/" + idExpedienteObra).load(function () {
            $("#superficies-delete").addClass("boton-deshabilitado");
            $("#superficies-panel-body").show();
            $("#superficie-panel-body").hide();
            columnsAdjust('superficies');
            boolSuperficies = true;
            checkAllTables();
        });
    }
}

function superficiesEnableControls(enable) {
    if (enable) {
        $("#superficies-delete").click(function () {
            if (selectedRowSuperficies) {
                var id = selectedRowSuperficies.find("td").eq(0).html();
                var tipo = selectedRowSuperficies.find("td").eq(5).html();
                var sup = selectedRowSuperficies.find("td").eq(6).html();
                modalConfirm("Eliminar - Superficie", "¿Desea eliminar la superficie con tipo " + tipo + " y superficie " + sup + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "Superficie/Delete?idExpedienteObraSuperficie=" + id,
                        type: "POST",
                        success: function (data) {
                            if (data == "Ok") {
                                $("#superficies").dataTable().api().row(".selected").remove().draw(false);
                                selectedRowSuperficies = null;
                                $("#superficies-delete").unbind("click").addClass("boton-deshabilitado");
                                $("#superficies-edit").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#superficies-edit").click(function () {
            if (selectedRowSuperficies) {
                var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
                    fechaLegajoParts = $("#FechaLegajo").val().split("/"),
                    fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
                    fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
                    minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
                $("#Fecha", "#superficie-form").datepicker("setStartDate", new Date(minDate));

                $("#superficies-panel-body").hide();
                $("#superficie-panel-body").show();
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#superficies-delete").unbind("click").addClass("boton-deshabilitado");
        $("#superficies-edit").unbind("click").addClass("boton-deshabilitado");
    }
}

function serviciosList(insert) {
    if (insert) {
        $("#IdServicio").select2("val", "0");
        $.get(BASE_URL + "Servicio/ClearId");
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();
        $.ajax({
            type: "POST",
            url: BASE_URL + "Servicio/List/" + idExpedienteObra,
            dataType: "json",
            success: function (data) {
                var selectData = [];
                $.each(data, function (key, object) {
                    selectData.push({ id: object.ServicioId, text: object.Servicio.Nombre });
                });
                $("#IdServicio").select2("data", selectData);
                boolServicios = true;
                checkAllTables();
            }
        });
    }
}

function servicioInit() {
    $("#servicio-form").ajaxForm();
}

function servicioFormContent() {
    var form = $("#servicio-form");
    form.load(BASE_URL + "Servicio/FormContent", function () {
        //Autocomplete select
        var select = $(".select2", "#servicio-form");
        select.select2();

        select.on("change", function (e) {
            if (e.added) {
                $.post(BASE_URL + "Servicio/Save/" + e.added.id);
            } else if (e.removed) {
                $.post(BASE_URL + "Servicio/Delete/" + e.removed.id);
            }
        });

        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            //icon: {
            //    valid: "glyphicon glyphicon-ok",
            //    invalid: "glyphicon glyphicon-remove",
            //    validating: "glyphicon glyphicon-refresh"
            //},
            fields: {
                IdServicio: {
                    validators: {
                        callback: {
                            message: "El campo Servicios es obligatorio",
                            callback: function (value, validator) {
                                // Get the selected options
                                var options = validator.getFieldElements("IdServicio").val();
                                return (options != null && options.length > 0);
                            }
                        }
                    }
                }
            }
        }).on("error.field.bv", function (e, data) {
            $("#save-all").addClass("boton-deshabilitado");
        }).on("status.field.bv", function (e, data) {
            $("#save-all").removeClass("boton-deshabilitado");
        });
    });
}

function documentosInit() {
    $("#documentos tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowDocumentos = null;
            documentosEnableControls(false);
        } else {
            $("tr.selected", "#documentos tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowDocumentos = $(this);

            if (selectedRowDocumentos.children().hasClass("dataTables_empty"))
                selectedRowDocumentos = null;
            else documentosEnableControls(true);
        }
    });

    $("#documentos-insert").click(function () {
        selectedRowDocumentos = unselectRow(selectedRowDocumentos);
        showLoading();
        $("#contenedor-forms-externos").load(BASE_URL + 'Documento/DatosDocumento', function () {
            $(document).one("documentoGuardado", function (evt) {
                documentoGuardado(evt.documento);
            });
        });
    });

    createDataTable("documentos");
}

function documentosList(insert) {
    if (insert) {
        $.get(BASE_URL + "DocumentoExpedienteObra/ClearId");
        $("#documentos").dataTable().api().clear().draw();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        destroyDataTable("documentos");

        $("#documentos tbody").load(BASE_URL + "DocumentoExpedienteObra/List/" + idExpedienteObra, function () {
            createDataTable("documentos");
            $("#documentos-delete").addClass("boton-deshabilitado");
            boolDocumentos = true;
            checkAllTables();
        });
    }
}

function documentosEnableControls(enable) {
    if (enable) {
        $("#documentos-delete").click(function () {
            if (selectedRowDocumentos) {
                var id = selectedRowDocumentos.find("td").eq(0).html();
                var tipo = selectedRowDocumentos.find("td").eq(1).html();
                var descripcion = selectedRowDocumentos.find("td").eq(2).html();
                var fecha = selectedRowDocumentos.find("td").eq(3).html();
                modalConfirm("Eliminar - Documento", "¿Desea eliminar el documento tipo " + tipo +
                    ", descripción " + descripcion + ", fecha " + fecha + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "DocumentoExpedienteObra/Delete?idDocumento=" + id,
                        type: "POST",
                        success: function (data) {
                            if (data == "Ok") {
                                var table = $("#documentos").dataTable().api();
                                table.row(".selected").remove().draw(false);
                                selectedRowDocumentos = null;
                                $("#documentos-delete").unbind("click").addClass("boton-deshabilitado");
                                $("#documentos-edit").unbind("click").addClass("boton-deshabilitado");
                                $("#documentos-view").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#documentos-edit").unbind("click").click(function () {
            if (selectedRowDocumentos) {
                var id = selectedRowDocumentos.find("td").eq(0).html();
                showLoading();
                $("#contenedor-forms-externos").load(BASE_URL + 'Documento/DatosDocumento?id=' + id);
            }
        }).removeClass("boton-deshabilitado");

        $("#documentos-view").click(function () {
            if (selectedRowDocumentos) {
                var id = selectedRowDocumentos.find("td").eq(0).html();
                showLoading();
                $.ajax({
                    url: BASE_URL + "Documento/Visualizar",
                    type: "POST",
                    async: false,
                    success: function (data) {
                    }
                })
                $("#contenedor-forms-externos").load(BASE_URL + 'Documento/DatosDocumento?id=' + id);
            }
            //var data = $("#imagenVisualizar").attr("src"),
            //filename = $('#DatosDocumento_nombre_archivo').val(),
            //id = selectedRowDocumentos.find("td").eq(0).html();
            //$.ajax({
            //    async: false,
            //    type: 'GET',
            //    url: BASE_URL + 'Documento/GetDataBaseState',
            //    dataType: 'json',
            //    success: function (data) {
            //        if (data == "1")
            //            window.location = BASE_URL + 'Documento/Download?id=' + id;
            //        else
            //            window.location = BASE_URL + 'Documento/DownloadServerFile?id=' + id;
            //    }

            //})
        }).removeClass("boton-deshabilitado");
    } else {
        $("#documentos-delete").unbind("click").addClass("boton-deshabilitado");
        $("#documentos-edit").unbind("click").addClass("boton-deshabilitado");
        $("#documentos-view").unbind("click").addClass("boton-deshabilitado");
    }
}

function documentoGuardado(data) {
    var table = $("#documentos").dataTable().api();
    if (selectedRowDocumentos) {
        var selectedRow = table.row(".selected");
        var d = selectedRow.data();
        d[1] = data.Tipo.Descripcion;
        d[2] = data.descripcion;
        d[3] = formatDate(data.fecha);
        d[4] = data.nombre_archivo;
        selectedRow.data(d).draw();
    } else {
        $.ajax({
            type: "POST",
            url: BASE_URL + "DocumentoExpedienteObra/Save?idDocumento=" + data.id_documento,
            success: function (responseText) {
                if (responseText == "Ok") {
                    var node = table.row.add([
                        data.id_documento,
                        data.Tipo.Descripcion,
                        data.descripcion,
                        formatDate(data.fecha),
                        data.nombre_archivo
                    ]).draw().node();
                    $(node).find("td:first").addClass("hide");

                    $("#save-all").removeClass("boton-deshabilitado");
                } else {
                    errorAlert(responseText);
                }
            }
        });
    }
}

//PERSONAS

function personasTab(enable) {
    enableFormContent("persona-form", enable);

    if (enable) {
        $("#personas-controls").show();
    } else {
        $("#personas-controls").hide();
    }
}

function personasInit() {
    $("#personas tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowPersonas = null;
            personasEnableControls(false);
        } else {
            $("tr.selected", "#personas tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowPersonas = $(this);

            if (selectedRowPersonas.children().hasClass("dataTables_empty"))
                selectedRowPersonas = null;
            else personasEnableControls(true);
        }
    });

    $("#personas-insert").click(function () {
        $("#personas-panel-body").hide();
        $("#persona-panel-body").show();
    });

    $("#persona-form").ajaxForm({
        beforeSubmit: function () {
        },
        success: function (data) {
            if (data == "Ok") {
                var table = $("#personas").dataTable().api();

                table.row.add({
                    "ExpedienteObraId": getNextId(table.rows().data(), "ExpedienteObraId"), //for sorting purpose only
                    "PersonaInmuebleId": $("#IdPersona").val(),
                    "RolId": $("#IdRol :selected").val(),
                    "Rol": $("#IdRol :selected").text(),
                    "NombreCompleto": $("#persona-form #Persona").val(),
                    "DomicilioFisico": $("#DomicilioFisico").val()
                }).order([0, "asc"]).draw();


                selectedRowPersonas = unselectRow(selectedRowPersonas);
                $("#persona-form").formValidation("resetForm", true);

                $("#personas-panel-body").show();
                $("#persona-panel-body").hide();

                columnsAdjust("personas");
            } else {
                errorAlert(data);
            }
        }
    });

    $("#personas").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { "data": "ExpedienteObraId", "className": "hide" },
            { "data": "PersonaInmuebleId", "className": "hide" },
            { "data": "RolId", "className": "hide" },
            { "data": "Rol" },
            { "data": "NombreCompleto" },
            { "data": "DomicilioFisico" }
        ]
    });

    $("#busqueda-personas tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        } else {
            $("tr.selected", "#busqueda-personas tbody").removeClass("selected");
            $(this).addClass("selected");

            var selectedRow = $(this);

            if (!selectedRow.children().hasClass("dataTables_empty")) {

            }
        }
    });

    $("#busqueda-personas").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { "data": "ExpedienteObraId", "className": "hide" },
            { "data": "PersonaInmuebleId", "className": "hide" },
            { "data": "RolId", "className": "hide" },
            { "data": "Rol", "className": "hide" },
            { "data": "NombreCompleto" },
            { "data": "DomicilioFisico", "className": "hide" }
        ]
    });

    $("#btnSearchOK").click(function () {
        var table = $("#busqueda-personas").dataTable().api();
        var data = table.row(".selected").data();
        if (data) {
            $("#persona-form #Persona").val(data.NombreCompleto);
            $("#persona-form #IdPersona").val(data.PersonaInmuebleId);
            $("#persona-form #DomicilioFisico").val(data.DomicilioFisico);//REVISAR SI ESTA FUNCIONANDO

            $("#persona-form").formValidation("revalidateField", "IdPersona");
            $("#persona-form").formValidation("revalidateField", "Persona");
        }
    });
}

function personaSearch() {
    buscarPersonas(true)
        .then(function (seleccion) {
            if (seleccion)
                $.post(BASE_URL + "PersonaExpedienteObra/GetPersona/" + seleccion[1], function (data) {
                    $("#persona-form #Persona").val(data.NombreCompleto);
                    $("#persona-form #IdPersona").val(data.PersonaInmuebleId);
                    $("#persona-form #DomicilioFisico").val(data.DomicilioFisico);

                    $("#persona-form").formValidation("revalidateField", "IdPersona");
                    $("#persona-form").formValidation("revalidateField", "Persona");
                });
        })
        .catch(function (err) { console.log(err); });
}

function search() {
    var nombre = $("#search-nombre-persona").val();
    var table = $("#busqueda-personas").dataTable().api();
    table.ajax.url(BASE_URL + "PersonaExpedienteObra/SearchPersona?nombre=" + nombre).load();
}

function personasList(insert) {
    var table = $("#personas").dataTable().api();
    if (insert) {
        $.get(BASE_URL + "PersonaExpedienteObra/ClearId");

        table.clear().draw();

        $("#personas-panel-body").show();
        $("#persona-panel-body").hide();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        table.ajax.url(BASE_URL + "PersonaExpedienteObra/List/" + idExpedienteObra).load(function () {
            $("#personas-delete").addClass("boton-deshabilitado");

            $("#personas-panel-body").show();
            $("#persona-panel-body").hide();

            boolPersonas = true;
            checkAllTables();
        });
    }
}

function personaFormContent() {
    var form = $("#persona-form");
    form.load(BASE_URL + "PersonaExpedienteObra/FormContent", function () {
        $("#persona-submit").click(function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();

            if (bootstrapValidator.isValid())
                $("#persona-form").submit();
        });

        $("#persona-cancel").click(function () {
            selectedRowPersonas = unselectRow(selectedRowPersonas);

            form.formValidation("resetForm", true);

            personasEnableControls(false);

            $("#personas-panel-body").show();
            $("#persona-panel-body").hide();
        });

        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            //icon: {
            //    valid: "glyphicon glyphicon-ok",
            //    invalid: "glyphicon glyphicon-remove",
            //    validating: "glyphicon glyphicon-refresh"
            //},
            fields: {
                IdPersona: {
                    validators: {
                        greaterThan: {
                            inclusive: false,
                            message: "La persona no existe, verifíquela",
                            value: 0
                        }
                    }
                },
                Persona: {
                    validators: {
                        notEmpty: {
                            message: "El campo Persona es obligatorio"
                        }
                    }
                }
            }
        });
    });
}

function personasEnableControls(enable) {
    if (enable) {
        $("#personas-delete").click(function () {
            if (selectedRowPersonas) {
                var id = selectedRowPersonas.find("td").eq(1).html();
                var rol = selectedRowPersonas.find("td").eq(2).html();
                var razonSocial = selectedRowPersonas.find("td").eq(4).html();
                modalConfirm("Eliminar - Persona", "¿Desea eliminar la persona " + razonSocial + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "PersonaExpedienteObra/Delete?idPersona=" + id + "&idRol=" + rol,
                        type: "POST",
                        success: function (data) {
                            if (data == "Ok") {
                                var table = $("#personas").dataTable().api();
                                table.row(".selected").remove().draw(false);
                                selectedRowPersonas = null;
                                $("#personas-delete").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

    } else {
        $("#personas-delete").unbind("click").addClass("boton-deshabilitado");
    }
}

//SEGUIMIENTO

function seguimientoTab(enable) {
    enableFormContent("liquidacion-form", enable);

    if (enable) {
        $("#liquidaciones-controls").show();
        //$("#inspecciones-controls").show();
        $("#controles-controls").show();
        $("#observaciones-controls").show();
    } else {
        $("#liquidaciones-controls").hide();
        //$("#inspecciones-controls").hide();
        $("#controles-controls").hide();
        $("#observaciones-controls").hide();
    }
}

function liquidacionesInit() {
    $("#liquidaciones tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowLiquidaciones = null;
            liquidacionesEnableControls(false);
        } else {
            $("tr.selected", "#liquidaciones tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowLiquidaciones = $(this);

            if (selectedRowLiquidaciones.children().hasClass("dataTables_empty"))
                selectedRowLiquidaciones = null;
            else {
                var data = $("#liquidaciones").dataTable().api().row(this).data();

                $.each(data, function (key, value) {
                    var control = $("[name='" + key + "']", "#liquidacion-form");
                    if (control.length > 0) {
                        if (control.hasClass("date")) {
                            control.datepicker("update", formatDate(value));
                        } else control.val(value);
                    }
                });

                liquidacionesEnableControls(true);
            }
        }
    });

    $("#liquidaciones-insert").click(function () {
        if ($("#NumeroExpediente").val() != "") {
            selectedRowLiquidaciones = unselectRow(selectedRowLiquidaciones);

            $("#liquidacion-form").formValidation("resetForm", true);
            $("#liquidacion-form")[0].reset();

            var table = $("#liquidaciones").dataTable().api();
            $("#LiquidacionId").val(getNextId(table.rows().data(), "LiquidacionId"));
            $("#Numero", "#liquidacion-form").val($("#NumeroExpediente").val());
            $("#Importe", "#liquidacion-form").val("");
            $("#Fecha", "#liquidacion-form").datepicker('setDate', new Date());

            var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
                fechaLegajoParts = $("#FechaLegajo").val().split("/"),
                fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
                fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
                minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
            $("#Fecha", "#liquidacion-form").datepicker("setStartDate", new Date(minDate));

            $("#liquidaciones-panel-body").hide();
            $("#liquidacion-panel-body").show();
        }
    });

    $("#liquidacion-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function () {
            var table = $("#liquidaciones").dataTable().api();
            if (selectedRowLiquidaciones == null) {
                table.row.add({
                    "LiquidacionId": $("#LiquidacionId").val(),
                    "Fecha": $("#Fecha", "#liquidacion-form").val(),
                    "Numero": $("#Numero", "#liquidacion-form").val(),
                    "Importe": $("#Importe", "#liquidacion-form").val(),
                    "Observaciones": $("#Observaciones", "#liquidacion-form").val()
                }).order([0, "asc"]).draw();
            } else {
                var selectedRow = table.row(".selected");
                var d = selectedRow.data();

                d.Fecha = $("#Fecha", "#liquidacion-form").val();
                d.Numero = $("#Numero", "#liquidacion-form").val();
                d.Importe = $("#Importe", "#liquidacion-form").val();
                d.Observaciones = $("#Observaciones", "#liquidacion-form").val();

                selectedRow.data(d).draw();
            }
        },
        success: function (data) {
            selectedRowLiquidaciones = unselectRow(selectedRowLiquidaciones);
            $("#liquidacion-form").formValidation("resetForm", false);

            $("#liquidaciones-panel-body").show();
            $("#liquidacion-panel-body").hide();

            columnsAdjust("liquidaciones");
        }
    });

    $("#liquidaciones").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { "data": "LiquidacionId", "className": "hide" },
            {
                "data": "Fecha", "render": function (data, type, full, meta) {
                    return formatDate(data);
                }
            },

            { "data": "Numero", "className": "hide" },
            { "data": "Importe" },
            { "data": "Observaciones" },
        ]
    });
}

function liquidacionesList(insert) {
    var table = $("#liquidaciones").dataTable().api();
    if (insert) {
        $.get(BASE_URL + "Liquidacion/ClearId");

        table.clear().draw();

        $("#liquidaciones-panel-body").show();
        $("#liquidacion-panel-body").hide();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        table.ajax.url(BASE_URL + "Liquidacion/List/" + idExpedienteObra).load(function () {
            $("#liquidaciones-panel-body").show();
            $("#liquidacion-panel-body").hide();
            boolLiquidaciones = true;
            checkAllTables();
        });
    }
}

function liquidacionFormContent() {
    var form = $("#liquidacion-form");
    form.load(BASE_URL + "Liquidacion/FormContent", function () {
        //Datepicker
        $("#Fecha", "#liquidacion-form").datepicker(getDatePickerConfig({ todayHighlight: true, endDate: new Date() }))
            .on("changeDate", function () {
                form.data("bootstrapValidator")
                    .updateStatus("Fecha", "NOT_VALIDATED", null)
                    .validateField("Fecha");
            });
        //$("#Fecha", "#liquidacion-form").datepicker('setDate', new Date());

        $("#liquidacion-submit").click(function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();

            if (bootstrapValidator.isValid())
                $("#liquidacion-form").submit();
        });

        $("#liquidacion-cancel").click(function () {
            selectedRowLiquidaciones = unselectRow(selectedRowLiquidaciones);

            form.formValidation("resetForm", true);

            liquidacionesEnableControls(false);

            $("#liquidaciones-panel-body").show();
            $("#liquidacion-panel-body").hide();

            columnsAdjust("liquidaciones");
        });

        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            //icon: {
            //    valid: "glyphicon glyphicon-ok",
            //    invalid: "glyphicon glyphicon-remove",
            //    validating: "glyphicon glyphicon-refresh"
            //},
            fields: {
                Fecha: {
                    validators: {
                        date: {
                            format: "DD/MM/YYYY",
                            message: "El campo Fecha debe ser una fecha válida"
                        },
                        notEmpty: {
                            message: "El campo Fecha es obligatorio"
                        }
                    }
                },
                //Numero: {
                //    validators: {
                //        callback: {
                //            message: "El campo Comprobante debe tener la forma XXXXXX/XXXX",
                //            callback: function (value, validator, $field) {
                //                return value.match(/^\d{6}\/\d{4}$/) != null;
                //            }
                //        }
                //    }
                //},
                Importe: {
                    validators: {
                        greaterThan: {
                            inclusive: false,
                            message: "El valor de Importe no debe ser cero",
                            value: 0
                        },
                        numeric: {
                            message: "El campo Importe debe ser numérico"
                        },
                        notEmpty: {
                            message: "El campo Importe es obligatorio"
                        }
                    }
                }
            }
        });
    });
}

function liquidacionesEnableControls(enable) {
    if (enable) {
        $("#liquidaciones-delete").click(function () {
            if (selectedRowLiquidaciones) {
                var id = selectedRowLiquidaciones.find("td").eq(0).html();
                var fecha = selectedRowLiquidaciones.find("td").eq(1).html();
                var comprobante = selectedRowLiquidaciones.find("td").eq(2).html();
                modalConfirm("Eliminar - Liquidación", "¿Desea eliminar la liquidación con fecha " + fecha + " y comprobante " + comprobante + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "Liquidacion/Delete?idLiquidacion=" + id,
                        type: "POST",
                        success: function (data) {
                            if (data == "Ok") {
                                $("#liquidaciones").dataTable().api().row(".selected").remove().draw(false);
                                selectedRowLiquidaciones = null;
                                $("#liquidaciones-delete").unbind("click").addClass("boton-deshabilitado");
                                $("#liquidaciones-edit").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#liquidaciones-edit").click(function () {
            if (selectedRowLiquidaciones) {
                //var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
                //    fechaLegajoParts = $("#FechaLegajo").val().split("/"),
                //    fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
                //    fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
                //    minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
                //$("#Fecha", "#liquidacion-form").datepicker("setStartDate", new Date(minDate));

                $("#liquidaciones-panel-body").hide();
                $("#liquidacion-panel-body").show();
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#liquidaciones-delete").unbind("click").addClass("boton-deshabilitado");
        $("#liquidaciones-edit").unbind("click").addClass("boton-deshabilitado");
    }
}

function inspeccionesInit() {
    $("#inspecciones tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        } else {
            $("tr.selected", "#inspecciones tbody").removeClass("selected");
            $(this).addClass("selected");
        }
    });

    $("#inspecciones-insert").click(function () {
        showLoading();
        $("#contenedor-forms-externos").load(BASE_URL + 'ObrasParticulares/GestionInspecciones?agregaExpedienteObra=true');
    });

    $("#inspecciones").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[2, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [
            {
                "targets": "no-sort",
                "orderable": false
            }],
        "columns": [
            { "data": "InspeccionID", "className": "hide" },
            { "data": "Tipo" },
            {
                "data": "Fecha", "render": function (data, type, full, meta) {
                    return formatDateTime(data);
                }
            },
            { "data": "Inspector" },
            { "data": "Observaciones" }
        ]
    });
    //    createDataTable("inspecciones");
}

/*function inspeccionesList(insert) {
    var table = $("#inspecciones").dataTable().api();
    if (insert) {
        $.get(BASE_URL + "Inspeccion/ClearId");
        table.clear().draw();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        table.ajax.url(BASE_URL + "Inspeccion/List/" + idExpedienteObra).load(function () {
            columnsAdjust('inspecciones');
            boolInspecciones = true;
            checkAllTables();
        });
    }
}*/

function inspeccionGuardada(id) {
    if ($("#inspecciones").dataTable().api().data().toArray().some(function (insp) { return insp.InspeccionID === parseInt(id); })) {
        errorAlert('La inspección seleccionada ya se escuentra relacionada al expediente');
    } else {
        $.ajax({
            type: "POST",
            url: BASE_URL + "Inspeccion/Save/" + id,
            success: function (data) {
                if (data.result === "Ok") {
                    var inspeccion = JSON.parse(data.inspeccion);
                    var table = $("#inspecciones").dataTable().api();
                    var node = table.row.add(inspeccion).draw().node();
                    $(node).find("td:first").addClass("hide");
                } else {
                    errorAlert(responseText);
                }
            }
        });
    }
}

function controlesInit() {
    $("#controles tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowControles = null;
            controlesEnableControls(false);
        } else {
            $("tr.selected", "#controles tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowControles = $(this);

            if (selectedRowControles.children().hasClass("dataTables_empty"))
                selectedRowControles = null;
            else {
                var data = $("#controles").dataTable().api().row(this).data();

                $.each(data, function (key, value) {
                    var control = $("[name='" + key + "']", "#control-form");
                    if (control.length > 0) {
                        if (control.hasClass("date")) {
                            control.datepicker("update", formatDate(value));
                        } else control.val(value);
                    }
                });

                controlesEnableControls(true);
            }
        }
    });

    $("#controles-insert").click(function () {
        selectedRowControles = unselectRow(selectedRowControles);

        $("#control-form")[0].reset();
        $("#control-form").formValidation("resetForm", true);

        var table = $("#controles").dataTable().api();
        $("#ControlTecnicoId").val(getNextId(table.rows().data(), "ControlTecnicoId"));

        var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
            fechaLegajoParts = $("#FechaLegajo").val().split("/"),
            fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
            fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
            minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
        $("#Fecha", "#control-form").datepicker("setStartDate", new Date(minDate));

        $("#controles-panel-body").hide();
        $("#control-panel-body").show();
    });

    $("#control-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function () {
            var table = $("#controles").dataTable().api();

            if (selectedRowControles == null) {
                table.row.add({
                    "ControlTecnicoId": $("#ControlTecnicoId").val(),
                    "Fecha": $("#Fecha", "#control-form").val(),
                    "Observaciones": $("#Observaciones", "#control-form").val()
                }).order([0, "asc"]).draw();
            } else {
                var selectedRow = table.row(".selected");
                var d = selectedRow.data();

                d.Fecha = $("#Fecha", "#control-form").val();
                d.Observaciones = $("#Observaciones", "#control-form").val();

                selectedRow.data(d).draw();
            }
        },
        success: function (data) {
            selectedRowControles = unselectRow(selectedRowControles);
            $("#control-form").formValidation("resetForm", true);

            $("#controles-panel-body").show();
            $("#control-panel-body").hide();

            columnsAdjust("controles");
        }
    });

    $("#controles").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { "data": "ControlTecnicoId", "className": "hide" },
            {
                "data": "Fecha", "render": function (data, type, full, meta) {
                    return formatDate(data);
                }
            },
            { "data": "Observaciones" }
        ]
    });
}

function controlesList(insert) {
    var table = $("#controles").dataTable().api();
    if (insert) {
        $.get(BASE_URL + "ControlTecnico/ClearId");

        table.clear().draw();

        $("#controles-panel-body").show();
        $("#control-panel-body").hide();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        table.ajax.url(BASE_URL + "ControlTecnico/List/" + idExpedienteObra).load(function () {
            $("#controles-delete").addClass("boton-deshabilitado");

            $("#controles-panel-body").show();
            $("#control-panel-body").hide();
            boolControlTecnico = true;
            checkAllTables();
        });
    }
}

function controlFormContent() {
    var form = $("#control-form");
    form.load(BASE_URL + "ControlTecnico/FormContent", function () {
        //Datepicker
        $("#Fecha", "#control-form").datepicker(getDatePickerConfig({ todayHighlight: true, endDate: new Date() }))
            .on("changeDate", function () {
                form.data("bootstrapValidator")
                    .updateStatus("Fecha", "NOT_VALIDATED", null)
                    .validateField("Fecha");
            });

        $("#control-submit").click(function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();

            if (bootstrapValidator.isValid())
                form.submit();
        });

        $("#control-cancel").click(function () {
            selectedRowControles = unselectRow(selectedRowControles);

            form.formValidation("resetForm", true);

            controlesEnableControls(false);

            $("#controles-panel-body").show();
            $("#control-panel-body").hide();
        });

        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            //icon: {
            //    valid: "glyphicon glyphicon-ok",
            //    invalid: "glyphicon glyphicon-remove",
            //    validating: "glyphicon glyphicon-refresh"
            //},
            fields: {
                Fecha: {
                    validators: {
                        date: {
                            format: "DD/MM/YYYY",
                            message: "El campo Fecha debe ser una fecha válida"
                        },
                        notEmpty: {
                            message: "El campo Fecha es obligatorio"
                        }
                    }
                }
            }
        });
    });
}

function controlesEnableControls(enable) {
    if (enable) {
        $("#controles-delete").click(function () {
            if (selectedRowControles) {
                var id = selectedRowControles.find("td").eq(0).html();
                var fecha = selectedRowControles.find("td").eq(1).html();
                modalConfirm("Eliminar - Control Técnico", "¿Desea eliminar el Control Técnico con fecha " + fecha + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "ControlTecnico/Delete?idControlTecnico=" + id,
                        type: "POST",
                        success: function (data) {
                            if (data == "Ok") {
                                $("#controles").dataTable().api().row(".selected").remove().draw(false);
                                selectedRowControles = null;
                                $("#controles-edit").unbind("click").addClass("boton-deshabilitado");
                                $("#controles-delete").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#controles-edit").click(function () {
            if (selectedRowControles) {
                var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
                    fechaLegajoParts = $("#FechaLegajo").val().split("/"),
                    fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
                    fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
                    minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
                $("#Fecha", "#control-form").datepicker("setStartDate", new Date(minDate));

                $("#controles-panel-body").hide();
                $("#control-panel-body").show();
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#controles-delete").unbind("click").addClass("boton-deshabilitado");
        $("#controles-edit").unbind("click").addClass("boton-deshabilitado");
    }
}

function observacionesInit() {
    $("#observaciones tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowObservaciones = null;
            observacionesEnableControls(false);
        } else {
            $("tr.selected", "#observaciones tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowObservaciones = $(this);

            if (selectedRowObservaciones.children().hasClass("dataTables_empty"))
                selectedRowObservaciones = null;
            else {
                var data = $("#observaciones").dataTable().api().row(this).data();

                $.each(data, function (key, value) {
                    var control = $("[name='" + key + "']", "#observacion-form");
                    if (control.length > 0) {
                        if (control.hasClass("date")) {
                            control.datepicker("update", formatDate(value));
                        } else control.val(value);
                    }
                });

                observacionesEnableControls(true);
            }
        }
    });

    $("#observaciones-insert").click(function () {
        selectedRowObservaciones = unselectRow(selectedRowObservaciones);

        $("#observacion-form")[0].reset();
        $("#observacion-form").formValidation("resetForm", true);

        var table = $("#observaciones").dataTable().api();
        $("#ObservacionExpedienteId").val(getNextId(table.rows().data(), "ObservacionExpedienteId"));

        var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
            fechaLegajoParts = $("#FechaLegajo").val().split("/"),
            fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
            fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
            minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
        $("#Fecha", "#observacion-form").datepicker("setStartDate", new Date(minDate));

        $("#observaciones-panel-body").hide();
        $("#observacion-panel-body").show();
    });

    $("#observacion-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function () {
            var table = $("#observaciones").dataTable().api();

            if (selectedRowObservaciones == null) {
                table.row.add({
                    "ObservacionExpedienteId": $("#ObservacionExpedienteId").val(),
                    "Fecha": $("#Fecha", "#observacion-form").val(),
                    "Observaciones": $("#Observaciones", "#observacion-form").val()
                }).order([0, "asc"]).draw();
            } else {
                var selectedRow = table.row(".selected");
                var d = selectedRow.data();

                d.Fecha = $("#Fecha", "#observacion-form").val();
                d.Observaciones = $("#Observaciones", "#observacion-form").val();

                selectedRow.data(d).draw();
            }
        },
        success: function (data) {
            selectedRowObservaciones = unselectRow(selectedRowObservaciones);
            $("#observacion-form").formValidation("resetForm", true);

            $("#observaciones-panel-body").show();
            $("#observacion-panel-body").hide();

            columnsAdjust("observaciones");
        }
    });

    $("#observaciones").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "processing": true,
        "dom": "rt",
        "order": [[0, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columns": [
            { "data": "ObservacionExpedienteId", "className": "hide" },
            {
                "data": "Fecha", "render": function (data, type, full, meta) {
                    return formatDate(data);
                }
            },
            { "data": "Observaciones" }
        ]
    });
}

function observacionesList(insert) {
    var table = $("#observaciones").dataTable().api();
    if (insert) {
        $.get(BASE_URL + "Observacion/ClearId");

        table.clear().draw();

        $("#observaciones-panel-body").show();
        $("#observacion-panel-body").hide();
    } else {
        var idExpedienteObra = 0;
        if (selectedRowExpedientes)
            idExpedienteObra = selectedRowExpedientes.find("td").eq(0).html();

        table.ajax.url(BASE_URL + "Observacion/List/" + idExpedienteObra).load(function () {
            $("#observaciones-delete").addClass("boton-deshabilitado");

            $("#observaciones-panel-body").show();
            $("#observacion-panel-body").hide();
            boolObservaciones = true;
            checkAllTables();
        });
    }
}

function observacionFormContent() {
    var form = $("#observacion-form");
    form.load(BASE_URL + "Observacion/FormContent", function () {
        //Datepicker
        $("#Fecha", "#observacion-form").datepicker(getDatePickerConfig({ todayHighlight: true, endDate: new Date() }))
            .on("changeDate", function () {
                form.data("bootstrapValidator")
                    .updateStatus("Fecha", "NOT_VALIDATED", null)
                    .validateField("Fecha");
            });

        $("#observacion-submit").click(function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();

            if (bootstrapValidator.isValid())
                form.submit();
        });

        $("#observacion-cancel").click(function () {
            selectedRowObservaciones = unselectRow(selectedRowObservaciones);

            form.formValidation("resetForm", true);

            observacionesEnableControls(false);

            $("#observaciones-panel-body").show();
            $("#observacion-panel-body").hide();
        });

        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            fields: {
                Fecha: {
                    validators: {
                        date: {
                            format: "DD/MM/YYYY",
                            message: "El campo Fecha debe ser una fecha válida"
                        },
                        notEmpty: {
                            message: "El campo Fecha es obligatorio"
                        }
                    }
                }
            }
        });
    });
}

function observacionesEnableControls(enable) {
    if (enable) {
        $("#observaciones-delete").click(function () {
            if (selectedRowObservaciones) {
                var id = selectedRowObservaciones.find("td").eq(0).html();
                var fecha = selectedRowObservaciones.find("td").eq(1).html();
                modalConfirm("Eliminar - Observaciones", "¿Desea eliminar la Observación con fecha " + fecha + "?");

                $("#btnAdvertenciaOK").unbind("click").click(function () {
                    $.ajax({
                        url: BASE_URL + "Observacion/Delete?idObservacionExpediente=" + id,
                        type: "POST",
                        success: function (data) {
                            if (data == "Ok") {
                                $("#observaciones").dataTable().api().row(".selected").remove().draw(false);
                                selectedRowObservaciones = null;
                                $("#observaciones-edit").unbind("click").addClass("boton-deshabilitado");
                                $("#observaciones-delete").unbind("click").addClass("boton-deshabilitado");
                            }
                        }
                    });
                });
            }
        }).removeClass("boton-deshabilitado");

        $("#observaciones-edit").click(function () {
            if (selectedRowObservaciones) {
                var fechaExpedienteParts = $("#FechaExpediente").val().split("/"),
                    fechaLegajoParts = $("#FechaLegajo").val().split("/"),
                    fechaExpediente = new Date(fechaExpedienteParts[2], fechaExpedienteParts[1] - 1, fechaExpedienteParts[0]),
                    fechaLegajo = new Date(fechaLegajoParts[2], fechaLegajoParts[1] - 1, fechaLegajoParts[0]),
                    minDate = fechaExpediente.getTime() < fechaLegajo.getTime() ? fechaExpediente : fechaLegajo;
                $("#Fecha", "#observacion-form").datepicker("setStartDate", new Date(minDate));

                $("#observaciones-panel-body").hide();
                $("#observacion-panel-body").show();
            }
        }).removeClass("boton-deshabilitado");
    } else {
        $("#observaciones-delete").unbind("click").addClass("boton-deshabilitado");
        $("#observaciones-edit").unbind("click").addClass("boton-deshabilitado");
    }
}

function getNextId(arr, key) {
    var elem;
    var max = 0;
    var id;
    for (elem in arr) {
        if (arr.hasOwnProperty(elem)) {
            id = Math.abs(arr[elem][key]);
            if (id > max)
                max = id;
        }
    }
    return (max + 1) * -1;
}

function buscarUnidadesTributarias(multiselect) {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.UnidadesTributarias, multiSelect: multiselect, verAgregar: false, titulo: 'Buscar Unidad Tributaria', campos: ['Nombre'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    if (multiselect) {
                        resolve(evt.seleccion.map(o => o[2]));
                    } else {
                        resolve(evt.seleccion.slice(1));
                    }
                } else {
                    resolve();
                }
            });
        });
    });
}

function buscarPersonas(agregarPersona) {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Personas, multiSelect: false, verAgregar: agregarPersona, titulo: 'Buscar Persona', campos: ['Nombre', 'dni:DNI'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
                if (agregarPersona) {
                    $(window).off('agregarObjetoBuscado');
                }
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    resolve(evt.seleccion.slice(1));
                } else {
                    resolve();
                }
            });
            if (agregarPersona) {
                $(window).one("agregarObjetoBuscado", function () {
                    showLoading();
                    $("#personas-externo-container").load(BASE_URL + "Persona/BuscadorPersona", function () {
                        $(".modal.mainwnd", this).one('hidden.bs.modal', function () {
                            $(window).off('personaAgregada');
                        });
                        $(window).one("personaAgregada", function (evt) {
                            resolve(evt.persona.PersonaId);
                        });
                    });
                });
            }
        });
    });
}

function columnsAdjust(tableId) {
    var tabla = '#' + tableId;
    if ($.fn.DataTable.isDataTable(tabla)) {
        $(tabla).dataTable().api().columns.adjust();
    }
}

function hideDatepicker(inputId) {
    var input = $(inputId);
    input.datepicker("hide");
    input.blur();
}

function formatDate(data) {
    if (!data) return null;
    var parts = data.split("-");
    if (!parts[2]) return data;
    var dd = parts[2].split("T");
    return $.datepicker.formatDate("dd/mm/yy", new Date(parts[0], parts[1] - 1, dd[0]));
}

function formatDate2(data) {
    if (!data) return null;
    var parts = data.split("-");
    if (!parts[2]) return data;
    var dd = parts[2].split("T");
    return $.datepicker.formatDate("yy/mm/dd", new Date(parts[0], parts[1] - 1, dd[0]));
}

function formatDateTime(data) {
    if (!data) return null;
    var parts = data.split("T");
    if (parts.length !== 2) return data;
    var date = parts[0].split("-");
    var time = parts[1].split(":");
    var dateTime = new Date(date[0], date[1] - 1, date[2], time[0], time[1]);
    return [padLeft((dateTime.getDate()), "00"),
    padLeft(dateTime.getMonth() + 1, "00"),
    dateTime.getFullYear("0000")].join('/') + ' ' +
        [padLeft(dateTime.getHours(), "00"),
        padLeft(dateTime.getMinutes(), "00")].join(':');
}

function padLeft(val, pad) {
    return (pad + val).slice(-1 * pad.length);
}

function errorAlertInit() {
    var message = $("#message-error");
    message.find(".close").click(function () {
        message.hide();
    });
}

function errorAlert(text) {
    var message = $("#message-error");
    message.find("p").html(text);
    message.fadeIn("slow").delay(5000).queue(function () {
        message.hide().dequeue();
    });
}


// MENSURA

function buscarMensuras(multiselect) {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Mensuras, multiSelect: multiselect, verAgregar: false, titulo: 'Buscar Mensura', campos: ['Nombre'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    if (multiselect) {
                        resolve(evt.seleccion.map(o => o[2]));
                    } else {
                        resolve(evt.seleccion.slice(1));
                    }
                } else {
                    resolve();
                }
            });
        });
    });
}

function buscarMensurasVep(multiselect) {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Mensuras, multiSelect: multiselect, verAgregar: false, titulo: 'Buscar Mensura VEP', campos: ['Nombre'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    if (multiselect) {
                        resolve(evt.seleccion.map(o => o[2]));
                    } else {
                        resolve(evt.seleccion.slice(1));
                    }
                } else {
                    resolve();
                }
            });
        });
    });
}
