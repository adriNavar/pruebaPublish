using GeoSit.Data.BusinessEntities.Temporal;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDominioTemporalRepository
    {
        DominioTemporal GetDominio(int idDominio, int tramite);
        DominioTitularTemporal GetDominioTitular(int idPersona, int tramite);
        IEnumerable<DominioTemporal> GetDominiosByTramite(int tramite);
    }
}
