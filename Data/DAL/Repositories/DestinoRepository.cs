using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class DestinoRepository : IDestinoRepository
    {
        private readonly GeoSITMContext _context;

        public DestinoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Destino> GetDestinos()
        {
            return _context.Destinos;
        }
    }
}
