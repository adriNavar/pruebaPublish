using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class VIRVbEuCoefEstadoRepository : IVIRVbEuCoefEstadoRepository
    {
        private readonly GeoSITMContext _context;

        public VIRVbEuCoefEstadoRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public List<VIRVbEuCoefEstado> GetVIRVbEuCoefEstados()
        {
            return _context.VIRVbEuCoefEstados.OrderBy(x => x.Id).ToList();
        }
    }
}
