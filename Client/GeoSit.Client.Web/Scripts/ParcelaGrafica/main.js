var GeoTag = {
    MapaController: null
};


var FeatIdManzana = 0;
var aux = true;

$(document).ready(initParcelaGrafica);
$(window).resize(ajustamodal);

function ajustamodal() {
    var altura = 510;
    $(".tagueador-body").css({ "height": altura });
    $(".tagueador-content").css({ "height": altura, "overflow": "hidden" });
    $("#panel-mapa > div").css({ "height": 430 });
    //ajustarScrollBars();
}

$('#modal-window-parcelagrafica').one('shown.bs.modal', function () {
    $('#Grilla_ParcelaGrafica').dataTable().api().columns.adjust();
    setTimeout(showMapa, 10);
    hideLoading();
});

function initParcelaGrafica() {
    $('#Grilla_ParcelaGrafica').dataTable({
        "scrollY": "350px",
        "paging": false,
        "searching": false,
        "bInfo": false,
        "columnDefs": [
            { "defaultContent": "-", "targets": "_all" },
            { "sortable": false, "targets": [3, 4] }
        ],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
        "createdRow": function (row) {
            $(row).on('click', function (evt) {
                if ($("td:first", row).hasClass("dataTables_empty") || $(evt.target).hasClass("fa-th")) {
                    return;
                }
                if ($(this).hasClass("selected")) {
                    $(this).removeClass("selected");
                    $("#btnAsociarGraficamente,#btnDesasociarGraficamente").addClass("hidden");
                    GeoTag.MapaController.limpiar();
                } else {
                    $("tr.selected", "#Grilla_ParcelaGrafica tbody").removeClass("selected");
                    $(this).addClass("selected");

                    var parcela = $("#Grilla_ParcelaGrafica").dataTable().api().row(".selected").data();
                    if (parcela[1] && parcela[1].length) {
                        GeoTag.MapaController.seleccionarObjetos([parcela[1]], [parcela[parcela.length - 1]]);
                        $("#btnAsociarGraficamente").addClass("hidden");
                        $("#btnDesasociarGraficamente").removeClass("hidden");
                    } else {
                        GeoTag.MapaController.limpiar();
                        $("#btnAsociarGraficamente").removeClass("hidden");
                        $("#btnDesasociarGraficamente").addClass("hidden");
                    }
                }
            });
        }
    });

    $("#btnAsociarGraficamente,#btnDesasociarGraficamente").addClass("hidden");

    $("#btnSearch").on("click", BuscarParcelas);

    $("#btnClearSearch").click(function () {
        $("#txtFiltroParcelas").val("");
        var tabla = $('#Grilla_ParcelaGrafica').dataTable().api();
        tabla.clear();
        tabla.draw();
        $("#btnAsociarGraficamente,#btnDesasociarGraficamente").addClass("hidden");
    });


    $("#btnAsociarGraficamente").click(function () {
        // Determina la identificación de la parcela.
        var parcela = $("#Grilla_ParcelaGrafica").dataTable().api().row(".selected").data(),
            parcelaid = "";

        try {
            parcelaid = parcela[0];
        } catch {
            parcelaid = "";
        }

        // Determina la identificación del objeto gráfico.
        var idGrafico = "";
        try {
            idGrafico = GeoTag.MapaController.obtenerSeleccion().seleccion[0][0];
        } catch (ex) {
            console.error(ex.message);
        }

        var mensaje = "";
        if (!idGrafico && !parcelaid) {
            mensaje = "Debe seleccionar una parcela gráfica";
        } else if (!idGrafico) {
            mensaje = "No pudo determinarse la identificación de la parcela gráfica en el mapa.";
        } else if (!parcelaid) {
            mensaje = "Debe seleccionar una parcela.";
        } else if (parcela[1] && parcela[1].length) {
            mensaje = "La parcela ya se encuentra asociada.";
        }
        if (mensaje) {
            mostrarMensaje("Información - Asociar Parcela", mensaje, 2);
        } else {
            showLoading();
            $.ajax({
                type: 'POST',
                url: BASE_URL + 'ParcelaGrafica/GetParcelaGrafByFeatidJson',
                dataType: 'json',
                data: { id: idGrafico },
                success: function (ParcelaGraf) {
                    if (ParcelaGraf.ParcelaID && Number(ParcelaGraf.ParcelaID) > 0) {
                        hideLoading();
                        mostrarMensaje("Información - Asociar Parcela", "Los datos gráficos ya se encuentran asociados.", 3);
                    } else {
                        saveParcela(idGrafico, parcelaid, "A");
                    }
                }, error: function (ex, error) {
                    hideLoading();
                    mostrarMensaje("Información - Asociar Parcela", error, 3);
                }
            });
        }
    });

    $("#btnDesasociarGraficamente").click(function () {
        // Determina la identificación de la parcela.
        var parcelaid = "";
        try {
            parcelaid = $("#Grilla_ParcelaGrafica").dataTable().api().row(".selected").data()[0];
        } catch (exception) {
            parcelaid = "";
        }
        if (!parcelaid) {
            mostrarMensaje("Advertencia - Desasociar Parcela", "Debe seleccionar una parcela.", 2);
        } else {
            mostrarMensaje("Advertencia - Desasociar Parcela", "¿Desea desasociar la parcela seleccionada?", 2, function () {
                var parcela = $("#Grilla_ParcelaGrafica").dataTable().api().row(".selected").data();
                $('#ModalAdvertenciaParcelaGraf').modal('hide');
                showLoading();
                //Le paso idGrilla en 0 para que la actualice con null
                saveParcela(parcela[1], null, "D");
            });
        }
    });

    $("#btnBuscarObjetoReferencia").on("click", function (evt) {
        buscarParcelaReferencia()
            .then(function (seleccion) {
                if (seleccion && seleccion.featids) {
                    GeoTag.MapaController.seleccionarObjetos([seleccion.featids], [seleccion.layer]);
                } else if (seleccion) {
                    mostrarMensaje('Buscar Parcela de Referencia', 'La parcela seleccionada no tiene gráfico asociado.', 3);
                } else {
                    mostrarMensaje('Buscar Parcela de Referencia', 'No se ha seleccionado ninguna parcela.', 2);
                }
            })
            .catch(function (err) {
                console.log(err);
            });
    });

    function buscarParcelaReferencia() {
        return new Promise(function (resolve) {
            var data = {
                tipos: `${BuscadorTipos.Parcelas},${BuscadorTipos.ParcelaSanear},${BuscadorTipos.Prescripciones}`,
                multiSelect: false,
                verAgregar: false,
                titulo: 'Buscar Parcela de Referencia',
                campos: ['Partida', 'nomenclatura:Nomenclatura'],
                retrieveFeatids: true
            };
            $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
                $(".modal", this).one('hidden.bs.modal', function () {
                    $(window).off('seleccionAceptada');
                });
                $(window).one("seleccionAceptada", function (evt) {
                    if (evt.seleccion) {
                        resolve({ featids: evt.seleccion[4], layer: evt.seleccion[5] });
                    } else {
                        resolve();
                    }
                });
            });
        });
    }

    ajustamodal();
    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-parcelagrafica .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-parcelagrafica").modal('show');
}

function saveParcela(idMapa, idGrilla, tipo) {
    let btnHabilitado = $("#btnAsociarGraficamente"),
        btnDeshabilitado = $("#btnDesasociarGraficamente"),
        titulo = "Asociar Parcela",
        mensaje = "Se ha realizado la asociación con éxito.";

    if (tipo === "D") {
        btnDeshabilitado = $("#btnAsociarGraficamente");
        btnHabilitado = $("#btnDesasociarGraficamente");
        titulo = "Desasociar Parcela";
        mensaje = "Se ha realizado la desasociación con éxito.";
    }
    $.ajax({
        type: "POST",
        url: BASE_URL + 'ParcelaGrafica/Save',
        data: { featid: idMapa, idparcela: idGrilla },
        success: function () {
            mensaje = `${mensaje}<br />Es posible que no se reflejen los cambios inmediatamente en el buscador ya que el mismo se está actualizando.`;
            mostrarMensaje(`Información - ${titulo}`, mensaje, 1);

            $("#Grilla_ParcelaGrafica").dataTable().api().row(".selected").data()[1] = idGrilla;
            $("#Grilla_ParcelaGrafica tr.selected td:eq(3) span").toggleClass("boton-deshabilitado");
            $("#Grilla_ParcelaGrafica tr.selected").removeClass("selected");
            GeoTag.MapaController.limpiar();
            GeoTag.MapaController.refrescar();

            btnHabilitado.addClass("hidden");
            btnDeshabilitado.addClass("hidden");
        },
        error: function () {
            mostrarMensaje(`Información - ${titulo}`, "No se pudo completar la operación.", 3);
        },
        complete: hideLoading
    });
}

function showMapa() {
    GeoTag.MapaController = new MapaController(3, "map-tag", true, false, true, false, true);
}

function fnClickMasInfo(valor) {
    showLoading();
    setTimeout(function () {
        $("#parcela-grafica-externo-container").load(BASE_URL + "MantenimientoParcelario/Get/" + valor);
    }, 10);
}

function runSearch(e) {
    if (e.keyCode === 13) {
        BuscarParcelas();
        return false;
    }
}

function ZoomToDivision(manzanaNomenc) {
    $.ajax({
        type: 'POST',
        url: BASE_URL + 'ParcelaGrafica/GetDivisionByNomenclaturaJson',
        dataType: 'json',
        data: { nomenclatura: manzanaNomenc },
        success: function (result) {
            if (result && result.length) {
                FeatIdManzana = result[0].FeatId;
                GeoTag.MapaController.seleccionarObjetos([[FeatIdManzana]], ["DIVISION"], true);
            }
        }, error: function (ex) {
            alert('Error al recuperar el objeto' + ex.responseText);
        }
    });
}

function BuscarParcelas() {
    if ($("#txtFiltroParcelas").val()) {
        showLoading();
        //var manzanaNomenc = $("#txtFiltroParcelas").val();

        //ZoomToDivision(manzanaNomenc);

        $.post(BASE_URL + "BuscadorGenerico/Suggests", { text: $("#txtFiltroParcelas").val(), tipos: "parcelas,prescripciones", listaCampos: "featids,idpadre,uid,capa" },
            function (resultSearch) {
                var tabla = $('#Grilla_ParcelaGrafica').dataTable().api();
                tabla.clear();
                var data = $.parseJSON(resultSearch.Result).response.docs;
                data.forEach(function (parcela) {
                    var asociado = '<span class="fa fa-thumb-tack black cursor-pointer ' + (!parcela.featids || !parcela.featids.length ? "boton-deshabilitado" : "") + '" aria-hidden="true"></span>';
                    var masInfo = '<span onclick="fnClickMasInfo(' + parcela.id + ')" class="fa fa-th black cursor-pointer" aria-hidden="true"></span>';
                    var node = tabla.row.add([parcela.id, parcela.featids, parcela.nombre, asociado, masInfo, parcela.uid, parcela.capa]).node();
                    $(node).find("td:first").addClass("hide");
                    $(node).find("td:eq(1)").addClass("hide");
                    $(node).find("td:eq(5)").addClass("hide");
                    $(node).find("td:last").addClass("hide");
                });
                tabla.draw();
                $("#btnDesasociarGraficamente", "#btnAsociarGraficamente").addClass("hidden");
                //if (data.length) {
                //    $("#btnDesasociarGraficamente", "#btnAsociarGraficamente").removeClass("hidden");
                //}
                ajustamodal();
                tabla.columns.adjust();
                hideLoading();
            });
    }
}

function mostrarMensaje(title, description, tipo, fn) {
    var modal = $('#ModalInfoParcelaGraf');
    var alertaBackground = $('div[role="alert"]', modal);
    $('.modal-title', modal).html(title);
    $('.modal-body p', modal).html(description);
    $('.modal-footer', modal).addClass("hidden");
    if (fn) {
        $('.modal-footer', modal).removeClass("hidden");
        $("#btnAceptarModalInfoParcelaGraf", modal).off().one("click", fn);
    }
    switch (tipo) {
        case 2:
            cls = 'alert-warning';
            break;
        case 3:
            cls = 'alert-danger';
            break;
        case 4:
            cls = 'alert-info';
            break;
        default:
            cls = 'alert-success';
            break;
    }
    alertaBackground.removeClass('alert-warning alert-success alert-danger alert-info').addClass(cls);
    modal.modal('show').one("hidden.bs.modal", function () { $("#btnAceptarModalInfoParcelaGraf", modal).off(); });
}
//@ sourceURL=parcelaGrafica.js