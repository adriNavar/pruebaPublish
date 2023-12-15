using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class VIRVbEuTipoEdifRepository : IVIRVbEuTipoEdifRepository
    {
        private readonly GeoSITMContext _context;

        public VIRVbEuTipoEdifRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public List<VIRVbEuTipoEdif> GetTipoEdif(int tipo)
        {
            return _context.VIRVbEuTipoEdifs.Where(x => x.IdUso == tipo).OrderBy(x => x.IdTipo).ToList();
        }

        //public List<VIRVbEuTipoEdif> GetTipoEdifByUso(string uso)
        //{
        //    return _context.VIRVbEuTipoEdifs.Where(x => x. == tipo).OrderBy(x => x.IdTipo).ToList();
        //}
    }
}
