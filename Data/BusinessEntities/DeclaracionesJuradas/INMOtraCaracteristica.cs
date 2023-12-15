using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class INMOtraCaracteristica
    {
        public long IdOtraCar { get; set; }
        public long? IdVersion { get; set; }
        public long? Orden { get; set; }
        public bool? Requerido { get; set; }

        public bool EsReal { get; set; }
        public string Descripcion { get; set; }    

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        //public ICollection<INMCaracteristica> Caracteristicas { get; set; }

    }
}
