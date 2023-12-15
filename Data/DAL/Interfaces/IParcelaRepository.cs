using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Valuaciones;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IParcelaRepository
    {
        string GetNextPartida(long idTipo, long idJurisdiccion);
        Parcela GetParcelaById(long idParcela, bool completa = true, bool utsHistoricas = false);
        Parcela GetParcelaByFeatIdDGC(long featId);
        Zonificacion GetZonificacion(long idParcela, bool esHistorico = false);
        List<AtributosZonificacion> GetAtributosZonificacion(long idParcela);
        VALValuacion GetValuacionParcela(long idParcela, bool esHistorico = false);
        void InsertParcela(Parcela parcela);
        void UpdateParcela(Parcela parcela);
        IEnumerable<Objeto> GetParcelaValuacionZonas();
        Objeto GetParcelaValuacionZona(long idAtributoZona);
        List<string> GetPartidabyId(long parcelaId);
        ICollection<Mejora> GetMejorasByIdParcela(long idParcela);
        ParcelaSuperficies GetSuperficiesByIdParcela(long id, bool esHistorico = false);
        void RefreshVistaMaterializadaParcela();
        Dictionary<long, List<Objeto>> GetJurisdiccionesByDepartamentoParcela(long id);
        bool EsVigente(long id);
        Parcela GetParcelaByUt(long idUnidadTributaria);
    }
}