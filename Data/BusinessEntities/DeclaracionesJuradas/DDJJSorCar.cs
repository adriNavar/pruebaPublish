using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJSorCar
    {
        public long IdDDJJSorCar { get; set; }
        public long IdSor { get; set; }
        public long IdAptCar { get; set; }      

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public VALAptCar AptCar { get; set; }
        public DDJJSor Sor { get; set; }

    }
}
