using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    class TipoInscripcionRepository : ITipoInscripcionRepository
    {
        private readonly GeoSITMContext _context;

        public TipoInscripcionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public TipoInscripcion GetTipoInscripcion(long id)
        {
            return _context.TipoInscripciones.Find(id);
        }

        public IEnumerable<TipoInscripcion> GetTipoInscripciones()
        {
            return _context.TipoInscripciones.ToList();
        }
    }
}
