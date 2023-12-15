using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface ITipoPlanoRepository
    {
        IEnumerable<TipoPlano> GetTipoPlanos();

    }
}
