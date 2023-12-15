using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoExpedienteObraRepository
    {
        IEnumerable<TipoExpedienteObra> GetTipoExpedienteObras(long idExpedienteObra);

        TipoExpedienteObra GetTipoExpedienteObraById(long idExpedienteObra, long idTipoExpediente);

        void InsertTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra, ExpedienteObra expedienteObra);

        void InsertTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra);

        void UpdateTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra);

        void DeleteTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra);

        void DeleteTipoExpedientesByExpedienteObraId(long idExpedienteObra);
    }
}
