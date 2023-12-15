using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class ClaseParcelaRepository : IClaseParcelaRepository
    {
        private readonly GeoSITMContext _context;

        public ClaseParcelaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ICollection<ClaseParcela> GetClasesParcelas()
        {
            return _context.ClasesParcela.ToList();
        }
    }
}
