using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoSeccionServiceController : ApiController 
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/TipoSeccion
        public IHttpActionResult GetTiposSeccion()
        {
            return Ok(db.Tipo_Seccion.ToList());
            //return Ok(db.Tramite_Seccion.ToList());
        }


        // GET api/TipoSeccionByIdTramite
        public IHttpActionResult GetTipoSeccionByIdTramite(long id)
        {
            //List<TramiteSeccion> aaa = db.Tramite_Seccion.Where(a => !a.Fecha_Baja.HasValue && a.Id_Tramite_Seccion == id).ToList();
            List<TramiteTipoSeccion> TiposSecciones = db.Tipo_Seccion.Where(a => !a.Fecha_Baja.HasValue && a.Id_Tipo_Tramite == id).ToList();
            return Ok(TiposSecciones);
        }


        [HttpPost]
        public IHttpActionResult SetTipoSeccion_Save(TramiteTipoSeccion parametros)
        {
            //var activityInDb = db.Tramite_Seccion.Find(parametros.Id_Tipo_Seccion);
            var activityInDb = db.Tipo_Seccion.Find(parametros.Id_Tipo_Seccion);
            try
            {

                if (activityInDb != null)
                {
                    db.Entry(activityInDb).CurrentValues.SetValues(parametros);
                    db.Entry(activityInDb).State = EntityState.Modified;
                    db.SaveChanges(new Auditoria(parametros.Id_Usu_Modif, Eventos.EliminarPerfiles, Mensajes.EliminarPerfilesOK, parametros._Machine_Name,
                  parametros._Ip, Autorizado.Si, parametros, null, "Tipo_Seccion", 1, TiposOperacion.Modificacion));
                }
                else
                {
                    db.Entry(parametros).State = EntityState.Added;
                    db.SaveChanges(new Auditoria(parametros.Id_Usu_Alta, Eventos.EliminarPerfiles, Mensajes.EliminarPerfilesOK, parametros._Machine_Name,
                  parametros._Ip, Autorizado.Si, parametros, null, "Tipo_Seccion", 1, TiposOperacion.Alta));
                }

                
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return Ok();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
   
}