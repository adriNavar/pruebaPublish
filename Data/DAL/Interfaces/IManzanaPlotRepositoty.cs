using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IManzanaPlotRepository
    {
        ManzanaPlot[] GetManzanaPlotByIdManzana(string esquema, string tabla, string campoGeometry, string campoIdManzana, long idManzana);
        ManzanaPlot[] GetManzanaPlotByCoords(string esquema, string tabla, string campoGeometry, string campoIdManzana, double x, double y);
        bool GetExisteEspacioVerde(string esquema, string tabla, string campoGeometry, string campoIdManzana, long idManzana, int idOCTipo);
    }
}
