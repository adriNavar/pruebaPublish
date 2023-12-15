using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class ProfesionServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Profesion<
        [ResponseType(typeof(ICollection<Profesion>))]
        public IHttpActionResult GetProfesiones()
        {
            return Ok(db.Profesion.ToList());
        }

        // GET api/GetProfesionByPersona
        [ResponseType(typeof(ICollection<Profesion>))]
        public IHttpActionResult GetProfesionByPersona(long id)
        {
            var profe = db.Profesion.Where(a => a.PersonaId == id).ToList();
            return profe == null ? (IHttpActionResult)NotFound() : Ok(profe);
        }

        [HttpPost]
        public IHttpActionResult SetProfesion_Save(Profesion parametros)
        {
            db.Entry(parametros).State = EntityState.Added;
            try
            {
                db.SaveChanges(new Auditoria(parametros._Id_Usuario, Eventos.AltadeProfesion, Mensajes.AltadeProfesionOK, parametros._Machine_Name,
                    parametros._Machine_Name, Autorizado.Si, null, parametros, "Profesion", 1, TiposOperacion.Alta));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ProfesionServiceController-SetProfesion_Save", ex);
                return InternalServerError();
            }
            /*
            var Profesiones = db.Profesion.ToList();
            var Var_Profesion = new Profesion();
            Var_Profesion.PersonaId = parametros.PersonaId;
            Var_Profesion.TipoProfesionId = parametros.TipoProfesionId;
            Var_Profesion.Matricula = parametros.Matricula;

            Profesiones.Add(Var_Profesion);
            db.SaveChanges();
            */
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SetProfesion_Delete(long idpersona)
        {
            var ListaProfesiones = db.Profesion.Where(x => x.PersonaId == idpersona).ToList();

            foreach (var item in ListaProfesiones)
            {
                db.Entry(item).State = EntityState.Deleted;
                try
                {
                    db.SaveChanges(new Auditoria(0, Eventos.BajadeProfesion, Mensajes.BajadeProfesionOK, item._Machine_Name,
                                   item._Machine_Name, Autorizado.Si, null, item, "Profesion", 1, TiposOperacion.Baja));
                }
                catch(Exception)
                {
                    //log
                }
            }
            return Ok();
        }
    }
}
