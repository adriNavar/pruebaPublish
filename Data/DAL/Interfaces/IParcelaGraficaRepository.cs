using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IParcelaGraficaRepository
    {
        ParcelaGrafica GetParcelaGraficaByIdParcela(long idParcela);        
        void DeleteParcelaGrafica(ParcelaGrafica parcelaGrafica);
        void InsertParcelaGrafica(ParcelaGrafica parcelaGrafica);
        void UpdateParcelaGrafica(ParcelaGrafica parcelaGrafica);
        void UpdateGeometry(long featId, int gtype, int srid, double[] point, int[] elemInfo, double[] ordinates);
    }
}
