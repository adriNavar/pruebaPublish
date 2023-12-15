using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IUnidadTributariaRepository
    {
        ICollection<UnidadTributaria> GetUnidadesTributarias();
        ICollection<UnidadTributaria> GetUnidadesTributariasByParcela(long idParcela, bool incluirTitulares = false, bool esHistorico = false);
        UnidadTributaria GetUnidadTributariaByParcela(long idParcela);
        ICollection<UnidadTributaria> GetUnidadesTributariasUF(long idParcela);//
        UnidadTributaria GetUnidadTributariaById(long idUnidadTributaria, bool incluirDominios = false);
        void InsertUnidadTributaria(UnidadTributaria unidadTributaria);
        void EditUnidadTributaria(UnidadTributaria unidadTributaria);
        void DeleteUnidadTributaria(UnidadTributaria unidadTributaria);
        ICollection<ParametrosGenerales> GetRegularExpression();
        UnidadTributaria GetUnidadTributariaByIdComplete(long idUnidadTributaria, long? parcela, long? partida);
        UnidadTributaria GetUnidadTributariaByPartida(string partida);
        UnidadTributaria GetUnidadTributuriaByIdDeclaracionJurada(long idDeclaracionJurada);
        ICollection<UnidadTributaria> GetUnidadesTributariasByParcelas(long[] idsParcelas);
        bool ValidarPartidaDisponible(long idUnidadTributaria, string partida);
    }
}
