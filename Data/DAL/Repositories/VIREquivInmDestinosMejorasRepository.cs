using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class VIREquivInmDestinosMejorasRepository: IVIREquivInmDestinosMejorasRepository
    {
        private readonly GeoSITMContext _context;

        public VIREquivInmDestinosMejorasRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public List<VIREquivInmDestinosMejoras> GetVIREquivInmDestinosMejoras()
        {
            return _context.VIREquivInmDestinosMejoras.OrderBy(x => x.Id).ToList();
        }
    }
}
