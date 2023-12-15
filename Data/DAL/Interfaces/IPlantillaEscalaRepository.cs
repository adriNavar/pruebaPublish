using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IPlantillaEscalaRepository
    {
        IEnumerable<PlantillaEscala> GetPlantillaTextosByPlantillaId(int idPlantilla);

        IEnumerable<PlantillaEscala> GetPlantillaEscalas();
        
        PlantillaEscala GetPlantillaEscalaById(int idPlantillaEscala);

        void InsertPlantillaEscala(PlantillaEscala plantillaEscala);

        void UpdatePlantillaEscala(PlantillaEscala plantillaEscala);
        
        void DeletePlantillaEscala(PlantillaEscala plantillaEscala);
    }
}
