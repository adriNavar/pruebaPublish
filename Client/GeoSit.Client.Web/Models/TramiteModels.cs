using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Client.Web.Models
{
    public class TramiteModels
    {
        public TramiteModels()
        {
            TramitesList = new List<TipoTramite>();
        }
        public long NumeroDeTramite { get; set; }
        public long Identificador { get; set; }
        public string Numero { get; set; }
        public string TipoDeTramite { get; set; }
        public string Operacion { get; set; }

        public List<TipoTramite> TramitesList { get; set; }
    }
  
}