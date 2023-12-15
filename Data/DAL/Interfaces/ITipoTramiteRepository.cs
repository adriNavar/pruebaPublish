using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoTramiteRepository
    {
        IEnumerable<TipoTramite> GetTiposTramites();

        TipoTramite GetTipoTramiteById(long idTipoTramite);

        void InsertTipoTramite(TipoTramite mTrtTipoTramite);

        void InsertTipoTramite(TipoTramite mTrtTipoTramite, Hoja hoja);

        void UpdateTipoTramite(TipoTramite mTrtTipoTramite);

        void DeleteTipoTramite(long idTipoTramite);

        void DeleteTipoTramite(TipoTramite mTrtTipoTramite);
    }
}
