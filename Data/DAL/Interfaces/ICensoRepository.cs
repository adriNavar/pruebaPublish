using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.DAL.Repositories
{
    public interface ICensoRepository
    {
        IEnumerable<Censo> GetCensos();

    }
}
