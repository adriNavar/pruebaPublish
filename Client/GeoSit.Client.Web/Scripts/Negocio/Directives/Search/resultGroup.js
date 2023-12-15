appJS.directive('resultGroup', function ($timeout) {
    return {
        scope: {
            pattern: '=',
            grupo: '=',
            permisos: '='
        },
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultGroupTemplate.html',
        link: function (scope) {
            //armo string con cantidad de registros para mostrar al pasar el mouse por encima de la cantidad
            scope.totalRegistros = 'mostrando ' + scope.grupo.Items.length + ' de ' + scope.grupo.Cantidad.toString() + ' registro' + (scope.grupo.Cantidad === 1 ? '' : 's') + ' encontrado' + (scope.grupo.Cantidad === 1 ? '' : 's');
            scope.groupsTotal = scope.$parent.groupsTotal;
            scope.openGroup = scope.$parent.cantidadTotal <= 5 || scope.groupsTotal === 1;
            scope.verEnMapa = function ($event) {
                $event.stopImmediatePropagation();
                $event.preventDefault();
                var selected = this.grupo.Items.filter(function (elem) { return elem.selected; });
                if (selected.length) {
                    selected = selected.reduce(function (acum, elem) { return acum.concat(elem.featids); }, []);
                    GeoSIT.MapaController.seleccionarObjetos([selected], [this.grupo.Items[0].capa]);
                    hideSearchBlur();
                }
            };
            scope.selected = false;
            scope.toggleGroupSelection = function () {
                var selected = this.selected;
                angular.forEach(this.grupo.Items, function (elem) { elem.selected = selected; });
            };
            if (scope.$parent.$last === true) {
                $timeout(function () {
                    scope.$emit('ngSearchFinished');
                }, 500);
            }
        }
    };
});