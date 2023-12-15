using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IPartidoRepository
    {
        IEnumerable<Partido> GetPartidos();

        Partido GetPartidoById(long idPartido);

        string GetRegionNombre(Componente componentePartido, long idPartido);
    }
}
