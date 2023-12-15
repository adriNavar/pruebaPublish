using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Web.Api.Solr;
using GeoSit.Data.DAL.Repositories;

namespace GeoSit.Web.Api.Controllers
{
    public class ParcelaGraficaServiceController : ApiController
    {
        private readonly GeoSITMContext db = GeoSITMContext.CreateContext();
        private readonly ParcelaRepository parcelaRepository;

        public ParcelaGraficaServiceController()
        {
            parcelaRepository = new ParcelaRepository(db);
        }
        // GET api/ParcelaGrafica
        [ResponseType(typeof(ICollection<ParcelaGrafica>))]
        public IHttpActionResult GetParcelasGrafica()
        {
            return Ok(db.ParcelaGrafica.Where(a => (a.UsuarioBajaID == null || a.UsuarioBajaID == 0)).ToList());
        }

        // GET api/ParcelaGraficaById
        [ResponseType(typeof(ParcelaGrafica))]
        public IHttpActionResult GetParcelaGraficaById(long id)
        {
            ParcelaGrafica parcelaGraf = db.ParcelaGrafica.Where(a => a.FeatID == id && (a.UsuarioBajaID == null || a.UsuarioBajaID == 0)).FirstOrDefault();
            if (parcelaGraf == null)
            {
                return NotFound();
            }

            return Ok(parcelaGraf);
        }

        // GET api/ParcelaGraficaByParcelaId
        [ResponseType(typeof(ParcelaGrafica))]
        public IHttpActionResult GetParcelaGraficaByParcelaId(long id)
        {
            ParcelaGrafica parcelaGraf = db.ParcelaGrafica.Where(a => a.ParcelaID == id && (a.UsuarioBajaID == null || a.UsuarioBajaID == 0)).FirstOrDefault();
            if (parcelaGraf == null)
            {
                return NotFound();
            }

            return Ok(parcelaGraf);
        }


        [HttpPost]
        public IHttpActionResult ParcelaGrafica_Save(ParcelaGrafica parametros)
        {
            var pgraf = db.ParcelaGrafica.Find(parametros.FeatID);
            if (pgraf == null)
            {
                return Conflict();
            }
            var palfa = db.Parcelas.Find(parametros.ParcelaID ?? pgraf.ParcelaID);
            palfa.UsuarioModificacionID = parametros._Id_Usuario;
            palfa.FechaModificacion = DateTime.Now;

            string evento = Eventos.ModificacionParcelaGrafica;
            string tipoOperacion = TiposOperacion.Modificacion;
            string msg = "Se modificó la Parcela Gráfica con Exito";

            pgraf.UsuarioModificacionID = palfa.UsuarioModificacionID;
            pgraf.FechaModificacion = palfa.FechaModificacion;
            pgraf.ParcelaID = parametros.ParcelaID;
            try
            {
                db.SaveChanges(new Auditoria(parametros._Id_Usuario, evento, msg, parametros._Machine_Name,
                                             parametros._Machine_Name, Autorizado.Si, db.Entry(pgraf).OriginalValues?.ToObject(), pgraf,
                                             "ParcelaGrafica", 1, tipoOperacion));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ParcelaGraficaServiceController-ParcelaGrafica_Save", ex);
                return InternalServerError();
            }

            reindexarCambiosParcelarioGrafico();
            try
            {
                parcelaRepository.RefreshVistaMaterializadaParcela();

                return Ok();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ParcelaGraficaServiceController-ParcelaGrafica_Save", ex);
                return StatusCode(System.Net.HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpPost]
        public IHttpActionResult ParcelaGrafica_Delete(ParcelaGrafica parametros)
        {
            var activityInDb = db.ParcelaGrafica.Where(a => a.ParcelaID == parametros.ParcelaID).FirstOrDefault();
            ParcelaGrafica Valores = new ParcelaGrafica();
            if (activityInDb != null)
            {
                Valores.UsuarioAltaID = activityInDb.UsuarioAltaID;
                Valores.FechaAlta = activityInDb.FechaAlta;
                Valores.ClassID = activityInDb.ClassID;
                Valores.FeatID = activityInDb.FeatID;

                Valores.UsuarioModificacionID = parametros.UsuarioModificacionID;
                Valores.FechaModificacion = parametros.FechaModificacion;
                Valores.FechaBaja = parametros.FechaBaja;
                Valores.UsuarioBajaID = parametros.UsuarioBajaID;
                Valores.ParcelaID = parametros.ParcelaID;
                Valores.RevisionNumber = parametros.RevisionNumber;
                //Valores.Geometry = 

                db.Entry(activityInDb).CurrentValues.SetValues(Valores);
                db.Entry(activityInDb).State = EntityState.Modified;

                try
                {

                    db.SaveChanges(new Auditoria(Valores.UsuarioBajaID ?? 0, Eventos.BajaParcelagrafica, "Se elimino la Parcela Grafica con Exito", parametros._Machine_Name,
                                                 parametros._Machine_Name, Autorizado.Si, null, Valores, "ParcelaGrafica", 1, TiposOperacion.Baja));

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Global.GetLogger().LogError("ParcelaGraficaServiceController-ParcelaGrafica_Delete", ex);
                    return NotFound();
                }

                reindexarCambiosParcelarioGrafico();
                try
                {
                    parcelaRepository.RefreshVistaMaterializadaParcela();
                }
                catch (Exception ex)
                {
                    Global.GetLogger().LogError("ParcelaGraficaServiceController-ParcelaGrafica_Delete", ex);
                    return StatusCode(System.Net.HttpStatusCode.ExpectationFailed);
                }
            }
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Reindexar()
        {
            reindexarCambiosParcelarioGrafico();
            return Ok();
        }
        private void reindexarCambiosParcelarioGrafico()
        {
            //SolrUpdater.Instance.Enqueue(Entities.parcela);
            //SolrUpdater.Instance.Enqueue(Entities.parcelasaneable);
            //SolrUpdater.Instance.Enqueue(Entities.unidadtributaria);
            return;
        }
    }
}