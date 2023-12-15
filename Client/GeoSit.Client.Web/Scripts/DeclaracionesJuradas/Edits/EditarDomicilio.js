var editarDomicilio = {
    setLocalEdit(domicilio) {
        $('#Id').val(domicilio.Id);
        $('#PersonaId').val(domicilio.PersonaId);
        $('#Tipo').val(domicilio.Tipo);
        $('#Provincia').val(domicilio.Provincia);
        $('#Localidad').val(domicilio.Localidad);
        $('#Barrio').val(domicilio.Barrio);
        $('#Calle').val(domicilio.Calle);
        $('#Altura').val(domicilio.Altura);
        $('#Piso').val(domicilio.Piso);
        $('#Departamento').val(domicilio.Departamento);
        $('#Barrio').val(domicilio.Barrio);
        $('#CodigoPostal').val(domicilio.CodigoPostal);

        $("#modalEditarDomicilio").modal("show");

        $('#editarDomicilioTitle').text('Edicion de Domicilio');

    },
    init: function () {


        $("#modalEditarDomicilio").modal("show");

        $('#btnGuardarDomicilio').on('click', function () {
            rubro3.updateDomiciliosRow({
                Id: parseInt($('#Id').val()),
                PersonaId: $('#PersonaId').val(),
                Tipo: $('#Tipo').val(),
                Provincia: $('#Provincia').val(),
                Localidad: $('#Localidad').val(),
                Barrio: $('#Barrio').val(),
                Calle: $('#Calle').val(),
                Altura: $('#Altura').val(),
                Piso: $('#Piso').val(),
                Departamento: $('#Departamento').val(),
                CodigoPostal: $('#CodigoPostal').val()

            });

            $("#modalEditarDomicilio").modal("hide");

        });


        $('.closeModalDomicilio').on('click', function () {
            $('#modalEditarDomicilio').modal('hide');
        });
    }
};