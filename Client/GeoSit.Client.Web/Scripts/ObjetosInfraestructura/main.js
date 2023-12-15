var ConsultaId, ConsultaComp, ConsultaAtributo,
    ConsultaValor, ConsultaConec, FeatId,
    selectedRowObjetos, interval, mapa;

$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-infraestructura').one('shown.bs.modal', function () {
    mapa = null;
    ajustarScrollBars();
    hideLoading();
});

function init() {
    $("#btn-aceptar-mapa").click(function () {
        var coords = mapa.obtenerDibujos()[0];
        $("#txtCoordenadas").val(coords);
        //Yo estoy poniendo en el Model estas coords
    });

    $("#btnLimpiar").click(function () {
        mapa.limpiar();
    });

    ///////////////////// Scrollbars ////////////////////////
    $("#infraestructura-datos").niceScroll(getNiceScrollConfig());
    $('#scroll-content-infraestructura .panel-body').resize(ajustarScrollBars);
    $('#infraestructura-datos .panel-heading').click(function () {
        setTimeout(function () {
            $("#infraestructura-datos").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////
    $("#headingResultado").click(function () {
        setTimeout(function () { columnsAdjust('Grilla_Objetos'); }, 30);
    });
    createDataTable("Grilla_Consulta");
    //createDataTable("Grilla_Objetos", 0);

    $("#Grilla_Consulta tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            selectedLayerId = 0;
            $("#btn_EliminarQ").addClass('boton-deshabilitado');
            $("#btn_ModificarQ").addClass('boton-deshabilitado');
        } else {
            $("tr", "#Grilla_Consulta tbody").not(this).removeClass("selected");

            ConsultaId = Number($(this).find("td:First").html());

            ConsultaComp = $(this).find("td:eq(2)").text();
            ConsultaAtributo = $(this).find("td:eq(1)").text();
            ConsultaValor = $(this).find("td:eq(3)").text();
            ConsultaConec = $(this).find("td:eq(4)").text();

            $("#CmbAtributo option:contains(" + ConsultaAtributo + ")").attr('selected', true);
            $("#CmbComparador option:contains(" + ConsultaComp + ")").attr('selected', true);
            $("#CmbConector option:contains(" + ConsultaConec + ")").attr('selected', true);
            $("#txtValor").val(ConsultaValor);

            $("#btn_EliminarQ").removeClass('boton-deshabilitado');
            $("#btn_ModificarQ").removeClass('boton-deshabilitado');
        }
        $(this).toggleClass("selected");
    });

    $('#CmbSubTipoObjeto').init(function () {
        ActualizarSubTipos();
    });

    $('#CmbTipoObjeto').change(function () {
        ActualizarSubTipos();
    });

    $('#CmbSubTipoObjeto').change(function () {
        ActualizarAtributos();
    });

    $("#CmbAtributo").change(function () {
        ChangeValor();
    });

    $("#btn_AgregarQ").click(function () {
        $.ajax({
            type: "POST",
            url: BASE_URL + "Infraestructura/AgregarConsulta",
            dataType: "json",
            data: {
                pId: 0,
                pAtributo: $("#CmbAtributo").children("option").filter(":selected").text(),
                pComparador: $("#CmbComparador").val(),
                pValor: $("#txtValor").val(),
                pConector: $('#CmbConector').val()
            },
            success: function (sok) {
                ActualizarGridEditor(sok);
            },
            error: function (ex) { }
        });
    });

    $("#btn_ModificarQ").click(function () {
        $.ajax({
            type: "POST",
            url: BASE_URL + "Infraestructura/ModificarConsulta",
            dataType: "json",
            data: {
                pId: ConsultaId,
                pAtributo: $("#CmbAtributo").children("option").filter(":selected").text(),
                pComparador: $("#CmbComparador").val(),
                pValor: $("#txtValor").val(),
                pConector: $("#CmbConector").val()
            },
            success: function (sok) {
                ActualizarGridEditor(sok);
            },
            error: function (ex) { }
        });
    });

    $("#btn_EliminarQ").click(function () {
        $.ajax({
            type: "POST",
            url: BASE_URL + "Infraestructura/EliminarConsulta",
            dataType: "json",
            data: { pId: ConsultaId },
            success: function (sok) {
                ActualizarGridEditor(sok);
            },
            error: function (ex) { }
        });
    });

    $("#btn_LimpiarQ").click(function () {
        LimpiarConsulta();
        ActualizarGridEditor();
    });

    $("#btn_Agregar").click(function () {
        EditarObjeto(0);
    });

    $("#btn_Modificar").click(function () {
        hideMapa();
        EditarObjeto(FeatId);
        $("accordion-infraestructura").addClass("hidden");
    });

    $("#btn_Eliminar").click(function () {
        if (selectedRowObjetos) {
            FeatId = selectedRowObjetos.find("td").eq(0).html();
            var descripcion = selectedRowObjetos.find("td").eq(1).html();
            modalConfirm("Eliminar - Objeto", "¿Desea eliminar el Objeto " + descripcion + "?");

            $("#btnAdvertenciaOK").off('click').click(function () {
                $.ajax({
                    url: BASE_URL + "Infraestructura/DeleteObjetoInfraestructura",
                    type: "POST",
                    dataType: "json",
                    data: { FeatId: FeatId },
                    success: function (data) {
                        if (data["success"]) {
                            selectedRowObjetos.remove();
                            selectedRowObjetos = null;
                        }
                    }
                });
            });
        }
    });

    $('#btnCerrarMsg').click(function () {
        $("#myModalDelete").removeClass("show");
        $("#myModalDelete").addClass("hide");
    });

    $('#btnCancelarMsg').click(function () {
        $("#myModalDelete").removeClass("show");
        $("#myModalDelete").addClass("hide");
    });

    $("#btnBuscar").click(function () {
        ActualizarGridObjetos();
        $("#Grilla_Objetos .dataTables_scroll .dataTables_scrollBody").css("height", "500px");
    });

    $("#btnGrabar").click(function () {
        if ($("#txtNombre").val()) {
            $("#txtNombreVal").addClass("hidden");

            //if ($("#txtDescripcion").val() != null && $("#txtDescripcion").val().trim() != "") {
            $("#txtDescripcionVal").addClass("hidden");

            var Atributos = "!NewDataSet¡!Datos¡";

            $('.schema-property').each(function (_, input) {
                var prop = input.id.substring(0, input.id.length - 2);
                Atributos += "!" + prop + "¡" + $(input).val() + "!/" + prop + "¡";
            });

            Atributos += "!Observaciones" + "¡" + $("#txtObservacionesId").val() + "!/Observaciones¡";
            Atributos += "!/Datos¡!/NewDataSet¡";

            var data = {
                FeatID: $("#FeatIdEditar").val(),
                Nombre: $("#txtNombre").val(),
                Descripcion: $("#txtDescripcion").val(),
                ID_Tipo_Objeto: $("#CmbTipoObjetoEditar").val(),
                ID_Subtipo_Objeto: $("#CmbSubTipoObjetoEditar").val(),
                ClassID: $("#CmbClaseEditar").val(),
                Atributos: JSON.stringify(Atributos),
                Geometry: null
            };

            $.ajax({
                type: "POST",
                url: BASE_URL + "Infraestructura/PostObjetoInfraestructura",
                dataType: "json",
                data: { model: data, geometry: $("#txtCoordenadas").val() },
                success: function () {
                    $("#btnCerrar").click();
                },
                error: function (ex) {
                    alert('Error actualizando el objeto');
                }
            });
        } else {
            $("#txtNombreVal").removeClass("hidden");
        }
        ActualizarGridObjetos();
    });

    $("#btnCerrarMapa").click(function () {
        hideMapa();
    });

    $("#btnCerrar").click(function () {
        hideMapa();
        $("#footer-infraestructura span").addClass('boton-deshabilitado');
        $("#EditarObjeto").addClass("class").empty();
        $("#accordion-infraestructura > .section-position:not(:last)").removeClass("hidden");
    });

    $("#btn_Mapa").click(function () {
        var feature = Number(selectedRowObjetos.find("td:eq(0)").html()),
            layerName = $("#CmbSubTipoObjeto :selected").attr("data-layer"),
            subtipo = $("#CmbSubTipoObjeto :selected").text().trim();

        $.get(BASE_URL + 'Infraestructura/TieneGrafico/' + feature, function (resp) {
            if (resp.wkt) {
                if (layerName) {
                    showMapa()
                        .then(function () {
                            mapa.limpiar();
                            mapa.seleccionarObjetos([[feature]], [layerName]);
                        })
                        .catch(function (error) {
                            console.log(error);
                        });
                } else {
                    $("#TituloInfo").text('Información');
                    $("#DescripcionInfo").text('El subtipo "' + subtipo + '" no tiene asociada ninguna capa en el mapa');
                    $("#MensajeInfo").addClass('alert-warning');
                    $("#infoModal").modal("show");
                }
            } else {
                $("#TituloInfo").text('Información');
                $("#DescripcionInfo").text('El objeto seleccionado no tiene gráfico asociado');
                $("#MensajeInfo").addClass('alert-warning');
                $("#infoModal").modal("show");
            }
        });
        return false;
    });

    $("#EditarObjeto").addClass('hidden');
    $("#mensaje-salida-infraestructura").addClass('hidden');
    $("#accordionMap").addClass('hidden');
    $("#HeaderEditar").addClass('hidden');

    ajustarmodal();

    $("#modal-window-infraestructura").modal("show");
}
function activateDigitizeOption() {
    var clase = Number($("#CmbClaseEditar :selected").val());
    switch (clase) {
        case 1:
            mapa.activarDibujoPunto();
            break;
        case 2:
            mapa.activarDibujoLinea();
            break;
        case 3:
            mapa.activarDibujoPoligono();
            break;
    }
}
function showMapa() {
    $("#infraestructura-datos").addClass("col-xs-6");
    $("#infraestructura-mapa").removeClass("hidden");
    $("#modal-window-infraestructura .modal-dialog:first").addClass("modal-xlg");
    return new Promise(function (resolve) {
        if (!mapa) {
            mapa = new MapaController(2, "div-mapa-container", true, false, false, false, true, true, false, false, true, true, true);
            setTimeout(resolve, 1000);
        } else {
            resolve();
        }
    });
}
function hideMapa() {
    $("#infraestructura-mapa").addClass("hidden");
    $("#infraestructura-datos").removeClass("col-xs-6");
    $("#modal-window-infraestructura .modal-dialog:first").removeClass("modal-xlg");
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter;
    $(".infraestructura-body", "#scroll-content-infraestructura").css({ "height": altura, "overflow": "hidden" });
    $('#infraestructura-datos').css({ "max-height": altura + 'px' });
    $('#infraestructura-map').css({ "max-height": altura + 'px' });
    $('#div-mapa-container').css({ height: altura - 15 - $('#infraestructura-datos .panel-heading:visible:first').outerHeight() });

    ajustarScrollBars();
}
function ajustarScrollBars() {
    $("#infraestructura-datos").getNiceScroll().resize();
    $("#infraestructura-datos").getNiceScroll().show();
}

function ActualizarSubTipos() {
    var selection = $("#CmbTipoObjeto").val();

    $("#CmbSubTipoObjeto").prop("disabled", true);

    var options = {};
    options.url = BASE_URL + "Infraestructura/GetSubTipos";
    options.type = "POST";
    options.data = JSON.stringify({ Id_Tipo: selection });
    options.dataType = "json";
    options.contentType = "application/json";
    options.success = function (result) {

        $("#CmbSubTipoObjeto").empty();
        for (var i = 0; i < result.length; i++) {
            var subtipo = document.createElement("option");
            subtipo.value = result[i].ID_Subtipo_Objeto;
            subtipo.text = result[i].Nombre;
            $(subtipo).attr("data-layer", result[i].Layer);
            $("#CmbSubTipoObjeto").append(subtipo);
        }

        ActualizarAtributos();

        $("#CmbSubTipoObjeto").prop("disabled", false);
    };
    options.error = function () { alert("Error cargando subtipos!"); };
    $.ajax(options);
}

function ActualizarAtributos() {
    var selection = $("#CmbSubTipoObjeto").val();

    $.ajax({
        type: "POST",
        url: BASE_URL + "Infraestructura/VaciarGrilla",
        dataType: "json",
        data: {},
        success: function () {
            $("#CmbAtributo").prop("disabled", true);

            var options = {};
            options.url = BASE_URL + "Infraestructura/GetAtributos";
            options.type = "POST";
            options.data = JSON.stringify({ ID_Subtipo_Objeto: selection });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (result) {

                $("#CmbAtributo").empty();
                for (var i = 0; i < result.length; i++) {
                    $("#CmbAtributo").append($('<option/>', {
                        value: i + 1,
                        text: result[i]
                    }));
                }
                ActualizarGridEditor();
                ChangeValor();
            };
            options.error = function () { $("#CmbAtributo").empty(); };
            $.ajax(options);

            $("#CmbAtributo").prop("disabled", false);
        }, error: function (ex) {

        }
    });



}

function ActualizarGridEditor() {
    destroyDataTable("Grilla_Consulta");
    $.get(BASE_URL + "Infraestructura/_DetalleConsulta", {}, function (result) {
        $("#Grilla_Consulta tbody").empty();
        $("#Grilla_Consulta tbody").append($(result));
        createDataTable("Grilla_Consulta", "250px");
    });
}

function ActualizarGridObjetos() {
    var SubTipo = $("#CmbSubTipoObjeto").val();
    showLoading();
    destroyDataTable("Grilla_Objetos");
    $.get(BASE_URL + "Infraestructura/_ObjetosInfraestructura?pIdSubTipo=" + SubTipo, function (data) {
        var columns = [
            { data: "FeatID", name: "FeatID", className: "hide" },
            { data: "Nombre", name: "Nombre", title: "Nombre" },
            { data: "Descripcion", name: "Descripción", title: "Descripción" }
        ];
        var comunes = columns.map(function (col) { return col.data; });
        data = JSON.parse(JSON.parse(data).data);
        if (data.length) {
            columns = Object.keys(data[0])
                .filter(function (key) { return comunes.indexOf(key) === -1; })
                .reduce(function (acum, key) { return acum.concat({ data: key, name: key, title: key }); }, columns);
        }
        $("#Grilla_Objetos thead").empty();
        $("#Grilla_Objetos").dataTable({
            scrollX: true,
            scrollY: '370px',
            destroy: true,
            paging: true,
            searching: false,
            processing: true,
            dom: 'rtp',
            aaSorting: [[0, "asc"]],
            language: {
                url: BASE_URL + "Scripts/dataTables.spanish.txt"
            },
            data: data,
            createdRow: function (row, data) {
                $(row).click(function () {
                    if ($(this).hasClass("selected")) {
                        selectedLayerId = 0;
                        selectedRowObjetos = null;
                        $("#btn_Eliminar").addClass('boton-deshabilitado');
                        $("#btn_Modificar").addClass('boton-deshabilitado');
                        $("#btn_Mapa").addClass('boton-deshabilitado');
                    } else {
                        $("tr", "#Grilla_Objetos tbody").not(this).removeClass("selected");

                        selectedRowObjetos = $(this);

                        FeatId = Number(data.FeatID);

                        $("#btn_Eliminar").removeClass('boton-deshabilitado');
                        $("#btn_Modificar").removeClass('boton-deshabilitado');
                        $("#btn_Mapa").removeClass('boton-deshabilitado');
                    }
                    $(this).toggleClass("selected");
                });
            },
            initComplete: function () {
                hideLoading();
                columnsAdjust('Grilla_Objetos');
                ajustarScrollBars();
                $("#headingBusqueda").find("a:first[aria-expanded=true]").click();
                $("#headingResultado").find("a:first[aria-expanded=false]").click();
            },
            columns: columns
        });
    });

}

function LimpiarConsulta() {

    $.ajax({
        type: "POST",
        url: BASE_URL + "Infraestructura/ClearConsulta",
        dataType: "json",
        data: {}
    });
}

function EditarObjeto(pFeatId) {
    var Tipo, SubTipo;
    Tipo = $("#CmbTipoObjeto").val();
    SubTipo = $("#CmbSubTipoObjeto").val();
    $.get(BASE_URL + "Infraestructura/_EditarObjetoInfraestructura", { FeatId: pFeatId, TipoId: Tipo, SubTipoId: SubTipo }, function (result) {
        $('#EditarObjeto').empty();
        $('#EditarObjeto').append(result);
        $("#EditarObjeto").removeClass('hidden');
        $("#accordion-infraestructura > .section-position:not(:last)").addClass("hidden");

        $("#CmbTipoObjetoEditar").prop('disabled', true);
        $("#CmbSubTipoObjetoEditar").prop('disabled', true);
        //$("#txtCoordenadas").attr("disabled", "disabled");

        if (!$("#accordionMap").hasClass("hidden")) {
            $("#accordionMap").addClass("hidden");
            $("#ModalDialogMantInfraestructura").css({ "width": "600px" });
        }

        $("#footer-infraestructura span").removeClass('boton-deshabilitado');

        //*** CONTROLES EDITAR OBJETO
        $('#CmbTipoObjetoEditar').change(function () {
            ActualizarSubTiposEditar();
        });

        $('#btn_MapaEditar').off('click').click(function () {
            var feature = Number($("#FeatIdEditar").val()),
                layerName = $("#CmbSubTipoObjeto :selected").attr("data-layer");

            if (layerName) {
                $.get(BASE_URL + 'Infraestructura/TieneGrafico/' + feature, function (resp) {
                    showMapa()
                        .then(function () {
                            mapa.limpiar();
                            if (feature !== 0 && resp.wkt) {
                                mapa.editarObjeto(resp.wkt);
                            } else {
                                activateDigitizeOption();
                            }
                        })
                        .catch(function (error) {
                            console.log(error);
                        });
                });
            } else {
                $("#TituloInfo").text('Información');
                $("#DescripcionInfo").text('El Subtipo Objeto no tiene un layer configurado');
                $("#MensajeInfo").addClass('alert-warning');
                $("#infoModal").modal("show");
            }
        });
        setTimeout(function () { ajustarScrollBars(); }, 100);
    });
}

function modalConfirm(title, message) {
    $("#TituloAdvertencia").text(title);
    $("#DescripcionAdvertencia").html(message);
    $("#confirmModal").modal("show");
}

function createDataTable(tableId, scrollY) {
    if (!scrollY)
        scrollY = "250px";

    $("#" + tableId).DataTable({
        dom: "rt",
        scrollY: scrollY,
        destroy: true,
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        }
    });
}

function destroyDataTable(tableId) {
    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        var table = $(id).dataTable();
        table.api().clear().draw();
        table.api().destroy();

    }
}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

function ActualizarSubTiposEditar() {

    $("#CmbSubTipoObjetoEditar").prop("disabled", true);

    var options = {};
    options.url = BASE_URL + "Infraestructura/GetSubTipos";
    options.type = "POST";
    options.data = JSON.stringify({ Id_Tipo: selection });
    options.dataType = "json";
    options.contentType = "application/json";
    options.success = function (result) {

        $("#CmbSubTipoObjetoEditar").empty();
        for (var i = 0; i < result.length; i++) {
            var subtipo = document.createElement("option");
            subtipo.value = result[i].ID_Subtipo_Objeto;
            subtipo.text = result[i].Nombre;
            $("#CmbSubTipoObjetoEditar").append(subtipo);
        }
        $("#CmbSubTipoObjetoEditar").prop('disabled', true);
    };
    options.error = function () { alert("Error cargando subtipos edición!"); };
    $.ajax(options);

    return false;
}

function ChangeValor() {
    var combo = $("#CmbAtributo option:selected").text();
    $("#valorContainer").html("");
    var selection = $("#CmbSubTipoObjeto").val();
    $("#valorContainer").load(`${BASE_URL}Infraestructura/ReturnCampo?valor=${combo}&idSubTipo=${selection}`, function () {
        $(".classdatepicker").datepicker(getDatePickerConfig())
            .on("changeDate", function () {
                $(this).datepicker(".classdatepicker", $(this).datepicker("getDate"));
            });
    });
}