appJS.directive('enterKeyPressed', function () {
    return function(scope, element, attrs) {
        element.bind("keypress", function(event) {
            var keyCode = event.which || event.keyCode;
            if (keyCode === 13) { //enterKeyCode = 13
                scope.$apply(function () {
                    scope.$eval(attrs.enterKeyPressed);
                });
                event.preventDefault();
            }
        });
    };
})