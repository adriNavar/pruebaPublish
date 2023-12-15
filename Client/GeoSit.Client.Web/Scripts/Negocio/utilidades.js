$(document).ready(function () {
    $.ajaxSetup({
        // Disable caching of AJAX responses
        cache: false
    });
});

function habilitarCamposNumericos() {
    $('[type="number"]').numeric();
}

function FormatFechaHora(Fecha, Hora) {

    if (!Fecha) return '';
    var mnt = moment(Fecha, ["DD/MM/YYYY", "DD/MM/YYYY HH:mm:ss", "YYYY-MM-DD", "YYYY-MM-DDTHH:mm:ss"]);

    var date = Fecha instanceof Date ? Fecha : /\/Date\(-?\d+\)\/$/.test(Fecha) ? parseJsonDate(Fecha) : mnt.isValid() ? mnt.toDate() : new Date(Fecha.replace('T', ' '));

    var curr_date = date.getDate().toString().padLeft('0', 2);
    var curr_month = (date.getMonth() + 1).toString().padLeft('0', 2);
    var curr_year = date.getFullYear().toString().padLeft('0', 4);
    var curr_hours = date.getHours().toString().padLeft('0', 2);
    var curr_minutes = date.getMinutes().toString().padLeft('0', 2);
    var curr_seconds = date.getSeconds().toString().padLeft('0', 2);

    return curr_date + '/' + curr_month + '/' + curr_year + (Hora ? ' ' + curr_hours + ':' + curr_minutes + ':' + curr_seconds : '');
}

function parseJsonDate(jsonDateString) {
    if (jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    } else {
        return '';
    }
}


// VALIDACION XML

var xt = "", h3OK = 1;
function checkErrorXML(x) {
    xt = "";
    h3OK = 1;
    checkXML(x);
}

function checkXML(n) {
    var l, i, nam;
    nam = n.nodeName;
    if (nam === "h3") {
        if (h3OK === 0) {
            return;
        }
        h3OK = 0;
    }
    if (nam === "#text") {

        xt = xt + n.nodeValue + "\n";
    }
    l = n.childNodes.length;
    for (i = 0; i < l; i++) {
        checkXML(n.childNodes[i]);
    }
}

function validateXML(txt) {
    if (txt === '') return "Sin Errores";

    var xmlDoc;
    if (window.ActiveXObject) { // codigo para IE
        xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = "false";
        xmlDoc.loadXML(txt);

        if (xmlDoc.parseError.errorCode !== 0) {

            txt = "Error Code: " + xmlDoc.parseError.errorCode + "\n";
            txt = txt + "Error Reason: " + xmlDoc.parseError.reason;
            txt = txt + "Error Line: " + xmlDoc.parseError.line;
            return txt;

        } else {
            return "Sin Errores";
        }
    } else if (document.implementation.createDocument) { //codigo para el resto de los browsers
        xmlDoc = new DOMParser().parseFromString(txt, "text/xml");

        if (xmlDoc.getElementsByTagName("parsererror").length > 0) {
            checkErrorXML(xmlDoc.getElementsByTagName("parsererror")[0]);
            if (xt.indexOf('Opening') > -1 || xt.indexOf('Extra') > -1) {
                return 'Las etiquetas de Apertura y/o Cierre no coinciden' + "\n" + 'Por favor, realice verificaciones' + "\n";
            } else {
                return xt;
            }
        } else {
            return "Sin Errores";
        }
    } else {
        return "Su navegador no puede manejar la validación XML";
    }
}

// FIN VALIDACION XML
String.prototype.padLeft = function (str, len) {
    return this.length > len
        ? String(this)
        : (Array(len + 1).join(str) + this).slice(-len);
};

function getFileData(fileControl) {
    var files = $(fileControl).get(0).files,
        filename = files[0].name;
    var data = new FormData();
    for (i = 0; i < files.length; i++) {
        data.append("file" + i, files[i]);
    }
    return { fileName: filename, data: data };
}

function getNiceScrollConfig(options = {}) {
    const defaults = {
        styler: "fb",
        cursorcolor: "rgba(0,0,0,0.5)",
        cursorwidth: '8px',
        autohidemode: false,
        cursorborderradius: '5px',
        background: '',
        spacebarenabled: false,
        horizrailenabled: false,
        cursorborder: '',
        railpadding: { top: 0, right: 0, left: 0, bottom: 0 }
    };
    return { ...defaults, ...options };
}

function getDatePickerConfig(options = {}) {
    const defaults = {
        orientation: "auto",
        language: 'es',
        autoclose: true,
        todayHighlight: true,
        todayBtn: "linked"
    };
    return { ...defaults, ...options };
}

Math.sign = function (x) {
    return typeof x === 'number' ? x ? x < 0 ? -1 : 1 : x === x ? 0 : NaN : NaN;
};

Number.prototype.countDecimals = function () {
    if (Math.floor(this.valueOf()) === this.valueOf()) return 0;
    return this.toString().split(".")[1].length || 0;
};

Number.prototype.countIntegers = function () {
    if (Math.floor(this.valueOf()) === this.valueOf()) return this.toString().length;
    return this.toString().split(".")[0].length || 0;
};

function setDecimal(input, precision) {
    if (Number(input.value) > input.max) {
        var defaultLength = Number(input.max).countIntegers();
        var str1 = input.value.toString().substring(0, defaultLength);
        var str2 = input.value.toString().substring(defaultLength, input.value.toString().length).replace(".", "");
        var finalValue = str1 + "." + str2;
        if (str2.length > precision)
            input.value = parseFloat(finalValue).toFixed(precision);
        else
            input.value = parseFloat(finalValue);
    }
    if (Number(input.value) < input.min) {
        input.value = input.min;
    }
    if (Number(input.value).countDecimals() > precision) {
        input.value = parseFloat(input.value).toFixed(precision);
    }
}

function crearElementoHtml(tag, opciones) {
    var elem = Object.assign(document.createElement(tag), opciones);
    if (opciones && opciones.type === 'hidden' && opciones.class) {
        elem.className = opciones.class;
    }
    return elem;
}

function alertaGenerico(titulo, mensaje, tipo) {
    var cls = "";
    switch (tipo) {
        case 's':
            cls = "alert-success";
            break;
        case 'w':
            cls = "alert-warning";
            break;
        case 'e':
            cls = "alert-danger";
            break;
        case 'i':
            cls = "alert-info";
            break;
    }
    $("#MensajeErrorGenerico").removeClass('alert-info alert-warning alert-danger alert-success').addClass(cls);
    $('#TituloModalErrorGenerico', '#ModalErrorGenerico').html(titulo);
    $('#DescripcionMensajeErrorGenerico', '#ModalErrorGenerico').html(mensaje);
    $('#ModalErrorGenerico').modal('show');
}

String.prototype.isValidEmail = function () {
    return /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i
        .test(this);
};

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

String.prototype.isValidDate = function () {
    var mnt = moment(this, ["DD/MM/YYYY", "DD/MM/YYYY HH:mm:ss", "YYYY-MM-DD", "YYYY-MM-DDTHH:mm:ss"]);
    return this instanceof Date || /\/Date\(-?\d+\)\/$/.test(this) || mnt.isValid();
};

String.prototype.toISOString = function () {
    var mnt = moment(this, ["DD/MM/YYYY", "DD/MM/YYYY HH:mm:ss", "YYYY-MM-DD", "YYYY-MM-DDTHH:mm:ss"]);
    try {
        const fecha = this instanceof Date ? this : /\/Date\(-?\d+\)\/$/.test(this) ? parseJsonDate(this) : mnt.isValid() ? mnt.toDate() : new Date(this.replace('T', ' '));
        return fecha.toISOString();
    } catch {
        return null;
    }
}