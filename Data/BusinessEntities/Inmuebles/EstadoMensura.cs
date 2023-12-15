using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class EstadoMensura
    {
        public long IdEstadoMensura { get; set; }
        public string Descripcion { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }


    }
}
