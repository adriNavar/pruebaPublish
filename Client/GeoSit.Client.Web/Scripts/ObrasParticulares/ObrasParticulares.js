var options;
var Grilla_UnidadesTributarias;
var Grilla_Documentos;
var currEditObject;
var selectedUT;
var selectedDoc;

var removeResult = "";
var tiposDocumento = null;
var _enabled = true;

$(document).ready(init);
$(window).resize(ajustarmodal);



function GetNameFromMonthNumber(month) {
    var monthNames = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];
    return monthNames[+month];
}

function GetMonthNumberFromName(month) {
    var monthNames = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];

    var monthNumber = (monthNames.indexOf(month) + 1);
    if (monthNumber < 10) monthNumber = "0" + monthNumber;

    return monthNumber;
}

function ChangeFechaDesde() {
    if (+$("#InspeccionId").val() <= 0) {
        if (new Date(convertDateFromText($("#FechaHoraDesde").val())) != "Invalid Date") {
            $("#FechaOrigenDesde").val($("#FechaHoraDesde").val());
            $("#FechaHoraHasta").val(moment(new Date(Date($("#FechaHoraDesde").val()))).add(1, 'hour').format('DD/MM/YYYY HH:mm'));
        }
        fnVerSiPuedeVerResultado();
    }
}

function ChangeFechaHasta() {
    if (+$("#InspeccionId").val() <= 0) {
        if (new Date(convertDateFromText($("#FechaHoraHasta").val())) != "Invalid Date") {
            $("#FechaOrigenHasta").val($("#FechaHoraHasta").val());
            fnVerSiPuedeVerResultado();
        }
    }
}


$('#modal-window-inspecciones').one('shown.bs.modal', function () {
    ajustarScrollBars();
    hideLoading();
});

function init() {
    ///////////////////// Scrollbars ////////////////////////
    $('#body-content', '.inspecciones-body').niceScroll(getNiceScrollConfig());
    $('#modal-window-inspecciones .panel-body').resize(ajustarScrollBars);
    $('#scroll-content-inspecciones .panel-heading a').click(function () {
        setTimeout(ajustarScrollBars, 10);
    });
    $('#headingResultados').click(function () {
        setTimeout(function () {
            columnsAdjust('documentos-inspecciones');
        }, 10);
    });

    ajustarmodal();

    options = {
        events_source: BASE_URL + 'ObrasParticulares/Inspecciones?tipoInspeccion=' + $("#SelectedTipoInspeccion").val() +
            '&Inspector=' + $("#SelectedInspector").val(),
        view: 'month',
        tmpl_path: BASE_URL + 'Scripts/bootstrap-calendar/tmpls/',
        tmpl_cache: false,
        day: moment(new Date()).format('YYYY-MM-DD'),
        first_day: 2,
        weekbox: false,
        onAfterEventsLoad: function (events) {
            if (!events) return;

            var list = $('#eventlist');
            list.html('');

            $.each(events, function (_, val) {
                $(document.createElement('li'))
                    .html('<a href="' + val.url + '">' + val.title + '</a>')
                    .appendTo(list);
            });
        },
        onAfterViewLoad: function (view) {
            $('.page-header h3').text(this.getTitle());
            $('.btn-group button').removeClass('active');
            $('button[data-calendar-view="' + view + '"]').addClass('active');
        },
        classes: {
            months: {
                general: 'label'
            }
        },
        language: "es-ES"
    };

    $("#btnCerrar").click(function () {
        switch ($("#idPanel").val()) {
            case "1":
                $("#modal-window-inspecciones").modal('hide');
                break;
            case "2":
            case "3":
                var valido = true;
                if (Number($("#InspeccionId").val()) === 0) {
                    if ($("#SelectedTipoInspeccion").val() && $("#SelectedTipoInspeccion").val() !== "0") {
                        valido = false;
                        console.debug('Invalido 1');
                    }
                    if ($("#SelectedInspector").val() && $("#SelectedInspector").val() !== "0") {
                        valido = false;
                        console.debug('Invalido 2');
                    }
                    if ($("#selectedUT").val()) {
                        valido = false;
                        console.debug('Invalido 3');
                    }
                    if ($("#ResultadoInspeccion").val()) {
                        valido = false;
                        console.debug('Invalido 4');
                    }
                    if ($("#SelectedObjeto").val() && $("#SelectedObjeto").val() !== "0") {
                        valido = false;
                        console.debug('Invalido 5');
                    }
                    if ($("#SelectedTipo").val() && $("#SelectedTipo").val() !== "0") {
                        valido = false;
                        console.debug('Invalido 6');
                    }
                    if ($("#Identificador").val()) {
                        valido = false;
                        console.debug('Invalido 7');
                    }
                    if ($("#FechaHoraDeInspeccion").val()) {
                        valido = false;
                        console.debug('Invalido 8');
                    }
                    if ($("#SelectedEstado").val() && $("#SelectedEstado").val() !== "1") {
                        valido = false;
                        console.debug('Invalido 9');
                    }
                    if ($("#ResultadoInspeccion").val()) {
                        valido = false;
                        console.debug('Invalido 10');
                    }
                    if ($("#Descripcion").val()) {
                        valido = false;
                        console.debug('Invalido 11');
                    }
                } else {
                    if ($("#InspeccionId").val() !== currEditObject.InspeccionID) {
                        valido = false;
                        console.debug('Invalido A 1');
                    }
                    if ($("#SelectedTipoInspeccion").val() !== currEditObject.TipoInspeccionID) {
                        valido = false;
                        console.debug('Invalido A 2');
                    }
                    if ($("#SelectedInspector").val() !== currEditObject.InspectorID) {
                        valido = false;
                        console.debug('Invalido A 3');
                    }
                    if ($("#FechaHoraDesde").val() !== convertDate(new Date(currEditObject.FechaHoraInicio.match(/\d+/)[0] * 1))) {
                        valido = false;
                        console.debug('Invalido A 4');
                    }
                    if ($("#FechaHoraHasta").val() !== convertDate(new Date(currEditObject.FechaHoraFin.match(/\d+/)[0] * 1))) {
                        valido = false;
                        console.debug('Invalido A 5');
                    }
                    if ($("#FechaOrigenDesde").val() !== convertDate(new Date(currEditObject.FechaHoraInicio.match(/\d+/)[0] * 1))) {
                        valido = false;
                        console.debug('Invalido A 6');
                    }
                    if ($("#FechaOrigenHasta").val() !== convertDate(new Date(currEditObject.FechaHoraFin.match(/\d+/)[0] * 1))) {
                        valido = false;
                        console.debug('Invalido A 7');
                    }

                    if ($("#Descripcion").val() !== currEditObject.Descripcion) {
                        valido = false;
                        console.debug('Invalido A 8');
                    }
                    if (currEditObject.Objeto) {
                        if (
                            (parseInt($("#SelectedObjeto").val()) != currEditObject.Objeto)
                            ||
                            (parseInt($("#SelectedObjeto").val()) != currEditObject.Objeto && parseInt($("#SelectedObjeto").val()) !== 0)
                        ) {
                            valido = false;
                            console.debug('Invalido A 9');
                        }
                    }
                    if (currEditObject.Tipo) {
                        if (
                            parseInt($("#SelectedTipo").val()) != currEditObject.Tipo
                            ||
                            parseInt($("#SelectedTipo").val()) != currEditObject.Tipo && parseInt($("#SelectedTipo").val()) !== 0
                        ) {
                            valido = false;
                            console.debug('Invalido A 11');
                        }
                    }
                    if (currEditObject.Identificador) {
                        if (
                            ($("#Identificador").val() != currEditObject.Identificador)
                            ||
                            ($("#Identificador").val() != currEditObject.Identificador && parseInt($("#Identificador").val()) !== 0)
                        ) {
                            valido = false;
                            console.debug('Invalido A 13');
                        }
                    }
                    if (!currEditObject.FechaHoraDeInspeccion) {
                        if ($("#FechaHoraDeInspeccion").val()) {
                            valido = false;
                            console.debug('Invalido A 15');
                        }
                        else {
                            if (
                                !$("#FechaHoraDeInspeccion").val()
                                ||
                                $("#FechaHoraDeInspeccion").val() != convertDate(new Date(currEditObject.FechaHoraDeInspeccion.match(/\d+/)[0] * 1))) {
                                valido = false;
                                console.debug('Invalido A 16');
                            }
                        }
                    }

                    if (parseInt($("#SelectedEstado").val()) != currEditObject.SelectedEstado) {
                        valido = false;
                        console.debug('Invalido A 17');
                    }
                    if (parseInt($("#SelectedEstadoHidden").val()) != currEditObject.SelectedEstado) {
                        valido = false;
                        console.debug('Invalido A 18');
                    }
                    if (
                        !$("#ResultadoInspeccion").val() && currEditObject.ResultadoInspeccion
                        ||
                        currEditObject.ResultadoInspeccion && $("#ResultadoInspeccion").val() != currEditObject.ResultadoInspeccion) {
                        valido = false;
                        console.debug('Invalido A 19');
                    }

                    if (currEditObject.InspeccionUnidadeTributarias) {
                        if ($("#selectedUT").val() !== currEditObject.InspeccionUnidadeTributarias.map(function (item) { return item.UnidadTributariaId; }).join(",")) {
                            valido = false;
                            console.debug('Invalido A 20');
                        }
                    }
                }

                if (!valido) {
                    alerta('Advertencia', 'Se perderán los cambios realizados ¿Continuar?', 2, function () {
                        if ($("#alert_message_btnSi_result").val() === "1") {
                            EnablePanel(1);
                        }
                    });
                } else {
                    EnablePanel(1);
                }
                break;
        }
    });

    EnablePanel(1);

    $("#FechaHoraDesde").change(function (e) {
        ChangeFechaDesde();
        e.preventDefault();
    });

    $("#FechaHoraHasta").change(function (e) {
        ChangeFechaHasta();
        e.preventDefault();
    });

    $("#btnAceptarAlert").click(function (e) {
        $("#alert_message_btnSi_result").val(1);
        $('#ModalInfoInspecccion').hide();
        if (fnResultAlerta) fnResultAlerta();
        e.preventDefault();
    });

    $("#btnCancelarAlert").click(function (e) {
        $("#alert_message_btnNo_result").val(1);
        $('#ModalInfoInspecccion').hide();
        if (fnResultAlerta) fnResultAlerta();
        e.preventDefault();
    });

    /* Date picker*/
    $('.fechaCalendarioValue').val(GetNameFromMonthNumber(new Date().getMonth()) + ' ' + new Date().getFullYear());

    $('.fechaCalendario').datepicker(getDatePickerConfig({ format: "MM yyyy", minViewMode: 'months', maxViewMode: 'months' }));

    $('.fechaCalendarioValue').keypress(function (event) {
        if (event.keyCode === 13) {
            const splittedCal = $('.fechaCalendarioValue').val().split(" ");
            options.day = `${splittedCal[1]}-${GetMonthNumberFromName(splittedCal[0])}-${options.day.split("-")[2]}`;
            $('#calendar').calendar(options);
            event.preventDefault();
        }
    });

    $('.fechaCalendario').datepicker().on('changeDate', function () {
        const splittedCal = $('.fechaCalendarioValue').val().split(" ");
        const month = splittedCal[0];

        $('.fechaCalendarioValue').val(`${month} ${splittedCal[1]}`);
        options.day = `${splittedCal[1]}-${GetMonthNumberFromName(month)}-${options.day.split("-")[2]}`;
        $('#calendar').calendar(options);
    });

    $('#calendar').calendar(options);

    $("#btnNavCalendarPrev").click(function (e) {
        MoveMonth(-1, e);
    });

    $("#btnNavCalendarNext").click(function (e) {
        MoveMonth(+1, e);

    });

    $(".cal-cell .cal-month-day .pull-right").click(function () {
        $(".tooltip-inner").remove();
    });

    $("#SelectedTipoInspeccionCal").change(function (e) {

        var selectedInspector = $("#SelectedInspectorCal").val();

        $.ajax({
            url: BASE_URL + 'ObrasParticulares/GetInspectoresPorTipoInspeccion/?tipoInspeccion=' + $("#SelectedTipoInspeccionCal").val() + '&incluyeBaja=true',
            dataType: 'json',
            type: 'POST',
            success: function (data) {
                if (data) {
                    $("#SelectedInspectorCal").empty();
                    $("#SelectedInspectorCal").append('<option value="0">Seleccione</option>');

                    var selected = "";
                    data.forEach(function (Inspector) {
                        var s = "";
                        if (!selected) {
                            selected = Inspector.InspectorID == selectedInspector ? "selected" : "";
                            s = selected;

                        }


                        $("#SelectedInspectorCal").append('<option value="' + Inspector.InspectorID + '" ' + s + '>' + Inspector.Usuario.Apellido + ' ' + Inspector.Usuario.Nombre + '</option>');
                    });

                    if (!selected)
                        $("#SelectedInspectorCal").val(0);
                    options.events_source = BASE_URL + 'ObrasParticulares/Inspecciones/?tipoInspeccion=' + $("#SelectedTipoInspeccionCal").val() + '&Inspector=' + $("#SelectedInspectorCal").val();
                    var calendar = $('#calendar').calendar(options);
                }



            }, error: function (ex) {
                $("#SelectedTipoInspeccionCal").empty();
                $("#SelectedInspectorCal").empty();
            }


        });


    });


    $("#SelectedInspectorCal").change(function (e) {

        var selectedTipoInspeccion = $("#SelectedTipoInspeccionCal").val();

        $.ajax({
            url: BASE_URL + 'ObrasParticulares/GetIiposInspeccionesPorInspector/?idInspector=' + $("#SelectedInspectorCal").val() + '&limitarTipos=true',
            dataType: 'json',
            type: 'POST',
            success: function (data) {
                if (data) {
                    $("#SelectedTipoInspeccionCal").empty();
                    $("#SelectedTipoInspeccionCal").append('<option value="0">Seleccione</option>');
                    var selected = "";
                    data.forEach(function (TipoInspeccion) {
                        var s = "";
                        if (!selected) {
                            selected = TipoInspeccion.TipoInspeccionID == selectedTipoInspeccion ? "selected" : "";
                            s = selected;
                        }

                        $("#SelectedTipoInspeccionCal").append('<option value="' + TipoInspeccion.TipoInspeccionID + '" ' + s + '>' + TipoInspeccion.Descripcion + '</option>');
                    });

                    if (!selected)
                        $("#SelectedTipoInspeccionCal").val(0);

                    clearErrores();
                    options.events_source = BASE_URL + 'ObrasParticulares/Inspecciones/?tipoInspeccion=' + $("#SelectedTipoInspeccionCal").val() + '&Inspector=' + $("#SelectedInspectorCal").val();
                    var calendar = $('#calendar').calendar(options);

                    CheckInspectorHabilitado($("#SelectedInspectorCal").val(), function () {
                        $("#SelectedInspector").val($("#SelectedInspectorCal").val());
                    });
                }

            }, error: function (ex) {
                $("#SelectedTipoInspeccionCal").empty();
                $("#SelectedInspectorCal").empty();
                alert(JSON.stringify(ex));
            }
        });
    });



    $("#SelectedTipoInspeccion").change(function (e) {

        var selectedInspector = $("#SelectedInspector").val();


        $.ajax({
            url: BASE_URL + 'ObrasParticulares/GetInspectoresPorTipoInspeccion/?tipoInspeccion=' + $("#SelectedTipoInspeccion").val() + '&incluyeBaja=false',
            dataType: 'json',
            type: 'POST',
            success: function (data) {

                if (data) {
                    $("#SelectedInspector").empty();
                    $("#SelectedInspector").append('<option value="0">Seleccione</option>');
                    var selected = "";
                    data.forEach(function (Inspector) {
                        var s = "";
                        if (!selected) {
                            selected = Inspector.InspectorID == selectedInspector ? "selected" : "";
                            s = selected;
                        }
                        if ($("#SelectedTipoInspeccion").val() == '0') {
                            $("#SelectedInspector").empty();
                            $("#SelectedInspector").append('<option value="0">Seleccione</option>');
                            $("#SelectedInspector").prop("disabled", true);
                        }
                        else {
                            $("#SelectedInspector").append('<option value="' + Inspector.InspectorID + '" ' + s + '>' + Inspector.Usuario.Apellido + ' ' + Inspector.Usuario.Nombre + '</option>');
                            $("#SelectedInspector").prop("disabled", false);
                        }

                    });
                    if (!selected)
                        $("#SelectedInspector").val(0);

                }

            }, error: function (ex) {
                $("#SelectedTipoInspeccion").empty();
                $("#SelectedInspector").empty();
                alert(JSON.stringify(ex));
            }

        });



    });


    $("#SelectedInspector").change(function (e) {

        var selectedTipoInspeccion = $("#SelectedTipoInspeccion").val();

        $.ajax({
            url: BASE_URL + 'ObrasParticulares/GetIiposInspeccionesPorInspector/?idInspector=' + $("#SelectedInspector").val() + '&limitarTipos=true',
            dataType: 'json',
            type: 'POST',
            success: function (data) {

                if (data) {
                    $("#SelectedTipoInspeccion").empty();
                    $("#SelectedTipoInspeccion").append('<option value="0">Seleccione</option>');
                    var selected = "";
                    data.forEach(function (TipoInspeccion) {
                        var s = "";
                        if (!selected) {
                            selected = TipoInspeccion.TipoInspeccionID == selectedTipoInspeccion ? "selected" : "";
                            s = selected;
                        }

                        $("#SelectedTipoInspeccion").append('<option value="' + TipoInspeccion.TipoInspeccionID + '" ' + s + '>' + TipoInspeccion.Descripcion + '</option>');

                    });
                    if (!selected) {
                        $("#SelectedTipoInspeccion").val(0);
                    }

                    clearErrores();

                    CheckInspectorHabilitado($("#SelectedInspector").val());
                }
            }, error: function (ex) {
                $("#SelectedTipoInspeccion").empty();
                $("#SelectedInspector").empty();
                alert(JSON.stringify(ex));
            }
        });
    });


    $("#SelectedObjeto").change(function () {
        changeSelectedObjeto();
    });

    Grilla_UnidadesTributarias = $('#Grilla_UnidadesTributarias').DataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "visible": false,
        }]
    });

    Grilla_Documentos = $('#documentos-inspecciones').DataTable({
        "scrollY": "100px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[0, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "visible": false,
        }]
    });


    $("#addPlanificacion").click(function () {
        VaciarDatos();
        var hoy = new Date();
        var hoyString = moment(hoy).format('DD/MM/YYYY HH:mm');
        $("#FechaHoraDesde").val(hoyString);
        $("#FechaOrigenDesde").val(hoyString);

        hoyString = moment(hoy).add(1, 'hour').format('DD/MM/YYYY HH:mm');
        $("#FechaHoraHasta").val(hoyString);
        $("#FechaOrigenHasta").val(hoyString);

        $("#SelectedEstado").val("1");
        $("#SelectedEstado").attr("disabled", "disabled");

        $("#FechaHoraDesde").removeAttr('disabled');
        $("#FechaHoraHasta").removeAttr('disabled');
        $(".fechaCompleta img").show();

        EnablePanel(2);
        SetEnabledInspeccion(true);
    });

    $("#btnEditar").click(function () {
        if ($("#puedeProgramar").val() !== 'false') {
            SetEnabledInspeccion(true);
        }
    });

    $("#btnBorrar").click(function () {
        if ($("#puedeProgramar").val() !== 'false' && $("#SelectedEstado").val() === '1') {
            alerta('Advertencia', 'Se dar&aacute; de baja la inspecci&oacute;n ¿Desea continuar?', 2, function () {
                if ($("#alert_message_btnSi_result").val() === "1") {
                    $.get(BASE_URL + 'ObrasParticulares/GetInspeccionBaja', "inspeccionID=" + $("#InspeccionId").val(),
                        function (data) {
                            if (data.Response === "OK") {
                                EnablePanel(1);
                                var calendar = $('#calendar').calendar(options);
                            }
                        });
                }
            });
        } else if ($("#SelectedEstado").val() !== '1') {
            alerta('Advertencia', 'No se puede dar de baja la inspecci&oacute;n debido a que no posee el estado "Planificada"', 2);
        }
    });

    $("#btnGrabar").click(function () {
        if (!_enabled && !$("input[name='aeo']").length) {
            return;
        } else if (!_enabled && typeof inspeccionGuardada === 'function') {
            inspeccionGuardada($("#InspeccionId").val());
            $("#modal-window-inspecciones").modal('hide');
            return;
        }
        $("#btnGrabar").addClass("boton-deshabilitado");
        var valido = true;
        if ($("#editionMode").val() === "false") {
            valido = false;
            $("#btnGrabar").removeClass("boton-deshabilitado");
            return;
        }
        if (!$("#SelectedTipoInspeccion").val() || $("#SelectedTipoInspeccion").val() === "0") {
            $("#SelectedTipoInspeccion_required").removeClass('oculto');
            valido = false;
        } else {
            $("#SelectedTipoInspeccion_required").addClass('oculto');
        }
        if (!$("#FechaHoraDesde").val()) {
            $("#FechaHoraDesde_required").removeClass('oculto');
            valido = false;
        } else {
            $("#FechaHoraDesde_required").addClass('oculto');
        }
        if (!$("#FechaHoraHasta").val()) {
            $("#FechaHoraHasta_required").removeClass('oculto');
            valido = false;
        } else {
            $("#FechaHoraHasta_required").addClass('oculto');
        }
        if (!$("#SelectedInspector").val() || $("#SelectedInspector").val() === "0") {
            $("#SelectedInspector_required").removeClass('oculto');
            valido = false;
        } else {
            $("#SelectedInspector_required").addClass('oculto');
        }
        if (!$("#selectedUT").val() || $("#selectedUT").val() === "0") {
            $("#UnidadesTributarias_required").removeClass('oculto');
            valido = false;
        } else {
            $("#UnidadesTributarias_required").addClass('oculto');
        }

        //if ($("#SelectedObjeto").val() !== "0") {
        //    if ($("#SelectedTipo").val() === "0") {
        //        $("#SelectedTipo_required").removeClass('oculto');
        //        valido = false;
        //    } else {
        //        if (!$("#Identificador").val()) {
        //            $("#Identificador_required").removeClass('oculto');
        //            valido = false;
        //        } else {
        //            $("#Identificador_required").addClass('oculto');
        //        }
        //    }
        //}
        if (valido) {
            var id = parseInt($("#InspeccionId").val());
            var desde = new Date(convertDateFromText($("#FechaHoraDesde").val()));
            var hasta = new Date(convertDateFromText($("#FechaHoraHasta").val()));

            var desdeorigen = new Date(convertDateFromText($("#FechaOrigenDesde").val()));

            if (!$("#FechaHoraDeInspeccion").val())
                finalizacion = new Date(convertDateFromText($("#FechaHoraDeInspeccion").val()));

            var replanificada = new Date(convertDateFromText($("#fechaHoraPrevia").val())) !== desde;

            if (desde > hasta) {
                valido = false;
                alerta('Error', 'La fecha desde no puede ser mayor a la fecha hasta', 3);
            } else if (desde < new Date() && hasta < new Date() && id <= 0) { // inspeccion no programada
                alerta('Advertencia', 'Se registrará una inspección no programada ¿Continuar?', 2, function () {
                    if ($("#alert_message_btnSi_result").val() === "1") {
                        $("#Descripcion").val("");
                        $("#SelectedEstado").val("4");
                        doPostInspeccion(desde);
                    }
                });
            } else if (id > 0 && desdeorigen > new Date() && desde < new Date() && replanificada) {
                alerta('Error', 'Una planificación que tiene fecha futura no puede re programarse con fecha anterior a la fecha del día', 3);
            } else if (id <= 0 && desde < new Date()) {
                alerta('Error', 'No puede planificarse una inspeccion con fecha y hora desde anterior a la actual', 3);
            } else {
                doPostInspeccion(desde);
            }
        }
        else {
            alerta('Error', 'Por favor verifique los datos', 3);
        }
        $("#btnGrabar").removeClass("boton-deshabilitado");
        //var selectedObjeto = $("#SelectedObjeto").val();
        //var selectedTipo = $("#SelectedTipo").val() || 0;
        //var identificador = $("#Identificador").val() || 0;
        //showLoading();
        //$.ajax({
        //    url: BASE_URL + 'ObrasParticulares/GetExisteObjeto/?objeto=' + parseInt(selectedObjeto) + '&tipo=' + parseInt(selectedTipo) + '&identificador=' + identificador,
        //    dataType: 'json',
        //    type: 'GET',
        //    success: function (data) {
        //        if (!isNaN(parseInt(data))) {
        //            if (data < 0 && ($("#SelectedObjeto").val() !== "0" || $("#SelectedTipo").val() !== "0" || $("#Identificador").val())) {
        //                alerta('Error', 'La relación seleccionada no existe', 2);
        //            } else {
        //                if (valido) {
        //                    var id = parseInt($("#InspeccionId").val());
        //                    var desde = new Date(convertDateFromText($("#FechaHoraDesde").val()));
        //                    var hasta = new Date(convertDateFromText($("#FechaHoraHasta").val()));

        //                    var desdeorigen = new Date(convertDateFromText($("#FechaOrigenDesde").val()));

        //                    if (!$("#FechaHoraDeInspeccion").val())
        //                        finalizacion = new Date(convertDateFromText($("#FechaHoraDeInspeccion").val()));

        //                    var replanificada = new Date(convertDateFromText($("#fechaHoraPrevia").val())) !== desde;

        //                    if (desde > hasta) {
        //                        valido = false;
        //                        alerta('Error', 'La fecha desde no puede ser mayor a la fecha hasta', 3);
        //                    } else if (desde < new Date() && hasta < new Date() && id <= 0) { // inspeccion no programada
        //                        alerta('Advertencia', 'Se registrará una inspección no programada ¿Continuar?', 2, function () {
        //                            if ($("#alert_message_btnSi_result").val() === "1") {
        //                                $("#Descripcion").val("");
        //                                $("#SelectedEstado").val("4");
        //                                doPostInspeccion(desde);
        //                            }
        //                        });
        //                    } else if (id > 0 && desdeorigen > new Date() && desde < new Date() && replanificada) {
        //                        alerta('Error', 'Una planificación que tiene fecha futura no puede re programarse con fecha anterior a la fecha del día', 3);
        //                    } else if (id <= 0 && desde < new Date()) {
        //                        alerta('Error', 'No puede planificarse una inspeccion con fecha y hora desde anterior a la actual', 3);
        //                    } else {
        //                        doPostInspeccion(desde);
        //                    }
        //                }
        //                else {
        //                    alerta('Error', 'Por favor verifique los datos', 3);
        //                }
        //            }
        //        }
        //        $("#btnGrabar").removeClass("boton-deshabilitado");
        //    },
        //    complete: hideLoading
        //});
    });

    $("#btnAddUT").click(function () {
        if (_enabled) {
            new Promise(function (resolve) {
                var data = {
                    tipos: BuscadorTipos.UnidadesTributarias,
                    multiSelect: true,
                    verAgregar: false,
                    titulo: 'Buscar Unidades Tributarias',
                    campos: ["Partida"],
                    includeSearch: false
                };
                if ($("#selectedUT").val()) {
                    data.seleccionActual = Grilla_UnidadesTributarias.data().toArray().map(function (elem) { return [BuscadorTipos.UnidadesTributarias, elem[1], elem[0]]; });
                }
                $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
                    $(".modal", this).one('hidden.bs.modal', function () {
                        $(window).off('seleccionAceptada');
                    });
                    $(window).one("seleccionAceptada", function (evt) {
                        if (evt.seleccion) {
                            resolve(evt.seleccion.map(function (elem) { return elem.slice(1); }));
                        } else {
                            resolve([]);
                        }
                    });
                });
            }).then(function (seleccion) {
                if (seleccion.length) {
                    $("#selectedUT").val(seleccion.map(function (data) { return data[1]; }).join(','));
                    LoadUnidadesTributarias(seleccion);
                }
            }).catch(function (err) { console.log(err); });
        }
    });

    $("#btnRemUT").click(function () {
        if (!_enabled) return;
        var deleted = Grilla_UnidadesTributarias.rows(".selected");
        if (deleted.length) {
            alerta('Advertencia', 'Se quitará la relación con las Unidades Tributarias Seleccionadas. ¿Continuar?', 2, function () {
                if ($("#alert_message_btnSi_result").val() === "1") {
                    deleted.remove().draw();
                    $("#selectedUT").val(Grilla_UnidadesTributarias.data().toArray().map(function (r) { return r[0]; }).join(","));
                    $("#btnRemUT").addClass('boton-deshabilitado');
                }
            });
        }
    });

    $('#btnRemDoc').on('click', function () {
        if (!_enabled) return;
        var deleted = Grilla_Documentos.rows(".selected");
        if (deleted.length) {
            alerta('Advertencia', 'Se quitará la relación con el documento seleccionado. ¿Continuar?', 2, function () {
                if ($("#alert_message_btnSi_result").val() === "1") {
                    deleted.remove().draw();
                    $("#selectedDocs").val(Grilla_Documentos.data().toArray().map(function (r) { return r[0]; }).join(","));
                    $("#btnRemDoc").addClass('boton-deshabilitado');
                    $("#btnEditDoc").addClass('boton-deshabilitado');
                }
            });
        }
    });

    $('#btnEditDoc').on('click', function () {
        if (!_enabled) return;
        var obj = $('#documentos-inspecciones').dataTable().api().row('.selected').data();
        loadDocumentos(obj[0]);
    });

    $('#btnViewDoc').on('click', function () {
        var obj = $('#documentos-inspecciones').dataTable().api().row('.selected').data();
        $.ajax({
            url: BASE_URL + "Documento/Visualizar",
            type: "POST",
            async: false,
            success: function (data) {
            }
        });
        loadDocumentos(obj[0]);
    });

    $('#btnAddDoc').on('click', function () {
        if (_enabled) {
            $('#documentos-inspecciones tbody tr').removeClass('selected');
            showLoading();
            loadDocumentos();
        }
    });

    ///////////////////// Tooltips /////////////////////////
    $('#gestionInspeccionesModal .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////

    ///////////////////// Agregar option NO PLANIFICADO /////////////////////////
    $("#SelectedEstado").append($("<option />").val("4").html("No planificado"));
    $("#SelectedEstado option[value= 4 ]").css("display", "none");
    ////////////////////////////////////////////////////////
    $("#modal-window-inspecciones").modal('show');
}

function loadDocumentos(id) {
    $("#documentos-externo").load(BASE_URL + "Documento/DatosDocumento?id=" + (id || 0), function () {
        $(document).one("documentoGuardado", function (evt) {
            documentoGuardado(evt.documento);
        });
    });
}

function documentoGuardado(data) {
    var selected = $('#documentos-inspecciones').dataTable().api().row('.selected').data();
    if (!selected) {
        var doc = {
            "0": data.id_documento,
            "1": data.Tipo.Descripcion,
            "2": data.nombre_archivo,
            "3": (new Date(data.fecha_alta_1)).toLocaleDateString(),
            "4": data.descripcion
        };
        $('#documentos-inspecciones').dataTable().api().row.add(doc).draw();

        //Agrego la relacion
        var arr = $("#selectedDocs").val() ? $("#selectedDocs").val().split(",") : [];
        $("#selectedDocs").val(arr.concat(data.id_documento).join(","));
    } else {
        selected.Tipo = data.id_tipo_documento;
        selected.Nombre = data.nombre_archivo;
        selected.FechaAlta = data.fecha_alta_1;
        selected.Descripcion = data.descripcion;
        $('#documentos-inspecciones').dataTable().api().row('.selected').data(selected).draw();
    }
    setEventGrillaDocumentos();
    $("#documentos-externo").html('');
}

function ajustarmodal() {
    var altura = $(window).height() - 190; //value corresponding to the modal heading + footer
    $(".inspecciones-body", "#modal-window-inspecciones").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ajustarScrollBars() {
    $('#body-content', ".inspecciones-body").css({ "max-height": $(".inspecciones-body").height() + 'px', "height": "100%" });
    $("#body-content", ".inspecciones-body").getNiceScroll().resize();
}

function editionMode(idPanel) {
    $("#idPanel").val(idPanel);
    if (parseInt(idPanel) === 1 || $("#puedeProgramar").val() === 'false') {
        $("#editionMode").val("false");
        $("#btnGrabar").addClass("boton-deshabilitado");
    } else {
        $("#editionMode").val("true");
    }
}

function LoadUnidadesTributarias(uts) {
    $("#btnRemUT").addClass('boton-deshabilitado');
    Grilla_UnidadesTributarias.clear().draw();
    if (uts) {
        uts.forEach(function (ut) {
            Grilla_UnidadesTributarias.row.add({
                "0": ut[1],
                "1": ut[0]
            });
        });
        Grilla_UnidadesTributarias.draw();
    }
    setEventGrillaUnidadesTributarias();
}

function setEventGrillaUnidadesTributarias() {
    $('#Grilla_UnidadesTributarias tbody').off('click', 'tr');
    $('#Grilla_UnidadesTributarias tbody').on('click', 'tr', function () {
        if (_enabled && $("#SelectedEstado").val() !== "4") {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
                $("#btnRemUT").addClass('boton-deshabilitado');
            } else {
                Grilla_UnidadesTributarias.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
                selectedUT = Grilla_UnidadesTributarias.row(this).data()[0];
                $("#btnRemUT").removeClass('boton-deshabilitado');
            }
        }
    });
}

function setEventGrillaDocumentos() {
    $('#documentos-inspecciones tbody').off('click', 'tr');
    $('#documentos-inspecciones tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
            selectedDoc = null;
            $("#btnViewDoc").addClass('boton-deshabilitado');
            if (_enabled) {
                $("#btnRemDoc").addClass('boton-deshabilitado');
                $("#btnEditDoc").addClass('boton-deshabilitado');
            }
        } else {
            Grilla_Documentos.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
            var d = Grilla_Documentos.row(this).data();
            if (_enabled) {
                selectedDoc = d[0];
                $("#btnRemDoc").removeClass('boton-deshabilitado');
                $("#btnEditDoc").removeClass('boton-deshabilitado');
            }
            if (d) {
                $("#btnViewDoc").removeClass('boton-deshabilitado');
            }
        }
    });
}

function CheckInspectorHabilitado(currentId, f) {
    $.ajax({
        url: BASE_URL + 'ObrasParticulares/PuedeProgramar',
        data: { Response: currentId },
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            $("#puedeProgramar").val(data.Response);
            editionMode($("#idPanel").val());
            if (f) f();
        }, error: function (ex) {
            alert(ex);
        }
    });
}

// Convierte la fecha recibida en un String con formato
// DIA(CON 0)/MES(CON 0)/AÑO HORA(CON 0)/MINUTOS(CON 0)
function convertDate(fecha) {
    var dd = fecha.getDate();
    var mm = fecha.getMonth() + 1; //January is 0!
    var hh = fecha.getHours();
    var ii = fecha.getMinutes()

    var yyyy = fecha.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    if (hh < 10) {
        hh = '0' + hh;
    }
    if (ii < 10) {
        ii = '0' + ii;
    }

    return dd + '/' + mm + '/' + yyyy + ' ' + hh + ':' + ii;
}


function convertDateFromText(fecha) {
    var splitted = fecha.split(' ');
    var f = splitted[0].split('/');
    return f[2] + '-' + (f[1]) + '-' + f[0] + ' ' + splitted[1] + ':00';
}

// ACA HAY QUILOMBO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
function showEvent(idInspeccion) {
    $.ajax({
        url: BASE_URL + 'ObrasParticulares/GetInspeccion/?inspeccion=' + idInspeccion,
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            if (data) {
                VaciarDatos();
                $("#SelectedEstado").empty();
                data.EstadosInspeccion.forEach(function (item) {
                    $("#SelectedEstado").append($('<OPTION>', {
                        value: item.EstadoInspeccionID,
                        text: item.Descripcion
                    }));
                });

                currEditObject = data;
                $("#InspeccionId").val(data.InspeccionID);
                $("#SelectedTipoInspeccion").val(data.TipoInspeccionID);
                $("#SelectedInspector").val(data.InspectorID);

                var fechaHoraDesde = convertDate(new Date(data.FechaHoraInicio.match(/\d+/)[0] * 1));
                $("#fechaHoraPrevia").val(fechaHoraDesde);
                $("#FechaHoraDesde").val(fechaHoraDesde);

                var fechaHoraHasta = convertDate(new Date(data.FechaHoraFin.match(/\d+/)[0] * 1));
                $("#FechaHoraHasta").val(fechaHoraHasta);
                $(".fechaCompleta img").hide();

                var fechaOrigenDesde = convertDate(new Date(data.FechaHoraInicioOriginal.match(/\d+/)[0] * 1));
                $("#FechaOrigenDesde").val(fechaOrigenDesde);

                var fechaOrigenHasta = convertDate(new Date(data.FechaHoraFinOriginal.match(/\d+/)[0] * 1));
                $("#FechaOrigenHasta").val(fechaOrigenHasta);

                $("#Descripcion").val(data.Descripcion);
                if (data.Objeto)
                    $("#SelectedObjeto").val(data.Objeto);
                else
                    $("#SelectedObjeto").val(0);

                changeSelectedObjeto();
                if (data.Tipo)
                    $("#SelectedTipo").val(data.Tipo);
                else
                    $("#SelectedTipo").val(0);

                if (data.Identificador)
                    $("#Identificador").val(data.Identificador);
                else
                    $("#Identificador").val(0);
                if (!data.FechaHoraDeInspeccion)
                    $("#FechaHoraDeInspeccion").val("");
                else {
                    var fechaHoraDeInspeccion = convertDate(new Date(data.FechaHoraDeInspeccion.match(/\d+/)[0] * 1));
                    $("#FechaHoraDeInspeccion").val(fechaHoraDeInspeccion);
                }

                $("#SelectedEstado").val(data.SelectedEstado);
                $("#SelectedEstadoHidden").val(data.SelectedEstado);
                $("#ResultadoInspeccion").val(data.ResultadoInspeccion);

                var arrUT = $("#selectedUT").val() ? $("#selectedUT").val().split(",") : [];
                if (data.InspeccionUnidadeTributarias) {
                    arrUT = arrUT.concat(data.InspeccionUnidadeTributarias.map(function (item) {
                        return [item.UnidadTributaria.CodigoProvincial, item.UnidadTributariaId];
                    }));
                    $("#selectedUT").val(arrUT.map(elem => elem[1]).join(","));
                }
                LoadUnidadesTributarias(arrUT);

                if (data.InspeccionDocumento) {
                    Grilla_Documentos.clear().draw();
                    $("#selectedDocs").val("");
                    $("#btnRemDoc").addClass('boton-deshabilitado');
                    $("#btnEditDoc").addClass('boton-deshabilitado');
                    $("#btnViewDoc").addClass('boton-deshabilitado');

                    var arrDoc = [];
                    data.InspeccionDocumento.forEach(function (doc) {
                        arrDoc.push(doc.id_documento);
                        var row = {
                            "0": doc.id_documento,
                            "1": doc.documento.Tipo.Descripcion,
                            "2": doc.documento.nombre_archivo,
                            "3": convertDate(new Date(doc.documento.fecha_alta_1.match(/\d+/)[0] * 1)),
                            "4": doc.documento.descripcion
                        };
                        Grilla_Documentos.row.add(row);
                    });
                    $("#selectedDocs").val(arrDoc.join(","));
                    Grilla_Documentos.draw();
                    setEventGrillaDocumentos();
                }

                EnablePanel(2);
            }
        }, error: function (ex) {
            alert(JSON.stringify(ex));
        }
    });
    SetEnabledInspeccion(false);
}

function checkEnabled() {
    $("#FechaHoraDeInspeccion").removeAttr('disabled');
    $("#SelectedEstado").removeAttr('disabled');
    $("#ResultadoInspeccion").removeAttr('disabled');

    switch ($("#SelectedEstado").val()) {
        case "1":
            if ($("#Planificador").val() === 'S') {
                $("#FechaHoraDeInspeccion").removeAttr('disabled', 'disabled');
                $("#SelectedEstado").removeAttr('disabled', 'disabled');
                $("#ResultadoInspeccion").removeAttr('disabled', 'disabled');
            }
            break;
        case "3":
            if ($("#Planificador").val() === 'S') {
                $("#FechaHoraDeInspeccion").attr('disabled', 'disabled');
                $("#SelectedEstado").attr('disabled', 'disabled');
                $("#ResultadoInspeccion").attr('disabled', 'disabled');
            }
            break;
    }
}

function MoveMonth(param, e) {
    var splitted = options.day.split("-");
    var currDate = new Date();
    currDate.setFullYear(parseInt(splitted[0]), parseInt(splitted[1]) - 1, parseInt(splitted[2]));
    currDate.setMonth(currDate.getMonth() + param);
    options.day = moment(currDate).format('YYYY-MM-DD');

    $('.fechaCalendarioValue').val(GetNameFromMonthNumber(currDate.getMonth()) + ' ' + currDate.getFullYear());
    $('.fechaCalendario').datepicker('update');
    $('#calendar').calendar(options);
    e.preventDefault();
}

function EnablePanel(idPanel) {
    switch (idPanel) {
        case 1:
            $("#Panel_Botones span[aria-controls='button']").removeClass("black");
            $("#Panel_Botones span[aria-controls='button']").addClass("boton-deshabilitado");
            $("#headingPlanificacion").removeClass("panel-deshabilitado");
            $("#collapsePlanificacion").removeClass("in").addClass("in").css('height', '');
            $("#headingDatos").addClass("panel-deshabilitado");
            $("#collapseDatos").removeClass("in").css('heigth', '0px');
            $("#headingResultados").addClass("panel-deshabilitado");
            $("#collapseResultados").removeClass("in").css('heigth', '0px');
            editionMode(idPanel);
            break;
        case 2:
        case 3:
            $("#Panel_Botones span[aria-controls='button']").addClass("black");
            $("#Panel_Botones span[aria-controls='button']").removeClass("boton-deshabilitado");
            $("#headingPlanificacion").addClass("panel-deshabilitado");
            $("#collapsePlanificacion").removeClass("in").css('heigth', '0px');
            $("#headingDatos").removeClass("panel-deshabilitado");
            $("#collapseDatos").removeClass("in").addClass("in").css('height', '');
            $("#headingResultados").removeClass("panel-deshabilitado");
            editionMode(idPanel);
            break;
    }
    setTimeout(function () {
        $(".inspecciones-content").getNiceScroll().show().resize();
        if (Grilla_UnidadesTributarias)
            Grilla_UnidadesTributarias.draw();
        if (Grilla_Documentos)
            Grilla_Documentos.draw();
    }, 700);
}

function clearErrores() {
    $("#SelectedTipoInspeccion_required").addClass('oculto');
    $("#FechaHoraDesde_required").addClass('oculto');
    $("#FechaHoraHasta_required").addClass('oculto');
    $("#SelectedInspector_required").addClass('oculto');
    $("#FechaOrigenDesde_required").addClass('oculto');
    $("#FechaOrigenHasta_required").addClass('oculto');
    $("#UnidadesTributarias_required").addClass('oculto');
    $("#Observaciones_required").addClass('oculto');
    $("#SelectedObjeto_required").addClass('oculto');
    $("#SelectedTipo_required").addClass('oculto');
    $("#Identificador_required").addClass('oculto');
}

function changeSelectedObjeto() {
    $(".tooltip-inner").remove();
    $("#SelectedTipo").empty();
    $("#SelectedTipo").append($('<OPTION>', {
        value: 0,
        text: 'Seleccione'
    }));
    switch ($("#SelectedObjeto").val()) {
        case "1":
            $("#SelectedTipo").append($('<OPTION>', {
                value: 1,
                text: 'Legajo'
            }));
            $("#SelectedTipo").append($('<OPTION>', {
                value: 2,
                text: 'Expediente de Obra'
            }));
            break;
        case "2":
            $.ajax({
                url: BASE_URL + 'Actas/GetActaTipos',
                dataType: 'json',
                type: 'GET',
                success: function (data) {
                    json = JSON.parse(data);
                    for (var i = 0; i < json.length; i++) {
                        var obj = json[i];
                        $("#SelectedTipo").append($('<OPTION>', {
                            value: obj.ActaTipoId,
                            text: obj.Descripcion
                        }));
                    }
                }
            });
            break;
        case "3":
            $.ajax({
                url: BASE_URL + 'TramitesCertificados/GetTiposJson',
                dataType: 'json',
                type: 'GET',
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        var obj = data[i];
                        $("#SelectedTipo").append($('<OPTION>', {
                            value: obj.Id_Tipo_Tramite,
                            text: obj.Nombre
                        }));
                    }
                }
            });
            break;
    }
}

var fnResultAlerta = null;

function alerta(titulo, mensaje, tipo, fn) {
    var cls = "";
    fnResultAlerta = fn;
    $(".modal-footer", "#ModalInfoInspeccion").hide();
    if (typeof fn === "function") {
        $(".modal-footer", "#ModalInfoInspeccion").show();
    }
    switch (tipo) {
        case 1:
            cls = "alert-success";
            break;
        case 2:
            cls = "alert-warning";
            break;
        case 3:
            cls = "alert-danger";
            break;
        case 4:
            cls = "alert-info";
            break;
    }

    $("#alert_message_btnSi_result").val(0);
    $("#alert_message_btnNo_result").val(0);
    $("#MensajeInfoInspecccion").removeClass("alert-info alert-warning alert-danger alert-success").addClass(cls);
    $("#TituloInfoInspecccion").html(titulo);
    $("#DescripcionInfoInspecccion").html(mensaje);
    $("#ModalInfoInspeccion").modal('show');
}

function VaciarDatos() {
    $("#InspeccionId").val(0);
    $("#SelectedTipoInspeccion").val(0);
    $("#Inspector").val(0);
    $("#fechaHoraPrevia").val("");
    $("#FechaHoraDesde").val("");
    $("#FechaHoraHasta").val("");
    $("#Descripcion").val("");
    $("#SelectedInspector").val(0);
    $("#FechaOrigenDesde").val("");
    $("#HoraOrigenDesde").val("");
    $("#FechaOrigenHasta").val("");
    $("#HoraOrigenHasta").val("");
    $("#Descripcion").val("");
    $("#SelectedObjeto").val(0);
    $("#SelectedTipo").val(0);
    $("#Identificador").val("");
    $("#FechaHoraDeInspeccion").val("");
    $("#SelectedEstado").val(0);
    $("#SelectedTipo").empty();
    $("#selectedUT").val("");
    $("#ResultadoInspeccion").val("");
    Grilla_UnidadesTributarias.clear().draw();
}

function SetEnabledInspeccion(enabled) {
    _enabled = enabled;
    var statusCerrados = $("#SelectedEstado").val() === "4" ? false : enabled;

    $("#InspeccionId").attr("disabled", !statusCerrados);
    $("#SelectedTipoInspeccion").attr("disabled", !statusCerrados);
    $("#Inspector").attr("disabled", !statusCerrados);
    $("#FechaHoraDesde").attr("disabled", !statusCerrados);
    $("#FechaHoraHasta").attr("disabled", !statusCerrados);
    $("#Descripcion").attr("disabled", !statusCerrados);
    $("#SelectedInspector").attr("disabled", statusCerrados);
    $("#FechaOrigenDesde").attr("disabled", !statusCerrados);
    $("#HoraOrigenDesde").attr("disabled", !statusCerrados);
    $("#FechaOrigenHasta").attr("disabled", !statusCerrados);
    $("#HoraOrigenHasta").attr("disabled", !statusCerrados);
    $("#Descripcion").attr("disabled", !statusCerrados);
    $("#SelectedObjeto").attr("disabled", !statusCerrados);
    $("#SelectedTipo").attr("disabled", !statusCerrados);
    $("#Identificador").attr("disabled", !statusCerrados);
    $("#FechaHoraDeInspeccion").attr("disabled", !statusCerrados);
    $("#SelectedEstado").attr("disabled", !statusCerrados);
    $("#SelectedTipo").attr("disabled", !statusCerrados);
    $("#selectedUT").attr("disabled", !statusCerrados);
    $("#ResultadoInspeccion").attr("disabled", !enabled);

    $("#btnRemUT").addClass('boton-deshabilitado');
    $("#btnRemDoc").addClass('boton-deshabilitado');
    $("#btnEditDoc").addClass('boton-deshabilitado');
    $("#btnViewDoc").addClass('boton-deshabilitado');
    if (!statusCerrados) {
        $("#btnAddUT").addClass('boton-deshabilitado');
    } else {
        $("#btnAddUT").removeClass('boton-deshabilitado');
    }
    if (!_enabled) {

        $("#btnAddDoc").addClass('boton-deshabilitado');
        $("#btnGrabar").addClass('boton-deshabilitado');
        $("#btnBorrar").addClass('boton-deshabilitado');

    } else {

        $("#btnAddDoc").removeClass('boton-deshabilitado');
        if ($("#puedeProgramar").val() != 'false') {
            $("#btnGrabar").removeClass("boton-deshabilitado");
            $("#btnBorrar").removeClass("boton-deshabilitado");
        }
    }

    console.log('end');
}

function SetStatusResultado(status) {
    if (_enabled) {
        if (status) {
            $("#FechaHoraDeInspeccion").attr('disabled', 'disabled');
            $(".fechaCompleta3 span").hide();
            $("#SelectedEstado").attr('disabled', 'disabled');
            $("#enabledResultado").val('false');
            $("#ResultadoInspeccion").attr('disabled', 'disabled');
        } else {
            $("#FechaHoraDeInspeccion").removeAttr('disabled');
            $(".fechaCompleta3 span").show();
            $("#SelectedEstado").removeAttr('disabled');
            $("#enabledResultado").val('true');
            $("#ResultadoInspeccion").removeAttr('disabled');
        }
    }
}

function fnVerSiPuedeVerResultado() {
    switch ($("#SelectedEstado").val()) {
        case 1: //planificada
            if (currEditObject.InspectorID == currEditObject.UsuarioUpdate) {
                SetStatusResultado(true);
            } else {
                SetStatusResultado(false);
            }
            break;
        case 2: //abierta
            break;
        case 3: //Vencida
            break;
        case 4: //Finalizada
            break;
    }
}

function doPostInspeccion(desde) {//
    var formData = {
        "InspeccionID": $("#InspeccionId").val(),
        "InspectorID": $("#SelectedInspector").val(),
        "TipoInspeccionID": $("#SelectedTipoInspeccion").val(),
        "Descripcion": $("#Descripcion").val(),
        "FechaHoraInicio": $("#FechaHoraDesde").val(),
        "FechaHoraFin": $("#FechaHoraHasta").val(),
        "UsuarioAltaID": "",
        "FechaAlta": "",
        "UsuarioModificacionID": "",
        "FechaModificacion": "",
        "UsuarioBajaID": "",
        "FechaBaja": "",
        "Objeto": $("#SelectedObjeto").val(),
        "Tipo": $("#SelectedTipo").val(),
        "Identificador": $("#Identificador").val(),
        "SelectedEstado": $("#SelectedEstado").val(),
        "FechaHoraDeInspeccion": $("#FechaHoraDeInspeccion").val(),
        "ResultadoInspeccion": $("#ResultadoInspeccion").val(),
        "selectedUT": $("#selectedUT").val(),
        "selectedDocs": $("#selectedDocs").val()
    };
    showLoading();
    $.ajax({
        url: BASE_URL + 'ObrasParticulares/PostInspeccionProgramar',
        type: "POST",
        data: formData,
        success: function (data) {
            if (data.Response === "OK") {
                alerta('Inspección', 'Se ha guardado correctamente la inspección', 1);
                $('.fechaCalendarioValue').val(GetNameFromMonthNumber(desde.getMonth()) + ' ' + desde.getFullYear());
                $('.fechaCalendario').datepicker('update');
                $('#calendar').calendar(options);
                EnablePanel(1);
                if (typeof inspeccionGuardada === 'function') {
                    inspeccionGuardada(data.inspeccion);
                }
            } else {
                alerta('Error', 'Ha ocurrido un error al guardar la inspección', 3);
            }

        },
        error: function (_, __, errorThrown) {
            alerta('Error', errorThrown, 3);
        },
        complete: hideLoading
    });
}

function CheckReplanificar() {
    var estado = parseInt($("#SelectedEstado").val());
    var desde = new Date(convertDateFromText($("#FechaHoraDesde").val()));
    if (estado === 3 && desde > new Date()) {    // Antes estaba Vencida y ahora se replanifica
        $("#SelectedEstado").val(1);
    }
}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

function loadBuscador() {
    $("#buscador-externo").load(BASE_URL + "Documento/DatosDocumento");
}

/* ---------------------------------< DATE TIME PICKERS >--------------------------------- */
$('.fechaCompleta').datetimepicker({
    format: "DD/MM/YYYY HH:mm",
    focusOnShow: true,
    widgetPositioning: {
        horizontal: "left",
        vertical: "bottom"
    }
});

$('.fechaCompleta').on("dp.change", CheckReplanificar);

$('.fechaCompleta3').datetimepicker({
    format: "DD/MM/YYYY HH:mm",
    focusOnShow: true,
    widgetPositioning: {
        horizontal: "left",
        vertical: "top"
    }
});
/* ---------------------------------< DATE TIME PICKERS >--------------------------------- */

//@ sourceURL=obrasParticulares.js