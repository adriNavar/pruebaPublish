using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GeoSITMContext _context;

        public PlanRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Plan> GetPlanes()
        {
            return _context.Planes;
        }
    }
}
