using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly GeoSITMContext _context;

        public RolRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Rol> GetRoles()
        {
            return _context.Roles;
        }
    }
}
