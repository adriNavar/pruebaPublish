using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoParcelaOperacionRepository : ITipoParcelaOperacionRepository
    {
        private readonly GeoSITMContext _context;

        public TipoParcelaOperacionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ICollection<TipoParcelaOperacion> GetTipoParcelaOperaciones()
        {
            return _context.TipoParcelaOperacion.OrderBy(x => x.Id).ToList();
        }
    }
}
