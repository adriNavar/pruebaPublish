appJS.filter('searchGroupHeader', function () {
    return function (texto) {
        var cabecera = texto;
        switch (texto.toString().toLowerCase()) {
            case "unidadestributarias":
                cabecera = "unidades tributarias";
                break;
            case "unidadestributariasprovinciales":
                cabecera = "unidades tributarias provinciales";
                break;
            case "parcelasprovinciales":
                cabecera = "parcelas provinciales";
                break;
            case "manzanasprovinciales":
                cabecera = "manzanas provinciales";
                break;
            case "mejorasprovinciales":
                cabecera = "mejoras provinciales";
                break;
            case "centrosurbanos":
                cabecera = "centros urbanos";
                break;
            case "redgeo1":
                cabecera = "red geodésica 1";
                break;
            case "redgeo2":
                cabecera = "red geodésica 2";
                break;
            case "redgeo3":
                cabecera = "red geodésica 3";
                break;
            case "redgeo4":
                cabecera = "red geodésica 4";
                break;
            case "tramites":
                cabecera = "trámites";
                break;
            case "paf":
                cabecera = "puntos de apoyo fotogramétrico";
                break;
            case "pcc":
                cabecera = "puntos de control de campo";
                break;
            case "parcelasmunicipales":
                cabecera = "parcelas municipales";
                break;
            case "parcelasproyectos":
                cabecera = "parcelas proyectos";
                break;
            case "parcelassaneables":
                cabecera = "parcelas a sanear";
                break;
            case "redplani":
                cabecera = "red planialtimétrica";
                break;
            case "parcelasbaja":
                cabecera = "parcelas históricas";
                break;
            case "unidadestributariasbaja":
                cabecera = "unidades tributarias históricas";
                break;
            default:
                break;
        }
        return cabecera;
    };
});