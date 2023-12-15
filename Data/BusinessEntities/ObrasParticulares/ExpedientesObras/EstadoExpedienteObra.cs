using System;
using GeoSit.Data.BusinessEntities.Interfaces;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class EstadoExpedienteObra : IEntity
    {
        public long ExpedienteObraId { get; set; }

        public long EstadoExpedienteId { get; set; }

        public DateTime Fecha { get; set; }

        public string Observaciones { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de Navegación 
        public ExpedienteObra ExpedienteObra { get; set; }

        public EstadoExpediente EstadoExpediente { get; set; }
    }
}
