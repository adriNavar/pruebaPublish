var libreDeDeuda = {
    init: function () {
        $("#fecha-vigencia, #fecha-emision").datepicker(getDatePickerConfig({ defaultDate: new Date(), enableOnReadonly: false }))
            .change(function () {
                $(this).valid();
            });

    }
}