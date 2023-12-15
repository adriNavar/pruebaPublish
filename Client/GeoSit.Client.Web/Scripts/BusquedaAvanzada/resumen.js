$('#myModalBA').on('shown.bs.modal', function (e) {
    hideLoading();
});
$(document).ready(function () {

    $("#formVolver").ajaxForm({
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#formVolver1").ajaxForm({
        success: function (data) {
            $("#contenedor-ventana").html(data);
        },
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#formVolver2").ajaxForm({
        success: function (data) {
            $("#contenedor-ventana").html(data);
        },
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#formVolver3").ajaxForm({
        success: function (data) {
            $("#contenedor-ventana").html(data);
        },
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#formVolver4").ajaxForm({
        success: function (data) {
            $("#contenedor-ventana").html(data);
        },
        error: function () {
            alert("error al volver a editar");
        },
        complete: hideLoading
    });

    $("#form-volver-componente").ajaxForm({
        success: function (data) {
            reload(data);
        },
        error: function () {
            alert("error al volver");
        }
    });
    $("#form-volver-atributos").ajaxForm({
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#form-volver-filtros").ajaxForm({
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#form-volver-visualizacion").ajaxForm({
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#form-Resumen").ajaxForm({
        success: function () {
            $('#myModalBA').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            //Obtener extents del mapa tematico generado
            //TODO RA Extent
            $.ajax({
                url: BASE_URL + "MapasTematicos/GetMapaTematicoExtents",
                //data: { id: response.Id, componenteId: response.ComponenteId, existe: response.Existe },
                dataType: 'json',
                type: 'POST',
                success: function (data) {
                    GeoSIT.GenerarMapaTematico(data, function ()
                    {
                        if (data) {
                            var aCoords = data.split(";");
                            if (aCoords.length > 3) {
                                var xmin = aCoords[0];
                                var ymin = aCoords[1];
                                var xmax = aCoords[2];
                                var ymax = aCoords[3];
                                GeoSIT.ZoomExtents(xmin, ymin, xmax, ymax);
                            }
                        }
                    });
                }, error: function (error) {
                    setTimeout(function () {
                        alerta('Búsqueda Avanzada - Eliminar', `Ha ocurrido un error al generar la búsqueda.<br>Error: ${error.status}`, 3);
                    }, 200);
                },
                complete: hideLoading
            });
        },
        error: function () {
            alert("error al finalizar");
        }
    });
    $('#form-Resumen').submit(function (evt) {
        /*evita el doble submit*/
        showLoading();
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });
    $('#formVolver').submit(function (evt) {
        /*evita el doble submit*/
        showLoading();
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });
    $('#formVolver1').submit(function (evt) {
        /*evita el doble submit*/
        showLoading();
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });
    $('#formVolver2').submit(function (evt) {
        /*evita el doble submit*/
        showLoading();
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });
    $('#formVolver3').submit(function (evt) {
        /*evita el doble submit*/
        showLoading();
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });
    $('#formVolver4').submit(function (evt) {
        /*evita el doble submit*/
        showLoading();
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $('#myModalBA').on('shown.bs.modal', function (e) {
        ajustarScrollBars();
        hideLoading();
    });
    ///////////////////// Scrollbars ////////////////////////
    $("#scroll-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content').resize(ajustarScrollBars);
    ////////////////////////////////////////////////////////
    ajustarmodal();

    var mensaje = $('#hfMensaje').val();
    if (mensaje.length > 0) {
        alert(mensaje);
    }

    //$("#mapName").val(GeoSIT.Mapa.GetName());
    //$("#mapSession").val(GeoSIT.Mapa.GetSession());

    $("#porAtributo").on('click', function () {
        $("#listo").attr("disabled", "disabled");
        $("#capasuperior").removeClass("invisible");
        $("#capasuperior").addClass("visible");
        $("#ModalAtributo").show();
    })

    $("#cerrar").click(function () {
        var mensaje = confirm("¿Desea Salir?");
        if (mensaje == true) {
            //window.location.href = ("@Href("~/")");
            $("#myModalBA").modal('hide');
        }
        else { return false };
    });

    $("#atras").click(function () {
        $("#myModalBA").modal('hide');
    });

    $("#cancelar").click(function () {
        $("#OperacionesUL").empty();
        $("#AtributoUL").empty();
        $("#ComponenteUL li").css("background-color", "transparent").find("input[type=checkbox]").prop('checked', false);
        $("#capasuperior").removeClass("visible");
        $("#capasuperior").addClass("invisible");
        $("#ModalAtributo").hide();
    });

    $("#listo").click(function () {
        $("#capasuperior").removeClass("visible");
        $("#capasuperior").addClass("invisible");
        $("#ModalAtributo").hide();

        var valores = "";
        $(".OperacionId:checked").parent().parent().find(".valores input").each(function (i, elem) {
            if (valores.length > 0) {
                valores = valores.concat(" y ");
            }
            valores = valores.concat('<span style="color:#3788A5">' + $(this).val() + '</span>');
        });
        $("#filtros").append('<li style="width:100%; height:78px; margin-top: 20px">' +
                '<div class="col-lg-1 col-xs-1 col-sm-1 col-md-1" style="background: #337ab7; height:78px; width:78px;  padding: 1.5em"> ' +
                    '<img src="../../Content/images/MapasTematicos/icons/light/32/atributo.png" /> ' +
                '</div>' +
               '<div class="col-lg-1 col-xs-1 col-sm-1 col-md-1" style=" white-space: nowrap; line-height:78px" > ' +
                '<span style="color:#3788A5">' + $(".AtributoId:checked").attr("name") + "</span> Por " +
                '<span style="color:#3788A5">' + $(".ComponenteId:checked").attr("name") + "</span> " +
                 $(".OperacionId:checked").attr("name") + " " + valores +

                '</div>' +
                '<div class="col-lg-1 col-xs-1 col-sm-1 col-md-1 pull-right" style=" white-space: nowrap; line-height:78px" > ' +
                '<img src="~/Content/images/MapasTematicos/icons/light/16/activo.png" />   ' +
                '<img src="~/Content/images/MapasTematicos/icons/light/16/editar.png" />   ' +
                '<img id="eliminar" src="~/Content/images/MapasTematicos/icons/light/16/cancelar.png" />' +
                '</div>' +
                '</li>');

        $("#OperacionesUL").empty();
        $("#AtributoUL").empty();
        $("#ComponenteUL li").css("background-color", "transparent").find("input[type=checkbox]").prop('checked', false);
    });


    $("#filtros").on('click', '#eliminar', function () {
        $(this).parent().parent().remove();
    });

    //recupera los atributos al cambiar el componente
    $("#ComponenteUL").on('click', 'li', function () {
        $("#listo").attr("disabled", "disabled");
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        $(this).find("input[type=checkbox]").prop('checked', true);
        $("#AtributoUL").empty();
        $("#OperacionesUL").empty();
        $.ajax({
            type: 'POST',
            //url: '@Url.Action("GetAtributosCombo")',
            url: BASE_URL + 'MapasTematicos/GetAtributosCombo',
            dataType: 'json',
            data: { id: $(".ComponenteId:checked").val() },
            success: function (atributos) {
                $.each(atributos, function (i, atributo) {
                    $("#AtributoUL")
                    .append('<li>' +
                            '<div id="' + atributo.Value + '" class="row" style="margin-left: 10px; margin-right: 0px;">' +
                            '<input type="checkbox" class="AtributoId" style="display:none" name="' + atributo.Text + '" value="' + atributo.Value + '" />' +
                            '<h5><label>' + atributo.Text + '</label></h5>' +
                             '</div></li>');
                });

            }, error: function (ex) {
                alert('Error al recuperar Atributos' + ex);
            }
        });
        return false;
    });

    //recupera las agrupaciones al elegir un componente distinto al primero.
    $("#AtributoUL").on('click', 'li', function () {
        $("#listo").attr("disabled", "disabled");
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        $(this).find("input[type=checkbox]").prop('checked', true);

        $("#OperacionesUL").empty();

        $.ajax({
            type: 'POST',
            //url: '@Url.Action("GetOperacionesCombo")',
            url: BASE_URL + 'MapasTematicos/GetOperacionesCombo',
            dataType: 'json',
            data: { id: $(".ComponenteId:checked").val() },
            success: function (agrupaciones) {

                $.each(agrupaciones, function (i, agrupacion) {
                    var campos = '<div class="pull-right valores" style="white-space:nowrap;width:60%">';
                    for (var i = 0; i < agrupacion.CantidadValores; i++) {
                        campos = campos.concat('<input id="valor_' + i + '"  class="subrayado" style="width:35%; text-align:center" type="text" placeholder=""/> ');
                        if ((i + 1) < agrupacion.CantidadValores) {
                            campos = campos.concat(" Y ");
                        }
                    }
                    campos = campos.concat("</div>");
                    $("#OperacionesUL")
                        .append('<li>' +
                                    '<div id="' + agrupacion.TipoOperacionId + '" class="row" style="margin-left: 10px; margin-right: 0px;white-space:nowrap; ">' +
                                        '<div class="col-lg-1 col-xs-1 col-sm-1 col-md-1 pull-left" style="white-space:nowrap; width:40%">' +
                                            '<input type="checkbox" class="OperacionId"  style="display:none" name="' + agrupacion.Nombre + '" value="' + agrupacion.TipoOperacionId + '" />' +
                                            '<h5><label>' + agrupacion.Nombre + '</label></h5>' +
                                        '</div>' +
                                        campos +
                                    '</div>' +
                               '</li>');
                });

            }, error: function (ex) {
                alert('Error al recuperar Operaciones' + ex);
            }
        });

        return false;
    });

    $("#OperacionesUL").on('click', 'li', function () {
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        $(this).find("input[type=checkbox]").prop('checked', true);
        //                if ($('#OperacionId:checked').length > 0) {
        $("#listo").removeAttr('disabled');
        //              }
    });

    $('#guardarBiblioteca').click(function () {
        showLoading();
        var nombreMT = $('#tituloNombre').val();
        var descripcionMT = $('#descripcion').val();
        jqxhr = $.ajax({
            type: "POST",
            url: BASE_URL + "MapasTematicos/GuardarBiblioteca",
            data: { nombre: nombreMT, descripcion: descripcionMT },
            dataType: 'json',
            success: function (response) {
                hideLoading();
                if (response == 1) {
                    alert("Se guardó la biblioteca correctamente.");
                }
                else {
                    alert("Ya existe una definición con dicho nombre. Cambie el nombre para continuar");
                }
            },
            error: function (error) {
                hideLoading();
                alert("Error al guardar la biblioteca: Status " + error.status);
            }
        });
    });
    $('#guardarColeccion').click(function () {
        showLoading();
        var nombreMT = $('#tituloNombre').val();
        jqxhr = $.ajax({
            type: "POST",
            url: BASE_URL + "MapasTematicos/GuardarColeccion",
            data: { nombre: nombreMT },
            dataType: 'json',
            success: function (response) {
                hideLoading();
                alert("Se guardó la colección correctamente.");
            },
            error: function (error) {
                hideLoading();
                alert("Error al guardar la colección: Status " + error.status);
            }
        });

    });
    $('#exportarExcel').click(function () {
        IS_DOWNLOAD = true;
        window.location = BASE_URL + 'MapasTematicos/ExportarExcelResMT';
        return false;
    });

    $('#generarPDF').click(function () {
        alert("llamada a generar ploteos predefinidos");
    });

    //Previene que avancen con Enter
    $(window).keydown(function (event) {
        if (event.keyCode == 13 && $("#myModalBA").css('display') != 'none') {
            event.preventDefault();
            return false;
        }
    });

    $("#volver").click(function () {
        $("#formVolver").submit();

    });
    $("#editarComponente img").click(function () {
        $("#form-volver-componente").submit();
    });
    $("#editarAtributo img").click(function () {
        $("#form-volver-atributos").submit();
    });

    $("#editarFiltro img").click(function () {
        $("#form-volver-filtros").submit();
    });
    $("#editarVisualizacion img").click(function () {
        $("#form-volver-visualizacion").submit();
    });
    $("#myModalBA").modal("show");
});
function reload(data) {
    $('#myModalBA').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    $("#contenido").html(data);
}
function ajustarmodal() {
    var altura = $(window).height() - 60; //value corresponding to the modal heading + footer
    var ancho = $(window).width() - 150; //value corresponding to the modal heading + footer
    $(".modal-content").css({ "height": altura, "overflow": "hidden" });
    $(".modal-content").css({ "width": ancho });
    $(".modal-dialog").css({ "height": altura });
    $(".modal-dialog").css({ "width": ancho });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $("#scroll-content").getNiceScroll().resize();
    $("#scroll-content").getNiceScroll().show();
}