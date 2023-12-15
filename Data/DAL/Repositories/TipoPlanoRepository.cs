using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoPlanoRepository : ITipoPlanoRepository
    {
        private readonly GeoSITMContext _context;
        
        public TipoPlanoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoPlano> GetTipoPlanos()
        {
            return _context.TipoPlanos.OrderBy(l => l.Nombre).ToList();
        }


    }
}
