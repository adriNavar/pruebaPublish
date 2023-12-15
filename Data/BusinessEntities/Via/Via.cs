using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Via
{
    public class Via : AuditableEntity
    {
        public long ViaId { get; set; }

        public long TipoViaId { get; set; }

        public long? FeatId { get; set; }

        public string Nombre { get; set; }

        public string PlanoId { get; set; }

        public double? Aforo { get; set; }

        public TipoVia Tipo { get; set; }

        public ICollection<TramoVia> Tramos { get; set; }
    }
}
