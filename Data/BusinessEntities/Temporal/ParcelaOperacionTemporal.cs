using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class ParcelaOperacionTemporal : IEntity, ITemporalTramite
    {
        public long ParcelaOperacionID { get; set; }
        public long TipoOperacionID { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public long? ParcelaOrigenID { get; set; }
        public long ParcelaDestinoID { get; set; }

        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBajaID { get; set; }
        public int IdTramite { get; set; }
    }
}
