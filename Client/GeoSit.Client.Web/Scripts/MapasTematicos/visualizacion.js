$('#myModalMT').one('shown.bs.modal', function () {
    ajustarTabla();
    hideLoading();
});
$(window).resize(ajustarmodal);
$(document).ready(init);

function init() {
    var timeout;
    $("#formVolver").ajaxForm({
        beforeSubmit: showLoading,
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () {
                $("#contenido").html(data);
            }, 250);
        },
        error: function () {
            alert("error al volver");
        }
    });
    $("#form-visualizacion").ajaxForm({
        beforeSubmit: showLoading,
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () { $("#contenido").html(data); }, 200);
        },
        error: function () {
            alert("error en siguiente");
        }
    });
    $('#volver').on('click', function () {
        $("#formVolver").submit();
    });
    $('#btn-visualizacion-siguiente').on('click', function () {
        $("#form-visualizacion").submit();
    });

    $("#cerrar").click(function () {
        var msj = "¿Desea salir del wizard de Mapas Temáticos? ";
        alerta('Mapas Temáticos: Visualización', msj, 2, function () {
            $('body').off('change', '.tdesde');
            $('body').off('change', '.thasta');
            $('body').off('click', '.colorManual');
            $('.modal-backdrop').remove();
            $("#myModalMT").modal('hide');
        });
        return false;
    });
    ///////////////////// Scrollbars ////////////////////////
    $(".sscroll").niceScroll(getNiceScrollConfig());
    ////////////////////////////////////////////////////////

    $(".pick-a-color").pickAColor({
        showSpectrum: true,
        showSavedColors: true,
        saveColorsPerElement: true,
        fadeMenuToggle: true,
        showAdvanced: true,
        showBasicColors: true,
        showHexInput: false,
        allowBlank: true,
        inlineDropdown: false
    });
    $('input[type="radio"][name="estilizadores"]').change(function (evt) {
        $('#Visualizacion_Coloreado').val(evt.target.value);
        var tipo = Number(evt.target.value);
        if (tipo === 3) {
            $(".colores").hide();
            $(".manual").show();
            $("a.colorManual").removeClass("remove-pointer-events");
        } else {
            if (tipo === 2) {
                $(".gradiente").show();
            } else {
                $(".gradiente").hide();
                resetPicker('#pickerSecundario');
            }
            $(".colores").show();
            $(".manual").hide();
            $('.manual > .row').hide();
            $("a.colorManual").addClass("remove-pointer-events");
        }
    });
    $('#chkTransparencia').change(function (evt) {
        if ($(evt.target).is(":checked")) {
            $(evt.target).parents("[data-toggle]").siblings().show();
        } else {
            $(evt.target).parents("[data-toggle]").siblings().hide();
        }
    });
    $('#numTransparencia').on('input', function (evt) {
        $("#sldTransparencia").val(evt.target.value);
    });
    $('#sldTransparencia').on('change mousemove keyup', function (evt) {
        $("#numTransparencia").val(evt.target.value);
    });
    $('#chkVerLabels').on('click', function (evt) {
        var val = !eval($(evt.currentTarget).attr("data-checked"));
        $(evt.currentTarget).attr("data-checked", val);
        $(evt.currentTarget).siblings("input[type='hidden']").val(val);
    });
    $("#cboDistribuciones").change(function (evt) {
        showLoading();
        if (Number(evt.target.value) !== 3) {
            $("#rangos").parents('.rangos').removeClass('hidden');
        } else {
            $("#rangos").parents('.rangos').addClass('hidden');
        }
        $.ajax({
            url: BASE_URL + 'MapasTematicos/GetVisualizacionPartialView',
            data: { Rangos: $("#rangos").val(), Distribucion: $(this).val(), Coloreado: "1" },
            dataType: 'html',
            success: function (data) {
                $('#tablaCasos').empty();
                $('#tablaCasos').html(data);
                actualizarDistribucion();
                AplicarColorPrincipal();
                AplicarColorContorno();
                setTimeout(function () {
                    ajustarTabla();
                    ajustarScrollBars();
                }, 100);


                $(".tdesde").inputmask('numeric', { rightAlign: false });
                $(".thasta").inputmask('numeric', { rightAlign: false });
            },
            complete: function () {
                hideLoading();
            }
        });
    });
    $("#rangos").on('input', function () {
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            var rangosMaximos = Number($("#hfRangosMaximos").val());
            if (!isNaN(rangosMaximos) && rangosMaximos > 0) {
                if (Number($("#rangos").val()) > rangosMaximos) {
                    alerta('Mapa Temático - Visualización Rangos', 'Ha ingresado una cantidad de rangos excesiva para la distribución elegida', 2);
                } else {
                    $("#cboDistribuciones").change();
                }
            }
        }, 300);

    });

    $('#iconoSeleccionado').click(function () {
        $(this).parents('.estilizador').hide();
        $(this).parents('.estilizador').siblings('.iconos').show();
        $('#btnAceptarIcono').one('click', aceptarIcono.bind(this));
        $('#btnCancelarIcono').one('click', cancelarIcono.bind(this));
    });
    $('#pickerPrincipal').change(function () {
        AplicarColorPrincipal();
        AplicarColorContorno();
        //if ($("#hfGraficos").val() == 3) {
        //    //es punto
        //    $("#iconoSeleccionado").css('color', '#' + $('#pickerPrimario').val());
        //}
    });
    $('#pickerSecundario').change(function () {
        AplicarColorPrincipal();
        AplicarColorContorno();
        //if ($("#hfGraficos").val() == 3) {
        //    var cantidadContorno = $('#dropdown_contorno').text();
        //    var colorContorno_hex = $('#pickerContorno').val();
        //    var nav = GetNavigator();
        //    if (cantidadContorno != undefined && cantidadContorno > 0 && colorContorno_hex != undefined) {
        //        if (nav == "msie") {
        //            $("#iconoSeleccionado").css("text-shadow", cantidadContorno + "px 0px 0 #" + colorContorno_hex + ", 0px " + cantidadContorno + "px 0 #" + colorContorno_hex + ", -" + cantidadContorno + "px 0px 0 #" + colorContorno_hex + ", 0px -" + cantidadContorno + "px 0 #" + colorContorno_hex);
        //        } else {
        //            $("#iconoSeleccionado").css('-webkit-text-stroke', cantidadContorno + 'px #' + colorContorno_hex)
        //        }
        //    }
        //}
    });
    $('#pickerContorno').change(function () {
        if (Number($('#anchoContorno').val()) === 0) {
            $('#anchoContorno').val(1);
        }
        AplicarColorPrincipal();
        AplicarColorContorno();
        //if ($("#hfGraficos").val() == 3) {
        //    var cantidadContorno = $('#dropdown_contorno').text();
        //    var colorContorno_hex = $('#pickerContorno').val();
        //    var nav = GetNavigator();
        //    if (cantidadContorno != undefined && cantidadContorno > 0 && colorContorno_hex != undefined) {
        //        if (nav == "msie") {
        //            $("#iconoSeleccionado").css("text-shadow", cantidadContorno + "px 0px 0 #" + colorContorno_hex + ", 0px " + cantidadContorno + "px 0 #" + colorContorno_hex + ", -" + cantidadContorno + "px 0px 0 #" + colorContorno_hex + ", 0px -" + cantidadContorno + "px 0 #" + colorContorno_hex);
        //        } else {
        //            $("#iconoSeleccionado").css('-webkit-text-stroke', cantidadContorno + 'px #' + colorContorno_hex)
        //        }
        //    }
        //}
    });
    $('#anchoContorno').change(function () {
        AplicarColorContorno();
    });

    $('#iconoSeleccionadoManual').click(function () {
        $(this).parents('.estilizador').hide();
        $(this).parents('.estilizador').siblings('.iconos').show();
        $('#btnAceptarIcono').one('click', aceptarIconoManual.bind(this));
        $('#btnCancelarIcono').one('click', cancelarIconoManual.bind(this));
    });
    $('#pickerPrincipalManual').change(function () {
        $('#iconoSeleccionadoManual').css('color', '#' + $(this).val());
    });
    $('#pickerContornoManual').change(function () {
        if (Number($('#anchoContornoManual').val()) === 0) {
            $('#anchoContornoManual').val(1);
        }
        AplicarContornoManual();
    });
    $('#anchoContornoManual').change(function () {
        AplicarContornoManual();
    });
    $('#btnAceptarManual').click(function () {
        var clone = $('#iconoSeleccionadoManual').clone(),
            anchor = $('a.colorManual.editando'),
            final = $('span', anchor);
        clone.removeClass('btn-icono');

        final.removeClass().addClass(clone.attr('class') + ' zona');
        final.css('color', clone.css('color'));
        final.css('-webkit-text-stroke', clone.css('-webkit-text-stroke'));
        final.css('text-shadow', clone.css('text-shadow'));

        anchor.siblings('input[type=hidden].colorh').val($('#pickerPrincipalManual').val());
        anchor.siblings('input[type=hidden].colorbordeh').val($('#pickerContornoManual').val());
        anchor.siblings('input[type=hidden].anchobordeh').val($('#anchoContornoManual').val());
        anchor.siblings('input[type=hidden].iconoh').val(clone.removeClass('glyphicon').attr('class'));

        clone.remove();
        $('#btnCancelarManual').click();
    });
    $('#btnCancelarManual').click(function () {
        $('a.colorManual.editando').removeClass('editando');
        $('.manual > .row').hide();
    });

    $("#ul_icono").on('click', 'li', function () {
        $(this).siblings().removeClass('seleccionado');
        $(this).toggleClass('seleccionado');
        if ($(this).hasClass('seleccionado')) {
            $('#btnAceptarIcono').removeClass('disabled');
        } else {
            $('#btnAceptarIcono').addClass('disabled');
        }
    });
    $('body').on('change', '.tdesde', function () {
        var valorActual = $($(this)).val();
        if (valorActual) {
            var rowId = Number($(".hfRowId", $(this).parents('tr')).val());
            var rowAnterior = 0;
            var leyenda = "";
            if (rowId > 0) {
                rowAnterior = parseInt(rowId - 1);
                $('#thasta' + rowAnterior).val(valorActual);
            }
            //Calcular casos
            var distribucion = $('#cboDistribuciones').val();
            var desde1 = 0;
            var hasta1 = 0;
            var desde2 = $('#tdesde' + rowId).val();
            var hasta2 = $('#thasta' + rowId).val();
            leyenda = desde2 + " - " + hasta2;
            $('#tleyenda' + rowId).val(leyenda);
            if (rowId > 0) {
                desde1 = $('#tdesde' + rowAnterior).val();
                hasta1 = $('#thasta' + rowAnterior).val();
                leyenda = desde1 + " - " + hasta1;
                $('#tleyenda' + rowAnterior).val(leyenda);
            }
            $.ajax({
                url: BASE_URL + "MapasTematicos/CalcularCasos",
                data: { distribucion: distribucion, desde1: desde1, hasta1: hasta1, desde2: desde2, hasta2: hasta2 },
                dataType: 'json',
                type: 'POST',
                success: function (data) {
                    if (rowId > 0) {
                        $('#casos' + rowAnterior).val(data.Casos1);
                    }
                    $('#casos' + rowId).val(data.Casos2);
                    calcularTotalCasos();
                }
            });
        }
    });
    $('body').on('change', '.thasta', function () {
        var valorActual = $($(this)).val();
        if (valorActual) {
            var rowId = Number($(".hfRowId", $(this).parents('tr')).val());
            var cantRangos = parseInt($("#rangos").val());
            var rowPosterior = 0;
            var leyenda = "";
            if (rowId < cantRangos - 1) {
                rowPosterior = parseInt(rowId + 1);
                $('#tdesde' + rowPosterior).val(valorActual);
            }
            //Calcular casos
            var distribucion = $('#cboDistribuciones').val();
            var desde1 = $('#tdesde' + rowId).val();
            var hasta1 = $('#thasta' + rowId).val();
            leyenda = desde1 + " - " + hasta1;
            $('#tleyenda' + rowId).val(leyenda);
            var desde2 = 0;
            var hasta2 = 0;
            if (rowId < cantRangos - 1) {
                desde2 = $('#tdesde' + rowPosterior).val();
                hasta2 = $('#thasta' + rowPosterior).val();
                leyenda = desde2 + " - " + hasta2;
                $('#tleyenda' + rowPosterior).val(leyenda);
            }
            $.ajax({
                url: BASE_URL + "MapasTematicos/CalcularCasos",
                data: { distribucion: distribucion, desde1: desde1, hasta1: hasta1, desde2: desde2, hasta2: hasta2 },
                dataType: 'json',
                type: 'POST',
                success: function (data) {
                    $('#casos' + rowId).val(data.Casos1);
                    if (rowId < cantRangos - 1) {
                        $('#casos' + rowPosterior).val(data.Casos2);
                    }
                    calcularTotalCasos();
                }
            });
        }
    });
    $('body').on('click', '.colorManual', function (evt) {
        $('.manual > .row').show();
        $('.colorManual').not(evt.currentTarget).removeClass('editando');
        $(evt.currentTarget).addClass('editando');
        var color = $(evt.currentTarget).siblings('.colorh').val(),
            contorno = $(evt.currentTarget).siblings('.colorbordeh').val(),
            ancho = $(evt.currentTarget).siblings('.anchobordeh').val(),
            icono = $(evt.currentTarget).siblings('.iconoh').val() || 'glyphicon-one-fine-dot';

        $('#iconoSeleccionadoManual')
            .removeClass(function (_, css) { return (css.match(/(^|\s)glyphicon\S+/g) || []).join(' '); })
            .addClass(icono);

        $('#pickerPrincipalManual').val(color);
        $('span.current-color', $('#pickerPrincipalManual').siblings()).css('background-color', '#' + color);
        $('#pickerContornoManual').val(contorno);
        $('span.current-color', $('#pickerContornoManual').siblings()).css('background-color', '#' + contorno);
        $('#anchoContornoManual').val(ancho);
        $('#pickerPrincipalManual').change();
        AplicarContornoManual();//no llamo al evento para que no me actualice el valor del combo
    });
    actualizarDistribucion();
    $('.zona').each(function (_, elem) { hackbordes(elem, $('input.colorbordeh', $(elem).parents('td')).val(), $('input.anchobordeh', $(elem).parents('td')).val()); });
    ajustarmodal();
    $('#myModalMT').modal('show');
}
function ajustarmodal() {
    var altura = $(window).height() - 190; //value corresponding to the modal heading + footer
    $(".visualizacion-body").css({ "height": altura });
    $(".panel-body", '.estilizador').css({ "height": altura - 144 });
    $(".panel-body", '.iconos').css({ "height": altura - 144 });
    ajustarScrollBars();
}
function ajustarTabla() {
    var alturaTabla = $(".visualizacion-body").height() - 99;
    $("#tablaWrapper").css({ "height": alturaTabla });
    $("#tablaWrapper").niceScroll(getNiceScrollConfig());
}
function ajustarScrollBars() {
    $(".sscroll").getNiceScroll().resize();
    $(".sscroll").getNiceScroll().show();
}

function resetPicker(picker) {
    $(picker).val("FFFFFF");
    $('.color-preview.current-color', $(picker).parent()).css({ 'background-color': '#FFFFFF' });
    $(picker).change();
}
function getColorComponents(numero) {
    var res = new Array();

    res[0] = parseInt("0x" + numero.substr(0, 2));
    res[1] = parseInt("0x" + numero.substr(2, 2));
    res[2] = parseInt("0x" + numero.substr(4, 2));

    return res;
}
function getGradientComponents(n1, n2, z) {
    var i = 0;
    var n = 0;
    var res = new Array();

    n = (n1 - n2) / (z - 1);

    for (i = 0; i < z; i++) {
        res[i] = n2 + (n * ((z - 1) - i));
        res[i] = Math.floor(res[i]);
        res[i] = res[i].toString(16);

        if (res[i].length < 2) res[i] = "0" + res[i];
    }

    return res;
}
function getGradientColors(firstColor, lastColor, gradientZones) {
    //En caso de querer que los colores sean del mas claro valor mas chico a mas oscuro valor mas grande invertir components1 con 2
    var components1 = getColorComponents(firstColor);
    var components2 = getColorComponents(lastColor);

    if (gradientZones > 1) {
        var colors = components1.map(function (cmp, idx) { return getGradientComponents(cmp, components2[idx], gradientZones); });
        var res = [];
        for (i = 0; i < gradientZones; i++) {
            res.push(colors[0][i] + colors[1][i] + colors[2][i]);
        }
    } else {
        return [components1];
    }
    return res;
}
function rgb2hex(rgb) {
    rgb = rgb.match(/^rgba?[\s+]?\([\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?/i);
    return rgb && rgb.length === 4 ? "#" +
        ("0" + parseInt(rgb[1], 10).toString(16)).slice(-2) +
        ("0" + parseInt(rgb[2], 10).toString(16)).slice(-2) +
        ("0" + parseInt(rgb[3], 10).toString(16)).slice(-2) : '';
}
function hex2rgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    var rgb = "";
    if (result) {
        rgb = "rgb(" + parseInt(result[1], 16).toString() + "," + parseInt(result[2], 16).toString() + "," + parseInt(result[3], 16).toString() + ")";
    }
    return rgb;
}
function aceptarIcono() {
    var span = $('span', "#ul_icono li.seleccionado"),
        img_icono = span.attr('class'),
        padding = 0;
    if (!span.hasClass("glyphicon-one-fine-dot")) {
        padding = 10;
    }
    $(".zona").attr('class', img_icono + ' zona');
    $(".zona").css('padding-top', padding + 'px');
    $(".iconoh").val(img_icono);

    $(this).removeClass(function (_, css) { return (css.match(/(^|\s)glyphicon\S+/g) || []).join(' '); })
        .addClass(img_icono);

    resetVistaIconos(this);
}
function cancelarIcono() {
    resetVistaIconos(this);
}
function aceptarIconoManual() {
    var img_icono = $('span', "#ul_icono li.seleccionado").attr('class');

    $(this).removeClass(function (_, css) { return (css.match(/(^|\s)glyphicon\S+/g) || []).join(' '); })
        .addClass(img_icono);

    resetVistaIconos(this);
}
function cancelarIconoManual() {
    resetVistaIconos(this);
}
function resetVistaIconos(btnlanzador) {
    $("#ul_icono li").removeClass("seleccionado");
    $("#ul_icono").parents('.iconos').hide();
    $($(btnlanzador).parents('.panel')[0]).show();
}
function editarManual() {

}

function AplicarColorPrincipal() {
    var colorPrincipal = $('#pickerPrincipal').val();
    var colorSecundario = $('#pickerSecundario').val();

    $("#Visualizacion_ColorPrincipal").val(colorPrincipal);
    $("#Visualizacion_ColorSecundario").val(colorSecundario);

    var rangos = $(".zona");
    if (rangos.length) {
        var colores = getGradientColors(colorPrincipal, colorSecundario, rangos.length);
        $('#iconoSeleccionado').css('color', '#' + colorPrincipal);
        $(rangos[0]).css('color', '#' + colorPrincipal);
        $('input[type=hidden].colorh', $(rangos[0]).parents('td')).val(colorPrincipal);
        for (i = 1; i < rangos.length; i++) {
            $(rangos[i]).css('color', '#' + colores[i]);
            $('input[type=hidden].colorh', $(rangos[i]).parents('td')).val(colores[i]);
        }
    }
}
function AplicarColorContorno() {
    var ancho = Number($('#anchoContorno').val()) || 0,
        color = $('#pickerContorno').val() || "FFFFFF";

    $("#Visualizacion_ColorContorno").val(color);
    $("#Visualizacion_CantidadContorno").val(ancho);
    $('input[type=hidden].colorbordeh', $('.zona').parents('td')).val(color);
    $('input[type=hidden].anchobordeh', $('.zona').parents('td')).val(ancho);
    hackbordes('.zona', color, ancho);
}

function AplicarContornoManual() {
    var ancho = Number($('#anchoContornoManual').val()) || 0,
        color = $('#pickerContornoManual').val() || "FFFFFF";
    hackbordes("#iconoSeleccionadoManual", color, ancho);
}

function actualizarDistribucion() {
    $('#THDesde,#THHasta,#THValor,#rangos,#rangoslbl').hide();

    var distribucion = Number($('#cboDistribuciones').val());
    if (distribucion === 1 || distribucion === 2) {
        $('#THDesde').show();
        $('#THHasta').show();
        $('#rangos').show();
        $('#rangoslbl').show();
    } else {
        $('#THValor').show();
    }
    $(".tdesde").inputmask('numeric', { rightAlign: false });
    $(".thasta").inputmask('numeric', { rightAlign: false });
}
function calcularTotalCasos() {
    var total = 0;
    $(".casos").each(function (_, elem) {
        total += Number($(elem).find(".tfcasos").val());
    });
    $('#totalCasos').html(total);
}

function hackbordes(elem, color, ancho) {
    if (detect.parse(window.navigator.userAgent).browser.family.toLowerCase() === "ie") {
        $(elem).css("text-shadow", ancho + "px 0px 0 #" + color + ", 0px " + ancho + "px 0 #" + color + ", -" + ancho + "px 0px 0 #" + color + ", 0px -" + ancho + "px 0 #" + color);
    } else {
        $(elem).css('-webkit-text-stroke', ancho + 'px #' + color);
    }
}

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    if (fn) {
        $("#btnAceptarInfoMTVisua").one('click', fn);
        $("#ModalInfoMTVisua .modal-footer").show();
    } else {
        $("#ModalInfoMTVisua .modal-footer").hide();
    }
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoMTVisua").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoMTVisua").html(titulo);
    $("#DescripcionInfoMTVisua").html(mensaje);
    $("#ModalInfoMTVisua").modal('show');
    return false;
}
//# sourceURL=visualizacion.js