using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Temporal;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITramiteRepository
    {

        IEnumerable<Tramite> GetTramites();

        Tramite GetTramiteById(long pIdTramite);

        MensuraTemporal GetTramiteMensura(long IdTramite);

        List<UnidadTributariaTramiteTemporal> GetTramiteUt(long IdTramite);

        void InsertTramite(Tramite mTramite);

        void UpdateTramite(Tramite mTramite);

        void DeleteTramite(Tramite mTramite);

        Tramite GetCertificadoByTipoCodigo(long idTipo, string codigo);

        IEnumerable<Tramite> GetTramitesByCriteria(long? pTipoId, long? pNumDesde, long? pNumHasta, string pFechaDesde, string pFechaHasta, string pEstadoId, int? pUnidadT, int? pIdTramite, string pIdentificador);
    }
}
