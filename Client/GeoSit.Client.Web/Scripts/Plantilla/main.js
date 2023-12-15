$(document).ready(init);
$(window).resize(ajustarmodal);
$('#plantillaModal').one('shown.bs.modal', function (e) {
    ajustarScrollBars();
    hideLoading();
});
var selectedRowPlantillas = null;
var selectedRowLayers = null;
var selectedRowTextos = null;
var selectedRowEscalas = null;

var modelPlantillas;
var modelLayers;
var modelTextos;
var modelEscalas;

var idPerfil = 0;

function init() {
    ///////////////////// Scrollbars ////////////////////////
    $(".plantilla-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-plantillas .panel-body').resize(ajustarScrollBars);
    $('.plantilla-content .panel-heading').click(function () {
        setTimeout(function () {
            ajustarScrollBars();
            ajustarTableScroll();
        }, 10);
    });
    ////////////////////////////////////////////////////////

    $('[data-toggle="tooltip"]').tooltip();

    $("#search").on("keyup", function () {
        search(this.value);
    });
    $("#search-clear").click(function () {
        $("#search").val("");
        search("");
    });

    $("#save-all").click(function () {
        var form = $("#plantilla-form");
        form.formValidation("revalidateField", "Pdf");
        var mensaje = "";
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();
        
        if (bootstrapValidator.isValid()) {
            //Area de Impresión
            if ($("#minXValue").val() > $("#maxXValue").val()) {
                mensaje += "Área de impresión > El valor de Mín X no puede ser mayor que el de Máx X.\n";

            }
            if ($("#minYValue").val() > $("#maxYValue").val()) {
                mensaje += "Área de impresión > El valor de Mín Y no puede ser mayor que el de Máx Y.\n";

            }
            //Referencias
            if ($("#minX").val() > $("#maxX").val()) {
                mensaje += "Referencias > El valor de Mín X no puede ser mayor que el de Máx X.\n";

            }
            if ($("#minY").val() > $("#maxY").val()) {
                mensaje += "Referencias > El valor de Mín Y no puede ser mayor que el de Máx Y.\n";

            }
            //Fuente
            if ($("#ReferenciaFuenteNombre").val() == null) {
                mensaje +="Fuente > El campo Letra es obligatorio.\n";

            }
            if (mensaje != "") {
                modalConfirm("Error de Validación", mensaje);
                return false
                
            }
            form.submit();
        }
    });

    $("#cancel-all").click(function () {
        $.ajax({
            url: BASE_URL + "Plantilla/CancelAll",
            type: "POST",
            success: function (data) {
                if (data === "Ok") {
                    if (selectedRowPlantillas) {
                        selectedRowPlantillas.removeClass("selected");
                        selectedRowPlantillas = null;
                    }

                    $("#buscar-input-group").show();

                    initPloteo();
                    layerTab(false);
                    textoTab(false);
                    escalaTab(false);

                }
            }
        });
    });

    //Plantillas

    $("#tabla-plantillas tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            plantillaEnableControls(false);

        } else {
            $("tr.selected", "#tabla-plantillas tbody").removeClass("selected");
            $(this).addClass("selected");
            plantillaEnableControls(true);

            selectedRowPlantillas = $(this);
            if (selectedRowPlantillas.children().hasClass("dataTables_empty"))
                selectedRowPlantillas = null;
            else {
                var id = Number(selectedRowPlantillas.find("td:first").html());
                plantillaLoadData(id);
                plantillaEnableControls(true);
            }
        }
    });

    $("#plantilla-insert").click(function () {
        insertClick();
    });

    createDataTablePlantilla();
    plantillaFormContent();

    $("#plantilla-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function (arr, $form, options) {
            showLoading();
        },
        success: function (data) {
            selectedRowPlantillas = null;
            $("#plantilla-form").formValidation("resetForm", false);
            $("#buscar-input-group").show();
            hideLoading();
            plantillasReload();
        },
        error: function () {
            alert("error");
            hideLoading();
        }
    });

    //Layers
    createDataTable("tabla-layers");

    $("#layer-insert").click(function () {
        if (selectedRowLayers) {
            selectedRowLayers.removeClass("selected");
            selectedRowLayers = null;
        }

        $("#plantilla-layer-form")[0].reset();
        enableFormContent("#plantilla-layer-form", true);
        $(".color-preview.current-color", "#plantilla-layer-form").css("background-color", "#000000");
        $("#ComponenteId").select2("val", "");
        $("#layer-atributo").select2("val", "");
        $("#layer-fuente-nombre").select2("val", "");
        $("#layer-relleno-trans").jRange("setValue", 0);

        $("input[name=PuntoRepresentacion][value=0]").prop("checked", true);
        $("input[name=PuntoPredeterminado][value=1]").prop("checked", true);
        $("input[name=PuntoPredeterminado]").enable(false);
        $("#ImagenPunto").fileinput("disable");
        $("#PuntoAlto").enable(false);
        $("#PuntoAncho").enable(false);
        $("#PuntoEscala").enable(false);

        $("#IdLayer").val(getFakeId(modelLayers, "IdLayer"));

        initLayerBehaviour(true);

        $("#plantilla-layer-form").formValidation("resetForm", true);
        var fileInput = $(":file", "#plantilla-layer-form");
        fileInput.fileinput("clear");
        fileInput.val("");

        $("#layer-panel-body").show();
        $("#layers-panel-body").hide();

        $("#layer-cancel").removeClass("hidden");
        $("#layer-submit").removeClass("hidden");

        layerRevalidations();
    });

    $("#tabla-layers tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowLayers = null;
            layerEnableControls(false);
            $("#layer-panel-body").hide();
        } else {
            $("tr.selected", "#tabla-layers tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowLayers = $(this);
            if (selectedRowLayers.children().hasClass("dataTables_empty")) {
                selectedRowLayers = null;
            } else {
                var id = Number(selectedRowLayers.find("td:first").html());

                var imagenPunto;

                $.each(modelLayers, function (index, object) {
                    if (object.IdLayer == id) {
                        $.each(object, function (key, value) {
                            var control = $("[name='" + key + "']", "#plantilla-layer-form");
                            if (control.length > 0) {
                                if (control.is(":radio")) {
                                    control.filter("[value=" + value + "]").prop("checked", true);
                                } else if (control.is(":checkbox")) {
                                    control.prop("checked", value);
                                } else if (control.is(":file")) {
                                    //no hacer nada
                                } else if (control.hasClass("pick-a-color")) {
                                    value = value || "#000000";
                                    control.val(value.split("#").pop());
                                    $(".color-preview.current-color", "#" + key).css("background-color", value);
                                } else if (control.hasClass("select2")) {
                                    control.select2("val", value);
                                } else if (control.hasClass("jslider-single")) {
                                    if (value)
                                        $("#layer-relleno-trans").jRange("setValue", value);
                                } else control.val(value);
                            }
                        });
                        actualizarAtributosLayer(object.ComponenteId, Number(object.EtiquetaIdAtributo | 0));

                        //Bold,Italic,Underline,Strikeout
                        if (object.EtiquetaFuenteEstilo == null || object.EtiquetaFuenteEstilo == "")
                            object.EtiquetaFuenteEstilo = '0,0,0,0';//si no tiene cargado nada da error al seleccionar la capa. ETIQUETA_FUENTE_ESTILO
                        var estilo = object.EtiquetaFuenteEstilo.split(",");
                        $("#layer-negrita").prop("checked", estilo[0] === "1" ? true : false);
                        $("#layer-cursiva").prop("checked", estilo[1] === "1" ? true : false);
                        $("#layer-subrayada").prop("checked", estilo[2] === "1" ? true : false);
                        $("#layer-tachada").prop("checked", estilo[3] === "1" ? true : false);

                        imagenPunto = object.PuntoImagenNombre != null ? object.PuntoImagenNombre : object.ImagenPunto;

                        return false;
                    }
                });

                $.ajax({
                    url: BASE_URL + "Layer/Layer/" + id,
                    type: "GET",
                    cache: false,
                    success: function (data) {
                        var input = $("#ImagenPunto");
                        if (imagenPunto && data) {
                            input.fileinput("refresh", {
                                overwriteInitial: true,
                                initialCaption: imagenPunto,
                                initialPreview: ["<img src='data:image/png;base64," + data + "' class=\"file-preview-image\">"]
                            });
                        } else {
                            $("#ImagenPunto").fileinput("clear");
                        }
                        input.fileinput("disable");
                    }

                });

                if ($("#layer-panel-body").is(":visible") === false) {
                    layerEnableControls(true);
                    $("#layer-panel-body").show();
                }

                enableFormContent("#plantilla-layer-form", false);

                $("#layer-relleno-trans").jRange("disable");
            }
        }
        setTimeout(function () { ajustarScrollBars(); }, 10);
    });

    layerFormContent();

    $("#plantilla-layer-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function (arr, $form, options) {
            modelLayers = modelLayers || [];

            var idLayer = Number($("#IdLayer").val());

            //Bold,Italic,Underline,Strikeout
            var negrita = $("#layer-negrita").is(":checked") ? "1" : "0";
            var cursiva = $("#layer-cursiva").is(":checked") ? "1" : "0";
            var subrayada = $("#layer-subrayada").is(":checked") ? "1" : "0";
            var tachada = $("#layer-tachada").is(":checked") ? "1" : "0";

            var componenteNombre = $("#ComponenteId").select2("data").text;

            var categoriaNombre = "";
            var categoria = $("[name=Categoria]", "#plantilla-layer-form").filter(":checked").val();
            switch (categoria) {
                case "0":
                    categoriaNombre = "Normal";
                    break;
                case "1":
                    categoriaNombre = "Principal";
                    break;
                case "2":
                    categoriaNombre = "Secundaria";
                    break;
            }

            var layer = {
                Brush: null,
                Categoria: null,
                Componente: 0,
                Contorno: null,
                ContornoColor: null,
                ContornoGrosor: 0,
                Etiqueta: null,
                EtiquetaColor: null,
                EtiquetaFuenteEstilo: null,
                EtiquetaFuenteNombre: null,
                EtiquetaFuenteTamanio: 0,
                EtiquetaIdAtributo: 0,
                EtiquetaMantieneOrientacion: null,
                FechaAlta: null,
                FechaBaja: null,
                FechaModificacion: null,
                FillBrush: null,
                FiltroGeografico: null,
                IBytes: null,
                ITipo: null,
                ComponenteId: 0,
                IdLayer: 0,
                IdPlantilla: 0,
                IdUsuarioAlta: 0,
                IdUsuarioBaja: 0,
                IdUsuarioModificacion: 0,
                Nombre: null,
                Orden: 0,
                Pen: null,
                PuntoAlto: 0,
                PuntoAncho: 0,
                PuntoImagen: null,
                PuntoImagenFormat: null,
                PuntoImagenNombre: null,
                PuntoPredeterminado: null,
                PuntoRepresentacion: null,
                Relleno: null,
                RellenoColor: null,
                RellenoTransparencia: 0,
                ImagenPunto: null,
                PuntoAtributoOrientacion: null,
                PuntoEscala: null,
                CapaFiltro: null
            };

            for (var key in layer) {
                if (layer.hasOwnProperty(key)) {
                    var control = $("[name='" + key + "']", "#plantilla-layer-form");
                    if (control.length > 0) {
                        if (control.is(":radio")) {
                            layer[key] = control.filter(":checked").val();
                        } else if (control.is(":checkbox")) {
                            layer[key] = control[0].checked;
                        } else if (control.is(":file")) {
                            var splitted = control.val().split('\\');
                            if (splitted.length > 1)
                                layer[key] = splitted.pop();
                            else layer[key] = $(".file-caption-name", "#plantilla-layer-form").attr("title");
                        } else layer[key] = control.val();
                    }
                }
            }

            layer.EtiquetaFuenteEstilo = negrita + "," + cursiva + "," + subrayada + "," + tachada;

            if (!selectedRowLayers) {
                layer.IdLayer = idLayer;
                modelLayers.push(layer);

                var table = $("#tabla-layers").dataTable().api();

                var node = table.row.add([
                    idLayer,
                    layer.Nombre,
                    componenteNombre,
                    categoriaNombre,
                    layer.Orden
                ]).draw().node();

                $(node).find("td:first").addClass("hide");
            } else {
                for (key in modelLayers) {
                    if (modelLayers.hasOwnProperty(key)) {
                        if (modelLayers[key].IdLayer === idLayer) {
                            modelLayers[key] = layer;
                            break;
                        }
                    }
                }

                selectedRowLayers.find("td").eq(1).html(layer.Nombre);
                selectedRowLayers.find("td").eq(2).html(componenteNombre);
                selectedRowLayers.find("td").eq(3).html(categoriaNombre);
                selectedRowLayers.find("td").eq(4).html(layer.Orden);
            }
        },
        success: function () {
            var firstRow = $("tr", "#tabla-layers tbody").eq(0);
            if (firstRow.children().hasClass("dataTables_empty"))
                firstRow.remove();

            if (selectedRowLayers) {
                selectedRowLayers.removeClass("selected");
                selectedRowLayers = null;
            }

            $("#plantilla-layer-form").formValidation("resetForm", false);

            layerEnableControls(false);
        }
    });

    //Textos
    createDataTable("tabla-textos");

    $("#texto-insert").click(function () {
        if (selectedRowTextos) {
            selectedRowTextos.removeClass("selected");
            selectedRowTextos = null;
        }

        $("#plantilla-texto-form")[0].reset();
        enableFormContent("#plantilla-texto-form", true);
        $(".color-preview.current-color", "#plantilla-texto-form").css("background-color", "#000000");
        $("#AtributoId").select2("val", "");
        $("#FuenteNombre").select2("val", "");

        $("#AtributoId").enable(false);

        $("#plantilla-texto-form").formValidation("resetForm", true);

        $("input[name=Tipo][value=1]").prop("checked", true);
        $("#AtributoId").select2("val", "0");

        $("#IdPlantillaTexto").val(getFakeId(modelTextos, "IdPlantillaTexto"));

        $("#texto-panel-body").show();
        $("#textos-panel-body").hide();

        $("#plantilla-texto-cancel").removeClass("hidden");
        $("#plantilla-texto-submit").removeClass("hidden");

        $("#textos-panel-body").hide();
        accordionTabHide($("#collapse-layers"));
        accordionTabHide($("#collapse-escalas"));
    });

    $("#tabla-textos tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowTextos = null;
            textoEnableControls(false);
            $("#texto-panel-body").hide();
        } else {
            $("tr.selected", "#tabla-textos tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowTextos = $(this);
            if (selectedRowTextos.children().hasClass("dataTables_empty"))
                selectedRowTextos = null;
            else {
                var id = Number(selectedRowTextos.find("td:first").html());

                $.each(modelTextos, function (index, object) {
                    if (object.IdPlantillaTexto === id) {

                        $.each(object, function (key, value) {
                            var control = $("[name='" + key + "']", "#plantilla-texto-form");
                            if (control.length > 0) {
                                if (control.is(":radio")) {
                                    control.filter("[value=" + value + "]").prop("checked", true);
                                } else if (control.is(":checkbox")) {
                                    control.prop("checked", value);
                                } else if (control.hasClass("pick-a-color")) {
                                    control.val(value.split("#").pop());
                                    $(".color-preview.current-color", "#" + key).css("background-color", value);
                                } else if (control.hasClass("select2")) {
                                    control.select2("val", value);
                                } else control.val(value);
                            }
                        });

                        //Bold,Italic,Underline,Strikeout
                        var estilo = object.FuenteEstilo.split(",");
                        $("#texto-negrita").prop("checked", estilo[0] === "1" ? true : false);
                        $("#texto-cursiva").prop("checked", estilo[1] === "1" ? true : false);
                        $("#texto-subrayada").prop("checked", estilo[2] === "1" ? true : false);
                        $("#texto-tachada").prop("checked", estilo[3] === "1" ? true : false);

                        return false;
                    }
                });

                if ($("#texto-panel-body").is(":visible") === false) {
                    textoEnableControls(true);
                    $("#texto-panel-body").show();
                }

                enableFormContent("#plantilla-texto-form", false);
            }
        }
        setTimeout(function () { ajustarScrollBars(); }, 10);
    });

    textoFormContent();

    $("#plantilla-texto-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function (arr, $form, options) {

            modelTextos = modelTextos || [];

            var idPlantillaTexto = Number($("#IdPlantillaTexto").val());

            //Bold,Italic,Underline,Strikeout
            var negrita = $("#texto-negrita").is(":checked") ? "1" : "0";
            var cursiva = $("#texto-cursiva").is(":checked") ? "1" : "0";
            var subrayada = $("#texto-subrayada").is(":checked") ? "1" : "0";
            var tachada = $("#texto-tachada").is(":checked") ? "1" : "0";

            var texto = {
                Atributo: null,
                FechaAlta: null,
                FechaBaja: null,
                FechaModificacion: null,
                FuenteAlineacion: 0,
                FuenteColor: null,
                FuenteEstilo: null,
                FuenteNombre: null,
                FuenteTamanio: 0,
                AtributoId: null,
                IdPlantilla: 0,
                IdPlantillaTexto: 0,
                IdUsuarioAlta: 0,
                IdUsuarioBaja: null,
                IdUsuarioModificacion: 0,
                Origen: null,
                Plantilla: null,
                Tipo: 0,
                X: 0,
                Y: 0
            };

            for (var key in texto) {
                if (texto.hasOwnProperty(key)) {
                    var control = $("[name='" + key + "']", "#plantilla-texto-form");
                    if (control.length > 0) {
                        if (control.is(":radio")) {
                            texto[key] = control.filter(":checked").val();
                        } else if (control.is(":checkbox")) {
                            texto[key] = control[0].checked;
                        } else texto[key] = control.val();
                    }
                }
            }

            texto.FuenteEstilo = negrita + "," + cursiva + "," + subrayada + "," + tachada;

            var textoTipo;

            switch (texto.Tipo) {
                case "1":
                    textoTipo = "Estático";
                    break;
                case "2":
                    textoTipo = "Variable";
                    break;
                case "3":
                    textoTipo = "De datos";
                    break;
                case "4":
                    textoTipo = "Atributos"; //Revisa para que sirve este metodo
                    break;
            }
            if (!selectedRowTextos) {
                texto.IdPlantillaTexto = idPlantillaTexto;
                modelTextos.push(texto);

                var table = $("#tabla-textos").dataTable().api();

                var node = table.row.add([
                    idPlantillaTexto,
                    textoTipo,
                    texto.Origen
                ]).draw().node();

                $(node).find("td:first").addClass("hide");
            } else {
                for (key in modelTextos) {
                    if (modelTextos.hasOwnProperty(key)) {
                        if (modelTextos[key].IdPlantillaTexto === idPlantillaTexto) {
                            modelTextos[key] = texto;
                            break;
                        }
                    }
                }

                selectedRowTextos.find("td").eq(1).html(textoTipo);
                selectedRowTextos.find("td").eq(2).html(texto.Origen);
            }
        },
        success: function () {
            var firstRow = $("tr", "#tabla-textos tbody").eq(0);
            if (firstRow.children().hasClass("dataTables_empty"))
                firstRow.remove();

            if (selectedRowTextos) {
                selectedRowTextos.removeClass("selected");
                selectedRowTextos = null;
            }

            $("#plantilla-texto-form").formValidation("resetForm", false);

            textoEnableControls(false);
        }
    });

    //Escalas
    createDataTable("tabla-escalas");

    $("#escala-insert").click(function () {
        if (selectedRowEscalas) {
            selectedRowEscalas.removeClass("selected");
            selectedRowEscalas = null;
        }

        enableFormContent("#plantilla-escala-form", true);

        $("#plantilla-escala-form").formValidation("resetForm", true);
        var newId = (getFakeId(modelEscalas, "IdPlantillaEscala"));
        $("#IdPlantillaEscala").val(newId);

        $("#escala-panel-body").show();
        $("#escalas-panel-body").hide();

        $("#escala-cancel").removeClass("hidden");
        $("#escala-submit").removeClass("hidden");

        $("#escalas-panel-body").hide();
        accordionTabHide($("#collapse-layers"));
        accordionTabHide($("#collapse-textos"));
    });

    $("#tabla-escalas tbody").on("click", "tr", function () {

        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            selectedRowEscalas = null;
            escalaEnableControls(false);
            $("#escala-panel-body").hide();

        } else {
            $("tr.selected", "#tabla-escalas tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowEscalas = $(this);
            if (selectedRowEscalas.children().hasClass("dataTables_empty"))
                selectedRowEscalas = null;
            else {
                var id = Number(selectedRowEscalas.find("td:first").html());

                $.each(modelEscalas, function (index, object) {
                    if (object.IdPlantillaEscala === id) {

                        $.each(object, function (key, value) {
                            var control = $("[name='" + key + "']", "#plantilla-escala-form");
                            if (control.length > 0)
                                control.val(value);
                        });

                        return false;
                    }
                });

                if ($("#escala-panel-body").is(":visible") === false) {
                    escalaEnableControls(true);
                    $("#escala-panel-body").show();
                }

                enableFormContent("#plantilla-escala-form", false);
            }
        }
        setTimeout(function () { ajustarScrollBars(); }, 10);
    });

    escalaFormContent();

    $("#plantilla-escala-form").ajaxForm({
        resetForm: true,
        beforeSubmit: function (arr, $form, options) {

            modelEscalas = modelEscalas || [];

            var idPlantillaEscala = Number($("#IdPlantillaEscala").val());

            var escala = {
                Escala: 0,
                FechaAlta: null,
                FechaBaja: null,
                FechaModificacion: null,
                IdPlantilla: 0,
                IdPlantillaEscala: 0,
                IdUsuarioAlta: 0,
                IdUsuarioBaja: 0,
                IdUsuarioModificacion: 0,
                Plantilla: null
            };

            for (var key in escala) {
                if (escala.hasOwnProperty(key)) {
                    var control = $("[name='" + key + "']", "#plantilla-escala-form");
                    if (control.length > 0)
                        escala[key] = control.val();
                }
            }

            if (!selectedRowEscalas) {
                escala.IdPlantillaEscala = idPlantillaEscala;
                modelEscalas.push(escala);

                var table = $("#tabla-escalas").dataTable().api();
                var node = table.row.add([
                    idPlantillaEscala,
                    escala.Escala
                ]).draw().node();
                $(node).find("td:first").addClass("hide");
            } else {
                for (key in modelEscalas) {
                    if (modelEscalas.hasOwnProperty(key)) {
                        if (modelEscalas[key].IdPlantillaEscala === idPlantillaEscala) {
                            modelEscalas[key] = escala;
                            break;
                        }
                    }
                }

                selectedRowEscalas.find("td").eq(1).html(escala.Escala);
            }
        },
        success: function () {
            var firstRow = $("tr", "#tabla-escalas tbody").eq(0);
            if (firstRow.children().hasClass("dataTables_empty"))
                firstRow.remove();

            if (selectedRowEscalas) {
                selectedRowEscalas.removeClass("selected");
                selectedRowEscalas = null;
            }

            $("#plantilla-escala-form").formValidation("resetForm", false);

            escalaEnableControls(false);
        }
    });

    //Dialog

    $("#collapseListado").on("shown.bs.collapse", function () {
        ajustarTableScroll("tabla-plantillas");
        columnsAdjust("tabla-plantillas");
    });
    $("#collapsePlantillaLayers").on("shown.bs.collapse", function () {
        ajustarTableScroll("tabla-layers");
        columnsAdjust("tabla-layers");
    });
    $("#collapsePlantillaTextos").on("shown.bs.collapse", function () {
        ajustarTableScroll("tabla-textos");
        columnsAdjust("tabla-textos");
    });
    $("#collapsePlantillaEscalas").on("shown.bs.collapse", function () {
        ajustarTableScroll("tabla-escalas");
        columnsAdjust("tabla-escalas");
    });

    $("#collapseListado").on("hidden.bs.collapse", function () {
        ajustarTableScroll("tabla-plantillas");
    });
    $("#collapsePlantillaLayers").on("hidden.bs.collapse", function () {
        ajustarTableScroll("tabla-layers");
    });
    $("#collapsePlantillaTextos").on("hidden.bs.collapse", function () {
        ajustarTableScroll("tabla-textos");
    });
    $("#collapsePlantillaEscalas").on("hidden.bs.collapse", function () {
        ajustarTableScroll("tabla-escalas");
    });

    $("#confirmModal").bind("hidden.bs.modal", function () {
        $("html").addClass("modal-open");
    });

    buscarUsuarioPerfil();

    ajustarmodal();
    $("#plantillaModal").modal("show");
}

function buscarUsuarioPerfil() {
    $.ajax({
        url: BASE_URL + "Plantilla/GetUsuarioPerfil",
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data) {
                idPerfil = data.IdPerfil;
            }
        },
        error: function (ex) {
            console.log(ex);
        }
    });
}

function publicarClick() {
    if (selectedRowPlantillas) {
        var id = selectedRowPlantillas.find("td").eq(0).html();
        showLoading();
        $.ajax({
            url: BASE_URL + "Plantilla/CambiarVisibilidad",
            data: { id: id },
            dataType: 'json',
            type: 'POST',
            success: function (data) {
                if (data) {
                    var msgPublicar = "La Plantilla se ha compartido correctamente";
                    if (Number(data) === 0) {
                        msgPublicar = "La Plantilla se ha descompartido correctamente";
                    }
                    selectedRowPlantillas = null;
                    plantillasReload();
                }
                hideLoading();
            },
            error: function (ex) {
                //alerta('Plantilla - Compartir', 'Ha ocurrido un error al compartir la plantilla seleccionada<br>' + ex.status, 3);
                hideLoading();
                alert('Ha ocurrido un error al compartir la plantilla seleccionada');
            }
        });
    }
    return false;
}

function exportClick() {
    if (selectedRowPlantillas) {
        var id = selectedRowPlantillas.find("td").eq(0).html();
        var nombre = selectedRowPlantillas.find("td").eq(2).html();
        IS_DOWNLOAD = true;
        window.location = BASE_URL + 'Plantilla/ExportarPlantilla?id=' + id + "&nombre=" + nombre;
    }
}

function copyClick() {
    if (selectedRowPlantillas) {
        var id = selectedRowPlantillas.find("td").eq(0).html();
        var nombre = selectedRowPlantillas.find("td").eq(2).html();

        $("#btnAdvertenciaOKPlantilla").click(function () {
            showLoading();
            if ($("#TipoAdvertenciaPlantilla").val() === "copy-plantilla") {
                $.ajax({
                    url: BASE_URL + "Plantilla/Copy/" + id,
                    type: "POST",
                    success: function (data) {
                        selectedRowPlantillas = null;
                        plantillasReload();
                        hideLoading();
                    },
                    error: function () {
                        hideLoading();
                    }
                });
            }
        });

        modalConfirm("Copiar - Plantilla", "¿Desea copiar la plantilla " + nombre + "?", "copy-plantilla");
    }
}

function editClick() {
    if (selectedRowPlantillas) {
        enableFormContent("#plantilla-form", true);
        $("#Pdf").fileinput("enable");
        insertUpdatePlantilla();
        plantillaRevalidations();
    }
}

function deleteClick() {
    if (selectedRowPlantillas) {
        var id = selectedRowPlantillas.find("td").eq(0).html();
        var nombre = selectedRowPlantillas.find("td").eq(2).html();
        $("#btnAdvertenciaOKPlantilla").bind('click', function () {
            $("#btnAdvertenciaOKPlantilla").unbind('click');
            if ($("#TipoAdvertenciaPlantilla").val() === "delete-plantilla") {
                showLoading();
                $.ajax({
                    cache: false,
                    url: BASE_URL + "Plantilla/Delete/" + id,
                    type: "POST",
                    success: function (data) {
                        selectedRowPlantillas = null;
                        plantillasReload();
                        hideLoading();
                    },
                    error: function () {
                        hideLoading();
                    }
                });
            }
        });
        modalConfirm("Eliminar - Plantilla", "¿Desea eliminar la plantilla " + nombre + "?", "delete-plantilla");
    }
}

function insertClick() {
    $("#IdPlantilla").val("0");

    if (selectedRowPlantillas) {
        selectedRowPlantillas.removeClass("selected");
        selectedRowPlantillas = null;
    }

    $("#plantilla-form")[0].reset();
    enableFormContent("#plantilla-form", true);
    $(".color-preview.current-color", "#plantilla-form").css("background-color", "#000000");
    $("#ReferenciaFuenteNombre").select2("val", "");

    $("#plantilla-form").formValidation("resetForm", true);
    var fileInput = $(":file", "#plantilla-form");
    fileInput.fileinput("enable");
    fileInput.fileinput("clear");
    fileInput.val("");

    selectedRowPlantillas = null;

    layerTab(true);
    textoTab(true);
    escalaTab(true);
    plantillaEnableControls(true);

    insertUpdatePlantilla();

    plantillaRevalidations();
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 185,
        altura = viewportHeight - headerFooter;
    $(".plantilla-body", "#scroll-content-plantillas").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    temp = $(".plantilla-body").height();
    var outerHeight = 20;
    $('#accordion-plantilla .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-plantilla .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.plantilla-content').css({ "max-height": temp + 'px' })
    $('#accordion-plantilla').css({ "max-height": temp + 1 + 'px' })
    $(".plantilla-content").getNiceScroll().resize();
    $(".plantilla-content").getNiceScroll().show();
}

function search(value) {
    $("#tabla-plantillas").DataTable().search(value).draw();
}

function initCtrlsPluggins(formId, fileInputs) {
    //Fix for switches
    var cmtoggle = $(".cmn-toggle", formId);
    $.each(cmtoggle, function (index, item) {
        $("#" + item.id).after("<label for=\"" + item.id + "\"></label>");
    });

    //File input
    if (typeof fileInputs === "undefined") {
        $(".file", formId).fileinput({
            showUpload: false
        });
    } else {
        var fileInput;
        var preview;
        for (var i = 0; i < fileInputs.length; i++) {
            fileInput = fileInputs[i];
            $(fileInput.id).empty();

            if (fileInput.isImg)
                preview = "<img src=\"CommonFiles/" + fileInput.img + "\" class=\"file-preview-image\">";
            else preview = "<object data=\"CommonFiles/" + fileInput.img + "\" type=\"application/pdf\" width=\"160px\" height=\"160px\">";

            $(fileInput.id).fileinput({
                showUpload: false,
                initialCaption: fileInput.img,
                initialPreview: [
                    preview
                ]
            });
        }
    }

    //Images tooltips
    $("[data-toggle=\"popover\"]", formId).popover({
        trigger: "focus",
        placement: "left",
        template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>'
    });

    //Color pickers
    $(".pick-a-color", formId).pickAColor({
        inlineDropdown: true
    });

    //Sliders
    $(".jslider-single", formId).jRange({
        from: 0,
        to: 254,
        step: 1,
        showLabels: true,
        theme: "theme-blue"
    });

    //Autocomplete select
    $(".select2", formId).select2();
}

function plantillaLoadData(id) {
    plantillaEnableControls(true);

    showLoading();
    $.get(BASE_URL + "Plantilla/Plantilla/" + id, function (data) {
        if (data) {
            var object = data.plantilla;
            $("#Pdf").fileinput("clear");
            $.each(object, function (key, value) {
                var control = $("[name='" + key + "']", "#plantilla-form");
                if (control.is(":radio")) {
                    control.filter("[value=" + value + "]").prop("checked", true);
                } else if (control.is(":checkbox")) {
                    control.prop("checked", value);
                } else if (control.hasClass("pick-a-color")) {
                    control.val(value.split("#").pop());
                    $(".color-preview.current-color", "#" + key).css("background-color", value);
                } else if (control.hasClass("select2")) {
                    control.select2("val", value);
                } else control.val(value);
            });

            //Bold,Italic,Underline,Strikeout
            var estilo = object.ReferenciaFuenteEstilo.split(",");
            $("#plantilla-negrita").prop("checked", estilo[0] === "1" ? true : false);
            $("#plantilla-cursiva").prop("checked", estilo[1] === "1" ? true : false);
            $("#plantilla-subrayada").prop("checked", estilo[2] === "1" ? true : false);
            $("#plantilla-tachada").prop("checked", estilo[3] === "1" ? true : false);

            if (data.fondo) {
                $("#Pdf").fileinput("refresh", {
                    showUpload: false,
                    overwriteInitial: true,
                    initialCaption: data.fondo.nombre + ".pdf",
                    initialPreview: ["<object data='data:application/pdf;base64," + data.fondo.content + "' class='file-preview-image' width='160px' height='160px'>"]
                });
            }
        }

        $("#Pdf").fileinput("disable");

        $("#imagen-norte").attr("src", "CommonFiles/norte" + $("#id-norte :selected").val() + ".png");
        $("#id-norte").change(function () {
            $("#imagen-norte").attr("src", "CommonFiles/norte" + $(this).val() + ".png");
        });

        layerTab(false);
        textoTab(false);
        escalaTab(false);
    }).fail(function () {
        alert("error");
    }).always(function () {
        hideLoading();
    });
}

function plantillaValidator() {

    $("#plantilla-form").bootstrapValidator({
        framework: "bootstrap",
        fields: {
            Nombre: {
                validators: {
                    notEmpty: {
                        message: "El nombre es obligatorio"
                    },
                    stringLength: {
                        max: 100,
                        message: 'El Nombre debe tener hasta 100 caracteres'
                    }
                }
            },
            Pdf: {
                validators: {
                    file: {
                        extension: "pdf",
                        type: "application/pdf",
                        message: "Por favor seleccione un PDF"
                    },
                    callback: {
                        message: "El PDF de fondo es obligatorio",
                        callback: function (value, validator, $field) {
                            var title = $(".file-caption-name", "#plantilla-form").attr("title");
                            return title !== "";
                        }
                    }
                }
            },
            ImpresionXMin: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Mín X no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Mín X debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Mín X es obligatorio"
                    }
                }
            },
            ImpresionYMin: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Mín Y no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Mín Y debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Mín Y es obligatorio"
                    }
                }
            },
            ImpresionXMax: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Máx X no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Máx X debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Máx X es obligatorio"
                    }
                }
            },
            ImpresionYMax: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Máx Y no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Máx Y debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Máx Y es obligatorio"
                    }
                }
            },
            DistBuffer: {
                validators: {
                    integer: {
                        message: "El campo Buffer debe ser entero"
                    },
                    notEmpty: {
                        message: "El campo Buffer es obligatorio"
                    }
                }
            },
            ReferenciaXMin: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Mín X no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Mín X debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Mín X es obligatorio"
                    }
                }
            },
            ReferenciaXMax: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Máx X no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Máx X debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Máx X es obligatorio"
                    }
                }
            },
            ReferenciaYMin: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Mín Y no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Mín Y debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Mín Y es obligatorio"
                    }
                }
            },
            ReferenciaYMax: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Máx Y no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Máx Y debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Máx Y es obligatorio"
                    }
                }
            },
            ReferenciaFuenteTamanio: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Tamaño no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Tamaño debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Tamaño es obligatorio"
                    }
                }
            },
            ReferenciaColor: {
                trigger: 'change keyup',
                validators: {
                    notEmpty: {
                        message: "El campo Color es obligatorio"
                    }
                }
            },
            ReferenciaEspaciado: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Espaciado no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Espaciado debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Espaciado es obligatorio"
                    }
                }
            },
            ReferenciaColumnas: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Columnas no debe ser cero",
                        value: 0
                    },
                    integer: {
                        message: "El campo Columnas debe ser entero"
                    },
                    notEmpty: {
                        message: "El campo Columnas es obligatorio"
                    }
                }
            },
            NorteX: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de X no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo X debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo X es obligatorio"
                    }
                }
            },
            NorteY: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Y no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Y debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Y es obligatorio"
                    }
                }
            },
            NorteAlto: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Altp no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Alto debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Alto es obligatorio"
                    }
                }
            },
            NorteAncho: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Ancho no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Ancho debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Ancho es obligatorio"
                    }
                }
            }
        }
    });

}

function insertUpdatePlantilla() {
    $("#headingListado").addClass("panel-deshabilitado");
    $("#headingListado").find("a:first[aria-expanded=true]").click();
    $("#footer-plantillas span[aria-controls='button']").removeClass("boton-deshabilitado");
    $("#footer-plantillas span[aria-controls='button']").addClass("black");

    $("#layer-insert").show();
    $("#layer-edit").show();
    $("#layer-delete").show();

    $("#texto-insert").show();
    $("#texto-edit").show();
    $("#texto-delete").show();

    $("#escala-insert").show();
    $("#escala-edit").show();
    $("#escala-delete").show();
}

function initPloteo() {

    if (selectedRowPlantillas == null) {
        plantillaEnableControls(false);
        //plantillaPrivadasEnableControls(false);
    }

    enableFormContent("#plantilla-form", false);
    columnsAdjust("tabla-plantillas");
}

function plantillaEnableControls(value) {
    if (value) {
        //if (idPerfil == 1) {
        $("#plantilla-export").removeClass("boton-deshabilitado");
        $("#plantilla-edit").removeClass("boton-deshabilitado");
        $("#plantilla-delete").removeClass("boton-deshabilitado");
        $("#plantilla-publicar").removeClass("boton-deshabilitado");
        //}
        $("#plantilla-copy").removeClass("boton-deshabilitado");
        $("div[role='region']").removeClass("panel-deshabilitado");
        $("#headingPlantillaInfo").find("a:first[aria-expanded=false]").click();
    } else {
        $("#headingListado").removeClass("panel-deshabilitado");
        $("#headingListado").find("a:first[aria-expanded=false]").click();
        $("#plantilla-publicar").addClass("boton-deshabilitado");
        $("#plantilla-export").addClass("boton-deshabilitado");
        $("#plantilla-edit").addClass("boton-deshabilitado");
        $("#plantilla-delete").addClass("boton-deshabilitado");
        $("#plantilla-copy").addClass("boton-deshabilitado");
        $("div[role='region']").find("a:first[aria-expanded=true]").click();
        $("div[role='region']").addClass("panel-deshabilitado");
        $("#footer-plantillas span[aria-controls='button']").removeClass("black");
        $("#footer-plantillas span[aria-controls='button']").addClass("boton-deshabilitado");
    }
}

function plantillaFormContent() {
    $("#plantilla-form").load(BASE_URL + "Plantilla/FormContent", function () {
        initCtrlsPluggins("#plantilla-form");

        plantillaValidator();

        enableFormContent("#plantilla-form", false);

        $("#imagen-norte").attr("src", "CommonFiles/norte" + $("#id-norte :selected").val() + ".png");
        $("#id-norte").change(function () {
            $("#imagen-norte").attr("src", "CommonFiles/norte" + $(this).val() + ".png");
        });

        $("#plantilla-insert").removeClass("boton-deshabilitado");
        $("#plantilla-import").removeClass("boton-deshabilitado");
    });

    //Init once only

    $("#plantilla-edit").click(function () {
        editClick();
    });
    $("#plantilla-delete").click(function () {
        deleteClick();
    });
    $("#plantilla-copy").click(function () {
        copyClick();
    });
    $('#plantilla-export').click(function () {
        exportClick();
    });

    $('.btn-file :file').on('change', function () {
        var nombrePlantilla = this.files[0].name.substring(0, this.files[0].name.length - 4),
            fileData = getFileData('.btn-file :file');
        showLoading();
        $.ajax({
            type: "POST",
            data: fileData.data,
            contentType: false,
            processData: false,
            url: BASE_URL + 'Plantilla/ImportarPlantilla?nombrePlantilla=' + nombrePlantilla,
            success: function (result) {
                if (result === "Ok") {
                    plantillasReload();
                } else {
                    hideLoading();
                    alert(result.msg);
                }
            },
            error: function (result) {
                //alerta('Error:', 'Ocurrió un problema al guardar los cambios, por favor intente nuevamente.', 3);
                hideLoading();
            }
        });
    });

    $("#plantilla-publicar").click(function () {
        publicarClick();
    });

}

function plantillaRevalidations() {
    //Revalidations
    $("#Pdf").fileinput().on("change", function () {
        $("#plantilla-form").data("bootstrapValidator")
            .updateStatus("Pdf", "NOT_VALIDATED", null)
            .validateField("Pdf");
    });

    $(".fileinput-remove-button", "#plantilla-form").click(function () {
        $("#plantilla-form").data("bootstrapValidator")
            .updateStatus("Pdf", "INVALID", null)
            .validateField("Pdf");
    });
}

function plantillasReload() {
    var cargas = [];
    cargas.push(new Promise(function (ok) { $("#tabla-plantillas").DataTable().ajax.reload(ok); }));
    cargas.push(new Promise(function (ok) {
        $("#plantilla-partial").load(BASE_URL + "Plantilla/Partial", function () {
            initPloteo();
            ok();
        });
    }));
    Promise.all(cargas)
        .then(function () {
            ajustarTableScroll("tabla-plantillas");
            hideLoading();
        })
        .catch(function () { hideLoading(); alert("error"); });
}

function enableFormContent(formId, enable) {
    var formElements = $(formId + " :input");
    if (enable) {
        $.each(formElements, function (_, element) {
            $(element).enable(true);
        });
    } else {
        $.each(formElements, function (_, element) {
            $(element).enable(false);
        });
    }
}

function accordionTabHide(accordionTab) {
    if (accordionTab.hasClass("in")) {
        accordionTab.collapse("hide");
        //arrow
        accordionTab.addClass("collapsed");
        accordionTab.attr("aria-expanded", "false");
    }
}

function accordionTabShow(accordionTab) {
    accordionTab.collapse("show");
    //arrow
    accordionTab.removeClass("collapsed");
    accordionTab.attr("aria-expanded", "true");
}

function layerTab(insert) {
    $("#plantilla-layer-form").formValidation("resetForm", true);

    $("#layer-insert").hide();
    $("#layer-edit").hide();
    $("#layer-delete").hide();

    var idPlantilla = 0;
    if (!insert && selectedRowPlantillas)
        idPlantilla = selectedRowPlantillas.find("td").eq(0).html();

    destroyDataTable("tabla-layers");

    $("#tabla-layers tbody").load(BASE_URL + "Layer/List/" + idPlantilla, function () {
        createDataTable("tabla-layers");

        $("#layer-edit").addClass("boton-deshabilitado");
        $("#layer-delete").addClass("boton-deshabilitado");

        $("#layers-panel-body").show();
        $("#layer-panel-body").hide();
    });
}

function layerValidator() {
    //Revalidate
    $("#plantilla-layer-form").bootstrapValidator({
        framework: "bootstrap",
        excluded: [":disabled"],
        fields: {
            Nombre: {
                validators: {
                    notEmpty: {
                        message: "El Nombre es obligatorio"
                    },
                    stringLength: {
                        max: 100,
                        message: 'El Nombre debe tener hasta 100 caracteres'
                    }
                }
            },
            Orden: {
                greaterThan: {
                    inclusive: false,
                    message: "El valor de Orden no debe ser cero",
                    value: 0
                },
                validators: {
                    integer: {
                        message: "El campo Orden debe ser entero"
                    },
                    notEmpty: {
                        message: "El campo Orden es obligatorio"
                    }
                }
            },
            ContornoColor: {
                validators: {
                    hex: {
                        message: "El campo Color debe ser hexadecimal"
                    },
                    notEmpty: {
                        message: "El campo Color es obligatorio"
                    }
                }
            },
            ContornoGrosor: {
                greaterThan: {
                    inclusive: false,
                    message: "El valor de Grosor no debe ser cero",
                    value: 0
                },
                validators: {
                    numeric: {
                        message: "El campo Grosor debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Grosor es obligatorio"
                    }
                }
            },
            ImagenPunto: {
                validators: {
                    file: {
                        extension: "png",
                        type: "image/png",
                        message: "Por favor seleccione una imagen PNG"
                    },
                    callback: {
                        message: "El campo Imagen es obligatorio",
                        callback: function (value, validator, $field) {
                            if ($("input[name=PuntoRepresentacion]:checked").val() !== "2") return true;
                            var error = $(".kv-fileinput-error.file-error-message").is(":visible");
                            if (error) return false;
                            var title = $(".file-caption-name", "#plantilla-layer-form").attr("title");
                            if (typeof title == "undefined") return false;
                            return title !== "";
                        }
                    }
                }
            },
            PuntoAlto: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Alto no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Alto debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Alto es obligatorio"
                    }
                }
            },
            PuntoAncho: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Ancho no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Ancho debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Ancho es obligatorio"
                    }
                }
            },
            PuntoEscala: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Escalado no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Escalado debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Escalado es obligatorio"
                    }
                }
            },
            EtiquetaFuenteTamanio: {
                greaterThan: {
                    inclusive: false,
                    message: "El valor de Tamaño no debe ser cero",
                    value: 0
                },
                validators: {
                    numeric: {
                        message: "El campo Tamaño debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Tamaño es obligatorio"
                    }
                }
            },
            RellenoColor: {
                validators: {
                    hex: {
                        message: "El campo Color debe ser hexadecimal"
                    },
                    notEmpty: {
                        message: "El campo Color es obligatorio"
                    }
                }
            },
            EtiquetaColor: {
                validators: {
                    hex: {
                        message: "El campo Color debe ser hexadecimal"
                    },
                    notEmpty: {
                        message: "El campo Color es obligatorio"
                    }
                }
            },
            EtiquetaFuenteNombre: {
                notEmpty: {
                    message: "El campo Letra es obligatorio"
                }
            },
            CapaFiltro: {
                validators: {
                    remote: {
                        live: 'submitted',
                        url: BASE_URL + "Plantilla/ValidarFiltro",
                        type: 'POST',
                        data: function () {
                            var idComponente = $('[name="ComponenteId"]').val();
                            var filtro = $('#CapaFiltro').val();
                            return {
                                idComponente: idComponente,
                                filtro: filtro
                            };
                        },
                        message: 'El campo Valor del Filtro no es válido'
                    }
                }
            }
        }
    });

    $("#layer-etiqueta-color").change(function () {
        $("#plantilla-layer-form").data("bootstrapValidator")
            .updateStatus("EtiquetaColor", "NOT_VALIDATED", null)
            .validateField("EtiquetaColor");
    });

    $("#layer-contorno-color").change(function () {
        $("#plantilla-layer-form").data("bootstrapValidator")
            .updateStatus("ContornoColor", "NOT_VALIDATED", null)
            .validateField("ContornoColor");
    });

    $("#layer-relleno-color").change(function () {
        $("#plantilla-layer-form").data("bootstrapValidator")
            .updateStatus("RellenoColor", "NOT_VALIDATED", null)
            .validateField("RellenoColor");
    });

    $("#layer-submit").click(function () {

        var form = $("#plantilla-layer-form");
        form.formValidation("revalidateField", "ImagenPunto");
        form.formValidation("revalidateField", "CapaFiltro");
        var bootstrapValidator = form.data("bootstrapValidator");
        setTimeout(function () {
            bootstrapValidator.validate();
            if (bootstrapValidator.isValid()) {
                form.submit();
            }
        }, 1000);
    });

    $("#layer-cancel").click(function () {
        if (selectedRowLayers) {
            selectedRowLayers.removeClass("selected");
            selectedRowLayers = null;
        }

        $("#plantilla-layer-form").formValidation("resetForm", true);

        $("#layers-panel-body").show();

        layerEnableControls(false);
    });

    $("input[name=PuntoRepresentacion]", "#plantilla-layer-form").change(function () {
        var form = $("#plantilla-layer-form");
        var option = $(this).val();
        switch (option) {
            case "0":
                form.data("bootstrapValidator")
                    .updateStatus("ImagenPunto", "VALID", null)
                    .validateField("ImagenPunto")
                    .updateStatus("PuntoAlto", "VALID", null)
                    .validateField("PuntoAlto")
                    .updateStatus("PuntoAncho", "VALID", null)
                    .validateField("PuntoAncho")
                    .updateStatus("PuntoEscala", "VALID", null)
                    .validateField("PuntoEscala");
                $("#PuntoAlto").val("");
                $("#PuntoAncho").val("");
                $("#PuntoEscala").val("");
                $("input[name=PuntoPredeterminado]").enable(false);
                $("#ImagenPunto").fileinput("clear");
                $("#ImagenPunto").fileinput("disable");
                $("#PuntoAlto").enable(false);
                $("#PuntoAncho").enable(false);
                $("#PuntoEscala").enable(false);
                break;
            case "1":
                form.data("bootstrapValidator")
                    .updateStatus("ImagenPunto", "VALID", null)
                    .validateField("ImagenPunto");
                $("input[name=PuntoPredeterminado]").enable(true);
                $("#ImagenPunto").fileinput("clear");
                $("#ImagenPunto").fileinput("disable");
                $("#PuntoAlto").enable(true);
                $("#PuntoAncho").enable(true);
                $("#PuntoEscala").enable(true);
                form.data("bootstrapValidator")
                    .updateStatus("PuntoAlto", "NOT_VALIDATED", null)
                    .validateField("PuntoAlto")
                    .updateStatus("PuntoAncho", "NOT_VALIDATED", null)
                    .validateField("PuntoAncho")
                    .updateStatus("PuntoEscala", "NOT_VALIDATED", null)
                    .validateField("PuntoEscala");

                break;
            case "2":
                $("input[name=PuntoPredeterminado]").enable(false);
                $("#ImagenPunto").fileinput("enable");
                $("#PuntoAlto").enable(true);
                $("#PuntoAncho").enable(true);
                $("#PuntoEscala").enable(true);
                form.data("bootstrapValidator")
                    .updateStatus("ImagenPunto", "NOT_VALIDATED", null)
                    .validateField("ImagenPunto")
                    .updateStatus("PuntoAlto", "NOT_VALIDATED", null)
                    .validateField("PuntoAlto")
                    .updateStatus("PuntoAncho", "NOT_VALIDATED", null)
                    .validateField("PuntoAncho")
                    .updateStatus("PuntoEscala", "NOT_VALIDATED", null)
                    .validateField("PuntoEscala");
                break;
        }
    });


    $('#CapaFiltro').off().on('keydown', function (e) { e.stopPropagation() });

    $('#CapaFiltro').parent().find('button').off().click(function () {
        var form = $("#plantilla-layer-form");
        if ($(this).is(':enabled')) {
            form.data("bootstrapValidator")
                .updateStatus("CapaFiltro", "NOT_VALIDATED", null)
                .validateField("CapaFiltro");
        }
    });


    initLayerBehaviour();
}

function initLayerBehaviour(edit) {
    if (edit) {
        if ($("#layer-contorno")[0].checked) {
            $("#layer-contorno-color").enable(true);
            $("#layer-contorno-grosor").enable(true);
        } else {
            $("#layer-contorno-color").enable(false);
            $("#layer-contorno-grosor").enable(false);
        }

        if ($("#layer-relleno")[0].checked) {
            $("#layer-relleno-color").enable(true);
            $("#layer-relleno-trans").enable(true);
            $("#layer-relleno-trans").jRange("enable");
        } else {
            $("#layer-relleno-color").enable(false);
            $("#layer-relleno-trans").enable(false);
        }

        if ($("#layer-etiqueta")[0].checked) {
            $("#layer-atributo").enable(true);
            $("#layer-fuente-nombre").enable(true);
            $("#layer-fuente-tamanio").enable(true);
            $("#layer-etiqueta-color").enable(true);
            $("#layer-negrita").enable(true);
            $("#layer-cursiva").enable(true);
            $("#layer-subrayada").enable(true);
            $("#layer-tachada").enable(true);
            $("#layer-mantiene-orientacion").enable(true);
        } else {
            $("#layer-atributo").enable(false);
            $("#layer-fuente-nombre").enable(false);
            $("#layer-fuente-tamanio").enable(false);
            $("#layer-etiqueta-color").enable(false);
            $("#layer-negrita").enable(false);
            $("#layer-cursiva").enable(false);
            $("#layer-subrayada").enable(false);
            $("#layer-tachada").enable(false);
            $("#layer-mantiene-orientacion").enable(false);
        }

    } else {

        $("#layer-contorno-color").enable(false);
        $("#layer-contorno-grosor").enable(false);

        $("#layer-relleno-color").enable(false);
        $("#layer-relleno-trans").enable(false);

        $("#layer-atributo").enable(false);
        $("#layer-fuente-nombre").enable(false);
        $("#layer-fuente-tamanio").enable(false);
        $("#layer-etiqueta-color").enable(false);
        $("#layer-negrita").enable(false);
        $("#layer-cursiva").enable(false);
        $("#layer-subrayada").enable(false);
        $("#layer-tachada").enable(false);
        $("#layer-mantiene-orientacion").enable(false);
    }

    $("#layer-contorno").change(function () {
        if ($(this)[0].checked) {
            $("#layer-contorno-color").enable(true);
            $("#layer-contorno-grosor").enable(true);
        } else {
            $("#layer-contorno-color").enable(false);
            $("#layer-contorno-grosor").enable(false);

            $("#plantilla-layer-form").data("formValidation").resetField("layer-contorno-color");
        }
    });

    $("#layer-relleno").change(function () {
        if ($(this)[0].checked) {
            $("#layer-relleno-color").enable(true);
            $("#layer-relleno-trans").enable(true);
            $("#layer-relleno-trans").jRange("enable");
        } else {
            $("#layer-relleno-color").enable(false);
            $("#layer-relleno-trans").enable(false);
            $("#layer-relleno-trans").jRange("disable");
        }
    });

    $("#layer-etiqueta").change(function () {
        if ($(this)[0].checked) {
            $("#layer-atributo").enable(true);
            $("#layer-fuente-nombre").enable(true);
            $("#layer-fuente-tamanio").enable(true);
            $("#layer-etiqueta-color").enable(true);
            $("#layer-negrita").enable(true);
            $("#layer-cursiva").enable(true);
            $("#layer-subrayada").enable(true);
            $("#layer-tachada").enable(true);
            $("#layer-mantiene-orientacion").enable(true);
        } else {
            $("#layer-atributo").enable(false);
            $("#layer-fuente-nombre").enable(false);
            $("#layer-fuente-tamanio").enable(false);
            $("#layer-etiqueta-color").enable(false);
            $("#layer-negrita").enable(false);
            $("#layer-cursiva").enable(false);
            $("#layer-subrayada").enable(false);
            $("#layer-tachada").enable(false);
            $("#layer-mantiene-orientacion").enable(false);
        }
    });

    $("#ComponenteId").on("change", function (e) {
        actualizarAtributosLayer(e.val, 0);
    });
}
function actualizarAtributosLayer(componente, atributo) {
    $.get(BASE_URL + "layer/atributos/" + componente, function (atributos) {
        $('#layer-atributo').html("");
        $('#layer-atributo').append($.map(atributos, function (attr) { return $('<option>', { val: attr.id, text: attr.text }); }));
        $("#layer-atributo").select2("val", atributo);
    });
}
function layerEnableControls(value) {
    if (value) {
        $("#layer-edit").removeClass("boton-deshabilitado");
        $("#layer-delete").removeClass("boton-deshabilitado");
    } else {
        $("#layers-panel-body").show();
        $("#layer-panel-body").hide();
        $("#layer-edit").addClass("boton-deshabilitado");
        $("#layer-delete").addClass("boton-deshabilitado");

        columnsAdjust("tabla-layers");

        $("#layer-cancel").addClass("hidden");
        $("#layer-submit").addClass("hidden");
    }
}

function layerFormContent() {
    $("#plantilla-layer-form").load(BASE_URL + "Layer/FormContent", function () {

        initCtrlsPluggins("#plantilla-layer-form");

        layerValidator();

        $("#layer-panel-body").show();
    });

    //Init once only

    $("#layer-edit").click(function () {
        if (selectedRowLayers) {
            enableFormContent("#plantilla-layer-form", true);
            $("#ImagenPunto").fileinput("enable");
            initLayerBehaviour(true);

            if ($("input[name=PuntoRepresentacion]:checked").val() === "0") {
                $("input[name=PuntoPredeterminado]").enable(false);
                $("#ImagenPunto").fileinput("disable");
                $("#PuntoAlto").enable(false);
                $("#PuntoAncho").enable(false);
                $("#PuntoEscala").enable(false);
            } else if ($("input[name=PuntoRepresentacion]:checked").val() === "1") {
                $("input[name=PuntoPredeterminado]").enable(true);
                $("#ImagenPunto").fileinput("disable");
                $("#PuntoAlto").enable(true);
                $("#PuntoAncho").enable(true);
                $("#PuntoEscala").enable(true);
            } else {
                $("input[name=PuntoPredeterminado]").enable(true);
                $("#ImagenPunto").fileinput("enable");
                $("#PuntoAlto").enable(true);
                $("#PuntoAncho").enable(true);
                $("#PuntoEscala").enable(true);
            }

            $("#layers-panel-body").hide();
            $("#layer-cancel").removeClass("hidden");
            $("#layer-submit").removeClass("hidden");

            $("#plantilla-panel-body").hide();
            accordionTabHide($("#collapse-plantilla"));
            accordionTabHide($("#collapse-textos"));
            accordionTabHide($("#collapse-escalas"));

            layerRevalidations();
        }
    });

    $("#layer-delete").click(function () {
        if (selectedRowLayers) {
            var id = selectedRowLayers.find("td").eq(0).html();
            var nombre = selectedRowLayers.find("td").eq(1).html();

            $("#btnAdvertenciaOKPlantilla").click(function () {
                if ($("#TipoAdvertenciaPlantilla").val() === "delete-layer")
                    $.ajax({
                        url: BASE_URL + "Layer/Delete/" + id,
                        type: "POST",
                        success: function (data) {
                            if (data === "Ok") {
                                selectedRowLayers.remove();
                                var tabla = $("#tabla-layers").dataTable().api();
                                tabla.row(".selected").remove().draw();
                                selectedRowLayers = null;
                                $("#layer-panel-body").hide();
                                layerEnableControls(false);
                            }
                        }
                    });
            });

            modalConfirm("Eliminar - Capa", "¿Desea eliminar la capa " + nombre + "?", "delete-layer");
        }
    });
}

function layerRevalidations() {
    //Revalidations
    $("#ImagenPunto").fileinput().on("change", function () {
        $("#plantilla-layer-form").data("bootstrapValidator")
            .updateStatus("ImagenPunto", "NOT_VALIDATED", null)
            .validateField("ImagenPunto");
    });

    $(".fileinput-remove-button", "#plantilla-layer-form").click(function () {
        $("#plantilla-layer-form").data("bootstrapValidator")
            .updateStatus("ImagenPunto", "INVALID", null)
            .validateField("ImagenPunto");
    });
}

function textoTab(insert) {
    $("#plantilla-texto-form").formValidation("resetForm", true);

    $("#texto-insert").hide();
    $("#texto-edit").hide();
    $("#texto-delete").hide();

    var idPlantilla = 0;
    if (!insert && selectedRowPlantillas)
        idPlantilla = selectedRowPlantillas.find("td").eq(0).html();

    destroyDataTable("tabla-textos");

    $("#tabla-textos tbody").load(BASE_URL + "PlantillaTexto/List/" + idPlantilla, function () {
        createDataTable("tabla-textos");

        $("#texto-edit").addClass("boton-deshabilitado");
        $("#texto-delete").addClass("boton-deshabilitado");

        $("#textos-panel-body").show();
        $("#texto-panel-body").hide();
    });
}

function textoValidator() {
    $("#plantilla-texto-form").bootstrapValidator({
        framework: "bootstrap",
        excluded: [":disabled"],
        fields: {
            X: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de X no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo X debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo X es obligatorio"
                    }
                }
            },
            Y: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Y no debe ser cero",
                        value: 0
                    },
                    numeric: {
                        message: "El campo Y debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Y es obligatorio"
                    }
                }
            },
            Origen: {
                validators: {
                    notEmpty: {
                        message: "El campo Origen es obligatorio"
                    }
                }
            },
            AtributoId: {
                validators: {
                    callback: {
                        message: "El campo Atributo es obligatorio",
                        callback: function (value, validator) {
                            return value != null && value !== "0";
                        }
                    }
                }
            },
            FuenteTamanio: {
                greaterThan: {
                    inclusive: false,
                    message: "El valor de Tamaño no debe ser cero",
                    value: 0
                },
                validators: {
                    numeric: {
                        message: "El campo Tamaño debe ser numérico"
                    },
                    notEmpty: {
                        message: "El campo Tamaño es obligatorio"
                    }
                }
            }
        }
    });

    $("#plantilla-texto-submit").click(function () {
        var bootstrapValidator = $("#plantilla-texto-form").data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {
            $("#plantilla-texto-form").submit();
        }
    });

    $("#plantilla-texto-cancel").click(function () {
        if (selectedRowTextos) {
            selectedRowTextos.removeClass("selected");
            selectedRowTextos = null;
        }

        $("#plantilla-texto-form").formValidation("resetForm", true);

        $("#textos-panel-body").show();

        textoEnableControls(false);
    });

    $("input[name=Tipo]", "#plantilla-texto-form").change(function () {
        var form = $("#plantilla-texto-form");
        var option = $(this).val();
        switch (option) {
            case "4":
            case "3":
                form.data("bootstrapValidator")
                    .updateStatus("Origen", "VALID", null)
                    .validateField("Origen");
                $("#Origen").val("");
                $("#Origen").enable(false);
                $("#AtributoId").enable(true);
                break;
            default:
                $("#Origen").enable(true);
                form.data("bootstrapValidator")
                    .updateStatus("Origen", "NOT_VALIDATED", null)
                    .validateField("Origen")
                    .updateStatus("AtributoId", "VALID", null)
                    .validateField("AtributoId");
                $("#AtributoId").select2("val", "0");
                $("#AtributoId").enable(false);
        }
    });

    $("#AtributoId").change(function () {
        $("#plantilla-texto-form").data("bootstrapValidator")
            .updateStatus("AtributoId", "NOT_VALIDATED", null)
            .validateField("AtributoId");
    });
}

function textoEnableControls(value) {
    if (value) {
        $("#texto-edit").removeClass("boton-deshabilitado");
        $("#texto-delete").removeClass("boton-deshabilitado");
    } else {
        $("#textos-panel-body").show();
        $("#texto-panel-body").hide();
        $("#texto-edit").addClass("boton-deshabilitado");
        $("#texto-delete").addClass("boton-deshabilitado");

        columnsAdjust("tabla-textos");

        $("#plantilla-texto-cancel").addClass("hidden");
        $("#plantilla-texto-submit").addClass("hidden");
    }
}

function textoFormContent() {
    $("#plantilla-texto-form").load(BASE_URL + "PlantillaTexto/FormContent", function () {
        initCtrlsPluggins("#plantilla-texto-form");
        textoValidator();
    });

    //Init once only

    $("#texto-edit").click(function () {
        if (selectedRowTextos) {
            enableFormContent("#plantilla-texto-form", true);

            if ($("input[name=Tipo]:checked").val() === "3") {
                $("#Origen").enable(false);
                $("#AtributoId").enable(true);
            } else {
                $("#Origen").enable(true);
                $("#AtributoId").enable(false);
            }

            $("#textos-panel-body").hide();
            $("#plantilla-texto-cancel").removeClass("hidden");
            $("#plantilla-texto-submit").removeClass("hidden");

            $("#plantilla-panel-body").hide();
            accordionTabHide($("#collapse-plantilla"));
            accordionTabHide($("#collapse-layers"));
            accordionTabHide($("#collapse-escalas"));
        }
    });

    $("#texto-delete").click(function () {
        if (selectedRowTextos) {
            var id = selectedRowTextos.find("td").eq(0).html();
            var tipo = selectedRowTextos.find("td").eq(1).html();
            var origen = selectedRowTextos.find("td").eq(2).html();

            $("#btnAdvertenciaOKPlantilla").click(function () {
                if ($("#TipoAdvertenciaPlantilla").val() === "delete-texto")
                    $.ajax({
                        url: BASE_URL + "PlantillaTexto/Delete/" + id,
                        type: "POST",
                        success: function (data) {
                            if (data === "Ok") {
                                selectedRowTextos.remove();
                                var tabla = $("#tabla-textos").dataTable().api();
                                tabla.row(".selected").remove().draw();
                                selectedRowTextos = null;
                                $("#texto-panel-body").hide();
                                textoEnableControls(false);
                            }
                        }
                    });
            });

            modalConfirm("Eliminar - Texto", "¿Desea eliminar el texto con Tipo: " + tipo + ", Origen: " + origen + "?", "delete-texto");
        }
    });
}

function escalaTab(insert) {

    $("#plantilla-escala-form").formValidation("resetForm", true);

    $("#escala-insert").hide();
    $("#escala-edit").hide();
    $("#escala-delete").hide();

    var idPlantilla = 0;
    if (!insert && selectedRowPlantillas)
        idPlantilla = selectedRowPlantillas.find("td").eq(0).html();

    destroyDataTable("tabla-escalas");

    $("#tabla-escalas tbody").load(BASE_URL + "PlantillaEscala/List/" + idPlantilla, function () {
        createDataTable("tabla-escalas");

        $("#escala-edit").addClass("boton-deshabilitado");
        $("#escala-delete").addClass("boton-deshabilitado");

        $("#escalas-panel-body").show();
        $("#escala-panel-body").hide();

    });
}

function escalaEnableControls(value) {
    if (value) {
        $("#escala-edit").removeClass("boton-deshabilitado");
        $("#escala-delete").removeClass("boton-deshabilitado");
    } else {
        $("#escalas-panel-body").show();
        $("#escala-panel-body").hide();
        $("#escala-edit").addClass("boton-deshabilitado");
        $("#escala-delete").addClass("boton-deshabilitado");

        columnsAdjust("tabla-escalas");

        $("#escala-cancel").addClass("hidden");
        $("#escala-submit").addClass("hidden");
    }
}

function escalaValidator() {
    $("#plantilla-escala-form").bootstrapValidator({
        framework: "bootstrap",
        excluded: [":disabled"],
        fields: {
            Escala: {
                validators: {
                    greaterThan: {
                        inclusive: false,
                        message: "El valor de Escala no debe ser cero",
                        value: 0
                    },
                    integer: {
                        message: "El campo Escala debe ser entero"
                    },
                    notEmpty: {
                        message: "El campo Escala es obligatorio"
                    }
                }
            }
        }
    });

    $("#escala-submit").click(function () {
        var bootstrapValidator = $("#plantilla-escala-form").data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {
            $("#plantilla-escala-form").submit();
        }
    });

    $("#escala-cancel").click(function () {
        if (selectedRowEscalas) {
            selectedRowEscalas.removeClass("selected");
            selectedRowEscalas = null;
        }

        $("#plantilla-escala-form").formValidation("resetForm", true);

        $("#escalas-panel-body").show();

        escalaEnableControls(false);
    });
}

function escalaFormContent() {
    $("#plantilla-escala-form").load(BASE_URL + "PlantillaEscala/FormContent", function () {
        enableFormContent("#plantilla-escala-form", false);
        escalaValidator();
    });

    //Init once only

    $("#escala-edit").click(function () {
        if (selectedRowEscalas) {
            enableFormContent("#plantilla-escala-form", true);

            $("#escalas-panel-body").hide();
            $("#escala-cancel").removeClass("hidden");
            $("#escala-submit").removeClass("hidden");

            $("#plantilla-panel-body").hide();
            accordionTabHide($("#collapse-plantilla"));
            accordionTabHide($("#collapse-layers"));
            accordionTabHide($("#collapse-textos"));
        }
    });

    $("#escala-delete").click(function () {
        if (selectedRowEscalas) {
            var id = selectedRowEscalas.find("td").eq(0).html();
            var escala = selectedRowEscalas.find("td").eq(1).html();

            $("#btnAdvertenciaOKPlantilla").click(function () {
                if ($("#TipoAdvertenciaPlantilla").val() === "delete-escala")
                    $.ajax({
                        url: BASE_URL + "PlantillaEscala/Delete/" + id,
                        type: "POST",
                        success: function (data) {
                            if (data === "Ok") {
                                selectedRowEscalas.remove();
                                var tabla = $("#tabla-escalas").dataTable().api();
                                tabla.row(".selected").remove().draw();
                                selectedRowEscalas = null;
                                $("#escala-panel-body").hide();
                                escalaEnableControls(false);
                            }
                        }
                    });
            });

            modalConfirm("Eliminar - Escala", "¿Desea eliminar la escala " + escala + "?", "delete-escala");
        }

        return false;
    });
}

function getFakeId(arr, key) {
    var elem;
    var max = 0;
    var id;
    for (elem in arr) {
        if (arr.hasOwnProperty(elem)) {
            id = Math.abs(arr[elem][key]);
            if (id > max)
                max = id;
        }
    }
    return (max + 1) * -1;
}

function setFocus(formId) {
    $(formId).find("input:first").focus();
}

function modalConfirm(title, message, tipoAdvertencia) {
    $("#TipoAdvertenciaPlantilla").val(tipoAdvertencia);
    $("#TituloAdvertenciaPlantilla").text(title);
    $("#DescripcionAdvertenciaPlantilla").text(message);
    $("#confirmModalPlantilla").modal("show");
}


function initTableScroll(tableId) {
    $('#' + tableId).parent('.dataTables_scrollBody').niceScroll(getNiceScrollConfig());
    setTimeout(function () { columnsAdjust(tableId); }, 100);
}

function ajustarTableScroll(tableId) {
    $('#' + tableId).parent('.dataTables_scrollBody').getNiceScroll().resize();
    $('#' + tableId).parent('.dataTables_scrollBody').getNiceScroll().show();
}

//datatables
function destroyDataTable(tableId) {
    var dataTable = $("#" + tableId).dataTable();
    dataTable.api().destroy();
}

function createDataTable(tableId) {
    $("#" + tableId).DataTable({
        dom: "rt",
        scrollY: "150px",
        scrollCollapse: true,
        paging: false,
        destroy: true,
        language: { url: BASE_URL + "Scripts/dataTables.spanish.txt" },
        fnInitComplete: function () {
            initTableScroll(this.attr("id"));
        }
    }).on('page.dt', function () {
        ajustarTableScroll(tableId);
    });
}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

function createDataTablePlantilla() {
    var tablaPlantillasPublicas = $("#tabla-plantillas").dataTable({
        "scrollY": "148px",
        "scrollCollapse": true,
        "paging": false,
        "searching": true,
        "dom": "rt",
        "aaSorting": [[1, "asc"]],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "ajax": {
            "url": BASE_URL + "Plantilla/List",
            "type": "POST",
        },
        "deferRender": true,
        "columns": [
            { "data": "IdPlantilla", "className": "hide" },
            { "data": "Categoria" },
            { "data": "Nombre" },
            { "data": "Hoja" },
            { "data": "Orientacion", "name": "Orientación" },
            { "data": "Componente" },
            { "data": "Visibilidad" }
        ],
        fnInitComplete: function () {
            initTableScroll(this.attr("id"));
        }
    });
}
//@ sourceURL=plantilla.js