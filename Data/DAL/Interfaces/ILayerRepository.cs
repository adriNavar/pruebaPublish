using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface ILayerRepository
    {
        IEnumerable<Layer> GetLayersByPlantillaId(int idPlantilla);
             
        IEnumerable<Layer> GetLayers();

        Layer GetLayerById(int idLayer);

        void InsertLayer(Layer layer);

        void UpdateLayer(Layer layer);
        
        void DeleteLayer(Layer layer);
    }
}
