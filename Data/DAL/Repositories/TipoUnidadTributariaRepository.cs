using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoUnidadTributariaRepository : ITipoUnidadTributariaRepository
    {
        private readonly GeoSITMContext _context;

        public TipoUnidadTributariaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public List<TipoUnidadTributaria> GetTiposUnidadTributaria()
        {
            return _context.TiposUnidadTributaria.OrderBy(tp => tp.TipoUnidadTributariaID).ToList();
        }

        public TipoUnidadTributaria GetTipoUnidadTributaria(int id)
        {
            return _context.TiposUnidadTributaria.Find(id);
        }
    }
}
