using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoInscripcionRepository
    {
        IEnumerable<TipoInscripcion> GetTipoInscripciones();
        TipoInscripcion GetTipoInscripcion(long id);
    }
}
