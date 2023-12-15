using GeoSit.Data.BusinessEntities.Temporal;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IUnidadTributariaTemporalRepository
    {
        UnidadTributariaTemporal GetById(long id, int idTramite);
        IEnumerable<UnidadTributariaTemporal> GetEntradasByIdTramite(int tramite);
    }
}
