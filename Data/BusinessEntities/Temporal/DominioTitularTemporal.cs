using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DominioTitularTemporal : ITemporalTramite
    {
        public long DominioID { get; set; }
        public long PersonaID { get; set; }
        public short? TipoTitularidadID { get; set; }
        public decimal PorcientoCopropiedad { get; set; }
        public int IdTramite { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
