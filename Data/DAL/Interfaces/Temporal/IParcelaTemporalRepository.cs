using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Temporal;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IParcelaTemporalRepository
    {
        ParcelaTemporal GetParcelaById(long idParcela, int tramite);
        List<ParcelaTemporal> GetEntradasByIdTramite(int tramite);
    }
}