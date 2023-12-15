using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Reportes.Api.Reportes;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeCatastralController : ApiController
    {
        // GET api/informecatastral
        private readonly HttpClient _cliente = new HttpClient();
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public InformeCatastralController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public IHttpActionResult GetInforme(int id)
        {
            try
            {                
                var resp = _cliente.GetAsync("api/Parcela/Get/" + id).Result;
                resp.EnsureSuccessStatusCode();
                var model = resp.Content.ReadAsAsync<Parcela>().Result;

                //Esto se puede quitar cuando la tabla inm_nomenclatura tenga FechaAlta y FechaModificacion
                //está en todos los reportes
                foreach (var nomenc in model.Nomenclaturas)
                {
                    nomenc.FechaAlta = model.FechaAlta;
                    nomenc.FechaModificacion = model.FechaModificacion;
                }                

                var result = _cliente.GetAsync("api/parcela/getzonificacion?idparcela=" + id).Result;
                result.EnsureSuccessStatusCode();
                model.Zonificacion = result.Content.ReadAsAsync<Zonificacion>().Result;

                if (model.Zonificacion != null)
                {
                    result = _cliente.GetAsync("api/parcela/getatributoszonificacion?idparcela=" + id).Result;
                    result.EnsureSuccessStatusCode();
                    model.Zonificacion.AtributosZonificacion = result.Content.ReadAsAsync<IEnumerable<AtributosZonificacion>>().Result
                        .OrderBy(x => x.Descripcion);
                }
                //model.Ubicacion = Image.FromFile(GetParcelaUbicacion(id));

                var bytes = new byte[0]; //ReporteHelper.GenerarReporte(new InformeCatastral(), model);
                return Ok(bytes);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                return NotFound();
            }
        }

        public string GetParcelaUbicacion(long idParcela)
        {
            return Path.Combine(_uploadPath, "ubicacion.png");
        }
    }
}
