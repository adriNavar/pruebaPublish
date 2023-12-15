$.fn.dataTableExt.oApi.fnStandingRedraw = function (oSettings) {
    //redraw to account for filtering and sorting
    // concept here is that (for client side) there is a row got inserted at the end (for an add)
    // or when a record was modified it could be in the middle of the table
    // that is probably not supposed to be there - due to filtering / sorting
    // so we need to re process filtering and sorting
    // BUT - if it is server side - then this should be handled by the server - so skip this step
    if (oSettings.oFeatures.bServerSide === false) {
        var before = oSettings._iDisplayStart;
        oSettings.oApi._fnReDraw(oSettings);
        //iDisplayStart has been reset to zero - so lets change it back
        oSettings._iDisplayStart = before;
        oSettings.oApi._fnCalculateEnd(oSettings);
    }

    //draw the 'current' page
    oSettings.oApi._fnDraw(oSettings);
};
$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-inspectores').on('shown.bs.modal', function (e) {
    $('#Grilla_Inspectores').dataTable().api().columns.adjust();
    ajustarScrollBars();
    hideLoading();
});
function EnablePanel(idPanel) {
    switch (idPanel) {
        case 1:
            $("#Panel_Botones span[aria-controls='button']").removeClass("black");
            $("#Panel_Botones span[aria-controls='button']").addClass("boton-deshabilitado");
            $("#headingCaracteristicas").removeClass("panel-deshabilitado");
            $("#headingCaracteristicas").addClass("panel-deshabilitado");
            $("#headingCaracteristicas").find("a:first[aria-expanded=true]").click();

            $("#headingInspectores").removeClass("panel-deshabilitado");
            $("#headingInspectores").find("a:first[aria-expanded=false]").click();

            editionMode(idPanel);
            Grilla_Inspectores.order([[2, 'asc']]).draw(false);
            break;
        case 2:
            $("#Panel_Botones span[aria-controls='button']").removeClass("boton-deshabilitado");
            $("#Panel_Botones span[aria-controls='button']").addClass("black");
            //$("#headingInspectores").removeClass("panel-deshabilitado");
            //$("#headingInspectores").addClass("panel-deshabilitado");
            //$("#headingInspectores").find("a:first[aria-expanded=true]").click();

            $("#headingCaracteristicas").removeClass("panel-deshabilitado");
            $("#headingCaracteristicas").find("a:first[aria-expanded=false]").click();
            //Grilla_UnidadesTributarias._fnReDraw();
            editionMode(idPanel);
            Grilla_TiposInspecciones.order([[1, 'asc']]).draw(false);
            break;
        //case 3:
        //    //if ($("#InspeccionId").val() != "0") {
        //    $(".titulo1").addClass("panel-desactivado");
        //    $(".titulo1 #Flecha_Detalle_Usuario").removeClass("glyphicon-triangle-top", 600).addClass("glyphicon-triangle-bottom", 600);
        //    $(".titulo2").addClass("panel-desactivado");
        //    $(".titulo2 #Flecha_Detalle_Usuario").removeClass("glyphicon-triangle-top", 600).addClass("glyphicon-triangle-bottom", 600);
        //    $(".titulo3").removeClass("panel-desactivado");
        //    $(".titulo3 #Flecha_Detalle_Usuario").removeClass("glyphicon-triangle-bottom", 600).addClass("glyphicon-triangle-top", 600);

        //    $("#accordion_panel_0").removeClass("in");
        //    $("#accordion_panel_1").removeClass("in");
        //    $("#accordion_panel_2").addClass("in");
        //    //Grilla_Documentos._fnReDraw();
        //    editionMode(idPanel);
        //    //}            
        //    break;
    }
    setTimeout(function () {
        ajustarmodal();
    }, 500);
}

function clearErrores() {
}


var fnResultAlerta = null;
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlerta = fn;
    $("#botones-modal-info").hide();
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $("#botones-modal-info").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#alert_message_btnSi_result").val(0);
    $("#alert_message_btnNo_result").val(0);
    $("#MensajeInfoInspector").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoInspector").html(titulo);
    $("#DescripcionInfoInspector").html(mensaje);
    $("#ModalInfoInspector").modal('show');
}

function VacirDatos() {
    $('#SelectedUsuario')[0].selectedIndex = 0;
    Grilla_TiposInspecciones.rows().eq(0).each(function (value, index) {
        Grilla_TiposInspecciones.rows(index).nodes().to$().removeClass('selected');
    });
    $("#esPlanificador").val('N');
    $("#TiposInspeccionesSelected").val('');
}
function editionMode(idPanel) {
    $("#idPanel").val(idPanel);

    if (idPanel == 1) {
        $("#editionMode").val("false")
        $("#btnGrabar").attr("src", BASE_URL + "Content/images/Seguridad/icons/blue/32/save.desactivado.png").removeClass('btnActivado').addClass('btnDesactivado');
    }
    else {
        $("#editionMode").val("true")
        $("#btnGrabar").attr("src", BASE_URL + "Content/images/Seguridad/icons/blue/32/save.png").removeClass('btnDesactivado').addClass('btnActivado');
    }
}
function refreshInspectores() {
    Grilla_Inspectores.clear().draw();
    console.log('Inicio Refresh');
    $.ajax({
        url: BASE_URL + 'ObrasParticulares/GetInspectores',
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            console.log('Refresh Success');
            if (data != null) {
                console.log('Refresh count ' + data.length);
                data.forEach(function (Inspector) {
                    var rowData = {
                        "0": Inspector.InspectorID,
                        "1": Inspector.UsuarioID,
                        "2": Inspector.Usuario.Apellido + ' ' + Inspector.Usuario.Nombre,
                        "3": Inspector.EsPlanificador
                    };
                    //alert(JSON.stringify(rowData));
                    Grilla_Inspectores.row.add(rowData);
                });

                Grilla_Inspectores.order([[2, 'asc']]).draw(false);
            }
        }, error: function (ex) {
            alert(JSON.stringify(ex));
        }
    });
}
var Grilla_Inspectores;
var Grilla_TiposInspecciones;

function init() {
    $(".inspectores-content").niceScroll(getNiceScrollConfig());
    $('#registroInspectoresModal .panel-body').resize(ajustarScrollBars);
    $('.inspectores-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".inspectores-content").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////
    ajustarmodal();
    $("#modal-window-inspectores").modal('show');
    $("#btnCerrar").click(function () {
        switch ($("#idPanel").val()) {
            case "1":
                $("#modal-window-inspectores").modal('hide');
                break;
            case "2":
            case "3":
                EnablePanel(1);
                $("#headingCaracteristicas").find("a:first[aria-expanded=false]").click();
                $("#Grilla_Inspectores tbody tr").removeClass("selected");
                break;
        }
    });

    $("#btnAceptarAlert").click(function (e) {
        $("#alert_message_btnSi_result").val(1);
        $('#ModalInfoInspecccion').hide();
        if (typeof fnResultAlerta === 'function') fnResultAlerta();
        e.preventDefault();
    });
    $("#btnCancelarAlert").click(function (e) {
        $("#alert_message_btnNo_result").val(1);
        $('#ModalInfoInspecccion').hide();
        if (typeof fnResultAlerta === 'function') fnResultAlerta();
        e.preventDefault();
    });

    Grilla_Inspectores = $('#Grilla_Inspectores').DataTable({
        "scrollY": "300px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [
            {
                "targets": 'rowId',
                "orderable": false,
                "visible": false,
                "searchable": false
            }
        ]
    });

    $('#Grilla_Inspectores tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
            $("#delInspector").addClass("boton-deshabilitado");
            $("#modInspector").addClass("boton-deshabilitado");
        }
        else {
            Grilla_Inspectores.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
            $("#delInspector").removeClass("boton-deshabilitado");
            $("#modInspector").removeClass("boton-deshabilitado");
        }

    });

    Grilla_TiposInspecciones = $('#Grilla_TiposInspecciones').DataTable({
        "scrollY": "300px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [
            {
                "targets": 'rowId',
                "orderable": false,
                "visible": false,
                "searchable": false
            }]
    });

    $('#Grilla_TiposInspecciones tbody').on('click', 'tr', function () {
        var d = Grilla_TiposInspecciones.row(this).data();

        var add = !$(this).hasClass('selected');
        var removeResult = "";
        var ssplit = $("#TiposInspeccionesSelected").val().split(',');
        var founded = false;

        ssplit.forEach(function (entry) {
            if (entry == d[0]) {
                founded = true;
            } else {
                if (!add)
                    removeResult += entry + ',';
            }

        });
        if (add) {
            if (!founded) {
                if ($("#TiposInspeccionesSelected").val() == "")
                    $("#TiposInspeccionesSelected").val(d[0]);
                else
                    $("#TiposInspeccionesSelected").val($("#TiposInspeccionesSelected").val() + ',' + d[0]);
            }
        } else {
            $("#TiposInspeccionesSelected").val(removeResult.substring(0, removeResult.length - 1));
        }


        $(this).toggleClass('selected');
    });



    $("#addInspector").click(function (e) {
        VacirDatos();
        $("#headingInspectores").find("a:first[aria-expanded=true]").click();
        $.ajax({
            url: BASE_URL + 'ObrasParticulares/GetUsuarios/?idUsuario=' + 0,
            dataType: 'json',
            type: "GET",
            success: function (data) {
                $("#SelectedUsuario").empty();
                data.forEach(function (Usuario) {
                    $("#SelectedUsuario").append('<option value="' + Usuario.Value + '">' + Usuario.Text + '</option>');
                });
                console.log(data);
                $("#idInspector").val(0);
                $("#esPlanificador").val('N');
                $("#SelectedUsuario").removeAttr('disabled');
                EnablePanel(2);
            },
            error: function () {
                //alerta('Error1', textStatus, 1);
                refreshInspectores();
                EnablePanel(1);
            }
        });


    });

    $("#delInspector").click(function (e) {
        var idInspector = Grilla_Inspectores.rows('.selected').data()[0][0];
        alerta('Consulta', '¿Desea eliminar el inspector?', 2, function () {
            if ($("#alert_message_btnSi_result").val() == "1")
                $.ajax({
                    url: BASE_URL + 'ObrasParticulares/GetInspectorRemove/?inspectorID=' + idInspector,
                    dataType: 'json',
                    type: "GET",
                    success: function () {
                        refreshInspectores();
                        EnablePanel(1);
                    },
                    error: function () {
                        refreshInspectores();
                        EnablePanel(1);
                    }
                });
        });

    });

    $("#modInspector").click(function (e) {
        editInspectorEvent();
        $("#headingInspectores").find("a:first[aria-expanded=true]").click();
        $("#headingInspectores").addClass("panel-deshabilitado");
    });

    $("#swich_Cambio_Clave").click(function () {
        if ($("#esPlanificador").val() == 'S') {
            $("#esPlanificador").val('N');
            $("#swich_Cambio_Clave").attr("src", BASE_URL + "Content/images/Seguridad/icons/blue/32/no.activo.png");
        } else {
            $("#esPlanificador").val('S');
            $("#swich_Cambio_Clave").attr("src", BASE_URL + "Content/images/Seguridad/icons/blue/32/activo.png");

        }
    });

    $("#editInspector").click(function (e) {
        editInspectorEvent();
    });
    $("#btnGrabar").click(function () {
        if ($("#SelectedUsuario").val() == "" || $("#SelectedUsuario").val() == "0") {
            alerta('Error', 'No ha seleccionado ningún usuario para dar de alta como inspector', 3);
        } else if ($("#TiposInspeccionesSelected").val() == "") {
            alerta('Error', 'Debe seleccionar al menos un tipo de inspeccion', 3);
        } else {
            var formData = {
                "InspectorID": $("#idInspector").val(),
                "UsuarioID": $("#SelectedUsuario").val(),
                "EsPlanificador": $("#esPlanificador").val(),
                "TiposInspeccionSelected": $("#TiposInspeccionesSelected").val()
            };

            $.ajax({
                url: BASE_URL + 'ObrasParticulares/PostInspectorUpdate',
                type: "POST",
                data: formData,
                success: function () {
                    alerta('Inspector', 'Se ha guardado correctamente el inspector', 1, function () {
                        refreshInspectores();
                        EnablePanel(1);
                    });

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alerta('Error', errorThrown, 3);
                }
            });
        }

    });

    EnablePanel(1);

}

function editInspectorEvent() {
    var idInspector = Grilla_Inspectores.rows('.selected').data()[0][0];
    var idUsuario = Grilla_Inspectores.rows('.selected').data()[0][1];
    var nombreInspector = Grilla_Inspectores.rows('.selected').data()[0][2];
    var esPlanificador = Grilla_Inspectores.rows('.selected').data()[0][3];

    $.ajax({
        url: BASE_URL + 'ObrasParticulares/GetUsuarios/?idUsuario=' + idUsuario,
        dataType: 'json',
        type: "GET",
        success: function (data) {
            $("#SelectedUsuario").empty();
            data.forEach(function (Usuario) {
                $("#SelectedUsuario").append('<option value="' + Usuario.Value + '">' + Usuario.Text + '</option>');
            });
            $("#SelectedUsuario").val(idUsuario);
            $("#SelectedUsuario").attr('disabled', 'disabled');
        },
        error: function () {
            refreshInspectores();
            EnablePanel(1);
        }
    });

    $("#idInspector").val(idInspector);
    $("#esPlanificador").val(esPlanificador);

    if ($("#esPlanificador").val() === 'S') {
        $("#swich_Cambio_Clave").attr("src", BASE_URL + "Content/images/Seguridad/icons/blue/32/activo.png");
    } else {
        $("#swich_Cambio_Clave").attr("src", BASE_URL + "Content/images/Seguridad/icons/blue/32/no.activo.png");
    }

    Grilla_TiposInspecciones.rows().nodes().to$().removeClass('selected');

    $.ajax({
        url: BASE_URL + 'ObrasParticulares/GetIiposInspeccionesPorInspector/?idInspector=' + idInspector + '&limitarTipos=false',
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            if (data) {
                var tiposInspeccionesVal = '';
                data.forEach(function (TipoInspeccion) {
                    tiposInspeccionesVal += TipoInspeccion.TipoInspeccionID + ',';
                    Grilla_TiposInspecciones.rows().data()
                        .each(function (valor, indice) {
                            if (parseInt(valor[0]) === TipoInspeccion.TipoInspeccionID)
                                Grilla_TiposInspecciones.rows(indice).nodes().to$().addClass('selected');
                        });
                    
                });
                Grilla_TiposInspecciones.columns.adjust();
                $("#TiposInspeccionesSelected").val(tiposInspeccionesVal.substring(0, tiposInspeccionesVal.length - 1));
            }
        }, error: function (ex) {
            $("#SelectedTipoInspeccionCal").empty();
            $("#SelectedInspectorCal").empty();
            alert(JSON.stringify(ex));
        }
    });

    EnablePanel(2);
}
function ajustarmodal() {
    var viewportHeight = $(window).height(),
        paddingBottom = 70,
        headerFooter = 150,
        altura = viewportHeight - headerFooter;
    altura -= (altura > headerFooter + paddingBottom ? paddingBottom : 0); //value corresponding to the modal heading + footer
    $(".inspectores-body", "#registroInspectoresModal").css({ "height": altura, "overflow": "hidden" });
    $('#inspectores-content').css({ "max-height": altura });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $(".inspectores-content").getNiceScroll().resize();
    $(".inspectores-content").getNiceScroll().show();
}

//@ sourceURL=inspectores.js