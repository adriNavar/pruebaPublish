using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class PlantillaEscalaRepository : IPlantillaEscalaRepository
    {
        private readonly GeoSITMContext _context;

        public PlantillaEscalaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<PlantillaEscala> GetPlantillaTextosByPlantillaId(int idPlantilla)
        {
            return _context.PlantillaEscalas.Where(pe => pe.IdPlantilla == idPlantilla && pe.FechaBaja == null).ToList();
        }

        public IEnumerable<PlantillaEscala> GetPlantillaEscalas()
        {
            //return _context.PlantillaEscalas.ToList();
            return _context.PlantillaEscalas.Where(pe => pe.FechaBaja == null).ToList();
        }

        public PlantillaEscala GetPlantillaEscalaById(int idPlantillaEscala)
        {
            return _context.PlantillaEscalas.Find(idPlantillaEscala);
        }

        public void InsertPlantillaEscala(PlantillaEscala plantillaEscala)
        {
            plantillaEscala.IdUsuarioAlta = plantillaEscala.IdUsuarioModificacion;
            plantillaEscala.FechaAlta = plantillaEscala.FechaModificacion;
            _context.PlantillaEscalas.Add(plantillaEscala);
        }

        public void UpdatePlantillaEscala(PlantillaEscala plantillaEscala)
        {
            _context.Entry(plantillaEscala).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(plantillaEscala).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(plantillaEscala).Property(x => x.FechaAlta).IsModified = false;
        }

        public void DeletePlantillaEscala(PlantillaEscala plantillaEscala)
        {
            plantillaEscala.IdUsuarioBaja = plantillaEscala.IdUsuarioModificacion;
            plantillaEscala.FechaBaja = plantillaEscala.FechaModificacion;

            _context.PlantillaEscalas.Attach(plantillaEscala);
            _context.Entry(plantillaEscala).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(plantillaEscala).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(plantillaEscala).Property(x => x.FechaAlta).IsModified = false;
        }
    }
}
