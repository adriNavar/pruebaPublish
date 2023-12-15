using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.IO;
using System.Data.Entity.Spatial;
using System.Drawing;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using Newtonsoft.Json;
using System.Text;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Componentes;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common.Enums;
using System.Net.Http.Formatting;

namespace GeoSit.Web.Api.Controllers
{
    public class MapasTematicosServiceController : ApiController
    {
        private readonly GeoSITMContext db = null;
        public MapasTematicosServiceController()
        {
            db = GeoSITMContext.CreateContext();
        }
        // GET api/Componentes
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentes()
        {
            return Ok(db.Componente.Where(m => !m.EsTemporal && !m.EsLista).ToList());
        }
        // GET api/Componentes
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentesGeograficos(bool excluirTemporales = true)
        {
            return Ok(db.Componente
                            .Where(m => !m.EsLista && (!excluirTemporales || !m.EsTemporal) && m.Graficos < 5 & m.Graficos > 0)
                            .OrderBy(m => m.Nombre).ToList());
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentesBusqueda(string id)
        {
            List<Componente> componente = new List<Componente>(db.Componente.Where(a => !a.EsTemporal && a.Nombre.ToLower().Contains(id.ToLower())));
            if (componente == null)
            {
                return NotFound();
            }
            return Ok(componente);
        }
        [HttpGet]
        [ResponseType(typeof(Componente))]
        public IHttpActionResult GetComponentesById(long id)
        {
            Componente componente = db.Componente.Where(a => a.ComponenteId == id).FirstOrDefault();
            if (componente == null)
            {
                return NotFound();
            }
            return Ok(componente);
        }

        [HttpGet]
        [ResponseType(typeof(Componente))]
        public IHttpActionResult GetComponenteByEsquemaTabla(string esquema, string tabla)
        {
            Componente componente = db.Componente.Where(a => a.Esquema == esquema && a.Tabla == tabla).FirstOrDefault();
            if (componente == null)
            {
                return NotFound();
            }
            return Ok(componente);
        }

        [HttpGet]
        [ResponseType(typeof(Componente))]
        public IHttpActionResult GetComponenteByCapa(string layer)
        {
            Componente componente = db.Componente.GetComponenteByCapa(layer);
            if (componente == null)
            {
                return NotFound();
            }
            return Ok(componente);
        }

        [HttpGet]
        [ResponseType(typeof(Agrupacion))]
        public IHttpActionResult GetAgrupacionesById(long id)
        {
            Agrupacion agrupacion = db.Agrupacion.Where(a => a.AgrupacionId == id).FirstOrDefault();
            if (agrupacion == null)
            {
                return NotFound();
            }
            return Ok(agrupacion);
        }
        [HttpGet]
        [ResponseType(typeof(Atributo))]
        public IHttpActionResult GetAtributosById(long id)
        {
            Atributo atributo = db.Atributo.Where(a => a.AtributoId == id).FirstOrDefault();
            if (atributo == null)
            {
                return NotFound();
            }
            return Ok(atributo);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentesRelacionados(long id)
        {
            List<Componente> componentes = new List<Componente>();
            componentes.AddRange(obtenerComponentesPadres(id));
            componentes.AddRange(new List<Componente>(db.Componente.Where(a => a.ComponenteId == id)));
            componentes.AddRange(obtenerComponentesHijos(id));

            if (componentes == null)
            {
                return NotFound();
            }

            return Ok(componentes.Distinct().OrderBy(c => c.Nombre).ToList());
        }
        [HttpGet]
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentesByPadre(long id)
        {

            List<Componente> componentes = new List<Componente>(db.Componente.Where(a => a.ComponenteId == id));

            if (componentes == null)
            {
                return NotFound();
            }
            componentes.AddRange(obtenerComponentesHijos(id).Distinct().ToList());
            return Ok(componentes.OrderBy(c => c.Nombre));
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Plantilla>))]
        public IHttpActionResult GetPlantillasForMapaTematico()
        {
            List<Plantilla> plantillas = db.Plantillas.Where(p => p.IdPlantillaCategoria == 6).ToList();
            if (plantillas == null)
            {
                return NotFound();
            }
            return Ok(plantillas.ToList());
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetParametrosGenerales()
        {
            List<ParametrosGenerales> lstParametrosGenerales = db.ParametrosGenerales.ToList();
            return Ok(lstParametrosGenerales);
        }

        private List<Componente> obtenerComponentesHijos(long idComponente)
        {

            List<Componente> componentes = new List<Componente>();

            List<Jerarquia> arbolJerarquiasPlano = obtenerJerarquiasHijas(idComponente);
            foreach (var item in arbolJerarquiasPlano)
            {
                componentes.AddRange(db.Componente.Where(a => a.ComponenteId == item.ComponenteInferiorId));
            }

            return componentes;
        }
        private List<Componente> obtenerComponentesPadres(long idComponente)
        {

            List<Componente> componentes = new List<Componente>();

            List<Jerarquia> arbolJerarquiasPlano = obtenerJerarquiasPadres(idComponente);
            foreach (var item in arbolJerarquiasPlano)
            {
                componentes.AddRange(db.Componente.Where(a => a.ComponenteId == item.ComponenteSuperiorId));
            }

            return componentes;
        }
        [HttpGet]
        [ResponseType(typeof(ICollection<DatoExternoConfiguracion>))]
        public IHttpActionResult GetDatosExternosConfiguracionById(long id)
        {
            List<DatoExternoConfiguracion> datoexternoconfiguracion = new List<DatoExternoConfiguracion>(db.DatoExternoConfiguracion.Where(a => a.DatoExternoConfiguracionId == id));
            if (datoexternoconfiguracion == null)
            {
                return NotFound();
            }
            return Ok(datoexternoconfiguracion);
        }

        private List<Jerarquia> obtenerJerarquiasHijas(long Idjerarquia)
        {
            var jerarquias = db.Jerarquia.Where(a => a.ComponenteSuperiorId == Idjerarquia).ToList();
            var jerarquiashijas = new List<Jerarquia>();
            if (jerarquias != null && jerarquias.Count > 0)
            {
                foreach (var item in jerarquias)
                {
                    jerarquiashijas.AddRange(obtenerJerarquiasHijas(item.ComponenteInferiorId));
                }
                jerarquias.AddRange(jerarquiashijas);
            }
            return jerarquias;
        }
        private List<Jerarquia> obtenerJerarquiasPadres(long Idjerarquia)
        {
            var jerarquias = db.Jerarquia.Where(a => a.ComponenteInferiorId == Idjerarquia).ToList();
            var jerarquiasPadres = new List<Jerarquia>();
            if (jerarquias != null && jerarquias.Count > 0)
            {
                foreach (var item in jerarquias)
                {
                    jerarquiasPadres.AddRange(obtenerJerarquiasPadres(item.ComponenteSuperiorId));
                }
                jerarquias.AddRange(jerarquiasPadres);
            }
            return jerarquias;
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Atributo>))]
        public IHttpActionResult GetAtributosByComponente(long id)
        {
            List<Atributo> atributos = new List<Atributo>(db.Atributo.Where(a => a.ComponenteId == id).Where(a => a.EsVisible));
            if (atributos == null)
            {
                return NotFound();
            }

            return Ok(atributos);
        }

        [HttpGet]
        [ResponseType(typeof(Atributo))]
        public IHttpActionResult GetAtributoFEATIDByComponente(long id)
        {
            try
            {
                return Ok(db.Atributo.GetAtributoFeatIdByComponente(id));
            }
            catch (ApplicationException appEx)
            {
                Global.GetLogger().LogError("Componente (id: " + id + ") mal configurado.", appEx);
                return InternalServerError(appEx);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("GetAtributoFEATIDByComponente", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Atributo>))]
        public IHttpActionResult GetAtributos()
        {
            List<Atributo> atributos = db.Atributo.ToList<Atributo>();
            if (atributos == null)
            {
                return NotFound();
            }
            return Ok(atributos);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<TipoOperacion>))]
        public IHttpActionResult GetOperaciones()
        {
            List<TipoOperacion> atributos = db.TipoOperacion.ToList<TipoOperacion>();
            if (atributos == null)
            {
                return NotFound();
            }
            else
            {
                atributos = atributos.OrderBy(t => t.TipoOperacionId).ToList();
            }
            return Ok(atributos);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<TipoOperacion>))]
        public IHttpActionResult GetOperacionesEspeciales(long id)
        {
            var objeto = (from atributo in db.Atributo
                          join componente in db.Componente on atributo.ComponenteId equals componente.ComponenteId
                          where atributo.AtributoId == id
                          select new
                          {
                              tipoDato = atributo.TipoDatoId,
                              campoNombre = atributo.Campo,
                              esquema = componente.Esquema,
                              tabla = componente.Tabla,
                              esLista = componente.EsLista,
                              idComponente = componente.ComponenteId
                          })
                         .First();

            using (var builder = db.CreateSQLQueryBuilder())
            {
                builder.AddTable(objeto.esquema, objeto.tabla, "t1");

                //FL 20190326: Agregado para tener en cuenta la fecha de baja y no mostrar esos valores. (BUG 8273)
                if (db.Atributo.Any(x => x.Campo == "FECHA_BAJA" && x.ComponenteId == objeto.idComponente))
                {
                    builder.AddFilter("FECHA_BAJA", null, SQLOperators.IsNull);
                }
                var lista = builder
                                .AddFields(objeto.campoNombre)
                                .Distinct()
                                .OrderBy(SQLSort.Asc, objeto.campoNombre)
                                .ExecuteQuery((IDataReader reader) =>
                                {
                                    return new TipoOperacion()
                                    {
                                        Nombre = reader.GetStringOrEmpty(0),
                                        CantidadValores = 0,
                                        TipoOperacionId = 0,
                                        TipoFiltroId = 1
                                    };
                                });

                return Ok(lista);
            }
        }

        [HttpGet]
        [ResponseType(typeof(DatoExternoConfiguracion))]
        public IHttpActionResult GrabarDatoExterno(long macheo, long columna, string descripcion, long fileid, long componenteId, string columnaMacheo, long usuarioId)
        {
            FileColumn fileColumnDatos = db.FileColumn.Single(f => f.FileDescriptorId == fileid && f.IndiceColumna == columna);
            FileColumn fileColumnMacheo = db.FileColumn.Single(f => f.FileDescriptorId == fileid && f.IndiceColumna == macheo);

            var datos = (from valor in db.FileData
                         join clave in db.FileData on valor.IndiceFila equals clave.IndiceFila
                         where valor.FileColumnId == fileColumnDatos.FileColumnId && clave.FileColumnId == fileColumnMacheo.FileColumnId
                         select new { clave = clave.Valor, valor = valor.Valor, fila = clave.IndiceFila }).OrderBy(d => d.fila).ToArray();

            Atributo atributoMacheo = db.Atributo.SingleOrDefault(a => a.Nombre.ToUpper() == columnaMacheo.ToUpper() && a.ComponenteId == componenteId);

            if (atributoMacheo == null)
            {
                Exception ex = new Exception(string.Format("No se encuentra el atributo {0} del componente con id {1}", columnaMacheo.ToUpper(), componenteId));
                Global.GetLogger().LogError("GrabarDatoExterno", ex);
                return InternalServerError(ex);
            }

            DatoExternoConfiguracion dec = new DatoExternoConfiguracion();
            dec.Componente = componenteId;
            dec.Nombre = descripcion;
            dec.TipoDato = fileColumnDatos.TipoDato;
            dec.Usuario = usuarioId;
            dec.AtributoId = atributoMacheo.AtributoId;
            db.Entry(dec).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();

            List<DatoExterno> datosExternos = new List<DatoExterno>();
            var cargados = new HashSet<string>();
            for (int j = 0; j < datos.Length; j++)
            {
                string idObjeto = datos[j].clave;
                if (cargados.Contains(idObjeto)) continue;
                cargados.Add(idObjeto);
                DatoExterno dato = new DatoExterno();
                dato.idComponente = idObjeto;
                dato.DatoExternoConfiguracionId = dec.DatoExternoConfiguracionId;
                dato.Valor = datos[j].valor;
                datosExternos.Add(dato);
            }
            db.DatoExterno.AddRange(datosExternos);
            db.SaveChanges();

            return Ok(dec);
        }

        [HttpGet]
        [ResponseType(typeof(DatoExternoConfiguracion))]
        public IHttpActionResult GetDatoExternoConfiguracionById(long id)
        {

            DatoExternoConfiguracion datoexterno = db.DatoExternoConfiguracion.Where(a => a.DatoExternoConfiguracionId == id).FirstOrDefault();
            if (datoexterno == null)
            {
                return NotFound();
            }

            return Ok(datoexterno);
        }
        [HttpGet]
        [ResponseType(typeof(List<DatoExternoConfiguracion>))]
        public IHttpActionResult GetDatoExternoConfiguracionByUsuario(long id)
        {

            List<DatoExternoConfiguracion> datosexternos = db.DatoExternoConfiguracion.Where(a => a.Usuario == id).ToList();
            if (datosexternos == null)
            {
                return NotFound();
            }

            return Ok(datosexternos);
        }
        [HttpGet]
        public IHttpActionResult BorrarDatoExternoByUsuario(long id)
        {
            List<DatoExternoConfiguracion> datosexternos = db.DatoExternoConfiguracion.Where(a => a.Usuario == id).ToList();
            foreach (var item in datosexternos)
            {
                List<DatoExterno> listaeliminar = db.DatoExterno.Where(d => d.DatoExternoConfiguracionId == item.DatoExternoConfiguracionId).ToList();
                db.DatoExterno.RemoveRange(listaeliminar);
                db.SaveChanges();
            }
            db.DatoExternoConfiguracion.RemoveRange(datosexternos);
            db.SaveChanges();

            return Ok();
        }
        [HttpGet]
        public IHttpActionResult BorrarDatoExterno(long id)
        {
            List<DatoExterno> listaeliminar = db.DatoExterno.Where(d => d.DatoExternoConfiguracionId == id).ToList();
            db.DatoExterno.RemoveRange(listaeliminar);
            db.SaveChanges();
            DatoExternoConfiguracion datoexterno = db.DatoExternoConfiguracion.Where(a => a.DatoExternoConfiguracionId == id).FirstOrDefault();
            db.DatoExternoConfiguracion.Remove(datoexterno);
            db.SaveChanges();

            return Ok();
        }


        [HttpGet]
        [ResponseType(typeof(TipoOperacion))]
        public IHttpActionResult GetOperacionesById(long id)
        {
            TipoOperacion operacion = db.TipoOperacion.Where(c => c.TipoOperacionId == id).FirstOrDefault();
            if (operacion == null)
            {
                return NotFound();
            }
            return Ok(operacion);
        }
        [HttpGet]
        [ResponseType(typeof(ICollection<Agrupacion>))]
        public IHttpActionResult GetAgrupaciones()
        {
            List<Agrupacion> atributos = db.Agrupacion.ToList<Agrupacion>();
            if (atributos == null)
            {
                return NotFound();
            }
            return Ok(atributos);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Agrupacion>))]
        public IHttpActionResult GetAgrupacionesByAtributos(long id)
        {
            IQueryable<Agrupacion> agrupacion = db.Agrupacion;

            //si el tipo de dato es string, no incluyo ni SUM ni AVG
            if (db.Atributo.Find(id).TipoDatoId == 6)
            {
                agrupacion = agrupacion.Where(c => c.AgrupacionId == 3 || c.AgrupacionId == 4 || c.AgrupacionId == 5);
            }

            if (!agrupacion.Any())
            {
                return NotFound();
            }
            return Ok(agrupacion.OrderBy(a => a.Nombre).ToList());
        }
        [HttpPost]
        public IHttpActionResult procesarArchivo(string fileName)
        {
            try
            {
                byte[] fileContent = Request.Content.ReadAsByteArrayAsync().Result;
                List<string[]> parsedData = new List<string[]>();

                List<FileData> fileDataToSave = new List<FileData>();
                FileData fileData;
                FileColumn fileColumn;
                List<FileColumn> fileColumnToSave = new List<FileColumn>();
                FileDescriptor fileDesc = new FileDescriptor();
                fileDesc.Nombre = fileName;
                fileDesc.Path = fileName;

                fileDesc = db.FileDescriptor.Add(fileDesc);
                db.SaveChanges();

                using (StreamReader readFile = new StreamReader(new MemoryStream(fileContent), System.Text.Encoding.Default))
                {
                    string line;
                    string[] row;
                    var splitChar = ';';

                    while ((line = readFile.ReadLine()) != null)
                    {
                        int cantComa = line.Count(x => x == ',');
                        int cantPuntoComa = line.Count(x => x == ';');
                        int cantPipe = line.Count(x => x == '|');
                        if (cantComa > cantPuntoComa && cantComa > cantPipe)
                        {
                            splitChar = ',';
                        }
                        else if (cantPuntoComa > cantComa && cantPuntoComa > cantPipe)
                        {
                            splitChar = ';';
                        }
                        else if (cantPipe > cantComa && cantPipe > cantPuntoComa)
                        {
                            splitChar = '|';
                        }
                        row = line.Split(splitChar);
                        parsedData.Add(row);
                    }
                }

                if (parsedData.Count >= 1)
                {
                    int ncabecera = 0;
                    for (int columna = 0; columna < parsedData[0].Count(); columna++)
                    {
                        fileColumn = new FileColumn();
                        fileColumn.FileDescriptorId = fileDesc.FileDescriptorId;
                        fileColumn.IndiceColumna = columna;
                        fileColumn.TipoDato = this.ParseDataType(parsedData[1][columna]);
                        if (this.comparaTipos(parsedData[0][columna], parsedData[0][columna]))
                        {
                            fileColumn.Nombre = "Columa_" + columna;
                        }
                        else
                        {
                            fileColumn.Nombre = parsedData[0][columna];
                            ncabecera = 1;
                        }
                        fileColumnToSave.Add(fileColumn);
                    }
                    db.FileColumn.AddRange(fileColumnToSave);
                    db.SaveChanges();

                    using (ORAGeoSIT ctx = new ORAGeoSIT())
                    {
                        ctx.Configuration.AutoDetectChangesEnabled = false;
                        ctx.Configuration.ValidateOnSaveEnabled = false;

                        for (int fila = ncabecera; fila < parsedData.Count; fila++)
                        {
                            for (int campo = 0; campo < parsedData[fila].Count(); campo++)
                            {
                                fileData = new FileData();
                                fileData.IndiceFila = fila;
                                fileData.Valor = parsedData[fila][campo];
                                fileData.FileColumnId = fileColumnToSave[campo].FileColumnId;
                                fileDataToSave.Add(fileData);
                            }
                        }
                        if (fileDataToSave.Any())
                        {
                            ctx.FileData.AddRange(fileDataToSave);
                            ctx.SaveChanges();
                            ctx.Dispose();
                        }
                    }
                }
                return Ok(string.Format("{0}@{1}", fileDesc.FileDescriptorId, JsonConvert.SerializeObject(parsedData)));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ProcesarArchivo", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ResponseType(typeof(List<object[]>))]
        public List<object[]> calcularCoincidencia(long componenteId, long fileId)
        {
            try
            {
                Componente componente = db.Componente.Single(a => a.ComponenteId == componenteId);
                string tabla = componente.Tabla;
                List<Atributo> atributos = db.Atributo.Where(a => a.ComponenteId == componenteId && a.EsClave != 0).ToList();

                //campos referidos al archivo importado.
                List<string> queryFields = new List<string>(new string[] { "IMPORTED_FILE.INDICE_COLUMNA", "IMPORTED_FILE.VALORES_TOTALES" });

                //consulta referida al archivo importado. 
                List<string> queryTables = new List<string>(new string[] { string.Format("(SELECT INDICE_COLUMNA, COUNT(*) AS VALORES_TOTALES " +
                                                                                          "FROM MT_FILE_DATA FD " +
                                                                                          "JOIN MT_FILE_COLUMN FC ON FD.ID_FILE_COLUMN = FC.ID_FILE_COLUMN " +
                                                                                          "WHERE FC.ID_FILE_DESCRIPTOR = {0} GROUP BY INDICE_COLUMNA) IMPORTED_FILE", fileId)});

                //se itera para agregar los totales para cada tabla relacionada
                foreach (var atributo in atributos)
                {
                    queryFields.AddRange(new string[] { string.Format("'{0}' AS CAMPO_C{1}", atributo.Nombre, queryTables.Count), string.Format("NVL(TOTALES_COLUMNA_C{0},0)", queryTables.Count) });

                    queryTables.Add(string.Format("(SELECT INDICE_COLUMNA AS INDICE_COLUMNA_C{0}, COUNT(*) AS TOTALES_COLUMNA_C{0} " +
                                                  "FROM MT_FILE_DATA FD " +
                                                  "JOIN MT_FILE_COLUMN FC ON FD.ID_FILE_COLUMN = FC.ID_FILE_COLUMN " +
                                                  "JOIN {1}.{2} TABLA_RELACION ON TO_CHAR (TABLA_RELACION.{3}) = FD.VALOR " +
                                                  "WHERE FC.ID_FILE_DESCRIPTOR = {4} GROUP BY INDICE_COLUMNA) {5}{0} ON IMPORTED_FILE.INDICE_COLUMNA = {5}{0}.{6}{0}",
                                                  queryTables.Count, componente.Esquema, componente.Tabla, atributo.Campo, fileId, "TABLA_RELACION_", "INDICE_COLUMNA_C"));

                }
                //recupero el resultado de la query
                using (IDbCommand objComm = db.Database.Connection.CreateCommand())
                {
                    objComm.CommandText = string.Format("SELECT {0} FROM {1}", string.Join(",", queryFields), string.Join(" LEFT JOIN ", queryTables));
                    db.Database.Connection.Open();
                    using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        List<object[]> lista = new List<object[]>();
                        while (data.Read())
                        {
                            Decimal mayor = 0;
                            string campoCoincidencia = "TODOS";
                            //recorro los campos "cantidad" para recuperar el mayor 
                            //empiezo en el 3 porque es el primer campo de cantidad en caso de que tenga
                            for (int a = 3; a < queryFields.Count; a = a + 2)
                            {
                                if (mayor < data.GetInt64(a))
                                {
                                    mayor = data.GetInt64(a);
                                    campoCoincidencia = data.GetString(a - 1);
                                }
                            }
                            lista.Add(new object[] { data.GetInt64(0), Decimal.Round(Decimal.Divide(mayor, data.GetDecimal(1)) * 100, 0), campoCoincidencia });
                        }
                        return lista;
                    }
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - CalcularCoincidencia", ex);
                return null;
            }
        }

        [HttpGet]
        [ResponseType(typeof(MapaTematicoConfiguracion))]
        public IHttpActionResult GetMapaTematicoByName(string nombre)
        {
            MapaTematicoConfiguracion MapaTematicoConfiguracion = db.MapaTematicoConfiguracion.Where(a => a.Nombre.ToLower() == nombre.ToLower()).Where(a => a.FechaBaja == null).FirstOrDefault();
            return Ok(MapaTematicoConfiguracion);
        }
        [HttpGet]
        [ResponseType(typeof(MapaTematicoConfiguracion))]
        public IHttpActionResult GetMapaTematicoById(long id)
        {
            MapaTematicoConfiguracion MapaTematicoConfiguracion = db.MapaTematicoConfiguracion.Where(a => a.ConfiguracionId == id).FirstOrDefault();

            //db.Entry(MapaTematicoConfiguracion).Reference(x => x.ComponenteConfiguracion).Load();          

            if (MapaTematicoConfiguracion == null)
            {
                return NotFound();
            }
            return Ok(MapaTematicoConfiguracion);
        }

        [HttpGet]
        [ResponseType(typeof(ComponenteConfiguracion))]
        public IHttpActionResult GetComponenteConfiguracionByMTId(long id)
        {
            return Ok(db.ComponenteConfiguracion
                        .Where(h => h.Configuracion.ConfiguracionId == id)
                        .Include(x => x.Configuracion)
                        .Include(x => x.Componente)
                        .Single());
        }
        [HttpGet]
        [ResponseType(typeof(List<Componente>))]
        public IHttpActionResult GetComponentesByBiblioteca(long id)
        {
            var componentes = (from c in db.Componente
                               join cc in db.ComponenteConfiguracion on c.ComponenteId equals cc.ComponenteId
                               where cc.ConfiguracionId == id
                               select c).ToList();

            if (componentes == null)
            {
                return NotFound();
            }
            return Ok(componentes);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<ComponenteConfiguracion>))]
        public IHttpActionResult GetMapaTematicosPublicos()
        {
            try
            {
                var bibliotecas = (from comp in db.Componente
                                   join compConf in db.ComponenteConfiguracion on comp.ComponenteId equals compConf.ComponenteId
                                   join conf in db.MapaTematicoConfiguracion on compConf.ConfiguracionId equals conf.ConfiguracionId
                                   where conf.IdConfigCategoria != 4 && conf.Visibilidad == 1 && conf.FechaBaja == null
                                   orderby comp.Nombre
                                   select compConf)
                                  .Include(x => x.Componente)
                                  .Include(x => x.Configuracion);
                return Ok(bibliotecas.ToList());
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - GetMapaTematicosPublicos", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<ComponenteConfiguracion>))]
        public IHttpActionResult GetBibliotecasPrivadasByUsuario(long id)
        {
            var bibliotecas = (from comp in db.Componente
                               join compConf in db.ComponenteConfiguracion on comp.ComponenteId equals compConf.ComponenteId
                               join conf in db.MapaTematicoConfiguracion on compConf.ConfiguracionId equals conf.ConfiguracionId
                               where conf.IdConfigCategoria != 4 && conf.Visibilidad != 1 && conf.Usuario == id && conf.FechaBaja == null
                               orderby comp.Nombre
                               select compConf)
                              .Include(c => c.Configuracion)
                              .Include(c => c.Componente);

            return Ok(bibliotecas.ToList());
        }

        [HttpGet]
        [ResponseType(typeof(List<ConfiguracionRango>))]
        public IHttpActionResult ConfiguracionesRangoByMT(long id)
        {
            return Ok(db.ConfiguracionRango.Where(c => c.ConfiguracionId == id).ToList());
        }
        [HttpGet]
        [ResponseType(typeof(List<ConfiguracionFiltro>))]
        public IHttpActionResult ConfiguracionFiltroByMT(long id)
        {
            var query = db.ConfiguracionFiltro
                          .Where(c => c.ConfiguracionId == id)
                          .Include(cf => cf.ConfiguracionesFiltroGrafico);

            return Ok(query.ToList());
        }
        [HttpGet]
        [ResponseType(typeof(List<ColeccionVista>))]
        public IHttpActionResult GetColeccionesCombo()
        {
            var colecciones = (from coleccion in db.Coleccion
                               join componente in db.ColeccionComponente on coleccion.ColeccionId equals componente.ColeccionId
                               group componente by coleccion into grp
                               select new { id = grp.Key.ColeccionId, nombre = grp.Key.Nombre, cantidad = grp.Count() })
                              .ToList()
                              .Select(reg => new ColeccionVista()
                              {
                                  ColeccionId = reg.id,
                                  Nombre = reg.nombre,
                                  Cantidad = reg.cantidad
                              });
            return Ok(colecciones.ToList());
        }
        public IHttpActionResult GetColeccionesByComponenteUsuario(long componente, long usuario)
        {
            var colecciones = (from c in db.Coleccion
                               join cc in db.ColeccionComponente on c.ColeccionId equals cc.ColeccionId
                               where cc.ComponenteId == componente && c.UsuarioAlta == usuario && c.FechaBaja == null
                               group cc by c into grp
                               select new { id = grp.Key.ColeccionId, nombre = grp.Key.Nombre, cantidad = grp.Count() })
                              .ToList()
                              .Select(reg => new ColeccionVista()
                              {
                                  ColeccionId = reg.id,
                                  Nombre = reg.nombre,
                                  Cantidad = reg.cantidad
                              });
            return Ok(colecciones.ToList());
        }
        public IHttpActionResult GetColeccionesById(long id)
        {
            var coleccion = db.Coleccion.Find(id);
            return coleccion == null ? (IHttpActionResult)NotFound() : Ok(coleccion);
        }
        // DELETE api/MapasTemativosService/5
        [HttpPost]
        public IHttpActionResult DeleteMapaTematicoById(long id)
        {
            var c = db.MapaTematicoConfiguracion.Find(id);
            if (c == null)
            {
                return NotFound();
            }
            c.FechaBaja = DateTime.Now;
            db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult DeleteObjetoResultado(FormDataCollection form)
        {
            string idUsuario = form["idUsuario"];
            string token = form["token"];
            try
            {
                using (var builder = db.CreateSQLQueryBuilder())
                {
                    builder.AddTable("mt_objeto_resultado", "")
                           .AddFilter("id_usuario", idUsuario, SQLOperators.EqualsTo)
                           .AddFilter("token_sesion", $"'{token}'", SQLOperators.EqualsTo, SQLConnectors.And);
                    return Ok(builder.ExecuteDelete(true));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"DeleteObjetoResultado({idUsuario},{token})", ex);
                return InternalServerError(ex);
            }
        }

        // DELETE api/MapasTemativosService/5
        [HttpPost]
        public IHttpActionResult CambiarVisibilidadMapaTematicoById(long id)
        {
            var config = db.MapaTematicoConfiguracion.Find(id);
            config.Visibilidad ^= 1;
            db.SaveChanges();
            return Ok(config.Visibilidad);
        }

        [HttpPost]
        public IHttpActionResult GrabaMapaTematico(MapaTematicoConfiguracion mtc)
        {
            try
            {
                mtc.ComponenteConfiguracion[0].Componente = db.Componente.Find(mtc.ComponenteConfiguracion[0].ComponenteId);
                mtc.ComponenteConfiguracion[0].Configuracion = mtc;

                mtc.Nombre = mtc.Nombre ?? $"Sin Título {DateTime.Now}";

                if (mtc.ConfiguracionId == 0)
                {
                    mtc.FechaCreacion = DateTime.Now;
                    db.Entry(mtc).State = EntityState.Added;
                }
                else
                {
                    long configId = mtc.ConfiguracionId;

                    var config = (from componente in db.ComponenteConfiguracion
                                  join configuracion in db.MapaTematicoConfiguracion on componente.ConfiguracionId equals configuracion.ConfiguracionId
                                  join rango in db.ConfiguracionRango on componente.ConfiguracionId equals rango.ConfiguracionId
                                  join filtro in db.ConfiguracionFiltro on componente.ConfiguracionId equals filtro.ConfiguracionId
                                  join graf in db.ConfiguracionFiltroGrafico on filtro.FiltroId equals graf.FiltroId into leftjoin
                                  from filtroGrafico in leftjoin.DefaultIfEmpty()
                                  where componente.ConfiguracionId == configId
                                  group new { rango, filtro, filtroGrafico } by new { componente, configuracion } into grp
                                  select new
                                  {
                                      grp.Key.componente,
                                      grp.Key.configuracion,
                                      rangos = grp.Select(e => e.rango),
                                      filtros = grp.Select(e => e.filtro),
                                      filtrosGraficos = grp.Where(e => e.filtroGrafico != null).Select(e => e.filtroGrafico)
                                  }).First();

                    foreach (var rango in mtc.ConfiguracionRango)
                    {
                        rango.ConfiguracionId = mtc.ConfiguracionId;
                    }
                    foreach (var filtro in mtc.ConfiguracionFiltro)
                    {
                        filtro.ConfiguracionId = mtc.ConfiguracionId;
                    }

                    db.ConfiguracionRango.RemoveRange(config.rangos.Distinct());
                    db.ConfiguracionFiltro.RemoveRange(config.filtros.Distinct());
                    db.ConfiguracionFiltroGrafico.RemoveRange(config.filtrosGraficos.Distinct());

                    db.Entry(config.componente.Configuracion).CurrentValues.SetValues(mtc);
                    config.componente.Configuracion.ConfiguracionRango = mtc.ConfiguracionRango;
                    config.componente.Configuracion.ConfiguracionFiltro = mtc.ConfiguracionFiltro;
                }
                db.SaveChanges();
                return Ok(mtc.ConfiguracionId);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosService - GrabaMapaTematico", ex);
                return InternalServerError();
            }
        }
        [HttpPost]
        [ResponseType(typeof(MapaTematicoConfiguracion))]
        public IHttpActionResult CopyMapaTematicoById(MapaTematicoConfiguracion mtc)
        {
            long configuracionId = mtc.ConfiguracionId;
            mtc.FechaCreacion = System.DateTime.Now;
            mtc.Visibilidad = 0;
            mtc.ConfiguracionId = 0;
            mtc.FechaBaja = null;

            var componenteId = mtc.ComponenteConfiguracion[0].ComponenteId;
            mtc.ComponenteConfiguracion[0].Componente = db.Componente.Where(comp => comp.ComponenteId == componenteId).FirstOrDefault();
            mtc.ComponenteConfiguracion[0].Configuracion = mtc;
            mtc.ComponenteConfiguracion[0].ConfiguracionId = 0;

            List<ConfiguracionRango> lstConfiguracionRango = db.ConfiguracionRango.Where(c => c.ConfiguracionId == configuracionId).ToList();
            if (lstConfiguracionRango.Count > 0)
            {
                List<ConfiguracionRango> lstConfiguracionRangoNew = new List<ConfiguracionRango>();
                foreach (var configuracionRango in lstConfiguracionRango)
                {
                    ConfiguracionRango cr = new ConfiguracionRango();
                    cr.ConfiguracionId = mtc.ConfiguracionId;
                    cr.Orden = configuracionRango.Orden;
                    cr.Desde = configuracionRango.Desde;
                    cr.Hasta = configuracionRango.Hasta;
                    cr.Cantidad = configuracionRango.Cantidad;
                    cr.Leyenda = configuracionRango.Leyenda;
                    cr.ColorRelleno = configuracionRango.ColorRelleno;
                    cr.ColorLinea = configuracionRango.ColorLinea;
                    cr.EspesorLinea = configuracionRango.EspesorLinea;
                    //TODO RA - CopyMapaTematicoById - cr.Icono asignarlo cuando funcione lo de los iconos de MG
                    //cr.Icono = configuracionRango.Icono;

                    lstConfiguracionRangoNew.Add(cr);
                }
                mtc.ComponenteConfiguracion[0].Configuracion.ConfiguracionRango = lstConfiguracionRangoNew;
            }
            List<ConfiguracionFiltro> lstConfiguracionFiltro = db.ConfiguracionFiltro.Where(c => c.ConfiguracionId == configuracionId).ToList();
            if (lstConfiguracionFiltro != null && lstConfiguracionFiltro.Count > 0)
            {
                List<ConfiguracionFiltro> lstConfiguracionFiltroNew = new List<ConfiguracionFiltro>();
                foreach (var configuracionFiltro in lstConfiguracionFiltro)
                {
                    ConfiguracionFiltro cf = new ConfiguracionFiltro();
                    cf.FiltroId = 0;
                    cf.ConfiguracionId = mtc.ConfiguracionId;
                    cf.FiltroTipo = configuracionFiltro.FiltroTipo;
                    cf.FiltroComponente = configuracionFiltro.FiltroComponente;
                    cf.FiltroAtributo = configuracionFiltro.FiltroAtributo;
                    cf.FiltroOperacion = configuracionFiltro.FiltroOperacion;
                    cf.FiltroColeccion = configuracionFiltro.FiltroColeccion;
                    cf.Valor1 = configuracionFiltro.Valor1;
                    cf.Valor2 = configuracionFiltro.Valor2;
                    cf.Ampliar = configuracionFiltro.Ampliar;
                    cf.Dentro = configuracionFiltro.Dentro;
                    cf.Tocando = configuracionFiltro.Tocando;
                    cf.Fuera = configuracionFiltro.Fuera;
                    cf.Habilitado = configuracionFiltro.Habilitado;
                    if (configuracionFiltro.FiltroTipo == 2)
                    {
                        cf.ConfiguracionesFiltroGrafico = new List<ConfiguracionFiltroGrafico>();
                        List<ConfiguracionFiltroGrafico> lstConfiguracionFiltroGrafico = db.ConfiguracionFiltroGrafico.Where(g => g.FiltroId == configuracionFiltro.FiltroId).ToList();
                        List<ConfiguracionFiltroGrafico> lstConfiguracionFiltroGraficoNew = new List<ConfiguracionFiltroGrafico>();
                        foreach (var configuracionFiltroGrafico in lstConfiguracionFiltroGrafico)
                        {
                            ConfiguracionFiltroGrafico cfg = new ConfiguracionFiltroGrafico();
                            cfg.ConfiguracionFiltroGraficoId = 0;
                            cfg.FiltroId = cf.FiltroId;
                            cfg.Coordenadas = configuracionFiltroGrafico.Coordenadas;

                            cfg.sGeometry = configuracionFiltroGrafico.sGeometry;

                            cf.ConfiguracionesFiltroGrafico.Add(cfg);
                        }
                    }
                    lstConfiguracionFiltroNew.Add(cf);
                }
                mtc.ComponenteConfiguracion[0].Configuracion.ConfiguracionFiltro = lstConfiguracionFiltroNew;
            }

            db.Entry(mtc).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();
            return Ok(mtc);
        }
        private bool comparaTipos(string str1, string str2)
        {
            bool boolValue;
            Int32 intValue;
            Int64 bigintValue;
            double doubleValue;
            DateTime dateValue;

            if (bool.TryParse(str1, out boolValue) && bool.TryParse(str2, out boolValue))
                return true;
            else if (Int32.TryParse(str1, out intValue) && Int32.TryParse(str2, out intValue))
                return true;
            else if (Int64.TryParse(str1, out bigintValue) && Int64.TryParse(str2, out bigintValue))
                return true;
            else if (double.TryParse(str1, out doubleValue) && double.TryParse(str2, out doubleValue))
                return true;
            else if (DateTime.TryParse(str1, out dateValue) && DateTime.TryParse(str2, out dateValue))
                return true;
            else return false;
        }
        private int ParseDataType(string str)
        {
            bool boolValue;
            Int32 intValue;
            Int64 bigintValue;
            double doubleValue;
            DateTime dateValue;

            if (bool.TryParse(str, out boolValue))
                return 1;
            else if (Int32.TryParse(str, out intValue))
                return 2;
            else if (Int64.TryParse(str, out bigintValue))
                return 3;
            else if (double.TryParse(str, out doubleValue))
                return 4;
            else if (DateTime.TryParse(str, out dateValue))
                return 5;
            else return 6;

        }

        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult GenerarResultadoMapaTematicoUbicaciones(long userId, string guid, ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            return StatusCode(System.Net.HttpStatusCode.Gone);
            //try
            //{
            //    MapaTematicoConfiguracion mapaTematico = new MapaTematicoConfiguracion();
            //    mapaTematico.Agrupacion = 0;
            //    mapaTematico.Atributo = 0;
            //    string[] ids = objetoResultadoDetalle.Rangos.Select(r => r.Valor).ToArray();
            //    StringBuilder insertResultado = new StringBuilder();
            //    insertResultado.AppendLine("INSERT INTO MT_OBJETO_RESULTADO(id_usuario, guid, id_origin, descripcion, valor, rango, fecha_alta, geometry) ");
            //    insertResultado.AppendLine(string.Format("SELECT {0}, '{1}', ID_UBICACION, DESCRIPCION, TIPO, null, SYSDATE, SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY (2001, 8307, SDO_POINT_TYPE(POS_X, POS_Y, 0), NULL, NULL),{2})", userId, guid, SRID));
            //    insertResultado.AppendLine(string.Format("FROM CT_UBICACION WHERE ID_UBICACION in ({0}) ", string.Join(",", ids)));
            //    db.Database.Connection.Open();
            //    using (IDbCommand dbCommand = db.Database.Connection.CreateCommand())
            //    {
            //        //Ejecucion por procedure
            //        dbCommand.CommandType = CommandType.StoredProcedure;
            //        dbCommand.CommandText = "GenerarObjetoResultado";
            //        IDbDataParameter parameter = dbCommand.CreateParameter();
            //        parameter.ParameterName = "pSQL";
            //        parameter.Value = insertResultado.ToString();
            //        dbCommand.Parameters.Add(parameter);

            //        dbCommand.CommandTimeout = 300;
            //        int rows = dbCommand.ExecuteNonQuery();
            //        //Fin ejecucion por procedure
            //        objetoResultadoDetalle = GetRangos(guid, 3, objetoResultadoDetalle.Rangos.Select(r => r.Leyenda).Distinct().Count());
            //        return Ok(objetoResultadoDetalle);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    db.Database.Connection.Close();
            //}
        }

        [HttpPost]
        [ResponseType(typeof(ObjetoResultadoDetalle))]
        public IHttpActionResult GenerarResultadoMapaTematico(RequestObjetoResultado requestObjetoResultado)
        {
            try
            {
                var resultado = GenerarObjetoResultado(requestObjetoResultado, out bool generado);
                if (generado)
                {
                    return Ok(resultado);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("GenerarResultadoMapaTematico", ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        [ResponseType(typeof(ObjetoResultadoDetalle))]
        public IHttpActionResult GetObjetoResultadoDetalle(string guid, long distribucion, long cantRangos)
        {
            var objetoResultadoDetalle = GetRangos(db.ObjetoResultados.Where(p => p.GUID == guid).ToList(), guid, distribucion, cantRangos);
            if (objetoResultadoDetalle == null)
            {
                return NotFound();
            }
            return Ok(objetoResultadoDetalle);
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult ActualizarResultadoMapaTematico(ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            try
            {
                int total = 0;
                int nroRango = 0;
                foreach (var rango in objetoResultadoDetalle.Rangos)
                {
                    using (var builder = db.CreateSQLQueryBuilder())
                    {
                        builder.AddTable("mt_objeto_resultado", null)
                               .AddFieldsToUpdate(new KeyValuePair<Atributo, object>(new Atributo { Campo = "rango" }, ++nroRango));
                        if (objetoResultadoDetalle.Distribucion == 1 || objetoResultadoDetalle.Distribucion == 2)
                        {
                            var attr = new Atributo { Campo = "valor_num" };
                            builder.AddFilter(attr, rango.Desde, SQLOperators.GreaterThan | SQLOperators.EqualsTo)
                                   .AddFilter(attr, rango.Hasta, SQLOperators.LowerThan | SQLOperators.EqualsTo, SQLConnectors.And);
                        }
                        else if (objetoResultadoDetalle.Distribucion == 3)
                        {
                            builder.AddFilter(new Atributo { Campo = "valor", TipoDatoId = 6 }, rango.Valor, SQLOperators.EqualsTo);
                        }
                        builder.AddFilter(new Atributo() { Campo = "guid", TipoDatoId = 6 }, objetoResultadoDetalle.GUID, SQLOperators.EqualsTo, SQLConnectors.And);

                        total += builder.ExecuteUpdate();
                    }
                }
                return Ok(total);
                //db.Database.Connection.Open();

                //int? nroRango = null;
                //for (int i = 0; i < objetoResultadoDetalle.Rangos.Count; i++)
                //{
                //    Rango rango = objetoResultadoDetalle.Rangos[i];
                //    nroRango = i + 1;

                //    IDbCommand dbCommand = db.Database.Connection.CreateCommand();
                //    dbCommand.CommandType = CommandType.StoredProcedure;
                //    dbCommand.CommandText = "ActualizarObjetoResultado";

                //    IDbDataParameter parameter = dbCommand.CreateParameter();

                //    parameter.ParameterName = "pGUID";
                //    parameter.Value = objetoResultadoDetalle.GUID;
                //    dbCommand.Parameters.Add(parameter);

                //    IDbDataParameter parameter1 = dbCommand.CreateParameter();
                //    parameter1.ParameterName = "pNroRango";
                //    parameter1.Value = nroRango;
                //    dbCommand.Parameters.Add(parameter1);

                //    IDbDataParameter parameter2 = dbCommand.CreateParameter();
                //    parameter2.ParameterName = "pDistribucion";
                //    parameter2.Value = objetoResultadoDetalle.Distribucion;
                //    dbCommand.Parameters.Add(parameter2);

                //    IDbDataParameter parameter3 = dbCommand.CreateParameter();
                //    parameter3.ParameterName = "pRangoDesde";
                //    if (rango.Desde != null)
                //    {
                //        parameter3.Value = rango.Desde;
                //    }
                //    else
                //    {
                //        parameter3.Value = 0;
                //    }
                //    dbCommand.Parameters.Add(parameter3);

                //    IDbDataParameter parameter4 = dbCommand.CreateParameter();
                //    parameter4.ParameterName = "pRangoHasta";
                //    if (rango.Hasta != null)
                //    {
                //        parameter4.Value = rango.Hasta;
                //    }
                //    else
                //    {
                //        parameter4.Value = 0;
                //    }
                //    dbCommand.Parameters.Add(parameter4);

                //    IDbDataParameter parameter5 = dbCommand.CreateParameter();
                //    parameter5.ParameterName = "pRangoValor";
                //    if (rango.Valor != null)
                //    {
                //        parameter5.Value = rango.Valor;
                //    }
                //    else
                //    {
                //        parameter5.Value = DBNull.Value;
                //    }
                //    dbCommand.Parameters.Add(parameter5);

                //    int rows = dbCommand.ExecuteNonQuery();

                //    cantUpd += rows;
                //}
                //db.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - ActualizarResultadoMapaTematico", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult ActualizarResultadoMapaTematicoPasarAPunto(string guid)
        {
            int cantUpd = 0;
            try
            {
                db.Database.Connection.Open();

                IDbCommand dbCommand = db.Database.Connection.CreateCommand();
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = "ActualizarObjetoResultadoPto";

                IDbDataParameter parameter = dbCommand.CreateParameter();

                parameter.ParameterName = "pGUID";
                parameter.Value = guid;
                dbCommand.Parameters.Add(parameter);

                int rows = dbCommand.ExecuteNonQuery();

                cantUpd += rows;
                db.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - ActualizarResultadoMapaTematicoPasarAPunto", ex);
            }
            return Ok(cantUpd);
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public IHttpActionResult ExportarExcelResultadoMapaTematico(string idPredefinido, ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            int cantObj = 0;
            byte[] bExcel = null;
            try
            {
                List<ObjetoResultado> lstObjetoResultado = db.ObjetoResultados.Where(p => p.GUID == objetoResultadoDetalle.GUID).ToList();
                if (lstObjetoResultado != null && lstObjetoResultado.Count > 0)
                {
                    using (MemoryStream memStreamTemp = new MemoryStream())
                    {
                        using (ExcelPackage package = new ExcelPackage(memStreamTemp))
                        {
                            // add a new worksheet to the empty workbook
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MapaTematico");
                            //Add the headers
                            int predefinidoId = 0;
                            try
                            {
                                predefinidoId = Convert.ToInt32(idPredefinido);
                            }
                            catch { }

                            string id_valor = GetExcelTitulos(worksheet, predefinidoId);
                            string[] aIdValor = id_valor.Split(',');
                            worksheet.Cells[1, 1].Value = aIdValor[0];
                            worksheet.Cells[1, 2].Value = aIdValor[1];

                            worksheet.Cells[1, 3].Value = "COLOR";
                            int iRow = 2;
                            foreach (var objetoResultado in lstObjetoResultado)
                            {
                                if (objetoResultado.Valor != null && !DBNull.Value.Equals(objetoResultado.Valor))
                                {
                                    if (predefinidoId == 0 && objetoResultadoDetalle.ComponenteId != 0)
                                    {
                                        worksheet.Cells[iRow, 1].Value = GetLabelValue(objetoResultadoDetalle.ComponenteId, objetoResultado.IdOrigin);
                                    }
                                    else
                                    {
                                        worksheet.Cells[iRow, 1].Value = objetoResultado.Descripcion;
                                    }
                                    worksheet.Cells[iRow, 2].Value = objetoResultado.Valor;

                                    Rango rango = GetRango((objetoResultado.Valor != null && !DBNull.Value.Equals(objetoResultado.Valor) ? objetoResultado.Valor.Replace(',', '.') : "0"), objetoResultadoDetalle.Rangos, objetoResultadoDetalle.Distribucion);

                                    worksheet.Cells[iRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    Color colorRango = System.Drawing.ColorTranslator.FromHtml("#" + rango.Color);
                                    worksheet.Cells[iRow, 3].Style.Fill.BackgroundColor.SetColor(colorRango);
                                    Color colorBorde = System.Drawing.ColorTranslator.FromHtml("#" + rango.ColorBorde);
                                    ExcelBorderStyle borderStyle = ExcelBorderStyle.None;
                                    switch (rango.AnchoBorde)
                                    {
                                        case 0:
                                            borderStyle = ExcelBorderStyle.None;
                                            break;
                                        case 1:
                                            borderStyle = ExcelBorderStyle.Thin;
                                            break;
                                        case 2:
                                        case 3:
                                            borderStyle = ExcelBorderStyle.Medium;
                                            break;
                                        case 4:
                                        case 5:
                                            borderStyle = ExcelBorderStyle.Thick;
                                            break;
                                        default:
                                            borderStyle = ExcelBorderStyle.None;
                                            break;
                                    }

                                    worksheet.Cells[iRow, 3].Style.Border.Top.Style = borderStyle;
                                    worksheet.Cells[iRow, 3].Style.Border.Bottom.Style = borderStyle;
                                    worksheet.Cells[iRow, 3].Style.Border.Left.Style = borderStyle;
                                    worksheet.Cells[iRow, 3].Style.Border.Right.Style = borderStyle;
                                    if (rango.AnchoBorde > 0)
                                    {
                                        worksheet.Cells[iRow, 3].Style.Border.Top.Color.SetColor(colorBorde);
                                        worksheet.Cells[iRow, 3].Style.Border.Bottom.Color.SetColor(colorBorde);
                                        worksheet.Cells[iRow, 3].Style.Border.Left.Color.SetColor(colorBorde);
                                        worksheet.Cells[iRow, 3].Style.Border.Right.Color.SetColor(colorBorde);

                                        worksheet.Cells[iRow, 3].Style.Border.BorderAround(borderStyle, colorBorde);
                                    }
                                    cantObj++;
                                    iRow++;
                                }
                            }
                            //Ok now format the values;
                            using (var range = worksheet.Cells[1, 1, 1, 3])
                            {
                                range.Style.Font.Bold = true;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                                range.Style.Font.Color.SetColor(Color.Black);
                            }

                            worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                            // lets set the header text 
                            worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" MapaTematico";
                            // add the page number to the footer plus the total number of pages
                            worksheet.HeaderFooter.OddFooter.RightAlignedText =
                                string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                            // add the sheet name to the footer
                            worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                            //// add the file path to the footer
                            //worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                            //worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                            //worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                            //// Change the sheet view to show it in page layout mode
                            //worksheet.View.PageLayoutView = true;

                            // set some document properties
                            package.Workbook.Properties.Title = "Mapa Tematico";
                            package.Workbook.Properties.Author = "Geosystems S.A.";
                            package.Workbook.Properties.Comments = "Resultado Mapa Tematico";

                            // set some extended property values
                            package.Workbook.Properties.Company = "Geosystems S.A.";

                            // set some custom property values
                            package.Workbook.Properties.SetCustomPropertyValue("Checked by", "");
                            package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "GeoSIT");
                            // save our new workbook and we are done!
                            bExcel = package.GetAsByteArray();
                        }
                    }
                }
                return Ok(Convert.ToBase64String(bExcel));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - ExportarExcelResultadoMapaTematico", ex);
                return InternalServerError(ex);
            }
        }

        private string GetLabelValue(long componentId, string elementId)
        {
            Componente componente = db.Componente.Include(a => a.Atributos).Single(c => c.ComponenteId == componentId);
            Atributo atributoLabel = componente.Atributos.GetAtributoLabel();
            Atributo atributoClave = componente.Atributos.GetAtributoClave();

            string query = string.Format("SELECT {0} FROM {1}.{2} WHERE {3} = {4}",
                atributoLabel.Campo, componente.Esquema, componente.Tabla, atributoClave.Campo, atributoClave.GetFormattedValue(elementId));

            //recupero el resultado de la query
            using (IDbCommand objComm = db.Database.Connection.CreateCommand())
            {
                db.Database.Connection.Open();
                objComm.CommandText = query;
                using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    data.Read();
                    return GetDataValue(data, 0, atributoLabel.TipoDatoId);
                }
            }
        }
        private static string GetExcelTitulos(ExcelWorksheet worksheet, int predefinidoId)
        {
            string id_valor = "FEATID" + "," + "VALOR";
            switch (predefinidoId)
            {
                case 0:
                    //worksheet.Cells[1, 1].Value = "FEATID";
                    //worksheet.Cells[1, 2].Value = "VALOR";
                    id_valor = "ID,VALOR";
                    break;
                case 1:
                    //Ranking por Antigüedad de Deuda
                    id_valor = "ID_EXPEDIENTE,ANTIGUEDAD";
                    break;
                case 2:
                    //Ranking Facturas Impagas
                    id_valor = "ID_EXPEDIENTE,CANTIDAD";
                    break;
                case 3:
                    //Ranking Por Total de Deuda
                    id_valor = "ID_EXPEDIENTE,DEUDA";
                    break;
                case 4:
                    //Todos los medidores
                    id_valor = "EQUIPO,EXPEDIENTE";
                    break;
                case 5:
                    //Medidores diferenciados por marca
                    id_valor = "ID_EQUIPO,EXPEDIENTE,MARCA";
                    break;
                case 6:
                    //Tipos de Corte
                    id_valor = "ID_EXPEDIENTE,TIPO_CORTE";
                    break;
                case 7:
                    //Corte con fecha anterior
                    id_valor = "ID_EXPEDIENTE,FECHA";
                    break;
                case 8:
                    //Consumos Promedios
                    id_valor = "ID_EXPEDIENTE,CONSUMO_PROMEDIO";
                    break;
                case 9:
                    //Ranking de cuadras
                    id_valor = "ID_CUADRA,CANTIDAD_RECLAMOS";
                    break;
                case 10:
                    //Intervenciones por Tipo y Subcuenca
                    id_valor = "ID_RECLAMO,TIPO";
                    break;
                case 12:
                    //Zona e Intervenciones
                    id_valor = "ID_RECLAMO,GRUPO";
                    break;
                case 13:
                    //Intervenciones Por Tipo y Malla
                    id_valor = "ID_RECLAMO,TIPO";
                    break;
                case 14:
                    //Ratio Mallas e Intervenciones
                    id_valor = "ID_MALLA,RATIO";
                    break;
                case 15:
                    //Ranking de mallas
                    id_valor = "ID_MALLA,CANTIDAD_RECLAMOS_FA_FP";
                    break;
                case 16:
                    //Ranking Subcuenca
                    id_valor = "ID_SUBCUENCA,INDICE_SUCIEDAD";
                    break;
                case 17:
                    //Malla e Intervenciones
                    id_valor = "ID_MALLA,RECLAMOS_KM_RED";
                    break;
                case 18:
                    //Ranking de Malla
                    id_valor = "ID_MALLA,CANTIDAD_RECLAMOS_FA_FP";
                    break;
                case 19:
                    //Ranking Cuadras
                    id_valor = "ID_RECLAMO,GRUPO";
                    break;
                default:
                    id_valor = "ID,VALOR";
                    break;
            }
            return id_valor;
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public IHttpActionResult ExportarExcelResultadoMTPredefinido_4(string idPredefinido, ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            int cantObj = 0;
            byte[] bExcel = null;
            try
            {
                using (MemoryStream memStreamTemp = new MemoryStream())
                {
                    using (ExcelPackage package = new ExcelPackage(memStreamTemp))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MapaTematico");
                        //Add the headers
                        int predefinidoId = 0;
                        try
                        {
                            predefinidoId = Convert.ToInt32(idPredefinido);
                        }
                        catch { }

                        worksheet.Cells[1, 1].Value = "EQUIPO";
                        worksheet.Cells[1, 2].Value = "EXPEDIENTE";

                        int iRow = 2;
                        //AYSA20180424 VERIFICAR cambia ct_parcela por vw_ct_parcela
                        string sSql = "SELECT m.EQUIPO,p.APIC_ID EXPEDIENTE " +
                                       " FROM MT_OBJETO_RESULTADO o " +
                                       " JOIN VW_CT_PARCELA p ON o.ID_ORIGIN=TO_CHAR(p.ID_PARCELA) " +
                                       " JOIN SAP.VW_MEDIDORES m ON p.APIC_ID=m.EXP_REF_CATASTRO " +
                                       " WHERE o.GUID='" + objetoResultadoDetalle.GUID + "'";
                        IDbCommand objComm = db.Database.Connection.CreateCommand();
                        db.Database.Connection.Open();
                        objComm.CommandText = sSql;
                        IDataReader data = objComm.ExecuteReader();
                        while (data.Read())
                        {
                            worksheet.Cells[iRow, 1].Value = data.GetValue(0).ToString();
                            worksheet.Cells[iRow, 2].Value = data.GetValue(1).ToString();

                            cantObj++;
                            iRow++;
                        }
                        db.Database.Connection.Close();

                        //Ok now format the values;
                        using (var range = worksheet.Cells[1, 1, 1, 2])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                            range.Style.Font.Color.SetColor(Color.Black);
                        }

                        worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                        // lets set the header text 
                        worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" MapaTematico";
                        // add the page number to the footer plus the total number of pages
                        worksheet.HeaderFooter.OddFooter.RightAlignedText =
                            string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                        // add the sheet name to the footer
                        worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                        // set some document properties
                        package.Workbook.Properties.Title = "Mapa Tematico";
                        package.Workbook.Properties.Author = "Geosystems S.A.";
                        package.Workbook.Properties.Comments = "Resultado Mapa Tematico";

                        // set some extended property values
                        package.Workbook.Properties.Company = "Geosystems S.A.";

                        // set some custom property values
                        package.Workbook.Properties.SetCustomPropertyValue("Checked by", "");
                        package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "GeoSIT");
                        // save our new workbook and we are done!
                        bExcel = package.GetAsByteArray();

                    }
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - ActualizarResultadoMTPredefinido_4", ex);
            }
            return Ok(Convert.ToBase64String(bExcel));
        }

        public IHttpActionResult ExportarExcelResultadoMTPredefinido_5(string idPredefinido, ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            int cantObj = 0;
            byte[] bExcel = null;
            try
            {
                using (MemoryStream memStreamTemp = new MemoryStream())
                {
                    using (ExcelPackage package = new ExcelPackage(memStreamTemp))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MapaTematico");
                        //Add the headers
                        int predefinidoId = 0;
                        try
                        {
                            predefinidoId = Convert.ToInt32(idPredefinido);
                        }
                        catch { }

                        worksheet.Cells[1, 1].Value = "EQUIPO";
                        worksheet.Cells[1, 2].Value = "EXPEDIENTE";
                        worksheet.Cells[1, 3].Value = "MARCA";
                        worksheet.Cells[1, 4].Value = "COLOR";
                        int iColColor = 4;
                        int iRow = 2;
                        //AYSA20180424 VERIFICAR cambia ct_parcela por vw_ct_parcela
                        string sSql = "SELECT m.EQUIPO,p.APIC_ID EXPEDIENTE, o.VALOR MARCA " +
                                       " FROM MT_OBJETO_RESULTADO o " +
                                       " JOIN VW_CT_PARCELA p ON o.ID_ORIGIN=TO_CHAR(p.ID_PARCELA) " +
                                       " JOIN SAP.VW_MEDIDORES m ON p.APIC_ID=m.EXP_REF_CATASTRO " +
                                       " WHERE o.GUID='" + objetoResultadoDetalle.GUID + "'";
                        IDbCommand objComm = db.Database.Connection.CreateCommand();
                        db.Database.Connection.Open();
                        objComm.CommandText = sSql;
                        IDataReader data = objComm.ExecuteReader();
                        while (data.Read())
                        {
                            worksheet.Cells[iRow, 1].Value = data.GetValue(0).ToString();
                            worksheet.Cells[iRow, 2].Value = data.GetValue(1).ToString();
                            string valor = data.GetValue(2).ToString();
                            worksheet.Cells[iRow, 3].Value = valor;

                            Rango rango = GetRango((valor != null && !DBNull.Value.Equals(valor) ? valor.Replace(',', '.') : "0"), objetoResultadoDetalle.Rangos, objetoResultadoDetalle.Distribucion);

                            worksheet.Cells[iRow, iColColor].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            Color colorRango = System.Drawing.ColorTranslator.FromHtml("#" + rango.Color);
                            worksheet.Cells[iRow, iColColor].Style.Fill.BackgroundColor.SetColor(colorRango);
                            Color colorBorde = System.Drawing.ColorTranslator.FromHtml("#" + rango.ColorBorde);
                            ExcelBorderStyle borderStyle = ExcelBorderStyle.None;
                            switch (rango.AnchoBorde)
                            {
                                case 0:
                                    borderStyle = ExcelBorderStyle.None;
                                    break;
                                case 1:
                                    borderStyle = ExcelBorderStyle.Thin;
                                    break;
                                case 2:
                                case 3:
                                    borderStyle = ExcelBorderStyle.Medium;
                                    break;
                                case 4:
                                case 5:
                                    borderStyle = ExcelBorderStyle.Thick;
                                    break;
                                default:
                                    borderStyle = ExcelBorderStyle.None;
                                    break;
                            }

                            worksheet.Cells[iRow, iColColor].Style.Border.Top.Style = borderStyle;
                            worksheet.Cells[iRow, iColColor].Style.Border.Bottom.Style = borderStyle;
                            worksheet.Cells[iRow, iColColor].Style.Border.Left.Style = borderStyle;
                            worksheet.Cells[iRow, iColColor].Style.Border.Right.Style = borderStyle;
                            if (rango.AnchoBorde > 0)
                            {
                                worksheet.Cells[iRow, iColColor].Style.Border.Top.Color.SetColor(colorBorde);
                                worksheet.Cells[iRow, iColColor].Style.Border.Bottom.Color.SetColor(colorBorde);
                                worksheet.Cells[iRow, iColColor].Style.Border.Left.Color.SetColor(colorBorde);
                                worksheet.Cells[iRow, iColColor].Style.Border.Right.Color.SetColor(colorBorde);

                                worksheet.Cells[iRow, iColColor].Style.Border.BorderAround(borderStyle, colorBorde);
                            }

                            cantObj++;
                            iRow++;
                        }
                        db.Database.Connection.Close();

                        //Ok now format the values;
                        using (var range = worksheet.Cells[1, 1, 1, 4])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                            range.Style.Font.Color.SetColor(Color.Black);
                        }

                        //worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //worksheet.Cells["A5:E5"].Style.Font.Bold = true;

                        worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                        // lets set the header text 
                        worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" MapaTematico";
                        // add the page number to the footer plus the total number of pages
                        worksheet.HeaderFooter.OddFooter.RightAlignedText =
                            string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                        // add the sheet name to the footer
                        worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                        //// add the file path to the footer
                        //worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                        //worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                        //worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                        //// Change the sheet view to show it in page layout mode
                        //worksheet.View.PageLayoutView = true;

                        // set some document properties
                        package.Workbook.Properties.Title = "Mapa Tematico";
                        package.Workbook.Properties.Author = "Geosystems S.A.";
                        package.Workbook.Properties.Comments = "Resultado Mapa Tematico";

                        // set some extended property values
                        package.Workbook.Properties.Company = "Geosystems S.A.";

                        // set some custom property values
                        package.Workbook.Properties.SetCustomPropertyValue("Checked by", "");
                        package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "GeoSIT");
                        // save our new workbook and we are done!
                        bExcel = package.GetAsByteArray();

                        //package.Save();

                        //string xlsxFileNameOut = @"C:\Temp\MapaTematicoResultado.xlsx";
                        //using (var fileStreamOut = File.Create(xlsxFileNameOut))
                        //{
                        //    fileStreamOut.Write(bExcel, 0, (int)bExcel.Length);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - ActualizarResultadoMTPredefinido_5", ex);
            }
            return Ok(Convert.ToBase64String(bExcel));
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public IHttpActionResult ExportarExcelResultadoMTPredefinido_7(string idPredefinido, ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            int cantObj = 0;
            byte[] bExcel = null;
            try
            {
                using (MemoryStream memStreamTemp = new MemoryStream())
                {
                    using (ExcelPackage package = new ExcelPackage(memStreamTemp))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MapaTematico");
                        //Add the headers
                        int predefinidoId = 0;
                        try
                        {
                            predefinidoId = Convert.ToInt32(idPredefinido);
                        }
                        catch { }

                        worksheet.Cells[1, 1].Value = "EXPEDIENTE";
                        worksheet.Cells[1, 2].Value = "FECHA_CORTE_ANTERIOR";

                        int iRow = 2;
                        //AYSA20180424 VERIFICAR cambia ct_parcela por vw_ct_parcela
                        string sSql = "SELECT p.APIC_ID EXPEDIENTE, m.F_ULT_CORTE " +
                                       " FROM MT_OBJETO_RESULTADO o " +
                                       " JOIN VW_CT_PARCELA p ON o.ID_ORIGIN=TO_CHAR(p.ID_PARCELA) " +
                                       " JOIN SAP.VW_CORTES m ON p.APIC_ID=m.EXP_REF_CATASTRO " +
                                       " WHERE o.GUID='" + objetoResultadoDetalle.GUID + "'";
                        IDbCommand objComm = db.Database.Connection.CreateCommand();
                        db.Database.Connection.Open();
                        objComm.CommandText = sSql;
                        IDataReader data = objComm.ExecuteReader();
                        while (data.Read())
                        {
                            worksheet.Cells[iRow, 1].Value = data.GetValue(0).ToString();
                            worksheet.Cells[iRow, 2].Value = Convert.ToDateTime(data.GetValue(1).ToString()).ToShortDateString();

                            cantObj++;
                            iRow++;
                        }
                        db.Database.Connection.Close();

                        //Ok now format the values;
                        using (var range = worksheet.Cells[1, 1, 1, 2])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                            range.Style.Font.Color.SetColor(Color.Black);
                        }

                        //worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //worksheet.Cells["A5:E5"].Style.Font.Bold = true;

                        worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                        // lets set the header text 
                        worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" MapaTematico";
                        // add the page number to the footer plus the total number of pages
                        worksheet.HeaderFooter.OddFooter.RightAlignedText =
                            string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                        // add the sheet name to the footer
                        worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                        //// add the file path to the footer
                        //worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                        //worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                        //worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                        //// Change the sheet view to show it in page layout mode
                        //worksheet.View.PageLayoutView = true;

                        // set some document properties
                        package.Workbook.Properties.Title = "Mapa Tematico";
                        package.Workbook.Properties.Author = "Geosystems S.A.";
                        package.Workbook.Properties.Comments = "Resultado Mapa Tematico";

                        // set some extended property values
                        package.Workbook.Properties.Company = "Geosystems S.A.";

                        // set some custom property values
                        package.Workbook.Properties.SetCustomPropertyValue("Checked by", "");
                        package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "GeoSIT");
                        // save our new workbook and we are done!
                        bExcel = package.GetAsByteArray();

                        //package.Save();

                        //string xlsxFileNameOut = @"C:\Temp\MapaTematicoResultado.xlsx";
                        //using (var fileStreamOut = File.Create(xlsxFileNameOut))
                        //{
                        //    fileStreamOut.Write(bExcel, 0, (int)bExcel.Length);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - ActualizarResultadoMTPredefinido_7", ex);
            }
            return Ok(Convert.ToBase64String(bExcel));
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult GrabarColeccion(ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            int cantObj = 0;
            try
            {
                //List<ObjetoResultado> lstObjetoResultado = db.ObjetoResultados.Where(p => p.GUID == objetoResultadoDetalle.GUID).ToList();
                //if (lstObjetoResultado != null && lstObjetoResultado.Count > 0)
                //{
                db.Database.Connection.Open();

                DateTime fechaActual = System.DateTime.Now;
                int? nroRango = null;
                for (int i = 0; i < objetoResultadoDetalle.Rangos.Count; i++)
                {
                    Rango rango = objetoResultadoDetalle.Rangos[i];
                    nroRango = i + 1;

                    IDbCommand dbCommand = db.Database.Connection.CreateCommand();
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    dbCommand.CommandText = "GrabarColeccion";

                    IDbDataParameter parameter = dbCommand.CreateParameter();

                    parameter.ParameterName = "pGUID";
                    parameter.Value = objetoResultadoDetalle.GUID;
                    dbCommand.Parameters.Add(parameter);

                    IDbDataParameter parameter1 = dbCommand.CreateParameter();
                    parameter1.ParameterName = "pIdUsuario";
                    parameter1.Value = objetoResultadoDetalle.IdUsuario;
                    dbCommand.Parameters.Add(parameter1);

                    IDbDataParameter parameter2 = dbCommand.CreateParameter();
                    parameter2.ParameterName = "pNroRango";
                    parameter2.Value = nroRango;
                    dbCommand.Parameters.Add(parameter2);

                    IDbDataParameter parameter3 = dbCommand.CreateParameter();
                    string nombre = objetoResultadoDetalle.NombreMT + " " + rango.Leyenda;
                    parameter3.ParameterName = "pNombre";
                    parameter3.Value = nombre.Length > 50 ? nombre.Substring(0, 49) : nombre;
                    dbCommand.Parameters.Add(parameter3);

                    IDbDataParameter parameter4 = dbCommand.CreateParameter();
                    parameter4.ParameterName = "pComponenteId";
                    parameter4.Value = objetoResultadoDetalle.ComponenteId;
                    dbCommand.Parameters.Add(parameter4);

                    #region Codigo Comentado
                    //Coleccion coleccion = new Coleccion();
                    //coleccion.Nombre = objetoResultadoDetalle.NombreMT + " " + rango.Leyenda;
                    //coleccion.UsuarioAlta = objetoResultadoDetalle.IdUsuario;
                    //coleccion.FechaAlta = fechaActual;
                    //coleccion.FechaModificacion = fechaActual;

                    //List<ColeccionComponente> lstColeccionComponente = new List<ColeccionComponente>();

                    //List<ObjetoResultado> lstObjetoResultadoRango = lstObjetoResultado.Where(o => o.Rango == nroRango).ToList();
                    //foreach (var objetoResultado in lstObjetoResultadoRango)
                    //{
                    //    ColeccionComponente coleccionComponente = new ColeccionComponente();
                    //    //coleccionComponente.ComponenteId = 
                    //    long objetoId = 0;
                    //    bool convLong = long.TryParse(objetoResultado.IdOrigin, out objetoId);
                    //    coleccionComponente.ObjetoId = objetoId;
                    //    coleccionComponente.FechaAlta = fechaActual;
                    //    coleccionComponente.UsuarioAlta = objetoResultadoDetalle.IdUsuario;

                    //    lstColeccionComponente.Add(coleccionComponente);
                    //    cantObj++;
                    //}
                    //coleccion.ColeccionComponentes = lstColeccionComponente;
                    //db.Coleccion.Add(coleccion);
                    //db.SaveChanges();
                    #endregion

                    int rows = dbCommand.ExecuteNonQuery();

                    cantObj += rows;
                }
                db.Database.Connection.Close();
                //}
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - GrabarColeccion", ex);
            }
            return Ok(cantObj);
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetMapaTematicoExtents(string guid)
        {
            string extents = string.Empty;
            try
            {
                using (var builder = db.CreateSQLQueryBuilder())
                {
                    string extentWkt = builder
                                        .AddTable("MT_OBJETO_RESULTADO", "mto")
                                        .AddFilter("GUID", $"'{guid}'", SQLOperators.EqualsTo)
                                        .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo { Campo = "GEOMETRY" }, "mto")
                                                                 .BoundingBox()
                                                                 .ToWKT(), "wkt")
                                        .ExecuteQuery((IDataReader reader) => reader.GetStringOrEmpty(0))
                                        .Single();

                    return Ok(extentWkt);
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - GetMapaTematicoExtents", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetMapaTematicoExtentsForModPlot(string guid)
        {
            return StatusCode(System.Net.HttpStatusCode.Gone);
            //string extents = string.Empty;
            //try
            //{
            //    string sSql = "SELECT c.MIN_COORDS.SDO_POINT.X  || ';' || c.MIN_COORDS.SDO_POINT.Y " +
            //                          "|| ';' || c.MAX_COORDS.SDO_POINT.X  || ';' || c.MAX_COORDS.SDO_POINT.Y " +
            //                  "FROM ( " +
            //                      "SELECT  SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY(2001, {0}, MDSYS.SDO_POINT_TYPE(m.X_MIN, m.Y_MIN, 0), null, null),{0},22195) MIN_COORDS " +
            //                      ",SDO_CS.TRANSFORM(MDSYS.SDO_GEOMETRY(2001, {0}, MDSYS.SDO_POINT_TYPE(m.X_MAX, m.Y_MAX, 0), null, null),{0},22195) MAX_COORDS " +
            //                      "FROM " +
            //                          "(SELECT MIN(t.x) X_MIN, MIN(t.y) Y_MIN, MAX(t.x) X_MAX, MAX(t.y) Y_MAX " +
            //                          "FROM MT_OBJETO_RESULTADO, TABLE(mdsys.SDO_UTIL.GETVERTICES(geometry)) t " +
            //                          "WHERE GUID='" + guid + "'" +
            //                          ") m " +
            //                      ") c ";

            //    using (IDbCommand objComm = db.Database.Connection.CreateCommand())
            //    {
            //        db.Database.Connection.Open();
            //        objComm.CommandText = string.Format(sSql, SRID);
            //        using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
            //        {
            //            while (data.Read())
            //            {
            //                extents = data.GetValue(0).ToString();
            //                extents = extents.Replace(',', '.');
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Global.GetLogger().LogError("MapasTematicosServiceController - GetMapaTematicoExtentsForModPlot", ex);
            //    return InternalServerError(ex);
            //}
            //return Ok(extents);
        }

        public string GetCoordsByComponenteIdAtributoId(long componenteId, long atributoId, long operacionId, string valor1, string valor2)
        {
            string coords = string.Empty;
            try
            {
                Componente componente = db.Componente.Find(componenteId);
                db.Entry(componente).Collection(a => a.Atributos).Load();
                Atributo atributo = componente.Atributos.Where(p => p.AtributoId == atributoId).ToArray()[0];
                TipoOperacion tipoOperacionFull = db.TipoOperacion.Find(operacionId);

                string esquema = componente.Esquema;
                string tabla = componente.Tabla;
                string campoFeatId = string.Empty;
                string campoLabel = string.Empty;
                string campoGeometry = string.Empty;
                string campoId = string.Empty;
                string campoValor = string.Empty;
                string sqlWhere = string.Empty;

                List<Jerarquia> lstJerarquiaAll = db.Jerarquia.ToList();
                List<string> lstJoin = new List<string>();
                string sJoin = string.Empty;
                string sCond = string.Empty;
                esquema = componente.Esquema;
                tabla = componente.Tabla;

                try
                {
                    campoId = componente.Atributos.GetAtributoClave().Campo;
                    campoGeometry = componente.Atributos.GetAtributoGeometry().Campo;
                }
                catch (ApplicationException appEx)
                {
                    Global.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", appEx);
                    throw;
                }

                string sSql = "SELECT TO_CHAR(SDO_CS.MAKE_2D(" + tabla + "." + campoGeometry + ", 8307).Get_WKT()) as WKT ";

                #region Filtro por Atributo

                if (tipoOperacionFull.CantidadValores == 0)
                {
                    sCond = " " + componente.Tabla + "." + atributo.Campo + " " + GetOperacionTipo(tipoOperacionFull.TipoOperacionId) + " ";
                }
                else if (tipoOperacionFull.CantidadValores == 1)
                {
                    sCond = string.Format(" {0}.{1} {2} {3} ",
                                            componente.Tabla,
                                            atributo.Campo,
                                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId),
                                            atributo.GetFormattedValue(valor1));
                }
                else if (tipoOperacionFull.CantidadValores == 2)
                {
                    sCond = string.Format(" {0}.{1} {2} {3} AND {4} ",
                                            componente.Tabla,
                                            atributo.Campo,
                                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId),
                                            atributo.GetFormattedValue(valor1),
                                            atributo.GetFormattedValue(valor2));
                }
                if (!string.IsNullOrEmpty(sCond))
                {
                    sSql += string.Format(" WHERE {0} ", sCond);
                }
                #endregion

                using (IDbCommand objComm = db.Database.Connection.CreateCommand())
                {
                    db.Database.Connection.Open();
                    objComm.CommandText = sSql;
                    using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        List<string> lstCoordsAll = new List<string>();
                        while (data.Read())
                        {
                            //string wkt = wkt = data.GetString(0);
                            DbGeometry geom = data.GetGeometryFromField(0);

                            List<string> lstCoords = new List<string>();
                            if (geom != null)
                            {
                                double x = 0;
                                double y = 0;
                                if (geom.XCoordinate == null)
                                {
                                    for (int i = 1; i <= (int)geom.PointCount; i++)
                                    {
                                        x = (double)geom.PointAt(i).XCoordinate;
                                        y = (double)geom.PointAt(i).YCoordinate;
                                        lstCoords.Add(string.Format("{0},{1},0", (double)geom.PointAt(i).XCoordinate, (double)geom.PointAt(i).YCoordinate));
                                    }
                                }
                                else
                                {
                                    x = (double)geom.XCoordinate;
                                    y = (double)geom.YCoordinate;
                                    lstCoords.Add(string.Format("{0},{1},0", (double)geom.XCoordinate, (double)geom.YCoordinate));
                                }
                            }
                            lstCoordsAll.Add(string.Join(",", lstCoords));
                        }
                        coords = string.Join("+", lstCoordsAll);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - GetCoordsByComponenteIdAtributoId", ex);
                return string.Empty;
            }
            return coords;
        }

        [HttpGet]
        [ResponseType(typeof(string))]
        public IHttpActionResult BuscarAyuda()
        {
            return Ok(Convert.ToBase64String(File.ReadAllBytes(ConfigurationManager.AppSettings["HelpFileMT"])));
        }

        [HttpGet]
        [ResponseType(typeof(Predefinido))]
        //editado para que filtre por categoria también mantis 1386: Bug 6127
        public IHttpActionResult GetPredefinidoByConfiguracionId(long configuracionId, int categoriaId)
        {
            Predefinido predefinido = db.Predefinido.Where(p => p.ConfiguracionId == configuracionId && p.IdPlantillaCategoria == categoriaId).FirstOrDefault();
            if (predefinido == null)
            {
                return NotFound();
            }
            return Ok(predefinido);
        }

        [HttpGet]
        [ResponseType(typeof(Predefinido))]
        public IHttpActionResult GetPredefinidoById(long id)
        {
            Predefinido predefinido = db.Predefinido.Where(p => p.IdPredefinido == id).FirstOrDefault();
            if (predefinido == null)
            {
                return NotFound();
            }
            return Ok(predefinido);
        }

        [HttpGet]
        [ResponseType(typeof(Plantilla))]
        public IHttpActionResult GetPlantillaByIdPlantillaCategoria(int idPlantillaCategoria)
        {
            Plantilla plantilla = db.Plantillas.Where(p => p.IdPlantillaCategoria == idPlantillaCategoria).FirstOrDefault();
            if (plantilla == null)
            {
                return NotFound();
            }
            return Ok(plantilla);
        }

        //[HttpGet]
        //[ResponseType(typeof(int))]
        //public IHttpActionResult GetObjetoResultadoGeometryType(string guid)
        //{
        //    int geometryType = 0;
        //    List<ObjetoResultado> lstObjetoResultado = db.ObjetoResultados.Where(p => p.GUID == guid).ToList();

        //    if (geometryType == 0)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(geometryType);
        //}

        //private int? GetNroRango(string valor, List<Rango> lstRangos, long distribucion)
        //{
        //    int? nroRango = null;
        //    for (int i = 0; i < lstRangos.Count; i++)
        //    {
        //        Rango rango = lstRangos[i];
        //        if (distribucion == 1 || distribucion == 2)
        //        {
        //            if (Convert.ToDouble(valor) >= Convert.ToDouble(rango.Desde) && Convert.ToDouble(valor) <= Convert.ToDouble(rango.Hasta))
        //            {
        //                nroRango = i + 1;
        //                break;
        //            }
        //        }
        //        else if (distribucion == 3)
        //        {
        //            if (valor == rango.Valor)
        //            {
        //                nroRango = i + 1;
        //                break;
        //            }
        //        }
        //    }
        //    return nroRango;
        //}

        private Rango GetRango(string valor, List<Rango> lstRangos, long distribucion)
        {
            Rango rangoEncontrado = null;
            for (int i = 0; i < lstRangos.Count; i++)
            {
                Rango rango = lstRangos[i];
                if (distribucion == 1 || distribucion == 2)
                {
                    if (Convert.ToDouble(valor) >= Convert.ToDouble(rango.Desde) && Convert.ToDouble(valor) <= Convert.ToDouble(rango.Hasta))
                    {
                        rangoEncontrado = rango;
                        break;
                    }
                }
                else if (distribucion == 3)
                {
                    if (valor == rango.Valor)
                    {
                        rangoEncontrado = rango;
                        break;
                    }
                }
            }
            return rangoEncontrado;
        }

        private ObjetoResultadoDetalle GenerarObjetoResultado(RequestObjetoResultado requestObjetoResultado, out bool generado)
        {
            generado = false;
            try
            {
                SQLSpatialRelationships getSpatialRelationships(ConfiguracionFiltro filtro)
                {
                    SQLSpatialRelationships mask = SQLSpatialRelationships.AnyInteract;
                    if (filtro.Fuera == 1)
                    {
                        mask = SQLSpatialRelationships.Disjoint;
                    }
                    else if (filtro.PorcentajeInterseccion.GetValueOrDefault() == 0L)
                    {
                        if (filtro.Dentro == 1 && filtro.Tocando == 1)
                        {
                            mask = SQLSpatialRelationships.Inside | SQLSpatialRelationships.CoveredBy |
                                   SQLSpatialRelationships.Equal | SQLSpatialRelationships.Touch |
                                   SQLSpatialRelationships.Overlaps;

                        }
                        else
                        {
                            if (filtro.Dentro == 1)
                            {
                                mask = SQLSpatialRelationships.Inside | SQLSpatialRelationships.CoveredBy |
                                       SQLSpatialRelationships.Equal;
                            }
                            else if (filtro.Tocando == 1)
                            {
                                mask = SQLSpatialRelationships.Touch | SQLSpatialRelationships.Overlaps;
                            }
                        }
                    }
                    return mask;
                }
                using (var insert = db.CreateSQLQueryBuilder())
                using (var dataBuilder = db.CreateSQLQueryBuilder())
                using (var resultBuilder = db.CreateSQLQueryBuilder())
                {
                    #region Datos Componente Tematizado
                    Componente componenteTematico = db.Componente
                                                        .Include(a => a.Atributos)
                                                        .SingleOrDefault(c => c.ComponenteId == requestObjetoResultado.Componente.ComponenteId);

                    Componente componenteTablaGrafica = null;
                    Componente componenteAtributoTematizado = componenteTematico;
                    Atributo campoLabel = null;
                    Atributo campoGeometry = null;
                    Atributo campoId = null;

                    try
                    {
                        campoId = componenteTematico.Atributos.GetAtributoClave();
                        campoGeometry = componenteTematico.Atributos.GetAtributoGeometry();
                        componenteTablaGrafica = new Componente
                        {
                            ComponenteId = componenteTematico.ComponenteId * -1,
                            Esquema = componenteTematico.Esquema,
                            Tabla = componenteTematico.TablaGrafica ?? componenteTematico.Tabla
                        };
                        if (requestObjetoResultado.MapaTematicoConfiguracion.ConfiguracionId != 24 && requestObjetoResultado.MapaTematicoConfiguracion.ConfiguracionId != 31)
                        {
                            campoLabel = componenteTematico.Atributos.GetAtributoLabel();
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.GetLogger().LogError("Componente (id: " + componenteTematico.ComponenteId + ") mal configurado.", ex);
                        return null;
                    }
                    #endregion
                    #region Atributo Tematizado
                    Atributo campoTematizado = requestObjetoResultado.EsImportado
                                                    ? requestObjetoResultado.Atributo
                                                    : db.Atributo.Find(requestObjetoResultado.Atributo.AtributoId);
                    Atributo campoMatcheo = null;
                    if (campoTematizado == null || campoTematizado.ComponenteId == 0)
                    {
                        Global.GetLogger().LogError($"Atributo {campoTematizado.Nombre} (importado: {requestObjetoResultado.EsImportado}) no existente.", new KeyNotFoundException());
                        return null;
                    }
                    if (requestObjetoResultado.EsImportado)
                    {
                        var datoExternoConfig = db.DatoExternoConfiguracion.FirstOrDefault(p => p.DatoExternoConfiguracionId == campoTematizado.AtributoId);
                        if (datoExternoConfig != null)
                        {
                            campoMatcheo = db.Atributo.Find(datoExternoConfig.AtributoId);
                            if (campoMatcheo == null)
                            {
                                Global.GetLogger().LogError($"Atributo Referido al Importado (atributo importado: {campoTematizado.AtributoId} - atributo referido: {datoExternoConfig.AtributoId}) no existente.", new KeyNotFoundException());
                                return null;
                            }
                        }
                    }
                    #endregion
                    #region Armado de SQL
					var agregados = new HashSet<long>();
                    bool hasAggregatedFunction = false;
                    dataBuilder.AddTable(componenteTematico, componenteTematico.Tabla)
                               .AddFields(campoId);

                    resultBuilder.AddTable(componenteTematico, "resultados");

                    if (componenteTematico.Tabla != componenteTablaGrafica.Tabla)
                    {
                        var campoIdTG = new Atributo { ComponenteId = componenteTablaGrafica.ComponenteId, Campo = campoId.Campo, TipoDatoId = campoId.TipoDatoId };
                        dataBuilder.AddJoin(componenteTablaGrafica, componenteTablaGrafica.Tabla, campoIdTG, campoId);
                        resultBuilder.AddJoin(componenteTablaGrafica, componenteTablaGrafica.Tabla, campoIdTG, campoId);

                        db.Entry(campoGeometry).State = EntityState.Detached;
                        campoGeometry.ComponenteId = componenteTablaGrafica.ComponenteId;
                        campoGeometry.Componente = componenteTablaGrafica;
                    }
                    dataBuilder.AddFilter(campoGeometry, null, SQLOperators.IsNotNull);
                    resultBuilder.AddFilter(campoGeometry, null, SQLOperators.IsNotNull);

                    if (requestObjetoResultado.EsImportado)
                    {
                        long tempId = DateTime.Now.Ticks * -1;
                        campoTematizado = new Atributo
                        {
                            Campo = "valor",
                            Componente = new Componente { Tabla = "mt_dato_externo", Esquema = ConfigurationManager.AppSettings["DATABASE"], ComponenteId = tempId },
                            ComponenteId = tempId,
                            TipoDatoId = 6 //siempre es texto
                        };
                    }
                    else
                    {
                        if (campoTematizado.ComponenteId != componenteAtributoTematizado.ComponenteId)
                        {
                            hasAggregatedFunction = true;
                            foreach (var relacion in GetJerarquia(componenteAtributoTematizado, campoTematizado.ComponenteId))
                            {
                                dataBuilder.AddJoin(relacion.ComponenteJoin, relacion.ComponenteJoin.Tabla, relacion.AtributoClaveJoin, relacion.AtributoClaveMain);
                            }
                            campoTematizado.Componente = (componenteAtributoTematizado = db.Componente.Find(campoTematizado.ComponenteId));
                        }
                        if (campoTematizado.AtributoParentId.HasValue)
                        {//Si el atributo se relaciona con otro, debe hacer un join con el componente del atributo.
                            var datosListado = (from attrParent in db.Atributo
                                                join compParent in db.Componente on attrParent.ComponenteId equals compParent.ComponenteId
                                                join attrParentClave in db.Atributo on attrParent.ComponenteId equals attrParentClave.ComponenteId
                                                where attrParent.AtributoId == campoTematizado.AtributoParentId && attrParentClave.EsClave == 1
                                                select new { attrParent, compParent, attrParentClave })
                                                .First();

                            dataBuilder.AddJoin(datosListado.compParent, datosListado.compParent.Tabla,
                                            datosListado.attrParentClave,
                                            campoTematizado, SQLJoin.Left);

                            componenteAtributoTematizado = datosListado.compParent;
                            campoTematizado = datosListado.attrParent;
							agregados.Add(datosListado.compParent.ComponenteId);
                        }
                    }
                    if (hasAggregatedFunction)
                    {
                        dataBuilder.AddAggregatedField(campoTematizado, (SQLAggregatedFunction)requestObjetoResultado.MapaTematicoConfiguracion.Agrupacion);
                        dataBuilder.GroupBy(campoId);
                    }
                    else
                    {
                        dataBuilder.AddFields(campoTematizado);
                    }
                    SQLConnectors connector = SQLConnectors.And;
                    foreach (var filtro in requestObjetoResultado.ConfiguracionFiltros)
                    {
                        switch (filtro.FiltroTipo)
                        {
                            case 1: //filtro alfa
                                {
                                    var tipoOperacion = db.TipoOperacion.Find(filtro.FiltroOperacion);
                                    var atributoFiltro = db.Atributo.Find(filtro.FiltroAtributo);
                                    var componenteFiltro = db.Componente.Find(filtro.FiltroComponente);

                                    if (filtro.FiltroComponente != componenteTematico.ComponenteId)
                                    {
                                        #region Si filtro no es del componente buscado
                                        foreach (var relacion in GetJerarquia(componenteTematico, filtro.FiltroComponente))
                                        {
                                            dataBuilder.AddJoin(relacion.ComponenteJoin, relacion.ComponenteJoin.Tabla, relacion.AtributoClaveJoin, relacion.AtributoClaveMain);
                                            if (relacion.AtributoFiltro != null)
                                            {
                                                dataBuilder.AddTableFilter(relacion.AtributoFiltro, relacion.ValorFiltro, SQLOperators.EqualsTo);
                                            }
                                        }
                                        #endregion
                                    }
                                    if (atributoFiltro.AtributoParentId.HasValue)
                                    {//Si el atributo se relaciona con otro, debe hacer un join con el componente del atributo.
                                        var datosListado = (from attrParent in db.Atributo
                                                            join compParent in db.Componente on attrParent.ComponenteId equals compParent.ComponenteId
                                                            join attrParentClave in db.Atributo on attrParent.ComponenteId equals attrParentClave.ComponenteId
                                                            join attrParentLabel in db.Atributo on attrParent.ComponenteId equals attrParentLabel.ComponenteId
                                                            where attrParent.AtributoId == atributoFiltro.AtributoParentId && attrParentClave.EsClave == 1 && attrParentLabel.EsLabel
                                                            select new { attrParent, compParent, attrParentClave, attrParentLabel })
															.AsNoTracking()
                                                            .First();

                                        if (agregados.Add(datosListado.compParent.ComponenteId))
                                        {//si devuelve false es porque ya existe y no lo agregó
                                            dataBuilder.AddJoin(datosListado.compParent, datosListado.compParent.Tabla,
                                                                atributoFiltro,
                                                                datosListado.attrParentClave, SQLJoin.Inner);
                                        }
                                        componenteFiltro = datosListado.compParent;
                                        atributoFiltro = datosListado.attrParentLabel;
                                        filtro.Valor1 = $"{string.Join(",", filtro.Valor1.Split(',').Select(v => atributoFiltro.GetFormattedValue(v)))}";
                                        atributoFiltro.TipoDatoId = 0;//esto es para que el builder no formatee nuevamente el valor
                                    }
                                    if (tipoOperacion != null)
                                    {
                                        if (tipoOperacion.CantidadValores != 2)
                                        {
                                            if (tipoOperacion.CantidadValores == 0)
                                            {
                                                filtro.Valor1 = null;
                                            }
                                            dataBuilder.AddFilter(atributoFiltro, filtro.Valor1, tipoOperacion, connector);
                                        }
                                        else if (tipoOperacion.CantidadValores == 2)
                                        {
                                            dataBuilder.BeginFilterGroup(connector)
                                                       .AddFilter(atributoFiltro, filtro.Valor1, SQLOperators.GreaterThan | SQLOperators.EqualsTo)
                                                       .AddFilter(atributoFiltro, filtro.Valor2, SQLOperators.LowerThan | SQLOperators.EqualsTo, SQLConnectors.And)
                                                       .EndFilterGroup();
                                        }
                                    }
                                }
                                break;
                            case 2: //filtro gráfico
                                {
                                    if (filtro.ConfiguracionesFiltroGrafico?.Count > 0)
                                    {
                                        #region Condicion para Filtro Grafico Normal
                                        foreach (var configGrafica in filtro.ConfiguracionesFiltroGrafico)
                                        {
                                            var coords = configGrafica.Coordenadas.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                                            if (coords.Any())
                                            {
                                                var geometry = dataBuilder.CreateGeometryFieldBuilder(coords.First(), SRID.DB);
                                                var geomTabla = dataBuilder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla);
                                                if (filtro.Fuera != 1)
                                                {
                                                    dataBuilder.BeginFilterGroup(connector);
                                                    if (filtro.Ampliar.GetValueOrDefault() != 0L)
                                                    {//tiene buffer, positivo o negativo
                                                        geometry.AddBuffer(filtro.Ampliar.GetValueOrDefault());
                                                    }
                                                    dataBuilder.AddFilter(geomTabla, geometry, getSpatialRelationships(filtro));
                                                    if (filtro.PorcentajeInterseccion.GetValueOrDefault() > 0L)
                                                    {
                                                        var overlapedGeom = dataBuilder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla)
                                                                                   .OverlappingArea(geometry);

                                                        dataBuilder.AddRawFilter(string.Format("({0} / {1})", overlapedGeom, geomTabla.Area()),
                                                                                filtro.PorcentajeInterseccion.GetValueOrDefault() / 100d,
                                                                                SQLOperators.GreaterThan, SQLConnectors.And);
                                                    }
                                                    dataBuilder.EndFilterGroup();
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Condicion para Filtro Grafico Avanzado
                                        if (filtro.FiltroComponente.GetValueOrDefault() > 0)
                                        {
                                            var componenteFGAAlfa = db.Componente
                                                                      .Include(a => a.Atributos)
                                                                      .Single(c => c.ComponenteId == filtro.FiltroComponente);

                                            Componente componenteFGAGraf;
                                            Atributo campoGeometryFGA;
                                            Atributo campoFeatIdFGA = componenteFGAAlfa.Atributos.GetAtributoFeatId();
                                            Atributo atributoFGA = db.Atributo.Find(filtro.FiltroAtributo);
                                            try
                                            {
                                                componenteFGAGraf = new Componente()
                                                {
                                                    Esquema = componenteFGAAlfa.Esquema,
                                                    Tabla = componenteFGAAlfa.TablaGrafica ?? componenteFGAAlfa.Tabla
                                                };
                                                campoGeometryFGA = componenteFGAAlfa.Atributos.GetAtributoGeometry();
                                            }
                                            catch (ApplicationException appEx)
                                            {
                                                Global.GetLogger().LogError("Componente (id: " + componenteFGAAlfa.ComponenteId + ") mal configurado.", appEx);
                                                return null;
                                            }
                                            TipoOperacion tipoOperacionFGA = null;
                                            dataBuilder.AddTable(componenteFGAAlfa, componenteFGAAlfa.Tabla);
                                            bool abreGrupoJoin = componenteFGAAlfa.Tabla != componenteFGAGraf.Tabla;
                                            if (abreGrupoJoin)
                                            {
                                                dataBuilder.AddTable(componenteFGAGraf, componenteFGAGraf.Tabla)
                                                       .BeginFilterGroup(connector)
                                                       .AddJoinFilter(componenteFGAAlfa.Tabla, campoFeatIdFGA, componenteFGAGraf.Tabla, campoFeatIdFGA);
                                                connector = SQLConnectors.And;
                                            }
                                            var geomTablaGraf = dataBuilder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla);
                                            var geomTablaFiltro = dataBuilder.CreateGeometryFieldBuilder(campoGeometryFGA, componenteFGAGraf.Tabla);

                                            dataBuilder.BeginFilterGroup(connector);
                                            dataBuilder.AddFilter(geomTablaGraf, geomTablaFiltro, getSpatialRelationships(filtro));
                                            if (filtro.FiltroOperacion != 0)
                                            {
                                                tipoOperacionFGA = db.TipoOperacion.Find(filtro.FiltroOperacion);
                                            }
                                            if (tipoOperacionFGA != null)
                                            {
                                                if (tipoOperacionFGA.CantidadValores != 2)
                                                {
                                                    if (tipoOperacionFGA.CantidadValores == 0)
                                                    {
                                                        filtro.Valor1 = null;
                                                    }
                                                    dataBuilder.AddFilter(atributoFGA, filtro.Valor1, tipoOperacionFGA, SQLConnectors.And);
                                                }
                                                else if (tipoOperacionFGA.CantidadValores == 2)
                                                {
                                                    dataBuilder.BeginFilterGroup(SQLConnectors.And)
                                                               .AddFilter(atributoFGA, filtro.Valor1, SQLOperators.GreaterThan | SQLOperators.EqualsTo)
                                                               .AddFilter(atributoFGA, filtro.Valor2, SQLOperators.LowerThan | SQLOperators.EqualsTo, SQLConnectors.And)
                                                               .EndFilterGroup();
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(filtro.Valor1))
                                            { //Es atributo con valor fijo
                                                dataBuilder.AddFilter(atributoFGA, filtro.Valor1, SQLOperators.EqualsTo, SQLConnectors.And);
                                            }
                                            dataBuilder.EndFilterGroup();
                                            if (abreGrupoJoin)
                                            {
                                                dataBuilder.EndFilterGroup();
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                break;
                            case 3: //filtro colección
                                {
                                    long idComponente = db.ColeccionComponente.First(cc => cc.ColeccionId == filtro.FiltroColeccion.Value).ComponenteId;
                                    var componenteColeccion = componenteTematico;
                                    var campoIdComponenteColeccion = campoId;
                                    var campoGeometryComponenteColeccion = campoGeometry;
                                    if (componenteTematico.ComponenteId != idComponente)
                                    {
                                        componenteColeccion = db.Componente.Find(idComponente);
                                        if (componenteColeccion.Graficos != 5)
                                        {
                                            var atributosComponenteColeccion = db.Atributo.Where(a => a.ComponenteId == idComponente);
                                            campoGeometryComponenteColeccion = atributosComponenteColeccion.GetAtributoGeometry();
                                            campoIdComponenteColeccion = atributosComponenteColeccion.GetAtributoClave();
                                        }
                                    }
                                    if (componenteTematico.ComponenteId == componenteColeccion.ComponenteId || componenteColeccion.Graficos != 5)
                                    {
                                        long tempId = filtro.FiltroColeccion.Value * -1;
                                        string aliasTablaColecComp = $"col_{filtro.FiltroColeccion}";
                                        string aliasTablaComp = componenteTematico.Tabla;
                                        dataBuilder.AddTable(new Componente() { Esquema = ConfigurationManager.AppSettings["DATABASE"], Tabla = "GE_COLEC_COMP", ComponenteId = tempId }, aliasTablaColecComp)
                                               .BeginFilterGroup(connector);

                                        connector = SQLConnectors.None;
                                        if (componenteTematico.ComponenteId != componenteColeccion.ComponenteId)
                                        {
                                            aliasTablaComp = $"tblrel_{filtro.FiltroColeccion}";
                                            dataBuilder.AddTable(componenteColeccion.Esquema, componenteColeccion.TablaGrafica ?? componenteColeccion.Tabla, aliasTablaComp)
                                                       .AddFilter(dataBuilder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla),
                                                                  dataBuilder.CreateGeometryFieldBuilder(campoGeometryComponenteColeccion, aliasTablaComp),
                                                                  SQLSpatialRelationships.AnyInteract);
                                            connector = SQLConnectors.And;
                                        }
                                        dataBuilder.AddJoinFilter(aliasTablaColecComp, new Atributo() { ComponenteId = tempId, Campo = "ID_OBJETO" }, aliasTablaComp, campoIdComponenteColeccion)
                                               .BeginFilterGroup(connector)
                                               .AddFilter(new Atributo() { ComponenteId = tempId, Campo = "ID_COLECCION" }, filtro.FiltroColeccion, SQLOperators.EqualsTo)
                                               .AddFilter(new Atributo() { ComponenteId = tempId, Campo = "ID_COMPONENTE" }, componenteColeccion.ComponenteId, SQLOperators.EqualsTo, SQLConnectors.And)
                                               .AddFilter(new Atributo() { ComponenteId = tempId, Campo = "FECHA_BAJA" }, null, SQLOperators.IsNull, SQLConnectors.And)
                                               .EndFilterGroup()
                                               .EndFilterGroup();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        connector = SQLConnectors.And;
                    }

                    resultBuilder.AddJoin(dataBuilder.ToString(), "datos", "resultados", new[] { campoId });
                    campoTematizado = db.Entry(campoTematizado).CurrentValues.ToObject() as Atributo;
                    campoTematizado.ComponenteId = DateTime.Now.Ticks * -1;
                    campoTematizado.Componente = new Componente() { Tabla = "datos" };

                    #endregion
                    #region Insert de Datos
                    string aliasTablaGrafica = componenteTablaGrafica.Tabla != componenteTematico.Tabla
                                                    ? componenteTablaGrafica.Tabla
                                                    : "resultados";

                    if (hasAggregatedFunction &&
                        (SQLAggregatedFunction)requestObjetoResultado.MapaTematicoConfiguracion.Agrupacion == SQLAggregatedFunction.Count)
                    { //si es count, siempre el resultado es numerico
                        campoTematizado.TipoDatoId = 3;
                    }

                    object valorNum = null;
                    var tiposNumero = new[] { 2, 3, 4, 5, 7 };
                    if (tiposNumero.Contains(campoTematizado.TipoDatoId))
                    {
                        valorNum = campoTematizado;
                    }

                    var fields = new KeyValuePair<Atributo, object>[]
                    {
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "id_usuario" }, requestObjetoResultado.IdUsuario),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "guid", TipoDatoId = 6 }, requestObjetoResultado.GUID),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "token_sesion", TipoDatoId = 6 }, requestObjetoResultado.TokenSesion),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "id_origin" }, campoId),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "descripcion" }, campoLabel ?? null),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "valor", TipoDatoId = campoTematizado.TipoDatoId }, campoTematizado),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "fecha_alta", TipoDatoId = 666 }, null),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "geometry" }, campoGeometry),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "geometry_type" }, dataBuilder.CreateGeometryFieldBuilder(campoGeometry,aliasTablaGrafica).GType()),
                        new KeyValuePair<Atributo, object>(new Atributo() { Campo = "valor_num" }, valorNum)
                    };

                    insert.AddTable(ConfigurationManager.AppSettings["DATABASE"], "mt_objeto_resultado", null)
                          .AddQueryToInsert(resultBuilder, fields)
                          .ExecuteInsert();

                    generado = true;
                    #endregion
                }
                #region Evaluación de Distribucion "best match"
                var lstObjetoResultado = db.ObjetoResultados.Where(p => p.GUID == requestObjetoResultado.GUID).ToList();
                int valoresDistintos = lstObjetoResultado.Select(obj => obj.Valor).Distinct().Count();
                long distribucion = requestObjetoResultado.MapaTematicoConfiguracion.Distribucion;

                if (distribucion == 0)
                {
                    double calculo = Convert.ToDouble(valoresDistintos) / Convert.ToDouble(lstObjetoResultado.Count);
                    string paramValue = db.ParametrosGenerales.FirstOrDefault(p => p.Descripcion == "Distribucion")?.Valor;
                    if (!string.IsNullOrEmpty(paramValue))
                    {
                        double distribucionParam = Convert.ToDouble(paramValue);
                        if (calculo >= 0.1)
                        {
                            distribucion = 1;
                        }
                        else
                        {
                            if (valoresDistintos <= 100)
                            {
                                distribucion = 3;
                            }
                            else
                            {
                                distribucion = 1;
                            }
                        }
                    }
                }
                #endregion
                return GetRangos(lstObjetoResultado, requestObjetoResultado.GUID, distribucion, requestObjetoResultado.MapaTematicoConfiguracion.Rangos);
            }
            catch
            {
                throw;
            }
        }

        public ObjetoResultadoDetalle GetRangos(IEnumerable<ObjetoResultado> lstObjetoResultado, string guid, long distribucion, long cantRangos)
        {
            if (cantRangos == 0)
            {
                cantRangos = 5;
            }
            try
            {
                double minValue = 999999999;
                double maxValue = -99999999;
                int maxCantDecimales = 0;

                var objetoResultadoDetalle = new ObjetoResultadoDetalle()
                {
                    GUID = guid,
                    Distribucion = distribucion
                };
                if (lstObjetoResultado.Any())
                {
                    objetoResultadoDetalle.GeometryType = lstObjetoResultado.Select(o => o.GeometryType).Distinct().Count() > 1 ? 4 : lstObjetoResultado.First().GeometryType;
                    //Determino rango desde y hasta
                    if (distribucion == 1 || distribucion == 2)
                    {
                        try
                        {
                            foreach (var objetoResultado in lstObjetoResultado.Where(obj => !string.IsNullOrEmpty(obj.Valor)))
                            {
                                double valor = Convert.ToDouble(objetoResultado.Valor.Replace(',', '.'));
                                minValue = Math.Min(minValue, valor);
                                maxValue = Math.Max(maxValue, valor);

                                string[] aDecimales = valor.ToString().Split('.');
                                int decimales = aDecimales.Length == 2 ? aDecimales[1].Length : 0;
                                maxCantDecimales = Math.Max(decimales, maxCantDecimales);
                            }
                            objetoResultadoDetalle.RangoDesde = Math.Floor(minValue);
                            objetoResultadoDetalle.RangoHasta = Math.Ceiling(maxValue);
                        }
                        catch
                        {
                            distribucion = 3;
                        }
                    }

                    Color start = Color.FromArgb(0, 148, 255);
                    Color end = Color.White;

                    var lstRango = new List<Rango>();
                    if (distribucion == 1)
                    {
                        #region Uniforme
                        double valores = objetoResultadoDetalle.RangoHasta - objetoResultadoDetalle.RangoDesde;

                        bool buscoCantRangos = false;
                        double paso = 0;
                        if (maxCantDecimales == 0)
                        {
                            while (!buscoCantRangos)
                            {
                                if ((valores / cantRangos) < 1)
                                {
                                    cantRangos--;
                                }
                                else
                                {
                                    buscoCantRangos = true;
                                }
                            }
                            cantRangos = Math.Max(cantRangos, 1);
                            paso = Math.Ceiling(valores / cantRangos);
                        }
                        else
                        {
                            paso = Math.Round(Convert.ToDouble(valores / cantRangos), maxCantDecimales);
                        }
                        var desde = objetoResultadoDetalle.RangoDesde;
                        var hasta = desde + paso;
                        for (int i = 0; i < cantRangos; i++)
                        {
                            int casos = 0;
                            if (cantRangos == 1 || maxCantDecimales >= 0 && i == cantRangos - 1)
                            {
                                casos = lstObjetoResultado.Count(p => ConvertToDouble(p.Valor) >= desde && ConvertToDouble(p.Valor) <= hasta);
                            }
                            else
                            {
                                casos = lstObjetoResultado.Count(p => ConvertToDouble(p.Valor) == desde || (ConvertToDouble(p.Valor) > desde && ConvertToDouble(p.Valor) < hasta));
                            }

                            int r = Interpolate(start.R, end.R, ((cantRangos > 1) ? Convert.ToInt32(cantRangos - 1) : 1), i),
                                g = Interpolate(start.G, end.G, ((cantRangos > 1) ? Convert.ToInt32(cantRangos - 1) : 1), i),
                                b = Interpolate(start.B, end.B, ((cantRangos > 1) ? Convert.ToInt32(cantRangos - 1) : 1), i);
                            Color colorRango = Color.FromArgb(r, g, b);
                            string sColorRango = colorRango.R.ToString("X2") + colorRango.G.ToString("X2") + colorRango.B.ToString("X2");

                            lstRango.Add(new Rango()
                            {
                                Desde = desde,
                                Hasta = hasta,
                                Casos = casos,
                                Color = sColorRango,
                                ColorBorde = sColorRango,
                                AnchoBorde = 0,
                                Leyenda = $"{desde} - {hasta}",
                                PonerLabel = lstObjetoResultado.Any(p => ConvertToDouble(p.Valor) >= desde && ConvertToDouble(p.Valor) < hasta && !string.IsNullOrEmpty(p.Descripcion))
                            });

                            desde = hasta;
                            if (i == cantRangos - 2)
                            {
                                hasta = maxCantDecimales == 0 ? Math.Ceiling(maxValue) : maxValue;
                            }
                            else
                            {
                                hasta += paso;
                            }
                        }
                        #endregion
                    }
                    else if (distribucion == 2)
                    {
                        #region Cuantiles
                        var lstObjetoResultadoOrder = lstObjetoResultado.OrderBy(p => Convert.ToDouble(p.Valor?.Replace(',', '.') ?? "0")).ToList();
                        int casos = Convert.ToInt32(Math.Floor((double)(lstObjetoResultado.Count() / (int)cantRangos)));
                        int resto = (lstObjetoResultado.Count() % (int)cantRangos);
                        double desde = objetoResultadoDetalle.RangoDesde;
                        double hasta = 0d;
                        int iObj = 0;
                        for (int i = 0; i < cantRangos; i++)
                        {
                            int cantMuestra = 0;
                            while (cantMuestra < casos && iObj < lstObjetoResultadoOrder.Count)
                            {
                                cantMuestra++;
                                iObj++;
                            }
                            if (cantMuestra == casos)
                            {
                                if (i == cantRangos - 1)
                                {
                                    hasta = Math.Ceiling(maxValue);
                                }
                                else
                                {
                                    hasta = ConvertToDouble(lstObjetoResultadoOrder[iObj].Valor);
                                    if (maxCantDecimales == 0)
                                    {
                                        hasta = Math.Floor(hasta);
                                    }
                                }
                                int r = Interpolate(start.R, end.R, Convert.ToInt32(cantRangos - 1), i),
                                    g = Interpolate(start.G, end.G, Convert.ToInt32(cantRangos - 1), i),
                                    b = Interpolate(start.B, end.B, Convert.ToInt32(cantRangos - 1), i);

                                Color colorRango = cantRangos == 1 ? start : Color.FromArgb(r, g, b);
                                string sColorRango = colorRango.R.ToString("X2") + colorRango.G.ToString("X2") + colorRango.B.ToString("X2");
                                lstRango.Add(new Rango()
                                {
                                    Desde = desde,
                                    Hasta = hasta,
                                    Casos = casos,
                                    Color = sColorRango,
                                    ColorBorde = sColorRango,
                                    AnchoBorde = 0,
                                    Leyenda = $"{desde} - {hasta}",
                                    PonerLabel = lstObjetoResultado.Any(p => ConvertToDouble(p.Valor) >= desde && Convert.ToDouble(p.Valor) <= hasta && !string.IsNullOrEmpty(p.Descripcion))
                                });
                                desde = hasta;
                            }
                        }
                        if (iObj < lstObjetoResultadoOrder.Count)
                        {
                            lstRango.Last().Hasta = Math.Ceiling(maxValue);
                            lstRango.Last().Casos += resto;
                            lstRango.Last().Leyenda = $"{lstRango.Last().Desde} - {lstRango.Last().Hasta}";
                        }
                        #endregion
                    }
                    else if (distribucion == 3)
                    {
                        #region Valores Individuales
                        var lstValores = lstObjetoResultado.OrderBy(p => p.Valor).Select(p => p.Valor).Distinct().ToList();
                        for (int i = 0; i < lstValores.Count; i++)
                        {
                            string valor = lstValores[i];
                            int casos = lstObjetoResultado.Count(p => p.Valor == valor);

                            int r = Interpolate(start.R, end.R, ((lstValores.Count > 1) ? Convert.ToInt32(lstValores.Count - 1) : 1), i),
                                g = Interpolate(start.G, end.G, ((lstValores.Count > 1) ? Convert.ToInt32(lstValores.Count - 1) : 1), i),
                                b = Interpolate(start.B, end.B, ((lstValores.Count > 1) ? Convert.ToInt32(lstValores.Count - 1) : 1), i);

                            Color colorRango = Color.FromArgb(r, g, b);
                            string sColorRango = colorRango.R.ToString("X2") + colorRango.G.ToString("X2") + colorRango.B.ToString("X2");

                            lstRango.Add(new Rango()
                            {
                                Valor = valor,
                                Casos = casos,
                                Color = sColorRango,
                                ColorBorde = sColorRango,
                                AnchoBorde = 0,
                                Leyenda = valor,
                                PonerLabel = !string.IsNullOrEmpty(valor) && lstObjetoResultado.Any(p => p.Valor == valor && !string.IsNullOrEmpty(p.Descripcion))
                            });
                        }
                        #endregion
                    }
                    objetoResultadoDetalle.Rangos = lstRango;
                    objetoResultadoDetalle.Distribucion = distribucion;
                }
                return objetoResultadoDetalle;
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - GetRangos", ex);
                return null;
            }
        }

        [HttpGet]
        [ResponseType(typeof(int))]
        public IHttpActionResult CalcularCasos(string guid, long distribucion, double desde, double hasta)
        {
            try
            {
                var lstObjetoResultado = db.ObjetoResultados.Where(p => p.GUID == guid).ToList();
                int casos = 0;
                if (lstObjetoResultado.Any())
                {
                    if (distribucion == 1 || distribucion == 2)
                    {
                        //Uniforme o Cuantiles
                        casos = lstObjetoResultado.Count(p => ConvertToDouble(p.Valor) >= desde && ConvertToDouble(p.Valor) < hasta);
                    }
                }
                return Ok(casos);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - CalcularCasos", ex);
                return InternalServerError(ex);
            }
        }

        // [Route("BorrarDescriptionGuid/{idGuid}")]
        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult BorrarDescriptionGuid(string idGuid)
        {
            bool ret = false;
            try
            {
                IDbCommand objComm = db.Database.Connection.CreateCommand();
                db.Database.Connection.Open();
                var query = string.Format("UPDATE MT_OBJETO_RESULTADO SET DESCRIPCION='' WHERE GUID='{0}'", idGuid);
                objComm.CommandText = query;
                objComm.ExecuteNonQuery();
                ret = true;
            }
            catch (System.Exception ex)
            {
                Global.GetLogger().LogError("MapasTematicosServiceController - BorrarDescription", ex);
            }
            finally
            {
                db.Database.Connection.Close();
            }
            return Ok(ret);
        }
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UpdateRangosIcono(ObjetoResultadoDetalle resultado)
        {
            var rangos = resultado.Rangos;
            foreach (var rango in resultado.Rangos)
            {
                try
                {
                    IDbCommand objComm = db.Database.Connection.CreateCommand();
                    string filter = "";
                    db.Database.Connection.Open();
                    if (rango.Desde == null && rango.Hasta == null)
                        filter = string.Format(" WHERE DESDE = '{0}'", rango.Valor);
                    else
                        filter = string.Format(" WHERE DESDE = '{0}' AND HASTA = '{1}'", rango.Desde, rango.Hasta);
                    var query = string.Format("UPDATE MT_CONFIG_RANGO SET ICONO = :img {0}", filter);
                    objComm.CommandText = query;
                    IDataParameter par = objComm.CreateParameter();
                    par.ParameterName = "img";
                    par.DbType = DbType.Binary;
                    par.Value = rango.Icono;
                    objComm.Parameters.Add(par);
                    objComm.ExecuteNonQuery();
                }
                catch
                {
                    return Ok(false);
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }

            return Ok(true);
        }


        private int Interpolate(int start, int end, int steps, int count)
        {
            float s = start, e = end, final = s + (((e - s) / steps) * count);
            return (int)final;
        }

        private string GetOperacionTipo(long idTipoOperacion)
        {
            string sOperacionTipo = string.Empty;
            switch (idTipoOperacion)
            {
                case 1:
                    sOperacionTipo = "=";
                    break;
                case 2:
                    sOperacionTipo = "<>";
                    break;
                case 3:
                    sOperacionTipo = ">";
                    break;
                case 4:
                    sOperacionTipo = ">=";
                    break;
                case 5:
                    sOperacionTipo = "<";
                    break;
                case 6:
                    sOperacionTipo = "<=";
                    break;
                case 7:
                    sOperacionTipo = "BETWEEN";
                    break;
                case 8: //comienza con
                case 10: //termina con
                case 12: //contiene
                    sOperacionTipo = "LIKE";
                    break;
                case 9: //no comienza con
                case 11: //no termina con
                case 13: //no contiene
                    sOperacionTipo = "NOT LIKE";
                    break;
                case 14:
                    sOperacionTipo = "IS NULL";
                    break;
                case 15:
                    sOperacionTipo = "IS NOT NULL";
                    break;
                case 16:
                    sOperacionTipo = "IN";
                    break;
                case 17:
                    sOperacionTipo = "NOT IN";
                    break;
            }
            return sOperacionTipo;
        }

        private string GetAtributoCampoFuncion(Componente componente, Atributo atributo)
        {
            string campoFuncion = string.Empty;
            if (!string.IsNullOrEmpty(atributo.Campo))
            {
                campoFuncion = componente.Tabla + "." + atributo.Campo;
            }
            else
            {
                campoFuncion = atributo.Funcion.Replace("@T@", componente.Tabla);
            }
            return campoFuncion;
        }

        private string GetJoinSapTable(string tablaAtr, string esquemaComp, string tablaComp, string campoId)
        {
            StringBuilder join = new StringBuilder();
            string esquema;
            string campoAtr;
            if (tablaAtr.Equals("VW_DEUDA"))
            {
                esquema = "SAP";
                campoAtr = "CC";
                join.AppendLine(" JOIN ").Append(esquema).Append(".").Append(tablaAtr);
                join.AppendLine(" ON ")
                    .Append(esquemaComp).Append(".").Append(tablaComp).Append(".").Append(campoId)
                    .Append(" = TO_NUMBER(").Append(tablaAtr).Append(".").Append(campoAtr).Append(")");
            }
            return join.ToString();
        }

        private bool GetJerarquiaMayor(List<Jerarquia> lstJerarquiaAll, long idComponentePadre, long idComponenteInferior, ref List<Jerarquia> lstJerarquiaPadres)
        {
            bool encontroPadre = false;
            List<Jerarquia> lstJerarquia = lstJerarquiaAll.Where(p => p.ComponenteInferiorId == idComponenteInferior).ToList();
            if (lstJerarquia != null && lstJerarquia.Count > 0)
            {
                foreach (Jerarquia jerarquia in lstJerarquia)
                {
                    //Jerarquia jerarquia = lstJerarquia[0];
                    Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
                    Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
                    if (componenteJerSup.ComponenteId == idComponentePadre)
                    {
                        //lstJerarquiaPadres.Add(jerarquia);
                        encontroPadre = true;
                        //break;
                    }
                    else
                    {
                        //lstJerarquiaPadres.Add(jerarquia);
                        idComponenteInferior = componenteJerSup.ComponenteId;
                        encontroPadre = GetJerarquiaMayor(lstJerarquiaAll, idComponentePadre, idComponenteInferior, ref lstJerarquiaPadres);
                    }
                    if (encontroPadre)
                    {
                        lstJerarquiaPadres.Add(jerarquia);
                        break;
                    }
                }
            }
            return encontroPadre;
        }
        private bool GetJerarquiaMenor(List<Jerarquia> lstJerarquiaAll, long idComponenteHijo, long idComponenteSuperior, ref List<Jerarquia> lstJerarquiaHijos)
        {
            bool encontroHijo = false;
            List<Jerarquia> lstJerarquia = lstJerarquiaAll.Where(p => p.ComponenteSuperiorId == idComponenteSuperior).ToList();
            if (lstJerarquia != null && lstJerarquia.Count > 0)
            {
                foreach (Jerarquia jerarquia in lstJerarquia)
                {
                    //Jerarquia jerarquia = lstJerarquia[0];
                    Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
                    Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
                    if (componenteJerInf.ComponenteId == idComponenteHijo)
                    {
                        //lstJerarquiaHijos.Add(jerarquia);
                        encontroHijo = true;
                    }
                    else
                    {
                        //lstJerarquiaHijos.Add(jerarquia);
                        idComponenteSuperior = componenteJerSup.ComponenteId;
                        //encontroHijo = GetJerarquiaMenor(lstJerarquiaAll, idComponenteHijo, idComponenteSuperior, ref lstJerarquiaHijos);
                        //encontroHijo = GetJerarquiaMenor(lstJerarquiaAll, jerarquia.ComponenteInferiorId, idComponenteSuperior, ref lstJerarquiaHijos);
                        encontroHijo = GetJerarquiaMenor(lstJerarquiaAll, idComponenteHijo, jerarquia.ComponenteInferiorId, ref lstJerarquiaHijos);
                    }
                    if (encontroHijo)
                    {
                        lstJerarquiaHijos.Add(jerarquia);
                        break;
                    }
                }
            }
            return encontroHijo;
        }

        private string GetAgrupacionOperacion(long idAgrupacion)
        {
            string sAgrupacion = string.Empty;
            switch (idAgrupacion)
            {
                case 1:
                    //Suma
                    sAgrupacion = "SUM";
                    break;
                case 2:
                    //Promedio
                    sAgrupacion = "AVG";
                    break;
                case 3:
                    //Cantidad
                    sAgrupacion = "COUNT";
                    break;
                case 4:
                    //Minimo
                    sAgrupacion = "MIN";
                    break;
                case 5:
                    //Maximo
                    sAgrupacion = "MAX";
                    break;
            }
            return sAgrupacion;
        }

        private string GetDataValue(IDataReader data, int posicion, long idTipoDato)
        {
            string valor = string.Empty;
            try
            {
                switch (idTipoDato)
                {
                    case 3:
                        //long
                        valor = data.GetInt64(posicion).ToString();
                        break;
                    case 4:
                        //double
                        try
                        {
                            valor = data.GetDouble(posicion).ToString();
                        }
                        catch
                        {
                            valor = data.GetString(posicion);
                        }
                        break;
                    case 2:
                        //int
                        valor = data.GetInt32(posicion).ToString();
                        break;
                    case 6:
                        //string
                        valor = data.GetString(posicion);
                        break;
                    case 7:
                        //float
                        try
                        {
                            valor = data.GetFloat(posicion).ToString();
                        }
                        catch
                        {
                            valor = data.GetString(posicion);
                        }
                        break;
                    case 1:
                        //boolean
                        valor = data.GetBoolean(posicion).ToString();
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                Global.GetLogger().LogInfo(string.Format("Falla al parsear {0} como tipo de dato {1}", data.GetValue(posicion), idTipoDato));
            }
            return valor;
        }

        private double ConvertToDouble(object objeto)
        {
            double wRet = 0;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                double.TryParse(objeto.ToString().Replace(',', '.'), out wRet);
            }
            return wRet;
        }

        private IEnumerable<Models.RelacionJerarquiaIntermedia> GetJerarquia(Componente componente, long? idComponente)
        {
            bool encontro = false;
            HashSet<long> agregados = new HashSet<long>();
            foreach (var jerarquia in JerarquiasPadres(componente.ComponenteId, idComponente.GetValueOrDefault()))
            {
                encontro = true;
                if (agregados.Add(jerarquia.ComponenteInferiorId))
                {
                    var componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
                    var atributoJerInf = db.Atributo.Find(jerarquia.AtributoInferiorId);
                    var componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
                    var atributoJerSup = db.Atributo.Find(jerarquia.AtributoSuperiorId);
                    var relacion = new Models.RelacionJerarquiaIntermedia()
                    {
                        ComponenteJoin = componenteJerInf,
                        AtributoClaveJoin = atributoJerInf,
                        AtributoClaveMain = atributoJerSup
                    };
                    if (!string.IsNullOrEmpty(jerarquia.TablaRelacion))
                    {
                        var componenteRelId = $"{jerarquia.JerarquiaId}_{DateTime.Now.Ticks}".GetHashCode();
                        yield return new Models.RelacionJerarquiaIntermedia()
                        {
                            ComponenteJoin = new Componente { ComponenteId = componenteRelId, Esquema = jerarquia.EsquemaTblRel, Tabla = jerarquia.TablaRelacion },
                            AtributoClaveJoin = new Atributo { ComponenteId = componenteRelId, Campo = "ID_TABLA_PADRE" },
                            AtributoClaveMain = atributoJerSup,
                            AtributoFiltro = new Atributo { ComponenteId = componenteRelId, Campo = "TABLA_PADRE" },
                            ValorFiltro = $"'{componenteJerSup.Tabla}'"
                        };

                        relacion = new Models.RelacionJerarquiaIntermedia()
                        {
                            ComponenteJoin = componenteJerInf,
                            AtributoClaveJoin = atributoJerInf,
                            AtributoClaveMain = new Atributo { ComponenteId = componenteRelId, Campo = "ID_TABLA_HIJO" },
                            AtributoFiltro = new Atributo { ComponenteId = componenteRelId, Campo = "TABLA_HIJO" },
                            ValorFiltro = $"'{componenteJerInf.Tabla}'"
                        };
                    }
                    yield return relacion;
                }
            }
            if (!encontro)
            {
                foreach (var jerarquia in JerarquiasHijos(componente.ComponenteId, idComponente.GetValueOrDefault()))
                {
                    encontro = true;
                    if (agregados.Add(jerarquia.ComponenteSuperiorId))
                    {
                        var componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
                        var atributoJerInf = db.Atributo.Find(jerarquia.AtributoInferiorId);
                        var componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
                        var atributoJerSup = db.Atributo.Find(jerarquia.AtributoSuperiorId);
                        var relacion = new Models.RelacionJerarquiaIntermedia()
                        {
                            ComponenteJoin = componenteJerSup,
                            AtributoClaveJoin = atributoJerSup,
                            AtributoClaveMain = atributoJerInf
                        };
                        if (!string.IsNullOrEmpty(jerarquia.TablaRelacion))
                        {
                            var componenteRelId = $"{jerarquia.JerarquiaId}_{DateTime.Now.Ticks}".GetHashCode();
                            yield return new Models.RelacionJerarquiaIntermedia()
                            {
                                ComponenteJoin = new Componente { ComponenteId = componenteRelId, Esquema = jerarquia.EsquemaTblRel, Tabla = jerarquia.TablaRelacion },
                                AtributoClaveJoin = new Atributo { ComponenteId = componenteRelId, Campo = "ID_TABLA_HIJO" },
                                AtributoClaveMain = atributoJerInf,
                                AtributoFiltro = new Atributo { ComponenteId = componenteRelId, Campo = "TABLA_HIJO" },
                                ValorFiltro = $"'{componenteJerInf.Tabla}'"
                            };

                            relacion = new Models.RelacionJerarquiaIntermedia()
                            {
                                ComponenteJoin = componenteJerSup,
                                AtributoClaveJoin = atributoJerSup,
                                AtributoClaveMain = new Atributo { ComponenteId = componenteRelId, Campo = "ID_TABLA_PADRE" },
                                AtributoFiltro = new Atributo { ComponenteId = componenteRelId, Campo = "TABLA_PADRE" },
                                ValorFiltro = $"'{componenteJerSup.Tabla}'"
                            };
                        }
                        yield return relacion;
                    }
                }
            }
        }

        private IEnumerable<Jerarquia> JerarquiasPadres(long idComponentePadre, long idComponenteInferior)
        {
            bool encontro = false;
            int i = 0;
            var jerarquias = db.Jerarquia.Where(j => j.ComponenteInferiorId == idComponenteInferior).ToList();
            while (!encontro && i < jerarquias.Count)
            {
                var jerarquia = jerarquias[i++];
                encontro = jerarquia.ComponenteSuperiorId == idComponentePadre;
                if (encontro)
                {
                    yield return jerarquia;
                }
                else if (string.IsNullOrEmpty(jerarquia.EsquemaTblRel))
                {
                    foreach (var j in JerarquiasPadres(idComponentePadre, jerarquia.ComponenteSuperiorId))
                    {
                        yield return j;
                    }
                    //yield return jerarquia;
                }
            }
        }

        private IEnumerable<Jerarquia> JerarquiasHijos(long idComponenteHijo, long idComponenteSuperior)
        {
            bool encontro = false;
            int i = 0;
            var jerarquias = db.Jerarquia.Where(j => j.ComponenteSuperiorId == idComponenteSuperior).ToList();
            while (!encontro && i < jerarquias.Count)
            {
                var jerarquia = jerarquias[i++];
                encontro = jerarquia.ComponenteInferiorId == idComponenteHijo;
                if (encontro)
                {
                    yield return jerarquia;
                }
                else if (string.IsNullOrEmpty(jerarquia.EsquemaTblRel))
                {
                    foreach (var j in JerarquiasHijos(idComponenteHijo, jerarquia.ComponenteInferiorId))
                    {
                        yield return j;
                    }
                    //yield return jerarquia;
                }
            }
        }
    }
}