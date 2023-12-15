using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITramiteUnidadTributariaRepository
    {
        IEnumerable<TramiteUnidadTributaria> GetTramitesUTSs(long pIdTramiteUts);

        TramiteUnidadTributaria GetTramiteUTSById(long pIdTramiteUts);

        IEnumerable<TramiteUnidadTributaria> GetTramiteUTSByTramite(long pIdTramite);

        void InsertTramiteUTS(TramiteUnidadTributaria mTramiteUts);

        void DeleteTramiteUTS(TramiteUnidadTributaria mTramiteUts);
    }
}
