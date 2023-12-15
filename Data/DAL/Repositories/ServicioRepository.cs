using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class ServicioRepository : IServicioRepository
    {
        private readonly GeoSITMContext _context;

        public ServicioRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Servicio> GetServicios()
        {
            return _context.Servicios;
        }
    }
}
