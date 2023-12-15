using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoUnidadTributariaRepository
    {
        List<TipoUnidadTributaria> GetTiposUnidadTributaria();
        TipoUnidadTributaria GetTipoUnidadTributaria(int id);
    }
}
