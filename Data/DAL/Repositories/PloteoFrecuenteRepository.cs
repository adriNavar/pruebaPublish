using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class PloteoFrecuenteRepository : IPloteoFrecuenteRepository
    {
        private readonly GeoSITMContext _context;

        public PloteoFrecuenteRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public AreaPloteoResponse ObtenerAreaPloteoByIdPloteo(long pIdPloteo)
        {
            return new AreaPloteoResponse();
        }

    }
}
