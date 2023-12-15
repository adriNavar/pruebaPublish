
$(window).keydown(function (event) {
    if (event.keyCode == 13) {
        event.preventDefault();
        return false;
    }
});

$(window).resize(ajustamodal);
function ajustamodal() {
    var altura = $(window).height() - 160; //value corresponding to the modal heading + footer
    $(".modal-body-scroll").css({ "height": altura, "overflow-y": "auto"});
}

var listaAtributo = [];
var listaDescripcion = [];
var listaTipoDato = [];
var listaLargo = [];
var listaEditable = [];
var listaObligatorio = [];
var listaEscala = [];
var listaFiltro = [];
var listaVisible = [];
var listaEnumeracion = [];
var listaCampos = [];
var dropdown = [];

var listatabla = [];
var listaesquema = [];
var listacamporelac = [];
var listadescriptor = [];
var listaorden = [];

$(document).ready(function () {
    $('#myModalTA').on('shown.bs.modal', function (e) {
        $('#Grilla_descripcion').dataTable().api().columns.adjust();
    });

    $('#myModalTA').modal('show');
    //$("#myModal").draggable();
    ajustamodal();



    $("#btnCancelarInfo").click(function () {
        $("#ModalInfo").modal('hide');
        //   window.location.href = "@Url.Action("TablasAuxiliares", "Mantenimiento")"
    });

    $("#btnCerrarErrorAM").click(function () {
        $("#ModalErrorAM").modal('hide');
    });



    function FiltrarGrilla() {

        $('#Grilla_descripcion').DataTable().search(
                $('#Filtrar').val()
        ).draw();

    }




    $('#Grilla_descripcion').dataTable({
        "scrollY": "200px",
        //"scrollX": true,
        "scrollCollapse": true,
        "paging": false,
        "searching": true,
        "bInfo": false,
        "destroy": true,
        "aaSorting": [[1, 'asc']],
        "language": {
            "url": BASE_URL +"Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [
            {
                "targets": 'no-sort',
                "orderable": false,
            }
        ]
    });



    $(".id_descripcion").click(function () {

        $("#Flecha_Tipo_Valuacion3").show();
        $("#Flecha_Tipo_Valuacion2").show();
        $("#accordion_panel_2").addClass("in");
        $("#Flecha_Tipo_Valuacion2").removeClass("glyphicon-triangle-bottom");
        $("#Flecha_Tipo_Valuacion2").addClass("glyphicon-triangle-top");
        $("#PanelResultado").removeClass("panel-desactivado");
        //$('#Grilla_descripcion').DataTable().columns.adjust().draw();

        var agregar = $(this).parent().find("#IdAgregar").val();
        var eliminar = $(this).parent().find("#IdEliminar").val();
        var modificar = $(this).parent().find("#IdModificar").val();

        if (agregar == "1") {
            $("#btn_AgregarResultado").removeClass("btn_desabilitado")
            $("#btn_AgregarResultado").addClass("btn_abilitado")
            $('#btn_AgregarResultado').removeAttr("disabled");
        } else {
            $("#btn_AgregarResultado").addClass("btn_desabilitado")
            $("#btn_AgregarResultado").removeClass("btn_abilitado")
            $('#btn_AgregarResultado').attr("disabled", "disabled");
        }

        if (eliminar == "1") {
            $("#btn_EliminarResultado").removeClass("btn_desabilitado")
            $("#btn_EliminarResultado").addClass("btn_abilitado")
            $("#btn_EliminarResultado").addClass("btn_eliminar_desabilitado")
            $('#btn_EliminarResultado').removeAttr("disabled");
        } else {
            $("#btn_EliminarResultado").addClass("btn_desabilitado")
            $("#btn_EliminarResultado").removeClass("btn_abilitado")
            $("#btn_EliminarResultado").addClass("btn_eliminar_desabilitado")
            $('#btn_EliminarResultado').attr("disabled", "disabled");
        }

        if (modificar == "1") {
            $("#btn_ModificarResultado").removeClass("btn_desabilitado")
            $("#btn_ModificarResultado").addClass("btn_abilitado")
            $("#btn_ModificarResultado").addClass("btn_modificar_desabilitado");
            $('#btn_ModificarResultado').removeAttr("disabled");
        } else {
            $("#btn_ModificarResultado").addClass("btn_desabilitado")
            $("#btn_ModificarResultado").removeClass("btn_abilitado")
            $("#btn_ModificarResultado").addClass("btn_modificar_desabilitado");
            $('#btn_ModificarResultado').attr("disabled", "disabled");
        }


        $.ajax({
            url: BASE_URL + 'Mantenimiento/GetAtributosByComponente',
            data: { IdComponente: $(".id_descripcion:checked").val() },
            dataType: 'json',
            success: function (data) {
                
                $('#Grilla_asignacion tbody').html('');

                $('#contedor-resultado').html('<table id="Grilla_resultado" class="table table-striped table-bordered table-responsive" cellspacing="0" style="width:100%;">'
                                                    + '<thead><tr><th class="no-sort"></th></tr></thead>'
                                                    + '<tbody></tbody>'
                                              + '</table>');

                listaAtributo = [];
                listaDescripcion = [];
                listaTipoDato = [];
                listaLargo = [];
                listaEditable = [];
                listaObligatorio = [];
                listaEscala = [];
                listaFiltro = [];
                listaVisible = [];
                listaCampos = [];
                listaEnumeracion = [];
                dropdown = [];

                listatabla = [];
                listaesquema = [];
                listacamporelac = [];
                listadescriptor = [];

                listaorden = [];

                $.each(data, function (i, atributo) {

                    if (atributo.Es_Visible == 1) {

                        $('#Grilla_resultado thead tr').append('<th>' + atributo.Campo + '</th>');

                        listaAtributo.push(atributo.Campo);
                        listaDescripcion.push(atributo.Descripcion);
                        listaTipoDato.push(atributo.Id_Tipo_Dato);
                        listaLargo.push(atributo.Precision);
                        listaEditable.push(atributo.Es_Editable);
                        listaCampos.push(atributo.Campo);
                        listaObligatorio.push(atributo.Es_Obligatorio);
                        listaEscala.push(atributo.Escala);
                        listaFiltro.push(atributo.Es_Filtro);
                        listaVisible.push(atributo.Es_Visible);
                        dropdown = atributo.Enumeracion.split(';');
                        listaEnumeracion.push(dropdown);

                        listatabla.push(atributo.Tabla);
                        listaesquema.push(atributo.Esquema);
                        listacamporelac.push(atributo.Campo_Relac);
                        listadescriptor.push(atributo.Descriptor);

                        listaorden.push(atributo.Orden);

                    }

                });

                $.ajax({
                    url: BASE_URL + 'Mantenimiento/GetGrillaAtributos',
                    data: { IdComponente: $(".id_descripcion:checked").val() },
                    dataType: 'html',
                    success: function (data) {

                        //$('#Grilla_resultado tbody').html('');
                        //$('#Grilla_resultado').dataTable().fnDestroy();

                        $('#Grilla_resultado tbody').html(data);

                        //var table = $('#Grilla_resultado').DataTable();

                        $('#Grilla_resultado').DataTable({
                            "scrollY": "200px",
                            "scrollX": true,
                            "scrollCollapse": true,
                            "paging": false,
                            "searching": true,
                            "bInfo": false,
                            "destroy": true,
                            "aaSorting": [[1, 'asc']],
                            "language": {
                                "url": BASE_URL +"Scripts/dataTables.spanish.txt"
                            },
                            "columnDefs": [
                                {
                                    "targets": 'no-sort',
                                    "orderable": false,
                                }
                            ]
                        });

                        AsignarEventos();

                        //$('#Grilla_resultado').DataTable().fnDraw();


                        //$('#Grilla_resultado tbody').html(data);


                        //$('#Grilla_resultado').DataTable().columns.adjust().draw();

                    },
                    error: function (error) {
                        alert(error.responseText);
                    }
                });
            },
            error: function (error) {
                alert(error.responseText);
            }
        });



    });
    var contador = 0;

    function AsignarEventos() {
        $("#Grilla_resultado").on("click", ".id_resultado", function () {

            //$("#accordion_panel_3").addClass("in");
            //$("#Flecha_Tipo_Valuacion3").removeClass("glyphicon-triangle-bottom");
            //$("#Flecha_Tipo_Valuacion3").addClass("glyphicon-triangle-top");
            //$("#PanelAsignacion").removeClass("panel-desactivado");
            $("#TablasAuxiliares_TablaID").val($(".id_resultado:checked").val());
            $("#btn_EliminarResultado").removeClass("btn_eliminar_desabilitado");
            $("#btn_ModificarResultado").removeClass("btn_modificar_desabilitado");

            $.ajax({
                url: BASE_URL + 'Mantenimiento/GetAtributoValores',
                data: { IdComponente: $(".id_descripcion:checked").val(), IdTabla: $(".id_resultado:checked").val() },
                dataType: 'json',
                success: function (data) {
                    // $('#Grilla_asignacion tbody').html('');
                    $('#contenedor_asignacion').html(' <table id="Grilla_asignacion" class="table table-striped table-bordered table-responsive" cellspacing="0" style="width:100%">'
                                           + '<thead>'
                                                + ' <tr>'
                                                    + ' <th>Descripción</th>'
                                                    + ' <th>Atributo</th>'
                                                    + ' <th>Valor</th>'
                                                    + ' <th>Orden</th>'
                                                + ' </tr>'
                                            + ' </thead>'
                                            + ' <tbody></tbody>'
                                        + ' </table>');
                    var j = 0;
                    $.each(data, function (key, atributo) {

                        var editable = '';
                        if (listaEditable[j] == 0) {
                            editable = 'readonly="readonly"'
                        }


                        opciones = '<option value="">Seleccione una opción</option>';
                        if (listaVisible[j] == 1) {

                            $('#Grilla_asignacion tbody').append('<tr></tr>');
                            $('#Grilla_asignacion tbody tr:last').append('<td>' + listaDescripcion[j] + '</td>');
                            $('#Grilla_asignacion tbody tr:last').append('<td>' + listaAtributo[j] + '<input type="hidden"  name="model.TablasAuxiliares.CamposTablas" value="' + listaCampos[j] + '" \></td>');


                            if (listaEnumeracion[j].length > 1) {
                                var opciones = "";
                                for (var i = 0; i < listaEnumeracion[j].length; i++) {
                                    opciones += '<option value="' + listaEnumeracion[j][i] + '">' + listaEnumeracion[j][i] + '</option>';
                                }
                                $('#Grilla_asignacion tbody tr:last').append('<td><select class="form-control sin-padding centrado" id="Dropdow' + [j] + '" name="model.TablasAuxiliares.ValoresTablas"">' +
                                                                                                opciones +
                                                                                         '</select>' +
                                                                                  '</td>');

                            } else {
                                if (listatabla[j] != null) {

                                    var tabla_relacion = listatabla[j];
                                    var esquema_relacion = listaesquema[j];
                                    var campo_relacion = listacamporelac[j];
                                    var descripcion_relacion = listadescriptor[j];

                                    opciones = '<option value="">Seleccione una opción</option>';
                                    $.ajax({
                                        async: false,
                                        cache: false,
                                        url: BASE_URL + 'Mantenimiento/GetAtributoRelacionado',
                                        data: { tabla_relacion: tabla_relacion, esquema_relacion: esquema_relacion, campo_relacion: campo_relacion, descripcion_relacion: descripcion_relacion },
                                        dataType: 'json',
                                        success: function (data) {
                                            $.each(data, function (key, atributoRelacionado) {
                                                if (atributoRelacionado.Id_Atributo == atributo) {
                                                    opciones += '<option value="' + atributoRelacionado.Id_Atributo + '" selected>' + atributoRelacionado.Descripcion + '</option>';
                                                } else {
                                                    opciones += '<option value="' + atributoRelacionado.Id_Atributo + '">' + atributoRelacionado.Descripcion + '</option>';
                                                }

                                            })

                                            $('#Grilla_asignacion tbody tr:last').append('<td><select class="form-control sin-padding centrado" id="Dropdow' + [j] + '" name="model.TablasAuxiliares.ValoresTablas"">' +
                                                                                                               opciones +
                                                                                                        '</select>' +
                                                                                                 '</td>');

                                        }

                                    });


                                } else {
                                    if (listaTipoDato[j] == 9 || listaTipoDato[j] == 10) {
                                        $('#Grilla_asignacion tbody tr:last').append('<td><textarea class="form-control valores esXML" cols="35" columns="40" id="asignacionArea' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" rows="5"  ' + editable + ' >' + atributo + '</textarea></td>');
                                    } else if (listaTipoDato[j] == 6) {

                                        $('#Grilla_asignacion tbody tr:last').append('<td><textarea class="form-control valores" cols="35" columns="40" id="asignacionArea' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" rows="5"  ' + editable + ' >' + atributo + '</textarea></td>');
                                        //$('#Grilla_asignacion tbody tr').append('<td>' + atributo.listaAtributo + '</td>');
                                    } else if (listaTipoDato[j] == 1 || listaTipoDato[j] == 11) {

                                        var chr_asignacion = "";


                                        if (atributo == 1) {
                                            chr_asignacion = '<td><input class="form-control valores chk_asignacion" id="asignacionCheckbox' + [j] + '" name="name_asignacionCheckbox' + [j] + '" value="' + atributo + '" checked="checked" type="checkbox" ' + editable + ' >';
                                        } else {
                                            chr_asignacion = '<td><input class="form-control valores chk_asignacion" id="asignacionCheckbox' + [j] + '" name="name_asignacionCheckbox' + [j] + '" value="' + atributo + '"  type="checkbox" ' + editable + ' >';
                                        }

                                        chr_asignacion += '<input id="asignacion_hidden_chk' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" value="' + atributo + '" type="hidden" ></td>';

                                        $('#Grilla_asignacion tbody tr:last').append(chr_asignacion);

                                    } else {

                                        if (listaTipoDato[j] == 5) {
                                            $('#Grilla_asignacion tbody tr:last').append('<td><input class="form-control valores fecha" id="asignacionText' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" value="' + atributo + '" type="text" ' + editable + ' ></td>');
                                        } else {
                                            $('#Grilla_asignacion tbody tr:last').append('<td><input class="form-control valores" id="asignacionText' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" value="' + atributo + '" type="text" ' + editable + ' ></td>');
                                        }
                                    }
                                }
                            }


                        }
                        $('#Grilla_asignacion tbody tr:last').append('<td>' + listaorden[j] + '</td>');
                        j++

                    });


                    $('#Grilla_asignacion').dataTable({
                        "scrollY": "200px",
                        "scrollCollapse": true,
                        "paging": false,
                        "searching": true,
                        "bInfo": false,
                        "destroy": true,
                        "aaSorting": [[3, 'asc']],
                        "language": {
                            "url": BASE_URL +"Scripts/dataTables.spanish.txt"
                        },
                        "columnDefs": [
                            {
                                "targets": 'no-sort',
                                "orderable": false,
                            },
                            {
                                "targets": [3],
                                "visible": false
                            }
                        ]
                    });
                    $('.fecha').datetimepicker({
                        useCurrent: false,
                        locale: 'es',
                        format: 'DD/MM/YYYY HH:mm:ss',
                        //widgetPositioning: { horizontal: 'left', vertical: 'top' },
                    }).on(
                      'show', function () {
                            // Obtener valores actuales z-index de cada elemento
                          var zIndexModal = $('#myModalTA').css('z-index');
                            var zIndexFecha = $('.datepicker').css('z-index');

                            alert(zIndexModal + zIndexFEcha);
                    });
                },
                error: function (error) {
                    alert(error.responseText);
                }
            });
        });
    }

    AsignarEventos();

    $("#btnGrabar").click(function () {

        //$('#TituloAdvertenciaSubmit').html("Advertencia - Alta");
        //var msj = "Confirma que desea guardar este registro ?";
        //$('#DescripcionAdvertenciaSubmit').html(msj);
        //alert($('.esXML').val())
        var nVerificacionTipo = '';
        if ($('.esXML').val() != undefined) {
            nVerificacionTipo = validateXML($('.esXML').val());
        }
        if (nVerificacionTipo == 'Sin Errores' || nVerificacionTipo == '') {
            $("#ModalAdvertenciaSubmit").modal('show');
        } else {
            $('#TituloErrorAM').html("Información - Error")
            $('#DescripcionErrorAM').html(nVerificacionTipo)
            $("#ModalErrorAM").modal('show');
            return false;
        };

    });

    $("#btnCerrarSubmit").click(function () {
        $("#ModalAdvertenciaSubmit").modal('hide');

    });

    $("#btnAgregarCheck").click(function () {
        var nVerificacionTipo = '';
        if ($('.esXML').val() != undefined) {
            nVerificacionTipo = validateXML($('.esXML').val());
        }
        if (nVerificacionTipo == 'Sin Errores' || nVerificacionTipo == '') {
            var w = 0;
            var arraytabla = new Array();
            var proximo = 0;
            var error = false;
            var inputs = "";
            arraytabla[0] = '<input type="radio" value="" class="" id="" name="" readonly="readonly"></td>';
            $("#Grilla_asignacion tbody tr td :input").each(function (i, elem) {

                if (i % 2 == 0) {
                    inputs += '<input type="text" name="model.TablasAuxiliares.CamposTablasAgregar[' + contador + '][' + w + ']" value="' + $(elem).val() + '" />';
                    proximo = listaCampos.indexOf($(elem).val());

                } else {
                    if (listaObligatorio[proximo] == 1 && $(elem).val() == "") {

                        $('#TituloErrorAM').html("Información - Error")
                        $('#DescripcionErrorAM').html("El Campo " + listaCampos[proximo] + " es OBLIGATORIO.")
                        $("#ModalErrorAM").modal('show');

                        error = true;
                    }
                    arraytabla[proximo + 1] = $(elem).val();
                    inputs += '<input type="text" name="model.TablasAuxiliares.ValoresTablasAgregar[' + contador + '][' + w + ']" value="' + $(elem).val() + '" />';
                    w++;
                }

            });
            if (error) {

                return false;
            }
            $("#valores").append(inputs);
            //alert(arraytabla);
            var tabla = $('#Grilla_resultado').dataTable();
            tabla.fnAddData(arraytabla);
            $('#Grilla_resultado').DataTable().draw();
            contador++;
            $("#btn_AgregarResultado").click();

        } else {
            //   nVerificacionTipo = "Error de Formato en el campo XML/XSD";
            $('#TituloErrorAM').html("Información - Error")
            $('#DescripcionErrorAM').html(nVerificacionTipo)
            $("#ModalErrorAM").modal('show');
            return false;
        };

    });

    $("#btnCancelarAgregar").click(function () {
        var msj = "Los datos ingresados se borraran";
        $('#TituloAdvertenciaAgregar').html("Advertencia - Cancelar Operación");
        $('#DescripcionAdvertenciaAgregar').html(msj);
        $("#ModalAdvertenciaAgregar").modal('show');
        $("#btnConfirmarAdvertenciaAgregar").off('click');
        $("#btnConfirmarAdvertenciaAgregar").click(function () {
            //$("#Grilla_asignacion tbody tr td :input").val("");
            $("#btn_AgregarResultado").click();
            $("#ModalAdvertenciaAgregar").modal('hide');
        });
    });
    $("#btnCancelarModificar").click(function () {
        var msj = "Los datos ingresados se borraran";
        $('#TituloAdvertenciaAgregar').html("Advertencia - Cancelar Operación");
        $('#DescripcionAdvertenciaAgregar').html(msj);
        $("#ModalAdvertenciaAgregar").modal('show');
        $("#btnConfirmarAdvertenciaAgregar").off('click');
        $("#btnConfirmarAdvertenciaAgregar").click(function () {
            $(".valores:input:not([readonly])").val("");

            $("#ModalAdvertenciaAgregar").modal('hide');
        });
    });
    $("#btnConfirmarAdvertenciaAgregar").click(function () {
        //$("#Grilla_asignacion tbody tr td :input").val("");
        $("#btn_AgregarResultado").click();
        $("#ModalAdvertenciaAgregar").modal('hide');
    });

    $("#btnCerrarAdvertenciaAgregar").click(function () {
        $("#ModalAdvertenciaAgregar").modal('hide');
    });
    MostrarMensaje();
    $("#btnConfirmarSubmit").click(function () {
        $(".id_resultado").removeAttr("disabled");
        $(".id_descripcion").removeAttr("disabled");

        $("#formTA").attr("Action", BASE_URL + "Mantenimiento/SetAgregarRegistro");
        $("#formTA").submit();
        contador = 0;
        $("#valores").html("");
    });

    function MostrarMensaje() {

        
        if (MensajeEntrada != null && MensajeEntrada != '') {
            if (MensajeEntrada == "AltaOK") {
                $('#TituloInfo').html("Información - Alta Guardada")
                $('#DescripcionInfo').html("Los datos han sido guardados satisfactoriamente.")
                $("#ModalInfo").modal('show');
            }

            if (MensajeEntrada == "ModificacionOK") {
                $('#TituloInfo').html("Información - Modificación Guardada")
                $('#DescripcionInfo').html("Los datos han sido guardados satisfactoriamente.")
                $("#ModalInfo").modal('show');
            }

            if (MensajeEntrada.indexOf("Error") != -1) {
                //alert(MensajeEntrada)
                $('#TituloErrorAM').html("Información - Error")
                $('#DescripcionErrorAM').html("Se ha producido un error al grabar los datos. <br>" + MensajeEntrada)
                $("#ModalErrorAM").modal('show');

            }
        }
    }

    $("#swich_Filtro_Avanzdo").click(function () {

        if ($("#accordion_panel_1").hasClass("in") == true) {
            $("#accordion_panel_1").removeClass("in");
            $("#PanelFiltro").addClass("panel-desactivado");
            $("#swich_Filtro_Avanzdo").attr('src', '~/Content/images/Mantenimiento/icons/dark.blue/32/activo.png')

        } else {
            $("#accordion_panel_1").addClass("in");
            $("#PanelFiltro").removeClass("panel-desactivado");
            $("#swich_Filtro_Avanzdo").attr('src', '~/Content/images/Seguridad/icons/green/32/no.activo.png')
        }

    });

    $("#Flecha_Tipo_Valuacion2").click(function () {

        if ($("#accordion_panel_2").hasClass("in") == false) {
            $("#accordion_panel_2").addClass("in");
            $("#Flecha_Tipo_Valuacion2").removeClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion2").addClass("glyphicon-triangle-top");

        } else {
            $("#accordion_panel_2").removeClass("in");
            $("#Flecha_Tipo_Valuacion2").addClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion2").removeClass("glyphicon-triangle-top");
        }

    });

    $("#Flecha_Tipo_Valuacion3").click(function () {

        if ($("#accordion_panel_3").hasClass("in") == false) {
            $("#accordion_panel_3").addClass("in");
            $("#Flecha_Tipo_Valuacion3").removeClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion3").addClass("glyphicon-triangle-top");

        } else {
            $("#accordion_panel_3").removeClass("in");
            $("#Flecha_Tipo_Valuacion3").addClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion3").removeClass("glyphicon-triangle-top");
        }

    });

    $("#btn_AgregarResultado").click(function () {

        if ($(this).hasClass("btn_abilitado") == true) {

            $("#btnAgregarCheck").show();
            $("#btnCancelarAgregar").show();
            $("#btnCancelarModificar").hide();
            $("#accordionAsignacion").show();
            $("#accordion_panel_3").addClass("in");
            $("#Flecha_Tipo_Valuacion3").removeClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion3").addClass("glyphicon-triangle-top");
            $("#PanelAsignacion").removeClass("panel-desactivado");
            $(".id_resultado").prop("checked", false);
            // $(".id_descripcion").prop("checked", false);
            ////////////////////////////////
            $(".id_resultado").attr("disabled", "disabled");
            $(".id_descripcion").attr("disabled", "disabled");
            $("#btn_AgregarResultado").addClass("btn_desabilitado");
            $("#btn_EliminarResultado").addClass("btn_desabilitado");
            $("#btn_ModificarResultado").addClass("btn_desabilitado");
            ////////////////////////////////
            $("#accordion_panel_2").removeClass("in");
            $("#Flecha_Tipo_Valuacion2").addClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion2").removeClass("glyphicon-triangle-top");
            $("#PanelResultado").addClass("panel-desactivado");

            //$('#Grilla_asignacion tbody').html('');
            $('#contenedor_asignacion').html(' <table id="Grilla_asignacion" class="table table-striped table-bordered table-responsive" cellspacing="0" style="width:100%">'
                                           + '<thead>'
                                                + ' <tr>'
                                                    + ' <th>Descripción</th>'
                                                    + ' <th>Atributo</th>'
                                                    + ' <th>Valor</th>'
                                                    + ' <th>Orden</th>'
                                                + ' </tr>'
                                            + ' </thead>'
                                            + ' <tbody></tbody>'
                                        + ' </table>');

            var j = 0;

            $.each(listaDescripcion, function () {

                var editable = '';
                if (listaEditable[j] == 0) {
                    editable = 'readonly="readonly"'
                }

                /*
                $('#Grilla_asignacion tbody').append('<tr></tr>');
                $('#Grilla_asignacion tbody tr:last').append('<td>' + listaDescripcion[j] + '</td>');
                $('#Grilla_asignacion tbody tr:last').append('<td>' + listaAtributo[j] + '<input type="hidden"  name="model.TablasAuxiliares.CamposTablas" value="' + listaCampos[j] + '" \></td>');
                */

                $('#Grilla_asignacion tbody').append('<tr></tr>');
                $('#Grilla_asignacion tbody tr:last').append('<td>' + listaDescripcion[j] + '</td>');
                $('#Grilla_asignacion tbody tr:last').append('<td>' + listaAtributo[j] + '<input type="hidden"  name="model.TablasAuxiliares.CamposTablas" value="' + listaCampos[j] + '"  \></td>');
                if (listaEnumeracion[j].length > 1) {
                    var opciones = '<option value="">Seleccione una opción</option>';
                    for (var i = 0; i < listaEnumeracion[j].length; i++) {
                        opciones += '<option value="' + listaEnumeracion[j][i] + '">' + listaEnumeracion[i][i] + '</option>';
                    }
                    $('#Grilla_asignacion tbody tr:last').append('<td><select class="form-control sin-padding centrado" id="Dropdow' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" ' + editable + '>' +
                                                                                    opciones +
                                                                             '</select>' +
                                                                      '</td>');

                } else {
                    if (listatabla[j] != null) {

                        var tabla_relacion = listatabla[j];
                        var esquema_relacion = listaesquema[j];
                        var campo_relacion = listacamporelac[j];
                        var descripcion_relacion = listadescriptor[j];
                            
                        opciones = '<option value="">Seleccione una opción</option>';
                        $.ajax({
                            async: false,
                            cache: false,
                            url: BASE_URL + 'Mantenimiento/GetAtributoRelacionado',
                            data: { tabla_relacion: tabla_relacion, esquema_relacion: esquema_relacion, campo_relacion: campo_relacion, descripcion_relacion: descripcion_relacion },
                            dataType: 'json',
                            success: function (data) {
                                $.each(data, function (key, atributoRelacionado) {

                                    opciones += '<option value="' + atributoRelacionado.Id_Atributo + '">' + atributoRelacionado.Descripcion + '</option>';


                                })

                                $('#Grilla_asignacion tbody tr:last').append('<td><select class="form-control sin-padding centrado" id="Dropdow' + [j] + '" name="model.TablasAuxiliares.ValoresTablas"">' +
                                                                                                   opciones +
                                                                                            '</select>' +
                                                                                     '</td>');

                            }

                        });


                    } else {
                        if (listaTipoDato[j] == 9 || listaTipoDato[j] == 10) {

                            $('#Grilla_asignacion tbody tr:last').append('<td><textarea class="form-control valores esXML" cols="35" columns="40" id="asignacionArea' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" rows="5" ' + editable + '></textarea></td>');
                            //$('#Grilla_asignacion tbody tr').append('<td>' + atributo.listaAtributo + '</td>');
                        } else if (listaTipoDato[j] == 6) {
                            $('#Grilla_asignacion tbody tr:last').append('<td><textarea class="form-control valores " cols="35" columns="40" id="asignacionArea' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" rows="5" ' + editable + '></textarea></td>');
                        } else if (listaTipoDato[j] == 1 || listaTipoDato[j] == 11) {
                            chr_asignacion = '<td><input class="form-control valores chk_asignacion" id="asignacionCheckbox' + [j] + '" name="asignacionCheckbox' + [j] + '" value="" type="checkbox" ' + editable + '>';

                            chr_asignacion += '<input id="asignacion_hidden_chk' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" value="0" type="hidden" ' + editable + ' ></td>';

                            $('#Grilla_asignacion tbody tr:last').append(chr_asignacion);

                        } else {
                            if (listaTipoDato[j] == 5) {
                                $('#Grilla_asignacion tbody tr:last').append('<td><input class="form-control valores fecha" id="asignacionText' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" value="" type="text" ' + editable + '></td>');
                            } else {
                                $('#Grilla_asignacion tbody tr:last').append('<td><input class="form-control valores" id="asignacionText' + [j] + '" name="model.TablasAuxiliares.ValoresTablas" value="" type="text" ' + editable + '></td>');
                            }

                        }
                    }
                }
                $('#Grilla_asignacion tbody tr:last').append('<td>' + listaorden[j] + '</td>');
                j++
            });

            $('#TituloAdvertenciaSubmit').html("Advertencia - Alta");
            var msj = "Confirma que desea guardar este registro ?";
            $('#DescripcionAdvertenciaSubmit').html(msj);

            $("#Panel_Botones").show();

            $('#Grilla_asignacion').dataTable({
                "scrollY": "200px",
                "scrollCollapse": true,
                "paging": false,
                "searching": true,
                "bInfo": false,
                "destroy": true,
                "aaSorting": [[3, 'asc']],
                "language": {
                    "url": BASE_URL +"Scripts/dataTables.spanish.txt"
                },
                "columnDefs": [
                    {
                        "targets": 'no-sort',
                        "orderable": false,
                    },
                    {
                        "targets": [3],
                        "visible": false
                    }
                ]
            });
            $('.fecha').datetimepicker({
                useCurrent: false,
                locale: 'es',
                format: 'DD/MM/YYYY HH:mm:ss',
                //widgetPositioning: { horizontal: 'left', vertical: 'top' },

            }).on(
                'show', function () {
                    // Obtener valores actuales z-index de cada elemento
                    var zIndexModal = $('#myModalTA').css('z-index');
                    var zIndexFecha = $('.datepicker').css('z-index');

                    alert(zIndexModal + zIndexFEcha);
                });
        }
    });

    $("#btnCancelar").click(function () {

        var msj = "Los datos ingresados se borraran";
        $('#TituloAdvertenciaAM').html("Advertencia - Cancelar Operación");
        $('#DescripcionAdvertenciaAM').html(msj);
        $("#ModalAdvertenciaAM").modal('show');

    });

    $("#btn_ModificarResultado").click(function () {
        $("#btnAgregarCheck").hide();
        $("#btnCancelarAgregar").hide();
        $("#btnCancelarModificar").show();

        if ($(this).hasClass("btn_abilitado") == true && $(this).hasClass("btn_modificar_desabilitado") == false) {

            $("#accordionAsignacion").show();
            $("#accordion_panel_3").addClass("in");
            $("#Flecha_Tipo_Valuacion3").removeClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion3").addClass("glyphicon-triangle-top");
            $("#PanelAsignacion").removeClass("panel-desactivado");
            //$(".id_resultado").prop("checked", false);
            //$(".id_descripcion").prop("checked", false);
            $(".id_resultado").attr("disabled", "disabled");
            $(".id_descripcion").attr("disabled", "disabled");
            $("#accordion_panel_2").removeClass("in");
            $("#Flecha_Tipo_Valuacion2").addClass("glyphicon-triangle-bottom");
            $("#Flecha_Tipo_Valuacion2").removeClass("glyphicon-triangle-top");
            $("#PanelResultado").addClass("panel-desactivado");

            $("#Panel_Botones").show();

            $('#TituloAdvertenciaSubmit').html("Advertencia - Modificación");
            var msj = "Confirma que desea modificar este registro ?";
            $('#DescripcionAdvertenciaSubmit').html(msj);

            $('#Grilla_asignacion').dataTable({
                "scrollY": "200px",
                "scrollCollapse": true,
                "paging": false,
                "searching": true,
                "bInfo": false,
                "destroy": true,
                "aaSorting": [[3, 'asc']],
                "language": {
                    "url": BASE_URL +"Scripts/dataTables.spanish.txt"
                },
                "columnDefs": [
                    {
                        "targets": 'no-sort',
                        "orderable": false,
                    },
                    {
                        "targets": [3],
                        "visible": false
                    }
                ]
            });
        }
    });

    $("#btn_EliminarResultado").click(function () {
        if ($(this).hasClass("btn_abilitado") == true && $(this).hasClass("btn_eliminar_desabilitado") == false) {

            $('#TituloAdvertencia').html("Advertencia - Eliminar");
            var msj = "Confirma que desea eliminar este registro ?";
            $('#DescripcionAdvertencia').html(msj);
            $("#ModalAdvertencia").modal('show');
        }
    });

    $("#btnCerrarAdvertenciaAM").click(function () {
        $('#myModalTA').modal('show');
        $('#ModalAdvertenciaAM').modal('hide');
        $(".id_resultado").prop("checked", false);
        $("#btn_EliminarResultado").addClass("btn_eliminar_desabilitado")
        $("#btn_ModificarResultado").addClass("btn_modificar_desabilitado")

    });

    $("#btnConfirmarAdvertenciaAM").click(function () {
        $('#myModalTA').modal('show');
        $('#ModalAdvertenciaAM').modal('hide');
        $(".valores").val("");
        $(".id_resultado").prop("checked", false);
        $("#btn_EliminarResultado").addClass("btn_eliminar_desabilitado")
        $("#btn_ModificarResultado").addClass("btn_modificar_desabilitado")
        $("#PanelAsignacion").addClass("panel-desactivado");
        $("#accordion_panel_3").removeClass("in");
        $("#Flecha_Tipo_Valuacion3").addClass("glyphicon-triangle-bottom");
        $("#Flecha_Tipo_Valuacion3").removeClass("glyphicon-triangle-top");
        $("#Panel_Botones").hide();
        $(".id_resultado").removeAttr("disabled");
        $(".id_descripcion").removeAttr("disabled");
        $("#accordionAsignacion").hide();
        $("#Flecha_Tipo_Valuacion3").hide();
        $("#Flecha_Tipo_Valuacion2").hide();

    });

    $("#btnCerrarAdvertencia").click(function () {
        $('#myModalTA').modal('show');
        $('#ModalAdvertencia').modal('hide');
        $(".id_resultado").prop("checked", false);
        $("#btn_EliminarResultado").addClass("btn_eliminar_desabilitado")
        $("#btn_ModificarResultado").addClass("btn_modificar_desabilitado")
        $("#PanelAsignacion").addClass("panel-desactivado");
        $("#accordion_panel_3").removeClass("in");
        $("#Flecha_Tipo_Valuacion3").addClass("glyphicon-triangle-bottom");
        $("#Flecha_Tipo_Valuacion3").removeClass("glyphicon-triangle-top");
    });

    $("#btnConfirmarAdvertencia").click(function () {

        $.ajax({
            url: BASE_URL + 'Mantenimiento/GetEliminaRegistroAsignacion',
            data: { IdComponente: $(".id_descripcion:checked").val(), IdTabla: $(".id_resultado:checked").val() },
            dataType: 'html',
            success: function (data) {
                $('#ModalAdvertencia').modal('hide');
                if (data == "ok") {
                    $(".id_resultado:checked").parent().parent().remove();

                    var msj = "Se ha eliminado el registro correctamente";
                    $('#TituloInfo').html("Información - Bajas Guardadas");
                    $('#DescripcionInfo').html(msj);
                    $("#ModalInfo").modal('show');
                    $("#btn_EliminarResultado").addClass("btn_eliminar_desabilitado")
                    $("#btn_ModificarResultado").addClass("btn_modificar_desabilitado")
                    
                } else {
                    var msj = data;
                    $('#TituloInfo').html("Información");
                    $('#DescripcionInfo').html(msj);
                    $('#ModalInfo').modal('show');
                }

            },
            error: function (error) {
                alert(error.responseText);
            }
        });

    });

    $("#clearSearch").click(function () {
        $("#Filtrar").val("");
        FiltrarGrilla();
    });

    $('#Filtrar').on('keyup click', function () {
        FiltrarGrilla();
    });

    $("#btnCerrar").click(function () {

        $("#myModalTA").modal('hide');
        $(".modal-backdrop").remove();
        //window.location.href = "@Url.Action("Index", "Home")"
    });

    $(document).on('change', '.chk_asignacion', function () {

        var id = $(this).attr('id');
        nroid = id.substr(18);
        if ($(this).is(":checked") == true) {
            $('#asignacion_hidden_chk' + nroid).val('1');
        } else {
            $('#asignacion_hidden_chk' + nroid).val('0');
        }


        //alert($(this).is(":checked"))
    });
    $("#formTA").ajaxForm({
        beforeSubmit: function (arr, $form, options) {
            showLoading();
            $("#myModalTA").modal('hide');
            return true; //it will continue your submission.
        },
        success: function (data) {
            $("#contenido").empty();
            $("#contenido").html(data);
        },
        error: function () {
            alert("error");
        }
    });
    $("#formTA").submit(function (evt) {
        /*evita el doble submit*/
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $('#myModalTA').modal('show');
    hideLoading();
});

//@ sourceURL=tablasAuxiliares.js