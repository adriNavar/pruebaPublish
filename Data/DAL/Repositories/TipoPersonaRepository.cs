using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoPersonaRepository : ITipoPersonaRepository
    {
        private readonly GeoSITMContext _context;

        public TipoPersonaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoPersona> GetTipoPersonas()
        {
            return _context.TipoPersonas.ToList();
        }
    }
}
