using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoSeccionRepository
    {
        IEnumerable<TramiteTipoSeccion> GetTipoSeccions();

        TramiteTipoSeccion GetTipoSeccionById(long pIdTipoSeccion);

        void InsertTipoSeccion(TramiteTipoSeccion mTipoSeccion);

        void UpdateTipoSeccion(TramiteTipoSeccion mTipoSeccion);

        void DeleteTipoSeccion(long pIdTipoSeccion);

        void DeleteTipoSeccion(TramiteTipoSeccion mTipoSeccion);
    }
}
