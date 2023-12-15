using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Reportes.Api.Models
{
    public class MEListadoGeneralTramitesModel
    {
        //public int IdTramite { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Numero { get; set; }
        public string TipoTramite { get; set; }
        public string ObjetoTramite { get; set; }
        public string Iniciador { get; set; }
        public string Destinatario { get; set; }
        public string Estado { get; set; }
    }
}