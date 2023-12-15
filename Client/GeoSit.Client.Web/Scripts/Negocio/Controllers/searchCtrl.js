appJS.controller('searchCtrl', ['$scope', '$http', 'securityService', function ($scope, $http, security) {
    $scope.visibleBtnErase = false;
    $scope.visibleBtnSearch = true;
    $scope.visibleBtnCollapse = false;
    $scope.generarColeccion = function ($evt) {
        $evt.stopImmediatePropagation();
        $('#TituloModalInputGenerico', '#ModalInputGenerico').html('Generar Colección');
        $('#txtInputGenerico', '#ModalInputGenerico').attr('placeholder', 'Ingrese el Nombre de la Colección...');
        $('#btnAceptarInputGenerico', '#ModalInputGenerico').one('click', function () {
            showLoading();
            $http({
                url: BASE_URL + 'Coleccion/NuevaColeccion?nombre=' + $('#txtInputGenerico', '#ModalInputGenerico').val(),
                method: "POST",
                dataType: 'json',
                data: getSelection(),
                responseType: 'text'
            }).then(function (resp) {
                $('#ModalInputGenerico').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
                setTimeout(function () { $("#contenido").html(resp.data); }, 200);
            }, function (err) {
                let errmsg;
                if (err.status === 400) {
                    errmsg = "No se ha especificado un nombre para la colección.";
                } else if (err.status === 411) {
                    errmsg = "No se ha seleccionado ningún elemento para la colección.";
                } else if (err.status === 401) {
                    errmsg = "No está autorizado a generar una colección.";
                } else if (err.status === 409) {
                    errmsg = "Ya existe una colección con el nombre especificado.";
                }
                alertaGenerico('Generar Colección - Advertencia', errmsg, 'w');
            }).then(hideLoading);
        });
        $('#ModalInputGenerico').modal('show');
    };
    $scope.exportarObjetosExcel = function ($evt, objetos) {
        $evt.stopPropagation();
        showLoading();
        var objsToExport = [];
        if (objetos) {
            if (Array.isArray(objetos)) {
                objsToExport = objetos;
            } else {
                objsToExport = [objetos];
            }
        } else {
            objsToExport = $scope.resultados
                .filter(function (grp) { return !!grp.exportable; })
                .flatMap(function (grp) { return grp.Items; })
                .filter(function (item) { return !!item.selected; });
        }
        $http({
            url: BASE_URL + 'DetalleObjeto/ExportarObjetoExcel',
            data: objsToExport.map(function (item) { return { id: item.id, componente: item.tipo }; }),
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' }
        }).then(function () {
            window.location = BASE_URL + "DetalleObjeto/DownloadFile";
        }, function () {
            alert('Error');
        }).then(hideLoading);
    };
    $scope.searchByText = function () {
        var url = BASE_URL + 'Search/ByText';
        $("#search-pattern").autocomplete("close");
        if (!!$scope.texto && $scope.texto.trim().length) {
            showLoading();
            $scope.pattern = $scope.texto;
            searchData(url, { "text": $scope.texto });
            $scope.visibleBtnErase = true;
            $scope.visibleBtnCollapse = true;
        }
    };
    $scope.eraseSearch = function () {
        console.log(this);
        toggleSearch(true);
        var search = $("#search-pattern");
        search.val('');
        search.keyup();
        $scope.resultados = null;
        $scope.mensaje = "";
        $scope.visibleBtnErase = false;
        $scope.visibleBtnCollapse = false;
        $scope.$emit('limpiarGrilla');
    };
    $scope.collapseSearch = function () {
        closeLeftSideMenu();
        toggleSearch();
    };
    $scope.searchByFeatures = function (features, filterType) {
        var url = BASE_URL + 'Search/ByFeatures';
        searchData(url, { features: JSON.stringify(features), filterType: filterType || 1 });
    };
    $scope.searchByFeaturesDocType = function (features, feature) {
        var url = BASE_URL + 'Search/ByFeatures';
        searchData(url, { features: JSON.stringify(features), filterType: 2, newFeature: JSON.stringify(feature) });
    };
    $scope.$on('ngSearchFinished', function () {
        updateResultsArea();
    });
    $scope.$on('exportarExcel', $scope.exportarObjetosExcel);
    $scope.getByFeatures = function (features) {
        var url = BASE_URL + 'Search/ByFeatures';
        showLoading();
        $scope.mensaje = 'Buscando...';
        return $http.post(url, { features: JSON.stringify(features) });
    };
    var searchData = function (url, params) {
        $scope.mensaje = 'Buscando...';
        var total = 0, groups = [];
        security.getPermisos([
            seguridadResource.MantenedorParcelario,
            seguridadResource.ABMDomicilios,
            seguridadResource.ABMDocumentos,
            seguridadResource.ExpedienteObra,
            seguridadResource.ABMPersonas,
            seguridadResource.ABMActas,
            seguridadResource.VisualizarValuacion,
            seguridadResource.ABMTramites,
            seguridadResource.ReportePersona,
            seguridadResource.ReporteBienesRegistrados,
            seguridadResource.ReporteHistoricoTitulares,
            seguridadResource.ReportePropiedad,
            seguridadResource.ReporteSituacion,
            seguridadResource.InformeParcelario,
            seguridadResource.VisualizarDDJJ,
            seguridadResource.CertificadoValuatorio
        ]).then(function (permisos) {
            $scope.permisos = permisos;
            $http({
                url: url,
                method: "POST",
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                data: $.param(params)
            })
                .success(function (data) {
                    data.grouped.tipo.groups.forEach(function (group) {
                        total += group.doclist.numFound;
                        groups.push({ Cantidad: group.doclist.numFound, Items: group.doclist.docs, Nombre: group.groupValue });
                    });

                    $scope.mensaje = total > 0 ? "Se ha" + (total === 1 ? "" : "n") + " encontrado " + total + " resultado" + (total === 1 ? "" : "s") : "No se han encontrado resultados";
                    $scope.resultados = groups;
                    $scope.cantidadTotal = total;
                    $scope.groupsTotal = groups.length;

                    if (!$scope.resultados.length) {
                        $scope.$emit('ngSearchFinished');
                    }
                    toggleSearch(null, true);
                })
                .error(function (data, status) {
                    $scope.data = data || "Request failed";
                    $scope.status = status;
                    $scope.mensaje = status;
                }).then(function () { hideLoading(); });
        });
        return { total: total, groups: groups };
    },
        getSelection = function () {
            return $scope.resultados.reduce(function (acum, grp) {
                return acum
                    .concat(grp.Items
                        .filter(function (elem) { return elem.selected; })
                        .map(function (elem) { return { ComponenteDocType: elem.tipo, ObjetoId: elem.id }; }));
            }, []);
        };
}]);