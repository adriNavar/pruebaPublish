using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    class TipoSuperficieRepository : ITipoSuperficieRepository
    {
         private readonly GeoSITMContext _context;

         public TipoSuperficieRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoSuperficie> GetTipoSuperficies()
        {
            return _context.TiposSuperficie;
        }
    }
}
