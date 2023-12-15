var clickedRowControl = false, timeoutfiltro;
$(document).ready(init);
$(window).resize(ajustarmodal);

$('#modal-window-coleccion').on('shown.bs.modal', function () {
    ajustarmodal();
    hideLoading();
});
$('#ModalAltaUniraColeccion').on('hidden.bs.modal', function () {
    $('#SeleccionArchivo').hide();
    $('#MensajeColeccionesSeleccionadas').hide();
    $('#MensajeInputColeccion').hide();
    $('#colecciones-seleccionadas').html('');
    $('#MensajeInputColeccion>input').val('');
});

function init() {
    $(".datos-adicionales", ".coleccion-body").addClass('right-scrollable-panel');
    $('.panel-title').removeClass('coleccion-modificada');
    /* evento click boton nueva coleccion*/
    $('#nuevaColeccion').click(function () {
        $('[data-toggle="popover"]').popover('hide');
        $('#tipoAccion').val('Nueva');
        $('#colecciones-seleccionadas').text('Ingrese el nombre de la nueva colección.').show();
        $('#MensajeColeccionesSeleccionadas').show();
        $('#MensajeInputColeccion').show();
        $('#TituloModalAltaUniraColeccion').text('Nueva Colección');
        $('#ModalAltaUniraColeccion').modal('show');
    });

    /* evento click boton unir colecciones*/
    $('#unirColeccion').click(function () {
        $('[data-toggle="popover"]').popover('hide');
        if ($('span[name="check-coleccion"].fa-check-square').length !== 2) {
            alerta("Atención", "Seleccione sólo dos colecciones.", 4);
            return;
        }

        var nombreColecciones = '¿Desea unir las colecciones:\n';
        var coleccionesId = [];
        $('#tipoAccion').val('Union');
        $('span[name="check-coleccion"].fa-check-square > input').each(function (i, e) {
            nombreColecciones = nombreColecciones + "[" + $(e).val() + "] ";
            var t = $(e).parents('.panel-heading').attr("data-id-coleccion-value");
            coleccionesId.push(t);
        });
        nombreColecciones += "?<br/>Ingrese el nombre para la Nueva Colección.";
        $('#colecciones-seleccionadas').html(nombreColecciones).show();
        $('#idColeccion').val(coleccionesId);
        $('#MensajeColeccionesSeleccionadas').show();
        $('#MensajeInputColeccion').show();
        $('#TituloModalAltaUniraColeccion').text('Unir Colección');
        $('#ModalAltaUniraColeccion').modal('show');
    });

    /* evento click boton importar*/
    $('#importarColeccion').click(function (event) {
        $('[data-toggle="popover"]').popover('hide');
        $("#SeleccionArchivo input").val("");
        $('#tipoAccion').val('ImportarArchivo');
        $('#colecciones-seleccionadas').text('Ingrese el nombre de la nueva colección.').show();
        $('#MensajeColeccionesSeleccionadas').show();
        $('#MensajeInputColeccion').show();
        $('#SeleccionArchivo').show();
        $('#TituloModalAltaUniraColeccion').text('Importar Colección');
        $('#ModalAltaUniraColeccion').modal('show');
    });

    $('.btn-file :file').on('change', function () {
        $(this).parents('.input-group').find(':text').val(this.files[0].name);
        $('#MensajeInfoColeccion #nombreColeccion').val(this.files[0].name.replace(/\.[^/.]+$/, ""));
    });

    /* evento click boton aceptar model Nueva o Unir colecciones*/
    $('#btnAceptarNuevaColeccion').click(actionHandler);
    $('#btnCancelarNuevaColeccion').click(function () { $('#nombreColeccion').val(''); });

    $('#nombreColeccion').keyup(function (e) {
        if (e.which === 13) {
            actionHandler();
        }
    });

    $("#clearSearch").click(function (evt) {
        $('[data-toggle="popover"]').popover('hide');
        $("#filtro-coleccion").val('');
        $("#filtro-coleccion").keyup();
        $('#tipoAccion').val('Filtro');
        actionHandler(evt);

    });

    $("#filtro-coleccion").on('input change', function (evt) {
        clearTimeout(timeoutfiltro);
        timeoutfiltro = setTimeout(function () {
            $("#tipoAccion").val("ordenar");
            actionHandler(evt);
        }, 250);
    });

    $('.ordenar').click(function (evt) {
        $("#tipoAccion").val("ordenar");
        actionHandler(evt);
    });

    initColecciones();

    $("#salir").click(function () {
        $('[data-toggle="popover"]').popover('hide');
        $("#modal-window-coleccion").modal('hide');
        return false;
    });

    $('#modal-window-coleccion').modal('show').draggable({ handle: ".modal-header" });
}
function destroyScrollBars() {
    $(".left-scrollable-panel").getNiceScroll().resize().hide();
    $(".right-scrollable-panel").getNiceScroll().resize().hide();
}
function actionHandler(evt) {
    var nombreColeccion = $('#nombreColeccion').val();
    var tipoAccion = $('#tipoAccion').val();
    var isFileDownload = false;
    showLoading();
    var ajaxOptions = {
        type: "POST",
        success: function (result) {
            $('#ModalAltaUniraColeccion').modal('hide');
            if (result.msg) {
                alerta('Información', result.msg, 2);
            } else {
                destroyScrollBars();
                $(".coleccion-content").html(result);
                initColecciones();
                setTimeout(function () { ajustarmodal(); }, 100);
            }
        },
        error: function () {
            $('#ModalAltaUniraColeccion').modal('hide');
            alerta('Error:', 'Ocurrió un problema al guardar los cambios, por favor intente nuevamente.', 3);
        },
        complete: hideLoading
    };
    switch (tipoAccion) {
        case 'Nueva':
            ajaxOptions.data = { nombreColeccion: nombreColeccion };
            ajaxOptions.url = `${BASE_URL}Coleccion/NuevaColeccion`;
            ajaxOptions.error = (err) => {
                let errmsg;
                if (err.status === 400) {
                    errmsg = "No se ha especificado un nombre para la colección.";
                } else if (err.status === 411) {
                    errmsg = "No se ha seleccionado ningún elemento para la colección.";
                } else if (err.status === 401) {
                    errmsg = "No está autorizado a generar una colección.";
                } else if (err.status === 409) {
                    errmsg = "Ya existe una colección con el nombre especificado.";
                }
                alerta('Error:', errmsg, 3);
            };
            break;
        case 'Union':
            const idCols = $('#idColeccion').val().split(',');
            ajaxOptions.data = { coleccionId1: idCols[0], coleccionId2: idCols[1], nombreColeccion: nombreColeccion };
            ajaxOptions.url = `${BASE_URL}Coleccion/UnirColecciones`;
            break;
        case 'Copiar':
            ajaxOptions.data = { coleccionId: $('#idColeccion').val(), nombreColeccion: nombreColeccion };
            ajaxOptions.url = `${BASE_URL}Coleccion/CopiarColeccion`;
            break;
        case 'Renombrar':
            ajaxOptions.data = { coleccionId: $('#idColeccion').val(), nombreColeccion: nombreColeccion };
            ajaxOptions.url = `${BASE_URL}Coleccion/RenombrarColeccion`;
            break;
        case 'Limpiar':
            ajaxOptions.data = { coleccionId: $('#idColeccion').val() };
            ajaxOptions.url = `${BASE_URL}Coleccion/LimpiarColeccion`;
            break;
        case 'Borrar':
            ajaxOptions.data = { coleccionId: $('#idColeccion').val() };
            ajaxOptions.url = `${BASE_URL}Coleccion/BajaColeccion`;
            break;
        case 'BorrarColecciones':
            let idColecciones = [];
            $('span[name="check-coleccion"].fa-check-square > input').each(function (_, e) {
                idColecciones = [...idColecciones, $(e).parents('.panel-heading').attr("data-id-coleccion-value")];
            });
            ajaxOptions.data = { idcolecciones: idColecciones };
            ajaxOptions.url = `${BASE_URL}Coleccion/BajaColecciones`;
            break;
        case 'ordenar':
            if ($('span[name="check-coleccion-all"]').hasClass('fa-check-square')) {
                $('span[name="check-coleccion-all"]').removeClass('fa-check-square').addClass('fa-square');
            }
            ajaxOptions.url = `${BASE_URL}Coleccion/Ordenar`;
            ajaxOptions.data = { orden: $(evt.target).data("ordenTipo"), filtro: $("#filtro-coleccion").val() };
            break;
        case 'BorrarObjeto':
            ajaxOptions.data = { objetoId: $('#idObjeto').val(), componenteId: $('#idComponente').val(), coleccionId: $('#idColeccion').val() };
            ajaxOptions.url = `${BASE_URL}Coleccion/QuitarObjetoColeccion`;
            break;

        case 'BorrarMultiplesObjetos':
            let idColeccion = $('#idColeccion').val();
            let idComponente = $('#idComponente').val();
            let ListaObjetos = $("div#accordion-componentes ul#ul-objetos li >div>div>span[id*='check-objeto-']");
            let idObjeto = [];
            for (var i = 0; i < ListaObjetos.length; i++) {
                if ($(ListaObjetos[i]).hasClass('fa-check-square-o')) {
                    idComponente = $(ListaObjetos).parents('div[id*="collapseComponente_"]').attr('data-id-componente-value');
                    idColeccion = $(ListaObjetos).parents('.cabecera-coleccion').children().attr('data-id-coleccion-value');
                    idObjeto = [...idObjeto, $(ListaObjetos[i]).parents("div#accordion-componentes ul#ul-objetos li[id*='li-objeto-']").attr('data-id-value')];
                }
            }
            ajaxOptions.data = { objetoId: idObjeto, componenteId: idComponente, coleccionId: idColeccion };
            ajaxOptions.url = `${BASE_URL}Coleccion/QuitarMultiplesObjetoColeccion`;
            break;

        case 'AgregarObjeto':
            ajaxOptions.data = { objetoId: $('#idObjeto').val(), componenteId: $('#idComponente').val(), coleccionId: $('#idColeccion').val() };
            ajaxOptions.url = `${BASE_URL}Coleccion/AgregarObjetoColeccion`;
            break;
        case 'AgregarObjetosSeleccionados':
            let objetosSeleccionados = [];
            $.each($(".results .srow .roundChkBox > input:checked"),
                function () {
                    objetosSeleccionados.push({ objetoId: $(this).attr('data-name'), ComponenteDocType: $(this).attr('data-componente') });
                });

            ajaxOptions.data = { coleccionId: $('#idColeccion').val(), objetoComponente: objetosSeleccionados };
            ajaxOptions.url = `${BASE_URL}Coleccion/ColeccionMedianteSeleccionObjetos`;
            break;
        case 'ImportarArchivo':
            ajaxOptions.data = getFileData('.btn-file :file').data;
            ajaxOptions.url = `${BASE_URL}Coleccion/ImportarArchivoColeccion?nombreColeccion=${$('#nombreColeccion').val()}`;
            ajaxOptions.contentType = false;
            ajaxOptions.processData = false;
            ajaxOptions.success = function (result) {
                if (result.success) {
                    alerta('Información', result.msg, 1, function () {
                        showLoading();
                        destroyScrollBars();
                        $(".coleccion-content").load(`${BASE_URL}Coleccion/RefrescarColecciones`, function () {
                            hideLoading();
                            initColecciones();
                            ajustarmodal();
                        });
                    });
                } else {
                    alerta('Información', result.msg, 2);
                }
            };
            break;
        case 'ExportarArchivo':
            isFileDownload = true;
            ajaxOptions.url = `${BASE_URL}Coleccion/ExportarColeccionArchivo`;
            ajaxOptions.data = { coleccionId: $('#idColeccion').val() };
            break;
        case 'ExportarExcel':
            isFileDownload = true;
            ajaxOptions.url = `${BASE_URL}Coleccion/ExportarColeccionExcel`;
            ajaxOptions.data = { coleccionId: $('#idColeccion').val() };
            break;
        default:
            return;
    }
    if (isFileDownload) {
        ajaxOptions.type = 'GET';
        ajaxOptions.success = function () {
            IS_DOWNLOAD = true;
            window.location = `${BASE_URL}Coleccion/DownloadFile`;
        };
        ajaxOptions.error = function () {
            alerta('Error:', 'Ocurrió un problema al exportar el archivo, por favor intente nuevamente.', 3);
        };
    }
    ajaxOptions.complete = hideLoading;
    $.ajax(ajaxOptions);
}

function initColecciones() {
    $('.posicion-objeto').hide();

    $('.left-scrollable-panel .panel-heading').click(function () {
        setTimeout(ajustarScrollBarsColecciones, 10);
    });
    $('.right-scrollable-panel .panel-heading').click(function () {
        setTimeout(ajustarScrollBarsDetalleColeccion, 10);
    });

    $(".hoverRow").on("click", function () {
        clickedRowControl = false;
    });

    $('.opciones-coleccion>span.fa-ellipsis-h').click(function (e) {
        $('#idColeccion').val($(e.target).parents('[data-id-coleccion-value]').attr("data-id-coleccion-value"));
        $('#nombreColeccion').val($(e.target).parents('[data-nombre-coleccion]').attr("data-nombre-coleccion"));
        return false;
    });
    $('.posicion-objeto>span.fa-ellipsis-h').click(() => false);

    $('.opciones-coleccion>span.fa-ellipsis-h').popover({
        animation: false,
        html: true,
        content: () => $('#coleccion-popover-content').html(),
        container: 'body'
    }).on('show.bs.popover', function () {
        $('[data-toggle="popover"]').popover('hide');
    });

    $('.posicion-objeto>span.fa-ellipsis-h').popover({
        animation: false,
        html: true,
        content: () => $('#objeto-popover-content').html(),
        container: 'body'
    }).on('show.bs.popover', function () {
        $('[data-toggle="popover"]').popover('hide');
    });

    $("[data-ver-coleccion]").click(function (event) {
        event.stopPropagation();
        coleccionToMap(event.target)
            .then(function (resultado) {
                featuresToMap(resultado, true);
            })
            .catch(function (error) {
                console.log(error);
            });
    });
    $("[data-seleccionar-coleccion]").click(function (event) {
        event.stopPropagation();
        coleccionToMap(event.target)
            .then(function (resultado) {
                featuresToMap(resultado, false);
            })
            .catch(function (error) {
                console.log(error);
            });
    });

    $('#accordion-colecciones').on('show.bs.collapse', function (eventArgs) {
        showLoading(true);
        hidePreviousCollection(eventArgs.target);
        var coleccionHeader = $(eventArgs.target).prev('[data-id-coleccion-value]');
        if (coleccionHeader.length > 0) {
            var colId = $(eventArgs.target).find('#hiddenColId').val();
            $(eventArgs.target).find("#componenteContent_" + colId).load(BASE_URL + "Coleccion/ComponentesColeccion?coleccionId=" + colId, function () {
                setTimeout(function () {
                    $('#accordion-componentes').on('show.bs.collapse', function (evt) {
                        setTimeout(function () { showAcordionOjetos(evt) }, 10);
                        evt.stopPropagation();
                    });
                    $('#accordion-componentes').on('hide.bs.collapse', function (evt) {
                        setTimeout(function () {
                            ajustarScrollBarsColecciones();
                        }, 100);
                        evt.stopPropagation();
                    });
                    $('.opciones-coleccion>span.fa-chevron-down', coleccionHeader).removeClass('fa-chevron-down').addClass('fa-chevron-up');
                    showAcordionOjetos(eventArgs);
                    addEventsToCollections(eventArgs);
                    validateCheckboxValue(eventArgs.target);
                    hideLoading();
                }, 10);
            });
        }
    });

    $('#accordion-colecciones').on('hide.bs.collapse', function (eventArgs) {
        var coleccionHeader = $(eventArgs.target).prev('[data-id-coleccion-value]');
        if (coleccionHeader.length > 0) {
            $('.opciones-coleccion>span.fa-chevron-up', coleccionHeader).removeClass('fa-chevron-up').addClass('fa-chevron-down');
        }
    });

    $(".left-scrollable-panel").niceScroll(getNiceScrollConfig());
    $(".right-scrollable-panel").niceScroll(getNiceScrollConfig());

    $('span[name="check-coleccion"]').unbind("click").click(function (event) { collectionClick(event, this); });
    /* evento click todas las colecciones */
    $('span[name="check-coleccion-all"]').unbind("click").click(function (event) { collectionClickAll(event, this); });

    checkColeccionTitle();
}

function validateCheckboxValue(crl) {
    var checkbox = $(crl).prev('[data-id-coleccion-value]').find('[name=check-coleccion].fa-check-square');
    if (checkbox) {
        $('span[name="check-componente"]', $(checkbox).parents('.cabecera-coleccion')).each(function (i, e) {
            $(e).removeClass('fa-square').addClass('fa-check-square');
        });
        $('span[id*="check-objeto"]', $(checkbox).parents('.cabecera-coleccion')).each(function (i, e) {
            $(this).removeClass('fa-square-o').addClass('fa-check-square-o');
        });
    }
}

function showAcordionOjetos(eventArgs) {
    var objetos = $(eventArgs.target).find('li[id*="li-objeto-"]:visible');
    if (objetos.length > 0) {
        seleccionarFila(objetos[0]);
    }
    setTimeout(function () {
        ajustarScrollBarsColecciones();
    }, 100);
}

function hidePreviousCollection(target) {
    var previousCollection = $('div[id^="collapseColecciones"]:not([id=' + target.id + '])');
    var coleccionHeader = previousCollection.prev('[data-id-coleccion-value]');
    if (coleccionHeader.length > 0) {
        $('.opciones-coleccion>span.fa-chevron-up', coleccionHeader).removeClass('fa-chevron-up').addClass('fa-chevron-down');
    }
    previousCollection.children().not('input').each(function () {
        this.innerHTML = "";
    });
    previousCollection.collapse('hide');
}

function addEventsToCollections(event) {
    /* evento click mostrar objeto en mapa*/
    $('li[id*="li-objeto-"] span[name="objeto-mapa"]').click(function (event) { showObjetoEnMapa(event); });

    /* evento click seleccionar*/
    $('li[id*="li-objeto-"] span[name="objeto-seleccionar"]').click(function (event) { clickSeleccionar(event); });

    /* evento click de la coleccion */
    $('span[name="check-coleccion"]').unbind("click").click(function (event) { collectionClick(event, this); });

    /* evento click check de los objetos */
    $('span[id*="check-objeto-"]').click(function (event) { checkObjectos(event, this); });

    /* evento click row objetos */
    $('ul[id*="ul-objetos"]').on('click', 'li', function (s, e) { clickRowObjetos(this); });

    /* evento click de los componentes */
    $('span[name="check-componente"]').click(function (event) { clickComponentes(event, this); });
}

/* evento click mostrar objeto en mapa*/
function showObjetoEnMapa(event) {
    /*event.stopPropagation();
    objetoToMap(event.target, featuresToMap, true);*/
    event.stopPropagation();
    objetoToMap(event.target)
        .then(function (resultado) {
            featuresToMap(resultado, true);
        })
        .catch(function (error) {
            console.log(error);
        });
}

/* evento click seleccionar*/
function clickSeleccionar(event) {
    event.stopPropagation();
    objetoToMap(event.target)
        .then(function (resultado) {
            featuresToMap(resultado, false);
        })
        .catch(function (error) {
            console.log(error);
        });
}

/* evento click de la coleccion */
function collectionClick(event, element) {
    event.stopPropagation();
    if ($(element).hasClass('fa-square')) {
        $(element).removeClass('fa-square').addClass('fa-check-square');
        $('span[name="check-componente"]', $(element).parents('.cabecera-coleccion')).each(function (i, e) {
            $(e).removeClass('fa-square').addClass('fa-check-square');
        });
        $('span[id*="check-objeto"]', $(element).parents('.cabecera-coleccion')).each(function (i, e) {
            $(this).removeClass('fa-square-o').addClass('fa-check-square-o');
        });
    } else {
        $(element).removeClass('fa-check-square').addClass('fa-square');
        $('span[name="check-componente"]', $(element).parents('.cabecera-coleccion')).each(function (i, e) {
            $(e).removeClass('fa-check-square').addClass('fa-square');
        });
        $('span[id*="check-objeto"]', $(element).parents('.cabecera-coleccion')).each(function (i, e) {
            $(this).removeClass('fa-check-square-o').addClass('fa-square-o');
        });
    }
}

// evento click todas las colecciones
function seleccionarTodas() {
    var elem = ($('span[name="check-coleccion-all"]'));
    if ($(elem).hasClass('fa-square')) {
        $(elem).removeClass('fa-square').addClass('fa-check-square');
        checks = $('#colecciones-panel').find(".fa-square");
        if ($(checks).hasClass('fa-square')) {
            $(checks).removeClass('fa-square').addClass('fa-check-square');
        }
    } else {
        $(elem).removeClass('fa-check-square').addClass('fa-square');
        $('#colecciones-panel').find(".fa-check-square").removeClass('fa-check-square').addClass('fa-square');
    }
}

/* evento click check de los objetos */
function checkObjectos(event, element) {
    if ($(element).hasClass('fa-square-o')) {
        //check 
        $(element).removeClass('fa-square-o').addClass('fa-check-square-o');
    } else {
        //uncheck y limpio
        $(element).removeClass('fa-check-square-o').addClass('fa-square-o');
    }

    checkComponentes(this);
    checkColeccion($('span[name="check-componente"]', $(this).parents('.cabecera-componente')));
}

/* evento click row objetos */
function clickRowObjetos(element) {
    seleccionarFila(element);
}

/* evento click de los componentes */
function clickComponentes(event, element) {
    event.stopPropagation();
    if ($(element).hasClass('fa-square')) {
        $(element).removeClass('fa-square').addClass('fa-check-square');
        // check todos los hijos del componente
        $('ul>li', $(element).parents('.cabecera-componente')).each(function (i, e) {
            var spanCheck = $('span[id*="check-objeto"]', e);
            spanCheck.removeClass('fa-square-o').addClass('fa-check-square-o');
        });
    } else {
        $(element).removeClass('fa-check-square').addClass('fa-square');
        // uncheck todos los hijos del componente
        $('ul>li', $(element).parents('.cabecera-componente')).each(function (i, e) {
            var spanCheck = $('span[id*="check-objeto"]', e);
            spanCheck.removeClass('fa-check-square-o').addClass('fa-square-o');
        });
    }

    checkColeccion(element);
}

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";

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

    $("#MensajeInfoColeccion").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoColeccion").html(titulo);
    $("#DescripcionInfoColeccion").html(mensaje);
    $("#ModalInfoColeccion").modal('show');
    $("#ModalInfoColeccion").on('hide.bs.modal', fn);
}

function getMaxHeight() {
    return $(window).height() - 230;
}
function ajustarmodal() {
    var altura = getMaxHeight();
    //$('.coleccion-body').css({ "height": altura, "overflow": "hidden" });
    $('.coleccion-body').css({ "height": altura });
    ajustarScrollBarsColecciones();
    ajustarScrollBarsDetalleColeccion();
}

function agregarObjetoColeccion(sender, event) {
    event.stopPropagation();
    const idColeccion = $('.row').filter(() => $(this).css('background-color') === 'rgb(151, 194, 208)')
        .closest('.cabecera-coleccion')
        .children().attr('data-id-coleccion-value');

    $('#tipoAccion').val('AgregarObjeto');
    $('#idObjeto').val($(sender).attr('data-id-objeto'));
    $('#idComponente').val($(sender).attr('data-id-componente'));
    $('#idColeccion').val(idColeccion);
    $('#colecciones-seleccionadas').text('¿Desea agregar el objeto seleccionado a la colección actual?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Agregar Objeto');
    $('#ModalAltaUniraColeccion').modal('show');
}
function agregarObjetosSeleccionadosColeccion(sender, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');
    var objetosSeleccionados = [];
    $.each($(".results .srow .roundChkBox > input:checked"),
        function () {
            var objeto = {
                objetoId: $(this).attr('data-name'), ComponenteDocType: $(this).attr('data-componente')
            };
            objetosSeleccionados.push(objeto);
        });

    if (!objetosSeleccionados.length) {
        alerta("Información", "No hay objetos seleccionados para agregar a la colección", "2");
        return;
    }

    $('#tipoAccion').val('AgregarObjetosSeleccionados');
    $('#colecciones-seleccionadas').text('¿Desea agregar los objetos seleccionados a la colección?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Agregar Objetos Seleccionados a Colección');
    $('#ModalAltaUniraColeccion').modal('show');
}
function eliminarObjeto(sender, event) {
    event.stopPropagation();

    const idObjeto = $(sender).parents('li[id*="li-objeto-"]').attr('data-id-value');
    const idComponente = $(sender).parents('div[id*="collapseComponente_"]').attr('data-id-componente-value');
    const idColeccion = $(sender).parents('.cabecera-coleccion').children().attr('data-id-coleccion-value');

    $('#tipoAccion').val('BorrarObjeto');
    $('#idObjeto').val(idObjeto);
    $('#idComponente').val(idComponente);
    $('#idColeccion').val(idColeccion);

    $('#colecciones-seleccionadas').text('¿Desea borrar el objeto seleccionado?');
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Borrar Objeto');
    $('#ModalAltaUniraColeccion').modal('show');
}
function borrarColeccion(sender, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');
    const idColeccion = $(sender).parents('[data-id-coleccion-value]').attr('data-id-coleccion-value');

    $('#tipoAccion').val('Borrar');
    $('#idColeccion').val(idColeccion);
    $('#colecciones-seleccionadas').text('¿Desea borrar la colección seleccionada?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Borrar Colección');
    $('#ModalAltaUniraColeccion').modal('show');
}
function borrarColecciones(sender, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');
    if ($('span[name="check-coleccion"].fa-check-square').length < 1) {
        alerta("Atención", "Seleccione una o más colecciones.", 4);
        return;
    }
    let idColecciones = [];
    $('span[name="check-coleccion"].fa-check-square > input').each(function (_, e) {
        idColecciones = [...idColecciones, $(e).parents('.panel-heading').attr("data-id-coleccion-value")];
    });

    $('#tipoAccion').val('BorrarColecciones');
    $('#idColeccion').val(idColecciones);
    $('#colecciones-seleccionadas').text('¿Desea borrar las colecciónes seleccionadas?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Borrar Colección');
    $('#ModalAltaUniraColeccion').modal('show');
}
function eliminarObjetosSeleccionados(_, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');
    const ListaObjetos = $("div#accordion-componentes ul#ul-objetos li >div>div>span[id*='check-objeto-']");
    let idComponente;
    let idColeccion;
    let idObj = [];
    for (var i = 0; i < ListaObjetos.length; i++) {
        if ($(ListaObjetos[i]).hasClass('fa-check-square-o')) {
            idComponente = $(ListaObjetos).parents('div[id*="collapseComponente_"]').attr('data-id-componente-value');
            idColeccion = $(ListaObjetos).parents('.cabecera-coleccion').children().attr('data-id-coleccion-value');
            idObj = [...idObj, $(ListaObjetos[i]).parents("div#accordion-componentes ul#ul-objetos li[id*='li-objeto-']").attr('data-id-value')];
        }
    };
    $('#tipoAccion').val('BorrarMultiplesObjetos');
    $('#idObjeto').val(idObj);
    $('#idComponente').val(idComponente);
    $('#idColeccion').val(idColeccion);

    $('#colecciones-seleccionadas').text('¿Desea borrar los objetos seleccionados?');
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Borrar Objetos');
    $('#ModalAltaUniraColeccion').modal('show');
}
function copiarColeccion(sender, event) {
    event.stopPropagation();

    $('[data-toggle="popover"]').popover('hide');

    $('#tipoAccion').val('Copiar');
    $('#colecciones-seleccionadas').text('¿Desea copiar la colección seleccionada?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#MensajeInputColeccion').show();
    $('#TituloModalAltaUniraColeccion').text('Copiar Colección');
    $('#ModalAltaUniraColeccion').modal('show');
}
function renombrarColeccion(_, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');
    $('#nombreColeccion').focus();
    $('#tipoAccion').val('Renombrar');
    $('#colecciones-seleccionadas').text('¿Desea renombrar la colección seleccionada?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#MensajeInputColeccion').show();
    $('#TituloModalAltaUniraColeccion').text('Renombrar Colección');
    $('#ModalAltaUniraColeccion').modal('show');
}
function exportarColeccion(_, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');

    $('#tipoAccion').val('ExportarArchivo');
    $('#colecciones-seleccionadas').text('¿Desea compartir la colección seleccionada?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Exportar Colección');
    $('#ModalAltaUniraColeccion').modal('show');
}
function exportarExcel(_, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');

    $('#tipoAccion').val('ExportarExcel');
    $('#colecciones-seleccionadas').text('¿Desea exportar la colección seleccionada a Excel?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Exportar Colección a Excel');
    $('#ModalAltaUniraColeccion').modal('show');
}
function limpiarColeccion(_, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');

    $('#tipoAccion').val('Limpiar');
    $('#colecciones-seleccionadas').text('¿Desea limpiar la colección seleccionada?').show();
    $('#MensajeColeccionesSeleccionadas').show();
    $('#TituloModalAltaUniraColeccion').text('Limpiar Colección');
    $('#ModalAltaUniraColeccion').modal('show');
}
function ploteoColeccion(_, event) {
    event.stopPropagation();
    $('[data-toggle="popover"]').popover('hide');
    $("#modal-window-coleccion").modal("hide");
    loadView(`${BASE_URL}Ploteo/PloteoGeneral/${$("#idColeccion").val()}`);
}

function ruteoColeccion() {
    $('[data-toggle="popover"]').popover('hide');
    showLoading();
    $.ajax({
        url: `${BASE_URL}Ruteo/DatosRuteoColeccion?idColeccion=${$("#idColeccion").val()}`,
        success: function (data) {
            $('#modal-window-coleccion').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#contenido').html(data);
        },
        error: function (err) {
            let msg = "Ha ocurrido un error al obtener los datos de ruteo.";
            if (err.status === 417) {
                msg = "Sólo pueden rutearse parcelas o componentes puntuales.";
            }
            alerta("Advertencia", msg, 2);
        },
        complete: hideLoading
    });
}
function inicializarColeccionesContent() {
    var opts = {
        paging: false,
        destroy: true,
        searching: false,
        bInfo: false,
        order: [],
        aaSorting: [[0, 'asc']],
        language: { url: `${BASE_URL}Scripts/dataTables.spanish.txt` }
    };
    $('.grilla-atributos', '#detalle-objeto-coleccion').DataTable(opts);

    $('.grilla-relaciones', '#detalle-objeto-coleccion').DataTable(opts);

    $("[data-tipo-grafico].table", '#detalle-objeto-coleccion').DataTable(opts);
}
function checkColeccion(sender) {
    var checkParent = true;
    $('span[name="check-componente"]', $(sender).parents('.cabecera-coleccion')).each(function (_, e) {
        if ($(e).hasClass('fa-square')) {
            checkParent = false;
            return;
        }
    });

    var parentColeccion = $('span[name="check-coleccion"]', $(sender).parents('.cabecera-coleccion'));
    if (checkParent) {
        parentColeccion.removeClass('fa-square').addClass('fa-check-square');
    } else {
        parentColeccion.removeClass('fa-check-square').addClass('fa-square');
    }
}
function checkComponentes(sender) {
    var checkParent = true;

    // uncheck el componente padre si al menos un objeto esta unchecked
    $('ul>li', $(sender).parents('.cabecera-componente')).each(function (_, e) {
        if ($('span', e).hasClass('fa-square-o')) {
            checkParent = false;
            return;
        }
    });

    var parentComponente = $('span[name="check-componente"]', $(sender).parents('.cabecera-componente'));
    if (checkParent) {
        parentComponente.removeClass('fa-square').addClass('fa-check-square');
    } else {
        parentComponente.removeClass('fa-check-square').addClass('fa-square');
    }
}
function checkColeccionTitle() {
    setTimeout(function () {
        $('.cabecera-coleccion span#titulo-coleccion').each(function (_, e) {
            var span = $(e);
            if (span.width() > 200) {
                var trimText = '';
                var cantidad = span.text().substring(span.text().lastIndexOf("("), span.text().length);
                while (span.width() > 175) {
                    trimText = span.text().substring(0, span.text().length - 1);
                    span.text(trimText);
                }
                span.text(trimText + '...' + cantidad);
            }
        });
    }, 500);
}
function seleccionarFila(sender) {
    //este es el evento de select
    $('.row').css('background-color', 'white');
    $('span[id*="check-objeto"]:not(.light-blue)').addClass('light-blue');
    $('.posicion-objeto').hide();

    if ($('.row', sender).css('background-color') === '#97c2d0') {
        $('.posicion-objeto', sender).hide();
    } else {
        $('.posicion-objeto', sender).show();
        $('.row', sender).css('background-color', '#97c2d0');
        $('span[id*="check-objeto"]', sender).removeClass('light-blue').css('color', 'black');

        showLoading();
        var idObjeto = $(sender).attr("data-id-value");
        var idComponente = $(sender).parents('.panel-collapse').attr("data-id-componente-value");
        $.ajax({
            url: BASE_URL + 'DetalleObjeto/GetDetalleObjeto?objetoId=' + idObjeto + "&componenteId=" + idComponente,
            type: 'POST',
            success: function (result) {
                inicializarColeccionesContent();
                loadDetalleObjeto('#detalle-objeto-coleccion', result, extraFieldsRelacionesColeccion);
                setTimeout(ajustarScrollBarsDetalleColeccion, 100);
                hideLoading();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                hideLoading();
                alerta('Error', errorThrown, 3);
            }
        });
    }
}

function extraFieldsRelacionesColeccion(campos, data) {
    campos.push('<span class="fa fa-plus-circle cursor-pointer" name="agegar-objeto-coleccion" ' +
        'data-id-objeto="' + data.ObjetoId + '" data-id-componente="' + data.ComponenteId + '" data-descripcion-componente="' + data.Descripcion +
        '" onclick="agregarObjetoColeccion(this, event)"></span>');
}

function ajustarScrollBarsColecciones() {
    ajustarScrollBarsSeccion(".left-scrollable-panel");
}
function ajustarScrollBarsDetalleColeccion() {
    ajustarScrollBarsSeccion(".right-scrollable-panel");
}
function ajustarScrollBarsSeccion(seccion) {
    $(seccion).css({ "max-height": `${$(".coleccion-body").height() - 20}px` });
    $(seccion).getNiceScroll().resize();
    $(seccion).getNiceScroll().show();
}

function objetoToMap(objeto) {
    return new Promise(function (resolve, reject) {
        try {
            var seleccionados = [];
            var data = $(objeto).parents('.panel-collapse').data();
            var seleccionado = {
                capa: data.capaComponenteValue,
                componente: data.idComponenteValue,
                objetos: [$(objeto).parents('li[data-id-value]').data().idValue]
            };
            seleccionados.push(seleccionado);

            console.log('terminé mi tarea');
            resolve(seleccionados);
        } catch (ex) {
            reject(ex);
        }
    });
}

function coleccionToMap(coleccion) {
    return new Promise(function (resolve, reject) {
        try {
            var seleccionados = [];
            $("#componenteContent_" + $(coleccion).parents("[data-id-coleccion-value]").attr("data-id-coleccion-value") + " div[data-capa-componente-value]").each(function () {
                var data = $(this).data();
                var seleccionado = {
                    capa: data.capaComponenteValue,
                    componente: data.idComponenteValue,
                    objetos: $("li[data-id-value]", this).map(() => $(this).data().idValue).toArray()
                };

                seleccionados.push(seleccionado);
            });
            console.log('terminé mi tarea');
            resolve(seleccionados);
        } catch (ex) {
            reject(ex);
        }
    });
}

function featuresToMap(seleccion, /*filtradas,*/ hacerZoom) {
    if (seleccion) {
        var capas = [], objetos = [];
        seleccion.filter(function (elem) { return !!elem.capa; }).forEach(function (elem) { capas.push(elem.capa); objetos.push(elem.objetos); });
        GeoSIT.MapaController.seleccionarObjetos(objetos, capas, hacerZoom);
    }
}

$('[data-action="minimizar"]').click(function () {
    if ($(this).hasClass('fa-chevron-up')) {
        $(this).minimizar();
    } else {
        $(this).maximizar();
    }
});

$.fn.minimizar = function () {
    var modal = $(this).parents('.modal');
    $(this).removeClass('fa-chevron-up').addClass('fa-chevron-down');
    modal.find('.modal-body').slideUp();
    $('.modal-backdrop').fadeOut();
};

$.fn.maximizar = function () {
    var modal = $(this).parents('.modal');
    $(this).removeClass('fa-chevron-down').addClass('fa-chevron-up');
    modal.find('.modal-body').slideDown();
    $('.modal-backdrop').fadeIn();
    //columnsAdjust("bandejaTramites");
};

function columnsAdjust(tableId) {

    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        $("#" + tableId).DataTable()
            .columns.adjust();
    }
}
//# sourceURL=colecciones.js