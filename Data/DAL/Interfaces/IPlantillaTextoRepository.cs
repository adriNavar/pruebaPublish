using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IPlantillaTextoRepository
    {
        IEnumerable<PlantillaTexto> GetPlantillaTextosByPlantillaId(int idPlantilla);

        IEnumerable<PlantillaTexto> GetPlantillaTextos();

        PlantillaTexto GetPlantillaTextoById(int idPlantillaTexto);

        void InsertPlantillaTexto(PlantillaTexto plantillaTexto);

        void UpdatePlantillaTexto(PlantillaTexto plantillaTexto);

        void DeletePlantillaTexto(PlantillaTexto plantillaTexto);
    }
}
