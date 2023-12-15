using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IPlantillaCategoriaRepository
    {
        IEnumerable<PlantillaCategoria> GetPlantillaCategorias();

        PlantillaCategoria GetPlantillaCategoriaById(int idPlantillaCategoria);
    }
}
