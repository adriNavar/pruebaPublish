using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class INMLibreDeDeudaTemporal : ITemporalTramite
    {

        public long IdLibreDeuda { get; set; }
        public int IdTramite { get; set; }
        public long IdUnidadTributaria { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVigencia { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long IdEnteEmisor { get; set; }
        public string NroCertificado { get; set; }
        public decimal Superficie { get; set; }
        public decimal Valuacion { get; set; }
        public decimal Deuda { get; set; }
    }
}
