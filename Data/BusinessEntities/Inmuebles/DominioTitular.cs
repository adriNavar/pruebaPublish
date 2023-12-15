using System;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class DominioTitular
    {
        public long IdDominioTitular { get; set; }
        public long DominioID { get; set; }

        public long PersonaID { get; set; }

        public long? TipoPersonaID { get; set; }

        public short? TipoTitularidadID { get; set; }

        public decimal PorcientoCopropiedad { get; set; }

        public long UsuarioAltaID { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionID { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaID { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedaddes de Navegación
        public Dominio Dominio { get; set; }

        public Persona Persona { get; set; }        

        public TipoTitularidad TipoTitularidad { get; set; }

    }
}
