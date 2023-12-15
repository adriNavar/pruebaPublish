using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IVIRVbEuCoefEstadoRepository
    {
        List<VIRVbEuCoefEstado> GetVIRVbEuCoefEstados();
    }
}
