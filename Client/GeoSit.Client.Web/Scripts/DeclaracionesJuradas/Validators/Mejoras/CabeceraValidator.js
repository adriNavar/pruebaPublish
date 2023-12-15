var CabeceraFormE1E2Validator = function (containers) {
    const form = $("#Cabecera", containers.section);
    return {
        validate: function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();
            return bootstrapValidator.isValid();
        },
        init: function () {
            
            $("#DDJJ_FechaVigencia", containers.section).datepicker(getDatePickerConfig())
                .on("changeDate", function () {
                    $(form).bootstrapValidator("revalidateField", "DDJJ.FechaVigencia");
                });
            form.bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {
                    'DDJJ.IdPoligono': {
                        validators: {
                            stringLength: {
                                max: 19,
                                message: 'El campo no debe superar los 19 caracteres'
                            }
                        }
                    },
                    'DDJJ.FechaVigencia': {
                        validators: {
                            notEmpty: {
                                message: "La Fecha de Vigencia es obligatoria"
                            },
                            remote: {
                                message: 'La fecha de vigencia no puede ser menor a la fecha de vigencia de la valuación actual',
                                type: "POST",
                                url: `${BASE_URL}DeclaracionesJuradas/EsFechaValida`,
                                data: () => ({
                                    IdUnidadTributaria: parseInt($("#DDJJ_IdUnidadTributaria", containers.formularioDDJJ).val()),
                                    IdVersion: parseInt($("#DDJJ_IdVersion", containers.formularioDDJJ).val())
                                })
                            }
                        },
                    },

                    'Mejora.IdDestinoMejora': {
                        validators: {
                            notEmpty: {
                                message: "El campo Destino del edificio es obligatorio"
                            }
                        }
                    }
                }
            });
        }
    };
};