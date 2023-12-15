using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class VIRVbEuUsoRepository : IVIRVbEuUsoRepository
    {
        private readonly GeoSITMContext _context;

        public VIRVbEuUsoRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public List<VIRVbEuUsos> GetVIRVbEuUsos()
        {
            return _context.VIRVbEuUsos.Where(x => x.FechaBaja == null).OrderBy(x => x.Uso).ToList();
        }

        public VIRVbEuUsos GetVIRVbEuUsoByUso(string uso)
        {
            return _context.VIRVbEuUsos.Where(x => x.Uso == uso && x.FechaBaja == null).OrderBy(x => x.Uso).FirstOrDefault();
        }
    }
}
