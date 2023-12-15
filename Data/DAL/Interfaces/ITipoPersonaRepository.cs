using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoPersonaRepository
    {
        IEnumerable<TipoPersona> GetTipoPersonas();
    }
}
