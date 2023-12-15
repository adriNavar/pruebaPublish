$('#myModalBA').one('shown.bs.modal', function () {
    habilitarBtnSiguiente();
    ajustarmodal();
    hideLoading();
});
$(document).ready(function () {
    $("#formVolver").ajaxForm({
        beforeSubmit: showLoading,
        success: function (data) {
            $('#myModalBA').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () { $("#contenido").html(data); }, 500);
        },
        error: function () {
            alert("error al volver");
        }
    });

    $("#salir").click(function () {
        var msj = "¿Desea salir del wizard del Búsqueda Avanzada? ";
        alerta('Búsqueda Avanzada: Agrupamiento', msj, 2, function () {
            $("#myModalBA").modal('hide');
        });
        return false;
    });
    $("#clearSearch").click(function () {
        $("#buscar").val('');
        $("#buscar").keyup();
    });
    ///////////////////// Scrollbars ////////////////////////
    $(".selector-body").niceScroll(getNiceScrollConfig());
    $(window).resize(ajustarmodal);
    ////////////////////////////////////////////////////////

    $('#btnAceptarInfoBAAtrib').on('click', function (e) {
        if (fnResultAlertaAtrib !== null) fnResultAlertaAtrib();
    });

    if (!$("#ComponenteUL li").length) {
        $("#AgrupamientoUL li:last").hide();
        $("#OperacionesUL").hide();
    }

    $('li', '.selector-body ul').on('click', function () {
        $(this).siblings().removeClass('seleccionado').find("input[type=checkbox]").prop('checked', false);
        $(this).addClass('seleccionado');
        $(this).find("input[type=checkbox]").prop('checked', true);
        habilitarBtnSiguiente();
    });
    $('li', '.selector-body #AgrupamientoUL').on('click', function () {
        /* sin agrupamiento: 0, con agrupamiento: 1*/
        if ($(this).index() === 1) {
            $("#ComponenteUL").show();
            $("#OperacionesUL").show();
            $("input[type=checkbox]:first", "#ComponenteUL").closest("li").click();
        } else {
            $("#ComponenteUL").hide();
            $("#OperacionesUL").hide();
        }
    });

    //siguiente boton
    $('#btn-atributo-siguiente').on("click", function () {
        var ConOsinAgrup = $("input[type=checkbox]:checked", "#AgrupamientoUL li div").val(),
            compId = "", Operaciones = "";

        if (ConOsinAgrup === "Con Agrupamiento") {
            ConOsinAgrup = 1;
            compId = $("input[type=checkbox]:checked", "#ComponenteUL li div").val();
            Operaciones = $("input[type=checkbox]:checked", "#OperacionesUL li div").val();
        } else {
            ConOsinAgrup = 0;
        }

        $('#myModalBA').modal('hide');
        setTimeout(function () {
            loadView(BASE_URL + 'BusquedaAvanzada/GetFiltrosView?agrupamiento=' + ConOsinAgrup + '&attrAgrup=' + compId + '&operaciones=' + Operaciones);
        }, 100);
    });

    //Previene que avancen con Enter
    $(window).keydown(function (event) {
        if (event.keyCode == 13 && $("#myModalBA").css('display') != 'none') {
            event.preventDefault();
            return false;
        }
    });

    if ($("#hfComponenteId").val() > 0) {
        $("#AgrupamientoUL li:last").click();
        $("#ComponenteUL li input[value=" + $("#hfComponenteId").val() + "]").parents("li").click();
        $("#OperacionesUL li input[value=" + $("#hfOperacion").val() + "]").parents("li").click();
    } else {
        $("#AgrupamientoUL li:first").click();
    }
    $("#buscar").keyup(function () {
        var texto = $("#buscar").val();
        if (texto) {
            $("#ComponenteUL li").hide();
            //dice spans pero luego del estilado son labels
            var spans = $("#ComponenteUL li label");
            $.each(spans, function (i, span) {
                if ($(span).text().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(span).closest("li").show();
                }
            });

            $("#ComponenteUL li:hidden").removeClass("seleccionado").find("input[type=checkbox]").prop('checked', false);
            $("#ComponenteUL li:hidden span").removeClass("tilde-seleccion");

            if ($("#ComponenteUL li input[type=checkbox]:checked").length < 1) {
                $("#AtributoUL").empty();
                $("#AgrupacionesUL").empty();
            }
        } else {
            $("#ComponenteUL li").show();
        }
        habilitarBtnSiguiente();
        return false;
    });

    $("#volver").on('click', function (evt) {
        $("#formVolver").submit();
    });
    $("#myModalBA").modal("show");
});

var fnResultAlertaAtrib = null;
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlertaAtrib = fn;
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
    $("#MensajeInfoBAAtrib").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoBAAtrib").html(titulo);
    $("#DescripcionInfoBAAtrib").html(mensaje);
    $("#ModalInfoBAAtrib").modal('show');
    return false;
}
function ajustarmodal() {
    var altura = $(window).height() - 190; //value corresponding to the modal heading + footer
    var alturaListas = altura - 94;
    $(".agrupamiento-body").css({ "height": altura });
    $(".selector-body").css({ "height": alturaListas });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $(".selector-body").getNiceScroll().resize();
    $(".selector-body").getNiceScroll().show();
}
function habilitarBtnSiguiente() {
    if ($("#AgrupamientoUL li.seleccionado:first-of-type").length ||
        ($("#AgrupamientoUL li.seleccionado:last-of-type").length &&
            $("#ComponenteUL li.seleccionado:not(:hidden)").length &&
            $("#OperacionesUL li.seleccionado").length)) {
        $('#btn-atributo-siguiente').removeClass('disabled');
    } else {
        $('#btn-atributo-siguiente').addClass('disabled');
    }
}