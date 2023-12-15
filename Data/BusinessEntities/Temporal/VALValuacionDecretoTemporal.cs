using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class VALValuacionDecretoTemporal : ITemporalTramite
    {
        public long IdValuacion { get; set; }
        public long IdDecreto { get; set; }
        public int IdTramite { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
