using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class NomenclaturaTemporal : IEntity, ITemporalTramite
    {
        public long NomenclaturaID { get; set; }
        public long ParcelaID { get; set; }
        public int IdTramite { get; set; }
        public string Nombre { get; set; }
        public long TipoNomenclaturaID { get; set; }
        public long? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public ParcelaTemporal Parcela { get; set; }
    }
}
