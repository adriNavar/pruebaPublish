using System;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class ObservacionExpediente
    {
        public long ObservacionExpedienteId { get; set; }

        public long ExpedienteObraId { get; set; }

        public DateTime? Fecha { get; set; }

        public string Observaciones { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de navegación
        public ExpedienteObra ExpedienteObra { get; set; }
    }
}
