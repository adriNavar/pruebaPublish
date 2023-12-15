using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;
namespace GeoSit.Data.DAL.Interfaces
{
    public interface IVIRValuacionRepository
    {
        List<VIRValuacion> GetVIRValuacionesByIdInmueble(long idInmueble);
        VIRValuacion GetValuacionVigente(long id);
    }
}
