using System;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class UnidadTributariaPersona
    {
        public long UnidadTributariaID { get; set; }

        public long PersonaID { get; set; }

        public long UsuarioAltaID { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionID { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaID { get; set; }

        public DateTime? FechaBaja { get; set; }

        public long TipoPersonaID { get; set; }

        public long PersonaSavedId { get; set; }

        /*Propiedades de Navegacion*/
        public Persona Persona { get; set; }

        public UnidadTributaria UnidadTributaria { get; set; }

        public TipoPersona TipoPersona { get; set; }

        public string CodSistemaTributario { get; set; }
    }
}
