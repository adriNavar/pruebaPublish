var Grilla_Novedades;
var mapaEstados;
var nombreEstado;

$(document).ready(init);
$(window).resize(ajustarmodal);

$('#modal-gestionNovedades').one('shown.bs.modal', function () {
    //debugger;
    ajustarmodal();
    hideLoading();
});

function init() {
    //$("#modal-gestionNovedades .date .form-control").datepicker({
    //    orientation: "top auto",
    //    //language: "es",
    //    todayBtn: "linked",
    //    autoclose: true,
    //    todayHighlight: true,
    //    format: 'dd/MM/yyyy',
    //    clearBtn: true
    //});

    //$("#fechaDesde").datepicker.regional;
    //$(function () {
        $('#fechaDesde').datepicker($.datepicker.regional["es"]);
    //});

    createDataTable("Grilla_Novedades", 0, {
        dom: "frtip",
        pageLength: 5,
        scrollCollapse: true,
        columnDefs: [
        {
            "orderable": false,
            "targets": [0]
            }]
    });

    let estados = $('#Estados').val(); 
    const tmpArray = estados.split(";");

    mapaEstados = new Map();
    for (const element of tmpArray) {
        let esta = element.split("-");
        
        if (esta[0] != "") {
            mapaEstados.set(esta[0], esta[1]);
        }
    }

    $('#modal-gestionNovedades').modal("show");
}

function createDataTable(tableId, scrollY, options) {
    let hoy = new Date();
    let hoyStr = hoy.getDate() + '-' + (hoy.getMonth() + 1) + '-' + hoy.getFullYear();
    
    var config = {
        dom: 'frtip', 
        buttons: [{
            extend: "excelHtml5",
            text: "<span class='fa fa-file-excel-o fa-2x light-blue cursor-pointer' title: 'Exportar a Excel' />",
            filename: function () {
                return 'Gestión de Novedades - ' + hoyStr;
            }
        }],
        language: {
            url: BASE_URL + "Scripts/dataTables.spanish.txt"
        }
    };
    config = $.extend(config, options);
    $("#" + tableId).DataTable(config);
}

function ajustarmodal() {
    var altura = $(window).height() - 150; //value corresponding to the modal heading + footer
    $(".gestionNovedades-body").css({ "height": altura, "overflow-y": "auto" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $('#gestionNovedades-content').css({ "max-height": $('#gestionNovedades-content').parent().height() + 'px', "height": "100%" });
    $('#gestionNovedades-content').getNiceScroll().resize();
    $('#gestionNovedades-content').getNiceScroll().show();
}

function Consultar() {
    showLoading();
    //debugger;
    var idMun = $('#IdMunicipio').val();
    if (idMun == 1) {
        idMun = $("#cboMunicipios").val();
    }
    var desde = $('#fechaDesde').val();
    var tdo = $('#switch-label-todo').prop('checked');
    var nvas = $('#switch-label-nuevas').prop('checked');
    var desc = $('#switch-label-descartadas').prop('checked');
    destroyDataTable('Grilla_Novedades');

    $.ajax({
        type: "POST",
        async: false,
        url: BASE_URL + 'InterfaseProvincial/Consultar',
        dataType: 'json',
        data: {
            idMunicipio: idMun,
            fDesde: desde,
            todo: tdo,
            nuevas: nvas,
            descartadas: desc
        },
        success: function (data) {
            Grilla_Novedades = $('#Grilla_Novedades').DataTable({
                scrollCollapse: true,
                paging: true,
                searching: true,
                processing: true,
                dom: 'frtip',
                pageLength: 5,
                language: {
                    url: BASE_URL + "Scripts/dataTables.spanish.txt"
                },
                columns: [
                    { data: 'IdDiferencia', orderable: false },
                    { data: 'MunPartida' },
                    { data: 'MunNomenclatura' },
                    { data: 'MunTipo' },
                    { data: 'PrvSupTierraRegis' },
                    { data: 'MunSupTierraRegis' },
                    { data: 'PrvSupTierraRelev' },
                    { data: 'MunSupTierraRelev' },
                    { data: 'MunUnidadMedida' },
                    { data: 'PrvSupMejoraRegis' },
                    { data: 'MunSupMejoraRegis' },
                    { data: 'PrvSupMejoraRelev' },
                    { data: 'MunSupMejoraRelev' },
                    { data: 'PrvIdEstado' },
                    { data: 'PrvUltCambio' },
                    { data: 'IdDiferencia', orderable: false }
                ],
                columnDefs: [{
                    "targets": [0],
                    "orderable": false,
                    "render": function (data, type) {
                        return type === 'display' ? '<input type="checkbox" value="' + data + '" /> <label style="display: none;">' + data : data + '</label>';
                    }
                },
                {
                    'targets': [13],
                    "render": function (data, type) {

                        mapaEstados.forEach((valor, clave) => {
                            if (clave == data) {
                                nombreEstado = valor;
                            }
                        });

                        est = nombreEstado;
                        return est;
                    }
                },
                {
                    'targets': [14],
                    'type': Date,
                    'render': function (data, type, full) {
                        f = data.substring(6, 19);
                        d = new Date(parseInt(f));
                        month = '' + (d.getMonth() + 1),
                            day = '' + d.getDate(),
                            year = d.getFullYear();

                        if (month.length < 2)
                            month = '0' + month;
                        if (day.length < 2)
                            day = '0' + day;

                        return [day, month, year].join('/');
                    }
                },
                {
                    'targets': [15],
                    'render': function (data, type, row) {
                        return type === 'display' ? '<button type="button" style="font-size: 9px; font-color: black; background-color:transparent;" onclick="showModalDetalle(' + data + ')" class="btn"><i class="fa fa-eye fa-2x light-blue cursor-pointer" title="Ver Detalle"></i></button> <label style="display: none;">' + data : data + '</label>';
                    }
                }
                ],
                data: data
            });
        }
    });

    hideLoading();

}

function HabilitaBotones(val) {
    
    if (val > 1) {
        $("#procesarButton").prop('disabled', false); 
        $("#consultarButton").prop('disabled', false);
        Consultar();
    }
    //else {
    //    $("#procesarButton").prop('disabled', true);
    //    $("#consultarButton").prop('disabled', true);
    //}
}

function Procesar() {
    showLoading();
    var procesaOk = false;
    var idMun = $('#IdMunicipio').val();

    if (idMun == 1) {
        idMun = $("#cboMunicipios").val();
    }

    destroyDataTable('Grilla_Novedades');

    $.ajax({
        type: "POST",
        async: false,
        url: BASE_URL + 'InterfaseProvincial/ProcesaDiferencias',
        dataType: 'json',
        data: {
            idMunicipio: idMun
        },
        success: function (data) {
            Grilla_Novedades = $('#Grilla_Novedades').DataTable({
                scrollCollapse: true,
                paging: true,
                searching: true,
                processing: true,
                dom: 'frtip',
                pageLength: 5,
                language: {
                    url: BASE_URL + "Scripts/dataTables.spanish.txt"
                },
                columns: [
                    { data: 'IdDiferencia', orderable: false },
                    { data: 'MunPartida' },
                    { data: 'MunNomenclatura' },
                    { data: 'MunTipo' },
                    { data: 'PrvSupTierraRegis' },
                    { data: 'MunSupTierraRegis' },
                    { data: 'PrvSupTierraRelev' },
                    { data: 'MunSupTierraRelev' },
                    { data: 'MunUnidadMedida' },
                    { data: 'PrvSupMejoraRegis' },
                    { data: 'MunSupMejoraRegis' },
                    { data: 'PrvSupMejoraRelev' },
                    { data: 'MunSupMejoraRelev' },
                    { data: 'PrvIdEstado' },
                    { data: 'PrvUltCambio' },
                    { data: 'IdDiferencia', orderable: false }
                ],
                columnDefs: [{
                    "targets": [0],
                    "orderable": false,
                    "render": function (data, type) {
                        return type === 'display' ? '<input type="checkbox" value="' + data + '" /> <label style="display: none;">' + data : data + '</label>';
                    }
                },
                {
                    'targets': [13],
                    "render": function (data, type) {

                        mapaEstados.forEach((valor, clave) => {
                            if (clave == data) {
                                nombreEstado = valor;
                            }
                        });

                        est = nombreEstado;
                        return est;
                    }
                },
                {
                    'targets': [14],
                    'type': Date,
                    'render': function (data, type, full) {
                        f = data.substring(6, 19);
                        d = new Date(parseInt(f));
                        month = '' + (d.getMonth() + 1),
                            day = '' + d.getDate(),
                            year = d.getFullYear();

                        if (month.length < 2)
                            month = '0' + month;
                        if (day.length < 2)
                            day = '0' + day;

                        return [day, month, year].join('/');
                    }
                },
                {
                    'targets': [15],
                    'render': function (data, type, row) {
                        return type === 'display' ? '<button type="button" style="font-size: 9px; font-color: black; background-color:transparent;" onclick="showModalDetalle(' + data + ')" class="btn"><i class="fa fa-eye fa-2x light-blue cursor-pointer" title="Ver Detalle"></i></button> <label style="display: none;">' + data : data + '</label>';
                    }
                }
                ],
                data: data
            });
            procesaOk = true;
            alert("Datos Generados Correctamente.");
        },
        error: function (xhr, status, data) {
            alert(data);
        }
    });

    if (procesaOk) {
        let ahora = new Date();
        let month = '' + (ahora.getMonth() + 1);
        let day = '' + ahora.getDate();
        let year = ahora.getFullYear();
        let hora = '' + ahora.getHours();
        let min = '' + ahora.getMinutes();
        let sec = '' + ahora.getSeconds();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;
        if (hora.length < 2)
            hora = '0' + hora;
        if (min.length < 2)
            min = '0' + min;
        if (sec.length < 2)
            sec = '0' + sec;
        $("#fecUltProc").val(day + "/" + month + "/" + year + " " + hora + ":" + min + ":" + sec);
    }
    hideLoading();
}

function destroyDataTable(tableId) {
    var id = "#" + tableId;
    if ($.fn.DataTable.isDataTable(id)) {
        var table = $(id).dataTable();
        table.api().clear().draw();
        table.api().destroy();

    }
}

function CambioEstado() {
    var table = $('#Grilla_Novedades').DataTable();
    var idMun = $('#IdMunicipio').val();
    if (idMun == 1) {
        idMun = $("#cboMunicipios").val();
    }

    var data = table.rows().nodes();
    var idModif = '';

    data.each(function (value, index) {
        if (value.cells[0].children[0].checked) {
            idModif = idModif + value.cells[0].children[0].value + ';';
        }
    });
    idModif = idModif.substr(0, idModif.length - 1);
    var idCambio = $('#cboEstados').val();

    if (idModif == '') {
        alert('Debe seleccionar algún registro.');
    }
    else {
        destroyDataTable('Grilla_Novedades');

        $.ajax({
            type: "POST",
            async: false,
            url: BASE_URL + 'InterfaseProvincial/CambioEstado',
            dataType: 'json',
            data: {
                idMuni: idMun,
                idDiferencia: idModif,
                estado: idCambio
            },
            success: function (data) {
                Grilla_Novedades = $('#Grilla_Novedades').DataTable({
                    scrollCollapse: true,
                    paging: true,
                    searching: true,
                    processing: true,
                    dom: 'frtip',
                    pageLength: 5,
                    language: {
                        url: BASE_URL + "Scripts/dataTables.spanish.txt"
                    },
                    columns: [
                        { data: 'IdDiferencia', orderable: false },
                        { data: 'MunPartida' },
                        { data: 'MunNomenclatura' },
                        { data: 'MunTipo' },
                        { data: 'PrvSupTierraRegis' },
                        { data: 'MunSupTierraRegis' },
                        { data: 'PrvSupTierraRelev' },
                        { data: 'MunSupTierraRelev' },
                        { data: 'MunUnidadMedida' },
                        { data: 'PrvSupMejoraRegis' },
                        { data: 'MunSupMejoraRegis' },
                        { data: 'PrvSupMejoraRelev' },
                        { data: 'MunSupMejoraRelev' },
                        { data: 'PrvIdEstado' },
                        { data: 'PrvUltCambio' },
                        { data: 'IdDiferencia', orderable: false }
                    ],
                    columnDefs: [{
                        "targets": [0],
                        "orderable": false,
                        "render": function (data, type) {
                            return type === 'display' ? '<input type="checkbox" value="' + data + '" /> <label style="display: none;">' + data : data + '</label>';
                        }
                    },
                    {
                        'targets': [13],
                        "render": function (data, type) {

                            mapaEstados.forEach((valor, clave) => {
                                if (clave == data) {
                                    nombreEstado = valor;
                                }
                            });

                            est = nombreEstado;
                            return est;
                        }
                    },
                    {
                        'targets': [14],
                        'type': Date,
                        'render': function (data, type, full) {
                            f = data.substring(6, 19);
                            d = new Date(parseInt(f));
                            month = '' + (d.getMonth() + 1),
                                day = '' + d.getDate(),
                                year = d.getFullYear();

                            if (month.length < 2)
                                month = '0' + month;
                            if (day.length < 2)
                                day = '0' + day;

                            return [day, month, year].join('/');
                        }
                    },
                    {
                        'targets': [15],
                        'render': function (data, type, row) {
                            return type === 'display' ? '<button type="button" style="font-size: 9px; font-color: black; background-color:transparent;" onclick="showModalDetalle(' + data + ')" class="btn"><i class="fa fa-eye fa-2x light-blue cursor-pointer" title="Ver Detalle"></i></button> <label style="display: none;">' + data : data + '</label>';
                        }
                    }
                    ],
                    data: data
                });
            }
        });
    }
}

function generarGrillaNovedades() {
    var loc = BASE_URL + "InterfaseProvincial/DownloadFile";
    var idMun = $('#IdMunicipio').val();

    if (idMun == 1) {
        idMun = $("#cboMunicipios").val();
    }

    $.ajax({
        url: BASE_URL + "InterfaseProvincial/GrillaDiferencias",
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        data: {
            idMunicipio: idMun
        },
        success: function () {
            window.location = loc;
        },
        error: function (error) {
            mostrarMensaje("Generar Reporte Parcelario", error.statusText, 3);
        },
        complete: hideLoading
    });
    return false;
}

function generarGrillaNovedadesDetalle() {
    var loc = BASE_URL + "InterfaseProvincial/DownloadFile";
    var idDif = $("#idDiferencia").val();

    $.ajax({
        url: BASE_URL + "InterfaseProvincial/GrillaDiferenciasDetalle",
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        data: {
            idDiferencia: idDif
        },
        success: function () {
            window.location = loc;
        },
        error: function (error) {
            mostrarMensaje("Generar Reporte Parcelario", error.statusText, 3);
        },
        complete: hideLoading
    });
    return false;
}