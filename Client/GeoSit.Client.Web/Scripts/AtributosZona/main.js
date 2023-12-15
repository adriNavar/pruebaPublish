var TipoOperacionId, IdZonaAtributo, FeatId, IdAtributoZona, Valor, UM, selectedRowZonas;
var IdAtributosListaDesplegable = [];
var cmbAtributosOptionsDefault;

$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-atributoszona').one('shown.bs.modal', function (e) {
    ajustarScrollBars();
    hideLoading();
    $('#Grilla_Atributos').dataTable().api().columns.adjust();
});

function ActualizarGrid() {
    $("#Grilla_Atributos").dataTable().api().ajax.reload();
}

function ActualizarListaDesplegableAtributos() {
    var idAtributosEnGrilla = [];

    /* 1) Colección de las IDs de las tr que estan en la grid */
    $("#Grilla_Atributos tbody tr").each(function () {
        idAtributosEnGrilla.push($(this).children('td').eq(1).text());
    });

    /* 2) Limpiamos la lista */
    CleanListaDesplegableAtributos();

    /* 3) La volvemos a crear, filtrando los elementos que no deben aparecer */
    for (var i = 0; i < IdAtributosListaDesplegable.length; i++) {
        if (idAtributosEnGrilla.indexOf(IdAtributosListaDesplegable[i]["val"]) === -1) {
            $('#CmbAtributosID').append($('<option>', {
                value: IdAtributosListaDesplegable[i]["val"],
                text: IdAtributosListaDesplegable[i]["text"]
            }));
        }
    }
}

function DisableAgregarButton() {
    $("#btn_Agregar").addClass("boton-deshabilitado");
}

function EnableAgregarButton() {
    $("#btn_Agregar").removeClass("boton-deshabilitado");
}

function CleanListaDesplegableAtributos() {
    $("#CmbAtributosID option").remove();
}

function GetObjetoAdministrativo() {
    $.ajax({
        type: "POST",
        url: BASE_URL + "ZonaAtributo/GetOAAtributo",
        dataType: "json",
        data: { FeatId: $('#CmbZonaID').val() },
        success: function (result) {
            $("#txtObservacionesId").val(result["Atributos"]);
        },
        error: function (ex) {
            alert(ex);
        }
    });
}

function ValControles() {
    if (!$("#CmbAtributosID").val()) {
        modalError("Error - Atributo", "El campo Atributo es obligatorio");
        return false;
    }
    if (!$("#txtValor").val()) {
        modalError("Error - Atributo", "El campo Valor es obligatorio");
        return false;
    }

    if ($("#txtValor").val().length > 50 || $("#txtValor").val().length < 0) {
        modalError("Error - Atributo", "El campo valor debe estar dentro del rango de 0 a 50");
        return false;
    }
    return true;
}

function modalConfirm(title, message) {
    $("#TituloAdvertencia").text(title);
    $("#DescripcionAdvertencia").text(message);
    $("#confirmModal").modal("show");
}

function modalExit(title, message) {
    $("#TituloExit").text(title);
    $("#DescripcionExit").text(message);
    $("#exitModal").modal("show");
}

function modalError(title, message) {
    $("#MensajeAlerta").removeClass("alert-success").addClass("alert-danger");
    $("#TituloError").text(title);
    $("#DescripcionError").text(message);
    $("#ErrorModal").modal("show");
}

function modalSuccess(title, message) {
    $("#MensajeAlerta").removeClass("alert-danger").addClass("alert-success");
    $("#TituloError").text(title);
    $("#DescripcionError").text(message);
    $("#ErrorModal").modal("show");
}

function createDataTable(tableId) {
    var scrollY = "150px";
    var options = {
        dom: "rt",
        scrollY: scrollY,
        destroy: true,
        columns: [
            { data: "Id_Zona_Atributo", "className": "hide" },
            { data: "Id_Atributo_Zona", "className": "hide" },
            { data: "FeatId_Objeto", "className": "hide" },
            { data: "Atributo.Descripcion" },
            { data: "Valor" },
            { data: "U_Medida" }
        ],
        ajax: {
            url: BASE_URL + "ZonaAtributo/MostrarAtributosZona",
            type: "POST",
            data: function () {
                return { FeatId: $("#CmbZonaID").val() };
            },
            dataSrc: function (data) {
                return data;
            }
        },
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        initComplete: function () {
            $('#CmbZonaID').change(function () {
                selectedLayerId = 0;
                selectedRowZonas = null;

                GetObjetoAdministrativo();
                ActualizarGrid();
            });

            var api = this.api();
            api.columns.adjust();
        }
    }
    $("#" + tableId)
        .on("xhr.dt", function (evt) {
            enableView();
            ActualizarListaDesplegableAtributos();
            if (IdAtributosListaDesplegable.length === $(evt.currentTarget).dataTable().api().data().length) {
                DisableAgregarButton();
            } else {
                EnableAgregarButton();
            }
            $("#MensajeSalida").addClass("hidden");
            $("#btn_Eliminar").addClass('boton-deshabilitado');
            $("#btn_Modificar").addClass('boton-deshabilitado');
        })
        .DataTable(options);
}

function destroyDataTable(tableId) {
    var table = $("#" + tableId).dataTable();
    table.api().destroy();
    $("#Grilla_Atributos tbody").empty();
}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

function enableEdicion(nuevo) {
    $("#footer-atributos [aria-controls='button']").addClass("boton-deshabilitado");
    $("#CmbZonaID").prop("disabled", true);
    $("#CmbAtributosID").prop("disabled", nuevo === false);
    $("#atributosZona-edicion").removeClass("hidden");
    $("#atributosZona-datos").addClass("hidden");
}

function enableView() {
    $("#footer-atributos [aria-controls='button']").removeClass("boton-deshabilitado");
    $("#CmbZonaID").prop("disabled", false);
    $("#atributosZona-datos").removeClass("hidden");
    $("#atributosZona-edicion").addClass("hidden");
}

function init() {
    ///////////////////// Scrollbars ////////////////////////
    $(".atributos-body").niceScroll(getNiceScrollConfig());
    ////////////////////////////////////////////////////////

    $("#CmbAtributosID option").each(function () {
        IdAtributosListaDesplegable.push({ val: $(this).val(), text: $(this).text() });
    });
    cmbAtributosOptionsDefault = $("#CmbAtributosID option");

    sortSelect("#CmbZonaID", "text", "asc");
    $('#CmbZonaID option').eq(0).prop('selected', true);

    GetObjetoAdministrativo();
    createDataTable("Grilla_Atributos");

    $("#Grilla_Atributos tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            $("#btn_Eliminar").addClass('boton-deshabilitado');
            $("#btn_Modificar").addClass('boton-deshabilitado');
            selectedLayerId = 0;
            selectedRowZonas = null;
        } else {
            $("tr.selected", "#Grilla_Atributos tbody").removeClass("selected");
            $(this).addClass("selected");

            selectedRowZonas = $(this);
            IdZonaAtributo = Number($(this).find("td:eq(0)").html());
            IdAtributoZona = Number($(this).find("td:eq(1)").html());
            FeatId = Number($(this).find("td:eq(2)").html());
            Valor = $(this).find("td:eq(4)").html();
            UM = $(this).find("td:eq(5)").html();

            $("#btn_Eliminar").removeClass('boton-deshabilitado');
            $("#btn_Modificar").removeClass('boton-deshabilitado');
        }
    });

    $("#btn_Eliminar").click(function () {
        if (selectedRowZonas) {
            var descripcion = selectedRowZonas.find("td").eq(3).html();
            //alert(descripcion);
            modalConfirm("Eliminar - Atributo", "¿Desea eliminar el atributo " + descripcion + "?");

            $("#btnAdvertenciaOK").click(function () {
                $.ajax({
                    url: BASE_URL + 'ZonaAtributo/DeleteAtributosZona/' + IdZonaAtributo,
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data["success"]) {
                            ActualizarGrid();
                            $("#btn_Eliminar").addClass('boton-deshabilitado');
                            $("#btn_Modificar").addClass('boton-deshabilitado');
                        }
                    }
                });
                $("#confirmModal").modal("toggle");
            });
        }
    });

    //Inicia la opción de agregar un nuevo atributo
    $("#btn_Agregar").click(function () {
        //Inicializar los controles de edición
        $('select[name^="CmbAtributos"] option[value="1"]').attr("selected", "selected");
        $("#txtValor").val(null);
        $("#txtUM").val(null);
        var FeatId = $("#CmbZonaID").val();
        TipoOperacionId = "Agregar";
        enableEdicion(true);
        ActualizarListaDesplegableAtributos();
    });

    //Inicia la modificación de un registro seleccionado
    $("#btn_Modificar").click(function () {
        if (IdZonaAtributo) {
            CleanListaDesplegableAtributos();
            $.each(cmbAtributosOptionsDefault, function (i, item) {
                $('#CmbAtributosID').append($('<option>', {
                    value: item.value,
                    text: item.text
                }));
            });
            $('select[name^="CmbAtributos"] option[value="' + IdAtributoZona + '"]').attr("selected", "selected");
            $("#txtValor").val(Valor);
            $("#txtUM").val(UM);
            TipoOperacionId = "Modificar";
            enableEdicion(false);
            //FormValidation(Reset) no funciona
            $(".form-group").removeClass("has-success");
            $(".form-group").removeClass("has-error");
            $(".help-block").css("display", "none");
        }
    });

    //Confirma la edición del atributo
    $("#atributo-save").click(function () {
        if (ValControles() === true) {
            if (TipoOperacionId === "Agregar") {
                $.ajax({
                    type: "POST",
                    url: BASE_URL + 'ZonaAtributo/AgregarAtributoZona',
                    data: {
                        Id_Zona_Atributo: 0,
                        Id_Atributo_Zona: $("#CmbAtributosID").val(),
                        Valor: $("#txtValor").val(),
                        U_Medida: $("#txtUM").val(),
                        FeatId_Objeto: $("#CmbZonaID").val()
                    },
                    success: function (sok) {
                        if (sok["success"] !== true) {
                            modalError("Error - Atributo", "Ya existe el atributo para la zona seleccionada");
                        }
                        ActualizarGrid();
                    },
                    error: function (ex) {
                        modalError('Agregar - Atributo', ex.statusText);
                    }
                });
            }
            else if (TipoOperacionId === "Modificar") {
                $.ajax({
                    type: "POST",
                    url: BASE_URL + 'ZonaAtributo/ModificarAtributoZona',
                    data: {
                        Id_Zona_Atributo: IdZonaAtributo,
                        Id_Atributo_Zona: $("#CmbAtributosID").val(),
                        Valor: $("#txtValor").val(),
                        U_Medida: $("#txtUM").val(),
                        FeatId_Objeto: $("#CmbZonaID").val()
                    },
                    success: function (sok) {
                        ActualizarGrid();
                    },
                    error: function (ex) {
                        modalError('Modificar - Atributo', ex.statusText);
                    }
                });
            }
            TipoOperacionId = null;
        }
    });

    $("#btnGrabar").click(function () {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'ZonaAtributo/PostAtributoZona',
            data: {
                FeatId: $("#CmbZonaID").val(),
                Observacion: $("#txtObservacionesId").val()
            },
            success: function (sok) {
                if (sok === true || sok === 'True') {
                    modalSuccess('Guardar - Zona', 'Se han guardado los cambios correctamente');
                }
                else {
                    modalError('Guardar - Zona', 'No se pudieron guardar los datos correctamente');
                }
            },
            error: function (ex) {
                modalError('Guardar - Zona', ex.statusText);
            }
        });
    });

    $("#btnCerrar").click(function () {
        modalExit("Salir de Matenimiento Atributos por Zona", "¿Esta Seguro?");

        $("#btnExitOk").click(function () {
            $('#modal-window-atributoszona').modal('toggle');
        })

        $("#btnCancelarExit").click(function () {
            $('#exitModal').modal('toggle');
        })

    });

    $("#atributo-cancel").click(function () {
        enableView();
    });

    $("#btnCancelarError").click(function () {
        enableView();
        $("#ErrorModal").modal("toggle");

    });
    ajustarmodal();
    $("#modal-window-atributoszona").modal("show");
    $('#CmbZonaID').change();

    $('#Form-Add').formValidation({
        framework: 'bootstrap',
        fields: {
            Valor: {
                validators: {
                    notEmpty: {
                        message: 'El campo es requerido'
                    },
                    stringLength: {
                        message: 'El campo no debe superar los 50 caracteres',
                        max: 50
                    }
                }
            }
        }
    });

}

function ajustarmodal() {
    var altura = $(window).height() - 220;
    $(".atributos-body", "#scroll-content-atributos").css({ "height": altura });
    $(".atributos-body", "#scroll-content-atributos").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $(".atributos-content").getNiceScroll().resize();
    $(".atributos-content").getNiceScroll().show();
}

function sortSelect(select, attr, order) {
    if (attr === 'text') {
        if (order === 'asc') {
            $(select).html($(select).children('option').sort(function (x, y) {
                return $(x).text().toUpperCase() < $(y).text().toUpperCase() ? -1 : 1;
            }));
            $(select).get(0).selectedIndex = 0;
        }// end asc
        if (order === 'desc') {
            $(select).html($(select).children('option').sort(function (y, x) {
                return $(x).text().toUpperCase() < $(y).text().toUpperCase() ? -1 : 1;
            }));
            $(select).get(0).selectedIndex = 0;
        }// end desc
    }

};
//@ sourceURL=atributoszona.js