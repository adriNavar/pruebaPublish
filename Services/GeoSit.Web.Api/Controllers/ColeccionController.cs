using System.Drawing;
using System.IO;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Componente = GeoSit.Data.BusinessEntities.MapasTematicos.Componente;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using System.Threading;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class ColeccionController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public ColeccionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IHttpActionResult ColeccionesUsuarioByComponentesPrincipales(long idUsuario, long[] componentesPrincipales)
        {
            return Ok(_unitOfWork.ColeccionRepository.GetColeccionesUsuarioByComponentesPrincipales(idUsuario, componentesPrincipales).ToList());
        }

        [HttpGet]
        [ResponseType(typeof(ColeccionModel))]
        public IHttpActionResult GetColeccionById(long id)
        {
            var coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(id);

            if (coleccion == null)
                return NotFound();

            return Ok(InicializaColeccionModel(coleccion));
        }

        [HttpGet]
        public IHttpActionResult Ordenar(int usuarioId, int orden, string filtro = null)
        {
            if (usuarioId <= 0)
            {
                return null;
            }

            var colecciones = _unitOfWork.ColeccionRepository
                                .GetColeccionesByUserId(usuarioId)
                                .Where(n => (filtro == null || n.Nombre.ToUpper().Contains(filtro.ToUpper())));
            if (orden == 1)
            {
                colecciones = colecciones.OrderBy(c => c.Nombre);
            }
            else if (orden == 2)
            {
                colecciones = colecciones.OrderByDescending(c => c.FechaModificacion);
            }

            return Ok(colecciones.Select(InicializaColeccionModelToShow).ToList());
            //return PartialView("~/Views/Coleccion/Colecciones.cshtml", Model);
            //return Ok(true);
            //return Ok(colecciones.ToList());
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<ColeccionModel>))]
        public IHttpActionResult GetColeccionesByUserId(int usuarioId)
        {
            var colecciones = _unitOfWork.ColeccionRepository.GetColeccionesByUserId(usuarioId);
            if (colecciones == null)
                return NotFound();




            return Ok(colecciones.Select(InicializaColeccionModelToShow).ToList());
        }

        public IHttpActionResult GetComponentesByColeccionId(long coleccionId)
        {
            ColeccionModel coleccionModel = GetComponentesByColeccion(coleccionId);
            return Ok(coleccionModel);
        }

        private ColeccionModel InicializaColeccionModelToShow(Coleccion coleccion)
        {

            var model = new ColeccionModel();

            if (coleccion == null)
                return model;

            model.ColeccionId = coleccion.ColeccionId;
            model.Nombre = coleccion.Nombre;
            model.Cantidad = GetColeccionCantidad(coleccion.ColeccionId);
            model.FechaModificacion = coleccion.FechaModificacion;
            return model;
        }

        private ColeccionModel GetComponentesByColeccion(long coleccionId)
        {
            var model = new ColeccionModel();
            Coleccion coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId);
            //_unitOfWork.ColeccionRepository.GetComponentes(coleccion);
            model.ColeccionId = coleccion.ColeccionId;
            model.Nombre = coleccion.Nombre;

            if (coleccion == null || coleccion.Componentes == null)
                return model;

            GetColeccionModel(coleccion, model);
            return model;
        }

        private void GetColeccionModel(Coleccion coleccion, ColeccionModel model)
        {
            var componentes = coleccion.Componentes
               .Where(c => c.UsuarioBaja == null)
               .GroupBy(g => g.ComponenteId)
               .Select(i => new { IdComponente = i.Key, Objetos = i.Select(o => o.ObjetoId).ToList() });
            long componente_carga_tecnica = long.Parse(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_CARGA_TECNICA"));
            var lista = new List<long>();
            foreach (var c in componentes)
            {
                var tmpComp = _unitOfWork.ComponenteRepository.GetComponenteById((int)c.IdComponente);
                var atributos = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(c.IdComponente);
                var componente = new Models.Componente
                {
                    ComponenteId = tmpComp.ComponenteId,
                    Nombre = tmpComp.Descripcion,
                    Esquema = tmpComp.Esquema,
                    Tabla = tmpComp.Tabla,
                    TablaGrafica = tmpComp.TablaGrafica ?? tmpComp.Tabla,
                    Ruteable = tmpComp.DocType == "parcelas" || tmpComp.Graficos == 3,
                    DocType = tmpComp.DocType,
                    Capa = tmpComp.Capa,
                    AplicaFiltro = tmpComp.ComponenteId == componente_carga_tecnica
                };

                string clave = string.Empty;
                string label = string.Empty;
                string geometry = string.Empty;
                try
                {
                    clave = atributos.GetAtributoClave().Campo;
                    label = atributos.GetAtributoLabel().Campo;
                    if (tmpComp.Graficos.GetValueOrDefault() != 5)
                    {
                        geometry = (atributos.GetAtributoGeometry() ?? new Atributo()).Campo;
                    }
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", ex);
                    throw;
                }
                using (var db = GeoSITMContext.CreateContext())
                {
                    int MAX_CANT_ID = 900;
                    db.Database.Connection.Open();

                    int idx = 0;
                    while (idx < c.Objetos.Count)
                    {
                        var objetos = c.Objetos.GetRange(idx, Math.Min(MAX_CANT_ID, c.Objetos.Count - idx));
                        idx += MAX_CANT_ID;

                        string query = string.Format("SELECT T.{0}, T.{1} FROM {2}.{3} T WHERE T.{0} IN({4})", clave, label, componente.Esquema, componente.Tabla, string.Join(",", objetos));

                        try
                        {
                            IDbCommand objComm = db.Database.Connection.CreateCommand();
                            objComm.CommandText = query;
                            IDataReader data = objComm.ExecuteReader();
                            while (data.Read())
                            {
                                var objeto = new Objeto
                                {
                                    ObjetoId = Convert.ToInt64(data.GetValue(0)),
                                    Descripcion = Convert.ToString(data.GetValue(1)),
                                };
                                componente.Objetos.Add(objeto);
                                //componente.Objetos.OrderBy(x => x.Descripcion);
                            }
                            componente.Objetos = componente.Objetos.OrderBy(x => x.Descripcion).ToList();

                            data.Close();
                        }
                        catch (Exception ex)
                        {
                            Global.GetLogger().LogError("ColeccionController - GetObjetosByComponentes", ex);
                        }
                    }
                }
                if (componente.Objetos.Count > 0)
                    model.Componentes.Add(componente);
                //model.Componentes.OrderBy(x => componente.Objetos.OrderBy(c=> c.Descripcion));
                //model.Componentes.OrderBy(x => componente.Objetos).ThenBy(c => c.Objetos.OrderBy);
            }
        }

        private int GetColeccionCantidad(long conleccionId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                try
                {
                    return db.ColeccionComponente.Count(x => x.ColeccionId == conleccionId && x.FechaBaja == null);
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("ColeccionController - GetColeccionCantidad", ex);
                    throw ex;
                }
            }
        }

        private ColeccionModel InicializaColeccionModel(Coleccion coleccion)
        {
            var model = new ColeccionModel();
            //_unitOfWork.ColeccionRepository.GetComponentes(coleccion);

            if (coleccion == null || coleccion.Componentes == null)
                return model;

            model.ColeccionId = coleccion.ColeccionId;
            model.Nombre = coleccion.Nombre;

            var componentes = coleccion.Componentes
                .Where(c => c.UsuarioBaja == null)
                .GroupBy(g => g.ComponenteId)
                .Select(i => new { IdComponente = i.Key, Objetos = i.Select(o => o.ObjetoId).ToList() });
            long componente_carga_tecnica = long.Parse(_unitOfWork.ParametroRepository.GetParametroByDescripcion("ID_COMPONENTE_CARGA_TECNICA"));
            var lista = new List<long>();
            foreach (var c in componentes)
            {
                var tmpComp = _unitOfWork.ComponenteRepository.GetComponenteById((int)c.IdComponente);
                var atributos = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(c.IdComponente);
                var componente = new Models.Componente
                {
                    ComponenteId = tmpComp.ComponenteId,
                    Nombre = tmpComp.Descripcion,
                    Esquema = tmpComp.Esquema,
                    Tabla = tmpComp.Tabla,
                    TablaGrafica = tmpComp.TablaGrafica,
                    Ruteable = tmpComp.DocType == "parcelas" || tmpComp.Graficos == 3,
                    DocType = tmpComp.DocType,
                    Capa = tmpComp.Capa,
                    AplicaFiltro = tmpComp.ComponenteId == componente_carga_tecnica
                };

                string clave = string.Empty;
                string label = string.Empty;
                Atributo campoGeometry = null;
                try
                {
                    clave = atributos.GetAtributoClave().Campo;
                    label = atributos.GetAtributoLabel().Campo;
                    campoGeometry = atributos.GetAtributoGeometry();
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", ex);
                    throw;
                }

                using (var db = GeoSITMContext.CreateContext())
                {
                    int MAX_CANT_ID = 900;
                    db.Database.Connection.Open();

                    int idx = 0;
                    while (idx < c.Objetos.Count)
                    {
                        var objetos = c.Objetos.GetRange(idx, Math.Min(MAX_CANT_ID, c.Objetos.Count - idx));
                        idx += MAX_CANT_ID;

                        string representacionGrafica = string.Empty;


                        /* si tiene tabla grafica no deberia rutear y como este metodo se llama de muchos lados, 
                         * es lo mas facil de hacer para no reescribir todo. 
                         * Esta parte solo se usa en Ruteo.... pero para variar, se sobrecarga en un solo lugar 
                         * con operaciones que no se usan */
                        if (string.IsNullOrEmpty(tmpComp.TablaGrafica) && tmpComp.Graficos.GetValueOrDefault() > 0 && tmpComp.Graficos.GetValueOrDefault() < 5)
                        {
                            /*
                             * si es una linea se trabaja con la geometria convertida al sistema LRS
                             *      1.- se convierte la linea a LRS 
                             *          --> SDO_LRS.CONVERT_TO_LRS_GEOM(geometria)
                             *      2.- se calcula la longitud y se la divide por 2 para obtener la distancia media 
                             *          --> SDO_GEOM.SDO_LENGTH(geometria, tolerancia) / 2
                             *      3.- se busca el punto sobre la linea mas cercano a esa distancia 
                             *          --> SDO_LRS.LOCATE_PT(geometria, distancia)
                             *      4.- se vuelve a convertir a una geometria estandar 
                             *          --> SDO_LRS.CONVERT_TO_STD_GEOM(geometria)
                             * 
                             * si es una geometria poligonal o punto, se puede obtener  el punto centroide 
                             * sin necesidad de ninguna conversion extra
                             */

                            /* para ambos casos se aplica una transformacion a LL84 y se obtiene puntos X (longitud) e Y (latitud) */

                            representacionGrafica = string.Format(string.Format("SDO_CS.TRANSFORM({0}, 8307).SDO_POINT.X AS X, SDO_CS.TRANSFORM({0}, 8307).SDO_POINT.Y AS Y", "CASE WHEN A.{0}.GET_GTYPE() = 2 THEN SDO_LRS.CONVERT_TO_STD_GEOM(SDO_LRS.LOCATE_PT(SDO_LRS.CONVERT_TO_LRS_GEOM(A.{0}), SDO_GEOM.SDO_LENGTH(A.{0}, 0.005)/2)) ELSE SDO_GEOM.SDO_CENTROID(A.{0},0.005) END"), campoGeometry.Campo);
                        }
                        else
                        {
                            /* al tener muchos objetos graficos o al no tener representacion grafica, lat y lon en 0 */
                            representacionGrafica = "0,0";
                        }

                        try
                        {
                            IDbCommand objComm = db.Database.Connection.CreateCommand();
                            objComm.CommandText = string.Format("SELECT A.{0}, A.{1}, {5} FROM {2}.{3} A WHERE A.{0} IN({4})", clave, label, componente.Esquema, componente.Tabla, string.Join(",", objetos), representacionGrafica);
                            IDataReader data = objComm.ExecuteReader();
                            while (data.Read())
                            {
                                var objeto = new Objeto
                                {
                                    ObjetoId = Convert.ToInt64(data.GetValue(0)),
                                    Descripcion = Convert.ToString(data.GetValue(1)),
                                    Longitud = Convert.ToDouble(data.GetValue(2)),
                                    Latitud = Convert.ToDouble(data.GetValue(3))
                                };
                                componente.Objetos.Add(objeto);
                            }
                            data.Close();
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                if (componente.Objetos.Count > 0)
                    model.Componentes.Add(componente);
            }
            return model;
        }

        [HttpGet]
        public string ObtenerDireccion(long idObjeto, long idComponente)
        {
            // 63534   2345

            //PASAR A METODOO
            Objeto obj = new Objeto();
            var atributos = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(idComponente);
            var tmpComp = _unitOfWork.ComponenteRepository.GetComponenteById((int)idComponente);
            string clave = string.Empty;
            string label = string.Empty;

            try
            {

                clave = atributos.GetAtributoClave().Campo;
                label = atributos.GetAtributoLabel().Campo;
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Componente (id: " + idComponente + ") mal configurado.", ex);
                throw;
            }

            using (var db = GeoSITMContext.CreateContext())
            {
                db.Database.Connection.Open();
                //int idx = 0;               
                //    //var objetos = c.Objetos.GetRange(idx, Math.Min(MAX_CANT_ID, c.Objetos.Count - idx));
                //    idx += MAX_CANT_ID;

                /* para ambos casos se aplica una transformacion a LL84 y se obtiene puntos X (longitud) e Y (latitud) */
                string representacionGrafica = "SDO_CS.TRANSFORM({0}, 8307).SDO_POINT.X, SDO_CS.TRANSFORM({0}, 8307).SDO_POINT.Y";
                if (tmpComp.Graficos.HasValue && tmpComp.Graficos == 2) //tiene representacion grafica y es linea
                {
                    /*
                     * al ser una linea se trabaja con la geometria convertida al sistema LRS
                     *      1.- se convierte la linea a LRS 
                     *          --> SDO_LRS.CONVERT_TO_LRS_GEOM(geometria)
                     *      2.- se calcula la longitud y se la divide por 2 para obtener la distancia media 
                     *          --> SDO_GEOM.SDO_LENGTH(geometria, tolerancia) / 2
                     *      3.- se busca el punto sobre la linea mas cercano a esa distancia 
                     *          --> SDO_LRS.LOCATE_PT(geometria, distancia)
                     *      4.- se vuelve a convertir a una geometria estandar 
                     *          --> SDO_LRS.CONVERT_TO_STD_GEOM(geometria)
                     */
                    representacionGrafica = string.Format(representacionGrafica, "SDO_LRS.CONVERT_TO_STD_GEOM(SDO_LRS.LOCATE_PT(SDO_LRS.CONVERT_TO_LRS_GEOM(GEOMETRY), SDO_GEOM.SDO_LENGTH(GEOMETRY, 0.005)/2))");
                }
                else if (tmpComp.Graficos.HasValue && tmpComp.Graficos != 5) //cualquier otro tipo de representacion grafica
                {
                    /* 
                     * al ser una geometria poligonal o punto, se puede obtener 
                     * el punto centroide sin necesidad de ninguna conversion extra
                     */
                    representacionGrafica = string.Format(representacionGrafica, "SDO_GEOM.SDO_CENTROID(GEOMETRY,0.005)");
                }
                else
                {
                    /* al no tener representacion grafica, lat y lon en 0*/
                    representacionGrafica = "0,0";
                }

                var query = string.Format("SELECT {0}, {1}, {5} FROM {2}.{3} WHERE {0} =({4})", clave, label, tmpComp.Esquema, tmpComp.Tabla, idObjeto, representacionGrafica);

                try
                {
                    IDbCommand objComm = db.Database.Connection.CreateCommand();
                    objComm.CommandText = query;
                    IDataReader data = objComm.ExecuteReader();
                    while (data.Read())
                    {

                        obj.ObjetoId = Convert.ToInt64(data.GetValue(0));
                        obj.Descripcion = Convert.ToString(data.GetValue(1));
                        obj.Longitud = Convert.ToDouble(data.GetValue(2));
                        obj.Latitud = Convert.ToDouble(data.GetValue(3));
                    }
                    data.Close();
                }
                catch
                {
                    return null;
                }

            }
            var resp = (new RuteoController(this._unitOfWork) { ControllerContext = this.ControllerContext })
                            .ObtenerDireccion(obj.Latitud, obj.Longitud)
                            .ExecuteAsync(CancellationToken.None)
                            .Result;
            return resp.Content.ReadAsStringAsync().Result.Substring(1, resp.Content.ReadAsStringAsync().Result.Length - 2);
        }


        private void CargarColeccionAtributos(Componente componente, Objeto model, long objetoId)
        {
            Atributo atributoClave = null;
            try
            {
                atributoClave = componente.Atributos.GetAtributoClave();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", ex);
            }

            string[] arrayCampos = componente.Atributos.Where(a => a.EsVisible & !a.EsGeometry).Select(a => a.Campo != null ? a.Campo : a.Funcion.Replace("@T@.", "")).ToArray();
            Atributo[] array = componente.Atributos.Where(a => a.EsVisible & !a.EsGeometry).ToArray();

            string query = string.Format("SELECT {0} FROM {1}.{2} WHERE {3} = {4}",
                                        string.Join(",", arrayCampos),
                                        componente.Esquema,
                                        componente.Tabla,
                                        atributoClave.Campo,
                                        objetoId);

            using (var db = GeoSITMContext.CreateContext())
            {
                try
                {
                    var objComm = db.Database.Connection.CreateCommand();
                    db.Database.Connection.Open();
                    objComm.CommandText = query;
                    var data = objComm.ExecuteReader();
                    data.Read();
                    for (var i = 0; i < array.Length; i++)
                    {
                        var objeto = new AtributoObjeto { AtributoId = array[i].AtributoId, Nombre = array[i].Nombre, Valor = data.GetValue(i).ToString() };
                        model.Atributos.Add(objeto);
                    }
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("ColeccionController - CargarColeccionAtributos", ex);
                }
            }
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult CopiarColeccion(int usuarioId, int coleccionId, string nombreColeccion)
        {
            try
            {
                if (usuarioId < 0 || coleccionId < 0 || string.IsNullOrWhiteSpace(nombreColeccion))
                    throw new Exception("Parametros invalidos.");

                var coleccionOrigen = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId);
                if (coleccionOrigen == null)
                    return NotFound();

                var coleccionDestino = new Coleccion
                {
                    Nombre = nombreColeccion,
                    UsuarioAlta = usuarioId,
                    FechaAlta = DateTime.Now,
                    UsuarioModificacion = usuarioId,
                    FechaModificacion = DateTime.Now
                };

                _unitOfWork.ColeccionRepository.NuevaColeccion(coleccionDestino);
                _unitOfWork.ColeccionRepository.AgregarComponentesColeccion(coleccionOrigen, coleccionDestino, false);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - CopiarColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UnirColecciones(int usuarioId, int coleccionId1, int coleccionId2, string nombreColeccion)
        {
            try
            {
                if (usuarioId < 0 || coleccionId1 < 0 || coleccionId1 < 0 || string.IsNullOrWhiteSpace(nombreColeccion))
                    throw new Exception("Parametros invalidos.");

                var coleccionOrigen1 = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId1);
                if (coleccionOrigen1 == null)
                    return NotFound();
                var coleccionOrigen2 = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId2);
                if (coleccionOrigen2 == null)
                    return NotFound();

                var coleccionDestino = new Coleccion
                {
                    Nombre = nombreColeccion,
                    UsuarioAlta = usuarioId,
                    FechaAlta = DateTime.Now,
                    UsuarioModificacion = usuarioId,
                    FechaModificacion = DateTime.Now
                };

                _unitOfWork.ColeccionRepository.NuevaColeccion(coleccionDestino);
                _unitOfWork.ColeccionRepository.AgregarComponentesColeccion(coleccionOrigen1, coleccionDestino, false);
                _unitOfWork.ColeccionRepository.AgregarComponentesColeccion(coleccionOrigen2, coleccionDestino, true);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - UnirColecciones", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult RenombrarColeccion(int usuarioId, int coleccionId, string nombreColeccion)
        {
            try
            {
                if (usuarioId < 0 || coleccionId < 0 || string.IsNullOrWhiteSpace(nombreColeccion))
                    throw new Exception("Parametros invalidos.");

                var coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId);
                if (coleccion == null)
                    return NotFound();

                coleccion.Nombre = nombreColeccion;
                coleccion.UsuarioModificacion = usuarioId;
                coleccion.FechaModificacion = DateTime.Now;
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - RenombrarColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult BajaColeccion(int usuarioId, long coleccionId)
        {
            try
            {
                if (usuarioId < 0 || coleccionId < 0)
                    throw new Exception("Parametros invalidos.");

                var coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId);
                if (coleccion == null)
                    return NotFound();
                DateTime ahora = DateTime.Now;
                coleccion.UsuarioBaja = usuarioId;
                coleccion.FechaBaja = ahora;
                coleccion.UsuarioModificacion = usuarioId;
                coleccion.FechaModificacion = ahora;

                if (coleccion.Componentes != null)
                {
                    foreach (var item in coleccion.Componentes)
                    {
                        item.FechaBaja = ahora;
                        item.UsuarioBaja = usuarioId;
                    }
                }
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - BajaColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult BajaColecciones(int usuarioId, string coleccionId)
        {
            try
            {
                foreach (long colec in coleccionId.Split(',').Select(c => Convert.ToInt64(c)))
                {
                    this.BajaColeccion(usuarioId, colec);
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - BajaColecciones", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult QuitarObjetoColeccion(long usuarioId, long objetoId, long componenteId, long coleccionId)
        {
            try
            {
                if (usuarioId < 0 || coleccionId < 0 || componenteId < 0 || objetoId < 0)
                    throw new Exception("Parametros invalidos.");

                var result = _unitOfWork.ColeccionRepository.QuitarObjetoColeccion(usuarioId, objetoId, componenteId, coleccionId);
                if (!result)
                    return NotFound();

                var coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId);
                coleccion.UsuarioModificacion = usuarioId;
                coleccion.FechaModificacion = DateTime.Now;
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - QuitarObjetoColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }
        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult QuitarMultiplesObjetos(long usuarioId, string objetoId, long componenteId, long coleccionId)
        {
            try
            {

                if (usuarioId < 0 || coleccionId < 0 || componenteId < 0 || string.IsNullOrEmpty(objetoId?.Trim()))
                    throw new Exception("Parametros invalidos.");

                foreach (long obj in objetoId.Split(',').Select(o => Convert.ToInt64(o)))
                {
                    this.QuitarObjetoColeccion(usuarioId, obj, componenteId, coleccionId);
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - QuitarObjetoColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }


        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult AgregarObjetoColeccion(int usuarioId, long objetoId, int componenteId, int coleccionId)
        {
            try
            {
                if (usuarioId < 0 || coleccionId < 0 || componenteId < 0 || objetoId < 0)
                    throw new Exception("Parametros invalidos.");

                var result = _unitOfWork.ColeccionRepository.AgregarObjetoColeccion(usuarioId, objetoId, componenteId, coleccionId);
                if (!result)
                    return NotFound();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - AgregarObjetoColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult LimpiarColeccion(int usuarioId, int coleccionId)
        {
            try
            {
                if (usuarioId < 0 || coleccionId < 0)
                    throw new Exception("Parametros invalidos.");

                var coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId);
                if (coleccion == null)
                    return NotFound();

                if (coleccion.Componentes != null)
                {
                    foreach (var item in coleccion.Componentes)
                    {
                        item.UsuarioBaja = usuarioId;
                        item.FechaBaja = DateTime.Now;
                    }
                }
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - LimpiarColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        //[HttpGet]
        //[ResponseType(typeof(bool))]
        //public IHttpActionResult NuevaColeccion(int usuarioId, string nombreColeccion)
        //{
        //    try
        //    {
        //        if (usuarioId < 0 || string.IsNullOrWhiteSpace(nombreColeccion))
        //            throw new Exception("Parametros invalidos.");

        //        var coleccion = new Coleccion
        //        {
        //            Nombre = nombreColeccion,
        //            UsuarioAlta = usuarioId,
        //            FechaAlta = DateTime.Now,
        //            UsuarioModificacion = usuarioId,
        //            UsuarioBaja = null,
        //            FechaModificacion = DateTime.Now
        //        };

        //        _unitOfWork.ColeccionRepository.NuevaColeccion(coleccion);
        //        _unitOfWork.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        Global.GetLogger().LogError("ColeccionController - NuevaColeccion", ex);
        //        return InternalServerError(ex);
        //    }

        //    return Ok(true);
        //}

        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult NuevaColeccion(long usuarioId, string nombre, ObjetoComponente[] objetos)
        {
            try
            {
                if (usuarioId <= 0)
                {
                    return Unauthorized();
                }
                else if (string.IsNullOrEmpty(nombre))
                {
                    return BadRequest("nombre");
                }
                else if (objetos == null || objetos.Length == 0)
                {
                    return BadRequest("objetos");
                }
                else if (_unitOfWork.ColeccionRepository.ValidarNombreColeccion(usuarioId, nombre))
                {
                    return Conflict();
                }
                else
                {
                    DateTime fecha = DateTime.Now;
                    var coleccion = new Coleccion
                    {
                        Nombre = nombre,
                        UsuarioAlta = usuarioId,
                        FechaAlta = fecha,
                        UsuarioModificacion = usuarioId,
                        FechaModificacion = fecha,
                        Componentes = objetos.GroupBy(obj => obj.ComponenteDocType, elem => elem.ObjetoId)
                                             .SelectMany(grp =>
                                             {
                                                 long cmpId = _unitOfWork.ComponenteRepository
                                                                         .GetComponenteByDocType(grp.Key)
                                                                         .ComponenteId;
                                                 return grp.Select(id => new ColeccionComponente()
                                                 {
                                                     ComponenteId = cmpId,
                                                     ObjetoId = id,
                                                     FechaAlta = fecha,
                                                     UsuarioAlta = usuarioId
                                                 });
                                             }).ToList()
                    };

                    _unitOfWork.ColeccionRepository.NuevaColeccion(coleccion);
                    _unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - NuevaColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult ValidarNombreColeccion(long usuarioId, string nombreColeccion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreColeccion))
                    throw new Exception("Parametros invalidos.");
                bool existe = _unitOfWork.ColeccionRepository.ValidarNombreColeccion(usuarioId, nombreColeccion);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - ValidarNombreColeccion", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult ImportarNuevaColeccion(ColeccionModel coleccion)
        {
            int contadorObjetosValidos = 0;
            try
            {
                if (coleccion == null)
                    throw new Exception("Parametros invalidos.");

                var nuevaColeccion = new Coleccion
                {
                    Nombre = coleccion.Nombre,
                    UsuarioAlta = coleccion.UsuarioId,
                    FechaAlta = DateTime.Now,
                    UsuarioModificacion = coleccion.UsuarioId,
                    FechaModificacion = DateTime.Now
                };

                _unitOfWork.ColeccionRepository.NuevaColeccion(nuevaColeccion);

                foreach (var item in coleccion.Componentes)
                {
                    //validar existencia componente
                    var componente = _unitOfWork.ComponenteRepository.GetComponenteById(item.ComponenteId);
                    _unitOfWork.ColeccionRepository.GetAtributos(componente);

                    if (componente == null)
                    {
                        continue;
                    }

                    foreach (var objeto in item.Objetos)
                    {
                        //validar existencia objeto
                        var objAux = new Objeto();
                        CargarColeccionAtributos(componente, objAux, objeto.ObjetoId);
                        if (objAux != null && !objAux.Atributos.Any())
                        {
                            continue;
                        }

                        var colecCompo = new ColeccionComponente
                        {
                            ObjetoId = objeto.ObjetoId,
                            ColeccionId = nuevaColeccion.ColeccionId,
                            UsuarioAlta = coleccion.UsuarioId,
                            FechaAlta = DateTime.Now,
                            ComponenteId = item.ComponenteId
                        };

                        if (!_unitOfWork.ColeccionRepository.AgregarColeccionComponente(colecCompo))
                        {
                            continue;
                        }

                        contadorObjetosValidos++;
                    }
                }

                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - ImportarNuevaColeccion", ex);
                return InternalServerError(ex);
            }

            return Ok(contadorObjetosValidos);
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult ColeccionMedianteSeleccionObjetos(int usuarioId, int coleccionId, List<ObjetoComponente> objetoComponente)
        {
            try
            {
                var coleccion = _unitOfWork.ColeccionRepository.GetColeccionById(coleccionId);
                //_unitOfWork.ColeccionRepository.GetComponentes(coleccion);

                if (coleccion == null)
                    throw new Exception("Parametros invalidos.");

                foreach (var item in objetoComponente)
                {
                    //validar existencia componente
                    var componente = _unitOfWork.ComponenteRepository.GetComponenteByDocType(item.ComponenteDocType);
                    if (componente == null)
                    {
                        continue;
                    }

                    if (coleccion.Componentes != null && coleccion.Componentes.Any(c => c.ComponenteId == componente.ComponenteId && c.ObjetoId == item.ObjetoId))
                    {
                        continue;
                    }

                    var colecCompo = new ColeccionComponente
                    {
                        ObjetoId = item.ObjetoId,
                        ColeccionId = coleccionId,
                        UsuarioAlta = usuarioId,
                        FechaAlta = DateTime.Now,
                        ComponenteId = componente.ComponenteId
                    };

                    _unitOfWork.ColeccionRepository.AgregarColeccionComponente(colecCompo);
                }

                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ColeccionController - ColeccionMedianteSeleccionObjetos", ex);
                return InternalServerError(ex);
            }

            return Ok(true);
        }


        public Coleccion[] ObtenerAllColeccionesAreasByUsuario(long idUsuario)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                //ComponentesTipoArea
                List<long> lstIdCompArea = db.Componente.Where(c => c.Graficos == 1).Select(c => c.ComponenteId).ToList();
                //La condicion de usuario esta con la coleccion, no con los componentes de la coleccion
                Coleccion[] resultColecciones = db.ColeccionComponente.Where(c => lstIdCompArea.Contains(c.ComponenteId) && !c.FechaBaja.HasValue && c.Coleccion.UsuarioAlta == idUsuario).Select(c => c.Coleccion).Distinct().ToArray();
                return resultColecciones;
            }
        }
        [HttpGet]
        [ResponseType(typeof(List<Coleccion>))]
        public IHttpActionResult ObtenerAllColeccionesAreasByUsuarioWeb(long idUsuario)
        {
            return Ok(ObtenerAllColeccionesAreasByUsuario(idUsuario));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idColeccion">Id de la coleccion que se desea obtener los componentes</param>
        /// <returns>Devuelve los ColeccionComponente con el componente tipo area, de la coleccion que recibe por parametro</returns>
        public List<ColeccionComponente> ObtenerObjetosColeccionAreaByColeccion(long idColeccion)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                //Obtengo los componentes de la coleccion
                List<ColeccionComponente> resultColecciones = db.ColeccionComponente.Where(c => c.ColeccionId == idColeccion && !c.FechaBaja.HasValue).ToList();
                //foreach (var comp in resultColecciones)//Cambiar por un while
                for (int i = 0; i < resultColecciones.Count; i++)
                {
                    var resComp = db.Componente.Find(resultColecciones[i].ComponenteId);
                    if (resComp.Graficos != 1) //Si no es tipo area lo elimino de la lista
                    {
                        resultColecciones.Remove(resultColecciones[i]);
                        i--;
                    }

                }

                return resultColecciones;
            }
        }
        [HttpPost]
        [ResponseType(typeof(List<ColeccionComponente>))]
        public IHttpActionResult ObtenerObjetosColeccionAreaByColeccionWeb(long idColeccion)
        {
            return Ok(ObtenerObjetosColeccionAreaByColeccion(idColeccion));
        }




        ////

        public Coleccion[] ColeccionesByComponentes(List<long> idsComponentes)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                Coleccion[] resultColecciones = db.ColeccionComponente.Where(c => idsComponentes.Contains(c.ComponenteId) && !c.FechaBaja.HasValue).Select(c => c.Coleccion).Distinct().ToArray();
                return resultColecciones;
            }
        }
        [HttpPost]
        [ResponseType(typeof(List<Coleccion>))]
        public IHttpActionResult ColeccionesByComponentesWeb(List<long> idsComponentes)
        {

            return Ok(ColeccionesByComponentes(idsComponentes));
        }
        public ColeccionComponente[] ObtenerObjetosColeccionByColeccion(List<long> idsComponentes)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                ColeccionComponente[] resultColecciones = db.ColeccionComponente.Where(c => idsComponentes.Contains(c.ComponenteId) && !c.FechaBaja.HasValue).ToArray();
                return resultColecciones;
            }
        }
        [HttpPost]
        [ResponseType(typeof(List<ColeccionComponente[]>))]
        public IHttpActionResult ObtenerObjetosColeccionByColeccionWeb(List<long> idsComponentes)
        {

            return Ok(ObtenerObjetosColeccionByColeccion(idsComponentes));
        }
    }
}
