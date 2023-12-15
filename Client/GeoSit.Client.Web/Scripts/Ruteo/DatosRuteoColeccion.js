$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-ruteo').on('shown.bs.modal', function (e) {
    ajustarScrollBars();
    hideLoading();
});

function init() {
    $('#btnAceptar').click(function () {
        var objetoInicio = $('.posicion-objeto > span.label-start').parents("li"),
            objetoFin = $('.posicion-objeto > span.label-end').parents("li"),
            objetosWaypoint = $('.posicion-objeto:visible span:not(".label-start"):not(".label-end")').parents("li").not(objetoInicio).not(objetoFin);

        if (objetoInicio.length === 0 || objetoFin.length === 0) {
            alerta('Error', 'Por favor seleccione los puntos Inicio y Fin para comenzar el ruteo.', 3);
            return;
        } else if (objetosWaypoint.length > 23) {
            alerta('Error', 'Solo se permite elegir veintitrés objetos intermedios más uno de Origen y uno de Destino.', 3);
            return;
        }

        obtieneIndicaciones(objetoInicio, objetoFin, objetosWaypoint);
    });

    $('#btnCerrar').click(function () {
        $('#modal-window-ruteo').modal('hide');
        loadView(BASE_URL + 'Coleccion/GestionarColecciones');
    });

    /* evento click de la coleccion */
    $('span[name="check-coleccion"]').click(function (event) {
        event.stopPropagation();
        $('span[name="check-componente"]').click();
    });

    /* evento click de los componentes */
    $('span[name="check-componente"]').click(function (event) {
        event.stopPropagation();
        $('ul>li', $(this).parents('.cabecera-componente')).click();
    });

    /* evento click de los objetos */
    $('ul[id*="ul-objetos-"]').on('click', 'li', function (e, s) {
        e.stopPropagation();
        if ($('span', this).hasClass('fa-square-o')) {
            //check y marco como waypoint
            $('span[id*="check-objeto"]', this).removeClass('fa-square-o').addClass('fa-check-square-o');
            $('.posicion-objeto', this).addClass('waypoint');
            $('.posicion-objeto>span', this).removeClass().addClass('label label-default cursor-pointer');
            $('.posicion-objeto', this).show();
        } else {
            //uncheck y limpio
            $('span[id*="check-objeto"]', this).removeClass('fa-check-square-o').addClass('fa-square-o');
            $('.posicion-objeto', this).hide();
            $('.posicion-objeto>span', this).removeClass().addClass('label label-default cursor-pointer');
        }
        checkComponentes(this);
        checkColeccion();
    });

    /* evento click de los botones de inicio/fin */
    $('.posicion-objeto>span').on('click', function (event) {
        event.stopPropagation();
        var span = $(this);

        if (span.hasClass('label-default')) {
            if (span.text() === 'I') {
                $('span[name="start-point"]').removeClass().addClass('label label-default cursor-pointer');
                span.addClass('label-start');
            } else {
                $('span[name="end-point"]').removeClass().addClass('label label-default cursor-pointer');
                $('.posicion-objeto:visible span:not(".label-start")').parent()
                span.addClass('label-end');
            }
        }
    });

    /* evento radio botones modo ruteo */
    $('#driving-mode-option, #walking-mode-option').click(function (event) {
        var spanOption = $('span.fa', this);
        var id = $(this).attr('id');

        if (spanOption.hasClass('fa-circle-o')) {
            spanOption.removeClass('fa-circle-o').addClass('fa-dot-circle-o');
            if (id === 'driving-mode-option') {
                $('#walking-mode-option>span.fa').removeClass('fa-dot-circle-o').addClass('fa-circle-o');
                $('input[id="modo-tipo-ruteo"]').val('driving');
            } else {
                $('#driving-mode-option>span.fa').removeClass('fa-dot-circle-o').addClass('fa-circle-o');
                $('input[id="modo-tipo-ruteo"]').val('walking');
            }
        }
    });

    $('.row', '#modal-window-ruteo').mouseover(function (event) {
        $(this).css('background-color', '#97c2d0');
        $('span[id*="check-objeto"]', this).removeClass('light-blue').css('color', 'black');
    });

    $('.row', '#modal-window-ruteo').mouseout(function (event) {
        $(this).css('background-color', 'white');
        $('span[id*="check-objeto"]', this).addClass('light-blue');
    });

    $(".ruteo-content").niceScroll(getNiceScrollConfig());

    $('.ruteo-content .panel-heading').click(function () {
        setTimeout(function () {
            ajustarScrollBars();
        }, 10);
    });

    $("#clearSearch").click(function () {
        $("#filtro-ruteo").val('');
        $("#filtro-ruteo").keyup();
    });

    $("#filtro-ruteo").keyup(function () {
        var texto = $("#filtro-ruteo").val();
        if (texto !== "") {
            $("li[id*='li-objeto']").hide();
            $(".cabecera-componente").hide();
            var divs = $("li[id*='li-objeto']").find("#label-objeto");
            $.each(divs, function (i, div) {
                if ($(div).html().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(div).closest("li").show();
                    $(div).closest(".cabecera-componente").show();
                }
            });
        } else {
            $(".cabecera-componente").show();
            $("li[id*='li-objeto']").show();
        }

        ajustarScrollBars();
    });

    $('#modal-window-ruteo').modal('show');
}

function obtieneIndicaciones(inicio, fin, intermedios) {
    var waypoints = new Array();

    $(intermedios).each(function (i, e) {
        waypoints.push({
            ObjetoId: $(e).children('input[name="id"]').val(),
            Componente: $(e).children('input[name="componente"]').val(),
            Descripcion: $(e).children('input[name="descripcion"]').val(),
            Latitud: $(e).children('input[name="latitud"]').val(),
            Longitud: $(e).children('input[name="longitud"]').val()
        });
    });

    var ruteoModel = {
        Modo: $('input[id="modo-tipo-ruteo"]').val(),
        Inicio: {
            ObjetoId: inicio.children('input[name="id"]').val(),
            Componente: inicio.children('input[name="componente"]').val(),
            Descripcion: inicio.children('input[name="descripcion"]').val(),
            Latitud: inicio.children('input[name="latitud"]').val(),
            Longitud: inicio.children('input[name="longitud"]').val()
        },
        Fin: {
            ObjetoId: fin.children('input[name="id"]').val(),
            Componente: fin.children('input[name="componente"]').val(),
            Descripcion: fin.children('input[name="descripcion"]').val(),
            Latitud: fin.children('input[name="latitud"]').val(),
            Longitud: fin.children('input[name="longitud"]').val()
        },
        Waypoints: waypoints
    };

    //showLoading(true);
    $('#modal-window-ruteo').modal('hide');
    loadView('Ruteo/IndicacionesRuteo', ruteoModel)
    //$.ajax({
    //    type: 'POST',
    //    url: BASE_URL + 'Ruteo/IndicacionesRuteo',
    //    data: JSON.stringify(ruteoModel),
    //    contentType: 'application/json',
    //    success: function (data) {
    //        $('#modal-window-ruteo').modal('hide');
    //        $('#dialogo').html(data);
    //    },
    //    error: function (jqXHR, textStatus, errorThrown) {
    //        hideLoading();
    //        alerta(textStatus, errorThrown, 3);
    //    }
    //});
}

function checkColeccion() {
    var hasUnchecked = $('span[name="check-componente"]').hasClass('fa-square');
    if (hasUnchecked) {
        $('span[name="check-coleccion"]', this).removeClass('fa-check-square').addClass('fa-square');
    } else {
        $('span[name="check-coleccion"]', this).removeClass('fa-square').addClass('fa-check-square');
    }
}

function checkComponentes(sender) {
    var hasUnchecked = $('ul>li span', $(sender).parents('.cabecera-componente')).hasClass('fa-square-o');

    var parentComponente = $('span[name="check-componente"]', $(sender).parents('.cabecera-componente'));
    if (hasUnchecked) {
        parentComponente.removeClass('fa-check-square').addClass('fa-square');
    } else {
        parentComponente.removeClass('fa-square').addClass('fa-check-square');
    }
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

    $("#MensajeInfoRuteo").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoRuteo").html(titulo);
    $("#DescripcionInfoRuteo").html(mensaje);
    $("#ModalInfoRuteo").modal('show');
}

function getMaxHeight() {
    return $(window).height() - 260; //220: cabecera y pie;
}

function ajustarmodal() {
    var altura = getMaxHeight();
    $('.ruteo-body').css({ "max-height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
   $('.ruteo-content').css({ "max-height": getMaxHeight() - 30 + 'px' })

    $(".ruteo-content").getNiceScroll().resize();
    $(".ruteo-content").getNiceScroll().show();
}
//# sourceURL=datosruteo.js