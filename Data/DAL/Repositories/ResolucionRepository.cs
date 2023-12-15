using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    class ResolucionRepository : IResolucionRepository
    {
        private readonly GeoSITMContext _context;

        public ResolucionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public Resolucion GetResolucionById(int idResolucion)
        {
            return _context.Resoluciones.Find(idResolucion);
        }
    }
}
