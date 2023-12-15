appJS.filter('capitalize', function ($sce) {
    return function (texto) {
        return $sce.trustAsHtml(texto.toString().toLowerCase().replace(/(?:^|\s)(?!(de|y|a|del) )[a-z]/g, function (a) { return a.toUpperCase(); }));
    };
});