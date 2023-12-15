$(document).ready(init);
$(window).resize(ajustarmodal);

$('#modal-GeneracionListadoGeneralTramites').one('shown.bs.modal', function () {
    ajustarmodal();
    hideLoading();
});

function init() {
    $("#generacionListadoGeneralTramites-content").niceScroll(getNiceScrollConfig());

    $(".date > input", "#generacionListadoGeneralTramites-content").datepicker(getDatePickerConfig());

    $("input.desde", "#generacionListadoGeneralTramites-content").datepicker().on("changeDate", function () {
        $("input.hasta", $(this).parents('.form-group')).datepicker("setStartDate", $(this).datepicker("getDate"));
    });

    $("input.hasta", "#generacionListadoGeneralTramites-content").datepicker().on("changeDate", function () {
        $("input.desde", $(this).parents('.form-group')).datepicker("setEndDate", $(this).datepicker("getDate"));
    });

    $('.date input.form-control').prop("disabled", true);

    tipoTramiteChange($("#cboTipoTramite").val());

    $('#modal-GeneracionListadoGeneralTramites').modal("show");
}

function dpClearButton(input) {
    setTimeout(function () {
        var buttonPane = $(input).datepicker("widget").find(".ui-datepicker-buttonpane");
        $("<button>", {
            text: "Limpiar",
            click: function () { jQuery.datepicker._clearDate(input); }
        }).appendTo(buttonPane).addClass("ui-datepicker-clear ui-state-default ui-priority-primary ui-corner-all btn btn-secondary");
    }, 1)
};

$("#chkFechaIngreso").change(function () {
    var deshabilitado = true;
    if (this.checked) {
        deshabilitado = false;
    }
    else {
        $("#txtFechaIngresoDesde").val("");
        $("#txtFechaIngresoHasta").val("");
    }
    $("#txtFechaIngresoDesde").prop("disabled", deshabilitado);
    $("#txtFechaIngresoHasta").prop("disabled", deshabilitado);
});
$("#chkFechaLibro").change(function () {
    var deshabilitado = true;
    if (this.checked) {
        deshabilitado = false;
    }
    else {
        $("#txtFechaLibroDesde").val("");
        $("#txtFechaLibroHasta").val("");
    }
    $("#txtFechaLibroDesde").prop("disabled", deshabilitado);
    $("#txtFechaLibroHasta").prop("disabled", deshabilitado);
});
$("#chkFechaVenc").change(function () {
    var deshabilitado = true;
    if (this.checked) {
        deshabilitado = false;
    }
    else {
        $("#txtFechaVencDesde").val("");
        $("#txtFechaVencHasta").val("");
    }
    $("#txtFechaVencDesde").prop("disabled", deshabilitado);
    $("#txtFechaVencHasta").prop("disabled", deshabilitado);
});
$("#chkIniciador").change(function () {
    $("#persona-search").toggleClass('boton-deshabilitado');
});

function tipoTramiteChange(tipoTramiteSelected) {
    cargarObjetosTramites(tipoTramiteSelected);
    cargarPrioridadesTramites(tipoTramiteSelected);
}

function cargarObjetosTramites(tipoTramite) {
    $("#cboObjetoTramite").empty();
    if (tipoTramite != null && tipoTramite != "") {
        $.get(BASE_URL + 'mesaentradas/RecuperarObjetosTramiteByTipo', { idTipoTramite: tipoTramite }, function (data) {
            var objetosTramite = data.data;
            if (objetosTramite && objetosTramite.length > 0) {
                [{ IdObjetoTramite: "", Descripcion: "- Todos -" }].concat((objetosTramite || [])).forEach(function (objetoTramite) {
                    $("#cboObjetoTramite").append("<option value='" + objetoTramite.IdObjetoTramite + "'>" + objetoTramite.Descripcion + "</option>");
                });
            }
        });
    }
}
function cargarPrioridadesTramites(tipoTramite) {
    $("#cboPrioridad").empty();
    if (tipoTramite != null && tipoTramite != "") {
        $.get(BASE_URL + 'mesaentradas/RecuperarPrioridadesByTipo', { idTipoTramite: tipoTramite }, function (data) {
            var prioridadesTramite = data.data;
            if (prioridadesTramite && prioridadesTramite.length > 0) {
                [{ IdPrioridadTramite: "", Descripcion: "- Seleccione -" }].concat((prioridadesTramite || [])).forEach(function (prioridadTramite) {
                    $("#cboPrioridad").append("<option value='" + prioridadTramite.IdPrioridadTramite + "'>" + prioridadTramite.Descripcion + "</option>");
                });
                $("#cboPrioridad").val($("#hdnIdPrioridadTramite").val());
                $("#hdnIdPrioridadTramite").val("");
            }
        });
    }
}

function jurisdiccionChange(jurisdiccionSelected) {
    cargarLocalidadesJurisdiccion(jurisdiccionSelected);
}

function cargarLocalidadesJurisdiccion(jurisdiccion) {
    $("#cboLocalidad").empty();
    if (jurisdiccion != null && jurisdiccion != "") {
        $.get(BASE_URL + 'mesaentradas/RecuperarLocalidadesByJurisdiccion', { idJurisdiccion: jurisdiccion }, function (data) {
            var localidades = data.data;
            if (localidades && localidades.length > 0) {
                [{ IdLocalidad: "", Descripcion: "- Todos -" }].concat((localidades || [])).forEach(function (localidad) {
                    $("#cboLocalidad").append("<option value='" + localidad.IdLocalidad + "'>" + localidad.Descripcion + "</option>");
                });
            }
        });
    }
}

$("#btnGenerarListadoGralTramites").click(function () {
    var fechaIngresoDesde = $('#txtFechaIngresoDesde').val();
    var fechaIngresoHasta = $('#txtFechaIngresoHasta').val();
    var fechaLibroDesde = $('#txtFechaLibroDesde').val();
    var fechaLibroHasta = $('#txtFechaLibroHasta').val();
    var fechaVencDesde = $('#txtFechaVencDesde').val();
    var fechaVencHasta = $('#txtFechaVencHasta').val();
    var idJurisdiccion = $("#cboJurisdiccion").val();
    var idLocalidad = $("#cboLocalidad").val() || "";
    var idPrioridad = $("#cboPrioridad").val() || "";
    var idTipoTramite = $("#cboTipoTramite").val();
    var idObjetoTramite = $("#cboObjetoTramite").val() || "";
    var idEstado = $("#cboEstado").val();
    var iniciadorId = $("#hdnIdPersona").val();
    var jurisdiccionText = $('#cboJurisdiccion option:selected').text() || "";
    var localidadText = $('#cboLocalidad option:selected').text() || "";
    var prioridadText = $('#cboPrioridad option:selected').text() || "";
    var tipoTramiteText = $('#cboTipoTramite option:selected').text() || "";
    var objetoTramiteText = $('#cboObjetoTramite option:selected').text() || "";
    var estadoText = $('#cboEstado option:selected').text() || "";
    var iniciadorText = $('#txtIniciador').val() || "";
    if ($("#chkFechaIngreso").prop('checked')) {
        if (!fechaIngresoDesde && !fechaIngresoHasta) {
            mostrarMensaje("Listado general de trámites", "Debe ingresar al menos una fecha de ingreso", 2);
            return false;
        }
    }
    if ($("#chkFechaLibro").prop('checked') === true) {
        if (!fechaLibroDesde || !fechaLibroHasta) {
            mostrarMensaje("Listado general de trámites", "Debe ingresar al menos una fecha de libro", 2);
            return false;
        }
    }
    if ($("#chkFechaVenc").prop('checked') === true) {
        if (!fechaVencDesde || !fechaVencHasta) {
            mostrarMensaje("Listado general de trámites", "Debe ingresar al menos una fecha de vencimiento", 2);
            return false;
        }
    }
    if ($("#chkIniciador").prop('checked') === true) {
        if (!iniciadorId) {
            mostrarMensaje("Listado general de trámites", "Debe ingresar el iniciador", 2);
            return false;
        }
    }
    showLoading();

    var form = new FormData();
    form.append("fechaIngresoDesde", fechaIngresoDesde);
    form.append("fechaIngresoHasta", fechaIngresoHasta);
    form.append("fechaLibroDesde", fechaLibroDesde);
    form.append("fechaLibroHasta", fechaLibroHasta);
    form.append("fechaVencDesde", fechaVencDesde);
    form.append("fechaVencHasta", fechaVencHasta);
    form.append("idJurisdiccion", idJurisdiccion);
    form.append("idLocalidad", idLocalidad);
    form.append("idPrioridad", idPrioridad);
    form.append("idTipoTramite", idTipoTramite);
    form.append("idObjetoTramite", idObjetoTramite);
    form.append("idEstado", idEstado);
    form.append("iniciadorId", iniciadorId);
    form.append("jurisdiccionText", jurisdiccionText);
    form.append("localidadText", localidadText);
    form.append("prioridadText", prioridadText);
    form.append("tipoTramiteText", tipoTramiteText);
    form.append("objetoTramiteText", objetoTramiteText);
    form.append("estadoText", estadoText);
    form.append("iniciadorText", iniciadorText);

    $.ajax({
        type: 'POST',
        url: BASE_URL + 'MesaEntradas/GenerarListadoGeneralTramites',
        data: form,
        contentType: false,
        processData: false,
        success: function () {
            window.open(BASE_URL + 'MesaEntradas/AbrirListadoGeneralTramites', "_blank");
        },
        error: function (err) {
            if (err.status === 404) {
                mostrarMensaje("Listado general de trámites", "No se pudo obtener el listado general de trámites", 3);
            } else {
                mostrarMensaje("Listado general de trámites", "No se pudo obtener el listado general de trámites", 3);
            }
        },
        complete: hideLoading
    });

});

$("#persona-search").click(function () {
    buscarPersonas()
        .then(function (data) {
            if (data) {
                var result = JSON.parse(data.Result).response.docs;
                if (result) {
                    $.post(BASE_URL + "PersonaExpedienteObra/GetPersona/" + result[0].id, function (data) {
                        $("#txtIniciador").val(data.NombreCompleto);
                        $("#hdnIdPersona").val(data.PersonaInmuebleId);
                    });
                }
            } else {
                mostrarMensaje(false, 'Buscar Personas', 'No se ha seleccionado ninguna persona.');
                return;
            }
        })
        .catch(function (err) {
            console.log(err);
        });
});
function buscarPersonas() {
    return new Promise(function (resolve) {
        $("#hfresultadobusqueda").val('');
        BuscadorGenerico(
            [Buscador_Per], //tipos
            "buscador-generico", //ubicacion
            "hfresultadobusqueda", //devolucion
            "Personas",
            "Descripcion",
            function () { //OK
                $.get(BASE_URL + 'BuscadorGenerico/GetElements', "elements=" + $("#hfresultadobusqueda").val() + "&tipos=" + Buscador_Per,
                    function (data) {
                        $("#buscador-generico").empty();
                        resolve(data);
                    });
            },
            function () { resolve('cancelado'); },
            false);
    });
}

function mostrarMensaje(title, description, tipo) {
    var modal = $('#mensajeModalGeneracionListadoGeneralTramites');
    var alertaBackground = $('div[role="alert"]', modal);
    $('.modal-title', modal).html(title);
    $('.modal-body p', modal).html(description);
    var cls = 'alert-success';
    if (tipo === 2) {
        cls = 'alert-warning';
    } else if (tipo === 3) {
        cls = 'alert-danger';
    }
    alertaBackground.removeClass('alert-warning alert-success alert-danger').addClass(cls);
    modal.modal('show');
}
function ajustarmodal() {
    var altura = $(window).height() - 150; //value corresponding to the modal heading + footer
    $(".generacionListadoGeneralTramites-body").css({ "height": altura, "overflow-y": "auto" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $('#generacionListadoGeneralTramites-content').css({ "max-height": $('#generacionListadoGeneralTramites-content').parent().height() + 'px', "height": "100%" });
    $('#generacionListadoGeneralTramites-content').getNiceScroll().resize();
    $('#generacionListadoGeneralTramites-content').getNiceScroll().show();
}
//# sourceURL=generarListadoGralTramites.js