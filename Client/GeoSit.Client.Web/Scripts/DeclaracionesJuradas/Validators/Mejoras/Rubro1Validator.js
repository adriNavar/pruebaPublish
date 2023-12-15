var Rubro1FormE1E2Validator = function (containers) {
    const form = $("#Rubro1", containers.section);

    function validateCaracteristicas() {
        debugger;
        var errorMessageCaracteristicas = false;

        var cantidadCaracteristica = document.querySelectorAll(".active input[type='checkbox']").length;

        if (cantidadCaracteristica === 0) {
            errorMessageCaracteristicas = true;
        }

        if (errorMessageCaracteristicas) {
            declaracionesJuradas.mostrarMensajeGeneral("Debe tener cargada al menos una característica", "Formulario E1", true);
        }

        return !errorMessageCaracteristicas;
    }

    function configure() {
        // Verificamos si tiene valor cuando se haya cargado el formulario.
        $('.featuresDrop').each(function (_, e) {
            if ($(e).val()) {
                $("button", $(this).siblings(".btn-group")).addClass("select-with-value");
            }
        });

        // Seteamos comportamiento para cuando el usuario cambie el valor.
        $('.featuresDrop').on('change', function (evt) {
            const botones = $("button", $(this).siblings(".btn-group"));
            if (evt.currentTarget.value) {
                botones.addClass("select-with-value");
            } else {
                botones.removeClass("select-with-value");
            }
        });

    }
    return {
        validate: function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();
            return bootstrapValidator.isValid() && validateCaracteristicas();
        },
        init: function () {

            form.bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {

                }
            });

            const opts = {
                buttonText: function (options) {
                    if (options.length === 0) {
                        return '- Seleccione -';
                    }
                    else {
                        var labels = [];
                        options.each(function () {
                            if ($(this).attr('label') !== undefined) {
                                labels.push($(this).attr('label'));
                            }
                            else {
                                labels.push($(this).html());
                            }
                        });
                        var numeroLabel = [];

                        for (var label in labels) {
                            var labelCaracteristicas = labels[label].toString();
                            var labelIndex = labelCaracteristicas.indexOf(" ");

                            numeroLabel.push(labelCaracteristicas.substring(0, labelIndex));
                        }


                        return numeroLabel.join(', ') + '';
                    }
                },
                buttonWidth: '120px'
            };

            $("select[multiple]:not(.dropRight):not(.dropUp)").multiselect(opts);
            $("select[multiple].dropRight:not(.dropUp)").multiselect({ ...opts, ...{ "dropRight": true } });
            $("select[multiple].dropUp:not(.dropRight)").multiselect({ ...opts, ...{ "maxHeight": 750, "dropUp": true } });
            $("select[multiple].dropRight.dropUp").multiselect({ ...opts, ...{ "dropRight": true, "maxHeight": 750, "dropUp": true } });

            configure();
        }
    };
};

//# sourceURL=Rubro1.js