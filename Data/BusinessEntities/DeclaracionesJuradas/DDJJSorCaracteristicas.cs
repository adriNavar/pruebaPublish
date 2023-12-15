using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJSorCaracteristicas
    {
        public long IdSorCaracteristica { get; set; }
        public long IdSorTipoCaracteristica { get; set; }
        public long Numero { get; set; }
        public string Descripcion { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJSorTipoCaracteristica TipoCaracteristica { get; set; }


        public ICollection<VALAptCar> AptCar { get; set; }

    }
}
