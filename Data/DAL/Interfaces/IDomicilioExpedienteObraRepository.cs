using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDomicilioExpedienteObraRepository
    {
        IEnumerable<DomicilioExpedienteObra> GetDomicilioExpedienteObras(long idExpedienteObra);

        IEnumerable<UbicacionExpedienteObra> GetUbicacionExpedienteObras(long idExpedienteObra);

        IEnumerable<DomicilioExpedienteObra> GetDomicilioExpedienteObras(long idDomicilio, long idExpedienteObra);

        DomicilioExpedienteObra GetDomicilioExpedienteObraById(long idDomicilio, long idExpedienteObra);

        long GetDomicilioExpedienteObraIdByUnidadTributariaId(long idUnidadTributaria);

        long GetUnidadTributariaExpedienteObraIdByDomicilioId(long idDomicilio);

        void InsertDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra, ExpedienteObra expedienteObra);

        void InsertDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra);

        void UpdateDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra);

        void DeleteDomicilioExpedienteObra(long idDomicilio, long idExpedienteObra);

        void DeleteDomiciliosByExpedienteObraId(long idExpedienteObra);

        void DeleteDomicilioExpedienteObra(DomicilioExpedienteObra domicilioExpedienteObra);
    }
}
