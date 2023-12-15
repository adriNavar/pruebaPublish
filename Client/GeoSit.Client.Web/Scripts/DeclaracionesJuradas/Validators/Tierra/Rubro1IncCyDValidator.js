var Rubro1IncCyDFormUSoRValidator = function (containers, readOnly) {
    const form = $("#Rubro1IncCyD", containers.section);
    let currentDominioRowIndex = -1,
        tableDominio = null,
        newDominioNegativeId = -100,
        dominios = [],
        tableDomicilios = null,
        domicilioRowIndex = -1,
        newDomicilioNegativeId = -100,
        tablePropietarios = null,
        propietarioRowIndex = -1,
        newPropietarioNegativeId = -100;

    function buscarMensura() {
        return new Promise(function (resolve) {
            var data = { tipos: BuscadorTipos.Mensuras, multiSelect: false, verAgregar: false, titulo: 'Buscar Mensura Registrada', campos: ['Nombre'] };
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

    function getDominioCurrentRow() {
        return tableDominio.row(currentDominioRowIndex).data();
    }
    function setDominioRow(row) {
        if (!tableDominio.row(tableDominio.row(row).index()).data()) {
            disableSelectDominio();
            getPropietarios();
            return;
        }

        if (currentDominioRowIndex === tableDominio.row(row).index()) {
            disableSelectDominio();
        }
        else {
            enableSelectDominio(row);
        }
        getPropietarios();
    }
    function enableSelectDominio(row) {
        currentDominioRowIndex = tableDominio.row(row).index();
        if (!readOnly)
            $('#btnAgregarPropietario, #btnEditarDominio, #btnEliminarDominio', containers.section).removeClass('disabled');
    }
    function disableSelectDominio() {
        currentDominioRowIndex = -1;
        $('#btnAgregarPropietario, #btnEditarDominio, #btnEliminarDominio', containers.section).addClass('disabled');
        disableSelectPropietario();
        getPropietarios();
    }
    function updateDominioRow(dominio) {
        if (!!dominio.IdDominio) {
            var row = tableDominio.row(currentDominioRowIndex).data();
            row.IdTipoInscripcion = dominio.IdTipoInscripcion;
            row.TipoInscripcion = dominio.TipoInscripcion;
            row.Inscripcion = dominio.Inscripcion;
            row.Fecha = dominio.Fecha;
            row.HasChanges = true;
            pushDominio(row);
        }
        else {
            dominio.IdDominio = --newDominioNegativeId;
            dominio.Titulares = [];
            pushDominio(dominio);
        }
        disableSelectDominio();
    }
    function deleteDominioRow() {
        var dominio = getDominioCurrentRow();
        dominio.IsDeleted = true;

        pushDominio(dominio);

        disableSelectDominio();
    }
    function pushDominio(dominio) {
        _.remove(dominios, { IdDominio: dominio.IdDominio });

        if (!dominio.IsDeleted) {
            dominios.push(dominio);
        }

        $('#dominiosJSON', containers.section).val(JSON.stringify(dominios));

        tableDominio.clear().draw();
        tableDominio.rows.add(dominios).draw();
    }
    function getPropietarioCurrentRow() {
        return tablePropietarios.row(propietarioRowIndex).data();
    }
    function setPropietarioRow(row) {
        if (!tablePropietarios.row(tablePropietarios.row(row).index()).data()) {
            disableSelectPropietario();
            getDomicilios();
            return;
        }

        if (propietarioRowIndex === tablePropietarios.row(row).index()) {
            disableSelectPropietario();
        }
        else {
            enableSelectPropietario(row);
        }
        getDomicilios();
    }
    function enableSelectPropietario(row) {
        propietarioRowIndex = tablePropietarios.row(row).index();
        if (!readOnly)
            $('#btnEditarPropietario, #btnEliminarPropietario, #btnAgregarDomicilio', containers.section).removeClass('disabled');
    }
    function disableSelectPropietario() {
        domicilioRowIndex = propietarioRowIndex = -1;
        $('#btnEditarPropietario, #btnEliminarPropietario, #btnAgregarDomicilio, #btnEliminarDomicilio', containers.section).addClass('disabled');
        getDomicilios();
    }
    function getPropietarioDomicilios(propietario) {
        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/GetPersonaDomicilios?idPersona=" + propietario.IdPersona,
            dataType: 'json',
            type: 'GET',
            success: function (data) {
                _.forEach(data, function (value) {
                    value.IdPersonaDomicilio = --newDomicilioNegativeId;
                    value.IdDominioTitular = propietario.IdDominioTitular;
                });

                propietario.PersonaDomicilio = data;
                pushPropietario(propietario);

            },
            error: function (error) {
                declaracionesJuradas.mostrarMensajeError(error.responseText, "Obtener los domicilios del propietario", true);
            }
        });
    }
    function updatePropietariosRow(propietario) {
        if (!!propietario.IdDominioTitular) {
            var row = tablePropietarios.row(propietarioRowIndex).data();
            row.IdTipoTitularidad = propietario.IdTipoTitularidad;
            row.TipoTitularidad = propietario.TipoTitularidad;
            row.IdPersona = propietario.IdPersona;
            row.TipoNoDocumento = propietario.TipoNoDocumento;
            row.NombreCompleto = propietario.NombreCompleto;
            row.PorcientoCopropiedad = propietario.PorcientoCopropiedad;
            row.HasChanges = true;
            pushPropietario(row);
        }
        else {
            propietario.IdDominioTitular = --newPropietarioNegativeId;
            propietario.IdDominio = getDominioCurrentRow().IdDominio;
            getPropietarioDomicilios(propietario);
        }
        disableSelectPropietario();
    }
    function deletePropietarioRow() {
        var propietario = getPropietarioCurrentRow();
        propietario.IsDeleted = true;

        pushPropietario(propietario);

        disableSelectPropietario();
    }
    function pushPropietario(propietario) {
        var currentDominio = _.find(dominios, function (d) {
            return d.IdDominio === getDominioCurrentRow().IdDominio;
        });

        _.remove(currentDominio.Titulares, { IdDominioTitular: propietario.IdDominioTitular });

        if (!propietario.IsDeleted) {
            currentDominio.Titulares.push(propietario);

            var index = _.findIndex(dominios, { IdDominio: getDominioCurrentRow().IdDominio });

            dominios.splice(index, 1, currentDominio);
        }

        $('#dominiosJSON', containers.section).val(JSON.stringify(dominios));

        tablePropietarios.clear().draw();
        tablePropietarios.rows.add(currentDominio.Titulares).draw();
    }
    function getPropietarios() {
        if (currentDominioRowIndex !== -1) {

            tablePropietarios.clear().draw();

            var currentDominio = _.find(dominios, function (d) {
                return d.IdDominio === getDominioCurrentRow().IdDominio;
            });

            if (currentDominio) {
                tablePropietarios.rows.add(currentDominio.Titulares).draw();
            }
        }
        else {
            tablePropietarios.clear().draw();
        }
    }
    function getDomicilioCurrentRow() {
        return tableDomicilios.row(domicilioRowIndex).data();
    }
    function setDomicilioRow(row) {
        if (!tableDomicilios.row(tableDomicilios.row(row).index()).data()) {
            disableSelectDomicilio();
            return;
        }

        if (domicilioRowIndex === tableDomicilios.row(row).index()) {
            disableSelectDomicilio();
        }
        else {
            enableSelectDomicilio(row);
        }
    }
    function enableSelectDomicilio(row) {
        domicilioRowIndex = tableDomicilios.row(row).index();
        if (!readOnly)
            $('#btnEliminarDomicilio', containers.section).removeClass('disabled');
    }
    function disableSelectDomicilio() {
        domicilioRowIndex = -1;
        $('#btnEliminarDomicilio', containers.section).addClass('disabled');
    }
    function updateDomiciliosRow(domicilio) {

        var newDomicilio = {
            IdPersonaDomicilio: --newDomicilioNegativeId,
            IdTipoDomicilio: domicilio.TipoDomicilioId,
            IdDominioTitular: getPropietarioCurrentRow().IdDominioTitular,
            IdDomicilio: domicilio.DomicilioId,
            Tipo: domicilio.TipoDomicilio.Descripcion,
            Provincia: domicilio.provincia,
            Localidad: domicilio.localidad,
            Barrio: domicilio.barrio,
            Calle: domicilio.ViaNombre,
            Altura: domicilio.numero_puerta,
            Piso: domicilio.piso,
            Departamento: domicilio.piso,
            CodigoPostal: domicilio.codigo_postal,
            Domicilio: domicilio
        };

        pushDomicilio(newDomicilio);
    }
    function deleteDomicilioRow() {
        var domicilio = getDomicilioCurrentRow();
        domicilio.IsDeleted = true;

        pushDomicilio(domicilio);

        disableSelectDomicilio();
    }
    function pushDomicilio(domicilio) {
        var currentDominio = _.find(dominios, function (d) {
            return d.IdDominio === getDominioCurrentRow().IdDominio;
        });

        var currentTitular = _.find(currentDominio.Titulares, function (t) {
            return t.IdPersona === getPropietarioCurrentRow().IdPersona;
        });

        _.remove(currentTitular.PersonaDomicilio, { IdPersonaDomicilio: domicilio.IdPersonaDomicilio });

        if (!domicilio.IsDeleted) {
            currentTitular.PersonaDomicilio.push(domicilio);

            var index = _.findIndex(dominios, { IdDominio: getDominioCurrentRow().IdDominio });

            dominios.splice(index, 1, currentDominio);
        }

        $('#dominiosJSON', containers.section).val(JSON.stringify(dominios));

        tableDomicilios.clear().draw();
        tableDomicilios.rows.add(currentTitular.PersonaDomicilio).draw();
    }
    function getDomicilios() {
        if (propietarioRowIndex !== -1) {
            tableDomicilios.clear().draw();

            var currentDominio = _.find(dominios, function (d) {
                return d.IdDominio === getDominioCurrentRow().IdDominio;
            });

            if (currentDominio) {
                var currentPersona = _.find(currentDominio.Titulares, function (d) {
                    return d.IdDominioTitular === getPropietarioCurrentRow().IdDominioTitular;
                });
                tableDomicilios.rows.add(currentPersona.PersonaDomicilio).draw();
            }
        }
        else {
            tableDomicilios.clear().draw();
        }
    }
    function formatDate(data) {
        if (!data) return null;
        var parts = data.split("-");
        if (!parts[2]) return data;
        var dd = parts[2].split("T");
        if (!dd[1]) return data;
        return $.datepicker.formatDate("dd/mm/yy", new Date(parts[0], parts[1] - 1, dd[0]));
    }
    function validate() {
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();
        return bootstrapValidator.isValid() && validateCopropiedad();
    }
    function validateCopropiedad() {

        if (!readOnly) {
            var errors = [];

            if (!dominios.length) {
                errors.push("Debe cargar al menos una inscripción de dominio");
            }

            if (!_.every(dominios, function (d) {
                return d.Titulares.length;
            })) {
                errors.push("Los dominios deben tener al menos un propietario");
            }

            if (!_.every(dominios, function (d) {
                return Math.round(_.sumBy(d.Titulares, function (e) {
                    return parseFloat(e.PorcientoCopropiedad);
                }), 2) === 100;
            })) {
                errors.push("La suma de los porcentajes de copropiedad debe ser igual a 100 para todos los dominios");
            }

            if (!_.every(dominios, function (d) {
                return _.every(d.Titulares, function (t) {
                    return t.PersonaDomicilio.length;
                })
            })) {
                errors.push("Los propietarios deben tener al menos un domicilio");
            }

            if (errors.length)
                declaracionesJuradas.mostrarMensajeError(errors.join('<br>'), "Validación", true, null);

            return !errors.length;
        }

        return true;

    }
    function agregarEditarDominio(row) {
        $(containers.formularioExterno).load(`${BASE_URL}DeclaracionesJuradas/EditarInscripcionDominio`, { model: row, idUnidadTributaria: $("#IdUnidadTributaria", containers.section).val(), idClaseParcela: $("#IdClaseParcela", containers.section).val() },
            () => {
                new EditarDominio().init();
                $(document)
                    .off("dominioGuardado")
                    .one("dominioGuardado", function (evt) {
                        updateDominioRow(evt.dominio);
                    });
            });
    }
    function agregarEditarPropietario(row, controller) {
        $(containers.formularioExterno).load(`${BASE_URL}DeclaracionesJuradas/EditarPropietario`, { model: row, idUnidadTributaria: $("#IdUnidadTributaria", containers.section).val(), idClaseParcela: $("#IdClaseParcela", containers.section).val() },
            () => {
                new EditarPropietario(containers).init(controller);
                $(document)
                    .off("propietarioGuardado")
                    .one("propietarioGuardado", function (evt) {
                        updatePropietariosRow(evt.propietario);
                    });
            });
    }
    function loadDomicilios() {
        var domicilio = getDomicilioCurrentRow();
        var parametros;
        if (!domicilio) {
            parametros = {};
        }
        else {
            parametros = { id: domicilio.DomicilioId, domicilio: domicilio };
        }
        $.ajax({
            url: BASE_URL + "Domicilio/DatosDomicilio",
            type: 'POST',
            data: parametros,
            dataType: 'html',
            success: function (result) {
                $(document).one("domicilioGuardado", function (evt) {
                    updateDomiciliosRow(evt.domicilio);
                });
                $(containers.formularioExterno).empty();
                $(containers.formularioExterno).html(result);
            },
            error: function (_, textStatus, errorThrown) {
                console.log(textStatus, errorThrown);
            }
        });
    }

    function init(controller) {
        form.bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            fields: {

            }
        });

        // $('#btnAgregarDominio', containers.section).on('click', () => agregarEditarDominio());

        $('#btnEditarDominio', containers.section).on('click', () => agregarEditarDominio(getDominioCurrentRow()));

        $('#btnEliminarDominio', containers.section).on('click', function () {
            controller.mostrarAdvertencia("Confirmación", "¿Desea eliminar la Inscripción de dominio seleccionada?", deleteDominioRow);
        });

        $("#btn-mensura-search", containers.section).click(function () {
            buscarMensura()
                .then(function (seleccion) {
                    if (seleccion.length) {
                        $("#Mensura", containers.section).val(seleccion[0]);
                        $("#DDJJU_IdMensura", containers.section).val(seleccion[1]);
                        $("#DDJJSor_IdMensura", containers.section).val(seleccion[1]);
                    } else {
                        controller.mostrarAdvertencia('Buscar Mensura', 'No se ha seleccionado ninguna mensura.');
                        return;
                    }
                })
                .catch(function (err) {
                    console.log(err);
                });
        });

        if (!readOnly) {
            $("#btn-mensura-search", containers.section).removeClass("disabled");
        }

        tableDominio = $("#GrillaInscripcionDominio", containers.section).DataTable({
            dom: "tr",
            destroy: true,
            language: {
                url: BASE_URL + "Scripts/dataTables.spanish.txt"
            },
            order: [1, 'asc'],
            data: [],
            bFilter: false,
            bPaginate: false,
            columns: [
                { data: "IdDominio", visible: false },
                { data: "IdTipoInscripcion", visible: false },
                { title: "Tipo", data: "TipoInscripcion" },
                { title: "Inscripción", data: "Inscripcion" },
                {
                    title: "Año", data: "Fecha", render: function (data) {
                        var date = formatDate(data);
                        var splitted = date.split('/');
                        return splitted[2];
                    }
                }
            ],
            initComplete: function (options) {
                //agregue un if que me pone readonly en true si la cosa tiene dato
                let tableDominiosNumRows = $(this).dataTable().api().data().length;

                if (tableDominiosNumRows > 0) {
                    console.log('tiene dato')
                    readOnly = true
                    $('#btnAgregarDominio', containers.section).addClass('disabled');
                } else {
                    console.log('no tiene nada')
                    $('#btnAgregarDominio', containers.section).on('click', () => agregarEditarDominio());

                }
                $(this).dataTable().api().columns.adjust();
                $(options.nTBody).off("click", "tr");
                $(options.nTBody).on("click", "tr", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (!$("td.dataTables_empty", this).length) {
                        $(this).siblings().removeClass('selected');
                        $(this).toggleClass('selected');
                        setDominioRow(this);
                    }
                });
            }
        });

        tablePropietarios = $("#GrillaPropietarios", containers.section).DataTable({
            dom: "tr",
            destroy: true,
            language: {
                url: BASE_URL + "Scripts/dataTables.spanish.txt"
            },
            order: [1, 'asc'],
            data: [],
            bFilter: false,
            bPaginate: false,
            columns: [
                { data: "IdDominioTitular", visible: false },
                { data: "IdPersona", visible: false },
                { data: "IdDominio", visible: false },
                { data: "IdTipoTitularidad", visible: false },
                { title: "Nombre / Razón social", data: "NombreCompleto" },
                { title: "Tipo y Nro.", data: "TipoNoDocumento" },
                { title: "Tipo Titularidad", data: "TipoTitularidad" },
                { title: "% Copro.", data: "PorcientoCopropiedad" }
            ],
            createdRow: function (row, data) {
                $(row).data('id', data.Id);
                $(row).addClass('cursor-pointer');
            },
            initComplete: function (options) {
                $(this).dataTable().api().columns.adjust();
                $(options.nTBody).off("click", "tr");
                $(options.nTBody).on("click", "tr", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (!$("td.dataTables_empty", this).length) {
                        $(this).siblings().removeClass('selected');
                        $(this).toggleClass('selected');
                        setPropietarioRow(this);
                    }
                });
            }
        });

        tableDomicilios = $("#GrillaDomicilios", containers.section).DataTable({
            dom: "tr",
            destroy: true,
            language: {
                url: BASE_URL + "Scripts/dataTables.spanish.txt"
            },
            order: [1, 'asc'],
            data: [],
            bFilter: false,
            bPaginate: false,
            columns: [
                { data: "IdPersonaDomicilio", visible: false },
                { data: "IdTipoDomicilio", visible: false },
                { data: "IdDominioTitular", visible: false },
                { data: "IdDomicilio", visible: false },
                { title: "Tipo", data: "Tipo", orderable: true },
                { title: "Provincia", data: "Provincia", orderable: true },
                { title: "Localidad", data: "Localidad", orderable: true },
                { title: "Barrio", data: "Barrio", orderable: true },
                { title: "Calle", data: "Calle", orderable: true },
                { title: "Nro", data: "Altura", orderable: true },
                { title: "Piso", data: "Piso", orderable: true },
                { title: "Depto", data: "Departamento", orderable: true },
                { title: "CP", data: "CodigoPostal", orderable: true }
            ],
            createdRow: function (row, data) {
                $(row).data('id', data.Id);
                $(row).addClass('cursor-pointer');
            },
            initComplete: function (options) {
                $(this).dataTable().api().columns.adjust();
                $(options.nTBody).off("click", "tr");
                $(options.nTBody).on("click", "tr", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (!$("td.dataTables_empty", this).length) {
                        $(this).siblings().removeClass('selected');
                        $(this).toggleClass('selected');
                        setDomicilioRow(this);
                    }
                });
            }
        });

        let loadDominios;
        if ($("#dominiosJSON", containers.section).val()) {
            loadDominios = Promise.resolve($('#dominiosJSON', containers.section).val());
        } else {
            loadDominios = new Promise((ok, error) => {
                $.ajax({
                    url: $('#urlActionGetInscripcionDominio').data('request-url') + '?IdDeclaracionJurada=' + $('#DDJJ_IdDeclaracionJurada').val(),
                    dataType: 'json',
                    type: 'GET',
                    success: function (data) {
                        $('#dominiosJSON', containers.section).val(data);
                        ok(data);
                    },
                    error: error
                });
            });
        }

        loadDominios
            .then((data) => {
                dominios = $.parseJSON(data);
                newDominioNegativeId = dominios.length * -1;
                tableDominio.clear().draw();
                tableDominio.rows.add(dominios).draw();
            })
            .catch(error => {
                controller.mostrarError(error.responseText, "Recuperar Dominios");
            });

        $('#btnAgregarPropietario', containers.section).on('click', () => agregarEditarPropietario(null, controller));

        $('#btnEditarPropietario', containers.section).on('click', () => agregarEditarPropietario(getPropietarioCurrentRow(), controller));

        $('#btnEliminarPropietario', containers.section).on('click', function () {
            controller.mostrarAdvertencia("Confirmación", "¿Desea eliminar el Propietario seleccionado?", deletePropietarioRow);
        });

        $('#btnAgregarDomicilio', containers.section).on('click', loadDomicilios);

        $('#btnEliminarDomicilio', containers.section).on('click', function () {
            controller.mostrarAdvertencia("Confirmación", "¿Desea eliminar el Domicilio seleccionado?", deleteDomicilioRow);
        });
    }

    return {
        init: init,
        validate: validate
    };
};
//# sourceURL=R1CyD.js