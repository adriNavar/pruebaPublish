

using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class VIRVbEuTipoEdif : IEntity
    {
        public int IdTipo { get; set; }
        public string TipoDescripcion { get; set; }
        public string IdFuenteBase { get; set; }
        public string IdFuenteActualizadaAnual { get; set; }
        public double? CostoBase { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long? IdUsuBaja { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? IdUsuAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? IdUsuModificacion { get; set; }
        public int IdTipoEdif { get; set; }
        public int? IdUso { get; set; }
    }
}
