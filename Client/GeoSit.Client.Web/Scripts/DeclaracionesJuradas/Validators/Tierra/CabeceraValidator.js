var CabeceraFormUSoRValidator = function (containers) {
    const form = $("#Cabecera", containers.section);
    return {
        validate: function () {
            var bootstrapValidator = form.data("bootstrapValidator");
            bootstrapValidator.validate();
            return bootstrapValidator.isValid();
        },
        init: function () {
            if (!$("#DDJJDesignacion_IdDepartamento", container).val()) {
                $("#DDJJDesignacion_IdDepartamento", containers.section).change(function () {
                    $("#DDJJDesignacion_Departamento", containers.section).val($("#DDJJDesignacion_IdDepartamento :selected", containers.section).text());
                    $.get(BASE_URL + "DeclaracionesJuradas/GetLocalidades", { idDepartamento: $("#DDJJDesignacion_IdDepartamento :selected", containers.section).val() },
                        function (data) {
                            $("#DDJJDesignacion_IdLocalidad", containers.section).empty();
                            $("#DDJJDesignacion_IdLocalidad", containers.section).append("<option value>- Seleccione -</option>");
                            data.forEach(row => $("#DDJJDesignacion_IdLocalidad", containers.section).append(`<option value="${row.Value}">${row.Text}</option>`));
                        });
                });
            }
            if (!$("#DDJJDesignacion_IdLocalidad", container).val()) {
                $("#DDJJDesignacion_IdLocalidad", containers.section).change(function () {
                    $("#DDJJDesignacion_Localidad", containers.section).val($("#DDJJDesignacion_IdLocalidad :selected", containers.section).text());
                });
            }

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
                    'DDJJDesignacion.IdDepartamento': {
                        excluded:'false',
                        validators: {
                            notEmpty: {
                                message: "El campo Departamento es obligatorio"
                            }
                        }
                    },
                    'DDJJDesignacion.IdLocalidad': {
                        excluded: 'false',
                        validators: {
                            notEmpty: {
                                message: "El campo Localidad es obligatorio"
                            }
                        }
                    }
                }
            });
        }
    };
};