var formularioU = {
    currentStep: 1,
    maxStep: $(".bs-wizard-step", "#modalFormularioU").length,
    minStep: 1,
    validators: [cabecera, rubro1IncB, rubro1IncCyD/*, rubro1IncEyF*/, rubro2],
    ajustarmodal: function () {
        var altura = $(window).height() - 210;
        $(".formulariou-body").css({ "height": altura });
        $(".formulariou-content").css({ "height": altura, "overflow": "hidden" });
        formularioU.ajustarScrollBars();
    },
    ajustarScrollBars: function () {
        $('#formulariou-content').getNiceScroll().resize();
        $('#formulariou-content').getNiceScroll().show();
    },
    appendValidation: function (message) {
        $('#formValidations').append('<p>' + message + '</p>');
    },
    isCurrentViewValid: function () {
        return formularioU.validators[formularioU.currentStep - 1].validate();
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
        debugger
        hideLoading();
        response = xhr.responseJSON;
        window.location.href = response.RedirectUrl;
    },
    init: function () {

        $(window).resize(formularioU.ajustarmodal);

        $('#btn-atras', "#modalFormularioU").off("click").on('click', function () {
            if (formularioU.isCurrentViewValid()) {

                $('#stepWiz' + formularioU.currentStep, "#modalFormularioU").addClass('disabled');
                $('#stepWiz' + formularioU.currentStep, "#modalFormularioU").removeClass('active');
                $('#stepWiz' + formularioU.currentStep, "#modalFormularioU").removeClass('complete');


                if (formularioU.currentStep > formularioU.minStep) {
                    formularioU.currentStep--;
                    $('#step' + formularioU.currentStep, "#modalFormularioU").removeAttr('disabled').trigger('click');
                    $('#stepWiz' + formularioU.currentStep, "#modalFormularioU").addClass('active');
                }

                $('#btn-siguiente', "#modalFormularioU").removeClass('disabled');

                if (formularioU.currentStep === formularioU.minStep) {
                    $('#btn-atras', "#modalFormularioU").addClass('disabled');
                }

                setTimeout(formularioU.ajustarScrollBars, 10);
            }
        });
        $('#btn-siguiente', "#modalFormularioU").off("click").on('click', function () {

            if (formularioU.isCurrentViewValid()) {

                $('#stepWiz' + formularioU.currentStep).addClass('complete');
                $('#stepWiz' + formularioU.currentStep).removeClass('active')

                if (formularioU.currentStep < formularioU.maxStep) {
                    formularioU.currentStep++;
                    $('#step' + formularioU.currentStep).removeAttr('disabled').trigger('click');
                }

                $('#stepWiz' + formularioU.currentStep).removeClass('disabled')
                $('#stepWiz' + formularioU.currentStep).addClass('active')

                $('#btn-atras').removeClass('disabled');

                if (formularioU.currentStep === formularioU.maxStep) {
                    $('#btn-siguiente').addClass('disabled');
                }

                setTimeout(formularioU.ajustarScrollBars, 10);
            }
        });

        var navListItems = $('div.setup-panel div a', "#modalFormularioU"), // tab nav items
            allWells = $('.setup-content', "#modalFormularioU"); // content div

        allWells.hide(); // hide all contents by default
        navListItems.off("click").on("click", function (e) {
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
            formularioU.ajustarmodal();
        });
        $("#formulariou-content", "#modalFormularioU").niceScroll(getNiceScrollConfig());
        $('#step1', "#formulariou-content").trigger('click');

        $('.closeFormularioU').off("click").on('click', function () {
            if ($('#formulariou-content #ReadOnly').val() === "true") {
                $('#modalFormulario').modal("hide");
            } else {
                declaracionesJuradas.mostrarMensajeGeneral("Si sale del formulario perderá los datos no guardados, ¿desea continuar?", "Formulario U", true, function () {
                    $('#modalFormulario').modal("hide");
                });
            }
        });

        if ($('#formulariou-content #ReadOnly').val() === "true") {
            $(function () {
                $('#formulariou-content').find('input, select, textarea').attr('disabled', 'disabled');
                $('#formulariou-content').find('span').addClass('disabled');
            });
        }

        $('#btnSave', "#modalFormularioU").off("click").on('click', function () {
            if (_.every(formularioU.validators, function (v) {
                return v.validate();
            })) {
                if ($("#hdnTemporal").length) {
                    const datos = $('#formFormularioU').serializeArray();
                    $(window).trigger({
                        type: 'ddjjTemporalGuardada',
                        formulario: {
                            ddjj: datos,
                            tipo: Number($("#DDJJ_IdVersion", "#formFormularioU").val())
                        }
                    });
                } else {
                    showLoading();
                    $('#formFormularioU').submit();
                }
            }
        });
    }
};

//# sourceURL=FormularioU.js