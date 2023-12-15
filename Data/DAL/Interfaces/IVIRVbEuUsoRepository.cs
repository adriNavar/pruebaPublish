using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IVIRVbEuUsoRepository
    {
        List<VIRVbEuUsos> GetVIRVbEuUsos();
        VIRVbEuUsos GetVIRVbEuUsoByUso(string uso);
    }
}
