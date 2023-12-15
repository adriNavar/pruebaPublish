using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using GeoSit.Data.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers.ReclamosDiarios
{
    public class ActualizacionController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        [ResponseType(typeof(DateTime))]
        public IHttpActionResult GetUltimaFechaActualizacionReclamosDiarios(char tipo)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                int idTipo=0;
                switch (tipo)
                {
                    case 'C':
                        idTipo = 3;
                        break;
                    case 'Q':
                        idTipo = 2;
                        break;
                    case 'X':
                        idTipo = 5;
                        break;
                    default:
                    case 'A':
                        idTipo = 1;
                        break;
                    
                }
                Reclamos_ReclamosDiarios ultimoReclamo = db.Reclamos_ReclamosDiarios.Where(a=>a.Fecha_Ingreso!=null && a.Id_Tipo==idTipo).OrderByDescending(x => x.Fecha_Ingreso).FirstOrDefault();

                if (ultimoReclamo == null)
                {
                    result = DateTime.MinValue;
                }
                else
                {
                    result = ultimoReclamo.Fecha_Ingreso.Value;
                }
                //db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                result = DateTime.Now;
                throw ex;
            }
            return Ok(result);
        }
        [ResponseType(typeof(DateTime))]
        public IHttpActionResult GetUltimaFechaParametros()
        {
            DateTime result = DateTime.MinValue;

            List<Reclamos_Actualizacion> actualizaciones = db.Reclamos_Actualizacion.ToList();

            if (actualizaciones.Count == 1)
            {
                Reclamos_Actualizacion actualizacion = actualizaciones[0];
                    result = actualizacion.Fecha_Parametros;
            }
            return Ok(result);
        }
        [ResponseType(typeof(bool))]
        public IHttpActionResult GetUpdateUltimaFechaActualizacionParametros(DateTime dateTime)
        {
            bool result = true;
            try
            {
                List<Reclamos_Actualizacion> actualizaciones = db.Reclamos_Actualizacion.Where(a => a.Nombre == "FechasActualizacion").ToList();

                if (actualizaciones != null && actualizaciones.Count == 1)
                {
                    actualizaciones[0].Fecha_Parametros = dateTime;
                }
                else
                {
                    Reclamos_Actualizacion reclamoActualizacion = new Reclamos_Actualizacion();
                    reclamoActualizacion.Nombre = "FechasActualizacion";
                    reclamoActualizacion.Fecha_Parametros = dateTime;
                    reclamoActualizacion.Fecha_Reclamos_Diarios = DateTime.MinValue;
                    db.Reclamos_Actualizacion.Add(reclamoActualizacion);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }

            return Ok(result);
        }
    }
}
