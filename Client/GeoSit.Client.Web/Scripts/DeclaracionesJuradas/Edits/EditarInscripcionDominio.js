var EditarDominio = function () {
    const modalContainer = "#modalEditarDominio";
    function ajustarmodal() {
        var altura = $(window).height() - 210;
        $(".dominio-body", modalContainer).css({ "max-height": altura });
        $(".dominio-content", modalContainer).css({ "max-height": altura, "overflow": "hidden" });
        setTimeout(ajustarScrollBars, 100);
    }
    function ajustarScrollBars() {
        $('#dominio-content', modalContainer).getNiceScroll().resize();
        $('#dominio-content', modalContainer).getNiceScroll().show();
    }
    return {
        init: function () {
            $('#IdTipoInscripcion', modalContainer).val($('#IdTipoInscripcionValue').val());
            $('.tooltips', modalContainer).tooltip({ container: 'body' });

            $(window).resize(ajustarmodal);
            $("#dominio-content", modalContainer).niceScroll(getNiceScrollConfig());

            $("#ddjj-dominio-form", modalContainer).bootstrapValidator({
                framework: "boostrap",
                excluded: [":disabled"],
                fields: {
                    Inscripcion: {
                        validators: {
                            callback: {
                                message: "El campo Inscripción no cumple con el formato requerido",
                                callback: function (value) {
                                    var regexp = new RegExp($("#IdTipoInscripcion :selected", modalContainer).attr("regexp").replace(/\"/g, ''));
                                    return regexp.test(value);
                                }
                            },
                            notEmpty: {
                                message: "El campo Inscripción es obligatorio"
                            }
                        }
                    },
                    FechaHora: {
                        validators: {
                            notEmpty: {
                                message: "El campo Fecha es obligatorio"
                            }
                        }
                    }
                }
            });

            $("#FechaHora", modalContainer).datepicker(getDatePickerConfig())
                .on("changeDate", function () {
                    $("#ddjj-dominio-form", modalContainer).bootstrapValidator("revalidateField", "FechaHora");
                });

            document.getElementById("labelNumero").style.display = 'none';
            $("#IdTipoInscripcion", modalContainer).change(function () {
                $.get(BASE_URL + "Dominio/GetInscripcionRegexExample?regex=" + btoa($("#IdTipoInscripcion :selected", modalContainer).attr("regexp").replaceAll("\"", "")), function (regex) {
                    $("input[name='Inscripcion']", modalContainer).attr("placeholder", "Formato: " + (regex.ejemplo || "Libre"));
                    document.getElementById("labelNumero").style.display = 'block';
                    if (regex.ejemplo != null) {
                        document.getElementById('labelNumero').innerHTML = regex.message;
                    } else {
                        document.getElementById('labelNumero').innerHTML = "El formato del campo Inscripción debe ser: Libre";
                    }
                });
                $("#Inscripcion", modalContainer).val("");
                var idTipoInscripcion = $("#IdTipoInscripcion", modalContainer).val();
                if (idTipoInscripcion === "5") {
                    $("#Inscripcion", modalContainer).val("0000000000/0000");
                    $("#Inscripcion", modalContainer).attr("readonly", "readonly");
                }
                $("#ddjj-dominio-form", modalContainer).data("formValidation").resetField("Inscripcion");
            });

            $(modalContainer).one("shown.bs.modal", ajustarmodal).modal("show");

            $('#btnGuardarDominio', modalContainer).off("click").on('click', function () {
                var form = $("#ddjj-dominio-form", modalContainer);
                var bootstrapValidator = form.data("bootstrapValidator");
                bootstrapValidator.validate();

                if (bootstrapValidator.isValid()) {
                    $(document).trigger({
                        type: 'dominioGuardado',
                        dominio: {
                            IdDominio: Number($("#IdDominio", form).val()),
                            IdTipoInscripcion: Number($("#IdTipoInscripcion :selected", form).val()),
                            TipoInscripcion: $("#IdTipoInscripcion :selected", form).text(),
                            Inscripcion: $("#Inscripcion", form).val(),
                            Fecha: $("#FechaHora", form).val().toISOString()
                        }
                    });
                    $(modalContainer).modal("hide");

                }
            });
        }
    };
};
//# sourceURL=dominio.js