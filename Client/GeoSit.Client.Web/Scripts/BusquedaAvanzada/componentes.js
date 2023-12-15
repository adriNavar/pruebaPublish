$('#myModalBA').one('shown.bs.modal', function (e) {
    habilitarBtnSiguiente();
    ajustarmodal();
    hideLoading();
});
$(document).ready(function () {
    ///////////////////// Scrollbars ////////////////////////
    $(".selector-body").niceScroll(getNiceScrollConfig());
    $(window).resize(ajustarmodal);
    ////////////////////////////////////////////////////////
    $("#clearSearch").click(function () {
        $("#buscar").val('');
        $("#buscar").keyup();
    });

    $("#volver").on('click', function (evt) {
        evt.preventDefault();
        evt.stopImmediatePropagation();
        $("#myModalBA").modal("hide");
        loadView(BASE_URL + "BusquedaAvanzada/GetBibliotecas");
    });

    $("#cerrar").click(function () {
        var msj = "¿Desea salir del wizard del Búsqueda Avanzada? ";
        alerta('Búsqueda Avanzada: Componentes', msj, 2, function () {
            $("#myModalBA").modal('hide');
        });
        return false;
    });

    $('#btnAceptarInfoBAComponentes').on('click', function (e) {
        if (fnResultAlertaCompo) fnResultAlertaCompo();
    });

    $("#myModalBA").on('click', 'li', function (evt) {
        evt.preventDefault();
        evt.stopImmediatePropagation();
        if ($(this).find("input[type=checkbox]").attr('checked')) {
            $(this).find("input[type=checkbox]").removeAttr('checked');
        } else {
            $(this).find("input[type=checkbox]").attr('checked', 'checked');
        }
        $(this).toggleClass("seleccionado");
        habilitarBtnSiguiente();
    });

    var componenteSelect = $("#hfComponenteSeleccionado").val();

    if (componenteSelect > 0) {
        $("input[value=" + componenteSelect + "]").parents("li").click();
    }

    $('#btn-componente-siguiente').on('click', function (evt) {
        evt.preventDefault();
        evt.stopImmediatePropagation();

        var idsarray = new Array();
        $("#form-componente input[type=checkbox]:checked").each(function () {
            //aca ya obtengo los ids de los componentes seleccionados                
            idsarray.push($(this).prop('value'));
        });
        $('#myModalBA').modal('hide');
        setTimeout(function () {
            loadView(BASE_URL + 'BusquedaAvanzada/GetAgrupamientoView?ids=' + idsarray);
        }, 100);
    });


    $("#buscar").keyup(function () {
        var texto = $("#buscar").val();
        if (texto) {
            $("#componentes li").hide();
            //dice spans pero luego del estilado son labels
            var spans = $("#componentes li").find("label");
            $.each(spans, function (i, span) {
                if ($(span).text().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(span).closest("li").show();
                }
            });
            $("#componentes li:hidden").children("div").css("background-color", "");
            $("#componentes li:hidden").children("div").find(".imagen").show();
            $("#componentes li:hidden").children("div").find(".fondoLogo").addClass("seleccionado");
        } else {
            $("#componentes li").show();
        }
        habilitarBtnSiguiente();
        return false;
    });
    //Previene que avancen con Enter
    $(window).keydown(function (event) {
        if (event.keyCode === 13 && $("#myModalBA").css('display') !== 'none') {
            event.preventDefault();
            return false;
        }
    });
    $("#myModalBA").modal("show");
});

var fnResultAlertaCompo = null;
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlertaCompo = fn;
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
    $("#MensajeInfoBAComponentes").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoBAComponentes").html(titulo);
    $("#DescripcionInfoBAComponentes").html(mensaje);
    $("#ModalInfoBAComponentes").modal('show');
    return false;
}

function ajustarmodal() {
    var altura = $(window).height() - 190; //value corresponding to the modal heading + footer
    var alturaListado = altura - 50;
    $(".componentes-body").css({ "height": altura });
    $(".selector-body").css({ "height": alturaListado });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $(".selector-body").getNiceScroll().resize();
    $(".selector-body").getNiceScroll().show();
}
function habilitarBtnSiguiente() {
    if ($("#componentes li:not(:hidden).seleccionado input[type=checkbox]:checked").length) {
        $("#btn-componente-siguiente").removeClass("disabled");
    } else {
        $("#btn-componente-siguiente").addClass("disabled");
    }
}