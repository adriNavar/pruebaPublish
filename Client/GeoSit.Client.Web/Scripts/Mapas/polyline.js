// Basado en la documentación del algoritmo de google: https://developers.google.com/maps/documentation/utilities/polylinealgorithm 
// y en la implementación de https://github.com/Project-OSRM/osrm-frontend/blob/master/WebContent/routing/OSRM.RoutingGeometry.js
var google = {};

google.decodePolyline = function (encodedPolyline) {
    var len = encodedPolyline.length, index = 0, lat = 0, lng = 0, array = [], precision = Math.pow(10, -5);
    while (index < len) {
        var b, shift = 0, result = 0;
        do {
            b = encodedPolyline.charCodeAt(index++) - 63;
            result |= (b & 0x1f) << shift;
            shift += 5;
        } while (b >= 0x20);
        var dlat = ((result & 1) ? ~(result >> 1) : (result >> 1));
        lat += dlat;
        shift = result = 0;
        do {
            b = encodedPolyline.charCodeAt(index++) - 63;
            result |= (b & 0x1f) << shift;
            shift += 5;
        } while (b >= 0x20);
        var dlng = ((result & 1) ? ~(result >> 1) : (result >> 1));
        lng += dlng;
        array.push([lat * precision, lng * precision]);
    }
    return array;
};