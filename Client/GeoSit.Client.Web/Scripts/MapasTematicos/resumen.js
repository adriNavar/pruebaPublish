$('#myModalMT').one('shown.bs.modal', function (e) {
    hideLoading();
});
$(window).resize(ajustarmodal);

$(document).ready(function () {
    GeoSIT.MapaController.agregarItemMenu('Último Mapa Temático', 'fa-history', function () { loadView(BASE_URL + 'MapasTematicos/CargarUltimoMapaTematico'); }, 'BA_MT', 'ultimaMT');
    $("#form-componente").ajaxForm({
        beforeSubmit:showLoading,
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#form-atributo").ajaxForm({
        beforeSubmit: showLoading,
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#form-filtros").ajaxForm({
        beforeSubmit: showLoading,
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#form-visualizacion").ajaxForm({
        beforeSubmit: showLoading,
        success: reload,
        error: function () {
            alert("error al volver");
        },
        complete: hideLoading
    });
    $("#form-resumen").ajaxForm({
        beforeSubmit: showLoading,
        success: function (data) {
            $('#myModalMT').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            GeoSIT.MapaController.agregarCapaTemporal(data.config);
            GeoSIT.MapaController.zoomExtent(data.extent);
        },
        error: function () {
            alert("error al finalizar");
        },
        complete: hideLoading
    });

    ///////////////////// Scrollbars ////////////////////////
    $("#scroll-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content').resize(ajustarScrollBars);
    ////////////////////////////////////////////////////////
  
    $("#salir").click(function () {
        var msj = "Desea salir del wizard del Mapa Temático? ";
        alerta('Mapa Temático - Resumen', msj, 2, function () {
            $("#myModalMT").modal('hide');
        });
        return false;
    });

    $('#guardarBiblioteca').click(function () {
        showLoading();
        $.ajax({
            type: "POST",
            url: BASE_URL + "MapasTematicos/GuardarBiblioteca",
            data: { nombre: $('#tituloNombre').val(), descripcion: $('#descripcion').val() },
            dataType: 'json',
            success: function (response) {
                if (Number(response) === 1) {
                    alerta("Mapas Temáticos - Resumen", "Se guardó la biblioteca correctamente.", 1);
                } else {
                    alerta('Mapas Temáticos - Resumen', 'Ya existe una definición con dicho nombre. Cambie el nombre para continuar', 3);
                }
            },
            error: function (error) {
                alerta('Mapa Temático - Resumen', `Error al guardar la biblioteca: Error: ${error.status}`, 3);
            },
            complete: hideLoading
        });
    });
    $('#guardarColeccion').click(function () {
        showLoading();
        $.ajax({
            type: "POST",
            url: BASE_URL + "MapasTematicos/GuardarColeccion",
            data: { nombre: $('#tituloNombre').val() },
            dataType: 'json',
            success: function () {
                alerta("Mapas Temáticos - Resumen", "La colección se generó correctamente.", 1);
            },
            error: function (error) {
                alerta('Mapas Temáticos - Resumen',`Error al guardar la colección: Error: ${error.status}`,3);
            },
            complete: hideLoading
        });
    });
    $('#exportarExcel').click(function () {
        window.location = BASE_URL + 'MapasTematicos/ExportarExcelResMT';
        return false;
    });

    $("#editarComponente > span").click(function () {
        $("#form-componente").submit();
    });
    $("#editarAtributo > span").click(function () {
        $("#form-atributo").submit();
    });
    $("#editarFiltros > span").click(function () {
        $("#form-filtros").submit();
    });
    $("#editarVisualizacion > span").click(function () {
        $("#form-visualizacion").submit();
    });
    $("#btn-generar").click(function () {
        $("#form-resumen").submit();
    });
    ajustarmodal();
    $("#myModalMT").modal("show");
});

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    if (fn) {
        $("#btnAceptarInfoMTResu").one('click', fn);
        $("#ModalInfoMTResu .modal-footer").show();
    } else {
        $("#ModalInfoMTResu .modal-footer").hide();
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
    $("#MensajeInfoMTResu").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoMTResu").html(titulo);
    $("#DescripcionInfoMTResu").html(mensaje);
    $("#ModalInfoMTResu").modal('show');
    return false;
}
function ajustarmodal() {
    var altura = $(window).height() - 190,
        alturaFiltros = altura - 106;

    $(".resumen-body").css({ "height": altura });
    $(".panel.accesos", ".panel-group.informacion").css({ "height": alturaFiltros + 50 });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $(".selector-body,.resumen").getNiceScroll().resize();
    $(".selector-body,.resumen").getNiceScroll().show();
}
function ajustarScrolls() {
    setTimeout(ajustarScrollBars, 100);
}


function reload(data) {
    $('#myModalMT').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    setTimeout(function () { $("#contenido").html(data); }, 200);
}
//@ sourceURL=resumenMT.js