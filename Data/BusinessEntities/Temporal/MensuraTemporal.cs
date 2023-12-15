using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class MensuraTemporal : ITemporalTramite
    {
        public long IdMensura { get; set; }
        public int IdTramite { get; set; }
        public long IdTipoMensura { get; set; }
        public long IdEstadoMensura { get; set; }
        public string Numero { get; set; }
        public string Letra { get; set; }
        public DateTime? FechaPresentacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string Descripcion { get; set; }
        public string MensurasRelacionadas { get; set; }
        public string Observaciones { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}