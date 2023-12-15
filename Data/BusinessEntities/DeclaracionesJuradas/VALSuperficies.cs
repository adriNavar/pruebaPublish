using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALSuperficies
    {
        public long IdSuperficie { get; set; }
        public long IdAptitud { get; set; }
        public long IdSor { get; set; }
        public double? Superficie { get; set; }      

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJSor Sor { get; set; }
        public VALAptitudes Aptitud { get; set; }


    }
}
