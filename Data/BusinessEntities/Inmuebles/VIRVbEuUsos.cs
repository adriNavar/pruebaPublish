

using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class VIRVbEuUsos : IEntity
    {
        public string Uso { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long? IdUsuBaja { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? IdUsuAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? IdUsuModificacion { get; set; }
        public int IdUso { get; set; }
    }
}
