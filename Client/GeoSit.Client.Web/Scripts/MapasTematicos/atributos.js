$('#myModalMT').one('shown.bs.modal', function (e) {
    hideLoading();
});
$(document).ready(function () {

    $("#formVolver").ajaxForm({
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $("#contenido").html(data);
            hideLoading();
        },
        error: function (xhr) {
            alert("error al volver");
        }
    });

    $("#form-atributos").ajaxForm({
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () { $("#contenido").html(data); }, 200);
        },
        error: function (xhr) {
            alert("error en siguiente");
        }
    });

    ///////////////////// Scrollbars ////////////////////////
    $(".StyleScrolls").niceScroll(getNiceScrollConfig());
    $('.StyleScrolls').resize(ajustarScrollBars);
    ////////////////////////////////////////////////////////

    // Cambia el color del glyphicon al cargar un nuevo archivo
    $("#nuevoArchivo").click(function () {
        if ($("#pfila").is(':checked')) {
            $("#pfila2").css("color", "#0C2D3C");
        }
    });

    $("#pfila").click(function () {
        // $("#pfila2").css("color", "#00ff90");
        if ($("#pfila").is(':checked')) {
            $("#pfila2").css("color", "#09C996");
        } else {
            $("#pfila2").css("color", "#0C2D3C");
        }
    });

    $("#salir").click(function () {
        var msj = "¿Desea salir del wizard del Mapa Temático? ";
        alerta('Mapa Temático - Atributos', msj, 2, function () {
            $("#myModalMT").modal('hide');
        });
        return false;
    });

    $('body').on('click', '#btnAceptarInfoMTAtrib', function () {
        if (fnResultAlertaAtrib) fnResultAlertaAtrib();
    });

    $("#cancelar").click(function () {
        //plegardesplegar("capasuperior");
        $("#capasuperior").removeClass("visible");
        $("#capasuperior").addClass("invisible");
        $("#capaprincipal").removeClass("invisible");
        $("#capaprincipal").addClass("visible");
        $("#fileModal").hide();
        ajustarScrollBars();
    });

    // Marca la fila seleccionado habilita las opciones.
    $(document).on('click', 'tr.filaTr', function (e) {
        if (e.target.id != "estrellaid") {
            //Este if filtra el evento disparado por el radiobutton.
            var linea = $(this);
            $(".filaTr").removeClass("seleccionado");
            //$(".filaTr td input[type=text]").hide();
            linea.removeClass("hover");
            linea.addClass("seleccionado");

            $("td.seleccion input[type=checkbox]").prop('checked', false);
            linea.find("td.seleccion input[type=checkbox]").prop('checked', 'checked');
            linea.find("td input[type=text]").show();
            linea.find("td input[type=text]").focus();
        }
    });

    //recupera los atributos al cambiar el componente
    $("#ComponentesUL").on('click', 'li', function () {
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).parent().children("li").not(this).find('span').removeClass("tilde-seleccion");
        $(this).addClass("seleccionado");
        $(this).find('span').addClass("tilde-seleccion");
        $(this).find("input[type=checkbox]").prop('checked', true);

        $("#AtributosUL").empty();
        $("#AgrupacionesUL").empty();
        $("#hfComponenteId").val($(".ComponenteId:checked").val());

        $.ajax({
            type: 'POST',
            url: BASE_URL + 'MapasTematicos/GetAtributosComboImport',
            dataType: 'json',
            data: { id: $(".ComponenteId:checked").val() },
            success: function (atributos) {
                $.each(atributos, function (_, atributo) {
                    var claseimportado = "";
                    if (atributo.EsImportado) {
                        claseimportado = "importado";
                    }
                    $("#AtributosUL")
                        .append('<li class="seleccionable ' + claseimportado + '">' +
                            '<div id="' + atributo.AtributoId + '">' +
                            '<input type="checkbox" class="AtributoId" name="AtributoId" value="' + atributo.AtributoId + '" />' +
                            '<label class="h5">' + atributo.Nombre + '<span class="fa fa-check pull-right tilde-seleccion"></span></label>' +
                            '</div></li>');

                });
                var atributo = $('#hfAtributoId').val();
                if (atributo > 0) {
                    $("#AtributosUL li").find("input[value=" + atributo + "]").parents("li").click();
                } else {
                    $($("#AtributosUL li")[0]).click();
                }

                $("#buscarAtributo").val("");
            }, error: function (ex) {
                alert('Error al recuperar Atributos' + ex);
            }
        });

        return false;
    });

    //recupera las agrupaciones al elegir un componente distinto al primero.
    $("#AtributosUL").on('click', 'li', function () {
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        $(this).parent().children("li").not(this).find('span').removeClass("tilde-seleccion");
        $(this).find('span').addClass("tilde-seleccion");
        $(this).find("input[type=checkbox]").prop('checked', true);
        $("#AgrupacionesUL").empty();

        $("#hfAtributoId").val($(".AtributoId:checked").val());
        $("#hfEsAtributoImportado").val(false);

        habilitarBtnSiguiente();
        if (Number($(".ComponenteId:checked").val()) !== Number($("#hfComponentePrincipalId").val())) {
            $.ajax({
                type: 'POST',
                url: BASE_URL + 'MapasTematicos/GetAgrupacionesCombo',
                dataType: 'json',
                data: { id: $(".AtributoId:checked").val() },
                success: function (agrupaciones) {
                    $.each(agrupaciones, function (i, agrupacion) {
                        $("#AgrupacionesUL")
                            .append('<li class="seleccionable">' +
                                '<div id="' + agrupacion.Value + '">' +
                                '<input type="checkbox" class="AgrupacionId"  name="AgrupacionId" value="' + agrupacion.Value + '" />' +
                                '<label class= "h5">' + agrupacion.Text + '<span class="fa fa-check pull-right"></span></label>' +
                                '</div>' +
                                '</li>');
                    });
                    var agrupacion = Number($('#hfAgrupacionId').val());
                    if (agrupacion > 0) {
                        $("#AgrupacionesUL li").find("input[value=" + agrupacion + "]").parents("li").click();
                    }

                    habilitarBtnSiguiente();
                }, error: function (ex) {
                    alert('Error al recuperar Operaciones' + ex);
                }
            });
        } else {
            $("#hfAgrupacionId").val(null);
        }
        return false;
    });

    //habilita el boton siguiente al elegir una agrupacion.
    $("#AgrupacionesUL").on('click', 'li', function () {
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        $(this).find("input[type=checkbox]").prop('checked', true);
        $(this).parent().children("li").not(this).find('span').removeClass("tilde-seleccion");
        $(this).find('span').addClass("tilde-seleccion");
        if ($(".AtributoId:checked").length > 0) {
            $("#hfAgrupacionId").val($(".AgrupacionId:checked").val());
            habilitarBtnSiguiente();
        }
        return false;
    });

    //Previene que avancen con Enter
    $(window).keydown(function (event) {
        if (event.keyCode == 13 && $("#myModalMT").css('display') != 'none') {
            event.preventDefault();
            return false;
        }
    });

    if ($("#hfComponenteId").val() > 0) {
        $("#ComponentesUL li").find("input[value=" + $("#hfComponenteId").val() + "]").parents("li").click();
    } else {
        //inicializo el primer registro de componentes
        $($("#ComponentesUL li")[0]).click();
        $($("#AtributosUL li")[0]).click();
    }
    $("#buscar").keyup(function () {
        var texto = $("#buscar").val();
        if (texto) {
            $("#ComponentesUL li").hide();

            //dice spans pero luego del estilado son labels
            var spans = $("#ComponentesUL li").find("label");
            $.each(spans, function (i, span) {
                if ($(span).text().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(span).closest("li").show();
                }
            });

            $("#ComponentesUL li:hidden").removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
            $("#ComponentesUL li:hidden").find('span').removeClass("tilde-seleccion");

            if ($("#ComponentesUL li input[type=checkbox]:checked").length < 1) {
                $("#AtributosUL").empty();
                $("#AgrupacionesUL").empty();
            }
        } else {
            $("#ComponentesUL li").show();
            $("#buscarAtributo").val("");
        }
        habilitarBtnSiguiente();
        return false;
    });

    $("#buscarAtributo").keyup(function () {
        var texto = $("#buscarAtributo").val();
        if (texto) {
            $("#AtributosUL li").hide();

            //dice spans pero luego del estilado son labels
            var spans = $("#AtributosUL li").find("label");
            $.each(spans, function (i, span) {
                if ($(span).text().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(span).closest("li").show();
                }
            });

            $("#AtributosUL li:hidden").removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
            $("#AtributosUL li:hidden").find('span').removeClass("tilde-seleccion");

            if ($("#AtributosUL li input[type=checkbox]:checked").length < 1) {
                $("#AgrupacionesUL").empty();
            }
        } else {
            $("#AtributosUL li").show();
        }
        habilitarBtnSiguiente();
        return false;
    });

    $("#volver").click(function () {
        $("#formVolver").submit();
    });
    $("#btn-atributo-siguiente").click(function () {
        $("#form-atributos").submit();
    });

    ajustarmodal();
    $("#myModalMT").modal("show");
});

var fnResultAlertaAtrib = null;
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlertaAtrib = fn;
    $("#botones-modal-info-mt-Atrib").find("span:last").hide();
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $("#botones-modal-info-mt-Atrib").find("span:last").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoMTAtrib").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoMTAtrib").html(titulo);
    $("#DescripcionInfoMTAtrib").html(mensaje);
    $("#ModalInfoMTAtrib").modal('show');
    return false;
}

function ajustarmodal() {
    var altura = $(window).height() - 190; //value corresponding to the modal heading + footer
    var alturaListas = altura - 94;
    $(".atributo-body").css({ "height": altura });
    $(".selector-body").css({ "height": alturaListas });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $(".StyleScrolls").getNiceScroll().resize();
    $(".StyleScrolls").getNiceScroll().show();
}
function habilitarBtnSiguiente() {
    if ($("#ComponentesUL li.seleccionado").length && $("#AtributosUL li.seleccionado").length &&
        (!$("#AgrupacionesUL li").length || $("#AgrupacionesUL li.seleccionado").length)) {
        $('#btn-atributo-siguiente').removeClass('disabled');
    } else {
        $('#btn-atributo-siguiente').addClass('disabled');
    }
}
$("#clearSearch").click(function () {
    $("#buscar").val('');
    $("#buscar").keyup();
});
$("#clearSearchAtributo").click(function () {
    $("#buscarAtributo").val('');
    $("#buscarAtributo").keyup();
});
//@ sourceURL=atributosMT.js