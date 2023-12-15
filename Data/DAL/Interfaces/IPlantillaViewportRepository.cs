using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IPlantillaViewportRepository
    {
        List<PlantillaViewport> ObtenerAllPlantillasViewPorts();

        List<Plantilla> ObtenerAllPlantillasViewPortsInPlantilla();

        PlantillaViewport ObtenerPlantillasViewPortsById(long pId);

        //PlantillaViewport CargarPlantillaViewport(PlantillaViewport PlantiView);

        Plantilla CargarPlantillaViewport(PlantillaViewport PlantiView);

        List<Plantilla> ObtenerAllPlantillasViewPortsInPlantillaByPloteoFrecuente(int pIdPlotFrec, string idDistrito);
    }
}
