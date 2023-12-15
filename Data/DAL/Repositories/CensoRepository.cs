using System.Collections.Generic;
using System.Data;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.Linq;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class CensoRepository : ICensoRepository
    {
        private readonly GeoSITMContext _context;

        public CensoRepository(GeoSITMContext context)
        {
            _context = context;
        }
        public IEnumerable<Censo> GetCensos()
        {
            return _context.Censos.OrderByDescending(l => l.Anio).ToList();
        }

     }
}
