using GeoSit.Reportes.Api.Helpers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Linq;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformesMesaEntradasController : ApiController
    {
        private readonly HttpClient cliente = new HttpClient();

        public InformesMesaEntradasController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }
        //public METramite GetTramiteById(int idTramite)
        //{
        //    cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        //    HttpResponseMessage resp = cliente.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
        //    resp.EnsureSuccessStatusCode();
        //    METramite tramite = (METramite)resp.Content.ReadAsAsync<METramite>().Result;
        //    return tramite;
        //}
        private Usuarios GetUsuarioById(long idUsuario)
        {
            using (var resp = cliente.GetAsync("api/SeguridadService/GetUsuarioById/" + idUsuario).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<Usuarios>().Result;
            }
        }

        private string GetPersonaByIdUnidadTributaria(long idUnidadTributaria)
        {
            var resp = cliente.GetAsync("api/MesaEntradas/GetPersonaByIdUt?idUnidadTributaria=" + idUnidadTributaria).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<string>().Result;
        }

        // GET api/mesaentrada
        public IHttpActionResult GetInformeCaratulaExpediente(int id, long idUsuario)
        {
            try
            {
                using (cliente)
                using (var resp = cliente.GetAsync($"api/MesaEntradas/Tramites/{id}").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    var model = resp.Content.ReadAsAsync<METramite>().Result;

                    string persona = "";
                    if (model.IdUnidadTributaria != null)
                    {
                        persona = GetPersonaByIdUnidadTributaria(model.IdUnidadTributaria.Value);
                    }
                    
                    var usuario = GetUsuarioById(idUsuario);

                    byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.MECaratulaExpediente(), model, persona, $"{usuario.Nombre} {usuario.Apellido}".Trim());
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GetInformeCaratulaExpediente(id:{id},usuario:{idUsuario})", ex);
                return NotFound();
            }
        }

        public IHttpActionResult GetInformeDetallado(int id, long idUsuario)
        {
            try
            {
                using (cliente)
                using (var resp = cliente.GetAsync($"api/MesaEntradas/Tramites/{id}").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    var model = resp.Content.ReadAsAsync<METramite>().Result;

                    var usuario = GetUsuarioById(idUsuario);

                    byte[] bytes = ReporteHelper.GenerarReporteInformeDetallado(new Reportes.MEInformeDetallado(), model, $"{usuario.Nombre} {usuario.Apellido}".Trim());
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GetInformeDetallado(id:{id},usuario:{idUsuario})", ex);
                return NotFound();
            }
        }

        public IHttpActionResult GetInformeObservaciones(int id, long idUsuario)
        {
            try
            {
                using (cliente)
                using (var resp = cliente.GetAsync($"api/MesaEntradas/Tramites/{id}").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    var model = resp.Content.ReadAsAsync<METramite>().Result;

                    var usuario = GetUsuarioById(idUsuario);

                    byte[] bytes = ReporteHelper.GenerarReporteObservaciones(new Reportes.MEInformeObservaciones(), model, $"{usuario.Nombre} {usuario.Apellido}".Trim());
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GetInformeDetallado(id:{id},usuario:{idUsuario})", ex);
                return NotFound();
            }
        }

        public IHttpActionResult GetInformeRemito(int idRemito, long idUsuario)
        {
            try
            {
                using (cliente)
                using (var resp = cliente.GetAsync($"api/MesaEntradas/Remitos/{idRemito}").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    var remito = resp.Content.ReadAsAsync<MERemito>().Result;

                    var usuario = GetUsuarioById(idUsuario);

                    byte[] bytes = ReporteHelper.GenerarReporteRemito(new Reportes.MERemitoReporte(), remito, $"{usuario.Nombre} {usuario.Apellido}".Trim());
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GetInformeRemito(remito:{idRemito},usuario:{idUsuario})", ex);
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult GenerarInformeGeneralTramites(long idUsuario, Dictionary<string, string> valores)
        {
            try
            {
                using (cliente)
                using (var respData = cliente.PostAsync("api/MesaEntradas/Tramites/TramitesByFiltro", valores, new JsonMediaTypeFormatter()).Result)
                {
                    respData.EnsureSuccessStatusCode();
                    var tramites = respData.Content.ReadAsAsync<List<METramite>>().Result;

                    var usuario = GetUsuarioById(idUsuario);

                    byte[] bytes = ReporteHelper.GenerarReporteGeneralTramites(new Reportes.MEListadoGeneralTramites(), valores, tramites, $"{usuario.Nombre} {usuario.Apellido}".Trim());
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInformeGeneralTramites(valores,usuario:{idUsuario})", ex);
                return NotFound();
            }
        }

        [HttpGet]
        public IHttpActionResult GetInformePendientesConfirmar(long idUsuario)
        {
            try
            {
                using (cliente)
                using (var resp = cliente.GetAsync("api/MesaEntradas/Tramites/TramitesPendientesConfirmar").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    var tramites = resp.Content.ReadAsAsync<List<METramite>>().Result;

                    var usuario = GetUsuarioById(idUsuario);

                    byte[] bytes = ReporteHelper.GenerarInformePendientesConfirmar(new Reportes.MEInformePendientesConfirmar(), tramites, $"{usuario.Nombre} {usuario.Apellido}".Trim());
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GetInformePendientesConfirmar(usuario:{idUsuario})", ex);
                return NotFound();
            }
        }
    }
}
