using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class LayerRepository : ILayerRepository
    {
        private readonly GeoSITMContext _context;

        public LayerRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Layer> GetLayersByPlantillaId(int idPlantilla)
        {
            return _context.Layers.Include(l => l.Componente).Where(l => l.IdPlantilla == idPlantilla && l.FechaBaja == null).ToList();
        }

        public IEnumerable<Layer> GetLayers()
        {
            return _context.Layers.Include(l => l.Componente).Where(l => l.FechaBaja == null).OrderBy(l => l.Nombre);
        }

        public Layer GetLayerById(int idLayer)
        {
            return _context.Layers.Find(idLayer);
        }

        public void InsertLayer(Layer layer)
        {
            layer.IdUsuarioAlta = layer.IdUsuarioModificacion;
            layer.FechaAlta = layer.FechaModificacion;

            _context.Layers.Add(layer);
        }

        public void UpdateLayer(Layer layer)
        {
            _context.Entry(layer).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(layer).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(layer).Property(x => x.FechaAlta).IsModified = false;
        }

        public void DeleteLayer(Layer layer)
        {
            layer.IdUsuarioBaja = layer.IdUsuarioModificacion;
            layer.FechaBaja = layer.FechaModificacion;
            _context.Layers.Attach(layer);
            _context.Entry(layer).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(layer).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(layer).Property(x => x.FechaAlta).IsModified = false;
        }
    }
}
