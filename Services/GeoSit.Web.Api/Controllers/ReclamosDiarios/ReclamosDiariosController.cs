using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using GeoSit.Data.BusinessEntities.SAR;
using GeoSit.Data.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers.ReclamosDiarios
{
    public class ReclamosDiariosController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostGuardarReclamo(List<ReclamoDiarioResult> reclamos)
        {
            if (reclamos.Any())
            {
                foreach (var reclamo in reclamos)
                {
                    Reclamos_ReclamosDiarios newReclamo = new Reclamos_ReclamosDiarios();
                    newReclamo.Nro_Reclamo = reclamo.Reclamo;
                    newReclamo.Distrito = reclamo.Distrito;
                    newReclamo.Calle = reclamo.Calle;
                    newReclamo.Altura = reclamo.Altura;
                    newReclamo.Interseccion = reclamo.Interseccion;
                    newReclamo.Manzana = reclamo.Manzana;
                    newReclamo.Fecha_Ingreso = reclamo.FechaIngreso;
                    newReclamo.Id_Clase = reclamo.Clase_Id;
                    newReclamo.Id_Tipo = reclamo.Tipo_Id;
                    newReclamo.Id_Motivo = reclamo.Motivo_Id;
                    newReclamo.FechaAlta = DateTime.Now;

                    db.Reclamos_ReclamosDiarios.Add(newReclamo);
                }
                //db.SaveChanges(new Auditoria(0, Eventos.AltaDomicilio, Mensajes.AltaDomicilioOK, reclamos[0]._Machine_Name,
                //    reclamos[0]._Machine_Name, Autorizado.Si, null, parametros, "Domicilio", 1, TiposOperacion.Baja));
                db.SaveChanges();
                //TODO REVISAR
            }

            return Ok(true);

        }
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostGuardarReclamosClase(List<Reclamos_Clase> reclamosClase)
        {
            db.Reclamos_Clase.RemoveRange(db.Reclamos_Clase.ToList());
            db.SaveChanges();
            foreach (var currReclamoClase in reclamosClase)
            {
                Reclamos_Clase newReclamoClase = db.Reclamos_Clase.FirstOrDefault<Reclamos_Clase>(x => x.Id == currReclamoClase.Id) ?? new Reclamos_Clase() { Clase_Id = 0 };
                bool add = newReclamoClase.Clase_Id == 0;
                newReclamoClase.Clase_Id = currReclamoClase.Clase_Id;
                newReclamoClase.Descripcion = currReclamoClase.Descripcion;
                newReclamoClase.Habilitado = currReclamoClase.Habilitado;
                newReclamoClase.Id = currReclamoClase.Id;
                if (add)
                    db.Reclamos_Clase.Add(newReclamoClase);
            }
            db.SaveChanges();
            return Ok(true);
        }
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostGuardarReclamosMotivos(List<Reclamos_Motivo> reclamosMotivo)
        {
            db.Reclamos_Motivo.RemoveRange(db.Reclamos_Motivo.ToList());
            db.SaveChanges();
            foreach (var reclamoMotivos in reclamosMotivo)
            {
                Reclamos_Motivo newReclamoMotivo = db.Reclamos_Motivo.FirstOrDefault<Reclamos_Motivo>(x => x.Id == reclamoMotivos.Id) ?? new Reclamos_Motivo() { Motivo_Id = 0 };
                bool add = newReclamoMotivo.Motivo_Id == 0;
                newReclamoMotivo.Motivo_Id = reclamoMotivos.Motivo_Id;
                newReclamoMotivo.Descripcion = reclamoMotivos.Descripcion;
                newReclamoMotivo.Habilitado = reclamoMotivos.Habilitado;
                newReclamoMotivo.Id = reclamoMotivos.Id;
                if (add)
                    db.Reclamos_Motivo.Add(newReclamoMotivo);
            }
            db.SaveChanges();
            return Ok(true);
        }
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostGuardarReclamosTipos(List<Reclamos_Tipo> reclamosTipo)
        {
            db.Reclamos_Tipo.RemoveRange(db.Reclamos_Tipo.ToList());
            db.SaveChanges();
            foreach (var reclamoTipo in reclamosTipo)
            {
                Reclamos_Tipo newReclamoTipo = db.Reclamos_Tipo.FirstOrDefault<Reclamos_Tipo>(x => x.Id == reclamoTipo.Id) ?? new Reclamos_Tipo() { Tipo_Id = 0 };
                bool add = newReclamoTipo.Tipo_Id == 0;
                newReclamoTipo.Tipo_Id = reclamoTipo.Tipo_Id;
                newReclamoTipo.Descripcion = reclamoTipo.Descripcion;
                newReclamoTipo.Habilitado = reclamoTipo.Habilitado;
                newReclamoTipo.Id = reclamoTipo.Id;
                if (add)
                    db.Reclamos_Tipo.Add(newReclamoTipo);

            }
            db.SaveChanges();
            return Ok(true);
        }
        [ResponseType(typeof(List<Reclamos_Clase>))]
        public IHttpActionResult GetReclamosClase()
        {
            return Ok(db.Reclamos_Clase.ToList<Reclamos_Clase>());
        }
        [ResponseType(typeof(List<Reclamos_Motivo>))]
        public IHttpActionResult GetReclamosMotivo()
        {
            return Ok(db.Reclamos_Motivo.ToList<Reclamos_Motivo>());
        }
        [ResponseType(typeof(List<Reclamos_Tipo>))]
        public IHttpActionResult GetReclamosTipo()
        {
            return Ok(db.Reclamos_Tipo.ToList<Reclamos_Tipo>());
        }

        [ResponseType(typeof(Point))]
        public IHttpActionResult PostPuntoDeDireccion(GetPointRequest request)
        {
            Point result = new Point();
            //result = db.PKG_Reclamos_Diarios.PostPuntoDeDireccion(request);
            return Ok(result);
        }

        [ResponseType(typeof(long))]
        public IHttpActionResult PostUbicacionPloteo(GetPlanoDetalleRequest request)
        {
            UbicacionPloteo ubicacion = new UbicacionPloteo();
            ubicacion.Distrito = request.Distrito;
            ubicacion.Calle = request.Calle;
            ubicacion.Altura = request.Numero;
            ubicacion.Interseccion = request.Interseccion;
            ubicacion.Manzana = request.Manzana;
            //ubicacion.Servicio = request.Servicio;
            ubicacion.FechaAlta = DateTime.Now;

            IList<int> sequence = db.Database.SqlQuery<int>("select RC_UBICACION_PLOTEO_SEQ.nextval from dual").ToList();
            ubicacion.IdUbicacionPloteo = sequence[0];

            db.Reclamos_UbicacionPloteo.Add(ubicacion);
            db.SaveChanges();

            return Ok(ubicacion.IdUbicacionPloteo);
        }
        [ResponseType(typeof(int))]
        public IHttpActionResult GetIdPlantilla(int idPlantillaCategoria)
        {
            int result = -1;

            Plantilla plantilla = db.Plantillas.First<Plantilla>(a => a.IdPlantillaCategoria == idPlantillaCategoria && a.FechaBaja == null);
            result = plantilla.IdPlantilla;

            return Ok(result);
        }
        [ResponseType(typeof(int))]
        public IHttpActionResult GetIdPlantillaFondo(int idPlantilla)
        {
            int result = -1;

            PlantillaFondo plantilla = db.PlantillaFondos.First<PlantillaFondo>(a => a.IdPlantilla == idPlantilla && a.FechaBaja == null);
            result = plantilla.IdPlantillaFondo;

            return Ok(result);
        }


    }
}
