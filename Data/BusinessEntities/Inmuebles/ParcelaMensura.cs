using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ParcelaMensura : IEntity
    {
        public long IdParcelaMensura { get; set; }
        public long IdParcela { get; set; }
        public long IdMensura { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        
        public Mensura Mensura { get; set; }
        public Parcela Parcela { get; set; }

    }
}
