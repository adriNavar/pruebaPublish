using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class MensuraRelacionada : IEntity
    {
        public long IdMensuraRelacionada { get; set; }
        public long IdMensuraOrigen { get; set; }
        public long IdMensuraDestino { get; set; }

        public long? IdUsuarioAlta { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? IdUsuarioModif { get; set; }
        public DateTime? FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public Mensura MensuraOrigen { get; set; }

        public Mensura MensuraDestino { get; set; }

    }
}
