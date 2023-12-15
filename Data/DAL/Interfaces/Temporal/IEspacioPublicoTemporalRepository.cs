using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Temporal;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IEspacioPublicoTemporalRepository
    {
        EspacioPublicoTemporal GetEspacioPublicoById(long id, int tramite);
        IEnumerable<EspacioPublicoTemporal> GetEspaciosPublicosByTramite(int tramite);
    }
}