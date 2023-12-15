using System;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class PlantillaCategoriaRepository: IPlantillaCategoriaRepository
    {
        private readonly GeoSITMContext _context;

        public PlantillaCategoriaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<PlantillaCategoria> GetPlantillaCategorias()
        {
            return _context.PlantillaCategorias.OrderBy(pl => pl.Nombre ).ToList();
        }

        public PlantillaCategoria GetPlantillaCategoriaById(int idPlantillaCategoria)
        {
            return _context.PlantillaCategorias.Find(idPlantillaCategoria);
        }
    }
}
