appJS.directive('autocomplete', ['$http', function ($http) {
    return function (scope, element, attrs) {
        function process() {
            scope.$apply(function () {
                scope.$eval(attrs.search);
            });
            event.preventDefault();
        };
        element.autocomplete({
            delay: 100, minLength: 1,
            source: function (request, response) {
                $http.get(BASE_URL + 'Search/Suggest', { params: { text: request.term } })
                    .success(function (data) {
                        var suggestions = [];

                        for (suggester in data.suggest) {
                            if (data.suggest[suggester][request.term.trim()]) {
                                suggestions = suggestions.concat(data.suggest[suggester][request.term.trim()].suggestions);
                            }
                        }
                        suggestions.sort(function (a, b) {
                            if (typeof Math.sign === 'undefined') { Math.sign = function (x) { return x > 0 ? 1 : x < 0 ? -1 : x; } }
                            Math.sign(a.weight - b.weight) * -1;
                        });
                        response(suggestions.slice(0, 15).map(function (item) {
                            return item.term;
                        }));
                    });
            },
            select: function (event, ui) {
                scope.texto = ui.item.value;
                process();
            }
        });
        element.bind("keypress", function (event) {
            var keyCode = event.which || event.keyCode;
            if (keyCode === 13) { //enterKeyCode = 13
                process();
            }
        });
    };
}]);