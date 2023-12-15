using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITramitePersonaRepository
    {
        IEnumerable<TramitePersona> GetTramitesPersonas(long pIdTramitePersona);

        TramitePersona GetTramitePersonaById(long pIdTramitePersona);

        IEnumerable<TramitePersona> GetTramitePersonaByTramite(long pIdTramite);

        void InsertTramitePersona(TramitePersona mTramitePersona);

        void DeleteTramitePersona(TramitePersona mTramitePersona);
    }
}
