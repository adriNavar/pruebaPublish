$('#myModalMT').one('shown.bs.modal', function () {
    habilitarBtnSiguiente();
    ajustarmodal();
    hideLoading();
});

$(window).resize(ajustarmodal);
$(document).ready(function () {
    ///////////////////// Scrollbars ////////////////////////
    $(".selector-body").niceScroll(getNiceScrollConfig());
    $(window).resize(ajustarScrollBars);
    ////////////////////////////////////////////////////////
    $("#clearSearch").click(function () {
        $("#buscar").val('');
        $("#buscar").keyup();
    });

    $("#volver").on('click', function (evt) {
        $("#myModalMT").modal("hide");
        loadView(BASE_URL + "MapasTematicos/GetBibliotecas");
    });

    $("#cerrar").click(function () {
        var msj = "¿Desea salir del wizard de Mapas Temáticos? ";
        alerta('Mapas Temáticos: Componentes', msj, 2, function () {
            $("#myModalMT").modal('hide');
        });
        return false;
    });

    $("#myModalMT").on('click', 'li', function () {
        $("input[type=checkbox]", $(this).siblings()).removeAttr('checked');
        if ($("input[type=checkbox]", this).attr('checked')) {
            $("input[type=checkbox]", this).removeAttr('checked');
            $("#hfComponenteId").val(null);
        } else {
            $("input[type=checkbox]", this).attr('checked', 'checked');
            $("#hfComponenteId").val($('input[type=checkbox]', this).val());
        }
        $(this).siblings().removeClass("seleccionado");
        $(this).toggleClass("seleccionado");
        habilitarBtnSiguiente();
    });

    var componenteSelect = $("#hfComponenteSeleccionado").val();

    if (componenteSelect > 0) {
        $("input[value=" + componenteSelect + "]").parents("li").click();
    }
    $("#form-componente").ajaxForm({
        beforeSubmit: showLoading,
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () { $("#contenido").html(data); }, 500);
        },
        error: function () {
            alert("Error al cargar los atributos");
        }
    });
    $('#btn-componente-siguiente').on('click', function () {
        $("#form-componente").submit();
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
    $("#myModalMT").modal("show");
});

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    $("#botones-modal-info-mt").find("span:last").hide();
    $('#btnAceptarInfoMTBiblio').off('click');
    $('#ModalInfoMTComponentes').off('hidden.bs.modal');
    if (tipo !== 2 && fn) {
        $('#ModalInfoMTComponentes').one('hidden.bs.modal', fn);
    } else if (fn) {
        $("#btnAceptarInfoMTComponentes").one('click', fn);
    }
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $("#botones-modal-info-mt").find("span:last").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoMTComponentes").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoMTComponentes").html(titulo);
    $("#DescripcionInfoMTComponentes").html(mensaje);
    $("#ModalInfoMTComponentes").modal('show');
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
    if ($("input[type=checkbox][checked]","#componentes li:not(:hidden)").length) {
        $("#btn-componente-siguiente").removeClass("disabled");
    } else {
        $("#btn-componente-siguiente").addClass("disabled");
    }
}
//# sourceURL=componentes.js