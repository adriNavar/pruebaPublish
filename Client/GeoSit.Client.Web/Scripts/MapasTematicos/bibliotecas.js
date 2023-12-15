$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-bibliotecas-mt').one('shown.bs.modal', function () {
    ajustarmodal();
    hideLoading();
});
function init() {
    $("#accordion-biblioteca-ba").niceScroll(getNiceScrollConfig());
    $('#modal-bibliotecas-mt .panel-body').resize(ajustarScrollBars);
    $('#reportes-content .panel-heading a').click(function () {
        setTimeout(ajustarScrollBars, 10);
    });
    $('.biblioteca-content [data-toggle=tab]').click(function () {
        setTimeout(ajustarScrollBars, 10);
    });
    $("#btnCrearBiblioteca").click(function () {
        $('#modal-bibliotecas-mt').modal('hide');
        $('.modal-backdrop').remove();
        loadView(BASE_URL + "MapasTematicos/Index");
    });

    $(".eliminarbtn").click(function () {
        var li = $(this).closest('li'),
            id = $(li).data('configId'),
            nombre = $('.BibliotecaNombre', li).text().trim();
        var msj = "Está a punto de eliminar el mapa temático " + nombre + ". " + '<br>' + '¿Desea Continuar?';
        alerta('Mapas Tematicos - Eliminar', msj, 2, function () {
            showLoading();
            $.ajax({
                url: BASE_URL + "MapasTematicos/EliminarMT",
                data: { id: id },
                type: 'POST',
                success: function () {
                    alerta('Mapas Tematicos - Eliminar', 'El mapa temático se ha borrado correctamente.', 1);
                    $(li).remove();
                },
                error: function (ex) {
                    alerta('Mapas Tematicos  - Eliminar', 'Ha ocurrido un error al eliminar el mapa temático seleccionado.<br>' + ex.status, 3);
                },
                complete: hideLoading
            });
        });
    });

    $(".exportarbtn").click(function () {
        IS_DOWNLOAD = true;
        window.location = BASE_URL + 'MapasTematicos/ExportarAExcelBiblioteca/' + $(this).closest('li').data('configId');
        return false;
    });
    $(".compartirbtn").click(function () {
        $.ajax({
            url: BASE_URL + "MapasTematicos/CambiarVisibilidad",
            data: { id: $(this).closest('li').data('configId') },
            dataType: 'json',
            type: 'POST',
            success: function (data) {
                var titulo = 'Descompartir', msg = 'descompartido';
                if (!!data) {
                    titulo = 'Compartir';
                    msg = 'compartido';
                }
                alerta('Mapas Tematicos - ' + titulo, 'El mapa temático se ha ' + msg + ' correctamente.', 1, function () {
                    $('#modal-bibliotecas-mt').modal('hide');
                    setTimeout(function () {
                        loadView(BASE_URL + "MapasTematicos/GetBibliotecas");
                    }, 200);
                });
            }, error: function (ex) {
                alerta('Mapas Tematicos - Compartir', 'Ha ocurrido un error al compartir el mapa temático seleccionado.<br>' + ex.status, 3);
            }
        });
        return false;
    });

    $("#clearSearch").click(function () {
        $("#filtro-biblioteca").val('');
        $("#filtro-biblioteca").keyup();
    });

    $("#filtro-biblioteca").keyup(function () {
        var texto = $("#filtro-biblioteca").val();
        if (texto) {
            $(".inner-accordion li").hide();
            $(".accordion-section", ".inner-accordion").hide();
            $.each($(".biblioteca-nombre", ".inner-accordion li"), function (i, elem) {
                if ($(elem).html().toLowerCase().indexOf(texto.toLowerCase()) > -1) {
                    $(elem).closest("li").show();
                    $(elem).closest(".accordion-section", ".inner-accordion").show();
                }
            });
        } else {
            $(".accordion-section", ".inner-accordion").show();
            $("li", ".biblioteca-body").show();
        }
        ajustarScrollBars();
    });


    $("li", "ul.bibliotecas").click(function () {
        $("li", "ul.bibliotecas").not(this).removeClass('seleccionado');
        $(this).toggleClass('seleccionado');
        if ($(this).hasClass('seleccionado')) {
            $("#btnCargarBiblioteca").removeClass('boton-deshabilitado');
        } else {
            $("#btnCargarBiblioteca").addClass('boton-deshabilitado');
        } 
    });

    $("#btnCargarBiblioteca").click(function () {
        if ($("li.seleccionado").length) {
            var idConfig = $("li.seleccionado").data('configId');
            $('#ConfiguracionId').val(idConfig);
            $('#modal-bibliotecas-mt').modal('hide');
            loadView(BASE_URL + "MapasTematicos/GetResumenView_Guardado?ConfiguracionId=" + idConfig);
        }
    });

    $('#form-Bibliotecas-Resumen').submit(function (evt) {
        /*evita el doble submit*/
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $("#btnSubirBiblioteca").click(function () {
        $("#subirMT").click();
    });

    $("#subirMT").change(SubirMTChange);

    $('#modal-bibliotecas-mt').modal('show');
}

function SubirMTChange() {
    if (document.getElementById("subirMT").files.length) {
        var fileData = getFileData("#subirMT");
        $.ajax({
            type: "POST",
            url: BASE_URL + "MapasTematicos/ImportarAExcel",
            data: fileData.data,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.Existe === 2) {
                    var msj = 'Ya existe una config con ese nombre, quisiera importarla de todos modos?<br>¿Desea Continuar?';
                    alerta('Mapas Tematicos - Importar Excel', msj, 2, function () {
                        $.ajax({
                            url: BASE_URL + "MapasTematicos/CopyMapaTematicoDeExcel",
                            data: { id: response.Id, componenteId: response.ComponenteId, existe: response.Existe },
                            dataType: 'json',
                            type: 'POST',
                            success: function (data) {
                                setTimeout(function () {
                                    alerta('Mapas Tematicos - Importar', 'El mapa temático se ha importado correctamente.', 1, function () {
                                        $("#modal-bibliotecas-mt").modal('hide');
                                        $('.modal-backdrop').remove();
                                        loadView(BASE_URL + "MapasTematicos/GetBibliotecas");
                                    });
                                }, 200);
                            }, error: function (ex) {
                                setTimeout(function () {
                                    alerta('Mapas Tematicos - Eliminar', 'Ha ocurrido un error al importar el archivo.<br>' + error.status, 3);
                                }, 200);
                            }
                        });
                    });
                } else if (response.Existe === 0) {
                    setTimeout(function () {
                        alerta('Mapas Tematicos - Importar', 'El mapa temático que desea importar no existe.', 1);
                    }, 200);
                } else {
                    setTimeout(function () {
                        alerta('Mapas Tematicos - Importar', 'El mapa temático se ha importado correctamente.', 1, function () {
                            $("#modal-bibliotecas-mt").modal('hide');
                            $('.modal-backdrop').remove();
                            loadView(BASE_URL + "MapasTematicos/GetBibliotecas");
                        });
                    }, 200);
                }
            },
            error: function (error) {
                alerta('Mapas Tematicos - Importar', 'Ha ocurrido un error al importar el archivo.<br>' + error.status, 3);
            },
            complete: function () {
                $("#subirMT").remove();
                $("<input id='subirMT' type='file' style='display:none' />").change(SubirMTChange).appendTo($("#btnSubirBiblioteca").parent());
            }
        });
    }
}
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    $('#btnAceptarInfoMTBiblio').off('click');
    $('#ModalInfoMTBiblio').off('hidden.bs.modal');
    $(".modal-footer", "#ModalInfoMTBiblio").hide();
    if (tipo !== 2 && fn) {
        $('#ModalInfoMTBiblio').one('hidden.bs.modal', fn);
    } else if (fn) {
        $("#btnAceptarInfoMTBiblio").one('click', fn);
    }
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $(".modal-footer", "#ModalInfoMTBiblio").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoMT").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoMT").html(titulo);
    $("#DescripcionInfoMT").html(mensaje);
    $("#ModalInfoMTBiblio").modal('show');
}
function ajustarmodal() {
    var altura = $(window).height() - 190,
        alturalistado = altura - 70;
    $('.biblioteca-body').css({ "max-height": altura, "overflow": "hidden" });
    $('#accordion-biblioteca-mt').css({ "max-height": alturalistado, "overflow": "hidden" });
    setTimeout(function () { ajustarScrollBars(); }, 100);
}
function ajustarScrollBars() {
    $("#accordion-biblioteca-mt").getNiceScroll().resize();
    $("#accordion-biblioteca-mt").getNiceScroll().show();
}