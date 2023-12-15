var formularioSoR = {
    currentStep: 1,
    maxStep: $('.bs-wizard-step').length,
    minStep: 1,
    validators: [cabecera, rubro1IncB, rubro1IncCyD, rubro5y6],    
    ajustarmodal: function () {
        var altura = $(window).height() - 210;
        $(".formulariosor-body").css({ "height": altura });
        $(".formulariosor-content").css({ "height": altura, "overflow": "hidden" });
        formularioSoR.ajustarScrollBars();
    },
    ajustarScrollBars: function () {
        $('#formulariosor-content').getNiceScroll().resize();
        $('#formulariosor-content').getNiceScroll().show();
    },
    appendValidation: function (message) {
        $('#formValidations').append('<p>' + message + '</p>');
    },
    isCurrentViewValid: function () {
        return formularioSoR.validators[formularioSoR.currentStep - 1].validate();                 
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
        $(window).resize(formularioSoR.ajustarmodal);

        $('#btn-atras').on('click', function () {
            
            if (formularioSoR.isCurrentViewValid()) {

                $('#stepWiz' + formularioSoR.currentStep).addClass('disabled');
                $('#stepWiz' + formularioSoR.currentStep).removeClass('active');
                $('#stepWiz' + formularioSoR.currentStep).removeClass('complete');


                if (formularioSoR.currentStep > formularioSoR.minStep) {
                    formularioSoR.currentStep--;
                    $('#step' + formularioSoR.currentStep).removeAttr('disabled').trigger('click');
                    $('#stepWiz' + formularioSoR.currentStep).addClass('active');
                }

                $('#btn-siguiente').removeClass('disabled');

                if (formularioSoR.currentStep == formularioSoR.minStep) {
                    $('#btn-atras').addClass('disabled');
                }

                setTimeout(formularioSoR.ajustarScrollBars, 10);
            }

        });
        $('#btn-siguiente').on('click', function () {

            if (formularioSoR.isCurrentViewValid()) {

                $('#stepWiz' + formularioSoR.currentStep).addClass('complete');
                $('#stepWiz' + formularioSoR.currentStep).removeClass('active')

                if (formularioSoR.currentStep < formularioSoR.maxStep) {
                    formularioSoR.currentStep++;
                    $('#step' + formularioSoR.currentStep).removeAttr('disabled').trigger('click');
                }

                $('#stepWiz' + formularioSoR.currentStep).removeClass('disabled')
                $('#stepWiz' + formularioSoR.currentStep).addClass('active')

                $('#btn-atras').removeClass('disabled');

                if (formularioSoR.currentStep === formularioSoR.maxStep) {
                    $('#btn-siguiente').addClass('disabled');
                }

                setTimeout(formularioSoR.ajustarScrollBars, 10);
            }
        });

        var navListItems = $('div.setup-panel div a'), // tab nav items
            allWells = $('.setup-content'); // content div

        allWells.hide(); // hide all contents by defauld
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
            formularioSoR.ajustarmodal();            
        });
        $("#formulariosor-content").niceScroll(getNiceScrollConfig());
        $('#step1').trigger('click');

        $('.closeFormularioSoR').on('click', function () {
            if ($('#formulariosor-content #ReadOnly').val() == "true") {
                $('#modalFormulario').modal("hide");
            }
            else {
                declaracionesJuradas.mostrarMensajeGeneral("Si sale del formulario perderá los datos no guardados, ¿desea continuar?", "Formulario SoR", true, function () {
                    $('#modalFormulario').modal("hide");
                });
            }
        });

        if ($('#formulariosor-content #ReadOnly').val() === "true") {
            $(function () {
                $('#formulariosor-content').find('input, select, textarea').attr('disabled', 'disabled');
                $('#formulariosor-content').find('span').addClass('disabled');
            });
        }

        $('#btnSave').on('click', function (e) {
            if (_.every(formularioSoR.validators, function (v) {
                return v.validate();
            })) {
                showLoading();
                $('#formFormularioSoR').submit();
            }
            else {
                declaracionesJuradas.mostrarMensajeError('Por favor, complete los campos obligatorios.', 'Guardar Formulario', false, null);
            }
        });
    }
}