using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITramiteDocumentoRepository
    {
        IEnumerable<TramiteDocumento> GetTramitesDocumentos(long pIdTramiteDocumento);

        TramiteDocumento GetTramiteDocumentoById(long pIdTramiteDocumento);

        IEnumerable<TramiteDocumento> GetTramiteDocumentoByTramite(long pIdTramite);

        void InsertTramiteDocumento(TramiteDocumento mTramiteDocumento);

        void DeleteTramiteDocumento(TramiteDocumento mTramiteDocumento);
    }
}
