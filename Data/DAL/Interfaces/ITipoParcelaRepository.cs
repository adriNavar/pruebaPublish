using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoParcelaRepository
    {
        ICollection<TipoParcela> GetTipoParcelas();
        TipoParcela GetTipoParcela(long TipoParcelaId);
    }
}
