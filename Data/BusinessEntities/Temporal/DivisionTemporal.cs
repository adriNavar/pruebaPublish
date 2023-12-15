using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Data.Spatial;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DivisionTemporal : ITemporalTramite
    {
        public long FeatId { get; set; }
        public int IdTramite { get; set; }
        public long TipoDivisionId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Descripcion { get; set; }
        public string Nomenclatura { get; set; }
        public long ObjetoPadreId { get; set; }
        public string PlanoId { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionId { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Nullable<int> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
    }
}
