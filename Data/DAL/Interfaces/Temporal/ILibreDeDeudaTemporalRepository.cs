using GeoSit.Data.BusinessEntities.Temporal;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ILibreDeDeudaTemporalRepository
    {
        INMLibreDeDeudaTemporal GetLibreDeDeuda(int idLibreDeuda, int tramite);
        IEnumerable<INMLibreDeDeudaTemporal> GetLibresDeDeudaByTramite(int tramite);
    }
}