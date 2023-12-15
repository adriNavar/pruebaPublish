using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Interfaces
{
    public class VistaKontaktar : IEntity
    {
        public string NroTramite {get; set;}
        public string Fecha {get; set;}
        public string Contribuyente {get; set;}
        public string DocContribuyente {get; set;}
        public string Imponible {get; set;}
        public string Observaciones { get; set; }
    }
}
