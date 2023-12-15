using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IComprobantePagoRepository
    {
        MEComprobantePago GetById(long id);

        IEnumerable<MEComprobantePago> GetByTramite(int tramite);
    }
}
