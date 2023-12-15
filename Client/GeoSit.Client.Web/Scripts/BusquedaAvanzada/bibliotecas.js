$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-bibliotecas-ba').one('shown.bs.modal', function () {
    ajustarmodal();
    hideLoading();
});
function init() {
    $("#accordion-biblioteca-ba").niceScroll(getNiceScrollConfig());
    $('#modal-bibliotecas-ba .panel-body').resize(ajustarScrollBars);
    $('#reportes-content .panel-heading a').click(function () {
        setTimeout(ajustarScrollBars, 10);
    });
    $('.biblioteca-content [data-toggle=tab]').click(function () {
        setTimeout(ajustarScrollBars, 10);
    });
    $("#btnCrearBiblioteca").click(function () {
        $('#modal-bibliotecas-ba').modal('hide');
        $('.modal-backdrop').remove();
        loadView(BASE_URL + "BusquedaAvanzada/Index");
    });

    $(".eliminarbtn").click(function () {
        var li = $(this).closest('li'),
            id = $(li).data('configId'),
            nombre = $('.BibliotecaNombre', li).text().trim();
        var msj = "Está a punto de eliminar la búsqueda avanzada " + nombre + ". " + '<br>' + '¿Desea Continuar?';
        alerta('Búsqueda Avanzada - Eliminar', msj, 2, function () {
            showLoading();
            $.ajax({
                url: BASE_URL + "BusquedaAvanzada/EliminarBA",
                data: { id: id },
                type: 'POST',
                success: function () {
                    alerta('Búsqueda Avanzada - Eliminar', 'La búsqueda avanzada se ha borrado correctamente', 1);
                    $(li).remove();
                },
                error: function (ex) {
                    alerta('Búsqueda Avanzada - Eliminar', 'Ha ocurrido un error al eliminar la búsqueda avanzada seleccionado<br>' + ex.status, 3);
                },
                complete: hideLoading
            });
        });
    });

    $(".exportarbtn").click(function () {
        IS_DOWNLOAD = true;
        window.location = BASE_URL + 'BusquedaAvanzada/ExportarAExcelBiblioteca/' + $(this).closest('li').data('configId');
        return false;
    });
    $(".compartirbtn").click(function () {
        $.ajax({
            url: BASE_URL + "BusquedaAvanzada/CambiarVisibilidad",
            data: { id: $(this).closest('li').data('configId') },
            dataType: 'json',
            type: 'POST',
            success: function (data) {
                var titulo = 'Descompartir', msg = 'descompartido';
                if (data == 1) {
                    titulo = 'Compartir';
                    msg = 'compartido';
                }
                alerta('Búsqueda Avanzada - ' + titulo, 'La búsqueda avanzada se ha ' + msg + ' correctamente', 1, function () {
                    $('#modal-bibliotecas-ba').modal('hide');
                    setTimeout(function () {
                        loadView(BASE_URL + "BusquedaAvanzada/GetBibliotecas");
                    }, 200);
                });
            }, error: function (ex) {
                alerta('Búsqueda Avanzada - Compartir', 'Ha ocurrido un error al compartir la búsqueda avanzada seleccionada<br>' + ex.status, 3);
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
            $('#modal-bibliotecas-ba').modal('hide');
            loadView(BASE_URL + "BusquedaAvanzada/GetResumenView_Guardado?ConfiguracionId=" + idConfig);
        } 
    });
    

    $('#form-Bibliotecas-Resumen').submit(function (evt) {
        /*evita el doble submit*/
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $("#btnSubirBiblioteca").click(function () {
        $("#subirBA").click();
    });

    $("#subirBA").change(SubirBAChange);

    $('#modal-bibliotecas-ba').modal('show');
}

function SubirBAChange() {
    if (document.getElementById("subirBA").files.length) {
        var fileData = getFileData("#subirBA");
        $.ajax({
            type: "POST",
            url: BASE_URL + "BusquedaAvanzada/ImportarAExcel",
            data: fileData.data,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.Existe === 2) {
                    var msj = "Ya existe una config con ese nombre, quisiera importarla de todos modos? " + '<br>' + '¿Desea Continuar?';
                    alerta('Búsqueda Avanzada - Importar Excel', msj, 2, function () {
                        $.ajax({
                            url: BASE_URL + "BusquedaAvanzada/CopyMapaTematicoDeExcel",
                            data: { id: response.Id, componenteId: response.ComponenteId, existe: response.Existe },
                            dataType: 'json',
                            type: 'POST',
                            success: function (data) {
                                setTimeout(function () {
                                    alerta('Búsqueda Avanzada - Importar', 'La Búsqueda avanzada se ha importado correctamente', 1, function () {
                                        $("#modal-bibliotecas-ba").modal('hide');
                                        $('.modal-backdrop').remove();
                                        loadView(BASE_URL + "BusquedaAvanzada/GetBibliotecas");
                                    });
                                }, 200);
                            }, error: function (ex) {
                                setTimeout(function () {
                                    alerta('Búsqueda Avanzada - Eliminar', 'Ha ocurrido un error al importar el archivo.<br>' + error.status, 3);
                                }, 200);
                            }
                        });
                    });
                } else if (response.Existe === 0) {
                    setTimeout(function () {
                        alerta('Búsqueda Avanzada - Importar', 'La búsqueda avanzada que desea importar no existe.', 1);
                    }, 200);
                } else {
                    setTimeout(function () {
                        alerta('Búsqueda Avanzada - Importar', 'La búsqueda avanzada se ha importado correctamente', 1, function () {
                            $("#modal-bibliotecas-ba").modal('hide');
                            $('.modal-backdrop').remove();
                            loadView(BASE_URL + "BusquedaAvanzada/GetBibliotecas");
                        });
                    }, 200);
                }
            },
            error: function (error) {
                alerta('Búsqueda Avanzada - Importar', 'Ha ocurrido un error al importar el archivo.<br>' + error.status, 3);
            },
            complete: function () {
                $("#subirBA").remove();
                $("<input id='subirBA' type='file' style='display:none' />").change(SubirBAChange).appendTo($("#btnSubirBiblioteca").parent());
            }
        });
    }
}
function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    $('#btnAceptarInfoBABiblio').off('click');
    $('#ModalInfoBABiblio').off('hidden.bs.modal');
    $(".modal-footer", "#ModalInfoBABiblio").hide();
    if (tipo !== 2 && fn) {
        $('#ModalInfoBABiblio').one('hidden.bs.modal', fn);
    } else if (fn) {
        $("#btnAceptarInfoBABiblio").one('click', fn);
    }
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            $(".modal-footer", "#ModalInfoBABiblio").show();
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }
    $("#MensajeInfoBA").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoBA").html(titulo);
    $("#DescripcionInfoBA").html(mensaje);
    $("#ModalInfoBABiblio").modal('show');
}
function ajustarmodal() {
    var altura = $(window).height() - 190,
        alturalistado=altura - 70;
    $('.biblioteca-body').css({ "max-height": altura, "overflow": "hidden" });
    $('#accordion-biblioteca-ba').css({ "max-height": alturalistado, "overflow": "hidden" });
    setTimeout(function () { ajustarScrollBars(); }, 100);
}
function ajustarScrollBars() {
    $("#accordion-biblioteca-ba").getNiceScroll().resize();
    $("#accordion-biblioteca-ba").getNiceScroll().show();
}
