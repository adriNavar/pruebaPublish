$(document).ready(function () {
    errorAlertInit();

    var form = $("#form");
    form.ajaxForm({
        success: function (result) {
            if (result == 'Ok') {                
                $("#userChangePassword").modal('hide');
                modalConfirm("Cambiar Contraseña", "La contraseña ha sido cambiada satisfactoriamente.", "success");
            } else {
                errorAlert(result);
            }
        }
    });
    form.bootstrapValidator({
        excluded: [":disabled"],
        fields: {
            Password: {
                validators: {
                    notEmpty: {
                        message: "El campo Contraseña Anterior es obligatorio"
                    }
                }
            },
            NewPassword: {
                validators: {
                    notEmpty: {
                        message: "El campo Nueva Contraseña es obligatorio"
                    }
                }
            },
            ConfirmNewPassword: {
                validators: {
                    identical: {
                        field: 'NewPassword',
                        message: 'Los campos Confirmar Contraseña y Nueva Contraseña no son iguales'
                    },
                    notEmpty: {
                        message: "El campo Confirmar Contraseña es obligatorio"
                    }
                }
            }
        }
    });

    $('#save').click(function (e) {
        var bootstrapValidator = form.data("bootstrapValidator");
        bootstrapValidator.validate();

        if (bootstrapValidator.isValid()) {
            //$('#Password').val(CryptoJS.MD5($('#Password').val()));
            form.submit();
        }
    });

    hideLoading();

    $("#userChangePassword").modal('show');
});

function errorAlertInit() {
    var message = $("#message-error");
    message.find(".close").click(function () {
        $("#message-error").hide();
    });
}

function errorAlert(text) {
    var message = $("#message-error");
    message.find("p").html(text);
    $("#message-error").fadeIn("slow").delay(5000).queue(function () {
        $("#message-error").hide().dequeue();
    });
}

function modalConfirm(title, message, type) {
    var mensajeAlerta = $("#MensajeAlerta");
    var clases = ["alert-success", "alert-warning", "alert-error"];
    for (var i = 0; i < clases.length; i++) {
        mensajeAlerta.removeClass(clases[i]);
    }
    if (type === "warning") {
        mensajeAlerta.addClass("alert-warning");
        $("#btnAdvertenciaCancel").show();
    } else if (type === "success") {
        mensajeAlerta.addClass("alert-success");
        $("#btnAdvertenciaCancel").hide();
    } if (type === "error") {
        mensajeAlerta.addClass("alert-error");
        $("#btnAdvertenciaCancel").hide();
    }
    $("#TituloAdvertencia").text(title);
    $("#DescripcionAdvertencia").html(message);
    $("#confirmModal").modal("show");
}