using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEPrioridadTipo
    {
        public int IdPrioridadTipo { get; set; }
        public int IdTipoTramite { get; set; }
        public int IdPrioridadTramite { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public MEPrioridadTramite PrioridadTramite { get; set; }
        public METipoTramite TipoTramite { get; set; }
    }
}
