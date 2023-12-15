using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoTramiteServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/TipoTramite
        [ResponseType(typeof(ICollection<TipoTramite>))]
        public IHttpActionResult GetTiposTramite()
        {
            return Ok(db.Tipo_Tramite.Where(a => !a.Fecha_Baja.HasValue).ToList());
        }

        // GET api/TipoTramite
        [ResponseType(typeof(TipoTramite))]
        public IHttpActionResult GetTipoTramiteById(long id)
        {
            TipoTramite tramite = db.Tipo_Tramite.Where(a => !a.Fecha_Baja.HasValue && a.Id_Tipo_Tramite == id).FirstOrDefault();
            return Ok(tramite);
        }

        // GET api/TipoTramite
        [ResponseType(typeof(TipoTramite))]
        public IHttpActionResult GetTipoTramiteByNombre(string id)
        {
            TipoTramite tramite = db.Tipo_Tramite.Where(a => !a.Fecha_Baja.HasValue && a.Nombre == id).FirstOrDefault();
            return Ok(tramite);
        }

        [HttpPost]
        public IHttpActionResult SetTipoTramite_Save(TipoTramite parametros)
        {
            parametros.Fecha_Modif = DateTime.Now;
            try
            {
                if (parametros.Id_Tipo_Tramite == 0)
                {
                    parametros.Fecha_Alta = parametros.Fecha_Modif;

                    db.Entry(parametros).State = EntityState.Added;
                    var id = parametros.Id_Tipo_Tramite;
                    db.SaveChanges(new Auditoria(parametros.Id_Usu_Alta, Eventos.AltadeTramites, Mensajes.AltadeTramitesOK, parametros._Machine_Name,
                      parametros._Ip, Autorizado.Si, null, parametros, "TipoTramite", 1, TiposOperacion.Alta));
                }
                else
                {
                    db.Entry(parametros).State = EntityState.Modified;
                    db.SaveChanges(new Auditoria(parametros.Id_Usu_Modif, Eventos.ModificarTramites, Mensajes.ModificarTramitesOK, parametros._Machine_Name,
                      parametros._Ip, Autorizado.Si, parametros, parametros, "TipoTramite", 1, TiposOperacion.Modificacion));
                }



            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult DeleteTipoTramite_Save(TipoTramite parametros)
        {
            var activityInDb = db.Tipo_Tramite.Find(parametros.Id_Tipo_Tramite);
            if (activityInDb != null)
            {
                activityInDb.Id_Usu_Baja = parametros.Id_Usu_Baja;
                activityInDb.Fecha_Baja = DateTime.Now;
                db.Entry(activityInDb).State = EntityState.Modified;
                try
                {
                    db.SaveChanges(new Auditoria(parametros.Id_Usu_Baja ?? 0, Eventos.BajadeTramite, Mensajes.BajadeTramiteOK, parametros._Machine_Name,
                                    parametros._Ip, Autorizado.Si, parametros, null, "TipoTramite", 1, TiposOperacion.Baja));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return Ok();
        }

    }
}