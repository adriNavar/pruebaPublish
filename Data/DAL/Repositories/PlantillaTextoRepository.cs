using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class PlantillaTextoRepository : IPlantillaTextoRepository
    {
        private readonly GeoSITMContext _context;

        public PlantillaTextoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<PlantillaTexto> GetPlantillaTextosByPlantillaId(int idPlantilla)
        {
            return _context.PlantillaTextos.Where(pt => pt.IdPlantilla == idPlantilla && pt.FechaBaja == null).OrderBy(pt => pt.FuenteNombre).ToList();
        }

        public IEnumerable<PlantillaTexto> GetPlantillaTextos()
        {
            return _context.PlantillaTextos.Where(pt => pt.FechaBaja == null).OrderBy(pts => pts.FuenteNombre).ToList();
        }

        public PlantillaTexto GetPlantillaTextoById(int idPlantillaTexto)
        {
            return _context.PlantillaTextos.Find(idPlantillaTexto);
        }

        public void InsertPlantillaTexto(PlantillaTexto plantillaTexto)
        {
            _context.PlantillaTextos.Add(plantillaTexto);
        }

        public void InsertPlantillaTexto(PlantillaTexto plantillaTexto, Plantilla plantilla)
        {
            plantillaTexto.IdUsuarioAlta = plantillaTexto.IdUsuarioModificacion;
            plantillaTexto.FechaAlta = plantillaTexto.FechaModificacion;
            plantillaTexto.Plantilla = plantilla;
            _context.PlantillaTextos.Add(plantillaTexto);
        }

        public void UpdatePlantillaTexto(PlantillaTexto plantillaTexto)
        {
            _context.Entry(plantillaTexto).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(plantillaTexto).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(plantillaTexto).Property(x => x.FechaAlta).IsModified = false;
        }

        public void DeletePlantillaTexto(PlantillaTexto plantillaTexto)
        {
            plantillaTexto.IdUsuarioBaja = plantillaTexto.IdUsuarioModificacion;
            plantillaTexto.FechaBaja = plantillaTexto.FechaModificacion;
            _context.PlantillaTextos.Attach(plantillaTexto);
            _context.Entry(plantillaTexto).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(plantillaTexto).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(plantillaTexto).Property(x => x.FechaAlta).IsModified = false;
        }
    }
}
