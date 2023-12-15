using System;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class Division : IEntity
    {
        public long FeatId { get; set; }
        public long TipoDivisionId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Descripcion { get; set; }
        public string Nomenclatura { get; set; }
        public string WKT { get; set; }
        public long? ObjetoPadreId { get; set; }
        public string PlanoId { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionId { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaId { get; set; }
        public DateTime? FechaBaja { get; set; }

        public TipoDivision TipoDivision { get; set; }
        public Objeto Localidad { get; set; }
    }
}
