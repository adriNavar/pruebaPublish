var Rubro2FormUValidator = function (modalClases, modalAforo, containers) {
    let esUsuarioInterno = false,
        currentClaseRowIndex = -1,
        tableClase,
        validated = false,
        idTMLSelected,
        idViaCalle,
        _controller;


    function buscar(tipo, titulo, idPadre) {
        return new Promise(function (resolve) {
            var data = { tipos: tipo, multiSelect: false, verAgregar: false, titulo: titulo, campos: ['Nombre'], filters: ["idpadre=" + idPadre] };
            $(containers.buscador).load(BASE_URL + "BuscadorGenerico", data, function () {
                $(".modal", this).one('hidden.bs.modal', function () {
                    $(window).off('seleccionAceptada');
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

    function onChangeClase(evt) { // Seleccion dropdown clase disponibles.

        clasesSeleccionadas = [];

        if (!isNaN(parseInt(evt.currentTarget.value))) {


            const curClaseParcela = clasesDisponibles.find(cd => cd.IdClaseParcela === parseInt(evt.currentTarget.value));

            clasesSeleccionadas = [curClaseParcela];

            
            var selected = $("#cboClasesDisponibles", containers.section).val();
            
            if (selected != 0) {
                $.ajax({
                    url: BASE_URL + "DeclaracionesJuradas/GetCroquisClaseParcela",
                    data: { id: parseInt(evt.currentTarget.value) },
                    type: "POST",
                    success: (croqui) => {
                        $("#Croquis_div").removeClass("hidden");
                        if (!$("#Croquis_div").is(":hidden")) {
                            $("#clase_div").removeClass("col-xs-12");
                            $("#clase_div").addClass("col-xs-6");
                        }
                        document.getElementById("croquisClaseParcela").src = croqui;
                    },
                });
            } else {
                $("#Croquis_div").addClass("hidden");
                $("#clase_div").removeClass("col-xs-6");
                $("#clase_div").addClass("col-xs-12");
            }
        }

        $("#Croquis_div").addClass("hidden");
        $("#clase_div").removeClass("col-xs-6");
        $("#clase_div").addClass("col-xs-12");

        serialize();

        refreshTiposLineales();

    }

    function refreshTiposLineales() {

        var c = clasesSeleccionadas[0];

        // Ocultamos todas las líneas de los tipos de medida lineal.
        $('.tipoMedidaLineal', containers.section).hide();

        // Ocultamos todas las busquedas de aforo.
        $('.buscarAforo', containers.section).hide();

        // Vaciamos todos los valores de todos los inputs.
        $('.metrosInput', containers.section).val('');
        $('.aforosInput', containers.section).val('');

        if (c) {

            // Buscamos si tiene mas de una busqueda de aforo dentro de la medida lineal.
            var cantidad = c.TiposMedidasLineales.filter(e => e.RequiereAforo).length;

            $(c.TiposMedidasLineales).each(function (_, el) {

                // Esto genera una búsqueda automática para aforo.
                if (el.RequiereAforo && cantidad === 1) {
                    el = buscarAforoAutomatic(el); // Busca el aforo automaticamente cuando es agregado y solo tiene un solo requiere aforo.
                    el.autoSetDireccion = true;
                }
                else if (el.RequiereAforo && cantidad !== 1) {
                    el.autoSetDireccion = false;
                }

                setTipoMedidaLinealValues(
                    el.IdTipoMedidaLineal,
                    el.ValorMetros,
                    el.Calle,
                    el.Desde,
                    el.Hasta,
                    el.Paridad,
                    el.ValorAforo,
                    el.RequiereAforo,
                    el.RequiereLongitud
                );
            });
        }

        setTimeout(_controller.ajustarScrollBars, 100);

    }
    function buscarAforoAutomatic(el) {
        if (el.ValorAforo) {
            return el;
        }
        $("#aforoCalle", modalAforo).val($("#DDJJDesignacion_Calle", containers.formularioDDJJ).val());
        $("#aforoCalleIdVia", modalAforo).val($("#DDJJDesignacion_IdCalle", containers.formularioDDJJ).val());
        $("#aforoAltura", modalAforo).val($('#DDJJDesignacion_Numero', containers.formularioDDJJ).val());


        el = buscarAforoServer(el);
        return el;
    }
    function setTipoMedidaLinealValues(id, metros, calle, desde, hasta, paridad, aforo, requiereAforo, requiereLongitud) { // Este setea los valores de un Id medida lineal.


        $("#GrillaCaracteristicas tbody tr", containers.section).last("tr").before($('#tipoMedidaLineal' + id, containers.section)); //hack de negro pero ahora anda jeje

        $('#tipoMedidaLineal' + id, containers.section).show();

        $('#metros' + id, containers.section).val(metros);
        $('#calle' + id, containers.section).text(calle);
        $('#desde' + id, containers.section).text(desde);
        $('#hasta' + id, containers.section).text(hasta);
        $('#paridad' + id, containers.section).text(paridad);
        if (requiereAforo) {
            $('#aforo' + id, containers.section).val(typeof aforo === 'number' ? aforo.toFixed(2) : aforo);
        } else {
            $('#aforo' + id, containers.section).hide();
        }

        if (requiereAforo) {
            $('#buscarAforo' + id, containers.section).show();
        }

        // Setea Validación.
        if (!metros && validated) {
            if (requiereLongitud) {
                $('#descripcion' + id, containers.section).addClass('mark-error');
                $('#metros' + id, containers.section).addClass('mark-error');
            }
        } else {
            $('#descripcion' + id, containers.section).removeClass('mark-error');
            $('#metros' + id, containers.section).removeClass('mark-error');
        }

        if (!aforo && validated) {
            if (requiereAforo) {
                $('#aforo' + id, containers.section).addClass('mark-error');
            }
        } else {
            $('#aforo' + id, containers.section).removeClass('mark-error');
        }

        // Validamos si se buscó aforo.
        if (!$('#aforo' + id, containers.section).val() && validated) {
            $('#buscarAforo' + id, containers.section).addClass('mark-error');
        }
        else {
            $('#buscarAforo' + id, containers.section).removeClass('mark-error');
        }


    }
    function updateBlurTipoMedidaLineal() {

        const c = clasesSeleccionadas[0];

        if (!c) return;

        var tml = parseInt($(this).data('tml'));


        $(c.TiposMedidasLineales).each(function (_, el) {

            if (el.IdTipoMedidaLineal === tml) {

                el.ValorMetros = $('#metros' + tml, containers.section).val();

                // Setea Validación.
                if (!el.ValorMetros && !el.RequiereLongitud) {
                    $('#descripcion' + el.IdTipoMedidaLineal).removeClass('mark-error');
                    $('#metros' + el.IdTipoMedidaLineal).removeClass('mark-error');
                }

            }
        });

        // Guardamos los cambios.
        serialize();

        return;

    }
    function updateTipoMedidaLineal(idTML, aforo) {

        const c = clasesSeleccionadas[0];

        if (!c) {
            return;
        }

        var tml = null;

        if (!idTML) {
            tml = $(this).data('tml');
        } else {
            tml = idTML;
        }


        $(c.TiposMedidasLineales).each(function (_, el) {

            if (el.IdTipoMedidaLineal === parseInt(tml)) {

                el.ValorMetros = $('#metros' + tml).val();

                // Si viene el aforo lo actualizo a la clase actual.
                if (aforo) {
                    el.IdTramoVia = aforo.IdTramoVia;
                    el.IdVia = aforo.IdVia;
                    el.Altura = aforo.Altura;
                    el.Calle = aforo.Calle;
                    el.Desde = aforo.Desde;
                    el.Hasta = aforo.Hasta;
                    el.Paridad = aforo.Paridad;
                    el.ValorAforo = typeof aforo.ValorAforo === 'number' ? aforo.ValorAforo.toFixed(2) : aforo.ValorAforo;
                }

                // Setea Validación.
                if (el.ValorMetros) {
                    $('#descripcion' + el.IdTipoMedidaLineal).removeClass('mark-error');
                    $('#metros' + el.IdTipoMedidaLineal).removeClass('mark-error');
                }

            }
        });



        // Guardamos los cambios.
        serialize();

        return;

    }
    function updateBlurAforo(evt) {
        const cml = clasesSeleccionadas[0];

        var tml = cml.TiposMedidasLineales.find(tml => tml.IdTipoMedidaLineal === Number($(evt.currentTarget).data("tml")));
        if (!isNaN(parseFloat(evt.currentTarget.value))) {
            tml.ValorAforo = parseFloat(evt.currentTarget.value);
        } else {
            tml.ValorAforo = null;
        }

        serialize();

    }
    function getTipoMedidaLineal(idTML) {

        const c = clasesSeleccionadas[0];

        if (!c) {
            return;
        }

        var tml = parseInt(idTML);

        var medida = null;

        $(c.TiposMedidasLineales).each(function (idx, el) {
            if (el.IdTipoMedidaLineal === tml) {
                medida = el;
                return false;
            }
        });

        return medida;

    }
    function serialize() {
        $('#ClasesJsonSerialized', containers.section).val(JSON.stringify(clasesSeleccionadas));
    }

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            //console.log("llamada carga imagen");
            reader.onload = function (e) {
                $('#imageDisplay', containers.section).attr('src', e.target.result);
                $('#CroquisBase64', containers.section).val(e.target.result);
                $(containers.formularioDDJJ).getNiceScroll().show();
            };
            reader.readAsDataURL(input.files[0]); // convert to base64 string                        

            $('#imageDisplay', containers.section).show();
            $(".rubro2-scroll-content", containers.section).niceScroll(getNiceScrollConfig());
        }
    }
    function onlyNumberKeyPress(evt) {
        var theEvent = evt || window.event;

        // Handle paste
        if (theEvent.type === 'paste') {
            key = event.clipboardData.getData('text/plain');
        } else {
            // Handle key press
            var key = theEvent.keyCode || theEvent.which;
            key = String.fromCharCode(key);
        }
        var regex = /[0-9]|\./;
        if (!regex.test(key)) {
            theEvent.returnValue = false;
            if (theEvent.preventDefault) theEvent.preventDefault();
        }
    }

    function getUsuarioInterno() {
        $.get(BASE_URL + "Seguridad/GetUsuarioInterno", function (interno) {
            esUsuarioInterno = interno;
        });
    }
    /*function validateClase() {
        if (esUsuarioInterno && clasesSeleccionadas.length === 0) {
            _controller.mostrarError("Rubro 2", "Debe cargar al menos una Clase");
            return false;
        }

        return true;
    }*/

    function validateClase() {
        if (esUsuarioInterno && $("#cboClasesDisponibles", containers.section).val() == '') {
            _controller.mostrarError("Rubro 2", "Debe cargar al menos una Clase");
            $('.tipoMedidaLineal', containers.section).hide();
            return false;
        }

        return true;
    }

    function validateSuperficie() {
        if ($('#DDJJU_SuperficiePlano', containers.section).val() == '' && $('#DDJJU_SuperficieTitulo', containers.section).val() == '') {
            _controller.mostrarError("Rubro 1", "Debe cargar al menos una de las superficies");
            return false;
        }

        return true;
    }

    function validateAforo() {
        let valido = false;
        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/ValoresAforoValido",
            type: "GET",
            async: false,
            success: (data) => {
                const aforo = JSON.parse(data);
                valido = !clasesSeleccionadas.some(cls => !cls.IsDeleted && cls.TiposMedidasLineales.some(ml => !isNaN(parseFloat(ml.ValorAforo)) && (aforo.minimo > parseFloat(ml.ValorAforo) || aforo.maximo < parseFloat(ml.ValorAforo))));
                if (!valido) {
                    _controller.mostrarError("Rubro 2", `Los aforos deben estar entre los valores: ${aforo.minimo} y ${aforo.maximo}`);
                }
            },
            error: (err) => {
                let msg = "Ha ocurrido un error al validar el aforo";
                if (err.status === 400) {
                    msg = "Falta configurar aforo máximo o aforo mínimo";
                }
                _controller.mostrarError("Rubro 2", msg);
            }
        });

        return valido;
    }

    let timeoutClases;
    $('#DDJJU_SuperficiePlano, #DDJJU_SuperficieTitulo', containers.section).on('input', function (evt) {
        clearTimeout(timeoutClases);
        timeoutClases = setTimeout(filtrarClases, 300, evt);
    });

    function filtrarClases(evt) {
        $("#cboClasesDisponibles", containers.section).empty();
        $('.tipoMedidaLineal', containers.section).hide();
        if (!evt) {
            return Promise.resolve();
        }
        return new Promise((ok) => {
            $.post(BASE_URL + 'DeclaracionesJuradas/GetClasesParcelasBySuperficie', { superficie: evt.target.value }, function (clases) {
                console.log(clases);
                const items = clases.reduce((accum, cl) => accum + `<option value='${cl.Value}'>${cl.Value} - ${cl.Text}</option>`, "");
                $("#cboClasesDisponibles", containers.section).append("<option value='0'>- Seleccione -</option>");
                $("#cboClasesDisponibles", containers.section).append(items);
                $("#Croquis_div").addClass("hidden");
            }).done(ok);
        });
    }

    function isValid() {
        // Indico que el formulario fue validado.
        validated = true;
        // Recorremos y buscamos por lógica que falten valores de metros en algun tipo medida lineal.
        var isValid = !clasesSeleccionadas.some(cls => !cls.IsDeleted && cls.TiposMedidasLineales.some(ml => ml.RequiereLongitud && isNaN(parseInt(ml.ValorMetros)) || ml.RequiereAforo && isNaN(parseInt(ml.ValorAforo))));
        // Actulizamos los colores según validación. (Solo es para esto).
        refreshTiposLineales();
        return isValid;
    }

    function validate() {
        return isValid() && validateClase() && validateAforo() && validateSuperficie();
    }

    function buscadorGenericoPorCallesShow() {
        var idLocalidad = getIdLocalidad();

        buscar('calles', 'Buscar Calles', idLocalidad)
            .then(function (seleccion) {
                if (seleccion.length) {
                    calleSelected(seleccion);
                } else {
                    _controller.mostrarAdvertencia('Buscar Calles', 'No se ha seleccionado ninguna calle.');
                    return;
                }
            })
            .catch(function (err) {
                console.log(err);
            });
    }
    function calleSelected(_calle) {
        $('#aforoCalle', modalAforo).val(_calle[0]);
        $('#aforoCalleIdVia', modalAforo).val(_calle[1]);
        return;
    }
    function getIdLocalidad() {
        return $("#DDJJDesignacion_IdLocalidad", containers.formularioDDJJ).val();
    }
    function cerrarBusquedaCalleAforo() {
        $(modalAforo).modal('hide');
    }
    function aplicarAforo() {

        var idLocalidad = getIdLocalidad();
        var calle = $("#aforoCalle", modalAforo).val();
        var idVia = $("#aforoCalleIdVia", modalAforo).val();
        var altura = $("#aforoAltura", modalAforo).val();

        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/BuscarAforo?idLocalidad=" + idLocalidad + "&calle=" + calle + "&idVia=" + idVia + "&altura=" + altura,
            dataType: 'json',
            type: 'GET',
            success: function (data) {
                updateTipoMedidaLineal(idTMLSelected, data);
                refreshTiposLineales();
                cerrarBusquedaCalleAforo();
            },
            error: function (error) {
                _controller.mostrarError("Obtención de aforo para medida lineal", error.responseText);
            }
        });

    }
    function buscarAforoServer(el) {

        var idLocalidad = getIdLocalidad();
        var calle = $("#aforoCalle", modalAforo).val();
        var idVia = $("#aforoCalleIdVia", modalAforo).val();
        var altura = $("#aforoAltura", modalAforo).val();

        $.ajax({
            url: BASE_URL + "DeclaracionesJuradas/BuscarAforo?idLocalidad=" + idLocalidad + "&calle=" + calle + "&idVia=" + idVia + "&altura=" + altura,
            dataType: 'json',
            type: 'GET',
            async: false,
            success: function (data) {
                el = updateDataAforo(el, data);
            },
            error: function (error) {
                _controller.mostrarError("Obtención de aforo para medida lineal", error.responseText);
            }
        });

        return el;

    }
    function updateDataAforo(el, aforo) {

        // Si viene el aforo lo actualizo a la clase actual.
        if (aforo) {
            el.IdTramoVia = aforo.IdTramoVia;
            el.IdVia = aforo.IdVia;
            el.Altura = aforo.Altura;
            el.Calle = aforo.Calle;
            el.Desde = aforo.Desde;
            el.Hasta = aforo.Hasta;
            el.Paridad = aforo.Paridad;
            el.ValorAforo = typeof aforo.ValorAforo === 'number' ? aforo.ValorAforo.toFixed(2) : aforo.ValorAforo;
        }

        return el;
    }
    function openBuscarAforo(medidaLineal) {

        if (medidaLineal.autoSetDireccion && (!medidaLineal.Calle || !medidaLineal.Altura)) {

            $('#aforoCalle', modalAforo).val($('#DDJJDesignacion_Calle').val());
            $("#aforoAltura", modalAforo).val($('#DDJJDesignacion_Numero').val());

        }

        $(modalAforo).modal('show');

        return;

    }
    function init(controller) {

        _controller = controller;

        getUsuarioInterno();

        //Rubro 1
        $('#AguaCorriente', containers.section).change(function () {
            $('#DDJJU_AguaCorriente', containers.section).val(this.checked ? 1 : 0);
        });
        $('#Cloaca', containers.section).change(function () {
            $('#DDJJU_Cloaca', containers.section).val(this.checked ? 1 : 0);
        });
        $('#DDJJU_SuperficiePlano', containers.section).on('input', function () {
            $("#Rubro2", containers.section).data("bootstrapValidator")
                .updateStatus("DDJJU.SuperficieTitulo", "NOT_VALIDATED", null)
                .validateField("DDJJU.SuperficieTitulo");
        });
        $('#DDJJU_SuperficieTitulo', containers.section).on('input', function () {
            $("#Rubro2", containers.section).data("bootstrapValidator")
                .updateStatus("DDJJU.SuperficiePlano", "NOT_VALIDATED", null)
                .validateField("DDJJU.SuperficiePlano");
        });

        // Seleccionamos la el idVia seleccionado en rubro 1 inc. b (Este es el id calle por default)
        idViaCalle = $("#DDJJDesignacion_IdCalle", containers.formularioDDJJ).val();

        //$("#btnCerrarSeleccionClase", modalClases).off("click").on("click", closeSeleccionClase);
        //$("#btnSeleccionClase", modalClases).off("click").on("click", seleccionClase);
        $("#cboClasesDisponibles", containers.section).off("change").on("change", onChangeClase);
        $("#btnCerrarBusquedaCalleAforo", modalAforo).off("click").on("click", cerrarBusquedaCalleAforo);
        $("#btnBuscarCalles", modalAforo).off("click").on("click", buscadorGenericoPorCallesShow);
        $("#btnAplicarAforo", modalAforo).off("click").on("click", aplicarAforo);

        $("#Rubro2", containers.section).bootstrapValidator({
            framework: "boostrap",
            excluded: [":disabled"],
            fields: {
                requiredInteger: {
                    selector: '.requiredInteger',
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio'
                        },
                        integer: {
                            message: 'El campo debe ser un número entero'
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        }
                    }
                },
                'DDJJU.SuperficiePlano': {
                    validators: {
                        numeric: {
                            message: 'El campo debe ser un número'
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                        callback: {
                            message: 'Si no indica una Superficie de Título debe especificar una Superficie de Plano y si indica una Superficie de Plano, debe completarla con un valor mayor a cero',
                            callback: function (value) {
                                return !isNaN(value) && Number(value) > 0 || !value && Number(!!$('#DDJJU_SuperficieTitulo', containers.section).val()) > 0;
                            }
                        }
                    }
                },
                'DDJJU.SuperficieTitulo': {
                    validators: {
                        numeric: {
                            message: 'El campo debe ser un número'
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                        callback: {
                            message: 'Si indica una Superficie de Título, debe completarla con un valor mayor a cero',
                            callback: function (value) {
                                return !isNaN(value) && Number(value) > 0 || !value && Number(!!$('#DDJJU_SuperficiePlano', containers.section).val()) > 0;
                            }
                        }
                    }
                },
                'DDJJU.NumeroHabitantes': {
                    validators: {
                        integer: {
                            message: 'El campo debe ser un número entero'
                        },
                        stringLength: {
                            max: 19,
                            message: 'El campo no debe superar los 19 caracteres'
                        },
                        callback: {
                            message: 'Número de habitantes es obligatorio',
                            callback: function (value) {
                                var caracteristica = _.find($.parseJSON($('#uOtrasCarJSON', containers.section).val()), function (e) { return e.OtrasCarRequerida.toUpperCase() === 'NUMERO DE HABITANTES'; });
                                if (caracteristica) {
                                    return caracteristica.Requerido === 1 && value !== '' || !caracteristica.Requerido;
                                }
                                return true;
                            }
                        }
                    }
                }
            }
        });

        // fix para que clase parcela tenga id
        for (var i = 0; i < clasesSeleccionadas.length; i++) {
            clasesSeleccionadas[i].Descripcion = `${clasesSeleccionadas[i].IdClaseParcela} - ${clasesSeleccionadas[i].Descripcion}`;
        }




        $(".buscarAforo", containers.section).click(function () {

            idTMLSelected = $(this).data('tml');

            medidaLineal = getTipoMedidaLineal(idTMLSelected);

            $("#aforoCalle", modalAforo).val(medidaLineal.Calle);
            $("#aforoAltura", modalAforo).val(medidaLineal.Altura);

            openBuscarAforo(medidaLineal);

            return;
        });




        $('.metrosInput', containers.section)
            .keypress(onlyNumberKeyPress)
            .blur(updateBlurTipoMedidaLineal);

        $('.aforoInput', containers.section)
            .keypress(onlyNumberKeyPress)
            .blur(updateBlurAforo);


        // Refrescamo dependiendo de la row seleccionada.
        refreshTiposLineales();

        // Inicializamos esto solo cuando es nuevo.
        if (!clasesSeleccionadas) {
            clasesSeleccionadas = [];
        }

        // Almacenamos el objeto serializado de entrada.
        serialize();

        $("#file", containers.section).change(function () {
            readURL(this);
        });
        //Borrado de imagen
        $(".u-rubro2-borrar", containers.section).click(function () {
            $("#file", containers.section).val("");
            $('#imageDisplay', containers.section).attr('src', "");
            $('#CroquisBase64', containers.section).val("");
            $('#imageDisplay', containers.section).hide();
            $('#DDJJU_Croquis', containers.section).val("");
        });
    }

    return {
        init: init,
        validate: validate
    };
};
//# sourceURL=rubro2.js