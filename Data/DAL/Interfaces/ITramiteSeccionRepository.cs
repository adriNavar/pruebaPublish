using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITramiteSeccionRepository
    {
        IEnumerable<TramiteSeccion> GetTramitesSecciones(long pIdTramiteSeccion);

        TramiteSeccion GetTramiteSeccionById(long pIdTramiteSeccion);

        IEnumerable<TramiteSeccion> GetTramiteSeccionByTramite(long pIdTramite);

        void InsertTramiteSeccion(TramiteSeccion mTramiteSeccion);

        void UpdateTramiteSeccion(TramiteSeccion mTramiteSeccion);
    }
}
