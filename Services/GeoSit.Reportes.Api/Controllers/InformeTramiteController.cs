using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Reportes.Api.Reportes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeTramiteController : ApiController
    {
        private HttpClient cliente = new HttpClient();

        public List<TipoTramite> GetTipos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TramitesCertificadosService/GetTipos").Result;
            resp.EnsureSuccessStatusCode();
            var lstTipoObjeto = (List<TipoTramite>)resp.Content.ReadAsAsync<IEnumerable<TipoTramite>>().Result;
            return lstTipoObjeto;
        }

        // GET api/informetramite
        public IHttpActionResult GetInforme(int id, string usuario)
        {
            try
            {

                //Tramite lstObjeto = resp.Content.ReadAsAsync<Tramite>().Result;
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                var resp = cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesCertificados?idTramite=" + id).Result;
                resp.EnsureSuccessStatusCode();
                var model = resp.Content.ReadAsAsync<Tramite>().Result;

                HttpResponseMessage respSecciones = cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesSecciones?idTramite=" + id).Result;
                respSecciones.EnsureSuccessStatusCode();
                List<TramiteSeccion> lstObjetoSecciones = respSecciones.Content.ReadAsAsync<IEnumerable<TramiteSeccion>>().Result.ToList();
                List<TramiteSeccion> lstSecciones = new List<TramiteSeccion>();
                if (lstObjetoSecciones != null)
                {
                    foreach (var trtTramiteSec in lstObjetoSecciones)
                    {
                        resp = cliente.GetAsync("api/TramitesCertificadosService/GetTiposSecciones?tipoTraId=" + model.Id_Tipo_Tramite).Result;
                        resp.EnsureSuccessStatusCode();
                        var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<TramiteTipoSeccion>>().Result.ToList();
                        if (lstObjeto.Count>0)
                        { 
                            var tipoSeccion = lstObjeto.Single(t => t.Id_Tipo_Tramite == model.Id_Tipo_Tramite && t.Id_Tipo_Seccion == trtTramiteSec.Id_Tipo_Seccion);
                            trtTramiteSec.TipoSeccion = tipoSeccion;

                            if (trtTramiteSec.Imprime)
                                lstSecciones.Add(trtTramiteSec);
                        }
                    }
                }
                model.Secciones = lstSecciones;

                // Determina las unidades tributarias asociadas al trámite.
                List<TramiteUnidadTributaria> lstUTs = new List<TramiteUnidadTributaria>();
                try
                {
                    HttpResponseMessage respUTs = cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesUTS?idTramite=" + id).Result;
                    respUTs.EnsureSuccessStatusCode();
                    var lstObjetoUTs = respUTs.Content.ReadAsAsync<IEnumerable<TramiteUnidadTributaria>>().Result.ToList();
                    foreach (var trtUTs in lstObjetoUTs)
                    {
                        HttpResponseMessage respParcela = cliente.GetAsync("api/parcela/getparcelabyid/" + trtUTs.UnidadTributaria.ParcelaID).Result;
                        respParcela.EnsureSuccessStatusCode();
                        var parcela = respParcela.Content.ReadAsAsync<Parcela>().Result;

                        trtUTs.UnidadTributaria.Parcela = parcela;

                        lstUTs.Add(trtUTs);
                    }
                }
                catch (Exception err)
                {
                    var error = err.Message;
                }

                HttpResponseMessage respPersonas = cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesPersonas?idTramite=" + id).Result;
                respPersonas.EnsureSuccessStatusCode();
                List<TramitePersona> lstObjetoPersona = respPersonas.Content.ReadAsAsync<IEnumerable<TramitePersona>>().Result.ToList();
                List<TramitePersona> lstPersonas = new List<TramitePersona>();
                if (lstObjetoPersona != null)
                {
                    foreach (var trtTramitePer in lstObjetoPersona)
                    {
                        HttpResponseMessage respPersona = cliente.GetAsync("api/PersonaService/GetPersonaById/" + trtTramitePer.Id_Persona).Result;
                        respPersona.EnsureSuccessStatusCode();
                        var persona = respPersona.Content.ReadAsAsync<Persona>().Result;

                        trtTramitePer.Persona = persona;
                        lstPersonas.Add(trtTramitePer);
                    }
                }
                model.Personas = lstPersonas;

                HttpResponseMessage respDoc = cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesDocumentos?idTramite=" + id).Result;
                resp.EnsureSuccessStatusCode();
                List<TramiteDocumento> lstObjetoDocs = respDoc.Content.ReadAsAsync<IEnumerable<TramiteDocumento>>().Result.ToList();
                List<Documento> lstDocs = new List <Documento>();

                if (lstDocs != null)
                {
                    foreach (var trtTramiteDoc in lstObjetoDocs)
                    {
                        HttpResponseMessage respDocumento = cliente.GetAsync("api/DocumentoService/GetDocumentoById/" + trtTramiteDoc.Id_Documento).Result;
                        respDocumento.EnsureSuccessStatusCode();
                        var documento = respDocumento.Content.ReadAsAsync<Documento>().Result;

                        lstDocs.Add(documento);
                    }
                }
                var tipo = GetTipos().Single(t => t.Id_Tipo_Tramite == model.Id_Tipo_Tramite);
                model.TipoTramite.Id_Tipo_Tramite = 0;
                model.TipoTramite.Nombre = tipo.Nombre;

                byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeTramite(), model, lstUTs, lstPersonas, lstDocs, usuario);
                return Ok(Convert.ToBase64String(bytes));
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                return NotFound();
            }
        }       
    }
}
