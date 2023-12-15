appJS.service('securityService', ['$http', '$q', function ($http, $q) {
    this.getPermisos = function (lista) {
        var async = $q.defer();
        var url = BASE_URL + "Home/GetPermisos";
        $http.get(url, { params: { permisos: lista } })
            .success(function (data) {
                var values = [];
                data.map(function(item) {
                    values.push(item.toString());
                });
                async.resolve(values);
            })
            .error(function(error) {
                async.resolve({});
            });
        return async.promise;
    };
}]);