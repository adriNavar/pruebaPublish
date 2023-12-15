using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IEstadoExpedienteRepository
    {
        IEnumerable<EstadoExpediente> GetEstadoExpedientes();
    }
}
