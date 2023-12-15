$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-documento').one('shown.bs.modal', function () {
    ajustarScrollBars();
    hideLoading();
});
$('#modal-window-documento').one('hidden.bs.modal', function () {
    $(document).off("documentoGuardado");
});
function init() {
    $("#form").ajaxForm({
        beforeSubmit: function () {
            showLoading();
            return ValidarDatos();
        },
        success: function (data) {
            hideLoading();
            $(document).trigger({ type: 'documentoGuardado', documento: data });
            $("#modal-window-documento").modal('hide');

        },
        error: function () {
            hideLoading();
            MostrarMensaje(false);
        }
    });
    ///////////////////// Scrollbars ////////////////////////
    $(".documento-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-documento .panel-body').resize(ajustarScrollBars);
    $('.documento-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".documento-content").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////
    $("#btnExplorar").click(function () {
        let checkPermisos = Promise.resolve({ EsEditable: true });
        if ($('#DatosDocumento_nombre_archivo').val()) {
            checkPermisos = getPermisosTipoDocumento($('#DatosDocumento_id_tipo_documento').val());
        }
        checkPermisos
            .then(tipoDocumento => {
                const nuevo = Number($("#DatosDocumento_id_documento").val()) === 0;
                if (tipoDocumento.EsEditable || nuevo) {
                    $('#archivo2').click();
                } else {
                    $('#TituloInfo').html("Atención: Operación no permitida.");
                    $('#DescripcionInfo').html("El tipo de documento no es editable.");
                    $('#ModalInfo').modal('show');
                }
            })
            .catch(err => {
                alert("Error al verificar los permisos para el tipo de documento");
                console.log(err);
            });
    });

    $("#btnEliminar").click(function () {
        getPermisosTipoDocumento($('#DatosDocumento_id_tipo_documento').val())
            .then(tipoDocumento => {
                if (tipoDocumento.EsEliminable && tipoDocumento.EsEditable) {
                    if ($("#imagenVisualizar").attr("src") && !!$("#imagenVisualizar").attr("src").length) {
                        const nombreArchivo = $('#DatosDocumento_nombre_archivo').val();
                        $("#btnGrabarAdvertenciaDoc").off("click").one("click", function () {
                            $("#ModalAdvertenciaDoc").hide();
                            $('#DatosDocumento_nombre_archivo').val("");
                            $('#DatosDocumento_extension_archivo').val("");
                            $("#archivo2").val("");
                            previewFile();

                            $('#TituloReemplazar').html("Advertencia - Reemplazar archivo");
                            $('#DescripcionReemplazar').html(`Reemplazar el archivo ${nombreArchivo}<br>¿Desea Reemplazarlo?`);
                            $('#ModalReemplazar').modal('show');
                        });
                        $('#TituloAdvertenciaDoc').html("Advertencia - Eliminar archivo");
                        $('#DescripcionAdvertenciaDoc').html(`¿Está seguro que desea eliminar el archivo ${nombreArchivo}?`);
                        $('#ModalAdvertenciaDoc').modal('show');
                    }
                } else {
                    $('#TituloInfo').html("Atención: Operación no permitida.")
                    $('#DescripcionInfo').html("Este tipo de documento no permite eliminar o editar el archivo.")
                    $('#ModalInfo').modal('show');
                }
            })
            .catch(err => {
                alert("Error al verificar los permisos para el tipo de documento");
                console.log(err);
            });
    });

    $("#btnGrabarReemplazar").click(function () {
        $("#ModalReemplazar").hide();
        $('#archivo2').click();
    });

    $("#btnVisualizar").click(function () {
        const data = $("#imagenVisualizar").attr("src");
        if (data) {
            var link = document.createElement('a');
            link.download = $('#DatosDocumento_nombre_archivo').val();
            link.href = data;
            link.click();
        } else {
            window.location = `${BASE_URL}Documento/Download/${$('#DatosDocumento_id_documento').val()}`;
        }
    });

    $("#btnGrabarDocumento").click(function () {
        var FechaActual = new Date();
        var FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds();
        if (!$('#DatosDocumento_id_documento').val() || $('#DatosDocumento_id_documento').val() == "0") {
            $('#DatosDocumento_id_usu_alta').val("1");
            $('#DatosDocumento_fecha_alta_1').val(FechaMostrar);
        }
        else {
            $('#DatosDocumento_id_usu_modif').val("1");
            $('#DatosDocumento_fecha_modif').val(FechaMostrar);
        }
        if (ValidarDatos()) {
            var Atributos = "!NewDataSet¡!Datos¡";
            $('.schema-property').each(function () {
                var Propiedad = $(this).find("div[id^='label_']").attr("value");
                var Control = "#" + Propiedad + "Id";
                var Valor = $(Control).val();
                Atributos += "!" + Propiedad + "¡" + Valor + "!/" + Propiedad + "¡";
            });
            Atributos += "!/Datos¡!/NewDataSet¡";

            if (Atributos === "!NewDataSet¡!Datos¡!/Datos¡!/NewDataSet¡")
                Atributos = null;
            $("#DatosDocumento_atributos").val(Atributos);
            $('#form').submit();
        }
    });

    $("#form").submit(function (evt) {
        /*evita el doble submit*/
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $("#btnCancelarDocumento").click(function () {
        $('#TituloAdvertenciaDoc').html("Advertencia - Cancelar Operación");
        $('#DescripcionAdvertenciaDoc').html("Al cancelar la operación se perderán los cambios.<br/>¿Desea Continuar?");
        $('#ModalAdvertenciaDoc').modal('show');
        $("#btnGrabarAdvertenciaDoc").off("click").one("click", function () {
            $("#ModalAdvertenciaDoc").modal('hide');
            $("#modal-window-documento").modal('hide');
        });
    });

    ConfiguraTextArea();

    ajustarmodal();
    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-documento .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-documento").modal('show');

    $("#btnAceptarErrorDoc").click(function () {
        $('#ModalErrorDoc').modal('hide');
    })

    $("#btnCancelarAdvertenciaDoc").click(function () {
        $('#ModalAdvertenciaDoc').modal('hide');
    })
    /*habilitarCampos()*/

    $("#DatosDocumento_id_tipo_documento").change(function (evt) {
        getPermisosTipoDocumento(evt.target.value)
            .then(tipoDocumento => {
                const nuevo = Number($("#DatosDocumento_id_documento").val()) === 0;
                $("#btnEliminar,#btnExplorar").addClass("hidden");
                if (tipoDocumento.EsEliminable) {
                    $("#btnEliminar").removeClass("hidden");
                }
                if (tipoDocumento.EsEditable || nuevo) {
                    $("#btnExplorar").removeClass("hidden");
                }
            });
        GetSchemas($("#DatosDocumento_id_tipo_documento").val(), $("#DatosDocumento_id_documento").val())
    })

    GetSchemas($("#DatosDocumento_id_tipo_documento").val(), $("#DatosDocumento_id_documento").val())//Traer Esquema Modificacion o nuevo en posicion default
};

function getPermisosTipoDocumento(id) {
    return new Promise((resolve, reject) => $.getJSON(`${BASE_URL}Documento/GetTipoDocumentoByIdJson/${id}`).done(resolve).fail(reject));
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        paddingBottom = 190,
        headerFooter = 220,
        altura = viewportHeight - headerFooter;
    altura = altura - (altura > headerFooter + paddingBottom ? paddingBottom : 0); //value corresponding to the modal heading + footer
    $(".documento-body", "#scroll-content-documento").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    $('.documento-content').collapse('show');
    temp = $(".documento-body").height();
    var outerHeight = 20;
    $('#accordion-documento .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-documento .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.documento-content').css({ "max-height": temp + 'px' })
    $('#accordion-documento').css({ "max-height": temp + 1 + 'px' })
    $(".documento-content").getNiceScroll().resize();
    $(".documento-content").getNiceScroll().show();
}
function previewFile(file) {
    const preview = document.getElementById("imagenVisualizar");
    if (file) {
        const reader = new FileReader();
        reader.onloadend = function () {
            preview.src = reader.result;
            $('#DatosDocumento_contenido').val(reader.result);
            $("#btnVisualizar").removeClass("hidden");
        }
        reader.readAsDataURL(file);
    } else {
        $("#btnVisualizar").addClass("hidden");
        $("#btnEliminar").addClass("hidden");
        preview.src = "";
    }
}

function ValidarDatos() {
    var mensaje = "";
    if (!$("#DatosDocumento_id_tipo_documento").val())
        mensaje = mensaje.concat("Debe Ingresar el tipo de documento" + '<br> ');
    if ($("#DatosDocumento_id_tipo_documento option:selected").text() !== "Nota") {
        if (!$("#DatosDocumento_nombre_archivo").val())
            mensaje = mensaje.concat("Debe Ingresar el adjunto del documento" + '<br> ');
    }
    if (!$("#DatosDocumento_descripcion").val())
        mensaje = mensaje.concat("Debe Ingresar el Nombre del documento" + '<br> ');
    if (!$("#DatosDocumento_fecha").val())
        mensaje = mensaje.concat("Debe Ingresar la fecha" + '<br> ');

    if (mensaje) {
        $('#TituloErrorDoc').html("Información - Grabar Documento");
        $('#MensajeErrorDoc').html("Los datos del documento están incompletos: " + '<br> ' + mensaje);
        $("#ModalErrorDoc").modal('show');
        hideLoading();
    }
    return !mensaje;
}
function changeFile(archivo) {
    let TamanioArchivo = Number.MAX_SAFE_INTEGER;
    $.ajax({
        async: false,
        type: 'POST',
        url: BASE_URL + 'Documento/GetTamanioMaxArchivo',
        dataType: 'json',
        success: function (Objeto) {
            TamanioArchivo = Number(Objeto);
        }
    });

    var maxSize = Number(TamanioArchivo);
    file = archivo.files[0];
    if (file.size > TamanioArchivo) {
        $('#TituloInfo').html("Información - Tamaño de Archivo")
        $('#DescripcionInfo').html(`El tamaño del archivo supera el máximo permitido de ${maxSize / Math.pow(1024,2)}MB.`);
        $("#ModalInfo").modal('show');
    }
    else {
        let nombre_archivo = archivo.value.substr(archivo.value.lastIndexOf('\\') + 1);
        const extension = nombre_archivo.substring(nombre_archivo.lastIndexOf(".") + 1);
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'Documento/SearchSameFile?' + nombre_archivo,
            dataType: 'json',
            data: { nombreArchivo: nombre_archivo },
            success: function (data) {
                if (Number(data) === 1) {
                    $('#TituloErrorDoc').html("Error - Archivo")
                    $('#MensajeErrorDoc').html(`El archivo ${$('#DatosDocumento_nombre_archivo').val()} ya existe.`)
                    $('#ModalErrorDoc').modal('show');
                } else {
                    $('#DatosDocumento_extension_archivo').val(extension);

                    if (nombre_archivo.length > 49) {
                        $('#TituloInfo').html("Información - Nombre Archivo")
                        $('#DescripcionInfo').html("El nombre del archivo es muy largo se truncará al largo máximo permitido.");
                        $("#ModalInfo").modal('show');
                        nombre_archivo = nombre_archivo.substr(1, 49);
                    }
                    const nombre_sin_extension = nombre_archivo.substring(0, nombre_archivo.lastIndexOf("."));
                    if (!$('#DatosDocumento_descripcion').val()) {
                        $('#DatosDocumento_descripcion').val(nombre_sin_extension);
                    }
                    $('#DatosDocumento_nombre_archivo').val(nombre_archivo);
                    previewFile(file);
                }
            }
        });
    }
}
function ConfiguraTextArea() {
    const d1 = document.getElementById('DatosDocumento_observaciones');
    d1.innerHTML = $('#hdfObservaciones').val() || "";
}
function MostrarMensaje(ok) {
    hideLoading();
    if (ok) {
        $('#TituloInfo').html("Información - Guardar Documento");
        $('#DescripcionInfo').html("Los datos del documento han sido guardados satisfactoriamente.");
    }
    else {
        $('#TituloInfo').html("Información - Error");
        $('#DescripcionInfo').html("Se ha producido un error al grabar los datos.");
    }
    $("#ModalInfo").modal('show');
}

function GetSchemas(valor, Id) {
    $.get(`${BASE_URL}Documento/_ObjetoDocumento`, { TipoId: valor, Id: Id }, (data) => $("#CustomInputs").html(data));
}