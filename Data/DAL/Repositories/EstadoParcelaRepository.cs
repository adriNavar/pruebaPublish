using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class EstadoParcelaRepository : IEstadoParcelaRepository
    {
        private readonly GeoSITMContext _context;

        public EstadoParcelaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ICollection<EstadoParcela> GetEstadosParcela()
        {
            return _context.EstadosParcela.OrderBy(ep => ep.EstadoParcelaID).Take(4).ToList();
        }
    }
}
