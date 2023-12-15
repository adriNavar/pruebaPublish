var formularioE2 = {
    currentStep: 1,
    maxStep: $('.bs-wizard-step').length,
    minStep: 1,
    validators: [cabecera, rubro1, rubro2],
    ajustarmodal: function () {
        var altura = $(window).height() - 210;
        $(".formularioe2-body").css({ "height": altura });
        $(".formularioe2-content").css({ "height": altura, "overflow": "hidden" });
        formularioE2.ajustarScrollBars();
    },
    ajustarScrollBars: function () {
        $('#formularioe2-content').getNiceScroll().resize();
        $('#formularioe2-content').getNiceScroll().show();
    },
    isCurrentViewValid: function () {
        return formularioE2.validators[formularioE2.currentStep - 1].validate();
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
        $(window).resize(formularioE2.ajustarmodal);

        $('#btn-atras').on('click', function () {

            if (formularioE2.isCurrentViewValid()) {

                $('#stepWiz' + formularioE2.currentStep).addClass('disabled');
                $('#stepWiz' + formularioE2.currentStep).removeClass('active');
                $('#stepWiz' + formularioE2.currentStep).removeClass('complete');


                if (formularioE2.currentStep > formularioE2.minStep) {
                    formularioE2.currentStep--;
                    $('#step' + formularioE2.currentStep).removeAttr('disabled').trigger('click');
                    $('#stepWiz' + formularioE2.currentStep).addClass('active');
                }

                $('#btn-siguiente').removeClass('disabled');

                if (formularioE2.currentStep === formularioE2.minStep) {
                    $('#btn-atras').addClass('disabled');
                }
                setTimeout(formularioE2.ajustarScrollBars, 10);
            }
        });
        $('#btn-siguiente').on('click', function () {

            if (formularioE2.isCurrentViewValid()) {

                $('#stepWiz' + formularioE2.currentStep).addClass('complete');
                $('#stepWiz' + formularioE2.currentStep).removeClass('active')

                if (formularioE2.currentStep < formularioE2.maxStep) {
                    formularioE2.currentStep++;
                    $('#step' + formularioE2.currentStep).removeAttr('disabled').trigger('click');
                }

                $('#stepWiz' + formularioE2.currentStep).removeClass('disabled')
                $('#stepWiz' + formularioE2.currentStep).addClass('active')

                $('#btn-atras').removeClass('disabled');

                if (formularioE2.currentStep === formularioE2.maxStep) {
                    $('#btn-siguiente').addClass('disabled');
                }

                setTimeout(formularioE2.ajustarScrollBars, 10);
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
            formularioE2.ajustarmodal();
        });
        $("#formularioe2-content").niceScroll(getNiceScrollConfig());
        $('#step1').trigger('click');

        $('.closeFormularioE2').on('click', function () {
            if ($('#formularioe2-content #ReadOnly').val() == "true") {
                $('#modalFormulario').modal("hide");
            } else {
                declaracionesJuradas.mostrarMensajeGeneral("Si sale del formulario perderá los datos no guardados, ¿desea continuar?", "Formulario E2", true, function () {
                    $('#modalFormulario').modal("hide");
                });
            }
        });

        if ($('#formularioe2-content #ReadOnly').val() === "true") {
            $(function () {
                $('#formularioe2-content').find('input, select, textarea').attr('disabled', 'disabled');
                $('#formularioe2-content').find('span').addClass('disabled');
            });
        }

        $('#btnSave').on('click', function (e) {
            if (_.every(formularioE2.validators, function (v) {
                return v.validate();
            })) {
                showLoading();
                $('#formFormularioE2').submit();
            }
            else {
                declaracionesJuradas.mostrarMensajeError('Por favor, complete los campos obligatorios.', 'Guardar Formulario', false, null);
            }
        });
    }
};