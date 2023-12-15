var EnumEstadoTramite = {
    None: 0,
    Provisorio: 1,
    Presentado: 2,
    Iniciado: 3,
    En_proceso: 4,
    Derivado: 5,
    Anulado: 6,
    Despachado: 7,
    Rechazado: 8,
    Finalizado: 9,
    Archivado: 10,
    Procesado: 11
};
var file;
let __datosEspecificos__ = [];

$(document).ready(init);
$(window).resize(ajustarmodal);

$('#modal-EdicionTramite').one('shown.bs.modal', function () {
    setTimeout(ajustarScrollBars, 10);
});

var operacion = 'Consulta';
var iDesgloseNuevo = 0;
var iDocumentoNuevo = 0;

function init() {
    $(".tab-content .tab-body", "#pnlEdicionTramite").niceScroll(getNiceScrollConfig());

    $('a[data-toggle="tab"]', '#pnlEdicionTramite').on('shown.bs.tab', ajustarScrollBars);

    $("#cerrar").click(function () {
        $('#modal-EdicionTramite').modal("hide");
        $("#js_objetos").jstree("destroy");
    });

    $('#divDatosGenerales').bootstrapValidator({
        message: 'Este valor no es válido',
        excluded: ['[readonly]', ':disabled'],
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            IdPrioridad: {
                message: 'Prioridad no válida',
                validators: {
                    notEmpty: {
                        message: 'Debe seleccionar una prioridad.'
                    }
                }
            },
            IdJurisdiccion: {
                message: 'Jurisdicción no válida',
                validators: {
                    notEmpty: {
                        message: 'Debe seleccionar una jurisdicción.'
                    }
                }
            },
            IdTipoTramite: {
                message: 'Tipo no válido',
                validators: {
                    notEmpty: {
                        message: 'Debe seleccionar un tipo.'
                    }
                }
            },
            IdObjetoTramite: {
                message: 'Objeto no válido',
                validators: {
                    notEmpty: {
                        message: 'Debe seleccionar un objeto.'
                    }
                }
            },
            Motivo: {
                message: 'Motivo no válido',
                validators: {
                    notEmpty: {
                        message: 'Debe ingresar un motivo.'
                    }
                }
            },
            txtIniciador: {
                excluded: 'false',
                validators: {
                    notEmpty: {
                        message: 'Debe seleccionar un iniciador.'
                    }
                }
            }
        }
    });

    var template = '<div class="tooltip blue-tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>';
    $("[data-toggle='tooltip'].blue-tooltip").tooltip({ trigger: "hover", container: "body", template: template });

    $("#cboObjetoTramite")
        .select2()
        .on("select2-selecting", (evt) => {
            if (getDatosEspecificos().length) {
                evt.preventDefault();
                ((event) => {
                    $("#cboObjetoTramite").select2("close");
                    modalConfirmAcciones("Esta acción borrará los Datos Específicos cargados.<br />¿Desea continuar?", "Cambiar objeto de trámite");
                    $("#btnAceptarAdvertenciaAcciones").off("click").one("click", function () {
                        $(event.target).val(event.choice.id);
                        $(event.target).trigger("change");
                        setDatosEspecificos([]);
                        resetAndReloadTree();
                        $('#divDatosGenerales').data("bootstrapValidator").validate();
                    });
                })(evt);
                return false;
            }
        });

    $("#persona-search").click(function () {
        buscarPersonas()
            .then(function (seleccion) {
                if (seleccion) {
                    $.post(BASE_URL + "Persona/GetDatosPersonaJson/" + seleccion, function (persona) {
                        $("#txtIniciador").val(persona.NombreCompleto);
                        $("#hdnIdPersona").val(persona.PersonaId);
                        $('#divDatosGenerales').data("bootstrapValidator").resetField("txtIniciador");
                        $("#divDatosGenerales").data("bootstrapValidator").validateField("txtIniciador");
                    });
                } else {
                    mostrarMensaje(2, 'Buscar Iniciador', 'No se ha seleccionado ningún iniciador.');
                    return;
                }
            })
            .catch(function (err) {
                console.log(err);
            });
    });

    $("#partida-search").off("click").on("click", function () {
        buscarUnidadesTributarias()
            .then(function (seleccion) {
                let partida = [seleccion[0]];
                document.getElementById("partida").value = partida;
                document.getElementById("idUnidadTributaria").value = seleccion[1];
                $.ajax({
                    type: "POST",
                    url: BASE_URL + "MesaEntradas/GetPersonaByIdUt?idUnidadTributaria=" + seleccion[1],
                    success: function (persona) {
                        if (persona) {
                            $("#txtTitular").val(persona);
                        } else {
                            $("#txtTitular").val("");
                        }
                    },
                    error: function (_, __, errorThrown) {
                        hideLoading();
                        alerta('Error', errorThrown, 3);
                    }
                });
            })
            .catch(function (err) {
                console.log(err);
            });
    });


    inicializarGrillaEdicionTramite('Grilla_movimientos');
    inicializarGrillaEdicionTramite('Grilla_documentos', {
        columnDefs: [
            { "targets": [0, 6, 7], "visible": false },
            { "targets": [3, 4], "width": "auto" },
            {
                "targets": 5, "render": function (data) {
                    return "<a href='javascript:void(0);' onclick='downloadFile(this);'>" + data + "</a>";
                }
            },
            {
                "targets": 7, "render": function (data) {
                    return "<span class='fa fa-lg fa-ellipsis-h' title='" + data + "'></span>";
                }
            }
        ],
        createdRow: function (row) {
            $(row).on('click', rowClickNotas);
        }
    });
    inicializarGrillaEdicionTramite('Grilla_desgloses', {
        columnDefs: [
            { "targets": [0, 7], "visible": false },
            { "targets": 5, "width": "auto" },
            {
                "targets": 6, "render": function (data) {
                    return "<span class='fa fa-lg fa-ellipsis-h' title='" + data + "'></span>";
                }
            }
        ],
        createdRow: function (row) {
            $(row).on('click', rowClickDesglose);
        }
    });

    showLoading();

    var promises = [];

    promises.push(cargarObjetosTramites(document.querySelector("#cboTipoTramite").value, true));
    promises.push(cargarPrioridadesTramites(document.querySelector("#cboTipoTramite").value));
    promises.push(jurisdiccionChange($("#cboJurisdiccion").val()));
    promises.push(loadDatosEspecificos());

    Promise.all(promises)
        .then(function () {
            operacion = $("#hdnOperacion").val();
            if (operacion === 'Consulta') {
                modoEdicionEnableControls(false);
            } else {
                modoEdicionEnableControls(true);
            }
            ajustarmodal();
            $('#modal-EdicionTramite').modal("show");
        })
        .catch(function (err) {
            console.log(err);
        })
        .finally(hideLoading);
}

function inicializarGrillaEdicionTramite(grilla, options) {
    var defaults = {
        language: {
            url: BASE_URL + 'Scripts/dataTables.spanish.txt'
        },
        dom: "t<'row'<'col-sm-6'i><'col-sm-6'p>>",
        orderCellsTop: true,
        fixedHeader: true,
        columnDefs: [
            { "targets": 0, "visible": false }
        ]
    };
    $('#' + grilla).dataTable(Object.assign(defaults, options || {}));
}

function rowClickDesglose(evt) {
    if (!evt) return;
    var tabla = $(evt.currentTarget).parents("table");
    $("tbody tr", tabla).not(evt.currentTarget).removeClass('selected');
    $(evt.currentTarget).toggleClass('selected');
    operacion = $("#hdnOperacion").val();
    if (operacion !== "Consulta") {
        $("#btnDesgloseEliminar").removeClass("boton-deshabilitado");
    }
}

function rowClickNotas(evt) {
    if (!evt) return;
    $("#btnEliminarNota").addClass("boton-deshabilitado");
    $("#btnEditarNota").addClass("boton-deshabilitado");
    var tabla = $(evt.currentTarget).parents("table");
    $("tbody tr", tabla).not(evt.currentTarget).removeClass('selected');
    $(evt.currentTarget).toggleClass('selected');
    var selectedRow = $("#Grilla_documentos").dataTable().api().row(".selected").data();
    if (selectedRow) {
        $.get(BASE_URL + `MesaEntradas/EsNotaEditable/${selectedRow[0]}`, () => {
            $("#btnEliminarNota").removeClass("boton-deshabilitado");
            $("#btnEditarNota").removeClass("boton-deshabilitado");
        });
    }
}

function modoEdicionEnableControls(enable) {
    if (enable) {
        $("#divTramite *").prop("disabled", false);
    } else {
        $("#divTramite *").prop("disabled", true);
        $("#btnDesgloseAgregar").removeClass("btn-habilitado").addClass("boton-deshabilitado");
        $("#btn_AgregarObjeto").addClass("boton-deshabilitado");
        $("#btn_ModificarObjeto").addClass("boton-deshabilitado");
        $("#btn_EliminarObjeto").addClass("boton-deshabilitado");
    }
}

function buscarPersonas() {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Personas, multiSelect: false, verAgregar: true, titulo: 'Buscar Iniciador', campos: ['Nombre', 'dni:DNI'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
                $(window).off('agregarObjetoBuscado');
                $("#buscador-container").empty();
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    resolve(evt.seleccion[2]);
                } else {
                    resolve();
                }
            });
            $(window).one("agregarObjetoBuscado", function () {
                showLoading();
                $("#container-externo").load(BASE_URL + "Persona/BuscadorPersona", function () {
                    $(".modal.mainwnd", this).one('hidden.bs.modal', function () {
                        $(window).off('personaAgregada');
                    });
                    $(window).one("personaAgregada", function (evt) {
                        resolve(evt.persona.PersonaId);
                    });
                });
            });
        });
    });
}

function buscarParcelas() {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.Parcelas, multiSelect: false, verAgregar: false, titulo: 'Buscar Parcela', campos: ['Partida', 'nomenclatura:Nomenclatura'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
                $("#buscador-container").empty();
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    resolve(evt.seleccion.slice(1));
                } else {
                    resolve();
                }
            });
        });
    });
}

function buscarUnidadesTributarias() {
    return new Promise(function (resolve) {
        var data = { tipos: BuscadorTipos.UnidadesTributarias, multiSelect: false, verAgregar: false, titulo: 'Buscar Partida', campos: ['Partida'] };
        $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
            $(".modal", this).one('hidden.bs.modal', function () {
                $(window).off('seleccionAceptada');
                $("#buscador-container").empty();
            });
            $(window).one("seleccionAceptada", function (evt) {
                if (evt.seleccion) {
                    resolve(evt.seleccion.slice(1));
                } else {
                    resolve();
                }
            });
        });
    });
}


function personaCall() {
    $.ajax({
        url: BASE_URL + "Persona/DatosPersona",
        type: 'POST',
        dataType: 'html',
        success: function (result) {
            $("#personas-externo-container").html(result);
        },
        error: function (_, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
        }
    });
}

function disableTree() {
    $('#js_objetos li').each(function () {
        $("#js_objetos").jstree().disable_node(this.id);
    });
}

function enableTree() {
    $('#js_objetos li').each(function () {
        $("#js_objetos").jstree().enable_node(this.id);
    });
}

$("#btn_Guardar").click(function (e) {

    e.preventDefault();

    var isValid = $('#divEdicionObjetoContainer').valid();
    if (!isValid) {
        return;
    }

    var guid = document.getElementById("hdnGuid").value;
    var idTramiteEntrada = document.getElementById("hdnIdTramiteEntrada").value;

    let datos = getDatosEspecificos();
    const obj = datos.find(e => e.Guid === guid);
    if (obj) {
        datos.splice(datos.indexOf(obj), 1, armarNodo(guid, isNaN(parseInt(idTramiteEntrada)) ? null : parseInt(idTramiteEntrada), obj.ParentGuids));
    } else {
        let guids = [];
        if (getTreeSelectedElements().length) {
            const selected = getTreeSelectedElements().map(elem => elem.data);
            guids = selected.map(elem => elem.Guid);
        }
        datos = [...datos, armarNodo(uuidv4(), null, guids)];
    }
    setDatosEspecificos(datos)

    //Agrego vuelta atras de los cambios realizados
    if ($('#TipoOperacion').val() === 'Modificar') {
        $('#TipoOperacion').val('');
        localStorage.setItem("datosEspecificosOri", []);
    }

    $("#js_objetos").jstree().deselect_all(true);
    resetAndReloadTree();
});

$("#btn_Cerrar").click(function () {
    $("#btnAceptarAdvertenciaCancelar").off("click").one("click", function () {
        //Agrego vuelta atras de los cambios realizados
        if ($('#TipoOperacion').val() === 'Modificar') {
            const storedArray = JSON.parse(localStorage.getItem("datosEspecificosOri")) || [];
            if (storedArray.length > 1) {
                setDatosEspecificos(storedArray);
            }
            $('#TipoOperacion').val('');
            localStorage.setItem("datosEspecificosOri", JSON.stringify([]));
        }
        setModoLectura();
        $("#js_objetos").jstree().deselect_all(true);
        $("#js_objetos").jstree().select_node(getTreeSelectedElements());
    });
    $("#mensajeModalCancelarTramite").modal("show");
});

function buscarEntradas(idObjetoTramite) {
    return new Promise((ok, error) => {
        $.get(BASE_URL + 'mesaentradas/GetEntradasByObjeto', { idObjetoTramite: idObjetoTramite })
            .done(function (data, _, resp) {
                if (resp.status === 204) {
                    ok();
                    return;
                }
                $("#cboObjetoEspecificoTramite").empty();
                const entradas = JSON.parse(data);
                if (entradas && entradas.length) {
                    [{ IdObjetoTramite: "", Descripcion: "- Seleccione -" }, ...entradas || []].forEach(function (e) {
                        $("#cboObjetoEspecificoTramite").append("<option value='" + e.IdEntrada + "'>" + e.Descripcion + "</option>");
                    });
                }
                ok(entradas);
            }).fail(error);
    });

}
function setModoEdicion() {
    $("#btn_EliminarObjeto").addClass("boton-deshabilitado");
    $("#divjsObjetos").attr("disabled", "true");
    disableButtonsOnAddOrEdit();
    disableTree();
}

function setModoLectura() {
    $("#divjsObjetos").removeAttr("disabled");
    enableTree();
    enableButtonsOnAddOrEdit();
}

function setModoEliminar() {
    $("#btn_EliminarObjeto").addClass("boton-deshabilitado");
    $("#btn_ModificarObjeto").addClass("boton-deshabilitado");
    $("#btn_AgregarObjeto").removeClass("boton-deshabilitado");
    document.getElementById("divEdicionObjetos").style.display = "none";
    $("#divjsObjetos").removeAttr("disabled");
    enableTree();
    resetAndReloadTree();
}

function grabar() {
    guardarTramite(false, false, false);
}

function confirmar() {
    var libroDiarioAbierto = $("#hdnLibroDiarioAbierto").val();
    if (libroDiarioAbierto === '1') {
        guardarTramite(true, false, false);
    }
    else {
        mostrarMensaje(3, "Confirmar", "No se puede Confirmar. El Libro Diario se encuentra Cerrado.");
    }
}

function presentar() {
    guardarTramite(false, true, false);
}

function reingresar() {
    guardarTramite(false, false, true);
}

function edicionEjecutar(accion) {
    showLoading();
    const idTramite = $("#IdTramite").val();
    if (accion === EnumAcciones.Imprimir_Caratula) {
        window.open(`${BASE_URL}MesaEntradas/GetInformeCaratulaExpediente/${idTramite}`, "_blank");
    } else if (accion === EnumAcciones.Imprimir_Informe_Detallado) {
        $.get(`${BASE_URL}MesaEntradas/GetInformeDetallado/${idTramite}`, () => window.open(`${BASE_URL}MesaEntradas/AbrirInformeDetallado/`, "_blank"));
    }
    hideLoading();
}

function setearValorPrevio(cbo) {
    $(cbo).data("valorPrevio", cbo.value);
}

function tipoTramiteChange(cbo, forzar) {
    const validador = $('#divDatosGenerales').data("bootstrapValidator");
    validador.updateStatus("IdPrioridad", "NOT_VALIDATED");
    validador.updateStatus("IdObjetoTramite", "NOT_VALIDATED");
    new Promise((ok) => {
        const datos = getDatosEspecificos();
        if (forzar || !datos.length || $(cbo).data("valorPrevio") == null) {
            Promise.all([cargarObjetosTramites(cbo.value), cargarPrioridadesTramites(cbo.value)])
                .then(() => ok(!forzar));
        }
        else if (!forzar) {
            ((newvalue) => {
                modalConfirmAcciones("Esta acción borrará los Datos Específicos cargados.<br />¿Desea continuar?", "Cambiar objeto de trámite");
                $("#btnAceptarAdvertenciaAcciones").off("click").one("click", function () {
                    Promise.all([cargarObjetosTramites(newvalue), cargarPrioridadesTramites(newvalue)])
                        .then(() => {
                            cbo.value = newvalue;
                            setDatosEspecificos([]);
                            resetAndReloadTree();
                            ok(true);
                        });
                });
            })(cbo.value);
            cbo.value = $(cbo).data("valorPrevio");
        }
    });
}
function setDatosEspecificos(datos = []) {
    __datosEspecificos__ = datos;
}
function getDatosEspecificos() {
    return __datosEspecificos__;
}
function objetoTramiteChange(val, cleanStorage) {
    if (val) {
        buscarEntradas(val)
            .then(entradas => {
                if (entradas && entradas.length) {
                    $('a[href="#tabDatosEspecificos"]').parent().removeClass("disabled");
                } else {
                    $('a[href="#tabDatosEspecificos"]').parent().addClass("disabled");
                }
            }).catch(error => {
                $('a[href="#tabDatosEspecificos"]').parent().addClass("disabled");
                console.log(error);
            });

        if (cleanStorage) {
            setDatosEspecificos([]);
        }

        cargarrequisitosporobjetos(val);
        GetPlantillaArbol(val);
        GetPlantillaObjeto(val);
    }
}

function GetPlantillaObjeto(idObjetoTramite) {
    $("#plantillaLoading").show();
    $("#plantillaObjeto").hide();
    $.ajax({
        type: "POST",
        url: BASE_URL + "MesaEntradas/GetPlantilla?idObjetoTramite=" + idObjetoTramite,
        success: function (objetoTrmaite) {
            var titulos = objetoTrmaite.Plantilla;
            if (titulos) {
                $("#plantillaObjeto").removeClass('boton-deshabilitado');
                $("#plantillaObjeto").attr('data-original-title', titulos);
            } else {
                $("#plantillaObjeto").addClass('boton-deshabilitado');
            }
        },
        error: function (_, __, errorThrown) {
            hideLoading();
            alerta('Error', errorThrown, 3);
            $("#plantillaObjeto").addClass('boton-deshabilitado');
        },
        complete: function () {
            $("#plantillaObjeto").show();
            $("#plantillaLoading").hide();
        }
    });
}

function GetPlantillaArbol(idObjetoTramite) {
    $.ajax({
        type: "POST",
        url: BASE_URL + "MesaEntradas/GetPlantilla?idObjetoTramite=" + idObjetoTramite,
        success: function (objetoTrmaite) {
            var titulos = objetoTrmaite.Plantilla;
            if (titulos) {
                $("#plantillaArbol").removeClass('boton-deshabilitado');
                $("#plantillaArbol").attr('data-original-title', titulos);
            } else {
                $("#plantillaArbol").addClass('boton-deshabilitado');
            }
        },
        error: function (_, __, errorThrown) {
            hideLoading();
            alerta('Error', errorThrown, 3);
        }
    });
}

function cargarrequisitosporobjetos(idObjeto) {
    $(".divChkRequisitos").html("");

    $.get(BASE_URL + 'mesaentradas/GetRequisitosTramitesByObjeto', { idObjetoTramite: idObjeto, idTramite: $("#IdTramite").val() }, function (requisitos) {
        if (requisitos && requisitos.length > 0) {
            var chksRequisitosHtml = '';
            var nObligatorio = 1;
            (requisitos || []).forEach(function (requisito) {
                if (parseInt(requisito.Obligatorio) > 1 && parseInt(requisito.Obligatorio) !== nObligatorio) {
                    chksRequisitosHtml += '<hr/>';
                    nObligatorio = parseInt(requisito.Obligatorio);
                }

                if (parseInt(requisito.Obligatorio) === 0) {
                    chksRequisitosHtml += '<div class="form-group row"><div class="col-xs-6"><label style="color:black; font-weight:bold;"><input id="check"  type="checkbox" value="' + requisito.IdObjetoRequisito + '" ' + (requisito.Cumplido === 1 ? 'checked' : ' ') + ($("#hdnOperacion").val() === "Consulta" ? ' disabled ' : ' ') + '>&nbsp; ' + requisito.Descripcion + '</label>&nbsp;</div></div> ';
                } else if (parseInt(requisito.Obligatorio) === 1) {
                    chksRequisitosHtml += '<div class="form-group row"><div class="col-xs-6"><label style="color:red; font-weight:bold;"><input id="check"  type="checkbox" value="' + requisito.IdObjetoRequisito + '" ' + (requisito.Cumplido === 1 ? 'checked' : ' ') + ($("#hdnOperacion").val() === "Consulta" ? ' disabled ' : ' ') + '>&nbsp; ' + requisito.Descripcion + '</label>&nbsp;</div></div> ';
                } else if (parseInt(requisito.Obligatorio) > 1) {
                    chksRequisitosHtml += '<div class="form-group row"><div class="col-xs-6"><label style="color:orange; font-weight:bold;"><input id="check"  type="checkbox" " value="' + requisito.IdObjetoRequisito + '" ' + (requisito.Cumplido === 1 ? 'checked' : ' ') + ($("#hdnOperacion").val() === "Consulta" ? ' disabled ' : ' ') + '>&nbsp; ' + requisito.Descripcion + '</label>&nbsp;</div></div> ';
                }
            });
        }
        else {
            chksRequisitosHtml = '<label style="color:red; font-weight:bold;"> No hay requisitos asociados</label>';
        }

        $(".divChkRequisitos").html(chksRequisitosHtml);
    });
}

function cargarRequisitos(tipoTramite) {
    $(".divChkRequisitos").html("");
    if (tipoTramite) {
        $.get(BASE_URL + 'mesaentradas/cargarrequisitos', { idTipoTramite: tipoTramite, idTramite: $("#IdTramite").val() }, function (data) {
            var requisitos = data.data;
            if (requisitos && requisitos.length > 0) {
                var chksRequisitosHtml = '';
                (requisitos || []).forEach(function (requisito) {
                    chksRequisitosHtml += '<div class="form-group row"><div class="col-xs-6"><label' + (requisito.Obligatorio == 1 ? ' style="color:red; font-weight:bold;"' : '') + '><input type="checkbox" value="' + requisito.IdRequisitoTramite + '" ' + (requisito.Cumplido == 1 ? 'checked' : ' ') + ($("#hdnOperacion").val() == "Consulta" ? ' disabled ' : ' ') + '>&nbsp; ' + requisito.Descripcion + '</label>&nbsp;</div></div> ';
                });
                $(".divChkRequisitos").html(chksRequisitosHtml);
            }
        });
    }
}

function cargarObjetosTramites(tipoTramite, isInitialLoad = false) {
    $("#cboObjetoTramite").val("");
    $("#cboObjetoTramite").empty();
    if (!tipoTramite) {
        return Promise.resolve();
    }
    return new Promise((ok) => {
        $("#plantillaObjeto").addClass('boton-deshabilitado');
        $("#plantillaArbol").addClass('boton-deshabilitado');
        $.get(BASE_URL + 'mesaentradas/RecuperarObjetosTramiteByTipo', { idTipoTramite: tipoTramite }, function (data) {
            var objetosTramite = data.data;
            if (objetosTramite.length) {
                [{ IdObjetoTramite: "", Descripcion: "- Seleccione -" }, ...(objetosTramite || [])].forEach(function (objetoTramite) {
                    $("#cboObjetoTramite").append("<option value='" + objetoTramite.IdObjetoTramite + "'>" + objetoTramite.Descripcion + "</option>");
                });
                $("#cboObjetoTramite").val($("#hdnIdObjetoTramite").val());
                $("#hdnIdObjetoTramite").val("");
                $("#cboObjetoTramite").select2().val();
            }

            $("#cboObjetoEspecificoTramite").empty();
            objetoTramiteChange(document.getElementById('cboObjetoTramite').value, !isInitialLoad);
        }).done(ok);
    });

}

function cargarPrioridadesTramites(tipoTramite) {
    $("#cboPrioridad").val("");
    $("#cboPrioridad").empty();
    if (!tipoTramite) {
        return Promise.resolve();
    }
    return new Promise((ok) => {
        $.get(BASE_URL + 'mesaentradas/RecuperarPrioridadesByTipo', { idTipoTramite: tipoTramite }, function (data) {
            var prioridadesTramite = data.data;
            if (prioridadesTramite.length) {
                [{ IdPrioridadTramite: "", Descripcion: "- Seleccione -" }, ...(prioridadesTramite || [])].forEach(function (prioridadTramite) {
                    $("#cboPrioridad").append("<option value='" + prioridadTramite.IdPrioridadTramite + "'>" + prioridadTramite.Descripcion + "</option>");
                });
                $("#cboPrioridad").val($("#hdnIdPrioridadTramite").val());
                $("#hdnIdPrioridadTramite").val("");
            }
        }).done(ok);
    });

}

function jurisdiccionChange(jurisdiccionSelected) {
    cargarLocalidadesJurisdiccion(jurisdiccionSelected);
}

function cargarLocalidadesJurisdiccion(jurisdiccion) {
    $("#cboLocalidad").empty();
    if (!jurisdiccion) {
        return Promise.resolve();
    }
    return new Promise((ok) => {
        $.get(BASE_URL + 'mesaentradas/RecuperarLocalidadesByJurisdiccion', { idJurisdiccion: jurisdiccion }, function (data) {
            var localidades = data.data;
            if (localidades.length) {
                [{ IdLocalidad: "", Descripcion: "- Seleccione -" }, ...(localidades || [])].forEach(function (localidad) {
                    $("#cboLocalidad").append("<option value='" + localidad.IdLocalidad + "'>" + localidad.Descripcion + "</option>");
                });
                $("#cboLocalidad").val($("#hdnIdLocalidad").val());
                $("#hdnIdLocalidad").val("");
            }
        }).done(ok);
    });

}

function armarNodo(guid, idTramiteEntrada, parentSelectedGuids) {
    var cboObjetoTramite = document.getElementById("cboObjetoEspecificoTramite");
    var divEdicionObjetoContainer = document.getElementById("divEdicionObjetoContainer");
    var inputElements = divEdicionObjetoContainer.querySelectorAll("input, textarea, select");

    var propiedadesStorage = [];
    [].forEach.call(inputElements, function (element) {
        var item = {};
        item["Id"] = element.id;
        item["Value"] = element.value;
        item["Text"] = $("option:selected", element).text();
        item["Visible"] = element.classList.contains("visibleArbolObjetos");
        item["Label"] = $("label[for=" + element.id + "]:not(.error)").text();
        propiedadesStorage.push(item);
    });

    return {
        "Guid": guid, //Usado solamente en el alta
        "ParentGuids": parentSelectedGuids, //Usado solamente en el alta
        "IdTramiteEntrada": idTramiteEntrada, //Usado solamente en la edicion
        "TipoObjeto": {
            "Id": cboObjetoTramite.options[cboObjetoTramite.selectedIndex].value,
            "Text": cboObjetoTramite.options[cboObjetoTramite.selectedIndex].text,
        },
        "Propiedades": propiedadesStorage
    };
}

function agregarNodoEnArbol(nodo, $tree) {
    if (!$tree) {
        $tree = $("#js_objetos");
    }
    const crearNodoArbol = (nodo, nodosPadre) => {
        const propiedades = nodo.Propiedades
            .filter(prop => prop["Visible"])
            .reduce((propsMostrables, prop) => {
                let texto;
                if (prop["Text"] && prop["Text"] != "...") {
                    texto = `${prop["Text"]}`;
                } else {
                    texto = prop["Id"] === "Descripcion" ? "..." : `${prop["Value"]}`;
                }
                return [...propsMostrables, texto];
            }, []);

        const data = { text: `${nodo.TipoObjeto.Text.toUpperCase()}: ${propiedades.join(" - ")}`, data: nodo };
        if (!nodosPadre) { //es nodo raiz
            $tree.jstree('create_node', null, data);
        } else {
            nodosPadre.forEach((nodoPadre) => $tree.jstree('create_node', nodoPadre, { ...data }));
        }
    };

    if (Array.isArray(nodo.ParentGuids) && nodo.ParentGuids.length) {
        nodo.ParentGuids.forEach(function (nodoPadre) {
            var all = $('#js_objetos').jstree(true).get_json('#', { 'flat': true });
            crearNodoArbol(nodo, all.filter(x => x.data.Guid === nodoPadre).map(y => y.id));
        });
    } else {
        crearNodoArbol(nodo);
    }
}

function guardarTramite(isConfirmar, isPresentar, isReingresar) {
    var form = $("#divDatosGenerales");
    var bootstrapValidator = form.data("bootstrapValidator");
    bootstrapValidator.validate();

    if (bootstrapValidator.isValid()) {
        showLoading();

        var model = {
            IdTramite: $("#IdTramite").val(),
            Numero: $("#Numero").val(),
            FechaIngreso: $("#FechaIngreso").val(),
            IdEstado: $("#hdnIdEstado").val(),
            IdPrioridad: $("#cboPrioridad").val(),
            FechaLibro: $("#FechaLibro").val(),
            FechaVenc: $("#FechaVenc").val(),
            IdJurisdiccion: $("#cboJurisdiccion").val(),
            IdLocalidad: $("#cboLocalidad").val(),
            IdTipoTramite: $("#cboTipoTramite").val(),
            IdObjetoTramite: $("#cboObjetoTramite").val(),
            Motivo: $("#Motivo").val(),
            IdIniciador: $("#hdnIdPersona").val(),
            IdUnidadTributaria: $("#idUnidadTributaria").val()
        };
        var aTramiteRequisito = [];
        $(".divChkRequisitos input[type='checkbox']:checked").each(function () {
            aTramiteRequisito.push($(this).val());
        });

        //desgloses
        var aDesglose = [];
        $('#Grilla_desgloses').DataTable().rows().every(function () {
            var d = this.data();
            var desgloseModel = {
                IdDesglose: d[0].substring(0, 1) === "d" ? 0 : parseInt(d[0]),
                IdTramite: $("#IdTramite").val(),
                FolioDesde: parseInt(d[2]),
                FolioHasta: parseInt(d[3]),
                FolioCant: parseInt(d[4]),
                Observaciones: d[6],
                IdDesgloseDestino: d[7]
            };
            aDesglose.push(desgloseModel);
        });
        var aTramiteDocumento = $('#Grilla_documentos').DataTable().rows().data().toArray().map(function (row) {
            var arch = row[5].split('.'),
                archNombre = arch.slice(0, arch.length - 1).join('.'),
                archExtension = row[5].replace(archNombre, '');

            return {
                id_documento: row[0].substring(0, 1) === "d" ? 0 : parseInt(row[0]),
                IdTramite: $("#IdTramite").val(),
                FechaAprobacion: row[7],
                Documento: {
                    id_documento: row[0].substring(0, 1) === "d" ? 0 : parseInt(row[0]),
                    descripcion: row[4],
                    nombre_archivo: archNombre,
                    extension_archivo: archExtension,
                    id_tipo_documento: row[6],
                    observaciones: row[8]
                }
            };
        });
        var accion = isConfirmar ? "Confirmar" : "Grabar";
        $.post(BASE_URL + 'mesaentradas/tramitesave', {
            model: model,
            tramitesRequisitos: aTramiteRequisito,
            desgloses: aDesglose,
            tramitesDocumentos: aTramiteDocumento,
            datosEspecificos: getDatosEspecificos(),
            esConfirmacion: isConfirmar,
            esPresentacion: isPresentar,
            esReingresar: isReingresar
        }).done(function (data) {
            let resultado = 1, texto = "", reload;
            if (!!data && data.error) {
                texto = isConfirmar ? "No se ha confirmado el trámite." : "El trámite no puede guardarse.";
                resultado = 3;
                texto = [texto + " Por favor verifique lo siguiente:", ...["", ...data.mensajes]].join("<br />");
            } else {
                reload = () => {
                    showLoading();
                    $(window).trigger("RefrescarGrilla");
                    $("#divEdicionTramiteContainer").load(BASE_URL + "MesaEntradas/ReloadTramite");
                };
                texto = isConfirmar ? "Se ha confirmado el trámite." : "Los cambios se guardaron correctamente.";
                if (!!data) {
                    resultado = 2;
                    texto = [texto + " Tenga en cuenta que:", ...["", ...data.mensajes]].join("<br />");
                }
            }
            mostrarMensaje(resultado, accion, texto, reload);
        }).fail(function () {
            mostrarMensaje(3, accion, `Ha ocurrido un error al ${accion.toLowerCase()} el trámite.`);
        }).always(hideLoading);
    }
    else {
        $('.nav-tabs a[href="#tabDatosGenerales"]').tab('show');
    }
}
$("#btnDesgloseAgregar").click(function () {
    showModalDesgloses('Agregar Desglose', callAgregarDesglose);
});

var fnResultModalDesgloses = null;

$('#ModalDesgloses').on('click', '#btnAceptarInfoDesgloses', function () {
    if (!!fnResultModalDesgloses) {
        fnResultModalDesgloses();
    }
});

function showModalDesgloses(titulo, fn) {
    fnResultModalDesgloses = fn;

    $("#TituloInfoDesgloses").html(titulo);

    $('#txtDesgloseFolioDesde').val('');
    $('#txtDesgloseFolioHasta').val('');
    $('#txtDesgloseFolioCant').val('');
    $('#cboDesgloseDestino').val('');
    $('#txtDesgloseObservacion').val('');

    $("#ModalDesgloses").modal('show');
    return false;
}

function callAgregarDesglose() {
    var folioDesde = parseInt($('#txtDesgloseFolioDesde').val()) || 0;
    var folioHasta = parseInt($('#txtDesgloseFolioHasta').val()) || 0;
    var folioCant = parseInt($('#txtDesgloseFolioCant').val()) || 0;
    var desgloseDestino = $('#cboDesgloseDestino').val() || 0;
    var observ = $('#txtDesgloseObservacion').val();
    var desgloseDestinoText = $('#cboDesgloseDestino option:selected').text() || 0;
    if (folioDesde === 0) {
        mostrarMensaje(2, "Agregar Desglose", "Debe ingresar el folio desde");
        return false;
    }
    if (folioHasta === 0) {
        mostrarMensaje(2, "Agregar Desglose", "Debe ingresar el folio hasta");
        return false;
    }
    if (folioCant === 0) {
        mostrarMensaje(2, "Agregar Desglose", "Debe ingresar la cantidad de folios");
        return false;
    }
    if (desgloseDestino == '') {
        mostrarMensaje(2, "Agregar Desglose", "Debe ingresar el destino");
        return false;
    }

    iDesgloseNuevo++;
    var table = $('#Grilla_desgloses').DataTable();

    table.row.add(["d" + iDesgloseNuevo.toString(), FormatFechaHora(new Date(), true), folioDesde.toString(), folioHasta.toString(), folioCant.toString(), desgloseDestinoText, observ, desgloseDestino.toString()]).draw();
}

$("#btnDesgloseEliminar").click(function () {
    $('#Grilla_desgloses').DataTable().row('.selected').remove().draw();
    $("#btnDesgloseEliminar").addClass("boton-deshabilitado");
});

//Documentos
$("#btnAgregarNota").click(function () {
    showModalDocumentos('Agregar Nota', callAgregarNota);
    HabilitarDeshabilitarFechaAprobacion();
});

$("#btnEliminarNota").click(function () {
    $('#Grilla_documentos').DataTable().row('.selected').remove().draw();
    $("#btnEliminarNota").addClass("boton-deshabilitado");
});

$("#btnEditarNota").click(function () {
    var data = $("#Grilla_documentos").dataTable().api().row(".selected").data();
    showModalDocumentos('Editar Nota', callEditarNota, data);
    $("#btnEditarNota").removeClass("boton-deshabilitado");
    HabilitarDeshabilitarFechaAprobacion();

});

function callEditarNota(nota) {
    var idDocumento = $("#Grilla_documentos").dataTable().api().row(".selected").data()[0];
    $("#Grilla_documentos").dataTable().api().row(".selected").data([idDocumento, FormatFechaHora(nota.fechaAlta, true), nota.tipoDocumentoDesc, nota.usuarioNombre, nota.descripcion, nota.archivo, nota.tipoDocumento.toString(), nota.fechaAprobacion, nota.observacion]).draw();
};

$("#fecha-aprobacion").datepicker(getDatePickerConfig({
    format: 'dd/mm/yyyy'
}))

$("#btnInformeObservaciones").click(function () {
    if ($("#IdTramite").val() >= 1) {
        window.open(BASE_URL + "mesaentradas/GetInformeObservaciones?id=" + $("#IdTramite").val());
    } else {
        mostrarMensaje(2, 'Informe Observaciones', 'Debe ser un trámite válido.');
    }
});

var fnResultModalDocumentos = null;

$('#ModalDocumentos').on('click', '#btnAceptarInfoDocumentos', function () {
    if (!!fnResultModalDocumentos) {
        showLoading();
        var archivo = $('#txtDocumentoArchivo').val(),
            extension = $('#hdnExtensionArchivo').val(),
            tipoDocumento = parseInt($('#cboTipoDocumento').val());

        if (isNaN(tipoDocumento)) {
            mostrarMensaje(2, "Agregar Nota", "Debe ingresar el tipo de documento");
            hideLoading();
            return false;
        }
        if (archivo && !extension) {
            mostrarMensaje(2, "Agregar Nota", "El archivo no tiene extensión");
            hideLoading();
            return false;
        }

        var nota = {
            tipoDocumento: tipoDocumento,
            tipoDocumentoDesc: $('#cboTipoDocumento option:selected').text(),
            descripcion: $('#txtDocumentoDescripcion').val(),
            archivo: archivo,
            extension: extension,
            observacion: $('#txtDocumentoObservacion').val(),
            usuarioNombre: $('#hdnUsuarioNombre').val(),
            usuarioLogin: $('#hdnUsuarioLogin').val(),
            fechaAlta: new Date(),
            fechaAprobacion: $("#fecha-aprobacion").val()
        };

        if (file) {
            var data = new FormData();
            data.append('idTramite', parseInt($("#IdTramite").val()));
            data.append('file', file);
            $.ajax({
                type: 'POST',
                url: BASE_URL + 'MesaEntradas/UploadDocumento',
                processData: false,
                contentType: false,
                data: data,
                success: function (data) {
                    file = null;
                    nota.archivo = data.nombreArchivo;
                    fnResultModalDocumentos(nota);
                    $("#ModalDocumentos").modal("hide");
                },
                error: function (err) {
                    console.log(err);
                },
                complete: hideLoading
            });
        }
        else {
            fnResultModalDocumentos(nota);
            $("#ModalDocumentos").modal("hide");
            hideLoading();
        }
    }
});

$("#cboTipoDocumento").on('change', function () {
    HabilitarDeshabilitarFechaAprobacion();
});

function HabilitarDeshabilitarFechaAprobacion() {
    if (parseInt($('#cboTipoDocumento').val()) == 7) {
        $("#fecha-aprobacion").removeClass("hidden");
        $("#lblFechaAprobacion").removeClass("hidden");
    } else {
        $("#fecha-aprobacion").addClass("hidden");
        $("#lblFechaAprobacion").addClass("hidden");
    }
}


function initGrillaPadres(selectedElements, enableGrid) {
    $("#modalAgregarPadre").modal("hide");
    var all = $('#js_objetos').jstree(true).get_json('#', { 'flat': true });
    var cols = [
        {
            title: "Entrada",
            render: (data, type, row, meta) => {
                var fatherId = $('#js_objetos').jstree('get_selected') && $('#js_objetos').jstree('get_selected')[0];
                var all = $('#js_objetos').jstree(true).get_json('#', { 'flat': true });
                var father = all.find(x => x.id === fatherId);
                if (!father || $('#js_objetos').jstree('get_selected') && $('#js_objetos').jstree('get_selected').length > 1) {
                    return `<label style="width:100%;">${row.objeto}</label>`;
                } else {
                    return `<label style="width:100%;">${row.objeto}<span onclick="removeFather('${father.data.Guid}', '${row.id}')" class="fa fa-2x fa-minus-circle cursor-pointer black pull-right remove_relation"></span></label>`;
                }
            },
            className: 'col-xs-12'
        }];

    var data = all
        .filter(x => selectedElements.selectedGuids?.includes(x.data.Guid))
        .map(y => ({ objeto: y.text, id: y.data.Guid }));

    var filtered = data.filter((item, pos, self) => pos === self.findIndex(t => t.id === item.id));

    if (!$.fn.DataTable.isDataTable('#Grilla_ObjetosRelacionados')) {
        $('#Grilla_ObjetosRelacionados').DataTable({
            language: { url: BASE_URL + 'Scripts/dataTables.spanish.txt' },
            columns: cols,
            data: filtered,
            paging: false,
            filter: false,
            bSort: false
        });
    } else {
        $('#Grilla_ObjetosRelacionados').DataTable().clear().rows.add(filtered).draw();
    }
    if (enableGrid) {
        $("#grillaPadresContainer").show();
    } else {
        $("#grillaPadresContainer").hide();
    }

    $(".add_relation").prop("disabled", true);
    $(".remove_relation").prop("disabled", true);
    $(".add_relation").addClass("boton-deshabilitado");
    $(".remove_relation").addClass("boton-deshabilitado");
}

function createNode(parent_node, new_node_id, new_node_text, position) {
    $('#js_objetos').jstree('create_node', $(parent_node), { "text": new_node_text, "id": new_node_id }, position, false, false);
}

function getTreeSelectedElements() {
    var $tree = $('#js_objetos');
    var selected = $tree.jstree('get_selected');
    var all = $tree.jstree(true).get_json('#', { 'flat': true });
    return all.filter(x => selected.includes(x.id));
}

function cboObjetoEspecificoTramiteChange(tipoObjetoSelected, operacion, onCompleteCallback) {
    if (isNaN(tipoObjetoSelected)) {
        $("#grillaPadresContainer").hide();
        $("#divEdicionObjetoContainer").empty();
    } else {
        showLoading();
        const form = $('#divEdicionObjetoContainer').get(0);
        $.removeData(form, 'validator');

        const storedArray = getDatosEspecificos();
        const selected = getTreeSelectedElements()[0];

        const arbolObjetoSeleccionado = storedArray.find(x => x.Guid === selected?.data.Guid
            && x.IdTramiteEntrada === selected?.data.IdTramiteEntrada);

        let padre = null;

        if (arbolObjetoSeleccionado && arbolObjetoSeleccionado.ParentGuids.length) {
            padre = storedArray.find(x => x.Guid === arbolObjetoSeleccionado.ParentGuids[0]).TipoObjeto.Id;
        }

        const settings = {
            "url": BASE_URL + "MesaEntradas/EdicionObjeto",
            "method": "POST",
            "data": {
                "operacion": operacion,
                "tipoObjetoSelected": tipoObjetoSelected,
                "arbolObjetoSeleccionado": arbolObjetoSeleccionado,
                "padre": padre
            }
        };

        $.ajax(settings)
            .done(resp => {
                $("#grillaPadresContainer").show();
                $("#divEdicionObjetoContainer").html(resp);
                document.getElementById("divAtributosDeObjeto").style.display = '';
                if (onCompleteCallback) {
                    onCompleteCallback();
                }
            })
            .fail(err => {
                let msg;
                switch (err.status) {
                    case 400:
                        msg = `La entrada seleccionada debe tener un padre seleccionado.`;
                        break;
                    case 404:
                        msg = `La entrada seleccionada no es válida.`;
                        break;
                    default:
                        msg = "Ha ocurrido un error al recuperar la configuración de la entrada seleccionada";
                        break;
                }
                mostrarMensaje(3, "Error en operación", msg);
            })
            .always(hideLoading);
    }
}

function ajustarScrollBarsConsultas() {
    $('#datosEspecificos-content').css({ "max-height": $(".datosEspecificos-body").height() + ' px' });
    $('#datosEspecificos-content').getNiceScroll().resize();
}

function ajustarScrollBars() {
    $(".tab-content .tab-body", "#pnlEdicionTramite").getNiceScroll().resize();
    $(".tab-content .tab-body", "#pnlEdicionTramite").getNiceScroll().show();
}

function createTree(tree, data) {
    $('#' + tree)
        .on('open_node.jstree', function () {
            ajustarScrollBarsConsultas();
        })
        .on('close_node.jstree', function () {
            ajustarScrollBarsConsultas();
        })
        .on('init', function () {
            $('#js_objetos ul.jstree-container-ul li').click(function () {
                alert(this);
            });
        })
        .jstree({
            "core": {
                "multiple": true,
                "animation": 100,
                "themes": {
                    "icons": false
                },
                "expand_selected_onload": true,
                "data": data,
                "check_callback": true
            },
            "checkbox": {
                'deselect_all': true,
                'three_state': false
            }
        }).bind('ready.jstree', function (e, data) {
            $('#' + tree).jstree('open_all');
        });
}

function resetTree(tree) {
    $('#' + tree).jstree().close_all();
    $('#' + tree).jstree().deselect_all();
    $('#' + tree).jstree().open_node('_todos');
}

function resetAndReloadTree() {
    var $tree = $("#js_objetos");
    $tree.jstree().delete_node($tree.jstree(true).get_json('#', { 'flat': true }));
    const datosEspecificos = getDatosEspecificos()
        .sort((a, b) => (a.ParentGuids.length === b.ParentGuids.length && !a.ParentGuids.length)
            ? a.Guid < b.Guid ? -1 : 1
            : !a.ParentGuids.length
                ? -1 : !b.ParentGuids.length
                    ? 1 : b.ParentGuids.some(x => x.Guid === a.Guid)
                        ? -1 : a.ParentGuids.some(x => x.Guid === b.Guid)
                            ? 1 : 0);
    for (let obj of datosEspecificos) {
        agregarNodoEnArbol(obj, $tree);
    }
    setDatosEspecificos(datosEspecificos);
    $tree.jstree('open_all');
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function showModalDocumentos(titulo, fn, currentDocumento) {
    fnResultModalDocumentos = fn;

    $("#TituloInfoDocumentos").html(titulo);
    $('#cboTipoDocumento').val('');
    $('#txtDocumentoDescripcion').val('');
    $('#txtDocumentoArchivo').val('');
    $('#txtDocumentoArchivoFile').val('');
    $('#hdnExtensionArchivo').val('');
    $('#txtDocumentoObservacion').val('');
    $("#fecha-aprobacion").val('')

    if (currentDocumento) {
        $('#cboTipoDocumento').val(currentDocumento[6]);
        $('#txtDocumentoDescripcion').val(currentDocumento[4]);
        $('#txtDocumentoArchivo').val(currentDocumento[5]);
        $('#hdnExtensionArchivo').val(currentDocumento[5].substring(currentDocumento[5].lastIndexOf(".")));
        $("#fecha-aprobacion").val(FormatFechaHora(currentDocumento[7]));
        $('#txtDocumentoObservacion').val(currentDocumento[8]);
    }
    $("#ModalDocumentos").modal('show');
    return false;
}

$("#btnDocumentoExplorar").click(function () { $('#txtDocumentoArchivoFile').click(); });

function changeFile(archivo) {
    file = archivo.files[0];
    var nombre_archivo = archivo.value;
    nombre_archivo = nombre_archivo.substr(nombre_archivo.lastIndexOf('\\') + 1);
    var extension = nombre_archivo.substring(nombre_archivo.lastIndexOf("."));

    $('#hdnExtensionArchivo').val(extension);

    var nombre_sin_extension = nombre_archivo.substring(0, nombre_archivo.lastIndexOf("."));
    if (!$('#txtDocumentoDescripcion').val())
        $('#txtDocumentoDescripcion').val(nombre_sin_extension);

    $('#txtDocumentoArchivo').val(nombre_archivo);
}
function callAgregarNota(nota) {
    iDocumentoNuevo++;
    var idDocumento = "d" + iDocumentoNuevo.toString();
    $('#Grilla_documentos').DataTable().row.add([idDocumento, FormatFechaHora(nota.fechaAlta, true), nota.tipoDocumentoDesc, nota.usuarioNombre, nota.descripcion, nota.archivo, nota.tipoDocumento.toString(), nota.fechaAprobacion, nota.observacion]).draw();
}

function mostrarMensaje(result, title, description, fn) {
    var modal = $('#mensajeModalEdicionTramite')
        .one('hidden.bs.modal', function () { $('#btnAceptarAdvertencia').off('click'); });

    var alertaBackground = $('div[role="alert"]', modal);
    $('.modal-title', modal).html(title);
    $('.modal-body p', modal).html(description);
    alertaBackground.removeClass('alert-warning alert-danger alert-success');

    if (fn) {
        if (result !== 1) {
            modal.one('hidden.bs.modal', fn);
        } else {
            $('.modal-footer', modal).show();
            $('#btnAceptarAdvertencia').one('click', fn);
        }
    } else {
        $('.modal-footer', modal).hide();
    }
    switch (result) {
        case 1:
            alertaBackground.addClass('alert-success');
            break;
        case 2:
            alertaBackground.addClass('alert-warning');
            break;
        case 3:
            alertaBackground.addClass('alert-danger');
            break;
    }
    modal.modal('show');
}
function ajustarmodal() {
    var altura = $(window).height() - 60; //value corresponding to the modal heading + footer
    var alturaContenedor = altura - 90; //MGB: altura del contenedor ajustable
    $("#modal-EdicionTramite .modal-content").css({ "max-height": altura, "min-width": 590, "overflow": "hidden" });
    $("#modal-EdicionTramite .modal-dialog").css({ "max-height": altura });
    var css = { "max-height": alturaContenedor, "height": alturaContenedor };
    $("#modal-EdicionTramite .edicionTramite-body").css(css);
}
function downloadFile(link) {
    showLoading();
    $.ajax({
        type: 'GET',
        url: BASE_URL + "MesaEntradas/ExisteArchivo",
        data: { tramite: $("#IdTramite").val(), archivo: btoa(link.text) },
        success: function () {
            window.location = BASE_URL + 'MesaEntradas/DownloadArchivo';
        },
        error: function (err) {
            var msg = "Ha ocurrido un error al descargar el archivo <strong>" + link.text + "</strong>.";
            switch (err.status) {
                case 404:
                    msg = "El archivo <strong>" + link.text + "</strong> no existe.";
                    break;
                default:
                    break;
            }
            mostrarMensaje(3, "Descargar Archivo", msg);
        },
        complete: hideLoading
    });
}

function enableButtonsOnAddOrEdit() {
    $("#btn_ModificarObjeto").removeClass("disabled");
    $("#btn_AgregarObjeto").removeClass("disabled");
    $("button", "#modal-EdicionTramite .modal-footer").removeAttr("disabled");
}

function disableButtonsOnAddOrEdit() {
    $("#btn_ModificarObjeto").addClass("disabled");
    $("#btn_AgregarObjeto").addClass("disabled");
    $("button", "#modal-EdicionTramite .modal-footer").attr("disabled", "disabled");
    $('#btn_Guardar').removeClass('disabled');
    $('#btn_Cerrar').removeClass('disabled');
}

$("#confirmFatherBtn").click(() => {
    const select = $("#selectAddFather");
    const guid = document.getElementById("hdnGuid")?.value;
    const stored = getDatosEspecificos();
    const item = stored.find(x => x.Guid === guid);
    item.ParentGuids.push(select.val());
    setDatosEspecificos(stored);

    initGrillaPadres({ selectedGuids: item.ParentGuids }, true);

    $(".add_relation").prop("disabled", false);
    $(".remove_relation").prop("disabled", false);
    $(".add_relation").removeClass("boton-deshabilitado");
    $(".remove_relation").removeClass("boton-deshabilitado");
});

function removeFather(source, father) {
    const stored = getDatosEspecificos();
    const item = stored.find(x => x.Guid === source);
    if (item.ParentGuids.length > 1) {
        var i = item.ParentGuids.findIndex(x => x === father);
        item.ParentGuids.splice(i, 1);
        setDatosEspecificos(stored);

        initGrillaPadres({ selectedGuids: item.ParentGuids }, true);

        $(".add_relation").prop("disabled", false);
        $(".remove_relation").prop("disabled", false);
        $(".add_relation").removeClass("boton-deshabilitado");
        $(".remove_relation").removeClass("boton-deshabilitado");

    } else {
        mostrarMensaje(2, 'Eliminar Relación', 'Debe tener una Entrada Padre Relacionada.');
    }
}

$("#addFatherBtn").click(() => {
    var select = $("#selectAddFather");
    var guid = document.getElementById("hdnGuid")?.value;
    select.empty();
    var all = $('#js_objetos').jstree('get_json');

    const stored = getDatosEspecificos();
    const item = stored.find(x => x.Guid === guid);

    if (item.ParentGuids.length > 0) {
        for (var i = 0; i < item.ParentGuids.length; i++) {
            const k = all.findIndex(x => x.data.Guid === item.ParentGuids[i]);
            all.splice(k, 1);
        }

        all.forEach(x => {
            select.append(`<option value=${x.data.Guid}>${x.text}</option>`);
        });

        if (all.length > 0) {
            $('#modalAgregarPadre').modal("show");
        } else {
            mostrarMensaje(2, 'Agregar Relación', 'No hay Entradas para Relacionar.');
        }
    }
});

$("#btn_AgregarObjeto").click(function () {
    $("#divEdicionObjetoContainer :input").prop("disabled", false);
    $("#cboObjetoEspecificoTramite").prop("disabled", false);
    $("#cboObjetoEspecificoTramite").empty();

    $("#divAtributosDeObjeto").hide();

    buscarEntradas(document.getElementById('cboObjetoTramite').value);

    $("#divEdicionObjetos").show();

    var selectedElements = {
        selectedGuids: getTreeSelectedElements().filter(t => t.data.Guid !== '00000000-0000-0000-0000-000000000000').map(x => x.data.Guid),
        selectedIds: getTreeSelectedElements().filter(t => t.data.IdTramiteEntrada).map(x => x.data.IdTramiteEntrada)
    };
    initGrillaPadres(selectedElements);
    setModoEdicion();
});

$("#btn_ModificarObjeto").click(function () {
    $("#divEdicionObjetoContainer :input").prop("disabled", false);

    $("#divEdicionObjetoContainer").trigger("ModificarObjeto");

    var selectedElements = {
        selectedGuids: getTreeSelectedElements()[0].data.ParentGuids
    };

    initGrillaPadres(selectedElements, true);

    //Agrego copia del original
    localStorage.setItem("datosEspecificosOri", JSON.stringify(getDatosEspecificos()));
    $('#TipoOperacion').val('Modificar');

    $(".add_relation").prop("disabled", false);
    $(".remove_relation").prop("disabled", false);
    $(".add_relation").removeClass("boton-deshabilitado");
    $(".remove_relation").removeClass("boton-deshabilitado");

    setModoEdicion();
});

$("#btn_EliminarObjeto").click(function () {
    $("#btnAceptarAdvertenciaEliminar").off("click").one("click", function () {
        const selected = getTreeSelectedElements()[0];
        let storedArray = getDatosEspecificos();

        if (selected.data.ParentGuids.length && selected.data.ParentGuids.length > 1) {
            selected.data.ParentGuids = selected.data.ParentGuids.filter(p => p !== $("#js_objetos").jstree(true).get_json().find(n => n.id === selected.parent).data.Guid)
            storedArray.splice(storedArray.findIndex(elem => elem.Guid === selected.data.Guid), 1, selected.data);
        } else {
            var jerarquiaEliminada = storedArray.reduce((accum, elem) => {
                if (elem.ParentGuids.some((p) => accum.indexOf(p) !== -1)) {
                    if (elem.ParentGuids.length === 1) {
                        return [...accum, elem.Guid];
                    }
                    elem.ParentGuids.splice(elem.ParentGuids.indexOf(selected.data.Guid), 1);
                }
                return accum;
            }, [selected.data.Guid]);
            storedArray = storedArray.filter(e => jerarquiaEliminada.indexOf(e.Guid) === -1);
        }
        setDatosEspecificos(storedArray);
        setModoEliminar();

    });
    $("#mensajeModalEliminarTramite").modal('show');
});
//# sourceURL=editarTramite.js