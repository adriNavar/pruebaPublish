using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IServicioExpedienteObraRepository
    {
        IEnumerable<ServicioExpedienteObra> GetServicioExpedienteObras(long idExpedienteObra);

        ServicioExpedienteObra GetServicioExpedienteObraById(long idExpedienteObra, long idServicioExpediente);

        void InsertServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra, ExpedienteObra expedienteObra);

        void InsertServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra);

        void UpdateServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra);

        void DeleteServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra);

        void DeleteServicioExpedientesByExpedienteObraId(long idExpedienteObra);
    }
}
