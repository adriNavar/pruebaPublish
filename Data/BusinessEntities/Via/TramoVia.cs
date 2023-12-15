using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System;

namespace GeoSit.Data.BusinessEntities.Via
{
    public class TramoVia : IEntity
    {
        public long TramoViaId { get; set; }
        public long ViaId { get; set; }
        public long? AlturaDesde { get; set; }
        public long? AlturaHasta { get; set; }
        public string Sufijo { get; set; }
        public string Paridad { get; set; }
        public string Cpa { get; set; }
        public string Geometry { get; set; }
        public string Atributos { get; set; }
        public long ObjetoPadreId { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModifId { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBajaId { get; set; }
        public DateTime? FechaBaja { get; set; }

        public double? Aforo { get; set; }
                

        public Via Via { get; set; }
        public Objeto Localidad { get; set; }
    }
}
