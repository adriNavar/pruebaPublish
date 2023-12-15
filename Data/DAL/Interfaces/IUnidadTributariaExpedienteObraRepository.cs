using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IUnidadTributariaExpedienteObraRepository
    {
        IEnumerable<UnidadTributaria> GetUnidadTributariaExpedienteObrasByIdExpedienteObra(long idExpedienteObra);

        UnidadTributariaExpedienteObra GetUnidadTributariaExpedienteObraById(long idExpedienteObra, long idUnidadTributaria);

        UnidadTributariaExpedienteObra GetUnidadTributariaExpedienteObraById(long idUnidadTributaria);

        void InsertUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra, ExpedienteObra expedienteObra);

        void InsertUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra);

        void UpdateUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra);

        void DeleteUnidadTributariaExpedienteObra(long idExpediente, long idUnidadTributaria);

        void DeleteUnidadTributariasByExpedienteObraId(long idExpedienteObra);

        void DeleteUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra);
    }
}
