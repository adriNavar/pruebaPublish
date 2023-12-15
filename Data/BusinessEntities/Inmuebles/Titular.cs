using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class Titular
    {
        public short DominioPersonaId { get; set; }

        public long PersonaId { get; set; }

        public short? TipoTitularidadId { get; set; }

        public string NombreCompleto { get; set; }

        //public string TipoPersona { get; set; }

        public string TipoTitular { get; set; }

        public string TipoNoDocumento { get; set; }

        public decimal PorcientoCopropiedad { get; set; }

        public DateTime FechaAlta { get; set; } 
    }
}
