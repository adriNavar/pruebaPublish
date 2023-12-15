var FormularioDDJJController = function (modalContainer, validators, ddjjController) {
    let currentStep = 1;
    const minStep = 1,
        readOnly = $("#ReadOnly", modalContainer).val().toLowerCase() === "true",
        mensajesCtrl = {
            mostrarAdvertencia: (titulo, mensaje, cb) => ddjjController.mostrarMensajeError(mensaje, titulo, false, cb),
            mostrarError: (titulo, mensaje) => ddjjController.mostrarMensajeError(mensaje, titulo, true),
            mostrarOk: (titulo, mensaje) => ddjjController.mostrarMensajeGeneral(mensaje, titulo, false)
        };

    function ajustarmodal() {
        var altura = $(window).height() - 210;
        $("#formulario-body", modalContainer).css({ "height": altura });
        $("#formulario-content", modalContainer).css({ "height": altura, "overflow": "hidden" });
        ajustarScrollBars();
    }
    function ajustarScrollBars() {
        $('#formulario-content', modalContainer).getNiceScroll().resize();
        $('#formulario-content', modalContainer).getNiceScroll().show();
    }
    function isCurrentViewValid() {
        return readOnly || validators[currentStep - 1].validate();
    }
    function onSuccess() {
        $(modalContainer).modal("hide");
        mensajesCtrl.mostrarOk("Declaración Jurada - Guardar", 'La declaración jurada se ha guardado correctamente');
        ddjjController.reloadData();
        ddjjController.informarCambio();
        return true;
    }
    function onFailure() {
        mensajesCtrl.mostrarError("Declaración Jurada - Guardar", "Ha ocurrido un error al guardar la declaración jurada");
    }
    function init() {
        const form = $("form", modalContainer);
        form.ajaxForm({
            beforeSubmit: showLoading,
            success: onSuccess,
            error: onFailure,
            complete: hideLoading
        });
        form.submit((evt) => {
            /*evita el doble submit*/
            evt.preventDefault();
            evt.stopImmediatePropagation();
            return false;
        });
        for (let validator in validators) {
            validators[validator].init({ ajustarScrollBars: ajustarScrollBars, ...mensajesCtrl }, readOnly);
        }
        $(window).on("resize", ajustarmodal);
        $('#btn-atras', modalContainer).off("click").on('click', function () {
            if (isCurrentViewValid()) {
                $('#stepWiz' + currentStep, modalContainer).addClass('disabled');
                $('#stepWiz' + currentStep, modalContainer).removeClass('active');
                $('#stepWiz' + currentStep, modalContainer).removeClass('complete');

                if (currentStep > minStep) {
                    currentStep--;
                    $('#step' + currentStep, modalContainer).removeAttr('disabled').trigger('click');
                    $('#stepWiz' + currentStep, modalContainer).addClass('active');
                }

                $('#btn-siguiente', modalContainer).removeClass('disabled');

                if (currentStep === minStep) {
                    $('#btn-atras', modalContainer).addClass('disabled');
                }

                setTimeout(ajustarScrollBars, 10);
            }
        });
        $('#btn-siguiente', modalContainer).off("click").on('click', function () {

            if (isCurrentViewValid()) {
                const maxStep = $(".body-content .stepwizard-step", modalContainer).length;

                $('#stepWiz' + currentStep, modalContainer).addClass('complete');
                $('#stepWiz' + currentStep, modalContainer).removeClass('active');

                if (currentStep < maxStep) {
                    currentStep++;
                    $('#step' + currentStep, modalContainer).removeAttr('disabled').trigger('click');
                }
                $('#stepWiz' + currentStep, modalContainer).removeClass('disabled');
                $('#stepWiz' + currentStep, modalContainer).addClass('active');
                $('#btn-atras', modalContainer).removeClass('disabled');

                if (currentStep === maxStep) {
                    $('#btn-siguiente', modalContainer).addClass('disabled');
                }

                setTimeout(ajustarScrollBars, 10);
            }
        });

        const navListItems = $('div.stepwizard-step > a', modalContainer), // tab nav items
            allWells = $('.setup-content', modalContainer); // content div

        allWells.hide(); // hide all contents by default
        navListItems.off("click").on("click", function (e) {
            e.preventDefault();
            const $target = $($(this).attr('href')),
                $item = $(this);

            if (!$item.hasClass('disabled')) {
                navListItems.removeClass('btn-primary').addClass('btn-default');
                $item.addClass('btn-primary');
                allWells.hide();
                $target.show();
                $target.find('input:eq(0)').focus();
            }
        });

        $(".body-content", modalContainer).niceScroll(getNiceScrollConfig());
        $('#step1', modalContainer).trigger('click');

        $('#btnCloseFormDDJJ', modalContainer).off("click").on('click', function () {
            if ($('#ReadOnly', modalContainer).val() === "true") {
                $(modalContainer).modal("hide");
            } else {
                mensajesCtrl.mostrarAdvertencia("Declaración Jurada", "Si sale del formulario perderá los datos no guardados, ¿desea continuar?", function () {
                    $(modalContainer).modal("hide");
                });
            }
        });

        if (readOnly) {
            $('.body-content', modalContainer).find('input, select, textarea').attr('disabled', 'disabled');
            $('.body-content', modalContainer).find('span').addClass('disabled');
        }

        $('#btnSave', modalContainer).off("click").on('click', function () {
            if (_.every(validators, function (v) {
                return v.validate();
            })) {
                
                if ($("#hdnTemporal", modalContainer).length) {
                    const datos = form.serializeArray();
                    $(window).trigger({
                        type: 'ddjjTemporalGuardada',
                        formulario: {
                            ddjj: datos,
                            tipo: Number($("#DDJJ_IdVersion", modalContainer).val())
                        }
                    });
                    $(modalContainer).modal("hide");
                } else {
                    form.submit();
                }
            }
        });

        $(modalContainer)
            .one("hide.bs.modal", () => {
                $(window).off("resize", ajustarmodal);
            }).one('shown.bs.modal', () => {
                hideLoading();
                ajustarmodal();
            }).modal("show");
    }
    return {
        init: init,
        onSuccess: onSuccess,
        onFailure: onFailure
    };
};