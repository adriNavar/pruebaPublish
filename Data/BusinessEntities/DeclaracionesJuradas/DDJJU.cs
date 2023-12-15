using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJU
    {
        public long IdU { get; set; }
        public long IdDeclaracionJurada { get; set; }
        public decimal? SuperficiePlano { get; set; }
        public decimal? SuperficieTitulo { get; set; }
        public int? AguaCorriente { get; set; }
        public int? Cloaca { get; set; }
        public long? NumeroHabitantes { get; set; }
        public byte[] Croquis { get; set; }
        public long? IdMensura { get; set; }       

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJ DeclaracionJurada { get; set; }

        public Mensura Mensuras { get; set; }
        public ICollection<DDJJUFracciones> Fracciones { get; set; }

    }
}
