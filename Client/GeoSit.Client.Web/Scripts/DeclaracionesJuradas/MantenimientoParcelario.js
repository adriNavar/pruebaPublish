var mantenimientoParcelario = {    
    ajustarScrollBars: function () {
        $(".selector-body").getNiceScroll().resize();
        $(".selector-body").getNiceScroll().show();
    },
    ajustarmodal: function () {
        var altura = $(window).height() - 190; //value corresponding to the modal heading + footer
        var alturaListado = altura - 50;
        $(".componentes-body").css({ "height": altura });
        $(".selector-body").css({ "height": alturaListado });
        mantenimientoParcelario.ajustarScrollBars();
    },
    mostrarMensaje: function (mensajes, titulo, tipo) {
        $('#TituloInfo', '#ModalInfo').html(titulo);
        $('#DescripcionInfo', '#ModalInfo').html(mensajes.join("<br />"));
        $("[role='alert']", "#ModalInfo").removeClass("alert-danger alert-success alert-info alert-warning").addClass(tipo);
        $(".modal-footer", "#ModalInfo").hide();
        if (tipo === "alert-info") {
            $(".modal-footer", "#ModalInfo").show();
        }
        $("#ModalInfo").modal('show');
    },
    mostrarMensajeError: function (mensajes, titulo, error) {
        mantenimientoParcelario.mostrarMensaje(mensajes, titulo, (error || false ? "alert-danger" : "alert-warning"));
    },
    mostrarMensajeGeneral: function (mensajes, titulo, confirmacion) {
        mantenimientoParcelario.mostrarMensaje(mensajes, titulo, (!!confirmacion ? "alert-info" : "alert-success"));
    },   
    init: function () {
        //$("#ddjj-content").niceScroll(getNiceScrollConfig());

        //$.ajax({
        //    url: $('#urlActionGetDeclaracionesJuradas').data('request-url'),
        //    dataType: 'json',
        //    type: 'GET',
        //    success: function (ddjj) {
        //        $("#Grilla").DataTable({
        //            dom: "<'row'<'col-sm-12'f>><'row remove-margin text-right'<'col-xs-11 remove-padding leyenda'><'col-xs-1 remove-padding switcher'<'row toggle-activos'>>>tr<'row'<'col-sm-12'p>>",
        //            destroy: true,
        //            language: {
        //                url: BASE_URL + "Scripts/dataTables.spanish.txt"
        //            },
        //            order: [1, 'asc'],
        //            data: ddjj,
        //            columns: [
        //                { title: "Tipo", data: "Tipo", orderable: true },
        //                { title: "Vigencia Desde", data: "VigenciaDesde", orderable: true },
        //                { title: "Vigencia Hasta", data: "VigenciaHasta", orderable: true },
        //                { title: "Version", data: "Version", orderable: true },
        //                { title: "Valor", data: "Valor", orderable: true },
        //                { title: "Tramite", data: "Tramite", orderable: true }
        //            ],
        //            createdRow: function (row, data) {
        //                $(row).data('id', data.Tramite);
        //                $(row).addClass('cursor-pointer');
        //            },
        //            initComplete: function (options) {
        //                $(this).dataTable().api().columns.adjust();
        //                $(options.nTBody).off("click", "tr");
        //                $(options.nTBody).on("click", "tr", function (e) {
        //                    e.preventDefault();
        //                    e.stopPropagation();
        //                    $(this).siblings().removeClass('selected');
        //                    $(this).toggleClass('selected');
        //                    declaracionesJuradas.getDatos(this);
        //                });
        //            }
        //        });
        //    },
        //    error: function (error) {
        //        declaracionesJuradas.mostrarMensajeError([error.responseText], "Recuperar DDJJ", true);
        //    }
        //});

        $('#btnDDJJ').on('click', function () {
            $("#ddjj-container").load(BASE_URL + "DeclaracionesJuradas/DeclaracionesJuradas?IdUnidadTributaria=" + $('#UnidadTributariaId').text());         
            $("#modalDDJJ").modal("show");
        });

        $('#btnValuaciones').on('click', function () {
            $("#ddjj-container").load(BASE_URL + "DeclaracionesJuradas/Valuaciones?partidaInmobiliaria=" + $('#partidaInmobiliaria').text());
            $("#modalDDJJ").modal("show");
        });
       

        $('#modal-mantenimiento-parcelario').on('shown.bs.modal', function () {
            //declaracionesJuradasIndex.ajustarmodal();
            hideLoading();
        });


        $("#modal-mantenimiento-parcelario").modal("show");
        $('#collapseParcela').collapse('show')
    },

    openActualizacionDecreto: function () {

        $("#ddjj-container").load(BASE_URL + "DeclaracionesJuradas/ActualizacionDecreto");
        $("#modalDDJJ").modal("show");
        return;
    }
}