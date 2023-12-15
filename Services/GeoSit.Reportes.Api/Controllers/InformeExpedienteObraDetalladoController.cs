using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Reportes.Api.Reportes;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities;
using System.Collections;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Valuaciones;
using DevExpress.XtraReports.UI;
using System.IO;
using GeoSit.Reportes.Api.Models;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeExpedienteObraDetalladoController : ApiController
    {
        // GET api/ExpedienteObra/
        private HttpClient cliente = new HttpClient();
        public IHttpActionResult GetInformeDetallado(string id, string tipo, string usuario)
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

                ExpedienteObra expObra = new ExpedienteObra();

                expObra = resp.Content.ReadAsAsync<ExpedienteObra>().Result;
                var model = new ExpedienteObraModel();
                model.Expedientes = new List<Expediente>();

                foreach (var tipoExpediente in expObra.TipoExpedienteObras)
                {
                    model.Expedientes.Add(new Expediente()
                    {
                        NumeroExpediente = expObra.NumeroExpediente,
                        NumeroLegajo = expObra.NumeroLegajo,
                        FechaExpediente = expObra.FechaExpediente,
                        TipoExpediente = tipoExpediente.TipoExpediente.Descripcion,
                        EstadoExpediente = expObra.EstadoExpedienteObras.OrderByDescending(x => x.Fecha).FirstOrDefault(x => x.ExpedienteObraId == tipoExpediente.ExpedienteObraId).EstadoExpediente.Descripcion
                    });
                }
                

                model.Observaciones = expObra.ControlTecnicos;
                model.UnidadTributariaExpedienteObras = expObra.UnidadTributariaExpedienteObras;

                /*foreach (var ut in model.UnidadTributariaExpedienteObras)
                {
                    resp = cliente.GetAsync("api/parcela/Get/" + ut.UnidadTributaria.ParcelaID).Result;
                    resp.EnsureSuccessStatusCode();
                    ut.UnidadTributaria.Parcela = resp.Content.ReadAsAsync<Parcela>().Result;
                }*/

                ArrayList datos = new ArrayList();
                datos.Add(model);

                //parcela ¿??

                foreach (var ut in expObra.UnidadTributariaExpedienteObras)
                {
                    resp = cliente.GetAsync("api/parcela/Get/" + ut.UnidadTributaria.ParcelaID).Result;
                    resp.EnsureSuccessStatusCode();
                    ut.UnidadTributaria.Parcela = resp.Content.ReadAsAsync<Parcela>().Result;
                }

                //ubicaciones
                resp = cliente.GetAsync("api/domicilioexpedienteobra/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.UbicacionExpedienteObra = resp.Content.ReadAsAsync<IEnumerable<UbicacionExpedienteObra>>().Result.ToList();
                foreach( var ub in expObra.UbicacionExpedienteObra)
                {
                    if (ub.DomicilioPrimario is false)
                        ub.DomicilioPrimarioSiNo = "No";
                    else
                        ub.DomicilioPrimarioSiNo = "Sí";
                }


                //servicios
                resp = cliente.GetAsync("api/servicioexpedienteobra/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.ServicioExpedienteObras = resp.Content.ReadAsAsync<IEnumerable<ServicioExpedienteObra>>().Result.ToList();

                //ph y permiso provisorio
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.LoadXml(expObra.Atributos);
                var ph = xml.SelectSingleNode("/Datos/Ph").InnerText;
                var permisoProvisorio = xml.SelectSingleNode("/Datos/PermisosProvisorios").InnerText;

                if (ph == "false")
                    ph = "No";
                else
                    ph = "Sí";

                if (permisoProvisorio == "false")
                    permisoProvisorio = "No";
                else
                    permisoProvisorio = "Sí";
                

                //superficies
                resp = cliente.GetAsync("api/superficieexpedienteobra/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.TipoSuperficieExpedienteObras = resp.Content.ReadAsAsync<IEnumerable<TipoSuperficieExpedienteObra>>().Result.ToList();

                //personas
                resp = cliente.GetAsync("api/PersonaExpedienteObra/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.PersonaExpedienteRolDomicilio = resp.Content.ReadAsAsync<IEnumerable<PersonaExpedienteRolDomicilio>>().Result.ToList();

                //liquidaciones
                resp = cliente.GetAsync("api/Liquidacion/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.Liquidaciones = resp.Content.ReadAsAsync<IEnumerable<Liquidacion>>().Result.ToList();

                //control tecnico
                resp = cliente.GetAsync("api/controltecnicoexpedienteobra/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.ControlTecnicos= resp.Content.ReadAsAsync<IEnumerable<ControlTecnico>>().Result.ToList();

                //observaciones
                resp = cliente.GetAsync("api/observacion/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.ObservacionExpedientes = resp.Content.ReadAsAsync<IEnumerable<ObservacionExpediente>>().Result.ToList();

                //estados
                resp = cliente.GetAsync("api/estadoexpedienteobra/get/" + expObra.ExpedienteObraId).Result;
                resp.EnsureSuccessStatusCode();
                expObra.EstadoExpedienteObras = resp.Content.ReadAsAsync<IEnumerable<EstadoExpedienteObra>>().Result.ToList();

                var reporte = new InformeExpedienteObraDetallado();

                var footer = ConfigurationManager.AppSettings["descMunicipio"];

                byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeExpedienteObraDetallado(), expObra, ph, permisoProvisorio, footer, usuario);
                return Ok(Convert.ToBase64String(bytes));

                 
                
                 

                //reporte.DataSource = datos;
                //reporte.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                //reporte.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];

                //return Ok(ReporteHelper.ExportToPDF(reporte));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}