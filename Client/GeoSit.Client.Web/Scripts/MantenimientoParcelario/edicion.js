var EdicionMantenedorParcelario = function (modules) {
    let __controller, __parentContainerElem, __validations = modules.filter(m => m.validate);
    
    const __customResets = new function ResetController() {
        var __dict = {};
        function watch(obj, cb) {
            var key = obj.prop("id");
            if (Object.keys(__dict).indexOf(key) === -1) {
                __dict[key] = {
                    elem: obj,
                    value: obj.val(),
                    reset: cb
                };
            }
        }
        function rollback() {
            for (var key in __dict) {
                if (__dict[key].elem.val() !== __dict[key].value) {
                    __dict[key].reset();
                }
            }
        }
        function commit() {
            for (var key in __dict) {
                __dict[key].value = __dict[key].elem.val();
            }
        }
        return {
            watch: watch,
            commit: commit,
            rollback: rollback
        };
    };
    function init(container, controller) {
        __controller = controller;
        __parentContainerElem = document.querySelector(container);
        __customResets.watch($("#afecta-ph", __parentContainerElem), function () { $("a[data-checked]", __parentContainerElem).click(); });
        __customResets.watch($("#Superficie", __parentContainerElem), function () { setValSuperficie(this.value); });

        $("#Superficie", __parentContainerElem).on("input", function (evt) { setValSuperficie(evt.currentTarget.value); });

        $("#cboClase", __parentContainerElem).on("change", function (evt) {
            __parentContainerElem.dispatchEvent(new CustomEvent("clase-parcela-changed", { detail: evt.currentTarget.value }));
        });

        $("#edit-all", __parentContainerElem).oneClick(toggleEdicion);
        $("#save-all", __parentContainerElem).oneClick(saveAll);
        $("#cancel-all", __parentContainerElem).oneClick(function () {
            __controller.mostrarConfirmacion("Mantenedor Parcelario - Cancelar Edición", "¿Está seguro que desea cancelar la edición de los datos?<br/>Esta operación no se puede deshacer.", resetAll);
        });
        $(window).on("cambio-ddjj", cambioDDJJ);
    }
    function setValSuperficie(value) {
        $("#superficie-tierra-registrada", __parentContainerElem).text(Number(value).toFixed(2));
    }
    function toggleEdicion() {
        $("[data-edicion]", __parentContainerElem).toggleClass("hidden");
        $("span[data-editable]", __parentContainerElem).toggle();
        $("dl", __parentContainerElem).toggleClass("editing");
        if ($(this).data("edicion")) {
            $("[data-editable='disabled']", __parentContainerElem).removeAttr("disabled");
            $("[data-editable='readonly']", __parentContainerElem).removeAttr("readonly");
            $("dl:not(.has-perma-buttons)", __parentContainerElem).parents(".tabla-sin-botones").addClass("tabla-con-botones").removeClass("tabla-sin-botones");
            __parentContainerElem.dispatchEvent(new CustomEvent("begin-edition"));
            $(window).on("nueva-valuacion", reloadValuacion);
            $(window).off("cambio-ddjj", cambioDDJJ);
        } else {
            $("[data-editable='disabled']", __parentContainerElem).attr("disabled", "disabled");
            $("[data-editable='readonly']", __parentContainerElem).attr("readonly", "readonly");
            $("dl:not(.has-perma-buttons)", __parentContainerElem).parents(".tabla-con-botones").addClass("tabla-sin-botones").removeClass("tabla-con-botones");
            __parentContainerElem.dispatchEvent(new CustomEvent("end-edition"));
            $(window).off("nueva-valuacion", reloadValuacion);
            $(window).on("cambio-ddjj", cambioDDJJ);
        }
        $("[data-editable='clase']", __parentContainerElem).toggleClass("disabled");
    }
    function reloadValuacion() {
        __parentContainerElem.dispatchEvent(new CustomEvent("reload-valuacion"));
    }
    function cambioDDJJ() {
        $(__parentContainerElem).modal("hide");
        setTimeout(loadView, 100, BASE_URL + "MantenimientoParcelario/Reload");
    }
    function saveAll() {
        const errores = __validations.map(m => m.validate()).filter(v => !v.valid);
        if (errores.length) {
            __controller.mostrarError("Mantenedor Parcelario - Validación", [...["No se puede guardar los cambios. Por favor revise lo siguiente:", ""], ...errores.map(v => v.error)].join("<br />"));
        } else {
            var cancelEdit = toggleEdicion.bind(this);
            const datosGernerales = {
                TipoParcelaID: $("#cboTipo").val(),
                ClaseParcelaID: $("#cboClase").val(),
                EstadoParcelaID: $("#cboEstado").val(),
                ExpedienteAlta: $("#ExpedienteAlta").val(),
                FechaAltaExpediente: $("#FechaAltaExpediente").val(),
                ExpedienteBaja: $("#ExpedienteBaja").val(),
                FechaBajaExpediente: $("#FechaBajaExpediente").val(),
                Superficie: $("#Superficie").val() || "0",
                PlanoId: $("#PlanoId").val() || "0",
                Observaciones: $("#observacionesParcela").val(),
                AfectaPH: $("#afecta-ph").siblings("a[data-checked]").data("checked") === 1
            };
            showLoading();
            $.ajax({
                url: BASE_URL + 'MantenimientoParcelario/Save',
                data: datosGernerales,
                type: 'POST',
                success: function (data) {
                    if (data && data.error) {
                        __controller.mostrarError("Mantenedor Parcelario - Guardar", "Ha ocurrido un error al guardar los datos.");
                    } else if (data && data.eliminada) {
                        __controller.mostrarOk("Mantenedor Parcelario - Guardar", "La parcela ha sido eliminada.", function () {
                            __parentContainerElem.dispatchEvent(new CustomEvent("close"));
                        });
                    } else {
                        __controller.mostrarOk("Mantenedor Parcelario - Guardar", "Los datos han sido guardados correctamente.");
                        __customResets.commit();
                        cancelEdit();
                        __parentContainerElem.dispatchEvent(new CustomEvent("reset-tables"));
                    }
                },
                error: function () {
                    __controller.mostrarError("Mantenedor Parcelario - Guardar", "Ha ocurrido un error al guardar los datos.");
                },
                complete: hideLoading
            });
        }
    }
    function resetAll() {
        var cancelEdit = toggleEdicion.bind(this);
        showLoading();
        $.ajax({
            url: BASE_URL + 'MantenimientoParcelario/Reset',
            type: 'POST',
            success: function () {
                __customResets.rollback();
                document.querySelector("#parcela-form", __parentContainerElem).reset();
                cancelEdit();
                __parentContainerElem.dispatchEvent(new CustomEvent("reset-tables"));
            },
            complete: hideLoading
        });
    }
    return {
        init: init
    };
};
//# sourceURL=edicion.js