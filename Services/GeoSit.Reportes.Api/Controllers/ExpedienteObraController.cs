using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.Collections;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Models;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeExpedienteObraController : ApiController
    {
        // GET api/ExpedienteObra/
        private HttpClient cliente = new HttpClient();
        public IHttpActionResult GetExpediente(string id, string tipo, string usuario)
        {
            try
            {
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                HttpResponseMessage resp = new HttpResponseMessage();
                if (tipo.ToUpper().Equals("E"))
                {
                    resp = cliente.GetAsync("api/ExpedienteObra/GetInformePorExpediente?numero=" + id).Result;
                }

                if (tipo.ToUpper().Equals("L"))
                {
                    resp = cliente.GetAsync("api/ExpedienteObra/GetInformePorLegajo?numero=" + id).Result;
                }

                if (resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = resp.Content.ReadAsStringAsync().Result;
                    return ResponseMessage(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Content = new StringContent(message)
                    });
                }

                var result = resp.Content.ReadAsAsync<ExpedienteObra>().Result;
                var model = new ExpedienteObraModel();
                model.Expedientes = new List<Expediente>();

                foreach (var tipoExpediente in result.TipoExpedienteObras)
                {
                    model.Expedientes.Add(new Expediente() { 
                        NumeroExpediente = result.NumeroExpediente,
                        NumeroLegajo = result.NumeroLegajo,
                        FechaExpediente = result.FechaExpediente,
                        TipoExpediente = tipoExpediente.TipoExpediente.Descripcion,
                        EstadoExpediente = result.EstadoExpedienteObras.OrderByDescending(x => x.Fecha).FirstOrDefault(x => x.ExpedienteObraId == tipoExpediente.ExpedienteObraId).EstadoExpediente.Descripcion
                    });
                }

                model.Observaciones = result.ControlTecnicos;
                model.UnidadTributariaExpedienteObras = result.UnidadTributariaExpedienteObras;

                foreach (var ut in model.UnidadTributariaExpedienteObras)
                {
                    resp = cliente.GetAsync("api/parcela/Get/" + ut.UnidadTributaria.ParcelaID).Result;
                    resp.EnsureSuccessStatusCode();
                    ut.UnidadTributaria.Parcela = resp.Content.ReadAsAsync<Parcela>().Result;
                }

                ArrayList datos = new ArrayList();
                datos.Add(model);

                InformeExpedienteObra reporte = new InformeExpedienteObra();
                
                reporte.DataSource = datos;
                //reporte.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                //reporte.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];

                return Ok(ReporteHelper.ExportToPDF(ReporteHelper.SetLogoUsuario(reporte, usuario)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}