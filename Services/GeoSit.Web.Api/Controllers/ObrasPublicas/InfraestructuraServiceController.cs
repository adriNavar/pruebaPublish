using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using System.Configuration;

namespace GeoSit.Web.Api.Controllers.ObrasPublicas
{
    public class InfraestructuraServiceController : ApiController
    {
        // GET: ZonaAtributoService
        [ResponseType(typeof(ICollection<TipoObjetoInfraestructura>))]
        public IHttpActionResult GetTipos()
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                List<TipoObjetoInfraestructura> Tipos = new List<TipoObjetoInfraestructura>(db.TipoObjetoInfraestructura.ToList());

                if (Tipos == null)
                {
                    return NotFound();
                }

                return Ok(Tipos);
            }
        }

        [ResponseType(typeof(ICollection<SubtipoObjetoInfraestructura>))]
        public IHttpActionResult GetSubTipos(long Id_Tipo)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                var subtipos = db.SubtipoObjetoInfraestructura
                                 .Where(s => s.ID_Tipo_Objeto == Id_Tipo)
                                 .ToList();

                if (!subtipos.Any())
                {
                    return NotFound();
                }

                return Ok(subtipos);
            }
        }

        [ResponseType(typeof(SubtipoObjetoInfraestructura))]
        public IHttpActionResult GetSubTipo(long Id_Tipo, long Id_SubTipo)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                SubtipoObjetoInfraestructura SubTipo = (db.SubtipoObjetoInfraestructura.Where(s => s.ID_Tipo_Objeto == Id_Tipo && s.ID_Subtipo_Objeto == Id_SubTipo).FirstOrDefault());

                if (SubTipo == null)
                {
                    return NotFound();
                }

                return Ok(SubTipo);
            }
        }

        [ResponseType(typeof(ICollection<ObjetoInfraestructura>))]
        public IHttpActionResult GetObjetosInfraestructura()
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                List<ObjetoInfraestructura> objetos = new List<ObjetoInfraestructura>(db.ObjetoInfraestructura.Where(s => !s.ID_Usu_Baja.HasValue).ToList());
                fillObjetos(db, objetos);

                if (objetos == null)
                {
                    return NotFound();
                }
                return Ok(objetos);
            }
        }

        [ResponseType(typeof(ICollection<ObjetoInfraestructura>))]
        public IHttpActionResult GetObjetosInfraestructura(long SubTipoId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                List<ObjetoInfraestructura> objetos = new List<ObjetoInfraestructura>(db.ObjetoInfraestructura.Where(s => !s.ID_Usu_Baja.HasValue && s.ID_Subtipo_Objeto == SubTipoId).ToList());

                //fillObjetos(db, objetos);

                if (objetos == null)
                {
                    return NotFound();
                }
                return Ok(objetos);
            }
        }

        private void fillObjetos(GeoSITMContext db, List<ObjetoInfraestructura> objetos)
        {
            foreach (ObjetoInfraestructura mObj in objetos)
            {
                db.Entry(mObj).Reference(r => r.SubtipoObjeto).Load();

                db.Entry(mObj.SubtipoObjeto).Reference(r => r.TipoObjeto).Load();
            }
        }

        [ResponseType(typeof(ObjetoInfraestructura))]
        public IHttpActionResult GetObjetosInfraestructura(long FeatId, long SubTipoId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                var mINF_Objeto = new ObjetoInfraestructura();
                if (FeatId != 0)
                {
                    mINF_Objeto = db.ObjetoInfraestructura.FirstOrDefault(s => s.ID_Usu_Baja == null && s.ID_Subtipo_Objeto == SubTipoId && s.FeatID == FeatId);

                    if (mINF_Objeto == null)
                    {
                        return NotFound();
                    }
                    db.Entry(mINF_Objeto).Reference(r => r.SubtipoObjeto).Load();
                    db.Entry(mINF_Objeto.SubtipoObjeto).Reference(r => r.TipoObjeto).Load();
                }
                else
                {
                    mINF_Objeto.ID_Subtipo_Objeto = SubTipoId;
                    mINF_Objeto.SubtipoObjeto = db.SubtipoObjetoInfraestructura.FirstOrDefault(s => s.ID_Subtipo_Objeto == SubTipoId);
                }

                return Ok(mINF_Objeto);
            }
        }

        public IHttpActionResult GetGeometryWKTByFeatId(long id)
        {
            using (var builder = GeoSITMContext.CreateContext().CreateSQLQueryBuilder())
            {
                var cmp = new Componente { ComponenteId = 1, Tabla = "INF_OBJETO", Esquema = ConfigurationManager.AppSettings["DATABASE"] };
                string wkt = builder.AddTable(cmp, cmp.Tabla)
                                    .AddFilter(new Atributo { Campo = "FEATID", ComponenteId = cmp.ComponenteId }, id, Data.DAL.Common.Enums.SQLOperators.EqualsTo)
                                    .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo { Campo = "GEOMETRY", ComponenteId = cmp.ComponenteId }, cmp.Tabla).ToWKT(), "geom")
                                    .ExecuteQuery((IDataReader reader) => reader.GetStringOrEmpty(0))
                                    .FirstOrDefault();
                return Ok(wkt);
            }
        }

        [ResponseType(typeof(SubtipoObjetoInfraestructura))]
        public IHttpActionResult GetAtributos(long ID_Subtipo_Objeto)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                SubtipoObjetoInfraestructura mINF_SubtipoObjeto = db.SubtipoObjetoInfraestructura.Where(s => s.ID_Subtipo_Objeto == ID_Subtipo_Objeto).FirstOrDefault();

                if (mINF_SubtipoObjeto == null)
                {
                    return NotFound();
                }

                return Ok(mINF_SubtipoObjeto);
            }
        }

        [Route("api/InfraestructuraService/DeleteObjetoInfraestructura/{FeatId}")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult DeleteObjetoInfraestructura(long FeatId)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                ObjetoInfraestructura mINF_Objeto = db.ObjetoInfraestructura.Find(FeatId);
                if (mINF_Objeto == null)
                {
                    return BadRequest("No se encontro el registro");
                }
                mINF_Objeto.ID_Usu_Baja = 1;
                mINF_Objeto.Fecha_Baja = DateTime.Now;

                db.Entry(mINF_Objeto).State = EntityState.Modified;

                db.SaveChanges(new Auditoria(mINF_Objeto.ID_Usu_Baja ?? 0, Eventos.BajaObjetoInf, "Se elimino el Objeto Infraestrcutura", mINF_Objeto._Machine_Name,
                   mINF_Objeto._Machine_Name, Autorizado.Si, mINF_Objeto, null, "ObjetoInfraestructura", 1, TiposOperacion.Baja));

                return Ok(mINF_Objeto);
            }
        }

        [HttpPost]
        public IHttpActionResult PostObjetoInfraestructura(ObjetoInfraestructura model)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                model.Fecha_Modif = DateTime.Now;
                var mINF_Objeto = db.ObjetoInfraestructura.Find(model.FeatID) ??
                                  db.ObjetoInfraestructura.Add(new ObjetoInfraestructura()
                                  {
                                      FeatID = model.FeatID,
                                      ID_Objeto_Padre = 0,
                                      ID_Subtipo_Objeto = model.ID_Subtipo_Objeto,
                                      Fecha_Alta = model.Fecha_Modif.Value,
                                      ID_Usu_Alta = model.ID_Usu_Modif.Value
                                  });
                
                mINF_Objeto.Atributos = model.Atributos;
                mINF_Objeto.Nombre = model.Nombre;
                mINF_Objeto.Descripcion = model.Descripcion;

                mINF_Objeto.Fecha_Modif = model.Fecha_Modif;
                mINF_Objeto.ID_Usu_Modif = model.ID_Usu_Modif;

                var evento = Eventos.AltaObjetoInf;
                string datosAdicionales = "Se agregó el objeto de infraestrcutura";
                object objetosOrigen = null;
                var tipoOperacion = TiposOperacion.Alta;

                if (db.Entry(mINF_Objeto).State == EntityState.Modified)
                {
                    evento = Eventos.ModificacionObjetoInf;
                    datosAdicionales = "Se modificó el objeto de infraestrcutura";
                    objetosOrigen = db.Entry(mINF_Objeto).OriginalValues.ToObject();
                    tipoOperacion = TiposOperacion.Modificacion;
                }
                
                db.SaveChanges(new Auditoria(model.ID_Usu_Modif.Value, evento, datosAdicionales, model._Machine_Name, model._Ip, Autorizado.Si, objetosOrigen, mINF_Objeto, "ObjetoInfraestructura", 1, tipoOperacion));
                
                using (var builder = db.CreateSQLQueryBuilder())
                {
                    var cmp = new Componente { ComponenteId = 1, Tabla = "INF_OBJETO", Esquema = ConfigurationManager.AppSettings["DATABASE"] };
                    try
                    {
                        builder.AddTable(cmp, null)
                               .AddFilter(new Atributo { Campo = "FEATID", ComponenteId = cmp.ComponenteId }, mINF_Objeto.FeatID, Data.DAL.Common.Enums.SQLOperators.EqualsTo)
                               .AddFieldsToUpdate(new KeyValuePair<Atributo, object>(new Atributo() { Campo = "GEOMETRY", ComponenteId = cmp.ComponenteId },
                                                                                     builder.CreateGeometryFieldBuilder(model.WKT, Data.DAL.Common.Enums.SRID.DB)),
                                                  new KeyValuePair<Atributo, object>(new Atributo() { Campo = "GEOM_TXT", ComponenteId = cmp.ComponenteId, TipoDatoId = 6 },
                                                                                     model.WKT))
                               .ExecuteUpdate();
                    }
                    catch
                    {
                        //
                    }
                }
                return Ok(mINF_Objeto);
            }
        }
    }
}