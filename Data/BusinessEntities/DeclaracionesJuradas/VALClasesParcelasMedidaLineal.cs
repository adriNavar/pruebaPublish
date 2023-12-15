using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALClasesParcelasMedidaLineal
    {
        public long IdClasesParcelasMedidaLineal { get; set; }
        public long IdClaseParcela { get; set; }
        public long IdTipoMedidaLineal { get; set; }
        public long? Orden { get; set; }
        public int? RequiereLongitud { get; set; }
        public int? RequiereAforo { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public VALClasesParcelas ClaseParcela { get; set; }
        public VALTiposMedidasLineales TipoMedidaLineal { get; set; }
        public ICollection<DDJJUMedidaLineal> MedidasLineales { get; set; }

    }
}
