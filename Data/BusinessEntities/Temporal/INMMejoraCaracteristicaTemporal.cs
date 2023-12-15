using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class INMMejoraCaracteristicaTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdMejora { get; set; }
        public long IdCaracteristica { get; set; }
        public int IdTramite { get; set; }


        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public INMCaracteristica Caracteristica { get; set; }
    }
}
