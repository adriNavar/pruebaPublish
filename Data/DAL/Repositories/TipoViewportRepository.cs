using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoViewportRepository : ITipoViewportRepository
    {
        private readonly GeoSITMContext _context;

        public TipoViewportRepository(GeoSITMContext context)
        {
            _context = context;
        }
    }
}
