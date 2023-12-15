$(window).resize(ajustarmodal);
$(document).ready(init);
$('#modal-window-informe-expediente-obra').on('shown.bs.modal', function (e) {
    //ajustarScrollBars();
    hideLoading();
    $("#numero").inputmask('Regex', { regex: $("#ExpresionRegularLegajo").val() });
    $("#regularExpressionMessage").html("El numero de 'Expediente de Obra' debe tener el siguiente formato: " + $("#ExpresionRegularLegajoVisible").val())
});

function init() {
    $(".informeexp-content").niceScroll(getNiceScrollConfig());
    ajustarmodal();
    
    $("#modal-window-informe-expediente-obra").modal('show');
    $("#aceptar-informe-expediente-obra").click(function () {

        var ExpOLeg = $('input[name=tipoBusqueda]:checked').val(),
            numero = $('#numero').val(),
            ExpRegLeg = $("#ExpresionRegularLegajo").val(),
            ExpRegExp = $("#ExpresionRegularExpediente").val(),
            viewExpLeg = $("#ExpresionRegularLegajoVisible").val(),
            viewExpExp = $("#ExpresionRegularExpedienteVisible").val();

        if ($('#numero').val() == '') {
            alerta('Advertencia', 'Debe ingresar un número de legajo o expediente valido.', 3);
        }
        else if (numero !== '' && ExpOLeg === "L")
        {
            if (numero.match(ExpRegLeg) != null)
            {
                GetInformacionExpedienteObra(numero);
            } else
            {
                alerta('Advertencia', 'El número de lejago debe tener el siguiente formato: ' + viewExpLeg, 3);
            }
        }
        else
        {
            if (numero.match(ExpRegExp) != null) {
                GetInformacionExpedienteObra(numero);
            } else {
                alerta('Advertencia', 'El número de expediente debe tener el siguiente formato: ' + viewExpLeg, 3);
            }
        }

    });

    $('input[name=tipoBusqueda]').click(function () {
        var ExpOLeg = $('input[name=tipoBusqueda]:checked').val()
        $("#numero").val('');
        if (ExpOLeg === "L") {
            $("#numero").inputmask('Regex', { regex: $("#ExpresionRegularLegajo").val() });
            $("#regularExpressionMessage").html("El numero de 'Expediente de Obra' debe tener el siguiente formato: " + $("#ExpresionRegularLegajoVisible").val())
        }
        else {
            $("#numero").inputmask('Regex', { regex: $("#ExpresionRegularExpediente").val() });
            $("#regularExpressionMessage").html("El numero de 'Expediente de Obra' debe tener el siguiente formato: " + $("#ExpresionRegularExpedienteVisible").val())
        }
        $("#regularExpressionMessage").addClass("hide");
    });

    $("#numero").keyup(function () {
        var ExpOLeg = $('input[name=tipoBusqueda]:checked').val()
        var numero = $("#numero").val();
        if (ExpOLeg === "L") {
            if (numero.match($("#ExpresionRegularLegajo").val()) == null) {
                $("#regularExpressionMessage").removeClass("hide");
            } else {
                $("#regularExpressionMessage").addClass("hide");
            }
        } else {
            if (numero.match($("#ExpresionRegularExpediente").val()) == null) {
                $("#regularExpressionMessage").removeClass("hide");
            } else {
                $("#regularExpressionMessage").addClass("hide");
            }
        }
    })
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

    $("#MensajeInfoInspector").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoInspector").html(titulo);
    $("#DescripcionInfoInspector").html(mensaje);
    $("#ModalInfoInspector").modal('show');
    $("#botones-modal-info").find("span:last").show();
}

function ajustarmodal() {
    //var altura = $(window).height() - 500;
    //$(".informeexp-body").css({ "height": altura });
    //$('.informeexp-content').css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $('.inspectores-content').collapse('show');
    temp = $(".inspectores-body").height();
    var outerHeight = 20;
    $('#accordion-inspectores .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-inspectores .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.inspectores-content').css({ "max-height": temp + 'px' })
    $('#accordion-inspectores').css({ "max-height": temp + 1 + 'px' })
    $(".inspectores-content").getNiceScroll().resize();
    $(".inspectores-content").getNiceScroll().show();
}

function GetInformacionExpedienteObra(numero) {
    var formData = {
        "numero": numero,
        "tipoBusqueda": $('input[name=tipoBusqueda]:checked').val()
    };
    showLoading();
    $.ajax({
        url: BASE_URL + 'ExpedienteObra/GenerateInformeExpedienteObra',
        type: 'POST',
        data: formData,
        success: function (data) {
            if (data.success) {
                window.open(BASE_URL + "ExpedienteObra/GetFileInformeExpedienteObra?file=" + data.file);
            } else {
                hideLoading();
                alerta('Advertencia', data.message, 3);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoading();
            alerta('Error', errorThrown, 3);
        }
    }).always(function () {
        hideLoading();
    });
}

//@ sourceURL=InformeExpedienteObra.js