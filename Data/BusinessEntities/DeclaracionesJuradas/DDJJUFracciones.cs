using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJUFracciones
    {
        public long IdFraccion { get; set; }
        public long IdU { get; set; }
        public long NumeroFraccion { get; set; }
     
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJU U { get; set; }

        public ICollection<DDJJUMedidaLineal> MedidasLineales { get; set; }
    }
}
