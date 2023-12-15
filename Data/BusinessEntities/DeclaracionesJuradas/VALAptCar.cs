using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALAptCar
    {
        public long IdAptCar { get; set; }
        public long IdSorCar { get; set; }
        public long IdAptitud { get; set; }
        public long Puntaje { get; set; }
      

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public VALAptitudes Aptitud { get; set; }
        public DDJJSorCaracteristicas SorCaracteristica { get; set; }

        public ICollection<DDJJSorCar> SorCar { get; set; }

    }
}
