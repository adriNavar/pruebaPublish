var componenteSelect;
var seleccionMultiple;
var atributoSelect;
var operacionSelect;
var valor1;
var valor2;
var liEditado;
var modificar = null;
var porcentaje = null;
var seleccionGrafica = null;
var Tocando = 0;
var Dentro = 1;
var Fuera = 0;
var api;
var iValor = 1;
var interval;
//1ro)La forma que se generan actualmente los id de los filtros no tiene sentido, hace que se repitan y las colecciones generan el id de forma diferente a geografia y atributos.
//2do)No tiene sentido alguno los id de los filtros ya que eventualmente se van a terminar agregando los filtros, basta con eliminar los filtros del modelo antes de insertar los de la web.

//Dejo todos los filtros con el mismo id para evitar errores.

var mapa;
$('#myModalBA').one('shown.bs.modal', function (e) {
    mapa = null;
    ajustarmodal(true);
    hideLoading();
});
$(document).ready(function () {
    $("#formVolver").ajaxForm({
        beforeSubmit: showLoading,
        success: function (data) {
            mapa = null;
            $('#myModalBA').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () { $("#contenido").html(data); }, 500);
        },
        error: function () {
            alert("error al volver");
        }
    });

    $("#form-filtros").ajaxForm({
        beforeSubmit: function (arr, $form, options) {
            //Tener en cuenta que los filtros que se muestran en la web, siempre deben ser los mismos que estan en (BusquedaAvanzadaModel)Session["BusquedaAvanzada"].Filtros;
            showLoading();
            var error = [];
            var filtros = $($form).find('#filtros li').filter(function (_, val) {
                return !$(val).hasClass('bloqueado');
            });

            //"1" = AND
            //"2" = OR
            //"3" = (
            //"4" = )
            //"5" = NOT

            //1)Primera condicion de la consulta debe ser diferente de ")" "AND" "OR"
            var primerFiltro = filtros.first(), tipoCondicion;
            if (primerFiltro.hasClass('fcondiciones'))//Filtro condicion
            {
                tipoCondicion = primerFiltro.find('#Valor_2').val(); //Tipo de condicion
                if (jQuery.inArray(tipoCondicion, ['1', '2', '4']) !== -1) {
                    error.push("Condición inicial errónea.");
                }
            }

            //2)No debe existir 2 filtros (no condicion o no colecciones) seguidos
            $($form).find('#filtros li').each(function (inx, val) {
                if ($(val).hasClass('fgeograficos') || $(val).hasClass('fatributos')) {
                    var siguiente = $(val).next();
                    if (siguiente.hasClass('fgeograficos') || siguiente.hasClass('fatributos')) {
                        error.push("Debe agregar conectores entre filtros.");//"Filtros consecutivos. ";
                    }
                }
            });

            //3)La ultima condicion de la consulta debe ser diferente de "AND" "OR" "NOT"
            var ultimoFiltro = filtros.last();

            if (ultimoFiltro.hasClass('fcondiciones')) {//Filtro condicion
                tipoCondicion = ultimoFiltro.find('#Valor_2').val(); //Tipo de condicion
                if (jQuery.inArray(tipoCondicion, ['1', '2', '5']) !== -1) {
                    error.push("Última condición errónea.");
                }
            }

            //4)Debe existir la misma cantidad de parentesis abiertos y de cerrados.
            var contParAbiert = 0;
            var contParCerra = 0;
            $($form).find('#filtros li').each(function (inx, val) {
                if ($(val).hasClass('fcondiciones')) {
                    var tipoCondicion = $(val).find('#Valor_2').val();
                    if (tipoCondicion == 3)
                        contParAbiert++;
                    else if (tipoCondicion == 4)
                        contParCerra++;
                }
            });
            if (contParAbiert < contParCerra) {//Si existen mas parentecis cerrados que abiertos la consulta esta mal
                error.push("Faltan abrir paréntesis.");
            } else if (contParCerra < contParAbiert) {//Si existen mas parentecis abiertos que cerrados la consulta esta mal
                error.push("Faltan cerrar paréntesis.");
            }

            //5)No debe existir 2 condiciones seguidos a menos que...
            $($form).find('#filtros li').each(function (inx, val) {
                if ($(val).hasClass('fcondiciones')) {//Primero es condicion
                    var siguiente = $(val).next();
                    if (siguiente.hasClass('fcondiciones')) {//Segundo es condicion
                        var primeroTipoCondicion = $(val).find('#Valor_2').val(); //Tipo de condicion
                        var segundoTipoCondicion = siguiente.find('#Valor_2').val(); //Tipo de condicion

                        if (primeroTipoCondicion === 3) {//Valido el primera condicion con (
                            //Siguiente condicion SOLAMENTE puede ser "NOT" o "("
                            if (jQuery.inArray(segundoTipoCondicion, ['3', '5']) === -1) {
                                error.push('Después de Paréntesis abierto "(" no se permite un "' + siguiente[0].innerText + '".');
                            }
                        } else if (primeroTipoCondicion === 4) {//Valido la primera condicion con )
                            //Siguiente condicion SOLAMENTE puede ser "AND" , "OR" o ")".
                            if (jQuery.inArray(segundoTipoCondicion, ['1', '2', '4']) === -1) {
                                error.push('Después de Paréntesis cerrado ")" no se permite un "' + siguiente[0].innerText + '".');
                            }
                        } else if (primeroTipoCondicion === 5) {//Valido la primera condicion con NOT
                            //Siguiente condicion SOLAMENTE puede ser "("
                            if (!segundoTipoCondicion === 3) {
                                error.push('Después de un NOT no se permite un "' + siguiente[0].innerText + '".');
                            }
                        }

                        if (segundoTipoCondicion === 3) {//Valido la segunda condicion con (
                            //Anterior condicion SOLAMENTE puede ser "AND", "OR", "NOT" o "("
                            if (jQuery.inArray(primeroTipoCondicion, ['1', '2', '3', '5']) === -1) {
                                error.push('Antes de un Paréntesis abierto "(" no se permite un "' + siguiente[0].innerText + '".');//serua val.innertext
                            }
                        } else if (segundoTipoCondicion === 4) {//Valido la segunda condicion con )
                            //Anterior condicion SOLAMENTE puede ser ")"
                            if (!primeroTipoCondicion === 4) {
                                error.push('Antes de un Paréntesis cerrado ")" no se permite un "' + siguiente[0].innerText + '".');//Seria val.innertext
                            }
                        } else if (segundoTipoCondicion === 5) {//Valido la segunda condicion con NOT
                            //Anterior condicion SOLAMENTE puede ser "AND", "OR", "NOT" o "("
                            if (jQuery.inArray(primeroTipoCondicion, ['1', '2', '3', '5']) === -1) {
                                error.push('Antes de un NOT no se permite un "' + siguiente[0].innerText + '".');//Seria val.innertext
                            }
                        }

                    }
                }
            });

            //6)Solo puede existir una coleccion
            if ($('#filtros li.fcolecciones', $form).length > 1) {
                error.push("Sólo puede haber una única colección.");
            } else {
                $('#filtros li.fcolecciones', $form).each(function (inx, val) {
                    //7)La coleccion debe ser el primer filtro
                    if (val !== primerFiltro[0]) {
                        error.push("La colección debe ser el primer filtro.");
                    }
                    //7)Despues de la coleccion debe seguir un filtro geografico o atributos.
                    if ($(val).next().hasClass('fcondiciones')) {
                        error.push("Después de filtro colección sigue un filtro geográfico o atributos.");
                    }
                });
            }

            //9)No debe existir un filtro y una condicion tipo "("

            if (error.length) {
                hideLoading();
                alerta('Error en consulta', error.join('<br/>'), 2);
            }
            return !error.length;
        },
        success: function (data) {
            $('#myModalBA').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () { $("#contenido").html(data); }, 200);
        },
        error: function (err) {
            //404 no tiene datos
            //500 Error de consulta u otro error
            if (err.status === 404) {
                alerta('Búsqueda Avanzada - Sin datos', 'No se encontraron datos', 3);
            } else if (err.status === 500) {
                alerta('Búsqueda Avanzada - Error', 'Error en la consulta.', 3);
            }
            hideLoading();
        }
    });

    ///////////////////// Scrollbars ////////////////////////
    $(".selector-body,.resumen").niceScroll(getNiceScrollConfig());
    /////////////////////////////////////////////////////////

    $("#btn-siguiente-filtros").on('click', function (evt) {
        $("#form-filtros").submit();
    });

    if ($("#filtros").is(":empty")) {
        $("#sinfiltro").show();
    } else {
        $("#sinfiltro").hide();
    }
    $('#cantFiltrosGeografico').val($('.fgeograficos').length);
    $('#cantFiltrosAtributo').val($('.fatributos').length);
    $('#cantFiltrosColeccion').val($('.fcolecciones').length);

    $("#geografico").click(function () {
        $('.panel-group.geograficos').show();
        $(this).parents('.panel-group.filtros').hide();
        $(".modal-footer").hide();
        ajustarmodal();
        new Promise(function (resolve) {
            if (!mapa) {
                mapa = new MapaController(4, "mapaBA", true, false, true, false, true, true, false, false, false, false, true);
                setTimeout(resolve, 1000);
            } else {
                resolve();
            }
        })
            .then(function () {
                mapa.limpiar();
                if (Dentro) {
                    Dentro = 0;
                    $('#dentro').click();
                }
                if (Tocando) {
                    Tocando = 0;
                    $('#tocando').click();
                }
                if (Fuera) {
                    Fuera = 0;
                    $('#fuera').click();
                }
                if (seleccionGrafica) {
                    if (seleccionGrafica.substr(0, 1) === "G") {
                        $('#seleccionar').click();
                    } else {
                        $('#dibujar').click();
                    }
                } else if (componenteSelect) {
                    $("#avanzado").click();
                }
            });
    });

    $("#porAtributo").on('click', function () {
        $('.panel-group.alfanumericos').show();
        $(this).parents('.panel-group.filtros').hide();
        $(".modal-footer").hide();
        ajustarmodal();
    });

    $("#porColeccion").on('click', function () {
        $('.panel-group.colecciones').show();
        $(this).parents('.panel-group.filtros').hide();
        $(".modal-footer").hide();
        ajustarmodal();
    });

    $("#cerrar").click(function () {
        var msj = "¿Desea salir del wizard de Búsqueda Avanzada? ";
        alerta('Búsqueda Avanzada: Filtros', msj, 2, function () {
            $('.modal-backdrop').remove();
            $("#myModalBA").modal('hide');
        });
        return false;
    });

    $("#cancelar").click(resetFiltroAlfa);

    $("#aceptar").click(function (evt) {
        generarFiltro('fatributos', 'fa-tag', generarFiltroAlfa);
        $("#cantFiltrosAtributo").val($(".fatributos").length);
        $("#sinfiltro").css('display', 'none');

        resetFiltroAlfa();
        volverFiltros(evt);
    });

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    $("#filtros").on('click', 'li', function () {
        $(this).siblings().removeClass("seleccionado");
        $(this).toggleClass("seleccionado");
        ActualizarBotones();
    });

    //recupera los atributos al cambiar el componente
    $("#ComponenteULAlfa").on('click', 'li', function () {
        $("#aceptar").attr("disabled", "disabled");
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        var chkComponente = $(this).find("input[type=checkbox]");
        chkComponente.prop('checked', true);
        $("#AtributoULAlfa").empty();
        $("#OperacionesULAlfa").empty();
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'BusquedaAvanzada/GetAtributosByComp',
            dataType: 'json',
            data: { id: $(".ComponenteId:checked").val() },
            success: function (atributos) {
                chkComponente.prop('checked', true);
                $.each(atributos, function (i, atributo) {
                    $("#AtributoULAlfa")
                        .append('<li class="seleccionable">' +
                            '<div id="' + atributo.AtributoId + '">' +
                            '<input type="checkbox" class="AtributoId" name="' + atributo.Nombre + '" value="' + atributo.AtributoId + '" />' +
                            '<label class="h5" for="' + atributo.AtributoId + '">' +
                            atributo.Nombre +
                            '<span class="fa fa-check pull-right"></span></label>' +
                            '</div>' +
                            '<input type="hidden" class="tipoDato" value="' + atributo.TipoDatoId + '" />' +
                            '<input type="hidden" class="precision" value="' + atributo.Precision + '" />' +
                            '</li>');
                });

                if (atributoSelect) {
                    $("#AtributoULAlfa li div[id=" + atributoSelect + "]").parent().click();
                    atributoSelect = undefined;
                }
            }, error: function (ex) {
                alert('Error al recuperar Atributos' + ex);
            }
        });
        return false;
    });

    //recupera las agrupaciones al elegir un componente distinto al primero.
    $("#AtributoULAlfa").on('click', 'li', function () {
        $("#aceptar").attr("disabled", "disabled");
        $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        var chkComponente = $(this).find("input[type=checkbox]");
        chkComponente.prop('checked', true);
        $("#OperacionesULAlfa").empty();

        $.ajax({
            type: 'POST',
            url: BASE_URL + 'BusquedaAvanzada/GetOperacionesCombo',
            dataType: 'json',
            data: {
                id: $(".AtributoId:checked").val()
            },
            success: function (data) {
                var agrupaciones = data.lista;
                seleccionMultiple = data.habilitaSeleccionMultiple;
                var sizeInput = 200;
                var tipo = $(".AtributoId:checked").parents('li').find('.tipoDato').val();
                var precision = $(".AtributoId:checked").parents('li').find('.precision').val();
                //5	Date
                //6	String
                //4	Double
                //1	Boolean
                //2	Integer
                //3	Long
                //7 Float

                if (tipo == "6") {
                    sizeInput = (precision * 10) + 15;
                    if (sizeInput > 340) {
                        sizeInput = 340;
                    }
                }
                if (tipo == "4" || tipo == "3" || tipo == "2" || tipo == "7") {
                    sizeInput = (precision * 10) + 15;
                    $('.valorAgrupacion').inputmask("numeric", {
                        rightAlign: true
                    });
                }
                if (tipo == "5") {
                    sizeInput = (precision * 10) + 15;
                    $('.valorAgrupacion').inputmask("date", {
                        placeholder: "dd/mm/yyyy"
                    }, {
                        rightAlign: false
                    });
                }

                $.each(agrupaciones, function (j, agrupacion) {
                    var campos = '<div class="col-lg-8 col-xs-8 col-sm-8 col-md-8 pull-right valores" id="subrayadoUL' + j + '" name="subrayadoUL" style="white-space:nowrap; display:none">';

                    if (agrupacion.CantidadValores > 1) {
                        sizeInput = 100;
                    }
                    for (var i = 0; i < agrupacion.CantidadValores; i++) {
                        campos = campos.concat('<input id="valor_' + i + '" name="valorAgrupacion" class="form-control valorAgrupacion texto-operacion-string" maxlength="25" style="display: inline; width:' + sizeInput.toString() + 'px; margin-top: 3px; max-width:calc(100%);" type="text" placeholder="0"/> ');
                        if ((i + 1) < agrupacion.CantidadValores) {
                            campos = campos.concat(" y ");
                        }
                    }
                    campos = campos.concat("</div>");
                    $("#OperacionesULAlfa")
                        .append('<li class="seleccionable"  id="' + j + '">' +
                            '<div  id="' + agrupacion.TipoOperacionId + '" class="row">' +
                            '<div class="col-lg-4 col-xs-4 col-sm-4 col-md-4 pull-left" style="white-space:nowrap;">' +
                            '<input type="checkbox" class="OperacionId"  style="display:none" name="' + agrupacion.Nombre + '" value="' + agrupacion.TipoOperacionId + '" />' +
                            '<label class="h5" style="width:100%">' + agrupacion.Nombre + '</label>' +
                            '</div>' +
                            campos +
                            '</div>' +
                            '</li>');
                });
                //ajustarScrolls();

                if (tipo == "5") {
                    $('.valorAgrupacion').inputmask("date", {
                        placeholder: "dd/mm/yyyy"
                    }, {
                        rightAlign: false
                    });
                }

                if (precision && precision > 0) {
                    $('.valorAgrupacion').attr('maxlength', precision);
                }
                if (operacionSelect) {
                    $("#OperacionesULAlfa li div[id=" + operacionSelect + "]").parent().click();

                    $("#OperacionesULAlfa li div[id=" + operacionSelect + "]").find('#valor_0').val(valor1);

                    if (valor2) {
                        $("#OperacionesULAlfa li div[id=" + operacionSelect + "]").find('#valor_1').val(valor2);

                    }
                    operacionSelect = undefined;
                    valor1 = null;
                    valor2 = null;
                } else {
                    $("#OperacionesULAlfa li").first().addClass("seleccionado");
                    $("#OperacionesULAlfa li").first().click();
                }
            }, error: function (ex) {
                alert('Error al recuperar Operaciones' + ex);
            }
        });
        return false;
    });

    $("#buscar").keyup(function () {
        var texto = $("#buscar").val();
        if (texto) {
            $("#ComponenteULAlfa").find("li").hide();

            $.each($("#AtributoULAlfa li").find("label"), function (_, span) {
                if ($(span).text().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(span).closest("li").show();
                }
            });
            $("#ComponenteULAlfa li:hidden").removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);


            if ($("#ComponenteULAlfa li input[type=checkbox]:checked").length < 1) {
                $("#AtributoULAlfa").empty();
                $("#OperacionesULAlfa").empty();
            }
        } else {
            $("#ComponenteULAlfa li").show();
            $("#buscarAtributo").val("");
        }
        return false;
    });

    $("#buscarAtributo").keyup(function () {
        var texto = $("#buscarAtributo").val();
        if (texto) {
            $("#AtributoULAlfa li").hide();

            //dice spans pero luego del estilado son labels
            var spans = $("#AtributoULAlfa li").find("label");
            $.each(spans, function (_, span) {
                if ($(span).text().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(span).closest("li").show();
                }
            });

            $("#AtributoULAlfa li:hidden").removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);

            if ($("#AtributoULAlfa li input[type=checkbox]:checked").length < 1) {
                $("#OperacionesULAlfa").empty();
                $("#btn-atributo-siguiente").attr("disabled", "disabled");
                $("#btn-siguiente-filtros").removeClass("seleccionable");
            }
        } else {
            $("#AtributoULAlfa li").show();
        }
        return false;
    });

    $("#OperacionesULAlfa").on('click', 'li', function () {
        if (!seleccionMultiple) {
            $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
            $(this).parent().children("li").not(this).find('span').removeClass("tilde-seleccion");
            $(this).parent().children("li").find('div[name=subrayadoUL]').css("display", "none");
        }
        if (seleccionMultiple && $(this).hasClass("seleccionado")) {
            $(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
            $(this).find('span').removeClass("tilde-seleccion");
        } else {
            $(this).addClass("seleccionado");
            $(this).find("input[type=checkbox]").prop('checked', true);
            $(this).find('span').addClass("tilde-seleccion");

            // Muetra y oculta el subrayado de operaciones
            var mostrarsubrayado = Number($(this).prop("id"));
            $("#subrayadoUL" + mostrarsubrayado).show();

            if (mostrarsubrayado === 6) {
                iValor ^= 1;
                $(this).find('input[id=valor_' + iValor + ']').focus();
            } else {
                $(this).parent().children("li").find('input[name=valorAgrupacion]').focus(); // muestra el puntero dentro del input
            }
        }
        habilitarAceptar();
        return false;
    });

    //Previene que avancen con Enter
    $(window).keydown(function (event) {
        if (event.keyCode === 13 && $("#myModalBA").css('display') !== 'none') {
            event.preventDefault();
            return false;
        }
    });

    $("#aceptarColeccion").click(function (evt) {
        //Funcionalidad diferente a aceptarGeografico, aceptarAtributo y aceptarCondicion.
        //Si se modifica elimina la coleccion modificada, arma el html y agrega la seleccionadas al final de todo.
        //Si se agrega arma el html y agrega la seleccionadas al final de todo.

        if (liEditado) {
            $($("#filtros li")[liEditado]).remove();
            liEditado = null;
        }

        var seleccionadas = $(".ColeccionId:checked");
        seleccionadas.each(function () {
            $(".ColeccionId").not(this).prop('checked', false);
            $(this).prop('checked', true);
            generarFiltro('fcolecciones', 'fa-copyright', generarFiltroColeccion);
        });
        $("#cantFiltrosColeccion").val($(".fcolecciones").length);

        $("#sinfiltro").hide();

        resetFiltroColeccion();
        volverFiltros(evt);
    });

    $("#ColeccionesUL").on('click', 'li', function () {
        if ($(this).find("input[type=checkbox]").is(':checked')) {
            $(this).removeClass("seleccionado");
            $("span", this).removeClass("tilde-seleccion");
            $(this).find("input[type=checkbox]").prop('checked', false);
        } else {
            $(this).addClass("seleccionado");
            $(this).find("input[type=checkbox]").prop('checked', true);
            $(this).find('span').addClass("tilde-seleccion");
        }

        if ($(".ColeccionId:checked").length) {
            $("#aceptarColeccion").removeClass("boton-deshabilitado");
        } else {
            $("#aceptarColeccion").addClass("boton-deshabilitado");
        }
    });

    $("#buscarColeccion").keyup(function () {
        var texto = $("#buscarColeccion").val();
        if (texto) {
            $("#ColeccionesUL li").hide();
            $("#CantidadesUL  li").hide();
            $.each($("#ColeccionesUL li label"), function (_, label) {
                if ($(label).text().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(label).closest("li").show();
                    $("#CantidadesUL li:eq(" + $(label).closest("li").index() + ")").show();
                }
            });

        } else {
            $("#ColeccionesUL").find("li").show();
            $("#CantidadesUL").find("li").show();
        }
        $("#ColeccionesUL li:hidden").removeClass("seleccionado").addClass("seleccionable");
        $("span", "#ColeccionesUL li:hidden").removeClass("tilde-seleccion");
        $("input[type=checkbox]", "#ColeccionesUL li:hidden").prop('checked', false);
        return false;

    });

    $("#cancelarColeccion").click(resetFiltroColeccion);

    $("#volver").one('click', function (evt) {
        $("#formVolver").submit();
    });

    $("#seleccionar").on('click', function () {
        $(".mapa", ".panel-group.geograficos").show();
        $(".avanzados", ".panel-group.geograficos").hide();
        $(this).addClass('seleccionado');
        $(this).siblings().removeClass('seleccionado');
        mapa.limpiar();
        mapa.activarSeleccion();
        habilitarAceptarGeo();
        if (seleccionGrafica && seleccionGrafica.substr(0, 1) === "G") {
            var partes = seleccionGrafica.substr(1).split(';');
            mapa.seleccionarObjetos([[partes[1]]], [partes[0]]);
        }
        ajustarScrolls();
        return false;
    });

    $("#dibujar").on('click', function () {
        $(".mapa", ".panel-group.geograficos").show();
        $(".avanzados", ".panel-group.geograficos").hide();
        $(this).addClass('seleccionado');
        $(this).siblings().removeClass('seleccionado');
        mapa.limpiar();
        mapa.activarDibujoPoligono();
        habilitarAceptarGeo();
        if (seleccionGrafica && seleccionGrafica.substr(0, 1) !== "G") {
            mapa.editarObjeto(seleccionGrafica);
        }
        ajustarScrolls();
        return false;
    });

    $("#avanzado").on('click', function () {
        $(".mapa", ".panel-group.geograficos").hide();
        $(".avanzados", ".panel-group.geograficos").show();
        $(this).addClass('seleccionado');
        $(this).siblings().removeClass('seleccionado');
        ajustarScrolls();
        return false;
    });

    $("#aceptarGeografico").click(function (evt) {
        if (!mapa.obtenerDibujos().length && !mapa.obtenerSeleccion().seleccion.length && !$("#OperacionesULGeografico li.seleccionado").length) {
            alerta('Búsqueda Avanzada: Filtro Geográfico', ['Debe seleccionar un filtro para continuar.'], 2);
            return;
        }
        //Arma el html y lo reemplaza en el filtro geografico seleccionado. Idem aceptarAtributo y aceptarCondicion
        generarFiltro('fgeograficos', 'fa-globe', generarFiltroGeografico);
        $("#cantFiltrosGeografico").val($(".fgeograficos").length);
        $("#sinfiltro").css('display', 'none');

        resetFiltroGeografico();
        volverFiltros(evt);
    });
    $("#cancelarGeografico").click(resetFiltroGeografico);

    //recupera los atributos al cambiar el componente
    $("#ComponenteULGeografico").on('click', 'li', function () {
        $("#aceptarGeografico").attr("disabled", "disabled");
        $(this).siblings().removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        $(this).addClass("seleccionado");
        var chkComponente = $(this).find("input[type=checkbox]");
        chkComponente.prop('checked', true);
        $("#AtributoULGeografico").empty();
        $("#OperacionesULGeografico").empty();
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'BusquedaAvanzada/GetAtributosByComp',
            dataType: 'json',
            data: { id: $(".ComponenteIdGeografico:checked").val() },
            success: function (atributos) {
                chkComponente.prop('checked', true);
                $.each(atributos, function (i, atributo) {
                    $("#AtributoULGeografico")
                        .append('<li class="seleccionable">' +
                            '<div id="' + atributo.AtributoId + '">' +
                            '<input type="checkbox" class="AtributoIdGeografico" name="' + atributo.Nombre + '" value="' + atributo.AtributoId + '" />' +
                            '<label class="h5" for="' + atributo.AtributoId + '">' +
                            atributo.Nombre +
                            '<span class="fa fa-check pull-right"></span></label>' +
                            '</div>' +
                            '<input type="hidden" class="tipoDato" value="' + atributo.TipoDatoId + '" />' +
                            '<input type="hidden" class="precision" value="' + atributo.Precision + '" />' +
                            '</li>');
                });

                if (atributoSelect) {
                    $("#AtributoULGeografico li div[id=" + atributoSelect + "]").parent().click();
                    atributoSelect = undefined;
                }
            }, error: function (ex) {
                alert('Error al recuperar Atributos' + ex);
            }
        });
        return false;
    });

    //recupera las agrupaciones al elegir un componente distinto al primero.
    $("#AtributoULGeografico").on('click', 'li', function () {
        $("#aceptarGeografico").attr("disabled", "disabled");
        var li = $(this);
        li.siblings().removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
        li.addClass("seleccionado");
        var chkComponente = $(this).find("input[type=checkbox]");
        chkComponente.prop('checked', true);
        $("#OperacionesULGeografico").empty();

        $.ajax({
            type: 'POST',
            url: BASE_URL + 'BusquedaAvanzada/GetOperacionesCombo',
            dataType: 'json',
            data: {
                id: $(".AtributoIdGeografico:checked", li).val()
            },
            success: function (data) {
                var agrupaciones = data.lista;
                seleccionMultiple = data.habilitaSeleccionMultiple;
                var sizeInput = 200;
                var tipo = parseInt($('.tipoDato', li).val());
                var precision = parseInt($('.precision', li).val());

                $.each(agrupaciones, function (j, agrupacion) {
                    var campos = '<div class="col-xs-8 valores" id="subrayadoUL' + j + '" name="subrayadoUL" style="white-space:nowrap; display:none">';

                    if (agrupacion.CantidadValores > 1) {
                        sizeInput = 100;
                    }

                    for (var i = 0; i < agrupacion.CantidadValores; i++) {
                        campos = campos.concat('<input id="valor_' + i + '" name="valorAgrupacion" class="form-control valorAgrupacion texto-operacion-string" maxlength="25" style="display: inline; width:' + sizeInput.toString() + 'px; margin-top: 3px; max-width:calc(100%);" type="text" placeholder="0"/> ');
                        if (i + 1 < agrupacion.CantidadValores) {
                            campos = campos.concat(" y ");
                        }
                    }
                    campos = campos.concat("</div>");
                    $("#OperacionesULGeografico")
                        .append('<li class="seleccionable"  id="' + j + '">' +
                            '<div  id="' + agrupacion.TipoOperacionId + '" class="row">' +
                            '<div class="col-xs-4" style="white-space: nowrap;">' +
                            '<input type="checkbox" class="OperacionIdGeografico"  style="display:none" name="' + agrupacion.Nombre + '" value="' + agrupacion.TipoOperacionId + '" />' +
                            '<label class="h5">' + agrupacion.Nombre + '</label>' +
                            '</div>' +
                            campos +
                            '</div>' +
                            '</li>');
                });

                //5	Date
                //6	String
                //4	Double
                //1	Boolean
                //2	Integer
                //3	Long
                //7 Float

                if (tipo === 5) {
                    $('.valorAgrupacion').inputmask("date", {
                        placeholder: "dd/mm/yyyy"
                    }, {
                        rightAlign: false
                    });
                    precision = 10;
                }
                if ([2, 3, 4, 7].indexOf(tipo) !== -1) {
                    $('.valorAgrupacion').inputmask({
                        mask: "999[9]", greedy: false,
                        placeholder: ""
                    }, {
                        rightAlign: false
                    });
                }
                if (precision && precision > 0) {
                    $('.valorAgrupacion').attr('maxlength', precision);
                }
                if (operacionSelect) {
                    $("#OperacionesULGeografico li div[id=" + operacionSelect + "]").parent().click();

                    $("#OperacionesULGeografico li div[id=" + operacionSelect + "]").find('#valor_0').val(valor1);

                    if (valor2) {
                        $("#OperacionesULGeografico li div[id=" + operacionSelect + "]").find('#valor_1').val(valor2);

                    }
                    operacionSelect = undefined;
                    valor1 = null;
                    valor2 = null;
                } else {
                    $("#OperacionesULGeografico li").first().addClass("seleccionado");
                    $("#OperacionesULGeografico li").first().click();
                }
            }, error: function (ex) {
                alert('Error al recuperar Operaciones' + ex);
            }
        });
        return false;
    });

    $("#OperacionesULGeografico").on('click', 'li', function () {
        if (!seleccionMultiple) {
            $(this).parent().children("li").not(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
            $(this).parent().children("li").not(this).find('span').removeClass("tilde-seleccion");
            $(this).parent().children("li").find('div[name=subrayadoUL]').css("display", "none");
        }
        if (seleccionMultiple && $(this).hasClass("seleccionado")) {
            $(this).removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
            $(this).find('span').removeClass("tilde-seleccion");
        } else {
            $(this).addClass("seleccionado");
            $(this).find("input[type=checkbox]").prop('checked', true);

            // Muetra y oculta el subrayado de operaciones
            var mostrarsubrayado = $(this).prop("id");
            $("#subrayadoUL" + mostrarsubrayado).show();

            if (parseInt(mostrarsubrayado) === 6) {
                if (iValor === 1) {
                    iValor = 0;
                } else {
                    iValor = 1;
                }
                $(this).find('input[id=valor_' + iValor + ']').focus();
            } else {
                $(this).parent().children("li").find('input[name=valorAgrupacion]').focus(); // muestra el puntero dentro del input
            }
        }
        habilitarAceptarGeo();
        return false;
    });

    $('#tocando').click(function () {
        if (Tocando === 1) {
            Tocando = 0;
            $(this).removeClass('seleccionado');
        } else {
            Tocando = 1;
            Fuera = 0;
            $("#fuera").removeClass('seleccionado');
            $(this).addClass('seleccionado');
        }
    });
    $('#fuera').click(function () {
        if (Fuera === 1) {
            Fuera = 0;
            $(this).removeClass('seleccionado');
        } else {
            Fuera = 1;
            Tocando = 0;
            Dentro = 0;
            $(this).addClass('seleccionado');
            $(this).siblings().removeClass('seleccionado');
        }
    });
    $('#dentro').click(function () {
        if (Dentro === 1) {
            Dentro = 0;
            $(this).removeClass('seleccionado');
        } else {
            Dentro = 1;
            Fuera = 0;
            $("#fuera").removeClass('seleccionado');
            $(this).addClass('seleccionado');
        }
    });

    $('#masOpciones').popover({
        html: true,
        placement: "bottom",
        content: '<div class="row" style="height: 100%; margin-left: 0px;">' +
            '<div class="row" style="margin-left: 0px;">' +
            'Modificar <span id="span_ampliar"> en ' +
            '<input id="modificarEn" class="subrayado" style="width:35%; text-align:center; margin-right:5px;" placeholder="000000" type="text" maxlength="6"> metros.' +
            '</span>' +
            '</div>' +
            '<div class="row" style="margin-top: 3px;">' +
            '<span class="fa fa-check-circle" style="color:#2D7A96; margin-right:5px; margin-left: 15px; cursor:pointer;" id="img_intersectando"></span> Intersectando' +
            '</div>' +
            '<div style="display:none;" id="inclusion">' +
            'Porcentaje de Inclusión <input class="subrayado centrado" id="inputPorcentaje" style="margin-top:5px; width:60px; text-align:center;" placeholder="00" type="text" maxlength="3" min="0" max="100"/>' +
            '</div>' +
            '</div>'
    });
    $('#masOpciones').click(function () {
        $('#modificarEn').inputmask('decimal', {
            digits: 1, rightAlign: false
        });

        $('#inputPorcentaje').inputmask('Regex', {
            regex: "^[1-9][0-9]?$|^100$"
        });

        $("#img_intersectando").click(function () {

            if ($(this).hasClass('selected')) {
                $(this).css('color', '#00BAEB');
                $("#img_intersectando").css('color', '#2D7A96');
                $(this).removeClass('selected');
                $("#inclusion").css('display', 'none');
                $(".popover").css('margin-top', '10px');
                porcentaje = null;
                $("#inputPorcentaje").val("");
            } else {
                $(this).css('color', '00BAEB');
                $("#img_intersectando").css('color', '#00BAEB');
                $(this).addClass('selected');
                $("#inclusion").css('display', 'inline');
                $(".popover").css('margin-top', '10px');
                Dentro = 0;
                $('#dentro').click();
            }
        });

        $("#modificarEn").change(function () {
            modificar = $("#modificarEn").val();
        });

        $("#inputPorcentaje").change(function () {
            porcentaje = $("#inputPorcentaje").val();
        });
        if (modificar) {
            $("#modificarEn").val(modificar);
        }
        if (porcentaje) {
            $("#inputPorcentaje").val(porcentaje);
            $("#img_intersectando").css('color', 'white');
            $("#img_intersectando").addClass('selected');
            $("#inclusion").css('display', 'inline');
            $(".popover").css('margin-top', '10px');
        }
    });

    $('[data-cancela-filtro]').on('click', volverFiltros);

    $("#clearSearch").click(function () {
        $("#buscar").val('');
        $("#buscar").keyup();
    });
    $("#clearSearchAtributo").click(function () {
        $("#buscarAtributo").val('');
        $("#buscarAtributo").keyup();
    });
    $("#clearSearchColeccion").click(function () {
        $("#buscarColeccion").val('');
        $("#buscarColeccion").keyup();
    });

    $("#cboCondicion").change(function () {
        if ($(this).val() > 0) {
            $("#aceptarCondicion").removeClass("boton-deshabilitado").addClass("blacks");
        }
        else {
            $("#aceptarCondicion").addClass("boton-deshabilitado").removeClass("blacks");
        }
    });

    $("#btnBorrar").on('click', function () {
        $("#filtros .seleccionado").each(function (inx, val) {

            if ($(val).hasClass('fcolecciones')) {
                $("#cantFiltrosColeccion").val(parseInt($("#cantFiltrosColeccion").val()) - 1);
            }
            if ($(val).hasClass('fatributos')) {
                $("#cantFiltrosAtributo").val(parseInt($("#cantFiltrosAtributo").val()) - 1);
            }
            if ($(val).hasClass('fgeograficos')) {
                $("#cantFiltrosGeografico").val(parseInt($("#cantFiltrosGeografico").val()) - 1);
            }

            var idFiltro = $(val).children().children().children("input:first").val();

            $.ajax({
                "url": "BusquedaAvanzada/RemoveFilter?idFilter=" + idFiltro,
                "async": false,
                "success": function (result) {
                    $(val).remove();

                    if ($("#filtros").is(":empty")) {
                        $("#sinfiltro").show();
                    } else {
                        $("#sinfiltro").css('display', 'none');
                    }
                    corregirSecuencia();
                    ajustarScrolls();
                }
            });

        });
        ActualizarBtnInformacion();
        ActualizarBotones();
    });

    $("#btnEditar").on("click", function () {
        var filtrosLI = $("#filtros .seleccionado");
        liEditado = $("#filtros li").index(filtrosLI);

        if (filtrosLI.hasClass('fcolecciones')) {
            $(".fcoleccion").each(function (_, elem) {
                var li = $("#ColeccionesUL input[type=checkbox][value=" + $(elem).val() + "]").closest("li");
                li.hide();
                $("#CantidadesUL li:eq(" + $("#ColeccionesUL li").index(li) + ")").hide();
            });
            $("#aceptarColeccion").addClass('modificacion');
            $("#porColeccion").click();
        } else if (filtrosLI.hasClass('fatributos')) {
            $("#filtros li").removeClass('seleccionado');
            componenteSelect = filtrosLI.find('.fcomponente').val();
            atributoSelect = filtrosLI.find('.fatributo').val();

            $("#ComponenteULAlfa li input[value=" + componenteSelect + "]").parent().click();
            $("#AtributoULAlfa li input[value=" + atributoSelect + "]").parent().click();
            interval = setInterval(function () {
                valor1 = filtrosLI.find('.fvalores_1').val();
                var operacion = filtrosLI.find('.foperacion').val(),
                    tipo = operacion === "0" ? "name=" + valor1 : "value=" + operacion,
                    optOperacion = $("#OperacionesULAlfa li input[type=checkbox][" + tipo + "]");

                if (optOperacion.length) {
                    clearInterval(interval);
                    optOperacion.parent().click();

                    if (operacion === "0") return;
                    $('input[name=valorAgrupacion]:first', optOperacion.parents('li')).val(valor1);
                    if (filtrosLI.find('.fvalores_2').length) {
                        valor2 = filtrosLI.find('.fvalores_2').val();
                        $('input[name=valorAgrupacion]:last', optOperacion.parents('li')).val(valor2);
                    }
                }
            }, 100);
            $("#porAtributo").click();
        } else if (filtrosLI.hasClass('fgeograficos')) {
            Dentro = Number(filtrosLI.find('.fdentro').val());
            Tocando = Number(filtrosLI.find('.ftocando').val());
            Fuera = Number(filtrosLI.find('.ffuera').val());
            modificar = Number(filtrosLI.find('.fampliar').val())
            porcentaje = Number(filtrosLI.find('.fporcentaje').val());
            seleccionGrafica = filtrosLI.find('.fcoordenadas').val();
            if (!seleccionGrafica) {
                filtrosLI.removeClass('seleccionado');//Le saco el seleccionado x el metodo kk seleccionadosOnTop que me desacomoda todo
                componenteSelect = filtrosLI.find('.fcomponenteGeo').val();
                atributoSelect = filtrosLI.find('.fatributoGeo').val();

                $("#ComponenteULGeografico li input[value=" + componenteSelect + "]").parent().click();
                $("#AtributoULGeografico li input[value=" + atributoSelect + "]").parent().click();
                interval = setInterval(function () {
                    valor1 = filtrosLI.find('.fvalores_1').val();
                    var operacion = filtrosLI.find('.foperacionGeo').val(),
                        tipo = operacion === "0" ? "name=" + valor1 : "value=" + operacion,
                        optOperacion = $("#OperacionesULGeografico li input[type=checkbox][" + tipo + "]");

                    if (optOperacion.length) {
                        clearInterval(interval);
                        optOperacion.parent().click();

                        if (operacion === "0") return;

                        $('input[name=valorAgrupacion]:first', optOperacion.parents('li')).val(valor1);
                        if (filtrosLI.find('.fvalores_2').length) {
                            valor2 = filtrosLI.find('.fvalores_2').val();
                            $('input[name=valorAgrupacion]:last', optOperacion.parents('li')).val(valor2);
                        }
                    }
                }, 100);
            }
            $("#geografico").click();
        } else if (filtrosLI.hasClass('fcondiciones')) {
            //Deshabilita todos los botones
            $("span", "panel-group.filtros dl").addClass("boton-deshabilitado");
            $("#geografico,#porAtributo,#porColeccion").addClass("boton-deshabilitado");
        }
        ajustarScrolls();
        return false;
    });
    $("#btnInformacion").click(MostrarInformacion);

    $("#aceptarCondicion").click(function (evt) {
        //Arma el html y lo reemplaza en el filtro condicion seleccionado. Idem aceptarAtributo y aceptarGeografico.
        if ($("#cboCondicion").val() > 0) {//Vuelvo a validar por si las moscas
            if (liEditado) {
                $("span", "panel-group.filtros dl").removeClass("boton-deshabilitado");
                $("#geografico,#porAtributo,#porColeccion").removeClass("boton-deshabilitado");
            }
            generarFiltro('fcondiciones', 'fa-asterisk', generarFiltroCondicion);
            resetFiltroCondicion(evt);
        }
    });

    $("#btnSubir").on("click", function () {
        var seleccion = $("#filtros .seleccionado");
        var anterior = seleccion.prev();
        var auxi = "";
        if (anterior.is('li')) {
            auxi = $(anterior).replaceWith(seleccion.clone());
            $(seleccion).replaceWith(auxi);
        }
        corregirSecuencia();
    });

    $("#btnBajar").on("click", function () {
        var seleccion = $("#filtros .seleccionado");
        var siguiente = seleccion.next();
        var auxi = "";
        if (siguiente.is('li')) {
            auxi = $(siguiente).replaceWith(seleccion.clone());
            $(seleccion).replaceWith(auxi);
        }
        corregirSecuencia();
    });

    ActualizarBtnInformacion();
    ActualizarBotones();

    $("#myModalBA").modal("show");
});

function habilitarAceptar() {
    if ($("#OperacionesULAlfa li.seleccionado").length) {
        $("#aceptar").removeClass("boton-deshabilitado");
    } else {
        $("#aceptar").addClass("boton-deshabilitado");
    }
}
function habilitarAceptarGeo() {
    $("#aceptarGeografico").removeClass("boton-deshabilitado");
}
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    if (fn) {
        $("#btnAceptarInfoBAFiltros").one('click', fn);
        $("#ModalInfoBAFiltros .modal-footer").show();
    } else {
        $("#ModalInfoBAFiltros .modal-footer").hide();
    }
    $("#botones-modal-info-ba").find("span:last").hide();
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $("#botones-modal-info-ba").find("span:last").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoBAFiltros").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoBAFiltros").html(titulo);
    $("#DescripcionInfoBAFiltros").html(mensaje);
    $("#ModalInfoBAFiltros").modal('show');
    return false;
}
function ajustarmodal(ventanaResumen) {
    var altura = $(window).height() - (ventanaResumen ? 190 : 104), //value corresponding to the modal heading + footer
        alturaFiltros = altura - 106; //MGB: altura del contenedor ajustable

    $(".filtros-body").css({ "height": altura });
    $(".panel.listado", ".panel-group.filtros").css({ "height": alturaFiltros + 50 });
    $(".panel:not(.filtro):not(.footer)", ".panel-group:not(.filtros)").css({ "height": alturaFiltros });
    $(".selector-body").css({ "height": alturaFiltros - 38 });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $(".selector-body,.resumen").getNiceScroll().resize();
    $(".selector-body,.resumen").getNiceScroll().show();
}
function ajustarScrolls() {
    setTimeout(ajustarScrollBars, 100);
}

function corregirSecuencia() {
    var filter = $("#filtros li");
    var counter = 0;

    filter.each(function (_, elem) {
        $(elem).html(function (_, html) {
            var newHtml = "";
            var newInner = html.toString();
            newHtml += newInner.replace(/(Filtros)+\[+[0-9]+\]/gi, 'Filtros[' + counter + ']');
            return newHtml;
        });
        counter++;
    });
}

function resetFiltroAlfa() {
    componenteSelect = null;
    seleccionMultiple = null;
    atributoSelect = null;
    operacionSelect = null;
    valor1 = null;
    valor2 = null;

    $("#OperacionesULAlfa").empty();
    $("#AtributoULAlfa").empty();
    $("#ComponenteULAlfa li").removeClass("seleccionado");
    $("#ComponenteULAlfa li input[type=checkbox]").prop('checked', false);
    $("#buscar").val("");
    $("#buscar").keyup();
    $("#aceptar").addClass('boton-deshabilitado');
}
function resetFiltroGeografico() {
    seleccionGrafica = null;
    componenteSelect = null;
    seleccionMultiple = null;
    atributoSelect = null;
    operacionSelect = null;
    valor1 = null;
    valor2 = null;
    Dentro = 1;
    Tocando = 0;
    Fuera = 0;
    modificar = null;
    porcentaje = null;
    $("#OperacionesULGeografico").empty();
    $("#AtributoULGeografico").empty();
    $("#ComponenteULGeografico li").removeClass("seleccionado");
    $("#ComponenteULGeografico li input[type=checkbox]").prop('checked', false);
    $("#aceptarGeografico").addClass('boton-deshabilitado');
}
function resetFiltroColeccion() {
    $("#ColeccionesUL li input[type=checkbox]").prop('checked', false);
    $("#ColeccionesUL li").removeClass("seleccionado");
    $("#buscarColeccion").val("");
    $("#buscarColeccion").keyup();
    $("#aceptarColeccion").addClass('boton-deshabilitado');
}
function resetFiltroCondicion() {
    $("#cboCondicion").val('0');
    $("#aceptarCondicion").addClass("boton-deshabilitado");
}
function volverFiltros(evt) {
    $(evt.currentTarget).parents('.panel-group').hide();
    $('.panel-group.filtros').show();
    $('.modal-footer').show();
    ActualizarBtnInformacion();
    ajustarmodal(true);
}
function centrarSeleccionados() {
    setTimeout(function () {
        $(".seleccionado").each(function () {//filtro trucho, mejorar plz
            var aux = $(this);
            var parent = $(this).parent();
            if (parent.attr("id") !== "OperacionesULAlfa") {
                $(this).remove();
                var listitems = parent.children('li').get();
                listitems.sort(function (a, b) {
                    return $(a).text().toUpperCase().localeCompare($(b).text().toUpperCase());
                })
                $.each(listitems, function (idx, itm) {
                    parent.append(itm);
                });
                parent.prepend(aux);
            }
            $(this).parent().scrollTop($(this).position().top);
        });
    }, 300);
}

function ActualizarBtnInformacion() {//Tiene diferente comportamiento al resto de botones.
    //Esta prendido siempre que haya filtros.
    if ($("#filtros li:not(.fcondiciones)").length > 0) {
        $("#btnInformacion").removeClass("boton-deshabilitado");
    } else {
        $("#btnInformacion").addClass("boton-deshabilitado");
    }
}
function ActualizarBotones() {
    var cantSeleccionados = $("#filtros li.seleccionado").length;

    if (cantSeleccionados === 1) {//Hay un solo filtro seleccionado
        $("#btnBorrar,#btnBloquear,#btnEditar,#btnSubir,#btnBajar").removeClass("boton-deshabilitado");
    } else if (cantSeleccionados > 1) {//Hay mas de un filtro seleccionado
        $("#btnBorrar,#btnBloquear").removeClass("boton-deshabilitado");
        $("#btnEditar,#btnSubir,#btnBajar").addClass("boton-deshabilitado");
    } else {//Si no tiene filtros seleccionados
        $("#btnBorrar,#btnBloquear,#btnEditar,#btnSubir,#btnBajar").addClass("boton-deshabilitado");
    }
}
function MostrarInformacion() {
    var info = $("#filtros li:not(.bloqueado)").toArray().map(function (val) { return val.innerText; }).join(" ");
    $("#txtConsulta").html(info);
    $("#ModalInfoBAListado").modal('show');
}

function generarFiltro(tipo, icono, generadorTexto) {
    var li = crearElementoHtml('li', { className: 'seleccionable ' + tipo + ' ' + (liEditado ? 'seleccionado' : '') }),
        contenedor = li.appendChild(crearElementoHtml('div', { id: Date.now() })),
        divIcono = contenedor.appendChild(crearElementoHtml('div', { className: 'col-xs-1 filtro-icono' })),
        divTexto = contenedor.appendChild(crearElementoHtml('div', { className: 'col-xs-8 filtro-texto' }));

    divIcono.appendChild(crearElementoHtml('span', { className: 'fa fa-2x ' + icono }));

    liEditado = liEditado == null ? $("#filtros li").length : liEditado;

    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroId', value: liEditado + 1, id: liEditado + 1 }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Habilitado', value: 1, class: 'habilitado' }));

    generadorTexto(divTexto);
    if (liEditado < $("#filtros li").length) {
        $($("#filtros li")[liEditado]).replaceWith(li);
    } else {
        $("#filtros").append(li);
    }
    liEditado = null;
}
function generarFiltroAlfa(divTexto) {
    var operacion = $('.OperacionId:checked').val(),
        listado, nombreOperacion, valores1, valores2;

    if (seleccionMultiple) {
        operacion = null;
        nombreOperacion = "dentro de";
        listado = $('.OperacionId:checked').toArray().map(function (elem) { return $(elem).attr("name"); });
        valores1 = listado.join();
    } else {
        nombreOperacion = $('.OperacionId:checked').attr("name");
        listado = $('[name="valorAgrupacion"]', $('.OperacionId:checked').parents('.seleccionado'))
            .toArray().map(function (elem) { return $(elem).val() === "" ? "0" : $(elem).val(); });
        valores1 = listado[0];
        valores2 = listado[1];
    }

    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroComponente', value: $('.ComponenteId:checked').val(), class: 'fcomponente' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroAtributo', value: $('.AtributoId:checked').val(), class: 'fatributo' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroOperacion', value: operacion, class: 'foperacion' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroTipo', value: 1 }));


    divTexto.appendChild(crearElementoHtml('span', { innerText: $(".AtributoId:checked").attr("name") }));
    divTexto.appendChild(document.createTextNode(" de "));
    divTexto.appendChild(crearElementoHtml('span', { innerText: $(".ComponenteId:checked").attr("name") }));

    if (valores1) {
        divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Valor1', value: valores1, class: 'fvalores_1' }));
    }
    if (valores2) {
        divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Valor2', value: valores2, class: 'fvalores_2' }));
    }
    if (nombreOperacion) {
        divTexto.appendChild(document.createTextNode(" " + nombreOperacion + " "));
    }
    if (listado.length) {
        if (listado.length === 1) {
            valores1 = listado[0];
        } else {
            valores1 = listado.slice(0, listado.length - 1).join(", ").concat(" y ").concat(listado.slice(listado.length - 1));
        }
        divTexto.appendChild(crearElementoHtml('span', { innerText: " " + valores1 }));
    }
}
function generarFiltroGeografico(divTexto) {
    var operacion = $('.OperacionIdGeografico:checked').val(),
        listado, nombreOperacion, valores1, valores2;

    if (porcentaje && porcentaje > 0) {
        Dentro = 1;
        Tocando = 0;
        Fuera = 0;
    }
    var coordenadas = mapa.obtenerDibujos();
    if (!coordenadas.length) {
        var datos = mapa.obtenerSeleccion();
        if (datos.capas.length) {
            coordenadas = "G" + datos.capas[0] + ";" + datos.seleccion[0][0];
        } else {
            coordenadas = null;
            if (seleccionMultiple) {
                operacion = null;
                listado = $('.OperacionIdGeografico:checked').toArray().map(function (elem) { return $(elem).attr("name"); });
                valores1 = listado.join();
            } else {
                nombreOperacion = $('.OperacionIdGeografico:checked').attr("name");
                listado = $('[name="valorAgrupacion"]', $('.OperacionIdGeografico:checked').parents('.seleccionado'))
                    .toArray().map(function (elem) { return $(elem).val() === "" ? "0" : $(elem).val(); });
                valores1 = listado[0];
                valores2 = listado[1];
            }
        }
    }

    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroTipo', value: 2 }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Tocando', value: Tocando, class: 'ftocando' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Dentro', value: Dentro, class: 'fdentro' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Fuera', value: Fuera, class: 'ffuera' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Ampliar', value: modificar, class: 'fampliar' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].PorcentajeInterseccion', value: porcentaje, class: 'fporcentaje' }));
    if (coordenadas) {
        divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Coordenadas', value: coordenadas, class: 'fcoordenadas' }));
    } else {
        divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroComponente', value: $('.ComponenteIdGeografico:checked').val(), class: 'fcomponenteGeo' }));
        divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroAtributo', value: $('.AtributoIdGeografico:checked').val(), class: 'fatributoGeo' }));
        divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroOperacion', value: operacion, class: 'foperacionGeo' }));
    }

    var operadorGraf = "Dentro de";
    if (Fuera) {
        operadorGraf = "Fuera de";
    } else if (Tocando && Dentro) {
        operadorGraf = "Dentro y Tocando";
    } else if (Tocando) {
        operadorGraf = "Tocando";
    }

    if (coordenadas) {
        divTexto.appendChild(crearElementoHtml('span', { innerText: operadorGraf + ' selección gráfica' }));
        if (modificar) {
            divTexto.appendChild(crearElementoHtml('span', { innerText: ' modificado en ' + modificar.toString() + ' metros.' }));
        }
    } else {
        divTexto.appendChild(crearElementoHtml('span', { innerText: operadorGraf + ' selección gráfica avanzada ' }));

        divTexto.appendChild(crearElementoHtml('span', { innerText: $(".AtributoIdGeografico:checked").attr("name") }));
        divTexto.appendChild(document.createTextNode(" de "));
        divTexto.appendChild(crearElementoHtml('span', { innerText: $(".ComponenteIdGeografico:checked").attr("name") }));

        if (valores1) {
            divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Valor1', value: valores1, class: 'fvalores_1' }));
        }
        if (valores2) {
            divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Valor2', value: valores2, class: 'fvalores_2' }));
        }

        if (nombreOperacion) {
            divTexto.appendChild(document.createTextNode(" " + nombreOperacion + " "));
        }
        if (listado.length) {
            if (listado.length === 1) {
                valores1 = listado[0];
            } else {
                valores1 = listado.slice(0, listado.length - 1).join().concat(" y ").concat(listado.slice(listado.length - 1));
            }
            divTexto.appendChild(crearElementoHtml('span', { innerText: valores1 }));
        }
    }
}
function generarFiltroColeccion(divTexto) {
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroColeccion', value: $('.ColeccionId:checked').val(), class: 'fcomponente' }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroTipo', value: 3 }));
    divTexto.appendChild(document.createTextNode(" En Colección "));
    divTexto.appendChild(crearElementoHtml('span', { innerText: $(".ColeccionId:checked").attr("name") }));
}
function generarFiltroCondicion(divTexto) {
    var condicion = $("#cboCondicion option:selected");

    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].FiltroTipo', value: 4 }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Valor1', value: condicion.text() }));
    divTexto.appendChild(crearElementoHtml('input', { type: 'hidden', name: 'Filtros[' + liEditado + '].Valor2', value: condicion.val() }));

    divTexto.appendChild(crearElementoHtml('span', { innerText: condicion.text() }));
}
//# sourceURL=filtros.js