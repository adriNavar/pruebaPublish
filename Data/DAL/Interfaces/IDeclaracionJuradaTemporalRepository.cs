using GeoSit.Data.BusinessEntities.Temporal;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDeclaracionJuradaTemporalRepository
    {
        DDJJTemporal GetById(long id, int idTramite);
        IEnumerable<Tuple<long, DDJJTemporal>> GetDDJJByTramite(int tramite);
    }
}
