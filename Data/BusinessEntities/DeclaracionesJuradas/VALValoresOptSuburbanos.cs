using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALValoresOptSuburbanos
    {
        public long IdValOptSuburbano { get; set; }
        public long IdLocalidad { get; set; }
        public long? SuperficieMinima { get; set; }
        public long? SuperficieMaxima { get; set; }
        public double? Valor { get; set; }       

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
