var MantenedorParcelario = function (modules) {
    var mainContainer = "#modal-window-mantenedor",
        scrollableContainer = mainContainer + " .body-content",
        maxKeyValue = {};
    controller = new function () {
        return {
            getNextId: function (arr, key) {
                let max = 0;
                if (maxKeyValue[key]) {
                    max = Math.abs(maxKeyValue[key]);
                } else {
                    for (var elem in arr) {
                        if (arr.hasOwnProperty(elem)) {
                            var id = Math.abs(arr[elem][key]);
                            if (id > max) {
                                max = id;
                            }
                        }
                    }
                }
                maxKeyValue[key] = (max + 1) * -1;
                return maxKeyValue[key];
            },
            mostrarAlerta: function (type) {
                return function (...params) {
                    mostrarAlerta(type, ...params);
                };
            },
            cargarHTML: loadHTML,
            formatoValorMoneda: new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }),
            registrarEvento: function (func) {
                return (function (evento, params) {
                    document.querySelector(mainContainer).addEventListener(evento, func.bind(null, params));
                });
            }
        };
    };

    function init() {
        $(scrollableContainer).niceScroll(getNiceScrollConfig());
        $(window).on("resize", ajustarmodal);
        $("a[data-toggle='collapse']", scrollableContainer).on("click", function () { setTimeout(ajustarScrollBars, 10); });
        $(mainContainer).one('hidden.bs.modal', function () {
            $(window).off("resize", ajustarmodal);
            $(window).off("nueva-valuacion");
            $(window).off("cambio-ddjj");
        });
        $(mainContainer).one('shown.bs.modal', function () {
            document.querySelector(mainContainer).dispatchEvent(new CustomEvent("form-loaded"));
            ajustarmodal();
            hideLoading();
        });
        initTabs();
        initGeneral();
        ajustarmodal();
        $(mainContainer).modal('show');
    }
    function addModule(module) {
        const ctrl = {
            getNextId: controller.getNextId,
            mostrarConfirmacion: controller.mostrarAlerta("warning"),
            mostrarError: controller.mostrarAlerta("error"),
            mostrarOk: controller.mostrarAlerta("success"),
            cargarHTML: controller.cargarHTML,
            formatoValorMoneda: controller.formatoValorMoneda.format,
            registrarAjustarColumnas: controller.registrarEvento(ajustarColumnas)
        };
        module.init(mainContainer, ctrl);
    }
    function ajustarScrollBars() {
        $(scrollableContainer).getNiceScroll().resize();
        $(scrollableContainer).getNiceScroll().show();
    }
    function ajustarmodal() {
        var altura = Math.max($(window).height() - 170, 150),
            cabecera = $(".body-header", mainContainer).outerHeight() + 20; //los 20 es por el margin-top y margin-bottom
        $(".mantenedor-body").css({ height: altura });
        $(scrollableContainer).css({ maxHeight: altura - cabecera });
        ajustarScrollBars();
    }
    function ajustarColumnas(tabla) {
        setTimeout(function () {
            tabla.api().columns.adjust();
            setTimeout(ajustarScrollBars, 10);
        }, 10);
    }
    function initTabs() {
        $('.tooltips', scrollableContainer).tooltip({ container: 'body' });
        $('[data-toggle="tab"]', scrollableContainer).on("click", function () {
            document.querySelector(mainContainer).dispatchEvent(new CustomEvent("tab-changed"));
            setTimeout(ajustarScrollBars, 10);
        });
    }
    function initGeneral() {
        $(".parcela-fecha").datepicker(getDatePickerConfig({ enableOnReadonly: false }));
        document.querySelector(mainContainer).addEventListener("close", function () {
            
            $(mainContainer).modal('hide');
        });
        $("a[data-checked]").on("click", function (evt) {
            var newVal = parseInt($(evt.currentTarget).attr("data-checked")) ^ 1;
            $(evt.currentTarget).attr("data-checked", newVal);
            $(evt.currentTarget).siblings("input[type='hidden']").val(newVal);
        });
    }

    function mostrarAlerta(type, title, message, func) {
        var dlg = $("#mantenedor-parcelario-confirm-modal").off("hidden.bs.modal");
        $("#btnAdvertenciaOK", dlg).off("click");
        $(".modal-footer", dlg).hide();
        if (func && type === "warning") {
            $("#btnAdvertenciaOK", dlg).one("click", func);
            $(".modal-footer", dlg).show();
        } else if (func) {
            dlg.one("hidden.bs.modal", func);
        }
        var mensajeAlerta = $("[role='alert']", dlg);
        mensajeAlerta.removeClass("alert-success alert-warning alert-danger");
        mensajeAlerta.addClass("alert-" + (type === "error" ? "danger" : type));
        $("#TituloAdvertencia", dlg).text(title);
        $("#DescripcionAdvertencia", dlg).html(message instanceof Array ? message.join("<br />") : message);
        dlg.modal("show");
    }
    function loadHTML(html) {
        $("#mantenedor-externo-container").html(html);
        $('.modal', "#mantenedor-externo-container").first().one('hidden.bs.modal', function () {
            $('#mantenedor-externo-container').empty();
        });
    }

    modules.forEach(addModule);

    return {
        init: init
    };
};