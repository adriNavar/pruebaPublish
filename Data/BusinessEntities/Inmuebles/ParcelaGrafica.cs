using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ParcelaGrafica : IEntity
    {
        public long FeatID { get; set; }
        public long? ParcelaID { get; set; }
        //public DbGeometry Geometry { get; set; }
        public string IdOrigen { get; set; }
        public long? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long? ClassID { get; set; }
        public long RevisionNumber { get; set; }
    }
}
