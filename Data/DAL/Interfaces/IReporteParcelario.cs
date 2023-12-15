//using GeoSit.Data.BusinessEntities.Reportes;
using GeoSit.Data.Intercambio.BusinessEntities;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IReporteParcelario
    {
        List<ReporteParcelario> GetReporteParcelario(string id_municipio);
    }
}
