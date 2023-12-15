appJS.filter('highlight', function ($sce) {
    return function (texto, pattern, ignorecase) {
        if (pattern === undefined || texto === undefined) return $sce.trustAsHtml(texto);
        let options = 'g';
        if (ignorecase) options += 'i';
        pattern = ["+", "-", "(", ")", "*", "?", ":"].reduce((accum, elem) => accum.replace(elem, `\\${elem}`), pattern);
        return $sce.trustAsHtml(texto.toString().replace(new RegExp(pattern, options), "<span style='background-color:#ff0;'>$&</span>"));
    };
});