using GeoSit.Data.BusinessEntities.Valuaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class INMMejoraCaracteristica
    {
        public long IdMejoraCaracteristica { get; set; }
        public long IdMejora { get; set; }
        public long IdCaracteristica { get; set; }


        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public INMMejora Mejora { get; set; }
        public INMCaracteristica Caracteristica { get; set; }

    }
}
