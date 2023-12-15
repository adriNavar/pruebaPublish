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
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Componentes;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Web.Api.Controllers
{
    public class BusquedaAvanzadaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly GeoSITMContext db = GeoSITMContext.CreateContext();

        public BusquedaAvanzadaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ValidateOnSaveEnabled = false;
        }

        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentes()
        {
            return Ok(db.Componente.Where(c => !c.EsLista && !c.EsTemporal).OrderBy(c => c.Nombre).ToList());
        }

        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentesGeograficos()
        {

            return Ok(db.Componente.Where(m => m.Graficos < 5 & m.Graficos > 0).ToList());
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetComponentesBusqueda(string id)
        {
            List<Componente> componente = new List<Componente>(db.Componente.Where(a => a.Nombre.ToLower().Contains(id.ToLower())));
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
            //return Ok(obtenerComponentesPadres(id)
            //                .Concat(obtenerComponentesHijos(id))
            //                .Concat(new[] { db.Componente.Find(id) })
            //                .OrderBy(c => c.Nombre));
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Componente>))]
        public IHttpActionResult GetParametrosGenerales()
        {
            return Ok(db.ParametrosGenerales.ToList());
        }

        private List<Componente> obtenerComponentesHijos(long idComponente)
        {
            List<Componente> componentes = new List<Componente>();

            List<Jerarquia> arbolJerarquiasPlano = obtenerJerarquiasHijas(idComponente).ToList();
            foreach (var item in arbolJerarquiasPlano)
            {
                componentes.AddRange(db.Componente.Where(a => a.ComponenteId == item.ComponenteInferiorId));
            }

            return componentes;
            //return (from jerarquia in obtenerJerarquiasHijas(idComponente)
            //        join componente in db.Componente on jerarquia.ComponenteInferiorId equals componente.ComponenteId
            //        select componente).Distinct().ToList();
        }

        public IHttpActionResult GetComponentesAgrupadores()
        {
            return Json(db.Componente.Where(c => c.Graficos != 5 && !c.EsTemporal).OrderBy(c => c.Nombre).ToList());
        }
        public IHttpActionResult GetComponentesPadres(string idComponentes)
        {
            string[] arrayComps = idComponentes.Split(',');
            var componentes = arrayComps.SelectMany(item => obtenerComponentesPadres(Convert.ToInt32(item)));
            var padres = obtenerPadresEnComun(componentes, arrayComps.Length);
            return Json(componentes.Distinct().Where(cmp => padres.Contains(cmp.ComponenteId)).ToList());
        }

        private List<long> obtenerPadresEnComun(IEnumerable<Componente> allPadres, int ocurrencias)
        {
            return allPadres.GroupBy(c => c.ComponenteId)
                            .Select(group => new { comp = group.Key, ocu = group.Count() })
                            .Where(group => group.ocu >= ocurrencias - 1)
                            .Select(group => group.comp)
                            .ToList();
        }
        private List<Componente> obtenerComponentesPadres(long idComponente)
        {
            List<Componente> componentes = new List<Componente>();

            List<Jerarquia> arbolJerarquiasPlano = obtenerJerarquiasPadres(idComponente).ToList();
            foreach (var item in arbolJerarquiasPlano)
            {
                componentes.AddRange(db.Componente.Where(a => a.ComponenteId == item.ComponenteSuperiorId));
            }

            return componentes;
            //return (from padre in obtenerJerarquiasPadres(idComponente)
            //        join componente in db.Componente on padre.ComponenteSuperiorId equals componente.ComponenteId
            //        select componente).Distinct().ToList();
        }
        private IEnumerable<Jerarquia> obtenerJerarquiasHijas(long Idjerarquia)
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
            //return db.Jerarquia
            //            .Where(j => j.ComponenteSuperiorId == Idjerarquia)
            //            .ToList()
            //            .SelectMany(j => obtenerJerarquiasPadres(j.ComponenteInferiorId));
        }
        private IEnumerable<Jerarquia> obtenerJerarquiasPadres(long Idjerarquia)
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

            return Ok(atributos.OrderBy(a => a.Nombre));
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

            if (MapaTematicoConfiguracion == null)
            {
                return NotFound();
            }
            return Ok(MapaTematicoConfiguracion);
        }

        [HttpGet]
        [ResponseType(typeof(List<ComponenteConfiguracion>))]
        public IHttpActionResult GetComponenteConfiguracionByMTId(long id)
        {
            return Ok(db.ComponenteConfiguracion
                        .Where(h => h.Configuracion.ConfiguracionId == id)
                        .Include(x => x.Configuracion)
                        .Include(x => x.Componente));
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
                                   where conf.IdConfigCategoria == 4 && conf.Visibilidad == 1 && conf.FechaBaja == null
                                   orderby comp.Nombre
                                   select compConf)
                                  .Include(x => x.Componente)
                                  .Include(x => x.Configuracion);
                return Ok(bibliotecas.ToList());
            }
            catch (Exception ex)
            {
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
                               where conf.IdConfigCategoria == 4 && conf.Visibilidad != 1 && conf.Usuario == id && conf.FechaBaja == null
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
        public IHttpActionResult GetColeccionesSoloUnComponenteByUsuario(long usuario)
        {
            var registros = from col in (from c in db.Coleccion
                                         join cc in
                                             (from cc in db.ColeccionComponente
                                              where cc.UsuarioAlta == usuario && cc.FechaBaja == null
                                              group cc by new { cc.ColeccionId, cc.ComponenteId } into g
                                              select g.Key) on c.ColeccionId equals cc.ColeccionId
                                         where c.UsuarioAlta == usuario && c.FechaBaja == null
                                         group c by c into g
                                         where g.Count() == 1
                                         select g.Key)
                            join objeto in db.ColeccionComponente on col.ColeccionId equals objeto.ColeccionId
                            where objeto.FechaBaja == null
                            group objeto by col into g
                            select new { coleccion = g.Key, cantidad = g.Count() };

            var colecciones = registros
                                    .Select(reg => new ColeccionVista()
                                    {
                                        Cantidad = reg.cantidad,
                                        ColeccionId = reg.coleccion.ColeccionId,
                                        Nombre = reg.coleccion.Nombre
                                    });

            return Ok(colecciones.ToList());
        }
        public IHttpActionResult GetColeccionesById(long id)
        {
            var coleccion = db.Coleccion.Find(id);
            return coleccion == null ? (IHttpActionResult)NotFound() : Ok(coleccion);
        }

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
        public IHttpActionResult DeleteObjetoResultado(int? dias)
        {
            int delOK = 0;
            try
            {
                if (dias == null)
                {
                    dias = dias - 2;
                }
                DateTime fechaAEliminar = DateTime.Now.AddDays(-(double)dias);
                List<ObjetoResultado> lstObjetoResultado = db.ObjetoResultados.Where(p => p.FechaAlta <= fechaAEliminar).ToList();
                db.ObjetoResultados.RemoveRange(lstObjetoResultado);
                db.SaveChanges();
                delOK = 1;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return Ok(delOK);
        }

        [HttpPost]
        public IHttpActionResult CambiarVisibilidadMapaTematicoById(long id)
        {
            var config = db.MapaTematicoConfiguracion.Find(id);
            config.Visibilidad ^= 1;
            db.Entry(config).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(config.Visibilidad);
        }

        [HttpPost]
        [ResponseType(typeof(MapaTematicoConfiguracion))]
        public IHttpActionResult GrabaMapaTematico(MapaTematicoConfiguracion mtc)
        {
            var componenteId = mtc.ComponenteConfiguracion[0].ComponenteId;
            mtc.ComponenteConfiguracion[0].Componente = db.Componente.Where(comp => comp.ComponenteId == componenteId).FirstOrDefault();
            mtc.ComponenteConfiguracion[0].Configuracion = mtc;
            if (mtc.ConfiguracionId == 0)
            {
                mtc.FechaCreacion = System.DateTime.Now;
            }
            else
            {
                var cc = db.ComponenteConfiguracion.Where(c => c.ConfiguracionId == mtc.ConfiguracionId).FirstOrDefault();
                List<ConfiguracionRango> cr = db.ConfiguracionRango.Where(c => c.ConfiguracionId == mtc.ConfiguracionId).ToList();
                List<ConfiguracionFiltro> cf = db.ConfiguracionFiltro.Where(c => c.ConfiguracionId == mtc.ConfiguracionId).ToList(); ;
                db.ConfiguracionRango.RemoveRange(cr);
                db.ConfiguracionFiltro.RemoveRange(cf);
                db.SaveChanges();

                if (cc.ComponenteId != mtc.ComponenteConfiguracion[0].ComponenteId)
                {
                    cc.ComponenteId = mtc.ComponenteConfiguracion[0].ComponenteId;
                    cc.Componente = mtc.ComponenteConfiguracion[0].Componente;
                    db.SaveChanges();
                }
                db.ConfiguracionRango.AddRange(mtc.ConfiguracionRango);
                db.ConfiguracionFiltro.AddRange(mtc.ConfiguracionFiltro);
                db.SaveChanges();
            }
            if (mtc.Nombre == null)
            {
                mtc.Nombre = "Sin Titulo " + System.DateTime.Now;
            }

            db.Entry(mtc).State = mtc.ConfiguracionId == 0 ? EntityState.Added : EntityState.Modified;
            db.SaveChanges();
            return Ok(mtc);
        }

        [HttpPost]
        [ResponseType(typeof(MapaTematicoConfiguracion))]
        public IHttpActionResult GrabaBusquedaAvanzada(MapaTematicoConfiguracion mtc)
        {
            foreach (var item in mtc.ConfiguracionFiltro)
            {
                item.FiltroOperacion = item.FiltroOperacion == 0 ? null : item.FiltroOperacion;
            }

            foreach (var item in mtc.ComponenteConfiguracion)
            {
                ComponenteConfiguracion cf = new ComponenteConfiguracion();
                var componenteId = item.ComponenteId;
                cf.Componente = db.Componente.Where(comp => comp.ComponenteId == componenteId).FirstOrDefault();
                item.Componente = cf.Componente;
                item.Configuracion = mtc;

            }
            //mtc.ComponenteConfiguracion[0].Componente = db.Componente.Where(comp => comp.ComponenteId == componenteId).FirstOrDefault();
            mtc.ComponenteConfiguracion[0].Configuracion = mtc;
            mtc.IdConfigCategoria = 4;//categoria de busquedaAvanzada
            if (mtc.ConfiguracionId == 0)
            {
                mtc.FechaCreacion = DateTime.Now;
            }
            else
            {
                var cc = db.ComponenteConfiguracion.Where(c => c.ConfiguracionId == mtc.ConfiguracionId).FirstOrDefault();
                var cr = db.ConfiguracionRango.Where(c => c.ConfiguracionId == mtc.ConfiguracionId).ToList();
                var cf = db.ConfiguracionFiltro.Where(c => c.ConfiguracionId == mtc.ConfiguracionId).ToList(); ;
                db.ConfiguracionRango.RemoveRange(cr);
                db.ConfiguracionFiltro.RemoveRange(cf);
                db.SaveChanges();

                if (cc.ComponenteId != mtc.ComponenteConfiguracion[0].ComponenteId)
                {
                    cc.ComponenteId = mtc.ComponenteConfiguracion[0].ComponenteId;
                    cc.Componente = mtc.ComponenteConfiguracion[0].Componente;
                    db.SaveChanges();
                }
                db.ConfiguracionRango.AddRange(mtc.ConfiguracionRango);
                db.ConfiguracionFiltro.AddRange(mtc.ConfiguracionFiltro);
                db.SaveChanges();
            }
            mtc.Nombre = mtc.Nombre ?? $"Sin Titulo {DateTime.Now}";


            db.Entry(mtc).State = mtc.ConfiguracionId == 0 ? EntityState.Added : EntityState.Modified;
            db.SaveChanges();
            return Ok(mtc);
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

            db.Entry(mtc).State = EntityState.Added;
            //db.Entry(mtc).State = mtc.ConfiguracionId == 0 ? EntityState.Added : EntityState.Modified;
            //db.Entry(mtc).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(mtc);
        }

        [HttpPost]
        [ResponseType(typeof(ObjetoResultadoDetalle))]
        public IHttpActionResult GenerarResultadoMapaTematico(RequestObjetoResultado requestObjetoResultado)
        {
            return InternalServerError();
            //ObjetoResultadoDetalle objetoResultadoDetalle = null;
            //try
            //{

            //    Componente componenteFull = db.Componente.Find(requestObjetoResultado.Componente.ComponenteId);
            //    db.Entry(componenteFull).Collection(a => a.Atributos).Load();

            //    Atributo atributoFull = null;
            //    if (requestObjetoResultado.EsImportado)
            //    {
            //        atributoFull = requestObjetoResultado.Atributo;
            //        atributoFull.Componente = db.Componente.Find(atributoFull.ComponenteId);
            //        db.Entry(atributoFull.Componente).Collection(a => a.Atributos).Load();
            //    }
            //    else
            //    {
            //        atributoFull = db.Atributo.Find(requestObjetoResultado.Atributo.AtributoId);
            //        atributoFull.Componente = db.Componente.Find(atributoFull.ComponenteId);
            //    }
            //    bool graboOK = GenerarObjetoResultado(requestObjetoResultado.GUID, componenteFull, atributoFull, requestObjetoResultado.ConfiguracionFiltros, requestObjetoResultado.MapaTematicoConfiguracion, requestObjetoResultado.EsImportado);
            //    if (graboOK)
            //    {
            //        List<ObjetoResultado> lstObjetoResultado = db.ObjetoResultados.Where(p => p.GUID == requestObjetoResultado.GUID).ToList();
            //        List<string> lstObjetoResultadoDistinctValor = lstObjetoResultado.Select(p => p.Valor).Distinct().ToList();
            //        long distribucion = requestObjetoResultado.MapaTematicoConfiguracion.Distribucion;
            //        //distribucion = 3;
            //        if (distribucion == 0)
            //        {
            //            double calculo = Convert.ToDouble(lstObjetoResultadoDistinctValor.Count) / Convert.ToDouble(lstObjetoResultado.Count);
            //            ParametrosGenerales parametroGeneral = db.ParametrosGenerales.FirstOrDefault(p => p.Descripcion == "Distribucion");
            //            if (parametroGeneral != null)
            //            {
            //                double distribucionParam = Convert.ToDouble(parametroGeneral.Valor);
            //                if (calculo >= 0.1)
            //                {
            //                    distribucion = 1;
            //                }
            //                else
            //                {
            //                    if (lstObjetoResultadoDistinctValor.Count <= 100)
            //                    {
            //                        distribucion = 3;
            //                    }
            //                    else
            //                    {
            //                        distribucion = 1;
            //                    }
            //                }
            //            }
            //        }
            //        objetoResultadoDetalle = GetRangos(requestObjetoResultado.GUID, distribucion, requestObjetoResultado.MapaTematicoConfiguracion.Rangos);
            //    }
            //    else
            //    {
            //        return NotFound();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string msgErr = ex.Message;
            //}
            //return Ok(objetoResultadoDetalle);
        }

        [HttpPost]
        public IHttpActionResult GrabarColeccion(Models.ColeccionModel coleccion)
        {
            try
            {
                DateTime fechaActual = System.DateTime.Now;
                Coleccion colec = new Coleccion();
                colec.Nombre = coleccion.Nombre;
                colec.UsuarioAlta = coleccion.UsuarioId;
                colec.UsuarioModificacion = coleccion.UsuarioId;
                colec.FechaAlta = fechaActual;
                colec.FechaModificacion = fechaActual;
                colec.Componentes = new List<ColeccionComponente>();

                foreach (var item in coleccion.Objetos)
                {
                    colec.Componentes.Add(new ColeccionComponente()
                    {
                        ObjetoId = item.ObjetoId,
                        ComponenteId = item.ComponenteID,
                        UsuarioAlta = coleccion.UsuarioId,
                        FechaAlta = fechaActual
                    });
                }

                _unitOfWork.ColeccionRepository.GuardarColeccion(colec);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();
        }

        [HttpGet]
        [ResponseType(typeof(string))]
        public IHttpActionResult BuscarAyuda()
        {
            return Ok(Convert.ToBase64String(File.ReadAllBytes(ConfigurationManager.AppSettings["HelpFileBA"])));
        }

        [HttpGet]
        [ResponseType(typeof(int))]
        public IHttpActionResult CalcularCasos(string guid, long distribucion, long desde, long hasta)
        {
            int casos = 0;
            try
            {
                List<ObjetoResultado> lstObjetoResultado = db.ObjetoResultados.Where(p => p.GUID == guid).ToList();
                if (lstObjetoResultado != null && lstObjetoResultado.Count > 0)
                {
                    if (distribucion == 1 || distribucion == 2)
                    {
                        //Uniforme o Cuantiles
                        casos = lstObjetoResultado.Count(p => Convert.ToDouble(p.Valor.Replace(',', '.')) >= desde && Convert.ToDouble(p.Valor.Replace(',', '.')) < hasta);
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return Ok(casos);
        }

        private bool GetJerarquiaMayor(List<Jerarquia> lstJerarquiaAll, long idComponentePadre, long idComponenteInferior, ref List<Jerarquia> lstJerarquiaPadres)
        {
            bool encontroPadre = false;
            List<Jerarquia> lstJerarquia = lstJerarquiaAll.Where(p => p.ComponenteInferiorId == idComponenteInferior).ToList();
            if (lstJerarquia != null && lstJerarquia.Count > 0)
            {
                Jerarquia jerarquia = lstJerarquia[0];
                Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
                Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
                if (componenteJerSup.ComponenteId == idComponentePadre)
                {
                    lstJerarquiaPadres.Add(jerarquia);
                    encontroPadre = true;
                }
                else
                {
                    lstJerarquiaPadres.Add(jerarquia);
                    idComponenteInferior = componenteJerSup.ComponenteId;
                    encontroPadre = GetJerarquiaMayor(lstJerarquiaAll, idComponentePadre, idComponenteInferior, ref lstJerarquiaPadres);
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
                Jerarquia jerarquia = lstJerarquia[0];
                Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
                Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
                if (componenteJerInf.ComponenteId == idComponenteHijo)
                {
                    lstJerarquiaHijos.Add(jerarquia);
                    encontroHijo = true;
                }
                else
                {
                    lstJerarquiaHijos.Add(jerarquia);
                    idComponenteSuperior = componenteJerSup.ComponenteId;
                    encontroHijo = GetJerarquiaMenor(lstJerarquiaAll, idComponenteHijo, jerarquia.ComponenteInferiorId, ref lstJerarquiaHijos);
                }
            }
            return encontroHijo;
        }

        //Porque devuelve una lista de listas ?
        [HttpPost]
        [ResponseType(typeof(List<Models.ObjetoModel>))]
        public IHttpActionResult ResultadoBusquedaAvanzada(Models.BusquedaAvanzadaModel BA)
        {
            if (BA.Agrupamiento == 0)
            {
                return Json(BA.Componentelist.SelectMany(cmp => ObtenerResultadoBusqueda(BA.lstConfiguracionFiltro, cmp)).ToArray());
                //return Json(BA.Componentelist.SelectMany(cmp => ObtenerObjetoModelSinAgrupamiento(BA.lstConfiguracionFiltro, cmp)));
            }
            else
            {
                #region Datos Componente Agrupador
                var componenteAgrupador = GetComponenteById(BA.AgrupIdComponente);
                componenteAgrupador.Atributos = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(componenteAgrupador.ComponenteId).ToArray();
                #endregion
                return Json(BA.Componentelist.SelectMany(cmp => ObtenerResultadoBusqueda(BA.lstConfiguracionFiltro, cmp, componenteAgrupador, BA.OperacionAgrup)).ToArray());
                //return Json(BA.Componentelist.SelectMany(cmp => ObtenerObjetoModelConAgrupamiento(BA.lstConfiguracionFiltro, cmp, componenteAgrupador, BA.OperacionAgrup)));
            }
            ////Si en algun momento se crea un modulo query x ambito, descomentar el metodo comentado.
            //List<Models.ObjetoModel> listaRetorno = new List<Models.ObjetoModel>();
            //if (BA.Agrupamiento == 0)
            //{
            //    # region Sin agrupamiento
            //    foreach (var componente in BA.Componentelist)
            //    {
            //        listaRetorno.AddRange(ObjetenerObjetoModelSinAgrupamiento(BA.lstConfiguracionFiltro, componente));
            //    }
            //    #endregion
            //}
            //else
            //{
            //    #region Con agrupamiento
            //    foreach (var componente in BA.Componentelist)
            //    {
            //        listaRetorno.AddRange(ObjetenerObjetoModelConAgrupamiento(BA.Filtros, BA.lstConfiguracionFiltro, componente, BA.AgrupIdComponente, BA.Agrupamiento, BA.OperacionAgrup));
            //    }
            //    #endregion
            //}
            //return Json(listaRetorno);
        }
        //private List<Models.ObjetoModel> ObtenerObjetoModelSinAgrupamiento(List<Models.ConfiguracionFiltro> lstConfigFiltro, Models.BusquedaAvanzadaModel.ComponenteModel componente)
        //{
        //    try
        //    {
        //        using (var builder = ObtenerSQLBuilder(lstConfigFiltro, componente))
        //        {
        //            return builder.ExecuteQuery((IDataReader reader) =>
        //            {
        //                return new Models.ObjetoModel()
        //                {
        //                    CompAgrupador = new Componente() { DocType = componente.DocType, Capa = componente.Capa, Descripcion = componente.Nombre },
        //                    ObjetoId = reader.GetNullableInt64(0).Value,
        //                    ComponenteID = componente.ComponenteId,
        //                    Descripcion = reader.GetStringOrEmpty(1)
        //                };
        //            })
        //            .OrderBy(x => x.Descripcion)
        //            .ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Global.GetLogger().LogError("ObtenerObjetoModelSinAgrupamiento", ex);
        //        throw new Exception(ex.Message, ex);
        //    }
        //}
        //private List<Models.ObjetoModel> ObtenerObjetoModelConAgrupamiento(List<Models.ConfiguracionFiltro> lstConfigFiltro, Models.BusquedaAvanzadaModel.ComponenteModel componente, Componente componenteAgrupador, string OperacionAgrupacion)
        //{
        //    try
        //    {
        //        using (var builder = ObtenerSQLBuilder(lstConfigFiltro, componente))
        //        {
        //            int datosAgrupadorOffset = 0;
        //            if (componente.ComponenteId != componenteAgrupador.ComponenteId)
        //            {
        //                Componente componenteTablaGrafica = null;
        //                Atributo campoIdAgrupador = null;
        //                Atributo campoLabelAgrupador = null;
        //                Atributo campoFeatIdAgrupador = null;
        //                Atributo campoGeometryAgrupador = null;
        //                try
        //                {
        //                    componenteTablaGrafica = new Componente
        //                    {
        //                        ComponenteId = componente.ComponenteId,
        //                        Esquema = componente.Esquema,
        //                        Tabla = componente.TablaGrafica ?? componente.Tabla
        //                    };
        //                    campoIdAgrupador = componenteAgrupador.Atributos.GetAtributoClave();
        //                    campoLabelAgrupador = componenteAgrupador.Atributos.GetAtributoLabel();
        //                    campoFeatIdAgrupador = componenteAgrupador.Atributos.GetAtributoFeatId();
        //                    campoGeometryAgrupador = componenteAgrupador.Atributos.GetAtributoGeometry();
        //                }
        //                catch (Exception ex)
        //                {
        //                    Global.GetLogger().LogError("Componente Agrupador (id: " + componenteAgrupador.ComponenteId + ") mal configurado.", ex);
        //                    throw;
        //                }
        //                builder.AddTable(componenteAgrupador, "agrupador")
        //                       .AddFields(campoIdAgrupador, campoLabelAgrupador);

        //                if (componenteAgrupador.Tabla != componenteTablaGrafica.Tabla)
        //                {
        //                    builder.AddTable(componenteTablaGrafica, "agrupadorGraf")
        //                           .AddJoinFilter("agrupador", campoFeatIdAgrupador, "agrupadorGraf", campoFeatIdAgrupador);
        //                }
        //                builder.AddTableFilter()
        //                datosAgrupadorOffset = 2;
        //            }
        //            return builder.ExecuteQuery((IDataReader reader) =>
        //            {
        //                return new Models.ObjetoModel()
        //                {
        //                    ObjetoId = reader.GetNullableInt64(0).Value,
        //                    ComponenteID = componente.ComponenteId,
        //                    Descripcion = reader.GetStringOrEmpty(1),
        //                    CompAgrupador = new Componente()
        //                    {
        //                        DocType = componente.DocType,
        //                        Capa = componente.Capa,
        //                        Descripcion = componente.Nombre,
        //                        ComponenteId = reader.GetNullableInt64(datosAgrupadorOffset).Value,
        //                        Nombre = reader.GetStringOrEmpty(datosAgrupadorOffset + 1)
        //                    }
        //                };
        //            })
        //            .OrderBy(x => x.CompAgrupador?.Descripcion)
        //            .ThenBy(x => x.Descripcion)
        //            .ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Global.GetLogger().LogError("ObtenerObjetoModelConAgrupamiento", ex);
        //        throw new Exception(ex.Message, ex);
        //    }
        //}

        private List<Models.ObjetoModel> ObtenerResultadoBusqueda(IEnumerable<Models.ConfiguracionFiltro> lstConfigFiltro, Models.BusquedaAvanzadaModel.ComponenteModel componente, Componente componenteAgrupador = null, string OperacionAgrupacion = null)
        {
            try
            {
                SQLSpatialRelationships getSpatialRelationships(Models.ConfiguracionFiltro filtro)
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
                using (var builder = db.CreateSQLQueryBuilder())
                {
                    #region Datos Básicos
                    Componente componentePrincipal = null;
                    Componente componenteTablaGrafica = null;
                    Atributo campoLabel = null;
                    Atributo campoGeometry = null;
                    Atributo campoId = null;
                    bool esComponenteGrafico = componente.Graficos != 5;

                    var atributos = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(componente.ComponenteId);
                    try
                    {
                        campoId = atributos.GetAtributoClave();
                        campoLabel = atributos.GetAtributoLabel();
                        componentePrincipal = new Componente
                        {
                            ComponenteId = componente.ComponenteId,
                            Esquema = componente.Esquema,
                            Tabla = componente.Tabla
                        };
                        componenteTablaGrafica = new Componente
                        {
                            ComponenteId = componente.ComponenteId * -1,
                            Esquema = componente.Esquema,
                            Tabla = componente.TablaGrafica ?? componente.Tabla
                        };
                        if (esComponenteGrafico)
                        {
                            campoGeometry = atributos.GetAtributoGeometry();
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", ex);
                        throw;
                    }
                    #endregion
                    #region Datos Agrupador
                    bool agrupa = componenteAgrupador != null;
                    int datosAgrupadorOffset = 0;
                    Componente componenteAgrupadorTablaGrafica = null;
                    Atributo campoIdAgrupador = null;
                    Atributo campoLabelAgrupador = null;
                    Atributo campoFeatIdAgrupador = null;
                    Atributo campoGeometryAgrupador = null;
                    if (agrupa && componente.ComponenteId != componenteAgrupador.ComponenteId)
                    {
                        datosAgrupadorOffset = 2;
                        try
                        {
                            componenteAgrupadorTablaGrafica = new Componente
                            {
                                ComponenteId = componenteAgrupador.ComponenteId * -1,
                                Esquema = componenteAgrupador.Esquema,
                                Tabla = componenteAgrupador.TablaGrafica ?? componenteAgrupador.Tabla
                            };
                            campoIdAgrupador = componenteAgrupador.Atributos.GetAtributoClave();
                            campoLabelAgrupador = componenteAgrupador.Atributos.GetAtributoLabel();
                            campoFeatIdAgrupador = componenteAgrupador.Atributos.GetAtributoFeatId();
                            campoGeometryAgrupador = componenteAgrupador.Atributos.GetAtributoGeometry();
                        }
                        catch (Exception ex)
                        {
                            Global.GetLogger().LogError("Componente Agrupador (id: " + componenteAgrupador.ComponenteId + ") mal configurado.", ex);
                            throw;
                        }
                    }
                    #endregion
                    #region Armado de consulta
                    builder.AddTable(componentePrincipal, componentePrincipal.Tabla)
                           .AddFields(campoId, campoLabel);

                    if (esComponenteGrafico && componentePrincipal.Tabla != componenteTablaGrafica.Tabla)
                    {
                        var campoIdTG = new Atributo { ComponenteId = componenteTablaGrafica.ComponenteId, Campo = campoId.Campo, TipoDatoId = campoId.TipoDatoId };
                        builder.AddJoin(componenteTablaGrafica, componenteTablaGrafica.Tabla, campoIdTG, campoId);
                    }
                    bool negated = false;
                    SQLConnectors connector = SQLConnectors.None;
                    var agregados = new HashSet<long>();
                    foreach (var filtro in lstConfigFiltro)
                    {
                        switch (filtro.FiltroTipo)
                        {
                            case 1: //filtro alfa
                                {
                                    var tipoOperacion = db.TipoOperacion.Find(filtro.FiltroOperacion);
                                    var atributoFiltro = db.Atributo.Find(filtro.FiltroAtributo);
                                    var componenteFiltro = db.Componente.Find(filtro.FiltroComponente);

                                    if (filtro.FiltroComponente != componente.ComponenteId)
                                    {
                                        #region Si filtro no es del componente buscado
                                        foreach (var relacion in GetJerarquia(componente, filtro))
                                        {
                                            builder.AddJoin(relacion.ComponenteJoin, relacion.ComponenteJoin.Tabla, relacion.AtributoClaveJoin, relacion.AtributoClaveMain);
                                            if (relacion.AtributoFiltro != null)
                                            {
                                                builder.AddTableFilter(relacion.AtributoFiltro, relacion.ValorFiltro, SQLOperators.EqualsTo);
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
                                            builder.AddJoin(datosListado.compParent, datosListado.compParent.Tabla,
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
                                            builder.AddFilter(atributoFiltro, filtro.Valor1, tipoOperacion, connector, negated);
                                        }
                                        else if (tipoOperacion.CantidadValores == 2)
                                        {
                                            builder.BeginFilterGroup(connector, negated)
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
                                                var geometry = builder.CreateGeometryFieldBuilder(coords.First(), SRID.DB);
                                                var geomTabla = builder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla);
                                                if (filtro.Fuera != 1)
                                                {
                                                    builder.BeginFilterGroup(connector, negated);
                                                    if (filtro.Ampliar.GetValueOrDefault() != 0L)
                                                    {//tiene buffer, positivo o negativo
                                                        geometry.AddBuffer(filtro.Ampliar.GetValueOrDefault());
                                                    }
                                                    builder.AddFilter(geomTabla, geometry, getSpatialRelationships(filtro));
                                                    if (filtro.PorcentajeInterseccion.GetValueOrDefault() > 0L)
                                                    {
                                                        var overlapedGeom = builder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla)
                                                                                   .OverlappingArea(geometry);

                                                        builder.AddRawFilter(string.Format("({0} / {1})", overlapedGeom, geomTabla.Area()),
                                                                            filtro.PorcentajeInterseccion.GetValueOrDefault() / 100d,
                                                                            SQLOperators.GreaterThan, SQLConnectors.And);
                                                    }
                                                    builder.EndFilterGroup();
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
                                            builder.AddTable(componenteFGAAlfa, componenteFGAAlfa.Tabla);
                                            bool abreGrupoJoin = componenteFGAAlfa.Tabla != componenteFGAGraf.Tabla;
                                            if (abreGrupoJoin)
                                            {
                                                builder.AddTable(componenteFGAGraf, componenteFGAGraf.Tabla)
                                                       .BeginFilterGroup(connector)
                                                       .AddJoinFilter(componenteFGAAlfa.Tabla, campoFeatIdFGA, componenteFGAGraf.Tabla, campoFeatIdFGA);
                                                connector = SQLConnectors.And;
                                            }
                                            var geomTablaGraf = builder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla);
                                            var geomTablaFiltro = builder.CreateGeometryFieldBuilder(campoGeometryFGA, componenteFGAGraf.Tabla);

                                            builder.BeginFilterGroup(connector, negated);
                                            builder.AddFilter(geomTablaGraf, geomTablaFiltro, getSpatialRelationships(filtro));
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
                                                    builder.AddFilter(atributoFGA, filtro.Valor1, tipoOperacionFGA, SQLConnectors.And);
                                                }
                                                else if (tipoOperacionFGA.CantidadValores == 2)
                                                {
                                                    builder.BeginFilterGroup(SQLConnectors.And)
                                                           .AddFilter(atributoFGA, filtro.Valor1, SQLOperators.GreaterThan | SQLOperators.EqualsTo)
                                                           .AddFilter(atributoFGA, filtro.Valor2, SQLOperators.LowerThan | SQLOperators.EqualsTo, SQLConnectors.And)
                                                           .EndFilterGroup();
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(filtro.Valor1))
                                            { //Es atributo con valor fijo
                                                builder.AddFilter(atributoFGA, filtro.Valor1, SQLOperators.EqualsTo, SQLConnectors.And);
                                            }
                                            builder.EndFilterGroup();
                                            if (abreGrupoJoin)
                                            {
                                                builder.EndFilterGroup();
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                break;
                            case 3: //filtro colección
                                {
                                    long idComponente = db.ColeccionComponente.First(cc => cc.ColeccionId == filtro.FiltroColeccion.Value).ComponenteId;
                                    var componenteColeccion = componentePrincipal;
                                    var campoIdComponenteColeccion = campoId;
                                    var campoGeometryComponenteColeccion = campoGeometry;
                                    if (componente.ComponenteId != idComponente)
                                    {
                                        componenteColeccion = _unitOfWork.ComponenteRepository.GetComponenteById(idComponente);
                                        if (componenteColeccion.Graficos != 5)
                                        {
                                            var atributosComponenteColeccion = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(idComponente);
                                            campoGeometryComponenteColeccion = atributosComponenteColeccion.GetAtributoGeometry();
                                            campoIdComponenteColeccion = atributosComponenteColeccion.GetAtributoClave();
                                        }
                                    }
                                    if (componente.ComponenteId == componenteColeccion.ComponenteId || componenteColeccion.Graficos != 5)
                                    {
                                        long tempId = filtro.FiltroColeccion.Value * -1;
                                        string aliasTablaColecComp = $"col_{filtro.FiltroColeccion}";
                                        string aliasTablaComp = componentePrincipal.Tabla;
                                        builder.AddTable(new Componente() { Esquema = ConfigurationManager.AppSettings["DATABASE"], Tabla = "GE_COLEC_COMP", ComponenteId = tempId }, aliasTablaColecComp)
                                               .BeginFilterGroup(connector);

                                        connector = SQLConnectors.None;
                                        if (componente.ComponenteId != componenteColeccion.ComponenteId)
                                        {
                                            aliasTablaComp = $"tblrel_{filtro.FiltroColeccion}";
                                            builder.AddTable(componenteColeccion.Esquema, componenteColeccion.TablaGrafica ?? componenteColeccion.Tabla, aliasTablaComp)
                                                   .AddFilter(builder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla),
                                                              builder.CreateGeometryFieldBuilder(campoGeometryComponenteColeccion, aliasTablaComp),
                                                              SQLSpatialRelationships.AnyInteract);
                                            connector = SQLConnectors.And;
                                        }
                                        builder.AddJoinFilter(aliasTablaColecComp, new Atributo() { ComponenteId = tempId, Campo = "ID_OBJETO" }, aliasTablaComp, campoIdComponenteColeccion)
                                               .BeginFilterGroup(connector, negated)
                                               .AddFilter(new Atributo() { ComponenteId = tempId, Campo = "ID_COLECCION" }, filtro.FiltroColeccion, SQLOperators.EqualsTo)
                                               .AddFilter(new Atributo() { ComponenteId = tempId, Campo = "ID_COMPONENTE" }, componenteColeccion.ComponenteId, SQLOperators.EqualsTo, SQLConnectors.And)
                                               .AddFilter(new Atributo() { ComponenteId = tempId, Campo = "FECHA_BAJA" }, null, SQLOperators.IsNull, SQLConnectors.And)
                                               .EndFilterGroup()
                                               .EndFilterGroup();
                                    }
                                }
                                break;
                            case 4: //operadores y paréntesis
                                {
                                    if (filtro.Valor2 == "1")
                                    {
                                        connector = SQLConnectors.And;
                                    }
                                    else if (filtro.Valor2 == "2")
                                    {
                                        connector = SQLConnectors.Or;
                                    }
                                    else if (filtro.Valor2 == "3")
                                    {
                                        builder.BeginFilterGroup(connector, negated);
                                        negated = false;
                                        connector = SQLConnectors.None;
                                    }
                                    else if (filtro.Valor2 == "4")
                                    {
                                        builder.EndFilterGroup();
                                    }
                                    else if (filtro.Valor2 == "5")
                                    {
                                        negated = true;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (agrupa && componente.ComponenteId != componenteAgrupador.ComponenteId)
                    {
                        string aliasGrafAG = "agrupador";
                        builder.AddTable(componenteAgrupador, aliasGrafAG)
                               .AddFields(campoIdAgrupador, campoLabelAgrupador);

                        if (componenteAgrupador.Tabla != componenteAgrupadorTablaGrafica.Tabla)
                        {
                            string mainAliasAG = aliasGrafAG = "agrupadorGraf";
                            builder.AddTable(componenteAgrupadorTablaGrafica, aliasGrafAG)
                                   .AddJoinFilter(mainAliasAG, campoFeatIdAgrupador, aliasGrafAG, new Atributo { ComponenteId = componenteAgrupadorTablaGrafica.ComponenteId, Campo = campoFeatIdAgrupador.Campo });
                        }
                        SQLSpatialRelationships relationships = OperacionAgrupacion == "Dentro"
                                                                    ? SQLSpatialRelationships.Contains | SQLSpatialRelationships.Covers | SQLSpatialRelationships.Equal
                                                                    : SQLSpatialRelationships.AnyInteract;
                        builder.AddTableFilter(builder.CreateGeometryFieldBuilder(campoGeometryAgrupador, aliasGrafAG),
                                               builder.CreateGeometryFieldBuilder(campoGeometry, componenteTablaGrafica.Tabla),
                                               relationships);
                    }
                    #endregion
                    #region Ejecución de consulta
                    return builder.Distinct().ExecuteQuery((IDataReader reader) =>
                    {
                        return new Models.ObjetoModel()
                        {
                            ObjetoId = reader.GetNullableInt64(0).Value,
                            ComponenteID = componente.ComponenteId,
                            Descripcion = reader.GetStringOrEmpty(1),
                            CompAgrupador = new Componente()
                            {
                                DocType = componente.DocType,
                                Capa = componente.Capa,
                                Descripcion = componente.Nombre,
                                ComponenteId = agrupa ? reader.GetNullableInt64(datosAgrupadorOffset).Value : 0,
                                Nombre = agrupa ? reader.GetStringOrEmpty(datosAgrupadorOffset + 1) : null
                            }
                        };
                    })
                    .OrderBy(x => x.CompAgrupador?.Descripcion)
                    .ThenBy(x => x.Descripcion)
                    .ToList();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ResultadoBusquedaAvanzada", ex);
                throw new Exception(ex.Message, ex);
            }
        }

        //public List<Models.ObjetoModel> ObtenerObjetoModelConAgrupamiento(List<Models.BusquedaAvanzadaModel.FiltroModel> filtros, List<Models.ConfiguracionFiltro> lstConfigFiltro, Models.BusquedaAvanzadaModel.ComponenteModel componente, long idCompAgrupador, long agrupamiento, string OperacionAgrup)
        //{
        //    var result = new List<Models.ObjetoModel>();
        //    bool ejecutaQuery = false;
        //    try
        //    {
        //        #region Variables

        //        var componenteAgrupador = GetComponenteById(idCompAgrupador);

        //        GetAtributos(componenteAgrupador);
        //        string campoIdAgrupador = componenteAgrupador.Atributos.GetAtributoClave().Campo;
        //        string campoLabelAgrupador = componenteAgrupador.Atributos.GetAtributoLabel().Campo;

        //        string agruparJoin = string.Empty;
        //        string esquemaAgrupador = componenteAgrupador.Esquema;
        //        string tablaAgrupador = componenteAgrupador.Tabla;
        //        string tablaGraficaAgrupador = componenteAgrupador.TablaGrafica ?? tablaAgrupador;
        //        string campoGeometryAgrupador = componenteAgrupador.Atributos.GetAtributoGeometry().Campo;


        //        string esquema = componente.Esquema;
        //        string tabla = componente.Tabla;
        //        string tablaGrafica = componente.TablaGrafica ?? tabla;
        //        string docType = componente.Nombre;

        //        string campoFeatId = string.Empty;
        //        string campoLabel = string.Empty;
        //        string campoGeometry = string.Empty;
        //        string campoId = string.Empty;
        //        string campoValor = string.Empty;
        //        string sqlWhere = string.Empty;

        //        //var lstCommaSeparatedJoins = new List<string>();
        //        string sJoin = string.Empty;
        //        string sCond = string.Empty;

        //        List<string> lstSelect = new List<string>();
        //        List<string> lstForm = new List<string>();
        //        List<string> lstJoin = new List<string>();
        //        List<string> lstCondiciones = new List<string>();


        //        try
        //        {
        //            var atributos = db.Atributo.Where(a => a.ComponenteId == componente.ComponenteId);
        //            campoId = atributos.GetAtributoClave().Campo;
        //            campoLabel = atributos.GetAtributoLabel().Campo;
        //            if (componente.Graficos != 5) campoGeometry = atributos.GetAtributoGeometry().Campo;
        //        }
        //        catch (Exception ex)
        //        {
        //            Global.GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", ex);
        //            throw;
        //        }
        //        #endregion

        //        #region Armado Query


        //        //SOLO EL SELECT CONCATENAR JOINS

        //        //Voy a probar eliminando el tag


        //        if (componente.Tabla != tablaAgrupador)
        //        {
        //            lstSelect.Add(string.Format("{0}.{1}", tablaAgrupador, campoIdAgrupador));
        //            lstSelect.Add(string.Format("{0}.{1}", tabla, campoId));
        //            lstSelect.Add(string.Format("{0}.{1}", tabla, campoLabel));
        //            lstSelect.Add(string.Format("{0}.{1}", tablaAgrupador, campoLabelAgrupador));

        //            lstForm.Add(string.Format("{0}.{1}", esquemaAgrupador, tablaAgrupador));
        //            lstForm.Add(string.Format("{0}.{1}", esquema, tabla));//Tabla del componente que se busca. 
        //        }
        //        else
        //        {
        //            lstSelect.Add(string.Format("{0}.{1}", tablaAgrupador, campoIdAgrupador));
        //            lstSelect.Add(string.Format("{0}.{1}", tabla, campoId));
        //            lstSelect.Add(string.Format("{0}.{1}", tabla, campoLabel));
        //            lstSelect.Add(string.Format("{0}.{1}", tablaAgrupador, campoLabelAgrupador));

        //            lstForm.Add(string.Format("{0}.{1}", esquema, tabla));//Tabla del componente que se busca.
        //        }


        //        /*if (tablaAgrupador != tablaGraficaAgrupador)
        //        {
        //            lstCommaSeparatedJoins.Add(string.Format("{0}.{1}", esquemaAgrupador, tablaAgrupador));
        //            lstCondiciones.Add(string.Format("{0}.{1}={2}.{1}", tablaGraficaAgrupador, campoIdAgrupador, tablaAgrupador));
        //        }*/
        //        if (tablaGrafica != tabla)
        //        {
        //            lstJoin.Add(string.Format(" JOIN {0}.{1} ON {2}.{3}={1}.{3}", esquema, tablaGrafica, tabla, campoId));
        //        }

        //        string operatorGeo = OperacionAgrup;
        //        switch (operatorGeo)
        //        {
        //            case "Dentro":
        //                operatorGeo = "CONTAINS+COVERS+EQUAL";
        //                break;
        //            default:
        //                operatorGeo = "ANYINTERACT";
        //                break;
        //        }
        //        //Cambio el tag de lado, creo que hace la consulta mas rapido.
        //        lstCondiciones.Add(string.Format("MDSYS.SDO_RELATE({2}.{3}, {0}.{1},'MASK={4} QUERYTYPE=WINDOW')='TRUE' ", tablaGrafica, campoGeometry, tablaGraficaAgrupador, campoGeometryAgrupador, operatorGeo));
        //        //lstCondiciones.Add(string.Format("MDSYS.SDO_RELATE({0}.{1},{2}.{3},'MASK={4} QUERYTYPE=WINDOW')='TRUE'", tablaGrafica, campoGeometry, tablaGraficaAgrupador, campoGeometryAgrupador, operatorGeo));


        //        string sSql = string.Empty;
        //        #endregion


        //        #region Sin Filtro
        //        if (lstConfigFiltro.Count == 0)
        //        {//Creo un filtro sin condiciones

        //            Models.ConfiguracionFiltro configFiltro = new Models.ConfiguracionFiltro();

        //            configFiltro.FiltroTipo = 5;


        //            lstConfigFiltro.Add(configFiltro);
        //        }
        //        #endregion

        //        for (int i = index; i < lstConfigFiltro.Count; i++)//recorro los filtros configurados
        //        {
        //            //Diferente a sin coleccion
        //            #region Filtros
        //            Models.ConfiguracionFiltro configuracionFiltro = lstConfigFiltro[i];
        //            sSql = string.Empty;
        //            if (configuracionFiltro.FiltroTipo == 1)
        //            {
        //                //28-2-19
        //                //Revisar la forma en la que relaciona los componentes.
        //                #region Filtro por Atributo
        //                if (filtros[i].FiltroComponente == componente.ComponenteId)
        //                {
        //                    #region Componente filtro es componente busqueda
        //                    Atributo atributoFiltroFull = db.Atributo.Find(configuracionFiltro.FiltroAtributo);
        //                    Componente componenteFiltroFull = db.Componente.Find(configuracionFiltro.FiltroComponente);

        //                    TipoOperacion tipoOperacionFull = db.TipoOperacion.Find(filtros[i].FiltroOperacion);

        //                    if (tipoOperacionFull != null)
        //                    {
        //                        if (tipoOperacionFull.CantidadValores == 0)
        //                        {
        //                            #region Sin valor
        //                            sCond = string.Format(" {0} {1} ",
        //                                            GetAtributoCampoFuncion(componenteFiltroFull, atributoFiltroFull),
        //                                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId));
        //                            #endregion
        //                        }
        //                        else if (tipoOperacionFull.CantidadValores == 1)
        //                        {
        //                            #region 1 Valor
        //                            string formattedValue = string.Empty;
        //                            if (tipoOperacionFull.TipoOperacionId == 8 || tipoOperacionFull.TipoOperacionId == 9)
        //                            {
        //                                formattedValue = "'" + configuracionFiltro.Valor1 + "%'";
        //                            }
        //                            else if (tipoOperacionFull.TipoOperacionId == 10 || tipoOperacionFull.TipoOperacionId == 11)
        //                            {
        //                                formattedValue = "'%" + configuracionFiltro.Valor1 + "'";
        //                            }
        //                            else if (tipoOperacionFull.TipoOperacionId == 12 || tipoOperacionFull.TipoOperacionId == 13)
        //                            {
        //                                formattedValue = "'%" + configuracionFiltro.Valor1 + "%'";
        //                            }
        //                            else if (tipoOperacionFull.TipoOperacionId == 16 || tipoOperacionFull.TipoOperacionId == 17)
        //                            {
        //                                if (atributoFiltroFull.AtributoParentId.HasValue)
        //                                {
        //                                    var datosListado = (from attrParent in db.Atributo
        //                                                        join compParent in db.Componente on attrParent.ComponenteId equals compParent.ComponenteId
        //                                                        join attrParentClave in db.Atributo on attrParent.ComponenteId equals attrParentClave.ComponenteId
        //                                                        where attrParent.AtributoId == atributoFiltroFull.AtributoParentId && attrParentClave.EsClave == 1
        //                                                        select new { attrParent, compParent, attrParentClave })
        //                                                        .First();

        //                                    if (tablaAgrupador.ToUpper() != datosListado.compParent.Tabla.ToUpper() && !lstJoin.Any(j => j.ToUpper().Contains(datosListado.compParent.Tabla.ToUpper())))
        //                                    {
        //                                        //lstCommaSeparatedJoins.Add(string.Format("{0}.{1} ", datosListado.compParent.Esquema, datosListado.compParent.Tabla));

        //                                        if (!lstJoin.Any(j => j.ToUpper().Contains(datosListado.compParent.Tabla.ToUpper())))
        //                                        {
        //                                            lstJoin.Add(string.Format(" JOIN {0}.{1} ON {1}.{2} = {3}.{4} ",
        //                                                                      datosListado.compParent.Esquema,
        //                                                                    datosListado.compParent.Tabla,
        //                                                                    datosListado.attrParentClave.Campo,
        //                                                                    componenteFiltroFull.Tabla,
        //                                                                    atributoFiltroFull.Campo));
        //                                        }
        //                                    }
        //                                    componenteFiltroFull = datosListado.compParent;
        //                                    atributoFiltroFull = datosListado.attrParent;
        //                                }
        //                                formattedValue = "(" + string.Join(",", configuracionFiltro.Valor1.Split(',').Select(v => atributoFiltroFull.GetFormattedValue(v))) + ")";
        //                            }
        //                            else
        //                            {
        //                                formattedValue = atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1).ToString();
        //                            }
        //                            sCond = string.Format(" {0} {1} {2} ",
        //                                            GetAtributoCampoFuncion(componenteFiltroFull, atributoFiltroFull),
        //                                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId),
        //                                            formattedValue);
        //                            #endregion
        //                        }
        //                        else if (tipoOperacionFull.CantidadValores == 2)
        //                        {
        //                            #region 2 Valores
        //                            sCond = string.Format(" {0} {1} {2} AND {3} ",
        //                                            GetAtributoCampoFuncion(componenteFiltroFull, atributoFiltroFull),
        //                                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId),
        //                                            atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1),
        //                                            atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor2));
        //                            #endregion
        //                        }
        //                    }
        //                    lstCondiciones.Add(sCond);
        //                    #endregion
        //                }
        //                else if (filtros[i].FiltroComponente != componente.ComponenteId)
        //                {
        //                    #region Componente filtro no es componente busqueda
        //                    //GetFiltroQuery(configuracionFiltro, componente, ref lstCondiciones, ref lstJoin);
        //                    //if (componente.Tabla != tablaAgrupador &&  GetComponenteById((long)filtros[i].FiltroComponente).IdAmbito == 1)
        //                    if (componente.Tabla != tablaAgrupador)
        //                    {//Cuando el agrupador es diferente a la tabla puede ser que exista en la relacion del join y de error de anbiguedad.
        //                        //lstJoin.RemoveAll(j => j.Contains(string.Format("JOIN {0}.{1} ON", esquemaAgrupador, tablaAgrupador)));
        //                        if (lstJoin.Any(j => j.Contains(string.Format("JOIN {0}.{1} ON", esquemaAgrupador, tablaAgrupador))))
        //                        {
        //                            lstForm.RemoveAll(j => j.Contains(string.Format("{0}.{1}", esquemaAgrupador, tablaAgrupador)));
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                #endregion
        //            }
        //            else if (configuracionFiltro.FiltroTipo == 2)
        //            {
        //                #region Filtro Geografico
        //                List<string> condicionesGraficas = new List<string>();

        //                if (configuracionFiltro.ConfiguracionesFiltroGrafico != null && configuracionFiltro.ConfiguracionesFiltroGrafico.Count > 0)
        //                {
        //                    #region Filtro Normal
        //                    foreach (var configuracionFiltroGrafico in configuracionFiltro.ConfiguracionesFiltroGrafico)
        //                    {
        //                        string mask = "ANYINTERACT";
        //                        if (configuracionFiltro.Dentro == 1 && configuracionFiltro.Tocando == 1)
        //                        {
        //                            mask = "INSIDE+COVEREDBY+EQUAL+TOUCH+OVERLAPBDYINTERSECT+OVERLAPBDYDISJOINT";
        //                        }
        //                        else if (configuracionFiltro.Dentro == 1)
        //                        {
        //                            mask = "INSIDE+COVEREDBY+EQUAL";
        //                        }
        //                        else if (configuracionFiltro.Tocando == 1)
        //                        {
        //                            mask = "TOUCH+OVERLAPBDYINTERSECT+OVERLAPBDYDISJOINT";
        //                        }

        //                        //no deberia haber aca pipes, deberia ser una coord por cada config_filtro_graf
        //                        //string[] aCoords = configuracionFiltroGrafico.Coordenadas.Split('|');

        //                        string[] aCoords = configuracionFiltroGrafico.Coordenadas.Split('+');
        //                        if (aCoords.Length > 0)
        //                        {
        //                            for (int iCords = 0; iCords < aCoords.Length; iCords++)
        //                            {
        //                                string coords = aCoords[iCords];
        //                                if (configuracionFiltro.Fuera == 1)
        //                                {
        //                                    #region Fuera
        //                                    condicionesGraficas.Add(" " + tabla + "." + campoId + " NOT IN " +
        //                                                  "( SELECT " + tablaGrafica + "." + campoId + " FROM " + esquema + "." + tablaGrafica +
        //                                                  " WHERE MDSYS.SDO_RELATE(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                  "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                  ",'MASK=" + mask + " QUERYTYPE=WINDOW') = 'TRUE')");
        //                                    #endregion
        //                                }
        //                                else if (configuracionFiltro.Ampliar != null && configuracionFiltro.Ampliar != 0)
        //                                {
        //                                    #region Ampliar
        //                                    if (configuracionFiltro.Ampliar > 0)
        //                                    {
        //                                        if (configuracionFiltro.PorcentajeInterseccion.GetValueOrDefault() > 0)
        //                                        {
        //                                            #region Interseccion con Buffer
        //                                            condicionesGraficas.Add(" " + tabla + "." + campoId + " IN " +
        //                                                          " (SELECT " + campoId + " FROM ( " +
        //                                                            " SELECT " + campoId + ", (overlap_area/total_area)*100 PORC_AREA FROM(" +
        //                                                            " SELECT " + tablaGrafica + "." + campoId + " " + campoId + "," +
        //                                                              " SDO_GEOM.SDO_AREA(" +
        //                                                                " SDO_GEOM.SDO_INTERSECTION(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                                        "SDO_GEOM.SDO_BUFFER(" +
        //                                                                            "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                                        ", " + configuracionFiltro.Ampliar.ToString() + ", 0.05, 'unit=m arc_tolerance=0.05')" +
        //                                                                " , 0.05)" +
        //                                                              ", 0.05) overlap_area" +
        //                                                              " ,SDO_GEOM.SDO_AREA(" + tablaGrafica + "." + campoGeometry + ", 0.05) total_area" +
        //                                                            " FROM " + esquema + "." + tablaGrafica + " " +
        //                                                            " WHERE  MDSYS.SDO_RELATE(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                                        "SDO_GEOM.SDO_BUFFER(" +
        //                                                                              "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                                        ", " + configuracionFiltro.Ampliar.ToString() + ", 0.05, 'unit=m arc_tolerance=0.05')" +
        //                                                                  ",'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' " +
        //                                                            " )" +
        //                                                            " WHERE overlap_area>0 )" +
        //                                                          " WHERE PORC_AREA>" + configuracionFiltro.PorcentajeInterseccion + ")");
        //                                            #endregion
        //                                        }
        //                                        else
        //                                        {
        //                                            condicionesGraficas.Add(" MDSYS.SDO_RELATE(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                              "SDO_GEOM.SDO_BUFFER(" +
        //                                                                 "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                              ", " + configuracionFiltro.Ampliar.ToString() + ", 0.05, 'unit=m arc_tolerance=0.05')" +
        //                                                          ",'MASK=" + mask + " QUERYTYPE=WINDOW') = 'TRUE'");
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        //Buffer Negativo
        //                                        condicionesGraficas.Add(" MDSYS.SDO_RELATE(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                      "SDO_GEOM.SDO_BUFFER(" +
        //                                                            "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                      ", " + configuracionFiltro.Ampliar.ToString() + ", 0.05, 'unit=m')" +
        //                                                      ",'MASK=" + mask + " QUERYTYPE=WINDOW') = 'TRUE'");
        //                                        //", 22193)" +
        //                                    }
        //                                    #endregion
        //                                }
        //                                else if (configuracionFiltro.PorcentajeInterseccion != null && configuracionFiltro.PorcentajeInterseccion > 0)
        //                                {
        //                                    #region Interseccion
        //                                    condicionesGraficas.Add(" " + tabla + "." + campoId + " IN " +
        //                                          " (SELECT " + campoId + " FROM ( " +
        //                                            " SELECT " + campoId + ", (overlap_area/total_area)*100 PORC_AREA FROM(" +
        //                                            " SELECT " + tablaGrafica + "." + campoId + " " + campoId + "," +
        //                                              " SDO_GEOM.SDO_AREA(" +
        //                                                " SDO_GEOM.SDO_INTERSECTION(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                    "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                " ,0.05)" +
        //                                              " , 0.05) overlap_area" +
        //                                              " ,SDO_GEOM.SDO_AREA(" + tablaGrafica + "." + campoGeometry + ", 0.05) total_area" +
        //                                            " FROM " + esquema + "." + tablaGrafica + " " +
        //                                            " WHERE  MDSYS.SDO_RELATE(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                  ",'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' " +
        //                                            " )" +
        //                                            " WHERE overlap_area>0 )" +
        //                                          " WHERE PORC_AREA>" + configuracionFiltro.PorcentajeInterseccion + ")");
        //                                    #endregion
        //                                }
        //                                else
        //                                {
        //                                    #region Sin Mask
        //                                    condicionesGraficas.Add(" MDSYS.SDO_RELATE(" + tablaGrafica + "." + campoGeometry + "," +
        //                                                        "FN_FROM_WKTGEOMETRY_WITH_SRID('" + coords + "', 8307)" +
        //                                                  ",'MASK=" + mask + " QUERYTYPE=WINDOW') = 'TRUE'");
        //                                    #endregion
        //                                }

        //                            }
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region Filtro Grafico Avanzado
        //                    if (configuracionFiltro.FiltroComponente != null && configuracionFiltro.FiltroComponente > 0)
        //                    {
        //                        string campoGeometryFGA = string.Empty;
        //                        Componente componenteFiltroFull = db.Componente.Find(configuracionFiltro.FiltroComponente);
        //                        db.Entry(componenteFiltroFull).Collection(a => a.Atributos).Load();

        //                        Atributo atributoFiltroFull = db.Atributo.Find(configuracionFiltro.FiltroAtributo);
        //                        try
        //                        {
        //                            campoGeometryFGA = componenteFiltroFull.Atributos.GetAtributoGeometry().Campo;
        //                        }
        //                        catch (ApplicationException appEx)
        //                        {
        //                            Global.GetLogger().LogError("Componente (id: " + componenteFiltroFull.ComponenteId + ") mal configurado.", appEx);
        //                        }

        //                        TipoOperacion tipoOperacionFull = null;
        //                        if (configuracionFiltro.FiltroOperacion != 0)
        //                        {
        //                            tipoOperacionFull = db.TipoOperacion.Find(configuracionFiltro.FiltroOperacion);
        //                        }
        //                        #region Condicion para Filtro Avanzado Grafico
        //                        if (tipoOperacionFull != null)
        //                        {
        //                            #region Condicion para Filtro Avanzado Grafico
        //                            if (tipoOperacionFull.CantidadValores == 0)
        //                            {
        //                                sCond = " FAG." + atributoFiltroFull.Campo + " " + GetOperacionTipo(tipoOperacionFull.TipoOperacionId) + " ";
        //                            }
        //                            else if (tipoOperacionFull.CantidadValores == 1)
        //                            {
        //                                sCond = " FAG." + atributoFiltroFull.Campo + " " + GetOperacionTipo(tipoOperacionFull.TipoOperacionId) + " " + atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1) + " ";
        //                            }
        //                            else if (tipoOperacionFull.CantidadValores == 2)
        //                            {
        //                                sCond = string.Format(" FAG.{0} {1} {2} AND {3} ",
        //                                                atributoFiltroFull.Campo,
        //                                                GetOperacionTipo(tipoOperacionFull.TipoOperacionId),
        //                                                atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1),
        //                                                atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor2));
        //                            }
        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            #region Atributo con valor fijo
        //                            if (configuracionFiltro.Valor1 != string.Empty)
        //                            {
        //                                sCond = " FAG." + atributoFiltroFull.Campo + " = " + atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1) + " ";
        //                            }
        //                            #endregion
        //                        }
        //                        #endregion

        //                        lstCondiciones.Add(sCond);

        //                        string mask = "ANYINTERACT";
        //                        if (configuracionFiltro.Dentro == 1 && configuracionFiltro.Tocando == 1)
        //                        {
        //                            mask = "INSIDE+COVEREDBY+EQUAL+TOUCH+OVERLAPBDYINTERSECT+OVERLAPBDYDISJOINT";
        //                        }
        //                        else if (configuracionFiltro.Dentro == 1)
        //                        {
        //                            mask = "INSIDE+COVEREDBY+EQUAL";
        //                        }
        //                        else if (configuracionFiltro.Tocando == 1)
        //                        {
        //                            mask = "TOUCH+OVERLAPBDYINTERSECT+OVERLAPBDYDISJOINT";
        //                        }
        //                        else if (configuracionFiltro.Fuera == 1)
        //                        {
        //                            mask = "ANYINTERACT";
        //                        }


        //                        lstForm.Add(string.Format("{0}.{1} FAG", componenteFiltroFull.Esquema, componenteFiltroFull.Tabla));//Agregamo el form del componente del filtro

        //                        //query = string.Format("SELECT DISTINCT {0}.{1},{2}.{3},{2}.{4},{0}.{9} FROM {5}.{2},{6}.{0},{7}.{8} FAG", tablaAgrupador, campoIdAgrupador, tabla, campoId, campoLabel, esquema, esquemaAgrupador, componenteFiltroFull.Esquema, componenteFiltroFull.TablaGrafica ?? componenteFiltroFull.Tabla, campoLabelAgrupador);

        //                        /*if (tablaGrafica != tabla)
        //                        {
        //                            query += string.Format(",{0}.{1}", esquema, tablaGrafica);
        //                            lstCondiciones.Add(string.Format("{0}.{1}={2}.{1}", tabla, campoId, tablaGrafica));
        //                        }*/
        //                        condicionesGraficas.Add(string.Format("MDSYS.SDO_RELATE({0}.{1},FAG.{2},'MASK={3} QUERYTYPE=WINDOW')='TRUE'", tablaGrafica, campoGeometry, campoGeometryAgrupador, mask));
        //                    }
        //                    #endregion
        //                }
        //                lstCondiciones.Add("(" + string.Join(" OR ", condicionesGraficas) + ")");
        //                #endregion
        //            }
        //            else if (configuracionFiltro.FiltroTipo == 3)
        //            {
        //                #region Filtro por Coleccion
        //                /*long idComponente = db.ColeccionComponente.First(cc => cc.ColeccionId == filtros[i].FiltroColeccion.Value).ComponenteId;
        //                if (componente.ComponenteId != idComponente)
        //                {
        //                    #region Componente de la coleccion igual al componente filtro
        //                    var componenteColeccion = _unitOfWork.ComponenteRepository.GetComponenteById(idComponente);
        //                    if (componenteColeccion.Graficos != 5)
        //                    {
        //                        var atributosComponenteColeccion = _unitOfWork.AtributoRepository.GetAtributosByIdComponente(idComponente);

        //                        sJoin = @", (SELECT {0} ID
        //                                                    FROM {1}.{2} TBL_PPAL,
        //                                                        (SELECT TBL_COMP_COL.{3} GEOMETRY
        //                                                        FROM GE_COLEC_COMP CC
        //                                                        INNER JOIN {4}.{5} TBL_COMP_COL ON TBL_COMP_COL.{6} = CC.ID_OBJETO 
        //                                                        WHERE CC.ID_COLECCION={7} AND CC.FECHA_BAJA IS NULL AND TBL_COMP_COL.FECHA_BAJA IS NULL) TBL_REL
        //                                                    WHERE SDO_RELATE(TBL_PPAL.{8}, TBL_REL.GEOMETRY, 'MASK=ANYINTERACT') = 'TRUE' AND
        //                                                          (SDO_GEOM.SDO_AREA(SDO_GEOM.SDO_INTERSECTION(TBL_PPAL.{8}, TBL_REL.GEOMETRY, 0.1),0.1) / 
        //                                                           SDO_GEOM.SDO_AREA(TBL_PPAL.{8},0.1) ) > 0.9) COL_{7}";
        //                        sJoin = string.Format(sJoin, campoId, esquema, tabla, atributosComponenteColeccion.GetAtributoGeometry().Campo,
        //                                                     componenteColeccion.Esquema, componenteColeccion.Tabla, atributosComponenteColeccion.GetAtributoClave().Campo,
        //                                                     filtros[i].FiltroColeccion, campoGeometry);
        //                        lstCondiciones.Add(string.Format("{0}.{1}=COL_{2}.ID", tabla, campoId, filtros[i].FiltroColeccion));
        //                        query += sJoin;
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region Componente de la coleccion diferente al componente filtro
        //                    query += string.Format(@", GE_COLEC_COMP COL_{0}", filtros[i].FiltroColeccion);
        //                    lstCondiciones.Add(string.Format(@"({1}.{2}=COL_{0}.ID_OBJETO AND COL_{0}.ID_COLECCION={0} AND 
        //                                                            COL_{0}.ID_COMPONENTE={3} AND COL_{0}.FECHA_BAJA IS NULL)",
        //                                         filtros[i].FiltroColeccion, tabla, campoId, componente.ComponenteId));
        //                    #endregion
        //                }*/
        //                //Revisar esto
        //                sJoin = " INNER JOIN  GE_COLEC_COMP ON " + tabla + "." + campoId + "=GE_COLEC_COMP.ID_OBJETO";

        //                sJoin += " AND GE_COLEC_COMP.ID_COLECCION=" + filtros[i].FiltroColeccion + " ";
        //                sJoin += " AND GE_COLEC_COMP.ID_COMPONENTE=" + componente.ComponenteId + " ";
        //                lstJoin.Add(sJoin);
        //                #endregion
        //            }
        //            else if (configuracionFiltro.FiltroTipo == 4)
        //            {
        //                #region Filtro Condicion
        //                if (configuracionFiltro.Valor2 == "3")//Parentecis abierto"("
        //                {
        //                    #region (
        //                    int aux = 0;
        //                    int cantAbi = 0;
        //                    int cantCer = 0;
        //                    for (int n = i; n < lstConfigFiltro.Count; n++)
        //                    {
        //                        if (lstConfigFiltro[n].Valor2 == "3")//Parentecis abierto
        //                        {
        //                            cantAbi++;
        //                        }
        //                        else if (lstConfigFiltro[n].Valor2 == "4")//Parentecis cerrado
        //                        {
        //                            cantCer++;
        //                        }

        //                        if (cantCer == cantAbi)
        //                        {
        //                            aux = n;
        //                            break;
        //                        }
        //                    }

        //                    List<Models.ConfiguracionFiltro> auxFiltro = new List<Models.ConfiguracionFiltro>();
        //                    //Ya tenemo el contexto
        //                    for (int o = i + 1; o < aux; o++)
        //                    {
        //                        auxFiltro.Add(lstConfigFiltro[o]);
        //                    }
        //                    var lo = 0;
        //                    result.AddRange(ObjetenerObjetoModelSinAgrupamiento(lstConfigFiltro, componente, ref lo, not));
        //                    i = aux + 1;
        //                    #endregion
        //                }
        //                else if (configuracionFiltro.Valor2 == "1")//AND
        //                {
        //                    #region AND
        //                    i++;

        //                    var resObj = ObjetenerObjetoModelSinAgrupamiento(lstConfigFiltro, componente, ref i, not);

        //                    result = result.Where(r => resObj.Select(s => s.ObjetoId).Contains(r.ObjetoId)).ToList();

        //                    i = lstConfigFiltro.Count;
        //                    #endregion
        //                }
        //                else if (configuracionFiltro.Valor2 == "2")//OR
        //                {
        //                    #region OR
        //                    i++;
        //                    result.AddRange(ObjetenerObjetoModelSinAgrupamiento(lstConfigFiltro, componente, ref i, not));
        //                    result = result.Distinct().ToList();
        //                    i = lstConfigFiltro.Count;
        //                    #endregion
        //                }
        //                else if (configuracionFiltro.Valor2 == "5")//NOT
        //                {
        //                    #region NOT
        //                    if (lstConfigFiltro[i + 1].FiltroTipo == 4)//Si es parentecis
        //                    {
        //                        int aux = 0;
        //                        int cantAbi = 0;
        //                        int cantCer = 0;
        //                        for (int y = i + 1; y < lstConfigFiltro.Count; y++)
        //                        {
        //                            if (lstConfigFiltro[y].Valor2 == "3")//Parentecis abierto
        //                            {
        //                                cantAbi++;
        //                            }
        //                            else if (lstConfigFiltro[y].Valor2 == "4")//Parentecis cerrado
        //                            {
        //                                cantCer++;
        //                            }

        //                            if (cantCer == cantAbi)
        //                            {
        //                                aux = y;
        //                                break;
        //                            }
        //                        }
        //                        List<Models.ConfiguracionFiltro> auxFiltro = new List<Models.ConfiguracionFiltro>();
        //                        //Ya tenemo el contexto
        //                        for (int o = i + 2; o < aux; o++)//+1 para que sea el parentecis, +1 para que pase el parentecis y no este en la lista.
        //                        {
        //                            auxFiltro.Add(lstConfigFiltro[o]);
        //                        }
        //                        var lo = 0;
        //                        result.AddRange(ObjetenerObjetoModelSinAgrupamiento(lstConfigFiltro, componente, ref lo, not == true ? false : true));
        //                        i = aux + 1;
        //                    }
        //                    else
        //                    {
        //                        i++;
        //                        List<Models.ConfiguracionFiltro> auxFiltro = new List<Models.ConfiguracionFiltro>();
        //                        auxFiltro.Add(lstConfigFiltro[i]);
        //                        var lo = 0;
        //                        result.AddRange(ObjetenerObjetoModelSinAgrupamiento(auxFiltro, componente, ref lo, not == true ? false : true));
        //                    }
        //                    #endregion
        //                }
        //                #endregion
        //            }
        //            #endregion

        //            if (configuracionFiltro.FiltroTipo != 4)
        //            {
        //                #region Query

        //                string query = "SELECT DISTINCT " + string.Join(", ", lstSelect);//lleva DISTINCT?

        //                query += " FROM ";

        //                /*if (filtros[i].FiltroComponente == idCompAgrupador)//eliminamos el componente del form, xq esta en el join
        //                {
        //                    List<string> lstAuxFormFiltro = lstForm.Where(l => !l.Contains(tablaAgrupador)).ToList();

        //                    //Primero los del filtro para que la tabla de joineo quede a lo ultimo.
        //                    if (lstForm.Count > 0)
        //                        query += string.Join(", ", lstAuxFormFiltro) + ", ";

        //                    List<string> lstAuxForm = lstForm.Where(l => !l.Contains(tablaAgrupador)).ToList();

        //                    query += string.Join(", ", lstAuxForm);
        //                }
        //                else
        //                {*/
        //                //Primero los del filtro para que la tabla de joineo quede a lo ultimo.
        //                if (lstForm.Count > 0)
        //                    query += string.Join(", ", lstForm);

        //                if (lstJoin.Count > 0)
        //                    query += " " + string.Join(" ", lstJoin);

        //                if (lstCondiciones.Count > 0)
        //                {
        //                    if (not)
        //                    {
        //                        query += " WHERE " + string.Join(" and ", lstCondiciones.Select(r => " NOT " + r).ToList());
        //                    }
        //                    else
        //                    {
        //                        query += " WHERE " + string.Join(" and ", lstCondiciones);
        //                    }
        //                }


        //                Global.GetLogger().LogInfo("Se va a ejecutar el siguiente query: " + query);
        //                ejecutaQuery = true;

        //                using (IDbCommand objComm = db.Database.Connection.CreateCommand())
        //                {
        //                    db.Database.Connection.Open();
        //                    objComm.CommandText = query;
        //                    using (IDataReader data = objComm.ExecuteReader(CommandBehavior.CloseConnection))
        //                    {
        //                        List<Models.ObjetoModel> lista = new List<Models.ObjetoModel>();
        //                        while (data.Read())
        //                        {
        //                            var obj = new Models.ObjetoModel();
        //                            obj.CompAgrupador.DocType = componente.DocType;
        //                            obj.CompAgrupador.Capa = componente.Capa;
        //                            obj.CompAgrupador.Descripcion = componente.Nombre;
        //                            obj.ObjetoId = data.GetInt64(1);
        //                            obj.ComponenteID = componente.ComponenteId;
        //                            obj.Descripcion = Convert.ToString(data.GetValue(2));
        //                            obj.CompAgrupador.ComponenteId = Convert.ToInt64(data.GetValue(0));
        //                            obj.CompAgrupador.Nombre = Convert.ToString(data.GetValue(3));
        //                            lista.Add(obj);
        //                        }
        //                        result.AddRange(lista);
        //                    }
        //                }
        //                Global.GetLogger().LogInfo("Query ejecutado correctamente.");
        //                ejecutaQuery = false;
        //                #endregion
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ejecutaQuery)
        //        {
        //            Global.GetLogger().LogInfo("Query no se pudo ejecutar.");
        //            ejecutaQuery = false;
        //        }
        //        Global.GetLogger().LogError("ResultadoBusquedaAvanzadaConAgrupamiento", ex);
        //        throw new Exception(ex.Message, ex);
        //    }
        //    return result;
        //}

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

        public Componente GetComponenteById(long idComponente)
        {
            return db.Componente.Find(idComponente);
        }
        public void GetAtributos(Componente componente)
        {
            db.Entry(componente).Collection(c => c.Atributos).Load();
        }

        [HttpGet]
        [ResponseType(typeof(bool))]
        public IHttpActionResult NuevaColeccion(int usuarioId, string nombreColeccion)
        {
            try
            {
                if (usuarioId < 0 || string.IsNullOrWhiteSpace(nombreColeccion))
                    throw new Exception("Parametros invalidos.");

                var coleccion = new Coleccion
                {
                    Nombre = nombreColeccion,
                    UsuarioAlta = usuarioId,
                    FechaAlta = DateTime.Now,
                    UsuarioModificacion = usuarioId,
                    UsuarioBaja = null,
                    FechaModificacion = DateTime.Now
                };
                CrearNuevaColeccion(coleccion);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok(true);
        }
        public void CrearNuevaColeccion(Coleccion coleccion)
        {
            db.Entry(coleccion).State = coleccion.ColeccionId == 0 ? EntityState.Added : EntityState.Modified;
            db.SaveChanges();
        }

        //private void GetFiltroQuery(Models.ConfiguracionFiltro configuracionFiltro, Models.BusquedaAvanzadaModel.ComponenteModel componente, ISQLQueryBuilder S)
        //{
        //    GetJerarquia(componente, configuracionFiltro);

        //    string sCond = string.Empty;
        //    var lstJerarquiaAll = db.Jerarquia;
        //    long idComponenteSuperior = componente.ComponenteId;
        //    var lstComponenteJerarquia = new List<Componente>();
        //    string sJoin = string.Empty;
        //    Componente componenteFiltroFull = db.Componente.Find(configuracionFiltro.FiltroComponente);

        //    Atributo atributoFiltroFull = db.Atributo.Find(configuracionFiltro.FiltroAtributo);

        //    TipoOperacion tipoOperacionFull = null;
        //    if (configuracionFiltro.FiltroOperacion != 0)
        //    {
        //        tipoOperacionFull = db.TipoOperacion.Find(configuracionFiltro.FiltroOperacion);
        //    }

        //    if (tipoOperacionFull != null)
        //    {
        //        if (tipoOperacionFull.CantidadValores == 0)
        //        {
        //            sCond = string.Format(" {0} {1} ",
        //                            GetAtributoCampoFuncion(componenteFiltroFull, atributoFiltroFull),
        //                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId));
        //        }
        //        else if (tipoOperacionFull.CantidadValores == 1)
        //        {
        //            string formattedValue = string.Empty;
        //            if (tipoOperacionFull.TipoOperacionId == 8 || tipoOperacionFull.TipoOperacionId == 9)
        //            {
        //                formattedValue = "'" + configuracionFiltro.Valor1 + "%'";
        //            }
        //            else if (tipoOperacionFull.TipoOperacionId == 12 || tipoOperacionFull.TipoOperacionId == 13)
        //            {
        //                formattedValue = "'%" + configuracionFiltro.Valor1 + "%'";
        //            }
        //            else if (tipoOperacionFull.TipoOperacionId == 10 || tipoOperacionFull.TipoOperacionId == 11)
        //            {
        //                formattedValue = "'%" + configuracionFiltro.Valor1 + "'";
        //            }
        //            else if (tipoOperacionFull.TipoOperacionId == 16 || tipoOperacionFull.TipoOperacionId == 17)
        //            {
        //                if (atributoFiltroFull.AtributoParentId.HasValue)
        //                {
        //                    var datosListado = (from attrParent in db.Atributo
        //                                        join compParent in db.Componente on attrParent.ComponenteId equals compParent.ComponenteId
        //                                        join attrParentClave in db.Atributo on attrParent.ComponenteId equals attrParentClave.ComponenteId
        //                                        where attrParent.AtributoId == atributoFiltroFull.AtributoParentId && attrParentClave.EsClave == 1
        //                                        select new { attrParent, compParent, attrParentClave })
        //                                        .First();

        //                    //lstJoin.Add(string.Format(" JOIN {0}.{1} ON {1}.{2} = {3}.{4} ",
        //                    //                    datosListado.compParent.Esquema,
        //                    //                    datosListado.compParent.Tabla,
        //                    //                    datosListado.attrParentClave.Campo,
        //                    //                    componenteFiltroFull.Tabla,
        //                    //                    atributoFiltroFull.Campo));

        //                    componenteFiltroFull = datosListado.compParent;
        //                    atributoFiltroFull = datosListado.attrParent;
        //                }
        //                formattedValue = "(" + string.Join(",", configuracionFiltro.Valor1.Split(',').Select(v => atributoFiltroFull.GetFormattedValue(v))) + ")";
        //            }
        //            else
        //            {
        //                formattedValue = atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1).ToString();
        //            }
        //            sCond = string.Format(" {0} {1} {2} ",
        //                            GetAtributoCampoFuncion(componenteFiltroFull, atributoFiltroFull),
        //                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId),
        //                            formattedValue);
        //        }
        //        else if (tipoOperacionFull.CantidadValores == 2)
        //        {
        //            sCond = string.Format(" {0} {1} {2} AND {3} ",
        //                            GetAtributoCampoFuncion(componenteFiltroFull, atributoFiltroFull),
        //                            GetOperacionTipo(tipoOperacionFull.TipoOperacionId),
        //                            atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1),
        //                            atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor2));
        //        }
        //    }
        //    else
        //    {
        //        //Es atributo con valor fijo
        //        if (!string.IsNullOrEmpty(configuracionFiltro.Valor1))
        //        {
        //            sCond = " " + GetAtributoCampoFuncion(componenteFiltroFull, atributoFiltroFull) + " = " + atributoFiltroFull.GetFormattedValue(configuracionFiltro.Valor1) + " ";
        //        }
        //    }
        //    //lstCondiciones.Add(sCond);
        //}

        //private void GetJerarquia(Models.BusquedaAvanzadaModel.ComponenteModel componente, Models.ConfiguracionFiltro configuracionFiltro, ref List<String> lstJoin)
        //{
        //    List<Jerarquia> lstJerarquiaAll = db.Jerarquia.ToList();
        //    long idComponenteSuperior = componente.ComponenteId;
        //    List<Componente> lstComponenteJerarquia = new List<Componente>();
        //    string sJoin = string.Empty;
        //    Componente componenteFiltroFull = db.Componente.Find(configuracionFiltro.FiltroComponente);
        //    db.Entry(componenteFiltroFull).Collection(a => a.Atributos).Load();
        //    List<Jerarquia> lstJerarquiaPadres = new List<Jerarquia>();

        //    if (GetJerarquiaMayorBA(lstJerarquiaAll, idComponenteSuperior, (long)configuracionFiltro.FiltroComponente, lstJerarquiaPadres))
        //    {
        //        foreach (var jerarquia in lstJerarquiaPadres)
        //        {
        //            Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
        //            Atributo atributoJerInf = db.Atributo.Find(jerarquia.AtributoInferiorId);
        //            Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
        //            Atributo atributoJerSup = db.Atributo.Find(jerarquia.AtributoSuperiorId);
        //            if (!lstComponenteJerarquia.Any(p => p.ComponenteId == componenteJerInf.ComponenteId))
        //            {
        //                if (!string.IsNullOrEmpty(jerarquia.TablaRelacion))
        //                {
        //                    Componente componenteRelacionado = new Componente();
        //                    componenteRelacionado.ComponenteId = 0;
        //                    componenteRelacionado.EntornoId = 0;
        //                    componenteRelacionado.Nombre = jerarquia.TablaRelacion;
        //                    componenteRelacionado.Descripcion = jerarquia.TablaRelacion;
        //                    componenteRelacionado.Esquema = jerarquia.EsquemaTblRel;
        //                    componenteRelacionado.Tabla = jerarquia.TablaRelacion;
        //                    componenteRelacionado.Graficos = 5;

        //                    sJoin = " JOIN " + jerarquia.EsquemaTblRel + "." + jerarquia.TablaRelacion + " ON " + componenteJerSup.Tabla + "." + atributoJerSup.Campo + " = " + jerarquia.TablaRelacion + ".ID_TABLA_PADRE AND " + jerarquia.TablaRelacion + ".TABLA_PADRE = '" + componenteJerSup.Tabla + "' ";
        //                    lstComponenteJerarquia.Add(componenteRelacionado);
        //                    lstJoin.Add(sJoin);

        //                    sJoin = " JOIN " + componenteJerInf.Esquema + "." + componenteJerInf.Tabla + " ON " + jerarquia.TablaRelacion + ".ID_TABLA_HIJO = " + componenteJerInf.Tabla + "." + atributoJerInf.Campo + " AND " + jerarquia.TablaRelacion + ".TABLA_HIJO = '" + componenteJerInf.Tabla + "' ";
        //                    lstComponenteJerarquia.Add(componenteJerInf);
        //                    lstJoin.Add(sJoin);
        //                }
        //                else
        //                {
        //                    sJoin = " JOIN " + componenteJerInf.Esquema + "." + componenteJerInf.Tabla + " ON " + componenteJerInf.Tabla + "." + atributoJerInf.Campo + "=" + componenteJerSup.Tabla + "." + atributoJerSup.Campo;
        //                    lstJoin.Add(sJoin);
        //                    lstComponenteJerarquia.Add(componenteJerInf);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        List<Jerarquia> lstJerarquiaHijos = new List<Jerarquia>();
        //        if (GetJerarquiaMenorBA(lstJerarquiaAll, idComponenteSuperior, (long)configuracionFiltro.FiltroComponente, lstJerarquiaHijos))
        //        {
        //            foreach (var jerarquia in lstJerarquiaHijos)
        //            {
        //                Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
        //                Atributo atributoJerInf = db.Atributo.Find(jerarquia.AtributoInferiorId);
        //                Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
        //                Atributo atributoJerSup = db.Atributo.Find(jerarquia.AtributoSuperiorId);
        //                if (!lstComponenteJerarquia.Any(p => p.ComponenteId == componenteJerSup.ComponenteId))
        //                {
        //                    if (!string.IsNullOrEmpty(jerarquia.TablaRelacion))
        //                    {
        //                        Componente componenteRelacionado = new Componente();
        //                        componenteRelacionado.ComponenteId = 0;
        //                        componenteRelacionado.EntornoId = 0;
        //                        componenteRelacionado.Nombre = jerarquia.TablaRelacion;
        //                        componenteRelacionado.Descripcion = jerarquia.TablaRelacion;
        //                        componenteRelacionado.Esquema = jerarquia.EsquemaTblRel;
        //                        componenteRelacionado.Tabla = jerarquia.TablaRelacion;
        //                        componenteRelacionado.Graficos = 5;

        //                        sJoin = " JOIN " + jerarquia.EsquemaTblRel + "." + jerarquia.TablaRelacion + " ON " + componenteJerInf.Tabla + "." + atributoJerInf.Campo + " = " + jerarquia.TablaRelacion + ".ID_TABLA_HIJO AND " + jerarquia.TablaRelacion + ".TABLA_HIJO = '" + componenteJerInf.Tabla + "' ";
        //                        lstComponenteJerarquia.Add(componenteRelacionado);
        //                        lstJoin.Add(sJoin);

        //                        sJoin = " JOIN " + componenteJerSup.Esquema + "." + componenteJerSup.Tabla + " ON " + jerarquia.TablaRelacion + ".ID_TABLA_PADRE = " + componenteJerSup.Tabla + "." + atributoJerSup.Campo + " AND " + jerarquia.TablaRelacion + ".TABLA_PADRE = '" + componenteJerSup.Tabla + "' ";
        //                        lstComponenteJerarquia.Add(componenteJerSup);
        //                        lstJoin.Add(sJoin);
        //                    }
        //                    else
        //                    {
        //                        sJoin = " JOIN " + componenteJerSup.Esquema + "." + componenteJerSup.Tabla + " ON " + componenteJerSup.Tabla + "." + atributoJerSup.Campo + "=" + componenteJerInf.Tabla + "." + atributoJerInf.Campo;
        //                        lstJoin.Add(sJoin);
        //                        lstComponenteJerarquia.Add(componenteJerSup);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private bool GetJerarquiaMayorBA(List<Jerarquia> lstJerarquiaAll, long idComponentePadre, long idComponenteInferior, List<Jerarquia> lstJerarquiaPadres)
        //{
        //    bool encontroPadre = false;
        //    foreach (var jerarquia in lstJerarquiaAll.Where(p => p.ComponenteInferiorId == idComponenteInferior))
        //    {
        //        //Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
        //        Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
        //        if (componenteJerSup.ComponenteId == idComponentePadre)
        //        {
        //            encontroPadre = true;
        //        }
        //        else if (string.IsNullOrEmpty(jerarquia.EsquemaTblRel))
        //        {
        //            encontroPadre = GetJerarquiaMayorBA(lstJerarquiaAll, idComponentePadre, componenteJerSup.ComponenteId, lstJerarquiaPadres);
        //        }
        //        if (encontroPadre)
        //        {
        //            lstJerarquiaPadres.Add(jerarquia);
        //            break;
        //        }
        //    }
        //    return encontroPadre;
        //}

        //private bool GetJerarquiaMenorBA(List<Jerarquia> lstJerarquiaAll, long idComponenteHijo, long idComponenteSuperior, List<Jerarquia> lstJerarquiaHijos)
        //{
        //    bool encontroHijo = false;
        //    foreach (var jerarquia in lstJerarquiaAll.Where(p => p.ComponenteSuperiorId == idComponenteSuperior))
        //    {
        //        Componente componenteJerInf = db.Componente.Find(jerarquia.ComponenteInferiorId);
        //        //Componente componenteJerSup = db.Componente.Find(jerarquia.ComponenteSuperiorId);
        //        if (componenteJerInf.ComponenteId == idComponenteHijo)
        //        {
        //            encontroHijo = true;
        //        }
        //        else if (string.IsNullOrEmpty(jerarquia.EsquemaTblRel))
        //        {
        //            encontroHijo = GetJerarquiaMenorBA(lstJerarquiaAll, idComponenteHijo, jerarquia.ComponenteInferiorId, lstJerarquiaHijos);
        //        }
        //        if (encontroHijo)
        //        {
        //            lstJerarquiaHijos.Add(jerarquia);
        //            break;
        //        }
        //    }
        //    return encontroHijo;
        //}

        private IEnumerable<Models.RelacionJerarquiaIntermedia> GetJerarquia(Models.BusquedaAvanzadaModel.ComponenteModel componente, Models.ConfiguracionFiltro configuracionFiltro)
        {
            bool encontro = false;
            HashSet<long> agregados = new HashSet<long>();
            foreach (var jerarquia in JerarquiasPadres(componente.ComponenteId, configuracionFiltro.FiltroComponente.GetValueOrDefault()))
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
                foreach (var jerarquia in JerarquiasHijos(componente.ComponenteId, configuracionFiltro.FiltroComponente.GetValueOrDefault()))
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
                }
            }
        }
    }
}

