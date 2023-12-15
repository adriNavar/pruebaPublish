using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoSuperficieExpedienteObraRepository
    {
        IEnumerable<TipoSuperficieExpedienteObra> GetTipoSuperficieExpedienteObras(long idExpedienteObra);

        TipoSuperficieExpedienteObra GetTipoSuperficieExpedienteObraById(long idExpedienteObraSuperficie);

        void InsertTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra, 
            ExpedienteObra expedienteObra);

        void InsertTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra);

        void UpdateTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra);

        void DeleteTipoSuperficiesByExpedienteObraId(long idExpedienteObra);

        void DeleteTipoSuperficieExpedienteObraById(long idExpedienteObraSuperficie);

        void DeleteTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra);
    }
}
