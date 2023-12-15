using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class INMMejora
    {
        public long IdMejora { get; set; }
        public long? IdEstadoConservacion { get; set; }
        public long? IdDestinoMejora { get; set; }
        public long IdDeclaracionJurada { get; set; }
        //public long? Ampliacion { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public EstadosConservacion EstadoConservacion { get; set; }
        public INMDestinoMejora DestinoMejora { get; set; }
        public DDJJ DeclaracionJurada { get; set; }

        public ICollection<INMMejoraOtraCar> OtrasCar { get; set; }

        public ICollection<INMMejoraCaracteristica> MejorasCar { get; set; }

    }
}
