using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IVIRVbEuTipoEdifRepository
    {
        List<VIRVbEuTipoEdif> GetTipoEdif(int tipo);

        //List<VIRVbEuTipoEdif> GetTipoEdifByUso(string uso);
    }
}
