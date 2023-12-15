using System;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class PersonaExpedienteObra : IEntity
    {
        public long PersonaInmuebleId { get; set; }

        public long ExpedienteObraId { get; set; }

        public long RolId { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de Navegación 
        public Persona PersonaInmueble { get; set; }

        public ExpedienteObra ExpedienteObra { get; set; }

        public Rol Rol { get; set; }
    }
}
