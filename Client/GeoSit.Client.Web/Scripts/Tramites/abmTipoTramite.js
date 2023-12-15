$(document).ready(initTramite);
$(window).resize(ajustarmodalTramite);
$('#modal-window-tramite').on('shown.bs.modal', function (e) {
    ajustarScrollBarsTramite();
    $('#Grilla_Tramites').dataTable().api().columns.adjust();
    $.ajax({
        type: "POST",
        async: false,
        url: BASE_URL + 'TramitesCertificados/GetParametroTramitePermiso',
        dataType: 'json',
        success: function (data) {
            if (data != 1) {
                $("#informe-final-permisos").remove();
            }
        }
    });
    hideLoading();
});
function initTramite() {
    ///////////////////// DataTable /////////////////////////
    $('#Grilla_Tramites').dataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[1, 'asc']],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" }
    });
    $("#Grilla_Tramites tbody").on("click", "tr", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            tramiteEnableControls(false);
        } else {
            $("tr.selected", "#Grilla_Tramites tbody").removeClass("selected");
            $(this).addClass("selected");
            CargarDatosDelTramite();
            tramiteEnableControls(true);
        }
    });

    //////////////main/////////////////////////////
    $("#form").ajaxForm({
        resetForm: true,
        success: function () {
            var mensaje = "";
            var Operacion = $("#EstadoOperacion").val();
            if (Operacion = "Edicion")
                mensaje = "ModificacionOK";
            else
                mensaje = "AltaOK";
            MostrarMensaje(mensaje);
            CargaGrillaTramite();
            if (typeof tipotramiteGuardado == 'function') {
                tipotramiteGuardado();
            }
        },
        error: function () {
            MostrarMensaje("Error");
        }
    });
    /////////////////// Scrollbars ////////////////////////
    $(".tramite-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-tramite .panel-body').resize(ajustarScrollBarsTramite);
    $('.tramite-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".tramite-content").getNiceScroll().resize();
        }, 10);
    });
    //////////////////////////////////////////////////////

    $("#btn_Agregar_seccion").click(function () {
        //var identRandom = num = Math.floor(10 + (1 + 50000 - 10) * Math.random());
        var identRandom = num = Math.floor(Math.random() * 50) + 9999999999999999;
        agregarSeccion(identRandom, "Nueva Sección", "", 0, true);
        ajustarScrollBarsTramite();
    });

    $("#btn_Modificar").click(function () {
        //if ($(this).hasClass("boton-deshabilitado") == false) {
        $("#EstadoOperacion").val("Edicion");
        var FechaActual = new Date();
        var FechaMostrar = '';
        var usuario = "1";
        FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds()

        //Modo edición se oculta grilla y se visulizan botones para grabar.
        grillaBusquedaEnableControls(false);
        //modoEdicionEnableControls(true);

        //$("#txtFecha_Modif").val(FormatFechaHora(FechaMostrar, true));
        //$("#txtId_Usu_Modif").val(usuario);
        //$("#txtFecha_Baja").val("");
        //$("#txtId_Usu_Alta").val("");

        $(".panel-collapse *").prop('disabled', false);

        // Configurar en modo edición
        $('#ModoEdicion').val("true");
        tramiteEnableEdit(true);

        $("#headingTramiteInfo").find("a:first[aria-expanded=false]").click();
        //}
    })

    $("#btn_Agregar").click(function () {
        $("#Grilla_Tramites tbody tr").removeClass("selected");

        $("#EstadoOperacion").val("Alta");

        var FechaActual = new Date();
        var FechaMostrar = '';
        var usuario = "1"; //Session("usuarioPortal");
        FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds()
        //LIMPIA CONTROLES ?
        InicializaCamposTramite();

        //$("#txtFecha_Alta").val(FormatFechaHora(FechaMostrar, true));
        //$("#txtFecha_Modif").val("");
        //$("#txtFecha_Baja").val("");
        //$("#txtId_Usu_Alta").val(usuario);
        //$("#txtId_Usu_Modif").val("");
        //$("#txtId_Usu_Baja").val("");

        grillaBusquedaEnableControls(false);
        tramiteEnableControls(true);
        //modoEdicionEnableControls(true);

        $(".panel-collapse *").prop('disabled', false);
        //$("#collapseTramiteInfo").removeClass("collapse");

        // Configurar en modo edición
        $('#ModoEdicion').val("true");
        tramiteEnableEdit(true);

        $("#headingTramiteInfo").find("a:first[aria-expanded=false]").click();
    })

    $("#btn_Eliminar").click(function () {
        $("#EstadoOperacion").val("Baja");
        var msj = "Está a punto de DAR DE BAJA el tipo de trámite " + $('#txtNombre').val() + '<br>' + '¿Desea Continuar?'
        $('#TituloAdvertenciaTramite').html("Advertencia - Dar de baja")
        $('#DescripcionAdvertenciaTramite').html(msj)
        $('#ModalAdvertenciaTramite').modal('show')
    });

    $("#btnGrabar").click(function () {
        var Operacion = $("#EstadoOperacion").val();
        //$('#TituloAdvertencia').html() == "Advertencia - Modificación de Datos"
        if (Operacion == "Edicion") {
            var msj = "Está a punto de modificar los datos del tramite " + $('#txtNombre').val() + '<br>' + '¿Desea Continuar?';
            $('#TituloAdvertenciaTramite').html("Advertencia - Modificación de Datos");
            $('#DescripcionAdvertenciaTramite').html(msj);
            $('#ModalAdvertenciaTramite').modal('show');
        }
        else {
            var Operacion = $("#EstadoOperacion").val();
            if (Operacion = "Alta") {
                var mensaje = "";
                mensaje = ValidarDatos();

                if (mensaje == "") {
                    actualizaValores();
                    $("#form").submit();
                    grillaBusquedaEnableControls(true);
                    tramiteEnableControls(false);
                    $(".panel-collapse *").prop('disabled', true);
                    $("#btnCerrar").css("display", "")
                    InicializaCamposTramite();
                    CargaGrillaTramite();
                }
                else {
                    $('#TituloInfoTramite').html("Información - Grabar Tramite")
                    $('#DescripcionInfoTramite').html("Los datos del tipo de trámite o secciones están incompletos: " + mensaje)
                    $("#ModalInfoTramite").modal('show');
                }
            }
        }
    });

    $("#btnCancelar").click(function () {
        var msj = "Está a punto de cancelar la operación y se perderán los cambios." + '<br>' + '¿Desea Continuar?'
        $('#TituloAdvertenciaTramite').html("Advertencia - Cancelar Operación")
        $('#DescripcionAdvertenciaTramite').html(msj)
        $('#ModalAdvertenciaTramite').modal('show')
    })

    $("#btnGrabarAdvertenciaTramite").click(function () {
        if ($('#TituloAdvertenciaTramite').html() == "Advertencia - Modificación de Datos") {
            var mensaje = "";
            mensaje = ValidarDatos();

            if (mensaje == "") {
                actualizaValores();
                $("#form").submit();
                grillaBusquedaEnableControls(true);
                tramiteEnableControls(false);
                $(".panel-collapse *").prop('disabled', true);
                $("#btnCerrar").css("display", "")
                InicializaCamposTramite();
                $('#ModalAdvertenciaTramite').modal('hide')
                CargaGrillaTramite();
            }
            else {
                $('#ModalAdvertenciaTramite').modal('hide')
                $('#TituloInfoTramite').html("Información - Grabar Tramite")
                $('#DescripcionInfoTramite').html("Los datos del tipo de trámite o secciones están incompletos: " + mensaje);
                $("#ModalInfoTramite").modal('show');
            }
        }

        if ($('#TituloAdvertenciaTramite').html() == "Advertencia - Dar de baja") {
            var FechaActual = new Date();
            var FechaMostrar = '';
            var usuario = "1";

            FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds()
            //$("#txtFecha_Baja").val(FormatFechaHora(FechaMostrar, true))
            //$("#txtId_Usu_Baja").val(usuario);
            var tiene_tramites_abiertos = false;

            $.ajax({
                type: "POST",
                async: false,
                url: BASE_URL + 'TipoTramiteCertificado/ObjetosTramitesCertificadosDelete',
                dataType: 'json',
                data: { pTipoId: $("#txtId_Tipo_Tramite").val() },
                success: function (data) { tiene_tramites_abiertos = data },
                error: tiene_tramites_abiertos = false
            });
            
            if (tiene_tramites_abiertos) {
                $('#ModalAdvertenciaTramite').modal('hide');
                $('#DescripcionInfoTramite').html("El tipo tramite seleccionado posee tramites aun NO finalizados.");
                $('#TituloInfoTramite').html("Información - Eliminar Tipo Tramite");
                $("#ModalInfoTramite").modal('show');
                return;
            }
            var error_baja = "";
            $.ajax({
                type: "POST",
                async: false,
                url: BASE_URL + 'TipoTramiteCertificado/DeleteTipoTramite',
                dataType: 'json',
                data: { id: $("#txtId_Tipo_Tramite").val() },
                success: error_baja = "",
                error: error_baja = ""
            });

            var table = $('#Grilla_Tramites').DataTable();
            table.row('.selected').remove().draw(false);

            $('#ModalAdvertenciaTramite').modal('hide');

            $('#DescripcionInfoTramite').html("La operación se realizó satisfactoriamente.")
            $('#TituloInfoTramite').html("Información - Eliminar Tipo Tramite")
            $("#ModalInfoTramite").modal('show');

            grillaBusquedaEnableControls(true);
            //modoEdicionEnableControls(false);
            tramiteEnableControls(false);
            $(".panel-collapse *").prop('disabled', true);
            $("#btnCerrar").css("display", "")
            InicializaCamposTramite();
            $('#ModoEdicion').val("false");
        }

        if ($('#TituloAdvertenciaTramite').html() == "Advertencia - Cancelar Operación") {
            //Modo edición se oculta grilla y se visulizan botones para grabar.
            grillaBusquedaEnableControls(true);
            //modoEdicionEnableControls(false);
            tramiteEnableControls(false);

            $(".panel-collapse *").prop('disabled', true);
            fnEnableCamposSecciones(false);

            $("#btnCerrar").css("display", "")
            // Inicializa campos de tramite.
            InicializaCamposTramite();
            $("#collapseTramiteInfo").addClass("collapse");
            // Configurar en modo cosulta
            $('#ModoEdicion').val("false");
            $('#ModalAdvertenciaTramite').modal('hide')
        }
    });

    $("#form").submit(function (evt) {
        evt.preventDefault();
        evt.stopImmediatePropagation();
        return false;
    });

    $("#informe-final-permisos").click(function () {
        var obj = $('#Grilla_Tramites').dataTable().api().row('.selected').data();
        if (obj != undefined){
            fnClicPermisos(0, obj[0])
        } else {
            //MENSAJE
            $('#TituloInfoTramite').html("Información - Error");
            $('#DescripcionInfoTramite').html("Debe guardar los datos del Tramite antes de configurar sus permisos");
            $("#ModalInfoTramite").modal('show');
        }
        
    })

    ///////////////////// Tooltips /////////////////////////
    //$('#modal-window-tramite .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    CargaGrillaTramite();
    ajustarmodalTramite();
    $("#modal-window-tramite").modal('show');
};
function ajustarmodalTramite() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter; //value corresponding to the modal heading + footer
    $(".tramite-body", "#scroll-content-tramite").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBarsTramite();
}
function ajustarScrollBarsTramite() {
    $('.tramite-content').collapse('show');
    temp = $(".tramite-body").height();
    var outerHeight = 20;
    outerHeight = $("#panel_listado_tramites").outerHeight();
    $('#accordion-tramite .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-tramite .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.tramite-content').css({ "max-height": temp + 'px' })
    $('#accordion-tramite').css({ "max-height": temp + 1 + 'px' })
    $(".tramite-content").getNiceScroll().resize();
    $(".tramite-content").getNiceScroll().show();
}

function tramiteEnableControls(enable) {
    if (enable) {
        $("#btn_Modificar").removeClass("boton-deshabilitado");
        $("#btn_Eliminar").removeClass("boton-deshabilitado");
        $("div[role='region']").removeClass("panel-deshabilitado");
        $("#div_secciones *").prop('disabled', false);
        $("#BoolImprimeCabecera").prop('disabled', false);
        $("#BoolImprimeUnidades").prop('disabled', false);
        $("#BoolImprimeInformeFinal").prop('disabled', false);
    }
    else {
        $("#btn_Modificar").addClass("boton-deshabilitado");
        $("#btn_Eliminar").addClass("boton-deshabilitado");
        $("div[role='region']").addClass("panel-deshabilitado");
        $("div[role='region']").find("a:first[aria-expanded=true]").click()
        $("#div_secciones *").prop('disabled', true);
        $("#BoolImprimeCabecera").prop('disabled', true);
        $("#BoolImprimeUnidades").prop('disabled', true);
        $("#BoolImprimeInformeFinal").prop('disabled', true);
    }
}

function agregarSeccion(idseccion, nombreseccion, textoseccion, imprimeseccion, nuevo) {
    //accordion-tramite
    var seccion = document.createElement('div');
    seccion.id = 'div_informacion_seccion_' + idseccion;
    seccion.setAttribute('style', 'margin-bottom: 3px; margin-top: 0;');

    var cabecera = document.createElement('div');
    cabecera.id = 'headingTramiteseccion_' + idseccion;
    cabecera.classList.add('panel-heading', 'bg-primary', 'main-heading');
    cabecera.setAttribute('role', 'region');
    cabecera.setAttribute('name', 'headingTramiteseccion');
    $(cabecera).on('click', function () {
        setTimeout(function () {
            $(".tramite-content").getNiceScroll().resize();
        }, 200);
    });

    var href = document.createElement('a');
    href.id = 'a_headingTramiteseccion_' + idseccion;
    href.href = '#collapseTramiteseccion_' + idseccion;
    href.classList.add('collapsed');
    href.setAttribute('data-toggle', 'collapse');
    href.setAttribute('data-parent', 'accordion-tramite');
    href.setAttribute('aria-expanded', 'false');
    href.setAttribute('aria-controls', 'collapseTramiteseccion_' + idseccion);

    var titulo = document.createElement('div');
    titulo.id = 'Tramiteseccion_panel-title_' + idseccion;
    titulo.classList.add('panel-title');
    titulo.innerHTML = nombreseccion;

    var icono = document.createElement('i');
    icono.classList.add('fa');
    titulo.appendChild(icono);
    href.appendChild(titulo);
    cabecera.appendChild(href);
    seccion.appendChild(cabecera);

    var collapse = document.createElement('div');
    collapse.id = 'collapseTramiteseccion_' + idseccion;
    collapse.classList.add('panel-collapse', 'main-collapse');
    collapse.setAttribute('aria-labelledby', 'headingTramiteseccion_' + idseccion);

    var body = document.createElement('div');
    body.classList.add('panel-body');

    var lblNombre = document.createElement('div');
    lblNombre.classList.add('panel-body', 'col-lg-4', 'col-xs-4', 'col-sm-4', 'col-md-4');
    lblNombre.innerHTML = 'Nombre';

    var datosNombre = document.createElement('div');
    datosNombre.classList.add('panel-body', 'col-lg-8', 'col-xs-8', 'col-sm-8', 'col-md-8');

    var hdnNombre = document.createElement('input');
    hdnNombre.id = 'TxtIdTipoSeccion' + idseccion;
    hdnNombre.setAttribute('type', 'hidden');
    hdnNombre.setAttribute('value', idseccion);
    hdnNombre.setAttribute('name', 'TxtIdTipoSeccion');

    var txtNombre = document.createElement('input'),
        nombre = nombreseccion;

    if (nuevo) nombre = '';

    txtNombre.id = 'TxtNombreInformeSeccion' + idseccion;
    txtNombre.classList.add('form-control');
    txtNombre.setAttribute('type', 'text');
    txtNombre.setAttribute('value', nombre);
    txtNombre.setAttribute('name', 'TxtNombreInformeSeccion');
    txtNombre.setAttribute('maxlength', 80);

    datosNombre.appendChild(hdnNombre);
    datosNombre.appendChild(txtNombre);

    var datosPlanilla = document.createElement('div');
    datosPlanilla.classList.add('panel-body');

    var plantilla = document.createElement('textarea');
    plantilla.id = 'TxtTextoInformeSeccion' + idseccion;
    plantilla.classList.add('form-control');
    plantilla.setAttribute('type', 'textarea');
    //plantilla.setAttribute('value', textoseccion);
    $(plantilla).val(textoseccion);
    plantilla.setAttribute('name', 'TxtTextoInformeSeccion');
   // plantilla.setAttribute('maxlength', 255);
    plantilla.setAttribute('style', 'overflow:hidden');
    plantilla.setAttribute('style', 'resize:vertical');
    plantilla.setAttribute('rows', 5);

    datosPlanilla.appendChild(plantilla);

    var datosImpresion = document.createElement('div');
    datosImpresion.classList.add('panel-body', 'col-lg-10', 'col-xs-10', 'col-sm-10', 'col-md-10');

    var hdnImpresion = document.createElement('input');
    hdnImpresion.setAttribute('type', 'hidden');
    hdnImpresion.setAttribute('value', imprimeseccion);
    hdnImpresion.setAttribute('name', 'TxtConfiguracionImpresionPorDefecto');

    var chkImpresion = document.createElement('input');
    chkImpresion.setAttribute('type', 'checkbox');
    chkImpresion.setAttribute('name', 'BoolConfiguracionImpresionPorDefecto' + idseccion);
    if (Number(imprimeseccion) === 1) chkImpresion.setAttribute('checked', 'checked');

    datosImpresion.appendChild(hdnImpresion);
    datosImpresion.appendChild(chkImpresion);
    datosImpresion.appendChild(document.createTextNode(' Configuración de impresión por defecto'));

    var btnEliminar = document.createElement('div');
    btnEliminar.classList.add('panel-body', 'col-lg-1', 'col-xs-1', 'col-sm-1', 'col-md-1');

    var span = document.createElement('span');
    span.classList.add('fa', 'fa-minus-circle', 'fa-2x', 'black', 'cursor-pointer');
    span.setAttribute('aria-hidden', 'true');
    span.setAttribute('onclick', 'fnClicEliminar("' + idseccion + '")');

    btnEliminar.appendChild(span);

    var btnPermiso = document.createElement('div');
    btnPermiso.classList.add('panel-body', 'col-lg-1', 'col-xs-1', 'col-sm-1', 'col-md-1');

    var obj = $('#Grilla_Tramites').dataTable().api().row('.selected').data();
    var spanPermiso = document.createElement('span');
    spanPermiso.classList.add('fa', 'fa-file-archive-o', 'fa-2x', 'black', 'cursor-pointer');
    spanPermiso.setAttribute('aria-hidden', 'true');
    if (obj != null) {
        spanPermiso.setAttribute('onclick', 'fnClicPermisos("' + idseccion + '","' + obj[0] + '")');
    }

    btnPermiso.appendChild(spanPermiso);

    body.appendChild(lblNombre);
    body.appendChild(datosNombre);
    body.appendChild(datosPlanilla);
    body.appendChild(datosImpresion);
    body.appendChild(btnEliminar);

    $.ajax({
        type: "POST",
        async: false,
        url: BASE_URL + 'TramitesCertificados/GetParametroTramitePermiso',
        dataType: 'json',
        success: function (data) {
            if (data != 0) {
                body.appendChild(btnPermiso);
            }
        }
    });

    collapse.appendChild(body);

    seccion.appendChild(collapse);

    if (!nuevo) {
        collapse.classList.add('collapse');
        txtNombre.setAttribute('disabled', true);
        plantilla.setAttribute('disabled', true);
        chkImpresion.setAttribute('disabled', true);
        span.classList.add('boton-deshabilitado');
    }
    $('#div_secciones').append(seccion);
}

function CargaGrillaTramite() {
    showLoading();
    var tabla = $('#Grilla_Tramites').dataTable().api();
    $.post(BASE_URL + "TipoTramiteCertificado/GetTiposTramites")
        .done(function (data) {
            var cantidad = "";
            tabla.clear();
            data.forEach(function (p) {
                $.ajax({
                    async: false,
                    type: 'POST',
                    url: BASE_URL + 'TipoTramiteCertificado/GetCantSeccionesByIdTipoTramite',
                    dataType: 'json',
                    data: { id: p.Id_Tipo_Tramite },
                    success: function (responseText) {
                        cantidad = responseText;
                    }, error: function (ex) {
                        mensaje = "";
                    }
                });

                var node = tabla.row.add([p.Id_Tipo_Tramite, p.Nombre, cantidad]).node();
                $(node).find("td:first").addClass("hide");
            })
            tabla.draw();
        })
        .fail(function (data) {
            console.log(data);
        }).always(function () {
            hideLoading();
        });
    $("#collapseTramiteInfo").addClass("collapse");
    tramiteEnableEdit(false);
}

function CargarDatosDelTramite() {
    var tramiteId = $("#Grilla_Tramites").dataTable().api().row(".selected").data()[0];
    showLoading();
    $.post(BASE_URL + "TipoTramiteCertificado/GetTipoTramiteById", { id: tramiteId })
     .done(function (data) {
         $("#txtNombre").val(data.Nombre);
         $("#txtNumerador").val(data.Numerador);
         $("#txtId_Tipo_Tramite").val(data.Id_Tipo_Tramite);
         $("#txtAutonumerico").val(data.Autonumerico);
         $("#txtImprime_Cab").val(data.Imprime_Cab);
         $("#txtImprime_Doc").val(data.Imprime_Doc);
         $("#txtImprime_UTS").val(data.Imprime_UTS);
         $("#txtImprime_Final").val(data.Imprime_Final);
         $("#txtPlantilla_Final").val(data.Plantilla_Final);

         var ctrlNumerador = document.getElementById('txtNumerador');
         if (data.Autonumerico == true) {
             document.getElementById('EsAutonumerico').checked = true;
             ctrlNumerador.disabled = false;
         }
         else {
             document.getElementById('EsAutonumerico').checked = false;
             ctrlNumerador.disabled = true;
         }
         if (data.Imprime_Cab == true)
             document.getElementById('BoolImprimeCabecera').checked = true;
         else
             document.getElementById('BoolImprimeCabecera').checked = false;

         if (data.Imprime_UTS == true)
             document.getElementById('BoolImprimeUnidades').checked = true;
         else
             document.getElementById('BoolImprimeUnidades').checked = false;

         if (data.Imprime_Final == true)
             document.getElementById('BoolImprimeInformeFinal').checked = true;
         else
             document.getElementById('BoolImprimeInformeFinal').checked = false;

         if (data.Imprime_Per == true)
             document.getElementById('BoolImprimePer').checked = true;
         else
             document.getElementById('BoolImprimePer').checked = false;
         if (data.Imprime_Doc == true)
             document.getElementById('BoolImprimeDoc').checked = true;
         else
             document.getElementById('BoolImprimeDoc').checked = false;

         $("#txtFecha_Alta").val(FormatFechaHora(data.Fecha_Alta));
         $("#txtId_Usu_Alta").val(data.Id_Usu_Alta);
         $("#txtFecha_Modif").val(FormatFechaHora(data.Fecha_Modif));
         $("#txtId_Usu_Modif").val(data.Id_Usu_Modif);
         $("#txtFecha_Baja").val(FormatFechaHora(data.Fecha_Baja));
         $("#txtId_Usu_Baja").val(data.Id_Usu_Baja);
     })
    .fail(function (data) { console.log(data); })
    .always(function () { hideLoading(); });

    document.getElementById("div_secciones").innerHTML = "";
    $.post(BASE_URL + "TipoTramiteCertificado/GetTiposSeccionByIdTramite", { id: tramiteId })
        .done(function (data) {
            data.forEach(function (p) {
                agregarSeccion(p.Id_Tipo_Seccion, p.Nombre, p.Plantilla, p.Imprime, false);
                ajustarScrollBarsTramite();
            });
            fnEnableCamposSecciones(false);
        })
        .fail(function (data) {
            console.log(data);
            alert("error");
        }).always(function () {
            hideLoading();
            tramiteEnableEdit(false);
        });
}

function FormatFechaHora(jsonDateTime) {
    if (jsonDateTime == null) return "";

    var Fecha = new Date(Number(jsonDateTime.match(/\d+/)[0]));

    var curr_date = Fecha.getDate();
    var curr_month = Fecha.getMonth();
    var curr_year = Fecha.getFullYear();
    var curr_hours = Fecha.getHours();
    var curr_minutes = Fecha.getMinutes();
    var curr_seconds = Fecha.getSeconds();

    curr_date = ("0" + (curr_date)).slice(-2)
    curr_month = ("0" + (curr_month + 1)).slice(-2)
    curr_hours = ("0" + (curr_hours)).slice(-2)
    curr_minutes = ("0" + (curr_minutes)).slice(-2)
    curr_seconds = ("0" + (curr_seconds)).slice(-2)

    return curr_date + "/" + curr_month + "/" + curr_year + " " + curr_hours + ":" + curr_minutes + ":" + curr_seconds;
}

function ValidarDatos() {
    var mensaje = "";

    if (document.getElementById('EsAutonumerico').checked) {
        if ($("#txtNumerador").val() == null || $("#txtNumerador").val() == "")
            mensaje = "Debe Ingresar el numerador.";
    }

    if ($("#txtNombre").val() == null || $("#txtNombre").val() == "")
        mensaje = "Debe Ingresar el nombre del tipo de informe.";

    //if ($("#txtPlantilla_Final").val() == null || $("#txtPlantilla_Final").val() == "")
    //    mensaje = "Debe Ingresar el texto de la plantilla final.";

    var txtIdSecc = document.getElementsByName("TxtIdTipoSeccion");
    for (var i = 0; i < txtIdSecc.length; i++) {

        //var txtSecc = document.getElementsByName("TxtTextoInformeSeccion");
        //if (txtSecc[i].value == "" || txtSecc[i].value == null)
        //    mensaje = "Debe Ingresar el texto de todas las secciones.";

        var txtNombreSecc = document.getElementsByName("TxtNombreInformeSeccion");
        if (txtNombreSecc[i].value == "" || txtNombreSecc[i].value == null)
            mensaje = "Debe Ingresar el nombre de todas las secciones.";
    }
    return mensaje;
}

function actualizaValores() {
    // Actualiza los radionbutton en los campos de la tabla.
    if (document.getElementById('EsAutonumerico').checked)
        $("#txtAutonumerico").val("true");
    else
        $("#txtAutonumerico").val("false");

    if (document.getElementById('BoolImprimeCabecera').checked)
        $("#txtImprime_Cab").val("true");
    else
        $("#txtImprime_Cab").val("false");

    if (document.getElementById('BoolImprimeUnidades').checked)
        $("#txtImprime_UTS").val("true");
    else
        $("#txtImprime_UTS").val("false");

    if (document.getElementById('BoolImprimeInformeFinal').checked)
        $("#txtImprime_Final").val("true");
    else
        $("#txtImprime_Final").val("false");

    if (document.getElementById('BoolImprimePer').checked)
        $("#txtImprime_Per").val("true");
    else
        $("#txtImprime_Per").val("false");

    if (document.getElementById('BoolImprimeDoc').checked)
        $("#txtImprime_Doc").val("true");
    else
        $("#txtImprime_Doc").val("false");

    var txtIdSecc = document.getElementsByName("TxtIdTipoSeccion");
    for (var i = 0; i < txtIdSecc.length; i++) {
        var radioimprime = document.getElementsByName("BoolConfiguracionImpresionPorDefecto" + txtIdSecc[i].value);
        var txtConf = document.getElementsByName("TxtConfiguracionImpresionPorDefecto");
        if (radioimprime[0].checked) {
            txtConf[i].value = "1";
        }
        else
            txtConf[i].value = "0";

        if (txtIdSecc[i].value.substring(0, 1) == "R")
            txtIdSecc[i].value = "0";
    }
}

function grillaBusquedaEnableControls(enable) {
    if (enable) {
        $("#panel_listado_tramites").css("display", "");
        $("#btnGrabar").css("display", "none");
        $("#btnCancelar").css("display", "none");
        $('#Grilla_Tramites').removeClass("selected");
        $("#btn_Agregar_seccion").addClass("boton-deshabilitado");
    }
    else {
        $("#panel_listado_tramites").css("display", "none")
        $("#btnGrabar").css("display", "")
        $("#btnCancelar").css("display", "")

        $("#btn_Agregar_seccion").removeClass("boton-deshabilitado");
    }
}

function MostrarMensaje(MensajeEntrada) {
    if (MensajeEntrada != null && MensajeEntrada != '') {
        if (MensajeEntrada == "AltaOK") {
            $('#TituloInfoTramite').html("Información - Nuevo Tipo de Trámite")
            $('#DescripcionInfoTramite').html("Los datos del nuevo Tipo de Trámite han sido guardados satisfactoriamente.")
            $("#ModalInfoTramite").modal('show');
        }

        if (MensajeEntrada == "ModificacionOK") {
            $('#TituloInfoTramite').html("Información - Tipo de Trámite")
            $('#DescripcionInfoTramite').html("Los datos del Tipo de Trámite han sido guardados satisfactoriamente.")
            $("#ModalInfoTramite").modal('show');
        }

        if (MensajeEntrada == "Error") {
            $('#TituloInfoTramite').html("Información - Error")
            $('#DescripcionInfoTramite').html("Se ha producido un error al grabar los datos.")
            $("#ModalInfoTramite").modal('show');
        }
    }
}

function InicializaCamposTramite() {
    $("#txtNombre").val("");
    $("#txtNumerador").val("");
    $("#txtPlantilla_Final").val("");
    $("#txtAutonumerico").val("");
    $("#txtId_Tipo_Tramite").val("");
    $("#txtImprime_Cab").val("");
    $("#txtImprime_Doc").val("");
    $("#txtImprime_UTS").val("");
    $("#txtImprime_Final").val("");
    $("#txtFecha_Baja").val("");
    $("#txtId_Usu_Baja").val("");
    $("#txtFecha_Alta").val("");
    $("#txtId_Usu_Alta").val("");
    $("#txtFecha_Modif").val("");
    $("#txtId_Usu_Modif").val("");
    document.getElementById("div_secciones").innerHTML = "";

    document.getElementById('EsAutonumerico').checked = false;
    document.getElementById('BoolImprimeInformeFinal').checked = false;
    document.getElementById('BoolImprimeUnidades').checked = false;
    document.getElementById('BoolImprimeCabecera').checked = false;
    //    document.getElementById('BoolImprimePer').checked = false;
    fnCtrlEsAutonumerico();
    
}

function validarSiNumero(numero) {
    if (!/^([0-9])*$/.test(numero)) {
        return "Numeror debe ser numérico.";
    }
    else {
        return "";
    }
}

function fnCtrlEsAutonumerico() {
    var ctrlNumerador = document.getElementById('txtNumerador');
    if (document.getElementById('EsAutonumerico').checked) {
        ctrlNumerador.disabled = false;
    }
    else {
        ctrlNumerador.disabled = true;
    }
}

function runkeyPressNomSeccion(e) {
    if (e.keyCode == 13) {
        $("#btnSearch").click();
        return false;
    }
}

function fnClicEliminar(valor) {
    var elem = document.getElementById('div_informacion_seccion_' + valor);
    elem.parentNode.removeChild(elem);
    ajustarScrollBarsTramite();
}

function fnEnableCamposSecciones(enable) {
    var nodes = document.getElementsByTagName('TxtNombreInformeSeccion');
    for (var i = 0; i < nodes.length; i++) {
        nodes[i].disabled = false;
    }
}

function fnClicPermisos(valor,idTipoTramite) {
    if (valor <= 9999999999999998) {
        $("#permisosContainer").load(BASE_URL + "TramiteCertificadoPermisos/GetPermisosForm?idSeccion=" + valor + "&idTipoTramite=" + idTipoTramite)
    } else {
        //MENSAJE
        $('#TituloInfoTramite').html("Información - Error");
        $('#DescripcionInfoTramite').html("Debe guardar los datos de la Seccion antes de configurar sus permisos");
        $("#ModalInfoTramite").modal('show');
    }
}

function tramiteEnableEdit(enable) {
    if (enable) {
        $("#btnGrabar").css("display", "");
        $("#btnCancelar").css("display", "");
        $("#BoolImprimeCabecera").prop('disabled', false);
        $("#BoolImprimeUnidades").prop('disabled', false);
        $("#BoolImprimeInformeFinal").prop('disabled', false);
        $("#EsAutonumerico").prop('disabled', false);
        $("#txtNombre").prop('disabled', false);
        $("#txtPlantilla_Final").prop('disabled', false);
        $("#BoolImprimePer").prop('disabled', false);
        $('#div_secciones .fa-minus-circle').removeClass('boton-deshabilitado');
        $('#div_secciones [type="text"]').prop('disabled', false);
        $('#div_secciones [type="textarea"]').prop('disabled', false);
        $('#div_secciones [type="checkbox"]').prop('disabled', false);
        $("#informe-final-permisos").removeClass("boton-deshabilitado");
        $('.fa-file-archive-o').removeClass('boton-deshabilitado');
        fnCtrlEsAutonumerico();
    } else {
        $("#btnGrabar").css("display", "none");
        $("#btnCancelar").css("display", "none");
        $("#BoolImprimeCabecera").prop('disabled', true);
        $("#BoolImprimeUnidades").prop('disabled', true);
        $("#BoolImprimeInformeFinal").prop('disabled', true);
        $("#EsAutonumerico").prop('disabled', true);
        $("#txtNombre").prop('disabled', true);
        $("#txtPlantilla_Final").prop('disabled', true);
        $("#BoolImprimePer").prop('disabled', true);
        $('#div_secciones .fa-minus-circle').addClass('boton-deshabilitado');
        $('#div_secciones [type="text"]').prop('disabled', true);
        $('#div_secciones [type="textarea"]').prop('disabled', true);
        $('#div_secciones [type="checkbox"]').prop('disabled', true);
        $("#informe-final-permisos").addClass("boton-deshabilitado");
        $('.fa-file-archive-o').addClass('boton-deshabilitado');
        fnCtrlEsAutonumerico();
    }
}

//@ sourceURL=abmTipoTramites.js