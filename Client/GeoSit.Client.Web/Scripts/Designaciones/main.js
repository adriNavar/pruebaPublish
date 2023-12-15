$(function () {
    $(document).ready(init);
    var modal = "#modal-window-designacion";
    function init() {
        $("[data-solr]", modal).on("click", buscarObjetos);
        $("#IdDepartamento", modal).on("change", departamentoChanged);
        $("#IdLocalidad", modal).on("change", localidadChanged);
        $("select", modal).on("change", function (evt) {
            $(evt.currentTarget).siblings("[type='hidden']").val(evt.currentTarget.selectedOptions[0].text);
        });
        var message = $("#message-error", modal);
        $(".close", message).click(function () {
            message.hide();
        });

        $(modal)
            .one("hidden.bs.modal", function () {
                $(document).off("designacionGuardada");
            })
            .one("shown.bs.modal", hideLoading)
            .modal("show");

        $("#btnGuardarDesignacion").oneClick(function () {
            showLoading();
            var fd = new FormData(document.querySelector("#form-designacion"));
            var designacion = Array.from(fd.keys()).reduce((accum, key) => ({ ...accum, [key]: fd.get(key) }), {});
            $.ajax({
                url: BASE_URL + "Designaciones/ValidarTipoDesignador",
                type: 'POST',
                data: { designacion: designacion },
                success: function (designacion) {
                    if (designacion.error) {
                        errorMessage(`Ya existe otra designación con el designador <strong>${designacion.designador}</strong>`);
                    } else {
                        $(document).trigger({ type: 'designacionGuardada', designacion: designacion });
                        $(modal).modal('hide');
                    }
                },
                error: function () {
                    errorMessage("Ha ocurrido un error al validar la designación.");
                },
                complete: hideLoading
            });
        });
    }
    function buscarObjetos(evt) {
        new Promise(function (resolve) {
            var padre = $("#" + $(evt.currentTarget).data("parent"));
            var includeSearch = $(evt.currentTarget).is("[data-include-search]");
            if (!padre.val() || padre.val() === "0") {
                errorMessage("No se ha seleccionado " + $("label", padre.parents(".form-group")).text() + ".");
                return;
            }
            var data = { tipos: $(evt.currentTarget).data("solr"), multiSelect: false, verAgregar: false, titulo: 'Buscar ' + $(evt.currentTarget).data("titulo"), campos: ['Nombre'], filters: ["idpadre=" + padre.val()], includeSearch: includeSearch };
            $("#buscador-container").load(BASE_URL + "BuscadorGenerico", data, function () {
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
        }).then(function (seleccion) {
            var ig = $(evt.currentTarget).parents(".input-group");
            $("input[type='hidden']", ig).val(seleccion[1]);
            $("input[type='text']", ig).val(seleccion[0]);
        });
    }
    function departamentoChanged(evt) {
        Array.from($("input[data-listen-departamento-changed]", modal)).forEach(function (input) {
            $(input).val("");
            $(input).siblings("[type='hidden']").val("");
        });
        Array.from($("select[data-listen-departamento-changed]", modal)).forEach(function (select) {
            var tipo = $(select).data("listenDepartamentoChanged");
            $.ajax({
                url: BASE_URL + "Designaciones/CargarObjetos",
                method: "GET",
                data: { idPadre: evt.currentTarget.value, tipo: tipo },
                dataType: "json",
                success: function (options) {
                    $(select).empty();
                    $(select).html(options.reduce(function (accum, opt) {
                        return accum + "<option value='" + opt.Value + "'>" + opt.Text + "</option>";
                    }, ""));
                    localidadChanged();
                },
                error: function (_, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                }
            });
        });
    }
    function localidadChanged() {
        Array.from($("input[data-listen-localidad-changed]", modal)).forEach(function (input) {
            $(input).val("");
            $(input).siblings("[type='hidden']").val("");
        });
    }
    function errorMessage(text) {
        var message = $("#message-error");
        message.find("p").html(text);
        $("#message-error").fadeIn("slow").delay(5000).queue(function () {
            $("#message-error").hide().dequeue();
        });
    }
});
//# sourceURL=designacion.js