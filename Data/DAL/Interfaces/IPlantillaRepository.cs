using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IPlantillaRepository
    {
        IEnumerable<Plantilla> GetPlantillas();

        IEnumerable<Plantilla> GetPlantillasByCategorias(int[] pIdsCategorias);

        Plantilla GetPlantillaById(int idPlantilla);

        Plantilla GetFuncionAdicional(Plantilla plantilla);

        void InsertPlantilla(Plantilla plantilla);

        void InsertPlantilla(Plantilla plantilla, Hoja hoja);

        void UpdatePlantilla(Plantilla plantilla);

        void UpdatePlantillaTransparencia(int idPlantilla, int transparencia);

        void DeletePlantilla(int idPlantilla);

        void DeletePlantilla(Plantilla plantilla);

        List<ParametrosGenerales> GetParametrosGenerales();

        bool ValidarNombrePlantilla(int usuarioId, string nombrePlantilla);
    }
}
