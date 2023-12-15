var formularioE1 = {
    currentStep: 1,
    maxStep: $('.bs-wizard-step').length,
    minStep: 1,
    validators: [cabecera, rubro1, rubro2],
    ajustarmodal: function () {
        var altura = $(window).height() - 210;
        $(".formularioe1-body").css({ "height": altura });
        $(".formularioe1-content").css({ "height": altura, "overflow": "hidden" });
        formularioE1.ajustarScrollBars();
    },
    ajustarScrollBars: function () {
        $('#formularioe1-content').getNiceScroll().resize();
        $('#formularioe1-content').getNiceScroll().show();
    },
    isCurrentViewValid: function () {
        return formularioE1.validators[formularioE1.currentStep - 1].validate();
    },
    onSuccess: function (data) {
        hideLoading();
        if (data.success) {
            declaracionesJuradas.mostrarMensajeGeneral('El formulario se ha guardado con éxito', 'Guardar Formulario', false, null);
            $('#modalFormulario').modal('hide');
            declaracionesJuradas.getDeclaracionesJuradas();
            declaracionesJuradas.getDeclaracionesJuradasHeader();
            declaracionesJuradas.informarCambio();
            return true;
        }
        else {
            declaracionesJuradas.mostrarMensajeError('Ha ocurrido un error al guardar el formulario: ' + (data.message === undefined ? 'Error Interno' : data.message), 'Guardar Formulario', false, null);
            return false;
        }
    },
    onFailure: function (xhr) {
        hideLoading();
        response = xhr.responseJSON;
        window.location.href = response.RedirectUrl;
    },
    init: function () {
        $(window).resize(formularioE1.ajustarmodal);

        $('#btn-atras').on('click', function () {

            if (formularioE1.isCurrentViewValid()) {

                $('#stepWiz' + formularioE1.currentStep).addClass('disabled');
                $('#stepWiz' + formularioE1.currentStep).removeClass('active');
                $('#stepWiz' + formularioE1.currentStep).removeClass('complete');


                if (formularioE1.currentStep > formularioE1.minStep) {
                    formularioE1.currentStep--;
                    $('#step' + formularioE1.currentStep).removeAttr('disabled').trigger('click');
                    $('#stepWiz' + formularioE1.currentStep).addClass('active');
                }

                $('#btn-siguiente').removeClass('disabled');

                if (formularioE1.currentStep === formularioE1.minStep) {
                    $('#btn-atras').addClass('disabled');
                }
                setTimeout(formularioE1.ajustarScrollBars, 10);
            }

        });
        $('#btn-siguiente').on('click', function () {

            if (formularioE1.isCurrentViewValid()) {

                $('#stepWiz' + formularioE1.currentStep).addClass('complete');
                $('#stepWiz' + formularioE1.currentStep).removeClass('active')

                if (formularioE1.currentStep < formularioE1.maxStep) {
                    formularioE1.currentStep++;
                    $('#step' + formularioE1.currentStep).removeAttr('disabled').trigger('click');
                }

                $('#stepWiz' + formularioE1.currentStep).removeClass('disabled')
                $('#stepWiz' + formularioE1.currentStep).addClass('active')

                $('#btn-atras').removeClass('disabled');

                if (formularioE1.currentStep === formularioE1.maxStep) {
                    $('#btn-siguiente').addClass('disabled');
                }

                setTimeout(formularioE1.ajustarScrollBars, 10);
            }
        });

        var navListItems = $('div.setup-panel div a'), // tab nav items
            allWells = $('.setup-content'); // content div

        allWells.hide(); // hide all contents by default

        navListItems.click(function (e) {
            e.preventDefault();
            var $target = $($(this).attr('href')),
                $item = $(this);

            if (!$item.hasClass('disabled')) {
                navListItems.removeClass('btn-primary').addClass('btn-default');
                $item.addClass('btn-primary');
                allWells.hide();
                $target.show();
                $target.find('input:eq(0)').focus();
            }
        });

        $('#modalFormulario').one('shown.bs.modal', function () {
            formularioE1.ajustarmodal();
        });

        $("#formularioe1-content").niceScroll(getNiceScrollConfig());
        $('#step1').trigger('click');

        $('.closeFormularioE1').on('click', function () {
            if ($('#formularioe1-content #ReadOnly').val() == "true") {
                $('#modalFormulario').modal("hide");
            }
            else {
                declaracionesJuradas.mostrarMensajeGeneral("Si sale del formulario perderá los datos no guardados, ¿desea continuar?", "Formulario E1", true, function () {
                    $('#modalFormulario').modal("hide");
                });
            }
        });

        if ($('#formularioe1-content #ReadOnly').val() === "true") {
            $(function () {
                $('#formularioe1-content').find('input, select, textarea').attr('disabled', 'disabled');
                $('#formularioe1-content').find('span').addClass('disabled');
            });
        }

        $('#btnSave').on('click', function (e) {
            if (_.every(formularioE1.validators, function (v) {
                return v.validate();
            })) {
                showLoading();
                $('#formFormularioE1').submit();
            }
            else {
                declaracionesJuradas.mostrarMensajeError('Por favor, complete los campos obligatorios.', 'Guardar Formulario', false, null);
            }
        });
    }
};
