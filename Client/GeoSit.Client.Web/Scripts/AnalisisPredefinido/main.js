$(document).ready(init);
$(window).resize(ajustarmodal);
$('#analisispredefinidoModal').on('shown.bs.modal', function (e) {
    ajustarScrollBars();
    //columnsAdjust("tabla-analisispredefinidos");
    hideLoading();
});
var selectedRowanalisispredefinidos = null;
var selectedRowLayers = null;
var selectedRowTextos = null;
var selectedRowEscalas = null;

var modelanalisispredefinidos;
var modelLayers;
var modelTextos;
var modelEscalas;

function init() {
    ///////////////////// Scrollbars ////////////////////////

    $(".analisispredefinido-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-analisispredefinidos .panel-body').resize(ajustarScrollBars);
    $('.analisispredefinidos-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".analisispredefinidos-content").getNiceScroll().resize();
        }, 10);
    });

    ////////////////////////////////////////////////////////
    //********** Acordion Menu Mutuamente-Excluyente Estudio Comercial
    $('#collapseIdName').on('show.bs.collapse', function () {
        $('#collapseIdName2').collapse('hide');
    });
    //**********Sub-Acordion Menu Mutuamente-Excluyente Estudio Comercial

    //collapseEstudioDeudas
    $('#collapseEstudioDeudas').on('show.bs.collapse', function () {
        $('#collapseEstudioMedidores').collapse('hide');
        $('#collapseCortesServicioComercial').collapse('hide');
        $('#collapseEstudioConsumo').collapse('hide');
    });


    //collapseEstudioMedidores
    $('#collapseEstudioMedidores').on('show.bs.collapse', function () {
        $('#collapseEstudioDeudas').collapse('hide');
        $('#collapseCortesServicioComercial').collapse('hide');
        $('#collapseEstudioConsumo').collapse('hide');
    });

    //collapseCortesServicioComercial
    $('#collapseCortesServicioComercial').on('show.bs.collapse', function () {
        $('#collapseEstudioMedidores').collapse('hide');
        $('#collapseEstudioDeudas').collapse('hide');
        $('#collapseEstudioConsumo').collapse('hide');
    });

    //collapseEstudioConsumo
    $('#collapseEstudioConsumo').on('show.bs.collapse', function () {
        $('#collapseEstudioMedidores').collapse('hide');
        $('#collapseCortesServicioComercial').collapse('hide');
        $('#collapseEstudioDeudas').collapse('hide');
    });

    //********** Acordion Menu Mutuamente-Excluyente Analisis Tecnico
    $('#collapseIdName2').on('show.bs.collapse', function () {
        $('#collapseIdName').collapse('hide');
    });

    //********** Modals **********************************************
    ajustarmodal();
    $("#analisispredefinidoModal").modal("show");
    //******* Resetear Botones
    //$("#btnUltimaCarga").removeClass("hidden");
    //$("#btnUltimaCarga").attr("disabled", "disabled");
    $("#btnCargar").removeClass("hidden");
    $("#btnCargar").attr("disabled", "disabled");

    OcultarUltimaCarga();
}

$("#search").on("keyup", function () {
    search(this.value);
});
$("#search-clear").click(function () {
    $("#search").val("");
    search("");
});

$("#save-all").click(function () {
    var form = $("#analisispredefinido-form");
    form.formValidation("revalidateField", "Pdf");

    var bootstrapValidator = form.data("bootstrapValidator");
    bootstrapValidator.validate();

    if (bootstrapValidator.isValid()) {
        form.submit();
    }
});
$("#cancel-all").click(function () {
    $.ajax({
        url: BASE_URL + "analisispredefinido/CancelAll",
        type: "POST",
        success: function (data) {
            if (data == "Ok") {
                if (selectedRowanalisispredefinidos) {
                    selectedRowanalisispredefinidos.removeClass("selected");
                    selectedRowanalisispredefinidos = null;
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

//analisispredefinidos

$("#tabla-analisispredefinidos tbody").on("click", "tr", function () {
    if ($(this).hasClass("selected")) {
        $(this).removeClass("selected");

        analisispredefinidoEnableControls(false);

    } else {
        $("tr.selected", "#tabla-analisispredefinidos tbody").removeClass("selected");
        $(this).addClass("selected");

        selectedRowanalisispredefinidos = $(this);
        if (selectedRowanalisispredefinidos.children().hasClass("dataTables_empty"))
            selectedRowanalisispredefinidos = null;
        else {
            var id = Number(selectedRowanalisispredefinidos.find("td:first").html());
            analisispredefinidoLoadData(id);

            layerTab(false);
            textoTab(false);
            escalaTab(false);
        }
    }
});

$("#analisispredefinido-insert").click(function () {
    $("#Idanalisispredefinido").val("0");

    if (selectedRowanalisispredefinidos) {
        selectedRowanalisispredefinidos.removeClass("selected");
        selectedRowanalisispredefinidos = null;
    }

    $("#analisispredefinido-form")[0].reset();
    enableFormContent("#analisispredefinido-form", true);
    $(".color-preview.current-color", "#analisispredefinido-form").css("background-color", "#000000");
    $("#ReferenciaFuenteNombre").select2("val", "");

    $("#analisispredefinido-form").formValidation("resetForm", true);
    var fileInput = $(":file", "#analisispredefinido-form");
    fileInput.fileinput("enable");
    fileInput.fileinput("clear");
    fileInput.val("");

    selectedRowanalisispredefinidos = null;

    layerTab(true);
    textoTab(true);
    escalaTab(true);

    insertUpdateanalisispredefinido();

    analisispredefinidoRevalidations();
});

createDataTableanalisispredefinido();
$("#analisispredefinido-partial").load(BASE_URL + "analisispredefinido/Partial");

analisispredefinidoFormContent();

$("#analisispredefinido-form").ajaxForm({
    resetForm: true,
    success: function (data) {
        selectedRowanalisispredefinidos = null;
        $("#analisispredefinido-form").formValidation("resetForm", false);
        $("#buscar-input-group").show();
        analisispredefinidosReload();
    }
});

//Layers
createDataTable("tabla-layers");

$("#layer-insert").click(function () {
    if (selectedRowLayers) {
        selectedRowLayers.removeClass("selected");
        selectedRowLayers = null;
    }

    $("#analisispredefinido-layer-form")[0].reset();
    enableFormContent("#analisispredefinido-layer-form", true);
    $(".color-preview.current-color", "#analisispredefinido-layer-form").css("background-color", "#000000");
    $("#ComponenteId").select2("val", "");
    $("#layer-atributo").select2("val", "");
    $("#layer-fuente-nombre").select2("val", "");
    $("#layer-relleno-trans").jRange("setValue", 0);

    $("input[name=PuntoRepresentacion][value=0]").prop("checked", true);
    $("input[name=PuntoPredeterminado][value=0]").prop("checked", true);
    $("input[name=PuntoPredeterminado]").enable(false);
    $("#ImagenPunto").fileinput("disable");
    $("#PuntoAlto").enable(false);
    $("#PuntoAncho").enable(false);

    $("#IdLayer").val(getFakeId(modelLayers, "IdLayer"));

    initLayerBehaviour(true);

    $("#analisispredefinido-layer-form").formValidation("resetForm", true);
    var fileInput = $(":file", "#analisispredefinido-layer-form");
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
        if (selectedRowLayers.children().hasClass("dataTables_empty"))
            selectedRowLayers = null;
        else {
            var id = Number(selectedRowLayers.find("td:first").html());

            var imagenPunto;

            $.each(modelLayers, function (index, object) {
                if (object.IdLayer == id) {

                    $.each(object, function (key, value) {
                        var control = $("[name='" + key + "']", "#analisispredefinido-layer-form");
                        if (control.length > 0) {
                            if (control.is(":radio")) {
                                control.filter("[value=" + value + "]").prop("checked", true);
                                //$.each(control, function (key, radio) {
                                //    if ($(radio).val() == value) $(radio).attr("checked", "checked");
                                //});
                            } else if (control.is(":checkbox")) {
                                control.prop("checked", value);
                            } else if (control.is(":file")) {
                                //no hacer nada
                            } else if (control.hasClass("pick-a-color")) {
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

                    //Bold,Italic,Underline,Strikeout
                    var estilo = object.EtiquetaFuenteEstilo.split(",");
                    $("#layer-negrita").prop("checked", estilo[0] === "1" ? true : false);
                    $("#layer-cursiva").prop("checked", estilo[1] === "1" ? true : false);
                    $("#layer-subrayada").prop("checked", estilo[2] === "1" ? true : false);
                    $("#layer-tachada").prop("checked", estilo[3] === "1" ? true : false);

                    imagenPunto = object.PuntoImagenNombre != null ? object.PuntoImagenNombre : object.ImagenPunto;

                    return false;
                }
            });

            $.post(BASE_URL + "Layer/Layer/" + id, function () {
                var input = $("#ImagenPunto");
                if (imagenPunto) {
                    input.fileinput("refresh", {
                        initialCaption: imagenPunto,
                        initialPreview: ["<img src=\"CommonFiles/" + imagenPunto + "\" class=\"file-preview-image\">"]
                    });
                } else {
                    $(".file-input .file-preview", "#analisispredefinido-layer-form").remove();
                    $(".file-input .fileinput-remove", "#analisispredefinido-layer-form").remove();
                    $(".file-input .file-caption-name", "#analisispredefinido-layer-form").remove();
                    $(".file-input .file-caption-ellipsis", "#analisispredefinido-layer-form").remove();
                }
                input.fileinput("disable");
            });

            if ($("#layer-panel-body").is(":visible") === false) {
                layerEnableControls(true);
                $("#layer-panel-body").show();
            }

            enableFormContent("#analisispredefinido-layer-form", false);

            $("#layer-relleno-trans").jRange("disable");
        }
    }
    setTimeout(function () { ajustarScrollBars(); }, 10);
});

layerFormContent();

$("#analisispredefinido-layer-form").ajaxForm({
    resetForm: true,
    beforeSubmit: function (arr, $form, options) {
        if (typeof modelLayers == "undefined")
            modelLayers = [];

        var idLayer = Number($("#IdLayer").val());

        //Bold,Italic,Underline,Strikeout
        var negrita = $("#layer-negrita").is(":checked") ? "1" : "0";
        var cursiva = $("#layer-cursiva").is(":checked") ? "1" : "0";
        var subrayada = $("#layer-subrayada").is(":checked") ? "1" : "0";
        var tachada = $("#layer-tachada").is(":checked") ? "1" : "0";

        var componenteNombre = $("#ComponenteId").select2("data").text;

        var categoriaNombre = "";
        var categoria = $("[name=Categoria]", "#analisispredefinido-layer-form").filter(":checked").val();
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
            Idanalisispredefinido: 0,
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
            ImagenPunto: null
        };

        for (var key in layer) {
            if (layer.hasOwnProperty(key)) {
                var control = $("[name='" + key + "']", "#analisispredefinido-layer-form");
                if (control.length > 0) {
                    if (control.is(":radio")) {
                        layer[key] = control.filter(":checked").val();
                    } else if (control.is(":checkbox")) {
                        layer[key] = control[0].checked;
                    } else if (control.is(":file")) {
                        var splitted = control.val().split('\\');
                        if (splitted.length > 1)
                            layer[key] = splitted.pop();
                        else layer[key] = $(".file-caption-name", "#analisispredefinido-layer-form").attr("title");
                    } else layer[key] = control.val();
                }
            }
        }

        layer.EtiquetaFuenteEstilo = negrita + "," + cursiva + "," + subrayada + "," + tachada;

        if (selectedRowLayers == null) {
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
                    if (modelLayers[key].IdLayer == idLayer) {
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
    success: function (data) {
        var firstRow = $("tr", "#tabla-layers tbody").eq(0);
        if (firstRow.children().hasClass("dataTables_empty"))
            firstRow.remove();

        if (selectedRowLayers) {
            selectedRowLayers.removeClass("selected");
            selectedRowLayers = null;
        }

        $("#analisispredefinido-layer-form").formValidation("resetForm", false);

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

    $("#analisispredefinido-texto-form")[0].reset();
    enableFormContent("#analisispredefinido-texto-form", true);
    $(".color-preview.current-color", "#analisispredefinido-texto-form").css("background-color", "#000000");
    $("#AtributoId").select2("val", "");
    $("#FuenteNombre").select2("val", "");

    $("#AtributoId").enable(false);

    $("#analisispredefinido-texto-form").formValidation("resetForm", true);

    $("input[name=Tipo][value=1]").prop("checked", true);
    $("#AtributoId").select2("val", "0");

    $("#IdanalisispredefinidoTexto").val(getFakeId(modelTextos, "IdanalisispredefinidoTexto"));

    $("#texto-panel-body").show();
    $("#textos-panel-body").hide();

    $("#analisispredefinido-texto-cancel").removeClass("hidden");
    $("#analisispredefinido-texto-submit").removeClass("hidden");

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
                if (object.IdanalisispredefinidoTexto == id) {

                    $.each(object, function (key, value) {
                        var control = $("[name='" + key + "']", "#analisispredefinido-texto-form");
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

            enableFormContent("#analisispredefinido-texto-form", false);
        }
    }
    setTimeout(function () { ajustarScrollBars(); }, 10);
});

textoFormContent();

$("#analisispredefinido-texto-form").ajaxForm({
    resetForm: true,
    beforeSubmit: function (arr, $form, options) {

        if (typeof modelTextos == "undefined")
            modelTextos = [];

        var idanalisispredefinidoTexto = Number($("#IdanalisispredefinidoTexto").val());

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
            Idanalisispredefinido: 0,
            IdanalisispredefinidoTexto: 0,
            IdUsuarioAlta: 0,
            IdUsuarioBaja: null,
            IdUsuarioModificacion: 0,
            Origen: null,
            analisispredefinido: null,
            Tipo: 0,
            X: 0,
            Y: 0
        };

        for (var key in texto) {
            if (texto.hasOwnProperty(key)) {
                var control = $("[name='" + key + "']", "#analisispredefinido-texto-form");
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
        }

        if (selectedRowTextos == null) {
            texto.IdanalisispredefinidoTexto = idanalisispredefinidoTexto;
            modelTextos.push(texto);

            var table = $("#tabla-textos").dataTable().api();

            var node = table.row.add([
                idanalisispredefinidoTexto,
                textoTipo,
                texto.Origen
            ]).draw().node();

            $(node).find("td:first").addClass("hide");
        } else {
            for (key in modelTextos) {
                if (modelTextos.hasOwnProperty(key)) {
                    if (modelTextos[key].IdanalisispredefinidoTexto == idanalisispredefinidoTexto) {
                        modelTextos[key] = texto;
                        break;
                    }
                }
            }

            selectedRowTextos.find("td").eq(1).html(textoTipo);
            selectedRowTextos.find("td").eq(2).html(texto.Origen);
        }
    },
    success: function (data) {
        var firstRow = $("tr", "#tabla-textos tbody").eq(0);
        if (firstRow.children().hasClass("dataTables_empty"))
            firstRow.remove();

        if (selectedRowTextos) {
            selectedRowTextos.removeClass("selected");
            selectedRowTextos = null;
        }

        $("#analisispredefinido-texto-form").formValidation("resetForm", false);

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

    enableFormContent("#analisispredefinido-escala-form", true);

    $("#analisispredefinido-escala-form").formValidation("resetForm", true);
    var newId = (getFakeId(modelEscalas, "IdanalisispredefinidoEscala"));
    $("#IdanalisispredefinidoEscala").val(newId);

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
                if (object.IdanalisispredefinidoEscala == id) {

                    $.each(object, function (key, value) {
                        var control = $("[name='" + key + "']", "#analisispredefinido-escala-form");
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

            enableFormContent("#analisispredefinido-escala-form", false);
        }
    }
    setTimeout(function () { ajustarScrollBars(); }, 10);
});

escalaFormContent();

$("#analisispredefinido-escala-form").ajaxForm({
    resetForm: true,
    beforeSubmit: function (arr, $form, options) {

        if (typeof modelEscalas == "undefined")
            modelEscalas = [];

        var idanalisispredefinidoEscala = Number($("#IdanalisispredefinidoEscala").val());

        var escala = {
            Escala: 0,
            FechaAlta: null,
            FechaBaja: null,
            FechaModificacion: null,
            Idanalisispredefinido: 0,
            IdanalisispredefinidoEscala: 0,
            IdUsuarioAlta: 0,
            IdUsuarioBaja: 0,
            IdUsuarioModificacion: 0,
            analisispredefinido: null
        };

        for (var key in escala) {
            if (escala.hasOwnProperty(key)) {
                var control = $("[name='" + key + "']", "#analisispredefinido-escala-form");
                if (control.length > 0)
                    escala[key] = control.val();
            }
        }

        if (selectedRowEscalas == null) {
            escala.IdanalisispredefinidoEscala = idanalisispredefinidoEscala;
            modelEscalas.push(escala);

            var table = $("#tabla-escalas").dataTable().api();
            var node = table.row.add([
                idanalisispredefinidoEscala,
                escala.Escala
            ]).draw().node();
            $(node).find("td:first").addClass("hide");
        } else {
            for (key in modelEscalas) {
                if (modelEscalas.hasOwnProperty(key)) {
                    if (modelEscalas[key].IdanalisispredefinidoEscala == idanalisispredefinidoEscala) {
                        modelEscalas[key] = escala;
                        break;
                    }
                }
            }

            selectedRowEscalas.find("td").eq(1).html(escala.Escala);
        }
    },
    success: function (data) {
        var firstRow = $("tr", "#tabla-escalas tbody").eq(0);
        if (firstRow.children().hasClass("dataTables_empty"))
            firstRow.remove();

        if (selectedRowEscalas) {
            selectedRowEscalas.removeClass("selected");
            selectedRowEscalas = null;
        }

        $("#analisispredefinido-escala-form").formValidation("resetForm", false);

        escalaEnableControls(false);
    }
});

//Dialog

$("#collapseanalisispredefinidoLayers").on("shown.bs.collapse", function () {
    columnsAdjust("tabla-layers");
});

$("#collapseanalisispredefinidoTextos").on("shown.bs.collapse", function () {
    columnsAdjust("tabla-textos");
});

$("#collapseanalisispredefinidoEscalas").on("shown.bs.collapse", function () {
    columnsAdjust("tabla-escalas");
});

$("#confirmModal").bind("hidden.bs.modal", function () {
    $("html").addClass("modal-open");
});

ajustarmodal();
$("#analisispredefinidoModal").modal("show");


function ajustarmodal() {

    var viewportHeight = $(window).height(),
        headerFooter = 185,
        altura = viewportHeight - headerFooter;
    $(".analisispredefinido-body", "#scroll-content-analisispredefinidos").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    temp = $(".analisispredefinido-body").height();
    var outerHeight = 20;
    $('#accordionAnalisisPredefinido .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordionAnalisisPredefinido .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.analisispredefinido-content').css({ "max-height": temp + 'px' })
    $('#accordionAnalisisPredefinido').css({ "max-height": temp + 1 + 'px' })
    $(".analisispredefinido-content").getNiceScroll().resize();
    $(".analisispredefinido-content").getNiceScroll().show();
}

function cargaranalisispredefinido(url, id) {
    //alert(url);
    $("#configuracion-analisispredefinido").load(url, function () {

        $("#idPredefinido").val(id);

        $("#btnCargar").removeAttr("disabled", "disabled");

        initCtrlsPluggins("configuracion-analisispredefinido");
        $("#idTipoDeCarga").val(url);


        //********** Calendario Fechas ***********************************
        CalendarioFechas();


        //************ Remarcar Tipo Estudio Comercial Seleccionado
        RemarcarTipoeEstudioComercialSeleccionado(id);

        //************ Remarcar Tipo Analisis Tecnico Seleccionado
        RemarcarTipoAnalisisTecnicoSeleccionado(id);

        //************ Acordiones - Analisis Predefinido
        acordionFiltrosComerciales();
        acordionCargasAnalisisTecnico(id);
        //*********** Seleccionar Unico Radio Button a la vez
        UnUnicoRadioButtonSeleccionadoALaVez();
        //*********** Marcar Coleccion Elegida
        MarcarColeccionSeleccionada();
        //*********** Default: Primer RadioButton de cada Reporte
        ReportesOpcionPorDefecto();

        if (id == 4 || id == 5) {
            $("#divFechas").css("visibility", "hidden");
        }
    });
}

//grLisColecciones
function MarcarColeccionSeleccionada() {
    eraseCookie('coleccionSeleccionada');

    $('#grLisColecciones li').on('click', function () {

        $("#grLisColecciones li").each(function () {
            $(this).removeClass("selected-option-light-blue");
        });

        $(this).addClass("selected-option-light-blue");
        createCookie('coleccionSeleccionada', $(this).text(), 2);
    });
}

function ReportesOpcionPorDefecto() {
    $('#rbIndiceSuc').prop('checked', true);
    $('#rbTotInvervRepZonLav').prop('checked', true);
    $('#rbRankMallas').prop('checked', true);
    $('#rbTotInterv').prop('checked', true);
}

function RestablecerAGeneradorInicial() {
    //******* Resetear Botones
    //Mostrar deshabilitar Botones
    //$("#btnUltimaCarga").removeClass("hide");
    //$("#btnUltimaCarga").attr("disabled", "disabled");
    //tituloUltimaCarga
    $("#tituloUltimaCarga").removeClass("hide");
    //collapseIdUltimaCarga
    $("#collapseIdUltimaCarga").removeClass("hide");

    //btnCargar
    $("#btnCargar").removeClass("hidden");
    $("#btnCargar").attr("disabled", "disabled");

    //******* Limpiar Seleccionados Comerciales
    $("#idRankingDeuda").removeClass("selected-option-light-blue");
    $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
    $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
    $("#idTodosMedidores").removeClass("selected-option-light-blue");
    $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
    $("#idTiposCorte").removeClass("selected-option-light-blue");
    $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
    $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");

    //******* Limpiar Seleccionados Tecnicos

    $("#idRastreo").removeClass("selected-option-light-blue");
    $("#idRANC").removeClass("selected-option-light-blue");
    $("#idPlanificZonas").removeClass("selected-option-light-blue");
    $("#idRedesAgua").removeClass("selected-option-light-blue");
    $("#idRedesCloaca").removeClass("selected-option-light-blue");
    $("#idConexAgua").removeClass("selected-option-light-blue");
    $("#idConexCloaca").removeClass("selected-option-light-blue");
    $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
    $("#idAnalisisLibre").removeClass("selected-option-light-blue");

}

function acordionFiltrosComerciales() {
    //********* Acordion Filtros- Estudios Comerciales 
    $('#collapseIdFiltrosPredefinidos').on('show.bs.collapse', function () {
        $("#grLisColecciones li").each(function () {
            $(this).removeClass("selected-option-light-blue");
        });
        $('#collapseIdColecciones').collapse('hide');
    });
    $('#collapseIdColecciones').on('show.bs.collapse', function () {
        $("#cboCargasDistritosFiltros").select2("val", 0);
        $("#FechaDesde").datepicker("clearDates");
        $("#FechaHasta").datepicker("clearDates");
        $('#collapseIdFiltrosPredefinidos').collapse('hide');
    });
}

function formattedDate(date) {
    var d = new Date(date || Date.now()),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join('/');
}

function acordionCargasAnalisisTecnico(id) {
    //******** Acordion Cargas- Analisis Tecnico

    $('#collapseIdCargaSistema').on('show.bs.collapse', function () {
        $(".file").fileinput("clear");
        $('#collapseIdCargaDdeArchivo').collapse('hide');
    });
    $('#collapseIdCargaDdeArchivo').on('show.bs.collapse', function () {
        $("#cboCargasDistritos").select2("val", 0)
        $("#FechaDesde").datepicker("clearDates");
        $("#FechaHasta").datepicker("clearDates");
        $('#collapseIdCargaSistema').collapse('hide');
    });

    OcultarUltimaCarga();

    if (id == 1 || id == 3 || id == 8) {
        //RESETEAR CARGA SISTEMA
        $("#titleCargaDdeSistema").removeClass("hide");
        $("#collapseIdCargaSistema").removeClass("hide");

        //RESETEAR ULTIMA CARGA
        $("#tituloUltimaCarga").removeClass("hide");
        $("#collapseIdUltimaCarga").removeClass("hide");
        //$("#btnUltimaCarga").removeClass("hide");

        //SWTICH CASE SIMULAR ULTIMA CARGA OP1-OP3
        // alert(id);
        switch (id) {
            case 1:
                var ultArchivoIngresado = readCookie('ultCargaDdeArchivoOp1');
                //alert(ultArchivoIngresado);
                if (ultArchivoIngresado != null && ultArchivoIngresado == 'TieneArchivo') {
                    //MOSTRAR ULTIMA CARGA
                    //$("#btnUltimaCarga").removeAttr("disabled", "disabled");

                    var ultFechaCargaDdeArchivo = readCookie('ultFechaCargaDdeArchivoOp1');
                    $("#lblUltDistritoCarga").text('Cargado desde archivo.');
                    //lblUltConsultaCarga
                    $("#lblUltConsultaCarga").text('');
                    //lblUltFechaCarga
                    $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaDdeArchivo));
                }
                else {
                    //CARGADO DDE SISTEMA
                    var ultDistritoIngresado = readCookie('distritoElegidoOp1');
                    var ultFechaDesdeIngresada = readCookie('fechaDesdeIngresadaOp1');
                    var ultFechaHastaIngresada = readCookie('fechaHastaIngresadaOp1');
                    var ultFechaCargaIngresada = readCookie('fechaUltimaCargaOp1');

                    if (ultDistritoIngresado != null) {
                        if (ultDistritoIngresado != 'Seleccione Distrito') {
                            //HABILITAR ULTIMA CARGA
                            //$("#btnUltimaCarga").removeAttr("disabled", "disabled");
                            $("#lblUltDistritoCarga").text('Distrito : ' + ultDistritoIngresado);
                            $("#lblUltConsultaCarga").text('Consulta : ' + formattedDate(ultFechaDesdeIngresada) + ' al ' + formattedDate(ultFechaHastaIngresada));
                            $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaIngresada));
                        }
                        else {
                            $("#lblUltDistritoCarga").text('No posee última carga.');
                        }
                    }
                    else {
                        $("#lblUltDistritoCarga").text('No posee última carga.');
                    }
                }
                break;
            //***** FIN MOSTRAR ULTIMA CARGA
            case 3:
                var ultArchivoIngresado = readCookie('ultCargaDdeArchivoOp3');
                // alert(ultArchivoIngresado);
                if (ultArchivoIngresado != null && ultArchivoIngresado == 'TieneArchivo') {
                    //MOSTRAR ULTIMA CARGA
                    var ultFechaCargaDdeArchivo = readCookie('ultFechaCargaDdeArchivoOp3');
                    $("#lblUltDistritoCarga").text('Cargado desde archivo.');
                    //lblUltConsultaCarga
                    $("#lblUltConsultaCarga").text('');
                    //lblUltFechaCarga
                    $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaDdeArchivo));
                }
                else {
                    //CARGADO DDE SISTEMA
                    var ultDistritoIngresado = readCookie('distritoElegidoOp3');
                    var ultFechaDesdeIngresada = readCookie('fechaDesdeIngresadaOp3');
                    var ultFechaHastaIngresada = readCookie('fechaHastaIngresadaOp3');
                    var ultFechaCargaIngresada = readCookie('fechaUltimaCargaOp3');
                    //var ultTipoReporteIngresado = readCookie('ultTipoReporteOp1');
                    //alert(ultDistritoIngresado);
                    if (ultDistritoIngresado != null) {
                        if (ultDistritoIngresado != 'Seleccione Distrito') {
                            //HABILITAR ULTIMA CARGA
                            //$("#btnUltimaCarga").removeAttr("disabled", "disabled");
                            $("#lblUltDistritoCarga").text('Distrito : ' + ultDistritoIngresado);
                            $("#lblUltConsultaCarga").text('Consulta : ' + formattedDate(ultFechaDesdeIngresada) + ' al ' + formattedDate(ultFechaHastaIngresada));
                            $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaIngresada));
                        }
                        else {
                            $("#lblUltDistritoCarga").text('No posee última carga.');
                        }
                    }
                    else {
                        $("#lblUltDistritoCarga").text('No posee última carga.');
                    }
                }
                break;
            case 8:
                var ultArchivoIngresado = readCookie('ultCargaDdeArchivoOp8');
                //alert(ultArchivoIngresado);
                if (ultArchivoIngresado != null && ultArchivoIngresado == 'TieneArchivo') {
                    //MOSTRAR ULTIMA CARGA
                    var ultFechaCargaDdeArchivo = readCookie('ultFechaCargaDdeArchivoOp8');
                    $("#lblUltDistritoCarga").text('Cargado desde archivo.');
                    //lblUltConsultaCarga
                    $("#lblUltConsultaCarga").text('');
                    //lblUltFechaCarga
                    $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaDdeArchivo));
                }
                else {
                    //CARGADO DDE SISTEMA
                    var ultDistritoIngresado = readCookie('distritoElegidoOp8');
                    var ultFechaDesdeIngresada = readCookie('fechaDesdeIngresadaOp8');
                    var ultFechaHastaIngresada = readCookie('fechaHastaIngresadaOp8');
                    var ultFechaCargaIngresada = readCookie('fechaUltimaCargaOp8');
                    //var ultTipoReporteIngresado = readCookie('ultTipoReporteOp1');
                    //alert(ultDistritoIngresado);
                    if (ultDistritoIngresado != null) {
                        if (ultDistritoIngresado != 'Seleccione Distrito') {
                            //HABILITAR ULTIMA CARGA
                            //$("#btnUltimaCarga").removeAttr("disabled", "disabled");
                            $("#lblUltDistritoCarga").text('Distrito : ' + ultDistritoIngresado);
                            $("#lblUltConsultaCarga").text('Consulta : ' + formattedDate(ultFechaDesdeIngresada) + ' al ' + formattedDate(ultFechaHastaIngresada));
                            $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaIngresada));
                        }
                        else {
                            $("#lblUltDistritoCarga").text('No posee última carga.');
                        }
                    }
                    else {
                        $("#lblUltDistritoCarga").text('No posee última carga.');
                    }
                }
                break;

        };

    }
    else {
        if (id == 2 || id == 4 || id == 5 || id == 6 || id == 7) {
            OcultarUltimaCarga();
        }
        else {
            //SOLO PARA OP9
            if (id == 9) {
                SoloCargaDesdeArchivo();
                var ultArchivoIngresado = readCookie('ultCargaDdeArchivoOp9');
                if (ultArchivoIngresado != null && ultArchivoIngresado == 'TieneArchivo') {
                    //MOSTRAR ULTIMA CARGA
                    var ultFechaCargaDdeArchivo = readCookie('ultFechaCargaDdeArchivoOp9');
                    $("#lblUltDistritoCarga").text('Cargado desde archivo.');
                    //lblUltConsultaCarga
                    $("#lblUltConsultaCarga").text('');
                    //lblUltFechaCarga
                    $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaDdeArchivo));
                }
                else {
                    //CARGADO DDE SISTEMA
                    var ultDistritoIngresado = readCookie('distritoElegidoOp9');
                    var ultFechaDesdeIngresada = readCookie('fechaDesdeIngresadaOp9');
                    var ultFechaHastaIngresada = readCookie('fechaHastaIngresadaOp9');
                    var ultFechaCargaIngresada = readCookie('fechaUltimaCargaOp9');
                    //var ultTipoReporteIngresado = readCookie('ultTipoReporteOp1');

                    if (ultDistritoIngresado != null) {
                        if (ultDistritoIngresado != 'Seleccione Distrito') {
                            //HABILITAR ULTIMA CARGA
                            //$("#btnUltimaCarga").removeAttr("disabled", "disabled");
                            $("#lblUltDistritoCarga").text('Distrito : ' + ultDistritoIngresado);
                            $("#lblUltConsultaCarga").text('Consulta : ' + formattedDate(ultFechaDesdeIngresada) + ' al ' + formattedDate(ultFechaHastaIngresada));
                            $("#lblUltFechaCarga").text('Fecha de carga: ' + formattedDate(ultFechaCargaIngresada));
                        }
                        else {
                            $("#lblUltDistritoCarga").text('No posee última carga.');
                        }
                    }
                    else {
                        $("#lblUltDistritoCarga").text('No posee última carga.');
                    }
                }
            }

        }

    }
}

function OcultarUltimaCarga() {

    //RESETEAR CARGA SISTEMA
    $("#titleCargaDdeSistema").removeClass("hide");
    $("#collapseIdCargaSistema").removeClass("hide");

    //OCULTAR ULTIMA CARGA
    $("#tituloUltimaCarga").addClass("hide");
    $("#collapseIdUltimaCarga").addClass("hide");
    $("#btnUltimaCarga").addClass("hide");
}

function SoloCargaDesdeArchivo() {
    //HIDE: SOLO ARCHIVO
    $("#titleCargaDdeSistema").addClass("hide");
    $("#collapseIdCargaSistema").addClass("hide");
    $("#btnUltimaCarga").removeClass("hide");
}

function CalendarioFechas() {
    var today = new Date();
    var yesterday = new Date(today.getTime() - 24 * 60 * 60 * 1000);

    $("#FechaDesde").datepicker(getDatePickerConfig({ endDate: yesterday }))
        .on("changeDate", function () {
            $("#FechaHasta").datepicker("setStartDate", $(this).datepicker("getDate"));
        });

    $("#FechaHasta").datepicker(getDatePickerConfig({ endDate: today }))
        .on("changeDate", function () {
            $("#FechaDesde").datepicker("setEndDate", $(this).datepicker("getDate"));
        });
}

function RemarcarTipoAnalisisTecnicoSeleccionado(id) {
    switch (id) {
        case 1:
            $("#idRastreo").addClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");
            break;
        case 2:
            $("#idRANC").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");

            break;
        case 3:
            $("#idPlanificZonas").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");
            break;
        case 4:
            $("#idRedesAgua").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");
            break;
        case 5:
            $("#idRedesCloaca").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");
            break;
        case 6:
            $("#idConexAgua").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");
            break;
        case 7:
            $("#idConexCloaca").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");
            break;
        case 8:
            $("#idFaltAguaPresion").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idAnalisisLibre").removeClass("selected-option-light-blue");
            break;
        case 9:
            $("#idAnalisisLibre").addClass("selected-option-light-blue");
            $("#idRastreo").removeClass("selected-option-light-blue");
            $("#idRANC").removeClass("selected-option-light-blue");
            $("#idPlanificZonas").removeClass("selected-option-light-blue");
            $("#idRedesAgua").removeClass("selected-option-light-blue");
            $("#idRedesCloaca").removeClass("selected-option-light-blue");
            $("#idConexAgua").removeClass("selected-option-light-blue");
            $("#idConexCloaca").removeClass("selected-option-light-blue");
            $("#idFaltAguaPresion").removeClass("selected-option-light-blue");
            break;
    }
}

function RemarcarTipoeEstudioComercialSeleccionado(id) {
    switch (id) {

        case 1:
            $("#idRankingDeuda").addClass("selected-option-light-blue");
            $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
            $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
            $("#idTodosMedidores").removeClass("selected-option-light-blue");
            $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
            $("#idTiposCorte").removeClass("selected-option-light-blue");
            $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
            $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");
            break;
        case 2:
            $("#idRankingFactImpagas").addClass("selected-option-light-blue");
            $("#idRankingDeuda").removeClass("selected-option-light-blue");
            $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
            $("#idTodosMedidores").removeClass("selected-option-light-blue");
            $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
            $("#idTiposCorte").removeClass("selected-option-light-blue");
            $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
            $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");
            break;
        case 3:
            $("#idRankingTotalDeuda").addClass("selected-option-light-blue");
            $("#idRankingDeuda").removeClass("selected-option-light-blue");
            $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
            $("#idTodosMedidores").removeClass("selected-option-light-blue");
            $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
            $("#idTiposCorte").removeClass("selected-option-light-blue");
            $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
            $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");
            break;
        case 4:
            $("#idTodosMedidores").addClass("selected-option-light-blue");
            $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
            $("#idRankingDeuda").removeClass("selected-option-light-blue");
            $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
            $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
            $("#idTiposCorte").removeClass("selected-option-light-blue");
            $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
            $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");
            break;
        case 5:
            $("#idMedidoresDifMarca").addClass("selected-option-light-blue");
            $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
            $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
            $("#idTodosMedidores").removeClass("selected-option-light-blue");
            $("#idRankingDeuda").removeClass("selected-option-light-blue");
            $("#idTiposCorte").removeClass("selected-option-light-blue");
            $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
            $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");
            break;
        case 6:
            $("#idTiposCorte").addClass("selected-option-light-blue");
            $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
            $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
            $("#idTodosMedidores").removeClass("selected-option-light-blue");
            $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
            $("#idRankingDeuda").removeClass("selected-option-light-blue");
            $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
            $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");
            break;
        case 7:
            $("#idExistenciaCorteAnterior").addClass("selected-option-light-blue");
            $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
            $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
            $("#idTodosMedidores").removeClass("selected-option-light-blue");
            $("#idRankingDeuda").removeClass("selected-option-light-blue");
            $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
            $("#idTiposCorte").removeClass("selected-option-light-blue");
            $("#idRangosConsumoPromAnual").removeClass("selected-option-light-blue");
            break;
        case 8:
            $("#idRangosConsumoPromAnual").addClass("selected-option-light-blue");
            $("#idRankingFactImpagas").removeClass("selected-option-light-blue");
            $("#idRankingTotalDeuda").removeClass("selected-option-light-blue");
            $("#idRankingDeuda").removeClass("selected-option-light-blue");
            $("#idTodosMedidores").removeClass("selected-option-light-blue");
            $("#idMedidoresDifMarca").removeClass("selected-option-light-blue");
            $("#idTiposCorte").removeClass("selected-option-light-blue");
            $("#idExistenciaCorteAnterior").removeClass("selected-option-light-blue");
            break;
    }
}


function UnUnicoRadioButtonSeleccionadoALaVez() {
    //alert('Llega al metodo radiobutton once');

    //REP ANALISIS LIBRE
    $("#rbTotInterv").on("change", function () {
        //AL ELEGIR ESTE BOTON SE DESHABILITA EL RESTO
        //$('#rbTotInterv').prop('checked', true);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);
    });


    $("#rbRankCuadras").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        //$('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbZonInter").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        //$('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbRankCuadr").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        //$('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    //========================
    //REP FALTA AGUA PRESION


    $("#rbRankMallas").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        // $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbRankMallasMap").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        //$('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);
    });

    $("#rbIntervTipoMalla").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        //$('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });
    //=========================
    //REP SISTEMATICO


    $("#rbIndiceSuc").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        //$('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbRankCuadrasRepSis").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        //$('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbIntervCuadraSubc").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        //$('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbRankCuadRepSis").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        //$('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbIntervCuadSubcuenRepSisMap").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        //$('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbRankSubcuenMapRepSis").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        //$('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });
    //==========================
    //REP ZONA LAVADO


    $("#rbTotInvervRepZonLav").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        //$('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbMallasInvervRepZonLav").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        //$('#rbMallasInvervRepZonLav').prop('checked', false);
        $('#rbRatioMallaInvervRepZonLav').prop('checked', false);

    });

    $("#rbRatioMallaInvervRepZonLav").on("change", function () {
        $('#rbTotInterv').prop('checked', false);

        $('#rbRankCuadras').prop('checked', false);
        $('#rbZonInter').prop('checked', false);
        $('#rbRankCuadr').prop('checked', false);
        $('#rbRankMallas').prop('checked', false);
        $('#rbRankMallasMap').prop('checked', false);
        $('#rbIntervTipoMalla').prop('checked', false);
        $('#rbIndiceSuc').prop('checked', false);
        $('#rbRankCuadrasRepSis').prop('checked', false);
        $('#rbIntervCuadraSubc').prop('checked', false);
        $('#rbRankCuadRepSis').prop('checked', false);
        $('#rbIntervCuadSubcuenRepSisMap').prop('checked', false);
        $('#rbRankSubcuenMapRepSis').prop('checked', false);
        $('#rbTotInvervRepZonLav').prop('checked', false);
        $('#rbMallasInvervRepZonLav').prop('checked', false);
        //$('#rbRatioMallaInvervRepZonLav').prop('checked', false);
    });
}

//Manejo de Cookies
function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function search(value) {
    $("#tabla-analisispredefinidos").DataTable().search(value).draw();
}

function initCtrlsPluggins(divId, fileInputs) {
    var div = "#" + divId;
    //Autocomplete select
    $(".select2", div).select2();


    //File input
    if (typeof fileInputs === "undefined") {
        $(".file", div).fileinput({
            showRemove: false,
            showUpload: false,
            showPreview: false,
            allowedFileExtensions: ["csv", "txt"]
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

}

function analisispredefinidoLoadData(id) {
    analisispredefinidoEnableControls(true);

    var pdf = null;

    $.each(modelanalisispredefinidos, function (index, object) {
        if (object.Idanalisispredefinido == id) {

            $.each(object, function (key, value) {
                var control = $("[name='" + key + "']", "#analisispredefinido-form");
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

            pdf = object.analisispredefinidoFondos[0].ImagenNombre + ".pdf";

            //Bold,Italic,Underline,Strikeout
            var estilo = object.ReferenciaFuenteEstilo.split(",");
            $("#analisispredefinido-negrita").prop("checked", estilo[0] === "1" ? true : false);
            $("#analisispredefinido-cursiva").prop("checked", estilo[1] === "1" ? true : false);
            $("#analisispredefinido-subrayada").prop("checked", estilo[2] === "1" ? true : false);
            $("#analisispredefinido-tachada").prop("checked", estilo[3] === "1" ? true : false);

            return false;
        }
    });

    $.post(BASE_URL + "analisispredefinido/analisispredefinido/" + id, function () {
        $("#Pdf").fileinput("refresh", {
            showUpload: false,
            initialCaption: pdf,
            initialPreview: ["<object data=\"CommonFiles/" + pdf + "\" type=\"application/pdf\" width=\"160px\" height=\"160px\">"]
        });
        $("#Pdf").fileinput("disable");
    });

    $("#imagen-norte").attr("src", "CommonFiles/norte" + $("#id-norte :selected").val() + ".png");
    $("#id-norte").change(function () {
        $("#imagen-norte").attr("src", "CommonFiles/norte" + $(this).val() + ".png");
    });
}

function analisispredefinidoValidator() {

    $("#analisispredefinido-form").bootstrapValidator({
        framework: "bootstrap",
        fields: {
            Nombre: {
                validators: {
                    notEmpty: {
                        message: "El nombre es obligatorio"
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
                            var title = $(".file-caption-name", "#analisispredefinido-form").attr("title");
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

function insertUpdateanalisispredefinido() {
    $("#headingListado").addClass("panel-deshabilitado");
    $("#headingListado").find("a:first[aria-expanded=true]").click();
    $("#footer-analisispredefinidos span[aria-controls='button']").removeClass("boton-deshabilitado");
    $("#footer-analisispredefinidos span[aria-controls='button']").addClass("black");

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

    if (selectedRowanalisispredefinidos == null) {
        analisispredefinidoEnableControls(false);
    }

    enableFormContent("#analisispredefinido-form", false);
    columnsAdjust("tabla-analisispredefinidos");
}

function analisispredefinidoEnableControls(value) {
    if (value) {
        $("#analisispredefinido-edit").removeClass("boton-deshabilitado");
        $("#analisispredefinido-delete").removeClass("boton-deshabilitado");
        $("#analisispredefinido-copy").removeClass("boton-deshabilitado");
        $("div[role='region']").removeClass("panel-deshabilitado");
        $("#headinganalisispredefinidoInfo").find("a:first[aria-expanded=false]").click();
    } else {
        $("#headingListado").removeClass("panel-deshabilitado");
        $("#headingListado").find("a:first[aria-expanded=false]").click();
        $("#analisispredefinido-edit").addClass("boton-deshabilitado");
        $("#analisispredefinido-delete").addClass("boton-deshabilitado");
        $("#analisispredefinido-copy").addClass("boton-deshabilitado");
        $("div[role='region']").find("a:first[aria-expanded=true]").click()
        $("div[role='region']").addClass("panel-deshabilitado");
        $("#footer-analisispredefinidos span[aria-controls='button']").removeClass("black");
        $("#footer-analisispredefinidos span[aria-controls='button']").addClass("boton-deshabilitado");
    }
}

function analisispredefinidoFormContent() {
    $("#analisispredefinido-form").load(BASE_URL + "analisispredefinido/FormContent", function () {
        initCtrlsPluggins("#analisispredefinido-form");

        analisispredefinidoValidator();

        enableFormContent("#analisispredefinido-form", false);

        $("#imagen-norte").attr("src", "CommonFiles/norte" + $("#id-norte :selected").val() + ".png");
        $("#id-norte").change(function () {
            $("#imagen-norte").attr("src", "CommonFiles/norte" + $(this).val() + ".png");
        });
    });

    //Init once only

    $("#analisispredefinido-edit").click(function () {
        if (selectedRowanalisispredefinidos) {
            enableFormContent("#analisispredefinido-form", true);
            $("#Pdf").fileinput("enable");
            insertUpdateanalisispredefinido();
            analisispredefinidoRevalidations();
        }
    });

    $("#dvEstudiosComerciales").click(function () {
        //HIDE PANEL TECNICO
        //pnlCargas
        RestablecerAGeneradorInicial();
        $("#pnlCargas").addClass("hidden");

    });

    $("#dvAnalisisTecnico").click(function () {
        //HIDE PANEL COMERCIAL
        //pnlFiltros
        RestablecerAGeneradorInicial();
        $("#pnlFiltros").addClass("hidden");

    });

    //$("#titleCargaDdeSistema").click(function () {
    //    //alert('');
    //    //cboCargasDistritos SELECT VAL 0
    //    //#FechaDesde CLEAR?
    //    //#FechaHasta CLEAR?
    //});

    //$("#titleCargaDdeArchivoUsuario").click(function () {
    //    //alert('');
    //    //listCargaTecnica
    //    //$(".file").fileinput("clear");
    //});

    $("#btnUltimaCarga").click(function () {
        var id = 10;
        showLoading();
        $.ajax({
            url: BASE_URL + "analisispredefinido/CargarUltimaCarga/" + id,
            type: "POST",
            success: function (data) {
                setTimeout(function () {
                    hideLoading();
                    modalConfirm("Ultima Carga", "Se cargaron 20 reclamos ", "delete-analisispredefinido");
                }, 5000);
            }
        });

        $("#btnAdvertenciaOKanalisispredefinido").click(function () {
            //Cierro el mensaje y el Modal Analisis Predefinido
            //alert('Carga de Reclamos por Filtro Finalizada.');
            //alert(tipoReporte);
        });

    });




    $("#btnCargar").click(function () {
        var idPredefinido = parseInt($("#idPredefinido").val(), 10);

        var id = 10;
        var tipoCarga = $("#idTipoDeCarga").val();
        var tipoReporte = 0;
        tipoReporte = tipoCarga.substring(tipoCarga.length - 1, tipoCarga.length);
        //alert(tipoReporte);

        var result = tipoCarga.split("/");
        //var tipoCarga = result[2].toString();
        var tipoCarga = result[result.length - 2].toString();
        //alert(tipoCarga);

        showLoading();

        if (tipoCarga == "GetFiltros") {

            if (idPredefinido >= 1 && idPredefinido <= 8 || idPredefinido == 17) {
                //Llamar a mapa tematico con parametros 

                var valDistritoElegidoID = -1;
                var valDistritoElegidoText1 = $("#cboCargasDistritosFiltros").select2('data').text;
                var valColeccionElegidaID = -1;
                var valColeccionElegidaText = $("#cboColeccion").select2('data').text;
                //long Tipo_Carga= 1: POR ACHIVO; 2: POR SISTEMA
                var tipoCarga = -1;
                var fechaDesdeValor1 = $("#FechaDesde").val();
                var fechaHastaValor1 = $("#FechaHasta").val();

                if (valDistritoElegidoText1 != 'Seleccione Distrito') {
                    valDistritoElegidoID = $("#cboCargasDistritosFiltros").select2('data').id;
                    tipoCarga = 0;
                }
                else {
                    tipoCarga = 1;
                }
                if (valColeccionElegidaText != 'Seleccione Colección') {
                    valColeccionElegidaID = $("#cboColeccion").select2('data').id;
                }
                var hayDistritoFiltro = valDistritoElegidoText1;
                var hayColeccion = valColeccionElegidaText;
                var colSeleccionadaFiltro = readCookie('coleccionSeleccionada');

                //if ((hayDistritoFiltro != 'Seleccione Distrito' && fechaDesdeValor1 != "" && fechaHastaValor1 != "") || (hayColeccion != 'Seleccione Colección' && fechaDesdeValor1 != "" && fechaHastaValor1 != "")) {
                if (hayDistritoFiltro != 'Seleccione Distrito' || hayColeccion != 'Seleccione Colección') {
                    if ((idPredefinido == 4 || idPredefinido == 5) || (idPredefinido != 4 && idPredefinido != 5 && fechaDesdeValor1 != "" && fechaHastaValor1 != "")) {

                        var idDistrito = (valDistritoElegidoID == -1 ? "" : valDistritoElegidoID.toString());
                        var idColeccion = (valColeccionElegidaID == -1 ? "0" : valColeccionElegidaID);

                        if (fechaDesdeValor1 == "") {
                            fechaDesdeValor1 = "01/01/1900";
                        }
                        if (fechaHastaValor1 == "") {
                            fechaHastaValor1 = "01/01/1900";
                        }
                        //var fechaDesde = new Date(2006, 4, 17);
                        //var fechaHasta = new Date(2008, 2, 19);
                        var aFechaDesde = fechaDesdeValor1.split('/');
                        var aFechaHasta = fechaHastaValor1.split('/');
                        var fechaDesde = new Date(aFechaDesde[2], aFechaDesde[1] - 1, aFechaDesde[0]);
                        var fechaHasta = new Date(aFechaHasta[2], aFechaHasta[1] - 1, aFechaHasta[0]);

                        //Buscar idConfig en MT_PREDEFINIDO. En base a idPredefinido buscar en MT_PREDEFINIDO y obtener el idConfig para pasarle como parametro
                        $.ajax({
                            url: BASE_URL + "MapasTematicos/GetIdConfigByIdPredefinido",
                            data: { idPredefinido: idPredefinido },
                            dataType: 'json',
                            type: 'POST',
                            success: function (data) {
                                var idConfig = data;
                                if (idConfig != null && idConfig > 0) {
                                    $('#analisispredefinidoModal').modal('hide');
                                    //var obj = { ConfiguracionId: Number(idConfig), idDistrito: "", idColeccion: 21, fechaDesde: fechaDesde, fechaHasta: fechaHasta };
                                    var obj = { ConfiguracionId: Number(idConfig), idDistrito: idDistrito, idColeccion: idColeccion, fechaDesde: fechaDesde, fechaHasta: fechaHasta };
                                    loadView(BASE_URL + "MapasTematicos/GetVisualizacionView_Guardada?parametros=" + JSON.stringify(obj));
                                }
                                else {
                                    hideLoading();
                                    alert('No se encontró la configuración.');
                                }
                            }, error: function (ex) {
                                hideLoading();
                                alert('Error: No se encontró la configuración.');
                            }
                        });
                    }
                    else {
                        hideLoading();
                        if (fechaDesdeValor1 == "" || fechaHastaValor1 == "") {
                            alert('Debe cargar un rango de fechas.');
                        }
                    }
                }
                else {
                    hideLoading();
                    if (hayDistritoFiltro == 'Seleccione Distrito' || colSeleccionadaFiltro != null) {
                        alert('Debe Seleccionar un Distrito o ingresar una Colección.');
                    }
                }

            }
            else {

                //OBTENER PARAMETROS PARA INSERTAR RECLAMOS TECNICO
                //long Id_Carga_Tecnica=AUTONUMERICO
                var IdAnalisis = tipoReporte;
                //string Id_Distrito : SELECCIONADO COMBO
                var valDistritoElegidoID = -1;
                var valDistritoElegidoText1 = $("#cboCargasDistritosFiltros").select2('data').text;
                //long Tipo_Carga= 1: POR ACHIVO; 2: POR SISTEMA
                var tipoCarga = -1;
                //DateTime? Fecha_Desde: SELEC DATEPICKER
                var fechaDesdeValor1 = $("#FechaDesde").val();
                //DateTime? Fecha_Hasta: SELEC DATEPICKER
                var fechaHastaValor1 = $("#FechaHasta").val();
                //long Usuario_Alta : 1
                var usuarioAlta = 1;
                //DateTime? Fecha_Alta: TODAY
                var fechaActualValor1 = dameFechaActual();
                //alert(fechaDesdeValor1);
                //alert(fechaHastaValor1);
                if (valDistritoElegidoText1 != 'Seleccione Distrito') {
                    valDistritoElegidoID = $("#cboCargasDistritosFiltros").select2('data').id;
                    tipoCarga = 0;
                }
                else {
                    tipoCarga = 1;
                }

                //************************************
                //VALIDACION DE CONTROLES CON DATOS ANTES DE LA CARGA
                //var hayArchivo = $("#listCargaTecnica").val();
                var hayDistritoFiltro = valDistritoElegidoText1;
                var hayFDesdeFiltro = fechaDesdeValor1;
                var hayFHastaFiltro = fechaHastaValor1;
                var fechahoyCargasFiltro = fechaActualValor1;
                var colSeleccionadaFiltro = readCookie('coleccionSeleccionada');

                if ((hayDistritoFiltro != 'Seleccione Distrito' && fechaDesdeValor1 != "" && fechaHastaValor1 != "") || colSeleccionadaFiltro != null) {

                    $.ajax({
                        url: BASE_URL + "analisispredefinido/CargaDeReclamos?analisisID=" + IdAnalisis + "&distritoElegidoID=" + valDistritoElegidoID + "&cargaTipo=" + tipoCarga + "&fechaDesdeInput=" + fechaDesdeValor + "&fechaHastaInput=" + fechaHastaValor + "&usuarioID=" + usuarioAlta + "&fechaDeAlta=" + fechaActualValor,
                        type: "POST",
                        success: function (data) {
                            setTimeout(function () {
                                hideLoading();

                                modalConfirm("Estudios Comerciales", "Se cargaron 20 reclamos ", "delete-analisispredefinido");
                            }, 5000);
                        }
                    });

                    $("#btnAdvertenciaOKanalisispredefinido").click(function () {
                        //Cierro el mensaje y el Modal Analisis Predefinido
                        //LimpiarControles();
                    });
                } //FIN VALIDACION DISTRITOVACIO
                else {
                    hideLoading();

                    if (hayDistritoFiltro == 'Seleccione Distrito' || colSeleccionadaFiltro != null || fechaDesdeValor1 == "" || fechaHastaValor1 == "") {
                        alert('OCURRIO UN ERROR: Debe Seleccionar un Distrito y cargar un rango de fechas o ingresar una coleccion.');
                    }
                }
                LimpiarControles();
            }
        }
        else {
            if (tipoCarga == "GetCargas") {

                //OBTENER PARAMETROS PARA INSERTAR RECLAMOS TECNICO
                //long Id_Carga_Tecnica=AUTONUMERICO
                var IdAnalisis = tipoReporte;
                //string Id_Distrito : SELECCIONADO COMBO
                var valDistritoElegidoID = -1;
                var valDistritoElegidoText = $("#cboCargasDistritos").select2('data').text;
                //long Tipo_Carga= 1: POR ACHIVO; 2: POR SISTEMA
                var tipoCarga = -1;
                //DateTime? Fecha_Desde: SELEC DATEPICKER
                var fechaDesdeValor = $("#FechaDesde").val();
                //DateTime? Fecha_Hasta: SELEC DATEPICKER
                var fechaHastaValor = $("#FechaHasta").val();
                //long Usuario_Alta : 1
                var usuarioAlta = 1;
                //DateTime? Fecha_Alta: TODAY
                var fechaActualValor = dameFechaActual();

                if (valDistritoElegidoText != 'Seleccione Distrito') {
                    valDistritoElegidoID = $("#cboCargasDistritos").select2('data').id;
                    tipoCarga = 0;
                }
                else {
                    tipoCarga = 1;
                }

                //AnalisisTecnicos AnalisisTecnico: SE OBTIENE POR ID_ANALISIS EN EL CONTROLLER
                if (tipoReporte == 1 || tipoReporte == 3 || tipoReporte == 8 || tipoReporte == 9) {
                    //VALIDACION DE CONTROLES CON DATOS ANTES DE LA CARGA
                    var hayArchivo = $("#listCargaTecnica").val();
                    var hayDistrito = valDistritoElegidoText;
                    var hayFDesde = fechaDesdeValor;
                    var hayFHasta = fechaHastaValor;
                    var fechahoyCargas = fechaActualValor;

                    if ((hayDistrito != 'Seleccione Distrito' && fechaDesdeValor != "" && fechaHastaValor != "") || hayArchivo != '') {


                        $.ajax({
                            url: BASE_URL + "analisispredefinido/CargaDeReclamos?analisisID=" + IdAnalisis + "&distritoElegidoID=" + valDistritoElegidoID + "&cargaTipo=" + tipoCarga + "&fechaDesdeInput=" + fechaDesdeValor + "&fechaHastaInput=" + fechaHastaValor + "&usuarioID=" + usuarioAlta + "&fechaDeAlta=" + fechaActualValor,
                            type: "POST",
                            success: function (data) {
                                setTimeout(function () {
                                    hideLoading();
                                    //LimpiarControles();
                                    modalConfirm("Análisis Técnicos", "Se cargaron 20 reclamos ", "delete-analisispredefinido");
                                }, 5000);
                            }
                        });

                        $("#btnAdvertenciaOKanalisispredefinido").click(function () {
                            //Hide PANEL IZQUIERDO
                            $("#MenuIzquierdo").addClass("hide");
                            //Hide Boton Ultima Carga
                            $("#btnUltimaCarga").addClass("hide");

                            //Hide Boton Cargar
                            $("#btnCargar").addClass("hide");

                            //Show Boton Procesar
                            $("#btnProcesarTipoReporte").removeClass("hide");

                            $("#dvPanelDerecho").removeClass("col-lg-8 col-md-8 col-sm-8 col-xs-8");


                            var altura = $(window).height() - 50; //value corresponding to the modal heading + footer
                            var ancho = $(window).width() - 800; //value corresponding to the modal heading + footer

                            $(" .modal-content").css({ "height": altura, "overflow": "hidden" });
                            $(" .modal-content").css({ "width": ancho });
                            $(".modal-dialog").css({ "height": altura });
                            $(".modal-dialog").css({ "width": ancho });

                            $("#analisispredefinidoModal .modal-content").getNiceScroll().resize();
                            $("#analisispredefinidoModal .modal-content").getNiceScroll().show();

                            //Load REPORTES - PANEL DERECHO
                            switch (tipoReporte) {
                                case "1":

                                    //SI CARGA DESDE ARCHIVO. LOGICA VA AQUI
                                    var archivoSubido = $("#listCargaTecnica").val();
                                    // alert(archivoSubido);
                                    if (archivoSubido != "") {
                                        //SI CARGA DESDE ARCHIVO, NO INTERESA EL RESTO DEL CODIGO
                                        //“Cargado desde archivo”
                                        //“Fecha de carga: “ + FECHA_ALTA
                                        var valTipoReporte = tipoReporte;
                                        var valFechaUltimaCargaDdeArchivo = new Date();
                                        var dd = valFechaUltimaCargaDdeArchivo.getDate();
                                        var mm = valFechaUltimaCargaDdeArchivo.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCargaDdeArchivo.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCargaDdeArchivo = mm + '/' + dd + '/' + yyyy;

                                        eraseCookie('ultCargaDdeArchivoOp1');
                                        eraseCookie('ultFechaCargaDdeArchivoOp1');
                                        eraseCookie('ultTipoReporteDdeArchivoOp1');

                                        createCookie('ultCargaDdeArchivoOp1', 'TieneArchivo', 2);
                                        createCookie('ultFechaCargaDdeArchivoOp1', valFechaUltimaCargaDdeArchivo, 2);
                                        // AGREGAR COOKIE PARA LLEVARME EL TIPO DE REPORTE
                                        createCookie('ultTipoReporteDdeArchivoOp1', valTipoReporte, 2);

                                    }
                                    else {
                                        //SINO CARGA DDE ARCHIVO, CARGA DDE SISTEMA
                                        //alert('REPORTE Rastreo sistemático. Elija Reporte o Mapa y haga click en Procesar');
                                        var valDistritoElegido = $("#cboCargasDistritos").select2('data').text;
                                        var valFechaDesdeIngresada = $("#FechaDesde").val();
                                        var valFechaHastaIngresada = $("#FechaHasta").val();
                                        var valTipoReporte = tipoReporte;

                                        var valFechaUltimaCarga = new Date();
                                        var dd = valFechaUltimaCarga.getDate();
                                        var mm = valFechaUltimaCarga.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCarga.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCarga = mm + '/' + dd + '/' + yyyy;
                                        eraseCookie('ultCargaDdeArchivoOp1');
                                        eraseCookie('distritoElegidoOp1');
                                        eraseCookie('fechaDesdeIngresadaOp1');
                                        eraseCookie('fechaHastaIngresadaOp1');
                                        eraseCookie('fechaUltimaCargaOp1');
                                        //valTipoReporte
                                        eraseCookie('ultTipoReporteOp1');

                                        createCookie('distritoElegidoOp1', valDistritoElegido, 2);
                                        createCookie('fechaDesdeIngresadaOp1', valFechaDesdeIngresada, 2);
                                        createCookie('fechaHastaIngresadaOp1', valFechaHastaIngresada, 2);
                                        createCookie('fechaUltimaCargaOp1', valFechaUltimaCarga, 2);
                                        createCookie('ultTipoReporteOp1', valTipoReporte, 2);

                                    }

                                    cargaranalisispredefinido(BASE_URL + 'AnalisisPredefinido/GetReporteSistematico/20', 20);
                                    $("#lblModalDomicilio").text("Rastreo Sistemático");
                                    break;
                                case "3":
                                    //SI CARGA DESDE ARCHIVO. LOGICA VA AQUI
                                    var archivoSubido = $("#listCargaTecnica").val();
                                    // alert(archivoSubido);
                                    if (archivoSubido != "") {
                                        //SI CARGA DESDE ARCHIVO, NO INTERESA EL RESTO DEL CODIGO
                                        //“Cargado desde archivo”
                                        //“Fecha de carga: “ + FECHA_ALTA
                                        var valTipoReporte = tipoReporte;
                                        var valFechaUltimaCargaDdeArchivo = new Date();
                                        var dd = valFechaUltimaCargaDdeArchivo.getDate();
                                        var mm = valFechaUltimaCargaDdeArchivo.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCargaDdeArchivo.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCargaDdeArchivo = mm + '/' + dd + '/' + yyyy;

                                        eraseCookie('ultCargaDdeArchivoOp3');
                                        eraseCookie('ultFechaCargaDdeArchivoOp3');
                                        eraseCookie('ultTipoReporteDdeArchivoOp3');
                                        createCookie('ultCargaDdeArchivoOp3', 'TieneArchivo', 2);
                                        createCookie('ultFechaCargaDdeArchivoOp3', valFechaUltimaCargaDdeArchivo, 2);
                                        // AGREGAR COOKIE PARA LLEVARME EL TIPO DE REPORTE
                                        createCookie('ultTipoReporteDdeArchivoOp3', valTipoReporte, 2);

                                    }
                                    else {
                                        //alert('REPORTE Zonas de lavado. Elija Reporte o Mapa y haga click en Procesar');
                                        var valDistritoElegido = $("#cboCargasDistritos").select2('data').text;
                                        var valFechaDesdeIngresada = $("#FechaDesde").val();
                                        var valFechaHastaIngresada = $("#FechaHasta").val();
                                        var valTipoReporte = tipoReporte;

                                        var valFechaUltimaCarga = new Date();
                                        var dd = valFechaUltimaCarga.getDate();
                                        var mm = valFechaUltimaCarga.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCarga.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCarga = mm + '/' + dd + '/' + yyyy;
                                        eraseCookie('ultCargaDdeArchivoOp3');
                                        eraseCookie('distritoElegidoOp3');
                                        eraseCookie('fechaDesdeIngresadaOp3');
                                        eraseCookie('fechaHastaIngresadaOp3');
                                        eraseCookie('fechaUltimaCargaOp3');
                                        //valTipoReporte
                                        eraseCookie('ultTipoReporteOp3');

                                        createCookie('distritoElegidoOp3', valDistritoElegido, 2);
                                        createCookie('fechaDesdeIngresadaOp3', valFechaDesdeIngresada, 2);
                                        createCookie('fechaHastaIngresadaOp3', valFechaHastaIngresada, 2);
                                        createCookie('fechaUltimaCargaOp3', valFechaUltimaCarga, 2);
                                        createCookie('ultTipoReporteOp3', valTipoReporte, 2);

                                    }

                                    cargaranalisispredefinido(BASE_URL + 'AnalisisPredefinido/GetReporteZonasLavado/21', 21);
                                    $("#lblModalDomicilio").text("Planificación de zonas de lavado");
                                    break;
                                case "8":
                                    //SI CARGA DESDE ARCHIVO. LOGICA VA AQUI
                                    var archivoSubido = $("#listCargaTecnica").val();
                                    // alert(archivoSubido);
                                    if (archivoSubido != "") {
                                        //SI CARGA DESDE ARCHIVO, NO INTERESA EL RESTO DEL CODIGO
                                        //“Cargado desde archivo”
                                        //“Fecha de carga: “ + FECHA_ALTA
                                        var valTipoReporte = tipoReporte;
                                        var valFechaUltimaCargaDdeArchivo = new Date();
                                        var dd = valFechaUltimaCargaDdeArchivo.getDate();
                                        var mm = valFechaUltimaCargaDdeArchivo.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCargaDdeArchivo.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCargaDdeArchivo = mm + '/' + dd + '/' + yyyy;

                                        eraseCookie('ultCargaDdeArchivoOp8');
                                        eraseCookie('ultFechaCargaDdeArchivoOp8');
                                        eraseCookie('ultTipoReporteDdeArchivoOp8');
                                        createCookie('ultCargaDdeArchivoOp8', 'TieneArchivo', 2);
                                        createCookie('ultFechaCargaDdeArchivoOp8', valFechaUltimaCargaDdeArchivo, 2);
                                        // AGREGAR COOKIE PARA LLEVARME EL TIPO DE REPORTE
                                        createCookie('ultTipoReporteDdeArchivoOp8', valTipoReporte, 2);

                                    }
                                    else {
                                        //alert('REPORTE Falta de agua y presión. Elija Reporte o Mapa y haga click en Procesar');
                                        var valDistritoElegido = $("#cboCargasDistritos").select2('data').text;
                                        var valFechaDesdeIngresada = $("#FechaDesde").val();
                                        var valFechaHastaIngresada = $("#FechaHasta").val();
                                        var valTipoReporte = tipoReporte;

                                        var valFechaUltimaCarga = new Date();
                                        var dd = valFechaUltimaCarga.getDate();
                                        var mm = valFechaUltimaCarga.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCarga.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCarga = mm + '/' + dd + '/' + yyyy;
                                        eraseCookie('ultCargaDdeArchivoOp8');
                                        eraseCookie('distritoElegidoOp8');
                                        eraseCookie('fechaDesdeIngresadaOp8');
                                        eraseCookie('fechaHastaIngresadaOp8');
                                        eraseCookie('fechaUltimaCargaOp8');
                                        //valTipoReporte
                                        eraseCookie('ultTipoReporteOp8');

                                        createCookie('distritoElegidoOp8', valDistritoElegido, 2);
                                        createCookie('fechaDesdeIngresadaOp8', valFechaDesdeIngresada, 2);
                                        createCookie('fechaHastaIngresadaOp8', valFechaHastaIngresada, 2);
                                        createCookie('fechaUltimaCargaOp8', valFechaUltimaCarga, 2);
                                        createCookie('ultTipoReporteOp8', valTipoReporte, 2);
                                    }

                                    cargaranalisispredefinido(BASE_URL + 'AnalisisPredefinido/GetReporteFaltaAguaPresion/22', 22);
                                    $("#lblModalDomicilio").text("Falta de agua y presión");
                                    break;
                                case "9":
                                    //SI CARGA DESDE ARCHIVO. LOGICA VA AQUI
                                    var archivoSubido = $("#listCargaTecnica").val();
                                    // alert(archivoSubido);
                                    if (archivoSubido != "") {
                                        //SI CARGA DESDE ARCHIVO, NO INTERESA EL RESTO DEL CODIGO
                                        //“Cargado desde archivo”
                                        //“Fecha de carga: “ + FECHA_ALTA
                                        var valTipoReporte = tipoReporte;
                                        var valFechaUltimaCargaDdeArchivo = new Date();
                                        var dd = valFechaUltimaCargaDdeArchivo.getDate();
                                        var mm = valFechaUltimaCargaDdeArchivo.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCargaDdeArchivo.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCargaDdeArchivo = mm + '/' + dd + '/' + yyyy;

                                        eraseCookie('ultCargaDdeArchivoOp9');
                                        eraseCookie('ultFechaCargaDdeArchivoOp9');
                                        eraseCookie('ultTipoReporteDdeArchivoOp9');
                                        createCookie('ultCargaDdeArchivoOp9', 'TieneArchivo', 2);
                                        createCookie('ultFechaCargaDdeArchivoOp9', valFechaUltimaCargaDdeArchivo, 2);
                                        // AGREGAR COOKIE PARA LLEVARME EL TIPO DE REPORTE
                                        createCookie('ultTipoReporteDdeArchivoOp9', valTipoReporte, 2);

                                    }
                                    else {
                                        //alert('REPORTE Analisis Libre. Elija Reporte o Mapa y haga click en Procesar');
                                        var valDistritoElegido = $("#cboCargasDistritos").select2('data').text;
                                        var valFechaDesdeIngresada = $("#FechaDesde").val();
                                        var valFechaHastaIngresada = $("#FechaHasta").val();
                                        var valTipoReporte = tipoReporte;

                                        var valFechaUltimaCarga = new Date();
                                        var dd = valFechaUltimaCarga.getDate();
                                        var mm = valFechaUltimaCarga.getMonth() + 1; //January is 0!
                                        var yyyy = valFechaUltimaCarga.getFullYear();

                                        if (dd < 10) {
                                            dd = '0' + dd
                                        }

                                        if (mm < 10) {
                                            mm = '0' + mm
                                        }

                                        valFechaUltimaCarga = mm + '/' + dd + '/' + yyyy;
                                        eraseCookie('ultCargaDdeArchivoOp9');
                                        eraseCookie('distritoElegidoOp9');
                                        eraseCookie('fechaDesdeIngresadaOp9');
                                        eraseCookie('fechaHastaIngresadaOp9');
                                        eraseCookie('fechaUltimaCargaOp9');
                                        //valTipoReporte
                                        eraseCookie('ultTipoReporteOp9');

                                        createCookie('distritoElegidoOp9', valDistritoElegido, 2);
                                        createCookie('fechaDesdeIngresadaOp9', valFechaDesdeIngresada, 2);
                                        createCookie('fechaHastaIngresadaOp9', valFechaHastaIngresada, 2);
                                        createCookie('fechaUltimaCargaOp9', valFechaUltimaCarga, 2);
                                        createCookie('ultTipoReporteOp9', valTipoReporte, 2);
                                    }

                                    cargaranalisispredefinido(BASE_URL + 'AnalisisPredefinido/GetReporteAnalisisLibre/23', 23);
                                    $("#lblModalDomicilio").text("Análisis libre");
                                    break;
                            };
                        });

                    } //FIN VALIDACION DISTRITOVACIO
                    else {
                        hideLoading();

                        //         var hayFDesde = fechaDesdeValor;
                        //          var hayFHasta = fechaHastaValor;
                        if (hayArchivo == '' || hayDistrito == 'Seleccione Distrito' || fechaDesdeValor == "" || fechaHastaValor == "") {
                            alert('OCURRIO UN ERROR: Debe Seleccionar un Distrito y cargar un rango de fechas o ingresar un Archivo CSV.');
                        }
                    }
                }
                else {
                    if (tipoReporte == 2 || tipoReporte == 4 || tipoReporte == 5 || tipoReporte == 6 || tipoReporte == 7) {
                        //alert('Tipo de Reporte: ' + tipoReporte);
                        //TO DO: ComportamientoOpc_2_4_5_6_7_9()
                        //alert('Pantallas para opciones 2, 4, 5, 6 y 7');
                        //alert('RUEDITA: CARGANDO LOS DATOS.');
                        //alert('CARTELITO: Se cargaron 20 reclamos');
                        $.ajax({
                            url: BASE_URL + "analisispredefinido/CargaDeReclamos?analisisID=" + IdAnalisis + "&distritoElegidoID=" + valDistritoElegidoID + "&cargaTipo=" + tipoCarga + "&fechaDesdeInput=" + fechaDesdeValor + "&fechaHastaInput=" + fechaHastaValor + "&usuarioID=" + usuarioAlta + "&fechaDeAlta=" + fechaActualValor,
                            type: "POST",
                            success: function (data) {
                                setTimeout(function () {
                                    hideLoading();
                                    modalConfirm("Análisis Técnicos", "Se cargaron 20 reclamos ", "delete-analisispredefinido");
                                }, 5000);
                            }
                        });
                        //RANC
                        //Redes de agua
                        //Redes de cloaca
                        //Conexiones de agua
                        //Conexiones de cloaca
                        //Análisis libre
                        $("#btnAdvertenciaOKanalisispredefinido").click(function () {
                            //Cierro el mensaje y el Modal Analisis Predefinido
                            //alert('Carga de Reclamos por Analisis Libre. Finalizada.');
                            // alert(tipoReporte);
                        });
                        LimpiarControles();
                    }
                }
            }
        }


        // }
    });

    //BOTON PROCESAR
    $("#btnProcesarTipoReporte").click(function () {
        var id2 = 20;
        //alert('RUEDITA: PROCESANDO LOS DATOS.');
        showLoading();

        $.ajax({
            url: BASE_URL + "analisispredefinido/ProcesarTipoReporte/" + id2,
            type: "POST",
            success: function (data) {

                setTimeout(function () {
                    hideLoading();

                    //modalConfirm("Reportes y Mapas - Rastreo sistemático", "Proceso finalizado.", "delete-analisispredefinido");
                    alert('Proceso Finalizado.');
                }, 5000);
            }
        });

        //$("#btnAdvertenciaOKanalisispredefinido").click(function () {
        //alert('Procesar Tipo Reporte finalizado');
        //});
    });


    $("#analisispredefinido-copy").click(function () {
        if (selectedRowanalisispredefinidos) {
            var id = selectedRowanalisispredefinidos.find("td").eq(0).html();
            var nombre = selectedRowanalisispredefinidos.find("td").eq(1).html();

            $("#btnAdvertenciaOKanalisispredefinido").click(function () {
                if ($("#TipoAdvertenciaanalisispredefinido").val() === "copy-analisispredefinido") {
                    $.ajax({
                        url: BASE_URL + "analisispredefinido/Copy/" + id,
                        type: "POST",
                        success: function (data) {
                            analisispredefinidosReload();
                        }
                    });
                }
            });

            modalConfirm("Copiar - analisispredefinido", "¿Desea copiar la analisispredefinido " + nombre + "?", "copy-analisispredefinido");
        }
    });
}

function LimpiarControles() {
    //CLEAR CARGA DDE SISTEM
    $("#cboCargasDistritosFiltros").select2("val", 0);
    $("#cboCargasDistritos").select2("val", 0);
    $("#FechaDesde").datepicker("clearDates");
    $("#FechaHasta").datepicker("clearDates");
    //=====

    //CLEAR ARCHIVO
    $(".file").fileinput("clear");
    //=====

    //CLEAR ITEM SELEC EN COLECCION
    $("#grLisColecciones li").each(function () {
        $(this).removeClass("selected-option-light-blue");
    });
}


function analisispredefinidoRevalidations() {
    //Revalidations
    $("#Pdf").fileinput().on("change", function () {
        $("#analisispredefinido-form").data("bootstrapValidator")
            .updateStatus("Pdf", "NOT_VALIDATED", null)
            .validateField("Pdf");
    });

    $(".fileinput-remove-button", "#analisispredefinido-form").click(function () {
        $("#analisispredefinido-form").data("bootstrapValidator")
            .updateStatus("Pdf", "INVALID", null)
            .validateField("Pdf");
    });
}

function analisispredefinidosReload() {
    $("#tabla-analisispredefinidos").dataTable().api().ajax.reload(function () {

    });
    $("#analisispredefinido-partial").load(BASE_URL + "analisispredefinido/Partial", function () {
        initPloteo();
    });
}

function enableFormContent(formId, enable) {
    var formElements = $(formId + " :input");
    if (enable) {
        $.each(formElements, function (index, element) {
            $(element).enable(true);
        });
    } else {
        $.each(formElements, function (index, element) {
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

function dameFechaActual() {
    var valFechaActual = new Date();
    var dd = valFechaActual.getDate();
    var mm = valFechaActual.getMonth() + 1; //January is 0!
    var yyyy = valFechaActual.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    valFechaActual = mm + '/' + dd + '/' + yyyy;

    return valFechaActual;
}

function layerTab(insert) {
    $("#analisispredefinido-layer-form").formValidation("resetForm", true);

    $("#layer-insert").hide();
    $("#layer-edit").hide();
    $("#layer-delete").hide();

    var idanalisispredefinido = 0;
    if (!insert && selectedRowanalisispredefinidos)
        idanalisispredefinido = selectedRowanalisispredefinidos.find("td").eq(0).html();

    destroyDataTable("tabla-layers");

    $("#tabla-layers tbody").load(BASE_URL + "Layer/List/" + idanalisispredefinido, function () {
        createDataTable("tabla-layers");

        $("#layer-edit").addClass("boton-deshabilitado");
        $("#layer-delete").addClass("boton-deshabilitado");

        $("#layers-panel-body").show();
        $("#layer-panel-body").hide();
    });
}

function layerValidator() {
    //Revalidate
    $("#layer-etiqueta-color").change(function () {
        $("#analisispredefinido-layer-form").data("bootstrapValidator")
            .updateStatus("EtiquetaColor", "NOT_VALIDATED", null)
            .validateField("EtiquetaColor");
    });

    $("#layer-contorno-color").change(function () {
        $("#analisispredefinido-layer-form").data("bootstrapValidator")
            .updateStatus("ContornoColor", "NOT_VALIDATED", null)
            .validateField("ContornoColor");
    });

    $("#layer-relleno-color").change(function () {
        $("#analisispredefinido-layer-form").data("bootstrapValidator")
            .updateStatus("RellenoColor", "NOT_VALIDATED", null)
            .validateField("RellenoColor");
    });

    $("#analisispredefinido-layer-form").bootstrapValidator({
        framework: "bootstrap",
        excluded: [":disabled"],
        fields: {
            Nombre: {
                validators: {
                    notEmpty: {
                        message: "El Nombre es obligatorio"
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
                            var title = $(".file-caption-name", "#analisispredefinido-layer-form").attr("title");
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
            }
        }
    });

    $("#layer-submit").click(function () {

        var form = $("#analisispredefinido-layer-form");
        form.formValidation("revalidateField", "ImagenPunto");

        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {
            form.submit();
        }
    });

    $("#layer-cancel").click(function () {
        if (selectedRowLayers) {
            selectedRowLayers.removeClass("selected");
            selectedRowLayers = null;
        }

        $("#analisispredefinido-layer-form").formValidation("resetForm", true);

        $("#layers-panel-body").show();

        layerEnableControls(false);
    });

    $("input[name=PuntoRepresentacion]", "#analisispredefinido-layer-form").change(function () {
        var form = $("#analisispredefinido-layer-form");
        var option = $(this).val();
        switch (option) {
            case "0":
                form.data("bootstrapValidator")
                    .updateStatus("ImagenPunto", "VALID", null)
                    .validateField("ImagenPunto")
                    .updateStatus("PuntoAlto", "VALID", null)
                    .validateField("PuntoAlto")
                    .updateStatus("PuntoAncho", "VALID", null)
                    .validateField("PuntoAncho");
                $("#PuntoAlto").val("");
                $("#PuntoAncho").val("");
                $("input[name=PuntoPredeterminado]").enable(false);
                $("#ImagenPunto").fileinput("clear");
                $("#ImagenPunto").fileinput("disable");
                $("#PuntoAlto").enable(false);
                $("#PuntoAncho").enable(false);
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
                form.data("bootstrapValidator")
                    .updateStatus("PuntoAlto", "NOT_VALIDATED", null)
                    .validateField("PuntoAlto")
                    .updateStatus("PuntoAncho", "NOT_VALIDATED", null)
                    .validateField("PuntoAncho");

                break;
            case "2":
                $("input[name=PuntoPredeterminado]").enable(false);
                $("#ImagenPunto").fileinput("enable");
                $("#PuntoAlto").enable(true);
                $("#PuntoAncho").enable(true);
                form.data("bootstrapValidator")
                    .updateStatus("ImagenPunto", "NOT_VALIDATED", null)
                    .validateField("ImagenPunto")
                    .updateStatus("PuntoAlto", "NOT_VALIDATED", null)
                    .validateField("PuntoAlto")
                    .updateStatus("PuntoAncho", "NOT_VALIDATED", null)
                    .validateField("PuntoAncho");
                break;
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

            $("#analisispredefinido-layer-form").data("formValidation").resetField("layer-contorno-color");
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
    $("#analisispredefinido-layer-form").load(BASE_URL + "Layer/FormContent", function () {

        initCtrlsPluggins("#analisispredefinido-layer-form");

        layerValidator();

        $("#layer-panel-body").show();
    });

    //Init once only

    $("#layer-edit").click(function () {
        if (selectedRowLayers) {
            enableFormContent("#analisispredefinido-layer-form", true);
            $("#ImagenPunto").fileinput("enable");
            initLayerBehaviour(true);

            if ($("input[name=PuntoRepresentacion]:checked").val() === "0") {
                $("input[name=PuntoPredeterminado]").enable(false);
                $("#ImagenPunto").fileinput("disable");
                $("#PuntoAlto").enable(false);
                $("#PuntoAncho").enable(false);
            } else if ($("input[name=PuntoRepresentacion]:checked").val() === "1") {
                $("input[name=PuntoPredeterminado]").enable(true);
                $("#ImagenPunto").fileinput("disable");
                $("#PuntoAlto").enable(true);
                $("#PuntoAncho").enable(true);
            } else {
                $("input[name=PuntoPredeterminado]").enable(true);
                $("#ImagenPunto").fileinput("enable");
                $("#PuntoAlto").enable(true);
                $("#PuntoAncho").enable(true);
            }

            $("#layers-panel-body").hide();
            $("#layer-cancel").removeClass("hidden");
            $("#layer-submit").removeClass("hidden");

            $("#analisispredefinido-panel-body").hide();
            accordionTabHide($("#collapse-analisispredefinido"));
            accordionTabHide($("#collapse-textos"));
            accordionTabHide($("#collapse-escalas"));

            layerRevalidations();
        }
    });

    $("#layer-delete").click(function () {
        if (selectedRowLayers) {
            var id = selectedRowLayers.find("td").eq(0).html();
            var nombre = selectedRowLayers.find("td").eq(1).html();

            $("#btnAdvertenciaOKanalisispredefinido").click(function () {
                if ($("#TipoAdvertenciaanalisispredefinido").val() === "delete-layer")
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
        $("#analisispredefinido-layer-form").data("bootstrapValidator")
            .updateStatus("ImagenPunto", "NOT_VALIDATED", null)
            .validateField("ImagenPunto");
    });

    $(".fileinput-remove-button", "#analisispredefinido-layer-form").click(function () {
        $("#analisispredefinido-layer-form").data("bootstrapValidator")
            .updateStatus("ImagenPunto", "INVALID", null)
            .validateField("ImagenPunto");
    });
}

function textoTab(insert) {
    $("#analisispredefinido-texto-form").formValidation("resetForm", true);

    $("#texto-insert").hide();
    $("#texto-edit").hide();
    $("#texto-delete").hide();

    var idanalisispredefinido = 0;
    if (!insert && selectedRowanalisispredefinidos)
        idanalisispredefinido = selectedRowanalisispredefinidos.find("td").eq(0).html();

    destroyDataTable("tabla-textos");

    $("#tabla-textos tbody").load(BASE_URL + "analisispredefinidoTexto/List/" + idanalisispredefinido, function () {
        createDataTable("tabla-textos");

        $("#texto-edit").addClass("boton-deshabilitado");
        $("#texto-delete").addClass("boton-deshabilitado");

        $("#textos-panel-body").show();
        $("#texto-panel-body").hide();
    });
}

function textoValidator() {
    $("#analisispredefinido-texto-form").bootstrapValidator({
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

    $("#analisispredefinido-texto-submit").click(function () {
        var bootstrapValidator = $("#analisispredefinido-texto-form").data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {
            $("#analisispredefinido-texto-form").submit();
        }
    });

    $("#analisispredefinido-texto-cancel").click(function () {
        if (selectedRowTextos) {
            selectedRowTextos.removeClass("selected");
            selectedRowTextos = null;
        }

        $("#analisispredefinido-texto-form").formValidation("resetForm", true);

        $("#textos-panel-body").show();

        textoEnableControls(false);
    });

    $("input[name=Tipo]", "#analisispredefinido-texto-form").change(function () {
        var form = $("#analisispredefinido-texto-form");
        var option = $(this).val();
        switch (option) {
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
        $("#analisispredefinido-texto-form").data("bootstrapValidator")
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

        $("#analisispredefinido-texto-cancel").addClass("hidden");
        $("#analisispredefinido-texto-submit").addClass("hidden");
    }
}

function textoFormContent() {
    $("#analisispredefinido-texto-form").load(BASE_URL + "analisispredefinidoTexto/FormContent", function () {
        initCtrlsPluggins("#analisispredefinido-texto-form");
        textoValidator();
    });

    // Init once only

    $("#texto-edit").click(function () {
        if (selectedRowTextos) {
            enableFormContent("#analisispredefinido-texto-form", true);

            if ($("input[name=Tipo]:checked").val() === "3") {
                $("#Origen").enable(false);
                $("#AtributoId").enable(true);
            } else {
                $("#Origen").enable(true);
                $("#AtributoId").enable(false);
            }

            $("#textos-panel-body").hide();
            $("#analisispredefinido-texto-cancel").removeClass("hidden");
            $("#analisispredefinido-texto-submit").removeClass("hidden");

            $("#analisispredefinido-panel-body").hide();
            accordionTabHide($("#collapse-analisispredefinido"));
            accordionTabHide($("#collapse-layers"));
            accordionTabHide($("#collapse-escalas"));
        }
    });

    $("#texto-delete").click(function () {
        if (selectedRowTextos) {
            var id = selectedRowTextos.find("td").eq(0).html();
            var tipo = selectedRowTextos.find("td").eq(1).html();
            var origen = selectedRowTextos.find("td").eq(2).html();

            $("#btnAdvertenciaOKanalisispredefinido").click(function () {
                if ($("#TipoAdvertenciaanalisispredefinido").val() === "delete-texto")
                    $.ajax({
                        url: BASE_URL + "analisispredefinidoTexto/Delete/" + id,
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

    $("#analisispredefinido-escala-form").formValidation("resetForm", true);

    $("#escala-insert").hide();
    $("#escala-edit").hide();
    $("#escala-delete").hide();

    var idanalisispredefinido = 0;
    if (!insert && selectedRowanalisispredefinidos)
        idanalisispredefinido = selectedRowanalisispredefinidos.find("td").eq(0).html();

    destroyDataTable("tabla-escalas");

    $("#tabla-escalas tbody").load(BASE_URL + "analisispredefinidoEscala/List/" + idanalisispredefinido, function () {
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
    $("#analisispredefinido-escala-form").bootstrapValidator({
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
        var bootstrapValidator = $("#analisispredefinido-escala-form").data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {
            $("#analisispredefinido-escala-form").submit();
        }
    });

    $("#escala-cancel").click(function () {
        if (selectedRowEscalas) {
            selectedRowEscalas.removeClass("selected");
            selectedRowEscalas = null;
        }

        $("#analisispredefinido-escala-form").formValidation("resetForm", true);

        $("#escalas-panel-body").show();

        escalaEnableControls(false);
    });
}

function escalaFormContent() {
    $("#analisispredefinido-escala-form").load(BASE_URL + "analisispredefinidoEscala/FormContent", function () {
        enableFormContent("#analisispredefinido-escala-form", false);
        escalaValidator();
    });

    //Init once only

    $("#escala-edit").click(function () {
        if (selectedRowEscalas) {
            enableFormContent("#analisispredefinido-escala-form", true);

            $("#escalas-panel-body").hide();
            $("#escala-cancel").removeClass("hidden");
            $("#escala-submit").removeClass("hidden");

            $("#analisispredefinido-panel-body").hide();
            accordionTabHide($("#collapse-analisispredefinido"));
            accordionTabHide($("#collapse-layers"));
            accordionTabHide($("#collapse-textos"));
        }
    });

    $("#escala-delete").click(function () {
        if (selectedRowEscalas) {
            var id = selectedRowEscalas.find("td").eq(0).html();
            var escala = selectedRowEscalas.find("td").eq(1).html();

            $("#btnAdvertenciaOKanalisispredefinido").click(function () {
                if ($("#TipoAdvertenciaanalisispredefinido").val() === "delete-escala")
                    $.ajax({
                        url: BASE_URL + "analisispredefinidoEscala/Delete/" + id,
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
    $("#TipoAdvertenciaanalisispredefinido").val(tipoAdvertencia);
    $("#TituloAdvertenciaanalisispredefinido").text(title);
    $("#DescripcionAdvertenciaanalisispredefinido").text(message);
    $("#confirmModalanalisispredefinido").modal("show");
}


function destroyDataTable(tableId) {
    var dataTable = $("#" + tableId).dataTable();
    dataTable.api().destroy();
}

function createDataTable(tableId) {
    $("#" + tableId).DataTable({
        dom: "rt",
        scrollY: "150px",
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        }
    });
}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

function createDataTableanalisispredefinido() {
    $("#tabla-analisispredefinidos").dataTable({
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
            "url": BASE_URL + "analisispredefinido/List",
            "type": "POST"
        },
        //"serverSide": true,
        //"deferLoading": 0,
        "columns": [
            { "data": "Idanalisispredefinido", "className": "hide" },
            { "data": "Nombre" },
            { "data": "Hoja" },
            { "data": "Componente" }
        ]
    });
}
//@ sourceURL=adminanalisispredefinidos.js