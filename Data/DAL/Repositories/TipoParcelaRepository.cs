using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoParcelaRepository : ITipoParcelaRepository
    {
        private readonly GeoSITMContext _context;

        public TipoParcelaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ICollection<TipoParcela> GetTipoParcelas()
        {
            return _context.TiposParcela.OrderBy(tp => tp.TipoParcelaID).Skip(1).ToList();
        }

        public TipoParcela GetTipoParcela(long TipoParcelaId)
        {
            return _context.TiposParcela.FirstOrDefault(tp => tp.TipoParcelaID == TipoParcelaId);
        }
    }
}
