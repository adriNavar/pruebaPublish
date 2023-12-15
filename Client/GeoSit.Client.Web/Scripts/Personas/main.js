$(document).ready(init);
$(window).resize(ajustarmodal);
$('#modal-window-persona').one('shown.bs.modal', function () {
    ajustarScrollBars();
    $('#Grilla_personas').dataTable().api().columns.adjust();
    $('#Domicilios').dataTable().api().columns.adjust();
    $('#Profesiones').dataTable().api().columns.adjust();
    hideLoading();
});
function init() {
    $('#Domicilios').dataTable({
        "scrollY": "200px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[3, 'asc']],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" }
    });

    $('#Profesiones').dataTable({
        "scrollY": "200px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[1, 'asc']],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" }
    });


    ///////////////////// DataTable /////////////////////////
    $('#Grilla_personas').dataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[1, 'asc']],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" }
    });
    $("#Grilla_personas tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            personaEnableControls(false);

        } else {
            $("tr.selected", "#Grilla_personas tbody").removeClass("selected");
            $(this).addClass("selected");
            CargarDatosDeLaPersona();
            personaEnableControls(true);
        }
    });

    $("#Profesiones tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            profesionEnableControls(false);

        } else {
            $("tr.selected", "#Profesiones tbody").removeClass("selected");
            $(this).addClass("selected");
            profesionEnableControls(true);
        }
    });

    $("#Domicilios tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            domicilioEnableControls(false);

        } else {
            $("tr.selected", "#Domicilios tbody").removeClass("selected");
            $(this).addClass("selected");
            domicilioEnableControls(true);
        }
    });
    ////////////////////////////////////////////////////////
    $("#form").ajaxForm({
        resetForm: true,
        success: function (data) {
            var mensaje = "";
            var Operacion = $("#EstadoOperacion").val();
            if (Operacion === "Edicion")
                mensaje = "ModificacionOK";
            else
                mensaje = "AltaOK";
            MostrarMensaje(mensaje);
            hideLoading();
            if (typeof personaGuardada === 'function') {
                personaGuardada(data);
            }
        },
        error: function () {
            MostrarMensaje("Error");
        }
    });
    ///////////////////// Scrollbars ////////////////////////
    $(".persona-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-persona .panel-body').resize(ajustarScrollBars);
    $('.persona-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".persona-content").getNiceScroll().resize();
        }, 10);
    });

    $("#form").submit(function (evt) {
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $("#btnGrabar").click(function () {
        var Operacion = $("#EstadoOperacion").val();
        //$('#TituloAdvertencia').html() == "Advertencia - Modificación de Datos"
        if (Operacion === "Edicion") {
            var msj = "Está a punto de modificar los datos de la persona " + $('#DatosPersona_Apellido').val() + ', ' + $('#DatosPersona_Nombre').val() + '<br>' + '¿Desea Continuar?';
            $('#TituloAdvertenciaPersona').html("Advertencia - Modificación de Datos");
            $('#DescripcionAdvertenciaPersona').html(msj);
            $('#ModalAdvertenciaPersona').modal('show');
        }
        else {
            if (Operacion === "Alta") {
                var mensaje = "";
                mensaje = ValidarDatos();

                if (!mensaje) {
                    $("#ResultadoBusqueda").val("");
                    $.ajax({
                        async: false,
                        type: 'POST',
                        url: BASE_URL + 'Persona/GetPersonaByDocumentoJson',
                        dataType: 'json',
                        data: { id: $("#DatosPersona_NroDocumento").val() },
                        success: function (Personas) {
                            $.each(Personas, function (_, Persona) {
                                var idtipodocID = Persona.TipoDocId;
                                var nrodoc = Persona.NroDocumento;
                                if ($("#DatosPersona_TipoDocId").val() == idtipodocID && $("#DatosPersona_NroDocumento").val() == nrodoc) {
                                    mensaje = "El número de documento ya existe.";
                                    //$("#ResultadoBusqueda").val("El número de documento ya existe.");
                                }
                            });
                        }, error: function () {
                            mensaje = "";
                            //$("#ResultadoBusqueda").val("");
                        }
                    });
                }

                if (mensaje === "") {
                    // Si el tipo de persona es Jurídica entonces limpia la grilla de profesioens antes del submit.
                    var tipo_persona = $("#DatosPersona_TipoPersonaId").val();
                    if (tipo_persona === "2") {
                        var tabla = $('#Profesiones').dataTable().api();
                        tabla.clear();
                        tabla.draw();
                    }
                    $("#form").submit();

                    $("#EstadoOperacion").val("Consulta");
                    $("#collapsePersonaInfo > .panel-body *").prop('disabled', true);
                    grillaBusquedaEnableControls(true);
                    modoEdicionEnableControls(false);
                    personaEnableControls(false);
                    $(".panel-collapse").prop('disabled', true);
                    $("#btnCerrar").css("display", "");
                    InicializaCamposPersona();

                    var tablaProf = $('#Profesiones').dataTable().api();
                    tablaProf.clear();
                    tablaProf.draw();
                    var tablaDom = $('#Domicilios').dataTable().api();
                    tablaDom.clear();
                    tablaDom.draw();

                    $('#ModoEdicion').val("false");
                }
                else {
                    $('#TituloInfoPersona').html("Información - Grabar Persona");
                    $('#DescripcionInfoPersona').html("Los datos de la persona están incompletos: " + mensaje);
                    $("#ModalInfoPersona").modal('show');
                }
            }
        }
    });

    $("#btnCerrar").click(function () {
        $("#modal-window-persona").modal('hide');
    });

    $("#btnGrabarAdvertenciaPersona").click(function () {
        //var Operacion = $("#EstadoOperacion").val();
        //if (Operacion = "Edicion") {
        if ($('#TituloAdvertenciaPersona').html() === "Advertencia - Modificación de Datos") {
            var mensaje = "";
            mensaje = ValidarDatos();
            if (mensaje === "") {
                $('#ModalAdvertenciaPersona').modal('hide');
                // Si el tipo de persona es Jurídica entonces limpia la grilla de profesioens antes del submit.
                var tipo_persona = $("#DatosPersona_TipoPersonaId").val();
                if (tipo_persona === "2") {
                    var tabla = $('#Profesiones').dataTable().api();
                    tabla.clear();
                    tabla.draw();
                }
                $("#form").submit();

                $("#EstadoOperacion").val("Consulta");
                grillaBusquedaEnableControls(true);
                modoEdicionEnableControls(false);
                personaEnableControls(false);
                $("#body-content-persona .panel-heading").prop('disabled', true);
                $("#collapsePersonaInfo > .panel-body *").prop('disabled', true);
                $("#btnCerrar").css("display", "");
                InicializaCamposPersona();

                var tablaProf = $('#Profesiones').dataTable().api();
                tablaProf.clear();
                tablaProf.draw();
                var tablaDom = $('#Domicilios').dataTable().api();
                tablaDom.clear();
                tablaDom.draw();
                $('#ModoEdicion').val("false");

                if ($("#txtFiltroPersona").val()) {
                    showLoading();
                    $.post(BASE_URL + "Persona/GetPersonasJson", { nombre_completo: $("#txtFiltroPersona").val() })
                        .done(function (data) {
                            var tabla = $('#Grilla_personas').dataTable().api();
                            tabla.clear();
                            data.forEach(function (p) {
                                var node = tabla.row.add([p.PersonaId, p.NombreCompleto, p.NroDocumento]).node();
                                $(node).find("td:first").addClass("hide");
                            });
                            tabla.draw();
                        })
                        .fail(function (data) {
                            console.log(data);
                        }).always(hideLoading);
                }
            }
            else {
                $('#TituloInfoPersona').html("Información - Grabar Persona")
                $('#DescripcionInfoPersona').html("Los datos de la persona están incompletos: " + mensaje)
                $("#ModalInfoPersona").modal('show');
            }
        }

        if ($('#TituloAdvertenciaPersona').html() === "Advertencia - Dar de baja") {
            var FechaActual = new Date();
            var FechaMostrar = '';
            var usuario = "1"; //Session("usuarioPortal");

            FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds();
            $("#DatosPersona_FechaBaja").val(FormatFechaHora(FechaMostrar, true));
            $("#DatosPersona_UsuarioBajaId").val(usuario);

            //$("#accordion_panel_0").removeClass("in");
            //$(".flecha-activada").addClass("flecha-desactivada");
            //$(".panel-collapse").prop('disabled', false);

            //$('#TituloAdvertencia').html() == "Advertencia - Dar de baja"
            var error_baja = "";
            $.ajax({
                type: "POST",
                async: false,
                url: BASE_URL + 'Persona/DeletePersonaJson',
                dataType: 'json',
                data: { id: $("#DatosPersona_PersonaId").val(), fecha_baja: $("#DatosPersona_FechaBaja").val(), usuario_baja: $("#DatosPersona_UsuarioBajaId").val() },
                success: error_baja = "",
                error: error_baja = ""
            });

            var table = $('#Grilla_personas').DataTable();
            table.row('.selected').remove().draw(false);

            $('#ModalAdvertenciaPersona').modal('hide');

            $('#DescripcionInfoPersona').html("La operación se realizó satisfactoriamente.");
            $('#TituloInfoPersona').html("Información - Eliminar Persona");
            $("#ModalInfoPersona").modal('show');

            grillaBusquedaEnableControls(true);
            modoEdicionEnableControls(false);
            personaEnableControls(false);
            $(".panel-collapse").prop('disabled', true);
            $("#btnCerrar").css("display", "");
            InicializaCamposPersona();
            var tablaProf = $('#Profesiones').dataTable().api();
            tablaProf.clear();
            tablaProf.draw();
            var tablaDom = $('#Domicilios').dataTable().api();
            tablaDom.clear();
            tablaDom.draw();

            $('#ModoEdicion').val("false");

            // $("#btnGrabar").click();
        }

        if ($('#TituloAdvertenciaPersona').html() == "Advertencia - Eliminar profesión") {
            var table = $('#Profesiones').DataTable();
            table.row('.selected').remove().draw(false);
            profesionEnableControls(false);
            $('#ModalAdvertenciaPersona').modal('hide');
        }

        if ($('#TituloAdvertenciaPersona').html() === "Advertencia - Cancelar Operación") {
            //Modo edición se oculta grilla y se visulizan botones para grabar.
            grillaBusquedaEnableControls(true);
            modoEdicionEnableControls(false);
            personaEnableControls(false);

            $(".panel-collapse").prop('disabled', true);
            $("#collapsePersonaInfo > .panel-body *").prop('disabled', true);
            $("#btnCerrar").css("display", "");

            // Inicializa campos de persona, y vacía grillas de domicilio y profesión.
            InicializaCamposPersona();
            var tablaProf = $('#Profesiones').dataTable().api();
            tablaProf.clear();
            tablaProf.draw();
            var tablaDom = $('#Domicilios').dataTable().api();
            tablaDom.clear();
            tablaDom.draw();

            // Configurar en modo cosulta
            $('#ModoEdicion').val("false");

            $('#ModalAdvertenciaPersona').modal('hide');
        }

        if ($('#TituloAdvertenciaPersona').html() == "Advertencia - Eliminar domicilio") {
            //var rdio = document.getElementsByName("rbDomicilio");
            ////var hidom = document.getElementsByName("DomicilioPersona");
            //for (var i = 0; i < rdio.length; i++) {
            //    if (rdio[i].checked) {
            //        var FechaActual = new Date();
            //        var FechaMostrar = '';
            //        var usuario = "1"; //Session("usuarioPortal");

            //        FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds()
            //        var identificador = rdio[i].value;
            //        var ident = identificador.substr(2);
            //        $('#filaDomicilio' + identificador).remove();
            //    }
            //}
            var tabla = $('#Domicilios').dataTable().api();
            tabla.row('.selected').remove().draw(false);

            $('#ModalAdvertenciaPersona').modal('hide');
            domicilioEnableControls(false);
        }

    });

    $("#btn_Modificar").click(function () {
        if ($(this).hasClass("boton-deshabilitado") == false) {
            $("#EstadoOperacion").val("Edicion");

            var FechaActual = new Date();
            var FechaMostrar = '';
            var usuario = "1"; //Session("usuarioPortal");

            FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds()

            //Modo edición se oculta grilla y se visulizan botones para grabar.
            grillaBusquedaEnableControls(false);
            modoEdicionEnableControls(true);

            $("#DatosPersona_FechaModif").val(FormatFechaHora(FechaMostrar, true));
            $("#DatosPersona_UsuarioModifId").val(usuario);
            $("#DatosPersona_FechaBaja").val("");
            $("#DatosPersona_UsuarioBajaId").val("");

            $("#collapsePersonaInfo > .panel-body *").prop('disabled', false);
        }
    });

    $("#btn_Agregar").click(function () {
        $("#EstadoOperacion").val("Alta");

        var FechaActual = new Date();
        var FechaMostrar = '';
        var usuario = "1"; //Session("usuarioPortal");

        FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds()
        //LIMPIA CONTROLES ?
        InicializaCamposPersona();

        $("#DatosPersona_FechaAlta").val(FormatFechaHora(FechaMostrar, true));
        $("#DatosPersona_FechaModif").val("");
        $("#DatosPersona_FechaBaja").val("");
        $("#DatosPersona_UsuarioAltaId").val(usuario);
        $("#DatosPersona_UsuarioModifId").val(usuario);
        $("#DatosPersona_UsuarioBajaId").val("");

        grillaBusquedaEnableControls(false);
        modoEdicionEnableControls(true);

        var tabla = $('#Domicilios').dataTable().api();
        tabla.clear();
        tabla.draw();

        var tablaProf = $('#Profesiones').dataTable().api();
        tablaProf.clear();
        tablaProf.draw();

        $("#collapsePersonaInfo > .panel-body *").prop('disabled', false);

        // Configura lista de tipo de personas
        $("#DatosPersona_TipoDocId").val("1");
        var TipoPersona = parseInt("1");
        $("#DatosPersona_TipoPersonaId").find("option[value='" + TipoPersona + "']").attr("selected", true);
        tipopersonaListChange("1");

        // Configurar en modo edición
        $('#ModoEdicion').val("true");
    });

    $("#btn_Eliminar").click(function () {

        if ($(this).hasClass("boton-deshabilitado") == false) {
            $("#EstadoOperacion").val("Baja");
            var msj = "Está a punto de DAR DE BAJA a la persona " + $('#DatosPersona_Nombre').val() + ', ' + $('#DatosPersona_Apellido').val() + '<br>' + '¿Desea Continuar?';
            $('#TituloAdvertenciaPersona').html("Advertencia - Dar de baja");
            $('#DescripcionAdvertenciaPersona').html(msj);
            $('#ModalAdvertenciaPersona').modal('show');
        }
    });

    $("#btnCancelar").click(function () {
        var msj = "Está a punto de cancelar la operación y se perderán los cambios." + '<br>' + '¿Desea Continuar?';
        $('#TituloAdvertenciaPersona').html("Advertencia - Cancelar Operación");
        $('#DescripcionAdvertenciaPersona').html(msj);
        $('#ModalAdvertenciaPersona').modal('show');
    });

    $('#panel_titulo_domicilio').click(function () {
        setTimeout(function () {
            $("#Domicilios").dataTable().api().columns.adjust();
        }, 10);
    });

    $('#panel_titulo_profesiones').click(function () {
        setTimeout(function () {
            $("#Profesiones").dataTable().api().columns.adjust();
        }, 10);
    });

    setTimeout(function () {
        var personaId = $("#PersonaId").val();
        if (personaId != "0") {
            var tabla = $("#Grilla_personas").dataTable().api();
            var node = tabla.row.add([personaId,
                $("#NombreCompleto").val(),
                $("#NroDocumento").val()]).node();
            $(node).find("td:first").addClass("hide");
            tabla.draw();
            $(node).addClass("selected");
            CargarDatosDeLaPersona();
            personaEnableControls(true);
        }
    }, 300);

    ////////////////////////////////////////////////////////
    ajustarmodal();
    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-persona .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-persona").modal('show');
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        headerFooter = 220,
        altura = viewportHeight - headerFooter;
    $(".persona-body", "#scroll-content-persona").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}
function ajustarScrollBars() {
    temp = $(".persona-body").height();
    var outerHeight = 20;
    $('#accordion-persona .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-persona .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.persona-content').css({ "max-height": temp + 'px' })
    $('#accordion-persona').css({ "max-height": temp + 1 + 'px' })
    $(".persona-content").getNiceScroll().resize();
    $(".persona-content").getNiceScroll().show();
}

function personaEnableControls(enable) {
    if (enable) {
        $("#btn_Modificar").removeClass("boton-deshabilitado");
        $("#btn_Eliminar").removeClass("boton-deshabilitado");
        $("#body-content-persona div[role='region']").removeClass("panel-deshabilitado");
    }
    else {
        $("#btn_Modificar").addClass("boton-deshabilitado");
        $("#btn_Eliminar").addClass("boton-deshabilitado");
        $("#body-content-persona div[role='region']").addClass("panel-deshabilitado");
        $("#body-content-persona div[role='region']").find("a:first[aria-expanded=true]").click();
    }
}

function profesionEnableControls(enable) {
    if (enable) {
        $("#btn_Modificar_profesion").removeClass("boton-deshabilitado");
        $("#btn_Eliminar_profesion").removeClass("boton-deshabilitado");
    }
    else {
        $("#btn_Modificar_profesion").addClass("boton-deshabilitado");
        $("#btn_Eliminar_profesion").addClass("boton-deshabilitado");
    }
}

function domicilioEnableControls(enable) {
    if (enable) {
        $("#btn_Modificar_domicilio").removeClass("boton-deshabilitado");
        $("#btn_Eliminar_domicilio").removeClass("boton-deshabilitado");
    }
    else {
        $("#btn_Modificar_domicilio").addClass("boton-deshabilitado");
        $("#btn_Eliminar_domicilio").addClass("boton-deshabilitado");
    }
}

function validaremail(mail) {
    var filter = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
    if (!filter.test(mail))
        return 0;
    else
        return 1;
}
function ValidarDatos() {
    var mensaje = "";

    // Valida que existan domicilios cargados.
    var table = $('#Domicilios').DataTable();
    var regDomicilios = table
        .column(0)
        .data()
        .length;
    if (regDomicilios === 0)
        mensaje += "<br> Debe cargar al menos un domicilio para la persona.";
    else {
        // Valida que el tipo de domicilio solo este una vez.
        for (i = 0; i < regDomicilios; i++) {
            var idtipoDom1 = $("#Domicilios").dataTable().api().row(i).data()[3];
            for (j = i + 1; j < regDomicilios; j++) {
                var idtipoDom2 = $("#Domicilios").dataTable().api().row(j).data()[3];
                if (idtipoDom1 === idtipoDom2)
                    mensaje += "<br> Existen tipos de domicilios repetidos.";
            }
        }
    }

    if (!$("#DatosPersona_TipoDocId").val()) {
        mensaje += "<br> Debe Ingresar el tipo de documento";
    }
    if (!$("#DatosPersona_NroDocumento").val()) {
        mensaje += "<br> Debe Ingresar el número de documento";
    } else {
        var docu = $("#DatosPersona_NroDocumento").val();
        if ($("#DatosPersona_TipoDocId").val() === "3") {
            if (!(docu.length === 13 && docu.substr(2, 1) === "-" && docu.substr(11, 1) === "-")) {
                mensaje += "<br> CUIT inválido";
            } else {
                Numer1 = docu.substr(0, 2);
                Numer2 = docu.substr(3, 8);
                Numer3 = docu.substr(12, 1);
                if (!validarSiNumero(Numer1) || !validarSiNumero(Numer2) || !validarSiNumero(Numer3)) {
                    mensaje += "<br> CUIT inválido";
                }
            }
        } else {
            if (!validarSiNumero($("#DatosPersona_NroDocumento").val())) {
                mensaje += "<br> Documento inválido";
            }
        }
    }
    if ($("#DatosPersona_Email").val() && !validaremail($("#DatosPersona_Email").val())) {
        mensaje += "<br> Dirección de mail inválida";
    }

    if (!$("#DatosPersona_Nombre").val()) {
        mensaje += "<br> Debe Ingresar el nombre";
    }
    if ($("#DatosPersona_TipoDocId").val() !== "3" && !$("#DatosPersona_Apellido").val()) {
        mensaje += "<br> Debe Ingresar el apellido";
    }
    return mensaje;
}

function modoEdicionEnableControls(enable) {
    if (enable) {
        $("#btn_Agregar_profesion").css("display", "");
        $("#btn_Eliminar_profesion").css("display", "");
        $("#btn_Modificar_profesion").css("display", "");
        $("#btn_Agregar_domicilio").css("display", "");
        $("#btn_Eliminar_domicilio").css("display", "");
        $("#btn_Modificar_domicilio").css("display", "");

        //Habilita los bloques de persona, domicilio y profesional.
        $("#body-content-persona div[role='region']").removeClass("panel-deshabilitado");
    }
    else {
        $("#btn_Agregar_profesion").css("display", "none");
        $("#btn_Eliminar_profesion").css("display", "none");
        $("#btn_Modificar_profesion").css("display", "none");
        $("#btn_Agregar_domicilio").css("display", "none");
        $("#btn_Eliminar_domicilio").css("display", "none");
        $("#btn_Modificar_domicilio").css("display", "none");

        //Deshabilita los bloques de persona, domicilio y profesional.
        $("#body-content-persona div[role='region']").addClass("panel-deshabilitado");
        $("#body-content-persona div[role='region']").find("a:first[aria-expanded=true]").click();
    }
}

function grillaBusquedaEnableControls(enable) {
    if (enable) {
        $("#panel_listado_personas").css("display", "");
        $("#Panel_Botones_Personas").css("display", "none");
        $("#btnGrabar").css("display", "none");
        $("#btnCancelar").css("display", "none");
        $("#btnCerrar").css("display", "");
    }
    else {
        $("#panel_listado_personas").css("display", "none");
        $("#Panel_Botones_Personas").css("display", "");
        $("#btnGrabar").css("display", "");
        $("#btnCancelar").css("display", "");
        $("#btnCerrar").css("display", "none");
    }
}

/*REVISAR E INCLUIR LO QUE SEA NECESARIO DENTRO DE LA FUNCION "init"*/
$(document).ready(function () {

    $("#btnSearch").click(function () {
        if ($("#txtFiltroPersona").val()) {
            showLoading();
            $.post(BASE_URL + "Persona/GetPersonasJson", { nombre_completo: $("#txtFiltroPersona").val() })
                .done(function (data) {
                    var tabla = $('#Grilla_personas').dataTable().api();
                    tabla.clear();
                    data.forEach(function (p) {
                        var node = tabla.row.add([p.PersonaId, p.NombreCompleto, p.NroDocumento]).node();
                        $(node).find("td:first").addClass("hide");
                    });
                    tabla.draw();
                })
                .fail(function (data) {
                    console.log(data);
                }).always(function () {
                    hideLoading();
                });
        }
    });

    $("#borrarBusqueda").click(function () {
        $("#txtFiltroPersona").val("");
        var tablaDom = $('#Domicilios').dataTable().api();
        tablaDom.clear();
        tablaDom.draw();

        var tablaProf = $('#Profesiones').dataTable().api();
        tablaProf.clear();
        tablaProf.draw();

        var tablaPer = $('#Grilla_personas').dataTable().api();
        tablaPer.clear();
        tablaPer.draw();

        InicializaCamposPersona();
    });

    $("#btn_Modificar_profesion").click(function () {
        var matriculadoc = $("#Profesiones").dataTable().api().row(".selected").data()[2];
        var tipoprof = $("#Profesiones").dataTable().api().row(".selected").data()[0];
        $("#ProfTipoProfesionId").val(tipoprof);
        $("#ProfMatricula").val(matriculadoc);
        $("#AccionProfesion").val("Modificar");
        $("#ProfTipoProfesionId").prop("disabled", true);
        $('#modal-window-profesion').modal('show');
    });

    $("#btn_Eliminar_profesion").click(function () {
        matriculadoc = $("#Profesiones").dataTable().api().row(".selected").data()[2];
        var msj = "Está a punto de Eliminar la profesión, matrícula: " + matriculadoc + '<br>' + '¿Desea Continuar?';
        $('#TituloAdvertenciaPersona').html("Advertencia - Eliminar profesión");
        $('#DescripcionAdvertenciaPersona').html(msj);
        $('#ModalAdvertenciaPersona').modal('show');
    });

    $("#btn_Agregar_profesion").click(function () {
        $("#ProfMatricula").val("");
        $("#ProfTipoProfesionId").val("");
        $("#AccionProfesion").val("Agregar");
        $("#ProfTipoProfesionId").prop("disabled", false);
        $('#modal-window-profesion').modal('show');
    });

    $("#btn_Modificar_domicilio").click(function () {
        var id = $("#Domicilios").dataTable().api().row(".selected").data()[0];
        var identificador = $("#Domicilios").dataTable().api().row(".selected").data()[2];
        var XMLDomicilio = $("#Domicilio_" + identificador).val();
        if (id && parseInt(id) !== 0) {
            showLoading();
            $('#persona-externo').load(BASE_URL + 'Domicilio/DatosDomicilio?id=' + id, function () {
                $(document).one("actualizaDataDomicilio", ActualizaDataDomicilio);
            });
        } else {
            //MODIFICACION DOMICILIO LOCAL
            //TOMAR DATOS XML
            //ENVIAR DATOS POR PARAMETRO (CREAR FUNCION)
            $("#editarLocal").val("true");
            $("#editarLocalIndex").val($("#Domicilios tbody .selected").index());
            showLoading();
            $('#persona-externo').load(BASE_URL + 'Domicilio/DatosDomicilioXml?xml=' + encodeURIComponent(window.btoa(XMLDomicilio)));
        }
        domicilioEnableControls(false);
    });

    $("#btn_Agregar_domicilio").click(function () {
        showLoading();
        $('#persona-externo').load(BASE_URL + 'Domicilio/DatosDomicilio', function () {
            $(document).one("actualizaDataDomicilio", ActualizaDataDomicilio);
        });
        domicilioEnableControls(false);
    });

    $("#btn_Eliminar_domicilio").click(function () {
        var desc = "";
        desc = $("#Domicilios").dataTable().api().row(".selected").data()[3];
        var msj = "Está a punto de Eliminar el domicilio: " + desc + '<br>' + '¿Desea Continuar?';
        $('#TituloAdvertenciaPersona').html("Advertencia - Eliminar domicilio");
        $('#DescripcionAdvertenciaPersona').html(msj);
        $('#ModalAdvertenciaPersona').modal('show');
    });


    $("#btnGrabarProfesion").click(function () {
        var mensaje = "";
        if (!$("#ProfTipoProfesionId").val())
            mensaje = "Debe Ingresar el tipo de Profesión";
        if (!$("#ProfMatricula").val())
            mensaje = "Debe Ingresar la matrícula";
        if ($("#ProfMatricula").val()) {
            if (!validarSiNumero($("#ProfMatricula").val())) {
                mensaje = "Matrícula inválida, deber ser numérica";
            }
        }
        //Controla si ya existe la profesión
        var idtipoprof = $("#ProfTipoProfesionId").val();
        if ($("#AccionProfesion").val() === "Agregar") {
            var descProofesion = $("#ProfTipoProfesionId").find("option[value='" + idtipoprof + "']").text();
            var table = $('#Profesiones').DataTable();
            var index = table
                .column(1)
                .data()
                .indexOf(descProofesion);
            if (index >= 0) {
                mensaje = "El tipo de profesión ya existe.";
            }
        }

        if (!mensaje) {
            if ($("#AccionProfesion").val() === "Modificar") {
                $('#Profesiones').DataTable().row('.selected').remove().draw(false);
            }

            var matricula = $("#ProfMatricula").val();
            var desc = $("#ProfTipoProfesionId").find("option[value='" + idtipoprof + "']").text();

            var tabla = $('#Profesiones').dataTable().api();
            var node = tabla.row.add([parseInt(idtipoprof), desc, matricula,
            '<input type="hidden" id="Profesion_' + idtipoprof + '" name="TipoProfesionId" value="' + idtipoprof + '" >',
            '<input type="hidden" id="Matricula_' + idtipoprof + '" name="MatriculaNumero" value="' + matricula + '" >'
            ]).node();
            $(node).find("td:first").addClass("hide");
            $(node).find("td:eq(3)").addClass("hide");
            $(node).find("td:eq(4)").addClass("hide");
            tabla.draw();
            $('#modal-window-profesion').modal('hide');
        }
        else {
            $('#TituloInfo').html("Información - Profesiones")
            $('#DescripcionInfo').html("Los datos de la profesión están incompletos o erróneos: " + mensaje);
            $("#ModalInfo").modal('show');
        }
    });

    $("#btnCancelarProfesion").click(function () {
        $('#modal-window-profesion').modal('hide');
    });

    $("#btnCerrarProfesion").click(function () {
        $('#modal-window-profesion').modal('hide');
    });

    MostrarMensaje();
});

function MostrarMensaje(MensajeEntrada) {
    if (MensajeEntrada) {
        if (MensajeEntrada === "AltaOK") {
            $('#TituloInfo').html("Información - Nueva Persona")
            $('#DescripcionInfo').html("Los datos de la nueva persona han sido guardados satisfactoriamente.");
            $("#ModalInfo").modal('show');
        }

        if (MensajeEntrada === "ModificacionOK") {
            $('#TituloInfo').html("Información - Persona");
            $('#DescripcionInfo').html("Los datos de la persona han sido guardados satisfactoriamente.");
            $("#ModalInfo").modal('show');
        }

        if (MensajeEntrada === "Error") {
            $('#TituloInfo').html("Información - Error")
            $('#DescripcionInfo').html("Se ha producido un error al grabar los datos.")
            $("#ModalInfo").modal('show');
        }

    }
}

function tipopersonaListChange(tipoPersonaSel) {
    if (tipoPersonaSel == "2") {
        //Configura el cuit.
        $("#DatosPersona_TipoDocId").val("3");
        var TipoPersona = parseInt("3");

        $("#DatosPersona_TipoPersonaId").find("option[value='" + TipoPersona + "']").attr("selected", true);

        //$("#DatosPersona_TipoDocId option[value='1']").hide();
        //$("#DatosPersona_TipoDocId option[value='2']").hide();
        //$("#DatosPersona_TipoDocId option[value='3']").show();

        var divProfesion = document.getElementById("div_profesion");
        divProfesion.style.display = "none";

        var elDiv1 = document.getElementById("panelApellido");
        elDiv1.style.display = "none";

        var elDiv2 = document.getElementById("campoApellido");
        elDiv2.style.display = "none";

        var elDiv3 = document.getElementById("panelSexo");
        elDiv3.style.display = "none";

        var elDiv4 = document.getElementById("campoSexo");
        elDiv4.style.display = "none";

        var elDiv5 = document.getElementById("panelEstadoCivil");
        elDiv5.style.display = "none";

        var elDiv6 = document.getElementById("campoEstadoCivil");
        elDiv6.style.display = "none";

        var elDiv7 = document.getElementById("panelNacionalidad");
        elDiv7.style.display = "none";

        var elDiv8 = document.getElementById("campoNacionalidad");
        elDiv8.style.display = "none";

    } else {
        //Configura el DN_I.
        $("#DatosPersona_TipoDocId").val("1");
        var TipoPersona = parseInt("1");

        $("#DatosPersona_TipoPersonaId").find("option[value='" + TipoPersona + "']").attr("selected", true);

        //$('#TipoDocId').find('option[value="3"]').hide();
        //$("#DatosPersona_TipoDocId option[value='1']").show();
        //$("#DatosPersona_TipoDocId option[value='2']").show();
        //$("#DatosPersona_TipoDocId option[value='3']").hide();

        var divProfesion = document.getElementById("div_profesion");
        divProfesion.style.display = "block";

        var elDiv1 = document.getElementById("panelApellido");
        elDiv1.style.display = "block";

        var elDiv2 = document.getElementById("campoApellido");
        elDiv2.style.display = "block";

        var elDiv3 = document.getElementById("panelSexo");
        elDiv3.style.display = "block";

        var elDiv4 = document.getElementById("campoSexo");
        elDiv4.style.display = "block";

        var elDiv5 = document.getElementById("panelEstadoCivil");
        elDiv5.style.display = "block";

        var elDiv6 = document.getElementById("campoEstadoCivil");
        elDiv6.style.display = "block";

        var elDiv7 = document.getElementById("panelNacionalidad");
        elDiv7.style.display = "block";

        var elDiv8 = document.getElementById("campoNacionalidad");
        elDiv8.style.display = "block";

    }
}

//function FormatFechaHora(Fecha_Entrada, FechaHora) {

//    if (Fecha_Entrada == null) {
//        return "";
//    } else {
//        var dateInt = parseInt(Fecha_Entrada.replace('/Date(', ''));
//        if (dateInt)
//            return $.datepicker.formatDate("dd/mm/yy", new Date(dateInt));

//        Fecha_Entrada = Fecha_Entrada.replace('T', ' ');
//    }

//    var Fecha = new Date(Fecha_Entrada);



//    var curr_date = Fecha.getDate();
//    var curr_month = Fecha.getMonth();
//    var curr_year = Fecha.getFullYear();
//    var curr_hours = Fecha.getHours();
//    var curr_minutes = Fecha.getMinutes();
//    var curr_seconds = Fecha.getSeconds();

//    curr_date = ("0" + (curr_date)).slice(-2)
//    curr_month = ("0" + (curr_month + 1)).slice(-2)
//    curr_hours = ("0" + (curr_hours)).slice(-2)
//    curr_minutes = ("0" + (curr_minutes)).slice(-2)
//    curr_seconds = ("0" + (curr_seconds)).slice(-2)



//    if (FechaHora == undefined) {
//        FechaHora = '';
//    }

//    if (FechaHora == '') {
//        return (curr_date + "/" + curr_month + "/" + curr_year);

//    } else {
//        return (curr_date + "/" + curr_month + "/" + curr_year + " " + curr_hours + ":" + curr_minutes + ":" + curr_seconds);
//    }


//}

function Solo_Numerico(variable) {
    if ($("#DatosPersona_TipoDocId").val() === "3" && variable.substr(variable.length - 1, 1) === "-" && (variable.length === 3 || variable.length === 12))
        return variable;
    else {
        return !variable || isNaN(variable) ? "" : parseInt(variable);
    }
}
function ValNumero(Control) {
    Control.value = Solo_Numerico(Control.value);
}

function InicializaCamposPersona() {
    $("#DatosPersona_PersonaId").val("");
    $("#DatosPersona_TipoDocId").val("");
    $("#DatosPersona_NroDocumento").val("");
    $("#DatosPersona_Nombre").val("");
    $("#DatosPersona_Apellido").val("");
    $("#DatosPersona_TipoPersonaId").val("");
    $("#DatosPersona_Sexo").val("");
    $("#DatosPersona_EstadoCivil").val("");
    $("#DatosPersona_Nacionalidad").val("");
    $("#DatosPersona_Telefono").val("");
    $("#DatosPersona_Email").val("");
}

function validarSiNumero(numero) {
    return /^([0-9])*$/.test(numero) ? 1 : 0;
}

function CargarDatosDeLaPersona() {
    var tipoDePersona = "",
        personaId = $("#Grilla_personas").dataTable().api().row(".selected").data()[0];

    $.post(BASE_URL + "Persona/GetDatosPersonaJson", { id: personaId })
        .done(function (data) {
            $("#DatosPersona_PersonaId").val(data.PersonaId);
            $("#DatosPersona_TipoDocId").val(data.TipoDocId);
            $("#DatosPersona_TipoPersonaId").val(data.TipoPersonaId);
            $("#DatosPersona_NroDocumento").val(data.NroDocumento);
            $("#DatosPersona_Nombre").val(data.Nombre);
            $("#DatosPersona_Apellido").val(data.Apellido);
            $("#DatosPersona_Sexo").val(data.Sexo);
            $("#DatosPersona_EstadoCivil").val(data.EstadoCivil);
            $("#DatosPersona_Nacionalidad").val(data.Nacionalidad);
            $("#DatosPersona_Telefono").val(data.Telefono);
            $("#DatosPersona_Email").val(data.Email);

            $("#DatosPersona_TipoPersonaId").find("option[value='" + parseInt(data.TipoPersonaId) + "']").attr("selected", true);
            tipoDePersona = data.TipoPersonaId;
            tipopersonaListChange(tipoDePersona);
            $("#DatosPersona_TipoPersonaId").val(data.TipoPersonaId);

            $("#DatosPersona_FechaAlta").val(FormatFechaHora(data.FechaAlta, true));
            $("#DatosPersona_UsuarioAltaId").val(data.UsuarioAltaId);
            $("#DatosPersona_FechaModif").val(FormatFechaHora(data.FechaModif, true));
            $("#DatosPersona_UsuarioModifId").val(data.UsuarioModifId);
            $("#DatosPersona_FechaBaja").val(FormatFechaHora(data.FechaBaja, true));
            $("#DatosPersona_UsuarioBajaId").val(data.UsuarioBajaId);

        })
        .fail(function (data) {
            console.log(data);
        });


    $.post(BASE_URL + "Persona/GetDatosProfesionByPersonaJson", { id: personaId })
        .done(function (data) {
            var tabla = $('#Profesiones').dataTable().api();
            tabla.clear();
            data.forEach(function (p) {
                var matricula = p.Matricula;
                var tipoprof = p.TipoProfesionId;
                var TipoProfInt = parseInt(p.TipoProfesionId);
                var desc = $("#ProfTipoProfesionId").find("option[value='" + TipoProfInt + "']").text();

                var node = tabla.row.add([TipoProfInt, desc, matricula,
                    '<input type="hidden" id="Profesion_' + tipoprof + '" name="TipoProfesionId" value="' + tipoprof + '" >',
                    '<input type="hidden" id="Matricula_' + tipoprof + '" name="MatriculaNumero" value="' + matricula + '" >'
                ]).node();
                $(node).find("td:first").addClass("hide");
                $(node).find("td:eq(3)").addClass("hide");
                $(node).find("td:eq(4)").addClass("hide");
            })
            tabla.draw();
        })
        .fail(function (data) {
            console.log(data);
        });

    $.post(BASE_URL + "Persona/GetDatosDomicilioByPersonaJson", { id: personaId })
        .done(function (data) {
            var tabla = $('#Domicilios').dataTable().api();
            tabla.clear();
            data.forEach(function (p) {
                var id_domicilio = p.DomicilioId;
                var viaNombre = p.ViaNombre == "" ? "SD" : p.ViaNombre;
                var numeroPuerta = p.numero_puerta == null ? "" : p.numero_puerta;
                var localidad = p.localidad == null ? "SD" : p.localidad;
                var codigoPostal = p.codigo_postal == null ? "SD" : p.codigo_postal;
                // Busca la descripción del tipo de domicilio.
                var desc = $("#DomiTiposId").find("option[value='" + parseInt(p.TipoDomicilioId) + "']").text(),
                    clave = p.TipoDomicilioId + "-" + viaNombre + "-" + numeroPuerta + "-" + localidad,
                    node = tabla.row.add([id_domicilio, '<input type="hidden" id="Domicilio_' + id_domicilio + '" name="DomicilioPersona" value="' + Object.toXML(p, "DatosDomicilio") + '" >',
                        id_domicilio, desc, viaNombre + " " + numeroPuerta, codigoPostal, localidad, clave]).node();
                $(node).find("td:first").addClass("hide");
                $(node).find("td:eq(1)").addClass("hide");
                $(node).find("td:eq(2)").addClass("hide");
                $(node).find("td:eq(7)").addClass("hide");
            })
            tabla.order([4, 'asc']);
            tabla.draw();
        })
        .fail(function (data) {
            console.log(data);
        });
}

function runSearch(e) {
    if (e.keyCode === 13) {
        $("#btnSearch").click();
        return false;
    }
}

function ActualizaDataDomicilio(evt) {
    var tabla = $('#Domicilios').dataTable().api();
    if (evt.domicilio.id_domicilio && parseInt(evt.domicilio.id_domicilio) !== 0)
        tabla.row('.selected').remove().draw(false);
    var node = tabla.row.add([evt.domicilio.id_domicilio, '<input type="hidden" id="Domicilio_' + evt.domicilio.id_registro + '" name="DomicilioPersona" value="' + evt.domicilio.XMLDomicilio + '" >',
    evt.domicilio.id_registro, evt.domicilio.desc, evt.domicilio.desc_domicilio, evt.domicilio.desc_cp, evt.domicilio.desc_localidad, evt.domicilio.clave]).node();
    $(node).find("td:first").addClass("hide");
    $(node).find("td:eq(1)").addClass("hide");
    $(node).find("td:eq(2)").addClass("hide");
    $(node).find("td:eq(7)").addClass("hide");
    tabla.draw();
}