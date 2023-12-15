function OpcionesMapa(zoom, rotacion, selectorCapas, streetView, seleccion, toolbarExterno, impresion, medicion,
    dibujoPunto, dibujoLinea, dibujoPoligono, scaleLine, mousePosition) {
    return {
        conZoom: zoom || false,
        conRotacion: rotacion || false,
        conSelectorCapas: selectorCapas || false,
        conStreetView: streetView || false,
        conSeleccionObjetos: seleccion || false,
        conMedicion: medicion || false,
        conDibujoPunto: dibujoPunto || false,
        conDibujoLinea: dibujoLinea || false,
        conDibujoPoligono: dibujoPoligono || false,
        conImpresion: impresion || false,
        conToolbarExterno: toolbarExterno || false,
        conScaleLine: scaleLine || false,
        conMousePosition: mousePosition || false
    };
}
function Mapa(container, opciones) {

    var ui = {
        mapContainer: container,
        btnsContainer: null,
        contextMenu: null,
        controls: {
            Zoom: null,
            Rotate: null,
            Switcher: null,
            StreetView: null,
            ScaleLine: null,
            MousePosition: null,
            ToolbarLauncher: null,
            Toolbar: null
        },
        interactions: {
            CurrentActive: null,
            BBoxSelect: null,
            Select: null,
            DoubleClickZoom: null,
            Measure: null,
            Modify: null,
            Draw: null,
            StreetView: null
        }
    };

    var olLegacy = {
        v2: {
            DOTS_PER_UNIT: 72,
            INCHES_PER_METER: 39.3701,
            getScaleFromResolution: function (r) {
                return r * this.DOTS_PER_UNIT * this.INCHES_PER_METER;
            },
            getResolutionFromScale: function (s) {
                return 1 / (1 / s * this.DOTS_PER_UNIT * this.INCHES_PER_METER);
            }
        }
    };
    const MAP_SRID = 'EPSG:3857';
    let map, measureLayer, markerLayer, drawLayer, streetViewLayer, routeLayer, measureTooltipElement, measureTooltip,
        infoTooltipElement, infoTooltip, timeout, cleanUpInteraction, cancelKeyCode, topMostLayerWithFeaturesClicked,
        initialResolve = true, layerSwitcherOpen = false, externalMenuItems = {}, selectionLayerGroups;

    measureLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: 'rgba(255,255,0, 0.25)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgb(255,255,0)',
                width: 2
            }),
            image: new ol.style.Circle({
                radius: 7,
                fill: new ol.style.Fill({
                    color: 'rgba(255,255,0, 0.2)'
                })
            })
        })
    });
    routeLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'rgba(23, 199, 0, 1)',
                width: 2
            })
        })
    });

    markerLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 0.5],
                anchorXUnits: 'fraction',
                anchorYUnits: 'fraction',
                opacity: 1,
                src: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAOESURBVGhD7VdNa9RQFH1VdGF1IyIuBG0t+AGCUhdqZ1ql6FIoij9ALQouRbuQtiLiRtrpVC246EYQ0Y1a6xeIVVt0Jm3BShdKwbUfMIgfnWSKeZ4bL1JnbqbJdJJRyYFDQt65557kvZfJqAgRIkT4f6Cb1Ao7plrtuBoA3+L8m8O4eoPjHRyP2NvVcpb/PbD3qGodV2cQ8iuOuhhJA3boerWEyysLHVO1CPRaCluMmI1XmLG1bFMZ2E2qDkE+SAG9kGrBGrYLF/ZOtQzNJ6VgfujMXpNayrbhAY3PS4FKIbzOsm04wBNbhac/LYUhYuyWblC7aHM7G5zO8RaStETov9s71Eq2Dx5oeFwKQrQbVBvLCoCbOC3VEOF5jGXBA80eiCFi6jZLXIHaAbE2ru6zJHgg6JQUgpYKS1xhN6rdUi15siR4oFlWDIH1zhJX0NtLrMWeYknwQLOMGGJ+N5BhSfBAs2ExBJYHS1zhuoTgyZLggWbtLiEGWOIKLL9Bl9p2lgQPfEKsRpAZMQhelSwrAMY6XGpmyJNl4YCethSGiLG7tFSczw1iTDWD9yQt0cvMlR1oulcKUwrJi23DBRqLm9kPyYPtwgd+uGJSKF+EB9tVBljb4lvFC6mWbSoHbNL1CGJKAYuRaqiWbSoLrONzUshipBourzzoDzqe6DspqERHW+k/9Z2TenF3OrctkbZaewyr62Ff/zO7sUoMPJukIS3VUG0ylasnL7YNFsmUuSmRNtvAYTCHEHo2J9qOiqFnkzT5db+8zOc9KfNU4qW5kduVB51aL+geNQ9Q6PzG+bw8ktGfW2rF4EQaI41U+ydNzJC5X2tdxTFKQ3I0uw7Bn8pNZN68MYRlsrAgPF2jManGlWlrqNvI1nIcf+gaz9Yh/EfReA6OdZwouAG6JmnnImWgLBzLO3rS5ohk6IUXX3zRnw5u/h2ezumapPXChGE94VjekTDMrGTmlVcHJ3SuudohnUsa7zT9/930u/YlPurrdyiN+SFm4DHH8g7ewCXtgXKSMlwan17DsfyhN5WtofUnGYdBevKUgeOUBnoXY0O3lGNJeSZen9iD++b9O5CP3jFzA4xPOjeTNi2xeSmEF3k63oYZzhfqlTG9KGlYW3tS1mF801xAgOvgMJ7eFKb+PYJkEOwHkc7pmjNGnyGGec2pMaxDOG4hL7aNECFChH8KSv0EdTj0mSYk56wAAAAASUVORK5CYII='
            }),
            text: new ol.style.Text({
                fill: new ol.style.Fill({
                    color: 'rgb(0,0,0)'
                }),
                offsetY: 27,
                font: '11px Arial'
            })
        }),
        zIndex: 9999999
    });
    drawLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: 'rgba(23, 199, 0, 0.2)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(23, 199, 0, 1)',
                width: 2
            }),
            image: new ol.style.Circle({
                radius: 7,
                fill: new ol.style.Fill({
                    color: 'rgba(23, 199, 0, 1)'
                })
            })
        }),
        zIndex: 99999999
    });
    map = new ol.Map({
        interactions: ol.interaction.defaults({
            keyboard: false,
            shiftDragZoom: false, //desactivo el comportamiento por default y redefino con ALT
            doubleClickZoom: opciones.conZoom,
            mouseWheelZoom: opciones.conZoom,
            dragPan: opciones.conZoom
        }),
        controls: [createZoom(), createRotate()/*, createStreetView()*/, createScaleLine(), createMousePosition(), createToolbarLauncher(), createToolbar()]
            .filter(function (ctrl) { return !!ctrl; })
            .map(function (ctrl) { return new ctrl(); }),
        layers: [],
        target: getMapElement(),
        view: new ol.View()
    });

    map.on('pointermove', function (evt) {
        if (ui.interactions.CurrentActive || evt.dragging) {
            removeInfoTooltip();
            return;
        }
        displayFeatureInfo(evt);
    });
    map.on('singleclick', function (evt) {
        if (ui.interactions.Select && ui.interactions.Select.getActive()) {
            map.forEachFeatureAtPixel(evt.pixel, (feature, layer) => { topMostLayerWithFeaturesClicked = layer; return !!feature; }, { layerFilter: ly => ly.get('GeoConfig') || ly === routeLayer });
        } else {
            topMostLayerWithFeaturesClicked = null;
            topMostLayerLayersGroup = null;
        }
    });

    if (createContextMenu()) {
        map.getViewport().addEventListener('contextmenu', ui.contextMenu);
    }

    if (opciones.conSeleccionObjetos) {
        ui.interactions.Select = new ol.interaction.Select({
            condition: ol.events.condition.singleClick,
            multi: true,
            layers: function (layer) {
                const isLayerByName = (ly, name) => name === ({ ...{ NombreFisico: "ROUTE_LAYER" }, ...ly.get('GeoConfig') }).NombreFisico;
                const groupLayerConfig = topMostLayerWithFeaturesClicked && selectionLayerGroups.find(grp => isLayerByName(topMostLayerWithFeaturesClicked, grp.principal));

                return (topMostLayerWithFeaturesClicked === layer || groupLayerConfig && groupLayerConfig.relacionadas.some(rel => isLayerByName(layer, rel))) && layer !== measureLayer && layer !== drawLayer && layer !== markerLayer;
            },
            filter: function () {
                return true;//!drawingMap; //esto se usa para evitar que se seleccione cuando se está en modo "dibujo"
            },
            style: new ol.style.Style({
                stroke: new ol.style.Stroke({ color: [0, 0, 255, 1], width: 3 }),
                fill: new ol.style.Fill({ color: [58, 95, 205, 0.1] }),
                image: new ol.style.Circle({
                    radius: 7,
                    stroke: new ol.style.Stroke({ color: [0, 0, 255, 1], width: 2 }),
                    fill: new ol.style.Fill({ color: [58, 95, 205, 0.4] })
                })
            })
        });
        ui.interactions.Select.on('select', function (evt) {
            if (!evt.mapBrowserEvent.originalEvent.shiftKey) {
                publishObjectEvent('buscarseleccion', getSelection());
            }
        });
        ui.interactions.BBoxSelect = new ol.interaction.DragBox({ condition: ol.events.condition.shiftKeyOnly });
        ui.interactions.BBoxSelect.on('boxstart', function () {
            document.addEventListener("keyup", autoSearch);
        });
        ui.interactions.BBoxSelect.on('boxend', function (evt) {
            var extent = evt.target.getGeometry().getExtent(),
                currentSelection = ui.interactions.Select.getFeatures().getArray().map(function (f) { return f.getId(); });
            evt.target.getMap()
                .getLayerGroup()
                .getLayersArray()
                .filter(function (ly) { return ly.get('GeoConfig') && ly.getVisible() && ly.getSource().forEachFeatureIntersectingExtent; })
                .forEach(function (ly) {
                    ly.getSource()
                        .forEachFeatureIntersectingExtent(extent, function (f) {
                            if (currentSelection.indexOf(f.getId()) === -1) {
                                ui.interactions.Select.getFeatures().push(f);
                            }
                        });
                });
        });
        map.addInteraction(ui.interactions.Select);
        map.addInteraction(ui.interactions.BBoxSelect);
    }

    if (opciones.conZoom) {
        map.addInteraction(new ol.interaction.DragZoom({ condition: ol.events.condition.altKeyOnly }));
        ui.interactions.DoubleClickZoom = map.getInteractions().getArray().filter(function (it) { return it instanceof ol.interaction.DoubleClickZoom; })[0];
    }
    function getOLStyles(rule) {
        var olStyles = [];
        for (var elem in rule.styles) {
            var options = { fill: null, circle: null, stroke: null, icon: null };
            var style = rule.styles[elem];
            if (style.circle) {
                options.image = new ol.style.Circle({
                    radius: style.circle.radius,
                    fill: new ol.style.Fill({ color: style.circle.fill.color }),
                    stroke: new ol.style.Stroke({ color: style.circle.stroke.color, width: style.circle.stroke.width })
                });
            }
            if (style.shape) {
                var shape = {
                    radius: style.shape.radius,
                    points: style.shape.points,
                    fill: new ol.style.Fill({ color: style.shape.fill.color }),
                    stroke: new ol.style.Stroke({ color: style.shape.stroke.color, width: style.shape.stroke.width })
                };
                if (style.shape.rotation) shape.rotation = style.shape.rotation;
                if (style.shape.radius2) shape.radius2 = style.shape.radius2;
                if (style.shape.points2) shape.points2 = style.shape.points2;
                if (style.shape.angle) shape.angle = style.shape.angle;
                options.image = new ol.style.RegularShape(shape);
            }
            if (style.icon) {
                options.image = new ol.style.Icon({
                    anchor: style.icon.anchor || [0.5, 0.5],
                    anchorXUnits: style.icon.xunits || 'fraction',
                    anchorYUnits: style.icon.yunits || 'fraction',
                    opacity: 1,
                    size: style.icon.size || [16, 16],
                    scale: style.icon.scale,
                    rotateWithView: true,
                    rotation: style.icon.rotation || 0,
                    src: style.icon.embedded || BASE_URL + 'content/images/' + style.icon.name
                });
                options.geometry = function (feature) {
                    var geom = feature.getGeometry();
                    switch (geom.getType()) {
                        case 'Polygon':
                            geom = new ol.geom.Point(geom.getInteriorPoint().getCoordinates());
                            break;
                        case 'MultiPolygon':
                            geom = new ol.geom.Point(geom.getFlatInteriorPoints().slice(0, 2));
                            break;
                        case 'LineString':
                            geom = new ol.geom.Point(geom.getCoordinateAt(0.5));
                            break;
                        default:
                            break;
                    }
                    return geom;
                };
            }
            if (style.fill) { options.fill = new ol.style.Fill({ color: style.fill.color }); }
            if (style.stroke) {
                var stroke = {
                    color: style.stroke.color,
                    width: style.stroke.width
                };
                if (style.stroke.linedash) stroke.lineDash = style.stroke.linedash;
                if (style.stroke.linecap) stroke.lineCap = style.stroke.linecap;
                if (style.stroke.linejoin) stroke.lineJoin = style.stroke.linejoin;
                options.stroke = new ol.style.Stroke(stroke);
            }
            olStyles.push(new ol.style.Style(options));
        }
        if (rule.text) {
            var textStyle = new ol.style.Text({ fill: new ol.style.Fill({ color: rule.text.color }), font: rule.text.font });
            if (rule.text.align) textStyle.setTextAlign(rule.text.align);
            if (rule.text.baseline) textStyle.setTextBaseline(rule.text.baseline);
            if (rule.text.placement) textStyle.setPlacement(rule.text.placement);
            if (rule.text.shadow) textStyle.setStroke(new ol.style.Stroke({ color: rule.text.shadow.color, width: rule.text.shadow.width }));
            if (rule.text.offsetX) textStyle.setOffsetX(rule.text.offsetX);
            if (rule.text.offsetY) textStyle.setOffsetY(rule.text.offsetY);
            if (!olStyles.length) {
                olStyles.push({});
            }
            olStyles[0].setText(textStyle);
        }
        return olStyles;
    }

    function getStylesFromRules(rules, nombreLayer) {
        return (function (layerStyles, nombreLayer) {
            return function (feature, resolution) {
                var mpu = map.getView().getProjection().getMetersPerUnit(), //esto para el caso que la unidad no sea 'metro'
                    scale = Math.round(olLegacy.v2.getScaleFromResolution(resolution) * mpu),
                    rule = layerStyles[0].rule,
                    styles = layerStyles[0].styles;

                feature.set("nombreLayer", nombreLayer, true);

                if (layerStyles.length > 1) {
                    var matches = layerStyles.filter(function (obj) {
                        /* EVALUO SEGUN TIPO, POR DEFAULT SOLO SE TIENE EN CUENTA ESCALA */
                        var typeMatched;
                        switch (obj.rule.type.toLowerCase()) {
                            case 'field':
                                typeMatched = obj.rule
                                    .fields
                                    .reduce(function (accum, field, idx) { return accum && ('' + (feature.get(field) || '')).toLowerCase() === ('' + (obj.rule.filters[idx] || '')).toLowerCase(); }, true);
                                break;
                            default:
                                typeMatched = true;
                                break;
                        }
                        return (!obj.rule.scale || obj.rule.scale >= scale) && typeMatched;
                    });
                    if (!matches.length) {
                        return null;
                    }
                    rule = matches[matches.length - 1].rule;
                    styles = matches[matches.length - 1].styles;
                }
                if (rule.text) {
                    styles[0].getText().setText(feature.get(rule.text.field) + '');
                }
                return styles;
            };
        })(rules.map(function (rule) { return { rule: rule, styles: getOLStyles(rule) }; }), nombreLayer);
    }

    function autoSearch(evt) {
        if (evt.keyCode === 16) { //shift
            document.removeEventListener("keyup", autoSearch);
            publishObjectEvent('buscarseleccion', getSelection());
        }
    }
    function getMapElement() {
        return document.getElementById(ui.mapContainer);
    }
    function getMapControlsElement() {
        if (!ui.btnsContainer) {
            ui.btnsContainer = 'mapbtns' + Date.now();
            getMapElement().insertAdjacentHTML('afterend', '<aside id="' + ui.btnsContainer + '" class="map-controls"><ul></ul></aside>');
        }
        return document.getElementById(ui.btnsContainer);
    }
    function getMenuContainerElement() {
        var elem = getMapElement().querySelector('div.ol-context-menu');
        if (elem) {
            elem.remove();
        }
        elem = getMapElement().appendChild(document.createElement('div'));
        elem.className = 'ol-context-menu dropdown clearfix';
        return elem;
    }
    function getToolbarContainerElement() {
        var toolbarContainer = getMapElement().querySelector('.map-controls.toolbar');
        if (!toolbarContainer) {
            toolbarContainer = document.createElement('div');
            toolbarContainer.className = 'map-controls toolbar';
            getMapElement().insertAdjacentElement('afterend', toolbarContainer);
        }
        return toolbarContainer;
    }
    function getStreetViewElement() {
        var overlay = getMapElement().querySelector('.street-view-overlay');
        if (overlay) {
            overlay.remove();
        }
        overlay = getMapElement().appendChild(document.createElement('div'));
        overlay.className = 'street-view-overlay';
        var closebar = overlay.appendChild(document.createElement('div')),
            icon = closebar.appendChild(document.createElement('i')),
            streetviewElem = overlay.appendChild(document.createElement('div'));

        closebar.className = "closebar";
        icon.className = 'fa fa-times';
        icon.addEventListener('click', function () { stopStreetView(overlay); });
        streetviewElem.className = 'street-view-canvas';
        publishObjectEvent('iniciarstreetview', { contenedor: overlay, target: closebar });
        return streetviewElem;
    }
    function getRouteImage(cerrado, inicio, fin) {
        var image, size = { width: 32, height: 37 };
        if (cerrado && inicio) { //mismo inicio y fin
            image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAlCAYAAAAjt+tHAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4AcWEAAVbDki6gAAAkZJREFUWMPFl0+KE0EUxn+VBBPiZML4BxVcDIE5gXqDwr2gg+B6wBvkDH0DIesB0b2rdwVPIAQXgg5qZCZx0pGEuMirWAndSVfShW/TdJrU76v3vm7qM2hZaxtADbip1wblVgpMgd/AVERSAOPBD4A2cKgi6kC1JPgMmCj8CrgERiKSGg9+C3gAHAEtFVBmTYAh8Av4CgyAUU3b3Vb4iZxKj4hl39kzryupm/khcCSn0uMYaEZywDUkIr2utc90FKOKZ7wWRIKzWHP8dHnXcmZ3bq8vZ96I0/r545Vbx2vU9IdMt88fzTMXMx/NPnB8Zm3THx3ICQkFb4AvqxLT8dvgUQUUgUcTUBQeRUAIvHQBofBSBewCL03ArvCt34H1D1HW92AfeOEPUeGdz+cbFjPhAnZuuzFxPbBv2/cSUCY8eASF4Vle2NcDQTsv2wNltz1IQEy4L2D2H+AzJyDVM/tkeXqNBE9XM8IESCteXBrC4ug83hVuTK4B9VTuaugimrHWHgD3gIfASSLZweRYo1ORGgCfc551rT0DPgFfgIsi0awKkIi8KSLCwbvWvl7zV2Y0KxJO3Rn+RiJyvkmEB38F/PFmnR9Ot8Tzql6bLr4lIm+zRHjwl7rLKx176nJgbjzfGCYXwtrqk7sq4r0vwoO/UPh34AK4dKC82pr/+/3+tNPpzFS9ASrS6Xx40u8/rwPj1Z0PQuCFOpDTifvAnUTknH8z/wF8C4EHCVgTcVsn0NRH17r7nyHwYAEZb0zde8WWzg5Z7y/oz+QLmVsgUQAAAABJRU5ErkJggg==";
        } else if (inicio) { // inicio
            image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAlCAYAAAAjt+tHAAAB50lEQVRYhe2XMWrjQBSGvxFBGGPMkiJlyAXCsqRath0fYBsd0k0OoGlDKhPCXiCk3MKEYIwRQpNC70l2LNuaIFlNHgzjaEDfp8mvQc947wGYzWYXQATEMl/QbeVAAWRAkaZpDmC89wqPgZGMWASijuCFCGTARkaWpmlurLUKHwNTmUf0swMbYA28y5zpU44EfuUS99gxeKfs3P6WnwWQG2vtROCXLnH/uKHegy5Ln/0F7NzeAkvgfTt4JbIPOHv31JxFmnYd/cD3JSqmpryrtIdUNBR432LICn/XX0+sX/ctcA3+zjcumYUJvt2XdsC8liD/txQx91vgc+yAijReD6zBQ/gt8C0wuEB3J6FeH+IkrA6kc5+EjXW2k7CjGjyEKlAMwC5UIN8a5ddrX1Xfu2JG1O1SubzuSUI/y+u/MqAw1toYmAA/ONaY3ACXLWFL4KV5SRqT/8AbsGrTmkUALnEPrSQEbuf2j1zRfDW2Zm2a0+ob3iVucVSiht+xm63DzemJ9lxnlRu7xD01StTwX9RJyj4JNLfnx0rERpQ5mYjE845EDf8p8JWMjYIO1UmBAxKTaifYefIVAfDWAg0SU5FYQPU/X1GGqzU8SOCTxFhGLEsZ5davQ+DBAlsSGkp9VfUVy0LgAB/hmt8QWSHeLgAAAABJRU5ErkJggg==";
        } else if (fin && !cerrado) { //fin
            image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAlCAYAAAAjt+tHAAAB20lEQVRYhe2Xz0qEUBSHPyVEhiFiFi2jF4iIVtH2zjPNM/hM3m20ioheIFq2iIghRERbeM7MnUnNG5oEHbjc+SN+n9efck9QVRUAy+XyAAiBSOYDhq0CKIEcKNM0LQCCqqoUHgGxjEgEwoHgpQjkQCYjT9O0CIwxCp8BhzLHjLMCGfABvMuc61XGAj9OrL0dGLxTK2Ou5GMJFIExZi7wRWLt4ynbJRiy9NKfaokz4BV4d4MXMxKcr+fUnIWadh2jwBskNkxN+VBp96lwKvBXiynL61l/7nncyVgCACfy6m6tIPA634/fds8doFFX4KegtvpbIdyvtlD+yi1oDeN/CD1r8hD+C3hnoCt8owsMEbr9mvwWqEA5AbtUgcIZZCMSnXNvmCHbdimDeus8hoRuy52vOVAGxpgImANHdDQmp8CiJ+yVev/fVNKYvABvwLpPaxYCJNbe9JFQ+MqYa/lJ89XYmvVpTjd7+MTauy4JB37Jbrbam9Nv2nOdVW6WWHvfJOHAL9hGKd8TaG7Pu0rEYuqczEXiwZVw4OcCX8vIFNRW3wq0SMx1Jdi98jUe8N4CDRKHInEHm3u+pg5Xb7iXwJ7ETEYkf+XUS//hA/cWcCQ0lPqo6iOW+8ABPgE+mtUhukI3ZQAAAABJRU5ErkJggg==";
        } else { //intermedio
            image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABIAAAASCAYAAABWzo5XAAAABmJLR0QALAAnAIVdol6wAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4AoMDhc3bVYaqAAAAB1pVFh0Q29tbWVudAAAAAAAQ3JlYXRlZCB3aXRoIEdJTVBkLmUHAAACOElEQVQ4y52US2tTYRCGn3dyaPDWhZciaSxYRRRxG5AKdS2IEKUi6A9wr5v8BcGFINKNii7EghERXbiQKtRF8QJeMEhrpSbZWC+F5Jzv5CTnc5FSb1XbPtth3pn3hRmxBLkr4QgwJO+LZCzvAbm4as6V1WxMzJZ2jv3eI4Dcpc/UT28id7nZT8YmkXKLVYGSDooiFEZY3EJhWNfcl8LM6HBtx4m7TN843BUCyF38NMyGDeMCvAF9HrL6aaTQvCN4VsMaEYocmps7OH3zyKMfG11p9JMJqnig18NG8VdMBC9m6Xn6AToJFkb5d7eP1qxbzEyKBZHN/xABSD1JYTvJ3q1Y5FDsJgGUuxqOyOymN2AbyydjrD9/B/vWgFZ83EBDnoVMVkKaEh3dj6IIi9yQgS8Cvwa7HDy0B/pQkiDnioaUx1gV8h6lHdTu5E2A2h1WTZriBUbcqip0IK1Yw5sgTgBVzZwrK4zQfLxCXyKYqi248WVTozmhVovgeQ1sBVuZ0XuhvNCjCZstDY5ZGNatERK8+IjPBv+3tC7L2rGHqOlAqr+plMYMQJ+/FhTF9DyboedxBTIGWtoOQYa11x6w5s6TbtZQWLw1gMGT94YNG7cowpPiivtpD2xB3i8GG0zVunaaDgHtbPZg5eWZH0c7eOo+768fYtexW/0KW5NyLqfIoaSF0g6kKcRJN1gTSHWg8LpSqu3Zd463r84uaYDdB0ZHzLkhOVdU0s57AagqKHuYeFMp/fHYvgNCUPPD2g/T8wAAAABJRU5ErkJggg==";
            size = { width: 18, height: 18 };
        }
        return { url: image, size: size };
    }
    function viewToolbar() {
        return !opciones.conToolbarExterno &&
            (opciones.conZoom || opciones.conMedicion ||
                opciones.conImpresion || opciones.conDibujoPunto ||
                opciones.conDibujoLinea || opciones.conDibujoPoligono);
    }
    function createZoom() {
        if (!opciones.conZoom) return null;
        ui.controls.Zoom = function () {
            var zoom = function (e, factor) {
                e.preventDefault();
                var view = map.getView(),
                    resolution = view.getResolution();
                view.cancelAnimations();
                view.animate({ resolution: Math.min(view.maxResolution_, Math.max(view.minResolution_, resolution * factor)), duration: 300 });
            },
                handleZoomIn = function (e) {
                    zoom(e, 0.5);
                },
                handleZoomOut = function (e) {
                    zoom(e, 2);
                },
                element = getMapControlsElement(),
                zoomInLi = element.querySelector('ul').appendChild(document.createElement('li')),
                zoomOutLi = element.querySelector('ul').appendChild(document.createElement('li')),
                zoomInBtn = zoomInLi.appendChild(document.createElement('button')),
                zoomOutBtn = zoomOutLi.appendChild(document.createElement('button'));

            zoomInLi.className = 'map-control zoom-in';
            zoomOutLi.className = 'map-control zoom-out';
            zoomInBtn.innerHTML = '<i class="icon-in"></i>';
            zoomOutBtn.innerHTML = '<i class="icon-out"></i>';

            zoomInBtn.setAttribute("title", "Acercarse");
            zoomInBtn.addEventListener('click', handleZoomIn, false);
            zoomInBtn.addEventListener('touchstart', handleZoomIn, false);
            zoomOutBtn.setAttribute("title", "Alejarse");
            zoomOutBtn.addEventListener('click', handleZoomOut, false);
            zoomOutBtn.addEventListener('touchstart', handleZoomOut, false);

            ol.control.Control.call(this, { element: element });
        };
        ol.inherits(ui.controls.Zoom, ol.control.Zoom);
        return ui.controls.Zoom;
    }
    function createRotate() {
        if (!opciones.conRotacion) return null;
        ui.controls.Rotate = function () {
            var _currentRotation = 0,
                restore = function (e) {
                    e.preventDefault();
                    var view = map.getView();
                    view.cancelAnimations();
                    view.animate({ rotation: 0, duration: 300 });
                },
                render = function (e) {
                    if (!!e.frameState && e.frameState.viewState.rotation !== _currentRotation) {
                        _currentRotation = e.frameState.viewState.rotation;

                        var label = btn.querySelector('i'),
                            transform = "rotate(" + _currentRotation + "rad)";

                        label.style.msTransform = transform;
                        label.style.webkitTransform = transform;
                        label.style.transform = transform;
                    }
                },
                element = getMapControlsElement(),
                li = document.createElement('li'),
                btn = li.appendChild(document.createElement('button'));

            element.querySelector('ul').insertAdjacentElement('afterbegin', li);

            /* IE, para variar, no soporta classList("c1","c2",...,"cN") */
            li.classList.add('map-control');
            li.classList.add('north-rotation');
            btn.innerHTML = '<i class="icon-mover"></i>';

            btn.setAttribute("title", "Restaurar Rotación");
            btn.addEventListener('click', restore, false);
            btn.addEventListener('touchstart', restore, false);

            ol.control.Control.call(this, { element: element, autoHide: true, render: render });
        };
        ol.inherits(ui.controls.Rotate, ol.control.Rotate);
        return ui.controls.Rotate;
    }
    //function createStreetView() {
    //    if (!opciones.conStreetView) return null;

    //    ui.controls.StreetView = function () {
    //        var element = getMapControlsElement(),
    //            li = document.createElement('li'),
    //            btn = li.appendChild(document.createElement('button'));

    //        /* siempre lo dejo primero */
    //        element.querySelector('ul').insertAdjacentElement('afterbegin', li);

    //        /* IE, para variar, no soporta classList("c1","c2",...,"cN") */
    //        li.classList.add('map-control');
    //        li.classList.add('street-view');
    //        btn.innerHTML = '<i class="icon-street-view"></i>';

    //        btn.setAttribute("title", "Iniciar StreetView");
    //        btn.addEventListener('click', startStreetView, false);
    //        btn.addEventListener('touchstart', startStreetView, false);

    //        ol.control.Control.call(this, { element: element });
    //    };
    //    ol.inherits(ui.controls.StreetView, ol.control.Control);
    //    return ui.controls.StreetView;
    //}
    function createSwitcher() {
        if (!opciones.conSelectorCapas) return null;

        ui.controls.Switcher = function (opts) {
            var options = $.extend({ layersHierarchy: {} }, opts),
                selector = document.createElement("div"),
                element = getMapControlsElement(),
                li = document.createElement('li'),
                btn = li.appendChild(document.createElement('button'));

            /* siempre lo dejo último */
            element.querySelector('ul').insertAdjacentElement('beforeend', li);
            var splitter = document.createElement("input");
            splitter.type = "range";
            splitter.className = "hidden map-splitter";
            getMapElement().insertAdjacentElement('afterbegin', splitter);
            splitter.addEventListener('input', function () { map.render(); }, false);

            /* IE, para variar, no soporta classList.add("c1","c2",...,"cN") */
            li.classList.add('map-control');
            li.classList.add('switcher');
            btn.innerHTML = '<i class="icon-001"></i>';

            selector.classList.add('layer-selector');
            selector.classList.add('closed');
            element.insertAdjacentElement('afterend', selector);

            var open = function () {
                selector.classList.remove("closed");
                selector.classList.add("opened");
                selector.querySelector("label.form-check-label").focus();
                layerSwitcherOpen = true;
            },
                close = function () {
                    selector.classList.remove("opened");
                    selector.classList.add("closed");
                    layerSwitcherOpen = false;
                },
                setCollapsable = function (elem) {

                },
                toggleSplitter = function (groupTree) {
                    if (groupTree[groupTree.length - 1].querySelectorAll('[raster][type=checkbox]:checked').length > 1) {
                        splitter.classList.remove('hidden');
                    } else {
                        splitter.classList.add('hidden');
                    }
                },
                layerVisibilityToggle = function (evt) {
                    setLayerVisibility(evt.target);
                    setIndeterminateParents(evt.target);
                },
                rasterVisibilityToggle = function (evt) {
                    var parents = getParents(evt.target, "ul");
                    parents[0].querySelectorAll("[type='checkbox']:not([obj-id='" + evt.target.getAttribute("obj-id") + "'])")
                        .forEach(function (chk) {
                            chk.checked = false;
                            setLayerVisibility(chk);
                        });

                    setLayerVisibility(evt.target);
                    setIndeterminateParents(evt.target);
                    toggleSplitter(parents);
                },
                rasterGroupVisibilityToggle = function (evt) {
                    setGroupVisibility(evt.target);
                    setIndeterminateParents(evt.target);
                    toggleSplitter(getParents(evt.target, '.layer-group'));
                },
                groupVisibilityToggle = function (evt) {
                    setGroupVisibility(evt.target);
                    setIndeterminateParents(evt.target);
                },
                setIndeterminate = function (parent) {
                    var input = parent.querySelector(".toggle-switcher > input"),
                        children = parent.querySelectorAll(".layer input"),
                        childrenChecked = parent.querySelectorAll(".layer input:checked"),
                        isIndeterminate = children.length !== childrenChecked.length && childrenChecked.length !== 0;

                    input.checked = isIndeterminate || childrenChecked.length !== 0;
                    input.indeterminate = isIndeterminate;
                },
                setIndeterminateChildren = function (groups) {
                    groups.forEach(setIndeterminate);
                },
                setIndeterminateParents = function (target) {
                    getParents(target, ".layer-group").forEach(setIndeterminate);
                },
                setLayerVisibility = function (target) {
                    var id = Number(target.getAttribute("obj-id"));
                    var ly = map.getLayerGroup()
                        .getLayersArray()
                        .filter(function (ly) { return ly.get("GeoConfig") && ly.get("GeoConfig").IdCapa === id; })[0],
                        geoConfig = ly.get('GeoConfig');

                    if (!geoConfig.isResolution) {
                        geoConfig.lastState = target.checked;
                        geoConfig.userControlled = true;
                    }
                    geoConfig.isResolution = null;
                    target.checked = !!geoConfig.lastState;
                    ly.setVisible(target.checked);
                    var ulTematico = getParents(target, '.layer')[0].querySelector('ul.tematico');
                    if (ulTematico && target.checked) {
                        ulTematico.classList.remove('closed');
                    } else if (ulTematico) {
                        ulTematico.classList.add('closed');
                    }
                    target.indeterminate = false;
                },
                setGroupVisibility = function (target) {
                    var id = Number(target.getAttribute("obj-id")),
                        grupoRaster = selector.querySelectorAll(".layer-group > ul[gid='" + id + "'] [raster]");
                    target.indeterminate = false;
                    if (grupoRaster.length) {
                        var raster = {};
                        grupoRaster.forEach(function (input) {
                            var grp = getParents(input, ".layer-group")[0],
                                grpid = grp.querySelector("ul").getAttribute("gid");

                            input.checked = target.checked && isNaN(Number(raster[grpid]));
                            raster[grpid] = 1;//solo prendo el primero de cada grupo
                            setLayerVisibility(input);
                            setIndeterminate(grp);
                        });
                        getParents(grupoRaster[grupoRaster.length - 1], '.layer-group').slice(1, -1).forEach(setIndeterminate);
                        toggleSplitter(getParents(grupoRaster[grupoRaster.length - 1], 'ul'));
                    } else {
                        selector.querySelectorAll(".layer-group > ul[gid='" + id + "'] > li > .layer:not(.out-of-range) input")
                            .forEach(function (input) {
                                input.checked = target.checked;
                                setLayerVisibility(input);
                            });
                    }
                    selector.querySelectorAll(".layer-group > ul[gid='" + id + "'] > li > .layer-group > div > input")
                        .forEach(function (grp) {
                            grp.checked = target.checked;
                            setGroupVisibility(grp);
                        });
                },
                visibilityDiv = function (id, defaultVisible, raster, grupo) {
                    var div = document.createElement("div"),
                        chk = document.createElement("input"),
                        label = document.createElement("label");
                    div.className = "toggle-switcher toggle-switcher-visible pull-left";
                    chk.setAttribute("id", id);
                    if (raster) {
                        chk.setAttribute("raster", grupo);
                    }
                    chk.setAttribute("type", "checkbox");
                    chk.setAttribute("obj-id", id.split('-').reverse()[0]);
                    chk.checked = defaultVisible;
                    label.setAttribute("for", id);
                    div.appendChild(chk);
                    div.appendChild(label);
                    return div;
                },
                groupLayer = function (layer, ul) {
                    var containerDiv = document.createElement('div'),
                        label = document.createElement('label'),
                        id = 'lid-visible-' + Date.now() + '-' + layer.IdCapa;

                    containerDiv.className = 'layer';
                    label.className = 'form-check-label';
                    label.innerText = layer.NombreCapa;
                    label.setAttribute('for', id);

                    containerDiv.appendChild(visibilityDiv(id, layer.VisibleDefault, layer.esLayerBase, layer.IdGrupoPadre));
                    containerDiv.querySelector('input').addEventListener('click', layer.esLayerBase ? rasterVisibilityToggle : layerVisibilityToggle, true);
                    containerDiv.appendChild(label);
                    if ((layer.ConfiguracionTematico || []).length) {
                        var ulLeyenda = document.createElement('ul');
                        ulLeyenda.className = 'tematico';
                        if (!layer.VisibleDefault) {
                            ulLeyenda.classList.add('closed');
                        }
                        eval('(' + layer.ConfiguracionTematico + ')')
                            .forEach(function (item) {
                                var img = document.createElement('img'),
                                    text = document.createElement('span'),
                                    itemDiv = document.createElement('div'),
                                    liLeyenda = document.createElement('li');

                                img.src = item.image;
                                text.innerText = item.text;
                                itemDiv.appendChild(img);
                                itemDiv.appendChild(text);
                                liLeyenda.appendChild(itemDiv);
                                ulLeyenda.appendChild(liLeyenda);
                            });
                        containerDiv.appendChild(ulLeyenda);
                    }
                    var li = document.createElement('li');
                    li.appendChild(containerDiv);
                    ul.appendChild(li);
                },
                groupHeader = function (header, raster, containerLi) {
                    var containerDiv = document.createElement("div"),
                        label = document.createElement("label"),
                        icon = document.createElement("i"),
                        id = "gid-visible-" + Date.now() + "-" + header.id;

                    containerDiv.className = "layer-group";
                    containerDiv.appendChild(icon);
                    containerDiv.appendChild(visibilityDiv(id, true, false));

                    label.className = "form-check-label";
                    label.innerText = header.nombreGrupo;
                    label.setAttribute("for", id);
                    icon.className = "fa fa-chevron-up pull-right closed";
                    icon.addEventListener('click', function (evt) {
                        selector.removeEventListener('mouseleave', close);
                        evt.target.classList.toggle('closed');
                        evt.target.parentElement.querySelector('ul').classList.toggle('closed');
                        getSiblings(getParents(evt.target, 'li')[0]).forEach(function (li) {
                            var layerGroup = li.querySelector('.layer-group');
                            if (layerGroup && layerGroup.children.length) {
                                layerGroup.querySelector('i').classList.add('closed');
                                layerGroup.querySelector('ul').classList.add('closed');
                            }
                        });
                        setTimeout(function () {
                            selector.addEventListener('mouseleave', close);
                        }, 250);
                    });
                    containerDiv.appendChild(label);
                    containerDiv.querySelector("input").addEventListener("click", raster ? rasterGroupVisibilityToggle : groupVisibilityToggle);
                    containerLi.appendChild(containerDiv);

                    return containerDiv;
                },
                treeGroup = function (grupo, ul) {
                    var li = document.createElement("li"),
                        ulPadre = selector.querySelector("ul[gid='" + grupo.padre + "']") || ul;

                    ulPadre.appendChild(li);
                    var ulHijo = groupHeader(grupo, grupo.layers.some(function (ly) { return ly.esLayerBase; }), li).appendChild(document.createElement("ul"));
                    ulHijo.setAttribute("gid", grupo.id);
                    ulHijo.className = "closed";
                    for (var subg in grupo.subgrupos) {
                        treeGroup(grupo.subgrupos[subg], ulHijo);
                    }
                    for (var layer in grupo.layers) {
                        groupLayer(grupo.layers[layer], ulHijo);
                    }
                },
                process = function (ul, groups) {
                    for (var root in groups) {
                        treeGroup(groups[root], ul);
                    }
                    setIndeterminateChildren(ul.querySelectorAll("li > .layer-group"));
                },
                buildTree = function (groups) {
                    var ul = document.createElement("ul");
                    selector.appendChild(ul);
                    process(ul, groups);
                },
                getParents = function (child, selector) {
                    if (!Element.prototype.matches) {
                        Element.prototype.matches =
                            Element.prototype.matchesSelector ||
                            Element.prototype.mozMatchesSelector ||
                            Element.prototype.msMatchesSelector ||
                            Element.prototype.oMatchesSelector ||
                            Element.prototype.webkitMatchesSelector ||
                            function (selector) {
                                var matches = (this.document || this.ownerDocument).querySelectorAll(selector),
                                    i = matches.length;
                                while (--i >= 0 && matches.item(i) !== this) { } //NO BORRAR
                                return i > -1;
                            };
                    }
                    var parents = [];
                    for (; child && child !== document; child = child.parentNode) {
                        if (selector) {
                            if (child.matches(selector)) {
                                parents.push(child);
                            }
                            continue;
                        }
                        parents.push(child);
                    }
                    return parents;
                },
                getSiblings = function (elem) {
                    var siblings = [];
                    var sibling = elem.parentElement.firstChild;

                    while (sibling) {
                        if (sibling !== elem) {
                            siblings.push(sibling);
                        }
                        sibling = sibling.nextSibling;
                    }
                    return siblings;
                };

            var groups = Object.keys(options.layersHierarchy).reduce(function (accum, elem) { return [...accum, options.layersHierarchy[elem]]; }, []);
            selector.innerHTML = "";
            buildTree(groups);

            btn.addEventListener('mousemove', open);
            selector.addEventListener('mouseleave', close);

            map.getView().on('change:resolution', function (evt) {
                var resolution = evt.target.getResolution(),
                    mpu = evt.target.getProjection().getMetersPerUnit(), //esto para el caso que la unidad no sea 'metro'
                    scale = Math.round(olLegacy.v2.getScaleFromResolution(resolution) * mpu);
                resolutionChanged(scale);
            });

            this.addTempLayers = function (layersHierarchy) {
                var groups = Object.keys(layersHierarchy).reduce(function (accum, elem) { return [...accum, layersHierarchy[elem]]; }, []);
                for (var grp in groups) {
                    var ul = selector.querySelector("ul[gid='" + groups[grp].id + "']");
                    if (!ul) {
                        process(selector.querySelector('ul'), [groups[grp]]);
                    } else {
                        for (var layer in groups[grp].layers) {
                            groupLayer(groups[grp].layers[layer], ul);
                        }
                    }
                }
            };

            ol.control.Control.call(this, { element: element });
        };
        ol.inherits(ui.controls.Switcher, ol.control.Control);
        return ui.controls.Switcher;
    }
    function createScaleLine() {
        if (!opciones.conScaleLine) return null;
        ui.controls.ScaleLine = function () {
            return new ol.control.ScaleLine({ units: 'metric' });
        };
        return ui.controls.ScaleLine;
    }
    function createMousePosition() {
        if (!opciones.conMousePosition) return null;
        ui.controls.MousePosition = function () {
            return new ol.control.MousePosition({ coordinateFormat: ol.coordinate.createStringXY(5), projection: APP_SRID });
        };
        return ui.controls.MousePosition;
    }
    function createToolbarLauncher() {
        if (!viewToolbar()) return null;
        ui.controls.ToolbarLauncher = function () {
            var toolbarContainer = getToolbarContainerElement(),
                launcherBtn = toolbarContainer.appendChild(document.createElement('button'));

            launcherBtn.className = 'map-control';
            launcherBtn.innerHTML = '<i class="fa fa-2x icon-00"></i>';
            launcherBtn.addEventListener('mousemove', function () {
                var toolbar = getMapElement().getElementsByClassName('map-toolbar')[0];
                toolbar.classList.remove("closed");
                toolbar.classList.add("opened");
            });
            ol.control.Control.call(this, { element: toolbarContainer });
        };
        ol.inherits(ui.controls.ToolbarLauncher, ol.control.Control);
        return ui.controls.ToolbarLauncher;
    }
    function createToolbar() {
        if (!viewToolbar()) return null;

        ui.controls.Toolbar = function () {
            var toolbarContainer = getToolbarContainerElement(),
                toolbar = toolbarContainer.appendChild(document.createElement('div'));

            toolbar.className = 'map-toolbar closed';
            toolbar.addEventListener('mouseleave', function () {
                this.classList.remove("opened");
                this.classList.add("closed");
            });

            ul = toolbar.appendChild(document.createElement('ul'));

            var addTool = function (insert, title, icon, handler) {
                if (!insert) return;
                var tool = ul.appendChild(document.createElement('li')),
                    toolBtn = tool.appendChild(document.createElement('button'));
                tool.className = 'map-tool';
                toolBtn.innerHTML = '<i class="fa fa-2x ' + icon + '"></i>';
                toolBtn.setAttribute('title', title);

                toolBtn.addEventListener('click', handler, false);
                toolBtn.addEventListener('touchstart', handler, false);
            };

            addTool(!!opciones.conZoom, 'Vista Inicial', 'icon-globe', function () { alert('vista inicial'); });
            addTool(!!opciones.conImpresion, 'Imprimir', 'icon-print', print);
            addTool(!!opciones.conMedicion, 'Medir Distancia', 'icon-medir', function () { measure('LineString'); });
            addTool(!!opciones.conMedicion, 'Medir Superficie', 'icon-superficie', function () { measure('Polygon'); });
            addTool(!!opciones.conMedicion, 'Medir Área Circular', 'icon-area-circ', function () { measure('Circle'); });
            addTool(!!opciones.conDibujoPunto, 'Dibujar Punto', 'icon-punto', function () { draw('Point'); });
            addTool(!!opciones.conDibujoLinea, 'Dibujar Línea', 'icon-linea', function () { draw('LineString'); });
            addTool(!!opciones.conDibujoPoligono, 'Dibujar Polígono', 'icon-cubo', function () { draw('Polygon'); });
            addTool(!!opciones.conDibujoPoligono, 'Dibujar Círculo', 'icon-dibujar-circ', function () { draw('Circle'); });
            addTool(!!opciones.conSeleccionObjetos || !opciones.conMedicion, 'Limpiar Mapa', 'icon-erase-2', cleanMap);

            ol.control.Control.call(this, { element: toolbar });
        };
        ol.inherits(ui.controls.Toolbar, ol.control.Control);
        return ui.controls.Toolbar;
    }
    function createContextMenu() {
        if (!opciones.conStreetView) return null;

        ui.contextMenu = function (evt) {
            if (!evt.ctrlKey) {
                evt.preventDefault();
                evt.stopImmediatePropagation();

                var menuContainer = getMenuContainerElement(),
                    ul = menuContainer.appendChild(document.createElement("ul")),
                    selection = getSelection(),
                    mostrarMenu = selection.length || opciones.conStreetView || Object.keys(externalItems).length;
                nuevoItem = function (padre, icon, texto, incluyeSeparador) {
                    if (incluyeSeparador) {
                        padre.appendChild(document.createElement("li").appendChild(document.createElement("hr")));
                    }
                    var anchor = padre.appendChild(document.createElement("li").appendChild(document.createElement("a")));
                    anchor.href = 'javascript:void(0)';
                    var ii = anchor.appendChild(document.createElement("i"));
                    ii.classList.add("fa");
                    ii.classList.add(icon);
                    anchor.insertAdjacentHTML('beforeend', '&nbsp;' + texto);
                    return anchor;
                };

                if (mostrarMenu) {
                    ul.classList.add('dropdown-menu');
                    ul.setAttribute('role', 'menu');
                    ul.setAttribute('aria-labelledby', 'dropdownMenu');
                    var separador = false;
                    if (opciones.conStreetView) {
                        separador = true;
                        nuevoItem(ul, 'fa-street-view', 'Iniciar Street View').addEventListener('click', function () { closeContextMenu(ul); startStreetView(evt); });
                    }
                    if (selection.capas.indexOf('vw_mejoras') > -1 || selection.capas.indexOf('PARCELAS_HUERFANAS') > -1 || selection.capas.indexOf('VW_PARCELAS_GRAF_ALFA') > -1 || selection.capas.indexOf('VW_PARCELAS_GRAF_ALFA_RURALES') > -1 || selection.capas.indexOf('VW_MANZANAS') > -1) {
                        nuevoItem(ul, 'fa-download', 'Descargar Geometrias').addEventListener('click', function () { closeContextMenu(ul); publishObjectEvent('descargarGeometrias', selection); });
                    }
                    Object.keys(externalMenuItems).sort().forEach(function (key) {
                        externalMenuItems[key]
                            .sort(function (a, b) { if (a.caption < b.caption) return -1; else return 1; })
                            .forEach(function (menu) {
                                nuevoItem(ul, menu.icon, menu.caption, separador).addEventListener('click', function () { closeContextMenu(ul); menu.action(); });
                                separador = false;
                            });
                        separador = true;
                    });
                    menuContainer.style.left = evt.offsetX + 'px';
                    menuContainer.style.top = evt.offsetY + 'px';
                    document.addEventListener('click', function clean(evt) { closeContextMenu(ul); evt.currentTarget.removeEventListener(evt.type, clean); });
                    registerCancellableAction(27, function () { closeContextMenu(ul); });
                } else {
                    menuContainer.remove();
                }
            }
        };
        return ui.contextMenu;
    }
    function addExternalMenuItem(caption, icon, action, group, itemid) {
        if (typeof action !== 'function') {
            action = function () {
                publishObjectEvent(action);
            };
        }
        externalMenuItems[group] = externalMenuItems[group] || [];
        var item = { caption: caption, icon: icon, action: action, id: itemid },
            idx = externalMenuItems[group].findIndex(function (elem) { return elem.id === itemid; });

        if (itemid && idx !== -1) {
            externalMenuItems[group].splice(idx, 1, item);
        } else {
            externalMenuItems[group].push(item);
        }
    }
    function createInfoTooltip() {
        removeInfoTooltip();
        infoTooltipElement = document.createElement('div');
        infoTooltipElement.className = 'map-tooltip map-tooltip-info';
        infoTooltip = new ol.Overlay({
            element: infoTooltipElement,
            offset: [0, -15],
            positioning: 'bottom-center'
        });
        map.addOverlay(infoTooltip);
    }
    function removeInfoTooltip() {
        if (infoTooltipElement) {
            infoTooltipElement.parentNode.removeChild(infoTooltipElement);
            infoTooltipElement = null;
            map.removeOverlay(infoTooltip);
        }
    }
    function displayFeatureInfo(evt) {
        if (layerSwitcherOpen) return;

        var feature = map.forEachFeatureAtPixel(evt.pixel, function (feature) { return feature; }, { layerFilter: function (ly) { return ly.get('GeoConfig') || ly === routeLayer; } });
        if (feature) {
            var ttipConfig, title, body;
            if (feature.get('tooltipWaypoint')) {
                ttipConfig = feature.get('tooltipWaypoint');
                body = ttipConfig.body;
                title = ttipConfig.title;
            } else {
                ttipConfig = map.getLayerGroup().getLayersArray()
                    .filter(function (ly) {
                        return ly instanceof ol.layer.Vector &&
                            ly.get('GeoConfig') &&
                            ly.get('GeoConfig').NombreFisico === feature.getId().split('.')[0];
                    })[0]
                    .get('GeoConfig').ConfiguracionTooltip;
                if (ttipConfig) {
                    ttipConfig = eval('(' + ttipConfig + ')');
                    title = ttipConfig.title.fields.map(function (field) { return field.text || feature.get(field.name); }).join(' ');
                    body = ttipConfig.body.fields.map(function (field) { return field.text || (field.template || "@name@").replace("@name@", feature.get(field.name) || ''); }).join('<br />');
                }
            }
            if (body) {
                createInfoTooltip();
                infoTooltipElement.innerHTML = '<u><h5>' + title + '</h5></u><span>' + body + '</span>';
                infoTooltip.setPosition(evt.coordinate);
            }
        } else {
            removeInfoTooltip();
        }
    }
    function resolutionChanged(scale) {
        map.getLayerGroup().getLayersArray()
            .filter(function (ly) { return ly instanceof ol.layer.Vector && ly.get('GeoConfig') && ly.get('GeoConfig').EscalaVisible; })
            .forEach(function (ly) {
                var geoConfig = ly.get('GeoConfig'),
                    chkBox = getMapElement().querySelector('.layer input[obj-id="' + geoConfig.IdCapa + '"]'),
                    layerDiv = chkBox.parentElement.parentElement,
                    maxScale = geoConfig.EscalaMaxima || 99999999999,
                    minScale = geoConfig.EscalaMinima || -1,
                    inScaleRange = maxScale >= scale && scale >= minScale,
                    currentState = inScaleRange ? 1 : 0,
                    change = initialResolve && inScaleRange ||
                        !initialResolve && geoConfig.lastState !== currentState && (!geoConfig.userControlled || chkBox.getAttribute('disabled'));
                if (geoConfig.lastState !== currentState && change) {
                    layerDiv.classList.remove('out-of-range');
                    chkBox.removeAttribute('disabled');
                    chkBox.checked = currentState === 1;
                    if (document.createEventObject) {
                        chkBox.fireEvent("onclick");
                    } else {
                        var evt = document.createEvent("HTMLEvents");
                        evt.initEvent("click", false, true);
                        chkBox.dispatchEvent(evt);
                    }
                    geoConfig.userControlled = false; //redefino el valor que setea el change:visible del layer
                    geoConfig.lastState = currentState;
                }
                if (!inScaleRange) {
                    layerDiv.classList.add('out-of-range');
                    chkBox.setAttribute('disabled', 'disabled');
                }
            });
        initialResolve = false;
    }

    function refresh() {
        map.getLayerGroup().getLayersArray()
            .forEach(function (layer) {
                if (layer.getSource().clear) //wfs
                    layer.getSource().clear();
                if (layer.getSource().updateParams) //wms
                    layer.getSource().updateParams({ 'time': new Date() });
            });
        map.dispatchEvent('change:resolution');
    }
    function cleanMap() {
        if (cleanUpInteraction) cleanUpInteraction();
        measureLayer.getSource().clear();
        markerLayer.getSource().clear();
        drawLayer.getSource().clear();
        routeLayer.getSource().clear();
        ui.interactions.Select.getFeatures().clear();
        var ttip = document.querySelector('.map-tooltip.map-tooltip-static');
        if (ttip) ttip.parentNode.removeChild(ttip);
    }
    function closeContextMenu(menu) {
        menu.remove();
    }
    function startStreetView(evt) {
        if (ui.interactions.StreetView) {
            return; //ya está iniciado
        }
        new Promise(function (resolve, reject) {
            var lnglat = ol.proj.toLonLat(map.getEventCoordinate(evt)),
                position = { lng: lnglat[0], lat: lnglat[1] };
            new google.maps.StreetViewService().getPanoramaByLocation(position, 50, (data, status) => {
                if (status === google.maps.StreetViewStatus.OK) {
                    //dejo comentado el cálculo del ángulo por ahora hasta ver si hay alguna condicion que se tenga que evaluar para justificar su uso
                    var degrees = 0;//Math.atan2(position.lng - data.location.latLng.lng(), position.lat - data.location.latLng.lat()) * 180 / Math.PI;
                    resolve({ position: data.location.latLng, angle: (degrees < 0.0 ? 360 + degrees : degrees) });
                } else {
                    reject();
                }
            });
        }).then(function (sv) {
            var svElem = getStreetViewElement();
            streetViewLayer = new ol.layer.Vector({ source: new ol.source.Vector(), visible: true, zIndex: 4999 });
            ui.interactions.StreetView = new ol.interaction.Translate({ layers: [streetViewLayer] });
            ui.interactions.StreetView.on('translateend', function (evt) { translateend(svElem, evt); });

            map.addInteraction(ui.interactions.StreetView);
            var marker = new ol.Feature({ geometry: new ol.geom.Point([0, 0]) });
            marker.setStyle(new ol.style.Style({
                image: new ol.style.Icon({
                    anchor: [0.5, 2],
                    anchorXUnits: 'fraction',
                    anchorYUnits: 'pixels',
                    size: [36, 36],
                    scale: 1,
                    rotateWithView: true,
                    opacity: 1,
                    rotation: 0,
                    src: BASE_URL + 'content/images/streetviewicon.svg'
                })
            }));
            streetViewLayer.getSource().addFeature(marker);
            map.addLayer(streetViewLayer);
            getMapElement().addEventListener('streetview:movemarker', updateStreetViewMarker);
            getMapElement().addEventListener('streetview:rotatemarker', updateStreetViewMarker);

            initPanorama(svElem, sv.position, sv.angle);
            //if (ui.interactions.Select.getFeatures().getArray().length) {
            //    var extent = ol.extent.createEmpty();
            //    ui.interactions.Select.getFeatures().getArray().forEach(function (f) {
            //        ol.extent.extend(extent, f.getGeometry().getExtent());
            //    });
            //    map.getView().fit(extent, map.getSize());
            //} else {
            //    map.getView().setZoom(15);
            //}
            //initPanorama(svElem, map.getView().getCenter());
        }).catch(function () {
            alert("no hay streetview");
        });
    }
    function stopStreetView(streetViewElem) {
        if (ui.interactions.StreetView) {
            ui.interactions.StreetView.un('translateend');
            getMapElement().removeEventListener('streetview:movemarker', updateStreetViewMarker);
            getMapElement().removeEventListener('streetview:rotatemarker', updateStreetViewMarker);
            map.removeInteraction(ui.interactions.StreetView);
            map.removeLayer(streetViewLayer);
            ui.interactions.StreetView = null;
            streetViewLayer = null;
            streetViewElem.remove();
        }
    }
    function initPanorama(target, latLng, angle) {
        panorama = new google.maps.StreetViewPanorama(
            target, {
            position: latLng,
            pov: { heading: angle, pitch: 0 },
            zoom: 1
        });

        panorama.addListener('pov_changed', function () {
            dispatchCustomEvent(getMapElement(), "streetview:rotatemarker", { pov: panorama.getPov() });
        });
        panorama.addListener('position_changed', function () {
            clearTimeout(timeout);
            timeout = setTimeout(function () {
                dispatchCustomEvent(getMapElement(), "streetview:movemarker", { coords: ol.proj.fromLonLat([panorama.getPosition().lng(), panorama.getPosition().lat()]), pov: panorama.getPov() });
            }, 100);
        });
        target.addEventListener('streetview:movepanorama', function (evt) {
            var lnglat = ol.proj.toLonLat(evt.detail.coords);
            panorama.setPosition({ lng: lnglat[0], lat: lnglat[1] });
        });
    }
    function updateStreetViewMarker(evt) {
        var marker = streetViewLayer.getSource().getFeatures()[0];
        if (evt.detail.coords) {
            if (!ol.extent.containsCoordinate(map.getView().calculateExtent(), evt.detail.coords)) {
                map.getView().animate({ center: evt.detail.coords, duration: 300 });
            }
            marker.setGeometry(new ol.geom.Point(evt.detail.coords));
        }
        marker.getStyle().getImage().setRotation(evt.detail.pov.heading * 0.01745329251);
        streetViewLayer.getSource().refresh();
    }
    function translateend(target, evt) {
        dispatchCustomEvent(target, "streetview:movepanorama", { coords: evt.coordinate });
    }
    function subscribeObjectEvent(event, handler) {
        getMapElement().addEventListener(event, function (evt) { handler(evt.detail); });
    }
    function publishObjectEvent(event, data) {
        dispatchCustomEvent(getMapElement(), event, data);
    }
    function dispatchCustomEvent(target, event, data) {
        target.dispatchEvent(new CustomEvent(event, { detail: data }));
    }
    function measure(tipo) {
        registerCancellableAction(27,
            function (cancelled) {
                if (cancelled) {
                    removeMeasureTooltip();
                }
                measureTooltipElement = null;
                if (listener) {
                    ol.Observable.unByKey(listener);
                }
            });

        deactivateInteraction(ui.interactions.Select);
        deactivateInteraction(ui.interactions.DoubleClickZoom);
        deactivateInteraction(ui.interactions.BBoxSelect);

        measureLayer.getSource().clear();

        var ttip = document.querySelector('.map-tooltip.map-tooltip-static');
        if (ttip) ttip.parentNode.removeChild(ttip);

        ui.interactions.Measure = new ol.interaction.Draw({
            source: measureLayer.getSource(),
            type: tipo,
            style: new ol.style.Style({
                fill: new ol.style.Fill({
                    color: 'rgba(255,237,0, 0.3)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(255,237,0, 0.75)',
                    width: 2
                }),
                image: new ol.style.Circle({
                    radius: 5,
                    stroke: new ol.style.Stroke({
                        color: 'rgba(255,237,0, 0.7)'
                    }),
                    fill: new ol.style.Fill({
                        color: 'rgba(255,237,0, 0.3)'
                    })
                })
            })
        });
        map.addInteraction(ui.interactions.Measure);

        var listener;
        ui.interactions.Measure.once('drawstart', function (evt) {
            createMeasureTooltip();

            feature = evt.feature;

            listener = feature.getGeometry().on('change', function (evt) {
                var medicion,
                    geom = evt.target,
                    posicionTooltip = evt.coordinate;

                if (geom instanceof ol.geom.Polygon) {
                    medicion = formatArea(geom);
                    posicionTooltip = geom.getInteriorPoint().getCoordinates();
                } else if (geom instanceof ol.geom.Circle) {
                    medicion = formatArea(ol.geom.Polygon.fromCircle(geom));
                    posicionTooltip = geom.getCenter();
                } else if (geom instanceof ol.geom.LineString) {
                    medicion = formatDistance(geom);
                    posicionTooltip = geom.getLastCoordinate();
                }
                measureTooltipElement.innerHTML = medicion;
                measureTooltip.setPosition(posicionTooltip);
            });
        }, this);

        ui.interactions.Measure.once('drawend', function () {
            feature = null;
            measureTooltipElement.className = 'map-tooltip map-tooltip-static';
            measureTooltip.setOffset([0, -7]);
            cleanUpInteraction();
        }, this);
        ui.interactions.CurrentActive = ui.interactions.Measure;
    }
    function onkeyup(evt) {
        var keyCode = evt.which || evt.keyCode;
        if (cleanUpInteraction && keyCode === cancelKeyCode) {
            cleanUpInteraction(true);
        }
    }
    function registerCancellableAction(code, callback) {
        if (cleanUpInteraction) {
            cleanUpInteraction();
        }
        cancelKeyCode = code;
        document.addEventListener('keyup', onkeyup);
        cleanUpInteraction = function (cancelled) {
            callback(cancelled);
            deactivateInteraction();
            cleanUpInteraction = null;
            cancelKeyCode = null;
            if (!opciones.conToolbarExterno) {
                setTimeout(function () {
                    activateInteraction(ui.interactions.Select);
                    activateInteraction(ui.interactions.DoubleClickZoom);
                    activateInteraction(ui.interactions.BBoxSelect);
                }, 100);
            }
            document.removeEventListener('keyup', onkeyup);
        };
    }
    function createMeasureTooltip() {
        removeMeasureTooltip();
        measureTooltipElement = document.createElement('div');
        measureTooltipElement.className = 'map-tooltip map-tooltip-measure';
        measureTooltip = new ol.Overlay({
            element: measureTooltipElement,
            offset: [0, -15],
            positioning: 'bottom-center'
        });
        map.addOverlay(measureTooltip);
    }
    function removeMeasureTooltip() {
        if (measureTooltipElement) {
            map.removeOverlay(measureTooltip);
            measureTooltipElement.parentNode.removeChild(measureTooltipElement);
        }
    }
    function activateInteraction(interaction) {
        if (interaction) {
            interaction.setActive(true);
        }
    }
    function deactivateInteraction(interaction) {
        if (interaction) {
            interaction.setActive(false);
        } else if (ui.interactions.CurrentActive) {
            map.removeInteraction(ui.interactions.CurrentActive);
            ui.interactions.CurrentActive = null;
        }
    }
    function print() {
        map.once('rendercomplete', function (evt) {
            var windowContent = '<!DOCTYPE html>';
            windowContent += '<html>';
            windowContent += '<head><title>Impresi&oacute;n de Mapa</title></head>';
            windowContent += '<body>';
            windowContent += '<img src="' + evt.context.canvas.toDataURL() + '">';
            windowContent += '</body>';
            windowContent += '</html>';

            var printWin = window.open('', '', 'width=' + screen.availWidth + ',height=' + screen.availHeight);
            printWin.document.open();
            printWin.document.write(windowContent);

            printWin.document.addEventListener('load', function () {
                printWin.focus();
                printWin.print();
                printWin.document.close();
                printWin.close();
            }, true);
        });
        map.renderSync();
    }
    function draw(type, preselected) {
        if (cleanUpInteraction) cleanUpInteraction();

        registerCancellableAction(27, function () {
            map.removeInteraction(ui.interactions.Modify);
            ui.interactions.Modify = null;
        });

        deactivateInteraction(ui.interactions.Select);
        deactivateInteraction(ui.interactions.DoubleClickZoom);
        deactivateInteraction(ui.interactions.BBoxSelect);

        drawLayer.getSource().clear();

        if (preselected) {
            drawLayer.getSource().addFeatures(preselected);
            var extent = ol.extent.createEmpty();
            preselected.forEach(function (f) {
                ol.extent.extend(extent, f.getGeometry().getExtent());
            });
            map.getView().fit(extent, map.getSize());
        } else {
            ui.interactions.Draw = new ol.interaction.Draw({
                source: drawLayer.getSource(),
                type: type,
                style: new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: 'rgba(23,199,0,0.3)'
                    }),
                    stroke: new ol.style.Stroke({
                        color: 'rgba(23,199,0,0.5)',
                        width: 2
                    }),
                    image: new ol.style.Circle({
                        radius: 5,
                        stroke: new ol.style.Stroke({
                            color: 'rgba(23,199,0,0.5)'
                        }),
                        fill: new ol.style.Fill({
                            color: 'rgba(23,199,0,0.3)'
                        })
                    })
                }),
                zIndex: 9999999
            });

            ui.interactions.Draw.once('drawstart', function () { drawLayer.getSource().clear(); });
            ui.interactions.Draw.once('drawend', function () { deactivateInteraction(ui.interactions.Draw); });
            map.addInteraction(ui.interactions.Draw);
            ui.interactions.CurrentActive = ui.interactions.Draw;
        }

        ui.interactions.Modify = new ol.interaction.Modify({
            source: drawLayer.getSource(),
            deleteCondition: function (event) {
                return ol.events.condition.shiftKeyOnly(event) && ol.events.condition.singleClick(event);
            }
        });
        map.addInteraction(ui.interactions.Modify);
    }
    function geometryToWkt(feature) {
        var geom = feature.clone().getGeometry();
        if (geom instanceof ol.geom.Circle) {
            geom = ol.geom.Polygon.fromCircle(geom);
        }
        return new ol.format.WKT().writeGeometry(geom.transform(MAP_SRID, APP_SRID));
    }
    function wktToGeometry(wkt) {
        return new ol.format.WKT().readGeometry(wkt).transform(APP_SRID, MAP_SRID);
    }
    function zoomExtent(extent) {
        if (typeof extent === "string") {
            extent = wktToGeometry(extent).getExtent();
        }
        var duration = ol.extent.intersects(extent, map.getView().calculateExtent()) ? 1500 : 0;
        map.getView().fit(extent, { size: map.getSize(), duration: duration });
    }
    function setCenter(coords, zoomLevel) {
        map.getView().setCenter(ol.proj.transform(coords, APP_SRID, MAP_SRID));
        map.getView().setZoom(zoomLevel || map.getView().getZoom());
    }
    function getViewExtent() {
        return ol.proj.transformExtent(map.getView().calculateExtent(), MAP_SRID, APP_SRID);
    }
    function addTempLayer(tempLayer) {
        var generator = new LayersGenerator(),
            switcher = map.getControls().getArray().filter(function (ctl) { return ctl instanceof ui.controls.Switcher; })[0];
        generator.loadLayers([tempLayer]).forEach(function (ly) { map.addLayer(ly); });
        switcher.addTempLayers(generator.getLayersHierarchy());
    }
    function initMap(layers, centerCoords, zoomLevel) {
        map.getLayers().clear();
        map.removeControl(map.getControls().getArray().filter(function (ctl) { return ctl === ui.controls.Switcher; })[0]);
        var generator = new LayersGenerator();
        generator.loadLayers(layers).forEach(function (ly) { map.addLayer(ly); });
        map.addLayer(measureLayer);
        map.addLayer(markerLayer);
        map.addLayer(drawLayer);
        map.addLayer(routeLayer);

        setCenter(centerCoords, zoomLevel);

        if (createSwitcher()) {
            map.addControl(new ui.controls.Switcher({ layersHierarchy: generator.getLayersHierarchy() }));
        }
        setTimeout(function () { map.getView().dispatchEvent('change:resolution'); }, 100);
        fetch(`${BASE_URL}Scripts/Mapas/MapaGrupoSeleccion.json`)
            .then(resp => resp.json())
            .then(data => selectionLayerGroups = data);
    }
    function setSelection(features, layers, fitSelection) {
        if (!opciones.conSeleccionObjetos) return;

        ui.interactions.Select.getFeatures().clear();
        var queries = [];
        for (layerIdx in layers) {
            queries = queries.concat(map.getLayerGroup().getLayersArray()
                .filter(function (ly) {
                    return ly.get('GeoConfig') && ly.get('GeoConfig').NombreFisico && ly.get('GeoConfig').NombreFisico.toLowerCase() === layers[layerIdx].toLowerCase();
                })
                .map(function (ly) {
                    return filterLayer(ly, features[layerIdx]);
                }));
        }
        Promise.all(queries)
            .then(function (results) {
                var zoom = {},
                    total = results.reduce(function (acc, layer) {
                        ol.extent.extend(acc.extent, layer.extent);
                        acc.features = acc.features.concat(layer.features);
                        return acc;
                    }, { extent: ol.extent.createEmpty(), features: [] });
                if (total.features.length === 1 && total.features[0].getGeometry().getType() === "Point") {
                    zoom = { minResolution: map.getView().getResolutionForZoom(19) };
                }
                if (!!fitSelection || fitSelection == null) {
                    var duration = ol.extent.intersects(total.extent, map.getView().calculateExtent()) ? 1500 : 0;
                    map.getView().fit(total.extent, Object.assign({ size: map.getSize(), duration: duration }, zoom));
                }
            });
    }
    function filterLayer(layer, features) {
        return new Promise(function (resolve) {
            var filterParams = eval('(' + layer.get('GeoConfig').ConfiguracionOrigen + ')'),
                prefixLayerID = filterParams.typename.split(':')[1],
                selection = features.filter(function (featid) { return !!featid; })
                    .map(function (featid) { return prefixLayerID + "." + featid.toString(); }).join(',');
            layer.getSource().get('filter')(filterParams, selection)
                .then(function (features) {
                    var extent = ol.extent.createEmpty();
                    features.forEach(function (f) {
                        ui.interactions.Select.getFeatures().push(f);
                        ol.extent.extend(extent, f.getGeometry().getExtent());
                        f.set("nombreLayer", layer.get('GeoConfig').NombreFisico, true);
                    });
                    resolve({ extent: extent, features: features });
                })
                .catch(function (err) {
                    console.log(err);
                    resolve([]);
                });
        });
    }
    function getSelection() {
        var selection = {};
        ui.interactions.Select.getFeatures().getArray()
            .forEach(function (feat) {
                var key = feat.get("nombreLayer"),
                    value = feat.getId().split('.')[1];
                (selection[key] = selection[key] || []).push(value);
            });
        return { seleccion: Object.keys(selection).map(function (key) { return selection[key]; }, []), capas: Object.keys(selection) };
    }
    function drawSelection(elemIdx = 0) {
        const selection = ui.interactions.Select.getFeatures().getArray().map(geometryToWkt);
        if (!selection.length || selection.length < elemIdx) return;
        modifyObject(selection[elemIdx])
    }
    function getDrawings() {
        return drawLayer
            .getSource()
            .getFeatures()
            .map(geometryToWkt);
    }
    function getVisibleLayers() {
        return map
            .getLayerGroup()
            .getLayersArray()
            .filter(function (ly) { return ly.getVisible() && !!ly.get('GeoConfig') && !!ly.get('GeoConfig').NombreFisico; })
            .map(function (ly) { return ly.get('GeoConfig').NombreFisico; });
    }
    function modifyObject(wkt) {
        var geom = wktToGeometry(wkt);
        draw(geom.getType(), [new ol.Feature({ geometry: geom })]);
    }
    function drawRoute(points, path) {
        routeLayer.getSource().clear();
        var closed = points[0].lat === points[points.length - 1].lat && points[0].lon === points[points.length - 1].lon,
            extent = ol.extent.createEmpty();

        //si la ruta inicia y finaliza en el mismo punto, 
        //quito el ultimo para no insertar 2 veces el mismo
        if (closed) points.splice(points.length - 1, 1);

        var lineString = new ol.Feature({ geometry: new ol.geom.LineString(path.map(function (coords) { return ol.proj.fromLonLat([coords[1], coords[0]]); })) });
        var locations = points.map(function (loc, idx) {
            // si la ruta es cerrada (mismo inicio y fin) muestro imagen compuesta
            var inicio = idx === 0,
                fin = idx === points.length - 1,
                image = getRouteImage(closed, inicio, fin);
            var routeStop = new ol.Feature({ geometry: new ol.geom.Point(ol.proj.fromLonLat([loc.lon, loc.lat])) });
            routeStop.setStyle(new ol.style.Style({
                image: new ol.style.Icon({
                    anchor: [0.5, 2],
                    anchorXUnits: 'fraction',
                    anchorYUnits: 'pixels',
                    size: [image.size.width, image.size.height],
                    scale: 1,
                    rotateWithView: true,
                    src: image.url
                })
            }));
            ol.extent.extend(extent, routeStop.getGeometry().getExtent());

            routeStop.set('tooltipWaypoint', {
                title: inicio ? 'Inicio' : fin ? 'Fin' : 'Paso ' + idx.toString(),
                body: loc.description
            });
            return routeStop;
        });

        ol.extent.extend(extent, lineString.getGeometry().getExtent());

        routeLayer.getSource().addFeatures(locations.concat(lineString));

        map.getView().fit(extent, map.getSize());
    }
    function formatArea(geom) {
        var area = ol.sphere.getArea(geom);
        var km2 = 1000000;
        if (area > km2) {
            return (Math.round(area / km2 * 100) / 100) + ' ' + 'km<sup>2</sup>';
        } else {
            return (Math.round(area * 100) / 100) + ' ' + 'm<sup>2</sup>';
        }
    }
    function formatDistance(geom) {
        var distance = ol.sphere.getLength(geom);
        var km = 1000;
        if (distance > km) {
            return (Math.round(distance / km * 100) / 100) + ' ' + 'km';
        } else {
            return (Math.round(distance * 100) / 100) + ' ' + 'm';
        }
    }
    function LayersGenerator() {
        var jerarquias = [],
            getSource = function (config) {
                var source,
                    params = eval('(' + config.ConfiguracionOrigen + ')');

                switch (config.NombreTipoOrigen) {
                    case "OSM":
                        source = new ol.source.OSM(params);
                        break;
                    case "BingMaps":
                        source = new ol.source.BingMaps(params);
                        break;
                    case "Stamen":
                        source = new ol.source.Stamen(params);
                        break;
                    case "ImageWMS":
                        source = new ol.source.ImageWMS(params);
                        break;
                    case "TileWMS":
                        source = new ol.source.TileWMS(params);
                        break;
                    case "Ortofoto":
                    case "TileWMTS":
                        var projExtent = ol.proj.get(MAP_SRID).getExtent();
                        source = new ol.source.WMTS({
                            url: params.url,
                            layer: params.layerName,
                            matrixSet: params.matrixSet,
                            format: params.format,
                            projection: params.projection,
                            crossOrigin: params.crossOrigin,
                            tileGrid: new ol.tilegrid.WMTS({
                                extent: params.extent, //extent que sirve de constraint para no consultar imagen si el tile está afuera
                                origin: ol.extent.getTopLeft(projExtent),
                                resolutions: Array.apply(null, Array(params.gridSetLevels)).map(function (_, idx) { return ol.extent.getWidth(projExtent) / params.tileSize / Math.pow(2, idx); }),
                                matrixIds: Array.apply(null, Array(params.gridSetLevels)).map(function (_, idx) { return (params.matrixIdTemplate || '') + idx; })
                            }),
                            tileLoadFunction: function (tile, src) {
                                tile.getImage().src = src + '&' + Date.now();
                            }
                        });
                        break;
                    case "GeoJSON":
                        source = new ol.source.GeoJSON(params);
                        break;
                    case "JSONP":
                        source = new ol.source.Vector({
                            format: new ol.format.GeoJSON(),
                            loader: function (extent) {
                                var callbackFunction = 'callbackLayer' + new Date().getTime().toString() + '_' + (Math.random() * 10000000000).toFixed(),
                                    script = document.createElement('script');

                                window[callbackFunction] = function (featureCollection) {
                                    delete window[callbackFunction];
                                    document.body.removeChild(script);
                                    source.addFeatures(new ol.format.GeoJSON().readFeatures(featureCollection));
                                };

                                var data = {
                                    service: params.service,
                                    version: params.version,
                                    request: params.request,
                                    typename: params.typename,
                                    outputFormat: params.outputFormat,
                                    srsname: params.projection,
                                    format_options: 'callback:' + callbackFunction
                                };
                                var nativeExtent = ol.proj.transformExtent(extent, MAP_SRID, APP_SRID);
                                data.CQL_Filter = 'BBOX(' + config.CampoGeometry + ',' + nativeExtent.join(',') + ')';
                                if (config.FiltroPredefinido) {
                                    data.CQL_Filter += ' AND ' + config.FiltroPredefinido;
                                }
                                script.src = params.url + '?' + Object.keys(data).map(function (k) { return [k, data[k]].join('='); }).join('&');
                                document.body.appendChild(script);
                            },
                            strategy: ol.loadingstrategy.tile(new ol.tilegrid.createXYZ({ tileSize: 1024 })),
                            projection: params.projection
                        });
                        source.on('changefeature', function (evt) {
                            evt.target.clear();
                            evt.target.dispatchEvent('change:resolution');
                        });
                        source.set('filter', function (params, filter) {
                            return new Promise(function (resolve) {
                                var callbackFunction = 'callbackLayer' + new Date().getTime().toString() + '_' + (Math.random() * 10000000000).toFixed(),
                                    script = document.createElement('script');

                                window[callbackFunction] = function (featureCollection) {
                                    delete window[callbackFunction];
                                    document.body.removeChild(script);
                                    resolve(new ol.format.GeoJSON().readFeatures(featureCollection));
                                };

                                var data = {
                                    service: params.service,
                                    version: params.version,
                                    request: params.request,
                                    typename: params.typename,
                                    outputFormat: params.outputFormat,
                                    srsname: params.projection,
                                    featureId: filter,
                                    format_options: 'callback:' + callbackFunction
                                };
                                script.src = params.url + '?' + Object.keys(data).map(function (k) { return [k, data[k]].join('='); }).join('&');
                                document.body.appendChild(script);
                            });
                        });
                        break;
                    case "Vector":
                        source = new ol.source.Vector({
                            format: new ol.format.GeoJSON(),
                            loader: function (extent) {
                                var req = new XMLHttpRequest();
                                var data = {
                                    service: params.service,
                                    version: params.version,
                                    request: params.request,
                                    typename: params.typename,
                                    outputFormat: params.outputFormat,
                                    srsname: params.projection,
                                    bbox: extent.join(',') + ',' + params.projection
                                };
                                req.open('POST', params.url, true);
                                req.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
                                req.onload = function () {
                                    source.addFeatures(new ol.format.GeoJSON().readFeatures(req.responseText));
                                    source.dispatchEvent('loaded');
                                };
                                req.onerror = function (err) {
                                    console.log(err);
                                };
                                req.send(Object.keys(data).map(function (k) { return [k, data[k]].join('='); }).join('&'));
                            },
                            strategy: ol.loadingstrategy.tile(new ol.tilegrid.createXYZ({ tileSize: 1024 })),
                            projection: params.projection
                        });
                        source.on('changefeature', function (evt) {
                            evt.target.clear();
                            evt.target.dispatchEvent('change:resolution');
                        });
                        source.set('filter', function (params, filter) {
                            return new Promise(function (resolve, reject) {
                                var req = new XMLHttpRequest();
                                var data = {
                                    service: params.service,
                                    version: params.version,
                                    request: params.request,
                                    typename: params.typename,
                                    outputFormat: params.outputFormat,
                                    srsname: params.projection,
                                    featureId: filter
                                };
                                req.open('POST', params.url, true);
                                req.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
                                req.onload = function () {
                                    resolve(new ol.format.GeoJSON().readFeatures(response));
                                };
                                req.onerror = function (err) {
                                    reject(err);
                                };
                                req.send(Object.keys(data).map(function (k) { return [k, data[k]].join('='); }).join('&'));
                            });
                        });
                        break;
                    default:
                        break;
                }
                return source;
            },
            getLayer = function (config) {
                if (config.IdCapa) {
                    var layer,
                        esLayerBase = false,
                        esSeleccionable = false,
                        params = {
                            id: config.IdCapa,
                            visible: config.VisibleDefault,
                            title: config.NombreCapa
                        };

                    switch (config.NombreTipoOrigen) {
                        case "OSM":
                        case "BingMaps":
                        case "Stamen":
                        case "Ortofoto":
                            esLayerBase = true;
                            layer = new ol.layer.Tile(params);
                            if (config.UbicacionSplitter) {
                                layer.on('precompose', function (evt) {
                                    var ctx = evt.context;
                                    var splitter = getMapElement().querySelector('[type=range]'),
                                        value = splitter.value;
                                    if (splitter.classList.contains('hidden')) {
                                        value = 0;
                                    }
                                    var width = ctx.canvas.width * (value / 100);

                                    ctx.save();
                                    ctx.beginPath();
                                    ctx.rect(width, 0, ctx.canvas.width - width, ctx.canvas.height);
                                    ctx.clip();
                                });

                                layer.on('postcompose', function (evt) {
                                    evt.context.restore();
                                });
                            }
                            break;
                        case "TileWMS":
                        case "TileWMTS":
                            layer = new ol.layer.Tile(params);
                            break;
                        case "GeoJSON":
                        case "JSONP":
                        case "Vector":
                            esSeleccionable = true;
                            var rules = eval('(' + config.ConfiguracionEstilo + ')');
                            layer = new ol.layer.Vector(Object.assign(params, { declutter: true, style: getStylesFromRules(rules, config.NombreFisico) }));
                            break;
                        case "ImageWMS":
                            layer = new ol.layer.Image(params);
                            break;
                        default:
                            break;
                    }
                    if (!esLayerBase) {
                        layer.on('change:visible', function (evt) {
                            evt.target.get('GeoConfig').lastState = evt.target.getVisible() ? 1 : 0;
                        });
                    }

                    layer.setSource(getSource(config));
                    if (config.EscalaMinima) {
                        layer.setMinResolution(olLegacy.v2.getResolutionFromScale(config.EscalaMinima));
                    }
                    if (config.EscalaMaxima) {
                        layer.setMaxResolution(olLegacy.v2.getResolutionFromScale(config.EscalaMaxima));
                    }
                    if (config.ZIndex) {
                        layer.setZIndex(config.ZIndex);
                    }
                    config.esLayerBase = esLayerBase;
                    config.userControlled = false;
                    config.lastState = 0;
                    config.esSeleccionable = esSeleccionable;
                    layer.setProperties({ GeoConfig: config });
                    layer.setProperties({ Seleccionable: esSeleccionable });
                    return layer;
                } else {
                    var layers = (config.subgrupos || []).reduce(function (accum, subg) { return [...accum, getLayer(subg)]; }, []);
                    layers = (config.layers || []).reduce(function (accum, ly) { return [...accum, getLayer(ly)]; }, layers);
                    return new ol.layer.Group({ layers: layers, title: config.nombreGrupo });
                }
            };

        return {
            loadLayers: function (layersConfigs) {
                jerarquias = [];
                var groups = layersConfigs
                    .reduce(function (accum, ly) { return accum.concat(ly.Ruta.split("/")); }, [])
                    .filter(function (grp, idx, arr) { return arr.indexOf(grp) === idx; })
                    .map(function (grp) {
                        var p = grp.split(":");
                        return { id: Number(p[0]), padre: Number(p[1]), nombreGrupo: p[2], layers: layersConfigs.filter(function (ly) { return Number(p[0]) === ly.IdGrupo; }) };
                    });

                var imap = groups.reduce(function (acc, grp, i) {
                    acc[grp.id] = i;
                    return acc;
                }, {});
                groups.forEach(function (grp) {
                    if (grp.padre === 0) {
                        jerarquias.push(grp);
                        return;
                    }
                    var grupoPadre = groups[imap[grp.padre]];
                    grupoPadre.subgrupos = [...(grupoPadre.subgrupos || []), grp];
                });
                return jerarquias.reduce(function (accum, g) { return accum.concat(getLayer(g)); }, []);
            },
            getLayersHierarchy: function () {
                return jerarquias;
            }
        };
    }

    //api publica
    var api = {
        //getMap: function () { return map; }, //para testing solamente, se borra cuando se terminen de implementar los metodos del mapa
        centrar: setCenter,
        iniciar: initMap,
        limpiar: cleanMap,
        refrescar: refresh,
        obtenerDibujos: getDrawings,
        obtenerSeleccion: getSelection,
        editarObjeto: modifyObject,
        seleccionarObjetos: setSelection,
        dibujarRuta: drawRoute,
        dibujarSeleccion: drawSelection,
        agregarCapaTemporal: addTempLayer,
        agregarItemMenu: addExternalMenuItem,
        zoomExtent: zoomExtent,
        obtenerExtent: getViewExtent,
        obtenerCapasVisibles: getVisibleLayers,
        on: subscribeObjectEvent
    };
    if (opciones.conToolbarExterno) {
        var apiToolbar = {
            filtrarCapa: filterLayer,
            limpiar: function () {
                if (cleanUpInteraction) cleanUpInteraction();
                deactivateInteraction(ui.interactions.Select);
                deactivateInteraction(ui.interactions.DoubleClickZoom);
                deactivateInteraction(ui.interactions.BBoxSelect);
                cleanMap();
            }
        };
        if (opciones.conSeleccionObjetos) {
            apiToolbar.activarSeleccion = function () {
                if (cleanUpInteraction) cleanUpInteraction();
                activateInteraction(ui.interactions.Select);
                activateInteraction(ui.interactions.DoubleClickZoom);
                activateInteraction(ui.interactions.BBoxSelect);
            };
        }
        if (opciones.conDibujoLinea) {
            apiToolbar.activarDibujoLinea = function (original) { draw('LineString', original); };
        }
        if (opciones.conDibujoPoligono) {
            apiToolbar.activarDibujoPoligono = function (original) { draw('Polygon', original); };
        }
        if (opciones.conDibujoPunto) {
            apiToolbar.activarDibujoPunto = function (original) { draw('Point', original); };
        }
        Object.assign(api, apiToolbar);
    }
    return api;
}