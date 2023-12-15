using GeoSit.Data.BusinessEntities.Temporal;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDivisionTemporalRepository
    {
        DivisionTemporal GetManzana(int idManzana, int tramite);
        IEnumerable<DivisionTemporal> GetByTramite(int tramite);
    }
}