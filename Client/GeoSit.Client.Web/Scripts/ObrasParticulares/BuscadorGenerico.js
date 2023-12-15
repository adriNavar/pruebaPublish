/***********************************************************************/
var Buscador_UT = 'unidadestributarias';
var Buscador_Per = 'personas';
var Buscador_Actas = 'acta';

var _GrillaBuscadorGenerico;
var _TiposBuscadorGenerico;
var _IdDevolucionBuscadorGenerico;
var _IdUbicacionBuscadorGenerico;
var _funcionOk;
var _funcionCancel;
var _callback;

var _TempValues;

var _disableSuggest = true;

function BuscadorGenerico
    (
        ParamTipos, //array de ints
        ParamIdUbicacion,
        ParamIdDevolucion,
        ParamTitulo,
        ParamDescripcion,
        ParamFuncionOK,
        ParamFuncionCancel,
        multiselect
    )
{
    console.log('Creo buscador de ');
    console.log(ParamTipos);
    /* Creo el buscador */
    _IdDevolucionBuscadorGenerico = ParamIdDevolucion;
    _IdUbicacionBuscadorGenerico = ParamIdUbicacion;
    _TiposBuscadorGenerico = ParamTipos.join(",");
    _funcionOk = ParamFuncionOK;
    _funcionCancel = ParamFuncionCancel;

    _TempValues = "";
    $("#" + _IdUbicacionBuscadorGenerico).html(
        '<div class="modal-dialog"> \
            <div class="modal-content"> \
                <div class="modal-header"> \
                    <h3 class="modal-title" id="lblModal">' + ParamTitulo + '</h3> \
                    <span aria-hidden="true" class="fa fa-close fa-2x white cursor-pointer pull-right" data-dismiss="modal" aria-label="Cerrar" title="Cerrar"></span> \
                </div> \
                <div class="modal-body"> \
                    <div class="row remove-margin" style="margin-top:7px;"> \
                        <div class= "col-xs-12"> \
                            <div class="form-group col-xs-12 remove-padding remove-margin"> \
                                <div class="row remove-margin"> \
                                    <div class="col-xs-12 remove-padding"> \
                                        <label>Filtrar</label> \
                                    </div> \
                                </div> \
                                <div class="row remove-margin"> \
                                    <div class="col-xs-12 remove-padding"> \
                                        <div class="input-group"> \
                                            <input name="Filtro_Generico" value="" id="Filtro_Generico" class="form-control" placeholder="Buscar..." type="text" /> \
                                            <span class="input-group-btn"> \
                                                <button data-ng-click="searchByText();" title="Buscar" id="btnBuscarGenerico" class="btn btn-default"> \
                                                    <i class="fa fa-search"></i> \
                                                </button> \
                                            </span> \
                                        </div> \
                                    </div> \
                                </div> \
                            </div> \
                        </div> \
                    </div> \
                    <div class="row remove-margin"> \
                        <div class="col-xs-12 remove-padding"> \
                            <div class="tabla-sin-botones" style="padding-left:15px;padding-right:15px;"> \
                                <table id="Grilla_BusquedaGenerica" class="table table-striped table-bordered table-responsive" cellspacing="0"> \
                                    <thead> \
                                        <tr> \
                                            <th class="no-show" width="5%"></th> \
                                            <th class="no-show" width="5%"></th> \
                                            <th width="100%">' + ParamDescripcion + '</th> \
                                        </tr> \
                                    </thead> \
                                    <tbody></tbody> \
                                </table> \
                            </div> \
                        </div> \
                    </div> \
                </div> \
                <div class="modal-footer"> \
                    <div class="col-lg-4 pull-right"> \
                        <span data-placement="bottom" title="Aceptar" data-original-title="Aceptar" id="BuscadorGenericoGrabar" class="fa fa-check-circle fa-2x light-blue cursor-pointer"></span> \
                    </div> \
                </div> \
            </div> \
        </div>'
    );
    $('#' + _IdUbicacionBuscadorGenerico).one('shown.bs.modal', function () {
        //ajustamodal();
        _callback = _funcionCancel();
    });
    $('#' + _IdUbicacionBuscadorGenerico).one('hidden.bs.modal', function () {
        if (typeof _callback !== 'undefined')
            _callback();
    });
    /* Inicializador */
    _GrillaBuscadorGenerico = $('#Grilla_BusquedaGenerica').DataTable({
        "scrollY": "190px",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[2, 'asc']],
        "language": {
            "url": BASE_URL + "Scripts/dataTables.spanish.txt"
        },
        "columnDefs": [{
            "targets": 'no-show',
            "orderable": false,
            "visible": false,
            "searchable": false
        }]
    });
    /* Eventos */

    $("#btnBuscarGenerico").click(function () {
        if ($("#Filtro_Generico").val()) {
            _GrillaBuscadorGenerico.clear().draw();
            $.get(BASE_URL + 'BuscadorGenerico/Suggests', "text=" + $("#Filtro_Generico").val() + "&tipos=" + _TiposBuscadorGenerico,
                function (data) {
                    var jsonResult = JSON.parse(data.Result);
                    jsonResult.response.docs.forEach(function (item) {
                        if (item.nombre) { _GrillaBuscadorGenerico.row.add({ "0": item.tipo, "1": item.id, "2": item.nombre }); }
                    });
                    _GrillaBuscadorGenerico.draw();
                });
        }
    });
    $('#Grilla_BusquedaGenerica tbody').on('click', 'tr', function () {
        var d = _GrillaBuscadorGenerico.row(this).data();
        var add = !$(this).hasClass('selected');
        var removeResult = "";
        var ssplit = _TempValues.split(',');
        var founded = false;
        var TipoObjetoIdObjeto = d[1];
        ssplit.forEach(function (entry) {
            if (entry === TipoObjetoIdObjeto) {
                founded = true;
            } else {
                if (!add)
                    removeResult += entry + ',';
            }

        });
        if (add) {
            if (multiselect === false)
            {
                _TempValues = TipoObjetoIdObjeto;
            } else {
                if (!founded) {
                    if (!_TempValues)
                        _TempValues = TipoObjetoIdObjeto;
                    else
                        _TempValues += ',' + TipoObjetoIdObjeto;
                }
            }
        } else {
            if (multiselect === false) {
                _TempValues = "";
            } else {

                _TempValues = removeResult.substring(0, removeResult.length - 1);
            }
        }
        if (multiselect === false) {
            _GrillaBuscadorGenerico.$('tr.selected').removeClass('selected');
        }
        $(this).toggleClass('selected');
    });
    $('#BuscadorGenericoGrabar').click(function (e) {
        if (multiselect === false) {
            $("#" + _IdDevolucionBuscadorGenerico).val(_TempValues);
        } else {
            if (!$("#" + _IdDevolucionBuscadorGenerico).val()) {
                $("#" + _IdDevolucionBuscadorGenerico).val(_TempValues);
            } else {
                //buscar por cada uno si esta el que existe
                var arrayDevalores = _TempValues.split(",");
                arrayDevalores.forEach(function (entry) {
                    if ($("#" + _IdDevolucionBuscadorGenerico).val().indexOf(entry) < 0) {
                        $("#" + _IdDevolucionBuscadorGenerico).val($("#" + _IdDevolucionBuscadorGenerico).val() + ',' + entry);
                    }
                });
            }
        }       
        
        _TempValues = "";
        $("#" + _IdUbicacionBuscadorGenerico).modal('hide');
        _callback = _funcionOk;
    });
    setTimeout(function () {
        //your code to be executed after 1 seconds
        $("#" + _IdUbicacionBuscadorGenerico).show();
        $("#" + _IdUbicacionBuscadorGenerico).modal('show');
        _GrillaBuscadorGenerico.draw();
        $("#" + _IdUbicacionBuscadorGenerico).on('shown.bs.modal', function (e) {
            $("#" + _IdUbicacionBuscadorGenerico + " .modal-body").height(330);
        });
    }, 200);
    $('#Filtro_Generico').on("keypress", function (e) {
        if (e.keyCode === 13) {
            console.log('Enter detected');
            $("#btnBuscarGenerico").click();
            return false; // prevent the button click from happening
        }
    });
   
}
/***********************************************************************/
//@ sourceURL=buscadorGenerico.js