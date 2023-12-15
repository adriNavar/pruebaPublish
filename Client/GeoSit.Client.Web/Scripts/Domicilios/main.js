$(document).ready(initDomicilio);
$(window).resize(ajustarmodalDomicilio);
$('#modal-window-domicilio').one('shown.bs.modal', function () {
    ajustarScrollBarsDomicilio();
    hideLoading();
});
$('#modal-window-domicilio').one('hidden.bs.modal', function () {
    $(document).off("actualizaDataDomicilio");
    $(document).off("domicilioGuardado");
});

document.getElementById("DatosDomicilio_municipio").selectedIndex = 0;
document.getElementById("DatosDomicilio_localidad").selectedIndex = 0;


function initDomicilio() {
    ///////////////////// Scrollbars ////////////////////////
    $(".domicilio-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-domicilio .panel-body').resize(ajustarScrollBarsDomicilio);
    $('.domicilio-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".domicilio-content").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////

    $("#btnGrabarAdvertenciaDomicilio").click(function () {
        $("#ModalAdvertenciaDomicilio").modal('hide');
        $("#modal-window-domicilio").modal('hide');
    });
    $("#btnGrabarDomicilio").click(function () {
        var mensaje = "";
        mensaje = ValidarDatosDomicilio();

        if (!mensaje) {
            var FechaActual = new Date();
            var FechaMostrar = '';
            FechaMostrar = FechaActual.getFullYear() + '-' + (FechaActual.getMonth() + 1) + '-' + FechaActual.getDate() + ' ' + FechaActual.getHours() + ':' + FechaActual.getMinutes() + ':' + FechaActual.getSeconds();
            if (!$('#DatosDomicilio_DomicilioId').val() || $('#DatosDomicilio_DomicilioId').val() === "0") {
                $('#DatosDomicilio_UsuarioAltaId').val("1");
                $('#DatosDomicilio_FechaAlta').val(FechaMostrar);
            }
            $('#DatosDomicilio_UsuarioModifId').val("1");
            $('#DatosDomicilio_FechaModif').val(FechaMostrar);

            // Configura la descripción de la localidad
            var desc_localidad = "";
            var loca = document.getElementById("DatosDomicilio_localidad");
            var id_localidad = "";
            var lista = $("#DatosDomicilio_localidad");
            if (loca.style.display === "block" || !loca.style.display) {
                var tiposLocal = document.getElementById('DatosDomicilio_localidad');
                var selectedTipoLocal = tiposLocal.options[tiposLocal.selectedIndex].text;
                desc_localidad = selectedTipoLocal;
                id_localidad = $(lista).val();
            }
            else {
                desc_localidad = $("#txtDatosDomicilio_localidad").val();
                id_localidad = "0";
            }

            // Configura la descripción de la municipio
            var desc_municipio = "";
            var muni = document.getElementById("DatosDomicilio_municipio");
            if (muni.style.display === "block" || !muni.style.display) {
                var tiposMuni = document.getElementById('DatosDomicilio_municipio');
                var selectedTipoMuni = tiposMuni.options[tiposMuni.selectedIndex].text;
                desc_municipio = selectedTipoMuni;
            }
            else { desc_municipio = $("#txtDatosDomicilio_municipio").val(); }

            // Configura la descripción de la provincia
            var desc_provincia = "";
            var id_provincia = "";
            lista = $("#DatosDomicilio_provincia");
            var prov = document.getElementById("DatosDomicilio_provincia");
            if (prov.style.display === "block" || !prov.style.display) {
                var tiposProv = document.getElementById('DatosDomicilio_provincia');
                var selectedTipoProv = tiposProv.options[tiposProv.selectedIndex].text;
                desc_provincia = selectedTipoProv;
                id_provincia = $(lista).val();
            }
            else {
                desc_provincia = $("#txtDatosDomicilio_provincia").val();
                id_provincia = "0";
            }


            //Configura la descripcion de la VIA y el id via tramo.
            var desc_via = "";
            var id_via = "";
            lista = $("#lstViaNombre");
            if ($(lista).is(':visible') && lista.val()) {
                //desc_via = lista.val() > 0 ? $(lista).text().trim() : "";
                id_via = $(lista).val();
                var tiposVia = document.getElementById('lstViaNombre');
                var selectedTipoVia = tiposVia.options[tiposVia.selectedIndex].text;
                desc_via = selectedTipoVia;
            }
            else {
                desc_via = $("#DatosDomicilio_ViaNombre").val();
                id_via = "0";
            }

            //Identificación del domicilio.
            var id_domicilio = parseInt($("#DatosDomicilio_DomicilioId").val() ? $("#DatosDomicilio_DomicilioId").val() : 0);

            //Pais
            var desc_pais = $("#DatosDomicilio_pais option[value='" + $('#DatosDomicilio_pais').val() + "']").text();

            var nro_puerta = $("#DatosDomicilio_numero_puerta").val();
            var tipo_domicilio = $("#DatosDomicilio_TipoDomicilioId").val();
            var desc_domicilio = desc_via + " " + $("#DatosDomicilio_numero_puerta").val();
            var identRandom = Math.floor(10 + (1 + 50000 - 10) * Math.random());
            var id_registro = "";
            if ($('#hdfIdentificadorConfigurado').val() === "0" || !$('#hdfIdentificadorConfigurado').val())
                id_registro = identRandom.toString();
            else
                id_registro = $('#hdfIdentificadorConfigurado').val();

            // Busca la descripción del tipo de domicilio.
            var IdTipoDomInt = parseInt(tipo_domicilio);
            var desc = $("#DomiTiposId").find("option[value='" + IdTipoDomInt + "']").text();

            // Descripción del código postal.
            var desc_cp = $("#DatosDomicilio_codigo_postal").val() || "SD";
            if (id_domicilio === -1) {
                id_domicilio = 0;
            }

            var data = {
                DomicilioId: ($("#DatosDomicilio_DomicilioId").val() ? $("#DatosDomicilio_DomicilioId").val() : 0),
                ViaNombre: desc_via,
                numero_puerta: $("#DatosDomicilio_numero_puerta").val(),
                piso: $("#DatosDomicilio_piso").val(),
                unidad: $("#DatosDomicilio_unidad").val(),
                barrio: null,
                localidad: desc_localidad,
                municipio: desc_municipio,
                provincia: desc_provincia,
                pais: $("#DatosDomicilio_pais option[value='" + $('#DatosDomicilio_pais').val() + "']").text(),
                ubicacion: $("#DatosDomicilio_ubicacion").val(),
                codigo_postal: $("#DatosDomicilio_codigo_postal").val(),
                UsuarioAltaId: $("#DatosDomicilio_UsuarioAltaId").val() || null,
                FechaAlta: moment($("#DatosDomicilio_FechaAlta").val()).format(),
                UsuarioModifId: $("#DatosDomicilio_UsuarioAltaId").val(),
                FechaModif: moment($("#DatosDomicilio_FechaModif").val()).format(),
                UsuarioBajaId: null,
                FechaBaja: null,
                ViaId: id_via,
                IdLocalidad: id_localidad,
                ProvinciaId: id_provincia,
                TipoDomicilioId: $("#DatosDomicilio_TipoDomicilioId").val(),
                identificador: $('#hdfIdentificadorConfigurado').val(),
                TipoDomicilio: {
                    Descripcion: $("#DatosDomicilio_TipoDomicilioId option[value='" + $('#DatosDomicilio_TipoDomicilioId').val() + "']").text(),
                    TipoDomicilioId: $("#DatosDomicilio_TipoDomicilioId").val()
                }

            };

            // XML Domicilio
            var XMLDomicilio = "<DatosDomicilio>";
            if (id_domicilio) { XMLDomicilio = XMLDomicilio + "<DomicilioId>" + id_domicilio + "</DomicilioId>"; }
            else { XMLDomicilio = XMLDomicilio + "<DomicilioId></DomicilioId>"; }
            XMLDomicilio = XMLDomicilio + "<ViaNombre>" + desc_via + "</ViaNombre>";
            if (!nro_puerta) { XMLDomicilio = XMLDomicilio + "<numero_puerta></numero_puerta>"; }
            else { XMLDomicilio = XMLDomicilio + "<numero_puerta>" + nro_puerta + "</numero_puerta>"; }
            if (!$("#DatosDomicilio_piso").val()) { XMLDomicilio = XMLDomicilio + "<piso></piso>"; }
            else { XMLDomicilio = XMLDomicilio + "<piso>" + $("#DatosDomicilio_piso").val() + "</piso>"; }
            if (!$("#DatosDomicilio_unidad").val()) { XMLDomicilio = XMLDomicilio + "<unidad></unidad>"; }
            else { XMLDomicilio = XMLDomicilio + "<unidad>" + $("#DatosDomicilio_unidad").val() + "</unidad>"; }
            XMLDomicilio = XMLDomicilio + "<barrio></barrio>";
            if (!desc_localidad) { XMLDomicilio = XMLDomicilio + "<localidad></localidad>"; }
            else { XMLDomicilio = XMLDomicilio + "<localidad>" + desc_localidad + "</localidad>"; }
            if (!desc_municipio) { XMLDomicilio = XMLDomicilio + "<municipio></municipio>"; }
            else { XMLDomicilio = XMLDomicilio + "<municipio>" + desc_municipio + "</municipio>"; }
            if (!desc_provincia) { XMLDomicilio = XMLDomicilio + "<provincia></provincia>"; }
            else { XMLDomicilio = XMLDomicilio + "<provincia>" + desc_provincia + "</provincia>"; }
            if (!desc_pais) { XMLDomicilio = XMLDomicilio + "<pais></pais>"; }
            else { XMLDomicilio = XMLDomicilio + "<pais>" + desc_pais + "</pais>"; }
            if (!$("#DatosDomicilio_ubicacion").val()) { XMLDomicilio = XMLDomicilio + "<ubicacion></ubicacion>"; }
            else { XMLDomicilio = XMLDomicilio + "<ubicacion>" + $("#DatosDomicilio_ubicacion").val() + "</ubicacion>"; }
            if (!$("#DatosDomicilio_codigo_postal").val()) { XMLDomicilio = XMLDomicilio + "<codigo_postal></codigo_postal>"; }
            else { XMLDomicilio = XMLDomicilio + "<codigo_postal>" + $("#DatosDomicilio_codigo_postal").val() + "</codigo_postal>"; }

            if ($("#DatosDomicilio_UsuarioAltaId").val()) { XMLDomicilio = XMLDomicilio + "<UsuarioAltaId>" + $("#DatosDomicilio_UsuarioAltaId").val() + "</UsuarioAltaId>"; }
            else { XMLDomicilio = XMLDomicilio + "<UsuarioAltaId></UsuarioAltaId>"; }
            XMLDomicilio = XMLDomicilio + "<FechaAlta>" + FormatFechaHoraDomicilio($("#DatosDomicilio_FechaAlta").val(), true) + "</FechaAlta>";
            if ($("#DatosDomicilio_UsuarioModifId").val()) { XMLDomicilio = XMLDomicilio + "<UsuarioModifId>" + $("#DatosDomicilio_UsuarioModifId").val() + "</UsuarioModifId>"; }
            else { XMLDomicilio = XMLDomicilio + "<UsuarioModifId></UsuarioModifId>"; }
            if (!$("#DatosDomicilio_FechaModif").val()) { XMLDomicilio = XMLDomicilio + "<FechaModif></FechaModif>"; }
            else { XMLDomicilio = XMLDomicilio + "<FechaModif>" + FormatFechaHoraDomicilio($("#DatosDomicilio_FechaModif").val(), true) + "</FechaModif>"; }
            if ($("#DatosDomicilio_UsuarioBajaId").val()) { XMLDomicilio = XMLDomicilio + "<UsuarioBajaId>" + $("#DatosDomicilio_UsuarioBajaId").val() + "</UsuarioBajaId>"; }
            else { XMLDomicilio = XMLDomicilio + "<UsuarioBajaId></UsuarioBajaId>"; }
            if (!$("#DatosDomicilio_FechaBaja").val()) { XMLDomicilio = XMLDomicilio + "<FechaBaja></FechaBaja>"; }
            else { XMLDomicilio = XMLDomicilio + "<FechaBaja>" + FormatFechaHoraDomicilio($("#DatosDomicilio_FechaBaja").val(), true) + "</FechaBaja>"; }
            if (id_via) { XMLDomicilio = XMLDomicilio + "<ViaId>" + id_via + "</ViaId>"; }
            else { XMLDomicilio = XMLDomicilio + "<ViaId></ViaId>"; }
            if (id_localidad) { XMLDomicilio = XMLDomicilio + "<IdLocalidad>" + id_localidad + "</IdLocalidad>"; }
            else { XMLDomicilio = XMLDomicilio + "<IdLocalidad></IdLocalidad>"; }
            if (id_provincia) { XMLDomicilio = XMLDomicilio + "<ProvinciaId>" + id_provincia + "</ProvinciaId>"; }
            else { XMLDomicilio = XMLDomicilio + "<ProvinciaId></ProvinciaId>"; }
            if (tipo_domicilio) { XMLDomicilio = XMLDomicilio + "<TipoDomicilioId>" + tipo_domicilio + "</TipoDomicilioId>"; }
            else { XMLDomicilio = XMLDomicilio + "<TipoDomicilioId></TipoDomicilioId>"; }
            XMLDomicilio = XMLDomicilio + "</DatosDomicilio>";

            var clave = tipo_domicilio + "-" + desc_via + "-" + nro_puerta + "-" + desc_localidad;

            if (typeof $("#editarLocal").val() !== "undefined") {
                if ($("#editarLocal").val() == "true") {

                    $('#Domicilios').dataTable().fnDeleteRow($("#editarLocalIndex").val());
                    //$("#Domicilios tbody .selected").remove();
                    $("#editarLocal").val("");
                }
            }

            if (id_domicilio === 0) {
                var tabla = $('#Domicilios').dataTable().api();
                var index;
                try {
                    index = tabla
                        .column(7)
                        .data()
                        .indexOf(clave);
                }
                catch (err) {
                    index = -1;
                    //mensaje = err.message;
                    mensaje = "";
                }

                if (index > 0) {
                    mensaje = "El Domicilio ya existe.";
                }
            }



            if (mensaje) {
                $('#TituloInfoDomicilio').html("Información - Datos Domicilio");
                $('#DescripcionInfoDomicilio').html("Los datos del domicilio están incompletos o erróneos: " + mensaje);
                $("#ModalInfoDomicilio").modal('show');
            }
            else {
                $(document).trigger({ type: 'domicilioGuardado', domicilio: data });
                $(document).trigger({
                    type: 'actualizaDataDomicilio', domicilio: {
                        id_domicilio: id_domicilio,
                        id_registro: id_registro,
                        XMLDomicilio: XMLDomicilio,
                        desc: desc,
                        desc_domicilio: desc_domicilio,
                        desc_cp: desc_cp,
                        desc_localidad: desc_localidad,
                        clave: clave
                    }
                });
                $("#modal-window-domicilio").modal('hide');
            }
        }
        else {
            $('#TituloInfoDomicilio').html("Información - Datos Domicilio");
            $('#DescripcionInfoDomicilio').html("Los datos del domicilio están incompletos o erróneos: " + mensaje);
            $("#ModalInfoDomicilio").modal('show');
        }


    });

    $("#btnCancelarDomicilio").click(function () {
        var msj = "Está a punto de cancelar la operación y se perderán los cambios." + '<br>' + '¿Desea Continuar?';
        $('#TituloAdvertenciaDomicilio').html("Advertencia - Cancelar Operación");
        $('#TituloAdvertenciaDomicilio').css({ "font-size": "16px" });
        $('#DescripcionAdvertenciaDomicilio').html(msj);
        $('#ModalAdvertenciaDomicilio').modal('show');
    });

    ajustarmodalDomicilio();
    confListasDomicilio();
    var lista = document.getElementById("lstViaNombre");
    lista.style.display = "block";

    var elcp = document.getElementById("DatosDomicilio_codigo_postal");
    elcp.style.display = "none";
    var elcptxt = document.getElementById("txtcodigo_postal");
    elcptxt.style.display = "block";

    //if ($("#DatosDomicilio_localidad").val() != 281) {
    //    var lista = document.getElementById("lstViaNombre");
    //    lista.style.display = "none";

    //    var elcp = document.getElementById("DatosDomicilio_codigo_postal");
    //    elcp.style.display = "block";
    //    var elcptxt = document.getElementById("txtcodigo_postal");
    //    elcptxt.style.display = "none";
    //}
    //else {
    //    var lista = document.getElementById("lstViaNombre");
    //    lista.style.display = "block";

    //    var elcp = document.getElementById("DatosDomicilio_codigo_postal");
    //    elcp.style.display = "none";
    //    var elcptxt = document.getElementById("txtcodigo_postal");
    //    elcptxt.style.display = "block";
    //}

    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-domicilio .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-domicilio").modal('show');

    $("#DatosDomicilio_pais, #DatosDomicilio_provincia, #DatosDomicilio_municipio, #DatosDomicilio_localidad").change(function () {

        $("#DatosDomicilio_ViaNombre").val("");
        $("lstViaNombre").find('option').remove();

        if ($("#DatosDomicilio_pais option:selected").text() == "EXTERIOR") {
            $('#txtDatosDomicilio_provincia').val('');
            $('#txtDatosDomicilio_municipio').val('');
            $('#txtDatosDomicilio_localidad').val('');
            $('#DatosDomicilio_ViaNombre').val('');
            $('#DatosDomicilio_piso').val('');
            $('#DatosDomicilio_codigo_postal').val('');
            $('#DatosDomicilio_ubicacion').val('');
            $('#DatosDomicilio_numero_puerta').val('');
            $('#DatosDomicilio_unidad').val('');
            document.getElementById("SinNumeroCheck").checked = false;
            $("#DatosDomicilio_numero_puerta").prop("disabled", false);
        } else if ($("#DatosDomicilio_provincia option:selected").text() != "corrientes") {
            $('#txtDatosDomicilio_municipio').val('');
            $('#txtDatosDomicilio_localidad').val('');
            $('#DatosDomicilio_ViaNombre').val('');
            $('#DatosDomicilio_piso').val('');
            $('#DatosDomicilio_codigo_postal').val('');
            $('#DatosDomicilio_ubicacion').val('');
            $('#DatosDomicilio_numero_puerta').val('');
            $('#DatosDomicilio_unidad').val('');
            $("#DatosDomicilio_numero_puerta").prop("disabled", false);
            document.getElementById("SinNumeroCheck").checked = false;
        }
    })

    $("#SinNumeroCheck").on('click', function () {
        if ($('#SinNumeroCheck:checked').length == 1) {
            $("#DatosDomicilio_numero_puerta").attr("disabled", "disabled");
            $("#DatosDomicilio_numero_puerta").val("S/N");
        }
        else {
            $("#DatosDomicilio_numero_puerta").prop("disabled", false);
            $("#DatosDomicilio_numero_puerta").val("");
        }
    })

    if ($("#DatosDomicilio_numero_puerta").val() == "S/N") {
        $("#SinNumeroCheck").click();
    }
};
function ajustarmodalDomicilio() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter; //value corresponding to the modal heading + footer
    $(".domicilio-body", "#scroll-content-domicilio").css({ "height": altura, "overflow": "hidden" });
    $('.domicilio-content').css({ "max-height": altura + 'px' });
    $('#accordion-domicilio').css({ "max-height": altura + 'px' });
    ajustarScrollBarsDomicilio();
}
function ajustarScrollBarsDomicilio() {
    $(".domicilio-content").getNiceScroll().resize();
    $(".domicilio-content").getNiceScroll().show();
}

function FormatFechaHoraDomicilio(Fecha_Entrada, FechaHora) {
    if (Fecha_Entrada == null) {
        return "";
    } else {

        Fecha_Entrada = Fecha_Entrada.replace('T', ' ');
    }

    var Fecha = new Date(Fecha_Entrada);

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

    if (FechaHora == undefined) {
        FechaHora = '';
    }

    if (FechaHora == '') {
        return (curr_date + "/" + curr_month + "/" + curr_year);

    } else {
        return (curr_date + "/" + curr_month + "/" + curr_year + " " + curr_hours + ":" + curr_minutes + ":" + curr_seconds);
    }
}

function ValidarDatosDomicilio() {
    var mensaje = "";
    var provincia = $("#DatosDomicilio_provincia :selected ").text();
    var lista = document.getElementById("lstViaNombre");
    if (($("#lstViaNombre").val() == null || $("#lstViaNombre").val() == "" || $("#lstViaNombre").val() == "0") &&
        ($("#DatosDomicilio_ViaNombre").val() == null || $("#DatosDomicilio_ViaNombre").val() == "") && (provincia == "Corrientes"))
        mensaje = "Debe Ingresar el nombre de la Vía";

    if (($("#DatosDomicilio_TipoDomicilioId").val() == null || $("#DatosDomicilio_TipoDomicilioId").val() == "") && (provincia == "Corrientes"))
        mensaje = "Debe Ingresar el tipo de domicilio";
    if (($("#DatosDomicilio_pais").val() == null || $("#DatosDomicilio_pais").val() == "") && (provincia == "Corrientes"))
        mensaje = "Debe Ingresar el Pais";
    if (($("#DatosDomicilio_numero_puerta").val() == null || $("#DatosDomicilio_numero_puerta").val() == "") && (provincia == "Corrientes"))
        mensaje = "Debe Ingresar el número de puerta";

    var prov = document.getElementById("DatosDomicilio_provincia");
    if (prov.style.display == "block" || prov.style.display == "") {
        if (($("#DatosDomicilio_provincia").val() == null || $("#DatosDomicilio_provincia").val() == "") && (provincia == "Corrientes"))
            mensaje = "Debe Ingresar la Provincia";
    }
    else {
        if (($("#txtDatosDomicilio_provincia").val() == null || $("#txtDatosDomicilio_provincia").val() == "") && (provincia == "Corrientes"))
            mensaje = "Debe Ingresar la Provincia";
    }

    var muni = document.getElementById("DatosDomicilio_municipio");
    if (muni.style.display == "block" || muni.style.display == "") {
        if (($("#DatosDomicilio_municipio").val() == null || $("#DatosDomicilio_municipio").val() == "") && (provincia == "Corrientes"))
            mensaje = "Debe Ingresar el Departamento";
    }
    else {
        if (($("#txtDatosDomicilio_municipio").val() == null || $("#txtDatosDomicilio_municipio").val() == "") && (provincia == "Corrientes"))
            mensaje = "Debe Ingresar el Departamento";
    }

    var loca = document.getElementById("DatosDomicilio_localidad");
    if (loca.style.display == "block" || loca.style.display == "") {
        if (($("#DatosDomicilio_localidad").val() == null || $("#DatosDomicilio_localidad").val() == "") && (provincia == "Corrientes"))
            mensaje = "Debe Ingresar la Localidad";
    }
    else {
        if (($("#txtDatosDomicilio_localidad").val() == null || $("#txtDatosDomicilio_localidad").val() == "") && (provincia == "Corrientes"))
            mensaje = "Debe Ingresar la Localidad";
    }

    var piso = $("#DatosDomicilio_piso").val();
    if (!validarSiNumero(piso) && piso != "" & piso != null && (provincia == "Corrientes")) {
        mensaje = "La identificación del piso debe ser un valor numérico.";
    }

    if ($('#SinNumeroCheck:checked').length < 1) {
        var puerta = $("#DatosDomicilio_numero_puerta").val();
        if (!validarSiNumero(puerta) && puerta != "" & puerta != null && (provincia == "Corrientes")) {
            mensaje = "El número de puerta debe ser un valor numérico.";
        }
    }

    var unidad = $("#DatosDomicilio_unidad").val();
    if (!validarSiAlfanumerico(unidad) && unidad != "" & unidad != null && (provincia == "Corrientes")) {
        mensaje = "La identificación del departamento debe ser un valor alfanumérico.";
    }
    var cp = $("#DatosDomicilio_codigo_postal").val();
    if (!validarSiAlfanumerico(cp) && cp != "" & cp != null && (provincia == "Corrientes")) {
        mensaje = "El código postal debe ser un valor alfanumérico.";
    }

    //Controla si el tramo existe.
    var id_via = $("#lstViaNombre").val();
    var valor_altura = $("#DatosDomicilio_numero_puerta").val();
    if (id_via != null && id_via != null && (provincia == "Corrientes")) {
        var existe = "";
        $.ajax({
            type: 'POST',
            async: false,
            url: BASE_URL + 'Domicilio/GetExisteTramoJson',
            dataType: 'json',
            data: { ViaId: id_via, altura: valor_altura },
            success: function (data) {
                mensaje = data;
            }, error: function (ex) {
                existe = "";
            }
        });
    }

    return mensaje;
}

function validarSiNumero(numero) {
    if (!/^([0-9])*$/.test(numero))
        return 0;
    else
        return 1;
}

function validarSiAlfanumerico(valor) {
    if (!/^[a-z\d]+$/i.test(valor))
        return false;
    return true;
}

function VaciarListaVias() {
    // Elimina los items actuales de vias..
    var lista = document.getElementById("lstViaNombre");
    var intTotalItems = lista.options.length;
    for (var intCounter = intTotalItems; intCounter >= 0; intCounter--) {
        lista.remove(intCounter);
    }
}

function mostarlistaviaDomicilio(palabra) {
    var loca = document.getElementById("DatosDomicilio_localidad");
    var IdLoca = $("#DatosDomicilio_localidad").val();
    if (loca.style.display === "block" || !loca.style.display) {
        if (palabra.length >= 3) {
            // Elimina los items actuales.
            var lista = document.getElementById("lstViaNombre");
            var intTotalItems = lista.options.length;
            for (var intCounter = intTotalItems; intCounter >= 0; intCounter--) {
                lista.remove(intCounter);
            }
            $.ajax({
                type: 'POST',
                url: BASE_URL + 'Domicilio/GetViasPorNombreJson',
                dataType: 'json',
                data: { id: palabra, LocaId: IdLoca },
                success: function (Objetos) {
                    $.each(Objetos, function (i, Objeto) {
                        $("#lstViaNombre").append("<option value='" + Objeto.ViaId + "'>" + Objeto.Nombre + "</option>");
                    });
                }, error: function (ex) {
                    alert('Error al recuperar los objetos' + ex);
                }
            });
        }
    }
}

function CargarLocalidad() {
    var idPadre = $("#DatosDomicilio_municipio").val();
    var lista = document.getElementById("DatosDomicilio_localidad");
    var intTotalItems = lista.options.length;
    for (var intCounter = intTotalItems; intCounter >= 0; intCounter--) {
        lista.remove(intCounter);
    }
    if (idPadre != null && idPadre != "") {
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'Domicilio/GetLocalidadPorPadreJson',
            dataType: 'json',
            data: { id: idPadre },
            success: function (Objetos) {
                $.each(Objetos, function (i, Objeto) {
                    $("#DatosDomicilio_localidad").append("<option value='" + Objeto.FeatId + "'>" + Objeto.Nombre + "</option>");
                });

                var lista = document.getElementById("DatosDomicilio_localidad");
                var intTotalItems = lista.options.length;
                if (intTotalItems == 0) {
                    EnableComboLocalidad(false);
                }
                else {
                    EnableComboLocalidad(true);
                }
            }, error: function (ex) {
                alert('Error al recuperar los objetos' + ex);
                EnableComboLocalidad(false);
            }
        });
    }

}

function CargarPartido() {
    var idPadre = $("#DatosDomicilio_provincia").val();
    var lista = document.getElementById("DatosDomicilio_municipio");
    var intTotalItems = lista.options.length;
    for (var intCounter = intTotalItems; intCounter >= 0; intCounter--) {
        lista.remove(intCounter);
    }
    if (idPadre != null && idPadre != "") {
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'Domicilio/GetPartidoPorPadreJson',
            dataType: 'json',
            data: { id: idPadre },
            success: function (Objetos) {
                $.each(Objetos, function (i, Objeto) {
                    $("#DatosDomicilio_municipio").append("<option value='" + Objeto.FeatId + "'>" + Objeto.Nombre + "</option>");
                });

                var lista = document.getElementById("DatosDomicilio_municipio");
                var intTotalItems = lista.options.length;
                if (intTotalItems == 0) {
                    EnableComboPartido(false);
                }
                else {
                    EnableComboPartido(true);
                }

                CargarLocalidad();
            }, error: function (ex) {
                alert('Error al recuperar los objetos' + ex);
                EnableComboPartido(false);
            }
        });
    }
}

function CargarProvincia() {
    var idPadre = $("#DatosDomicilio_pais").val();
    var lista = document.getElementById("DatosDomicilio_provincia");
    var intTotalItems = lista.options.length;
    for (var intCounter = intTotalItems; intCounter >= 0; intCounter--) {
        lista.remove(intCounter);
    }
    if (idPadre != null && idPadre != "") {
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'Domicilio/GetProvinciaPorPadreJson',
            dataType: 'json',
            data: { id: idPadre },
            success: function (Objetos) {

                //var tmpAry = new Array();
                //var i = 0;
                $.each(Objetos, function (i, Objeto) {
                    //tmpAry[i] = new Array();
                    //tmpAry[i][0] = Objeto.Nombre;
                    //tmpAry[i][1] = Objeto.FeatId;

                    $("#DatosDomicilio_provincia").append("<option value='" + Objeto.FeatId + "'>" + Objeto.Nombre + "</option>");
                    //i++;
                });
                //tmpAry.sort();
                //for (var i = 0; i < tmpAry.length; i++) {
                //    $("#DatosDomicilio_provincia").append("<option value='" + tmpAry[i][1] + "'>" + tmpAry[i][0] + "</option>");
                //}


                var lista = document.getElementById("DatosDomicilio_provincia");
                var intTotalItems = lista.options.length;
                //if (paisSel != 1) {
                if (intTotalItems == 0) {
                    EnableComboProvincia(false);
                }
                else {
                    EnableComboProvincia(true);
                }
                CargarPartido();
            }, error: function (ex) {
                alert('Error al recuperar los objetos' + ex);
                EnableComboProvincia(false);
            }
        });
    }
}

function provinciaListChangeDomicilio() {
    VaciarListaVias();
    CargarPartido();
}

function municipioListChangeDomicilio() {
    VaciarListaVias();
    CargarLocalidad();
}

function paisesListChangeDomicilio() {
    VaciarListaVias();
    CargarProvincia();
}

function confListasDomicilio() {
    $("#txtDatosDomicilio_provincia").val($("#hdfTxtProvincia").val());
    $("#txtDatosDomicilio_municipio").val($("#hdfTxtPartido").val());
    $("#txtDatosDomicilio_localidad").val($("#hdfTxtLocalidad").val());

    $("#DatosDomicilio_provincia").val($("#hdfItemProvincia").val());
    $("#DatosDomicilio_municipio").val($("#hdfItemPartido").val());
    $("#DatosDomicilio_localidad").val($("#hdfItemLocalidad").val());
    $("#DatosDomicilio_pais").val($("#hdfItemPais").val());
    if ($("#DatosDomicilio_ViaNombre").val()) {
        mostarlistaviaDomicilio($("#DatosDomicilio_ViaNombre").val());
    }
    $("#lstViaNombre").val($("#hdfViaId").val());

    if (!$("#hdfItemPais").val() || $("#hdfItemPais").val() === "0") {
        EnableComboProvincia(false);
    }
    if (!$("#hdfItemProvincia").val() || $("#hdfItemProvincia").val() === "0") {
        EnableComboProvincia(false);
    }
    if (!$("#hdfItemPartido").val() || $("#hdfItemPartido").val() === "0") {
        EnableComboPartido(false);
    }

    hideLoading();

    //if ($("#DatosDomicilio_ViaNombre").val() != null) {
    //    mostarlistavia($("#DatosDomicilio_ViaNombre").val());
    //    var elcp = document.getElementById("DatosDomicilio_codigo_postal");
    //    elcp.style.display = "none";
    //    var elcptxt = document.getElementById("txtcodigo_postal");
    //    elcptxt.style.display = "block";
    //    if ($("#DatosDomicilio_codigo_postal").val() == "") {
    //        $("#DatosDomicilio_codigo_postal").val("9100");
    //        $("#txtcodigo_postal").val("9100");
    //    }
    //}
    //else {
    //    var elcp = document.getElementById("DatosDomicilio_codigo_postal");
    //    elcp.style.display = "block";
    //    var elcptxt = document.getElementById("txtcodigo_postal");
    //    elcptxt.style.display = "none";
    //}
    //$("#lstViaNombre").val($("#hdfItemPais").val());
}

function EnableComboProvincia(habilita) {
    if (habilita == false) {
        var prov = document.getElementById("DatosDomicilio_provincia");
        prov.style.display = "none";

        var prov = document.getElementById("txtDatosDomicilio_provincia");
        prov.style.display = "block";
    }
    else {
        var prov = document.getElementById("DatosDomicilio_provincia");
        prov.style.display = "block";

        var prov = document.getElementById("txtDatosDomicilio_provincia");
        prov.style.display = "none";
    }
    EnableComboPartido(habilita);
}

function EnableComboPartido(habilita) {
    if (habilita == false) {
        var muni = document.getElementById("DatosDomicilio_municipio");
        muni.style.display = "none";

        var muni = document.getElementById("txtDatosDomicilio_municipio");
        muni.style.display = "block";
    }
    else {
        var muni = document.getElementById("DatosDomicilio_municipio");
        muni.style.display = "block";

        var muni = document.getElementById("txtDatosDomicilio_municipio");
        muni.style.display = "none";
    }
    EnableComboLocalidad(habilita);
}

function EnableComboLocalidad(habilita) {
    if (habilita == false) {
        var loca = document.getElementById("DatosDomicilio_localidad");
        loca.style.display = "none";

        var loca = document.getElementById("txtDatosDomicilio_localidad");
        loca.style.display = "block";

        var lista = document.getElementById("lstViaNombre");
        lista.style.display = "none";

        //Permite la carga del código postal manual.
        var elcp = document.getElementById("DatosDomicilio_codigo_postal");
        elcp.style.display = "block";
        var elcptxt = document.getElementById("txtcodigo_postal");
        elcptxt.style.display = "none";
        $("#DatosDomicilio_codigo_postal").val("");
        $("#txtcodigo_postal").val("");
    }
    else {
        var loca = document.getElementById("DatosDomicilio_localidad");
        loca.style.display = "block";
        sortSelect(loca);

        var loca = document.getElementById("txtDatosDomicilio_localidad");
        loca.style.display = "none";

        var lista = document.getElementById("lstViaNombre");
        lista.style.display = "block";

        //Anula la carga del código postal manual, pudiendo ser cargado solo desde la selección de la vía.
        var lista = document.getElementById("lstViaNombre");
        lista.style.display = "block";

        var elcp = document.getElementById("DatosDomicilio_codigo_postal");
        elcp.style.display = "none";
        var elcptxt = document.getElementById("txtcodigo_postal");
        elcptxt.style.display = "block";
    }
}

function runValidaAltura(e) {
    if (e.keyCode === 13) {
        if ($("#DatosDomicilio_numero_puerta").val() && $("#lstViaNombre").val())
            fnBuscarCPA($("#lstViaNombre").val(), $("#DatosDomicilio_numero_puerta").val());
        return false;
    }
}

function fnBuscarCPA(id_via, valor_altura) {
    showLoading();
    var cpa = "";
    $.post(BASE_URL + "Domicilio/GetCpaTramoViaJson", { ViaId: id_via, altura: valor_altura })
        .done(function (data) {
            $("#DatosDomicilio_codigo_postal").val(data);
            $("#txtcodigo_postal").val(data);
        })
        .fail(function (data) {
            console.log(data);
        }).always(function () {
            hideLoading();
        });
}

function sortSelect(selElem) {
    var tmpAry = new Array();
    for (var i = 0; i < selElem.options.length; i++) {
        tmpAry[i] = new Array();
        tmpAry[i][0] = selElem.options[i].text;
        tmpAry[i][1] = selElem.options[i].value;
    }
    tmpAry.sort();
    while (selElem.options.length > 0) {
        selElem.options[0] = null;
    }
    for (var i = 0; i < tmpAry.length; i++) {
        var op = new Option(tmpAry[i][0], tmpAry[i][1]);
        selElem.options[i] = op;
    }
    return;
}
//@ sourceURL=abmDomicilio.js
