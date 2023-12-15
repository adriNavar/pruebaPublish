using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoExpedienteRepository
    {
        IEnumerable<TipoExpediente> GetTipoExpedientes();

        IEnumerable<TipoExpediente> GetTipoExpedientes(long idExpedienteObra);

        TipoExpediente GetTipoExpedienteById(long idTipoExpediente);

        void InsertTipoExpediente(long idTipoExpediente);

        void UpdateTipoExpediente(TipoExpediente tipoExpediente);

        void DeleteTipoExpediente(long idTipoExpediente);

        void DeleteTipoExpediente(TipoExpediente tipoExpediente);
    }
}
