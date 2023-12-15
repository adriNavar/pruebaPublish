using GeoSit.Reportes.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Reportes.Api.Controllers
{
    public class ReporteEstadoTramiteController : ApiController
    {
        private HttpClient cliente = new HttpClient();
        // GET: ReporteEstadoTramite
        public IHttpActionResult Index()
        {
            return null;
        }



        [HttpGet]
        public IHttpActionResult GetInformeEstadoTramite(string Id, string Numero, string Operacion, string usuario)
        {
            try
            {
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesCertificados").Result;

                if (resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = resp.Content.ReadAsStringAsync().Result;
                    return ResponseMessage(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Content = new StringContent(message)
                    });
                }

                List<Tramite> lista = resp.Content.ReadAsAsync<List<Tramite>>().Result;
                List<InformeEstadoTramite> modelos = new List<InformeEstadoTramite>();

                if (Operacion.Equals("T"))
                {
                    long idTramite = long.Parse(Numero);
                    try
                    {
                        InformeEstadoTramite model = new InformeEstadoTramite();
                        Tramite item = lista.Where(a => a.Nro_Tramite == idTramite && a.Id_Tipo_Tramite == long.Parse(Id)).First();
                        model.Tipo = item.TipoTramite.Nombre;
                        model.NroIdentificacion = item.Cod_Tramite;
                        model.Numero = item.Nro_Tramite.ToString();
                        model.FechaInicio = item.Fecha_Alta;
                        model.FechaUltimoEstado = item.Fecha_Modif;
                        switch (item.Estado)
                        {
                            case "1": model.EstadoActual = "Iniciado";
                                break;
                            case "2": model.EstadoActual = "En Proceso";
                                break;
                            case "3": model.EstadoActual = "Pendiente de Cierre";
                                break;
                            case "4": model.EstadoActual = "Finalizado";
                                break;
                            case "5": model.EstadoActual = "Anulado";
                                break;
                            default: model.EstadoActual = "";
                                break;
                        }
                        modelos.Add(model);

                    }
                    catch (System.Exception)
                    {
                        throw new HttpRequestException("No Existe Tramite para la Busqueda.");
                    }

                }

                if (Operacion.Equals("I"))
                {
                    try
                    {
                        InformeEstadoTramite model = new InformeEstadoTramite();
                        Tramite item = lista.Where(a => a.Cod_Tramite == Numero && a.Id_Tipo_Tramite == long.Parse(Id)).First();
                        model.Tipo = item.TipoTramite.Nombre;
                        model.NroIdentificacion = item.Cod_Tramite;
                        model.Numero = item.Nro_Tramite.ToString();
                        model.FechaInicio = item.Fecha;
                        model.FechaUltimoEstado = item.Fecha_Modif;
                        switch (item.Estado)
                        {
                            case "1": model.EstadoActual = "Iniciado";
                                break;
                            case "2": model.EstadoActual = "En Proceso";
                                break;
                            case "3": model.EstadoActual = "Pendiente de Cierre";
                                break;
                            case "4": model.EstadoActual = "Finalizado";
                                break;
                            case "5": model.EstadoActual = "Anulado";
                                break;
                            default: model.EstadoActual = "";
                                break;
                        }
                        modelos.Add(model);
                    }
                    catch (Exception)
                    {

                        return ResponseMessage(new HttpResponseMessage()
                        {
                            StatusCode = System.Net.HttpStatusCode.BadRequest,
                            Content = new StringContent("No se encontro un trámite para los datos ingresados")
                        });
                    }

                }


                InformeEstadoReporte reporte = new InformeEstadoReporte();
                reporte.DataSource = modelos;
                //reporte.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                //reporte.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];

                return Ok(ReporteHelper.ExportToPDF(ReporteHelper.SetLogoUsuario(reporte, usuario)));
            }
            catch (Exception ex)
            {
                return ResponseMessage(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(ex.Message)
                });
            }

        }
    }
}