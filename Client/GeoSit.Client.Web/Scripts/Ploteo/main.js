var listaPlantillas;
$(document).ready(init);
$(window).resize(ajustarmodal);
$('#ploteadorModal').on('shown.bs.modal', function (e) {
    ajustarLeftScrollBar();
    ajustarRightScrollBar();
    hideLoading();
});

function init() {
    ///////////////////// Scrollbars ////////////////////////
    $(".left-scrollable-panel").niceScroll(getNiceScrollConfig());
    $('.left-scrollable-panel .panel-heading').click(function () {
        setTimeout(function () {
            ajustarLeftScrollBar();
        }, 10);
    });

    ////////////////////////////////////////////////////////
    ajustarmodal();
    //TODO: PONER LA LLAMADA A CARGAR PLANTILLAS AQUI!
    MarcarColeccionSeleccionada();

    $("#ploteadorModal").modal("show");
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 200,
        altura = viewportHeight - headerFooter;
    $(".ploteador-body", ".left-scrollable-panel").css({ "height": altura, "overflow": "hidden" });
    $(".ploteador-body").css({ "height": altura });
    ajustarLeftScrollBar();
    ajustarRightScrollBar();
}

function ajustarLeftScrollBar() {
    temp = $(".ploteador-body").height();
    $('.left-scrollable-panel').css({ "max-height": temp + 'px' });
    $('#accordion-ploteador').css({ "max-height": temp + 1 + 'px' });
    $(".left-scrollable-panel").getNiceScroll().resize();
    $(".left-scrollable-panel").getNiceScroll().show();
}

function ajustarRightScrollBar() {
    setTimeout(function () {
        temp = $(".ploteador-body").height();
        $('.right-scrollable-panel').css({ "max-height": temp + 'px' });
        $('#configuracion-ploteo').css({ "height": Math.min(temp, $(".detalle-body").height()) || temp + 'px' });
        $(".right-scrollable-panel").getNiceScroll().resize();
        $(".right-scrollable-panel").getNiceScroll().show();
    }, 10);
}

function cargarPlantilla(url, id, idTipo) {
    //MarcarColeccionSeleccionada();
    showLoading();
    $("#btnGenerar").attr('disabled', true);
    $("#configuracion-ploteo").load(url, function (response, status, xhr) {
        if (status === "success") {
            $("#btnGenerar").off("click");
            $("#btnGenerar").attr('disabled', true);

            var idFormulario = $("#configuracion-ploteo form").attr("id");
            cargarTextosVariables();
            buscarComponente();
            checkAllAgregar();
            checkAllEliminar();
            Agregar();
            borrarObjetos();
            CallGenerar();
            mostrarColeccionGral();
            VisualizacionGeneral();
            mostrarColeccion();
            mostrarListaManzanas();
            mostrarListaExpte();
            initCtrlsPluggins(idFormulario);
            ComposicionColeccionA4();
            ComposicionColeccionGeneral();
            ComposicionColeccionObra();
            if (idTipo != 13)//Corregir cambiando el id del componente
                ManzanaZonaResultado();
            else if (idTipo == 13)
                ObrasZonaResultado();
            BuscarObjetosConTeclaEnter();
            initScroll();
            ajustarRightScrollBar();
        }
        hideLoading();
    });

}

//CARGA DE LA COLUMNA DERECHA

function initScroll() {
    $(".right-scrollable-panel").niceScroll(getNiceScrollConfig());
    $('.right-scrollable-panel input[type=radio]').click(function () {
        ajustarRightScrollBar();
    });
    $('.right-scrollable-panel  .panel-heading').click(function () {
        ajustarRightScrollBar();
    });
}


//COMPORTAMIENTO PREDEFINIDOS//

function BuscarObjetosConTeclaEnter() {
    $('#filtro').keypress(function (e) {
        if (e.keyCode == 13)
            $('#buscarComponente').click();
    });
}


function MarcarColeccionSeleccionada() {
    $('#grLisColecciones li').on('click', function () {

        $("#grLisColecciones li").each(function () {
            $(this).removeClass("selected-option-light-blue");
        });

        $(this).addClass("selected-option-light-blue");

    });
}

function buscarComponente() {

    $("#buscarComponente").click(function () {
        $("#listCheckAgregar").empty();
        var idComponente = $("#cboComponentes").val();
        var componente = $("#cboComponentes option:selected").attr('data-doctype');
        var filtroVal = $("#filtro").val();
        $.ajax({
            url: BASE_URL + "Search/ByType",
            type: "POST",
            data: {
                tipo: componente,
                filtro: filtroVal,
                idComponente: idComponente
            },
            success: function (data) {
                var json = $.parseJSON(data),
                    values = [];
                json.response.docs.map(function (doc) {
                    //doc.id ES SOLO PARA AYSA
                    //PARA OTROS URL COMO GSTW, ES doc.gid
                    //HABRA QUE REVER DE HOMOGENEIZAR LOS ESQUEMAS DE LOS DISTINTOS CORE.
                    values.push({ id: doc.id, descripcion: doc.nombre });
                });
                //inserto los datos recuperados en la busqueda
                $.each(values, function (i) {
                    var idItem = values[i].id;
                    var descripcionItem = values[i].descripcion;
                    //agrego el div con el checkbox, label y span
                    $("#listCheckAgregar").append('<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">\
                                                        <label class="cursor-pointer">\
                                                            <input style=" margin-top: 2px" class="faChkSqr" id="' + idItem + '" type="checkbox" >\
                                                                <span id="span_' + idItem + '">' + descripcionItem + '</span>\
                                                        </label>\
                                                    </div>');
                });
            },
            error: function (error) {
                console.debug(error);
            }
        });
    });
}

function checkAllAgregar() {
    $("#checkGralAgregar").click(function () {
        if ($("#checkGralAgregar").is(':checked')) {
            $("#listCheckAgregar input[type=checkbox]").each(function () {
                $(this).prop("checked", true);
            });
        } else {
            $("#listCheckAgregar input[type=checkbox]").each(function () {
                $(this).prop("checked", false);
            });
        }
    });
}


function checkAllEliminar() {
    $("#checkGralEliminar").click(function () {
        if ($("#checkGralEliminar").is(':checked')) {
            $("#agregados input[type=checkbox]").each(function () {
                $(this).prop("checked", true);
            });
        } else {
            $("#agregados input[type=checkbox]").each(function () {
                $(this).prop("checked", false);
            });
        }
    });
}

function Agregar() {
    $("#btn_Agregar").on("click", function () {

        $("div#listCheckAgregar input[type=checkbox]").each(function () {
            var selected = [];
            if ($(this).is(":checked")) {
                selected.push($(this).attr("id"));

                if (!$("#agregados").find("#" + selected).length) {
                    addCheckbox($(this).attr("id"), $("#span_" + $(this).attr("id")).text());
                    $(this).prop("checked", false);
                    $("#checkGralAgregar").prop("checked", false);
                    selected = [];
                }
            }
        });


    });
}

function addCheckbox(id, label) {
    $("#agregados").append('<div class="agregado col-lg-12 col-md-12 col-sm-12 col-xs-12">\
                                <label class="cursor-pointer">\
                                    <input style=" margin-top: 2px" class="faChkSqr" id="' + id + '" type="checkbox" onchange=\'cleanCheckGralAgregados();\' >\
                                        <span id="span_' + id + '">' + label + '</span>\
                                </label>\
                            </div>');
}

function cleanCheckGralAgregados() {
    $("#checkGralEliminar").prop("checked", false);
}

function borrarObjetos() {

    $("#btn_Eliminar").on("click", function () {
        $("#checkGralEliminar").prop("checked", false);
        $("#agregados input:checked").parents("div.agregado").remove();
    });
}


//COMPORTAMIENTO PLANCHETA A4//

var componentesSeleccionados = {};

function ManzanaZonaResultado() {
    $("#btnGenerar").attr('disabled', false);
    $('#radioObjetos').attr('checked', true);
    $('#ordManzana').prop('checked', true);

    //DESHABILITADAS
    //ordExpte
    $('#ordExpte').prop('checked', false);
    $("#ordExpte").attr("disabled", "disabled");

    //ordOrigen
    $('#ordOrigen').prop('checked', false);
    $("#ordOrigen").attr("disabled", "disabled");

    $("#grabarListado").removeAttr("disabled", "disabled");
    $("#grabarListado").prop("checked", false);

    //Revisar elementos seleccionados
    componentesSeleccionados = [];
    $.each($(".results [data-componente-ploteable='true']:checked"), function () {
        var el = $(this);
        var componenteSeleccionado = $(this).data("componente");
        $.each(ComponentesPloteables, function (i, componente) {
            if (componente.DocType == componenteSeleccionado) {
                componentesSeleccionados.push({
                    Id: el.data('name'),
                    Componente: componenteSeleccionado,
                    ComponenteDisplay: componente.Nombre
                });
            }
        });

    });

    var soloManzanas = true;
    $.each(componentesSeleccionados, function (i, val) {
        if (val.Componente !== "manzanas") {
            soloManzanas = false;
        }
    });

    $("#manzanasDupliacadas").prop("checked", !soloManzanas);
    if (soloManzanas)
        habilitarOpcionesSoloManzanas();
    else
        habilitarOpcionesMultiplesComponentes();

    dibujarBadges();

    //ONCHANGE
    $("#radioObjetos").on("change", function () {

        $("#btnGenerar").attr('disabled', false);
        $("#divColeccion").addClass("hide");
        $("#divManzanas").addClass("hide");
        $("#divExpte").addClass("hide");

        //ordenamiento
        var soloManzanas = true;
        $.each(componentesSeleccionados, function (i, val) {
            if (val.Componente !== "manzanas") {
                soloManzanas = false;
            }
        });
        $("#manzanasDupliacadas").prop("checked", !soloManzanas);
        if (soloManzanas)
            habilitarOpcionesSoloManzanas();
        else
            habilitarOpcionesMultiplesComponentes();

        $("#cotas").prop("checked", false);

        $(".file").fileinput("clear");
    });
}

function ObrasZonaResultado() {
    $('#radioObjetos').attr('checked', true);
    $("#btnGenerar").attr('disabled', false);

    var idComponentePrincipal = $("#idComponentePrincipal").val();
    //$.each($(".results [data-componente-ploteable='true']:checked"), function () {
    componentesSeleccionados = [];
    $.each($(".results :checked"), function () {
        var el = $(this);
        var componenteSeleccionado = $(this).data("componente");
        $.each(ComponentesPloteables, function (i, componente) {
            if (componente.DocType == componenteSeleccionado) {
                componentesSeleccionados.push({
                    Id: el.data('name'),
                    Componente: componenteSeleccionado,
                    ComponenteDisplay: componente.Nombre
                });
            }
        });

    });

    dibujarBadges(true);

    $("#radioObjetos").on("change", function () {
        $("#btnGenerar").attr('disabled', false);
        $("#divColeccion").addClass("hide");
        $("#divManzanas").addClass("hide");
        $("#divExpte").addClass("hide");

        //Coleccion
        $("#cboColeccionesObras").select2("val", 0);

        //ordenamiento
        var soloManzanas = true;
        $.each(componentesSeleccionados, function (i, val) {
            if (val.Componente !== "manzanas") {
                soloManzanas = false;
            }
        });
        $("#manzanasDupliacadas").prop("checked", !soloManzanas);
        if (soloManzanas)
            habilitarOpcionesSoloManzanas();
        else
            habilitarOpcionesMultiplesComponentes();

        $("#cotas").prop("checked", false);

        $(".file").fileinput("clear");
    });
}

function getBadge(componentes, esObjeto) {

    var badge = $('<span>').addClass('badge');

    var componentesString = [];
    var total = 0;
    $.each(componentes, function (componente, cant) {
        componentesString.push(cant + " " + componente);
        total += cant;
    });
    if (esObjeto)
        badge.text(total + " " + (total != 1 ? "Objetos" : "Objeto"));
    else
        badge.text(total + " " + (total != 1 ? "Componentes" : "Componente"));

    if (total > 0) {

        badge.tooltip({
            title: componentesString.join(", "),
            container: 'body'
        });

    }

    return badge
}

function dibujarBadges(esObjeto) {

    var container = $('#objetosSeleccionados');
    container.empty();

    var componentes = {};
    $.each(componentesSeleccionados, function (i, val) {
        componentes[val.ComponenteDisplay] = componentes[val.ComponenteDisplay] != null ? componentes[val.ComponenteDisplay] : 0;
        componentes[val.ComponenteDisplay]++;
    });

    var badge = getBadge(componentes, esObjeto);

    container.append(badge);
}

function mostrarColeccion() {

    //COMPORTAMIENTO DEFAULT COLECCIONA4, NI BIEN SE CARGA PLANTILLAA4
    //DESHABILITAR BOTON GENERAR, SI NOY HAY COLECCION SELECCIONADA
    $("#btnGenerar").attr('disabled', true);

    //HABILITAR ORD MANZANA
    $("#lblManzana").removeClass("disabled");
    $('#ordManzana').removeAttr("disabled", "disabled");
    $('#ordManzana').prop('checked', true);

    //DESHABILITAR ORD EXP. Y ORD ORIGEN
    $("#lblExpte").addClass("disabled");
    $("#ordExpte").attr("disabled", "disabled");
    $('#ordExpte').prop('checked', false);

    $("#lblOrigen").addClass("disabled");
    $("#ordOrigen").attr("disabled", "disabled");
    $('#ordOrigen').prop('checked', false);

    //DESHABILITAR CHECKBOX MANZANA DUPLICADA
    $("#manzanasDupliacadas").prop("checked", false);
    $("#manzanasDupliacadas").attr("disabled", "disabled");

    $("#grabarListado").prop("checked", false);
    $("#grabarListado").removeAttr("disabled", "disabled");

    //ONCHANGE- COMPORTAMIENTO COLECCIONA4, AL VOLVER A LA OPCION COLECCION A4
    //DESDE ALGUNA DE LAS OTRAS TRES
    $("#radioColeccion").on("change", function () {

        //HABILITAR ORD MANZANA
        $("#lblManzana").removeClass("disabled");
        $('#ordManzana').removeAttr("disabled", "disabled");
        $('#ordManzana').prop('checked', true);

        $("#cboColeccionesA4").select2("val", 0)

        //VENGO DE OTRA OPCION
        $("#lblColeccionCompuesta").empty();
        //DESHABILITAR BOTON GENERAR, SI NO HAY COLECCION SELECCIONADA
        $("#btnGenerar").attr('disabled', true);

        //MOSTRAR COLECCION
        $("#divColeccion").removeClass("hide");

        //OCULAR LISTADO MANZANA Y LISTADO EXPEDIENTE
        $("#divManzanas").addClass("hide");
        $("#divExpte").addClass("hide");

        //DESHABILITAR ORD EXP. Y ORD ORIGEN
        $("#lblExpte").addClass("disabled");
        $("#ordExpte").attr("disabled", "disabled");
        $('#ordExpte').attr('checked', false);

        $("#lblOrigen").addClass("disabled");
        $("#ordOrigen").attr("disabled", "disabled");
        $('#ordOrigen').attr('checked', false);

        //DESHABILITAR CHECKBOX MANZANA DUPLICADA
        $("#manzanasDupliacadas").prop("checked", false);
        $("#manzanasDupliacadas").attr("disabled", "disabled");

        $("#cotas").prop("checked", false);
        $(".file").fileinput("clear");
        $("#grabarListado").prop("checked", false);
        $("#grabarListado").removeAttr("disabled", "disabled");

    });
}

function mostrarListaManzanas() {

    $("#cboColeccionesA4").select2("val", 0)
    //SI NO HAY ARCHIVO SELECCIONADO, DESHABILITAR BOTON GENERAR
    $("#btnGenerar").attr('disabled', true);
    //VERIFICAR SI YA ESTABA EL COMPORTAMIENTO DEL CUADRITO
    $("#manzanasDupliacadas").prop("checked", false);
    $("#manzanasDupliacadas").attr("disabled", "disabled");

    $("#grabarListado").prop("checked", false);
    $("#grabarListado").attr("disabled", "disabled");

    //ONCHANGE
    $("#radioListManzanas").on("change", function () {

        //$("#listManzanas").val("");
        //SI NO HAY ARCHIVO SELECCIONADO, DESHABILITAR BOTON GENERAR
        $("#btnGenerar").attr('disabled', true);

        $("#divManzanas").removeClass("hide");

        $("#divExpte").addClass("hide");
        $("#divColeccion").addClass("hide");

        //ordenamiento
        $("#manzanasDupliacadas").prop("checked", false);
        $("#manzanasDupliacadas").attr("disabled", "disabled");
        $("#lblOrigen").removeClass("disabled");
        $("#ordOrigen").removeAttr("disabled", "disabled");

        $("#lblExpte").addClass("disabled");
        $("#ordExpte").attr("disabled", "disabled");
        $('#ordExpte').prop('checked', false);

        $("#grabarListado").prop("checked", false);
        $("#grabarListado").attr("disabled", "disabled");
        $("#cotas").prop("checked", false);
        $(".file").fileinput("clear");

        $("#lblManzana").removeClass("disabled");
        $('#ordManzana').removeAttr("disabled", "disabled");
        $('#ordManzana').prop('checked', true);

    });

}

function mostrarListaExpte() {

    //LIMPIAR EL ARCHIVO SELECCIONADO, SI TENIA. VER SI HACE FALTA EN LA CARGA POR DEFAULT

    //SI NO HAY ARCHIVO SELECCIONADO, DESHABILITAR BOTON GENERAR
    $("#btnGenerar").attr('disabled', true);

    //HABILITAR CHECKBOX MANZANA DUPLICADA
    $("#manzanasDupliacadas").prop("checked", true);
    $("#manzanasDupliacadas").removeAttr("disabled", "disabled");

    //COMPORTAMIENTO DEFAULT ListaExpte, NI BIEN SE CARGA PLANTILLAA4
    //HABILITAR ORD MANZANA
    $("#lblManzana").removeClass("disabled");
    $('#ordManzana').removeAttr("disabled", "disabled");
    $('#ordManzana').attr('checked', true);

    //HABILITAR ORD EXP. Y ORD ORIGEN
    $("#lblExpte").removeClass("disabled");
    $('#ordExpte').removeAttr("disabled", "disabled");
    $('#ordExpte').attr('checked', true);

    $("#lblOrigen").removeClass("disabled");
    $('#ordOrigen').removeAttr("disabled", "disabled");
    $('#ordOrigen').attr('checked', true);

    $("#grabarListado").prop("checked", false);
    $("#grabarListado").attr("disabled", "disabled");

    //ONCHANGE- COMPORTAMIENTO COLECCIONA4, AL VOLVER A LA OPCION ListaExpte
    $("#radioListExpte").on("change", function () {

        //SI NO HAY ARCHIVO SELECCIONADO, DESHABILITAR BOTON GENERAR
        $("#btnGenerar").attr('disabled', true);

        $("#divExpte").removeClass("hide");

        $("#divManzanas").addClass("hide");
        $("#divColeccion").addClass("hide");

        //HABILITAR CHECKBOX MANZANA DUPLICADA
        $("#manzanasDupliacadas").prop("checked", true);
        $("#manzanasDupliacadas").removeAttr("disabled", "disabled");

        //COMPORTAMIENTO DEFAULT ListaExpte, NI BIEN SE CARGA PLANTILLAA4
        //HABILITAR ORD MANZANA
        $("#lblManzana").removeClass("disabled");
        $('#ordManzana').removeAttr("disabled", "disabled");
        $('#ordManzana').attr('checked', true);

        //HABILITAR ORD EXP. Y ORD ORIGEN
        $("#lblExpte").removeClass("disabled");
        $('#ordExpte').removeAttr("disabled", "disabled");
        $('#ordExpte').attr('checked', true);

        $("#lblOrigen").removeClass("disabled");
        $('#ordOrigen').removeAttr("disabled", "disabled");
        $('#ordOrigen').attr('checked', true);

        $("#grabarListado").prop("checked", false);
        $("#grabarListado").attr("disabled", "disabled");
        $("#cotas").prop("checked", false);

        //SI VENGO DE OTRA OPCION, LIMPIAR ARCHIVO SELECCIONADO, SI TENIA ALGUNO PREVIAMENTE
        $(".file").fileinput("clear");

    });

}

//Controles input-file
function initCtrlsPluggins(formId) {
    var form = "#" + formId;
    if ($("#cboComponentes").length > 0) {
        $('#cboComponentes').on("change", function (e) {
            var readonly = $('#cboComponentes').val() === "0";
            if (readonly) {
                $("#filtro").attr("readonly", "readonly");
                $("#buscarComponente").css("pointer-events", "none");
            } else {
                $("#filtro").removeAttr("readonly");
                $("#buscarComponente").css("pointer-events", "auto");
            }
        });
    }
    if ($("#pdf-preview").length > 0) {
        var imagen = $("#file-name").val() + ".png";
        $("#pdf-preview").fileinput("refresh", {
            showUpload: false,
            showClose: false,
            showRemove: false,
            showCaption: false,
            showPreview: true,
            initialPreview: ["<img src=\"CommonFiles/" + imagen + "\" class=\"file-preview-image\" width=\"160px\" height=\"160px\">"]
        });
        //pdf = $("#file-name").val() + ".pdf";
        //$("#pdf-preview").fileinput("refresh", {
        //    showUpload: false,
        //    showClose: false,
        //    showRemove: false,
        //    showCaption: false,
        //    showPreview: true,
        //    initialPreview: ["<object data=\"CommonFiles/" + pdf + "\" type=\"application/pdf\" width=\"160px\" height=\"160px\">"]
        //});
        $("#pdf-preview").fileinput("disable");
        $(".fileinput-remove").addClass("hidden");
        $("#pdf-preview").parent().addClass("hidden");
    }
    $(".file", form).fileinput({
        showRemove: false,
        showUpload: false,
        showPreview: false,
        allowedFileExtensions: ["txt", "xlsx", "csv"]
    });

    $(".select2", form).select2();

    if ($(".transparencia").length > 0) {
        $("#chkHabilitarImagen").after("<label for='chkHabilitarImagen'></label>");
        $("#chkHabilitarImagen").change(function () {
            $("#cboImagenSatelitalA4").enable($(this)[0].checked);
            if (!$("#chkHabilitarImagen").is(":checked")) {
                $("#cboImagenSatelitalA4").val() == "0";
            }
            $("#InputSlider").enable($(this)[0].checked);
            $("#slider").enable($(this)[0].checked);
        });

        $("#InputSlider").inputmask('Regex', { regex: "^[0-9][0-9]?" });

        $("#slider").val($("#InputSlider").val());

        $("#slider").mousemove(function () {
            $("#InputSlider").val($(this).val());
        });
        $("#slider").change(function () {
            $("#InputSlider").val($(this).val());
        });
        $("#slider").keypress(function () {
            $("#InputSlider").val($(this).val());
        });
        $("#InputSlider").keyup(function () {
            $("#slider").val($(this).val());
        });
        $("#cboImagenSatelitalA4").enable(false);
        $("#InputSlider").enable(false);
        $("#slider").enable(false);
    }
}


//COMPORTAMIENTO PLOTEO GENERAL//
function VisualizacionGeneral() {

    $("#radioVisualizacion").on("change", function () {
        $("#btnGenerar").attr('disabled', false);
        $("#cboColeccionesGral").select2("val", 0)
        $("#lblColeccionCompuesta").empty();
        $("#divColeccionGral").addClass("hide");
    });
}

function mostrarColeccionGral() {
    $("#radioColeccionGral").on("change", function () {
        $("#btnGenerar").attr('disabled', true);
        $("#divColeccionGral").removeClass("hide");
        $(".select2").select2();
        $("#lblColeccionCompuesta").empty();
    });
}

//FUNCION CARGAR TEXTOS VARIABLES

function cargarTextosVariables() {

    $("#listTextosVariables").load(BASE_URL + "Ploteo/GetTextosVariables");
}



//GENERAR PLOTEO

function CallGenerar() {
    $("#btnGenerar").on("click", function () {
        var idForm = $("#configuracion-ploteo form").attr("id");
        var idPlantilla = $("#IdPlantilla").val();
        var idComponentePrincipal = $("#idComponentePrincipal").val();

        //datos imagen satelital
        var idImagenSatelital = 0;
        var transparenciaPorc = 0;
        if ($("#chkHabilitarImagen").is(":checked")) {
            idImagenSatelital = $("#cboImagenSatelitalA4").select2("data").id
            transparenciaPorc = $("#InputSlider").val();
        }

        if ($("#chkHabilitarImagen").is(":checked") && $("#cboImagenSatelitalA4").select2("data").id == "0") {
            hideLoading();
            $("#mensaje-error-ploteo").text("No se ha seleccionado una imágen satelital");
            $("#ploteo-error-modal").modal('show');
            return;
        }

        var url = "";
        var data = {};
        switch (idForm) {
            case "predefinido-form":
                //PARAMETROS PARA GENERAR
                var idsObjetoGraf = [];
                idComponentePrincipal = $("#cboComponentes").select2("data").id
                $("#agregados input[type=checkbox]").each(function () {
                    idsObjetoGraf.push($(this).attr("id"));
                });

                if (idComponentePrincipal == 0) {
                    hideLoading();
                    $("#mensaje-error-ploteo").text("No se ha seleccionado un componente principal para plotear");
                    $("#ploteo-error-modal").modal('show');
                    return;
                }
                if (idsObjetoGraf.length == 0) {
                    hideLoading();
                    $("#mensaje-error-ploteo").text("No se han seleccionado objetos para plotear");
                    $("#ploteo-error-modal").modal('show');
                    return;
                }
                data = {
                    idComponentePrincipal: idComponentePrincipal,
                    idsObjetoGraf: JSON.stringify(idsObjetoGraf),
                    idImagenSatelital: idImagenSatelital,
                    transparenciaPorc: transparenciaPorc
                };
                url = "Ploteo/GenerarPloteoPredefinido";
                break;

            case "plancheta-form":
                var idColeccionA4 = $("#cboColeccionesA4").select2("data").id;

                var grafico = {};
                $.each($('[data-grafico]:checked'), function () {
                    var val = $(this).data('grafico');
                    grafico[val] = true;
                });

                var leyenda = {};
                $.each($('[data-leyenda]:checked'), function () {
                    var val = $(this).data('leyenda');
                    leyenda[val] = true;
                });

                var infoLeyenda = $('[name="leyenda"]').length > 0 ? $('[name="leyenda"]').val() : null;

                if ($("#radioObjetos").is(":checked")) {

                    //Por Manzana Zona Resultado
                    var IsTildadaManzanaDuplicada = $("#manzanasDupliacadas").is(':checked');

                    var ordenarPorExpediente = $("#ordExpte").is(":checked");
                    var ordenarPorManzana = $("#ordManzana").is(":checked");
                    var graboListado = $("#grabarListado").is(":checked");
                    var cotasSeleccionado = $("#cotas").is(":checked");

                    if (componentesSeleccionados.length == 0) {
                        hideLoading();
                        $("#mensaje-error-ploteo").text("No se han seleccionado objetos para plotear");
                        $("#ploteo-error-modal").modal('show');
                        return;
                    }
                    data = {
                        idsObjetoGraf: JSON.stringify(componentesSeleccionados),
                        ordenarPorExpediente: ordenarPorExpediente,
                        ordenarPorManzana: ordenarPorManzana,
                        grabarListado: graboListado,
                        isManzanaDuplicadaCheck: IsTildadaManzanaDuplicada,
                        verCotas: cotasSeleccionado,
                        grafico: grafico,
                        leyenda: leyenda,
                        infoLeyenda: infoLeyenda,
                        idImagenSatelital: idImagenSatelital,
                        transparenciaPorc: transparenciaPorc
                    };
                    url = "Ploteo/GenerarPloteoPlanchetaA4ByManzanaZonaResultado";
                } else if ($("#radioColeccion").is(":checked")) {
                    //Por coleccion
                    var IsTildadaManzanaDuplicada = $("#manzanasDupliacadas").is(':checked');

                    var ordenarPorExpediente = $("#ordExpte").is(":checked");
                    var ordenarPorManzana = $("#ordManzana").is(":checked");
                    var graboListado = $("#grabarListado").is(":checked");
                    var cotasSeleccionado = $("#cotas").is(":checked");
                    data = {
                        idColeccion: idColeccionA4,
                        ordenarPorExpediente: ordenarPorExpediente,
                        ordenarPorManzana: ordenarPorManzana,
                        grabarListado: graboListado,
                        isManzanaDuplicadaCheck: IsTildadaManzanaDuplicada,
                        verCotas: cotasSeleccionado,
                        grafico: grafico,
                        leyenda: leyenda,
                        infoLeyenda: infoLeyenda,
                        idImagenSatelital: idImagenSatelital,
                        transparenciaPorc: transparenciaPorc
                    };
                    url = "Ploteo/GenerarPloteoPlanchetaA4ByColeccion";
                } else if ($("#radioListManzanas").is(":checked")) {
                    //Por Listado Manzanas

                    //Recorre el archivo que suban y saca los datos para plotear
                    var tipoGrafico = "esManzana";
                    var IsTildadaManzanaDuplicada = $("#manzanasDupliacadas").is(':checked');

                    var ordenarPorExpediente = $("#ordExpte").is(":checked");
                    var ordenarPorManzana = $("#ordManzana").is(":checked");
                    var graboListado = $("#grabarListado").is(":checked");
                    var cotasSeleccionado = $("#cotas").is(":checked");
                    var reader = new FileReader();
                    reader.onload = function () {
                        var filecontent = reader.result;
                        data = {
                            content: filecontent,
                            fileExt: $("#listManzanas")[0].files[0].name.split('.').pop(),
                            idColeccion: idColeccionA4,
                            tipoGrafico: tipoGrafico,
                            idComponentePrincipal: idComponentePrincipal,
                            ordenarPorExpediente: ordenarPorExpediente,
                            ordenarPorManzana: ordenarPorManzana,
                            grabarListado: graboListado,
                            isManzanaDuplicadaCheck: IsTildadaManzanaDuplicada,
                            verCotas: cotasSeleccionado,
                            grafico: grafico,
                            leyenda: leyenda,
                            infoLeyenda: infoLeyenda,
                            idImagenSatelital: idImagenSatelital,
                            transparenciaPorc: transparenciaPorc
                        };
                        url = "Ploteo/GenerarPloteoPlanchetaA4ByFile";
                        CallGetPlantilla(url, data);
                    };
                    reader.readAsDataURL($("#listManzanas")[0].files[0]);
                    return;
                } else if ($("#radioListExpte").is(":checked")) {
                    //Por Listado Expediente

                    var ordenarPorExpediente = $("#ordExpte").is(":checked");
                    var ordenarPorManzana = $("#ordManzana").is(":checked");
                    var graboListado = $("#grabarListado").is(":checked");
                    var cotasSeleccionado = $("#cotas").is(":checked");
                    var tipoGrafico = "esExpediente";
                    var IsTildadaManzanaDuplicada = $("#manzanasDupliacadas").is(':checked');

                    var reader = new FileReader();
                    reader.onload = function () {
                        var filecontent = reader.result;
                        data = {
                            content: filecontent,
                            fileExt: $("#listExpte")[0].files[0].name.split('.').pop(),
                            idColeccion: idColeccionA4,
                            tipoGrafico: tipoGrafico,
                            idComponentePrincipal: idComponentePrincipal,
                            ordenarPorExpediente: ordenarPorExpediente,
                            ordenarPorManzana: ordenarPorManzana,
                            grabarListado: graboListado,
                            isManzanaDuplicadaCheck: IsTildadaManzanaDuplicada,
                            verCotas: cotasSeleccionado,
                            grafico: grafico,
                            leyenda: leyenda,
                            infoLeyenda: infoLeyenda,
                            idImagenSatelital: idImagenSatelital,
                            transparenciaPorc: transparenciaPorc
                        };
                        url = "Ploteo/GenerarPloteoPlanchetaA4ByFile";
                        CallGetPlantilla(url, data);
                    };
                    reader.readAsDataURL($("#listExpte")[0].files[0]);
                    return;
                }
                break;
            case "general-form":
                let textosVariables = [];

                $("#listTextosVariables label").each(function () {
                    textosVariables = [...textosVariables, `${$(this).text()},${$("#" + $(this).attr("for")).val()}`];
                });

                if ($("#radioVisualizacion").is(":checked")) {
                    data = {
                        idPlantillaFondo: 0,
                        textosVariables: textosVariables.join(";"),
                        extent: GeoSIT.MapaController.obtenerExtent().join(),
                        scale: 1,
                        layersVisibles: GeoSIT.MapaController.obtenerCapasVisibles().join(),
                        idImagenSatelital: idImagenSatelital,
                        transparenciaPorc: transparenciaPorc
                    };
                    url = "Ploteo/GenerarPloteoVistaActual";
                    CallGetPlantilla(url, data);
                    return;
                } else {
                    data = {
                        idColeccion: $("#cboColeccionesGral").select2("data").id,
                        idComponentePrincipal: idComponentePrincipal,
                        textosVariables: textosVariables.join(";"),
                        idImagenSatelital: idImagenSatelital,
                        transparenciaPorc: transparenciaPorc
                    };
                    url = "Ploteo/GenerarPloteoGeneral";
                }
                break;
            case "obra-form":
                var textosAtributos = "";

                var idsAtributos = [];
                $.each($('[data-atributo]:checked'), function () {
                    idsAtributos.push($(this).data('atributo'));
                });

                textosAtributos = idsAtributos.join(",");
                //Caso Obras
                if ($("#radioObjetos").is(":checked")) {
                    //Objetos de Resultados
                    if (!componentesSeleccionados.length) {
                        hideLoading();
                        $("#mensaje-error-ploteo").text("No se han seleccionado objetos para plotear");
                        $("#ploteo-error-modal").modal('show');
                        return;
                    }
                    let idsObjetos = [];
                    $.each(componentesSeleccionados, function (_, comp) {
                        idsObjetos = [...idsObjetos, comp.Id];
                    });

                    data = {
                        idsObjetoGraf: idsObjetos.join(","),
                        idComponentePrincipal: idComponentePrincipal,
                        textosVariables: textosAtributos
                    };
                    url = "Ploteo/GenerarPloteoObrasByIds";
                } else if ($("#radioColeccion").is(":checked")) {
                    //Coleccion
                    var idColeccion = $("#cboColeccionesObras").select2("data").id;
                    data = {
                        idColeccion: idColeccion,
                        idComponentePrincipal: idComponentePrincipal,
                        textosVariables: textosAtributos
                    };
                    url = "Ploteo/GenerarPloteoObrasByColeccion";
                }
                break;
        }
        CallGetPlantilla(url, data);
    });

}

function CallGetPlantilla(method, data) {
    showLoading();
    $.ajax({
        url: BASE_URL + method,
        data: data,
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        success: function () {
            IS_DOWNLOAD = true;
            window.location = BASE_URL + "Ploteo/DownloadZip";
        },
        error: function (error) {
            let msg;
            switch (error.status) {
                case 409:
                    msg = "Error en plantilla. Uno o más campos requeridos no contienen entradas.";
                    break;
                case 411:
                    msg = "No se encontraron todos los objetos a plotear";
                    break;
                case 412:
                    msg = "El documento no contiene páginas.";
                    break;
                case 417:
                    msg = "Supera el máximo permitido de ploteos.";
                    break;
                default:
                    msg = "Ha ocurrido un error al generar el ploteo.";
                    break;
            }
            $("#mensaje-error-ploteo").text(msg);
            $("#ploteo-error-modal").modal('show');
        },
        complete: hideLoading
    });
    return false;
}

function GenerarListadoTxt(idPlA4, idColA4, idCompPpalA4, ordPorExpediente, ordPorManzana, graboList) {
    IS_DOWNLOAD = true;
    window.location = BASE_URL + "Ploteo/GenerarListadoTXTPlanchetaA4ByColeccion?id=" + idPlA4 + "&idColeccion=" + idColA4 + "&idComponentePrincipal=" + idCompPpalA4 + "&ordenarPorExpediente=" + ordPorExpediente + "&ordenarPorManzana=" + ordPorManzana + "&grabarListado=" + graboList;
}

function ComposicionColeccionA4() {
    $("[name='optradio'").change(function () { $("#lblColeccionCompuesta").empty(); });
    //DESHABILITA FUNCION TildarManzanasDuplicadas
    //DESTILDAR CHECKBOX MANZANADUPLICADA
    $("#manzanasDupliacadas").prop("checked", false);
    //DESHABILITAR CHECKBOX MANZANADUPLICADA
    $("#manzanasDupliacadas").attr("disabled", "disabled");

    //SETEAR CONTROLES DE ORDENAMIENTO PARA COLECCION PLANCHETA A4

    //HABILITAR ORD MANZANA
    $("#ordManzana").removeAttr("disabled", "disabled");
    // SELECCIONAR MANZANA
    $('#ordManzana').attr('checked', true);

    //DESHABILITAR ORD EXPEDIENTE
    $("#ordExpte").attr("disabled", "disabled");
    // LIMPIAR SELECCION SI ESTABA SELECCIONADO DE ANTES
    $('#ordExpte').attr('checked', false);

    //DESHABILITAR ORD ORIGEN    
    //LIMPIAR SELECCION SI ESTABA SELECCIONADO DE ANTES
    $("#ordOrigen").prop("checked", false);
    //DESHABILITAR LUEGO
    $("#ordOrigen").attr("disabled", "disabled");

    //ON CHANGE
    $("#cboColeccionesA4").on("change", function () {
        //SI ELEGI UNA COLECCION, HABILITAR BOTON GENERAR
        $("#btnGenerar").attr('disabled', false);

        var idColeccionA4 = $("#cboColeccionesA4").select2("data").id;
        var valorCombo = $("#cboColeccionesA4").val();
        var coleccionContiene = "";
        if (valorCombo != 0) {
            $.ajax({
                url: BASE_URL + "Ploteo/ComposicionColeccion",
                type: "GET",
                cache: false,
                data: { idColeccion: idColeccionA4 },
                success: function (data) {

                    var soloManzanas = true;
                    $.each(data, function (componente, cantidad) {
                        if (componente != "manzanas")
                            soloManzanas = false;
                    });

                    $("#lblColeccionCompuesta").html(getBadge(data));

                    //COLECCION ELEGIDA TIENE SOLO MANZANAS 
                    if (soloManzanas) {
                        habilitarOpcionesSoloManzanas();
                    } else {
                        habilitarOpcionesMultiplesComponentes();
                    }

                },
                error: function (error) {
                    console.debug(error);
                }
            });
        } else {
            $("#lblColeccionCompuesta").empty();
        }
    });

    $('[data-comercial]').change(function () {
        var obj = $(this);
        var opc = $(this).data('comercial');

        if (obj.prop("checked")) {
            $('[name="grafico[' + opc + ']"]').prop('checked', true);
            $('[name="grafico[' + opc + ']"]').prop('disabled', false);
            $('[name="leyenda[' + opc + ']"]').prop('checked', true);
            $('[name="leyenda[' + opc + ']"]').prop('disabled', false);
        } else {
            $('[name="grafico[' + opc + ']"]').prop('checked', false);
            $('[name="grafico[' + opc + ']"]').prop('disabled', true);
            $('[name="leyenda[' + opc + ']"]').prop('checked', false);
            $('[name="leyenda[' + opc + ']"]').prop('disabled', true);
        }

    });

    $('#collapseInformacionComercial [data-toggle="tooltip"]').tooltip({
        placement: 'right'
    });

    $('#collapseOpciones [data-toggle="tooltip"]').tooltip({
        placement: 'right'
    });
}

function habilitarOpcionesSoloManzanas() {

    //DESHABILITA FUNCION TildarManzanasDuplicadas
    //DESTILDAR CHECKBOX MANZANADUPLICADA
    $("#manzanasDupliacadas").prop("checked", false);
    //DESHABILITAR CHECKBOX MANZANADUPLICADA
    $("#manzanasDupliacadas").attr("disabled", "disabled");

    //SETEAR CONTROLES DE ORDENAMIENTO PARA COLECCION PLANCHETA A4

    //HABILITAR ORD MANZANA
    $("#lblManzana").removeClass("disabled");
    $("#ordManzana").removeAttr("disabled", "disabled");
    // SELECCIONAR MANZANA
    $('#ordManzana').attr('checked', true);

    //DESHABILITAR ORD EXPEDIENTE
    $("#lblExpte").addClass("disabled");
    $("#ordExpte").attr("disabled", "disabled");
    // LIMPIAR SELECCION SI ESTABA SELECCIONADO DE ANTES
    $('#ordExpte').attr('checked', false);

    //DESHABILITAR ORD ORIGEN    
    //LIMPIAR SELECCION SI ESTABA SELECCIONADO DE ANTES
    $("#ordOrigen").prop("checked", false);
    //DESHABILITAR LUEGO
    $("#lblOrigen").addClass("disabled");
    $("#ordOrigen").attr("disabled", "disabled");

    $("#grabarListado").prop("checked", false);
    $("#grabarListado").removeAttr("disabled", "disabled");
}

function habilitarOpcionesMultiplesComponentes() {
    //HABILITA FUNCION TildarManzanasDuplicadas
    //HABILITAR CHECKBOX MANZANADUPLICADA
    $("#manzanasDupliacadas").prop("checked", true);
    $("#manzanasDupliacadas").removeAttr("disabled", "disabled");

    //HABILITAR ORD MANZANA
    $("#lblOrigen").removeClass("disabled");
    $("#ordManzana").removeAttr("disabled", "disabled");
    // SELECCIONAR MANZANA
    $('#ordManzana').attr('checked', true);

    //HABILITAR ORD EXPEDIENTE
    $("#lblExpte").removeClass("disabled");
    $("#ordExpte").removeAttr("disabled", "disabled");
    // LIMPIAR SELECCION SI ESTABA SELECCIONADO DE ANTES
    $('#ordExpte').attr('checked', false);

    //DESHABILITAR ORD ORIGEN    
    //LIMPIAR SELECCION SI ESTABA SELECCIONADO DE ANTES
    $("#ordOrigen").prop("checked", false);
    //DESHABILITAR LUEGO
    $("#lblOrigen").addClass("disabled");
    $("#ordOrigen").attr("disabled", "disabled");

    $("#grabarListado").prop("checked", false);
    $("#grabarListado").removeAttr("disabled", "disabled");

}

function ComposicionColeccionGeneral() {
    $("#cboColeccionesGral").on("change", function () {
        var idPlantillaGenerar2 = $("#IdPlantilla").val();

        var idColeccionGeneral = $("#cboColeccionesGral").select2("data").id;
        var valorCombo = $("#cboColeccionesGral").val();

        var composicion = "";
        $("#btnGenerar").attr('disabled', false);

        if (valorCombo != 0) {
            $.ajax({
                url: BASE_URL + "Ploteo/ValidacionColeccion",
                type: "GET",
                data: { idPlantilla: idPlantillaGenerar2, idColeccion: idColeccionGeneral },
                success: function (data) {
                    var composicion = data;
                    $("#lblColeccionCompuesta").html(composicion);
                    if (composicion == "La colección seleccionada contiene más de un componente principal de esta plantilla.") {
                        $("#btnGenerar").attr('disabled', true);
                    }
                },
                error: function (error) {
                    console.debug(error);
                }
            });
        } else {
            $("#lblColeccionCompuesta").empty();
        }
    });
}


//ON CHANGE
function ComposicionColeccionObra() {
    $("#cboColeccionesObras").on("change", function () {
        //SI ELEGI UNA COLECCION, HABILITAR BOTON GENERAR
        $("#btnGenerar").attr('disabled', false);

        var idColeccionA4 = $("#cboColeccionesObras").select2("data").id;
        var valorCombo = $("#cboColeccionesObras").val();
        var coleccionContiene = "";

        var idComponentePrincipal = $("#idComponentePrincipal").val();

        if (valorCombo != 0) {
            $.ajax({
                url: BASE_URL + "Ploteo/ComposicionColeccionObra",
                type: "GET",
                cache: false,
                data: {
                    idColeccion: idColeccionA4,
                    idCompPrincipal: idComponentePrincipal
                },
                success: function (data) {

                    var badge = $('<span>').addClass('badge');

                    badge.text(data + " " + (data != 1 ? "Objetos" : "Objeto"));

                    $("#lblColeccionCompuesta").html(badge);
                },
                error: function (error) {
                    console.debug(error);
                }
            });
        } else {
            $("#lblColeccionCompuesta").empty();
        }
    });
}

function TildarManzanasDuplicadas() {
    //SI LA COLECCION TIENE SOLO PARCELAS,SE HABILITA CHECK MANZANA DUPLICADA
    //DEFAULT ES CHECK DESTILDADO
    if ($("#radioColeccion").is(":checked")) {
        //ManzanasDuplicadas TILDADA
        if ($("#manzanasDupliacadas").is(':checked')) {
            //HABILITAR
            //ORIGEN            
            //$("#lblOrigen").removeClass("disabled");
            //$("#ordOrigen").removeAttr("disabled");

            //EXPEDIENTE
            $("#lblExpte").removeClass("disabled");
            $("#ordExpte").removeAttr("disabled");

            //MANZANA
            $("#lblManzana").removeClass("disabled");
            $("#ordManzana").removeAttr("disabled");
        }
        else {

            //ManzanasDuplicadas DESTILDADA
            //DESELECCIONAR*****************************
            //ORIGEN
            $("#ordOrigen").attr('checked', false);
            //EXPEDIENTE
            $("#ordExpte").attr('checked', false);

            //DESHABILITAR
            //ORIGEN
            $("#lblOrigen").addClass("disabled");
            $("#ordOrigen").attr("disabled", "disabled");
            //EXPEDIENTE
            $("#lblExpte").addClass("disabled");
            $("#ordExpte").attr("disabled", "disabled");

            //HABILITAR*********************************
            //MANZANA
            $("#lblManzana").removeAttr("disabled");
            $('#ordManzana').removeAttr("disabled", "disabled");

            //SELECCIONAR
            //MANZANA
            $('#ordManzana').attr('checked', true);
        }
    }


    if ($("#radioListExpte").is(":checked")) {

        if ($("#manzanasDupliacadas").is(':checked')) {
            //HABILITAR
            //ORIGEN            
            $("#lblOrigen").removeClass("disabled");
            $("#ordOrigen").removeAttr("disabled");

            //EXPEDIENTE
            $("#lblExpte").removeClass("disabled");
            $("#ordExpte").removeAttr("disabled");

        }
        else {
            //DESELECCIONAR*****************************
            //ORIGEN
            $("#ordOrigen").attr('checked', false);
            //EXPEDIENTE
            $("#ordExpte").attr('checked', false);

            //DESHABILITAR
            //ORIGEN
            $("#lblOrigen").addClass("disabled");
            $("#ordOrigen").attr("disabled", "disabled");
            //EXPEDIENTE
            $("#lblExpte").addClass("disabled");
            $("#ordExpte").attr("disabled", "disabled");

            //HABILITAR*********************************
            //MANZANA
            $("#lblManzana").removeAttr("disabled");
            $('#ordManzana').removeAttr("disabled", "disabled");

            //SELECCIONAR
            //MANZANA
            $('#ordManzana').attr('checked', true);
        }
    }
}

function HabilitarLisBtnGenerar() {
    //SI NO HAY ARCHIVO SELECCIONADO, DESHABILITAR BOTON GENERAR
    $("#btnGenerar").attr('disabled', false);
}

//@ sourceURL=ploteo.js