using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class ParcelaMensuraTemporal : ITemporalTramite
    {
        public long IdParcelaMensura { get; set; }
        public long IdParcela { get; set; }
        public int IdTramite { get; set; }
        public long IdMensura { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        public MensuraTemporal Mensura { get; set; }

    }
}
