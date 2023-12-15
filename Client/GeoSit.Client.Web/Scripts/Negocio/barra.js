

function search(token) {

    $("#smessage").html("Buscando...").show();
    if (token) {
        
    } else {
        setTimeout(function () {
            search((new Date()).getTime());
        }, 500);
    }
}

function hl(t1, t2) {
    return t1.replace(new RegExp(t2, 'g'), "<span style='background-color:#ff0;'>" + t2 + "</span>");
}