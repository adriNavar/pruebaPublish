using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    public class LiquidacionesSeguimiento
    {
        public String Padron { get; set; }
        public String Expediente { get; set; }
        public DateTime Fecha { get; set; }
        public string Numero { get; set; }
        public double Importe { get; set; }
        public string Observaciones { get; set; }
    }
}
