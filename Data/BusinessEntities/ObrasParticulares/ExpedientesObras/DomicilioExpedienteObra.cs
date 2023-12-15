using System;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class DomicilioExpedienteObra : IEntity
    {
        public long DomicilioInmuebleId { get; set; }

        public long ExpedienteObraId { get; set; }

        public bool Primario { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de navegación
        public ExpedienteObra ExpedienteObra { get; set; }

        public Domicilio DomicilioInmueble { get; set; }
    }
}
