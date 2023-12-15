using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class PlantillaFondoRepository : IPlantillaFondoRepository
    {
        private readonly GeoSITMContext _context;

        public PlantillaFondoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<PlantillaFondo> GetPlantillaFondos()
        {
            return _context.PlantillaFondos.OrderBy(pf => pf.Plantilla).ToList();
        }

        public PlantillaFondo GetPlantillaFondoById(int idPlantillaFondo)
        {
            return _context.PlantillaFondos
                           .Include(pf => pf.Resolucion)
                           .SingleOrDefault(pf => pf.IdPlantillaFondo == idPlantillaFondo);
        }

        public PlantillaFondo GetPlantillaFondoByIdPlantilla(int idPlantilla)
        {
            return _context.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == idPlantilla && !pf.FechaBaja.HasValue);
        }

        public void InsertPlantillaFondo(PlantillaFondo plantillaFondo, Resolucion resolucion)
        {
            plantillaFondo.IdUsuarioAlta = plantillaFondo.IdUsuarioModificacion;
            plantillaFondo.FechaAlta = plantillaFondo.FechaModificacion;

            plantillaFondo.Resolucion = resolucion;
            _context.PlantillaFondos.Add(plantillaFondo);
        }

        public void DeletePlantillaFondo(PlantillaFondo plantillaFondo)
        {
            plantillaFondo.IdUsuarioBaja = plantillaFondo.IdUsuarioModificacion;
            plantillaFondo.FechaBaja = plantillaFondo.FechaModificacion;
            _context.PlantillaFondos.Attach(plantillaFondo);
            _context.Entry(plantillaFondo).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(plantillaFondo).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(plantillaFondo).Property(x => x.FechaAlta).IsModified = false;
        }
    }
}
