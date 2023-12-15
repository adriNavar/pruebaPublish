using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class ImagenSatelitalRepository : IImagenSatelitalRepository
    {
        private readonly GeoSITMContext _context;

        public ImagenSatelitalRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<ImagenSatelital> GetAllImagenSatelital()
        {
            return _context.ImagenesSatelitales.OrderBy(isa => isa.Nombre).ToList();
        }

        public ImagenSatelital GetImagenSatelitalById(int idImagenSatelital)
        {
            return _context.ImagenesSatelitales.Find(idImagenSatelital);
        }

    }
}
