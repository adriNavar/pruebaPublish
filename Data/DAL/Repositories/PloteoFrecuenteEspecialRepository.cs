using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class PloteoFrecuenteEspecialRepository : IPloteoFrecuenteEspecialRepository
    {
        private readonly GeoSITMContext _context;

        public PloteoFrecuenteEspecialRepository(GeoSITMContext context)
        {
            _context = context;
        }


    }
}
