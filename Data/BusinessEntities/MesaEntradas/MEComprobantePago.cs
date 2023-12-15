using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEComprobantePago
    {
        public long IdComprobantePago { get; set; }
        public long? IdTipoTasa { get; set; }
        public long? IdTramite { get; set; }
        public string TipoTramiteDgr { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaLiquidacion { get; set; }
        public DateTime? FechaPago { get; set; }
        public string MedioPago { get; set; }
        public double? Importe { get; set; }
        public string EstadoPago { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
