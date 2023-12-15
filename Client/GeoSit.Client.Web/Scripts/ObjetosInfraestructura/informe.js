var ConsultaId, ConsultaComp, ConsultaAtributo, ConsultaValor, ConsultaConec, FeatId, selectedRowObjetos;

$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-infraestructura').on('shown.bs.modal', function (e) {
    ajustarScrollBars();
    hideLoading();
});

function init() {

    ///////////////////// Scrollbars ////////////////////////
    $("#infraestructura-datos").niceScroll(getNiceScrollConfig());
    $('#scroll-content-infraestructura .panel-body').resize(ajustarScrollBars);
    $('#infraestructura-datos .panel-heading').click(function () {
        setTimeout(function () {
            $("#infraestructura-datos").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////

    createDataTable("Grilla_Consulta");

    $("#Grilla_Consulta tbody").on("click", "tr", function () {
        if (!$(this).parents('table').DataTable().data().length) return;
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
            selectedLayerId = 0;
            $("#btn_EliminarQ").addClass('boton-deshabilitado');
            $("#btn_ModificarQ").addClass('boton-deshabilitado');
        } else {
            $("tr.selected", "#Grilla_Consulta tbody").removeClass("selected");
            $(this).addClass("selected");

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

    $("#confirmModal").bind("hidden.bs.modal", function () {
        $("html").addClass("modal-open");
    });

    $("#btnGrabar").click(function () {
        ActualizarGridObjetos();
    });

    $("#mensaje-salida-infraestructura").addClass('hidden');

    //*** CONTROLES EDITAR OBJETO
    ajustarmodal();
    $("#modal-window-infraestructura").modal("show");
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter;
    $(".infraestructura-body", "#scroll-content-infraestructura").css({ "height": altura, "overflow": "hidden" });
    $('#infraestructura-datos').css({ "max-height": altura + 'px' });
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
            $(subtipo).attr("data-layer", result[i].Layer)
            $("#CmbSubTipoObjeto").append(subtipo);
        }

        ActualizarAtributos();

        $("#CmbSubTipoObjeto").prop("disabled", false);
    };
    options.error = function () { alert("Error cargando subtipos!"); };
    $.ajax(options);
}

function ActualizarAtributos() {
    destroyDataTable("Grilla_Columnas");
    var selection = $("#CmbSubTipoObjeto").val();

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
        var scrollY = $(".infraestructura-body").height() - ($("#collapseCampos").position().top + $("#Grilla_Columnas").position().top + 20);
        createDataTable("Grilla_Columnas", {
            scrollY: scrollY + 'px',
            columns: [{ data: "nombre", name: "nombre" }],
            data: result.map(function (c) { return { nombre: c }; }),
            createdRow: function (row, data) {
                $(row).click(function () {
                    $(this).toggleClass("selected");
                    $(":checkbox", this).prop("checked", $(this).hasClass("selected"));
                }).addClass("selected")
                    .append('<input type="checkbox" value="' + data.nombre + '" name="colum" class="CheckColumn" style="display:none;" checked="checked">');
            },
            initComplete: function () {
                columnsAdjust("Grilla_Columnas");
            }
        });
        ActualizarGridEditor();
        ChangeValor();
    };
    options.error = function () { $("#CmbAtributo").empty(); };
    $.ajax(options);

    $("#CmbAtributo").prop("disabled", false);
}

function ActualizarGridEditor() {
    destroyDataTable("Grilla_Consulta");
    $.get(BASE_URL + "Infraestructura/_DetalleConsulta", {}, function (result) {
        $("#Grilla_Consulta tbody").empty();
        $("#Grilla_Consulta tbody").append($(result));

        createDataTable("Grilla_Consulta");
    });
}

function ActualizarGridObjetos() {
    var Tipo = $("#CmbTipoObjeto option:selected").text(),
        Sbtext = $("#CmbSubTipoObjeto option:selected").text(),
        SubTipo = $("#CmbSubTipoObjeto").val(),
        Ids = [];

    $('.CheckColumn:checked').each(function () {
        Ids.push($(this).val());
    });
    Ids.push(Tipo);
    Ids.push(Sbtext);
    Ids.push(SubTipo);

    showLoading();
    $.ajax({
        url: BASE_URL + "Infraestructura/GetFileInforme?array=" + Ids,
        type: 'GET',
        success: function (data) {
            if (data.success) {
                window.open(BASE_URL + "Infraestructura/GetFileInformeObjetosInfraestructura?file=" + data.file);
            } else {
                hideLoading();
                alerta('Advertencia', data.message, 3);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoading();
            alerta('Error', errorThrown, 3);
        }
    }).always(function () {
        hideLoading();
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

function modalConfirm(title, message) {
    $("#TituloAdvertencia").text(title);
    $("#DescripcionAdvertencia").text(message);
    $("#confirmModal").modal("show");
}

function createDataTable(tableId, opts) {
    var defaultOpts = { scrollY: "100px", dom: "t", language: { url: BASE_URL + "Scripts/dataTables.spanish.txt" } };
    Object.assign(defaultOpts, opts);
    $("#" + tableId).DataTable(defaultOpts);
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

function ChangeValor() {
    var combo = $("#CmbAtributo option:selected").text();
    $("#valorContainer").html("");
    var selection = $("#CmbSubTipoObjeto").val();
    $("#valorContainer").load(BASE_URL + "Infraestructura/ReturnCampo?valor=" + combo + "&idSubTipo=" + selection);
}

//@ sourceURL=ObjetosInfraestructura.js