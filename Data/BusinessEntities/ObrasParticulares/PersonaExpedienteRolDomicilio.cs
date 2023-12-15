namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class PersonaExpedienteRolDomicilio
    {
        public long PersonaInmuebleId { get; set; }

        public long ExpedienteObraId { get; set; }

        public long RolId { get; set; }

        public string Rol { get; set; }

        public string NombreCompleto { get; set; }

        public string DomicilioFisico { get; set; }
    }
}
