using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALPuntajesCaminos
    {
        public long IdPuntajeCamino { get; set; }
        public long IdCamino { get; set; }
        public float? DistanciaMinima { get; set; }
        public float DistanciaMaxima { get; set; }
        public double? Puntaje { get; set; }  

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }  
    }
}
