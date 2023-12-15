using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IParcelaOperacionRepository
    {
        void InsertParcelaOperacion(ParcelaOperacion parcelaOperacion);
        void EditParcelaOperacion(ParcelaOperacion parcelaOperacion);
        void DeleteParcelaOperacion(ParcelaOperacion parcelaOperacion);

        IEnumerable<ParcelaOrigen> GetParcelasOrigenOperacion(long idParcelaDestino);

        IEnumerable<ParcelaOperacion> GetParcelaOperacionesOrigen(long idParcelaOrigen);
        IEnumerable<ParcelaOperacion> GetParcelaOperacionesDestino(long idParcelaOrigen);
        Parcela GetParcelaDatos(long idParcelaOperacion);
    }
}
