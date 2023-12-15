$(document).ready(init);
$(window).resize(ajustarmodal);

$('#modal-window-tramite-permisos').on('shown.bs.modal', function (e) {
    $("#Grilla_Tramites_Permisos").dataTable().api().columns.adjust();
})

function init() {
    ///////////////////// Scrollbars ////////////////////////
    $(".permisos-content").niceScroll(getNiceScrollConfig());
    $('.permisos-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".permisos-content").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////
    $("#Grilla_Tramites_Permisos").dataTable({
        "scrollY": "200px",
        "aaSorting": [[0, "asc"]],
        "paging":   false,
        "ordering": false,
        "info": false,
        "searching": false,
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [
            {
                "targets": [ 0 ],
                "visible": false
            }
        ],
        "initComplete": function () {
            $.ajax({
                url: BASE_URL + 'TramiteCertificadoPermisos/GetResults',
                type: 'GET',
                success: function (data) {
                    //var result = JSON.parse(data.Result).response.docs;
                    var jsonResult = JSON.parse(data);
                    for (var i = 0; i < jsonResult.length; i++) {
                        var obj = jsonResult[i];

                        if (obj.estado == true) {
                            //Checkbox OK
                            var checkelement = "<img title='Desactivar' style='cursor:pointer' class=' pull-left escala_img switchPermiso' src='"+BASE_URL +"Content/images/Seguridad/icons/blue/32/activo.png' />"
                        } else {
                            //Checkbox No OK
                            var checkelement = "<img title='Activar' style='cursor:pointer' class=' pull-left escala_img switchPermiso' src='" + BASE_URL + "Content/images/Seguridad/icons/blue/32/no.activo.png' />"
                        }

                        var perm = {
                            "0": obj.id_funcion,
                            "1": obj.descripcion,
                            "2": checkelement
                        }
                        $('#Grilla_Tramites_Permisos').dataTable().api().row.add(perm).draw();
                    }

                    $(".switchPermiso").click(function () {
                        if ($(this).prop("title") == "Desactivar") {
                            $(this).attr('src', BASE_URL + 'Content/images/Seguridad/icons/blue/32/no.activo.png')
                            $(this).prop("title", "Activar");
                            $(this).parent().parent().addClass("selected");
                            ChangeState(false);
                        } else {
                            $(this).attr('src', BASE_URL + 'Content/images/Seguridad/icons/blue/32/activo.png')
                            $(this).prop("title", "Desactivar");
                            $(this).parent().parent().addClass("selected");
                            ChangeState(true);
                        }
                    });
                }, error: function (ex) {
                    alert(ex);
                }
            });
        }
    });

    $("#btnGrabarPermiso").click(function () {
        showLoading();
        $.ajax({
            async: false,
            type: 'GET',
            url: BASE_URL + 'TramiteCertificadoPermisos/Save',
            dataType: 'json',
            success: function (data) {
                if (data == "Ok") {
                    $("#Grilla_Tramites_Permisos").children().children().removeClass("selected");

                    $("#TituloInfoTramite-Permiso").text("Permisos");
                    $("#MensajeInfoTramite-Permiso").text("Se han guardado los nuevos permisos correctamente");
                    //$("#DescripcionInfoTramite-Permiso").text("Se ha guardo los nuevos permisos correctamente");
                    $("#ModalInfoTramite-Permiso").modal("show");

                    $("#btnCancelarAdvertenciaTramite-Permiso").click(function () {
                        $("#ModalInfoTramite-Permiso").modal("hide");
                        $("#modal-window-tramite-permisos").modal("hide");
                        //$("#permisosContainer").html("");
                    })
                }
                else {
                    $("#Grilla_Tramites_Permisos").children().children().removeClass("selected");
                    //Console.log("Error");
                }
            }

        });
        hideLoading();
    })

    $("#modal-window-tramite-permisos").modal("show");
}

function ajustarmodal() {
    var viewportHeight = $(window).height(),
        paddingBottom = 190,
        headerFooter = 220,
        altura = viewportHeight - headerFooter,
        altura = altura - (altura > headerFooter + paddingBottom ? paddingBottom : 0);
    $(".permisos-body", "#scroll-content-permisos").css({ "height": altura, "overflow": "hidden" });
    ajustarScrollBars();
}

function ChangeState(state)
{
    var objeto = $("#Grilla_Tramites_Permisos").dataTable().api().row('.selected').data();

    $.ajax({
        async: false,
        type: 'GET',
        url: BASE_URL + 'TramiteCertificadoPermisos/Change?estado='+state+'&idFuncion='+objeto[0],
        dataType: 'json',
        success: function (data) {
        }
    });

    $("#Grilla_Tramites_Permisos").children().children().removeClass("selected");

}

function columnsAdjust(tableId) {
    $("#" + tableId).dataTable().api().columns.adjust();
}

//@ sourceURL=TramiteSeccionPermiso.js