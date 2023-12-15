$('#myModalBA').on('shown.bs.modal', function () {
    ajustarmodal();
    hideLoading();
});

$(document).ready(function () {
    GeoSIT.MapaController.agregarItemMenu('Última Búsqueda Avanzada', 'fa-history', function () { loadView(BASE_URL + 'BusquedaAvanzada/CargarUltimaBusquedaAvanzada'); }, 'BA_MT', 'ultimaBA');
    $("#formVolver").ajaxForm({
        beforeSubmit: showLoading,
        success: function (data) {
            $('#myModalBA').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            setTimeout(function () { $("#contenido").html(data); }, 100);
        },
        error: function () {
            alert("error al volver");
        }
    });

    $('#myModalBA').on('shown.bs.modal', function (e) {
        ajustarScrollBars();
        hideLoading();
    });
    ///////////////////// Scrollbars ////////////////////////
    $(".columna", "#resultado-content").niceScroll(getNiceScrollConfig());
    $(window).resize(ajustarmodal);

    $('.panel-heading', "#resultado-content").click(function () {
        setTimeout(function () {
            ajustarScrollBars();
        }, 10);
    });
    ////////////////////////////////////////////////////////

    $("#volver").one('click', function () {
        $("#formVolver").submit();
    });

    $("#aceptar").click(function () {
        $("#ModalVisualizacion").hide();
    });

    $("#cerrar").click(function () {
        var msj = "¿Desea salir del wizard del Búsqueda Avanzada? ";
        alerta('Búsqueda Avanzada: Resultado', msj, 2, function () {
            $("#myModalBA").modal('hide');
        });
        return false;
    });

    $("#myModalBA").modal("show");

    if (Number($('#hfCantRangosBA').val()) === 0) {
        alerta('', 'No hay información para los datos consultados.', 4, null);
    }

    setTimeout(function () {
        $('.grilla-atributos', '#accordion-detalle-objeto-ba').DataTable({
            "paging": false,
            "searching": false,
            "bInfo": false,
            "order": [],
            "aaSorting": [[0, 'asc']],
            "language": {
                "url": BASE_URL + "Scripts/dataTables.spanish.txt"
            }
        });
    }, 1);

    setTimeout(function () {
        $('.grilla-relaciones', '#accordion-detalle-objeto-ba').DataTable({
            "paging": false,
            "searching": false,
            "bInfo": false,
            "aaSorting": [[0, 'asc']],
            "language": {
                "url": BASE_URL + "Scripts/dataTables.spanish.txt"
            }
        });
    }, 1);

    setTimeout(function () {
        $('table[data-tipo-grafico]', '#accordion-detalle-objeto-ba').DataTable({
            "paging": false,
            "searching": false,
            "bInfo": false,
            "aaSorting": [[0, 'asc']],
            "language": {
                "url": BASE_URL + "Scripts/dataTables.spanish.txt"
            }
        });
    }, 1);
});

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    $(".modal-footer", "#ModalInfoBAVisua").hide();
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $('#btnAceptarInfoBAVisua').one('click', fn);
            $(".modal-footer", "#ModalInfoBAVisua").show();
            $('#ModalInfoBAVisua').one('hidden.bs.modal', function () {
                $('#btnAceptarInfoBAVisua').off();
            });
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoBAVisua").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoBAVisua").html(titulo);
    $("#DescripcionInfoBAVisua").html(mensaje);
    $("#ModalInfoBAVisua").modal('show');
    return false;
}

function ajustarmodal() {
    var altura = $(window).height() - 190; //value corresponding to the modal heading + footer

    $(".resultado-body").css({ "height": altura });
    $(".columna", "#resultado-content").css({ "height": altura - 64 });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $(".columna", "#resultado-content").getNiceScroll().resize();
    $(".columna", "#resultado-content").getNiceScroll().show();
}

$("ul.objetos > li").on("click", function (e) {
    showLoading();
    if (!$(this).hasClass("seleccionado")) {
        $(this).addClass("seleccionado");
        var idObjeto = $(this).attr("objetoid");
        var idComponente = $(this).attr("componenteid");
        $.ajax({
            url: BASE_URL + 'DetalleObjeto/GetDetalleObjeto?objetoId=' + idObjeto + "&componenteId=" + idComponente,
            type: 'POST',
            success: function (result) {
                loadDetalleObjeto("#accordion-detalle-objeto-ba", result);
                setTimeout(ajustarScrollBars, 200);
            },
            error: function () {
                alert('Error');
            },
            complete: hideLoading
        });
    } else {
        $(this).removeClass("seleccionado");
        hideLoading();
    }
});

$('#guardarBiblioteca').click(function () {
    $("#ModalNombreBiblioteca").modal("show");
});

$("#btnAceptarNuevaBiblioteca").on("click", function () {
    showLoading();
    var nombre = $('#nombreBibliotecaIndex').val();
    var descripcion = $('#nombreBibliotecaDescripcion').val();
    $.ajax({
        type: "POST",
        url: BASE_URL + "BusquedaAvanzada/GuardarBiblioteca",
        data: { nombre: nombre, descripcion: descripcion },
        dataType: 'json',
        success: function (response) {
            if (response === 1) {
                alerta("Búsqueda Avanzada", "Se guardó la biblioteca correctamente.", 1);
            } else {
                alerta("Búsqueda Avanzada", "Ya existe una definición con dicho nombre. Cambie el nombre para continuar.", 2);
            }
        },
        error: function (error) {
            alert("Error al guardar la biblioteca: Status " + error.status);
        },
        complete: hideLoading
    });
});

$("#pasarGrillaDeResultados").on("click", function () {
    var clase = '';
    if ($("ul.objetos > li.seleccionado").length) {
        clase = '.seleccionado';
    }

    var seleccionados = $("ul.objetos > li" + clase).map(function () { return { objeto: Number($(this).attr("objetoid")), componente: Number($(this).attr("componenteid")) }; }).toArray();
    $.ajax({
        type: "POST",
        url: BASE_URL + "BusquedaAvanzada/PasarGrillaResultados",
        dataType: 'json',
        data: { seleccionados: JSON.stringify(seleccionados) },
        cache: false,
        success: function (response) {
            if (response.success) {
                var agrupado = {};
                response.data.forEach(function (elem) { return agrupado[elem.layer] = (agrupado[elem.layer] || []).concat(parseInt(elem.id)); });

                var busqueda = { seleccion: [], capas: [] };
                Object.keys(agrupado).forEach(function (key) { busqueda.seleccion.push(agrupado[key]); busqueda.capas.push(key); });

                GeoSIT.SolrController.searchByFeatures(busqueda, 2);

                $("#myModalBA").hide();
                $(".modal-backdrop").hide();
            }
        },
        error: function (error) {
            alerta("Búsqueda Avanzada", "Error al guardar la biblioteca: Status " + error.status, 2);
        },
        complete: hideLoading
    });
});

$("#verEnMapa").on("click", function () {
    var clase = '';
    if ($("ul.objetos > li.seleccionado").length) {
        clase = '.seleccionado';
    }

    var seleccionados = $("ul.objetos > li" + clase).map(function () { return { objeto: Number($(this).attr("objetoid")), componente: Number($(this).attr("componenteid")) }; }).toArray();

    $.ajax({
        type: "POST",
        url: BASE_URL + "BusquedaAvanzada/VerResultadosEnMapa",
        dataType: 'json',
        data: { seleccionados: JSON.stringify(seleccionados) },
        cache: false,
        success: function (response) {
            if (response.data && response.data.length > 0) {
                var capas = response.data.map(function (e) { return e.capa; });
                var objetos = response.data.map(function (e) { return e.objetos; });
                GeoSIT.MapaController.seleccionarObjetos(objetos, capas);
                $("#myModalBA").modal('hide');
                $(".modal-backdrop").hide();
            }
        },
        error: function (error) {
            alerta("Búsqueda Avanzada", "Error al guardar la biblioteca: Status " + error.status, 2);
        },
        complete: hideLoading
    });


});

$("#guardarColeccion").on("click", function () {
    $("#ModalNombreColeccion").modal("show");
});

$('#btnAceptarNuevaColeccionBA').click(function () {
    showLoading();
    var nombreMT = $('#nombreColeccionBA').val();
    jqxhr = $.ajax({
        type: "POST",
        url: BASE_URL + "BusquedaAvanzada/GuardarColeccion",
        data: { nombre: nombreMT },
        dataType: 'json',
        success: function () {
            alerta("Búsqueda Avanzada", "Se guardó la colección correctamente.", 1);
        },
        error: function (error) {
            alerta("Búsqueda Avanzada", "Error al guardar la colección: Status " + error.status, 2);
        },
        complete: hideLoading
    });

});

$('#exportarExcel').click(function () {
    window.location = BASE_URL + 'BusquedaAvanzada/ExportarAExcel';
    return false;
});

//# sourceURL=visualizacion.js