var virDetalle = {

    currentRowIndex: -1,
    tramiteId: 0,
    esTemporal: false,
    init: function () {
        $("#fechaConst", "#modalVIRDetalle").datepicker(getDatePickerConfig({ endDate: new Date() }))
        $("#btnGuardar", "#modalVIRDetalle").on('click', GrabaVIR);
        $("#modalVIRDetalle").modal("show");
    }
}

function GrabaVIR() {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'vir/GrabaVIR',
        data: { IdInmueble: $("#txtIdInmueble").val(), Partida: $("#txtPartida").val(), SupCub: $("#txtSupCub").val(), SupSemiCub: $("#txtSupSemiCub").val(), Usos: $("#lstUsos").val(), TipoEdif: $("#lstTipoEdif").val(), FechaConst: $("#fechaConst").val(), Estados: $("#lstEstados").val() },
        success: function () {
            $("#modalVIRDetalle").modal('hide');
        },
        error: function (err) {
            const data = {
                validation: true,
                message: "Ha ocurrido un error al guardar los datos."
            };
            switch (err.status) {
                case 400:
                    data.message = "La fecha de construcción ingresada no es una fecha válida.";
                    break;
                case 409:
                    data.message = "La fecha de construcción ingresada no puede ser mayor a la fecha actual.";
                    break;
                default:
                    data.validation = false;
            }
            $(document).trigger({ type: 'virError', error: data });
        }
    });
}

function CargarTipoPredominante(dato) {
    var uso = $("#lstUsos").val();
    $("#lstTipoEdif").empty();
    if (uso) {
        $.ajax({
            type: 'POST',
            url: BASE_URL + 'vir/GetTipoEdifJson',
            dataType: 'json',
            data: { uso: uso },
            success: function (Objetos) {
                for (var i = 0; i < Objetos.length; i++) {
                    $("#lstTipoEdif").append("<option value='" + Objetos[i].IdTipoEdif + "'>" + Objetos[i].TipoDescripcion + "</option>");
                }
            }, error: function (ex) {
                alert('Error al recuperar los objetos' + ex);
            }
        });
    }
}


