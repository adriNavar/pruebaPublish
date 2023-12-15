using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using System.Configuration;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeZonaController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetInforme(string usuario, List<AtributosZonificacion> lista)
        {
            try
            {
                InformeZonaAtributos informe = new InformeZonaAtributos();
                informe.DataSource = lista;
                if(!string.IsNullOrEmpty(lista[0].Valor))
                    informe.Parameters["Observaciones"].Value = "Observaciones ";
                   
                //informe.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                //informe.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];
                return Ok(ReporteHelper.ExportToPDF(ReporteHelper.SetLogoUsuario(informe, usuario)));
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }
    }
}
