var directivaParcelas = function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultParcelasTemplate.html',
        link: function (scope) {
            scope.allowMantenedor = $.inArray(seguridadResource.MantenedorParcelario, scope.permisos) > -1;
            scope.allowInformeParcelario = $.inArray(seguridadResource.InformeParcelario, scope.permisos) > -1;
            scope.parcela = scope.$parent.elem;
            let partes = [scope.parcela.nombre];
            if (scope.parcela.dato_nomenclatura) {
                partes = [...partes, scope.parcela.dato_nomenclatura];
            }
            scope.parcela.partida_nomenclatura = partes.join("-");
            scope.parcela.selected = false;
            scope.abrir = function (url, id) {
                loadView(BASE_URL + url + '/' + id);
                hideSearchBlur();
            };
            scope.getInformeParcelario = function () {
                showLoading();
                var request = {
                    method: 'POST',
                    url: BASE_URL + 'MantenimientoParcelario/GetInformeParcelario/' + scope.parcela.id,
                };
                $http(request)
                    .then(function () {
                        $window
                            .open(BASE_URL + 'MantenimientoParcelario/abrir/');
                    }, function (error) {
                        alert(error.statusText);
                    })
                    .then(hideLoading);
            };

        }
    };
};
var directivaParcelasProvinciales = function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultParcelasTemplate.html',
        link: function (scope) {
            scope.allowInformeParcelario = $.inArray(seguridadResource.InformeParcelario, scope.permisos) > -1;
            scope.parcela = scope.$parent.elem;
            let partes = [scope.parcela.nombre];
            if (scope.parcela.dato_nomenclatura) {
                partes = [...partes, scope.parcela.dato_nomenclatura];
            }
            scope.parcela.partida_nomenclatura = partes.join("-");
            scope.parcela.selected = false;
            scope.getInformeParcelario = function () {
                showLoading();
                var request = {
                    method: 'POST',
                    url: `${BASE_URL}MantenimientoParcelario/GetInformeParcelarioProvincial`,
                    data: { id: Number(scope.parcela.id) }
                };
                $http(request)
                    .then(function () {
                        $window.open(`${BASE_URL}MantenimientoParcelario/abrir/`);
                    }, function (error) {
                        alert(error.statusText);
                    })
                    .then(hideLoading);
            };
        }
    };
};
appJS.directive('resultType', function ($compile, $http) {
    return {
        scope: {
            pattern: "=",
            tipo: "=",
            permisos: "="
        },
        restrict: 'A',
        terminal: true,
        priority: 1000,
        link: function (scope, element) {
            /*agrego el atributo dinamicamente dependiendo del valor que este dado por el atributo result-type*/
            element.attr("result-" + scope.tipo.toLowerCase(), "");
            /*remuevo los atributos para evitar loop infinito (contemplo las 2 posibles nomenclaturas)*/
            element.removeAttr("result-type");
            element.removeAttr("data-result-type");
            scope.verEnMapa = function ($evt, elem, zoom) {
                $evt.stopImmediatePropagation();
                GeoSIT.MapaController.seleccionarObjetos([elem.featids], [elem.capa], zoom);
                hideSearchBlur();
            };
            scope.exportarObjetoExcel = function ($evt) {
                $evt.stopImmediatePropagation();
                scope.$emit('exportarExcel', this.$parent.elem);
            };
            scope.showAtributos = function () {
                scope.$emit('showAtributos');
                showLoading();
                var request = {
                    method: 'POST',
                    url: BASE_URL + 'DetalleObjeto/GetDetalleObjetoByDocType',
                    params: { objetoId: scope.$parent.elem.id, docType: scope.$parent.elem.tipo }
                };
                $http(request)
                    .then(function (resp) { mostrarDetalleObjetoSearch(resp.data); }, function (error) {
                        console.log(error.status);
                        alert("El componente no está bien configurado");
                    })
                    .then(hideLoading);
            };
            scope.verDocumento = function ($evt) {
                $evt.stopImmediatePropagation();
                loadView(`${BASE_URL}PdfInternalViewer/View/${scope.$parent.elem.iddocumento}?esDocProvincial=${scope.esDocumentoProvincial}`);
            };
            scope.selectedChanged = function () {
                this.$parent.$parent.selected = false;
            };
            scope.chkboxid = scope.$parent.elem.tipo + scope.$parent.elem.id;
            scope.ploteable = true;
            scope.$parent.grupo.exportable = true;
            scope.tieneDescripcion = !!scope.$parent.elem.descripcion;
            scope.tieneGrafico = !!scope.$parent.elem.featids && !!scope.$parent.elem.featids.length;
            scope.tieneDocumento = !!scope.$parent.elem.iddocumento;
            scope.esDocumentoProvincial = false;
            $compile(element)(scope);
        }
    };
});
appJS.directive('resultUnidadestributarias', function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultUnidadesTributariasTemplate.html',
        link: function (scope) {
            scope.allowMantenedor = $.inArray(seguridadResource.MantenedorParcelario, scope.permisos) > -1 && scope.$parent.elem.idpadre;
            scope.allowDDJJ = $.inArray(seguridadResource.VisualizarDDJJ, scope.permisos) > -1 && scope.$parent.elem.idpadre;
            scope.allowReporteHistoricoTitulares = $.inArray(seguridadResource.ReporteHistoricoTitulares, scope.permisos) > -1;
            scope.allowReportePropiedad = $.inArray(seguridadResource.ReportePropiedad, scope.permisos) > -1;
            scope.allowReporteSituacion = $.inArray(seguridadResource.ReporteSituacion, scope.permisos) > -1;
            scope.allowCertificadoValuatorio = $.inArray(seguridadResource.CertificadoValuatorio, scope.permisos) > -1;
            scope.esTipoPH = Number(scope.$parent.elem.dato_esTipoPH) === 1;
            scope.unidadTributaria = scope.$parent.elem;
            scope.unidadTributaria.selected = false;
            scope.abrir = function (url, id) {
                loadView(BASE_URL + url + '/' + id);
                hideSearchBlur();
            };
            scope.generarReporteSituacion = function () {
                showLoading();
                $http({
                    method: "POST",
                    url: BASE_URL + 'MantenimientoParcelario/GetInformeSituacionPartidaInmobiliaria/' + scope.unidadTributaria.idpadre
                }).then(function () {
                    $window.open(BASE_URL + 'MantenimientoParcelario/Abrir/', '_blank');
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
            scope.generarReporteHistoricoTitulares = function () {
                showLoading();
                $http({
                    method: "POST",
                    url: BASE_URL + 'UnidadTributaria/GenerarReporteHistoricoTitulares?idUnidadTributaria=' + scope.unidadTributaria.id
                }).then(function () {
                    $window.open(BASE_URL + 'UnidadTributaria/AbrirReporte/', '_blank');
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
            scope.generarReportePropiedad = function () {
                showLoading();
                $http({
                    method: "POST",
                    url: BASE_URL + 'UnidadTributaria/GenerarReportePropiedad?idUnidadTributaria=' + scope.unidadTributaria.id
                }).then(function () {
                    $window.open(BASE_URL + 'UnidadTributaria/AbrirReporte/', '_blank');
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
            scope.getReporteUnidadTributaria = function () {
                showLoading();
                $http({
                    method: "POST",
                    url: BASE_URL + "MantenimientoParcelario/GetInformeUTFromSearch",
                    params: { idUnidadTributaria: scope.unidadTributaria.id, idParcela: scope.unidadTributaria.idpadre }
                }).then(function () {
                    $window.open(BASE_URL + "MantenimientoParcelario/Abrir");
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
            scope.getCertificadoValuatorio = function () {
                $http({
                    url: BASE_URL + "Valuacion/ValidateCertificadoValuatorio",
                    params: { idUnidadTributaria: scope.unidadTributaria.id },
                    method: 'POST'
                }).then(function (validacion) {
                    if (validacion.data == "False") {
                        alert("Se ha producido un error al generar el Certificado Valuatorio. La partida no posee DDJJ, carguela.");
                    } else {
                        showLoading();
                        $http({
                            url: BASE_URL + "Valuacion/GenerarReporteValuatorio",
                            params: { idUnidadTributaria: scope.unidadTributaria.id },
                            method: 'POST'
                        }).then(function () {
                            $window.open(BASE_URL + "Valuacion/AbrirReporte");
                        }, function (err) {
                            if (err.status === 409) {
                                alert("Se ha producido un error al generar el Certificado Valuatorio. La partida no posee dominio o su superficie es 0.");
                            } else {
                                alert("error");
                            }
                        }).then(hideLoading);
                    }
                })
            };
            scope.getAdministracionDDJJ = function () {
                showLoading();
                $http({
                    url: BASE_URL + "DeclaracionesJuradas/DeclaracionesJuradas",
                    method: "GET",
                    params: { idUnidadTributaria: scope.unidadTributaria.id, cargadoDelBuscador: true },
                    responseType: "string"
                }).then(function (resp) {
                    $("#contenido").html(resp.data);
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
        }
    };
});
appJS.directive('resultUnidadestributariasprovinciales', function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultUnidadesTributariasTemplate.html',
        link: function (scope) {
            scope.esUTPH = scope.$parent.elem.dato_esUTPH;
            scope.unidadTributaria = scope.$parent.elem;
            scope.unidadTributaria.selected = false;
            scope.getReporteUnidadTributaria = function () {
                showLoading();
                $http({
                    method: "POST",
                    url: BASE_URL + "MantenimientoParcelario/GetInformeUTProvincialFromSearch",
                    data: { idUnidadTributaria: Number(scope.unidadTributaria.id), idParcela: Number(scope.unidadTributaria.idpadre), partida: scope.unidadTributaria.nombre }
                }).then(function () {
                    $window.open(BASE_URL + "MantenimientoParcelario/Abrir");
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
        }
    };
});
appJS.directive('resultUnidadestributariasbaja', function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultUnidadesTributariasHistoricasTemplate.html',
        link: function (scope) {
            scope.esUTPH = scope.$parent.elem.dato_esUTPH;
            scope.unidadTributaria = scope.$parent.elem;
            scope.unidadTributaria.selected = false;
            scope.getReporteUnidadTributaria = function () {
                showLoading();
                $http({
                    method: "POST",
                    url: `${BASE_URL}MantenimientoParcelario/GetInformeUTBaja`,
                    data: { idUnidadTributaria: Number(scope.unidadTributaria.id), idParcela: Number(scope.unidadTributaria.idpadre), partida: scope.unidadTributaria.nombre }
                }).then(function () {
                    $window.open(`${BASE_URL}MantenimientoParcelario/abrir`);
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
        }
    };
});
appJS.directive('resultDepartamentos', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultJurisdicciones', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultMunicipios', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultLocalidades', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultSecciones', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultBarrios', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultCentrosurbanos', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultParajes', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultAfluentes', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultLagunas', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultReservas', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultRedgeo1', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
            scope.esDocumentoProvincial = true;
        }
    };
});
appJS.directive('resultRedgeo2', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
            scope.esDocumentoProvincial = true;
        }
    };
});
appJS.directive('resultRedgeo3', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
            scope.esDocumentoProvincial = true;
        }
    };
});
appJS.directive('resultRedgeo4', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
            scope.esDocumentoProvincial = true;
        }
    };
});

appJS.directive('resultParcelas', directivaParcelas);
appJS.directive('resultPrescripciones', directivaParcelas);
appJS.directive('resultParcelasmunicipales', directivaParcelas);
appJS.directive('resultParcelasproyectos', directivaParcelas);
appJS.directive('resultParcelasprovinciales', directivaParcelasProvinciales);
appJS.directive('resultParcelasbaja', function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultParcelasHistoricasTemplate.html',
        link: function (scope) {
            scope.allowInformeParcelario = $.inArray(seguridadResource.InformeParcelario, scope.permisos) > -1;
            scope.parcela = scope.$parent.elem;
            let partes = [scope.parcela.nombre];
            if (scope.parcela.dato_nomenclatura) {
                partes = [...partes, scope.parcela.dato_nomenclatura];
            }
            scope.parcela.partida_nomenclatura = partes.join("-");
            scope.parcela.selected = false;
            scope.getInformeParcelario = function () {
                showLoading();
                var request = {
                    method: 'POST',
                    url: `${BASE_URL}MantenimientoParcelario/GetInformeParcelarioBaja`,
                    data: { id: Number(scope.parcela.id), partida: scope.parcela.nombre }
                };
                $http(request)
                    .then(function () {
                        $window.open(`${BASE_URL}MantenimientoParcelario/abrir`);
                    }, function (error) {
                        alert(error.statusText);
                    })
                    .then(hideLoading);
            };

        }
    };
});

appJS.directive('resultManzanas', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultManzanasprovinciales', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultPersonas', function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultPersonasTemplate.html',
        link: function (scope) {
            scope.$parent.grupo.exportable = false;
            scope.ploteable = false;
            scope.allowPersona = $.inArray(seguridadResource.ABMPersonas, scope.permisos) > -1;
            scope.allowReportePersona = $.inArray(seguridadResource.ReportePersona, scope.permisos) > -1;
            scope.allowReporteBienesRegistrados = $.inArray(seguridadResource.ReporteBienesRegistrados, scope.permisos) > -1;
            scope.persona = scope.$parent.elem;
            scope.persona.selected = false;
            scope.abrir = function (url, id) {
                loadView(BASE_URL + url + id);
                hideSearchBlur();
            };
            scope.generarReportePersona = function () {
                showLoading();
                $http({
                    type: "POST",
                    url: BASE_URL + 'Persona/GenerarReportePersona/' + scope.persona.id
                }).then(function () {
                    $window.open(BASE_URL + 'Persona/AbrirReporte/', "_blank");
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
            scope.generarReporteBienesRegistrados = function () {
                showLoading();
                $http({
                    type: "POST",
                    url: BASE_URL + 'Persona/GenerarReporteBienesRegistrados/' + scope.persona.id
                }).then(function () {
                    $window.open(BASE_URL + 'Persona/AbrirReporte/', "_blank");
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
        }
    };
});
appJS.directive('resultDocumentos', function ($window) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultDocumentosTemplate.html',
        link: function (scope) {
            scope.$parent.grupo.exportable = false;
            scope.ploteable = false;
            scope.allowDocumento = $.inArray(seguridadResource.ABMDocumentos, scope.permisos) > -1 && scope.$parent.elem.dato_tieneDocumento === "S";
            scope.documento = scope.$parent.elem;
            scope.documento.selected = false;
            scope.download = function () {
                $window.location = BASE_URL + "Documento/Download/" + scope.documento.id;
            }
        }
    };
});
appJS.directive('resultDomicilios', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultDomiciliosTemplate.html',
        link: function (scope) {
            scope.$parent.grupo.exportable = false;
            scope.ploteable = false;
            scope.allowDomicilio = $.inArray(seguridadResource.ABMDomicilios, scope.permisos) > -1;
            scope.domicilio = scope.$parent.elem;
            scope.domicilio.selected = false;
            scope.abrir = function (url, id) {
                loadView(BASE_URL + url + id);
                hideSearchBlur();
            };
        }
    };
});
appJS.directive('resultCuadras', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultCuadrasTemplate.html',
        link: function (scope) {
            scope.$parent.grupo.exportable = false;
            scope.ploteable = false;
            scope.cuadra = scope.$parent.elem;
            scope.cuadra.selected = false;
        }
    };
});
appJS.directive('resultTramites', function ($window, $http) {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultTramitesTemplate.html',
        link: function (scope) {
            scope.$parent.grupo.exportable = false;
            scope.ploteable = false;
            scope.allowTramite = $.inArray(seguridadResource.ABMTramites, scope.permisos) > -1;
            scope.tramite = scope.$parent.elem;
            scope.tramite.selected = false;
            scope.abrirInformeDetallado = function () {
                showLoading();
                $http({
                    type: "POST",
                    url: BASE_URL + "MesaEntradas/GetInformeDetallado/" + scope.tramite.id
                }).then(function () {
                    $window.open(BASE_URL + "MesaEntradas/AbrirInformeDetallado/", "_blank");
                }, function () {
                    alert("error");
                }).then(hideLoading);
            };
            scope.abrirBandeja = function () {
                loadView(BASE_URL + 'MesaEntradas');
            };
        }
    };
});
appJS.directive('resultCalles', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultMensuras', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultPaf', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultParcelassaneables', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultMejoras', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultMejorasprovinciales', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultPcc', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});
appJS.directive('resultRedplani', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});

appJS.directive('resultInfraestructuras', function () {
    return {
        restrict: 'A',
        templateUrl: BASE_URL + 'Templates/resultObjetoGraficoComunTemplate.html',
        link: function (scope) {
            scope.objeto = scope.$parent.elem;
            scope.objeto.selected = false;
        }
    };
});