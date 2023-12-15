using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IPlantillaFondoRepository
    {
        IEnumerable<PlantillaFondo> GetPlantillaFondos();
        PlantillaFondo GetPlantillaFondoById(int idPlantillaFondo);
        PlantillaFondo GetPlantillaFondoByIdPlantilla(int idPlantilla);
        void InsertPlantillaFondo(PlantillaFondo plantillaFondo, Resolucion resolucion);
        void DeletePlantillaFondo(PlantillaFondo plantillaFondo);
    }
}
