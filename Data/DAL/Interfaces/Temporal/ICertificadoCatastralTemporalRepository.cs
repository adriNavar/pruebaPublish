using GeoSit.Data.BusinessEntities.Temporal;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ICertificadoCatastralTemporalRepository
    {
        INMCertificadoCatastralTemporal GetCertificado(int idCertificado, int tramite);
        IEnumerable<INMCertificadoCatastralTemporal> GetCertificadosByTramite(int tramite);
    }
}
