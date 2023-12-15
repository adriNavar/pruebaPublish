using System;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    public class ServicioExpedienteObra
    {
        public long ExpedienteObraId { get; set; }

        public long ServicioId { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de navegación
        public ExpedienteObra ExpedienteObra { get; set; }

        public Servicio Servicio { get; set; } 
    }
}
